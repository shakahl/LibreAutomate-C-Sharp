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

namespace Au
{
	/// <summary>
	/// Floating toolbar based on <see cref="ToolStrip"/>.
	/// Can be attached to windows and other UI objects of other programs.
	/// </summary>
	public partial class AToolbar : AMTBase
	{
		_ToolStrip _c;
		readonly _Settings _sett;
		readonly bool _constructed; //ctor finished setting default properties
		bool _loaded; //Show() called
		bool _topmost; //no owner or topmost owner
		readonly string _name;

		static int s_treadId;

		/// <summary>
		/// Initializes this object.
		/// Reads the settings file. Creates the <b>ToolStrip</b> object (<see cref="Control"/>) but not its window handle.
		/// </summary>
		/// <param name="name">Toolbar name. Must be valid filename. Used for the toolbar's settings file name. Also it is the initial <b>Name</b> and <b>Text</b> of <see cref="Control"/>.</param>
		/// <param name="resetSettings">Reset toolbar settings to default values. It deletes the settings file of this toolbar.</param>
		/// <param name="f"><see cref="CallerFilePathAttribute"/></param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		/// <example>
		/// <code><![CDATA[
		/// var t = new AToolbar("example");
		/// t["button 1"] = o => Print(o);
		/// t[false, "menu"] = m => {
		/// 	m["item 1"] = o => Print(o);
		/// 	m["item 2"] = o => Print(o);
		/// };
		/// t["button 2"] = o => Print(o);
		/// t.Show();
		/// ADialog.Show();
		/// ]]></code>
		/// </example>
		public AToolbar(string name, bool resetSettings = false, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0)
			: base(f, l)
		{
			APerf.First();

			int tid = Thread.CurrentThread.ManagedThreadId;
			if(s_treadId == 0) s_treadId = tid; else if(tid != s_treadId) PrintWarning("All toolbars should be in single thread. Multiple threads use more CPU.");

			if(Empty(name)) throw new ArgumentException("Empty name");
			_name = name;

			string s = AFolders.Workspace; if(s == null) s = AFolders.ThisAppDocuments;
			_sett = _Settings.Load(s + @"\toolbars\" + name + ".json", resetSettings);

			_c = new _ToolStrip(this) {
				Name = _name,
				Text = _name,
				Size = _sett.size,
			};

			_anchor = _sett.anchor;
			_xy = _sett.location;
			Border = _sett.border; //default Sizable2

			_constructed = true;
		}

		/// <summary>
		/// Gets the toolbar window as <see cref="ToolStrip"/>.
		/// You can use its properties, methods and events.
		/// </summary>
		public ToolStrip Control => _c;

		private protected override ToolStrip MainToolStrip => _c; //used by AMTBase

		/// <summary>
		/// Toolbar name.
		/// </summary>
		public string Name => _name;

		/// <summary>
		/// True if properties of this toolbar were modified now or in the past (the settings JSON file exists).
		/// </summary>
		/// <remarks>
		/// To delete the settings JSON file you can use the constructor's parameter <i>resetSettings</i>.
		/// </remarks>
		public bool SettingsModified => _sett.Modified;

		#region add item

		/// <summary>
		/// Adds new button as <see cref="ToolStripButton"/>.
		/// The same as <see cref="Add(string, Action{MTClickArgs}, object, string, int)"/>.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// tb["Example"] = o => Print(o);
		/// ]]></code>
		/// </example>
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
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
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
		/// Adds item of any <b>ToolStripItem</b>-based type, for example <b>ToolStripLabel</b>, <b>ToolStripTextBox</b>, <b>ToolStripComboBox</b>.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="icon">See <see cref="AMenu.Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="tooltip">Tooltip text.</param>
		/// <param name="onClick">Callback function. Called when the item clicked. Not useful with most item types.</param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
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

				//rejected: make min button height like of native toolbars
				//item.Padding = new Padding(1); //default 0
				//because of this ToolStripSplitButton bug: ignores vertical padding. Then split buttons are smaller.
				if(item is ToolStripSplitButton sb) {
					//sb.AutoSize = false;
					sb.DropDownButtonWidth += 4;
				}
			}
			if(tooltip != null) item.ToolTipText = tooltip;

