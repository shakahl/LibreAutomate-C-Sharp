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
//using System.Linq;

using Au.Types;

namespace Au
{
	/// <summary>
	/// Popup menu based on <see cref="ContextMenuStrip"/>. Can be used everywhere, not only in forms.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// var m = new AMenu();
	/// m["One"] = o => AOutput.Write(o);
	/// m["Two", icon: AFolders.System + "shell32.dll,15"] = o => { AOutput.Write(o); ADialog.Show(o.ToString()); };
	/// m.LastItem.ToolTipText = "tooltip";
	/// using(m.Submenu("Submenu")) {
	/// 	m["Three"] = o => AOutput.Write(o);
	/// 	m["Four"] = o => AOutput.Write(o);
	/// }
	/// m.ExtractIconPathFromCode = true;
	/// m["notepad"] = o => AExec.TryRun(AFolders.System + "notepad.exe");
	/// m.Show();
	/// ]]></code>
	/// </example>
	public partial class AMenu : MTBase, IDisposable
	{
		//The main wrapped object. The class is derived from ContextMenuStrip.
		_ContextMenuStrip _c;
		string _name;

		/// <summary>
		/// Gets the main drop-down menu <see cref="ContextMenuStrip"/> control.
		/// You can use all its properties, methods and events. You can assign it to a control or toolstrip's drop-down button etc.
		/// </summary>
		public ContextMenuStrip Control => _c;

		private protected override ToolStrip MainToolStrip => _c;

		/// <summary>
		/// Initializes this object.
		/// Use this overload with non-user-editable menus.
		/// </summary>
		public AMenu() : base(null, 0)
		{
			_c = new _ContextMenuStrip(this, isMain: true);
		}

		/// <summary>
		/// Initializes this object.
		/// Use this overload with user-editable menus.
		/// </summary>
		/// <param name="name">Menu name. Must be valid filename. Currently used only as the initial <b>Name</b> and <b>Text</b> of <see cref="Control"/>.</param>
		/// <param name="f"><see cref="CallerFilePathAttribute"/></param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		public AMenu(string name, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0) : base(f, l)
		{
			if(name.NE()) throw new ArgumentException("Empty name");
			_name = name;

			_c = new _ContextMenuStrip(this, isMain: true);
		}

		//~AMenu() { AOutput.Write("main dtor"); }

		///
		public void Dispose()
		{
			if(!_c.IsDisposed) _c.Dispose();
			//AOutput.Write(_c.Items.Count); //0
			//tested: ContextMenuStrip disposes its items but not their ToolStripDropDownMenu. And not their Image, therefore base._Dispose disposes images.
			if(_submenusToDispose != null) {
				foreach(var dd in _submenusToDispose) {
					Debug.Assert(!dd.IsDisposed);
					if(!dd.IsDisposed) dd.Dispose();
				}
				_submenusToDispose = null;
			}
			base._Dispose(true);
		}

		///
		public bool IsDisposed => _c.IsDisposed;

		void _CheckDisposed()
		{
			if(IsDisposed) throw new ObjectDisposedException(nameof(AMenu), "Disposed. Use MultiShow=true.");
		}

		#region add

		/*
		CONSIDER:
		Specialized AddX methods.
		
		For example, instead of
		m["Label"] = o => AExec.TryRun("notepad.exe");
		can use
		m.Run("notepad.exe", "label"); //label is optional
		Then can auto-get icon without disassembling the callback.

		Another example: instead of
		m["Label"] = o => AClipboard.Paste("notepad.exe");
		can use
		m.Paste("notepad.exe", "label"); //label is optional

		Can use extension methods for this. Example:
		public static void Run(this AMenu m, string path, string label = null)
		{
			m[label ?? path, path] = o => AExec.TryRun(path);
		}

		*/

