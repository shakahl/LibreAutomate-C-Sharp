using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	public class CatMenu :Base_CatMenu_CatBar, IDisposable
	{
		//The main wrapped object. The class is derived from ContextMenuStrip.
		ContextMenuStrip_ _cm;

		/// <summary>
		/// Gets ContextMenuStrip that is used to show the main drop-down menu.
		/// You can use all its properties, methods and events. You can assign it to a control or toolstrip's drop-down button etc.
		/// </summary>
		public ContextMenuStrip CMS { get { return _cm; } }

		//Our base uses this.
		protected override ToolStrip MainToolStrip { get { return _cm; } }

		/// <summary>
		/// Initializes a new instance of the CatMenu class.
		/// </summary>
		public CatMenu()
		{
			_cm = new ContextMenuStrip_(this);
		}

		/// <summary>
		/// Initializes a new instance of the CatMenu class and associates its ContextMenuStrip with the specified container.
		/// </summary>
		public CatMenu(IContainer container)
		{
			_cm = new ContextMenuStrip_(this, container);
		}

		//~CatMenu() { Out("main dtor"); } //info: don't need finalizer. _cm and base have their own, and we don't have other unmanaged resources.

		public void Dispose()
		{
			if(!_cm.IsDisposed) _cm.Dispose();
			//Out(_cm.Items.Count); //0
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

		public bool IsDisposed { get { return _cm.IsDisposed; } }

		void _CheckDisposed()
		{
			if(IsDisposed) throw new ObjectDisposedException(nameof(CatMenu), "Disposed. Use MultiShow=true.");
		}

		#region add

		/// <summary>
		/// Adds new item as ToolStripMenuItem.
		/// Sets its text, icon and Click event handler delegate. Other properties can be specified later. See example.
		/// Code <c>m.Add("text", o => Out(o));</c> is the same as <c>m["text"] = o => Out(o);</c>.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">Can be:
		/// string - icon file, as with <see cref="Files.GetIconHandle"/>.
		/// string - image name (key) in the ImageList (<see cref="ToolStripItem.ImageKey"/>).
		/// int - image index in the ImageList (<see cref="ToolStripItem.ImageIndex"/>).
		/// IntPtr - unmanaged icon handle (the function makes its own copy).
		/// Icon, Image, Folders.FolderPath.
		/// </param>
		/// <example>
		/// var m = new CatMenu();
		/// m["One"] = o => Out(o);
		/// m["Two", @"icon file path"] = o => { Out(o); Show.TaskDialog(o.ToString()); };
		/// m.LastItem.ToolTipText = "tooltip";
		/// m["Three"] = o => { Out(o.MenuItem.Checked); };
		/// m.LastMenuItem.Checked = true;
		/// m.Show();
		/// </example>
		public Action<ClickEventData> this[string text, object icon = null]
		{
			set { Add(text, value, icon); }
		}

		/// <summary>
		/// Adds new item as ToolStripMenuItem.
		/// Sets its text, icon and Click event handler delegate. Other properties can be specified later. See example.
		/// Code <c>m.Add("text", o => Out(o));</c> is the same as <c>m["text"] = o => Out(o);</c>.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onClick">Lambda etc function to be called when the menu item clicked.</param>
		/// <param name="icon">Can be:
		/// string - icon file, as with <see cref="Files.GetIconHandle"/>.
		/// string - image name (key) in the ImageList (<see cref="ToolStripItem.ImageKey"/>).
		/// int - image index in the ImageList (<see cref="ToolStripItem.ImageIndex"/>).
		/// IntPtr - unmanaged icon handle (the function makes its own copy).
		/// Icon, Image, Folders.FolderPath.
		/// </param>
		/// <example>
		/// var m = new CatMenu();
		/// m.Add("One", o => Out(o), @"icon file path");
		/// m.Add("Two", o => { Out(o.MenuItem.Checked); });
		/// m.LastMenuItem.Checked = true;
		/// m.Show();
		/// </example>
		public ToolStripMenuItem Add(string text, Action<ClickEventData> onClick, object icon = null)
		{
			var item = new ToolStripMenuItem(text);
			_Add(item, onClick, icon);
			return item;
		}

		void _Add(ToolStripItem item, Action<ClickEventData> onClick, object icon)
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
		/// <param name="icon">The same as with other overload.</param>
		/// <param name="onClick">Lambda etc function to be called when the item clicked. Not useful for most item types.</param>
		public void Add(ToolStripItem item, object icon = null, Action<ClickEventData> onClick = null)
		{
			_Add(item, onClick, icon);

			//Activate window when a child control clicked, or something may not work, eg cannot enter text in Edit control.
			var cb = item as ToolStripControlHost; //combo, edit, progress
			if(cb != null) cb.GotFocus += _Item_GotFocus;
		}

		//Called when a text box or combo box clicked. Before MouseDown, which does not work well with combo box.
		void _Item_GotFocus(object sender, EventArgs e)
		{
			//OutFunc();
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
		/// Adds new item (ToolStripMenuItem) that will open a submenu.
		/// Then the add-item functions will add items to its submenu.
		/// Can be used in 2 ways:
		/// 1. <c>using(m.Submenu(...)) { add items; }</c>. See example.
		/// 2. <c>m.Submenu(...); add items; m.EndSubmenu();</c>. See <see cref="EndSubmenu"/>.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">The same as with <see cref="Add"/>.</param>
		/// <param name="onClick">Lambda etc function to be called when the menu item clicked. Rarely used.</param>
		/// <remarks>
		/// Submenus inherit these properties of the main menu, set before adding submenus (see example):
		/// BackgroundImage, BackgroundImageLayout, ContextMenu, Cursor, Font, ForeColor, ImageList, ImageScalingSize, Renderer, ShowCheckMargin, ShowImageMargin.
		/// </remarks>
		/// <example>
		/// var m = new CatMenu();
		/// m.CMS.BackColor = Color.PaleGoldenrod;
		/// m["One"] = o => Out(o);
		/// m["Two"] = o => Out(o);
		/// using(m.Submenu("Submenu")) {
		/// 	m["Three"] = o => Out(o);
		/// 	m["Four"] = o => Out(o);
		/// 	using(m.Submenu("Submenu")) {
		/// 		m["Five"] = o => Out(o);
		/// 		m["Six"] = o => Out(o);
		/// 	}
		/// 	m["Seven"] = o => Out(o);
		/// }
		/// m["Eight"] = o => Out(o);
		/// m.Show();
		/// </example>
		public SubmenuBlock Submenu(string text, object icon = null, Action<ClickEventData> onClick = null)
		{
			ToolStripDropDownMenu_ dd;
			var item = _Submenu(out dd, text, onClick, icon);
			_submenuStack.Push(dd);
			return new SubmenuBlock(this, item);
		}

		ToolStripMenuItem _Submenu(out ToolStripDropDownMenu_ dd, string text, Action<ClickEventData> onClick, object icon)
		{
			var item = new ToolStripMenuItem(text);
			_Add(item, onClick, icon);

			dd = new ToolStripDropDownMenu_(this);
			item.DropDown = dd;

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
		/// Call this to end adding items to the current submenu if Submenu() was called without 'using' and without a callback function that adds submenu items.
		/// </summary>
		/// <example>
		/// var m = new CatMenu();
		/// m["One"] = o => Out(o);
		/// m["Two"] = o => Out(o);
		/// m.Submenu("Submenu");
		/// 	m["Three"] = o => Out(o);
		/// 	m["Four"] = o => Out(o);
		/// 	m.EndSubmenu();
		/// m["Five"] = o => Out(o);
		/// m.Show();
		/// </example>
		public void EndSubmenu()
		{
			var dd = _submenuStack.Pop();
		}

		//Allows to use code: using(m.Submenu(...)) { add items; }
		public class SubmenuBlock :IDisposable
		{
			CatMenu _cat;
			public ToolStripMenuItem MenuItem { get; }

			internal SubmenuBlock(CatMenu cat, ToolStripMenuItem mi) { _cat = cat; MenuItem = mi; }

			public void Dispose() { _cat.EndSubmenu(); }
		}

		/// <summary>
		/// Adds new item (ToolStripMenuItem) that will open a submenu.
		/// When showing the submenu first time, your callback function will be called and can add submenu items.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="onOpening">Lambda etc callback function that should add submenu items.</param>
		/// <param name="icon">The same as with <see cref="Add"/>.</param>
		/// <param name="onClick">Lambda etc function to be called when the menu item clicked. Rarely used.</param>
		/// <example>
		/// var m = new CatMenu();
		/// m["One"] = o => Out(o);
		/// m["Two"] = o => Out(o);
		/// m.Submenu("Submenu 1", m1 =>
		/// {
		/// 	Out("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 	m["Three"] = o => Out(o);
		/// 	m["Four"] = o => Out(o);
		/// 	m.Submenu("Submenu 2", m2 =>
		/// 	{
		/// 		Out("adding items of " + m.CurrentAddMenu.OwnerItem);
		/// 		m["Five"] = o => Out(o);
		/// 		m["Six"] = o => Out(o);
		/// 	});
		/// 	m["Seven"] = o => Out(o);
		/// });
		/// m["Eight"] = o => Out(o);
		/// m.Show();
		/// </example>
		public ToolStripMenuItem Submenu(string text, Action<CatMenu> onOpening, object icon = null, Action<ClickEventData> onClick = null)
		{
			ToolStripDropDownMenu_ dd;
			var item = _Submenu(out dd, text, onClick, icon);
			dd._lazySubmenuDelegate = onOpening;

			//add one item, or it will not work like a submenu parent item
			dd.SuspendLayout();
			dd.Items.Add(new ToolStripSeparator());
			dd.ResumeLayout(false);

			return item;
		}

		Stack<ToolStripDropDownMenu> _submenuStack = new Stack<ToolStripDropDownMenu>();

		bool _AddingSubmenuItems { get { return _submenuStack.Count > 0; } }

		/// <summary>
		/// Gets ToolStripDropDownMenu of the main menu or submenu where currently m.Add(...), m[...]=, m.Separator() and m.Submenu(...) would add new item.
		/// </summary>
		/// <remarks>
		/// Initially it is the main menu.
		/// In code of <c>using(m.Submenu(...)) { code; }</c>, <c>m.Submenu(...); code; m.EndSubmenu();</c> and <c>m.Submenu(..., m1 => { code; });</c> it is that submenu.
		/// </remarks>
		public ToolStripDropDownMenu CurrentAddMenu
		{
			get
			{
				_CheckDisposed(); //used by all Add(), Submenu()
				return _submenuStack.Count > 0 ? _submenuStack.Peek() : _cm;
			}
		}

		/// <summary>
		/// Gets the last added item as ToolStripMenuItem.
		/// Returns null if it is not a ToolStripMenuItem, eg a button or separator.
		/// The item can be added with m.Add(...), m[...]= and m.Submenu(...).
		/// </summary>
		/// <remarks>
		/// You can instead use LastItem, which gets ToolStripItem, which is the base class of all supported item types; cast it to a derived type if need.
		/// </remarks>
		public ToolStripMenuItem LastMenuItem { get { return LastItem as ToolStripMenuItem; } }

		#endregion

		#region show

		/// <summary>
		/// Shows the menu at the mouse cursor position.
		/// </summary>
		public void Show()
		{
			_Show(1);
		}

		/// <summary>
		/// Shows the menu at the specified position.
		/// </summary>
		/// <param name="x">Mouse X position in screen.</param>
		/// <param name="y">Mouse Y position in screen.</param>
		/// <param name="direction">Menu drop direction.</param>
		public void Show(int x, int y, ToolStripDropDownDirection direction = ToolStripDropDownDirection.Default)
		{
			_Show(2, x, y, direction);
		}

		/// <summary>
		/// Shows the menu in a form or control.
		/// </summary>
		/// <param name="owner">A control or form that will own the menu.</param>
		/// <param name="x">Mouse X position in control's client area.</param>
		/// <param name="y">Mouse Y position in control's client area.</param>
		/// <param name="direction">Menu drop direction.</param>
		/// <remarks>
		/// Alternatively you can assign the context menu to a control or toolstrip's drop-down button etc, then don't need to call Show(). Use the CMS property, which gets ContextMenuStrip.
		/// </remarks>
		public void Show(Control owner, int x, int y, ToolStripDropDownDirection direction = ToolStripDropDownDirection.Default)
		{
			_Show(3, x, y, direction, owner);
		}

		void _Show(int overload, int x = 0, int y = 0, ToolStripDropDownDirection direction = 0, Control control = null)
		{
			_CheckDisposed();
			if(_cm.Items.Count == 0) return;

			if(!_showedOnce) {
				//_showedOnce = true; //OnOpening() sets it
				if(!ActivateMenuWindow) Util.LibWorkarounds.WaitCursorWhenShowingMenuEtc();
			}
			Perf.Next(); //TODO

			_isOwned = control != null;
			_isModal = ModalAlways ? true : !Application.MessageLoop;

			_inOurShow = true;
			switch(overload) {
			case 1: _cm.Show(Mouse.XY); break;
			case 2: _cm.Show(new Point(x, y), direction); break;
			case 3: _cm.Show(control, new Point(x, y), direction); break;
			}
			_inOurShow = false;
			Perf.Next();

			if(_isModal) {
				_msgLoop.Loop();
				if(!MultiShow) Dispose();
			}
		}

		bool _showedOnce;
		bool _inOurShow; //to detect when ContextMenuStrip.Show called not through CatMenu.Show
		bool _isOwned; //control==0
		bool _isModal; //depends on ModalAlways or Application.MessageLoop
		Util.MessageLoop _msgLoop = new Util.MessageLoop();

		/// <summary>
		/// If false, calls Dispose() when the menu is closed.
		/// If true, does not call Dispose(); then you can call Show() multiple times for the same object.
		/// Default is false, but is automatically set to true when showing the menu not with CatMenu.Show(), eg when assigned to a control.
		/// </summary>
		/// <seealso cref="DefaultMultiShow"/>
		public bool MultiShow { get; set; } = DefaultMultiShow;

		/// <summary>
		/// Default MultiShow value for all new CatMenu instances.
		/// </summary>
		public static bool DefaultMultiShow { get; set; }

		/// <summary>
		/// If true, Show() always waits until the menu is closed.
		/// If false, does not wait if the thread has a message loop (Application.MessageLoop==true).
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
		/// Default ActivateMenuWindow value for CatMenu instances.
		/// A CatMenu instance inherits this at the moment it is created.
		/// </summary>
		public static bool DefaultActivateMenuWindow { get; set; }

		uint _ExStyle { get { return ActivateMenuWindow ? Api.WS_EX_TOOLWINDOW | Api.WS_EX_TOPMOST : Api.WS_EX_TOOLWINDOW | Api.WS_EX_TOPMOST | Api.WS_EX_NOACTIVATE; } }

		#endregion

		//Extends ContextMenuStrip of the main menu to change its behavior as we need.
		class ContextMenuStrip_ :ContextMenuStrip, _ICatToolStrip
		{
			CatMenu _cat;

			internal ContextMenuStrip_(CatMenu cat)
			{
				_cat = cat;
			}

			internal ContextMenuStrip_(CatMenu cat, IContainer container) : base(container)
			{
				_cat = cat;
			}

			protected override CreateParams CreateParams
			{
				get
				{
					//note: this func is called several times, first time before ctor
					//OutList("CreateParams", _cat, IsHandleCreated, p.ExStyle.ToString("X"));
					var p = base.CreateParams;
					if(_cat != null) p.ExStyle |= (int)_cat._ExStyle;
					return p;
				}
			}

			protected override void WndProc(ref Message m)
			{
				//Util.Debug_.OutMsg(ref m, Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_ERASEBKGND, Api.WM_CTLCOLOREDIT);

				if(_cat._WndProc_Before(true, this, ref m)) return;
				//var t = new Perf.Inst(true);
				base.WndProc(ref m);
				//t.Next(); if(t.TimeTotal >= 100) { OutList(t.Times, m); }
				_cat._WndProc_After(true, this, ref m);
			}

			protected override void OnOpening(CancelEventArgs e)
			{
				if(!e.Cancel) _cat._OnOpeningMain();
				base.OnOpening(e);

				//info: e.Cancel true if layout suspended. Then does not show, unless we set it false.
			}

			protected override void OnOpened(EventArgs e)
			{
				base.OnOpened(e);
				_cat._OnOpenedAny(false, this);
			}

			protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
			{
				_cat._OnClosingAny(false, this, e);
				base.OnClosing(e);
			}

			protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
			{
				base.OnClosed(e);
				_cat._OnClosedAny(false, this);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
				_cat._OnHandleCreatedDestroyed(true, this);
			}

			protected override void OnHandleDestroyed(EventArgs e)
			{
				base.OnHandleDestroyed(e);
				_cat._OnHandleCreatedDestroyed(false, this);
			}

			protected override void Dispose(bool disposing)
			{
				if(disposing && Visible) Close(); //else OnClosed not called etc
				base.Dispose(disposing);
				//OutList("menu disposed", disposing);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				//var perf = new Perf.Inst(true);

				//OutFunc();
				base.OnPaint(e);

				//perf.Next(); OutList("------------------ paint", perf.Times);

				_paintedOnce = true;
			}

			//ToolStrip _ICatToolStrip.ToolStrip { get { return this; } }

			bool _paintedOnce;
			bool _ICatToolStrip.PaintedOnce { get { return _paintedOnce; } }

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
		internal class ToolStripDropDownMenu_ :ToolStripDropDownMenu, _ICatToolStrip
		{
			CatMenu _cat;
			bool _openedOnce;
			internal Action<CatMenu> _lazySubmenuDelegate;

			internal ToolStripDropDownMenu_(CatMenu cat)
			{
				_cat = cat;
			}

			protected override CreateParams CreateParams
			{
				get
				{
					//note: this prop is called several times, first time before ctor
					//Out("CreateParams");
					var p = base.CreateParams;
					if(_cat != null) p.ExStyle |= (int)_cat._ExStyle;
					return p;
				}
			}

			protected override void WndProc(ref Message m)
			{
				if(_cat._WndProc_Before(false, this, ref m)) return;
				base.WndProc(ref m);
				_cat._WndProc_After(false, this, ref m);
			}

			protected override void OnOpening(CancelEventArgs e)
			{
				if(!_openedOnce && !e.Cancel) {
					_openedOnce = true;

					//call the caller-provided callback function that should add submenu items on demand
					if(_lazySubmenuDelegate != null) {
						Items.Clear(); //remove the placeholder separator
						_cat._submenuStack.Push(this);
						_lazySubmenuDelegate(_cat);
						_cat._submenuStack.Pop();
						_lazySubmenuDelegate = null;
					}

					PerformLayout();
				}

				if(AsyncIcons != null) {
					_cat._GetIconsAsync(this, AsyncIcons);
					AsyncIcons = null;
				}

				base.OnOpening(e);
			}

			//Base_CatMenu_CatBar creates this. We call GetAllAsync.
			internal List<Icons.AsyncIn> AsyncIcons { get; set; }

			protected override void OnOpened(EventArgs e)
			{
				base.OnOpened(e);
				_cat._OnOpenedAny(true, this);
			}

			protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
			{
				_cat._OnClosingAny(true, this, e);
				base.OnClosing(e);
			}

			protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
			{
				base.OnClosed(e);
				_cat._OnClosedAny(true, this);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
				_cat._OnHandleCreatedDestroyed(true, this);
			}

			protected override void OnHandleDestroyed(EventArgs e)
			{
				_cat._OnHandleCreatedDestroyed(false, this);
				base.OnHandleDestroyed(e);
			}

			//protected override void Dispose(bool disposing)
			//{
			//	base.Dispose(disposing);
			//	OutList("submenu disposed", disposing);
			//}

			protected override void OnPaint(PaintEventArgs e)
			{
				//OutFunc();
				base.OnPaint(e);
				_paintedOnce = true;
			}

			//ToolStrip _ICatToolStrip.ToolStrip { get { return this; } }

			bool _paintedOnce;
			bool _ICatToolStrip.PaintedOnce { get { return _paintedOnce; } }
		}

		#region wndproc

		//Called in WndProc before calling base.WndProc, for main and context menu.
		//If returns true, return without calling base.WndProc and _WndProc_After.
		bool _WndProc_Before(bool isMainMenu, ToolStripDropDownMenu dd, ref Message m)
		{
			if(isMainMenu) {
				switch((uint)m.Msg) {
				case Api.WM_HOTKEY:
					if(_OnHotkey((int)m.WParam)) return true;
					break;
				}
			}

			switch((uint)m.Msg) {
			case Api.WM_CLOSE:
				//OutList("WM_CLOSE", dd.Visible);
				if((int)m.WParam != _wmCloseWparam && dd.Visible) { Close(); return true; } //something tried to close from outside
				break;
			case Api.WM_WINDOWPOSCHANGING:
				unsafe
				{
					var p = (Api.WINDOWPOS*)m.LParam;
					//after right-click, enabling etc somebody tries to make the menu non-topmost by placing it after some window, eg a hidden tooltip (which is topmost...???)
					if((p->flags & Api.SWP_NOZORDER) == 0 && !p->hwndInsertAfter.Is0) {
						//OutList(p->hwndInsertAfter, p->hwndInsertAfter.IsTopmost);
						p->flags |= Api.SWP_NOZORDER;
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
				switch((uint)m.Msg) {
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

			switch((uint)m.Msg) {
			//case Api.WM_DESTROY:
			//	OutList("WM_DESTROY", isMainMenu, m.HWnd);
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

				//OutList(_cm.OwnerItem, _cm.SourceControl);
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
				//OutList(e.CloseReason, dd.Focused, dd.ContainsFocus, Wnd.ActiveWindow, Wnd.Get.RootOwnerOrThis(Wnd.ActiveWindow));
				switch(e.CloseReason) {
				case ToolStripDropDownCloseReason.AppClicked: //eg clicked a context menu item of a child textbox. Note: the AppClicked documentation lies; actually we receive this when clicked a window of this thread (or maybe this process, not tested).
					if(Wnd.ActiveWindow.Handle == dd.Handle) e.Cancel = true;
					break;
				case ToolStripDropDownCloseReason.AppFocusChange: //eg showed a dialog owned by this menu window
					var wa = Wnd.ActiveWindow;
					if(wa.Handle == dd.Handle) { //eg when closing this activated submenu because mouse moved to the parent menu
						if(isSubmenu) Api.SetForegroundWindow((Wnd)dd.OwnerItem.Owner.Handle); //prevent closing the parent menu
					} else if(Wnd.Get.RootOwnerOrThis(wa).Handle == dd.Handle) e.Cancel = true;
					break;
				}
				//Out(e.Cancel);
			}
		}

		void _OnOpenedAny(bool isSubmenu, ToolStripDropDownMenu dd)
		{
			if(isSubmenu) {
				_visibleSubmenus.Add(dd);
			} else {
				_InitUninitClosing(true);
				if(ActivateMenuWindow) ((Wnd)dd.Handle).ActivateRaw();
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

				if(!MultiShow && !_isModal) Time.SetTimer(10, true, o => { Dispose(); }); //cannot dispose now, exception

				if(_isModal) _msgLoop.Stop();
			}
		}

		void _OnHandleCreatedDestroyed(bool created, ToolStripDropDownMenu dd)
		{
			//OutList("_OnHandleCreatedDestroyed", created);
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
		/// Default is equal to CatMenu.DefaultMouseClosingDistance, default 200.
		/// </summary>
		/// <seealso cref="DefaultMouseClosingDistance"/>
		public int MouseClosingDistance { get; set; } = DefaultMouseClosingDistance;

		/// <summary>
		/// Default MouseClosingDistance value of CatMenu instances.
		/// A CatMenu instance inherits this at the moment it is created.
		/// </summary>
		public static int DefaultMouseClosingDistance { get; set; } = 200;

		List<ToolStripDropDownMenu> _visibleSubmenus = new List<ToolStripDropDownMenu>();
		Time.Timer_ _timer;
		bool _mouseWasIn;
		bool _hotkeyRegistered;
		const int _hotkeyEsc = 8405;

		//Called from OnOpened and OnClosed of main menu. Does nothing if already is in that state.
		void _InitUninitClosing(bool init)
		{
			if(init == (_timer != null)) return;
			//Out(init);
			_visibleSubmenus.Clear();
			_mouseWasIn = false;
			if(init) {
				_timer = Time.SetTimer(100, false, _OnTimer);
				if(!(_isOwned || ActivateMenuWindow)) _hotkeyRegistered = Api.RegisterHotKey((Wnd)_cm.Handle, _hotkeyEsc, 0, Api.VK_ESCAPE);
			} else {
				if(_timer != null) { _timer.Stop(); _timer = null; }
				if(_hotkeyRegistered) _hotkeyRegistered = !Api.UnregisterHotKey((Wnd)_cm.Handle, _hotkeyEsc);
			}
		}

		//100 ms
		void _OnTimer(Time.Timer_ t)
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

			Api.GUITHREADINFO g;
			if(Wnd.Misc.GetGUIThreadInfo(out g, Api.GetCurrentThreadId()) && !g.hwndMenuOwner.Is0) {
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

	/// <summary>
	/// Base class of CatMenu and CatBar.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public abstract class Base_CatMenu_CatBar
	{
		internal bool m_inRightClick;
		EventHandler _onClick;
		System.Collections.Hashtable _clickDelegates = new System.Collections.Hashtable();

		internal Base_CatMenu_CatBar()
		{
			_onClick = _OnClick;
		}

		//Common Click even handler of all items.
		//Calls true item's onClick delegate if need.
		void _OnClick(object sender, EventArgs args)
		{
			//Out(_inRightClick);
			if(m_inRightClick) return;
			var d = _clickDelegates[sender] as Action<ClickEventData>;
			Debug.Assert(d != null); if(d == null) return;
			d(new ClickEventData(sender as ToolStripItem));
		}

		/// <summary>
		/// Data passed to Click event handler functions.
		/// </summary>
		public class ClickEventData
		{
			/// <summary>
			/// Gets the clicked item as ToolStripItem.
			/// </summary>
			public ToolStripItem Item { get; }

			/// <summary>
			/// Gets the clicked item as ToolStripMenuItem.
			/// Returns null if it is not ToolStripMenuItem.
			/// </summary>
			public ToolStripMenuItem MenuItem { get { return Item as ToolStripMenuItem; } }

			internal ClickEventData(ToolStripItem item) { Item = item; }

			/// <summary>
			/// Gets item text.
			/// </summary>
			public override string ToString()
			{
				return Item.ToString();
			}
		}

		//Gets ToolStrip from CatMenu and CatBar, which override this.
		protected abstract ToolStrip MainToolStrip { get; }

		/// <summary>
		/// Gets the last added item as ToolStripItem, which is the base type of ToolStripMenuItem, ToolStripButton and other supported types.
		/// The item can be added with m.Add(...), m[...]=, m.Separator() and m.Submenu(...).
		/// </summary>
		public ToolStripItem LastItem { get; protected set; }

		/// <summary>
		/// Occurs when an item is added.
		/// Allows to set item properties in single place instead of after each 'add item' code line.
		/// For example, the event handler can set item properties common to all items, or set item properties encoded in item text.
		/// </summary>
		public event Action<ToolStripItem> ItemAdded;

		/// <summary>
		/// ContextMenu to show when right-clicked.
		/// //todo: how to know which item clicked? Maybe add a common default context menu. Currently not used.
		/// </summary>
		//public ContextMenu ContextMenu
		//{
		//	get { return MainToolStrip.ContextMenu; }
		//	set { MainToolStrip.ContextMenu = value; }
		//}

		/// <summary>
		/// Folder path to prepend to icon filenames specified when adding items.
		/// </summary>
		public string IconDirectory { get; set; }

		/// <summary>
		/// Flags to pass to <see cref="Icons.GetIconHandle"/>. See <see cref="Icons.IconFlag"/>.
		/// </summary>
		public Icons.IconFlag IconFlags { get; set; }

		/// <summary>
		/// Image width and height.
		/// Also can be enum <see cref="Icons.ShellSize"/>, cast to int.
		/// Set it before adding items.
		/// </summary>
		/// <remarks>
		/// To set different size for a submenu: <c>using(m.Submenu("sub")) { m.LastMenuItem.DropDown.ImageScalingSize = new Size(24, 24);</c>
		/// </remarks>
		public int IconSize
		{
			get { return MainToolStrip.ImageScalingSize.Width; }
			set { MainToolStrip.ImageScalingSize = new Size(value, value); }
		}

		//Sets icon and onClick delegate.
		//Sets LastItem.
		//Calls ItemAdded event handlers.
		internal void _SetItemProp(bool isBar, ToolStripItem item, Action<ClickEventData> onClick, object icon)
		{
			if(onClick != null) {
				_clickDelegates[item] = onClick;
				item.Click += _onClick;
			}

#if true //to quickly disable icons when measuring speed
			if(icon != null) {
				try {
					var s = icon as string;
					if(s != null) {
						_SetItemFileIcon(isBar, item, s);
					} else if(icon is int) {
						int i = (int)icon;
						if(i >= 0) item.ImageIndex = i;
					} else if(icon is Image) {
						item.Image = icon as Image;
					} else if(icon is Icon) {
						item.Image = (icon as Icon).ToBitmap();
					} else if(icon is IntPtr) {
						var hi = (IntPtr)icon;
						item.Image = hi == Zero ? null : Icon.FromHandle(hi).ToBitmap();
					} else {
						s = icon.ToString();
						if(0 != Files.FileOrDirectoryExists(s)) _SetItemFileIcon(isBar, item, s);
					}
				}
				catch(Exception e) { OutDebug(e.Message); } //ToBitmap() may throw
			}
#endif
			LastItem = item;

			if(ItemAdded != null) ItemAdded(item);
		}

		void _SetItemFileIcon(bool isBar, ToolStripItem item, string s)
		{
			if(Empty(s)) return;
			var owner = item.Owner;
			var il = owner.ImageList;
			if(il != null && il.Images.ContainsKey(s)) {
				item.ImageKey = s;
			} else {
				//var perf = new Perf.Inst(true);
				item.ImageScaling = ToolStripItemImageScaling.None; //we'll get icons of correct size, except if size is 256 and such icon is unavailable, then show smaller

				if(IconDirectory != null && !Path_.IsFullPath(s)) s = Path_.Combine(IconDirectory, s);

				if(_AsyncIcons == null) _AsyncIcons = new Icons.AsyncIcons(); //used by submenus too
				var submenu = !isBar ? (owner as CatMenu.ToolStripDropDownMenu_) : null;
				bool isFirstImage = false;

				if(submenu == null) {
					if(_AsyncIcons.Count == 0) isFirstImage = true;
					_AsyncIcons.Add(s, item);
				} else {
					if(submenu.AsyncIcons == null) {
						submenu.AsyncIcons = new List<Icons.AsyncIn>();
						isFirstImage = true;
					}
					submenu.AsyncIcons.Add(new Icons.AsyncIn(s, item));
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
		Icons.AsyncIcons _AsyncIcons { get; set; }

		//list - used by submenus.
		internal void _GetIconsAsync(ToolStrip ts, List<Icons.AsyncIn> list = null)
		{
			if(_AsyncIcons == null) return;
			if(list != null) _AsyncIcons.Add(list);
			if(_AsyncIcons.Count == 0) return;
			_AsyncIcons.GetAllAsync(_AsyncCallback, ts.ImageScalingSize.Width, IconFlags, ts);
		}

		void _AsyncCallback(Icons.AsyncResult r, object objCommon, int nLeft)
		{
			var ts = objCommon as ToolStrip;
			var item = r.obj as ToolStripItem;

			//OutList(r.image, r.hIcon);
			//Image im = r.image;
			//if(im == null && r.hIcon != Zero) im = Icons.HandleToImage(r.hIcon);

			Image im = Icons.HandleToImage(r.hIcon);

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
				Perf.Next();
				ts.Update();
				Perf.NW();
			}
		}

		void _SetItemIcon(ToolStrip ts, ToolStripItem item, Image im)
		{
			Wnd w = Wnd0;
			var its = ts as _ICatToolStrip;
			if(its.PaintedOnce) {
				if(_region1 == Zero) _region1 = Api.CreateRectRgn(0, 0, 0, 0);
				if(_region2 == Zero) _region2 = Api.CreateRectRgn(0, 0, 0, 0);

				w = (Wnd)ts.Handle;
				Api.GetUpdateRgn(w, _region1, false);
			}

			//RECT u;
			//Api.GetUpdateRect((Wnd)ts.Handle, out u, false); OutList(its.PaintedOnce, u);

			ts.SuspendLayout(); //without this much slower, especially when with overflow arrows (when many items)
			item.Image = im;
			ts.ResumeLayout(false);

			if(its.PaintedOnce) {
				Api.ValidateRect(w); //tested: with WM_SETREDRAW 3 times slower
				RECT r = item.Bounds; //r.Inflate(-2, -1);
									  //r.right = r.left + r.Height; //same speed
				Api.SetRectRgn(_region2, r.left, r.top, r.right, r.bottom);
				Api.CombineRgn(_region1, _region1, _region2, Api.RGN_OR);

				//RECT b; GetRgnBox(_region1, out b); OutList(b, _region1);

				Api.InvalidateRgn(w, _region1, false);
			}

			//Api.GetUpdateRect((Wnd)ts.Handle, out u, false); OutList("after", u);
		}

		IntPtr _region1, _region2;
		internal List<Image> _images;
		bool _isDisposed;

		internal void _Dispose(bool disposing)
		{
			//OutList("_Dispose", _isDisposed);
			if(_isDisposed) return;
			_isDisposed = true;

			if(disposing) {
				if(_AsyncIcons != null) _AsyncIcons.Dispose();

				if(_images != null) {
					foreach(var im in _images) im.Dispose();
					_images = null;
				}
			}

			if(_region1 != Zero) Api.DeleteObject(_region1);
			if(_region2 != Zero) Api.DeleteObject(_region2);

			LastItem = null;
		}

		~Base_CatMenu_CatBar() { /*Out("base dtor");*/ _Dispose(false); }
	}

	interface _ICatToolStrip
	{
		//ToolStrip ToolStrip { get; } //currently not used; we use MainToolStrip instead.
		bool PaintedOnce { get; }
	}
}
