using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Text; //StringBuilder, we don't use it, very slow
using System.Drawing; //Point, Size
using Microsoft.Win32.SafeHandles;

[module: DefaultCharSet(CharSet.Unicode)] //change default DllImport CharSet from ANSI to Unicode

//[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32|DllImportSearchPath.UserDirectories)]

[assembly: InternalsVisibleTo("Au.Controls, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Tools, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Editor, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Tasks, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Compiler, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Triggers, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("SdkConverter, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("TreeList, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("LineBreaks, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]

#pragma warning disable IDE1006 // Naming Styles

namespace Au.Types
{
	[DebuggerStepThrough]
	[System.Security.SuppressUnmanagedCodeSecurity]
	//[CLSCompliant(false)]
	internal static unsafe partial class Api
	{
		#region util

		/// <summary>
		/// Gets the native size of a struct variable.
		/// Returns (uint)Marshal.SizeOf(typeof(T)).
		/// Speed: the same (in Release config) as Marshal.SizeOf(typeof(T)), and 2 times faster than Marshal.SizeOf(v).
		/// </summary>
		internal static uint SizeOf<T>(T v) { return (uint)Marshal.SizeOf(typeof(T)); }

		/// <summary>
		/// Gets the native size of a type.
		/// Returns (uint)Marshal.SizeOf(typeof(T)).
		/// </summary>
		internal static uint SizeOf<T>() { return (uint)Marshal.SizeOf(typeof(T)); }

		/// <summary>
		/// Gets dll module handle (Api.GetModuleHandle) or loads dll (Api.LoadLibrary), and returns unmanaged exported function address (Api.GetProcAddress).
		/// See also: GetDelegate.
		/// </summary>
		internal static IntPtr GetProcAddress(string dllName, string funcName)
		{
			IntPtr hmod = GetModuleHandle(dllName);
			if(hmod == default) { hmod = LoadLibrary(dllName); if(hmod == default) return hmod; }

			return GetProcAddress(hmod, funcName);
		}

		/// <summary>
		/// Calls <see cref="GetProcAddress(string, string)"/> (loads dll or gets handle) and <see cref="Marshal.GetDelegateForFunctionPointer"/>.
		/// </summary>
		internal static bool GetDelegate<T>(out T deleg, string dllName, string funcName) where T : class
		{
			deleg = null;
			IntPtr fa = GetProcAddress(dllName, funcName); if(fa == default) return false;
			deleg = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(fa, typeof(T)));
			return deleg != null;
		}

		/// <summary>
		/// Calls API <see cref="GetProcAddress(IntPtr, string)"/> and <see cref="Marshal.GetDelegateForFunctionPointer"/>.
		/// </summary>
		internal static bool GetDelegate<T>(out T deleg, IntPtr hModule, string funcName) where T : class
		{
			deleg = null;
			IntPtr fa = GetProcAddress(hModule, funcName); if(fa == default) return false;
			deleg = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(fa, typeof(T)));
			return deleg != null;
		}

		/// <summary>
		/// Calls <see cref="Marshal.GetDelegateForFunctionPointer"/>.
		/// </summary>
		/// <typeparam name="T">Delegate type.</typeparam>
		/// <param name="f">Unmanaged function address.</param>
		/// <param name="deleg">Receives managed delegate of type T.</param>
		internal static void GetDelegate<T>(IntPtr f, out T deleg) where T : class
		{
			deleg = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(f, typeof(T)));
		}

		/// <summary>
		/// If o is not null, calls <see cref="Marshal.ReleaseComObject"/>.
		/// </summary>
		internal static void ReleaseComObject<T>(T o) where T: class
		{
			if(o != null) Marshal.ReleaseComObject(o);
		}

		#endregion

		#region user32

		[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
		internal static extern LPARAM SendMessage(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
		internal static extern LPARAM SendMessageTimeout(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam, uint SMTO_X, uint uTimeout, out LPARAM lpdwResult);

		[DllImport("user32.dll", EntryPoint = "SendNotifyMessageW", SetLastError = true)]
		internal static extern bool SendNotifyMessage(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
		internal static extern bool PostMessage(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", EntryPoint = "PostThreadMessageW")]
		internal static extern bool PostThreadMessage(uint idThread, uint Msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
		internal static extern int GetWindowLong32(Wnd hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
		internal static extern LPARAM GetWindowLong64(Wnd hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
		internal static extern int SetWindowLong32(Wnd hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
		internal static extern LPARAM SetWindowLong64(Wnd hWnd, int nIndex, LPARAM dwNewLong);

		[DllImport("user32.dll", EntryPoint = "GetClassLongW", SetLastError = true)]
		internal static extern int GetClassLong32(Wnd hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtrW", SetLastError = true)]
		internal static extern LPARAM GetClassLong64(Wnd hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "SetClassLongW", SetLastError = true)]
		internal static extern int SetClassLong32(Wnd hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetClassLongPtrW", SetLastError = true)]
		internal static extern LPARAM SetClassLong64(Wnd hWnd, int nIndex, LPARAM dwNewLong);

		[DllImport("user32.dll", EntryPoint = "GetClassNameW", SetLastError = true)]
		internal static extern int GetClassName(Wnd hWnd, char* lpClassName, int nMaxCount);

		[DllImport("user32.dll", EntryPoint = "InternalGetWindowText", SetLastError = true)]
		internal static extern int InternalGetWindowText(Wnd hWnd, [Out] char[] pString, int cchMaxCount);

		internal struct COPYDATASTRUCT
		{
			public LPARAM dwData;
			public int cbData;
			public IntPtr lpData;

			public COPYDATASTRUCT(LPARAM dwData, int cbData, IntPtr lpData)
			{
				this.dwData = dwData; this.cbData = cbData; this.lpData = lpData;
			}
		}

		[DllImport("user32.dll")]
		internal static extern bool IsWindow(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsWindowVisible(Wnd hWnd);

		internal const int SW_HIDE = 0;
		internal const int SW_SHOWNORMAL = 1;
		internal const int SW_SHOWMINIMIZED = 2;
		internal const int SW_SHOWMAXIMIZED = 3;
		//internal const int SW_SHOWNOACTIVATE = 4; //restores min/max window
		internal const int SW_SHOW = 5;
		internal const int SW_MINIMIZE = 6;
		internal const int SW_SHOWMINNOACTIVE = 7;
		internal const int SW_SHOWNA = 8;
		internal const int SW_RESTORE = 9;
		internal const int SW_SHOWDEFAULT = 10;
		internal const int SW_FORCEMINIMIZE = 11;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern void ShowWindow(Wnd hWnd, int SW_X);
		//note: the returns value does not say succeeded/failed.
		//	It is non-zero if was visible, 0 if was hidden.
		//	Declared void to avoid programming errors.

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsWindowEnabled(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern void EnableWindow(Wnd hWnd, bool bEnable);
		//note: the returns value does not say succeeded/failed.
		//	It is non-zero if was disabled, 0 if was enabled.
		//	Declared void to avoid programming errors.

		[DllImport("user32.dll", EntryPoint = "FindWindowW", SetLastError = true)]
		internal static extern Wnd FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", EntryPoint = "FindWindowExW", SetLastError = true)]
		internal static extern Wnd FindWindowEx(Wnd hWndParent, Wnd hWndChildAfter, string lpszClass, string lpszWindow);

		internal struct WNDCLASSEX
		{
			public uint cbSize;
			public uint style;
			public IntPtr lpfnWndProc; //not WNDPROC to avoid auto-marshaling where don't need. Use Marshal.GetFunctionPointerForDelegate/GetDelegateForFunctionPointer.
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			public IntPtr lpszMenuName;
			public char* lpszClassName; //not string because CLR would call CoTaskMemFree
			public IntPtr hIconSm;

			public WNDCLASSEX(Wnd.Misc.MyWindow.WndClassEx ex = null) : this()
			{
				this.cbSize = SizeOf<WNDCLASSEX>();
				if(ex == null) {
					hCursor = LoadCursor(default, IDC_ARROW);
					hbrBackground = (IntPtr)(COLOR_BTNFACE + 1);
				} else {
					this.style = ex.style;
					this.cbClsExtra = ex.cbClsExtra;
					this.cbWndExtra = ex.cbWndExtra;
					//this.hInstance = ex.hInstance;
					this.hIcon = ex.hIcon;
					this.hCursor = ex.hCursor ?? LoadCursor(default, IDC_ARROW);
					this.hbrBackground = ex.hbrBackground ?? (IntPtr)(COLOR_BTNFACE + 1);
					this.hIconSm = ex.hIconSm;
				}
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

		[DllImport("user32.dll", EntryPoint = "GetClassInfoExW", SetLastError = true)]
		internal static extern ushort GetClassInfoEx(IntPtr hInstance, string lpszClass, ref WNDCLASSEX lpwcx);

		[DllImport("user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true)]
		internal static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

		[DllImport("user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true)]
		internal static extern bool UnregisterClass(uint classAtom, IntPtr hInstance);

		[DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
		internal static extern Wnd CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, Wnd hWndParent, LPARAM hMenu, IntPtr hInstance, LPARAM lpParam);

		[DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
		internal static extern LPARAM DefWindowProc(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
		internal static extern LPARAM CallWindowProc(LPARAM lpPrevWndFunc, Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);
		//internal static extern LPARAM CallWindowProc(Native.WNDPROC lpPrevWndFunc, Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool DestroyWindow(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern void PostQuitMessage(int nExitCode);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetMessage(out Native.MSG lpMsg, Wnd hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool TranslateMessage(ref Native.MSG lpMsg);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern LPARAM DispatchMessage(ref Native.MSG lpmsg);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool WaitMessage();

		internal const uint PM_NOREMOVE = 0x0;
		internal const uint PM_REMOVE = 0x1;
		internal const uint PM_NOYIELD = 0x2;
		internal const uint PM_QS_SENDMESSAGE = 0x400000;
		internal const uint PM_QS_POSTMESSAGE = 0x980000;
		internal const uint PM_QS_PAINT = 0x200000;
		internal const uint PM_QS_INPUT = 0x1C070000;

		[DllImport("user32.dll", EntryPoint = "PeekMessageW", SetLastError = true)]
		internal static extern bool PeekMessage(out Native.MSG lpMsg, Wnd hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

		internal const int WH_MSGFILTER = -1;
		internal const int WH_KEYBOARD = 2;
		internal const int WH_GETMESSAGE = 3;
		internal const int WH_CALLWNDPROC = 4;
		internal const int WH_CBT = 5;
		internal const int WH_SYSMSGFILTER = 6;
		internal const int WH_MOUSE = 7;
		internal const int WH_DEBUG = 9;
		internal const int WH_SHELL = 10;
		internal const int WH_FOREGROUNDIDLE = 11;
		internal const int WH_CALLWNDPROCRET = 12;
		internal const int WH_KEYBOARD_LL = 13;
		internal const int WH_MOUSE_LL = 14;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ReplyMessage(LPARAM lResult);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int WH_X, HOOKPROC lpfn, IntPtr hMod, int dwThreadId);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern LPARAM CallNextHookEx(IntPtr hhk, int nCode, LPARAM wParam, LPARAM lParam);

		internal const int HCBT_MOVESIZE = 0;
		internal const int HCBT_MINMAX = 1;
		//internal const int HCBT_QS = 2;
		internal const int HCBT_CREATEWND = 3;
		internal const int HCBT_DESTROYWND = 4;
		internal const int HCBT_ACTIVATE = 5;
		internal const int HCBT_CLICKSKIPPED = 6;
		internal const int HCBT_KEYSKIPPED = 7;
		internal const int HCBT_SYSCOMMAND = 8;
		internal const int HCBT_SETFOCUS = 9;

		internal const int HC_ACTION = 0;

		internal delegate LPARAM HOOKPROC(int code, LPARAM wParam, LPARAM lParam);

		internal struct MSLLHOOKSTRUCT
		{
			public Point pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public LPARAM dwExtraInfo;
		}

		internal const int GA_PARENT = 1;
		internal const int GA_ROOT = 2;
		internal const int GA_ROOTOWNER = 3;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetAncestor(Wnd hwnd, uint GA_X);

		[DllImport("user32.dll")]
		internal static extern Wnd GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetForegroundWindow(Wnd hWnd);

		internal const int ASFW_ANY = -1;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool AllowSetForegroundWindow(int dwProcessId);

		internal const uint LSFW_LOCK = 1;
		internal const uint LSFW_UNLOCK = 2;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool LockSetForegroundWindow(uint LSFW_X);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd SetFocus(Wnd hWnd);

		[DllImport("user32.dll")]
		internal static extern Wnd GetFocus();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd SetActiveWindow(Wnd hWnd);

		[DllImport("user32.dll")]
		internal static extern Wnd GetActiveWindow();

		internal struct WINDOWPOS
		{
			public Wnd hwnd;
			public Wnd hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetWindowPos(Wnd hWnd, Wnd hWndInsertAfter, int X, int Y, int cx, int cy, uint SWP_X);

		internal struct FLASHWINFO
		{
			public uint cbSize;
			public Wnd hwnd;
			public uint dwFlags;
			public uint uCount;
			public uint dwTimeout;
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

		internal const int GW_OWNER = 4;
		internal const int GW_HWNDPREV = 3;
		internal const int GW_HWNDNEXT = 2;
		internal const int GW_HWNDLAST = 1;
		internal const int GW_HWNDFIRST = 0;
		internal const int GW_ENABLEDPOPUP = 6;
		internal const int GW_CHILD = 5;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetWindow(Wnd hWnd, uint GW_X);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetTopWindow(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetParent(Wnd hWnd);

		[DllImport("user32.dll")]
		internal static extern Wnd GetDesktopWindow();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetShellWindow();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetLastActivePopup(Wnd hWnd);

		[DllImport("user32.dll")]
		internal static extern bool IntersectRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

		[DllImport("user32.dll")]
		internal static extern bool UnionRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

		//Gets DPI physical cursor pos, ie always in pixels.
		//The classic GetCursorPos API gets logical pos. Also it has a bug: randomly gets physical pos, even for same point.
		//Make sure that the process is DPI-aware.
		[DllImport("user32.dll", EntryPoint = "GetPhysicalCursorPos", SetLastError = true)]
		internal static extern bool GetCursorPos(out Point lpPoint);

		[DllImport("user32.dll", EntryPoint = "LoadImageW", SetLastError = true)]
		internal static extern IntPtr LoadImage(IntPtr hInst, string name, uint type, int cx, int cy, uint LR_X);
		[DllImport("user32.dll", EntryPoint = "LoadImageW", SetLastError = true)]
		internal static extern IntPtr LoadImage(IntPtr hInst, LPARAM resId, uint type, int cx, int cy, uint LR_X);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr CopyImage(IntPtr h, uint type, int cx, int cy, uint flags);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowRect(Wnd hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetClientRect(Wnd hWnd, out RECT lpRect);

		internal const uint WPF_SETMINPOSITION = 0x1;
		internal const uint WPF_RESTORETOMAXIMIZED = 0x2;
		internal const uint WPF_ASYNCWINDOWPLACEMENT = 0x4;

		internal struct WINDOWPLACEMENT
		{
			public uint length;
			/// <summary> WPF_ </summary>
			public uint flags;
			public int showCmd;
			public Point ptMinPosition;
			public Point ptMaxPosition;
			public RECT rcNormalPosition;
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowPlacement(Wnd hWnd, ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetWindowPlacement(Wnd hWnd, ref WINDOWPLACEMENT lpwndpl);

		internal struct WINDOWINFO
		{
			public uint cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public uint dwStyle;
			public uint dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public ushort atomWindowType;
			public ushort wCreatorVersion;
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowInfo(Wnd hwnd, ref WINDOWINFO pwi);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsZoomed(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsIconic(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetWindowThreadProcessId(Wnd hWnd, out int lpdwProcessId);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsWindowUnicode(Wnd hWnd);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);


		[DllImport("user32.dll", EntryPoint = "GetPropW", SetLastError = true)]
		internal static extern LPARAM GetProp(Wnd hWnd, string lpString);

		[DllImport("user32.dll", EntryPoint = "GetPropW", SetLastError = true)]
		//internal static extern LPARAM GetProp(Wnd hWnd, [MarshalAs(UnmanagedType.SysInt)] ushort atom); //exception, must be U2
		internal static extern LPARAM GetProp(Wnd hWnd, LPARAM atom);

		[DllImport("user32.dll", EntryPoint = "SetPropW", SetLastError = true)]
		internal static extern bool SetProp(Wnd hWnd, string lpString, LPARAM hData);

		[DllImport("user32.dll", EntryPoint = "SetPropW", SetLastError = true)]
		internal static extern bool SetProp(Wnd hWnd, LPARAM atom, LPARAM hData);

		[DllImport("user32.dll", EntryPoint = "RemovePropW", SetLastError = true)]
		internal static extern LPARAM RemoveProp(Wnd hWnd, string lpString);

		[DllImport("user32.dll", EntryPoint = "RemovePropW", SetLastError = true)]
		internal static extern LPARAM RemoveProp(Wnd hWnd, LPARAM atom);

		internal delegate bool PROPENUMPROCEX(Wnd hwnd, IntPtr lpszString, LPARAM hData, LPARAM dwData);

		[DllImport("user32.dll", EntryPoint = "EnumPropsExW", SetLastError = true)]
		internal static extern int EnumPropsEx(Wnd hWnd, PROPENUMPROCEX lpEnumFunc, LPARAM lParam);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd GetDlgItem(Wnd hDlg, int nIDDlgItem);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetDlgCtrlID(Wnd hWnd);

		internal delegate bool WNDENUMPROC(Wnd w, void* p);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool EnumChildWindows(Wnd hWndParent, WNDENUMPROC lpEnumFunc, void* p);

		[DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW", SetLastError = true)]
		internal static extern uint RegisterWindowMessage(string lpString);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsChild(Wnd hWndParent, Wnd hWnd);

		#region GetSystemMetrics, SystemParametersInfo

		internal const int SM_YVIRTUALSCREEN = 77;
		internal const int SM_XVIRTUALSCREEN = 76;
		internal const int SM_TABLETPC = 86;
		internal const int SM_SWAPBUTTON = 23;
		internal const int SM_STARTER = 88;
		internal const int SM_SLOWMACHINE = 73;
		internal const int SM_SHUTTINGDOWN = 8192;
		internal const int SM_SHOWSOUNDS = 70;
		internal const int SM_SERVERR2 = 89;
		internal const int SM_SECURE = 44;
		internal const int SM_SAMEDISPLAYFORMAT = 81;
		internal const int SM_RESERVED4 = 27;
		internal const int SM_RESERVED3 = 26;
		internal const int SM_RESERVED2 = 25;
		internal const int SM_RESERVED1 = 24;
		internal const int SM_REMOTESESSION = 4096;
		internal const int SM_REMOTECONTROL = 8193;
		internal const int SM_PENWINDOWS = 41;
		internal const int SM_NETWORK = 63;
		internal const int SM_MOUSEWHEELPRESENT = 75;
		internal const int SM_MOUSEPRESENT = 19;
		internal const int SM_MIDEASTENABLED = 74;
		internal const int SM_MENUDROPALIGNMENT = 40;
		internal const int SM_MEDIACENTER = 87;
		internal const int SM_IMMENABLED = 82;
		internal const int SM_DEBUG = 22;
		internal const int SM_DBCSENABLED = 42;
		internal const int SM_CYVTHUMB = 9;
		internal const int SM_CYVSCROLL = 20;
		internal const int SM_CYVIRTUALSCREEN = 79;
		internal const int SM_CYSMSIZE = 53;
		internal const int SM_CYSMICON = 50;
		internal const int SM_CYSMCAPTION = 51;
		internal const int SM_CYSIZEFRAME = SM_CYFRAME;
		internal const int SM_CYSIZE = 31;
		internal const int SM_CYSCREEN = 1;
		internal const int SM_CYMINTRACK = 35;
		internal const int SM_CYMINSPACING = 48;
		internal const int SM_CYMINIMIZED = 58;
		internal const int SM_CYMIN = 29;
		internal const int SM_CYMENUSIZE = 55;
		internal const int SM_CYMENUCHECK = 72;
		internal const int SM_CYMENU = 15;
		internal const int SM_CYMAXTRACK = 60;
		internal const int SM_CYMAXIMIZED = 62;
		internal const int SM_CYKANJIWINDOW = 18;
		internal const int SM_CYICONSPACING = 39;
		internal const int SM_CYICON = 12;
		internal const int SM_CYHSCROLL = 3;
		internal const int SM_CYFULLSCREEN = 17;
		internal const int SM_CYFRAME = 33;
		internal const int SM_CYFOCUSBORDER = 84;
		internal const int SM_CYFIXEDFRAME = SM_CYDLGFRAME;
		internal const int SM_CYEDGE = 46;
		internal const int SM_CYDRAG = 69;
		internal const int SM_CYDOUBLECLK = 37;
		internal const int SM_CYDLGFRAME = 8;
		internal const int SM_CYCURSOR = 14;
		internal const int SM_CYCAPTION = 4;
		internal const int SM_CYBORDER = 6;
		internal const int SM_CXVSCROLL = 2;
		internal const int SM_CXVIRTUALSCREEN = 78;
		internal const int SM_CXSMSIZE = 52;
		internal const int SM_CXSMICON = 49;
		internal const int SM_CXSIZEFRAME = SM_CXFRAME;
		internal const int SM_CXSIZE = 30;
		internal const int SM_CXSCREEN = 0;
		internal const int SM_CXMINTRACK = 34;
		internal const int SM_CXMINSPACING = 47;
		internal const int SM_CXMINIMIZED = 57;
		internal const int SM_CXMIN = 28;
		internal const int SM_CXMENUSIZE = 54;
		internal const int SM_CXMENUCHECK = 71;
		internal const int SM_CXMAXTRACK = 59;
		internal const int SM_CXMAXIMIZED = 61;
		internal const int SM_CXICONSPACING = 38;
		internal const int SM_CXICON = 11;
		internal const int SM_CXHTHUMB = 10;
		internal const int SM_CXHSCROLL = 21;
		internal const int SM_CXFULLSCREEN = 16;
		internal const int SM_CXFRAME = 32;
		internal const int SM_CXFOCUSBORDER = 83;
		internal const int SM_CXFIXEDFRAME = SM_CXDLGFRAME;
		internal const int SM_CXEDGE = 45;
		internal const int SM_CXDRAG = 68;
		internal const int SM_CXDOUBLECLK = 36;
		internal const int SM_CXDLGFRAME = 7;
		internal const int SM_CXCURSOR = 13;
		internal const int SM_CXBORDER = 5;
		internal const int SM_CMOUSEBUTTONS = 43;
		internal const int SM_CMONITORS = 80;
		internal const int SM_CMETRICS = 90;
		internal const int SM_CLEANBOOT = 67;
		internal const int SM_CARETBLINKINGENABLED = 8194;
		internal const int SM_ARRANGE = 56;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetSystemMetrics(int nIndex);

		internal const int SPI_SETWORKAREA = 47;
		internal const int SPI_SETWHEELSCROLLLINES = 105;
		internal const int SPI_SETUIEFFECTS = 4159;
		internal const int SPI_SETTOOLTIPFADE = 4121;
		internal const int SPI_SETTOOLTIPANIMATION = 4119;
		internal const int SPI_SETTOGGLEKEYS = 53;
		internal const int SPI_SETSTICKYKEYS = 59;
		internal const int SPI_SETSOUNDSENTRY = 65;
		internal const int SPI_SETSNAPTODEFBUTTON = 96;
		internal const int SPI_SETSHOWSOUNDS = 57;
		internal const int SPI_SETSHOWIMEUI = 111;
		internal const int SPI_SETSERIALKEYS = 63;
		internal const int SPI_SETSELECTIONFADE = 4117;
		internal const int SPI_SETSCREENSAVETIMEOUT = 15;
		internal const int SPI_SETSCREENSAVERRUNNING = 97;
		internal const int SPI_SETSCREENSAVEACTIVE = 17;
		internal const int SPI_SETSCREENREADER = 71;
		internal const int SPI_SETPOWEROFFTIMEOUT = 82;
		internal const int SPI_SETPOWEROFFACTIVE = 86;
		internal const int SPI_SETPENWINDOWS = 49;
		internal const int SPI_SETNONCLIENTMETRICS = 42;
		internal const int SPI_SETMOUSEVANISH = 4129;
		internal const int SPI_SETMOUSETRAILS = 93;
		internal const int SPI_SETMOUSESPEED = 113;
		internal const int SPI_SETMOUSESONAR = 4125;
		internal const int SPI_SETMOUSEKEYS = 55;
		internal const int SPI_SETMOUSEHOVERWIDTH = 99;
		internal const int SPI_SETMOUSEHOVERTIME = 103;
		internal const int SPI_SETMOUSEHOVERHEIGHT = 101;
		internal const int SPI_SETMOUSECLICKLOCKTIME = 8201;
		internal const int SPI_SETMOUSECLICKLOCK = 4127;
		internal const int SPI_SETMOUSEBUTTONSWAP = 33;
		internal const int SPI_SETMOUSE = 4;
		internal const int SPI_SETMINIMIZEDMETRICS = 44;
		internal const int SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES;
		internal const int SPI_SETMENUSHOWDELAY = 107;
		internal const int SPI_SETMENUFADE = 4115;
		internal const int SPI_SETMENUDROPALIGNMENT = 28;
		internal const int SPI_SETMENUANIMATION = 4099;
		internal const int SPI_SETLOWPOWERTIMEOUT = 81;
		internal const int SPI_SETLOWPOWERACTIVE = 85;
		internal const int SPI_SETLISTBOXSMOOTHSCROLLING = 4103;
		internal const int SPI_SETLANGTOGGLE = 91;
		internal const int SPI_SETKEYBOARDSPEED = 11;
		internal const int SPI_SETKEYBOARDPREF = 69;
		internal const int SPI_SETKEYBOARDDELAY = 23;
		internal const int SPI_SETKEYBOARDCUES = 4107;
		internal const int SPI_SETICONTITLEWRAP = 26;
		internal const int SPI_SETICONTITLELOGFONT = 34;
		internal const int SPI_SETICONS = 88;
		internal const int SPI_SETICONMETRICS = 46;
		internal const int SPI_SETHOTTRACKING = 4111;
		internal const int SPI_SETHIGHCONTRAST = 67;
		internal const int SPI_SETHANDHELD = 78;
		internal const int SPI_SETGRIDGRANULARITY = 19;
		internal const int SPI_SETGRADIENTCAPTIONS = 4105;
		internal const int SPI_SETFOREGROUNDLOCKTIMEOUT = 8193;
		internal const int SPI_SETFOREGROUNDFLASHCOUNT = 8197;
		internal const int SPI_SETFONTSMOOTHINGTYPE = 8203;
		internal const int SPI_SETFONTSMOOTHINGORIENTATION = 8211;
		internal const int SPI_SETFONTSMOOTHINGCONTRAST = 8205;
		internal const int SPI_SETFONTSMOOTHING = 75;
		internal const int SPI_SETFOCUSBORDERWIDTH = 8207;
		internal const int SPI_SETFOCUSBORDERHEIGHT = 8209;
		internal const int SPI_SETFLATMENU = 4131;
		internal const int SPI_SETFILTERKEYS = 51;
		internal const int SPI_SETFASTTASKSWITCH = 36;
		internal const int SPI_SETDROPSHADOW = 4133;
		internal const int SPI_SETDRAGWIDTH = 76;
		internal const int SPI_SETDRAGHEIGHT = 77;
		internal const int SPI_SETDRAGFULLWINDOWS = 37;
		internal const int SPI_SETDOUBLECLKWIDTH = 29;
		internal const int SPI_SETDOUBLECLKHEIGHT = 30;
		internal const int SPI_SETDOUBLECLICKTIME = 32;
		internal const int SPI_SETDESKWALLPAPER = 20;
		internal const int SPI_SETDESKPATTERN = 21;
		internal const int SPI_SETDEFAULTINPUTLANG = 90;
		internal const int SPI_SETCURSORSHADOW = 4123;
		internal const int SPI_SETCURSORS = 87;
		internal const int SPI_SETCOMBOBOXANIMATION = 4101;
		internal const int SPI_SETCARETWIDTH = 8199;
		internal const int SPI_SETBORDER = 6;
		internal const int SPI_SETBLOCKSENDINPUTRESETS = 4135;
		internal const int SPI_SETBEEP = 2;
		internal const int SPI_SETANIMATION = 73;
		internal const int SPI_SETACTIVEWNDTRKZORDER = 4109;
		internal const int SPI_SETACTIVEWNDTRKTIMEOUT = 8195;
		internal const int SPI_SETACTIVEWINDOWTRACKING = 4097;
		internal const int SPI_SETACCESSTIMEOUT = 61;
		internal const int SPI_LANGDRIVER = 12;
		internal const int SPI_ICONVERTICALSPACING = 24;
		internal const int SPI_ICONHORIZONTALSPACING = 13;
		internal const int SPI_GETWORKAREA = 48;
		internal const int SPI_GETWINDOWSEXTENSION = 92;
		internal const int SPI_GETWHEELSCROLLLINES = 104;
		internal const int SPI_GETUIEFFECTS = 4158;
		internal const int SPI_GETTOOLTIPFADE = 4120;
		internal const int SPI_GETTOOLTIPANIMATION = 4118;
		internal const int SPI_GETTOGGLEKEYS = 52;
		internal const int SPI_GETSTICKYKEYS = 58;
		internal const int SPI_GETSOUNDSENTRY = 64;
		internal const int SPI_GETSNAPTODEFBUTTON = 95;
		internal const int SPI_GETSHOWSOUNDS = 56;
		internal const int SPI_GETSHOWIMEUI = 110;
		internal const int SPI_GETSERIALKEYS = 62;
		internal const int SPI_GETSELECTIONFADE = 4116;
		internal const int SPI_GETSCREENSAVETIMEOUT = 14;
		internal const int SPI_GETSCREENSAVERRUNNING = 114;
		internal const int SPI_GETSCREENSAVEACTIVE = 16;
		internal const int SPI_GETSCREENREADER = 70;
		internal const int SPI_GETPOWEROFFTIMEOUT = 80;
		internal const int SPI_GETPOWEROFFACTIVE = 84;
		internal const int SPI_GETNONCLIENTMETRICS = 41;
		internal const int SPI_GETMOUSEVANISH = 4128;
		internal const int SPI_GETMOUSETRAILS = 94;
		internal const int SPI_GETMOUSESPEED = 112;
		internal const int SPI_GETMOUSESONAR = 4124;
		internal const int SPI_GETMOUSEKEYS = 54;
		internal const int SPI_GETMOUSEHOVERWIDTH = 98;
		internal const int SPI_GETMOUSEHOVERTIME = 102;
		internal const int SPI_GETMOUSEHOVERHEIGHT = 100;
		internal const int SPI_GETMOUSECLICKLOCKTIME = 8200;
		internal const int SPI_GETMOUSECLICKLOCK = 4126;
		internal const int SPI_GETMOUSE = 3;
		internal const int SPI_GETMINIMIZEDMETRICS = 43;
		internal const int SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES;
		internal const int SPI_GETMENUSHOWDELAY = 106;
		internal const int SPI_GETMENUFADE = 4114;
		internal const int SPI_GETMENUDROPALIGNMENT = 27;
		internal const int SPI_GETMENUANIMATION = 4098;
		internal const int SPI_GETLOWPOWERTIMEOUT = 79;
		internal const int SPI_GETLOWPOWERACTIVE = 83;
		internal const int SPI_GETLISTBOXSMOOTHSCROLLING = 4102;
		internal const int SPI_GETKEYBOARDSPEED = 10;
		internal const int SPI_GETKEYBOARDPREF = 68;
		internal const int SPI_GETKEYBOARDDELAY = 22;
		internal const int SPI_GETKEYBOARDCUES = 4106;
		internal const int SPI_GETICONTITLEWRAP = 25;
		internal const int SPI_GETICONTITLELOGFONT = 31;
		internal const int SPI_GETICONMETRICS = 45;
		internal const int SPI_GETHOTTRACKING = 4110;
		internal const int SPI_GETHIGHCONTRAST = 66;
		internal const int SPI_GETGRIDGRANULARITY = 18;
		internal const int SPI_GETGRADIENTCAPTIONS = 4104;
		internal const int SPI_GETFOREGROUNDLOCKTIMEOUT = 8192;
		internal const int SPI_GETFOREGROUNDFLASHCOUNT = 8196;
		internal const int SPI_GETFONTSMOOTHINGTYPE = 8202;
		internal const int SPI_GETFONTSMOOTHINGORIENTATION = 8210;
		internal const int SPI_GETFONTSMOOTHINGCONTRAST = 8204;
		internal const int SPI_GETFONTSMOOTHING = 74;
		internal const int SPI_GETFOCUSBORDERWIDTH = 8206;
		internal const int SPI_GETFOCUSBORDERHEIGHT = 8208;
		internal const int SPI_GETFLATMENU = 4130;
		internal const int SPI_GETFILTERKEYS = 50;
		internal const int SPI_GETFASTTASKSWITCH = 35;
		internal const int SPI_GETDROPSHADOW = 4132;
		internal const int SPI_GETDRAGFULLWINDOWS = 38;
		internal const int SPI_GETDESKWALLPAPER = 115;
		internal const int SPI_GETDEFAULTINPUTLANG = 89;
		internal const int SPI_GETCURSORSHADOW = 4122;
		internal const int SPI_GETCOMBOBOXANIMATION = 4100;
		internal const int SPI_GETCARETWIDTH = 8198;
		internal const int SPI_GETBORDER = 5;
		internal const int SPI_GETBLOCKSENDINPUTRESETS = 4134;
		internal const int SPI_GETBEEP = 1;
		internal const int SPI_GETANIMATION = 72;
		internal const int SPI_GETACTIVEWNDTRKZORDER = 4108;
		internal const int SPI_GETACTIVEWNDTRKTIMEOUT = 8194;
		internal const int SPI_GETACTIVEWINDOWTRACKING = 4096;
		internal const int SPI_GETACCESSTIMEOUT = 60;

		internal const uint SPIF_UPDATEINIFILE = 0x1;
		internal const uint SPIF_SENDCHANGE = 0x2;

		[DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
		internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, LPARAM pvParam, uint fWinIni);

		//[DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
		//internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, void* pvParam, uint fWinIni);

		#endregion

		[DllImport("user32.dll")]
		internal static extern Wnd WindowFromPoint(Point Point);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd RealChildWindowFromPoint(Wnd hwndParent, Point ptParentClientCoords);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ScreenToClient(Wnd hWnd, ref Point lpPoint);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ClientToScreen(Wnd hWnd, ref Point lpPoint);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int MapWindowPoints(Wnd hWndFrom, Wnd hWndTo, ref Point lpPoints, int cPoints = 1);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int MapWindowPoints(Wnd hWndFrom, Wnd hWndTo, ref RECT lpPoints, int cPoints = 2);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int MapWindowPoints(Wnd hWndFrom, Wnd hWndTo, void* lpPoints, int cPoints);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetGUIThreadInfo(int idThread, ref Native.GUITHREADINFO pgui);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

		[Flags]
		internal enum IKFlag :uint
		{
			Extended = 1, Up = 2, Unicode = 4, Scancode = 8
		};

		internal struct INPUTKEY
		{
			LPARAM _type;
			public ushort wVk;
			public ushort wScan;
			public IKFlag dwFlags;
			public uint time;
			public LPARAM dwExtraInfo;
			int _u1, _u2; //need INPUT size

			public INPUTKEY(int vk, int sc, IKFlag flags = 0)
			{
				_type = INPUT_KEYBOARD;
				wVk = (ushort)vk; wScan = (ushort)sc; dwFlags = flags;
				time = 0; dwExtraInfo = AuExtraInfo;
				_u2 = _u1 = 0;
				Debug.Assert(sizeof(INPUTKEY) == sizeof(INPUTMOUSE));
			}

			public const uint AuExtraInfo = 0xA1427fa5;
			const int INPUT_KEYBOARD = 1;
		}

		[Flags]
		internal enum IMFlag :uint
		{
			Move = 1,
			LeftDown = 2, LeftUp = 4,
			RightDown = 8, RightUp = 16,
			MiddleDown = 32, MiddleUp = 64,
			XDown = 0x80, XUp = 0x100,
			Wheel = 0x0800, HWheel = 0x01000,
			NoCoalesce = 0x2000,
			VirtualdDesktop = 0x4000,
			Absolute = 0x8000,
			//not API
			X1 = 0x1000000,
			X2 = 0x2000000,
		};

		internal struct INPUTMOUSE
		{
			LPARAM _type;
			public int dx;
			public int dy;
			public int mouseData;
			public IMFlag dwFlags;
			public uint time;
			public LPARAM dwExtraInfo;

			public INPUTMOUSE(IMFlag flags, int x = 0, int y = 0, int data = 0)
			{
				_type = INPUT_MOUSE;
				dx = x; dy = y; dwFlags = flags; mouseData = data;
				time = 0; dwExtraInfo = AuExtraInfo;
			}

			public const uint AuExtraInfo = 0xA1427fa5;
			const int INPUT_MOUSE = 0;
		}

		//[DllImport("user32.dll", SetLastError = true)]
		//internal static extern int SendInput(int cInputs, ref INPUTKEY pInputs, int cbSize);
		//[DllImport("user32.dll", SetLastError = true)]
		//internal static extern int SendInput(int cInputs, INPUTKEY[] pInputs, int cbSize);
		//[DllImport("user32.dll", SetLastError = true)]
		//internal static extern int SendInput(int cInputs, ref INPUTMOUSE pInputs, int cbSize);
		//[DllImport("user32.dll", SetLastError = true)]
		//internal static extern int SendInput(int cInputs, INPUTMOUSE[] pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int SendInput(int cInputs, void* pInputs, int cbSize);
		//note: the API never indicates a failure if arguments are valid. Tested UAC (documented), BlockInput, ClipCursor.

		internal static bool SendInput_Key(ref INPUTKEY ik)
		{
			fixed (void* p = &ik) {
				return SendInput(1, p, sizeof(INPUTKEY)) != 0;
			}
		}

		internal static bool SendInput_Key(INPUTKEY[] ik)
		{
			if(ik == null || ik.Length == 0) return false;
			fixed (void* p = ik) {
				return SendInput(ik.Length, p, sizeof(INPUTKEY)) != 0;
			}
		}

		internal static bool SendInput_Mouse(ref INPUTMOUSE ik)
		{
			fixed (void* p = &ik) {
				return SendInput(1, p, sizeof(INPUTMOUSE)) != 0;
			}
		}

		//internal static bool SendInput_Mouse(INPUTMOUSE[] ik)
		//{
		//	if(ik == null || ik.Length == 0) return false;
		//	fixed (void* p = ik) {
		//		return SendInput(ik.Length, p, sizeof(INPUTMOUSE)) != 0;
		//	}
		//}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsHungAppWindow(Wnd hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetLayeredWindowAttributes(Wnd hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr CreateIcon(IntPtr hInstance, int nWidth, int nHeight, byte cPlanes, byte cBitsPixel, byte[] lpbANDbits, byte[] lpbXORbits);

		[DllImport("user32.dll", EntryPoint = "LoadCursorW", SetLastError = true)]
		internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

		internal delegate void TIMERPROC(Wnd param1, uint param2, LPARAM param3, uint param4);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern LPARAM SetTimer(Wnd hWnd, LPARAM nIDEvent, uint uElapse, TIMERPROC lpTimerFunc);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool KillTimer(Wnd hWnd, LPARAM uIDEvent);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd SetParent(Wnd hWndChild, Wnd hWndNewParent);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ChangeWindowMessageFilter(uint message, uint dwFlag);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern short GetKeyState(int nVirtKey);

		internal const uint MWMO_WAITALL = 0x1;
		internal const uint MWMO_ALERTABLE = 0x2;
		internal const uint MWMO_INPUTAVAILABLE = 0x4;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint MsgWaitForMultipleObjectsEx(uint nCount, IntPtr* pHandles, uint dwMilliseconds = INFINITE, uint dwWakeMask = QS_ALLINPUT, uint MWMO_Flags = 0);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool RegisterHotKey(Wnd hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool UnregisterHotKey(Wnd hWnd, int id);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool EndMenu();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool InvalidateRect(Wnd hWnd, ref RECT lpRect, bool bErase);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool InvalidateRect(Wnd hWnd, IntPtr lpRect, bool bErase);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ValidateRect(Wnd hWnd, ref RECT lpRect);
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ValidateRect(Wnd hWnd, LPARAM zero = default);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetUpdateRect(Wnd hWnd, out RECT lpRect, bool bErase);
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetUpdateRect(Wnd hWnd, LPARAM zero, bool bErase);

		internal const int ERROR = 0;
		internal const int NULLREGION = 1;
		internal const int SIMPLEREGION = 2;
		internal const int COMPLEXREGION = 3;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetUpdateRgn(Wnd hWnd, IntPtr hRgn, bool bErase);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool InvalidateRgn(Wnd hWnd, IntPtr hRgn, bool bErase);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool DragDetect(Wnd hwnd, Point pt);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr SetCursor(IntPtr hCursor);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Wnd SetCapture(Wnd hWnd);

		[DllImport("user32.dll")]
		internal static extern Wnd GetCapture();

		[DllImport("user32.dll")]
		internal static extern bool ReleaseCapture();

		//[DllImport("user32.dll", EntryPoint = "CharLowerBuffW")]
		//internal static unsafe extern int CharLowerBuff(char* lpsz, int cchLength);

		//[DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int wsprintfW(char* lpOut1024, string lpFmt, __arglist);
		//note: with __arglist always returns 0. Could instead use void*, but then much work to properly pack arguments.

		//tested speed (time %) of various formatting functions, with two int and one string arg, with converting to string:
		//	StringBuilder.Append + int.ToString(CultureInfo.InvariantCulture): 85% - FASTEST
		//	StringBuilder.Append: 100%
		//	StringBuilder.AppendFormat + int.ToString() (avoid int boxing): 140%
		//	StringBuilder.AppendFormat: 150%
		//	$"{var} string": 160% (probably uses StringBuilder)
		//	wsprintfW (user32.dll): 150%
		//	_snwprintf (msvcrt.dll): 500% - SLOWEST

		[DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		internal static extern int wsprintfA(byte* lpOut1024, string lpFmt, __arglist);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool BlockInput(bool fBlockIt);

		[DllImport("user32.dll")]
		internal static extern IntPtr MonitorFromPoint(Point pt, uint dwFlags);

		[DllImport("user32.dll")]
		internal static extern IntPtr MonitorFromRect(ref RECT lprc, uint dwFlags);

		[DllImport("user32.dll")]
		internal static extern IntPtr MonitorFromWindow(Wnd hwnd, uint dwFlags);

		internal struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public bool fErase;
			public RECT rcPaint;
			public bool fRestore;
			public bool fIncUpdate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
		}

		[DllImport("user32.dll")]
		internal static extern IntPtr BeginPaint(Wnd hWnd, out PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		internal static extern bool EndPaint(Wnd hWnd, ref PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		internal static extern bool UpdateWindow(Wnd hWnd);




		#endregion

		#region gdi32

		[DllImport("gdi32.dll")] //this and many other GDI functions don't use SetLastError
		internal static extern bool DeleteObject(IntPtr ho);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

		internal const int RGN_AND = 1;
		internal const int RGN_OR = 2;
		internal const int RGN_XOR = 3;
		internal const int RGN_DIFF = 4;
		internal const int RGN_COPY = 5;

		[DllImport("gdi32.dll")]
		internal static extern int CombineRgn(IntPtr hrgnDst, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int iMode);

		[DllImport("gdi32.dll")]
		internal static extern bool SetRectRgn(IntPtr hrgn, int left, int top, int right, int bottom);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr GetDC(Wnd hWnd);

		//[DllImport("user32.dll", SetLastError = true)]
		//internal static extern IntPtr GetWindowDC(Wnd hWnd);

		[DllImport("user32.dll")] //note: no SetLastError = true
		internal static extern int ReleaseDC(Wnd hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

		[DllImport("gdi32.dll", EntryPoint = "GetObjectW")]
		internal static extern int GetObject(IntPtr h, int c, void* pv);

		[DllImport("gdi32.dll")] //tested: does not set last error
		internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

		[DllImport("gdi32.dll")]
		internal static extern int GetDeviceCaps(IntPtr hdc, int index);

		[DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32W")]
		internal static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int c, out Size psizl);

		[DllImport("gdi32.dll", EntryPoint = "CreateFontW")]
		internal static extern IntPtr CreateFont(int cHeight, int cWidth = 0, int cEscapement = 0, int cOrientation = 0, int cWeight = 0, int bItalic = 0, int bUnderline = 0, int bStrikeOut = 0, int iCharSet = 0, int iOutPrecision = 0, int iClipPrecision = 0, int iQuality = 0, int iPitchAndFamily = 0, string pszFaceName = null);

		//[DllImport("user32.dll", EntryPoint = "CharUpperBuffW")]
		//internal static unsafe extern int CharUpperBuff(char* lpsz, int cchLength);

		[DllImport("user32.dll")]
		internal static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

		[DllImport("gdi32.dll")] //tested: in some cases does not set last error even if returns false
		internal static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

		internal struct BITMAPINFOHEADER
		{
			public int biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
		}

		[DllImport("gdi32.dll")]
		internal static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int start, int cLines, void* lpvBits, BITMAPINFOHEADER* lpbmi, uint usage);

		internal const int WHITE_BRUSH = 0;
		internal const int LTGRAY_BRUSH = 1;
		internal const int GRAY_BRUSH = 2;
		internal const int DKGRAY_BRUSH = 3;
		internal const int BLACK_BRUSH = 4;
		internal const int NULL_BRUSH = 5;
		internal const int HOLLOW_BRUSH = 5;
		internal const int WHITE_PEN = 6;
		internal const int BLACK_PEN = 7;
		internal const int NULL_PEN = 8;
		internal const int OEM_FIXED_FONT = 10;
		internal const int ANSI_FIXED_FONT = 11;
		internal const int ANSI_VAR_FONT = 12;
		internal const int SYSTEM_FONT = 13;
		internal const int DEVICE_DEFAULT_FONT = 14;
		internal const int DEFAULT_PALETTE = 15;
		internal const int SYSTEM_FIXED_FONT = 16;
		internal const int DEFAULT_GUI_FONT = 17;
		internal const int DC_BRUSH = 18;
		internal const int DC_PEN = 19;

		[DllImport("gdi32.dll")]
		internal static extern IntPtr GetStockObject(int i);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateSolidBrush(uint color);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateRectRgnIndirect(ref RECT lprect);

		[DllImport("gdi32.dll")]
		internal static extern bool FrameRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr, int w, int h);




		#endregion

		#region kernel32

		[DllImport("kernel32.dll", SetLastError = true)] //note: without 'SetLastError = true' Marshal.GetLastWin32Error is unaware that we set the code to 0 etc and returns old captured error code
		internal static extern void SetLastError(int errCode);

		internal const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		internal const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
		internal const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;

		[DllImport("kernel32.dll", EntryPoint = "FormatMessageW")]
		internal static extern int FormatMessage(uint dwFlags, IntPtr lpSource, int code, uint dwLanguageId, char** lpBuffer, int nSize, IntPtr Arguments);

		[DllImport("kernel32.dll", EntryPoint = "SetDllDirectoryW", SetLastError = true)]
		internal static extern bool SetDllDirectory(string lpPathName);

		[DllImport("kernel32.dll")]
		internal static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

		[DllImport("kernel32.dll")]
		internal static extern long GetTickCount64();

		[DllImport("kernel32.dll")]
		internal static extern bool QueryUnbiasedInterruptTime(out long UnbiasedTime);

		[DllImport("kernel32.dll", EntryPoint = "CreateEventW", SetLastError = true)]
		internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetEvent(IntPtr hEvent);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		//[DllImport("kernel32.dll")]
		//internal static extern uint SignalObjectAndWait(IntPtr hObjectToSignal, IntPtr hObjectToWaitOn, uint dwMilliseconds, bool bAlertable);
		//note: don't know why, this often is much slower than setevent/waitforsingleobject.

		[DllImport("kernel32.dll")] //note: no SetLastError = true
		internal static extern bool CloseHandle(IntPtr hObject);

		//currently not used
		//[DllImport("kernel32.dll")] //note: no SetLastError = true
		//internal static extern bool CloseHandle(HandleRef hObject);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetCurrentThread();

		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentThreadId();

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentProcessId();

		[DllImport("kernel32.dll", EntryPoint = "QueryFullProcessImageNameW", SetLastError = true)]
		internal static extern bool QueryFullProcessImageName(IntPtr hProcess, bool nativeFormat, [Out] char[] lpExeName, ref int lpdwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr CreateFileMapping(IntPtr hFile, SECURITY_ATTRIBUTES lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		//[DllImport("kernel32.dll", EntryPoint = "OpenFileMappingW", SetLastError = true)]
		//internal static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, LPARAM dwNumberOfBytesToMap);

		//[DllImport("kernel32.dll", SetLastError = true)]
		//internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string name);
		//see also Util.Misc.GetModuleHandleOf(Type|Assembly).

		[DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string lpLibFileName);

		[DllImport("kernel32.dll")]
		internal static extern bool FreeLibrary(IntPtr hLibModule);

		[DllImport("kernel32.dll", BestFitMapping = false, SetLastError = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

		internal const uint PROCESS_TERMINATE = 0x0001;
		internal const uint PROCESS_CREATE_THREAD = 0x0002;
		internal const uint PROCESS_SET_SESSIONID = 0x0004;
		internal const uint PROCESS_VM_OPERATION = 0x0008;
		internal const uint PROCESS_VM_READ = 0x0010;
		internal const uint PROCESS_VM_WRITE = 0x0020;
		internal const uint PROCESS_DUP_HANDLE = 0x0040;
		internal const uint PROCESS_CREATE_PROCESS = 0x0080;
		internal const uint PROCESS_SET_QUOTA = 0x0100;
		internal const uint PROCESS_SET_INFORMATION = 0x0200;
		internal const uint PROCESS_QUERY_INFORMATION = 0x0400;
		internal const uint PROCESS_SUSPEND_RESUME = 0x0800;
		internal const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
		internal const uint PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;
		internal const uint DELETE = 0x00010000;
		internal const uint READ_CONTROL = 0x00020000;
		internal const uint WRITE_DAC = 0x00040000;
		internal const uint WRITE_OWNER = 0x00080000;
		internal const uint SYNCHRONIZE = 0x00100000;
		internal const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		internal const uint STANDARD_RIGHTS_READ = READ_CONTROL;
		internal const uint STANDARD_RIGHTS_WRITE = READ_CONTROL;
		internal const uint STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
		internal const uint STANDARD_RIGHTS_ALL = 0x001F0000;
		internal const uint TIMER_MODIFY_STATE = 0x2;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", EntryPoint = "GetFullPathNameW", SetLastError = true)]
		internal static extern int GetFullPathName(string lpFileName, int nBufferLength, [Out] char[] lpBuffer, char** lpFilePart);

		[DllImport("kernel32.dll", EntryPoint = "GetLongPathNameW", SetLastError = true)]
		internal static extern int GetLongPathName(string lpszShortPath, [Out] char[] lpszLongPath, int cchBuffer);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool ProcessIdToSessionId(int dwProcessId, out int pSessionId);

		internal const uint PAGE_NOACCESS = 0x1;
		internal const uint PAGE_READONLY = 0x2;
		internal const uint PAGE_READWRITE = 0x4;
		internal const uint PAGE_WRITECOPY = 0x8;
		internal const uint PAGE_EXECUTE = 0x10;
		internal const uint PAGE_EXECUTE_READ = 0x20;
		internal const uint PAGE_EXECUTE_READWRITE = 0x40;
		internal const uint PAGE_EXECUTE_WRITECOPY = 0x80;
		internal const uint PAGE_GUARD = 0x100;
		internal const uint PAGE_NOCACHE = 0x200;
		internal const uint PAGE_WRITECOMBINE = 0x400;

		internal const uint MEM_COMMIT = 0x1000;
		internal const uint MEM_RESERVE = 0x2000;
		internal const uint MEM_DECOMMIT = 0x4000;
		internal const uint MEM_RELEASE = 0x8000;
		internal const uint MEM_RESET = 0x80000;
		internal const uint MEM_TOP_DOWN = 0x100000;
		internal const uint MEM_WRITE_WATCH = 0x200000;
		internal const uint MEM_PHYSICAL = 0x400000;
		internal const uint MEM_RESET_UNDO = 0x1000000;
		internal const uint MEM_LARGE_PAGES = 0x20000000;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr VirtualAlloc(IntPtr lpAddress, LPARAM dwSize, uint flAllocationType = MEM_COMMIT | MEM_RESERVE, uint flProtect = PAGE_EXECUTE_READWRITE);

		[DllImport("kernel32.dll")]
		internal static extern bool VirtualFree(IntPtr lpAddress, LPARAM dwSize = default, uint dwFreeType = MEM_RELEASE);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr VirtualAllocEx(HandleRef hProcess, IntPtr lpAddress, LPARAM dwSize, uint flAllocationType = MEM_COMMIT | MEM_RESERVE, uint flProtect = PAGE_EXECUTE_READWRITE);

		[DllImport("kernel32.dll")]
		internal static extern bool VirtualFreeEx(HandleRef hProcess, IntPtr lpAddress, LPARAM dwSize = default, uint dwFreeType = MEM_RELEASE);

		[DllImport("kernel32.dll", EntryPoint = "GetFileAttributesW", SetLastError = true)]
		internal static extern System.IO.FileAttributes GetFileAttributes(string lpFileName);

		[DllImport("kernel32.dll", EntryPoint = "SetFileAttributesW", SetLastError = true)]
		internal static extern bool SetFileAttributes(string lpFileName, System.IO.FileAttributes dwFileAttributes);

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal struct WIN32_FILE_ATTRIBUTE_DATA
		{
			public System.IO.FileAttributes dwFileAttributes;
			public long ftCreationTime;
			public long ftLastAccessTime;
			public long ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
		}

		[DllImport("kernel32.dll", EntryPoint = "GetFileAttributesExW", SetLastError = true)]
		internal static extern bool GetFileAttributesEx(string lpFileName, int zero, out WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

		[DllImport("kernel32.dll", EntryPoint = "SearchPathW", SetLastError = true)]
		internal static extern int SearchPath(string lpPath, string lpFileName, string lpExtension, int nBufferLength, [Out] char[] lpBuffer, char** lpFilePart);

		internal const uint BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE = 0x1;
		internal const uint BASE_SEARCH_PATH_DISABLE_SAFE_SEARCHMODE = 0x10000;
		internal const uint BASE_SEARCH_PATH_PERMANENT = 0x8000;

		[DllImport("kernel32.dll")]
		internal static extern bool SetSearchPathMode(uint Flags);

		internal const uint SEM_FAILCRITICALERRORS = 0x1;

		[DllImport("kernel32.dll")]
		internal static extern uint SetErrorMode(uint uMode);

		[DllImport("kernel32.dll")]
		internal static extern uint GetErrorMode();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetThreadPriority(IntPtr hThread, int nPriority);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr LocalAlloc(uint uFlags, LPARAM uBytes);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr LocalFree(void* hMem);

		[DllImport("kernel32.dll", EntryPoint = "lstrcpynW")]
		internal static extern char* lstrcpyn(char* sTo, char* sFrom, int sToBufferLength);

		[DllImport("kernel32.dll", EntryPoint = "lstrcpynW")]
		internal static extern char* lstrcpyn(char* sTo, string sFrom, int sToBufferLength);

		internal struct FILETIME
		{
			public uint dwLowDateTime;
			public uint dwHighDateTime;

			public static implicit operator long(FILETIME ft) { return (long)((ulong)ft.dwHighDateTime << 32 | ft.dwLowDateTime); } //in Release faster than *(long*)&ft
		}

		[DllImport("kernel32.dll")]
		internal static extern void GetSystemTimeAsFileTime(out long lpSystemTimeAsFileTime);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);

		[DllImport("kernel32.dll")]
		internal static extern bool Wow64RevertWow64FsRedirection(IntPtr OlValue);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcessHeap();
		[DllImport("kernel32.dll")]
		internal static extern void* HeapAlloc(IntPtr hHeap, uint dwFlags, LPARAM dwBytes);
		[DllImport("kernel32.dll")]
		internal static extern void* HeapReAlloc(IntPtr hHeap, uint dwFlags, void* lpMem, LPARAM dwBytes);
		[DllImport("kernel32.dll")]
		internal static extern bool HeapFree(IntPtr hHeap, uint dwFlags, void* lpMem);

		internal const int CP_UTF8 = 65001;

		[DllImport("kernel32.dll")]
		internal static extern int MultiByteToWideChar(uint CodePage, uint dwFlags, byte* lpMultiByteStr, int cbMultiByte, char* lpWideCharStr, int cchWideChar);

		[DllImport("kernel32.dll")]
		internal static extern int WideCharToMultiByte(uint CodePage, uint dwFlags, string lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, IntPtr lpDefaultChar, int* lpUsedDefaultChar);

		[DllImport("kernel32.dll")]
		internal static extern int WideCharToMultiByte(uint CodePage, uint dwFlags, char* lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, IntPtr lpDefaultChar, int* lpUsedDefaultChar);

		internal struct STARTUPINFO
		{
			public uint cb;
			public IntPtr lpReserved;
			public IntPtr lpDesktop;
			public IntPtr lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public ushort wShowWindow;
			public ushort cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[DllImport("kernel32.dll", EntryPoint = "GetStartupInfoW")]
		internal static extern void GetStartupInfo(out STARTUPINFO lpStartupInfo);

		internal const uint FILE_READ_DATA = 0x1;
		internal const uint FILE_LIST_DIRECTORY = 0x1;
		internal const uint FILE_WRITE_DATA = 0x2;
		internal const uint FILE_ADD_FILE = 0x2;
		internal const uint FILE_APPEND_DATA = 0x4;
		internal const uint FILE_ADD_SUBDIRECTORY = 0x4;
		internal const uint FILE_CREATE_PIPE_INSTANCE = 0x4;
		internal const uint FILE_READ_EA = 0x8;
		internal const uint FILE_WRITE_EA = 0x10;
		internal const uint FILE_EXECUTE = 0x20;
		internal const uint FILE_TRAVERSE = 0x20;
		internal const uint FILE_DELETE_CHILD = 0x40;
		internal const uint FILE_READ_ATTRIBUTES = 0x80;
		internal const uint FILE_WRITE_ATTRIBUTES = 0x100;
		internal const uint FILE_ALL_ACCESS = 0x1F01FF;
		internal const uint FILE_GENERIC_READ = 0x120089;
		internal const uint FILE_GENERIC_WRITE = 0x120116;
		internal const uint FILE_GENERIC_EXECUTE = 0x1200A0;

		internal const int CREATE_NEW = 1;
		internal const int CREATE_ALWAYS = 2;
		internal const int OPEN_EXISTING = 3;
		internal const int OPEN_ALWAYS = 4;
		internal const int TRUNCATE_EXISTING = 5;

		internal const uint FILE_SHARE_READ = 0x1;
		internal const uint FILE_SHARE_WRITE = 0x2;
		internal const uint FILE_SHARE_DELETE = 0x4;
		internal const uint FILE_SHARE_ALL = 0x7;

		internal const uint GENERIC_READ = 0x80000000;
		internal const uint GENERIC_WRITE = 0x40000000;

		//internal const uint FILE_ATTRIBUTE_READONLY = 0x1;
		//internal const uint FILE_ATTRIBUTE_HIDDEN = 0x2;
		//internal const uint FILE_ATTRIBUTE_SYSTEM = 0x4;
		//internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
		//internal const uint FILE_ATTRIBUTE_ARCHIVE = 0x20;
		//internal const uint FILE_ATTRIBUTE_DEVICE = 0x40;
		internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
		//internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x100;
		//internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x200;
		//internal const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x400;
		//internal const uint FILE_ATTRIBUTE_COMPRESSED = 0x800;
		//internal const uint FILE_ATTRIBUTE_OFFLINE = 0x1000;
		//internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000;
		//internal const uint FILE_ATTRIBUTE_ENCRYPTED = 0x4000;
		//internal const uint FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x8000;
		//internal const uint FILE_ATTRIBUTE_VIRTUAL = 0x10000;
		//internal const uint FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x20000;
		//internal const uint FILE_ATTRIBUTE_EA = 0x40000;

		internal const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
		internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
		internal const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
		internal const uint FILE_FLAG_RANDOM_ACCESS = 0x10000000;
		internal const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000;
		internal const uint FILE_FLAG_DELETE_ON_CLOSE = 0x4000000;
		internal const uint FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
		internal const uint FILE_FLAG_POSIX_SEMANTICS = 0x1000000;
		internal const uint FILE_FLAG_SESSION_AWARE = 0x800000;
		internal const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x200000;
		internal const uint FILE_FLAG_OPEN_NO_RECALL = 0x100000;
		internal const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 0x80000;
		internal const uint FILE_FLAG_OPEN_REQUIRING_OPLOCK = 0x40000;

		[DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, SECURITY_ATTRIBUTES lpSecurityAttributes, int creationDisposition, uint dwFlagsAndAttributes = FILE_ATTRIBUTE_NORMAL, IntPtr hTemplateFile = default);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool WriteFile(SafeFileHandle hFile, void* lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, OVERLAPPED* lpOverlapped = null);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool ReadFile(SafeFileHandle hFile, void* lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, OVERLAPPED* lpOverlapped = null);

		internal struct OVERLAPPED
		{
			public LPARAM Internal;
			public LPARAM InternalHigh;

			[StructLayout(LayoutKind.Explicit)]
			internal struct TYPE_2
			{
				internal struct TYPE_1
				{
					public uint Offset;
					public uint OffsetHigh;
				}
				[FieldOffset(0)] public TYPE_1 _1;
				[FieldOffset(0)] public IntPtr Pointer;
			}
			public TYPE_2 _3;
			public IntPtr hEvent;
		}

		internal struct BY_HANDLE_FILE_INFORMATION
		{
			public uint dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public uint dwVolumeSerialNumber;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint nNumberOfLinks;
			public uint nFileIndexHigh;
			public uint nFileIndexLow;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		internal const int FILE_BEGIN = 0;
		internal const int FILE_CURRENT = 1;
		internal const int FILE_END = 2;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetFilePointerEx(SafeFileHandle hFile, long liDistanceToMove, long* lpNewFilePointer, uint dwMoveMethod);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetEndOfFile(SafeFileHandle hFile);

		[DllImport("kernel32.dll", EntryPoint = "CreateMailslotW", SetLastError = true)]
		internal static extern SafeFileHandle CreateMailslot(string lpName, uint nMaxMessageSize, uint lReadTimeout, SECURITY_ATTRIBUTES lpSecurityAttributes);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetMailslotInfo(SafeFileHandle hMailslot, uint* lpMaxMessageSize, out int lpNextSize, out int lpMessageCount, uint* lpReadTimeout = null);

		internal struct SYSTEMTIME
		{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMilliseconds;
		}

		[DllImport("kernel32.dll")]
		internal static extern void GetLocalTime(out SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref int AppModelIDLength, [Out] char[] sbAppUserModelID);

		[DllImport("kernel32.dll", EntryPoint = "GetEnvironmentVariableW", SetLastError = true)]
		internal static extern int GetEnvironmentVariable(string lpName, [Out] char[] lpBuffer, int nSize);

		[DllImport("kernel32.dll", EntryPoint = "ExpandEnvironmentStringsW")]
		internal static extern int ExpandEnvironmentStrings(string lpSrc, [Out] char[] lpDst, int nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int GetProcessId(IntPtr Process);

		internal struct WIN32_FIND_DATA
		{
			public System.IO.FileAttributes dwFileAttributes;
			public Api.FILETIME ftCreationTime;
			public Api.FILETIME ftLastAccessTime;
			public Api.FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			public fixed char cFileName[260];
			public fixed char cAlternateFileName[14];

			internal unsafe string Name
			{
				get
				{
					fixed (char* p = cFileName) {
						if(p[0] == '.') {
							if(p[1] == '\0') return null;
							if(p[1] == '.' && p[2] == '\0') return null;
						}
						return new string(p);
					}
				}
			}
		}

		[DllImport("kernel32.dll", EntryPoint = "FindFirstFileW", SetLastError = true)]
		internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll", EntryPoint = "FindNextFileW", SetLastError = true)]
		internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll")]
		internal static extern bool FindClose(IntPtr hFindFile);

#if TEST_FINDFIRSTFILEEX
			internal enum FINDEX_INFO_LEVELS
			{
				FindExInfoStandard,
				FindExInfoBasic,
				FindExInfoMaxInfoLevel
			}

			internal const uint FIND_FIRST_EX_LARGE_FETCH = 0x2;

			[DllImport("kernel32.dll", EntryPoint = "FindFirstFileExW")]
			internal static extern IntPtr FindFirstFileEx(string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, out WIN32_FIND_DATA lpFindFileData, int fSearchOp, IntPtr lpSearchFilter, uint dwAdditionalFlags);
#endif

		internal const uint MOVEFILE_REPLACE_EXISTING = 0x1;
		internal const uint MOVEFILE_COPY_ALLOWED = 0x2;
		internal const uint MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;
		internal const uint MOVEFILE_WRITE_THROUGH = 0x8;
		internal const uint MOVEFILE_CREATE_HARDLINK = 0x10;
		internal const uint MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20;

		[DllImport("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
		internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

		//[DllImport("kernel32.dll", EntryPoint = "CopyFileW", SetLastError = true)]
		//internal static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

		internal const uint COPY_FILE_FAIL_IF_EXISTS = 0x1;
		internal const uint COPY_FILE_RESTARTABLE = 0x2;
		internal const uint COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x4;
		internal const uint COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x8;
		internal const uint COPY_FILE_COPY_SYMLINK = 0x800;
		internal const uint COPY_FILE_NO_BUFFERING = 0x1000;

		[DllImport("kernel32.dll", EntryPoint = "CopyFileExW", SetLastError = true)]
		internal static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, IntPtr lpData, int* pbCancel, uint dwCopyFlags);

		internal delegate uint LPPROGRESS_ROUTINE(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, uint dwStreamNumber, uint dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

		[DllImport("kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true)]
		internal static extern bool DeleteFile(string lpFileName);

		[DllImport("kernel32.dll", EntryPoint = "RemoveDirectoryW", SetLastError = true)]
		internal static extern bool RemoveDirectory(string lpPathName);

		[DllImport("kernel32.dll", EntryPoint = "CreateDirectoryW", SetLastError = true)]
		internal static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes); //ref SECURITY_ATTRIBUTES

		[DllImport("kernel32.dll", EntryPoint = "CreateDirectoryExW", SetLastError = true)]
		internal static extern bool CreateDirectoryEx(string lpTemplateDirectory, string lpNewDirectory, IntPtr lpSecurityAttributes); //ref SECURITY_ATTRIBUTES

		[DllImport("kernel32.dll", EntryPoint = "GlobalAddAtomW")]
		internal static extern ushort GlobalAddAtom(string lpString);

		[DllImport("kernel32.dll")]
		internal static extern ushort GlobalDeleteAtom(ushort nAtom);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool ReadProcessMemory(HandleRef hProcess, IntPtr lpBaseAddress, void* lpBuffer, LPARAM nSize, LPARAM* lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool WriteProcessMemory(HandleRef hProcess, IntPtr lpBaseAddress, void* lpBuffer, LPARAM nSize, LPARAM* lpNumberOfBytesWritten);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static IntPtr CreateActCtx(ref ACTCTX actctx);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static bool DeactivateActCtx(int dwFlags, IntPtr lpCookie);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static bool GetCurrentActCtx(out IntPtr handle);

		internal const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;
		internal const int ACTCTX_FLAG_RESOURCE_NAME_VALID = 0x008;

		internal struct ACTCTX
		{
			public uint cbSize;
			public uint dwFlags;
			public string lpSource;
			public ushort wProcessorArchitecture;
			public ushort wLangId;
			public IntPtr lpAssemblyDirectory;
			public IntPtr lpResourceName;
			public IntPtr lpApplicationName;
			public IntPtr hModule;
		}




		#endregion

		#region advapi32

		internal const uint TOKEN_WRITE = STANDARD_RIGHTS_WRITE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT;
		internal const uint TOKEN_SOURCE_LENGTH = 8;
		internal const uint TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY;
		internal const uint TOKEN_QUERY_SOURCE = 16;
		internal const uint TOKEN_QUERY = 8;
		internal const uint TOKEN_IMPERSONATE = 4;
		internal const uint TOKEN_EXECUTE = STANDARD_RIGHTS_EXECUTE;
		internal const uint TOKEN_DUPLICATE = 2;
		internal const uint TOKEN_AUDIT_SUCCESS_INCLUDE = 1;
		internal const uint TOKEN_AUDIT_SUCCESS_EXCLUDE = 2;
		internal const uint TOKEN_AUDIT_FAILURE_INCLUDE = 4;
		internal const uint TOKEN_AUDIT_FAILURE_EXCLUDE = 8;
		internal const uint TOKEN_ASSIGN_PRIMARY = 1;
		internal const uint TOKEN_ALL_ACCESS_P = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT;
		internal const uint TOKEN_ALL_ACCESS = TOKEN_ALL_ACCESS_P | TOKEN_ADJUST_SESSIONID;
		internal const uint TOKEN_ADJUST_SESSIONID = 256;
		internal const uint TOKEN_ADJUST_PRIVILEGES = 32;
		internal const uint TOKEN_ADJUST_GROUPS = 64;
		internal const uint TOKEN_ADJUST_DEFAULT = 128;

		[DllImport("advapi32.dll", SetLastError = true)]
		internal static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

		internal enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUIAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			//Win8
			TokenIsAppContainer,
			TokenCapabilities,
			TokenAppContainerSid,
			TokenAppContainerNumber,
			TokenUserClaimAttributes,
			TokenDeviceClaimAttributes,
			TokenRestrictedUserClaimAttributes,
			TokenRestrictedDeviceClaimAttributes,
			TokenDeviceGroups,
			TokenRestrictedDeviceGroups,
			TokenSecurityAttributes,
			TokenIsRestricted,
			TokenProcessTrustLevel,
			TokenPrivateNameSpace,
			MaxTokenInfoClass  // MaxTokenInfoClass should always be the last enum
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		internal static extern bool GetTokenInformation(HandleRef TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, void* TokenInformation, uint TokenInformationLength, out uint ReturnLength);

		[DllImport("advapi32.dll")]
		internal static extern byte* GetSidSubAuthorityCount(IntPtr pSid);

		[DllImport("advapi32.dll")]
		internal static extern uint* GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

		[DllImport("advapi32.dll")]
		internal static extern int RegSetValueEx(IntPtr hKey, string lpValueName, int Reserved, Microsoft.Win32.RegistryValueKind dwType, void* lpData, int cbData);

		[DllImport("advapi32.dll")]
		internal static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, IntPtr Reserved, out Microsoft.Win32.RegistryValueKind dwType, void* lpData, ref int cbData);

		[StructLayout(LayoutKind.Sequential)]
		internal sealed class SECURITY_ATTRIBUTES :IDisposable
		{
			public int nLength;
			public void* lpSecurityDescriptor;
			public int bInheritHandle;

			/// <summary>
			/// Creates SECURITY_ATTRIBUTES that allows UAC low integrity level processes to open the kernel object.
			/// </summary>
			public SECURITY_ATTRIBUTES()
			{
				nLength = IntPtr.Size * 3;
				if(!ConvertStringSecurityDescriptorToSecurityDescriptor("D:NO_ACCESS_CONTROLS:(ML;;NW;;;LW)", 1, out lpSecurityDescriptor)) throw new AuException(0, "SECURITY_ATTRIBUTES");
			}

			public void Dispose()
			{
				if(lpSecurityDescriptor != null) {
					LocalFree(lpSecurityDescriptor);
					lpSecurityDescriptor = null;
				}
			}

			~SECURITY_ATTRIBUTES() => Dispose();

			public static SECURITY_ATTRIBUTES Common = new SECURITY_ATTRIBUTES();
		}

		[DllImport("advapi32.dll", EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW", SetLastError = true)]
		internal static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(string StringSecurityDescriptor, uint StringSDRevision, out void* SecurityDescriptor, uint* SecurityDescriptorSize = null);

		//[DllImport("advapi32.dll", EntryPoint = "ConvertSecurityDescriptorToStringSecurityDescriptorW")]
		//internal static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(void* SecurityDescriptor, uint RequestedStringSDRevision, uint SecurityInformation, out char* StringSecurityDescriptor, out uint StringSecurityDescriptorLen);




		#endregion

		#region shell32

		//[DllImport("shell32.dll")]
		//internal static extern bool IsUserAnAdmin();

		internal const uint SHGFI_ICON = 0x000000100;     // get icon;
		internal const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name;
		internal const uint SHGFI_TYPENAME = 0x000000400;     // get type name;
		internal const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes;
		internal const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location;
		internal const uint SHGFI_EXETYPE = 0x000002000;     // return exe type;
		internal const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index;
		internal const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon;
		internal const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state;
		internal const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes;
		internal const uint SHGFI_LARGEICON = 0x000000000;     // get large icon;
		internal const uint SHGFI_SMALLICON = 0x000000001;     // get small icon;
		internal const uint SHGFI_OPENICON = 0x000000002;     // get open icon;
		internal const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon;
		internal const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl;
		internal const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute;
		internal const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays;
		internal const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay;

		internal struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW")]
		internal static extern LPARAM SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW")]
		internal static extern LPARAM SHGetFileInfo(IntPtr pidl, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetDesktopFolder(out IShellFolder ppshf);

		[DllImport("shell32.dll")]
		internal static extern int SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr pidl, uint sfgaoIn, uint* psfgaoOut);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetNameFromIDList(IntPtr pidl, Native.SIGDN sigdnName, out string ppszName);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHCreateShellItem(IntPtr pidlParent, IShellFolder psfParent, IntPtr pidl, out IShellItem ppsi);
		//This classic API supports absolute PIDL and parent+relative PIDL.
		//There are 2 newer API - SHCreateItemFromIDList (absoulte) and SHCreateItemWithParent (parent+relative). They can get IShellItem2 too, which is currently not useful here. Same speed.

		//[DllImport("shell32.dll", PreserveSig = true)]
		//internal static extern int SHCreateItemFromIDList(IntPtr pidl, ref Guid riid, out IShellItem ppv); //or IShellItem2

		//[DllImport("shell32.dll", PreserveSig = true)]
		//internal static extern int SHBindToParent(IntPtr pidl, ref Guid riid, out IShellFolder ppv, out IntPtr ppidlLast);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetPropertyStoreForWindow(Wnd hwnd, ref Guid riid, out IPropertyStore ppv);

		internal static PROPERTYKEY PKEY_AppUserModel_ID = new PROPERTYKEY() { fmtid = new Guid(0x9F4C2855, 0x9F79, 0x4B39, 0xA8, 0xD0, 0xE1, 0xD4, 0x2D, 0xE1, 0xD5, 0xF3), pid = 5 };

		[DllImport("shell32.dll")]
		internal static extern IntPtr* CommandLineToArgvW(string lpCmdLine, out int pNumArgs);

		internal struct NOTIFYICONDATA
		{
			public uint cbSize;
			public Wnd hWnd;
			public uint uID;
			public uint uFlags;
			public uint uCallbackMessage;
			public IntPtr hIcon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szTip;
			public uint dwState;
			public uint dwStateMask;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szInfo;

			[StructLayout(LayoutKind.Explicit)]
			public struct TYPE_1
			{
				[FieldOffset(0)]
				public uint uTimeout;
				[FieldOffset(0)]
				public uint uVersion;
			}
			public TYPE_1 _11;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szInfoTitle;
			public uint dwInfoFlags;
			public Guid guidItem;
			public IntPtr hBalloonIcon;
		}

		internal const uint NIN_SELECT = 0x400;
		internal const uint NINF_KEY = 0x1;
		internal const uint NIN_KEYSELECT = 0x401;
		internal const uint NIN_BALLOONSHOW = 0x402;
		internal const uint NIN_BALLOONHIDE = 0x403;
		internal const uint NIN_BALLOONTIMEOUT = 0x404;
		internal const uint NIN_BALLOONUSERCLICK = 0x405;
		internal const uint NIN_POPUPOPEN = 0x406;
		internal const uint NIN_POPUPCLOSE = 0x407;
		internal const uint NIM_ADD = 0x0;
		internal const uint NIM_MODIFY = 0x1;
		internal const uint NIM_DELETE = 0x2;
		internal const uint NIM_SETFOCUS = 0x3;
		internal const uint NIM_SETVERSION = 0x4;
		internal const int NOTIFYICON_VERSION = 3;
		internal const int NOTIFYICON_VERSION_4 = 4;
		internal const uint NIF_MESSAGE = 0x1;
		internal const uint NIF_ICON = 0x2;
		internal const uint NIF_TIP = 0x4;
		internal const uint NIF_STATE = 0x8;
		internal const uint NIF_INFO = 0x10;
		internal const uint NIF_GUID = 0x20;
		internal const uint NIF_REALTIME = 0x40;
		internal const uint NIF_SHOWTIP = 0x80;
		internal const uint NIS_HIDDEN = 0x1;
		internal const uint NIS_SHAREDICON = 0x2;
		internal const uint NIIF_NONE = 0x0;
		internal const uint NIIF_INFO = 0x1;
		internal const uint NIIF_WARNING = 0x2;
		internal const uint NIIF_ERROR = 0x3;
		internal const uint NIIF_USER = 0x4;
		internal const uint NIIF_ICON_MASK = 0xF;
		internal const uint NIIF_NOSOUND = 0x10;
		internal const uint NIIF_LARGE_ICON = 0x20;
		internal const uint NIIF_RESPECT_QUIET_TIME = 0x80;

		[DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
		internal static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

		//internal struct SHSTOCKICONINFO
		//{
		//	public uint cbSize;
		//	public IntPtr hIcon;
		//	public int iSysImageIndex;
		//	public int iIcon;
		//	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		//	public string szPath;
		//}

		internal struct SHSTOCKICONINFO
		{
			public uint cbSize;
			public IntPtr hIcon;
			public int iSysImageIndex;
			public int iIcon;
			public fixed char szPath[260];
		}

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetStockIconInfo(Native.SHSTOCKICONID siid, uint uFlags, ref SHSTOCKICONINFO psii);

		[DllImport("shell32.dll", EntryPoint = "#6", PreserveSig = true)]
		internal static extern int SHDefExtractIcon(string pszIconFile, int iIndex, uint uFlags, IntPtr* phiconLarge, IntPtr* phiconSmall, int nIconSize);

		internal const int SHIL_LARGE = 0;
		internal const int SHIL_SMALL = 1;
		internal const int SHIL_EXTRALARGE = 2;
		internal const int SHIL_SYSSMALL = 3;
		internal const int SHIL_JUMBO = 4;

		//[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
		//internal static extern int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppvObj);
		[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
		internal static extern int SHGetImageList(int iImageList, ref Guid riid, out IntPtr ppvObj);

		internal const uint SHCNE_RENAMEITEM = 0x1;
		internal const uint SHCNE_CREATE = 0x2;
		internal const uint SHCNE_DELETE = 0x4;
		internal const uint SHCNE_MKDIR = 0x8;
		internal const uint SHCNE_RMDIR = 0x10;
		internal const uint SHCNE_MEDIAINSERTED = 0x20;
		internal const uint SHCNE_MEDIAREMOVED = 0x40;
		internal const uint SHCNE_DRIVEREMOVED = 0x80;
		internal const uint SHCNE_DRIVEADD = 0x100;
		internal const uint SHCNE_NETSHARE = 0x200;
		internal const uint SHCNE_NETUNSHARE = 0x400;
		internal const uint SHCNE_ATTRIBUTES = 0x800;
		internal const uint SHCNE_UPDATEDIR = 0x1000;
		internal const uint SHCNE_UPDATEITEM = 0x2000;
		internal const uint SHCNE_SERVERDISCONNECT = 0x4000;
		internal const uint SHCNE_UPDATEIMAGE = 0x8000;
		internal const uint SHCNE_DRIVEADDGUI = 0x10000;
		internal const uint SHCNE_RENAMEFOLDER = 0x20000;
		internal const uint SHCNE_FREESPACE = 0x40000;
		internal const uint SHCNE_EXTENDED_EVENT = 0x4000000;
		internal const uint SHCNE_ASSOCCHANGED = 0x8000000;
		internal const uint SHCNE_DISKEVENTS = 0x2381F;
		internal const uint SHCNE_GLOBALEVENTS = 0xC0581E0;
		internal const uint SHCNE_ALLEVENTS = 0x7FFFFFFF;
		internal const uint SHCNE_INTERRUPT = 0x80000000;

		internal const uint SHCNF_IDLIST = 0x0;
		internal const uint SHCNF_DWORD = 0x3;
		internal const uint SHCNF_PATH = 0x5;
		internal const uint SHCNF_PRINTER = 0x6;
		internal const uint SHCNF_FLUSH = 0x1000;
		internal const uint SHCNF_FLUSHNOWAIT = 0x3000;
		internal const uint SHCNF_NOTIFYRECURSIVE = 0x10000;

		[DllImport("shell32.dll")]
		internal static extern void SHChangeNotify(uint wEventId, uint uFlags, string dwItem1, string dwItem2);

		internal const uint SEE_MASK_CONNECTNETDRV = 0x80;
		internal const uint SEE_MASK_NOZONECHECKS = 0x800000;
		internal const uint SEE_MASK_UNICODE = 0x4000;
		internal const uint SEE_MASK_FLAG_NO_UI = 0x400;
		internal const uint SEE_MASK_INVOKEIDLIST = 0xC;
		internal const uint SEE_MASK_NOCLOSEPROCESS = 0x40;
		internal const uint SEE_MASK_NOASYNC = 0x100;
		internal const uint SEE_MASK_NO_CONSOLE = 0x8000;
		internal const uint SEE_MASK_HMONITOR = 0x200000;
		internal const uint SEE_MASK_WAITFORINPUTIDLE = 0x2000000;

		internal struct SHELLEXECUTEINFO
		{
			public uint cbSize;
			public uint fMask;
			public Wnd hwnd;
			public string lpVerb;
			public string lpFile;
			public string lpParameters;
			public string lpDirectory;
			public int nShow;
			public IntPtr hInstApp;
			public IntPtr lpIDList;
			public string lpClass;
			public IntPtr hkeyClass;
			public uint dwHotKey;
			public IntPtr hMonitor;
			public IntPtr hProcess;
		}

		[DllImport("shell32.dll", EntryPoint = "ShellExecuteExW", SetLastError = true)]
		internal static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO pExecInfo);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHOpenFolderAndSelectItems(HandleRef pidlFolder, uint cidl, IntPtr[] apidl, uint dwFlags);

		[DllImport("shell32.dll")]
		internal static extern int ILGetSize(IntPtr pidl);

		internal const uint FO_MOVE = 0x1;
		internal const uint FO_COPY = 0x2;
		internal const uint FO_DELETE = 0x3;
		internal const uint FO_RENAME = 0x4;

		internal const uint FOF_MULTIDESTFILES = 0x1;
		internal const uint FOF_CONFIRMMOUSE = 0x2;
		internal const uint FOF_SILENT = 0x4;
		internal const uint FOF_RENAMEONCOLLISION = 0x8;
		internal const uint FOF_NOCONFIRMATION = 0x10;
		internal const uint FOF_WANTMAPPINGHANDLE = 0x20;
		internal const uint FOF_ALLOWUNDO = 0x40;
		internal const uint FOF_FILESONLY = 0x80;
		internal const uint FOF_SIMPLEPROGRESS = 0x100;
		internal const uint FOF_NOCONFIRMMKDIR = 0x200;
		internal const uint FOF_NOERRORUI = 0x400;
		internal const uint FOF_NOCOPYSECURITYATTRIBS = 0x800;
		internal const uint FOF_NORECURSION = 0x1000;
		internal const uint FOF_NO_CONNECTED_ELEMENTS = 0x2000;
		internal const uint FOF_WANTNUKEWARNING = 0x4000;
		internal const uint FOF_NORECURSEREPARSE = 0x8000;
		internal const uint FOF_NO_UI = 0x614;

		internal struct SHFILEOPSTRUCT
		{
			public Wnd hwnd;
			public uint wFunc;
			public string pFrom;
			public string pTo;
			public ushort fFlags;

			//workaround for this problem: the 32-bit version of SHFILEOPSTRUCT uses Pack = 1, ie no 2-byte gap after fFlags.
			//	I don't want to use two versions. Then also would need two versions of code that use this struct.
			//	The last two members are not useful, but we need fAnyOperationsAborted. This workaround gets it through a property function.
			//	Update: we don't need fAnyOperationsAborted. We use FOF_SILENT therefore cannot be aborted. And it is unreliable.
			//		But the workaround is tested, on both platformas.

#if use_fAnyOperationsAborted
				//public bool fAnyOperationsAborted; //BOOL
				ushort _fAnyOperationsAborted_32, _fAnyOperationsAborted_common, _fAnyOperationsAborted_64;
				public bool fAnyOperationsAborted
				{
					get
					{
						if(_fAnyOperationsAborted_common != 0) return true;
						var v = Ver.Is64BitProcess ? _fAnyOperationsAborted_64 : _fAnyOperationsAborted_32;
						return v != 0;
					}
				}
#else
			private int fAnyOperationsAborted;
#endif
			private IntPtr hNameMappings;
			private string lpszProgressTitle;
			//these are private and not used, because would be at invalid offsets on 32-bit
		}

		//internal struct SHFILEOPSTRUCT
		//{
		//	public Wnd hwnd;
		//	public uint wFunc;
		//	public string pFrom;
		//	public string pTo;
		//	public ushort fFlags;
		//	public bool fAnyOperationsAborted;
		//	public IntPtr hNameMappings;
		//	public string lpszProgressTitle;
		//}

		//[StructLayout(LayoutKind.Sequential, Pack = 1)]
		//internal struct SHFILEOPSTRUCT__32
		//{
		//	public Wnd hwnd;
		//	public uint wFunc;
		//	public string pFrom;
		//	public string pTo;
		//	public ushort fFlags;
		//	public bool fAnyOperationsAborted;
		//	public IntPtr hNameMappings;
		//	public string lpszProgressTitle;
		//}

		[DllImport("shell32.dll", EntryPoint = "SHFileOperationW")]
		internal static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);




		#endregion

		#region shlwapi

		[DllImport("shlwapi.dll", EntryPoint = "#176", PreserveSig = true)]
		internal static extern int IUnknown_QueryService(IntPtr punk, ref Guid guidService, ref Guid riid, out IntPtr ppvOut);
		//internal static extern int IUnknown_QueryService(IntPtr punk, ref Guid guidService, ref Guid riid, void* ppvOut);
		//internal static extern int IUnknown_QueryService([MarshalAs(UnmanagedType.IUnknown)] object punk, ref Guid guidService, ref Guid riid, out IntPtr ppvOut);

		[DllImport("shlwapi.dll", EntryPoint = "PathIsDirectoryEmptyW")]
		internal static extern bool PathIsDirectoryEmpty(string pszPath);
		//speed: slightly faster than with EnumDirectory.

		[DllImport("shlwapi.dll")]
		internal static extern uint ColorAdjustLuma(uint clrRGB, int n, bool fScale);

		//internal enum ASSOCSTR
		//{
		//	ASSOCSTR_COMMAND = 1,
		//	ASSOCSTR_EXECUTABLE,
		//	ASSOCSTR_FRIENDLYDOCNAME,
		//	ASSOCSTR_FRIENDLYAPPNAME,
		//	ASSOCSTR_NOOPEN,
		//	ASSOCSTR_SHELLNEWVALUE,
		//	ASSOCSTR_DDECOMMAND,
		//	ASSOCSTR_DDEIFEXEC,
		//	ASSOCSTR_DDEAPPLICATION,
		//	ASSOCSTR_DDETOPIC,
		//	ASSOCSTR_INFOTIP,
		//	ASSOCSTR_QUICKTIP,
		//	ASSOCSTR_TILEINFO,
		//	ASSOCSTR_CONTENTTYPE,
		//	ASSOCSTR_DEFAULTICON,
		//	ASSOCSTR_SHELLEXTENSION,
		//	ASSOCSTR_DROPTARGET,
		//	ASSOCSTR_DELEGATEEXECUTE,
		//	ASSOCSTR_SUPPORTED_URI_PROTOCOLS,
		//	ASSOCSTR_PROGID,
		//	ASSOCSTR_APPID,
		//	ASSOCSTR_APPPUBLISHER,
		//	ASSOCSTR_APPICONREFERENCE,
		//	ASSOCSTR_MAX
		//}

		//[DllImport("shlwapi.dll", PreserveSig = true, EntryPoint = "AssocQueryStringW")]
		//internal static extern int AssocQueryString(uint flags, ASSOCSTR str, string pszAssoc, string pszExtra, char* pszOut, ref int pcchOut);




		#endregion

		#region comctl32

		[DllImport("comctl32.dll")]
		internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

		[DllImport("comctl32.dll")]
		internal static extern bool ImageList_GetIconSize(IntPtr himl, out int cx, out int cy);




		#endregion

		#region oleaut32

		[DllImport("oleaut32.dll", EntryPoint = "#6")]
		internal static extern void SysFreeString(char* bstrString);

		[DllImport("oleaut32.dll", EntryPoint = "#7")]
		internal static extern int SysStringLen(char* pbstr);

		[DllImport("oleaut32.dll", EntryPoint = "#4")]
		internal static extern BSTR SysAllocStringLen(string strIn, int len);

		[DllImport("oleaut32.dll", EntryPoint = "#2")]
		internal static extern BSTR SysAllocString(char* psz);

		[DllImport("oleaut32.dll", EntryPoint = "#147", PreserveSig = true)]
		internal static extern int VariantChangeTypeEx(ref VARIANT pvargDest, ref VARIANT pvarSrc, uint lcid, ushort wFlags, VARENUM vt);

		[DllImport("oleaut32.dll", EntryPoint = "#9", PreserveSig = true)]
		internal static extern int VariantClear(ref VARIANT pvarg);




		#endregion

		#region ole32

		[DllImport("ole32.dll", PreserveSig = true)]
		internal static extern int PropVariantClear(ref PROPVARIANT pvar);

		internal enum APTTYPE
		{
			APTTYPE_CURRENT = -1,
			APTTYPE_STA,
			APTTYPE_MTA,
			APTTYPE_NA,
			APTTYPE_MAINSTA
		}

		[DllImport("ole32.dll", PreserveSig = true)]
		internal static extern int CoGetApartmentType(out APTTYPE pAptType, out int pAptQualifier);

		[DllImport("ole32.dll", PreserveSig = true)]
		internal static extern int OleInitialize(IntPtr pvReserved);




		#endregion

		#region oleacc

		//internal static Guid IID_IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

		//internal static Guid IID_IAccessible2 = new Guid(0xE89F726E, 0xC4F4, 0x4c19, 0xBB, 0x19, 0xB6, 0x47, 0xD7, 0xFA, 0x84, 0x78);

		//[DllImport("oleacc.dll", PreserveSig = true)]
		//internal static extern int AccessibleObjectFromWindow(Wnd hwnd, AccOBJID dwId, ref Guid riid, out IntPtr ppvObject);

		//[DllImport("oleacc.dll", PreserveSig = true)]
		//internal static extern int WindowFromAccessibleObject(IntPtr iacc, out Wnd phwnd);

		//[DllImport("oleacc.dll", PreserveSig = true)]
		//internal static extern int AccessibleObjectFromPoint(Point ptScreen, out IntPtr ppacc, out VARIANT pvarChild);

		[DllImport("oleacc.dll", PreserveSig = true)]
		internal static extern int AccessibleObjectFromEvent(Wnd hwnd, int dwId, int dwChildId, out IntPtr ppacc, out VARIANT pvarChild);

		[DllImport("oleacc.dll")]
		internal static extern IntPtr GetProcessHandleFromHwnd(Wnd hwnd);

		internal delegate void WINEVENTPROC(IntPtr hWinEventHook, AccEVENT event_, Wnd hwnd, int idObject, int idChild, int idEventThread, int dwmsEventTime);

		[DllImport("user32.dll")]
		internal static extern IntPtr SetWinEventHook(AccEVENT eventMin, AccEVENT eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, int idProcess, int idThread, LibAccHookFlags dwFlags);

		[DllImport("user32.dll")]
		internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);




		#endregion

		#region msvcrt

		//don't use, because for eg "0xffffffff" returns int.Max instead of -1.
		//[DllImport("msvcrt.dll", EntryPoint = "wcstol", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int strtoi(char* s, char** endPtr = null, int radix = 0);

		//don't use the u API because they return 1 if the value is too big and the string contains '-'.
		//[DllImport("msvcrt.dll", EntryPoint = "wcstoul", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern uint strtoui(char* s, char** endPtr = null, int radix = 0);

		[DllImport("msvcrt.dll", EntryPoint = "_wcstoi64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern long strtoi64(char* s, char** endPtr = null, int radix = 0);
		//info: ntdll also has wcstol, wcstoul, _wcstoui64, but not _wcstoi64.

		//[DllImport("msvcrt.dll", EntryPoint = "_wcstoui64", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern ulong strtoui64(char* s, char** endPtr = null, int radix = 0);

		[DllImport("msvcrt.dll", EntryPoint = "_strtoi64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern long strtoi64(byte* s, byte** endPtr = null, int radix = 0);

		//not used
		//[DllImport("msvcrt.dll", EntryPoint = "_strtoui64", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern long strtoui64(byte* s, byte** endPtr = null, int radix = 0);

		//This is used when working with char*. With C# strings use String_.ToInt32 etc.
		internal static int strtoi(char* s, char** endPtr = null, int radix = 0)
		{
			return (int)strtoi64(s, endPtr, radix);
		}

		//This is used with UTF-8 text.
		internal static int strtoi(byte* s, byte** endPtr = null, int radix = 0)
		{
			return (int)strtoi64(s, endPtr, radix);
		}

#if false //not used, because we have String_.ToInt32 etc, which has no overflow problems. But it supports only decimal and hex, not any radix.
		/// <summary>
		/// Converts part of string to int.
		/// Returns the int value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="radix">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is invalid.</exception>
		internal static int strtoi(string s, int startIndex, out int numberEndIndex, int radix = 0)
		{
			int R = 0, len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s) {
					char* t = p + startIndex, e = t;
					R = strtoi(t, &e, radix);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		/// <summary>
		/// Converts part of string to long.
		/// Returns the long value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="radix">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is invalid.</exception>
		internal static long strtoi64(string s, int startIndex, out int numberEndIndex, int radix = 0)
		{
			long R = 0;
			int len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s) {
					char* t = p + startIndex, e = t;
					R = strtoi64(t, &e, radix);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		internal static int strtoi(string s, int startIndex = 0, int radix = 0)
		{
			return strtoi(s, startIndex, out _, radix);
		}

		internal static long strtoi64(string s, int startIndex = 0, int radix = 0)
		{
			return strtoi64(s, startIndex, out _, radix);
		}
#endif

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern char* _ltoa(int value, byte* s, int radix);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void* memcpy(void* to, void* from, LPARAM n);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void* memmove(void* to, void* from, LPARAM n);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void* memset(void* ptr, int ch, LPARAM n);




#endregion

#region dwmapi

		[DllImport("dwmapi.dll")]
		internal static extern int DwmGetWindowAttribute(Wnd hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

		[DllImport("dwmapi.dll")]
		internal static extern int DwmGetWindowAttribute(Wnd hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);




#endregion

#region ntdll

		[DllImport("ntdll.dll")]
		internal static extern uint NtQueryTimerResolution(out uint maxi, out uint mini, out uint current);
		//info: NtSetTimerResolution can set min 0.5 ms resolution. timeBeginPeriod min 1.

		internal struct MD5_CTX
		{
			long _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;
			public long r1, r2;
			//public fixed byte r[16]; //same speed, maybe slightly slower
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			//public byte[] r; //slow like .NET API
		}

		[DllImport("ntdll.dll")]
		internal static extern void MD5Init(out MD5_CTX context);

		[DllImport("ntdll.dll")]
		internal static extern void MD5Update(ref MD5_CTX context, byte[] data, int dataLen);

		[DllImport("ntdll.dll")]
		internal static extern void MD5Final(ref MD5_CTX context);




#endregion

#region other

		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int SetWindowTheme(Wnd hwnd, string pszSubAppName, string pszSubIdList);


		[DllImport("msi.dll", EntryPoint = "#217")]
		internal static extern int MsiGetShortcutTarget(string szShortcutPath, char* szProductCode, char* szFeatureId, char* szComponentCode);

		[DllImport("msi.dll", EntryPoint = "#173")]
		internal static extern int MsiGetComponentPath(char* szProduct, char* szComponent, [Out] char[] lpPathBuf, ref int pcchBuf);


		[DllImport("winmm.dll")]
		internal static extern uint timeBeginPeriod(uint uPeriod);

		[DllImport("winmm.dll")]
		internal static extern uint timeEndPeriod(uint uPeriod);


		[DllImport("hhctrl.ocx", EntryPoint = "HtmlHelpW")]
		internal static extern Wnd HtmlHelp(Wnd hwndCaller, string pszFile, uint uCommand, LPARAM dwData);



#endregion

	}
}
