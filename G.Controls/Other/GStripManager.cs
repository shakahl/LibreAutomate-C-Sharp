using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	/// <summary>
	/// <see cref="GStripManager"/> uses this interface to get properties of menu/toolbar items that it cannot get from XML (Click event handlers, images etc).
	/// Also used to notify about some events.
	/// </summary>
	public interface IGStripManagerCallbacks
	{
		/// <summary>
		/// Gets menu/toolbar item image.
		/// </summary>
		/// <param name="imageName">XML i attribute.</param>
		Image GetImage(string imageName);

		/// <summary>
		/// Gets menu/toolbar item Click event handler.
		/// Returns null if the item is not a command, for example if it is a submenu-item or control-item.
		/// Called before creating the ToolStripItem.
		/// </summary>
		/// <param name="itemName">Menu/toolbar item name (XML tag).</param>
		EventHandler GetClickHandler(string itemName);

		/// <summary>
		/// Called before adding an item to a toolbar or menu/submenu.
		/// </summary>
		/// <param name="item">Can be ToolStripMenuItem, ToolStripButton, ToolStripSplitButton, ToolStripDropDownButton, ToolStripSpringTextBox, ToolStripSpringComboBox, ToolStripSeparator.</param>
		/// <param name="owner">ToolStrip, MenuStrip or ToolStripDropDownMenu to which will be added this item.</param>
		void ItemAdding(ToolStripItem item, ToolStrip owner);
	}

	//[DebuggerStepThrough]
	public partial class GStripManager
	{
		Form _form;
		string _xmlFileDefault, _xmlFileCustomized;
		IGStripManagerCallbacks _callbacks;
		XElement _xDoc; //root element
		CatToolStrip _tsCustom1, _tsCustom2;

		/// <summary>
		/// Menu bar.
		/// </summary>
		public MenuStrip MenuBar { get; private set; }

		/// <summary>
		/// Toolbars.
		/// </summary>
		public Dictionary<string, CatToolStrip> Toolbars { get; }

		/// <summary>
		/// Submenus.
		/// </summary>
		public Dictionary<string, ToolStripDropDownMenu> Submenus { get; }

		/// <param name="form">Form used as owner of dialog boxes.</param>
		/// <param name="xmlFileDefault">File containing default XML.</param>
		/// <param name="xmlFileCustomized">File containing customized XML. Don't have to exist; it will be created when saving customizations.</param>
		/// <param name="callbacks"></param>
		public GStripManager(Form form, string xmlFileDefault, string xmlFileCustomized, IGStripManagerCallbacks callbacks)
		{
			_form = form;
			_xmlFileDefault = xmlFileDefault;
			_xmlFileCustomized = xmlFileCustomized;
			_callbacks = callbacks;
			Toolbars = new Dictionary<string, CatToolStrip>();
			Submenus = new Dictionary<string, ToolStripDropDownMenu>();
		}

		/// <summary>
		/// Opens XML file and creates toolbars/menus/submenus.
		/// </summary>
		public void BuildAll()
		{
			Debug.Assert(_xDoc == null); if(_xDoc != null) throw new InvalidOperationException();
			_LoadXml();

			//TODO: version control

			var tsRenderer = new _ToolStripRenderer(); //remove rounded corners
			var isize = Catkeys.Util.Dpi.SmallIconSize; //if high DPI, auto scale images

			//create top-level toolstrips (menu bar and toolbars), and call _AddChildItems to add items and create drop-down menus and submenus
			bool isMenu = true;
			foreach(var x in _xDoc.Elements()) {
				string name = x.Name.LocalName;
				ToolStrip t;
				if(isMenu) {
					t = MenuBar = new MenuStrip();
				} else {
					var cts = new CatToolStrip();
					Toolbars.Add(name, cts);
					t = cts;
					switch(name) {
					case "Custom1": _tsCustom1 = cts; break;
					case "Custom2": _tsCustom2 = cts; break;
					}
				}

				t.SuspendLayout();

				t.Tag = x;
				t.Text = t.Name = name;
				//t.AllowItemReorder = true; //don't use, it's buggy etc
				t.Renderer = tsRenderer;
				if(isMenu) {
					t.Padding = new Padding(); //remove menu bar paddings
				} else {
					if(isize.Height != 16) t.ImageScalingSize = isize;
				}
				//GDockPanel will set other styles

				_AddChildItems(x, t, isMenu);

				t.ResumeLayout(false);
				isMenu = false;
			}
		}

		/// <summary>
		/// Adds toolbar buttons or menu bar items.
		/// Call this for each top-level toolstrip (toolbar or menu bar).
		/// </summary>
		/// <param name="xParent">XML element of the toolbar or menu bar.</param>
		/// <param name="owner">The toolbar or menu bar.</param>
		/// <param name="isMenu">true if menu, false if toolbar.</param>
		void _AddChildItems(XElement xParent, ToolStrip owner, bool isMenu)
		{
			foreach(XElement x in xParent.Elements()) {
				var item = _CreateChildItem(x, isMenu);
				if(item != null) _AddChildItem(item, owner);
			}
		}

		/// <summary>
		/// Adds item to owner.
		/// Before it calls <see cref="IGStripManagerCallbacks.ItemAdding"/>.
		/// </summary>
		/// <param name="item">Can be ToolStripMenuItem, ToolStripButton, ToolStripSplitButton, ToolStripDropDownButton, ToolStripSpringTextBox, ToolStripSpringComboBox, ToolStripSeparator.</param>
		/// <param name="owner">ToolStrip, MenuStrip or ToolStripDropDownMenu to which to add this item.</param>
		/// <param name="insertAt">If not negative, inserts at this index. If negative, adds to the end.</param>
		void _AddChildItem(ToolStripItem item, ToolStrip owner, int insertAt = -1)
		{
			if(owner != MenuBar) {
				item.MouseUp += _OnMouseUp; //context menu
				if(!(owner is ToolStripDropDownMenu)) item.MouseDown += _OnMouseDown; //Alt+drag
			}

			_callbacks.ItemAdding(item, owner);

			var k = owner.Items;
			if(insertAt >= 0) k.Insert(insertAt, item); else k.Add(item);
		}

		/// <summary>
		/// Creates a menu or toolbar item.
		/// </summary>
		/// <param name="x">XML element containing item properties.</param>
		/// <param name="isMenu">false if toolbar item, true if menu item (also if menu bar item).</param>
		ToolStripItem _CreateChildItem(XElement x, bool isMenu)
		{
			string s, tag = x.Name.LocalName;

			if(tag == "sep") return new ToolStripSeparator() { Tag = x };

			ToolStripItem item = null;
			bool isControl = false, needHandler = false;
			if(isMenu) {
				var mi = new ToolStripMenuItem();
				item = mi;
				if(x.HasElements || x.HasAttribute_("dd")) {
					_Submenu(x, mi, tag);
				} else {
					needHandler = true;
					if(x.HasAttribute_("check")) mi.Checked = true;
				}
			} else if(x.HasAttribute_("type")) {
				isControl = true;
				string cue = x.Attribute_("cue");
				s = x.Attribute_("type");
				switch(s) {
				case "edit":
					var ed = new ToolStripSpringTextBox();
					if(cue != null) ed.SetCueBanner(cue);
					item = ed;
					break;
				case "combo":
					var combo = new ToolStripSpringComboBox();
					if(cue != null) combo.SetCueBanner(cue);
					item = combo;
					break;
				default:
					Debug.Assert(false);
					return null;
				}
				if(cue != null) item.AccessibleName = cue;
			} else if(x.HasElements || x.HasAttribute_("dd")) {
				ToolStripDropDownItem ddi;
				var handler = _callbacks.GetClickHandler(tag);
				if(handler != null) {
					var sb = new ToolStripSplitButton();
					sb.ButtonClick += handler;
					ddi = sb;
				} else {
					ddi = new ToolStripDropDownButton();
				}
				item = ddi;
				_Submenu(x, ddi, tag);
			} else {
				needHandler = true;
				var b = new ToolStripButton();
				if(x.HasAttribute_("check")) b.Checked = true;
				item = b;
			}

			string text = _GetDefaultItemText(x);

			var tt = x.Attribute_("tt");
			item.ToolTipText = tt ?? (isMenu ? null : text);

			if(!isControl) {
				_SetItemProperties(isMenu, x, item, text);
			}

			if(!isMenu) {
				if(x.HasAttribute_("hide")) item.Overflow = ToolStripItemOverflow.Always;
			}

			//TODO: hotkey.

			if(needHandler) {
				var handler = _callbacks.GetClickHandler(tag);
				if(handler != null) item.Click += handler;
				else DebugPrint("no handler of " + tag);
			}

			item.Name = tag;
			item.Tag = x;

			return item;
		}

		void _Submenu(XElement x, ToolStripDropDownItem ddItem, string tag)
		{
			var s = x.Attribute_("dd");
			if(!Empty(s)) {
				ddItem.DropDown = Submenus[s];
			} else {
				var dd = ddItem.DropDown as ToolStripDropDownMenu; //note: not ddItem.DropDown=new ToolStripDropDownMenu(). Then eg hotkeys don't work.
				dd.Name = tag;
				dd.Tag = x;
				Submenus.Add(tag, dd);

				if(s != null) { //attribute dd="". Will be filled later, eg on Opening event.
					dd.Items.Add(new ToolStripSeparator()); //else no drop arrow and no Opening event
					return;
				}
#if !LAZY_MENUS
				dd.SuspendLayout(); //with this don't need lazy menus
				_AddChildItems(x, dd, true);
				dd.ResumeLayout(false);
#else
					//Fill menu items later. This saves ~50 ms of startup time if not using dd.SuspendLayout. With dd.SuspendLayout - just 5 ms.
					//Can do it with Opening event or with timer. With timer easier. With event users cannot use MSAA etc to automate clicking menu items (with timer cannot use it only the first 1-2 seconds).
#if true
					Time.SetTimer(500, true, t =>
					{
						dd.SuspendLayout();
						_AddChildItems(x, dd, true);
						dd.ResumeLayout(false);
					});
#else
					dd.Items.Add(new ToolStripSeparator());
					CancelEventHandler eh = null;
					eh = (sender, e) =>
					{
						dd.Opening -= eh;
						dd.Items.Clear();
						_AddChildItems(x, dd, true);
					};
					dd.Opening += eh;
#endif
#endif
			}
		}

		void _SetItemProperties(bool isMenu, XElement x, ToolStripItem item, string defaultText = null, bool reset = false)
		{
			string s = x.Attribute_("icon"); //custom image as icon file
			if(s != null) {
				item.Image = Icons.GetFileIconImage(s, (int)Icons.ShellSize.SysSmall);
			} else {
				s = x.Attribute_("i"); //image from resources
				if(s != null) item.Image = _callbacks.GetImage(s);
				else if(reset) item.Image = null;
			}

			s = x.Attribute_("color");
			if(!Empty(s)) {
				var color = s[0] == '0' ? Calc.ColorFromRGB((uint)s.ToInt32_()) : Color.FromName(s);
				if(color.A != 0) item.ForeColor = color;
			} else if(reset) item.ForeColor = Color.FromKnownColor(KnownColor.ControlText);

			s = x.Attribute_("text");
			item.Text = s ?? defaultText ?? _GetDefaultItemText(x);

			if(!isMenu) {
				var style = (ToolStripItemDisplayStyle)x.Attribute_("style", 0);
				if(item.Image == null) style = ToolStripItemDisplayStyle.ImageAndText;
				else if(style == 0) style = ToolStripItemDisplayStyle.Image; //ToolStripItemDisplayStyle.None
				item.DisplayStyle = style;
			}
		}

		string _GetDefaultItemText(XElement x)
		{
			string tag = x.Name.LocalName;
			string s = x.Attribute_("t");
			if(s == null) {
				s = tag.Remove(0, tag.LastIndexOf('_') + 1); //eg "Edit_Copy" -> "Copy"
				s = s.RegexReplace_("(?<=[^A-Z])(?=[A-Z])", " "); //"OneTwoThree" -> "One Two Three". //speed: don't need to optimize.
			}
			return s;
		}

		void _LoadXml()
		{
			var s = _xmlFileCustomized;
			bool useCustom = false;
			if(Files.FileExists(s)) useCustom = true; else s = _xmlFileDefault;
			try { _xDoc = XElement.Load(s); }
			catch(Exception e) when(useCustom) {
				TaskDialog.ShowWarning("Failed to load XML file", s, TDFlags.Wider, null, e.Message);
				_xDoc = XElement.Load(_xmlFileDefault);
			}
		}

		void _SaveXml()
		{
			try { _xDoc.Save(_xmlFileCustomized); }
			catch(Exception e) {
				TaskDialog.ShowWarning("Failed to save XML file", _xmlFileCustomized, TDFlags.Wider, null, e.Message);
			}
		}

		/// <summary>
		/// Toolbar button MouseUp handler. Implements context menu that allows to customize.
		/// </summary>
		private void _OnMouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Right) return;
			var item = sender as ToolStripItem;
			var ts = item.Owner;
			bool isMenu = ts is ToolStripDropDownMenu;
			bool isCustom = ts == _tsCustom1 || ts == _tsCustom2;
			bool isSeparator = item is ToolStripSeparator;
			bool isHidden = item.Overflow == ToolStripItemOverflow.Always;
			var x = item.Tag as XElement;
			//PrintList(item, ts);

			var m = new CatMenu();
			m["Properties..."] = o =>
			{
				using(var f = new GSMProp()) {
					if(isMenu) f.groupStyle.Enabled = false;
					switch(x.Attribute_("style", 0)) {
					case 0: f.radioDefault.Checked = true; break;
					case 1: f.radioOnlyText.Checked = true; break;
					case 2: f.radioOnlyIcon.Checked = true; break;
					case 3: f.radioIconAndText.Checked = true; break;
					}
					f.textText.Text = x.Attribute_("text");
					f.comboColor.Text = x.Attribute_("color");
					f.textIcon.Text = x.Attribute_("icon");

					if(f.ShowDialog(_form) == DialogResult.OK) _Strips_Customize(6, item, null, f);
				}
			};
			if(!isMenu) {
				if(!isSeparator) { m["Hide"] = o => _Strips_Customize(5, item); if(isHidden) m.LastMenuItem.Checked = true; }
				if(isCustom || isSeparator) m["Remove"] = o => _Strips_Customize(3, item);
				if(!isHidden) m["Add separator"] = o => _Strips_Customize(4, item, ts);
			}
			if(isCustom) {
				if(_tsCustom1 != null && _tsCustom2 != null) {
					if(ts == _tsCustom1) m["Move to toolbar Custom2"] = o => _Strips_Customize(2, item, _tsCustom2);
					else m["Move to toolbar Custom1"] = o => _Strips_Customize(2, item, _tsCustom1);
				}
			} else {
				if(_tsCustom1 != null) m["Copy to toolbar Custom1"] = o => _Strips_Customize(1, item, _tsCustom1);
				if(_tsCustom2 != null) m["Copy to toolbar Custom2"] = o => _Strips_Customize(1, item, _tsCustom2);
			}
			if(!isMenu) {
				m.Separator();
				m["How to customize..."] = o => TaskDialog.ShowInfo("How to customize toolbars",
					"There are several standard toolbars and two custom toolbars (initially empty). Standard toolbar buttons cannot be added and removed, but can be hidden and reordered. Menu items cannot be added/removed/etc, but can be changed some properties." +
					"\n\nAdd button to a custom toolbar:  right-click it in a standard toolbar or menu, and select 'Copy to'." +
					"\nRemove button from a custom toolbar:  right-click and select 'Remove'." +
					"\nReorder button (move in the same toolbar):  Alt+drag." +
					"\nHide button:  right-click and select 'Hide'. Or move (Alt+drag) it to the end and resize the toolbar. Hidden buttons can be used like combo box items." +
					"\n\nToolbars work like panels. You can hide, make floating, resize, drag, Alt+drag to dock anywhere in the window, etc. To show the context menu, right-click the small rectangle that changes color when the mouse pointer is there. Use splitters to resize. Right click a splitter to change its thickness."
					, TDFlags.Wider);
				string folder = Folders.ThisAppData, link = $"<a href=\"{folder}\">{folder}</a>";
				m["How to backup, restore, reset..."] = o =>
				{
					TaskDialog.ShowEx("How to backup, restore or reset customizations",
					"All customizations are saved in XML files in folder\n" +
					link +
					"\n\nToolbar buttons in CmdStrips.xml." +
					"\nPanel/toolbar layout in Panels.xml." +
					"\n\nTo backup:  copy the file." +
					"\nTo restore:  exit this application and replace the file with the backup file." +
					"\nTo reset:  exit this application and delete the file."
					, icon: TDIcon.Info, flags: TDFlags.Wider, onLinkClick: h => { Process.Start(h.LinkHref); });
				};
			}
			m.Show(ts);
		}

		/// <summary>
		/// Implements toolbar button customization.
		/// </summary>
		/// <param name="action">1 copy, 2 move, 3 remove, 4 separator, 5 hide/unhide, 6 properties.</param>
		/// <param name="item">Button.</param>
		/// <param name="tsTo">Destination toolbar, or null if don't need.</param>
		/// <param name="etc">
		/// With action 2 (move) can be the target button as ToolStripItem; if null, moves to the end.
		/// With action 6 (properties) - GSMProp.
		/// </param>
		void _Strips_Customize(uint action, ToolStripItem item, ToolStrip tsTo = null, object etc = null)
		{
			string s;
			XElement x = null, xtbTo = null;
			if(item != null) x = item.Tag as XElement;
			if(tsTo != null) xtbTo = tsTo.Tag as XElement;

			switch(Calc.LoUshort(action)) {
			case 1: //copy
				var xNew = new XElement(x.Name, x.Attributes()); //copy without descendants but with attributes
				xtbTo.Add(xNew);
				if(item is ToolStripDropDownItem ddi && ddi.HasDropDown) {
					var dd = ddi.DropDown as ToolStripDropDownMenu;
					xNew.SetAttributeValue("dd", dd.Name);
				}
				_AddChildItem(_CreateChildItem(xNew, false), tsTo);
				break;
			case 2: //move
				if(etc != null) {
					var itemTo = etc as ToolStripItem;
					int i1 = tsTo.Items.IndexOf(item), i2 = tsTo.Items.IndexOf(itemTo);
					if(i2 == i1) return;
					var xiTo = itemTo.Tag as XElement;
					x.Remove(); if(i2 < i1) xiTo.AddBeforeSelf(x); else xiTo.AddAfterSelf(x);
					tsTo.Items.Insert(i2, item); //moves
				} else {
					x.Remove(); xtbTo.Add(x);
					tsTo.Items.Add(item); //moves from old parent toolstrip
				}
				break;
			case 3: //remove
				x.Remove();
				item.Owner.Items.Remove(item);
				break;
			case 4: //separator
				var xSep = new XElement("sep");
				var sep = new ToolStripSeparator() { Tag = xSep };
				x.AddBeforeSelf(xSep);
				_AddChildItem(sep, tsTo, tsTo.Items.IndexOf(item));
				break;
			case 5: //hide/unhide
				bool hide = !(item.Overflow == ToolStripItemOverflow.Always);
				x.SetAttributeValue("hide", hide ? "" : null);
				item.Overflow = hide ? ToolStripItemOverflow.Always : ToolStripItemOverflow.AsNeeded;
				break;
			case 6: //properties
				var f = etc as GSMProp;
				bool isMenu = item.Owner is ToolStripDropDownMenu;

				if(!isMenu) {
					if(f.radioOnlyText.Checked) s = "1";
					else if(f.radioOnlyIcon.Checked) s = "2";
					else if(f.radioIconAndText.Checked) s = "3";
					else s = null;
					x.SetAttributeValue("style", s);
				}

				s = f.textText.Text; if(s == "") s = null;
				x.SetAttributeValue("text", s);

				s = f.comboColor.Text; if(s == "") s = null;
				x.SetAttributeValue("color", s);

				s = f.textIcon.Text; if(s == "") s = null;
				x.SetAttributeValue("icon", s);

				_SetItemProperties(isMenu, x, item, null, true);
				break;
			}

			//Print(x);
			_SaveXml();
		}

		/// <summary>
		/// Toolbar button MouseDown handler. Implements toolbar button reordering with Alt+drag.
		/// </summary>
		private void _OnMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left || Control.ModifierKeys != Keys.Alt) return;
			var item = sender as ToolStripItem;
			ToolStripItem target = null; bool isOutside = false;
			var ts = item.Owner;
			bool isCustom = ts == _tsCustom1 || ts == _tsCustom2;
			//PrintList(item, ts, isCustom);
			if(!Catkeys.Util.DragDrop.SimpleDragDrop(ts, MouseButtons.Left, k =>
			{
				if(k.Msg.message != Api.WM_MOUSEMOVE) return;
				target = ts.GetItemAt(ts.MouseClientXY_());
				//Print(target);
				isOutside = (target == null && Wnd.FromMouse() != (Wnd)ts);
				k.Cursor = isOutside ? Cursors.No : Cursors.Hand;
			}) || isOutside) return;
			_Strips_Customize(2, item, ts, target);
		}

		class _ToolStripRenderer :ToolStripProfessionalRenderer
		{
			internal _ToolStripRenderer()
			{
				this.RoundedEdges = false;
			}
		}
	}
}
