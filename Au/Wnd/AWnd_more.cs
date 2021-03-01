//#define CW_CBT

using Au.Types;
using Au.Util;
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
using System.Linq;

namespace Au
{
	public partial struct AWnd
	{
		/// <summary>
		/// Miscellaneous window-related functions and classes. Rarely used, or useful only for programmers.
		/// </summary>
		public static partial class More
		{
			/// <summary>
			/// Calls API <msdn>GetGUIThreadInfo</msdn>. It gets info about mouse capturing, menu mode, move/size mode, focus, caret, etc.
			/// </summary>
			/// <param name="g">API <msdn>GUITHREADINFO</msdn>.</param>
			/// <param name="idThread">Thread id. If 0 - the foreground (active window) thread. See <see cref="ThreadId"/>, <see cref="AThread.Id"/>.</param>
			public static bool GetGUIThreadInfo(out Native.GUITHREADINFO g, int idThread = 0) {
				g = new Native.GUITHREADINFO(); g.cbSize = Api.SizeOf(g);
				return Api.GetGUIThreadInfo(idThread, ref g);
			}

			//public void ShowAnimate(bool show)
			//{
			//	//Don't add AWnd function, because:
			//		//Rarely used.
			//		//Api.AnimateWindow() works only with windows of current thread.
			//		//Only programmers would need it, and they can call the API directly.
			//}

			/// <summary>
			/// Registers new window class in this process.
			/// </summary>
			/// <param name="className">Class name.</param>
			/// <param name="wndProc">
			/// Delegate of a window procedure. See <msdn>Window Procedures</msdn>.
			/// 
			/// Use null when you need a different delegate (method or target object) for each window instance; create windows with <see cref="CreateWindow(Native.WNDPROC, string, string, WS, WS2, int, int, int, int, AWnd, LPARAM, IntPtr, LPARAM)"/> or <see cref="CreateMessageOnlyWindow(Native.WNDPROC, string)"/>.
			/// If not null, it must be a static method; create windows with any other function, including API <msdn>CreateWindowEx</msdn>.
			/// </param>
			/// <param name="etc">
			/// Can be used to specify API <msdn>WNDCLASSEX</msdn> fields.
			/// To set cursor use field <b>mCursor</b> (standard cursor) or <b>hCursor</b> (native handle of a custom cursor).
			/// If null, this function sets arrow cursor and style CS_VREDRAW | CS_HREDRAW.
			/// </param>
			/// <exception cref="ArgumentException"><i>wndProc</i> is an instance method. Must be static method or null. If need instance method, use null here and pass <i>wndProc</i> to <see cref="CreateWindow"/>.</exception>
			/// <exception cref="InvalidOperationException">The class already registered with this function and different <i>wndProc</i> (another method or another target object).</exception>
			/// <exception cref="Win32Exception">Failed, for example if the class already exists and was registered not with this function.</exception>
			/// <remarks>
			/// Calls API <msdn>RegisterClassEx</msdn>.
			/// The window class is registered until this process ends. Don't need to unregister.
			/// If called next time for the same window class, does nothing if <i>wndProc</i> is equal to the previous (or both null). Then ignores <i>etc</i>. Throws exception if different.
			/// Thread-safe.
			/// Protects the <i>wndProc</i> delegate from GC.
			/// </remarks>
			public static unsafe void RegisterWindowClass(string className, Native.WNDPROC wndProc = null, RWCEtc etc = null) {
				if (wndProc?.Target != null) throw new ArgumentException("wndProc must be static method or null. Use non-static wndProc with CreateWindow.");

				lock (s_classes) {
					if (s_classes.TryGetValue(className, out var wpPrev)) {
						if (wpPrev != wndProc) throw new InvalidOperationException("Window class already registered"); //another method or another target object
						return;
					}
					var x = new Api.WNDCLASSEX(etc);

					fixed (char* pCN = className) {
						x.lpszClassName = pCN;
						if (wndProc != null) {
							x.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);
						} else {
#if CW_CBT
							if(s_defWindowProc == default) s_defWindowProc = Api.GetProcAddress("user32.dll", "DefWindowProcW");
							x.lpfnWndProc = s_defWindowProc;
#else
							if (s_cwProcFP == default) s_cwProcFP = Marshal.GetFunctionPointerForDelegate(s_cwProc);
							x.lpfnWndProc = s_cwProcFP;
#endif
						}
						x.style |= Api.CS_GLOBALCLASS;

						if (0 == Api.RegisterClassEx(x)) throw new Win32Exception();
						//note: we don't return atom because: 1. Rarely used. 2. If assigned to an unused field, compiler may remove the function call.

						s_classes.Add(className, wndProc);
					}
				}
			}

