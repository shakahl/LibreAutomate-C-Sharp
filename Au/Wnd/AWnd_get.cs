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
//using System.Linq;

using Au.Types;

namespace Au
{
	public partial struct AWnd
	{
		/// <summary>
		/// Gets related windows and controls.
		/// Use like <c>AWnd w2 = w1.Get.Owner;</c> (here w1 is an AWnd variable).
		/// </summary>
		public GetWnd Get => new GetWnd(this);

		/// <summary>
		/// Static functions of this class are used to get special windows (used like <c>AWnd w = AWnd.GetWnd.Top;</c>) and all windows.
		/// Instances of this class are used to get related windows and controls, like <c>AWnd w2 = w1.Get.FirstChild;</c> (here w1 is an AWnd variable).
		/// </summary>
		public partial struct GetWnd
		{
			AWnd _w;
			///
			public GetWnd(AWnd wThis) => _w = wThis;

			#region instance

			/// <summary>
			/// Gets nearest visible sibling control to the left from this.
			/// Returns default(AWnd) if not found.
			/// </summary>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too. Skips maximized/minimized windows and desktop.
			/// </remarks>
			public AWnd SiblingLeft() => _w._SiblingXY(_SibXY.Left);
			//FUTURE: add these to the window tool, like navig in acc tool. Also add previous/next/parent/child in Z order.

			/// <summary>
			/// Gets nearest visible sibling control to the right from this.
			/// Returns default(AWnd) if not found.
			/// </summary>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too. Skips maximized/minimized windows and desktop.
			/// </remarks>
			public AWnd SiblingRight() => _w._SiblingXY(_SibXY.Right);

			/// <summary>
			/// Gets nearest visible sibling control above this.
			/// Returns default(AWnd) if not found.
			/// </summary>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too. Skips maximized/minimized windows and desktop.
			/// </remarks>
			public AWnd SiblingAbove() => _w._SiblingXY(_SibXY.Above);

			/// <summary>
			/// Gets nearest visible sibling control to below this.
			/// Returns default(AWnd) if not found.
			/// </summary>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too. Skips maximized/minimized windows and desktop.
			/// </remarks>
			public AWnd SiblingBelow() => _w._SiblingXY(_SibXY.Below);

			/// <summary>
			/// Gets a visible sibling control to the left from this.
			/// Returns default(AWnd) if there is no sibling.
			/// </summary>
			/// <param name="distance">Horizontal distance from the left of this control.</param>
			/// <param name="yOffset">Vertical offset from the top of this control. If negative - up. Default 5.</param>
			/// <param name="topChild">If at that point is a visible child of the sibling, get that child. Default false.</param>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too.
			/// </remarks>
			public AWnd SiblingLeft(int distance, int yOffset = 5, bool topChild = false) => _w._SiblingXY(_SibXY.Left, distance, yOffset, topChild);

			/// <summary>
			/// Gets a visible sibling control to the right from this.
			/// Returns default(AWnd) if there is no sibling.
			/// </summary>
			/// <param name="distance">Horizontal distance from the right of this control.</param>
			/// <param name="yOffset">Vertical offset from the top of this control. If negative - up. Default 5.</param>
			/// <param name="topChild">If at that point is a visible child of the sibling, get that child. Default false.</param>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too.
			/// </remarks>
			public AWnd SiblingRight(int distance, int yOffset = 5, bool topChild = false) => _w._SiblingXY(_SibXY.Right, distance, yOffset, topChild);

			/// <summary>
			/// Gets a visible sibling control above this.
			/// Returns default(AWnd) if there is no sibling.
			/// </summary>
			/// <param name="distance">Vertical distance from the top of this control.</param>
			/// <param name="xOffset">Horizontal offset from the left of this control. If negative - to the left. Default 5.</param>
			/// <param name="topChild">If at that point is a visible child of the sibling, get that child. Default false.</param>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too.
			/// </remarks>
			public AWnd SiblingAbove(int distance, int xOffset = 5, bool topChild = false) => _w._SiblingXY(_SibXY.Above, distance, xOffset, topChild);