		/// <summary>
		/// Adds new item.
		/// The same as <see cref="Add(string, Action{MTClickArgs}, MTImage, int)"/>.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => AOutput.Write(o);
		/// m["Two", @"icon file path"] = o => { AOutput.Write(o); ADialog.Show(o.ToString()); };
		/// m.LastItem.ToolTipText = "tooltip";
		/// m["Three"] = o => { AOutput.Write(o.MenuItem.Checked); };
		/// m.LastMenuItem.Checked = true;
		/// m.ExtractIconPathFromCode = true;
		/// m["notepad"] = o => AExec.TryRun(AFolders.System + "notepad.exe");
		/// m.Show();
		/// ]]></code>
		/// </example>
		public Action<MTClickArgs> this[string text, MTImage icon = default, [CallerLineNumber] int l = 0] {
			set { Add(text, value, icon, l); }
		}

		/// <summary>
		/// Adds new item as <see cref="ToolStripMenuItem"/>.
		/// </summary>
		/// <param name="text">Text. If contains a tab character, like "Open\tCtrl+O", displays text after it as shortcut keys (right-aligned).</param>
		/// <param name="onClick">Callback function. Called when clicked the menu item.</param>
		/// <param name="icon"></param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		/// <remarks>
		/// Sets menu item text, icon and <b>Click</b> event handler. Other properties can be specified later. See example.
		/// 
		/// Code <c>m.Add("text", o => AOutput.Write(o));</c> is the same as <c>m["text"] = o => AOutput.Write(o);</c>. See <see cref="this[string, MTImage, int]"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m.Add("One", o => AOutput.Write(o), icon: AFolders.System + "shell32.dll,9");
		/// var mi = m.Add("Two", o => { AOutput.Write(o.MenuItem.Checked); ADialog.Show(o.ToString()); });
		/// mi.Checked = true;
		/// m.ExtractIconPathFromCode = true;
		/// m.Add("notepad", o => AExec.TryRun(AFolders.System + "notepad.exe"));
		/// m.Show();
		/// ]]></code>
		/// </example>
		public ToolStripMenuItem Add(string text, Action<MTClickArgs> onClick, MTImage icon = default, [CallerLineNumber] int l = 0)
		{
			string sk = null;
			if(!text.NE()) {
				int i = text.IndexOf('\t');
				if(i >= 0) { sk = text.Substring(i + 1); text = text.Remove(i); }
			}

			var item = new ToolStripMenuItem(text);

			if(sk != null) item.ShortcutKeyDisplayString = sk;

			_Add(false, item, icon, onClick, l);
			return item;
		}

		/// <summary>
		/// Adds item of any <b>ToolStripItem</b>-based type, for example <b>ToolStripLabel</b>, <b>ToolStripTextBox</b>, <b>ToolStripComboBox</b>.
		/// </summary>
		/// <param name="item">An already created item of any supported type.</param>
		/// <param name="icon"></param>
		/// <param name="onClick">Callback function. Called when the item clicked. Not useful with most item types.</param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		public void Add(ToolStripItem item, MTImage icon = default, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
			=> _Add(false, item, icon, onClick, l);

		void _Add(bool isSub, ToolStripItem item, MTImage icon, Action<MTClickArgs> onClick, int l)
		{
			var dd = CurrentAddMenu;
			dd.SuspendLayout(); //makes adding items much faster. It's OK to suspend/resume when already suspended; .NET uses a layoutSuspendCount.
			dd.Items.Add(item);
			_SetItemProp(false, isSub, item, onClick, icon, l);
			_OnItemAdded(item);
			dd.ResumeLayout(false);
		}

		/// <summary>
		/// Adds separator.
		/// </summary>
		public ToolStripSeparator Separator()
		{
			var item = new ToolStripSeparator();
			_Add(false, item, default, null, 0);
			return item;
		}