			internal static bool IsClassRegistered_(string name, out Native.WNDPROC wndProc) {
				lock (s_classes) {
					return s_classes.TryGetValue(name, out wndProc);
				}
			}

			static Dictionary<string, Native.WNDPROC> s_classes = new(StringComparer.OrdinalIgnoreCase); //allows to find registered classes and protects wndProc delegates from GC
			[ThreadStatic] static List<(AWnd w, Native.WNDPROC p)> t_windows; //protects wndProc delegates of windows created in this thread from GC

#if CW_CBT
			static IntPtr s_defWindowProc;
#else
			static LPARAM _CWProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam) {
				//PrintMsg(w, msg, wParam, lParam);
				var wndProc = t_cwProc;
				if (wndProc == null) return Api.DefWindowProc(w, msg, wParam, lParam); //creating not with our CreateWindow(wndProc, ...)
				t_cwProc = null;
				w.SetWindowLong(Native.GWL.WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProc));
				return wndProc(w, msg, wParam, lParam);
			}
			static Native.WNDPROC s_cwProc = _CWProc; //GC
			static IntPtr s_cwProcFP;
			[ThreadStatic] static Native.WNDPROC t_cwProc;
#endif

			/// <summary>
			/// Creates native/unmanaged window of a class registered with <see cref="RegisterWindowClass"/> with null <i>wndProc</i>, and sets its window procedure.
			/// </summary>
			/// <exception cref="ArgumentException">The class is not registered with <see cref="RegisterWindowClass"/>, or registered with non-null <i>wndProc</i>.</exception>
			/// <exception cref="AuException">Failed to create window. Unlikely.</exception>
			/// <remarks>
			/// Calls API <msdn>CreateWindowEx</msdn>.
			/// Protects the <i>wndProc</i> delegate from GC.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </remarks>
			public static AWnd CreateWindow(Native.WNDPROC wndProc, string className, string name = null, WS style = 0, WS2 exStyle = 0, int x = 0, int y = 0, int width = 0, int height = 0, AWnd parent = default, LPARAM controlId = default, IntPtr hInstance = default, LPARAM param = default) {
				var a = t_windows ??= new List<(AWnd w, Native.WNDPROC p)>();
				for (int i = a.Count; --i >= 0;) if (!a[i].w.IsAlive) a.RemoveAt(i);

				if (!IsClassRegistered_(className, out var wp) || wp != null) throw new ArgumentException("Window class must be registered with AWnd.More.RegisterWindowClass with null wndProc");

				AWnd w;
				//need to cubclass the new window. But not after CreateWindowEx, because wndProc must receive all messages.
#if CW_CBT //slightly slower and dirtier. Invented before Core, to support multiple appdomains.
				using(AHookWin.ThreadCbt(c => { //let CBT hook subclass before any messages
					if(c.code == HookData.CbtEvent.CREATEWND) {
						//note: unhook as soon as possible. Else possible exception etc.
						//	If eg hook proc uses 'lock' and that 'lock' must wait,
						//		our hook proc is called again and again while waiting, until 'lock' throws exception.
						//	In STA thread 'lock' dispatches messages, but I don't know why hook proc is called multiple times for same event.
						c.hook.Unhook();

						var ww = (AWnd)c.wParam;
						Debug.Assert(ww.ClassNameIs(className));
						ww.SetWindowLong(Native.GWL.WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProc));
					} else Debug.Assert(false);
					return false;
				})) {
					w = Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param);
				}