			/// <summary>
			/// Gets a visible sibling control below this.
			/// Returns default(AWnd) if there is no sibling.
			/// </summary>
			/// <param name="distance">Vertical distance from the bottom of this control.</param>
			/// <param name="xOffset">Horizontal offset from the left of this control. If negative - to the left. Default 5.</param>
			/// <param name="topChild">If at that point is a visible child of the sibling, get that child. Default false.</param>
			/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// This function is used mostly with controls, but supports top-level windows too.
			/// </remarks>
			public AWnd SiblingBelow(int distance, int xOffset = 5, bool topChild = false) => _w._SiblingXY(_SibXY.Below, distance, xOffset, topChild);

			AWnd _GetWindow(int dir, int skip)
			{
				if(skip < 0) return default;
				AWnd w;
				for(w = _w; skip >= 0 && !w.Is0; skip--) {
					w = Api.GetWindow(w, dir);
				}
				return w;
			}

			/// <summary>
			/// Gets next sibling window or control in the Z order.
			/// Returns default(AWnd) if this is the last or if fails.
			/// </summary>
			/// <param name="skip">How many next windows to skip.</param>
			/// <remarks>
			/// If this is a top-level window, gets next top-level window, else gets next control of the same direct parent.
			/// Calls API <msdn>GetWindow</msdn>(GW_HWNDNEXT).
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd Next(int skip = 0) => _GetWindow(Api.GW_HWNDNEXT, skip);

			/// <summary>
			/// Gets previous sibling window or control in the Z order.
			/// Returns default(AWnd) if this is the first or if fails.
			/// </summary>
			/// <param name="skip">How many previous windows to skip.</param>
			/// <remarks>
			/// If this is a top-level window, gets previous top-level window, else gets previous control of the same direct parent.
			/// Calls API <msdn>GetWindow</msdn>(GW_HWNDPREV).
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd Previous(int skip = 0) => _GetWindow(Api.GW_HWNDPREV, skip);

			/// <summary>
			/// Gets the first sibling window or control in the Z order.
			/// If this is the first, returns this.
			/// </summary>
			/// <remarks>
			/// If this is a top-level window, gets the first top-level window, else gets the first control of the same direct parent.
			/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDFIRST).
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd FirstSibling => Api.GetWindow(_w, Api.GW_HWNDFIRST);

			/// <summary>
			/// Gets the last sibling window or control in the Z order.
			/// If this is the last, returns this, not default(AWnd).
			/// </summary>
			/// <remarks>
			/// If this is a top-level window, gets the last top-level window, else gets the last control of the same direct parent.
			/// Calls API <msdn>GetWindow</msdn>(this, GW_HWNDLAST).
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd LastSibling => Api.GetWindow(_w, Api.GW_HWNDLAST);

			/// <summary>
			/// Gets the first direct child control in the Z order.
			/// Returns default(AWnd) if no children or if fails.
			/// </summary>
			/// <remarks>
			/// Calls API <msdn>GetWindow</msdn>(GW_CHILD).
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd FirstChild => Api.GetWindow(_w, Api.GW_CHILD);

			/// <summary>
			/// Gets the last direct child control in the Z order.
			/// Returns default(AWnd) if no children or if fails.
			/// </summary>
			/// <remarks>
			/// Calls API <msdn>GetWindow</msdn>.
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd LastChild { get { var t = Api.GetWindow(_w, Api.GW_CHILD); return t.Is0 ? t : Api.GetWindow(t, Api.GW_HWNDLAST); } }

