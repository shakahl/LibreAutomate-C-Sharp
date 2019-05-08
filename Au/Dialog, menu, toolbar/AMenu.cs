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
using System.Reflection.Emit;

using Au.Types;
using static Au.NoClass;

//TODO: AMenu etc should have before/after delegate properties too. And dedicated threads. Like TriggerOptions.

namespace Au
{
	/// <summary>
	/// TODO
	/// </summary>
	public class AMenu : AMTBase, IDisposable
	{
		//The main wrapped object. The class is derived from ContextMenuStrip.
		ContextMenuStrip_ _cm;

		/// <summary>
		/// Gets ContextMenuStrip that is used to show the main drop-down menu.
		/// You can use all its properties, methods and events. You can assign it to a control or toolstrip's drop-down button etc.
		/// </summary>
		public ContextMenuStrip CMS => _cm;

		/// <summary>Infrastructure.</summary>
		protected override ToolStrip MainToolStrip => _cm;
		//Our base uses this.

		///
		public AMenu()
		{
			_cm = new ContextMenuStrip_(this);
		}

		//~AMenu() { Print("main dtor"); } //info: don't need finalizer. _cm and base have their own, and we don't have other unmanaged resources.

		///
		public void Dispose()
		{
			if(!_cm.IsDisposed) _cm.Dispose();
			//Print(_cm.Items.Count); //0
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
		public bool IsDisposed => _cm.IsDisposed;

		void _CheckDisposed()
		{
			if(IsDisposed) throw new ObjectDisposedException(nameof(AMenu), "Disposed. Use MultiShow=true.");
		}

		#region add

		/*
		CONSIDER:
		Specialized AddX methods.
		
		For example, instead of
		m["Label"] = o => Shell.TryRun("notepad.exe");
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
			m[label ?? path, path] = o => Shell.TryRun(path);
		}

		*/

		/// <summary>
		/// Adds new item.
		/// The same as <see cref="Add(string, Action{MTClickArgs}, object)"/>.
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
		/// m["notepad"] = o => Shell.TryRun(Folders.System + "notepad.exe"));
		/// m.Show();
		/// ]]></code>
		/// </example>
		public Action<MTClickArgs> this[string text, object icon = null] {
			set { Add(text, value, icon); }
		}

		/// <summary>
		/// Adds new item as <see cref="ToolStripMenuItem"/>.
		/// </summary>
		/// <param name="text">Text. If contains a tab character, like "Open\tCtrl+O", displays text after it as shortcut keys (right-aligned).</param>
		/// <param name="onClick">Callback function. Called when clicked the menu item.</param>
		/// <param name="icon">Can be:
		/// - string - path of .ico or any other file or folder or non-file object. See <see cref="AIcon.GetFileIcon"/>. If not full path, searches in <see cref="Folders.ThisAppImages"/>; see also <see cref="AMTBase.IconFlags"/>.
		/// - string - image name (key) in the ImageList (<see cref="ToolStripItem.ImageKey"/>).
		/// - int - image index in the ImageList (<see cref="ToolStripItem.ImageIndex"/>).
		/// - Icon, Image, Folders.FolderPath.
		/// - null (default) - no icon. If <see cref="AMTBase.ExtractIconPathFromCode"/> == true, extracts icon path from <i>onClick</i> code like <c>Shell.TryRun(@"c:\path\file.exe")</c> or <c>Shell.TryRun(Folders.System + "file.exe")</c>.
		/// - "" - no icon.
		/// </param>
		/// <remarks>
		/// Sets menu item text, icon and <b>Click</b> event handler. Other properties can be specified later. See example.
		/// 
		/// Code <c>m.Add("text", o => Print(o));</c> is the same as <c>m["text"] = o => Print(o);</c>. See <see cref="this[string, object]"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m.Add("One", o => Print(o), @"icon file path");
		/// m.Add("Two", o => { Print(o.MenuItem.Checked); ADialog.Show(o.ToString()); });
		/// m.LastMenuItem.Checked = true;
		/// m.ExtractIconPathFromCode = true;
		/// m.Add("notepad", o => Shell.TryRun(Folders.System + "notepad.exe"));
		/// m.Show();
		/// ]]></code>
		/// </example>
		public ToolStripMenuItem Add(string text, Action<MTClickArgs> onClick, object icon = null)
		{
			string sk = null;
			if(!Empty(text)) {
				int i = text.IndexOf('\t');
				if(i >= 0) { sk = text.Substring(i + 1); text = text.Remove(i); }
			}

			var item = new ToolStripMenuItem(text);

			if(sk != null) item.ShortcutKeyDisplayString = sk;

			_Add(item, onClick, icon);
			return item;
		}

		void _Add(ToolStripItem item, Action<MTClickArgs> onClick, object icon)
		{
			var dd = CurrentAddMenu;
			dd.SuspendLayout(); //makes adding items much faster. It's OK to suspend/resume when already suspended; .NET uses a layoutSuspendCount.
			dd.Items.Add(item);
			_SetItemProp(false, item, onClick, icon);
			dd.ResumeLayout(false);
		}

		/// <summary>
		/// Adds item of any supported type, for example ToolStripLabel, ToolStripTextBox, ToolStripComboBox, ToolStripProgressBar, ToolStripButton.
		/// Supports types derived from ToolStripItem.
		/// </summary>
		/// <param name="item">An already created item of any supported type.</param>
		/// <param name="icon"></param>
		/// <param name="onClick">Callback function. Called when the item clicked. Not useful for most item types.</param>
		public void Add(ToolStripItem item, object icon = null, Action<MTClickArgs> onClick = null)
		{
			_Add(item, onClick, icon);

			//Activate window when a child control clicked, or something may not work, eg cannot enter text in Edit control.
			if(item is ToolStripControlHost cb) cb.GotFocus += _Item_GotFocus; //combo, edit, progress
		}

		//Called when a text box or combo box clicked. Before MouseDown, which does not work well with combo box.
		void _Item_GotFocus(object sender, EventArgs e)
		{
			//ADebug.PrintFunc();
			if(!(_isOwned || ActivateMenuWindow)) {
				var t = sender as ToolStripItem;

				//Activate the clicked menu or submenu window to allow eg to enter text in text box.
				var w = (Wnd)t.Owner.Handle;
				Api.SetForegroundWindow(w); //does not fail, probably after a mouse click this process is allowed to activate windows, even if the click did not activate because of the window style

				//see also: both OnClosing
			}
		}

		/// <summary>
		/// Adds separator.
		/// </summary>
		public ToolStripSeparator Separator()
		{
			var item = new ToolStripSeparator();
			_Add(item, null, null);
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
		/// <param name="icon">See <see cref="Add(string, Action{MTClickArgs}, object)"/>.</param>
		/// <param name="onClick">Callback function. Called when the item clicked. Rarely used.</param>
		/// <remarks>
		/// Submenus inherit these properties of the main menu, set before adding submenus (see example):
		/// <b>BackgroundImage</b>, <b>BackgroundImageLayout</b>, <b>ContextMenu</b>, <b>Cursor</b>, <b>Font</b>, <b>ForeColor</b>, <b>ImageList</b>, <b>ImageScalingSize</b>, <b>Renderer</b>, <b>ShowCheckMargin</b>, <b>ShowImageMargin</b>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m.CMS.BackColor = Color.PaleGoldenrod;
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
		public MUsingSubmenu Submenu(string text, object icon = null, Action<MTClickArgs> onClick = null)
		{
			var item = _Submenu(out var dd, text, onClick, icon);
			_submenuStack.Push(dd);
			return new MUsingSubmenu(this, item);
		}

		ToolStripMenuItem _Submenu(out ToolStripDropDownMenu_ dd, string text, Action<MTClickArgs> onClick, object icon)
		{
			var item = new ToolStripMenuItem(text);
			_Add(item, onClick, icon);

			dd = new ToolStripDropDownMenu_(this);
			item.DropDown = dd;
			//SHOULDDO: should be 'dd=item.DropDown' (auto-created).
			//	Because now eg hotkeys don't work.
			//	But then cannot be ToolStripDropDownMenu_.
			//	It is important only if using in menu bar.

			//var t = new Perf.Inst(); t.First();
			dd.SuspendLayout();
			if(_cm._changedBackColor) dd.BackColor = _cm.BackColor; //because by default gives 0xf0f0f0, although actually white, and then submenus would be gray
			dd.BackgroundImage = _cm.BackgroundImage;
			dd.BackgroundImageLayout = _cm.BackgroundImageLayout;
			dd.ContextMenu = _cm.ContextMenu;
			dd.Cursor = _cm.Cursor;
			dd.Font = _cm.Font;
			if(_cm._changedForeColor) dd.ForeColor = _cm.ForeColor;
			dd.ImageList = _cm.ImageList;
			dd.ImageScalingSize = _cm.ImageScalingSize;
			dd.Renderer = _cm.Renderer;
			dd.ShowCheckMargin = _cm.ShowCheckMargin;
			dd.ShowImageMargin = _cm.ShowImageMargin;
			dd.ResumeLayout(false);
			//t.NW();

			if(_AddingSubmenuItems) item.MouseHover += _Item_MouseHover;

			if(_submenusToDispose == null) _submenusToDispose = new List<ToolStripDropDownMenu_>();
			_submenusToDispose.Add(dd);

			return item;
		}

		List<ToolStripDropDownMenu_> _submenusToDispose;

		//Workaround for ToolStripDropDown bug: sometimes does not show 3-rd level submenu; it happens when the mouse moves from the 1-level menu to the 2-nd level submenu-item over an unrelated item of 1-st level menu.
		void _Item_MouseHover(object sender, EventArgs e)
		{
			var k = sender as ToolStripMenuItem;
			if(k.HasDropDownItems && !k.DropDown.Visible) k.ShowDropDown();

			//Note that submenus are shown not on hover.
			//The hover time is SPI_GETMOUSEHOVERTIME, and submenu delay is SPI_GETMENUSHOWDELAY.
			//This usually works well because both times are equal, 400 ms.
			//If they are different, it is not important, because don't need and we don't use this workaround for direct submenus. Indirect submenus are rarely used.
		}

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
		public void EndSubmenu()
		{
			var dd = _submenuStack.Pop();
		}

		/// <summary>
		/// Adds new item (<see cref="ToolStripMenuItem"/>) that will open a submenu.
		/// When showing the submenu first time, your callback function will be called and can add submenu items.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onOpening">Callback function that should add submenu items.</param>
		/// <param name="icon">See <see cref="Add(string, Action{MTClickArgs}, object)"/>.</param>
		/// <param name="onClick">Callback function. Called when the item clicked. Rarely used.</param>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AMenu();
		/// m["One"] = o => Print(o);
		/// m["Two"] = o => Print(o);
		/// m.Submenu("Submenu 1", m1 =>
		/// {
		/// 	Print("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 	m["Three"] = o => Print(o);
		/// 	m["Four"] = o => Print(o);
		/// 	m.Submenu("Submenu 2", m2 =>
		/// 	{
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
		public ToolStripMenuItem Submenu(string text, Action<AMenu> onOpening, object icon = null, Action<MTClickArgs> onClick = null)
		{
			var item = _Submenu(out var dd, text, onClick, icon);
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
		/// Gets <see cref="ToolStripDropDownMenu"/> of the main menu or submenu where new items currently are added.
		/// </summary>
		public ToolStripDropDownMenu CurrentAddMenu {
			get {
				_CheckDisposed(); //used by all Add(), Submenu()
				return _submenuStack.Count > 0 ? _submenuStack.Peek() : _cm;
			}
		}

		/// <summary>
		/// Gets the last added item as <see cref="ToolStripMenuItem"/>.
		/// Returns null if it is not a <b>ToolStripMenuItem</b>, for example a button or separator.
		/// </summary>
		/// <remarks>
		/// You can instead use <see cref="AMTBase.LastItem"/>, which gets <see cref="ToolStripItem"/>, which is the base class of all supported item types.
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
		/// Alternatively you can assign the context menu to a control or toolstrip's drop-down button etc, then don't need to call <b>Show</b>. Use the <see cref="CMS"/> property, which gets <see cref="ContextMenuStrip"/>.
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
		/// Alternatively you can assign the context menu to a control or toolstrip's drop-down button etc, then don't need to call <b>Show</b>. Use the <see cref="CMS"/> property, which gets <see cref="ContextMenuStrip"/>.
		/// </remarks>
		public void Show(Control owner, ToolStripDropDownDirection direction = ToolStripDropDownDirection.Default)
		{
			var p = owner.MouseClientXY();
			_Show(3, p.x, p.y, direction, owner);
		}

		void _Show(int overload, int x = 0, int y = 0, ToolStripDropDownDirection direction = 0, Control control = null)
		{
			_CheckDisposed();
			if(_cm.Items.Count == 0) return;

			if(!_showedOnce) {
				//_showedOnce = true; //OnOpening() sets it
				if(!ActivateMenuWindow) Util.LibWorkarounds.WaitCursorWhenShowingMenuEtc();
			}
			//Perf.Next();

			_isOwned = control != null;
			_isModal = ModalAlways ? true : !AThread.HasMessageLoop();

			_inOurShow = true;
			switch(overload) {
			case 1: _cm.Show(Mouse.XY); break;
			case 2: _cm.Show(new Point(x, y), direction); break;
			case 3: _cm.Show(control, new Point(x, y), direction); break;
			case 4:
				Keyb.More.GetTextCursorRect(out RECT cr, out _, orMouse: true);
				_cm.Show(new Point(cr.left - 32, cr.bottom + 2));
				break;
			}
			_inOurShow = false;
			//Perf.Next();

			if(_isModal) {
				_msgLoop.Loop();
				if(!MultiShow) Dispose();
			}
		}

		bool _showedOnce;
		bool _inOurShow; //to detect when ContextMenuStrip.Show called not through AMenu.Show
		bool _isOwned; //control==0
		bool _isModal; //depends on ModalAlways or Application.MessageLoop
		Util.MessageLoop _msgLoop = new Util.MessageLoop();

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

		#endregion

		//Extends ContextMenuStrip of the main menu to change its behavior as we need.
		class ContextMenuStrip_ : ContextMenuStrip, _IAuToolStrip
		{
			AMenu _am;

			internal ContextMenuStrip_(AMenu am)
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
				//Wnd.Misc.PrintMsg(ref m, Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_ERASEBKGND, Api.WM_CTLCOLOREDIT);

				if(_am._WndProc_Before(true, this, ref m)) return;
				//var t = Perf.StartNew();
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
				//Print("menu disposed", disposing);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				//var perf = Perf.StartNew();

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
					_am._GetIconsAsync(this, AsyncIcons);
					AsyncIcons = null;
				}

				base.OnOpening(e);
			}

			//AMTBase creates this. We call GetAllAsync.
			internal List<Util.IconsAsync.Item> AsyncIcons { get; set; }

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
		bool _WndProc_Before(bool isMainMenu, ToolStripDropDownMenu dd, ref Message m)
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
			case Api.WM_WINDOWPOSCHANGING:
				unsafe {
					var p = (Api.WINDOWPOS*)m.LParam;
					//after right-click, enabling etc somebody tries to make the menu non-topmost by placing it after some window, eg a hidden tooltip (which is topmost...???)
					if((p->flags & Native.SWP.NOZORDER) == 0 && !p->hwndInsertAfter.Is0) {
						//Print(p->hwndInsertAfter, p->hwndInsertAfter.IsTopmost);
						p->flags |= Native.SWP.NOZORDER;
					}
				}
				break;
			case Api.WM_RBUTTONUP:
				m_inRightClick = true;
				_restoreAutoClose = dd.AutoClose; dd.AutoClose = false;
				break;
			}

			return false;
		}