		/// <summary>
		/// Adds new item (<see cref="ToolStripMenuItem"/>) that will open a submenu.
		/// Then the add-item functions will add items to the submenu.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon"></param>
		/// <param name="onClick">Callback function. Called when the item clicked. Rarely used.</param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		/// <remarks>
		/// Can be used in 2 ways:
		/// - <c>using(m.Submenu(...)) { add items; }</c>. See example.
		/// - <c>m.Submenu(...); add items; m.EndSubmenu();</c>. See <see cref="EndSubmenu"/>.
		/// 
		/// Submenus inherit these properties of the main menu, set before adding submenus (see example):
		/// <b>BackgroundImage</b>, <b>BackgroundImageLayout</b>, <b>Cursor</b>, <b>Font</b>, <b>ForeColor</b>, <b>ImageList</b>, <b>ImageScalingSize</b>, <b>Renderer</b>, <b>ShowCheckMargin</b>, <b>ShowImageMargin</b>.
		/// </remarks>
		/// <seealso cref="LazySubmenu"/>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m.Control.BackColor = Color.PaleGoldenrod;
		/// m["One"] = o => AOutput.Write(o);
		/// m["Two"] = o => AOutput.Write(o);
		/// using(m.Submenu("Submenu")) {
		/// 	m["Three"] = o => AOutput.Write(o);
		/// 	m["Four"] = o => AOutput.Write(o);
		/// 	using(m.Submenu("Submenu2")) {
		/// 		m["Five"] = o => AOutput.Write(o);
		/// 		m["Six"] = o => AOutput.Write(o);
		/// 	}
		/// 	m["Seven"] = o => AOutput.Write(o);
		/// }
		/// m["Eight"] = o => AOutput.Write(o);
		/// m.Show();
		/// ]]></code>
		/// </example>
		public UsingAction Submenu(string text, MTImage icon = default, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			var item = _Submenu(out var dd, text, onClick, icon, l);
			_submenuStack.Push(dd);
			return new UsingAction(() => EndSubmenu());
		}

		ToolStripMenuItem _Submenu(out _ContextMenuStrip dd, string text, Action<MTClickArgs> onClick, MTImage icon, int sourceLine)
		{
			var item = new ToolStripMenuItem(text);

			dd = new _ContextMenuStrip(this, isMain: false);
			//var t = APerf.Create();
			dd.SuspendLayout();
			if(_c._changedBackColor) dd.BackColor = _c.BackColor; //because by default gives 0xf0f0f0, although actually white, and then submenus would be gray
			dd.BackgroundImage = _c.BackgroundImage;
			dd.BackgroundImageLayout = _c.BackgroundImageLayout;
			dd.Cursor = _c.Cursor;
			dd.Font = _c.Font;
			if(_c._changedForeColor) dd.ForeColor = _c.ForeColor;
			dd.ImageList = _c.ImageList;
			dd.ImageScalingSize = _c.ImageScalingSize;
			dd.Renderer = _c.Renderer;
			dd.ShowCheckMargin = _c.ShowCheckMargin;
			dd.ShowImageMargin = _c.ShowImageMargin;
			dd.ResumeLayout(false);
			//t.NW();
			item.DropDown = dd;
			//never mind: should be 'dd=item.DropDown' (auto-created).
			//	Because now eg hotkeys don't work.
			//	But then cannot be ToolStripDropDownMenu_.
			//	It is important only if using in menu bar.

			_Add(true, item, icon, onClick, sourceLine);

			//Workaround for ToolStripDropDown bug: sometimes does not show 3-rd level submenu.
			//	It happens when the mouse moves from the 1-level menu to the 2-nd level submenu-item over an unrelated item of 1-st level menu.
			if(_AddingSubmenuItems) item.MouseHover += (sender, e) => {
				var k = sender as ToolStripMenuItem;
				if(k.HasDropDownItems && !k.DropDown.Visible) k.ShowDropDown();

				//Note: submenus are shown not on hover.
				//The hover time is SPI_GETMOUSEHOVERTIME, and submenu delay is SPI_GETMENUSHOWDELAY.
				//This usually works well because both times are equal, 400 ms.
				//If they are different, it is not important, because don't need and we don't use this workaround for direct submenus. Indirect submenus are rarely used.
			};

			_submenusToDispose ??= new List<_ContextMenuStrip>();
			_submenusToDispose.Add(dd);

			return item;
		}