			/// <summary>
			/// Gets a direct child control by index.
			/// Returns default(AWnd) if no children or if index is invalid or if fails.
			/// </summary>
			/// <param name="index">0-based index of the child control in the Z order.</param>
			/// <remarks>
			/// Calls API <msdn>GetWindow</msdn>.
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd Child(int index)
			{
				if(index < 0) return default;
				AWnd c = Api.GetWindow(_w, Api.GW_CHILD);
				for(; index > 0 && !c.Is0; index--) c = Api.GetWindow(c, Api.GW_HWNDNEXT);
				return c;
			}

			/// <summary>
			/// Gets the owner window of this top-level window.
			/// Returns default(AWnd) if this window isn't owned or if fails.
			/// </summary>
			/// <remarks>
			/// A window that has an owner window is always on top of it.
			/// Controls don't have an owner window.
			/// Supports <see cref="ALastError"/>.
			/// This function is the same as <see cref="AWnd.OwnerWindow"/>, which also allows to change owner.
			/// </remarks>
			public AWnd Owner => Api.GetWindow(_w, Api.GW_OWNER);

			/// <summary>
			/// Gets the top-level parent window of this control.
			/// If this is a top-level window, returns this.
			/// Returns default(AWnd) if fails.
			/// </summary>
			/// <remarks>
			/// Supports <see cref="ALastError"/>.
			/// This function is the same as <see cref="AWnd.Window"/>.
			/// </remarks>
			public AWnd Window => _w.Window;

			/// <summary>
			/// Gets the direct parent window of this control. It can be the top-level window or another control.
			/// Returns default(AWnd) if this is a top-level window or if fails.
			/// </summary>
			/// <remarks>
			/// Supports <see cref="ALastError"/>.
			/// Unlike API <msdn>GetParent</msdn>, this function never returns the owner window.
			/// </remarks>
			public AWnd DirectParent {
				get {
#if true
					var p = _w.GetWindowLong(Native.GWL.HWNDPARENT);
					if(p == default) {
//#if DEBUG
//						var p2 = Api.GetAncestor(_w, Api.GA_PARENT);
//						Debug.Assert(p2.Is0 || p2 == Root);
//#endif
						return default;
					}
					ALastError.Clear();
					var o = Api.GetWindow(_w, Api.GW_OWNER);
					if(o.Is0) {
						var ec = ALastError.Code;
						if(ec == 0) return (AWnd)p;
						Debug.Assert(ec == Api.ERROR_INVALID_WINDOW_HANDLE);
					}
					return default;
#else //the msdn-recommended version, but >=6 times slower. The same IsTopLevelWindow (undocumented API).
					var p = Api.GetAncestor(_w, Api.GA_PARENT);
					if(p.Is0 || p == Root) return default;
					return p;
#endif
				}
			}

			//rejected. Not much faster than our DirectParent.
			///// <summary>
			///// Calls API <msdn>GetParent</msdn>.
			///// </summary>
			///// <remarks>
			///// The API function is fast but unreliable. It can get parent or owner window, and fails in some cases. Read more in the API documentation. It is reliable only if you know that this window is a child window and has WS_CHILD style.
			///// Supports <see cref="ALastError"/>.
			///// </remarks>
			//[Obsolete("Unreliable")]
			//public AWnd GetParentApi => Api.GetParent(_w);

			/// <summary>
			/// Gets the first (in Z order) enabled window owned by this window.
			/// </summary>
			/// <param name="orThis">Return this window if there are no enabled owned windows. If false, then returns default(AWnd).</param>
			/// <remarks>
			/// Calls API <msdn>GetWindow</msdn>(GW_ENABLEDPOPUP).
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd EnabledOwned(bool orThis)
			{
				var r = Api.GetWindow(_w, Api.GW_ENABLEDPOPUP);
				if(orThis) {
					if(r.Is0) r = _w;
				} else {
					if(r == _w) r = default;
				}
				return r;
				//MSDN documentation is incorrect. It says returns this window if there are no owned. But returns 0.
			}