		bool _restoreAutoClose;

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
					//Api.SetActiveWindow((Wnd)Handle);

					break;
				}
			}

			switch(m.Msg) {
			//case Api.WM_DESTROY:
			//	Print("WM_DESTROY", isMainMenu, m.HWnd);
			//	break;
			case Api.WM_RBUTTONUP:
				if(!dd.IsDisposed) dd.AutoClose = _restoreAutoClose;
				m_inRightClick = false;
				break;
			}
		}

		//dd used for submenus, else null
		void _OnOpeningMain()
		{
			//Support showing not through our Show, for example when assigned to a control or toolstrip's drop-down button.
			if(!_inOurShow) {

				//Print(_cm.OwnerItem, _cm.SourceControl);
				_isOwned = _cm.SourceControl != null || _cm.OwnerItem != null;
				_isModal = false;
				MultiShow = true; //programmers would forget it
			}

			if(!_showedOnce) {
				_showedOnce = true;
				_cm.PerformLayout();
			}

			_GetIconsAsync(_cm);
		}

		//Prevents closing when working with focusable child controls and windows created by child controls or event handlers.
		void _OnClosingAny(bool isSubmenu, ToolStripDropDownMenu dd, ToolStripDropDownClosingEventArgs e)
		{
			if(!_isOwned && !e.Cancel) {
				//Print(e.CloseReason, dd.Focused, dd.ContainsFocus, Wnd.Active, Wnd.Active.Get.RootOwnerOrThis());
				switch(e.CloseReason) {
				case ToolStripDropDownCloseReason.AppClicked: //eg clicked a context menu item of a child textbox. Note: the AppClicked documentation lies; actually we receive this when clicked a window of this thread (or maybe this process, not tested).
					if(Wnd.Active.Handle == dd.Handle) e.Cancel = true;
					break;
				case ToolStripDropDownCloseReason.AppFocusChange: //eg showed a dialog owned by this menu window
					var wa = Wnd.Active;
					if(wa.Handle == dd.Handle) { //eg when closing this activated submenu because mouse moved to the parent menu
						if(isSubmenu) Api.SetForegroundWindow((Wnd)dd.OwnerItem.Owner.Handle); //prevent closing the parent menu
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
				if(ActivateMenuWindow) ((Wnd)dd.Handle).ActivateLL();
			}
		}

		void _OnClosedAny(bool isSubmenu, ToolStripDropDownMenu dd)
		{
			if(isSubmenu) {
				int n = _visibleSubmenus.Count; if(n > 0) _visibleSubmenus.RemoveAt(n - 1);
			} else {
				_InitUninitClosing(false);

				//Close menu windows. Else they are just hidden and prevent garbage collection until appdomain ends.
				foreach(var k in _windows) ((Wnd)k.Handle).Post(Api.WM_CLOSE, _wmCloseWparam);

				if(!MultiShow && !_isModal) ATimer.After(10, () => { Dispose(); }); //cannot dispose now, exception

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
				if(!(_isOwned || ActivateMenuWindow)) _hotkeyRegistered = Api.RegisterHotKey((Wnd)_cm.Handle, _hotkeyEsc, 0, KKey.Escape);
			} else {
				if(_timer != null) { _timer.Stop(); _timer = null; }
				if(_hotkeyRegistered) _hotkeyRegistered = !Api.UnregisterHotKey((Wnd)_cm.Handle, _hotkeyEsc);
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
			POINT p = Mouse.XY;
			for(int i = -1; i < _visibleSubmenus.Count; i++) {
				var k = i >= 0 ? _visibleSubmenus[i] : _cm as ToolStripDropDown;
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

			if(Wnd.More.GetGUIThreadInfo(out var g, Api.GetCurrentThreadId()) && !g.hwndMenuOwner.Is0) {
				Api.EndMenu();
				if(onEsc) return;
			}
			if(onEsc && _visibleSubmenus.Count > 0) {
				_visibleSubmenus[_visibleSubmenus.Count - 1].Close();
				return;
			}
			_cm.Close();
		}

		/// <summary>
		/// Closes the menu and its submenus.
		/// Also closes its context menu (CMS.ContextMenu).
		/// </summary>
		public void Close()
		{
			_Close(false);
		}

		#endregion
	}
}

namespace Au.Types
{
	interface _IAuToolStrip
	{
		//ToolStrip ToolStrip { get; } //currently not used; we use MainToolStrip instead.
		bool PaintedOnce { get; }
	}

	/// <summary>
	/// Base class of <see cref="AMenu"/> and <see cref="AToolbar"/>.
	/// </summary>
	public abstract class AMTBase
	{
		struct _ClickAction
		{
			public Action<MTClickArgs> action;
			public MTThread threadOpt;
			public MTExcept exceptOpt;
		}

		Dictionary<object, _ClickAction> _clickActions;
		EventHandler _onClick;
		internal bool m_inRightClick;

		internal AMTBase()
		{
			_onClick = _OnClick;
			_clickActions = new Dictionary<object, _ClickAction>();
		}

		//Common Click even handler of all items.
		//Calls true item's onClick callback if need.
		void _OnClick(object sender, EventArgs args)
		{
			if(m_inRightClick) return; //disable this item while showing a context menu for this item
			var x = _clickActions[sender];

			switch(x.threadOpt) {
			case MTThread.Current:
				_ExecItem(sender, x);
				break;
			case MTThread.ThreadPool:
				Task.Run(() => _ExecItem(sender, x));
				break;
			case MTThread.StaThread:
			case MTThread.StaBackgroundThread:
				AThread.Start(() => _ExecItem(sender, x), x.threadOpt == MTThread.StaBackgroundThread);
				break;
			}
		}

		void _ExecItem(object sender, in _ClickAction x)
		{
			var ca = new MTClickArgs(sender as ToolStripItem);
			if(x.exceptOpt == MTExcept.Exception && x.threadOpt == MTThread.Current) {
				x.action(ca);
			} else {
				try {
					x.action(ca);
				}
				catch(Exception e) when(!(e is ThreadAbortException)) {
					if(x.exceptOpt != MTExcept.Silent) PrintWarning(e.ToString(), -1);
				}
			}
		}

		/// <summary>
		/// In what thread to execute item callback functions.
		/// Default: current thread.
		/// </summary>
		/// <remarks>
		/// If current thread is a UI thread (has windows etc), and item callback functions execute some long automations in the same thread, current thread probably is hung during that time. Use this property to avoid it.
		/// This property is applied to items added afterwards.
		/// </remarks>
		public MTThread ItemThread { get; set; }

		/// <summary>
		/// Whether/how to handle unhandled exceptions in item code.
		/// Default: <see cref="MTExcept.Exception"/> (don't handle exceptions if <see cref="ItemThread"/> is <see cref="MTThread.Current"/> (default), else show warning).
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards.
		/// </remarks>
		public MTExcept ExceptionHandling { get; set; }
		//FUTURE: public Type[] ExceptionTypes { get; set; }
		//	Or bool ExceptionHandling and Func<Exception, bool> ExceptionFilter.

		/// <summary>
		/// Gets ToolStrip of AMenu and AToolbar, which override this.
		/// </summary>
		protected abstract ToolStrip MainToolStrip { get; }

		/// <summary>
		/// Gets the last added item as <see cref="ToolStripItem"/>, which is the base type of <see cref="ToolStripMenuItem"/>, <see cref="ToolStripButton"/> and other supported types.
		/// </summary>
		public ToolStripItem LastItem { get; protected set; }

		/// <summary>
		/// Occurs when an item is added.
		/// Allows to set item properties in single place instead of after each 'add item' code line.
		/// For example, the event handler can set item properties common to all items, or set item properties encoded in item text.
		/// </summary>
		public event Action<ToolStripItem> ItemAdded;

		///// <summary>
		///// ContextMenu to show when right-clicked.
		///// //_TODO: how to know which item clicked? Maybe add a common default context menu. Currently not used.
		///// </summary>
		//public ContextMenu ContextMenu
		//{
		//	get => MainToolStrip.ContextMenu;
		//	set { MainToolStrip.ContextMenu = value; }
		//}

		/// <summary>
		/// Flags to pass to <see cref="AIcon.GetFileIcon"/>. See <see cref="GIFlags"/>.
		/// </summary>
		/// <remarks>
		/// This property is applied to all items.
		/// </remarks>
		public GIFlags IconFlags { get; set; }

		/// <summary>
		/// Image width and height.
		/// Also can be enum <see cref="IconSize"/>, cast to int.
		/// </summary>
		/// <exception cref="InvalidOperationException">The 'set' function is called after adding items.</exception>
		/// <remarks>
		/// This property is applied to all items, and can be set only before adding items (else exception).
		/// To set different icon size for a submenu: <c>using(m.Submenu("sub")) { m.LastMenuItem.DropDown.ImageScalingSize = new Size(24, 24);</c>
		/// </remarks>
		public int IconSize {
			get => MainToolStrip.ImageScalingSize.Width;
			set {
				if(MainToolStrip.Items.Count != 0) throw new InvalidOperationException();
				MainToolStrip.ImageScalingSize = new Size(value, value);
			}
		}

		//Sets icon and onClick delegate.
		//Sets LastItem.
		//Calls ItemAdded event handlers.
		internal void _SetItemProp(bool isBar, ToolStripItem item, Action<MTClickArgs> onClick, object icon)
		{
			if(onClick != null) {
				_clickActions[item] = new _ClickAction() { action = onClick, threadOpt = this.ItemThread, exceptOpt = this.ExceptionHandling };
				item.Click += _onClick;

				//Perf.First();
				if(icon == null && ExtractIconPathFromCode) icon = _IconPathFromCode(onClick.Method);
				//Perf.NW(); //ngened about 10 ms first time, then fast. Else 30-40 ms first time.
				//Print(icon);
			}

#if true //to quickly disable icons when measuring speed
			if(icon != null) {
				try {
					switch(icon) {
					case string path: _SetItemFileIcon(isBar, item, path); break;
					case int index: if(index >= 0) item.ImageIndex = index; break;
					case Image img: item.Image = img; break;
					case Icon ico: item.Image = ico.ToBitmap(); break;
					case Folders.FolderPath fp: _SetItemFileIcon(isBar, item, fp); break;
					}
				}
				catch(Exception e) { ADebug.Print(e.Message); } //ToBitmap() may throw
			}
#endif
			LastItem = item;

			ItemAdded?.Invoke(item);
		}

		/// <summary>
		/// When adding items without explicitly specified icon, extract icon from item code.
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards.
		/// </remarks>
		public bool ExtractIconPathFromCode { get; set; }

		void _SetItemFileIcon(bool isBar, ToolStripItem item, string s)
		{
			if(Empty(s)) return;
			var owner = item.Owner;
			var il = owner.ImageList;
			if(il != null && il.Images.ContainsKey(s)) {
				item.ImageKey = s;
			} else {
				//var perf = Perf.StartNew();
				item.ImageScaling = ToolStripItemImageScaling.None; //we'll get icons of correct size, except if size is 256 and such icon is unavailable, then show smaller

				if(_AsyncIcons == null) _AsyncIcons = new Util.IconsAsync(); //used by submenus too
				var submenu = !isBar ? (owner as AMenu.ToolStripDropDownMenu_) : null;
				bool isFirstImage = false;

				if(submenu == null) {
					if(_AsyncIcons.Count == 0) isFirstImage = true;
					_AsyncIcons.Add(s, item);
				} else {
					if(submenu.AsyncIcons == null) {
						submenu.AsyncIcons = new List<Util.IconsAsync.Item>();
						isFirstImage = true;
					}
					submenu.AsyncIcons.Add(new Util.IconsAsync.Item(s, item));
				}

				//Reserve space for image.
				//If toolbar, need to do it for each button, else only for the first item (it sets size of all items).
				if(isFirstImage) {
					var z = owner.ImageScalingSize;
					_imagePlaceholder = new Bitmap(z.Width, z.Height);
				}
				if(isBar || isFirstImage) item.Image = _imagePlaceholder;
				//perf.NW();
			}
		}
		Image _imagePlaceholder;

		//This is shared by toolbars and main menus. Submenus have their own.
		Util.IconsAsync _AsyncIcons { get; set; }

		//list - used by submenus.
		internal void _GetIconsAsync(ToolStrip ts, List<Util.IconsAsync.Item> list = null)
		{
			if(_AsyncIcons == null) return;
			if(list != null) _AsyncIcons.AddRange(list);
			if(_AsyncIcons.Count == 0) return;
			_AsyncIcons.GetAllAsync(_AsyncCallback, ts.ImageScalingSize.Width, IconFlags, ts);
		}

		void _AsyncCallback(Util.IconsAsync.Result r, object objCommon, int nLeft)
		{
			var ts = objCommon as ToolStrip;
			var item = r.obj as ToolStripItem;

			//Print(r.image, r.hIcon);
			//Image im = r.image;
			//if(im == null && r.hIcon != default) im = AIcon.HandleToImage(r.hIcon);

			Image im = AIcon.HandleToImage(r.hIcon, true);

			//if(im != null) _SetItemIcon(ts, item, im);
			if(im != null) {
				_SetItemIcon(ts, item, im);

				//to dispose images in our Dispose()
				if(_images == null) _images = new List<Image>();
				_images.Add(im);
			}

			//#if DEBUG
			//			if(im == null) item.ForeColor = Color.Red;
			//#endif
			if(nLeft == 0) {
				//Perf.Next();
				ts.Update();
				//Perf.NW();
			}
		}

		void _SetItemIcon(ToolStrip ts, ToolStripItem item, Image im)
		{
			Wnd w = default;
			var its = ts as _IAuToolStrip;
			if(its.PaintedOnce) {
				if(_region1 == default) _region1 = Api.CreateRectRgn(0, 0, 0, 0);
				if(_region2 == default) _region2 = Api.CreateRectRgn(0, 0, 0, 0);

				w = (Wnd)ts.Handle;
				Api.GetUpdateRgn(w, _region1, false);
			}

			//RECT u;
			//Api.GetUpdateRect((Wnd)ts.Handle, out u, false); Print(its.PaintedOnce, u);

			ts.SuspendLayout(); //without this much slower, especially when with overflow arrows (when many items)
			item.Image = im;
			ts.ResumeLayout(false);

			if(its.PaintedOnce) {
				Api.ValidateRect(w); //tested: with WM_SETREDRAW 3 times slower
				RECT r = item.Bounds; //r.Inflate(-2, -1);
									  //r.right = r.left + r.Height; //same speed
				Api.SetRectRgn(_region2, r.left, r.top, r.right, r.bottom);
				Api.CombineRgn(_region1, _region1, _region2, Api.RGN_OR);

				//RECT b; GetRgnBox(_region1, out b); Print(b, _region1);

				Api.InvalidateRgn(w, _region1, false);
			}

			//Api.GetUpdateRect((Wnd)ts.Handle, out u, false); Print("after", u);
		}

		IntPtr _region1, _region2;
		internal List<Image> _images;

		/// <summary>
		/// Gets icon path from code that contains string like <c>@"c:\windows\system32\notepad.exe"</c> or <c>@"%Folders.System%\notepad.exe"</c> or URL/shell.
		/// Also supports code patterns like 'Folders.System + "notepad.exe"' or 'Folders.Virtual.RecycleBin'.
		/// Returns null if no such string/pattern.
		/// </summary>
		static string _IconPathFromCode(MethodInfo mi)
		{
			//support code pattern like 'Folders.System + "notepad.exe"'.
			//	Opcodes: call(Folders.System), ldstr("notepad.exe"), Folders.FolderPath.op_Addition.
			//also code pattern like 'Folders.System' or 'Folders.Virtual.RecycleBin'.
			//	Opcodes: call(Folders.System), Folders.FolderPath.op_Implicit(FolderPath to string).
			//also code pattern like 'Shell.TryRun("notepad.exe")'.
			int i = 0, patternStart = -1; MethodInfo f1 = null; string filename = null, filename2 = null;
			try {
				var reader = new Util.ILReader(mi);
				foreach(var instruction in reader.Instructions) {
					if(++i > 100) break;
					var op = instruction.Op;
					//Print(op);
					if(instruction.Op == OpCodes.Ldstr) {
						var s = instruction.Data as string;
						//Print(s);
						if(i == patternStart + 1) filename = s;
						else {
							if(APath.IsFullPathExpandEnvVar(ref s)) return s; //eg Shell.TryRun(@"%Folders.System%\notepad.exe");
							if(APath.IsUrl(s) || APath.LibIsShellPath(s)) return s;
							filename = null; patternStart = -1;
							if(i == 1) filename2 = s;
						}
					} else if(op == OpCodes.Call && instruction.Data is MethodInfo f && f.IsStatic) {
						//Print(f, f.DeclaringType, f.Name, f.MemberType, f.ReturnType, f.GetParameters().Length);
						var dt = f.DeclaringType;
						if(dt == typeof(Folders) || dt == typeof(Folders.Virtual)) {
							if(f.ReturnType == typeof(Folders.FolderPath) && f.GetParameters().Length == 0) {
								//Print(1);
								f1 = f;
								patternStart = i;
							}
						} else if(dt == typeof(Folders.FolderPath)) {
							if(i == patternStart + 2 && f.Name == "op_Addition") {
								//Print(2);
								var fp = (Folders.FolderPath)f1.Invoke(null, null);
								if((string)fp == null) return null;
								return fp + filename;
							} else if(i == patternStart + 1 && f.Name == "op_Implicit" && f.ReturnType == typeof(string)) {
								//Print(3);
								return (Folders.FolderPath)f1.Invoke(null, null);
							}
						}
					}
				}
				if(filename2 != null && filename2.Ends(".exe", true)) return AFile.SearchPath(filename2);
			}
			catch(Exception ex) { ADebug.Print(ex); }
			return null;
		}

		internal void _Dispose(bool disposing)
		{
			//Print("_Dispose", _isDisposed);
			if(_isDisposed) return;
			_isDisposed = true;

			if(disposing) {
				if(_AsyncIcons != null) _AsyncIcons.Dispose();

				if(_images != null) {
					foreach(var im in _images) im.Dispose();
					_images = null;
				}
			}

			if(_region1 != default) Api.DeleteObject(_region1);
			if(_region2 != default) Api.DeleteObject(_region2);

			LastItem = null;
		}
		bool _isDisposed;

		///
		~AMTBase() { /*Print("base dtor");*/ _Dispose(false); }
	}

	/// <summary>
	/// Data passed to Click event handler functions of <see cref="AMenu"/> and <see cref="AToolbar"/>.
	/// </summary>
	public class MTClickArgs
	{
		/// <summary>
		/// Gets the clicked item as ToolStripItem.
		/// </summary>
		public ToolStripItem Item { get; }

		/// <summary>
		/// Gets the clicked item as ToolStripMenuItem.
		/// Returns null if it is not ToolStripMenuItem.
		/// </summary>
		public ToolStripMenuItem MenuItem => Item as ToolStripMenuItem;

		internal MTClickArgs(ToolStripItem item) { Item = item; }

		/// <summary>
		/// Gets item text.
		/// </summary>
		public override string ToString() => Item.ToString();
	}

	/// <summary>
	/// Used with <see cref="AMTBase.ItemThread"/>.
	/// </summary>
	public enum MTThread : byte
	{
		/// <summary>
		/// Execute item callback functions in current thread. This is default.
		/// </summary>
		Current,

		/// <summary>
		/// Execute item callback functions in thread pool threads (<see cref="Task.Run"/>).
		/// Note: current thread does not wait until the callback function finishes.
		/// </summary>
		ThreadPool,

		/// <summary>
		/// Execute item callback functions in new STA threads (<see cref="Thread.SetApartmentState"/>).
		/// Note: current thread does not wait until the callback function finishes.
		/// </summary>
		StaThread,

		/// <summary>
		/// Execute item callback functions in new STA background threads (<see cref="Thread.IsBackground"/>).
		/// Note: current thread does not wait until the callback function finishes.
		/// </summary>
		StaBackgroundThread,
	}

	/// <summary>
	/// Used with <see cref="AMTBase.ExceptionHandling"/>.
	/// </summary>
	public enum MTExcept : byte
	{
		/// <summary>
		/// Don't handle exceptions. This is default.
		/// However if <see cref="AMTBase.ItemThread"/> is not <see cref="MTThread.Current"/>, handles exceptions and shows warning.
		/// </summary>
		Exception,

		/// <summary>Handle exceptions. On exception call <see cref="PrintWarning"/>.</summary>
		Warning,

		/// <summary>Handle exceptions. On exception do nothing.</summary>
		Silent,
	}

	/// <summary>
	/// Allows to create <see cref="AMenu"/> submenus easier.
	/// Example: <c>using(m.Submenu("Name")) { add items; }</c> .
	/// </summary>
	public struct MUsingSubmenu : IDisposable
	{
		AMenu _m;

		/// <summary>
		/// Gets <b>ToolStripMenuItem</b> of the submenu-item.
		/// </summary>
		public ToolStripMenuItem MenuItem { get; }

		internal MUsingSubmenu(AMenu m, ToolStripMenuItem mi) { _m = m; MenuItem = mi; }

		/// <summary>
		/// Calls <see cref="AMenu.EndSubmenu"/>.
		/// </summary>
		public void Dispose() { _m.EndSubmenu(); }
	}
}