#else
				t_cwProc = wndProc; //let _DefWndProc subclass on first message
				try { w = Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param); }
				finally { t_cwProc = null; } //if CreateWindowEx failed and _CWProc not called
#endif

				if (w.Is0) throw new AuException(0);
				a.Add((w, wndProc));
				return w;
			}

			/// <summary>
			/// Creates native/unmanaged window.
			/// </summary>
			/// <exception cref="AuException">Failed to create window. Unlikely.</exception>
			/// <remarks>
			/// Calls API <msdn>CreateWindowEx</msdn>.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </remarks>
			/// <seealso cref="RegisterWindowClass"/>
			public static AWnd CreateWindow(string className, string name = null, WS style = 0, WS2 exStyle = 0, int x = 0, int y = 0, int width = 0, int height = 0, AWnd parent = default, LPARAM controlId = default, IntPtr hInstance = default, LPARAM param = default) {
				var w = Api.CreateWindowEx(exStyle, className, name, style, x, y, width, height, parent, controlId, hInstance, param);
				if (w.Is0) throw new AuException(0);
				return w;
			}

			/// <summary>
			/// Creates native/unmanaged <msdn>message-only window</msdn>.
			/// </summary>
			/// <param name="className">Window class name. Can be any existing class.</param>
			/// <exception cref="AuException">Failed to create window. Unlikely.</exception>
			/// <remarks>
			/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </remarks>
			public static AWnd CreateMessageOnlyWindow(string className) {
				return CreateWindow(className, null, WS.POPUP, WS2.NOACTIVATE, parent: Native.HWND.MESSAGE);
				//note: WS_EX_NOACTIVATE is important.
			}

			/// <summary>
			/// Creates native/unmanaged <msdn>message-only window</msdn> of a class registered with <see cref="RegisterWindowClass"/> with null <i>wndProc</i>, and sets its window procedure.
			/// </summary>
			/// <param name="className">Window class name.</param>
			/// <param name="wndProc"></param>
			/// <exception cref="ArgumentException">The class is not registered with <see cref="RegisterWindowClass"/>, or registered with non-null <i>wndProc</i>.</exception>
			/// <exception cref="AuException">Failed to create window. Unlikely.</exception>
			/// <remarks>
			/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
			/// Protects the <i>wndProc</i> delegate from GC.
			/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
			/// </remarks>
			public static AWnd CreateMessageOnlyWindow(Native.WNDPROC wndProc, string className) {
				return CreateWindow(wndProc, className, null, WS.POPUP, WS2.NOACTIVATE, parent: Native.HWND.MESSAGE);
				//note: WS_EX_NOACTIVATE is important.
			}

			/// <summary>
			/// Destroys a native window of this thread.
			/// Calls API <msdn>DestroyWindow</msdn>.
			/// Returns false if failed. Supports <see cref="ALastError"/>.
			/// </summary>
			/// <seealso cref="Close"/>
			public static bool DestroyWindow(AWnd w) {
				return Api.DestroyWindow(w);
			}

			/// <summary>
			/// Sets font.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="font">
			/// Native font handle.
			/// If default(IntPtr), sets font that is used by most windows and controls on this computer, usually Segoe UI, 9, DPI-scaled for w screen.
			/// </param>
			/// <remarks>
			/// Sends <msdn>WM_SETFONT</msdn> message.
			/// </remarks>
			public static void SetFont(AWnd w, IntPtr font = default) {
				w.Send(Api.WM_SETFONT, font != default ? font : NativeFont_.RegularCached(ADpi.OfWindow(w)).Handle);
			}

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
			public static string GetWindowsStoreAppId(AWnd w, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false) {
				if (0 != Internal_.GetWindowsStoreAppId(w, out var R, prependShellAppsFolder, getExePathIfNotWinStoreApp)) return R;
				return null;
			}

			//rejected. Rarely used. Easy to send message.
			///// <summary>
			///// Sets native font handle.
			///// Sends message API <msdn>WM_SETFONT</msdn> with lParam 1.
			///// Does not copy the font; don't dispose it while the window is alive.
			///// Use this function only with windows of current process.
			///// </summary>
			//public static void SetFontHandle(AWnd w, IntPtr fontHandle)
			//{
			//	w.Send(Api.WM_SETFONT, fontHandle, 1);
			//}

			///// <summary>
			///// Gets native font handle.
			///// Sends message API <msdn>WM_GETFONT</msdn>.
			///// Does not copy the font; don't need to dispose.
			///// Use this function only with windows of current process.
			///// </summary>
			//public static IntPtr GetFontHandle(AWnd w)
			//{
			//	return w.Send(Api.WM_GETFONT);
			//}

			///// <summary>
			///// Sets native icon handle.
			///// Sends message API <msdn>WM_SETICON</msdn>.
			///// Does not copy the icon; don't dispose it while the window is alive.
			///// Use this function only with windows of current process.
			///// </summary>
			///// <seealso cref="AIcon"/>
			//public static void SetIconHandle(AWnd w, IntPtr iconHandle, bool size32 = false)
			//{
			//	w.Send(Api.WM_SETICON, size32, iconHandle);
			//}

			/// <summary>
			/// Calls API <msdn>GetClassLongPtr</msdn>.
			/// </summary>
			/// <remarks>
			/// Supports <see cref="ALastError"/>.
			/// For index can be used constants from <see cref="Native.GCL"/>. All values are the same in 32-bit and 64-bit process.
			/// In 32-bit process actually calls <b>GetClassLong</b>, because <b>GetClassLongPtr</b> is unavailable.
			/// </remarks>
			public static LPARAM GetClassLong(AWnd w, int index) => Api.GetClassLongPtr(w, index);

			//probably not useful. Dangerous.
			///// <summary>
			///// Calls API <msdn>SetClassLongPtr</msdn> (SetClassLong in 32-bit process).
			///// </summary>
			///// <exception cref="AuWndException"/>
			//public static LPARAM SetClassLong(AWnd w, int index, LPARAM newValue)
			//{
			//	ALastError.Clear();
			//	LPARAM R = Api.SetClassLongPtr(w, index, newValue);
			//	if(R == 0 && ALastError.Code != 0) w.ThrowUseNative();
			//	return R;
			//}

			//Rejected. Does not work with many windows. Unreliable. Rarely used.
			///// <summary>
			///// Gets atom of a window class.
			///// To get class atom when you have a window w, use <c>AWnd.More.GetClassLong(w, Native.GCL.ATOM)</c>.
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
			public static int RegisterMessage(string name, bool uacEnable = false) {
				var m = Api.RegisterWindowMessage(name);
				if (uacEnable && m != 0) Api.ChangeWindowMessageFilter(m, 1);
				return m;
			}

			/// <summary>
			/// Calls API <msdn>ChangeWindowMessageFilter</msdn> for each message in the list of messages.
			/// It allows processes of lower [](xref:uac) integrity level to send these messages to this process.
			/// </summary>
			public static void UacEnableMessages(params int[] messages) {
				foreach (var m in messages) Api.ChangeWindowMessageFilter(m, 1);
			}

			/// <summary>
			/// Writes a Windows message to a string.
			/// If the message is specified in <i>options</i>, sets <c>s=null</c> and returns false.
			/// </summary>
			public static bool PrintMsg(out string s, in System.Windows.Forms.Message m, PrintMsgOptions options = null, [CallerMemberName] string caller = null) {
				if (options?.Skip?.Contains(m.Msg) ?? false) { s = null; return false; }

				var sm = m.ToString();

				int i = 0;
				if (options?.Indent ?? true) { //makes 5-10 times slower, but not too slow
					MethodBase m0 = null;
					foreach (var f in new StackTrace(1).GetFrames()) {
						var m1 = f.GetMethod();
						if (m1.Name != caller) continue;
						if (m0 == null) m0 = m1; else if ((object)m1 == m0) i++;
					}
				}
				string si = i == 0 ? null : new string('\t', i);

				if (options?.Number ?? true) {
					AWnd w = (AWnd)m.HWnd;
					uint counter = (uint)w.Prop["PrintMsg"]; w.Prop.Set("PrintMsg", ++counter);
					s = si + counter.ToString() + ", " + sm;
				} else {
					s = si + sm;
				}
				return true;
			}

			/// <summary>
			/// Writes a Windows message to a string.
			/// If the message is specified in <i>options</i>, sets <c>s=null</c> and returns false.
			/// </summary>
			public static bool PrintMsg(out string s, AWnd w, int msg, LPARAM wParam, LPARAM lParam, PrintMsgOptions options = null, [CallerMemberName] string caller = null) {
				var m = System.Windows.Forms.Message.Create(w.Handle, msg, wParam, lParam);
				return PrintMsg(out s, in m, options, caller);
			}

			/// <summary>
			/// Writes a Windows message to a string.
			/// If the message is specified in <i>options</i>, sets <c>s=null</c> and returns false.
			/// </summary>
			public static bool PrintMsg(out string s, in Native.MSG m, PrintMsgOptions options = null, [CallerMemberName] string caller = null) {
				return PrintMsg(out s, m.hwnd, m.message, m.wParam, m.lParam, options, caller);
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			public static void PrintMsg(in System.Windows.Forms.Message m, PrintMsgOptions options = null, [CallerMemberName] string caller = null) {
				if (PrintMsg(out var s, in m, options, caller)) AOutput.Write(s);
			}

			/// <summary>
			/// Writes a Windows message to the output.
			/// </summary>
			public static void PrintMsg(AWnd w, int msg, LPARAM wParam, LPARAM lParam, PrintMsgOptions options = null, [CallerMemberName] string caller = null) {
				var m = System.Windows.Forms.Message.Create(w.Handle, msg, wParam, lParam);
				PrintMsg(in m, options, caller);
			}

			/// <summary>
			/// Writes a Windows message as API <msdn>MSG</msdn> to the output.
			/// </summary>
			public static void PrintMsg(in Native.MSG m, PrintMsgOptions options = null, [CallerMemberName] string caller = null) {
				PrintMsg(m.hwnd, m.message, m.wParam, m.lParam, options, caller);
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Used with <see cref="AWnd.More.RegisterWindowClass"/>.
	/// </summary>
	[NoDoc]
	public class RWCEtc
	{
#pragma warning disable 1591 //XML doc
		public uint style;
		public int cbClsExtra;
		public int cbWndExtra;
		public IntPtr hIcon;
		public IntPtr hCursor;
		public MCursor mCursor;
		public nint hbrBackground;
		public IntPtr hIconSm;
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Options for <see cref="AWnd.More.PrintMsg"/>.
	/// </summary>
	public class PrintMsgOptions
	{
		///
		public PrintMsgOptions() { }

		/// <summary>
		/// Sets the <see cref="Skip"/> PrintMsgSettings.
		/// </summary>
		public PrintMsgOptions(params int[] skip) { Skip = skip; }

		/// <summary>
		/// Prepend 1, 2, 3...
		/// Default is true. As well as if <i>options</i> is null.
		/// </summary>
		public bool Number { get; set; } = true;

		/// <summary>
		/// Prepend one or more tabs if the caller function (usually WndProc) is called recursively.
		/// Default is true. As well as if <i>options</i> is null.
		/// </summary>
		public bool Indent { get; set; } = true;

		/// <summary>
		/// Ignore these messages.
		/// </summary>
		public int[] Skip { get; set; }
	}
}