			/// <summary>
			/// Gets the most recently active window in the chain of windows owned by this window, or this window itself if there are no such windows.
			/// Returns default(AWnd) if fails.
			/// </summary>
			/// <param name="includeOwners">Can return an owner (or owner's owner and so on) of this window too.</param>
			/// <remarks>
			/// Supports <see cref="ALastError"/>.
			/// </remarks>
			public AWnd LastActiveOwnedOrThis(bool includeOwners = false)
			{
				var wRoot = RootOwnerOrThis(); //always use the root owner because GetLastActivePopup always returns _w if _w is owned
				var R = Api.GetLastActivePopup(wRoot);
				if(!includeOwners) {
					if(R != _w && wRoot != _w && !R.Is0) {
						for(var t = _w; !t.Is0; t = t.OwnerWindow) if(t == R) return _w;
					}
				}
				return R;
			}

			/// <summary>
			/// Gets the bottom-most owner window in the chain of owner windows of this window.
			/// If this window is not owned, returns this window.
			/// Returns default(AWnd) if fails.
			/// </summary>
			/// <param name="supportControls">If this is a child window, use its top-level parent window instead.</param>
			/// <remarks>Supports <see cref="ALastError"/>.</remarks>
			public AWnd RootOwnerOrThis(bool supportControls = false)
			{
				//return Api.GetAncestor(_w, Api.GA_ROOTOWNER); //slow, and can return Get.Root, eg for combolbox

				if(supportControls) {
					var r = Api.GetAncestor(_w, Api.GA_ROOTOWNER);
					return r == Root ? _w : r;
					//never mind speed, better make it simple
				} else { //fast
					for(AWnd r = _w, t; ; r = t) {
						t = r.OwnerWindow;
						if(t.Is0) return r.IsAlive ? r : default;
					}
				}
			}

			/// <summary>
			/// Gets all owner windows of this window, including this window or its top-level parent.
			/// Returns array that starts with this window or its top-level parent (if control).
			/// </summary>
			/// <param name="onlyVisible">Skip invisible windows.</param>
			/// <remarks>
			/// This window can be top-level window or control. If control, its top-level parent window will be the first in the array.
			/// This function for example can be used to temporarily hide a tool window and its owners when capturing something from the screen.
			/// </remarks>
			public AWnd[] OwnersAndThis(bool onlyVisible = false)
			{
				var a = new List<AWnd>();
				for(var w = Window; !w.Is0 && w != Root; w = w.Get.Owner)
					if(!onlyVisible || w.IsVisible) a.Add(w);
				return a.ToArray();
			}

			#endregion

			#region static

			/// <summary>
			/// Gets the first top-level window in the Z order.
			/// </summary>
			/// <remarks>
			/// Probably it is a topmost window. To get the first non-topmost window, use <see cref="Top2"/>.
			/// Calls API <msdn>GetTopWindow</msdn>(default(AWnd)).
			/// </remarks>
			public static AWnd Top => Api.GetTopWindow(default);

			/// <summary>
			/// Finds and returns the first non-topmost window in the Z order.
			/// </summary>
			/// <param name="lastTopmost">Receives the last topmost window.</param>
			/// <remarks>
			/// This function is slower than <see cref="Top"/> etc. Enumerates windows, because there is no API to get directly.
			/// </remarks>
			public static AWnd Top2(out AWnd lastTopmost) {
				AWnd w, lastTM, w2 = default, lastTM2 = default;
				for (; ; ) { //repeat until gets same results 2 times. At any time windows can be destroyed, reordered, reparented. The second time 2-3 times faster.
					lastTM = default; w = Top; if (w.Is0) break; //no windows in this session
					for (; w.IsTopmost; w = w.Get.Next()) lastTM = w;
					if (w == w2 && lastTM == lastTM2 && !w.Is0) break;
					w2 = w; lastTM2 = lastTM;
				}
				lastTopmost = lastTM;
				return w;
			}

