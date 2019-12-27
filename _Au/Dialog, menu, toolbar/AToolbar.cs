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
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au.Types;
using static Au.AStatic;

#pragma warning disable 1591 //XML doc //TODO

namespace Au
{
	/// <summary>
	/// TODO
	/// </summary>
	public partial class AToolbar : AMTBase
	{
		_ToolStrip _c;
		_Settings _sett;
		bool _constructed;
		string _name;

		/// <summary>
		/// Gets the toolbar window as <see cref="ToolStrip"/>.
		/// You can use its properties, methods and events.
		/// </summary>
		public ToolStrip Control => _c;

		private protected override ToolStrip MainToolStrip => _c; //used by AMTBase

		public string Name => _name;

		public AToolbar(string name, AWnd owner = default, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0)
			: base(f, l)
		{
			APerf.First();

			if(Empty(name)) throw new ArgumentException("Empty name");
			_name = name;

			string s = AFolders.Workspace; if(s == null) s = AFolders.ThisAppDocuments;
			_sett = _Settings.Load(s + @"\toolbars\" + name + ".json");

			_c = new _ToolStrip(this) {
				Text = "Toolbar " + name,
				Bounds = _sett.bounds,
				Border = _sett.border, //default Sizable2
				BorderColor = _sett.borderColor,
			};

			_constructed = true;
		}

		#region add item

		/// <summary>
		/// Adds new button as <see cref="ToolStripButton"/>.
		/// The same as <see cref="Add(string, Action{MTClickArgs}, object, string, int)"/>.
		/// </summary>
		public Action<MTClickArgs> this[string text, object icon = null, string tooltip = null, [CallerLineNumber] int l = 0] {
			set { Add(text, value, icon, tooltip, l); }
		}

		/// <summary>
		/// Adds new button as <see cref="ToolStripButton"/>.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onClick">Callback function. Called when the button clicked.</param>
		/// <param name="icon">See <see cref="AMenu.Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="tooltip">Tooltip text.</param>
		/// <param name="l"></param>
		/// <remarks>
		/// Sets button text, icon and Click event handler. Other properties can be specified later. See example.
		/// 
		/// Code <c>t.Add("text", o => Print(o));</c> is the same as <c>t["text"] = o => Print(o);</c>. See <see cref="this[string, object, string, int]"/>.
		/// </remarks>
		public ToolStripButton Add(string text, Action<MTClickArgs> onClick, object icon = null, string tooltip = null, [CallerLineNumber] int l = 0)
		{
			var item = new ToolStripButton(text);
			_Add(item, onClick, icon, tooltip, l);
			return item;
		}

		/// <summary>
		/// Adds item of any ToolStripItem type, for example ToolStripLabel, ToolStripTextBox, ToolStripComboBox, ToolStripProgressBar.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="icon">See <see cref="AMenu.Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="tooltip">Tooltip text.</param>
		/// <param name="onClick">Callback function. Called when the item clicked. Not useful with most item types.</param>
		/// <param name="l"></param>
		public void Add(ToolStripItem item, object icon = null, string tooltip = null, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			_Add(item, onClick, icon, tooltip, l, true);
			//Print(item.Padding, item.Margin);

			//Activate window when a child control clicked, or something may not work, eg cannot enter text in Edit control.
			if(item is ToolStripControlHost h && h.CanSelect) { //combo, edit, progress
				h.GotFocus += (unu, sed) => { //info: before MouseDown, which does not work well with combo box
					Api.SetForegroundWindow((AWnd)_c); //does not fail, probably after a mouse click this process is allowed to activate windows, even if the click did not activate because of the window style
				};
				h.KeyDown += (_, e) => {
					if(e.KeyData == Keys.Escape && ((AWnd)_c).IsActive) _c.Focus();
				};
			}
		}

		void _Add(ToolStripItem item, Action<MTClickArgs> onClick, object icon, string tooltip, int sourceLine, bool custom = false)
		{
			if(!custom) {
				item.Margin = default; //default top 1, bottom 2
				item.Padding = new Padding(1); //default 0
			}
			if(tooltip != null) item.ToolTipText = tooltip;

			_c.Items.Add(item);

			_SetItemProp(true, item, onClick, icon, sourceLine);

			bool onlyImage = NoText && (item.Image != null || item.ImageIndex >= 0 || !Empty(item.ImageKey));
			if(onlyImage) item.DisplayStyle = ToolStripItemDisplayStyle.Image; //default ImageAndText
			else item.AutoToolTip = false; //default true
		}

		/// <summary>
		/// Adds separator.
		/// By default it's vertical. Horizontal if layout is vertical or <i>groupName</i> not null.
		/// </summary>
		/// <param name="groupName">If not null, the separator is horizontal. If not "", this text will be displayed.</param>
		public ToolStripSeparator Separator(string groupName = null)
		{
			var item = new ToolStripSeparator();
			if(groupName != null) {
				item.AccessibleName = item.Name = groupName;
				item.AutoSize = false;
				item.Height = groupName.Length == 0 ? 3 : TextRenderer.MeasureText("A", _c.Font).Height + 3;
				item.Width = 70000;
			}
			_c.Items.Add(item);
			LastItem = item;
			return item;
		}

		/// <summary>
		/// Adds new button as <see cref="ToolStripDropDownButton"/> with a drop-down menu.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="menu">Callback function that adds menu items. Called when opening the menu first time. If sets <see cref="AMenu.MultiShow"/> = false, called each time.</param>
		/// <param name="icon">See <see cref="AMenu.Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="tooltip">Tooltip text.</param>
		/// <param name="l"></param>
		public ToolStripDropDownButton MenuButton(string text, Action<AMenu> menu, object icon = null, string tooltip = null, [CallerLineNumber] int l = 0)
		{
			var item = new _MenuButton(this, text, menu);
			_Add(item, null, icon, tooltip, l);
			return item;
		}

		/// <summary>
		/// Adds new button as <see cref="ToolStripSplitButton"/> with a drop-down menu.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="menu">Callback function that adds menu items. Called when need the menu first time (when opening it, or when clicked the button if <i>onClick</i> null). If sets <see cref="AMenu.MultiShow"/> = false, called each time.</param>
		/// <param name="icon">See <see cref="AMenu.Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="tooltip">Tooltip text.</param>
		/// <param name="onClick">Callback function. Called when the button clicked. If null, will execute the first menu item.</param>
		/// <param name="l"></param>
		public ToolStripSplitButton SplitButton(string text, Action<AMenu> menu, object icon = null, string tooltip = null, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			var item = new _SplitButton(this, text, menu, onClick);
			_Add(item, onClick, icon, tooltip, l);
			return item;
		}

		AMenu _CreateMenu(ToolStripDropDownItem item, ref Action<AMenu> menu)
		{
			if(menu == null) return null;
			var m = new AMenu(item.Text, _sourceFile, GetItemSourceLine_(item)) { MultiShow = true };
			item.DropDown = m.Control; //attaches
			m.Control.OwnerItem = item; //the callback may ned it
			menu(m);
			if(m.MultiShow) menu = null; //default true, but callback may set false to call it each time
			return m;
		}

		class _MenuButton : ToolStripDropDownButton
		{
			AToolbar _tb;
			Action<AMenu> _action;

			public _MenuButton(AToolbar tb, string text, Action<AMenu> menu) : base(text)
			{
				_tb = tb;
				_action = menu;
			}

			protected override void OnDropDownShow(EventArgs e)
			{
				_tb._CreateMenu(this, ref _action);
				base.OnDropDownShow(e);
			}

			protected override void OnDropDownClosed(EventArgs e)
			{
				base.OnDropDownClosed(e);
				if(_action != null) DropDown = null; //GC
			}
		}

		class _SplitButton : ToolStripSplitButton
		{
			AToolbar _tb;
			Action<AMenu> _action;
			AMenu _m;
			ToolStripMenuItem _mi;
			bool _autoButtonAction;

			public _SplitButton(AToolbar tb, string text, Action<AMenu> menu, Action<MTClickArgs> onClick) : base(text)
			{
				_tb = tb;
				_action = menu;
				_autoButtonAction = onClick == null && menu != null;
			}

			protected override void OnDropDownShow(EventArgs e)
			{
				if(_action != null) {
					if(_autoButtonAction) _CreateMenu2(); //need to find default item and make bold
					else _tb._CreateMenu(this, ref _action);
				}
				base.OnDropDownShow(e);
			}

			protected override void OnDropDownClosed(EventArgs e)
			{
				base.OnDropDownClosed(e);
				if(_action != null) DropDown = null; //GC
			}

			protected override void OnButtonClick(EventArgs e)
			{
				if(_autoButtonAction) {
					if(_action != null) _CreateMenu2();
					if(_mi != null && _mi.Enabled) _m.OnClick_(_mi);
				}
				base.OnButtonClick(e);
			}

			void _CreateMenu2()
			{
				_m = _tb._CreateMenu(this, ref _action);
				_mi = _m.Control.Items.OfType<ToolStripMenuItem>().FirstOrDefault();
				if(_mi != null) _mi.Font = new Font(_mi.Font, FontStyle.Bold);
			}
		}

		public Action<AMenu> this[bool splitButton, string text, object icon = null, string tooltip = null, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0] {
			set {
				if(splitButton) SplitButton(text, value, icon, tooltip, onClick, l);
				else MenuButton(text, value, icon, tooltip, l);
			}
		}

		/// <summary>
		/// Gets the last added item as <see cref="ToolStripButton"/>.
		/// Returns null if it is not a <b>ToolStripButton</b>.
		/// </summary>
		/// <remarks>
		/// You can instead use <see cref="AMTBase.LastItem"/>, which gets <see cref="ToolStripItem"/>, the base class of all supported item types; cast it to a derived type if need.
		/// </remarks>
		public ToolStripButton LastButton => LastItem as ToolStripButton;

		#endregion

		public Rectangle Bounds {
			get => _c.Bounds;
			set {
				_c.Bounds = value;
			}
		}

		public void Show()
		{
			if(!_c.Created) {
				GetIconsAsync_(_c);
				_c.Create();
				APerf.Next();
			}
			_c.Show();
		}

		public void Close()
		{
			_c.Dispose();
		}

		public bool Owned => false;

		/// <summary>
		/// When adding items with images, set to not display item text. If tooltip not specified, use item text for it.
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards. If true and the item has an image, sets item <b>DisplayStyle</b> property = Image. Else sets item <b>AutoToolTip</b> property = false.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var t = new AToolbar();
		/// t.NoText = true;
		/// t["Find", @"Q:\images\find.ico"] = o => Print(o);
		/// t["Copy", @"Q:\images\copy.ico"] = o => Print(o);
		/// t.NoText = false;
		/// t["Delete", @"Q:\images\delete.ico"] = o => Print(o);
		/// t.NoText = true;
		/// t["Run", @"Q:\images\run.ico"] = o => Print(o);
		/// ]]></code>
		/// </example>
		public bool NoText { get; set; }

		public TBBorder Border {
			get => _c.Border;
			set => _c.Border = value;
		}

		public ColorInt BorderColor {
			get => _c.BorderColor;
			set => _c.BorderColor = value;
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Used with <see cref="AToolbar.Border"/>.
	/// </summary>
	public enum TBBorder
	{
		/// <summary>No border. User cannot resize the toolbar.</summary>
		None,
		//note: don't reorder.

		/// <summary>1 pixel border. User cannot resize the toolbar.</summary>
		Fixed1,

		/// <summary>1 pixel border + 1 pixel padding. User cannot resize the toolbar.</summary>
		Fixed2,

		/// <summary>1 pixel border + 2 pixels padding. User cannot resize the toolbar.</summary>
		Fixed3,

		/// <summary>1 pixel border + 3 pixels padding. User cannot resize the toolbar.</summary>
		Fixed4,

		/// <summary>1 pixel border. User can resize the toolbar.</summary>
		Sizable1,

		/// <summary>1 pixel border + 1 pixel padding. User can resize the toolbar.</summary>
		Sizable2,

		/// <summary>1 pixel border + 2 pixels padding. User can resize the toolbar.</summary>
		Sizable3,

		/// <summary>1 pixel border + 3 pixels padding. User can resize the toolbar.</summary>
		Sizable4,

		/// <summary>3D border. User cannot resize.</summary>
		Fixed3D,

		/// <summary>3D border. User can resize the toolbar.</summary>
		Sizable3D,

		/// <summary>Standard sizing border.</summary>
		Sizable,

		/// <summary>Title bar. User cannot resize the toolbar.</summary>
		FixedWithCaption,

		/// <summary>Title bar and standard sizing border.</summary>
		SizableWithCaption,

		/// <summary>Title bar and [x] button to close. User cannot resize the toolbar.</summary>
		FixedWithCaptionX,

		/// <summary>Title bar, [x] button and standard sizing border.</summary>
		SizableWithCaptionX,
	}
}