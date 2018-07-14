//Class Wnd.Get and several related Wnd functions.

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
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	public partial struct Wnd
	{
		/// <summary>
		/// Gets or sets the owner window of this top-level window.
		/// </summary>
		/// <exception cref="WndException">Failed (only 'set' function).</exception>
		/// <remarks>
		/// A window that has an owner window is always on top of its owner window.
		/// Don't call this for controls, they don't have an owner window.
		/// The 'get' function returns default(Wnd) if this window isn't owned or is invalid. Supports <see cref="Native.GetError"/>.
		/// The 'set' function can fail, eg if the owner's process has higher <see cref="Process_.UacInfo">UAC</see> integrity level.
		/// </remarks>
		public Wnd WndOwner
		{
			get => Api.GetWindow(this, Api.GW_OWNER);
			set { SetWindowLong(Native.GWL.HWNDPARENT, (LPARAM)value); }
		}

		/// <summary>
		/// Gets the top-level parent window of this control.
		/// If this is a top-level window, returns this. Returns default(Wnd) if this window is invalid.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndWindow
		{
			get
			{
				var w = Api.GetAncestor(this, Api.GA_ROOT);
				if(w.Is0 && this == Misc.WndRoot) w = this;
				return w;
			}
		}

		/// <summary>
		/// Returns true if this is a child window (control), false if top-level window.
		/// </summary>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>.
		/// Uses <see cref="WndDirectParent"/>.
		/// Another way is <c>w.HasStyle(Native.WS.CHILD)</c>. It is faster but less reliable, because some top-level windows have WS_CHILD style and some child windows don't.
		/// </remarks>
		public bool IsChild => !WndDirectParent.Is0;

		/// <summary>
		/// Returns true if this is a direct or indirect child (descendant) of window w.
		/// Calls API <msdn>IsChild</msdn>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public bool IsChildOf(Wnd w) { return Api.IsChild(w, this); }

		/// <summary>
		/// Gets the window or control that is the direct parent of this control.
		/// Returns default(Wnd) if this is a top-level window.
		/// If you need the top-level parent window, use <see cref="WndWindow"/> instead.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndDirectParent
		{
			get
			{
				Wnd R = Api.GetParent(this);
				if(R.Is0 || R == Api.GetWindow(this, Api.GW_OWNER) || R == _wDesktop) return default;
				return R;
				//tested: GetAncestor much slower. IsChild also slower.
				//About 'R == _wDesktop': it is rare, eg combolbox.
				//	If with createwindow owner is 0 or desktop, both getparent and getwindow return 0.
				//	But if with setwindowlong owner is desktop, both return desktop.
			}
		}

		static Wnd _wDesktop = Api.GetDesktopWindow();

		/// <summary>
		/// Gets the window or control that is the direct parent of this control or owner of this top-level window.
		/// Calls API <msdn>GetParent</msdn>. Faster than <see cref="WndDirectParent"/>.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndDirectParentOrOwner => Api.GetParent(this);

		/// <summary>
		/// Gets the window of the same type that is highest in the Z order.
		/// If this is a top-level window, gets first top-level window, else gets first control of the same direct parent.
		/// If this is the first, returns this, not default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDFIRST).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndFirstSibling => Api.GetWindow(this, Api.GW_HWNDFIRST);

		/// <summary>
		/// Gets the window of the same type that is lowest in the Z order.
		/// If this is a top-level window, gets last top-level window, else gets last control of the same direct parent.
		/// If this is the last, returns this, not default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDLAST).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndLastSibling => Api.GetWindow(this, Api.GW_HWNDLAST);

		/// <summary>
		/// Gets the window of the same type that is next (below this) in the Z order.
		/// If this is a top-level window, gets next top-level window, else gets next control of the same direct parent.
		/// If this is the last, returns default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDNEXT).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndNext => Api.GetWindow(this, Api.GW_HWNDNEXT);

		/// <summary>
		/// Gets the window of the same type that is previous (above this) in the Z order.
		/// If this is a top-level window, gets previous top-level window, else gets previous control of the same direct parent.
		/// If this is the first, returns default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDPREV).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndPrev => Api.GetWindow(this, Api.GW_HWNDPREV);

		/// <summary>
		/// Gets the child control at the top of the Z order.
		/// Returns default(Wnd) if no children.
		/// The same as <see cref="WndChild"/>(0).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_CHILD).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndFirstChild => Api.GetWindow(this, Api.GW_CHILD);

		/// <summary>
		/// Gets the child control at the bottom of the Z order.
		/// Returns default(Wnd) if no children.
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndLastChild { get { var c = Api.GetWindow(this, Api.GW_CHILD); return c.Is0 ? c : Api.GetWindow(c, Api.GW_HWNDLAST); } }

		/// <summary>
		/// Gets the child control at the specified position in the Z order.
		/// Returns default(Wnd) if no children or if index is invalid.
		/// </summary>
		/// <param name="index">0-based index of the child control in the Z order.</param>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndChild(int index)
		{
			if(index < 0) return default;
			Wnd c = Api.GetWindow(this, Api.GW_CHILD);
			for(; index > 0 && !c.Is0; index--) c = Api.GetWindow(c, Api.GW_HWNDNEXT);
			return c;
		}

		/// <summary>
		/// Gets the active (foreground) window.
		/// Calls API <msdn>GetForegroundWindow</msdn>.
		/// Returns default(Wnd) if there is no active window; more info: <see cref="Misc.WaitForAnActiveWindow"/>.
		/// </summary>
		public static Wnd WndActive => Api.GetForegroundWindow();

		/// <summary>
		/// Returns true if this window is the active (foreground) window.
		/// </summary>
		public bool IsActive => !Is0 && this == Api.GetForegroundWindow();

		/// <summary>
		/// Returns true if this window is the active (foreground) window.
		/// If this is <see cref="Misc.WndRoot"/>, returns true if there is no active window.
		/// </summary>
		internal bool LibIsActiveOrNoActiveAndThisIsWndRoot
		{
			get
			{
				if(Is0) return false;
				var f = Api.GetForegroundWindow();
				return this == (f.Is0 ? Misc.WndRoot : f);
			}
		}

		public static partial class Misc
		{
			/// <summary>
			/// Gets the very first top-level window in the Z order.
			/// Usually it is a topmost window.
			/// Calls API <msdn>GetTopWindow</msdn>(default(Wnd)).
			/// </summary>
			public static Wnd WndTop => Api.GetTopWindow(default);

			//rejected: not useful
			///// <summary>
			///// Gets an enabled owned window in the chain of windows owned by w, or w itself if there are no such windows.
			///// Calls API <msdn>GetWindow</msdn>(w, GW_ENABLEDPOPUP).
			///// </summary>
			///// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
			//public static Wnd WndEnabledOwnedOrThis(Wnd w) { return Api.GetWindow(w, Api.GW_ENABLEDPOPUP); }

			/// <summary>
			/// Gets the most recently active window in the chain of windows owned by w, or w itself if there are no such windows.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="includeOwners">Can return an owner (or owner's owner and so on) of w too.</param>
			/// <remarks>
			/// Supports <see cref="Native.GetError"/>.
			/// </remarks>
			public static Wnd WndLastActiveOwnedOrThis(Wnd w, bool includeOwners = false)
			{
				var wRoot = WndRootOwnerOrThis(w); //always use the root owner because GetLastActivePopup always returns w if w is owned
				var R = Api.GetLastActivePopup(wRoot);
				if(!includeOwners) {
					if(R != w && wRoot != w && !R.Is0) {
						for(var t = w; !t.Is0; t = t.WndOwner) if(t == R) return w;
					}
				}
				return R;
			}

			/// <summary>
			/// Gets the most bottom owner window in the chain of owner windows of w.
			/// If w is not owned, returns w. If w is invalid, returns default(Wnd).
			/// </summary>
			/// <param name="w"></param>
			/// <param name="supportControls">If w is a child window, use its top-level parent window instead.</param>
			/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
			public static Wnd WndRootOwnerOrThis(Wnd w, bool supportControls = false)
			{
				//return Api.GetAncestor(w, Api.GA_ROOTOWNER); //slow, and can return WndRoot, eg for combolbox

				if(supportControls) {
					var r = Api.GetAncestor(w, Api.GA_ROOTOWNER);
					return r == _wDesktop ? w : r;
					//never mind speed, better make it simple
				} else { //fast
					for(Wnd r = w, t; ; r = t) {
						t = r.WndOwner;
						if(t.Is0) return r.IsAlive ? r : default;
					}
				}
			}

			/// <summary>
			/// Gets the virtual parent window of all top-level windows.
			/// Calls API <msdn>GetDesktopWindow</msdn>.
			/// <note>It is not the visible desktop window (see <see cref="WndDesktop"/>)</note>
			/// </summary>
			public static Wnd WndRoot => _wDesktop;

			/// <summary>
			/// Gets a window of the shell process (usually process "explorer", class name "Progman").
			/// Calls API <msdn>GetShellWindow</msdn>.
			/// <note>In most cases it is not the window that contains desktop icons (see <see cref="WndDesktop"/>). But it belongs to the same thread.</note>
			/// </summary>
			public static Wnd WndShell => Api.GetShellWindow();

			/// <summary>
			/// Gets the desktop window.
			/// It displays desktop icons and wallpaper in its child control <see cref="WndDesktopControl"/>. The "Show Desktop" command (Win+D) activates it.
			/// <note>It is not API <msdn>GetDesktopWindow</msdn> (see <see cref="WndRoot"/>)</note>
			/// </summary>
			/// <remarks>
			/// <note>This function is not very reliable. May stop working on a new Windows version or don't work with a custom shell.</note>
			/// </remarks>
			public static Wnd WndDesktop => _WndDesktop(out var lv);

			/// <summary>
			/// Gets the control of "SysListView32" class that contains desktop icons and wallpaper. It is a child of <see cref="WndDesktop"/>.
			/// </summary>
			/// <remarks>
			/// <note>This function is not very reliable. May stop working on a new Windows version or don't work with a custom shell.</note>
			/// </remarks>
			public static Wnd WndDesktopControl { get { _WndDesktop(out var lv); return lv; } }

			static Wnd _WndDesktop(out Wnd lvControl)
			{
				Wnd w = WndShell;
				var f = new ChildFinder(className: "SysListView32");
				if(!f.Find(w)) w = Wnd.Find(null, "WorkerW", WFEtc.Thread(w.ThreadId), also: t => f.Find(t));
				lvControl = f.Result;
				return w;

				//info:
				//If no wallpaper, desktop is GetShellWindow, else a visible WorkerW window.
				//When was no wallpaper and user selects a wallpaper, explorer creates WorkerW and moves the same SysListView32 control to it.
			}

			/// <summary>
			/// Gets all owner windows of the specified window.
			/// Returns array that starts with the specified window or its top-level parent (if control).
			/// </summary>
			/// <param name="w">Window or control. If control, its top-level parent window will be the first in the array.</param>
			/// <param name="onlyVisible">Skip invisible windows.</param>
			/// <remarks>
			/// This function for example can be used to temporarily hide a tool window and its owners when capturing something from the screen.
			/// The array does not include <see cref="WndRoot"/>.
			/// </remarks>
			public static Wnd[] OwnerWindowsAndThis(Wnd w, bool onlyVisible = false)
			{
				var a = new List<Wnd>();
				for(w = w.WndWindow; !w.Is0 && w != WndRoot; w = w.WndOwner)
					if(!onlyVisible || w.IsVisible) a.Add(w);
				return a.ToArray();
			}

			//FUTURE: impl these:

			//public static Wnd AuManager { get { return ; } }

			//public static Wnd AuEditor { get { return ; } }

			//public static Wnd AuCodeEditControl { get { return ; } }

			/// <summary>
			/// Returns true if window w is considered a main window, ie probably is in the Windows taskbar.
			/// Returns false if it is invisible, cloaked, owned, toolwindow, menu, etc.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="allDesktops">On Windows 10 include (return true for) windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has <see cref="Process_.UacInfo">UAC</see> integrity level uiAccess).</param>
			/// <param name="skipMinimized">Return false if w is minimized.</param>
			public static bool IsMainWindow(Wnd w, bool allDesktops = false, bool skipMinimized = false)
			{
				if(!w.IsVisible) return false;

				var exStyle = w.ExStyle;
				if((exStyle & Native.WS_EX.APPWINDOW) == 0) {
					if((exStyle & (Native.WS_EX.TOOLWINDOW | Native.WS_EX.NOACTIVATE)) != 0) return false;
					if(!w.WndOwner.Is0) return false;
				}

				if(skipMinimized && w.IsMinimized) return false;

				if(Ver.MinWin10) {
					if(w.IsCloaked) {
						if(!allDesktops) return false;
						if((exStyle & Native.WS_EX.NOREDIRECTIONBITMAP) != 0) { //probably a store app
							switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
							case 1: return false; //Windows search, experience host, etc. Also app windows that normally would sit on ApplicationFrameWindow windows.
							case 2: if(_WindowsStoreAppFrameChild(w).Is0) return false; break; //skip hosts
							}
						}
					}
				} else if(Ver.MinWin8) {
					if((exStyle & Native.WS_EX.NOREDIRECTIONBITMAP) != 0 && !w.HasStyle(Native.WS.CAPTION)) {
						if(!allDesktops && (exStyle & Native.WS_EX.TOPMOST) != 0) return false; //skip store apps
						if(WndShell.GetThreadProcessId(out var pidShell) != 0 && w.GetThreadProcessId(out var pid) != 0 && pid == pidShell) return false; //skip captionless shell windows
					}
					//On Win8 impossible to get next window like Alt+Tab.
					//	All store apps are topmost, covering non-topmost desktop windows.
					//	DwmGetWindowAttribute makes no sense here.
					//	Desktop windows are never cloaked, inactive store windows are cloaked, etc.
				}

				return true;
			}

			/// <summary>
			/// Gets main windows, ie those that probably are in the Windows taskbar.
			/// Returns array containing 0 or more Wnd.
			/// </summary>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has <see cref="Process_.UacInfo">UAC</see> integrity level uiAccess).</param>
			/// <remarks>
			/// Uses <see cref="IsMainWindow"/>.
			/// Does not match the order of buttons in the Windows taskbar.
			/// </remarks>
			public static Wnd[] MainWindows(bool allDesktops = false)
			{
				var a = new List<Wnd>();
				foreach(var w in AllWindows(onlyVisible: true)) {
					if(IsMainWindow(w, allDesktops: allDesktops)) a.Add(w);
				}
				return a.ToArray();

				//Another way - UI Automation:
				//	var x = new CUIAutomation();
				//	var cond = x.CreatePropertyCondition(30003, 0xC370); //UIA_ControlTypePropertyId, UIA_WindowControlTypeId
				//	var a = x.GetRootElement().FindAll(TreeScope.TreeScope_Children, cond);
				//	for(int i = 0; i < a.Length; i++) Print((Wnd)a.GetElement(i).CurrentNativeWindowHandle);
				//Advantages: 1. Maybe can filter unwanted windows more reliably, although I did't notice a difference.
				//Disadvantages: 1. Skips windows of higher integrity level (UAC). 2. Cannot include cloaked windows, eg those in inactive Win10 virtual desktops. 3. About 1000 times slower, eg 70 ms vs 70 mcs; cold 140 ms.
			}

			/// <summary>
			/// Gets next window in the Z order, skipping invisible and other windows that probably are not in the Windows taskbar.
			/// Returns default(Wnd) if there are no such windows.
			/// </summary>
			/// <param name="w">Start from this window. If default(Wnd), starts from the top of the Z order.</param>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has <see cref="Process_.UacInfo">UAC</see> integrity level uiAccess).</param>
			/// <param name="skipMinimized">Skip minimized windows.</param>
			/// <param name="retryFromTop">If w is not default(Wnd) and there are no matching windows after it, retry from the top of the Z order. Then can return w.</param>
			/// <remarks>
			/// Uses <see cref="IsMainWindow"/>.
			/// This function is quite slow. Does not match the order of buttons in the Windows taskbar.
			/// </remarks>
			public static Wnd WndNextMain(Wnd w = default, bool allDesktops = false, bool skipMinimized = false, bool retryFromTop = false)
			{
				if(w.Is0) retryFromTop = false;

				for(; ; ) {
					w = w.Is0 ? WndTop : w.WndNext;
					if(w.Is0) {
						if(retryFromTop) { retryFromTop = false; continue; }
						return default;
					}
					if(IsMainWindow(w, allDesktops: allDesktops, skipMinimized: skipMinimized)) return w;
				}
			}

			/// <summary>
			/// Activates next non-minimized main window, like with Alt+Tab.
			/// Returns true if activated, false if there is no such window or failed to activate.
			/// </summary>
			/// <remarks>
			/// Uses <see cref="WndNextMain"/>, <see cref="WndLastActiveOwnedOrThis"/>, <see cref="Activate()"/>.
			/// An alternative way - send Alt+Tab keys, but it works not everywhere.
			/// </remarks>
			public static bool SwitchActiveWindow()
			{
				try {
					Wnd wActive = WndActive, wRO = WndRootOwnerOrThis(wActive);
					Wnd wMain = WndNextMain(wRO, skipMinimized: true, retryFromTop: true);
					if(!wMain.Is0 && wMain != wActive && wMain != wRO) {
						var wMainOrOwned = WndLastActiveOwnedOrThis(wMain);
						if(!wMainOrOwned.Is0) {
							wMainOrOwned.Activate();
							return true;
						}
					}
				}
				catch(WndException) { }
				return false;

				//notes:
				//This function ignores minimized windows, because:
				//	Impossible to get exactly the same window as Alt+Tab, which on Win10 is the most recently minimized.
				//	Activating a random minimized window is not very useful. Maybe in the future.
				//The order of windows used by Alt+Tab is not the same as the Z order, especially when there are minimized windows.
				//	We cannot get that order and have to use the Z order. It seems that the order is different on Win10 but not on Win7.
				//After minimizing a window its position in the Z order is undefined.
				//	Most windows then are at the bottom when used the Minimize button or ShowWindow etc.
				//	But some are at the top, just after the active window, for example MS Document Explorer (Win7 SDK).
				//	Also at the top when minimized with the taskbar button.
			}
		}
	}
}