			/// <summary>
			/// Calls API <msdn>GetDesktopWindow</msdn>. It gets the virtual parent window of all top-level windows.
			/// </summary>
			/// <remarks>
			/// <note>It is not the desktop window (see <see cref="Desktop"/>) that displays icons and wallpaper.</note>
			/// </remarks>
			public static AWnd Root { get; } = Api.GetDesktopWindow();

			/// <summary>
			/// Calls API <msdn>GetShellWindow</msdn>. It gets a window of the shell process (usually process "explorer", class name "Progman").
			/// Returns default(AWnd) if there is no shell process, for example Explorer process killed/crashed and still not restarted, or if using a custom shell that does not register a shell window.
			/// </summary>
			/// <remarks>
			/// It can be the window that contains desktop icons (see <see cref="Desktop"/>) or other window of the same thread.
			/// </remarks>
			public static AWnd ShellWindow => Api.GetShellWindow();

			/// <summary>
			/// Gets the desktop window and its child control that displays desktop icons and wallpaper.
			/// Returns false if fails.
			/// </summary>
			/// <param name="window">Receives the desktop window. Class name "Progman" or "WorkerW".</param>
			/// <param name="control">Receives the control of "SysListView32" class that contains icons and wallpaper.</param>
			/// <remarks>
			/// This function is not very reliable. May stop working on a new Windows version or don't work with a custom shell.
			/// Fails if there is no shell process, for example Explorer process killed/crashed and still not restarted, or if using a custom shell that does not register a shell window.
			/// </remarks>
			public static bool Desktop(out AWnd window, out AWnd control)
			{
				AWnd w = ShellWindow;
				var f = new ChildFinder(cn: "SysListView32");
				if(!f.Find(w)) w = AWnd.Find(null, "WorkerW", WOwner.Thread(w.ThreadId), also: t => f.Find(t));
				window = w;
				control = f.Result;
				return !w.Is0;

				//info:
				//If no wallpaper, desktop is GetShellWindow, else a visible WorkerW window.
				//When was no wallpaper and user selects a wallpaper, explorer creates WorkerW and moves the same SysListView32 control to it.

				//tested: with COM (IShellWindows -> IShellBrowser -> IShellView.GetWindow) slower 17 times.
			}

			//FUTURE:
			//public static AWnd AuEditor =>
			//public static AWnd AuCodeEditControl =>

			#endregion
		}

		#region main windows

