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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	public partial struct Wnd
	{
		static Wnd _wDesktop = Api.GetDesktopWindow();

		/// <summary>
		/// Gets or sets the owner window of this top-level window.
		/// A window that has an owner window is always on top of its owner window.
		/// Don't call this for controls, they don't have an owner window.
		/// <para>The 'get' function calls Api.GetWindow(Api.GW_OWNER). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().</para>
		/// <para>The 'set' function calls Api.SetWindowLong(Api.GWL_HWNDPARENT, (LPARAM)value); it can fail, eg if the owner's process has higher UAC integrity level; supports ThreadError.</para>
		/// </summary>
		public Wnd Owner
		{
			get { return Api.GetWindow(this, Api.GW_OWNER); }
			set { SetWindowLong(Api.GWL_HWNDPARENT, (LPARAM)value); }
		}

		/// <summary>
		/// Gets the top-level parent window of this control.
		/// If this is a top-level window, returns this window.
		/// Calls Api.GetAncestor(Api.GA_ROOT). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd ToplevelParentOrThis { get { return Api.GetAncestor(this, Api.GA_ROOT); } }

		/// <summary>
		/// Gets direct parent of this control. It can be its top-level parent window or parent control.
		/// Returns Wnd0 (0) if this is a top-level window.
		/// Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd DirectParent
		{
			get
			{
				Wnd R = Api.GetParent(this);
				if(R.Is0 || R == Api.GetWindow(this, Api.GW_OWNER) || R == _wDesktop) return Wnd0;
				return R;
				//tested: GetAncestor much slower. IsChild also slower.
				//About 'R == _wDesktop': it is rare but I have seen that desktop is owner.
				//	If with createwindow owner is 0 or desktop, both getparent and getwindow return 0.
				//	But if with setwindowlong owner is desktop, both return desktop.
			}
		}

		/// <summary>
		/// Gets direct parent of this control. It can be its top-level parent window or parent control.
		/// If this is a top-level window, gets its owner, or returns Wnd0 (0) if it is unowned.
		/// Calls Api.GetParent(). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
		/// </summary>
		public Wnd DirectParentOrOwner { get { return Api.GetParent(this); } }

		/// <summary>
		/// Returns true if this is a child window (control), false if top-level window.
		/// Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
		/// </summary>
		public bool IsChildWindow { get { return !DirectParent.Is0; } }

		/// <summary>
		/// Returns true if this is a direct or indirect child of window w.
		/// Calls Api.IsChild(). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
		/// </summary>
		/// <param name="w">A top-level window or control.</param>
		public bool IsChildOf(Wnd w) { return Api.IsChild(w, this); }

		/// <summary>
		/// Static functions that get windows/controls in Z order, parent/child, owner/owned, special windows.
		/// Example: <c>Wnd w2=Wnd.Get.NextSibling(w1);</c>
		/// The Wnd class also has copies of some often used Wnd.Get functions.
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
			/// Normally it is a topmost window.
			/// Calls Api.GetTopWindow(Wnd0).
			/// </summary>
			public static Wnd FirstToplevel() { return Api.GetTopWindow(Wnd0); }

			/// <summary>
			/// Gets the first window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDFIRST). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd FirstSibling(Wnd w) { return Api.GetWindow(w, Api.GW_HWNDFIRST); }

			/// <summary>
			/// Gets the last window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDLAST). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd LastSibling(Wnd w) { return Api.GetWindow(w, Api.GW_HWNDLAST); }

			/// <summary>
			/// Gets next window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDNEXT). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd NextSibling(Wnd w) { return Api.GetWindow(w, Api.GW_HWNDNEXT); }

			/// <summary>
			/// Gets next window in the same Z order as w. If w is top-level, gets a top-level window, else gets a control of the same parent.
			/// Calls Api.GetWindow(w, Api.GW_HWNDPREV). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd PreviousSibling(Wnd w) { return Api.GetWindow(w, Api.GW_HWNDPREV); }

			/// <summary>
			/// Gets the first w child control in the Z order.
			/// Calls Api.GetWindow(w, Api.GW_CHILD). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd FirstChild(Wnd w) { return Api.GetWindow(w, Api.GW_CHILD); }

			/// <summary>
			/// Gets the owner window of top-level window w.
			/// A window that has an owner window is always on top of its owner window.
			/// Calls Api.GetWindow(w, Api.GW_OWNER). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd Owner(Wnd w) { return Api.GetWindow(w, Api.GW_OWNER); }

			/// <summary>
			/// Gets the first (top) enabled window in the chain of windows owned by w, or w itself if there are no such windows.
			/// Calls Api.GetWindow(w, Api.GW_ENABLEDPOPUP). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd FirstEnabledOwnedOrThis(Wnd w) { return Api.GetWindow(w, Api.GW_ENABLEDPOPUP); }

			/// <summary>
			/// Gets the most recently active window owned by w, or w itself if it was the most recently active.
			/// Calls Api.GetLastActivePopup(w). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd LastSeenActiveOwnedOrThis(Wnd w) { return Api.GetLastActivePopup(w); }

			/// <summary>
			/// Gets the top-level parent window of control w. If w is a top-level window, returns w.
			/// Calls Api.GetAncestor(Api.GA_ROOT). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd ToplevelParentOrThis(Wnd w) { return Api.GetAncestor(w, Api.GA_ROOT); }

			/// <summary>
			/// Gets the most bottom owner window in the chain of owner windows of w. If w is not owned, returns w.
			/// Calls Api.GetAncestor(Api.GA_ROOTOWNER). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd RootOwnerOrThis(Wnd w) { return Api.GetAncestor(w, Api.GA_ROOTOWNER); }

			/// <summary>
			/// Gets direct parent of the specified control. It can be its top-level parent window or parent control.
			/// If w is a top-level window, gets its owner, or returns Wnd0 (0) if it is unowned.
			/// Calls Api.GetParent(). Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd DirectParentOrOwner(Wnd w) { return Api.GetParent(w); }

			/// <summary>
			/// Gets direct parent of the specified control. It can be its top-level parent window or parent control.
			/// Returns Wnd0 (0) if w is a top-level window.
			/// Supports Api.SetLastError(0)/Marshal.GetLastWin32Error().
			/// </summary>
			public static Wnd DirectParent(Wnd w) { return w.DirectParent; }

			/// <summary>
			/// Gets the virtual parent window of all top-level windows.
			/// It is not the window that contains desktop icons.
			/// Calls Api.GetDesktopWindow().
			/// </summary>
			public static Wnd DesktopWindow { get { return _wDesktop; } }

			/// <summary>
			/// Gets a window of the shell process (usually process "explorer", class name "Progman").
			/// The window belongs to the same thread as the window that contains desktop icons.
			/// Calls Api.GetShellWindow().
			/// </summary>
			public static Wnd ShellWindow { get { return Api.GetShellWindow(); } }

			/// <summary>
			/// Gets the top-level window that contains desktop icons.
			/// </summary>
			public static Wnd Desktop { get { return DesktopListview.ToplevelParentOrThis; } }

			/// <summary>
			/// Gets the "SysListView32" control that contains desktop icons.
			/// </summary>
			public static Wnd DesktopListview
			{
				get
				{
					if(!__wDesktopLV.IsValid) {
						Wnd wShell = ShellWindow;
						Wnd wLV = wShell.Child(null, "SysListView32");
						if(wLV.Is0) {
							foreach(Wnd t in ThreadWindows(wShell.ThreadId, "WorkerW", true)) {
								wLV = t.Child(null, "SysListView32"); if(!wLV.Is0) break;
							}
						}
						__wDesktopLV = wLV;
					}
					return __wDesktopLV;

					//info:
					//If no wallpaper, desktop is GetShellWindow, else a visible WorkerW window.
					//When was no wallpaper and user selects a wallpaper, explorer creates WorkerW and moves the same SysListView32 control to it.
				}
			}
			static Wnd __wDesktopLV; //cached Desktop SysListView32 control

			/// <summary>
			/// Gets next window in the Z order, skipping invisible and other windows that would not be added to taskbar or not activated by Alt+Tab.
			/// Returns Wnd0 if there are no such windows.
			/// </summary>
			/// <param name="wFrom">Window after (behind) which to search. If omitted or Wnd0, starts from the top of the Z order.</param>
			/// <param name="retryFromTop">If wFrom is used and there are no matching windows after it, retry from the top of the Z order. Like Alt+Tab does. Can return wFrom.</param>
			/// <param name="skipMinimized">Skip minimized windows.</param>
			/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has uiAccess).</param>
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
				Wnd lastFound = Wnd0, w2 = Wnd0, w = wFrom;
				if(w.Is0) retryFromTop = false;

				for(;;) {
					w = w.Is0 ? FirstToplevel() : NextSibling(w);
					if(w.Is0) {
						if(retryFromTop) { retryFromTop = false; continue; }
						return lastFound;
					}

					if(!w.IsVisible) continue;

					uint exStyle = w.ExStyle;
					if((exStyle & Api.WS_EX_APPWINDOW) == 0) {
						if((exStyle & (Api.WS_EX_TOOLWINDOW | Api.WS_EX_NOACTIVATE)) != 0) continue;
						w2 = w.Owner; if(!w2.Is0) { if(!likeAltTab || w2.IsVisible) continue; }
					}

					#region IsVisibleReally

					if(skipMinimized && w.IsMinimized) continue;

					if(WinVer >= Win10) {
						if(w.IsCloaked) {
							if(!allDesktops) continue;
							if((exStyle & Api.WS_EX_NOREDIRECTIONBITMAP) != 0) { //probably a store app
								switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
								case 1: continue; //Windows search, experience host, etc. Also app windows that normally would sit on ApplicationFrameWindow windows.
								case 2: if(_WindowsStoreAppFrameChild(w).Is0) continue; break;
								}
							}
						}
					} else if(WinVer >= Win8) {
						if((exStyle & Api.WS_EX_NOREDIRECTIONBITMAP) != 0 && !w.HasStyle(Api.WS_CAPTION)) {
							if(!allDesktops && (exStyle & Api.WS_EX_TOPMOST) != 0) continue; //skip store apps
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
							if(!w2.IsVisible || (skipMinimized && w2.IsMinimized)) w2 = w; //don't check cloaked etc for owned window if its owner passed
							if(w2 == wFrom) { lastFound = w2; continue; }
							w = w2;
						}
					}

					return w;
				}
			}

			//TODO: impl these

			//public static Wnd CatkeysManager { get { return ; } }

			//public static Wnd CatkeysEditor { get { return ; } }

			//public static Wnd CatkeysCodeEditControl { get { return ; } }

		}

		#region enum main windows

		/// <summary>
		/// Get windows that have taskbar button and/or are included in the Alt+Tab sequence.
		/// Returns list containing 0 or more window handles as Wnd.
		/// Uses Get.NextMainWindow().
		/// </summary>
		/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has uiAccess).</param>
		/// <remarks>Can get not exactly the same windows than are in the taskbar and Alt+Tab.</remarks>
		public static List<Wnd> MainWindows(bool allDesktops = false)
		{
			List<Wnd> a = new List<Wnd>();

			MainWindows(e =>
			{
				a.Add(e);
				return false;
			}, allDesktops: allDesktops);

			return a;
		}

		/// <summary>
		/// Calls callback function for each window that has taskbar button and/or is included in the Alt+Tab sequence.
		/// Uses Get.NextMainWindow().
		/// </summary>
		/// <param name="f">Lambda etc callback function to call for each matching window. Can return true to stop.</param>
		/// <param name="allDesktops">On Windows 10 include windows on all virtual desktops. On Windows 8 include Windows Store apps (only if this process has uiAccess).</param>
		/// <remarks>Can get not exactly the same windows than are in the taskbar and Alt+Tab.</remarks>
		public static void MainWindows(Func<Wnd, bool> f, bool allDesktops = false)
		{
			for(Wnd w = Wnd0; ;) {
				w = Get.NextMainWindow(w, allDesktops: allDesktops);
				if(w.Is0) break;
				if(f(w)) break;
			}
		}

		#endregion
	}
}
