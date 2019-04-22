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

#pragma warning disable 282 //intellisense bug: it thinks that Wnd has multiple fields.

namespace Au
{
	public partial struct Wnd
	{
		/// <summary>
		/// Contains miscellaneous static window-related functions and classes. Rarely used, or useful only for programmers.
		/// </summary>
		[System.Security.SuppressUnmanagedCodeSecurity]
		public static partial class Misc
		{
			/// <summary>
			/// Calculates window rectangle from client area rectangle and style.
			/// Calls API <msdn>AdjustWindowRectEx</msdn>.
			/// </summary>
			/// <param name="r">Input - client area rectangle in screen. Output - window rectangle in screen.</param>
			/// <param name="style"></param>
			/// <param name="exStyle"></param>
			/// <param name="hasMenu"></param>
			/// <remarks>
			/// Ignores styles WS_VSCROLL, WS_HSCROLL and wrapped menu bar.
			/// </remarks>
			public static bool WindowRectFromClientRect(ref RECT r, WS style, WS_EX exStyle, bool hasMenu = false)
			{
				return Api.AdjustWindowRectEx(ref r, style, hasMenu, exStyle);
			}

			/// <summary>
			/// Calculates window border width from style.
			/// </summary>
			public static int BorderWidth(WS style, WS_EX exStyle)
			{
				RECT r = default;
				Api.AdjustWindowRectEx(ref r, style, false, exStyle);
				return r.right;
			}

			/// <summary>
			/// Gets window border width.
			/// </summary>
			public static int BorderWidth(Wnd w)
			{
				w.LibGetWindowInfo(out var x);
				return x.cxWindowBorders;
			}

			/// <summary>
			/// Calls API <msdn>GetGUIThreadInfo</msdn>, which can get some GUI info, eg mouse capturing, menu mode, move/size mode, focus, caret.
			/// </summary>
			/// <param name="g">Variable that receives the info.</param>
			/// <param name="idThread">Thread id. If 0 - the foreground (active window) thread. See <see cref="ThreadId"/>.</param>
			public static bool GetGUIThreadInfo(out Native.GUITHREADINFO g, int idThread = 0)
			{
				g = new Native.GUITHREADINFO(); g.cbSize = Api.SizeOf(g);
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
			/// Creates native/unmanaged window.
			/// Calls API <msdn>CreateWindowEx</msdn>.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// Usually don't need to specify hInstance.
			/// </summary>
			public static Wnd CreateWindow(string className, string name = null, WS style = 0, WS_EX exStyle = 0, int x = 0, int y = 0, int width = 0, int height = 0, Wnd parent = default, LPARAM controlId = default, IntPtr hInstance = default, LPARAM param = default)
			{
				return Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param);
			}

			/// <summary>
			/// Creates native/unmanaged window like <see cref="CreateWindow"/> and sets font.
			/// If customFontHandle not specified, sets the system UI font, usually it is Segoe UI, 9.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </summary>
			public static Wnd CreateWindowAndSetFont(string className, string name = null, WS style = 0, WS_EX exStyle = 0, int x = 0, int y = 0, int width = 0, int height = 0, Wnd parent = default, LPARAM controlId = default, IntPtr hInstance = default, LPARAM param = default, IntPtr customFontHandle = default)
			{
				var w = Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param);
				if(!w.Is0) SetFontHandle(w, (customFontHandle == default) ? Util.LibNativeFont.RegularCached : customFontHandle);
				return w;
			}

			/// <summary>
			/// Creates native/unmanaged <msdn>message-only window</msdn>.
			/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </summary>
			/// <param name="className">Window class name. Can be any existing class.</param>
			public static Wnd CreateMessageOnlyWindow(string className)
			{
				return CreateWindow(className, null, WS.POPUP, WS_EX.NOACTIVATE, parent: Native.HWND.MESSAGE);
				//note: WS_EX_NOACTIVATE is important.
			}

			/// <summary>
			/// Destroys a native window of this thread.
			/// Calls API <msdn>DestroyWindow</msdn>.
			/// Returns false if failed. Supports <see cref="WinError"/>.
			/// </summary>
			/// <seealso cref="Close"/>
			public static bool DestroyWindow(Wnd w)
			{
				return Api.DestroyWindow(w);
			}

