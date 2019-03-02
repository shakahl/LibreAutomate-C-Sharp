using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	public partial struct Wnd
	{
		public static partial class Misc
		{
			/// <summary>
			/// Creates window and allows you to replace its window procedure.
			/// </summary>
			/// <remarks>
			/// Similar to <see cref="System.Windows.Forms.NativeWindow"/>, but more lightweight and does not change the class name.
			/// Can register a new window class or extend (subclass) an existing class.
			/// </remarks>
			public class MyWindow
			{
				static LPARAM _defWindowProc = Api.GetProcAddress("user32.dll", "DefWindowProcW");
				LPARAM _nativeWndProc; //old window proc, usually _defWindowProc
				Native.WNDPROC _ourWndProc; //delegate of _WndProc (keeps from GC)
				Native.WNDPROC _userWndProc; //_ourWndProc calls it
				[ThreadStatic] static List<MyWindow> t_windows; //keep all MyWindow of this thread from GC

				/// <summary>
				/// Sets window procedure of your window class.
				/// It shoud call <see cref="DefWndProc"/> and return its return value. Except when don't need default processing.
				/// More info: <msdn>Window Procedures</msdn>.
				/// </summary>
				/// <seealso cref="PrintMsg(Wnd, int, LPARAM, LPARAM, int[])"/>
				public MyWindow(Native.WNDPROC wndProc)
				{
					_userWndProc = wndProc;
					_ourWndProc = _WndProc;
				}

				/// <summary>
				/// Native window handle.
				/// </summary>
				public Wnd Handle { get; private set; }

				/// <summary>
				/// Creates window.
				/// Calls API <msdn>CreateWindowEx</msdn>.
				/// Returns false if failed, for example className is not registered.
				/// See also: <see cref="RegisterClass"/>, <see cref="Handle"/>.
				/// </summary>
				/// <param name="className">The name of any window class existing in this process. You can register a class with <see cref="RegisterClass"/> or use any other class.</param>
				/// <param name="name">Window name or null.</param>
				/// <param name="style"></param>
				/// <param name="exStyle"></param>
				/// <param name="x"></param>
				/// <param name="y"></param>
				/// <param name="width"></param>
				/// <param name="height"></param>
				/// <param name="parent">Owner or parent window, or default.</param>
				/// <param name="controlId">Control id or 0.</param>
				/// <remarks>
				/// The window will be destroyed in these cases: 1. Called <see cref="Destroy"/>. 2. Closed by the user or some program/script. 3. When this thread ends. 4. This function called again (then destroys old window and creates new).
				/// </remarks>
				public bool Create(string className, string name = null, WS style = 0, WS_EX exStyle = 0, int x = 0, int y = 0, int width = 0, int height = 0, Wnd parent = default, LPARAM controlId = default)
				{
					if(t_windows == null) t_windows = new List<MyWindow>();
					Destroy();
					if(!_Create(className, name, style, exStyle, x, y, width, height, parent, controlId)) return false;
					t_windows.Add(this);
					return true;
				}

				/// <summary>
				/// Creates <msdn>message-only window</msdn> with styles WS_POPUP, WS_EX_NOACTIVATE.
				/// More info: <see cref="Create"/>.
				/// </summary>
				public bool CreateMessageWindow(string className, string name = null)
				{
					return Create(className, name, WS.POPUP, WS_EX.NOACTIVATE, 0, 0, 0, 0, Native.HWND.MESSAGE);
					//note: WS_EX_NOACTIVATE is important.
				}

				/// <summary>
				/// Destroys the window.
				/// </summary>
				/// <remarks>
				/// Calls API <msdn>DestroyWindow</msdn>.
				/// Does nothing if the window is already destroyed, for example closed by the user.
				/// If the window is not destroyed explicitly, the system destroys it when its thread ends.
				/// Must be called from window's thread.
				/// </remarks>
				public void Destroy()
				{
					if(Handle.Is0) return;
					bool ok = Api.DestroyWindow(Handle);
					Debug.Assert(ok);
					Debug.Assert(Handle == default && !t_windows.Contains(this));
				}

				LPARAM _WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
				{
					var R = _userWndProc(w, message, wParam, lParam);

					if(message == Api.WM_NCDESTROY) {
						Handle = default;
						t_windows.Remove(this);
					}

					return R;
				}

				/// <summary>
				/// Calls the native window procedure of the window class.
				/// More info: <msdn>Window Procedures</msdn>.
				/// </summary>
				public LPARAM DefWndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
				{
					return _nativeWndProc == _defWindowProc
						? Api.DefWindowProc(w, message, wParam, lParam) //not necessary but presumably faster
						: Api.CallWindowProc(_nativeWndProc, w, message, wParam, lParam);
				}

				bool _Create(string className, string name, WS style, WS_EX exStyle, int x, int y, int width, int height, Wnd parent, LPARAM controlId)
				{
					using(Util.WinHook.ThreadCbt(c => {
						if(c.code == HookData.CbtEvent.CREATEWND) {
							//note: unhook as soon as possible. Else possible exception etc.
							//	If eg hook proc uses 'lock' and that 'lock' must wait,
							//	our hook proc is called again and again while waiting, until 'lock' throws exception.
							//	It is known that 'lock' in STA thread dispatches messages, but I don't know why hook proc
							//	is called multiple times for same event.
							c.hook.Unhook();

							var ww = (Wnd)c.wParam;
							Debug.Assert(ww.ClassNameIs(className));
							_nativeWndProc = ww.SetWindowLong(Native.GWL.WNDPROC, Marshal.GetFunctionPointerForDelegate(_ourWndProc));
						} else Debug.Assert(false);
						return false;
					})) Handle = CreateWindow(className, name, style, exStyle, x, y, width, height, parent, controlId);
					return !Handle.Is0;

					//Replaces the window procedure (usually DefWindowProcW) with _WndProc.
					//	For it uses CBT hook. It is called before the system sends any messages to the window.
					//	_WndProc receives all messages, including WM_NCCREATE.
				}

				/// <summary>
				/// Registers new window class.
				/// Returns class atom.
				/// </summary>
				/// <param name="className">Class name.</param>
				/// <param name="ex">
				/// Can be used to specify more fields of <msdn>WNDCLASSEX</msdn> that is passed to <msdn>RegisterClassEx</msdn>.
				/// Defaults: hCursor = arrow; hbrBackground = COLOR_BTNFACE+1; style = CS_GLOBALCLASS; others = 0/null/default.
				/// This function also adds CS_GLOBALCLASS style.
				/// </param>
				/// <exception cref="Win32Exception">Failed, for example if the class already exists and was registered not with this function.</exception>
				/// <remarks>
				/// Calls API <msdn>RegisterClassEx</msdn>.
				/// The class then can be used in all appdomains of this process. To create windows, use <see cref="Create"/>.
				/// You provide a window procedure when creating a MyWindow variable. Windows created not with <see cref="Create"/> use API <msdn>DefWindowProc</msdn> as window procedure, and therefore are not useful.
				/// The window class remains registered until this process ends. Don't need to unregister.
				/// This function can be called multiple times for the same class, for example called once in each appdomain. Next time it just returns class atom.
				/// Thread-safe.
				/// <note type="note">Don't use code like <c>static ushort _atom = Wnd.Misc.MyWindow.RegisterClass("MyClass");</c>, because in Release configuration compiler removes this code if _atom is not used. Instead you can call this function in a static constructor.</note>
				/// </remarks>
				public static unsafe ushort RegisterClass(string className, WndClassEx ex = null)
				{
					lock("jU0tLiIbtE6KWg5aCu7RDg") {
						string interDomainVarName = "jU0tLiIbtE6KWg5aCu7RDg" + className.ToLower_();
						if(!Util.InterDomainVariables.GetVariable(interDomainVarName, out ushort atom)) {
							var x = new Api.WNDCLASSEX(ex);

							fixed (char* pCN = className) {
								x.lpszClassName = pCN;
								x.lpfnWndProc = _defWindowProc;
								x.style |= Api.CS_GLOBALCLASS;

								atom = Api.RegisterClassEx(x);
								if(atom == 0) throw new Win32Exception();
							}

							Util.InterDomainVariables.SetVariable(interDomainVarName, atom);
						}
						return atom;
					}
				}

				//FUTURE: Superclass. Not very useful, because now we can do the same with subclassing, except changing the class name.

				/// <summary>
				/// Used with <see cref="RegisterClass"/>.
				/// </summary>
				/// <tocexclude />
				public class WndClassEx
				{
#pragma warning disable 1591 //XML doc
					public uint style;
					public int cbClsExtra;
					public int cbWndExtra;
					public IntPtr hIcon;
					public IntPtr? hCursor;
					public IntPtr? hbrBackground;
					public IntPtr hIconSm;
#pragma warning restore 1591 //XML doc
				}
			}

		}
	}
}
