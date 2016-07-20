using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

//using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public partial struct Wnd
	{
		/// <summary>
		/// Sets transparency.
		/// On Windows 7 works only with top-level windows, on newer OS also with controls.
		/// </summary>
		/// <param name="allowTransparency">Set or remove Api.WS_EX_LAYERED style that is required for transparency. If false, other parameters are not used.</param>
		/// <param name="opacity">Opacity from 0 (completely transparent) to 255 (opaque). If null, sets default value (opaque).</param>
		/// <param name="color">Make pixels painted with this color completely transparent. If null, sets default value (no transparent color).</param>
		public bool Transparency(bool allowTransparency, uint? opacity = null, uint? colorABGR = null)
		{
			uint est = ExStyle;
			bool layered = (est & Api.WS_EX_LAYERED) != 0;

			if(allowTransparency) {
				if(!layered && !SetExStyle(est | Api.WS_EX_LAYERED)) return false;

				uint col = 0, op = 0, f = 0;
				if(colorABGR != null) { f |= 1; col = colorABGR.Value; }
				if(opacity != null) { f |= 2; op = opacity.Value; if(op > 255) op = 255; }

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
		/// <param name="big">Get 32x32 icon.</param>
		public IntPtr GetIconHandle(bool big = false)
		{
			//support Windows Store apps
			string appId = null;
			if(1 == _WindowsStoreAppId(this, out appId, true)) {
				IntPtr R = Files.GetIconHandle(appId, big ? 32 : 16);
				if(R != Zero) return R;
			}

			LPARAM _R;
			SendTimeout(1000, out _R, Api.WM_GETICON, big);
			if(_R == 0) SendTimeout(1000, out _R, Api.WM_GETICON, !big);
			if(_R == 0) _R = GetClassLong(big ? Api.GCL_HICON : Api.GCL_HICONSM);
			if(_R == 0) _R = GetClassLong(big ? Api.GCL_HICONSM : Api.GCL_HICON);

			if(_R != 0) return Api.CopyIcon(_R);

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
			Api.WINDOWPLACEMENT p;
			if(GetWindowPlacement(out p)) {
				//OutList(p.showCmd, p.flags);
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
			Api.WINDOWPLACEMENT p; int siz = Marshal.SizeOf(typeof(Api.WINDOWPLACEMENT));
			if(siz == Registry_.GetBinary(&p, siz, valueName, key)) {
				//OutList(p.showCmd, p.flags);
				if(!showActivate && !Visible) {
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
				_Show(true);
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
				Zero = 0, //or Wnd0
				Top = 0, //SetWindowPos(HWND_TOP)
				Bottom = 1, //SetWindowPos(HWND_BOTTOM)
				Topmost = -1, //SetWindowPos(HWND_TOPMOST)
				NoTopmost = -2, //SetWindowPos(HWND_NOTOPMOST)
				Message = -3, //CreateWindowEx(HWND_MESSAGE)
				Broadcast = 0xffff //SendMessage(HWND_BROADCAST)
			}

			/// <summary>
			/// Calculates window rectangle from client area rectangle and style.
			/// Calls Api.AdjustWindowRectEx().
			/// </summary>
			/// <param name="r">Input - client area rectangle in screen. Output - window rectangle in screen.</param>
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
			public static int BorderWidth(uint style, uint exStyle)
			{
				var r = new RECT();
				Api.AdjustWindowRectEx(ref r, style, false, exStyle);
				return r.right;
			}

			/// <summary>
			/// Gets window border width.
			/// </summary>
			public static int BorderWidth(Wnd w)
			{
				Api.WINDOWINFO x;
				w.GetWindowInfo(out x);
				return (int)x.cxWindowBorders;
			}

			/// <summary>
			/// Calls Api.GetGUIThreadInfo(), which can get some GUI info, eg mouse capturing, menu mode, move/size mode, focus, caret.
			/// More info: MSDN -> GUITHREADINFO.
			/// </summary>
			/// <param name="g">Variable that receives the info.</param>
			/// <param name="idThread">Thread id. If 0 - the foreground (active window) thread.</param>
			public static bool GetGUIThreadInfo(out Api.GUITHREADINFO g, uint idThread = 0)
			{
				g = new Api.GUITHREADINFO(); g.cbSize = Api.SizeOf(g);
				return Api.GetGUIThreadInfo(idThread, ref g);
			}

			//public void ShowAnimate(bool show)
			//{
			//	//Don't add Wnd function, because:
			//		//Rarely used.
			//		//Api.AnimateWindow() works only with windows of current thread.
			//		//Only programmers would need it, and they can call the API directly.
			//}

			/// <summary>
			/// Registers and unregisters a window class.
			/// Normally you declare a static RegisterClass variable and call Register() or Superclass().
			/// </summary>
			public class RegisterClass
			{
				/// <summary>
				/// Class atom.
				/// </summary>
				public ushort Atom { get; private set; }

				/// <summary>
				/// Base class extra memory size.
				/// </summary>
				public int BaseClassWndExtra { get; private set; }

				/// <summary>
				/// Base class window procedure to pass to Api.CallWindowProc() in your window procedure.
				/// </summary>
				public Api.WNDPROC BaseClassWndProc { get; private set; }

				IntPtr _hinst; //need for Unregister()
				Api.WNDPROC _wndProc; //to keep reference to the caller's delegate to prevent garbage collection
				string _className; //for warning text in Unregister()

				~RegisterClass()
				{
					Unregister();
				}

				/// <summary>
				/// Registers window class.
				/// Calls Api.RegisterClassEx() and returns class atom.
				/// Does nothing and returns class atom if the class is already registered using this variable.
				/// More info: MSDN -> WNDCLASSEX.
				/// </summary>
				/// <param name="className">Class name.</param>
				/// <param name="wndProc">Window procedure delegate. This variable will keep a reference to it to prevent garbage-collecting.</param>
				/// <param name="wndExtra">Size of extra window memory which can be accessed with SetWindowLong/GetWindowLong with >=0 offset. Example: IntPtr.Size.</param>
				/// <param name="style">Class style.</param>
				/// <param name="ex">
				/// Can be used to specify WNDCLASSEX fields other than cbSize, lpszClassName, lpfnWndProc, cbWndExtra and style.
				/// If not used, the function sets: hCursor = arrow; hbrBackground = COLOR_BTNFACE+1; others = 0/null/Zero.
				/// </param>
				/// <exception cref="Win32Exception">When fails, for example if class className already exists.</exception>
				/// <remarks>
				/// If style does not have CS_GLOBALCLASS and ex is null or its hInstance field is not set, for hInstance uses exe module handle.
				/// </remarks>
				public ushort Register(string className, Api.WNDPROC wndProc, int wndExtra = 0, uint style = 0, Api.WNDCLASSEX? ex = null)
				{
					lock (this) {
						if(Atom == 0) {
							Api.WNDCLASSEX x = ex == null ? new Api.WNDCLASSEX() : ex.Value;
							if(ex == null) {
								x.hCursor = Api.LoadCursor(Zero, Api.IDC_ARROW);
								x.hbrBackground = (IntPtr)(Api.COLOR_BTNFACE + 1);
							}

							_Register(ref x, className, wndProc, wndExtra, style);
						}
						return Atom;

						//hInstance=Api.GetModuleHandle(null); //tested: RegisterClassEx uses this if hInstance is Zero, even for app-local classes

						//tested:
						//For app-global classes, CreateWindowEx and GetClassInfo ignore their hInst argument (always succeed).
						//For app-local classes, CreateWindowEx and GetClassInfo fail if their hInst argument does not match. However CreateWindowEx always succeeds if its hInst argument is Zero.
					}
				}

				void _Register(ref Api.WNDCLASSEX x, string className, Api.WNDPROC wndProc, int wndExtra, uint style)
				{
					try {
						x.cbSize = Api.SizeOf(x);
						x.lpszClassName = Marshal.StringToCoTaskMemUni(className);
						x.lpfnWndProc = wndProc;
						x.cbWndExtra = wndExtra;
						x.style = style;
						if(x.hInstance == Zero && (style & Api.CS_GLOBALCLASS) == 0) x.hInstance = Api.GetModuleHandle(null);

						Atom = Api.RegisterClassEx(ref x);
						if(Atom == 0) throw new Win32Exception();

						_hinst = x.hInstance; //if was set in ex, will need this for Unregister()
						_wndProc = wndProc; //keep the delegate from GC
						_className = className;
					}
					finally {
						Marshal.FreeCoTaskMem(x.lpszClassName);
					}
				}

				/// <summary>
				/// Registers window class that extends an existing class.
				/// Calls Api.GetClassInfoEx() and Api.RegisterClassEx(), and returns class atom.
				/// Does nothing and returns class atom if the class is already registered using this variable.
				/// More info: MSDN -> WNDCLASSEX.
				/// </summary>
				/// <param name="baseClassName">Existing class name.</param>
				/// <param name="className">New class name.</param>
				/// <param name="wndProc">Window procedure delegate. This variable will keep a reference to it to prevent garbage-collecting.</param>
				/// <param name="wndExtra">Size of extra window memory not including extra memory of base class. Can be accessed with SetMyLong/GetMyLong. Example: IntPtr.Size.</param>
				/// <param name="globalClass">If false, the function removes CS_GLOBALCLASS style.</param>
				/// <param name="baseModuleHandle">If the base class is global (CS_GLOBALCLASS style), don't use this parameter, else pass the module handle of the exe or dll that registered the base class.</param>
				/// <exception cref="Win32Exception">When fails, for example the base class does not exist, or class className already exists.</exception>
				public ushort Superclass(string baseClassName, string className, Api.WNDPROC wndProc, int wndExtra = 0, bool globalClass = false, IntPtr baseModuleHandle = default(IntPtr))
				{
					lock (this) {
						if(Atom == 0) {
							var x = new Api.WNDCLASSEX();
							if(0 == Api.GetClassInfoEx(baseModuleHandle, baseClassName, out x)) throw new Win32Exception();

							Api.WNDPROC wp = x.lpfnWndProc;
							int we = x.cbWndExtra;

							_Register(ref x, className, wndProc, x.cbWndExtra + wndExtra, globalClass ? x.style : x.style & ~Api.CS_GLOBALCLASS);

							BaseClassWndProc = wp;
							BaseClassWndExtra = we;
						}
						return Atom;
					}
				}

				/// <summary>
				/// Unregisters the window class if registered with Register() or Superclass().
				/// Called implicitly when garbage-collecting the object.
				/// Uses Debug.Assert.
				/// </summary>
				public void Unregister()
				{
					if(Atom != 0) {
						bool ok = Api.UnregisterClass(Atom, _hinst);
						if(!ok) Output.Warning($"Failed to unregister window class '{_className}'. {ThreadError.GetWinErorText()}.");
						Debug.Assert(ok);
						Atom = 0;
						_hinst = Zero;
						BaseClassWndProc = null;
						BaseClassWndExtra = 0;
					}
				}

				/// <summary>
				/// Calls SetWindowLong() and returns its return value.
				/// </summary>
				/// <param name="w">Window.</param>
				/// <param name="value">Value.</param>
				/// <param name="offset">Offset in extra memory, not including the size of extra memory of base class.</param>
				public LPARAM SetMyLong(Wnd w, LPARAM value, int offset = 0)
				{
					return w.SetWindowLong(BaseClassWndExtra + offset, value);
				}

				/// <summary>
				/// Calls GetWindowLong() and returns its return value.
				/// </summary>
				/// <param name="w">Window.</param>
				/// <param name="offset">Offset in extra memory, not including the size of extra memory of base class.</param>
				public LPARAM GetMyLong(Wnd w, int offset = 0)
				{
					return w.GetWindowLong(BaseClassWndExtra + offset);
				}

				/// <summary>
				/// Gets atom of a window class.
				/// To get class atom when you have a window w, use w.GetClassLong(Api.GCW_ATOM).
				/// </summary>
				/// <param name="className">Class name.</param>
				/// <param name="moduleHandle">Native module handle of the exe or dll that registered the class. Don't use if it is a global class (CS_GLOBALCLASS style).</param>
				public static ushort GetClassAtom(string className, IntPtr moduleHandle = default(IntPtr))
				{
					var x = new Api.WNDCLASSEX();
					return _GetClassInfoEx(moduleHandle, className, ref x);
				}

				//use this with [In], to avoid marshaling lpfnWndProc from native callback to C# delegate
				[DllImport("user32.dll", EntryPoint = "GetClassInfoExW", SetLastError = true)]
				static extern ushort _GetClassInfoEx(IntPtr hInstance, string lpszClass, [In] ref Api.WNDCLASSEX lpwcx);
			}

			/// <summary>
			/// Taskbar button flash, progress, add/delete.
			/// </summary>
			public static class TaskbarButton
			{
				/// <summary>
				/// Starts or stops flashing the taskbar button.
				/// </summary>
				/// <param name="w">Window.</param>
				/// <param name="count">The number of times to flash. If 0, stops flashing.</param>
				public static void Flash(Wnd w, int count)
				{
					//const uint FLASHW_STOP = 0;
					//const uint FLASHW_CAPTION = 0x00000001;
					const uint FLASHW_TRAY = 0x00000002;
					//const uint FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY;
					//const uint FLASHW_TIMER = 0x00000004;
					//const uint FLASHW_TIMERNOFG = 0x0000000C;

					var fi = new Api.FLASHWINFO(); fi.cbSize = Api.SizeOf(fi); fi.hwnd = w;
					if(count > 0) {
						fi.uCount = (uint)count;
						//fi.dwTimeout = (uint)periodMS; //not useful
						fi.dwFlags = FLASHW_TRAY;
					}
					Api.FlashWindowEx(ref fi);

					//tested. FlashWindow is easier but does not work for taskbar button, only for caption when no taskbar button.
				}

				public enum ProgressState
				{
					NoProgress = 0,
					Indeterminate = 0x1,
					Normal = 0x2,
					Error = 0x4,
					Paused = 0x8
				}

				/// <summary>
				/// Sets the state of the progress indicator displayed on the taskbar button.
				/// More info in MSDN, ITaskbarList3.SetProgressState.
				/// </summary>
				/// <param name="state">Progress indicator state and color.</param>
				public static void SetProgressState(Wnd w, ProgressState state)
				{
					_TaskbarButton.taskbarInstance.SetProgressState(w, state);
				}

				/// <summary>
				/// Sets the value of the progress indicator displayed on the taskbar button.
				/// More info in MSDN, ITaskbarList3.SetProgressValue.
				/// </summary>
				/// <param name="state">If not null, sets progress indicator state and color. Calls ITaskbarList3.SetProgressState.</param>
				/// <param name="progressValue">Progress indicator value, 0 to progressTotal.</param>
				/// <param name="progressTotal">Max progress indicator value.</param>
				public static void SetProgressValue(Wnd w, int progressValue, int progressTotal = 100)
				{
					_TaskbarButton.taskbarInstance.SetProgressValue(w, progressValue, progressTotal);
				}

				/// <summary>
				/// Adds taskbar button.
				/// Uses ITaskbarList.AddTab().
				/// </summary>
				/// <param name="w">Window.</param>
				public static void Add(Wnd w)
				{
					_TaskbarButton.taskbarInstance.AddTab(w);
					//info: succeeds without HrInit(), tested on Win10 and 7.
					//info: always returns 0, even if w is 0. Did not test ITaskbarList3 methods.
				}

				/// <summary>
				/// Deletes taskbar button.
				/// Uses ITaskbarList.DeleteTab().
				/// </summary>
				/// <param name="w">Window.</param>
				public static void Delete(Wnd w)
				{
					_TaskbarButton.taskbarInstance.DeleteTab(w);
				}

				internal static class _TaskbarButton
				{
					[ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
					internal interface ITaskbarList3
					{
						// ITaskbarList
						[PreserveSig]
						int HrInit();
						[PreserveSig]
						int AddTab(Wnd hwnd);
						[PreserveSig]
						int DeleteTab(Wnd hwnd);
						[PreserveSig]
						int ActivateTab(Wnd hwnd);
						[PreserveSig]
						int SetActiveAlt(Wnd hwnd);

						// ITaskbarList2
						[PreserveSig]
						int MarkFullscreenWindow(Wnd hwnd, bool fFullscreen);

						// ITaskbarList3
						[PreserveSig]
						int SetProgressValue(Wnd hwnd, long ullCompleted, long ullTotal);
						[PreserveSig]
						int SetProgressState(Wnd hwnd, ProgressState state);
						[PreserveSig]
						int RegisterTab(Wnd hwndTab, Wnd hwndMDI);
						[PreserveSig]
						int UnregisterTab(Wnd hwndTab);
						[PreserveSig]
						int SetTabOrder(Wnd hwndTab, Wnd hwndInsertBefore);
						[PreserveSig]
						int SetTabActive(Wnd hwndTab, Wnd hwndMDI, uint dwReserved);
						[PreserveSig]
						int ThumbBarAddButtons(Wnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig]
						int ThumbBarUpdateButtons(Wnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig]
						int ThumbBarSetImageList(Wnd hwnd, IntPtr himl);
						[PreserveSig]
						int SetOverlayIcon(Wnd hwnd, IntPtr hIcon, string pszDescription);
						[PreserveSig]
						int SetThumbnailTooltip(Wnd hwnd, string pszTip);
						[PreserveSig]
						int SetThumbnailClip(Wnd hwnd, ref RECT prcClip);
					}

					[ComImport]
					[Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
					[ClassInterface(ClassInterfaceType.None)]
					internal class TaskbarInstance
					{
					}

					internal static ITaskbarList3 taskbarInstance = (ITaskbarList3)new TaskbarInstance();
				}
			}

			/// <summary>
			/// Arranges windows, shows/hides desktop.
			/// The same as the taskbar right-click menu commands.
			/// </summary>
			public static class Arrange
			{
				/// <summary>
				/// Shows or hides desktop.
				/// If there are non-minimized main windows, minimizes them. Else restores windows recently minimized by this function.
				/// </summary>
				public static void ShowDesktop()
				{
					_Do(0);
				}
				
				/// <summary>
				/// Minimizes main windows.
				/// </summary>
				public static void MinimizeWindows()
				{
					_Do(1);
				}

				/// <summary>
				/// Cascades non-minimized main windows.
				/// </summary>
				public static void CascadeWindows()
				{
					_Do(3);
				}
				
				/// <summary>
				/// Arranges non-minimized main windows horizontally or vertically.
				/// </summary>
				public static void TileWindows(bool vertically)
				{
					_Do(vertically ? 5 : 4);
				}

				/// <summary>
				/// Restores windows recently minimized, cascaded or tiled with other functions of this class.
				/// </summary>
				public static void UndoMinimizeCascadeTile()
				{
					_Do(2);
				}

				static void _Do(int what)
				{
					try {
						dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application")); //speed: faster than calling a method
						switch(what) {
						case 0: shell.ToggleDesktop(); break;
						case 1: shell.MinimizeAll(); break;
						case 2: shell.UndoMinimizeALL(); break;
						case 3: shell.CascadeWindows(); break;
						case 4: shell.TileHorizontally(); break;
						case 5: shell.TileVertically(); break;
						}
						Marshal.ReleaseComObject(shell);
					}
					catch { }
				}
			}

			//FUTURE: use IVirtualDesktopManager to manage virtual desktops.
			//Currently almost not useful, because its MoveWindowToDesktop does not work with windows of other processes.
			//But in the future, if we'll have a dll to inject into another process, eg to find accessible objects faster, then also can add this to it.
			//The inteface also has IsWindowOnCurrentVirtualDesktop and GetWindowDesktopId.
			//Also there are internal/undocumented interfaces to add/remove/switch desktops etc. There is a GitHub library. And another library that injects.
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
	}
}