		List<_ContextMenuStrip> _submenusToDispose;

		/// <summary>
		/// Call this to end adding items to the current submenu if <see cref="Submenu"/> was called without 'using' and without a callback function that adds submenu items.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => AOutput.Write(o);
		/// m["Two"] = o => AOutput.Write(o);
		/// m.Submenu("Submenu");
		/// 	m["Three"] = o => AOutput.Write(o);
		/// 	m["Four"] = o => AOutput.Write(o);
		/// 	m.EndSubmenu();
		/// m["Five"] = o => AOutput.Write(o);
		/// m.Show();
		/// ]]></code>
		/// </example>
		public void EndSubmenu() => _submenuStack.Pop();

		/// <summary>
		/// Adds new item (<see cref="ToolStripMenuItem"/>) that will open a submenu.
		/// When showing the submenu first time, your callback function will be called and can add submenu items.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onOpening">Callback function that should add submenu items.</param>
		/// <param name="icon"></param>
		/// <param name="onClick">Callback function. Called when the item clicked. Rarely used.</param>
		/// <param name="l"><see cref="CallerLineNumberAttribute"/></param>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => AOutput.Write(o);
		/// m["Two"] = o => AOutput.Write(o);
		/// m.LazySubmenu("Submenu", _ => {
		/// 	AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 	m["Three"] = o => AOutput.Write(o);
		/// 	m["Four"] = o => AOutput.Write(o);
		/// 	m.LazySubmenu("Submenu2", _ => {
		/// 		AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 		m["Five"] = o => AOutput.Write(o);
		/// 		m["Six"] = o => AOutput.Write(o);
		/// 	});
		/// 	m["Seven"] = o => AOutput.Write(o);
		/// });
		/// m["Eight"] = o => AOutput.Write(o);
		/// m.Show();
		/// ]]></code>
		/// </example>
		public ToolStripMenuItem LazySubmenu(string text, Action<AMenu> onOpening, MTImage icon = default, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			var item = _Submenu(out var dd, text, onClick, icon, l);
			dd._submenu_lazyDelegate = onOpening;

			//add one item, or it will not work like a submenu parent item
			dd.SuspendLayout();
			dd.Items.Add(new ToolStripSeparator());
			dd.ResumeLayout(false);

			return item;
		}

		Stack<ToolStripDropDownMenu> _submenuStack = new Stack<ToolStripDropDownMenu>();

		bool _AddingSubmenuItems => _submenuStack.Count > 0;

		/// <summary>
		/// Gets <see cref="ToolStripDropDownMenu"/> of the main menu or submenu where new items currently would be added.
		/// </summary>
		public ToolStripDropDownMenu CurrentAddMenu {
			get {
				_CheckDisposed(); //used by all Add(), Submenu()
				return _submenuStack.Count > 0 ? _submenuStack.Peek() : _c;
			}
		}

		/// <summary>
		/// Gets the last added item as <see cref="ToolStripMenuItem"/>.
		/// Returns null if it is not a <b>ToolStripMenuItem</b>.
		/// </summary>
		/// <remarks>
		/// You can instead use <see cref="MTBase.LastItem"/>, which gets <see cref="ToolStripItem"/>, the base class of all supported item types; cast it to a derived type if need.
		/// </remarks>
		public ToolStripMenuItem LastMenuItem => LastItem as ToolStripMenuItem;

		#endregion

		#region show

		/// <summary>
		/// Shows the menu at the mouse cursor position.
		/// </summary>
		/// <param name="byCaret">Show at the text cursor (caret) position, if available.</param>
		public void Show(bool byCaret = false)
		{
			_Show(byCaret ? 4 : 1);
		}

