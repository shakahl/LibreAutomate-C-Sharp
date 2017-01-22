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
		/// <summary>
		/// Sets transparency.
		/// On Windows 7 works only with top-level windows, on newer OS also with controls.
		/// </summary>
		/// <param name="allowTransparency">Set or remove Api.WS_EX_LAYERED style that is required for transparency. If false, other parameters are not used.</param>
		/// <param name="opacity">Opacity from 0.0 (completely transparent) to 1.0 (opaque). If null, sets default value (opaque).</param>
		/// <param name="colorABGR">Make pixels painted with this color completely transparent. If null, sets default value (no transparent color).</param>
		public bool Transparency(bool allowTransparency, double? opacity = null, uint? colorABGR = null)
		{
			//TODO: colorABGR: use Color or RGB color
			uint est = ExStyle;
			bool layered = (est & Api.WS_EX_LAYERED) != 0;

			if(allowTransparency) {
				if(!layered && !SetExStyle(est | Api.WS_EX_LAYERED)) return false;

				uint col = 0, op = 0, f = 0;
				if(colorABGR != null) { f |= 1; col = colorABGR.Value; }
				if(opacity != null) { f |= 2; op = (uint)(opacity.Value * 255); if(op > 255) op = 255; }

				if(!Api.SetLayeredWindowAttributes(this, col, (byte)op, f)) return ThreadError.SetWinError();
			} else if(layered) {
				//if(!Api.SetLayeredWindowAttributes(this, 0, 0, 0)) return ThreadError.SetWinError();
				if(!SetExStyle(est & ~Api.WS_EX_LAYERED)) return false;
			}

			return true;
		}

		/// <summary>
		/// Gets icon that is displayed in window title bar and in its taskbar button.
		/// Returns icon handle if successful, else Zero. Later call Api.DestroyIcon().
		/// </summary>
		/// <param name="large">Get large icon.</param>
		/// <remarks>Icon size depends on DPI (text size, can be changed in Control Panel). By default small is 16, large 32.</remarks>
		public IntPtr GetIconHandle(bool large = false)
		{
			int size = Api.GetSystemMetrics(large ? Api.SM_CXICON : Api.SM_CXSMICON);

			//support Windows Store apps
			string appId = null;
			if(1 == _WindowsStoreAppId(this, out appId, true)) {
				IntPtr hi = Icons.GetFileIconHandle(appId, size);
				if(hi != Zero) return hi;
			}

			LPARAM R;
			SendTimeout(1000, out R, Api.WM_GETICON, large);
			if(R == 0) SendTimeout(1000, out R, Api.WM_GETICON, !large);
			if(R == 0) R = GetClassLong(large ? Api.GCL_HICON : Api.GCL_HICONSM);
			if(R == 0) R = GetClassLong(large ? Api.GCL_HICONSM : Api.GCL_HICON);
			//tested this code with DPI 125%. Small icon of most windows match DPI (20), some 16, some 24.

			//Copy, because will DestroyIcon, also it resizes if need.
			if(R != 0) return Api.CopyImage(R, Api.IMAGE_ICON, size, size, 0);
			return Zero;
		}

		/// <summary>
		/// Gets or sets native font handle.
		/// Sends message Api.WM_GETFONT or Api.WM_SETFONT (redraws).
		/// </summary>
		public IntPtr FontHandle
		{
			get { return Send(Api.WM_GETFONT); }
			set { Send(Api.WM_SETFONT, value, true); }
		}

		/// <summary>
		/// Saves window position, dimensions and state in registry.
		/// </summary>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="Registry_.ParseKeyString"/>.</param>
		/// <param name="canBeMinimized">If now this window is minimized, let RegistryRestore make it minimized. If false, RegistryRestore will restore it to the most recent non-minimized state.</param>
		public unsafe bool RegistrySave(string valueName, string key = null, bool canBeMinimized = false)
		{
			Native.WINDOWPLACEMENT p;
			if(GetWindowPlacement(out p)) {
				//PrintList(p.showCmd, p.flags);
				if(!canBeMinimized && p.showCmd == Api.SW_SHOWMINIMIZED) p.showCmd = (p.flags & Api.WPF_RESTORETOMAXIMIZED) != 0 ? Api.SW_SHOWMAXIMIZED : Api.SW_SHOWNORMAL;
				return Registry_.SetBinary(&p, (int)Api.SizeOf(p), valueName, key);
			}
			return false;
		}

		/// <summary>
		/// Restores window position, dimensions and state saved in registry with RegistrySave().
		/// Returns true if the registry value exists and successfully restored size/dimensions.
		/// In any case uses ensureInScreen and onlySetPositionAndSize parameters.
		/// </summary>
		/// <param name="valueName">Registry value name.</param>
		/// <param name="key">Registry key. <see cref="Registry_.ParseKeyString"/>.</param>
		/// <param name="ensureInScreen">Call <see cref="EnsureInScreen"/>.</param>
		/// <param name="showActivate">Unhide and activate.</param>
		public unsafe bool RegistryRestore(string valueName, string key = null, bool ensureInScreen = false, bool showActivate = false)
		{
			bool R = false;
			Native.WINDOWPLACEMENT p; int siz = Marshal.SizeOf(typeof(Native.WINDOWPLACEMENT));
			if(siz == Registry_.GetBinary(&p, siz, valueName, key)) {
				//PrintList(p.showCmd, p.flags);
				if(!showActivate && !IsVisible) {
					uint style = Style;
					switch(p.showCmd) {
					case Api.SW_SHOWMAXIMIZED:
						if((style & Api.WS_MAXIMIZE) == 0) {
							SetStyle(style | Api.WS_MAXIMIZE);
							RECT r = Screen.FromHandle(_h).WorkingArea;
							r.Inflate(1, 1); //normally p.ptMaxPosition.x/y are -1 even if on non-primary monitor
							Rect = r;
						}
						break;
					case Api.SW_SHOWMINIMIZED:
						if((style & Api.WS_MINIMIZE) == 0) SetStyle(style | Api.WS_MINIMIZE);
						break;
					case Api.SW_SHOWNORMAL:
						if((style & (Api.WS_MAXIMIZE | Api.WS_MINIMIZE)) != 0) SetStyle(style & ~(Api.WS_MAXIMIZE | Api.WS_MINIMIZE));
						//never mind: if currently minimized, will not be restored. Usually currently is normal, because this func called after creating window, especially if invisible. But will restore if currently maximized.
						break;
					}
					p.showCmd = 0;
				}
				R = SetWindowPlacement(ref p);
			}

			if(ensureInScreen) EnsureInScreen();
			if(showActivate) {
				Show(true);
				ActivateRaw();
			}
			return R;
		}

		/// <summary>
		/// Returns true if this is a full-screen window and not desktop.
		/// </summary>
		public bool IsFullScreen
		{
			get
			{
				if(Is0) return false;

				//is client rect equal to window rect (no border)?
				RECT r, rc, rm;
				r = Rect; //fast
				int cx = r.right - r.left, cy = r.bottom - r.top;
				if(cx < 400 || cy < 300) return false; //too small
				rc = ClientRect; //fast
				if(rc.right != cx || rc.bottom != cy) {
					if(cx - rc.right > 2 || cy - rc.bottom > 2) return false; //some windows have 1-pixel border
				}

				//covers whole monitor rect?
				rm = Screen.FromHandle(Handle).Bounds; //fast except first time, because uses caching
				if(r.left > rm.left || r.top > rm.top || r.right < rm.right || r.bottom < rm.bottom - 1) return false; //info: -1 for inactive Chrome

				//is it desktop?
				if(IsOfShellThread) return false;
				if(this == Get.DesktopWindow) return false;

				return true;

				//This is the best way to test for fullscreen (FS) window. Fast.
				//Window and client rect was equal of almost all my tested FS windows. Except Winamp visualization.
				//Most FS windows are same size as screen, but some slightly bigger.
				//Don't look at window styles. For some FS windows they are not as should be.
				//Returns false if the active window is owned by a fullscreen window. This is different than appbar API interprets it. It's OK for our purposes.
			}
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's thread (usually it is the desktop window).
		/// </summary>
		public bool IsOfShellThread
		{
			get { return 1 == __isShellWindow.IsShellWindow(this); }
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's process (eg a folder window, desktop, taskbar).
		/// </summary>
		public bool IsOfShellProcess
		{
			get { return 0 != __isShellWindow.IsShellWindow(this); }
		}

		struct __ISSHELLWINDOW
		{
			uint _tidW, _tidD, _pidW, _pidD;
			IntPtr _w, _wDesk; //not Wnd because then TypeLoadException

			public int IsShellWindow(Wnd w)
			{
				Wnd wDesk = Get.ShellWindow; //fast
				if(w == wDesk) return 1; //Progman. Usually other window (WorkerW) is active when desktop active.

				//cache because GetWindowThreadProcessId quite slow
				if(w.Handle != _w) { _w = w.Handle; _tidW = Api.GetWindowThreadProcessId(w, out _pidW); }
				if(wDesk.Handle != _wDesk) { _wDesk = wDesk.Handle; _tidD = Api.GetWindowThreadProcessId(wDesk, out _pidD); }

				if(_tidW == _tidD) return 1;
				if(_pidW == _pidD) return 2;
				return 0;
			}
		}
		static __ISSHELLWINDOW __isShellWindow;

		/// <summary>
		/// Returns true if this window has Metro style, ie is not a classic desktop window.
		/// On Windows 8/8.1 most Windows Store app windows and shell windows have Metro style.
		/// On Windows 10 few windows have Metro style.
		/// On Windows 7 there are no Metro style windows.
		/// </summary>
		public bool IsWin8MetroStyle
		{
			get
			{
				if(WinVer < Win8) return false;
				if(!HasExStyle(Api.WS_EX_TOPMOST | Api.WS_EX_NOREDIRECTIONBITMAP) || (Style & Api.WS_CAPTION) != 0) return false;
				if(ClassNameIs("Windows.UI.Core.CoreWindow")) return true;
				if(WinVer < Win10 && IsOfShellProcess) return true;
				return false;
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// Returns non-zero if this window is a Windows 10 Store app window.
		/// Returns 1 if class name is "ApplicationFrameWindow", 2 if "Windows.UI.Core.CoreWindow".
		/// </summary>
		public int IsWin10StoreApp
		{
			get
			{
				if(WinVer < Win10) return 0;
				if(!HasExStyle(Api.WS_EX_NOREDIRECTIONBITMAP)) return 0;
				return ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow");
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// Minimizes or restores main windows, to show or hide desktop.
		/// </summary>
		/// <param name="on">
		/// If omitted or null, calls <see cref="Misc.Arrange.ShowDesktop"/>, which shows or hides desktop.
		/// If true, calls <see cref="Misc.Arrange.MinimizeWindows"/>, which minimizes main windows.
		/// If false, calls <see cref="Misc.Arrange.UndoMinimizeCascadeTile"/>, which restores windows recently minimized by this function.
		/// </param>
		public static void ShowDesktop(bool? on = null)
		{
			if(on == null) Misc.Arrange.ShowDesktop();
			else if(on.Value) Misc.Arrange.MinimizeWindows();
			else Misc.Arrange.UndoMinimizeCascadeTile();
		}

		/// <summary>
		/// Contains miscellaneous static window-related functions and classes, mostly rarely used or useful only for programmers.
		/// </summary>
		public static partial class Misc
		{
			/// <summary>
			/// Can be used with Windows API as a special window handle value that is implicitly converted to Wnd.
			/// Example: <c>SetWindowPos(w, Wnd.Misc.SpecHwnd.Topmost, ...); //HWND_TOPMOST</c>
			/// </summary>
			public enum SpecHwnd
			{
				/// <summary>0, the same as Wnd0 or default(Wnd).</summary>
				Zero = 0,
				/// <summary>SetWindowPos(HWND_TOP)</summary>
				Top = 0,
				/// <summary>SetWindowPos(HWND_BOTTOM)</summary>
				Bottom = 1,
				/// <summary>SetWindowPos(HWND_TOPMOST)</summary>
				Topmost = -1,
				/// <summary>SetWindowPos(HWND_NOTOPMOST)</summary>
				NoTopmost = -2,
				/// <summary>CreateWindowEx(HWND_MESSAGE)</summary>
				Message = -3,
				/// <summary>SendMessage(HWND_BROADCAST)</summary>
				Broadcast = 0xffff
			}

			/// <summary>
			/// Returns true if w is one of enum <see cref="SpecHwnd"/> members.
			/// </summary>
			public static bool IsSpecHwnd(Wnd w)
			{
				int i = (int)(LPARAM)w;
				return (i <= 1 && i >= -3) || i == 0xffff;
			}

			/// <summary>
			/// Calculates window rectangle from client area rectangle and style.
			/// Calls Api.AdjustWindowRectEx().
			/// </summary>
			/// <param name="r">Input - client area rectangle in screen. Output - window rectangle in screen.</param>
			/// <param name="style"></param>
			/// <param name="exStyle"></param>
			/// <param name="hasMenu"></param>
			/// <remarks>
			/// Ignores styles Api.WS_VSCROLL, Api.WS_HSCROLL and wrapped menu bar.
			/// </remarks>
			public static bool WindowRectFromClientRect(ref RECT r, uint style, uint exStyle, bool hasMenu = false)
			{
				return Api.AdjustWindowRectEx(ref r, style, hasMenu, exStyle);
			}

			/// <summary>
			/// Calculates window border width from style.
			/// </summary>
			/// <param name="style"></param>
			/// <param name="exStyle"></param>
			public static int BorderWidth(uint style, uint exStyle)
			{
				var r = new RECT();
				Api.AdjustWindowRectEx(ref r, style, false, exStyle);
				return r.right;
			}

			/// <summary>
			/// Gets window border width.
			/// </summary>
			/// <param name="w"></param>
			public static int BorderWidth(Wnd w)
			{
				Native.WINDOWINFO x;
				w.GetWindowInfo(out x);
				return (int)x.cxWindowBorders;
			}

			/// <summary>
			/// Calls Api.GetGUIThreadInfo(), which can get some GUI info, eg mouse capturing, menu mode, move/size mode, focus, caret.
			/// More info: MSDN -> GUITHREADINFO.
			/// </summary>
			/// <param name="g">Variable that receives the info.</param>
			/// <param name="idThread">Thread id. If 0 - the foreground (active window) thread.</param>
			public static bool GetGUIThreadInfo(out Native.GUITHREADINFO g, uint idThread = 0)
			{
				g = new Native.GUITHREADINFO(); g.cbSize = Api.SizeOf(g);
				return Api.GetGUIThreadInfo(idThread, ref g);
			}

			/// <summary>
			/// Creates unmanaged message-only window.
			/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
			/// </summary>
			/// <param name="className">Window class name.</param>
			public static Wnd CreateMessageWindow(string className)
			{
				return Api.CreateWindowEx(Api.WS_EX_NOACTIVATE, className, null, Api.WS_POPUP, 0, 0, 0, 0, SpecHwnd.Message, 0, Zero, 0);
				//note: WS_EX_NOACTIVATE is important.
			}

			//public void ShowAnimate(bool show)
			//{
			//	//Don't add Wnd function, because:
			//		//Rarely used.
			//		//Api.AnimateWindow() works only with windows of current thread.
			//		//Only programmers would need it, and they can call the API directly.
			//}
		}






		//UTIL

		static partial class _Api
		{
			[DllImport("kernel32.dll")]
			internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref uint AppModelIDLength, [Out] StringBuilder sbAppUserModelID);
		}

		/// <summary>
		/// Gets window Windows Store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
		/// Returns 1 if gets user model id, 2 if gets path, 0 if fails.
		/// </summary>
		/// <param name="w">Window.</param>
		/// <param name="appId">Receives app ID.</param>
		/// <param name="prependShellAppsFolder">Prepend @"shell:AppsFolder\" (to run or get icon).</param>
		/// <param name="getExePathIfNotWinStoreApp">Get exe full path if hwnd is not a Windows Store app.</param>
		static int _WindowsStoreAppId(Wnd w, out string appId, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
		{
			appId = null;

			if(WinVer >= Win8) {
				switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
				case 1:
					using(var p = new Process_.LibProcessHandle(w)) {
						if(!p.Is0) {
							uint u = 1000; var sb = new StringBuilder((int)u);
							if(0 == _Api.GetApplicationUserModelId(p, ref u, sb)) appId = sb.ToString();
						}
					}
					break;
				case 2:
					if(WinVer >= Win10) {
						Api.IPropertyStore ps; Api.PROPVARIANT_LPARAM v;
						if(0 == Api.SHGetPropertyStoreForWindow(w, ref Api.IID_IPropertyStore, out ps)) {
							if(0 == ps.GetValue(ref Api.PKEY_AppUserModel_ID, out v)) {
								if(v.vt == (ushort)Api.VARENUM.VT_LPWSTR) appId = Marshal.PtrToStringUni(v.value);
								Api.PropVariantClear(ref v);
							}
							Marshal.ReleaseComObject(ps);
						}
					}
					break;
				}

				if(appId != null) {
					if(prependShellAppsFolder) appId = @"shell:AppsFolder\" + appId;
					return 1;
				}
			}

			if(getExePathIfNotWinStoreApp) {
				appId = w.ProcessPath;
				if(appId != null) return 2;
			}

			return 0;
		}

		//static int _WindowsStoreAppId2(Wnd w, out string appID, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
		//{
		//	appID = null;

		//	if(getExePathIfNotWinStoreApp) {
		//		if(WinVer >= Win8) {
		//			bool isApp = false;
		//			switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
		//			case 1:
		//				isApp = true;
		//				break;
		//			case 2:
		//				if(WinVer >= Win10) {
		//					Wnd t = _WindowsStoreAppFrameChild(w);
		//					if(!t.Is0) { w = t; isApp = true; }
		//				}
		//				break;
		//			}
		//			if(!isApp) {
		//				appID = w.ProcessPath;
		//				return (appID == null) ? 0 : 2;
		//			}
		//		}

		//	}

		//	using(var p = new Process_.LibProcessHandle(w)) {
		//		if(p.Is0) return 0;
		//		uint u = 1000;
		//		var sb = new StringBuilder((int)u);
		//		if(0 != _Api.GetApplicationUserModelId(p, ref u, sb)) return 0;
		//		if(prependShellAppsFolder) appID = @"shell:AppsFolder\" + sb.ToString(); else appID = sb.ToString();
		//		return 1;
		//	}
		//}

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
				if(c.IsCloaked) return c; //else probably it is an unrelated window
			}

			retry = true;
			goto g1;
		}

		/// <summary>
		/// The reverse of _WindowsStoreAppFrameChild.
		/// </summary>
		static Wnd _WindowsStoreAppHost(Wnd w)
		{
			if(WinVer < Win10 || !w.ClassNameIs("Windows.UI.Core.CoreWindow")) return Wnd0;
			Wnd wo = w.DirectParent; if(!wo.Is0 && wo.ClassNameIs("ApplicationFrameWindow")) return wo;
			string s = w.Name; if(Empty(s)) return Wnd0;
			return Api.FindWindow("ApplicationFrameWindow", s);
		}


		//This can be used, but not much simpler than calling ATI directly and using try/finally.
		//internal struct LibAttachThreadInput :IDisposable
		//{
		//	uint _tid;

		//	public bool Attach(uint tid)
		//	{
		//		if(!Api.AttachThreadInput(Api.GetCurrentThreadId(), _tid, true)) return false;
		//		_tid = tid; return true;
		//	}

		//	public bool Attach(Wnd w) { return Attach(w.ThreadId); }

		//	public void Dispose() { if(_tid != 0) Api.AttachThreadInput(Api.GetCurrentThreadId(), _tid, false); }
		//}
	}
}
