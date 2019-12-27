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
//using System.Linq;

using Au.Types;
using static Au.AStatic;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Popup menu based on <see cref="ContextMenuStrip"/>. Can be used everywhere, not only in forms.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// var m = new AMenu();
	/// m["One"] = o => Print(o);
	/// m["Two", icon: AFolders.System + "shell32.dll,15"] = o => { Print(o); ADialog.Show(o.ToString()); };
	/// m.LastItem.ToolTipText = "tooltip";
	/// using(m.Submenu("Submenu")) {
	/// 	m["Three"] = o => Print(o);
	/// 	m["Four"] = o => Print(o);
	/// }
	/// m.ExtractIconPathFromCode = true;
	/// m["notepad"] = o => AExec.TryRun(AFolders.System + "notepad.exe");
	/// m.Show();
	/// ]]></code>
	/// </example>
	public class AMenu : AMTBase, IDisposable
	{
		//The main wrapped object. The class is derived from ContextMenuStrip.
		_ContextMenuStrip _c;
		string _name;

		/// <summary>
		/// Gets the main drop-down menu <see cref="ContextMenuStrip"/> control.
		/// You can use all its properties, methods and events. You can assign it to a control or toolstrip's drop-down button etc.
		/// </summary>
		public ContextMenuStrip Control => _c;

		private protected override ToolStrip MainToolStrip => _c; //used by AMTBase

		///
		public AMenu() : base(null, 0)
		{
			_c = new _ContextMenuStrip(this);
		}

		///
		public AMenu(string name, [CallerFilePath] string f = null, [CallerLineNumber] int l = 0) : base(f, l)
		{
			if(Empty(name)) throw new ArgumentException("Empty name");
			_name = name;

			_c = new _ContextMenuStrip(this);
		}

		//~AMenu() { Print("main dtor"); }

		///
		public void Dispose()
		{
			if(!_c.IsDisposed) _c.Dispose();
			//Print(_c.Items.Count); //0
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
		m["Label"] = o => Paste("notepad.exe");
		can use
		m.Paste("notepad.exe", "label"); //label is optional
		And the same for Key.

		Can even use extension methods for this. Example:
		public static void Run(this AMenu m, string path, string label = null)
		{
			m[label ?? path, path] = o => AExec.TryRun(path);
		}

		*/

		/// <summary>
		/// Adds new item.
		/// The same as <see cref="Add(string, Action{MTClickArgs}, object, int)"/>.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => Print(o);
		/// m["Two", @"icon file path"] = o => { Print(o); ADialog.Show(o.ToString()); };
		/// m.LastItem.ToolTipText = "tooltip";
		/// m["Three"] = o => { Print(o.MenuItem.Checked); };
		/// m.LastMenuItem.Checked = true;
		/// m.ExtractIconPathFromCode = true;
		/// m["notepad"] = o => AExec.TryRun(AFolders.System + "notepad.exe");
		/// m.Show();
		/// ]]></code>
		/// </example>
		public Action<MTClickArgs> this[string text, object icon = null, [CallerLineNumber] int l = 0] {
			set { Add(text, value, icon, l); }
		}

		/// <summary>
		/// Adds new item as <see cref="ToolStripMenuItem"/>.
		/// </summary>
		/// <param name="text">Text. If contains a tab character, like "Open\tCtrl+O", displays text after it as shortcut keys (right-aligned).</param>
		/// <param name="onClick">Callback function. Called when clicked the menu item.</param>
		/// <param name="icon">Can be:
		/// - string - path of .ico or any other file or folder or non-file object. See <see cref="AIcon.GetFileIcon"/>. If not full path, searches in <see cref="AFolders.ThisAppImages"/>; see also <see cref="AMTBase.IconFlags"/>.
		/// - string - image name (key) in the ImageList (<see cref="ToolStripItem.ImageKey"/>).
		/// - int - image index in the ImageList (<see cref="ToolStripItem.ImageIndex"/>).
		/// - Icon, Image, FolderPath.
		/// - null (default) - no icon. If <see cref="AMTBase.ExtractIconPathFromCode"/> == true, extracts icon path from <i>onClick</i> code like <c>AExec.TryRun(@"c:\path\file.exe")</c> or <c>AExec.TryRun(AFolders.System + "file.exe")</c>.
		/// - "" - no icon.
		/// </param>
		/// <param name="l"></param>
		/// <remarks>
		/// Sets menu item text, icon and <b>Click</b> event handler. Other properties can be specified later. See example.
		/// 
		/// Code <c>m.Add("text", o => Print(o));</c> is the same as <c>m["text"] = o => Print(o);</c>. See <see cref="this[string, object, int]"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m.Add("One", o => Print(o), icon: AFolders.System + "shell32.dll,9");
		/// var mi = m.Add("Two", o => { Print(o.MenuItem.Checked); ADialog.Show(o.ToString()); });
		/// mi.Checked = true;
		/// m.ExtractIconPathFromCode = true;
		/// m.Add("notepad", o => AExec.TryRun(AFolders.System + "notepad.exe"));
		/// m.Show();
		/// ]]></code>
		/// </example>
		public ToolStripMenuItem Add(string text, Action<MTClickArgs> onClick, object icon = null, [CallerLineNumber] int l = 0)
		{
			string sk = null;
			if(!Empty(text)) {
				int i = text.IndexOf('\t');
				if(i >= 0) { sk = text.Substring(i + 1); text = text.Remove(i); }
			}

			var item = new ToolStripMenuItem(text);

			if(sk != null) item.ShortcutKeyDisplayString = sk;

			_Add(item, onClick, icon, l);
			return item;
		}

		void _Add(ToolStripItem item, Action<MTClickArgs> onClick, object icon, int sourceLine)
		{
			var dd = CurrentAddMenu;
			dd.SuspendLayout(); //makes adding items much faster. It's OK to suspend/resume when already suspended; .NET uses a layoutSuspendCount.
			dd.Items.Add(item);
			_SetItemProp(false, item, onClick, icon, sourceLine);
			dd.ResumeLayout(false);
		}

		/// <summary>
		/// Adds item of any supported type, for example ToolStripLabel, ToolStripTextBox, ToolStripComboBox, ToolStripProgressBar, ToolStripButton.
		/// Supports types derived from ToolStripItem.
		/// </summary>
		/// <param name="item">An already created item of any supported type.</param>
		/// <param name="icon"></param>
		/// <param name="onClick">Callback function. Called when the item clicked. Not useful with most item types.</param>
		/// <param name="l"></param>
		public void Add(ToolStripItem item, object icon = null, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			_Add(item, onClick, icon, l);

			//Activate window when a child control clicked, or something may not work, eg cannot enter text in Edit control.
			if(item is ToolStripControlHost cb) //combo, edit, progress
				cb.GotFocus += (sender, e) => { //info: before MouseDown, which does not work well with combo box
					if(!(_isOwned || ActivateMenuWindow)) {
						var t = sender as ToolStripItem;

						//Activate the clicked menu or submenu window to allow eg to enter text in text box.
						var w = (AWnd)t.Owner.Handle;
						Api.SetForegroundWindow(w); //does not fail, probably after a mouse click this process is allowed to activate windows, even if the click did not activate because of the window style

						//see also: both OnClosing
					}
				};
		}

		/// <summary>
		/// Adds separator.
		/// </summary>
		public ToolStripSeparator Separator()
		{
			var item = new ToolStripSeparator();
			_Add(item, null, null, 0);
			return item;
		}

		/// <summary>
		/// Adds new item (<see cref="ToolStripMenuItem"/>) that will open a submenu.
		/// Then the add-item functions will add items to the submenu.
		/// Can be used in 2 ways:
		/// 1. <c>using(m.Submenu(...)) { add items; }</c>. See example.
		/// 2. <c>m.Submenu(...); add items; m.EndSubmenu();</c>. See <see cref="EndSubmenu"/>.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">See <see cref="Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="onClick">Callback function. Called when the item clicked. Rarely used.</param>
		/// <param name="l"></param>
		/// <remarks>
		/// Submenus inherit these properties of the main menu, set before adding submenus (see example):
		/// <b>BackgroundImage</b>, <b>BackgroundImageLayout</b>, <b>Cursor</b>, <b>Font</b>, <b>ForeColor</b>, <b>ImageList</b>, <b>ImageScalingSize</b>, <b>Renderer</b>, <b>ShowCheckMargin</b>, <b>ShowImageMargin</b>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m.Control.BackColor = Color.PaleGoldenrod;
		/// m["One"] = o => Print(o);
		/// m["Two"] = o => Print(o);
		/// using(m.Submenu("Submenu")) {
		/// 	m["Three"] = o => Print(o);
		/// 	m["Four"] = o => Print(o);
		/// 	using(m.Submenu("Submenu")) {
		/// 		m["Five"] = o => Print(o);
		/// 		m["Six"] = o => Print(o);
		/// 	}
		/// 	m["Seven"] = o => Print(o);
		/// }
		/// m["Eight"] = o => Print(o);
		/// m.Show();
		/// ]]></code>
		/// </example>
		public MUsingSubmenu Submenu(string text, object icon = null, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			var item = _Submenu(out var dd, text, onClick, icon, l);
			_submenuStack.Push(dd);
			return new MUsingSubmenu(this, item);
		}

		ToolStripMenuItem _Submenu(out ToolStripDropDownMenu_ dd, string text, Action<MTClickArgs> onClick, object icon, int sourceLine)
		{
			var item = new ToolStripMenuItem(text);
			_Add(item, onClick, icon, sourceLine);

			dd = new ToolStripDropDownMenu_(this);
			item.DropDown = dd;
			//SHOULDDO: should be 'dd=item.DropDown' (auto-created).
			//	Because now eg hotkeys don't work.
			//	But then cannot be ToolStripDropDownMenu_.
			//	It is important only if using in menu bar.

			//var t = new APerf.Inst(); t.First();
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

			_submenusToDispose ??= new List<ToolStripDropDownMenu_>();
			_submenusToDispose.Add(dd);

			return item;
		}

		List<ToolStripDropDownMenu_> _submenusToDispose;

		/// <summary>
		/// Call this to end adding items to the current submenu if <see cref="Submenu"/> was called without 'using' and without a callback function that adds submenu items.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => Print(o);
		/// m["Two"] = o => Print(o);
		/// m.Submenu("Submenu");
		/// 	m["Three"] = o => Print(o);
		/// 	m["Four"] = o => Print(o);
		/// 	m.EndSubmenu();
		/// m["Five"] = o => Print(o);
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
		/// <param name="icon">See <see cref="Add(string, Action{MTClickArgs}, object, int)"/>.</param>
		/// <param name="onClick">Callback function. Called when the item clicked. Rarely used.</param>
		/// <param name="l"></param>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => Print(o);
		/// m["Two"] = o => Print(o);
		/// m.Submenu("Submenu 1", _ => {
		/// 	Print("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 	m["Three"] = o => Print(o);
		/// 	m["Four"] = o => Print(o);
		/// 	m.Submenu("Submenu 2", _ => {
		/// 		Print("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 		m["Five"] = o => Print(o);
		/// 		m["Six"] = o => Print(o);
		/// 	});
		/// 	m["Seven"] = o => Print(o);
		/// });
		/// m["Eight"] = o => Print(o);
		/// m.Show();
		/// ]]></code>
		/// </example>
		public ToolStripMenuItem Submenu(string text, Action<AMenu> onOpening, object icon = null, Action<MTClickArgs> onClick = null, [CallerLineNumber] int l = 0)
		{
			var item = _Submenu(out var dd, text, onClick, icon, l);
			dd._lazySubmenuDelegate = onOpening;

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
		/// You can instead use <see cref="AMTBase.LastItem"/>, which gets <see cref="ToolStripItem"/>, the base class of all supported item types; cast it to a derived type if need.
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

			if(!_showedOnce) {
				//_showedOnce = true; //OnOpening() sets it
				if(!ActivateMenuWindow) LibWorkarounds.WaitCursorWhenShowingMenuEtc();
			}
			//APerf.Next();

			_isOwned = control != null;
			_isModal = ModalAlways ? true : !AThread.HasMessageLoop();

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
			//APerf.Next();

			if(_isModal) {
				_msgLoop.Loop();
				if(!MultiShow) Dispose();
			}
		}

		bool _showedOnce;
		bool _inOurShow; //to detect when ContextMenuStrip.Show called not through AMenu.Show
		bool _isOwned; //control==0
		bool _isModal; //depends on ModalAlways or Application.MessageLoop
		AMessageLoop _msgLoop = new AMessageLoop();

		/// <summary>
		/// If false, disposes the menu when it is closed.
		/// If true, does not dispose. Then you can call <b>Show</b> again and again.
		/// Default is false, but is automatically set to true when showing the menu not with <b>Show</b>, eg when assigned to a control.
		/// </summary>
		/// <seealso cref="DefaultMultiShow"/>
		public bool MultiShow { get; set; } = DefaultMultiShow;
		//FUTURE: try, even if false, maybe it is possible to just destroy menu window and not fully dispose, so that the menu can be reshown.

		/// <summary>
		/// Default <see cref="MultiShow"/> value for new <b>AMenu</b> instances.
		/// </summary>
		public static bool DefaultMultiShow { get; set; }

		/// <summary>
		/// If true, <b>Show</b> always waits until the menu is closed.
		/// If false, does not wait if the thread has a .NET message loop (<see cref="AThread.HasMessageLoop"/>).
		/// </summary>
		public bool ModalAlways { get; set; }
		//note: don't allow to make non-modal when there is no message loop, because it can crash Windows if the programmer does not create a loop then, and it is not useful.

		/// <summary>
		/// Activate the menu window.
		/// It enables selecting menu items with the keyboard (arrows, Tab, Enter, etc).
		/// If false, only Esc works, it closes the menu.
		/// If the menu is owned by a control or toolbar button, keyboard navigation works in any case, don't need this property to enable it.
		/// </summary>
		/// <seealso cref="DefaultActivateMenuWindow"/>
		public bool ActivateMenuWindow { get; set; } = DefaultActivateMenuWindow;

		/// <summary>
		/// Default <see cref="ActivateMenuWindow"/> value for new <b>AMenu</b> instances.
		/// </summary>
		public static bool DefaultActivateMenuWindow { get; set; }

		WS_EX _ExStyle => ActivateMenuWindow ? WS_EX.TOOLWINDOW | WS_EX.TOPMOST : WS_EX.TOOLWINDOW | WS_EX.TOPMOST | WS_EX.NOACTIVATE;
		//WS_EX.NOACTIVATE is a workaround for 2 .NET bugs:
		//1. In inactive thread the menu window may have a taskbar button.
		//2. In inactive thread, when clicked a menu item, temporarily activates some window of this thread.

		#endregion

		//Extends ContextMenuStrip of the main menu to change its behavior as we need.
		class _ContextMenuStrip : ContextMenuStrip, _IAuToolStrip
		{
			AMenu _am;

			internal _ContextMenuStrip(AMenu am)
			{
				_am = am;
			}

			protected override CreateParams CreateParams {
				get {
					//note: this func is called several times, first time before ctor
					//Print("CreateParams", _cat, IsHandleCreated, p.ExStyle.ToString("X"));
					var p = base.CreateParams;
					if(_am != null) p.ExStyle |= (int)_am._ExStyle;
					return p;
				}
			}

			protected override void WndProc(ref Message m)
			{
				//AWnd.More.PrintMsg(m, Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_ERASEBKGND, Api.WM_CTLCOLOREDIT);

				if(_am._WndProc_Before(true, this, ref m)) return;
				//var t = APerf.Create();
				base.WndProc(ref m);
				//t.Next(); if(t.TimeTotal >= 100) { Print(t.ToString(), m); }
				_am._WndProc_After(true, this, ref m);
			}

			protected override void OnOpening(CancelEventArgs e)
			{
				if(!e.Cancel) _am._OnOpeningMain();
				base.OnOpening(e);

				//info: e.Cancel true if layout suspended. Then does not show, unless we set it false.
			}

			protected override void OnOpened(EventArgs e)
			{
				base.OnOpened(e);
				_am._OnOpenedAny(false, this);
			}

			protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
			{
				_am._OnClosingAny(false, this, e);
				base.OnClosing(e);
			}

			protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
			{
				base.OnClosed(e);
				_am._OnClosedAny(false, this);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
				_am._OnHandleCreatedDestroyed(true, this);
			}

			protected override void OnHandleDestroyed(EventArgs e)
			{
				base.OnHandleDestroyed(e);
				_am._OnHandleCreatedDestroyed(false, this);
			}

			protected override void Dispose(bool disposing)
			{
				if(disposing && Visible) Close(); //else OnClosed not called etc
				base.Dispose(disposing);
			}

			//~_ContextMenuStrip() => Print("dtor");

			protected override void OnPaint(PaintEventArgs e)
			{
				//var perf = APerf.Create();

				//ADebug.PrintFunc();
				base.OnPaint(e);

				//perf.Next(); Print("------------------ paint", perf.ToString());

				_paintedOnce = true;
			}

			//ToolStrip _IAuToolStrip.ToolStrip => this;

			bool _paintedOnce;
			bool _IAuToolStrip.PaintedOnce => _paintedOnce;

			protected override void OnBackColorChanged(EventArgs e)
			{
				_changedBackColor = true;
				base.OnBackColorChanged(e);
			}

			internal bool _changedBackColor, _changedForeColor;

			protected override void OnForeColorChanged(EventArgs e)
			{
				_changedForeColor = true;
				base.OnBackColorChanged(e);
			}
		}

		//Extends ToolStripDropDownMenu of a submenu to change its behavior as we need.
		internal class ToolStripDropDownMenu_ : ToolStripDropDownMenu, _IAuToolStrip
		{
			AMenu _am;
			bool _openedOnce;
			internal Action<AMenu> _lazySubmenuDelegate;

			internal ToolStripDropDownMenu_(AMenu am)
			{
				_am = am;
			}

			protected override CreateParams CreateParams {
				get {
					//note: this prop is called several times, first time before ctor
					//Print("CreateParams");
					var p = base.CreateParams;
					if(_am != null) p.ExStyle |= (int)_am._ExStyle;
					return p;
				}
			}

			protected override void WndProc(ref Message m)
			{
				if(_am._WndProc_Before(false, this, ref m)) return;
				base.WndProc(ref m);
				_am._WndProc_After(false, this, ref m);
			}

			protected override void OnOpening(CancelEventArgs e)
			{
				if(!_openedOnce && !e.Cancel) {
					_openedOnce = true;

					//call the caller-provided callback function that should add submenu items on demand
					if(_lazySubmenuDelegate != null) {
						Items.Clear(); //remove the placeholder separator
						_am._submenuStack.Push(this);
						_lazySubmenuDelegate(_am);
						_am._submenuStack.Pop();
						_lazySubmenuDelegate = null;
					}

					PerformLayout();
				}

				if(AsyncIcons != null) {
					_am.GetIconsAsync_(this, AsyncIcons);
					AsyncIcons = null;
				}

				base.OnOpening(e);
			}

			//AMTBase creates this. We call GetAllAsync.
			internal List<IconsAsync.Item> AsyncIcons { get; set; }

			protected override void OnOpened(EventArgs e)
			{
				base.OnOpened(e);
				_am._OnOpenedAny(true, this);
			}

			protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
			{
				_am._OnClosingAny(true, this, e);
				base.OnClosing(e);
			}

			protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
			{
				base.OnClosed(e);
				_am._OnClosedAny(true, this);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
				_am._OnHandleCreatedDestroyed(true, this);
			}

			protected override void OnHandleDestroyed(EventArgs e)
			{
				_am._OnHandleCreatedDestroyed(false, this);
				base.OnHandleDestroyed(e);
			}

			//protected override void Dispose(bool disposing)
			//{
			//	base.Dispose(disposing);
			//	Print("submenu disposed", disposing);
			//}

			protected override void OnPaint(PaintEventArgs e)
			{
				//ADebug.PrintFunc();
				base.OnPaint(e);
				_paintedOnce = true;
			}

			//ToolStrip _IAuToolStrip.ToolStrip => this;

			bool _paintedOnce;
			bool _IAuToolStrip.PaintedOnce => _paintedOnce;
		}

		#region wndproc

		//Called in WndProc before calling base.WndProc, for main and context menu.
		//If returns true, return without calling base.WndProc and _WndProc_After.
		unsafe bool _WndProc_Before(bool isMainMenu, ToolStripDropDownMenu dd, ref Message m)
		{
			if(isMainMenu) {
				switch(m.Msg) {
				case Api.WM_HOTKEY:
					if(_OnHotkey((int)m.WParam)) return true;
					break;
				}
			}

			switch(m.Msg) {
			case Api.WM_CLOSE:
				//Print("WM_CLOSE", dd.Visible);
				if((int)m.WParam != _wmCloseWparam && dd.Visible) { Close(); return true; } //something tried to close from outside
				break;
			case Api.WM_SHOWWINDOW when m.WParam != default:
				//workaround for .NET bug: When showing context menu in inactive thread after showing in active, immediately closes it.
				//	Workaround: temporarily set AutoClose=false.
				_TempDisableAutoClose(dd);
				//workaround for .NET bug: makes a randowm window of this thread the owner window of the context menu.
				//	Then AutoClose=true tries to make both non-topmost. The below SWP_NOOWNERZORDER does not help.
				AWnd w = (AWnd)dd, ow = w.Owner; if(!ow.Is0) w.Owner = default;
				break;
			case Api.WM_RBUTTONUP:
				_inRightClick = true;
				//workaround for .NET bug: closes the context menu on right click.
				_TempDisableAutoClose(dd);
				break;
			case Api.WM_WINDOWPOSCHANGING when _disableZorder:
				var wp = (Api.WINDOWPOS*)m.LParam;
				wp->flags |= Native.SWP.NOZORDER | Native.SWP.NOOWNERZORDER;
				break;
			case Api.WM_CONTEXTMENU:
				_ContextMenu();
				return true;
			}

			return false;
		}

		//Called in WndProc after calling base.WndProc, for main and context menu.
		void _WndProc_After(bool isMainMenu, ToolStripDropDownMenu dd, ref Message m)
		{
			if(isMainMenu) {
				switch(m.Msg) {
				case Api.WM_CREATE:
					//Prevent 'wait' cursor appearing briefly when mouse enters a thread window first time.
					//It happens because initial thread cursor when creating the first thread window is 'wait', and the first mouse message is WM_NCHITTEST, followed by WM_SETCURSOR which sets correct cursor, and Windows briefly shows 'wait' cursor before sending WM_NCHITTEST.
					//Tested: it does not solve the 'wait' cursor problem when creating a context menu or submenu (when another workaround not applied); then on all messages Cursor.Current says 'Default'.
					//if(Cursor.Current==Cursors.WaitCursor) Cursor.Current = Cursors.Arrow; //I also have seen some other cursor briefly
					if(Cursor.Current != Cursors.Arrow) Cursor.Current = Cursors.Arrow;

					//This would solve the 'wait' cursor problem like WaitCursorWhenShowingMenuEtc.
					//Also need to eat WM_IME_SETCONTEXT, which creates a second hidden IME window and makes slower.
					//Faster by 1-2 ms.
					//Api.SetActiveWindow((AWnd)Handle);

					break;
				}
			}

			switch(m.Msg) {
			//case Api.WM_DESTROY:
			//	Print("WM_DESTROY", isMainMenu, m.HWnd);
			//	break;
			case Api.WM_RBUTTONUP:
				_inRightClick = false;
				break;
			}
		}

		//dd used for submenus, else null
		void _OnOpeningMain()
		{
			//Support showing not through our Show, for example when assigned to a control or toolstrip's drop-down button.
			if(!_inOurShow) {

				//Print(_c.OwnerItem, _c.SourceControl);
				_isOwned = _c.SourceControl != null || _c.OwnerItem != null;
				_isModal = false;
				MultiShow = true; //programmers would forget it
			}

			if(!_showedOnce) {
				_showedOnce = true;
				_c.PerformLayout();
			}

			GetIconsAsync_(_c);
		}

		//Prevents closing when working with focusable child controls and windows created by child controls or event handlers.
		void _OnClosingAny(bool isSubmenu, ToolStripDropDownMenu dd, ToolStripDropDownClosingEventArgs e)
		{
			if(!_isOwned && !e.Cancel) {
				//Print(e.CloseReason, dd.Focused, dd.ContainsFocus, AWnd.Active, AWnd.Active.Get.RootOwnerOrThis());
				switch(e.CloseReason) {
				case ToolStripDropDownCloseReason.AppClicked: //eg clicked a context menu item of a child textbox. Note: the AppClicked documentation lies; actually we receive this when clicked a window of this thread (or maybe this process, not tested).
					if(AWnd.Active.Handle == dd.Handle) e.Cancel = true;
					break;
				case ToolStripDropDownCloseReason.AppFocusChange: //eg showed a dialog owned by this menu window
					var wa = AWnd.Active;
					if(wa.Handle == dd.Handle) { //eg when closing this activated submenu because mouse moved to the parent menu
						if(isSubmenu) Api.SetForegroundWindow((AWnd)dd.OwnerItem.Owner.Handle); //prevent closing the parent menu
					} else if(wa.Get.RootOwnerOrThis().Handle == dd.Handle) e.Cancel = true;
					break;
				}
				//Print(e.Cancel);
			}
		}

		void _OnOpenedAny(bool isSubmenu, ToolStripDropDownMenu dd)
		{
			if(isSubmenu) {
				_visibleSubmenus.Add(dd);
			} else {
				_InitUninitClosing(true);
				if(ActivateMenuWindow) ((AWnd)dd.Handle).ActivateLL();
			}
		}

		void _OnClosedAny(bool isSubmenu, ToolStripDropDownMenu dd)
		{
			if(isSubmenu) {
				int n = _visibleSubmenus.Count; if(n > 0) _visibleSubmenus.RemoveAt(n - 1);
			} else {
				_InitUninitClosing(false);

				//Close menu windows. Else they are just hidden and prevent GC until process ends.
				foreach(var k in _windows) ((AWnd)k.Handle).Post(Api.WM_CLOSE, _wmCloseWparam);

				if(!MultiShow && !_isModal) ATimer.After(10, _ => Dispose()); //cannot dispose now, exception

				if(_isModal) _msgLoop.Stop();
			}
		}

		void _OnHandleCreatedDestroyed(bool created, ToolStripDropDownMenu dd)
		{
			//Print("_OnHandleCreatedDestroyed", created);
			if(created) {
				_windows.Add(dd);
			} else {
				_windows.Remove(dd);
			}
		}

		List<ToolStripDropDownMenu> _windows = new List<ToolStripDropDownMenu>(); //all menu windows, including hidden submenus
		const int _wmCloseWparam = 827549482;

		void _TempDisableAutoClose(ToolStripDropDownMenu dd)
		{
			if(!dd.AutoClose) return;
			_disableZorder = true; //workaround for .NET bug: AutoClose makes topmost
			dd.AutoClose = false;
			_disableZorder = false;
			ATimer.After(1, _ => {
				_disableZorder = true; //workaround for .NET bug: AutoClose makes non-topmost
				dd.AutoClose = true;
				_disableZorder = false;
			});
		}
		bool _disableZorder;

		void _ContextMenu()
		{
			if(_name == null) return;
			var m = new AMenu();
			var wmsg = ATask.WndMsg;
			if(!wmsg.Is0) {
				var contextItem = _c.GetItemAt(_c.MouseClientXY());
				m["Edit"] = o => GoToEdit_(contextItem);
			} else return; //FUTURE: maybe add more items. Could save menu options like AToolbar does.
			m.Show(_c);
		}

		#endregion

		#region close

		/// <summary>
		/// Close the menu when the mouse cursor moves away from it to this distance, pixels.
		/// At first the mouse must be or move at less than half of the distance.
		/// Default is equal to AMenu.DefaultMouseClosingDistance, default 200.
		/// </summary>
		/// <seealso cref="DefaultMouseClosingDistance"/>
		public int MouseClosingDistance { get; set; } = DefaultMouseClosingDistance;

		/// <summary>
		/// Default MouseClosingDistance value of AMenu instances.
		/// A AMenu instance inherits this at the moment it is created.
		/// </summary>
		public static int DefaultMouseClosingDistance { get; set; } = 200;

		List<ToolStripDropDownMenu> _visibleSubmenus = new List<ToolStripDropDownMenu>();
		ATimer _timer;
		bool _mouseWasIn;
		bool _hotkeyRegistered;
		const int _hotkeyEsc = 8405;

		//Called from OnOpened and OnClosed of main menu. Does nothing if already is in that state.
		void _InitUninitClosing(bool init)
		{
			if(init == (_timer != null)) return;
			//Print(init);
			_visibleSubmenus.Clear();
			_mouseWasIn = false;
			if(init) {
				_timer = ATimer.Every(100, _OnTimer);
				if(!(_isOwned || ActivateMenuWindow)) _hotkeyRegistered = Api.RegisterHotKey((AWnd)_c.Handle, _hotkeyEsc, 0, KKey.Escape);
			} else {
				if(_timer != null) { _timer.Stop(); _timer = null; }
				if(_hotkeyRegistered) _hotkeyRegistered = !Api.UnregisterHotKey((AWnd)_c.Handle, _hotkeyEsc);
			}
		}

		//100 ms
		void _OnTimer(ATimer t)
		{
			Debug.Assert(!IsDisposed); if(IsDisposed) return;

			_CloseIfMouseIsFar();
		}

		void _CloseIfMouseIsFar()
		{
			int dist = MouseClosingDistance;
			POINT p = AMouse.XY;
			for(int i = -1; i < _visibleSubmenus.Count; i++) {
				var k = i >= 0 ? _visibleSubmenus[i] : _c as ToolStripDropDown;
				RECT r = k.Bounds;
				if(!_mouseWasIn) {
					int half = dist / 2;
					r.Inflate(half, half);
					_mouseWasIn = r.Contains(p);
					r.Inflate(-half, -half);
				}
				r.Inflate(dist, dist);
				if(r.Contains(p)) return;
			}

			if(_mouseWasIn) Close();
		}

		bool _OnHotkey(int hkId)
		{
			if(hkId == _hotkeyEsc) {
				_Close(true);
				return true;
			}
			return false;
		}

		void _Close(bool onEsc)
		{
			Debug.Assert(!IsDisposed); if(IsDisposed) return;

			if(onEsc && _visibleSubmenus.Count > 0) {
				_visibleSubmenus[_visibleSubmenus.Count - 1].Close();
				return;
			}
			_c.Close();
		}

		/// <summary>
		/// Closes the menu and its submenus.
		/// </summary>
		public void Close()
		{
			_Close(false);
		}

		#endregion
	}
}
