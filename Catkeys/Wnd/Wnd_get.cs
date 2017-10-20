//Class Wnd.Get and several related Wnd functions.

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
//using System.Linq;

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	public partial struct Wnd
	{
		/// <summary>
		/// Gets or sets the owner window of this top-level window.
		/// A window that has an owner window is always on top of its owner window.
		/// Don't call this for controls, they don't have an owner window.
		/// The 'get' function supports <see cref="Native.GetError"/>.
		/// The 'set' function can fail, eg if the owner's process has higher UAC integrity level.
		/// </summary>
		/// <exception cref="WndException">Failed (only 'set' function).</exception>
		public Wnd WndOwner
		{
			get => Api.GetWindow(this, Api.GW_OWNER);
			set { SetWindowLong(Native.GWL_HWNDPARENT, (LPARAM)value); }
		}

		/// <summary>
		/// Gets the top-level parent window of this control.
		/// If this is a top-level window, returns this. Returns default(Wnd) if this is invalid.
		/// Calls API <msdn>GetAncestor</msdn>(GA_ROOT). Unlike the API, this function does not return default(Wnd) when this is Misc.WndRoot.
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
		/// Another way is <c>w.HasStyle(Native.WS_CHILD)</c>. It is faster but less reliable, because some top-level windows have WS_CHILD style and some child windows don't.
		/// </remarks>
		public bool IsChildWindow { get => !WndDirectParent.Is0; }

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
				//About 'R == _wDesktop': it is rare but I have seen that desktop is owner.
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
		public Wnd WndDirectParentOrOwner { get => Api.GetParent(this); }

		/// <summary>
		/// Gets the window of the same type that is highest in the Z order.
		/// If this is a top-level window, gets first top-level window, else gets first control of the same direct parent.
		/// If this is the first, returns this, not default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDFIRST).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndFirstSibling { get => Api.GetWindow(this, Api.GW_HWNDFIRST); }

		/// <summary>
		/// Gets the window of the same type that is lowest in the Z order.
		/// If this is a top-level window, gets last top-level window, else gets last control of the same direct parent.
		/// If this is the last, returns this, not default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDLAST).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndLastSibling { get => Api.GetWindow(this, Api.GW_HWNDLAST); }

		/// <summary>
		/// Gets the window of the same type that is next (below this) in the Z order.
		/// If this is a top-level window, gets next top-level window, else gets next control of the same direct parent.
		/// If this is the last, returns default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDNEXT).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndNext { get => Api.GetWindow(this, Api.GW_HWNDNEXT); }

		/// <summary>
		/// Gets the window of the same type that is previous (above this) in the Z order.
		/// If this is a top-level window, gets previous top-level window, else gets previous control of the same direct parent.
		/// If this is the first, returns default(Wnd).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDPREV).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndPrev { get => Api.GetWindow(this, Api.GW_HWNDPREV); }

		/// <summary>
		/// Gets the child control at the top of the Z order.
		/// Returns default(Wnd) if no children.
		/// The same as <see cref="WndChild">WndChild</see>(0).
		/// Calls API <msdn>GetWindow</msdn>(this, GW_CHILD).
		/// </summary>
		/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
		public Wnd WndFirstChild { get => Api.GetWindow(this, Api.GW_CHILD); }

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
		public static Wnd WndActive { get => Api.GetForegroundWindow(); }

		/// <summary>
		/// Returns true if this window is the active (foreground) window.
		/// </summary>
		public bool IsActive { get => !Is0 && this == Api.GetForegroundWindow(); }

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
			/// Gets the active window of this thread.
			/// Calls API <msdn>GetActiveWindow</msdn>.
			/// </summary>
			public static Wnd WndActiveOfThisThread { get => Api.GetActiveWindow(); }

			/// <summary>
			/// Gets the very first top-level window in the Z order.
			/// Usually it is a topmost window.
			/// Calls API <msdn>GetTopWindow</msdn>(default(Wnd)).
			/// </summary>
			public static Wnd WndTop { get => Api.GetTopWindow(default); }

			/// <summary>
			/// Gets the first (top) enabled window in the chain of windows owned by w, or w itself if there are no such windows.
			/// Calls API <msdn>GetWindow</msdn>(w, GW_ENABLEDPOPUP).
			/// </summary>
			/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
			public static Wnd WndFirstEnabledOwnedOrThis(Wnd w) { return Api.GetWindow(w, Api.GW_ENABLEDPOPUP); }

			/// <summary>
			/// Gets the most recently active window owned by w, or w itself if it was the most recently active.
			/// Calls API <msdn>GetLastActivePopup</msdn>(w).
			/// </summary>
			/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
			public static Wnd WndLastSeenActiveOwnedOrThis(Wnd w) { return Api.GetLastActivePopup(w); }

			/// <summary>
			/// Gets the most bottom owner window in the chain of owner windows of w. If w is not owned, returns w.
			/// Calls API <msdn>GetAncestor</msdn>(GA_ROOTOWNER).
			/// </summary>
			/// <remarks>Supports <see cref="Native.GetError"/>.</remarks>
			public static Wnd WndRootOwnerOrThis(Wnd w) { return Api.GetAncestor(w, Api.GA_ROOTOWNER); }

			/// <summary>
			/// Gets the virtual parent window of all top-level windows.
			/// Calls API <msdn>GetDesktopWindow</msdn>.
			/// <note>It is not the visible desktop window (see <see cref="WndDesktop"/>)</note>.
			/// </summary>
			public static Wnd WndRoot { get => _wDesktop; }

			/// <summary>
			/// Gets a window of the shell process (usually process "explorer", class name "Progman").
			/// Calls API <msdn>GetShellWindow</msdn>.
			/// <note>In most cases it is not the window that contains desktop icons (see <see cref="WndDesktop"/>). But it belongs to the same thread.</note>
			/// </summary>
			public static Wnd WndShell { get => Api.GetShellWindow(); }

			/// <summary>
			/// Gets the desktop window.
			/// It displays desktop icons and wallpaper in its child control <see cref="WndDesktopControl"/>. The "Show Desktop" command (Win+D) activates it.
			/// <note>It is not the same as API <msdn>GetDesktopWindow</msdn> (see <see cref="WndRoot"/>)</note>.
			/// <note>This function is not very reliable. May stop working on a new Windows version or don't work with a custom shell.</note>
			/// </summary>
			public static Wnd WndDesktop { get => _WndDesktop(out var lv); }

			/// <summary>
			/// Gets the control of "SysListView32" class that contains desktop icons and wallpaper. It is a child of <see cref="WndDesktop"/>.
			/// <note>This function is not very reliable. May stop working on a new Windows version or don't work with a custom shell.</note>
			/// </summary>
			public static Wnd WndDesktopControl { get { _WndDesktop(out var lv); return lv; } }

			static Wnd _WndDesktop(out Wnd lvControl)
			{
				Wnd w = WndShell;
				var f = new ChildFinder(className: "SysListView32");
				if(!f.FindIn(w)) w = Wnd.Find(null, "WorkerW", WFOwner.ThreadId(w.ThreadId), also: t => f.FindIn(t));
				lvControl = f.Result;
				return w;

				//info:
				//If no wallpaper, desktop is GetShellWindow, else a visible WorkerW window.
				//When was no wallpaper and user selects a wallpaper, explorer creates WorkerW and moves the same SysListView32 control to it.
			}


			//FUTURE: impl these:

			//public static Wnd CatkeysManager { get { return ; } }

			//public static Wnd CatkeysEditor { get { return ; } }

			//public static Wnd CatkeysCodeEditControl { get { return ; } }
		}

		/// <summary>
		/// Gets next window in the Z order, skipping invisible and other windows that would not be added to taskbar or not activated by Alt+Tab.
		/// Returns default(Wnd) if there are no such windows.
		/// If this is default(Wnd), starts from the top of the Z order.
		/// </summary>
		/// <param name="retryFromTop">If this is not default(Wnd) and there are no matching windows after it, retry from the top of the Z order. Like Alt+Tab does. Can return wFrom.</param>
		/// <param name="skipMinimized">Skip minimized windows.</param>
		/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has UAC integrity level uiAccess).</param>
		/// <param name="likeAltTab">
		/// Emulate Alt+Tab behavior with owned windows (message boxes, dialogs):
		///		If this is such owned window, skip its owner.
		///		If the found window has an owned window that was active more recently, return that owned window.
		/// </param>
		/// <remarks>
		/// This function is quite slow and does not exactly match the behavior of the Windows taskbar and Alt+Tab.
		/// </remarks>
		/// <seealso cref="Misc.MainWindows"/>
		public Wnd WndNextMain(bool retryFromTop = false, bool skipMinimized = false, bool allDesktops = false, bool likeAltTab = false)
		{
			Wnd lastFound = default, w2 = default, w = this;
			if(w.Is0) retryFromTop = false;

			for(;;) {
				w = w.Is0 ? Misc.WndTop : w.WndNext;
				if(w.Is0) {
					if(retryFromTop) { retryFromTop = false; continue; }
					return lastFound;
				}

				if(!w.IsVisible) continue;

				uint exStyle = w.ExStyle;
				if((exStyle & Native.WS_EX_APPWINDOW) == 0) {
					if((exStyle & (Native.WS_EX_TOOLWINDOW | Native.WS_EX_NOACTIVATE)) != 0) continue;
					w2 = w.WndOwner; if(!w2.Is0) { if(!likeAltTab || w2.IsVisible) continue; }
				}

				#region IsVisibleReally

				if(skipMinimized && w.IsMinimized) continue;

				if(Ver.MinWin10) {
					if(w.IsCloaked) {
						if(!allDesktops) continue;
						if((exStyle & Native.WS_EX_NOREDIRECTIONBITMAP) != 0) { //probably a store app
							switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
							case 1: continue; //Windows search, experience host, etc. Also app windows that normally would sit on ApplicationFrameWindow windows.
							case 2: if(_WindowsStoreAppFrameChild(w).Is0) continue; break;
							}
						}
					}
				} else if(Ver.MinWin8) {
					if((exStyle & Native.WS_EX_NOREDIRECTIONBITMAP) != 0 && !w.HasStyle(Native.WS_CAPTION)) {
						if(!allDesktops && (exStyle & Native.WS_EX_TOPMOST) != 0) continue; //skip store apps
						if(Misc.WndShell.GetThreadProcessId(out var pidShell) != 0 && w.GetThreadProcessId(out var pid) != 0 && pid == pidShell) continue; //skip captionless shell windows
					}
					//On Win8 impossible to get next window like Alt+Tab.
					//	All store apps are topmost, covering non-topmost desktop windows.
					//	DwmGetWindowAttribute has no sense here.
					//	Desktop windows are never cloaked, inactive store windows are cloaked, etc.
				}

				#endregion

				if(likeAltTab) {
					w2 = Misc.WndLastSeenActiveOwnedOrThis(Misc.WndRootOwnerOrThis(w)); //call with the root owner, because GLAP returns w if w has an owner (documented)
					if(w2 != w) {
						if(!w2.IsVisible || (skipMinimized && w2.IsMinimized)) w2 = w; //don't check cloaked etc for owned window if its owner passed
						if(w2 == this) { lastFound = w2; continue; }
						w = w2;
					}
				}

				return w;
			}
		}

		public static partial class Misc
		{
			/// <summary>
			/// Get windows that have taskbar button and/or are included in the Alt+Tab sequence.
			/// Returns array containing 0 or more Wnd.
			/// </summary>
			/// <param name="allDesktops">See <see cref="WndNextMain"/>.</param>
			/// <param name="likeAltTab">See <see cref="WndNextMain"/>.</param>
			/// <remarks>
			/// The list order does not match the order of buttons in the Windows taskbar. Possibly also does not exactly match the list of Alt+Tab windows.
			/// </remarks>
			/// <seealso cref="WndNextMain"/>
			public static Wnd[] MainWindows(bool allDesktops = false, bool likeAltTab = false)
			{
				var a = new List<Wnd>();
				for(Wnd w = default; ;) {
					w = w.WndNextMain(allDesktops: allDesktops, likeAltTab: likeAltTab);
					if(w.Is0) break;
					a.Add(w);
				}
				return a.ToArray();

				//SHOULDDO: instaed of WndNextMain, get all windows and filter like now WndNextMain does.

				//Another way - UI Automation:
				//	var x = new CUIAutomation();
				//	var cond = x.CreatePropertyCondition(30003, 0xC370); //UIA_ControlTypePropertyId, UIA_WindowControlTypeId
				//	var a = x.GetRootElement().FindAll(TreeScope.TreeScope_Children, cond);
				//	for(int i = 0; i < a.Length; i++) PrintList((Wnd)a.GetElement(i).CurrentNativeWindowHandle);
				//Advantages: 1. Easier to implement. 2. Maybe can filter unwanted windows more reliably, although I did't notice a difference.
				//Disadvantages: 1. Skips windows of higher integrity level (UAC). 2. Does not have (or I don't know) an option to include cloaked windows, eg those in inactive Win10 virtual desktops. 3. About 1000 times slower, eg 70 ms vs 70 mcs; cold 140 ms.
			}

			///// <summary>
			///// Gets all visible windows.
			///// Sorts minimized windows after visible and in reverse order. Like Win10 Alt+Tab.
			///// Cannot use this. Alt+Tab works differently.
			///// </summary>
			//static List<Wnd> __MainWindows_GetAll()
			//{
			//	var a = new List<Wnd>();
			//	int i = 0;
			//	LibAllWindows(t =>
			//	{
			//		uint k = t.ExStyle;
			//		if(((k & Native.WS_EX_APPWINDOW) == 0) && ((k & (Native.WS_EX_TOOLWINDOW | Native.WS_EX_NOACTIVATE)) != 0)) return false;

			//		if(t.IsCloaked) return false;

			//		a.Insert(i, t);
			//		//i++;
			//		if(!t.IsMinimized) i++;
			//		return false;
			//	}, true);
			//	return a;
			//}

			/// <summary>
			/// Activates next non-minimized main window, like with Alt+Tab.
			/// Returns true if activated, false if there is no such window or failed to activate.
			/// Calls <see cref="WndNextMain">WndNextMain</see> and <see cref="Activate()"/>.
			/// An alternative way - send Alt+Tab keys, but it works not everywhere; it can activate a minimized window too.
			/// </summary>
			public static bool SwitchActiveWindow()
			{
				try {
					//Prefer non-minimized windows. But if all minimized, restore the first main window.
					Wnd wa = WndActive;
					Wnd w = wa.WndNextMain(retryFromTop: true, skipMinimized: true, likeAltTab: true);
					if(!(w.Is0 || w == wa)) {
						w.Activate();
						return true;
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

			///// <summary>
			///// Activates another main window, like with Alt+Tab.
			///// Returns the window. Returns default(Wnd) if there is no such window or fails to activate.
			///// Calls <see cref="WndNextMain"/> and <see cref="Activate()"/>.
			///// When all or all except one windows are minimized, may restore a different window than Alt+Tab would.
			///// An alternative way - send Alt+Tab keys, but it works not everywhere.
			///// </summary>
			//public static Wnd SwitchActiveWindow()
			//{
			//	try {
			//		//Prefer non-minimized windows. But if all minimized, restore the first main window.
			//		Wnd wa = WndActive;
			//		Wnd w = wa.WndNextMain(retryFromTop: true, skipMinimized: true, likeAltTab: true);
			//		if(w.Is0 || w == wa) { //0 or 1 non-minimized windows; activate the most recently minimized, which usually is at the Z bottom.
			//			w = default;
			//			var a = Misc.MainWindows(likeAltTab: true);
			//			int i = a.Count - 1;
			//			if(i >= 0 && a[i] != wa) w = a[i];
			//		}
			//		if(!w.Is0) w._Activate();
			//		return w;
			//	}
			//	catch(WndException) { }
			//	return default;

			//	//notes:
			//	//The order of windows used by Alt+Tab is not the same as the Z order, especially when there are minimized windows.
			//	//	We cannot get that order and have to use the Z order. It seems that the order is different on Win10 but not on Win7.
			//	//After minimizing a window its position in the Z order is undefined.
			//	//	Most windows then are at the bottom when used the Minimize button or ShowWindow etc.
			//	//	But some are at the top, just after the active window, for example MS Document Explorer (Win7 SDK).
			//	//	Also at the top when minimized with the taskbar button.
			//}

			//This works more like Alt+Tab on Win7. The above code - more like on Win10, which is better.
			//public static Wnd SwitchActiveWindow()
			//{
			//	try {
			//		//Prefer non-minimized windows. But if all minimized, restore the first main window.
			//		Wnd wa = WndActive;
			//		for(int i = 0; i < 2; i++) {
			//			Wnd w = WndNextMain(wa, retryFromTop: true, skipMinimized: i == 0, likeAltTab: true);
			//			if(w.Is0 || w == wa) {
			//				if(i == 0 && w.Is0) wa = default; //0 non-minimized windows; activate the first minimized.
			//				continue;
			//			}
			//			w._Activate();
			//			return w;
			//		}
			//	}
			//	catch(WndException) { }
			//	return default;
			//}
		}
	}
}
