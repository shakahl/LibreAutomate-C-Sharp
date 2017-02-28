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

using static Catkeys.NoClass;

#pragma warning disable 282 //C#7 intellisense bug: it thinks that Wnd has multiple fields.

namespace Catkeys
{
	public partial struct Wnd
	{
		/// <summary>
		/// Contains miscellaneous static window-related functions and classes, mostly rarely used or useful only for programmers.
		/// </summary>
		public static partial class Misc
		{
			/// <summary>
			/// Can be used with some functions as a special window handle value. It is implicitly converted to Wnd.
			/// </summary>
			/// <tocexclude />
			public enum SpecHwnd
			{
				/// <summary>API HWND_TOP. Used with SetWindowPos.</summary>
				Top = 0,
				/// <summary>API HWND_BOTTOM. Used with SetWindowPos.</summary>
				Bottom = 1,
				/// <summary>API HWND_TOPMOST. Used with SetWindowPos.</summary>
				Topmost = -1,
				/// <summary>API HWND_NOTOPMOST. Used with SetWindowPos.</summary>
				NoTopmost = -2,
				/// <summary>API HWND_MESSAGE. Used with API CreateWindowEx, Wnd.WndOwner etc.</summary>
				Message = -3,
				/// <summary>API HWND_BROADCAST. Used with API SendMessage, Wnd.Send etc.</summary>
				Broadcast = 0xffff
			}

			/// <summary>
			/// Returns true if w is one of enum <see cref="SpecHwnd"/> members.
			/// </summary>
			public static bool IsSpecHwnd(Wnd w)
			{
				int i = (LPARAM)w;
				return (i <= 1 && i >= -3) || i == 0xffff;
			}

			/// <summary>
			/// Calculates window rectangle from client area rectangle and style.
			/// Calls API <msdn>AdjustWindowRectEx</msdn>.
			/// </summary>
			/// <param name="r">Input - client area rectangle in screen. Output - window rectangle in screen.</param>
			/// <param name="style">Native.WS_ styles.</param>
			/// <param name="exStyle">Native.WS_EX_ styles.</param>
			/// <param name="hasMenu"></param>
			/// <remarks>
			/// Ignores styles WS_VSCROLL, WS_HSCROLL and wrapped menu bar.
			/// </remarks>
			public static bool WindowRectFromClientRect(ref RECT r, uint style, uint exStyle, bool hasMenu = false)
			{
				return Api.AdjustWindowRectEx(ref r, style, hasMenu, exStyle);
			}

			/// <summary>
			/// Calculates window border width from style.
			/// </summary>
			/// <param name="style">Native.WS_ styles.</param>
			/// <param name="exStyle">Native.WS_EX_ styles.</param>
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
				w.LibGetWindowInfo(out var x);
				return (int)x.cxWindowBorders;
			}

			/// <summary>
			/// Calls API <msdn>GetGUIThreadInfo</msdn>, which can get some GUI info, eg mouse capturing, menu mode, move/size mode, focus, caret.
			/// </summary>
			/// <param name="g">Variable that receives the info.</param>
			/// <param name="idThread">Thread id. If 0 - the foreground (active window) thread. See <see cref="ThreadId"/>.</param>
			public static bool GetGUIThreadInfo(out Native.GUITHREADINFO g, uint idThread = 0)
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
			/// Calls <msdn>CreateWindowEx</msdn>.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// For style and exStyle you can use Native.WS_ constants.
			/// Usually don't need to specify hInstance.
			/// </summary>
			public static Wnd CreateWindow(uint exStyle, string className, string name, uint style, int x = 0, int y = 0, int width = 0, int height = 0, Wnd parent = default(Wnd), LPARAM controlId = default(LPARAM), IntPtr hInstance = default(IntPtr), LPARAM param = default(LPARAM))
			{
				return Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param);
			}

