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
using System.Windows.Forms;
using System.Drawing;
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
			/// Creates window and allows your derived class to replace its window procedure.
			/// Similar to <see cref="NativeWindow"/>, but more lightweight and does not change the class name.
			/// Can register a new window class or extend (subclass) an existing class.
			/// </summary>
			public class MyWindow
			{
				static LPARAM _defWindowProc = Api.GetProcAddress("user32.dll", "DefWindowProcW");
				LPARAM _classWndProc; //old window proc, usually _defWindowProc
				Native.WNDPROC _wndProc; //keep from GC
				[ThreadStatic] static List<MyWindow> t_windows; //keep all MyWindow of this thread from GC

				/// <summary>
				/// Native window handle.
				/// </summary>
				public Wnd Handle { get; private set; }

				/// <summary>
				/// Creates window.
				/// Calls API <msdn>CreateWindowEx</msdn>.
				/// Returns false if failed, for example className is not registered.
				/// </summary>
				/// <param name="className">The name of any window class existing in this process. You can register a class with <see cref="RegisterClass"/> or use any other class.</param>
				/// <param name="name">Window name or null.</param>
				/// <param name="style">One or more Native.WS_ constants, like <c>Native.WS_OVERLAPPEDWINDOW|Native.WS_VISIBLE.</c></param>
				/// <param name="exStyle">One or more Native.WS_EX_ constants.</param>
				/// <param name="x"></param>
				/// <param name="y"></param>
				/// <param name="width"></param>
				/// <param name="height"></param>
				/// <param name="parent">Owner or parent window, or default.</param>
				/// <param name="controlId">Control id or 0.</param>
				/// <remarks>
				/// Your derived class should override <see cref="WndProc"/>, which calls the window procedure of window class <paramref name="className"/>.
				/// The window will be destroyed in these cases: 1. Called <see cref="Destroy"/>. 2. Closed by the user or some program/script. 3. When this thread ends. 4. This function called again (then destroys old window and creates new).
				/// </remarks>
				public bool Create(string className, string name = null, uint style = 0, uint exStyle = 0, int x = 0, int y = 0, int width = 0, int height = 0, Wnd parent = default, LPARAM controlId = default)
				{
					Destroy();
					if(_wndProc == null) _wndProc = WndProc;

					if(!_Create(className, name, style, exStyle, x, y, width, height, parent, controlId)) return false;

					if(t_windows == null) t_windows = new List<MyWindow>();
					t_windows.Add(this);

					return true;
				}

				/// <summary>
				/// Creates <msdn>message-only window</msdn>.
				/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
				/// More info: <see cref="Create"/>.
				/// </summary>
				public bool CreateMessageWindow(string className)
				{
					return Create(className, null, Native.WS_POPUP, Native.WS_EX_NOACTIVATE, 0, 0, 0, 0, SpecHwnd.HWND_MESSAGE);
					//note: WS_EX_NOACTIVATE is important.
				}

				/// <summary>
				/// Destroys the window.
				/// Calls API <msdn>DestroyWindow</msdn>.
				/// Does nothing if the window is already destroyed, for example closed by the user.
				/// If the window is not destroyed explicitly, the system destroys it when its thread ends.
				/// </summary>
				public void Destroy()
				{
					if(Handle.Is0) return;
					Api.DestroyWindow(Handle);
					Debug.Assert(Handle == default && !t_windows.Contains(this));
				}

				/// <summary>
				/// Calls the window procedure of the window class (see <see cref="Create"/>) and manages the lifetime of this variable.
				/// Your derived class should override this function. Your WndProc must call it (base.WndProc) and return its return value, except when don't need default processing. Always call it on WM_NCCREATE and WM_NCDESTROY.
				/// More info: <msdn>Window Procedures</msdn>.
				/// </summary>
				/// <seealso cref="Wnd.Misc.PrintMsg(Wnd, uint, LPARAM, LPARAM, uint[])"/>
				public virtual LPARAM WndProc(Wnd w, uint message, LPARAM wParam, LPARAM lParam)
				{
					var R = _classWndProc == _defWindowProc
						? Api.DefWindowProc(w, message, wParam, lParam) //not necessary but presumably faster
						: Api.CallWindowProc(_classWndProc, w, message, wParam, lParam);

					if(message == Api.WM_NCDESTROY) {
						Handle = default;
						t_windows.Remove(this);
					}

					return R;
				}

				bool _Create(string className, string name, uint style, uint exStyle, int x, int y, int width, int height, Wnd parent, LPARAM controlId)
				{
					IntPtr hh = default;
					try {
						hh = Api.SetWindowsHookEx(Api.WH_CBT, (code, wParam, lParam) =>
						{
							Debug.Assert(code == Api.HCBT_CREATEWND);
							if(code == Api.HCBT_CREATEWND) {
								var ww = (Wnd)wParam;
								Debug.Assert(ww.ClassNameIs(className));
								_classWndProc = ww.SetWindowLong(Native.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_wndProc));
								Api.UnhookWindowsHookEx(hh); hh = default;
								return 0;
							}
							return Api.CallNextHookEx(default, code, wParam, lParam);
						}, default, Api.GetCurrentThreadId());
						Handle = CreateWindow(className, name, style, exStyle, x, y, width, height, parent, controlId);
					}
					finally { if(hh != default) Api.UnhookWindowsHookEx(hh); }
					return !Handle.Is0;

					//Replaces the window procedure (usually DefWindowProcW) with _wndProc.
					//	For it uses CBT hook. It is called before the system sends any messages to the window.
					//	_wndProc receives all messages, including WM_NCCREATE.
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
				/// Each class derived from MyWindow provides its own window procedure (overrides <see cref="WndProc"/>). Windows created not with <see cref="Create"/> use API <msdn>DefWindowProc</msdn> as window procedure, and therefore are not useful.
				/// The window class remains registered until this process ends. Don't need to unregister.
				/// This function can be called multiple times for the same class, for example called once in each appdomain. Next time it just returns class atom.
				/// Thread-safe.
				/// <note type="note">Don't use code like <c>static ushort _atom = Wnd.Misc.MyWindow.RegisterClass("MyClass");</c>, because in Release configuration compiler removes this code if _atom is not used. Instead you can call this function in a static constructor.</note>
				/// </remarks>
				public static unsafe ushort RegisterClass(string className, WndClassEx ex = null)
				{
					lock("jU0tLiIbtE6KWg5aCu7RDg") {
						string interDomainVarName = "jU0tLiIbtE6KWg5aCu7RDg" + className.ToLower_();
						if(!InterDomainVariables.GetVariable(interDomainVarName, out ushort atom)) {
							var x = new Api.WNDCLASSEX(ex);

							fixed (char* pCN = className) {
								x.lpszClassName = pCN;
								x.lpfnWndProc = _defWindowProc;
								x.style |= Api.CS_GLOBALCLASS;

								atom = Api.RegisterClassEx(ref x);
								if(atom == 0) throw new Win32Exception();
							}

							InterDomainVariables.SetVariable(interDomainVarName, atom);
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