			_c.Items.Add(item);

			_SetItemProp(true, item, onClick, icon, sourceLine);

			bool onlyImage = NoText && (item.Image != null || item.ImageIndex >= 0 || !Empty(item.ImageKey));
			if(onlyImage) item.DisplayStyle = ToolStripItemDisplayStyle.Image; //default ImageAndText
			else item.AutoToolTip = false; //default true

			_OnItemAdded(item);
		}

		/// <summary>
		/// Adds new vertical separator. Horizontal if layout is vertical.
		/// </summary>
		public ToolStripSeparator Separator()
		{
			var item = new ToolStripSeparator();
			_c.Items.Add(item);
			LastItem = item;
			return item;
		}

		/// <summary>
		/// Adds new horizontal separator, optionally with text.
		/// </summary>
		/// <param name="name">If not null, this text will be displayed.</param>
		/// <exception cref="InvalidOperationException">Unsupported <b>LayoutStyle</b> (must be flow or vertical) or <b>FlowDirection</b> (must be horizontal).</exception>
		/// <example>
		/// Add separator and set color.
		/// <code><![CDATA[
		/// t.Group("Group").ForeColor = Color.MediumPurple;
		/// ]]></code>
		/// </example>
		public TBGroupSeparator Group(string name = null)
		{
			var item = new TBGroupSeparator(_c, name);
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
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
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
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
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

		/// <summary>
		/// Adds new button with a drop-down menu.
		/// </summary>
		/// <param name="splitButton">If true, calls <see cref="SplitButton"/>, else <see cref="MenuButton"/>.</param>
		/// <param name="text">Text.</param>
		/// <param name="icon">See <see cref="AMenu.Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="tooltip">Tooltip text.</param>
		/// <param name="onClick">Callback function. Called when the button part of the split button clicked. If null, will execute the first menu item. Not used if it is not split button.</param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		/// <returns>The value is a callback function that adds drop-down menu items. Te same as parameter <i>menu</i> of <see cref="MenuButton"/> or <see cref="SplitButton"/>.</returns>
		/// <example>
		/// <code><![CDATA[
		/// var t = new AToolbar("example");
		/// t["button 1"] = o => Print(o);
		/// t[false, "menu"] = m => {
		/// 	m["item 1"] = o => Print(o);
		/// 	m["item 2"] = o => Print(o);
		/// };
		/// t["button 2"] = o => Print(o);
		/// t.Show();
		/// ADialog.Show();
		/// ]]></code>
		/// </example>
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

		#region show, close, owner

		/// <summary>
		/// Shows the toolbar and attaches to a screen.
		/// </summary>
		/// <param name="screen">Can be used to define the screen. For example a screen index (0 the primary, 1 the first non-primary, and so on). If not specified, the toolbar will be attached to the screen where it is now or where will be moved later.</param>
		/// <remarks>
		/// The toolbar will be moved when the screen moved or resized.
		/// </remarks>
		public void Show(AScreen screen = default) => _Show(false, default, null, screen);

		/// <summary>
		/// Shows the toolbar and attaches to a window.
		/// </summary>
		/// <param name="ownerWindow">Window or control. Can belong to any process.</param>
		/// <param name="clientArea">Let the toolbar position be relative to the client area of the window.</param>
		/// <remarks>
		/// The toolbar will be above the window in the Z order; moved when the window moved or resized; hidden when the window hidden, cloaked or minimized; destroyed when the window destroyed.
		/// </remarks>
		public void Show(AWnd ownerWindow, bool clientArea = false)
		{
			_followClientArea = clientArea;
			_Show(true, ownerWindow, null, default);
		}

		/// <summary>
		/// Shows the toolbar and attaches to an object in a window.
		/// </summary>
		/// <param name="ownerWindow">Window that contains the object. Can be control. Can belong to any process.</param>
		/// <param name="oo">A variable of a user-defined class that implements <see cref="ITBOwnerObject"/> interface. It provides object location, visibility, etc.</param>
		/// <remarks>
		/// The toolbar will be above the window in the Z order; moved when the object or window moved or resized; hidden when the object or window hidden, cloaked or minimized; destroyed when the object or window destroyed.
		/// </remarks>
		public void Show(AWnd ownerWindow, ITBOwnerObject oo) => _Show(true, ownerWindow, oo, default);

		void _Show(bool owned, AWnd owner, ITBOwnerObject oo, AScreen screen)
		{
			//if(_c.IsDisposed) throw new ObjectDisposedException("AToolbar");
			if(_loaded) throw new InvalidOperationException();

			AWnd c = default;
			if(owned) {
				if(owner.Is0) throw new ArgumentException();
				var w = owner.Window; if(w.Is0) return;
				if(w != owner) { c = owner; owner = w; }
			}

			_CreateControl(owned, owner, screen);
			_Manager.Add(this, owner, c, oo);
		}

		void _CreateControl(bool owned, AWnd owner, AScreen screen = default)
		{
			_topmost = !owned || owner.IsTopmost;
			if(!owned || _anchor == TBAnchor.None) _os = new _OwnerScreen(this, screen);

			//if(!_loaded) {
			GetIconsAsync_(_c);
			_c.ResumeLayout();
			_AutoSize();
			APerf.Next();
			//}
			_c.Hwnd(create: true);
			_loaded = true;
		}

		/// <summary>
		/// Closes and disposes the toolbar.
		/// </summary>
		public void Close()
		{
			_c.Dispose();
		}

		/// <summary>
		/// Returns true if the toolbar is attached to a window or an object in a window.
		/// </summary>
		public bool IsOwned => _ow != null;

		/// <summary>
		/// Returns the owner top-level window.
		/// Returns default(AWnd) if the toolbar is not owned by a window or an object in a window.
		/// </summary>
		public AWnd OwnerWindow => _ow?.w ?? default;

		/// <summary>
		/// Returns the owner control.
		/// Returns default(AWnd) if not owned by a control or an object in a control.
		/// </summary>
		public AWnd OwnerControl => _oc?.c ?? default;

		#endregion

		#region properties

		/// <summary>
		/// If true, buttons added afterwards will have image without text displayed (unless there is no image). Text will be displayed as tooltip, unless tooltip is specified.
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

		/// <summary>
		/// Border style.
		/// </summary>
		public TBBorder Border {
			get => _border;
			set {
				if(value == _border) return;
				_c.SetBorder(value);
				_border = value;
				if(_constructed) _sett.border = value;
			}
		}
		TBBorder _border;

		/// <summary>
		/// Border color.
		/// </summary>
		public ColorInt BorderColor {
			get => _sett.borderColor;
			set {
				if(value == _sett.borderColor) return;
				_sett.borderColor = (int)value;
				var w = _c.Hwnd();
				if(!w.Is0) unsafe { Api.RedrawWindow(w, flags: Api.RDW_FRAME | Api.RDW_INVALIDATE); }
			}
		}

		/// <summary>
		/// Specifies to which owner window's edges the toolbar keeps constant distance when moving or resizing the owner.
		/// </summary>
		/// <remarks>
		/// The owner also can be a screen, control or other object. It is specified when calling <see cref="Show"/>.
		/// </remarks>
		/// <seealso cref="Location"/>
		public TBAnchor Anchor {
			get => _anchor;
			set {
				if(value == _anchor) return;
				var prev = _anchor;
				_sett.anchor = _anchor = value;
				if(IsOwned) {
					_os = _anchor == TBAnchor.None ? new _OwnerScreen(this, default) : null; //follow toolbar's screen
					if(prev == TBAnchor.None && _followedOnce) {
						if(_oc != null) _oc.UpdateRect(out _); else _ow.UpdateRect(out _);
					}
				}
				if(_followedOnce) {
					var b = _c.Bounds;
					_UpdateXY(b.X, b.Y, b.Width, b.Height);
				}
			}
		}
		TBAnchor _anchor;

		/// <summary>
		/// Specifies distances between edges of the toolbar and edges of its owner, depending on <see cref="Anchor"/>.
		/// </summary>
		/// <remarks>
		/// The owner also can be a screen, control or other object. It is specified when calling <see cref="Show"/>.
		/// This property is updated when moving or resizing the toolbar.
		/// </remarks>
		public TBLocation Location {
			get => _xy;
			set {
				_preferSize = false;
				if(value.Equals(_xy)) return;
				_sett.location = _xy = value;
				if(_followedOnce) _FollowRect();
				//CONSIDER: add ScreenIndex property or something. Now if screen is auto-selected, this sets xy in that screen, but caller may want in primary screen.
			}
		}
		TBLocation _xy;

		/// <summary>
		/// Whether the border can be used to resize the toolbar.
		/// </summary>
		public bool Sizable {
			get => _sett.sizable;
			set => _sett.sizable = value;
		}

		/// <summary>
		/// Toolbar width and height.
		/// </summary>
		/// <remarks>
		/// This property is updated when resizing the toolbar.
		/// </remarks>
		public SIZE Size {
			get => _c.Size;
			set {
				if((Size)value != _c.Size) {
					_sett.size = value;
					_c.Size = value;
				}
				if(!_followedOnce) _preferSize = true;
			}
		}

		/// <summary>
		/// Automatically resize the toolbar to make all buttons visible.
		/// </summary>
		/// <remarks>
		/// Autosizing occurs in these cases:
		/// - When showing the toolbar.
		/// - When the user resizes the toolbar. It also sets the wrap width.
		/// - When setting this property = true (in code or in the context menu) or changing <see cref="AutoSizeWrapWidth"/> wile it is true.
		/// </remarks>
		public bool AutoSize {
			get => _sett.autoSize;
			set {
				if(!value.Equals(_sett.autoSize)) {
					_sett.autoSize = value;
					if(_loaded) _AutoSize();
				}
			}
		}

		/// <summary>
		/// When <see cref="AutoSize"/> is true, this is the preferred width at which buttons are moved to the next row. Unlimited if 0.
		/// </summary>
		/// <remarks>
		/// This property is updated when the user resizes the toolbar while <see cref="AutoSize"/> is true.
		/// If flow direction is vertical, this is the preferred height at which buttons are moved to the next column.
		/// </remarks>
		public int AutoSizeWrapWidth {
			get => _sett.wrapWidth;
			set {
				if(value != _sett.wrapWidth) {
					_sett.wrapWidth = value;
					if(_loaded) _AutoSize();
				}
			}
		}

		void _AutoSize()
		{
			if(!_sett.autoSize) return;
			int wrap = _sett.wrapWidth; if(wrap <= 0) wrap = 1000000;
			bool verticalWrap = _IsVerticalFlow;
			var ps = _c.GetPreferredSize(new Size(verticalWrap ? 1000000 : wrap, verticalWrap ? wrap : 1000000));
			_c.ClientSize = ps;
			if(!_followedOnce) {
				_preferSize = true;
				_sett.size = _c.Size;
			}
		}

		bool _IsVerticalFlow => _c.LayoutSettings is FlowLayoutSettings f && (f.FlowDirection == FlowDirection.TopDown || f.FlowDirection == FlowDirection.BottomUp);

		public TBFlags MiscFlags {
			get => _sett.miscFlags;
			set {
				_sett.miscFlags = value;
			}
		}

		/// <summary>
		/// Opacity and transparent color.
		/// </summary>
		/// <seealso cref="AWnd.SetTransparency(bool, int?, ColorInt?)"/>
		/// <example>
		/// <code><![CDATA[
		/// tb.Transparency = (64, null);
		/// ]]></code>
		/// </example>
		public (int? opacity, ColorInt? colorKey) Transparency {
			get => _transparency;
			set {
				if(value != _transparency) {
					_transparency = value;
					if(_loaded) _c.Hwnd().SetTransparency(value != default, value.opacity, value.colorKey);
				}
			}
		}
		(int? opacity, ColorInt? colorKey) _transparency;

		#endregion

		#region satellite

		/// <summary>
		/// A toolbar attached to this toolbar. Can be null.
		/// </summary>
		/// <exception cref="InvalidOperationException">The 'set' function throws if the satellite toolbar was attached to another toolbar or was shown as non-satellite toolbar.</exception>
		/// <remarks>
		/// The satellite toolbar is shown when mouse enters its owner and hidden when mouse leaves it and its owner. Like an "auto hide" feature.
		/// A toolbar can have multiple satellite toolbars at different times. A satellite toolbar can be attached/detached multiple times to the same toolbar.
		/// </remarks>
		public AToolbar Satellite {
			get => _satellite;
			set {
				if(value != _satellite) {
					if(value == null) {
						_SatHide();
						_satellite = null;
						//and don't clear _satPlanet etc
					} else {
						if((_c?.IsDisposed ?? false) || (value._c?.IsDisposed ?? false)) throw new ObjectDisposedException("AToolbar");
						var p = value._satPlanet; if(p != this) { if(p != null || value._loaded) throw new InvalidOperationException(); }
						_satellite = value;
						_satellite._satPlanet = this;
					}
				}
			}
		}

		/// <summary>
		/// If this is a sattellite toolbar (<see cref="Satellite"/>), gets its owner toolbar. Else null.
		/// </summary>
		public AToolbar SatellitePlanet => _satPlanet;

		AToolbar _satellite;
		AToolbar _satPlanet;
		bool _satVisible;
		int _satAnimation;
		ATimer _satTimer;
		ATimer _satAnimationTimer;

		void _SatMouse()
		{
			if(_satellite == null || _satVisible) return;
			_satVisible = true;

			//Print("show");
			if(!_satellite._loaded) {
				var owner = _c.Hwnd();
				_satellite._CreateControl(true, owner);
				_satellite._ow = new _OwnerWindow(owner);
				_satellite._ow.a.Add(_satellite);
				var w1 = _satellite._c.Hwnd(); w1.Owner = owner; //let OS keep Z order and close/hide when owner toolbar closed/minimized
			}
			_SatFollow();
			_SatShowHide(true, animate: true);

			_satTimer ??= new ATimer(_SatTimer);
			_satTimer.Every(100);
		}

		void _SatTimer(ATimer _)
		{
			Debug.Assert(!_c.IsDisposed);

			POINT p = AMouse.XY;
			int dist = Util.ADpi.ScaleInt(30);
			//var r = _c.Hwnd().Rect;
			//r.Inflate(dist, dist);
			//if(r.Contains(p.x, p.y)) return;
			var wa = AWnd.Active;

			RECT ru = default;
			foreach(var w in AWnd.GetWnd.ThreadWindows(AThread.NativeId, onlyVisible: true)) { //TODO
				if(ToolStrip.FromHandle(w.Handle) is ToolStrip ts) {
					if(w == wa) return;
					if(ts is ContextMenuStrip) return;
					var r = w.Rect;
					r.Inflate(dist, dist);
					if(r.Contains(p.x, p.y)) return;
					ru.Union(r);
				}
			}
			if(ru.Contains(p.x, p.y)) return;

			_SatHide(animate: true);
		}
		//void _SatTimer(ATimer _)
		//{
		//	if(_c.IsDisposed) {
		//		_SatClose();
		//		return;
		//	}

		//	POINT xy = AMouse.XY;
		//	int dist = Util.ADpi.ScaleInt(30);
		//	var rp = _c.Hwnd().Rect;
		//	rp.Inflate(dist, dist);
		//	if(rp.Contains(xy.x, xy.y)) return;

		//	int nVisible = 0;
		//	foreach(var v in _satList) {
		//		var w = v._c.Hwnd();
		//		if(!w.IsActive) {
		//			if(!w.GetRect(out var r)) continue;
		//			r.Inflate(dist, dist);
		//			if(!r.Contains(xy.x, xy.y)) {
		//				Print("hide");
		//				w.ShowLL(false);
		//				continue;
		//			}
		//		}
		//		nVisible++;
		//	}
		//	if(nVisible == 0) {
		//		_satVisible = false;
		//		_satTimer.Stop();
		//	}
		//}
		//void _SatTimer(ATimer t)
		//{
		//	int nVisible = 0;
		//	POINT xy = AMouse.XY;
		//	foreach(var v in _satList) {
		//		var w = v._c.Hwnd();
		//		if(!w.IsActive) {
		//			if(!w.GetRect(out var r)) continue;
		//			int dist = Util.ADpi.ScaleInt(30);
		//			r.Inflate(dist, dist);
		//			if(!r.Contains(xy.x, xy.y)) {
		//				Print("hide");
		//				w.ShowLL(false);
		//				continue;
		//			}
		//		}
		//		nVisible++;
		//	}
		//	if(nVisible == 0) {
		//		_satVisible = false;
		//		t.Stop();
		//	}
		//}

		void _SatDestroying()
		{
			if(_satPlanet != null) _satPlanet.Satellite = null;
			ADebug.PrintIf(_satellite != null, "_satellite");
			//When destroying planet, OS at first destroys satellites (owned windows).
		}

		//Hides _satellite and stops _satTimer.
		void _SatHide(bool animate = false/*, [CallerMemberName] string cmn=null*/)
		{
			if(_satellite == null) return;
			//Print("hide", cmn, _satVisible);
			if(_satVisible) {
				_satVisible = false;
				_satTimer.Stop();
				_SatShowHide(false, animate);
			} else if(!animate && (_satAnimationTimer?.IsRunning ?? false)) {
				_SatShowHide(false, false);
			}
		}

		//Shows or hides _satellite and manages animation.
		void _SatShowHide(bool show, bool animate)
		{
			if(!animate || _satellite._transparency != default) {
				var w = _satellite._c.Hwnd();
				if(show != w.IsVisible) w.ShowLL(show);
				if(_satellite._transparency == default) w.SetTransparency(false);
				_satAnimationTimer?.Stop();
				_satAnimation = 0;
				return;
			}

			_satAnimationTimer ??= new ATimer(_ => {
				var w = _satellite._c.Hwnd();
				_satAnimation += _satVisible ? 64 : -32;
				bool stop; if(_satVisible) { if(stop = _satAnimation >= 255) _satAnimation = 255; } else { if(stop = _satAnimation <= 0) _satAnimation = 0; }
				if(stop) {
					_satAnimationTimer.Stop();
					if(_satAnimation == 0) w.ShowLL(false);
				}
				if(_satellite._transparency == default) w.SetTransparency(!stop, _satAnimation);
			});
			_satAnimationTimer.Now();
			_satAnimationTimer.Every(30);

			if(show) _satellite._c.Hwnd().ShowLL(true);
		}

		void _SatFollow()
		{
			if(!_satVisible) return;
			if(!_satellite._ow.UpdateRect(out bool changed) || !changed) return;
			_satellite._FollowRect(onFollowOwner: true);
		}

		#endregion
	}
}