			/// <summary>
			/// Creates native/unmanaged window like <see cref="CreateWindow"/> and sets font.
			/// If customFontHandle not specified, sets the system UI font, usually it is Segoe UI, 9.
			/// </summary>
			public static Wnd CreateWindowAndSetFont(uint exStyle, string className, string name, uint style, int x = 0, int y = 0, int width = 0, int height = 0, Wnd parent = default(Wnd), LPARAM controlId = default(LPARAM), IntPtr hInstance = default(IntPtr), LPARAM param = default(LPARAM), IntPtr customFontHandle = default(IntPtr))
			{
				var w = Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param);
				if(!w.Is0) SetFontHandle(w, (customFontHandle == Zero) ? _msgBoxFont : customFontHandle);
				return w;
			}
			static Util.LibNativeFont _msgBoxFont = new Util.LibNativeFont(SystemFonts.MessageBoxFont.ToHfont());

			/// <summary>
			/// Creates native/unmanaged <msdn>message-only window</msdn>.
			/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </summary>
			/// <param name="className">Window class name. Can be any existing class.</param>
			public static Wnd CreateMessageWindow(string className)
			{
				return CreateWindow(Native.WS_EX_NOACTIVATE, className, null, Native.WS_POPUP, parent: SpecHwnd.Message);
				//note: WS_EX_NOACTIVATE is important.
			}

			/// <summary>
			/// Destroys a native window of this thread.
			/// Calls API <msdn>DestroyWindow</msdn>.
			/// Returns false if failed. Supports <see cref="Native.GetError"/>.
			/// </summary>
			/// <seealso cref="Close"/>
			public static bool DestroyWindow(Wnd w)
			{
				return Api.DestroyWindow(w);
			}

			/// <summary>
			/// Gets icon that is displayed in window title bar and in its taskbar button.
			/// Returns icon handle if successful, else Zero. Later call <see cref="Icons.DestroyIconHandle"/>.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="size32">Get 32x32 icon. If false, gets 16x16 icon.</param>
			/// <remarks>
			/// Icon size depends on DPI (text size, can be changed in Control Panel). By default small is 16, large 32.
			/// This function can be used with windows of any process.
			/// </remarks>
			/// <seealso cref="Icons"/>
			public static IntPtr GetIconHandle(Wnd w, bool size32 = false)
			{
				int size = Api.GetSystemMetrics(size32 ? Api.SM_CXICON : Api.SM_CXSMICON);

				//support Windows Store apps
				if(1 == _WindowsStoreAppId(w, out var appId, true)) {
					IntPtr hi = Icons.GetFileIconHandle(appId, size);
					if(hi != Zero) return hi;
				}

				LPARAM R;
				bool ok = w.SendTimeout(2000, out R, Api.WM_GETICON, size32);
				if(R == 0 && ok) w.SendTimeout(2000, out R, Api.WM_GETICON, !size32);
				if(R == 0) R = WindowClass.GetClassLong(w, size32 ? Native.GCL_HICON : Native.GCL_HICONSM);
				if(R == 0) R = WindowClass.GetClassLong(w, size32 ? Native.GCL_HICONSM : Native.GCL_HICON);
				//tested this code with DPI 125%. Small icon of most windows match DPI (20), some 16, some 24.

				//Copy, because will DestroyIcon, also it resizes if need.
				if(R != 0) return Api.CopyImage(R, Api.IMAGE_ICON, size, size, 0);
				return Zero;
			}

			/// <summary>
			/// Sets native icon handle.
			/// Sends message API <msdn>WM_SETICON</msdn>.
			/// Does not copy the icon; don't dispose it while the window is alive.
			/// Use this function only with windows of current process.
			/// </summary>
			/// <seealso cref="Icons"/>
			public static void SetIconHandle(Wnd w, IntPtr iconHandle, bool size32 = false)
			{
				w.Send(Api.WM_SETICON, size32, iconHandle);
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
			/// Removes '&amp;' characters from string.
			/// Replaces "&amp;&amp;" to "&amp;".
			/// Returns true if s had '&amp;' characters.
			/// </summary>
			/// <remarks>
			/// Character '&amp;' is used to underline next character in displayed text of dialog controls and menu items. Two '&amp;' are used to display single '&amp;'.
			/// The underline is displayed when using the keyboard (eg Alt key) to select dialog controls and menu items.
			/// </remarks>
			public static bool StringRemoveUnderlineAmpersand(ref string s)
			{
				if(!Empty(s)) {
					int i = s.IndexOf('&');
					if(i >= 0) {
						i = s.IndexOf_("&&");
						if(i >= 0) s = s.Replace("&&", "\0");
						s = s.Replace("&", "");
						if(i >= 0) s = s.Replace("\0", "&");
						return true;
					}
				}
				return false;
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			/// <param name="m"></param>
			/// <param name="ignore">Messages to not show.</param>
			public static void PrintMsg(ref Message m, params uint[] ignore)
			{
				uint msg = (uint)m.Msg;
				if(ignore != null) foreach(uint t in ignore) { if(t == msg) return; }

				Wnd w = (Wnd)m.HWnd;
				uint counter = w.PropGet("PrintMsg"); w.PropSet("PrintMsg", ++counter);
				PrintList(counter, m);
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="msg"></param>
			/// <param name="wParam"></param>
			/// <param name="lParam"></param>
			/// <param name="ignore">Messages to not show.</param>
			public static void PrintMsg(Wnd w, uint msg, LPARAM wParam, LPARAM lParam, params uint[] ignore)
			{
				if(ignore != null) foreach(uint t in ignore) { if(t == msg) return; }
				var m = Message.Create(w.Handle, (int)msg, wParam, lParam);
				PrintMsg(ref m);
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			/// <param name="m"></param>
			/// <param name="ignore">Messages to not show.</param>
			public static void PrintMsg(ref Native.MSG m, params uint[] ignore)
			{
				PrintMsg(m.hwnd, m.message, m.wParam, m.lParam, ignore);
			}

			/// <summary><msdn>SUBCLASSPROC</msdn></summary>
			/// <tocexclude />
			public delegate LPARAM SUBCLASSPROC(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, IntPtr dwRefData);

			/// <summary><msdn>SetWindowSubclass</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#410")]
			public static extern bool SetWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass, IntPtr dwRefData);

			/// <summary><msdn>GetWindowSubclass</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#411")] //this is exported only by ordinal
			public static extern bool GetWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass, out IntPtr pdwRefData);

			/// <summary><msdn>RemoveWindowSubclass</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#412")]
			public static extern bool RemoveWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass);

			/// <summary><msdn>DefSubclassProc</msdn></summary>
			[DllImport("comctl32.dll", EntryPoint = "#413")]
			public static extern LPARAM DefSubclassProc(Wnd hWnd, uint uMsg, LPARAM wParam, LPARAM lParam);

			/// <summary><msdn>DefWindowProc</msdn></summary>
			[DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
			public static extern LPARAM DefWindowProc(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);
		}

	}
}
