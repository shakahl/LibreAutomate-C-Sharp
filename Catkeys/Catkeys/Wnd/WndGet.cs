//Classes Wnd.Get, Wnd.All and several related functions of Wnd.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	public partial struct Wnd
	{
		Wnd _GetWindow(uint GW_X)
		{
			ResetLastError();
			return Api.GetWindow(this, GW_X);
		}

		Wnd _GetAncestor(uint GA_X)
		{
			ResetLastError();
			return Api.GetAncestor(this, GA_X);
		}

		/// <summary>
		/// Gets or sets the owner window of this top-level window.
		/// A window that has an owner window is always on top of its owner window.
		/// Don't call this for controls, they don't have an owner window.
		/// The 'get' function calls Api.GetWindow(Api.GW_OWNER).
		/// The 'set' function calls Api.SetWindowLong(Api.GWL_HWNDPARENT, (LPARAM)value); it can fail, eg if the owner's process has higher UAC integrity level.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd Owner
		{
			get { return _GetWindow(Api.GW_OWNER); }
			set { Api.SetWindowLong(this, Api.GWL_HWNDPARENT, (LPARAM)value); }
		}

		/// <summary>
		/// Gets the top-level parent window of this control.
		/// If this is a top-level window, returns this window.
		/// Calls Api.GetAncestor(Api.GA_ROOT).
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd ToplevelParentOrThis { get { return _GetAncestor(Api.GA_ROOT); } }

		/// <summary>
		/// Gets direct parent of this control. It can be its top-level parent window or parent control.
		/// Returns Wnd0 (0) if this is a top-level window.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd DirectParent { get { return Get.DirectParent(this); } }

		/// <summary>
		/// Gets direct parent of this control. It can be its top-level parent window or parent control.
		/// If this is a top-level window, gets its owner, or returns Wnd0 (0) if it is unowned.
		/// Calls Api.GetParent().
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd DirectParentOrOwner { get { return Get.DirectParentOrOwner(this); } }

		/// <summary>
		/// Returns true if this is a child window (control), false if top-level window.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		public bool IsControl { get { return !DirectParent.Is0; } }

		/// <summary>
		/// Returns true if this is a direct or indirect child of window w.
		/// Supports Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="w">A top-level window or control.</param>
		public bool IsChildOf(Wnd w) { ResetLastError(); return Api.IsChild(w, this); }

		/// <summary>
		/// Static functions that get windows/controls in Z order, parent/child, owner/owned, special windows.
		/// Example: <c>Wnd w2=Wnd.Get.NextSibling(w1);</c>
		/// Wnd also has copies of some often used Wnd.Get functions.
		/// </summary>
		//[DebuggerStepThrough]
		public static class Get
		{
			/// <summary>
			/// Gets the foreground window, like Wnd.ActiveWindow.
			/// Calls Api.GetForegroundWindow().
			/// </summary>
			public static Wnd Active() { return Api.GetForegroundWindow(); }

			/// <summary>
			/// Gets the first top-level window in the Z order.
			/// Calls Api.GetTopWindow(Wnd0).
			/// </summary>
			public static Wnd FirstToplevel() { return Api.GetTopWindow(Wnd0); }

			/// <summary>
			/// Gets the first window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDFIRST).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd FirstSibling(Wnd w) { return w._GetWindow(Api.GW_HWNDFIRST); }

			/// <summary>
			/// Gets the last window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDLAST).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd LastSibling(Wnd w) { return w._GetWindow(Api.GW_HWNDLAST); }

			/// <summary>
			/// Gets next window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDNEXT).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd NextSibling(Wnd w) { return w._GetWindow(Api.GW_HWNDNEXT); }

			/// <summary>
			/// Gets next window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDPREV).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd PreviousSibling(Wnd w) { return w._GetWindow(Api.GW_HWNDPREV); }

			/// <summary>
			/// Gets the first w child control in the Z order.
			/// Calls Api.GetWindow(w, Api.GW_CHILD).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd FirstChild(Wnd w) { return w._GetWindow(Api.GW_CHILD); }

			/// <summary>
			/// Gets the owner window of top-level window w.
			/// A window that has an owner window is always on top of its owner window.
			/// Calls Api.GetWindow(w, Api.GW_OWNER).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd Owner(Wnd w) { return w._GetWindow(Api.GW_OWNER); }

			/// <summary>
			/// Gets the first enabled window in the chain of windows owned by w, or w itself if there are no such windows.
			/// Calls Api.GetWindow(w, Api.GW_ENABLEDPOPUP).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd FirstEnabledOwnedOrThis(Wnd w) { return w._GetWindow(Api.GW_ENABLEDPOPUP); }

			/// <summary>
			/// Gets the most recently active window owned by w, or w itself if it was the most recently active.
			/// Calls Api.GetLastActivePopup(w).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd LastSeenActiveOwnedOrThis(Wnd w) { ResetLastError(); return Api.GetLastActivePopup(w); }

			/// <summary>
			/// Gets the top-level parent window of control w. If w is a top-level window, returns w.
			/// Calls Api.GetAncestor(Api.GA_ROOT).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd ToplevelParentOrThis(Wnd w) { return w._GetAncestor(Api.GA_ROOT); }

			/// <summary>
			/// Gets the most bottom owner window in the chain of owner windows of w. If w is not owned, returns w.
			/// Calls Api.GetAncestor(Api.GA_ROOTOWNER).
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd RootOwnerOrThis(Wnd w) { return w._GetAncestor(Api.GA_ROOTOWNER); }

			/// <summary>
			/// Gets direct parent of the specified control. It can be its top-level parent window or parent control.
			/// Returns Wnd0 (0) if w is a top-level window.
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd DirectParent(Wnd w)
			{
				Wnd r = DirectParentOrOwner(w);
				if(r.Is0 || r == Api.GetWindow(w, Api.GW_OWNER) || r == _wDesktop) return Wnd0;
				return r;
				//tested: GetAncestor much slower. IsChild also slower.
				//About 'r == _wDesktop': it is rare but I have seen that desktop is owner.
				//	If with createwindow owner is 0 or desktop, both getparent and getwindow return 0.
				//	But if with setwindowlong owner is desktop, both return desktop.
			}

			static Wnd _wDesktop = Api.GetDesktopWindow();

			/// <summary>
			/// Gets direct parent of the specified control. It can be its top-level parent window or parent control.
			/// If w is a top-level window, gets its owner, or returns Wnd0 (0) if it is unowned.
			/// Calls Api.GetParent().
			/// Supports Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd DirectParentOrOwner(Wnd w) { ResetLastError(); return Api.GetParent(w); }

			/// <summary>
			/// Gets the special window that is used like the parent window of all top-level windows.
			/// Calls Api.GetDesktopWindow().
			/// </summary>
			public static Wnd DesktopWindow { get { return Api.GetDesktopWindow(); } }

			/// <summary>
			/// Gets a window of the shell process (usually explorer.exe).
			/// The window belongs to the same thread as the window that contains desktop icons.
			/// Calls Api.GetShellWindow().
			/// </summary>
			public static Wnd ShellWindow { get { return Api.GetShellWindow(); } }

			/// <summary>
			/// Gets next window in the Z order, skipping invisible and other windows that would not be added to taskbar or not activated by Alt+Tab.
			/// Returns Wnd0 if there are no such windows.
			/// </summary>
			/// <param name="wFrom">Window after (behind) which to search. If omitted or Wnd0, starts from the top of the Z order.</param>
			/// <param name="retryFromTop">If wFrom is used and there are no matching windows after it, retry from the top of the Z order. Like Alt+Tab does. Can return wFrom.</param>
			/// <param name="skipMinimized">Skip minimized windows.</param>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows store apps.</param>
			/// <param name="likeAltTab">
			/// Emulate Alt+Tab behavior with owned windows (message boxes, dialogs):
			///		If wFrom is such owned window, skip its owner.
			///		If the found window has an owned window that was active more recently, return that owned window.
			/// </param>
			/// <remarks>
			/// NextMainWindow(Wnd.ActiveWindow, retryFromTop:true, likeAltTab:true) ideally should get the same window as would be activated by Alt+Tab. However it is not always possible. Sometimes it will be a different window.
			/// Without retryFromTop:true, likeAltTab:true this function can be used to get main application windows like in taskbar. It is used by All.MainWindows.
			/// </remarks>
			public static Wnd NextMainWindow(Wnd wFrom = default(Wnd),
				bool retryFromTop = false, bool skipMinimized = false, bool allDesktops = false, bool likeAltTab = false)
			{
				//TODO: test all flags. Test on all OS.

				Wnd lastFound = Wnd0, w2 = Wnd0, w = wFrom;
				if(w.Is0) retryFromTop = false;

				for(;;) {
					w = w.Is0 ? FirstToplevel() : NextSibling(w);
					if(w.Is0) {
						if(retryFromTop) { retryFromTop = false; continue; }
						return lastFound;
					}

					if(!w.Visible) continue;

					uint exStyle = w.ExStyle;
					if((exStyle & Api.WS_EX_APPWINDOW) == 0) {
						if((exStyle & (Api.WS_EX_TOOLWINDOW | Api.WS_EX_NOACTIVATE)) != 0) continue;
						w2 = w.Owner; if(!w2.Is0) { if(!likeAltTab || w2.Visible) continue; }
					}

					#region IsVisibleReally

					if(skipMinimized && w.StateMinimized) continue;

					if(WinVer >= Win10) {
						if(w.Cloaked) {
							if(!allDesktops) continue;
							if((exStyle & Api.WS_EX_NOREDIRECTIONBITMAP) != 0) { //probably a store app
								switch(w.ClassNameIsAny("Windows.UI.Core.CoreWindow|ApplicationFrameWindow")) {
								case 1: continue; //Windows search, experience host, etc. Also app windows that normally would sit on ApplicationFrameWindow windows.
								case 2: if(_WindowsStoreAppFrameChild(w).Is0) continue; break;
								}
							}
                        }
					} else if(WinVer >= Win8_0) {
						if((exStyle & Api.WS_EX_NOREDIRECTIONBITMAP) != 0 && !w.HasStyle(Api.WS_CAPTION)) {
							if(!likeAltTab && (exStyle & Api.WS_EX_TOPMOST) != 0) continue; //skip store apps
							uint pid, pidShell;
							if(ShellWindow.GetThreadAndProcessId(out pidShell) != 0 && w.GetThreadAndProcessId(out pid) != 0 && pid == pidShell) continue; //skip captionless shell windows
						}
						//On Win8 impossible to get next window like Alt+Tab.
						//	All store apps are topmost, covering non-topmost desktop windows.
						//	DwmGetWindowAttribute has no sense here.
						//	Desktop windows are never cloaked, inactive store windows are cloaked, etc.
					}

					#endregion

					if(likeAltTab) {
						w2 = LastSeenActiveOwnedOrThis(RootOwnerOrThis(w)); //call with the root owner, because GLAP returns w if w has an owner (documented)
						if(w2 != w) {
							if(!w2.Visible || (skipMinimized && w2.StateMinimized)) w2 = w; //don't check cloaked etc for owned window if its owner passed
							if(w2 == wFrom) { lastFound = w2; continue; }
							w = w2;
						}
					}

					return w;
				}
			}

			/// <summary>
			/// On Win10+, if w is "ApplicationFrameWindow", returns the real app window "Windows.UI.Core.CoreWindow" hosted by w.
			/// If w is minimized, cloaked (eg on other desktop) or the app is starting, the "Windows.UI.Core.CoreWindow" is not its child. Then searches for a top-level window with the same name as of w. It is unreliable, but MS does not provide API for this.
			/// Info: "Windows.UI.Core.CoreWindow" windows hosted by "ApplicationFrameWindow" belong to separate processes. All "ApplicationFrameWindow" windows belong to a single process.
			/// </summary>
			static Wnd _WindowsStoreAppFrameChild(Wnd w)
			{
				bool retry = false;
				string name = null;
				g1:
				if(WinVer < Win10 || !w.ClassNameIs("ApplicationFrameWindow")) return Wnd0;
				Wnd c = Api.FindWindowEx(w, Wnd0, "Windows.UI.Core.CoreWindow", null);
				if(!c.Is0) return c;
				if(retry) return Wnd0;

				name = w.Name; if(Empty(name)) return Wnd0;

				for(;;) {
					c = Api.FindWindowEx(Wnd0, c, "Windows.UI.Core.CoreWindow", name); //I could not find API for it
					if(c.Is0) break;
					if(c.Cloaked) return c; //else probably it is an unrelated window
				}

				retry = true;
				goto g1;
			}

			//TODO: impl these

			//public static Wnd CatkeysManager { get { return ; } }

			//public static Wnd CatkeysEditor { get { return ; } }

			//public static Wnd CatkeysCodeEditControl { get { return ; } }

		}

		/// <summary>
		/// Static functions to get all windows or controls that match specified properties.
		/// </summary>
		//[DebuggerStepThrough]
		public static class All
		{
			#region top-level windows

			/// <summary>
			/// Gets list of top-level windows.
			/// Uses Api.EnumWindows().
			/// By default the list elements are sorted to match the Z order, but it may be not true if sortFirstVisible is true.
			/// </summary>
			/// <param name="className">If not null/"", gets only windows of this class. String by default is interpreted as wildcard, case-insensitive.</param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="sortFirstVisible">Place all list elements of hidden windows at the end of the returned list, even if the hidden windows are before some visible windows in the Z order.</param>
			public static List<Wnd> Windows(WildStringI className = null, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				List<Wnd> a = null, aHidden = null;
				if(onlyVisible) sortFirstVisible = false;

				Windows(e =>
				{
					if(sortFirstVisible && !e.w.Visible) {
						if(aHidden == null) aHidden = new List<Wnd>(32);
						aHidden.Add(e.w);
					} else {
						if(a == null) a = new List<Wnd>(32);
						a.Add(e.w);
					}
				}, className, onlyVisible);

				if(aHidden != null) a.AddRange(aHidden);
				return a;

				//info: tried to add a flag to skip tooltips, IME, MSCTFIME UI. But for it need to get class. It is slow. Other ways are unreliable and also make slower. Only the onlyVisible flag is really effective.
			}

			/// <summary>
			/// Calls callback function for each top-level window.
			/// Uses Api.EnumWindows().
			/// </summary>
			/// <param name="f">Lambda etc callback function to call for each matching window. Example: <c>e =˃ { Out(e.w); if(e.w.Name=="Find") e.Stop(); }</c></param>
			/// <param name="className">If not null/""/"*", gets only windows of this class. String by default is interpreted as wildcard, case-insensitive.</param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			public static void Windows(Action<CallbackArgs> f, WildStringI className = null, bool onlyVisible = false)
			{
				var e = new CallbackArgs();

				Api.EnumWindows((w, param) =>
				{
					if(onlyVisible && !w.Visible) return 1;
					if(className != null && !w.ClassNameIs(className)) return 1;
					e.w = w; f(e);
					return e.stop ? 0 : 1;
				}, Zero);
			}

			/// <summary>
			/// Gets list of top-level windows of current thread or another thread.
			/// Uses Api.EnumThreadWindows().
			/// </summary>
			/// <param name="threadId">Unmanaged thread id. If 0, gets windows of current thread.</param>
			/// <param name="className">If not null/""/"*", gets only windows of this class. String by default is interpreted as wildcard, case-insensitive.</param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			public static List<Wnd> ThreadWindows(uint threadId = 0, WildStringI className = null, bool onlyVisible = false)
			{
				if(threadId == 0) threadId = Api.GetCurrentThreadId();
				List<Wnd> a = null;

				Api.EnumThreadWindows(threadId, (w, param) =>
				{
					if(onlyVisible && !w.Visible) return 1;
					if(className != null && !w.ClassNameIs(className)) return 1;
					if(a == null) a = new List<Wnd>();
					a.Add(w);
					return 1;
				}, 0);

				return a;

				//speed: ~40% of EnumWindows time, tested with a foreign thread with 30 windows.
			}

			#endregion

			#region controls

			/// <summary>
			/// Gets list of child controls of a window.
			/// Uses Api.EnumChildWindows().
			/// </summary>
			/// <param name="w">The top-level window or control whose child controls you need.</param>
			/// <param name="className">If not null/"", gets only controls of this class. String by default is interpreted as wildcard, case-insensitive.</param>
			/// <param name="directChild">Need only direct children, not grandchildren.</param>
			/// <param name="onlyVisible">Need only visible controls.</param>
			/// <param name="sortFirstVisible">Place all list elements of hidden controls at the end of the returned list.</param>
			public static List<Wnd> Controls(Wnd w, WildStringI className = null, bool directChild = false, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				List<Wnd> a = null, aHidden=null;
				if(onlyVisible) sortFirstVisible = false;

				Controls(e =>
				{
					if(sortFirstVisible && !e.w.Visible) {
						if(aHidden == null) aHidden = new List<Wnd>();
						aHidden.Add(e.w);
					} else {
						if(a == null) a = new List<Wnd>();
						a.Add(e.w);
					}
				}, w, className, directChild, onlyVisible);

				if(aHidden != null) a.AddRange(aHidden);
				return a;

				//tested: using a non-anonymous callback function does not make faster.
			}

			/// <summary>
			/// Calls callback function for each child control of a window.
			/// Uses Api.EnumChildWindows().
			/// </summary>
			/// <param name="f">Lambda etc callback function to call for each matching control. Example: <c>e =˃ { Out(e.w); if(e.w.Name=="Find") e.Stop(); }</c></param>
			/// <param name="w">The top-level window or control whose child controls you need.</param>
			/// <param name="className">If not null/""/"*", gets only controls of this class. String by default is interpreted as wildcard, case-insensitive.</param>
			/// <param name="directChild">Need only direct children, not grandchildren.</param>
			/// <param name="onlyVisible">Need only visible controls.</param>
			public static void Controls(Action<CallbackArgs> f, Wnd w, WildStringI className = null, bool directChild = false, bool onlyVisible = false)
			{
				var e = new CallbackArgs();

				Api.EnumChildWindows(w, (c, param) =>
				{
					if(onlyVisible && !c.Visible) return 1;
					if(directChild && c.DirectParentOrOwner != w) return 1;
					if(className != null && !c.ClassNameIs(className)) return 1;
					e.w = c; f(e);
					return e.stop ? 0 : 1;
				}, Zero);
			}

			//Better don't need this.
			///// <summary>
			///// Gets list of direct child controls of a window.
			///// Uses Get.FirstChild and Get.NextSibling. It is faster than Api.EnumChildWindows.
			///// Should be used only with windows of current thread. Else it is unreliable because, if some controls are zordered or destroyed while enumerating, some controls can be skipped or retrieved more than once.
			///// </summary>
			///// <param name="w">The top-level window or control whose child controls you need.</param>
			///// <param name="className">If not null/"", gets only controls of this class. Case-insensitive, wildcard (uses String.Like_()).</param>
			//public static List<Wnd> DirectChildControlsFastUnsafe(Wnd hwnd, string className = null)
			//{
			//	var wild = _GetWildcard(className);
			//	List<Wnd> a = null;
			//	for(Wnd c = Get.FirstChild(hwnd); !c.Is0; c = Get.NextSibling(c)) {
			//		if(wild != null && !c._ClassNameIs(wild)) continue;
			//		if(a == null) a = new List<Wnd>();
			//		a.Add(c);
			//	}
			//	return a;
			//}

			//Cannot use this because need a callback function. Unless we at first get all and store in an array (similar speed).
			//public static IEnumerable<Wnd> Controls(Wnd hwnd)
			//{
			//	Api.EnumChildWindows(hwnd, (t, param)=>
			//	{
			//		yield return t; //error, yield cannot be in an anonymous method etc
			//		return 1;
			//	}, Zero);
			//}

			#endregion

			#region main windows

			/// <summary>
			/// Get windows that have taskbar button and/or are included in the Alt+Tab sequence.
			/// Uses Get.NextMainWindow().
			/// </summary>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows store apps.</param>
			/// <remarks>Can get not exactly the same windows than are in the taskbar and Alt+Tab.</remarks>
			public static List<Wnd> MainWindows(bool allDesktops = false)
			{
				List<Wnd> a = null;

				MainWindows(e =>
				{
					if(a == null) a = new List<Wnd>(32);
					a.Add(e.w);
				}, allDesktops: allDesktops);

				return a;
			}

			/// <summary>
			/// Calls callback function for each window that has taskbar button and/or is included in the Alt+Tab sequence.
			/// Uses Get.NextMainWindow().
			/// </summary>
			/// <param name="f">Lambda etc callback function to call for each matching window. Example: <c>e =˃ { Out(e.w); if(e.w.Name=="Find") e.Stop(); }</c></param>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows store apps.</param>
			/// <remarks>Can get not exactly the same windows than are in the taskbar and Alt+Tab.</remarks>
			public static void MainWindows(Action<CallbackArgs> f, bool allDesktops = false)
			{
				var e = new CallbackArgs();

				for(Wnd w = Wnd0; ;) {
					w = Get.NextMainWindow(w, allDesktops: allDesktops);
					if(w.Is0) break;
					e.w = w; f(e);
					if(e.stop) break;
				}
			}

			#endregion
		}

		#region util

		public class CallbackArgs
		{
			public Wnd w;
			internal bool stop;

			public void Stop() { stop = true; }
		}

		#endregion
	}
}
