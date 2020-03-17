using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;

namespace Au.Controls
{
	/// <summary>
	/// <see cref="AuStripManager"/> uses this interface to get properties of menu/toolbar items that it cannot get from XML (Click event handlers, images etc).
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
		/// <param name="item">Can be ToolStripMenuItem, ToolStripButton, ToolStripSplitButton, ToolStripDropDownButton, ToolStripSpringTextBox, ToolStripSpringComboBox, ToolStripSeparator. Its Tag property is XElement.</param>
		/// <param name="owner">ToolStrip, MenuStrip or ToolStripDropDownMenu to which will be added this item. Its Tag property is XElement.</param>
		void ItemAdding(ToolStripItem item, ToolStrip owner);
	}

	public partial class AuStripManager
	{
		string _xmlFileDefault, _xmlFileCustom;
		XElement _xStrips; //XML root element, contains menubar and toolbars
		Util.AToolStrip _tsCustom1, _tsCustom2; //custom toolbars
		Form _form;
		IGStripManagerCallbacks _callbacks;
		bool _inBuildAll; //true while we are in BuildAll()

		/// <summary>
		/// Menu bar.
		/// </summary>
		public MenuStrip MenuBar { get; private set; }

		/// <summary>
		/// Toolbars.
		/// </summary>
		public Dictionary<string, Util.AToolStrip> Toolbars { get; }

		/// <summary>
		/// Submenus.
		/// </summary>
		public Dictionary<string, ToolStripDropDownMenu> Submenus { get; }


		public XElement Xml => _xStrips;

		/// <param name="form">Form used as owner of dialog boxes.</param>
		/// <param name="callbacks"></param>
		public AuStripManager(Form form, IGStripManagerCallbacks callbacks)
		{
			_form = form;
			_callbacks = callbacks;
			Toolbars = new Dictionary<string, Util.AToolStrip>();
			Submenus = new Dictionary<string, ToolStripDropDownMenu>();
		}

		/// <summary>
		/// Opens XML file and creates toolbars/menus/submenus from XML tags.
		/// </summary>
		/// <param name="xmlFile">XML file containing menus/toolbars without user customizations.</param>
		/// <param name="xmlFileCustom">XML file containing user customizations. It will be created or updated when saving customizations.</param>
		/// <param name="tsRenderer"></param>
		public void BuildAll(string xmlFile, string xmlFileCustom, ToolStripRenderer tsRenderer = null)
		{
			Debug.Assert(_xStrips == null); if(_xStrips != null) throw new InvalidOperationException();

			_xmlFileDefault = xmlFile;
			_xmlFileCustom = xmlFileCustom;
			try { _xStrips = AExtXml.LoadElem(xmlFile); }
			catch(Exception ex) { ADialog.ShowError("Failed to load file", ex.ToString()); throw; }
			XElement xCustom = null;
			if(AFile.ExistsAsFile(_xmlFileCustom)) {
				try { xCustom = AExtXml.LoadElem(_xmlFileCustom); }
				catch(Exception e) { AOutput.Write("Failed to load file", _xmlFileCustom, e.Message); }
			}

			Size imageScalingSize = Au.Util.ADpi.SmallIconSize; //if high DPI, auto scale images

			//create top-level toolstrips (menu bar and toolbars), and call _AddChildItems to add items and create drop-down menus and submenus
			_inBuildAll = true;
			bool isMenu = true;
			foreach(var xe in _xStrips.Elements().ToArray()) { //info: ToArray() because _MergeCustom replaces XML elements of custom toolbars
				string name = xe.Name.LocalName;
				var x = xe;
				if(xCustom != null) _MergeCustom(xCustom, ref x, name, isMenu);

				ToolStrip t;
				if(isMenu) {
					t = MenuBar = new Util.AMenuStrip();
				} else {
					var cts = new Util.AToolStrip();
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
				//t.AllowItemReorder = true; //don't use, it's has bugs etc, we know better how to do it
				if(tsRenderer != null) t.Renderer = tsRenderer;
				if(isMenu) {
					t.Padding = new Padding(); //remove menu bar paddings
				} else {
				}
				if(imageScalingSize.Height != 16) t.ImageScalingSize = imageScalingSize; //info: all submenus will inherit it from menubar

				_AddChildItems(x, t, isMenu);

				t.ResumeLayout(false);
				isMenu = false;
			}
			_inBuildAll = false;
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
				if(item != null) _AddChildItem(x, item, owner);
			}
		}

		/// <summary>
		/// Adds item to owner.
		/// Before it calls <see cref="IGStripManagerCallbacks.ItemAdding"/>.
		/// </summary>
		/// <param name="x">item's element. This function assigns item to its annotation. Caller must assign x to item.Tag before calling this function.</param>
		/// <param name="item">Can be ToolStripMenuItem, ToolStripButton, ToolStripSplitButton, ToolStripDropDownButton, ToolStripSpringTextBox, ToolStripSpringComboBox, ToolStripSeparator.</param>
		/// <param name="owner">ToolStrip, MenuStrip or ToolStripDropDownMenu to which to add this item.</param>
		/// <param name="insertAt">If not negative, inserts at this index. If negative, adds to the end.</param>
		void _AddChildItem(XElement x, ToolStripItem item, ToolStrip owner, int insertAt = -1)
		{
			if(owner != MenuBar) {
				item.MouseUp += _OnMouseUp; //context menu
				if(!(owner is ToolStripDropDownMenu)) item.MouseDown += _OnMouseDown; //Alt+drag
			}

			Debug.Assert(x != null && object.ReferenceEquals(x, item.Tag));
			x.AddAnnotation(item); //info: this method is so smartly optimized, it just assigns item to its member 'object annotation;'. Creates array only if assigned more items.

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

			ToolStripItem item = null; ToolStripMenuItem mi = null;
			bool isControl = false, needHandler = false;
			if(isMenu) {
				mi = new ToolStripMenuItem();
				item = mi;
				if(x.HasElements || x.HasAttr("dd")) {
					_Submenu(x, mi, tag);
				} else {
					needHandler = true;
				}
			} else if(x.HasAttr("type")) {
				isControl = true;
				string cue = x.Attr("cue");
				s = x.Attr("type");
				switch(s) {
				case "edit":
					var ed = new ToolStripSpringTextBox();
					if(cue != null) ed.ZSetCueBanner(cue);
					item = ed;
					break;
				case "combo":
					var combo = new ToolStripSpringComboBox();
					if(cue != null) combo.ZSetCueBanner(cue);
					item = combo;
					break;
				default:
					Debug.Assert(false);
					return null;
				}
				if(cue != null) item.AccessibleName = cue;
			} else if(x.HasElements || x.HasAttr("dd")) {
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
				item = b;
			}

			item.Name = tag;
			item.Tag = x;

			_SetItemProperties(x, item, isMenu, isControl);

			if(needHandler) {
				var handler = _callbacks.GetClickHandler(tag);
				if(handler != null) item.Click += handler;
				else {
					ADebug.Print("no handler of " + tag);
					//return null;
					//item.Enabled = false;
					item.Visible = false;
				}
			}

			return item;
		}

		void _Submenu(XElement x, ToolStripDropDownItem ddItem, string tag)
		{
			var s = x.Attr("dd");
			if(!s.NE()) {
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
					ATimer.After(500, t =>
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

		void _SetItemProperties(XElement x, ToolStripItem item, bool isMenu, bool isControl)
		{
			string s, defaultText = null;

			var tt = x.Attr("tt"); //tooltip
			item.ToolTipText = tt ?? (isMenu ? null : (defaultText = _GetDefaultItemText(x)));

			if(!isMenu) {
				if(x.HasAttr("hide")) item.Overflow = ToolStripItemOverflow.Always;
				else if(!_inBuildAll) item.Overflow = ToolStripItemOverflow.AsNeeded;
			}
			if(isControl) return;

			Image im = null;
			if(x.Attr(out s, "i2")) { //custom image as icon file
				im = AIcon.GetFileIconImage(s, (int)IconSize.SysSmall, GIFlags.SearchPath);
				if(im == null) AOutput.Write($"Failed to get {(isMenu ? "menu item" : "toolbar button")} {x.Name} icon from file {s}\n\tTo fix this, right-click it and select Properties...");
				//SHOULDDO: async or cache
			}
			if(im == null && x.Attr(out s, "i")) im = _callbacks.GetImage(s); //image from resources
			item.Image = im;

			if(x.Attr(out s, "color") && ColorInt.FromString(s, out var color)) item.ForeColor = (Color)color;
			else if(!_inBuildAll) item.ForeColor = Color.FromKnownColor(KnownColor.ControlText);

			bool hasCustomText = x.Attr(out s, "t2"); //custom text
			item.Text = s ?? defaultText ?? _GetDefaultItemText(x);

			if(isMenu) {
				var mi = item as ToolStripMenuItem;
				if(!_inBuildAll) {
					mi.ShortcutKeys = 0;
					mi.ShortcutKeyDisplayString = null;
				}
				if(x.Attr(out s, "hk")) {
					bool ok = AKeys.More.ParseHotkeyString(s, out var hk);
					if(ok) try { mi.ShortcutKeys = hk; } catch { ok = false; }
					if(!ok) ADebug.Print("Invalid hotkey: " + s);
				}
				if(x.Attr(out string ss, "hkText")) mi.ShortcutKeyDisplayString = (s == null) ? ss : s + ", " + ss;
			} else {
				var style = item.Image == null ? ToolStripItemDisplayStyle.ImageAndText : (ToolStripItemDisplayStyle)x.Attr("style", 0);
				if(style == 0) style = hasCustomText ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image; //0 is ToolStripItemDisplayStyle.None
				item.DisplayStyle = style;
			}
		}

		/// <summary>
		/// If x has attribute t, gets its value.
		/// Else gets its name and converts to text, eg "File_OneTwo" to "One Two".
		/// </summary>
		string _GetDefaultItemText(XElement x)
		{
			if(!x.Attr(out string s, "t")) {
				string tag = x.Name.LocalName;
				s = tag.Remove(0, tag.LastIndexOf('_') + 1); //eg "Edit_Copy" -> "Copy"
				s = s.RegexReplace("(?<=[^A-Z])(?=[A-Z])", " "); //"OneTwoThree" -> "One Two Three". //speed: don't need to optimize.
			}
			return s;
		}

		/// <summary>
		/// Merges custom attributes into default menubar or toolbar XML.
		/// Reorders toolbar buttons if need.
		/// </summary>
		/// <param name="xCustom">Root element of customizations file.</param>
		/// <param name="xtsDef">Default menustrip or toolstrip. For custom toolbars the function can replace it.</param>
		/// <param name="name">xtsDef name, just to avoid getting it again.</param>
		/// <param name="isMenu"></param>
		void _MergeCustom(XElement xCustom, ref XElement xtsDef, string name, bool isMenu)
		{
			var xtsCust = xCustom.Element(xtsDef.Name); if(xtsCust == null) return;

			if(isMenu) {
			} else if(name.Starts("Custom")) {
				var xc = xCustom.Element(name); if(xc == null) return;
				xc.Remove();
				xtsDef.ReplaceWith(xc);
				xtsDef = xc;
				return;
				//MSDN: "if the new content has no parent, then the objects are simply attached to the XML tree. If the new content already is parented and is part of another XML tree, then the new content is cloned". Tested, it's true.
			} else {
				//reorder toolbar buttons
				if(xtsCust.Attr(out string s, "order")) {
					xtsDef.Elements("sep").Remove(); //remove all default <sep/>, because all separators now are in the 'order' attribute
					var a = s.SegSplit(" ");
					for(int i = a.Length - 1; i >= 0; i--) {
						if(a[i] == "sep") {
							xtsDef.AddFirst(new XElement("sep"));
						} else {
							var xb = xtsDef.Element(a[i]); if(xb == null) continue;
							xb.Remove(); xtsDef.AddFirst(xb);
						}
					}
				}
			}

			foreach(var xCust in xtsCust.Elements()) {
				foreach(var xDef in xtsDef.Descendants(xCust.Name)) {
					foreach(var att in xCust.Attributes()) xDef.SetAttributeValue(att.Name, att.Value);
				}
			}
		}

		/// <summary>
		/// Extracts differences between _xmlFileDefault and _xStrips and saves to _xmlFileCustom.
		/// </summary>
		void _DiffCustom()
		{
			var xStripsDefault = AExtXml.LoadElem(_xmlFileDefault);
			var xStripsCustom = new XElement("strips");
			string s;

			//menus
			_DiffDescendants(MenuBar.Tag as XElement, true);

			//standard toolbars
			foreach(var ts in Toolbars.Values) {
				if(ts == _tsCustom1 || ts == _tsCustom2) continue;
				_DiffDescendants(ts.Tag as XElement, false);
			}

			void _DiffDescendants(XElement xts, bool isMenu)
			{
				XElement xtsDef = xStripsDefault.Element(xts.Name), xtsCust = null;
				foreach(var x in xts.Descendants()) {
					var name = x.Name; if(name == "sep") continue;
					XElement xDef = xtsDef.Desc(name), xCust = null;
					foreach(var att in x.Attributes()) {
						var aname = att.Name;
						if(att.Value == xDef.Attr(aname)) continue;
						if(xtsCust == null) xStripsCustom.Add(xtsCust = new XElement(xts.Name));
						if(xCust == null) xtsCust.Add(xCust = new XElement(name));
						xCust.SetAttributeValue(aname, att.Value);
					}
				}
				if(isMenu) return;
				//order
				var s1 = new StringBuilder();
				var s2 = new StringBuilder();
				foreach(var x in xts.Elements()) { if(s1.Length > 0) s1.Append(' '); s1.Append(x.Name); }
				foreach(var x in xtsDef.Elements()) { if(s2.Length > 0) s2.Append(' '); s2.Append(x.Name); }
				s = s1.ToString();
				if(s != s2.ToString()) {
					if(xtsCust == null) xStripsCustom.Add(xtsCust = new XElement(xts.Name));
					xtsCust.SetAttributeValue("order", s);
				}
			}

			//custom toolbars. Temporarily move them from _xStrips to xCustom.
			var xCust1 = _tsCustom1.Tag as XElement;
			var xCust2 = _tsCustom2.Tag as XElement;
			if(xCust1.HasElements) { xCust1.Remove(); xStripsCustom.Add(xCust1); }
			if(xCust2.HasElements) { xCust2.Remove(); xStripsCustom.Add(xCust2); }

			//AOutput.Clear();
			//AOutput.Write(xStripsCustom);
#if true
			//save
			try {
				AFile.CreateDirectoryFor(_xmlFileCustom);
				xStripsCustom.SaveElem(_xmlFileCustom);
			}
			catch(Exception e) {
				AOutput.Write("Failed to save XML file", _xmlFileCustom, e.Message);
			}
#endif

			if(xCust1.HasElements) { xCust1.Remove(); _xStrips.Add(xCust1); }
			if(xCust2.HasElements) { xCust2.Remove(); _xStrips.Add(xCust2); }
		}

		/// <summary>
		/// Toolbar button MouseUp handler. Implements context menu that allows to customize.
		/// </summary>
		private void _OnMouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Right) return;
			var item = sender as ToolStripItem;
			var ts = item.Owner;
			var dd = ts as ToolStripDropDownMenu;
			bool isMenu = dd != null;
			bool isCustom = ts == _tsCustom1 || ts == _tsCustom2;
			bool isSeparator = item is ToolStripSeparator;
			bool isHidden = item.Overflow == ToolStripItemOverflow.Always;
			var x = item.Tag as XElement;

			var m = new AMenu();
			m["Properties..."] = o =>
			{
				using(var f = new FSMProperties(this, item.Tag as XElement, isMenu)) {
					if(f.ShowDialog(_form) == DialogResult.OK)
						_Strips_Customize(6, item, null, f);
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
				m["How to customize..."] = o => ADialog.ShowInfo("Customizing toolbars and menus",
					"There are several standard toolbars and two custom toolbars (initially empty). Standard toolbar buttons cannot be added and removed, but can be hidden and reordered. Menu items cannot be added, removed, hidden and reordered." +
					"\n\nYou can find most customization options in two context menus. Right-clicking a button or menu item shows its context menu. Right-clicking before the first button shows toolbar's context menu. You can Alt+drag toolbar buttons to reorder them on the same toolbar. You can Alt+drag toolbars to dock them somewhere else. Use splitters to resize. Right click a splitter to change its thickness."
					);
				string folder = APath.GetDirectoryPath(_xmlFileCustom), link = $"<a href=\"{folder}\">{folder}</a>";
				m["How to backup, restore, reset..."] = o =>
				{
					ADialog.Show("How to backup, restore or reset customizations",
					"All customizations are saved in XML files in folder\n" +
					link +
					"\n\nTo backup:  copy the file." +
					"\nTo restore:  exit this application and replace the file with the backup file." +
					"\nTo reset:  exit this application and delete the file."
					, icon: DIcon.Info, onLinkClick: h => { AExec.Run(h.LinkHref); });
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
		/// With action 6 (properties) - AuStripManagerPropertiesDialog.
		/// </param>
		void _Strips_Customize(uint action, ToolStripItem item, ToolStrip tsTo = null, object etc = null)
		{
			string s;
			XElement x = item.Tag as XElement, xtbTo = tsTo?.Tag as XElement;

			switch(AMath.LoUshort(action)) {
			case 1: //copy from menu or standard toolbar to custom toolbar
				var xNew = new XElement(x.Name, x.Attributes()); //copy without descendants but with attributes
				if(item is ToolStripDropDownItem ddi && ddi.HasDropDown) {
					var dd = ddi.DropDown as ToolStripDropDownMenu;
					xNew.SetAttributeValue("dd", dd.Name);
				}
				xtbTo.Add(xNew);
				var k = _CreateChildItem(xNew, false);
				if(k is ToolStripButton b) { //copy checked and disabled states
					if(item is ToolStripMenuItem m1) b.Checked = m1.Checked; else if(item is ToolStripButton b1) b.Checked = b1.Checked;
					b.Enabled = item.Enabled;
				}
				_AddChildItem(xNew, k, tsTo);
				break;
			case 2: //move
				if(etc != null) {
					var itemTo = etc as ToolStripItem;
					int i1 = tsTo.Items.IndexOf(item), i2 = tsTo.Items.IndexOf(itemTo);
					if(i2 > i1) i2--;
					if(i2 == i1) return;
					x.Remove(); (itemTo.Tag as XElement).AddBeforeSelf(x);
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
				_AddChildItem(xSep, sep, tsTo, tsTo.Items.IndexOf(item));
				break;
			case 5: //hide/unhide
				bool hide = !(item.Overflow == ToolStripItemOverflow.Always);
				x.SetAttributeValue("hide", hide ? "" : null);
				item.Overflow = hide ? ToolStripItemOverflow.Always : ToolStripItemOverflow.AsNeeded;
				break;
			case 6: //properties
				var f = etc as FSMProperties;
				bool isMenu = item.Owner is ToolStripDropDownMenu;

				s = f.textText.Text; if(s == "") s = null;
				x.SetAttributeValue("t2", s);

				s = f.comboColor.Text; if(s == "") s = null;
				x.SetAttributeValue("color", s);

				s = f.textIcon.Text; if(s == "") s = null;
				x.SetAttributeValue("i2", s);

				if(isMenu) {
					s = f.textHotkey.Text; if(s == "") s = null;
					x.SetAttributeValue("hk", s);
					//remove the hotkey from another item, to avoid duplicates
					if(AKeys.More.ParseHotkeyString(s, out var hk)) {
						var xx = f.FindUsedHotkey(hk, x);
						if(xx != null) {
							xx.SetAttributeValue("hk", null);
							_XElementToMenuItem(xx).ShortcutKeys = 0;
						}
					}
				} else {
					if(f.radioOnlyText.Checked) s = "1";
					else if(f.radioOnlyIcon.Checked) s = "2";
					else if(f.radioIconAndText.Checked) s = "3";
					else s = null;
					x.SetAttributeValue("style", s);
				}

				_SetItemProperties(x, item, isMenu, false);
				break;
			}

			_DiffCustom();
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
			//AOutput.Write(item, ts, isCustom);
			if(!Au.Util.ADragDrop.SimpleDragDrop(ts, MButtons.Left, k =>
			{
				if(k.Msg.message != Api.WM_MOUSEMOVE) return;
				target = ts.GetItemAt(ts.MouseClientXY());
				//AOutput.Write(target);
				isOutside = (target == null && AWnd.FromMouse() != (AWnd)ts);
				k.Cursor = isOutside ? Cursors.No : Cursors.Hand;
			}) || isOutside) return;
			_Strips_Customize(2, item, ts, target);
		}

		ToolStripItem _XElementToItem(XElement x)
		{
			return x.Annotation<ToolStripItem>();
		}

		ToolStripMenuItem _XElementToMenuItem(XElement x)
		{
			return x.Annotation<ToolStripItem>() as ToolStripMenuItem;
		}

		/// <summary>
		/// Gets menu items and toolbar buttons that have the specified name (XML tag).
		/// If not found, returns empty List.
		/// </summary>
		/// <param name="name">XML tag.</param>
		public List<ToolStripItem> Find(string name)
		{
			var a = new List<ToolStripItem>();
			foreach(var x in _xStrips.Descendants(name)) {
				if(x.Parent == _xStrips) continue;
				a.Add(_XElementToItem(x));
			}
			return a;
		}
	}
}