			/// <summary>
			/// Returns true if the window is a <msdn>message-only window</msdn>.
			/// </summary>
			public static bool IsMessageOnlyWindow(Wnd w)
			{
				var v = w.LibParentGWL;
				if(v != default) {
					if(s_messageOnlyParent.Is0) s_messageOnlyParent = FindMessageOnlyWindow(null, null).LibParentGWL;
					return v == s_messageOnlyParent;
				}
				return false;
			}
			static Wnd s_messageOnlyParent;

			/// <summary>
			/// Gets window Windows Store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
			/// Returns null if fails or if called on Windows 7.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="prependShellAppsFolder">Prepend <c>@"shell:AppsFolder\"</c> (to run or get icon).</param>
			/// <param name="getExePathIfNotWinStoreApp">Get program path if it is not a Windows Store app.</param>
			/// <remarks>
			/// Windows Store app window class name can be "Windows.UI.Core.CoreWindow" or "ApplicationFrameWindow".
			/// </remarks>
			public static string GetWindowsStoreAppId(Wnd w, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
			{
				if(0 != _GetWindowsStoreAppId(w, out var R, prependShellAppsFolder, getExePathIfNotWinStoreApp)) return R;
				return null;
			}

			/// <summary>
			/// Sets native font handle.
			/// Sends message API <msdn>WM_SETFONT</msdn> with lParam 1.
			/// Does not copy the font; don't dispose it while the window is alive.
			/// Use this function only with windows of current process.
			/// </summary>
			public static void SetFontHandle(Wnd w, IntPtr fontHandle)
			{
				w.Send(Api.WM_SETFONT, fontHandle, 1);
			}

			/// <summary>
			/// Gets native font handle.
			/// Sends message API <msdn>WM_GETFONT</msdn>.
			/// Does not copy the font; don't need to dispose.
			/// Use this function only with windows of current process.
			/// </summary>
			public static IntPtr GetFontHandle(Wnd w)
			{
				return w.Send(Api.WM_GETFONT);
			}

			/// <summary>
			/// Sets native icon handle.
			/// Sends message API <msdn>WM_SETICON</msdn>.
			/// Does not copy the icon; don't dispose it while the window is alive.
			/// Use this function only with windows of current process.
			/// </summary>
			/// <seealso cref="Icon_"/>
			public static void SetIconHandle(Wnd w, IntPtr iconHandle, bool size32 = false)
			{
				w.Send(Api.WM_SETICON, size32, iconHandle);
			}

			/// <summary>
			/// Gets icon that is displayed in window title bar and in its taskbar button.
			/// Returns icon handle if successful, else default(IntPtr). Later call <see cref="Icon_.DestroyIconHandle"/> or <see cref="Icon_.HandleToImage"/>.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="size32">Get 32x32 icon. If false, gets 16x16 icon.</param>
			/// <remarks>
			/// Icon size depends on DPI (text size, can be changed in Windows Settings). By default small is 16, large 32.
			/// This function can be used with windows of any process.
			/// </remarks>
			/// <seealso cref="Icon_"/>
			public static IntPtr GetIconHandle(Wnd w, bool size32 = false)
			{
				int size = Api.GetSystemMetrics(size32 ? Api.SM_CXICON : Api.SM_CXSMICON);

				//support Windows Store apps
				if(1 == _GetWindowsStoreAppId(w, out var appId, true)) {
					IntPtr hi = Icon_.GetFileIconHandle(appId, size);
					if(hi != default) return hi;
				}

				bool ok = w.SendTimeout(2000, out LPARAM R, Api.WM_GETICON, size32);
				if(R == 0 && ok) w.SendTimeout(2000, out R, Api.WM_GETICON, !size32);
				if(R == 0) R = GetClassLong(w, size32 ? Native.GCL.HICON : Native.GCL.HICONSM);
				if(R == 0) R = GetClassLong(w, size32 ? Native.GCL.HICONSM : Native.GCL.HICON);
				//tested this code with DPI 125%. Small icon of most windows match DPI (20), some 16, some 24.
				//TEST: undocumented API InternalGetWindowIcon.

				//Copy, because will DestroyIcon, also it resizes if need.
				if(R != 0) return Api.CopyImage(R, Api.IMAGE_ICON, size, size, 0);
				return default;
			}

			/// <summary>
			/// Calls API <msdn>GetClassLongPtr</msdn>.
			/// </summary>
			/// <remarks>
			/// Supports <see cref="WinError"/>.
			/// For index can be used constants from <see cref="Native.GCL"/>. All values are the same in 32-bit and 64-bit process.
			/// In 32-bit process actually calls <b>GetClassLong</b>, because <b>GetClassLongPtr</b> is unavailable.
			/// </remarks>
			public static LPARAM GetClassLong(Wnd w, int index) => Api.GetClassLongPtr(w, index);

			//probably not useful. Dangerous.
			///// <summary>
			///// Calls API <msdn>SetClassLongPtr</msdn> (SetClassLong in 32-bit process).
			///// </summary>
			///// <exception cref="WndException"/>
			//public static LPARAM SetClassLong(Wnd w, int index, LPARAM newValue)
			//{
			//	WinError.Clear();
			//	LPARAM R = Api.SetClassLongPtr(w, index, newValue);
			//	if(R == 0 && WinError.Code != 0) w.ThrowUseNative();
			//	return R;
			//}

			//Rejected. Does not work with many windows. Unreliable. Rarely used.
			///// <summary>
			///// Gets atom of a window class.
			///// To get class atom when you have a window w, use <c>Wnd.Misc.GetClassLong(w, Native.GCL.ATOM)</c>.
			///// </summary>
			///// <param name="className">Class name.</param>
			///// <param name="moduleHandle">Native module handle of the exe or dll that registered the class. Don't use if it is a global class (CS_GLOBALCLASS style).</param>
			//public static ushort GetClassAtom(string className, IntPtr moduleHandle = default)
			//{
			//	var x = new Api.WNDCLASSEX();
			//	x.cbSize = Api.SizeOf(x);
			//	return Api.GetClassInfoEx(moduleHandle, className, ref x);
			//}

			/// <summary>
			/// Calls API <msdn>RegisterWindowMessage</msdn>.
			/// </summary>
			/// <param name="name">Message name. Can be any unique string.</param>
			/// <param name="uacEnable">Also call API <msdn>ChangeWindowMessageFilter</msdn> for the message. More info: <see cref="UacEnableMessages"/>.</param>
			public static int RegisterMessage(string name, bool uacEnable = false)
			{
				var m = Api.RegisterWindowMessage(name);
				if(uacEnable && m != 0) Api.ChangeWindowMessageFilter(m, 1);
				return m;
			}

			/// <summary>
			/// Calls API <msdn>ChangeWindowMessageFilter</msdn> for each message in the list of messages.
			/// It allows processes of lower [](xref:uac) integrity level to send these messages to this process.
			/// </summary>
			public static void UacEnableMessages(params int[] messages)
			{
				foreach(var m in messages) Api.ChangeWindowMessageFilter(m, 1);
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			/// <param name="m"></param>
			/// <param name="ignore">Messages to not show.</param>
			public static void PrintMsg(in System.Windows.Forms.Message m, params int[] ignore)
			{
				if(ignore != null) foreach(uint t in ignore) { if(t == m.Msg) return; }

				Wnd w = (Wnd)m.HWnd;
				uint counter = (uint)w.Prop["PrintMsg"]; w.Prop.Set("PrintMsg", ++counter);
				Print(counter.ToString(), m.ToString());
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="msg"></param>
			/// <param name="wParam"></param>
			/// <param name="lParam"></param>
			/// <param name="ignore">Messages to not show.</param>
			public static void PrintMsg(Wnd w, int msg, LPARAM wParam, LPARAM lParam, params int[] ignore)
			{
				if(ignore != null) foreach(uint t in ignore) { if(t == msg) return; }
				var m = System.Windows.Forms.Message.Create(w.Handle, (int)msg, wParam, lParam);
				PrintMsg(in m);
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			/// <param name="m"></param>
			/// <param name="ignore">Messages to not show.</param>
			public static void PrintMsg(in Native.MSG m, params int[] ignore)
			{
				PrintMsg(m.hwnd, m.message, m.wParam, m.lParam, ignore);
			}

			/// <summary>API <msdn>SetWindowSubclass</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#410")]
			public static extern bool SetWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass, IntPtr dwRefData);

			/// <summary>API <msdn>GetWindowSubclass</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#411")] //this is exported only by ordinal
			public static extern bool GetWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass, out IntPtr pdwRefData);

			/// <summary>API <msdn>RemoveWindowSubclass</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#412")]
			public static extern bool RemoveWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass);

			/// <summary>API <msdn>DefSubclassProc</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#413")]
			public static extern LPARAM DefSubclassProc(Wnd hWnd, uint uMsg, LPARAM wParam, LPARAM lParam);
		}
	}
}

namespace Au.Types
{
	/// <summary>API <msdn>SUBCLASSPROC</msdn></summary>
	public delegate LPARAM SUBCLASSPROC(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, IntPtr dwRefData);
}