		/// <summary>
		/// Shows the menu at the specified position.
		/// </summary>
		/// <param name="x">X position in screen.</param>
		/// <param name="y">Y position in screen.</param>
		/// <param name="direction">Menu drop direction.</param>
		public void Show(int x, int y, ToolStripDropDownDirection direction = ToolStripDropDownDirection.Default)
		{
			//CONSIDER: Coord, screen. Maybe as new overload.
			_Show(2, x, y, direction);
		}

		/// <summary>
		/// Shows the menu on a form or control.
		/// </summary>
		/// <param name="owner">A control or form that will own the menu.</param>
		/// <param name="x">X position in control's client area.</param>
		/// <param name="y">Y position in control's client area.</param>
		/// <param name="direction">Menu drop direction.</param>
		/// <remarks>
		/// Alternatively you can assign the context menu to a control or toolstrip's drop-down button etc, then don't need to call <b>Show</b>. Use the <see cref="Control"/> property, which gets <see cref="ContextMenuStrip"/>.
		/// </remarks>
		public void Show(Control owner, int x, int y, ToolStripDropDownDirection direction = ToolStripDropDownDirection.Default)
		{
			_Show(3, x, y, direction, owner);
		}

		/// <summary>
		/// Shows the menu at the mouse cursor position.
		/// </summary>
		/// <param name="owner">A control or form that will own the menu.</param>
		/// <param name="direction">Menu drop direction.</param>
		/// <remarks>
		/// Alternatively you can assign the context menu to a control or toolstrip's drop-down button etc, then don't need to call <b>Show</b>. Use the <see cref="Control"/> property, which gets <see cref="ContextMenuStrip"/>.
		/// </remarks>
		public void Show(Control owner, ToolStripDropDownDirection direction = ToolStripDropDownDirection.Default)
		{
			var p = owner.MouseClientXY();
			_Show(3, p.x, p.y, direction, owner);
		}

		void _Show(int overload, int x = 0, int y = 0, ToolStripDropDownDirection direction = 0, Control control = null)
		{
			_CheckDisposed();
			if(_c.Items.Count == 0) return;

			_isModal = Modal ?? !AThread.HasMessageLoop();

			_inOurShow = true;
			switch(overload) {
			case 1: _c.Show(AMouse.XY); break;
			case 2: _c.Show(new Point(x, y), direction); break;
			case 3: _c.Show(control, new Point(x, y), direction); break;
			case 4:
				AKeys.More.GetTextCursorRect(out RECT cr, out _, orMouse: true);
				_c.Show(new Point(cr.left - 32, cr.bottom + 2));
				break;
			}
			_inOurShow = false;

			if(_isModal) {
				_msgLoop.Loop();
				if(!MultiShow) Dispose();
			}
		}

		void _OnOpeningMain()
		{
			//Support showing not through our Show, for example when assigned to a control or toolstrip's drop-down button.
			if(!_inOurShow) {
				_isModal = false;
				MultiShow = true; //programmers would forget it
			}

			if(!_showedOnce) {
				_showedOnce = true;
				_c.PerformLayout();
			}

			GetIconsAsync_(_c);
		}

		bool _showedOnce;
		bool _inOurShow; //to detect when ContextMenuStrip.Show called not through AMenu.Show
		bool _isModal; //depends on ModalAlways or Application.MessageLoop
		Util.AMessageLoop _msgLoop = new Util.AMessageLoop();

		/// <summary>
		/// If false, disposes the menu when it is closed.
		/// If true, does not dispose. Then you can call <b>Show</b> again and again.
		/// Default is false, but is automatically set to true when showing the menu not with <b>Show</b>, eg when assigned to a control's <b>ContextMenuStrip</b>.
		/// </summary>
		/// <seealso cref="DefaultMultiShow"/>
		public bool MultiShow { get; set; } = DefaultMultiShow;