		public partial struct GetWnd
		{
			/// <summary>
			/// Returns true if window w is considered a main window, ie probably is in the Windows taskbar.
			/// Returns false if it is invisible, cloaked, owned, toolwindow, menu, etc.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="allDesktops">On Windows 10 include (return true for) windows on all virtual desktops. On Windows 8 include Windows Store apps if possible; read more: <see cref="AllWindows(bool, bool)"/>.</param>
			/// <param name="skipMinimized">Return false if w is minimized.</param>
			public static bool IsMainWindow(AWnd w, bool allDesktops = false, bool skipMinimized = false)
			{
				if(!w.IsVisible) return false;

				var exStyle = w.ExStyle;
				if((exStyle & WS2.APPWINDOW) == 0) {
					if((exStyle & (WS2.TOOLWINDOW | WS2.NOACTIVATE)) != 0) return false;
					if(!w.OwnerWindow.Is0) return false;
				}

				if(skipMinimized && w.IsMinimized) return false;

				if(AVersion.MinWin10) {
					if(w.IsCloaked) {
						if(!allDesktops) return false;
						if((exStyle & WS2.NOREDIRECTIONBITMAP) != 0) { //probably a store app
							switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
							case 1: return false; //Windows search, experience host, etc. Also app windows that normally would sit on ApplicationFrameWindow windows.
							case 2: if(_WindowsStoreAppFrameChild(w).Is0) return false; break; //skip hosts
							}
						}
					}
				} else if(AVersion.MinWin8) {
					if((exStyle & WS2.NOREDIRECTIONBITMAP) != 0 && !w.HasStyle(WS.CAPTION)) {
						if(!allDesktops && (exStyle & WS2.TOPMOST) != 0) return false; //skip store apps
						if(ShellWindow.GetThreadProcessId(out var pidShell) != 0 && w.GetThreadProcessId(out var pid) != 0 && pid == pidShell) return false; //skip captionless shell windows
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
			/// Returns array containing 0 or more AWnd.
			/// </summary>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps if possible; read more: <see cref="AllWindows(bool, bool)"/>.</param>
			/// <remarks>
			/// Uses <see cref="IsMainWindow"/>.
			/// Does not match the order of buttons in the Windows taskbar.
			/// </remarks>
			public static AWnd[] MainWindows(bool allDesktops = false)
			{
				var a = new List<AWnd>();
				foreach(var w in AllWindows(onlyVisible: true)) {
					if(IsMainWindow(w, allDesktops: allDesktops)) a.Add(w);
				}
				return a.ToArray();

				//Another way - UI Automation:
				//	var x = new CUIAutomation();
				//	var cond = x.CreatePropertyCondition(30003, 0xC370); //UIA_ControlTypePropertyId, UIA_WindowControlTypeId
				//	var a = x.GetRootElement().FindAll(TreeScope.TreeScope_Children, cond);
				//	for(int i = 0; i < a.Length; i++) AOutput.Write((AWnd)a.GetElement(i).CurrentNativeWindowHandle);
				//Advantages: 1. Maybe can filter unwanted windows more reliably, although I did't notice a difference.
				//Disadvantages: 1. Skips windows of higher integrity level (UAC). 2. Cannot include cloaked windows, eg those in inactive Win10 virtual desktops. 3. About 1000 times slower, eg 70 ms vs 70 mcs; cold 140 ms.
			}

			/// <summary>
			/// Gets next window in the Z order, skipping invisible and other windows that probably are not in the Windows taskbar.
			/// Returns default(AWnd) if there are no such windows.
			/// </summary>
			/// <param name="w">Start from this window. If default(AWnd), starts from the top of the Z order.</param>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps if possible; read more: <see cref="AllWindows(bool, bool)"/>.</param>
			/// <param name="skipMinimized">Skip minimized windows.</param>
			/// <param name="retryFromTop">If w is not default(AWnd) and there are no matching windows after it, retry from the top of the Z order. Then can return w.</param>
			/// <remarks>
			/// Uses <see cref="IsMainWindow"/>.
			/// This function is quite slow. Does not match the order of buttons in the Windows taskbar.
			/// </remarks>
			public static AWnd NextMain(AWnd w = default, bool allDesktops = false, bool skipMinimized = false, bool retryFromTop = false)
			{
				if(w.Is0) retryFromTop = false;

				for(; ; ) {
					w = w.Is0 ? GetWnd.Top : w.Get.Next();
					if(w.Is0) {
						if(retryFromTop) { retryFromTop = false; continue; }
						return default;
					}
					if(IsMainWindow(w, allDesktops: allDesktops, skipMinimized: skipMinimized)) return w;
				}
			}
		}

		/// <summary>
		/// Activates next non-minimized main window, like with Alt+Tab.
		/// Returns true if activated, false if there is no such window or failed to activate.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetWnd.NextMain"/>, <see cref="GetWnd.LastActiveOwnedOrThis"/>, <see cref="Activate()"/>.
		/// An alternative way - send Alt+Tab keys, but it works not everywhere.
		/// </remarks>
		public static bool SwitchActiveWindow()
		{
			try {
				AWnd wActive = Active, wRO = wActive.Get.RootOwnerOrThis();
				AWnd wMain = GetWnd.NextMain(wRO, skipMinimized: true, retryFromTop: true);
				if(!wMain.Is0 && wMain != wActive && wMain != wRO) {
					var wMainOrOwned = wMain.Get.LastActiveOwnedOrThis();
					if(!wMainOrOwned.Is0) {
						//AOutput.Write(wMainOrOwned);
						wMainOrOwned.Activate();
						return true;
					}
				}
			}
			catch(AuWndException) { }
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

		#endregion

		//These are in GetWnd and here, because frequently used. Also because some have setters.

		/// <summary>
		/// Gets or sets the owner window of this top-level window.
		/// </summary>
		/// <exception cref="AuWndException">The 'set' function failed.</exception>
		/// <remarks>
		/// A window that has an owner window is always on top of it.
		/// Don't call this for controls, they don't have an owner window.
		/// The 'get' function returns default(AWnd) if this window isn't owned or is invalid. Supports <see cref="ALastError"/>.
		/// The 'set' function can fail, eg if the owner's process has higher [](xref:uac) integrity level or is a Store app.
		/// </remarks>
		public AWnd OwnerWindow {
			get => Api.GetWindow(this, Api.GW_OWNER);
			set {
				SetWindowLong(Native.GWL.HWNDPARENT, (LPARAM)value);
				//if (!value.Is0 && !ZorderIsAbove(value)) ZorderAbove(value);
				if(!value.Is0) {
					bool tm = value.IsTopmost;
					if (tm != IsTopmost) { if (tm) ZorderTopmost(); else ZorderNoTopmost(); }
				}
			}
		}

		/// <summary>
		/// Gets the top-level parent window of this control.
		/// If this is a top-level window, returns this. Returns default(AWnd) if this window is invalid.
		/// </summary>
		/// <remarks>Supports <see cref="ALastError"/>.</remarks>
		public AWnd Window {
			get {
				var w = Api.GetAncestor(this, Api.GA_ROOT);
				if(w.Is0 && this == GetWnd.Root) w = this;
				return w;
			}
		}

		/// <summary>
		/// Returns true if this is a child window (control), false if top-level window.
		/// </summary>
		/// <remarks>
		/// Supports <see cref="ALastError"/>.
		/// Another way is <c>w.HasStyle(WS.CHILD)</c>. It is faster but less reliable, because some top-level windows have WS_CHILD style and some child windows don't.
		/// </remarks>
		/// <seealso cref="GetWnd.DirectParent"/>
		public bool IsChild => !Get.DirectParent.Is0;

		/// <summary>
		/// Returns true if this is a child or descendant of window w.
		/// </summary>
		/// <remarks>
		/// Calls API <msdn>IsChild</msdn>.
		/// Supports <see cref="ALastError"/>.
		/// </remarks>
		public bool IsChildOf(AWnd w) { return Api.IsChild(w, this); }

		/// <summary>
		/// Returns <c>(AWnd)GetWindowLong(Native.GWL.HWNDPARENT)</c>.
		/// </summary>
		internal AWnd ParentGWL_ => (AWnd)GetWindowLong(Native.GWL.HWNDPARENT);

		/// <summary>
		/// Gets the active (foreground) window.
		/// Calls API <msdn>GetForegroundWindow</msdn>.
		/// Returns default(AWnd) if there is no active window; more info: <see cref="More.WaitForAnActiveWindow"/>.
		/// </summary>
		public static AWnd Active => Api.GetForegroundWindow();

		/// <summary>
		/// Returns true if this window is the active (foreground) window.
		/// </summary>
		public bool IsActive => !Is0 && this == Api.GetForegroundWindow();

		//FUTURE: static bool IsActiveAny(list of AWnd or Finder).

		/// <summary>
		/// Returns true if this window is the active (foreground) window.
		/// If this is <see cref="GetWnd.Root"/>, returns true if there is no active window.
		/// </summary>
		internal bool IsActiveOrNoActiveAndThisIsWndRoot_ {
			get {
				if(Is0) return false;
				var f = Api.GetForegroundWindow();
				return this == (f.Is0 ? GetWnd.Root : f);
			}
		}
	}
}