		/// <summary>
		/// Default <see cref="MultiShow"/> value for new <b>AMenu</b> instances.
		/// </summary>
		public static bool DefaultMultiShow { get; set; }

		/// <summary>
		/// If true, <b>Show</b> waits until the menu is closed. If false, does not wait (this thread must have a message loop).
		/// If null (default), does not wait if the thread has a .NET message loop (<see cref="AThread.HasMessageLoop"/>).
		/// </summary>
		/// <remarks>
		/// <note>If false, this thread must have a message loop, either already running or started soon after showing the menu. Without it the menu will not work.</note>
		/// </remarks>
		public bool? Modal { get; set; }

		/// <summary>
		/// Activate the menu window.
		/// It enables selecting menu items with the keyboard (arrows, Tab, Enter, etc).
		/// If false, only Esc works, it closes the menu.
		/// If the menu is owned by a control or toolbar button, keyboard navigation works in any case, don't need this property to enable it.
		/// </summary>
		public bool ActivateMenuWindow { get; set; }// = DefaultActivateMenuWindow;

		//rejected
		///// <summary>
		///// Default <see cref="ActivateMenuWindow"/> value for new <b>AMenu</b> instances.
		///// </summary>
		//public static bool DefaultActivateMenuWindow { get; set; }
		///// <seealso cref="DefaultActivateMenuWindow"/>

		WS2 _ExStyle => ActivateMenuWindow ? WS2.TOOLWINDOW | WS2.TOPMOST : WS2.TOOLWINDOW | WS2.TOPMOST | WS2.NOACTIVATE;
		//WS2.NOACTIVATE is a workaround for 2 .NET bugs:
		//1. In inactive thread the menu window may have a taskbar button.
		//2. In inactive thread, when clicked a menu item, temporarily activates some window of this thread.

		#endregion

		#region close

		/// <summary>
		/// Close the menu when the mouse cursor moves away from it to this distance, pixels. Only if the toolbar's thread is not the active UI thread.
		/// </summary>
		/// <remarks>
		/// At first the mouse must be or move at less than half of the distance.
		/// Default is <see cref="DefaultMouseClosingDistance"/>, default <c>Au.Util.ADpi.ScaleInt(200)</c>.
		/// </remarks>
		/// <seealso cref="DefaultMouseClosingDistance"/>
		public int MouseClosingDistance { get; set; } = DefaultMouseClosingDistance;

		/// <summary>
		/// Default <see cref="MouseClosingDistance"/> value of <b>AMenu</b> instances.
		/// </summary>
		public static int DefaultMouseClosingDistance { get; set; } = Util.ADpi.ScaleInt(200);

		const int c_wmCloseWparam = 827549482;
		List<ToolStripDropDown> _closing_allMenus = new List<ToolStripDropDown>(); //all menu windows, including hidden submenus
		ToolStripDropDown _closing_lastVisibleMenu;
		ATimer _closing_timer;
		bool _closing_escKey;
		byte _closing_mouseState;
		bool _closing_mouseWasIn;
		bool _closing_classicMenu;
		bool _closing;

		void _OnOpened(ToolStripDropDown m, bool isMain)
		{
			if(isMain) {
				if(Api.GetFocus() == default) {
					_closing_timer = ATimer.Every(100, _ClosingTimer);
					_closing_escKey = AKeys.UI.IsToggled(KKey.Escape);
					_closing_mouseState = _GetMouseState();
					_closing_mouseWasIn = _closing_classicMenu = false;
				}
			}
			_closing_lastVisibleMenu = m;
			if(isMain && ActivateMenuWindow) m.Hwnd().ActivateLL();
		}

		void _OnClosed(ToolStripDropDown m, bool isMain)
		{
			_closing_lastVisibleMenu = m.OwnerItem?.Owner as ToolStripDropDown;
			if(isMain) {
				_closing_timer?.Stop(); _closing_timer = null;

				//Close menu windows. Else they are just hidden and prevent GC until process ends.
				foreach(var k in _closing_allMenus) ((AWnd)k.Handle).Post(Api.WM_CLOSE, c_wmCloseWparam);

				if(!MultiShow && !_isModal) ATimer.After(10, _ => Dispose()); //cannot dispose now, exception

				if(_isModal) _msgLoop.Stop();
			}
		}

		//100 ms
		void _ClosingTimer(ATimer timer)
		{
			Debug.Assert(!IsDisposed);
			if(IsDisposed) { timer.Stop(); return; }

			bool escState = AKeys.UI.IsToggled(KKey.Escape);
			byte mouseState = _GetMouseState();
			bool escPressed = escState != _closing_escKey, mouseClicked = mouseState != _closing_mouseState;
			_closing_escKey = escState;
			_closing_mouseState = mouseState;

			if(Api.GetFocus() != default) return;
			var closeReason = ToolStripDropDownCloseReason.AppClicked;

			//is/was classic context menu active?
			bool wasClassicMenu = _closing_classicMenu;
			_closing_classicMenu = AWnd.More.GetGUIThreadInfo(out var g, AThread.NativeId) && !g.hwndMenuOwner.Is0;
			if(wasClassicMenu && !_closing_classicMenu) return;

			//close if Esc key
			if(escPressed) {
				closeReason = ToolStripDropDownCloseReason.Keyboard;
				goto g1;
			}

			//close if mouse clicked an alien window
			if(mouseClicked) {
				//AOutput.Write(_IsMenuWindow());
				if(!AWnd.FromMouse(WXYFlags.NeedWindow).IsOfThisThread) {
					goto g1;
				}
			}

			//close if mouse is far
			if(_closing_classicMenu) return;
			int dist = MouseClosingDistance;
			POINT p = AMouse.XY;
			for(var v = _closing_lastVisibleMenu; v != null; v = v.OwnerItem?.Owner as ToolStripDropDown) {
				RECT r = v.Bounds;
				if(!_closing_mouseWasIn) {
					int half = dist / 2;
					r.Inflate(half, half);
					_closing_mouseWasIn = r.Contains(p);
					r.Inflate(-half, -half);
				}
				r.Inflate(dist, dist);
				if(r.Contains(p)) return;
			}

			if(!_closing_mouseWasIn) return;
			g1:
			if(_closing_classicMenu) {
				Api.EndMenu();
				if(closeReason == ToolStripDropDownCloseReason.Keyboard) return;
			}
			_Close(closeReason);
		}

		byte _GetMouseState() => (byte)(
			(AKeys.UI.GetKeyState(KKey.MouseLeft) & 1)
			| ((AKeys.UI.GetKeyState(KKey.MouseRight) & 1) << 1)
			| ((AKeys.UI.GetKeyState(KKey.MouseMiddle) & 1) << 2)
			);

		//bool _IsMenuWindow()
		//{
		//	var w = AWnd.FromMouse(WXYFlags.NeedWindow);
		//	if(w.IsOfThisThread) {
		//		if(w == _c.Hwnd()) return true;
		//		foreach(var v in _visibleSubmenus) if(w == v.Hwnd()) return true;
		//		//if()
		//	}
		//	return false;
		//}

		void _Close(ToolStripDropDownCloseReason reason = ToolStripDropDownCloseReason.CloseCalled)
		{
			if(IsDisposed) return;

			for(var v = _closing_lastVisibleMenu; v != null;) {
				var v2 = v.OwnerItem?.Owner as ToolStripDropDown;
				_closing = true;
				v.Close(reason);
				_closing = false;
				if(reason == ToolStripDropDownCloseReason.Keyboard) break;
				v = v2;
			}
		}

		/// <summary>
		/// Closes the menu and its submenus.
		/// </summary>
		public void Close()
		{
			_Close();
		}

		#endregion
	}
}
