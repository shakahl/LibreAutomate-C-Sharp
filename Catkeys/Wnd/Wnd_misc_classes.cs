//Wnd.Misc nested classes.

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
using System.Collections.Concurrent;

using static Catkeys.NoClass;

#pragma warning disable 282 //C#7 intellisense bug: it thinks that Wnd has multiple fields.

namespace Catkeys
{
	public partial struct Wnd
	{
		public static partial class Misc
		{

			/// <summary>
			/// Registers and unregisters a window class.
			/// Example: <c>static Wnd.Misc.WindowClass _myWndProc = Wnd.Misc.WindowClass.Register("MyClass", _MyWndProc); ... Wnd w = MyNatives.CreateWindowEx(0, _myWndProc.Name, ...);</c>.
			/// </summary>
			public class WindowClass
			{
				private WindowClass() { } //disable '=new WindowClass()'

				/// <summary>
				/// Class atom.
				/// </summary>
				public ushort Atom { get; private set; }

				/// <summary>
				/// Actual class name that must be used to create windows.
				/// It is not exactly the same as passed to Create() etc. It has a suffix containing current appdomain identifer.
				/// </summary>
				public string Name { get => _className; }

				/// <summary>
				/// Base class extra memory size.
				/// </summary>
				public int BaseClassWndExtra { get; private set; }

				/// <summary>
				/// Base class window procedure to pass to API <msdn>CallWindowProc</msdn> in your window procedure.
				/// </summary>
				public Native.WNDPROC BaseClassWndProc { get; private set; }

				IntPtr _hinst; //need for Unregister()
				Native.WNDPROC _wndProc; //to keep reference to the caller's delegate to prevent garbage collection
				string _className; //for warning text in Unregister()

				///
				~WindowClass()
				{
					Unregister();
				}

				/// <summary>
				/// Registers new window class.
				/// Returns new WindowClass variable that holds class atom and other data.
				/// Calls API <msdn>RegisterClassEx</msdn>.
				/// </summary>
				/// <param name="className">Class name.</param>
				/// <param name="wndProc">Window procedure delegate. This variable will keep a reference to it to prevent garbage-collecting.</param>
				/// <param name="wndExtra">Size of extra window memory which can be accessed with SetWindowLong/GetWindowLong with >=0 offset. Example: IntPtr.Size.</param>
				/// <param name="style">Class style.</param>
				/// <param name="ex">
				/// Can be used to specify API <msdn>WNDCLASSEX</msdn> fields not specified in parameters.
				/// If not used, the function sets: hCursor = arrow; hbrBackground = COLOR_BTNFACE+1; others = 0/null/Zero.
				/// </param>
				/// <exception cref="Win32Exception">Failed, for example if the class already exists.</exception>
				/// <remarks>
				/// The actual class name is like "MyClass.2", where "MyClass" is className and "2" is current appdomain id. The <see cref="Name"/> property returns it.
				/// If style does not have CS_GLOBALCLASS and ex is null or its hInstance field is not set, for hInstance uses exe module handle.
				/// Not thread-safe.
				/// </remarks>
				public static WindowClass Register(string className, Native.WNDPROC wndProc, int wndExtra = 0, uint style = 0, WndClassEx ex = null)
				{
					var x = (ex != null) ? new Api.WNDCLASSEX(ex) : new Api.WNDCLASSEX(true);

					var r = new WindowClass();
					r._Register(ref x, className, wndProc, wndExtra, style);
					return r;

					//hInstance=Api.GetModuleHandle(null); //tested: RegisterClassEx uses this if hInstance is Zero, even for app-local classes

					//tested:
					//For app-global classes, CreateWindowEx and GetClassInfo ignore their hInst argument (always succeed).
					//For app-local classes, CreateWindowEx and GetClassInfo fail if their hInst argument does not match. However CreateWindowEx always succeeds if its hInst argument is Zero.
				}

				//static Dictionary<string, WindowClass> _a=new Dictionary<string, WindowClass>();

				unsafe void _Register(ref Api.WNDCLASSEX x, string className, Native.WNDPROC wndProc, int wndExtra, uint style)
				{
					//Add appdomain id to the class name. It solves 2 problems:
					//	1. Multiple domains cannot register exactly the same class name because they cannot use a common procedure.
					//	2. In Release build compiler completely removes code 'static Wnd.Misc.WindowClass _myWndProc = Wnd.Misc.WindowClass.Register("MyClass", _MyWndProc);' if _myWndProc is not referenced elsewhere; then the class is not registered when we create window; now programmers must use _myWndProc.Name with CreateWindowEx etc and it prevents removing the code.
					className = className + "." + AppDomain.CurrentDomain.Id;
					//Print(className);

					fixed (char* pCN = className) {
						x.cbSize = Api.SizeOf(x);
						x.lpszClassName = pCN;
						x.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);
						x.cbWndExtra = wndExtra;
						x.style = style;
						if(x.hInstance == Zero && (style & Api.CS_GLOBALCLASS) == 0) x.hInstance = Api.GetModuleHandle(null);

						Atom = Api.RegisterClassEx(ref x);
						if(Atom == 0) throw new Win32Exception();

						_hinst = x.hInstance; //if was set in ex, will need this for Unregister()
						_wndProc = wndProc; //keep the delegate from GC
						_className = className;
					}

					//_a.Add(className, this);
				}

				/// <summary>
				/// Registers new window class that extends an existing class.
				/// Returns new WindowClass variable that holds class atom and other data.
				/// Calls API <msdn>GetClassInfoEx</msdn> and API <msdn>RegisterClassEx</msdn>.
				/// </summary>
				/// <param name="baseClassName">Existing class name.</param>
				/// <param name="className">New class name.</param>
				/// <param name="wndProc">Window procedure delegate. This variable will keep a reference to it to prevent garbage-collecting.</param>
				/// <param name="wndExtra">Size of extra window memory not including extra memory of base class. Can be accessed with SetMyLong/GetMyLong. Example: IntPtr.Size.</param>
				/// <param name="globalClass">If false, the function removes CS_GLOBALCLASS style.</param>
				/// <param name="baseModuleHandle">If the base class is global (CS_GLOBALCLASS style), don't use this parameter, else pass the module handle of the exe or dll that registered the base class.</param>
				/// <exception cref="Win32Exception">Failed, for example if the class already exists or class baseClassName does not exist.</exception>
				/// <remarks>
				/// The actual class name is like "MyClass.2", where "MyClass" is className and "2" is current appdomain identifer. The <see cref="Name"/> property returns it.
				/// Not thread-safe.
				/// </remarks>
				public static WindowClass Superclass(string baseClassName, string className, Native.WNDPROC wndProc, int wndExtra = 0, bool globalClass = false, IntPtr baseModuleHandle = default(IntPtr))
				{
					var r = new WindowClass();
					var x = new Api.WNDCLASSEX();
					x.cbSize = Api.SizeOf(x);
					if(0 == Api.GetClassInfoEx(baseModuleHandle, baseClassName, ref x)) throw new Win32Exception();

					Native.WNDPROC wp = (Native.WNDPROC)Marshal.GetDelegateForFunctionPointer(x.lpfnWndProc, typeof(Native.WNDPROC));
					int we = x.cbWndExtra;

					r._Register(ref x, className, wndProc, x.cbWndExtra + wndExtra, globalClass ? x.style : x.style & ~Api.CS_GLOBALCLASS);

					r.BaseClassWndProc = wp;
					r.BaseClassWndExtra = we;
					return r;
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
						if(!ok) Output.Warning($"Failed to unregister window class '{_className}'. {Native.GetErrorMessage()}.");
						Debug.Assert(ok);
						Atom = 0;
						_hinst = Zero;
						BaseClassWndProc = null;
						BaseClassWndExtra = 0;
					}
				}

				/// <summary>
				/// Calls <see cref="SetWindowLong">SetWindowLong</see> and returns its return value.
				/// </summary>
				/// <param name="w">Window.</param>
				/// <param name="value">Value.</param>
				/// <param name="offset">Offset in extra memory, not including the size of extra memory of base class.</param>
				/// <exception cref="WndException"/>
				public LPARAM SetMyLong(Wnd w, LPARAM value, int offset = 0)
				{
					return w.SetWindowLong(BaseClassWndExtra + offset, value);
				}

				/// <summary>
				/// Calls <see cref="GetWindowLong">GetWindowLong</see> and returns its return value.
				/// </summary>
				/// <param name="w">Window.</param>
				/// <param name="offset">Offset in extra memory, not including the size of extra memory of base class.</param>
				public LPARAM GetMyLong(Wnd w, int offset = 0)
				{
					return w.GetWindowLong(BaseClassWndExtra + offset);
				}

				/// <summary>
				/// Calls API GetClassLong if current process is 32-bit, GetClassLongPtr if 64-bit.
				/// </summary>
				/// <remarks>
				/// All Native.GCL_/GCW_ values are the same in 32-bit and 64-bit process.
				/// Supports <see cref="Native.GetError"/>.
				/// </remarks>
				public static LPARAM GetClassLong(Wnd w, int index)
				{
					LPARAM R;
					if(IntPtr.Size == 8) R = _Api.GetClassLong64(w, index); else R = _Api.GetClassLong32(w, index);
					return R;
				}

				//Not useful. Dangerous.
				///// <summary>
				///// Calls API SetClassLong if this process is 32-bit, SetClassLongPtr if 64-bit.
				///// </summary>
				///// <remarks>
				///// All Native.GCL_ values are the same in 32-bit and 64-bit process.
				///// </remarks>
				///// <exception cref="WndException"/>
				//public static LPARAM SetClassLong(Wnd w, int index, LPARAM newValue)
				//{
				//	Native.ClearError();
				//	LPARAM R;
				//	if(IntPtr.Size == 8) R = _Api.SetClassLong64(w, index, newValue); else R = _Api.SetClassLong32(w, index, newValue);
				//	if(R == 0 && Native.GetError() != 0) ThrowUseNative();
				//	return R;
				//}

				/// <summary>
				/// Gets atom of a window class.
				/// To get class atom when you have a window w, use <c>Wnd.Misc.WindowClass.GetClassLong(w, Native.GCW_ATOM)</c>.
				/// </summary>
				/// <param name="className">Class name.</param>
				/// <param name="moduleHandle">Native module handle of the exe or dll that registered the class. Don't use if it is a global class (CS_GLOBALCLASS style).</param>
				public static ushort GetClassAtom(string className, IntPtr moduleHandle = default(IntPtr))
				{
					var x = new Api.WNDCLASSEX();
					x.cbSize = Api.SizeOf(x);
					return Api.GetClassInfoEx(moduleHandle, className, ref x);
				}

				/// <summary>
				/// Registers new window class that can be used by all app domains of this process.
				/// Returns class atom.
				/// Calls API <msdn>RegisterClassEx</msdn>.
				/// </summary>
				/// <param name="className">Class name.</param>
				/// <param name="wndProc">Window procedure. Can be classic window procedure that calls API <msdn>DefWindowProc</msdn>. Don't need to protect it from GC. If null, will use DefWindowProc as window procedure.</param>
				/// <param name="wndExtra">Size of extra window memory which can be accessed with SetWindowLong/GetWindowLong with >=0 offset. Example: IntPtr.Size.</param>
				/// <param name="style">Class style. Adds CS_GLOBALCLASS.</param>
				/// <param name="ex">
				/// Can be used to specify API WNDCLASSEX fields not specified in parameters.
				/// If not used, the function sets: hCursor = arrow; hbrBackground = COLOR_BTNFACE+1; others = 0/null/Zero.
				/// Always uses hInstance=Zero and adds style CS_GLOBALCLASS.
				/// </param>
				/// <exception cref="Win32Exception">Failed, for example if the class already exists and was registered not with this function.</exception>
				/// <remarks>
				/// This function can be called multiple times for the same class, for example called once in each appdomain. Next time it just returns class atom.
				/// Each appdomain that calls this function uses its own instance of window procedure delegate.
				/// The window class remains registered until this process ends. Don't need to unregister.
				/// Thread-safe.
				/// </remarks>
				public static unsafe ushort InterDomainRegister(string className, Native.WNDPROC wndProc, int wndExtra = 0, uint style = 0, WndClassEx ex = null)
				{
					lock("jU0tLiIbtE6KWg5aCu7RDg") {
						string interDomainVarName = "jU0tLiIbtE6KWg5aCu7RDg" + className.ToLower_();
						if(!InterDomain.GetVariable(interDomainVarName, out ushort atom)) {
							var x = (ex != null) ? new Api.WNDCLASSEX(ex) : new Api.WNDCLASSEX(true);

							fixed (char* pCN = className) {
								x.cbSize = Api.SizeOf(x);
								x.lpszClassName = pCN;
								x.lpfnWndProc = Api.GetProcAddress("user32", "DefWindowProcW");
								x.cbWndExtra = wndExtra;
								x.style = style | Api.CS_GLOBALCLASS;
								x.hInstance = Zero;

								atom = Api.RegisterClassEx(ref x);
								if(atom == 0) throw new Win32Exception();
							}

							InterDomain.SetVariable(interDomainVarName, atom);
						}
						_interDomain[className] = wndProc;
						return atom;
					}
				}

				//className/wndProc of classes registered with InterDomainRegister in this appdomain.
				//InterDomainCreateWindow() uses it to get wndProc. Also it protects wndProc from GC.
				static ConcurrentDictionary<string, Native.WNDPROC> _interDomain = new ConcurrentDictionary<string, Native.WNDPROC>(StringComparer.OrdinalIgnoreCase);

				/// <summary>
				/// Creates new window of a class registered with <see cref="InterDomainRegister"/>.
				/// All parameters are the same as with API <msdn>CreateWindowEx</msdn>.
				/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
				/// </summary>
				/// <remarks>
				/// The window procedure does not receive messages until this function returns. It never receives creation messages, eg WM_CREATE.
				/// </remarks>
				public static Wnd InterDomainCreateWindow(uint exStyle, string className, string name, uint style, int x, int y, int width, int height, Wnd parent = default(Wnd), LPARAM controlId = default(LPARAM))
				{
					if(!_interDomain.TryGetValue(className, out Native.WNDPROC wndProc)) return default(Wnd);
					Wnd w = Misc.CreateWindow(exStyle, className, name, style, x, y, width, height, parent, controlId);
					if(!w.Is0 && wndProc != null) w.SetWindowLong(Native.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProc));
					return w;
				}

				/// <summary>
				/// Creates native/unmanaged <msdn>message-only window</msdn> of a class registered with <see cref="InterDomainRegister"/>.
				/// Styles: WS_POPUP, WS_EX_NOACTIVATE.
				/// Later call <see cref="DestroyWindow"/> or <see cref="Close"/>.
				/// </summary>
				public static Wnd InterDomainCreateMessageWindow(string className)
				{
					return InterDomainCreateWindow(Native.WS_EX_NOACTIVATE, className, null, Native.WS_POPUP, 0, 0, 0, 0, SpecHwnd.HWND_MESSAGE);
					//note: WS_EX_NOACTIVATE is important.
				}

				/// <summary>
				/// Rarely used API <msdn>WNDCLASSEX</msdn> fields. Used with some WindowClass functions.
				/// </summary>
				/// <tocexclude />
				public class WndClassEx
				{
#pragma warning disable 1591 //XML doc
					public int cbClsExtra;
					public IntPtr hInstance;
					public IntPtr hIcon;
					public IntPtr hCursor;
					public IntPtr hbrBackground;
					public IntPtr hIconSm;
#pragma warning restore 1591 //XML doc
				}
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

				/// <tocexclude />
				public enum ProgressState
				{
#pragma warning disable 1591 //XML doc
					NoProgress = 0,
					Indeterminate = 0x1,
					Normal = 0x2,
					Error = 0x4,
					Paused = 0x8
#pragma warning restore 1591 //XML doc
				}

				/// <summary>
				/// Sets the state of the progress indicator displayed on the taskbar button.
				/// Calls <msdn>ITaskbarList3.SetProgressState</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				/// <param name="state">Progress indicator state and color.</param>
				public static void SetProgressState(Wnd w, ProgressState state)
				{
					_TaskbarButton.taskbarInstance.SetProgressState(w, state);
				}

				/// <summary>
				/// Sets the value of the progress indicator displayed on the taskbar button.
				/// Calls <msdn>ITaskbarList3.SetProgressValue</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				/// <param name="progressValue">Progress indicator value, 0 to progressTotal.</param>
				/// <param name="progressTotal">Max progress indicator value.</param>
				public static void SetProgressValue(Wnd w, int progressValue, int progressTotal = 100)
				{
					_TaskbarButton.taskbarInstance.SetProgressValue(w, progressValue, progressTotal);
				}

				/// <summary>
				/// Adds taskbar button.
				/// Calls <msdn>ITaskbarList.AddTab</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				public static void Add(Wnd w)
				{
					_TaskbarButton.taskbarInstance.AddTab(w);
					//info: succeeds without HrInit(), tested on Win10 and 7.
					//info: always returns 0, even if w is 0. Did not test ITaskbarList3 methods.
				}

				/// <summary>
				/// Deletes taskbar button.
				/// Calls <msdn>ITaskbarList.DeleteTab</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				public static void Delete(Wnd w)
				{
					_TaskbarButton.taskbarInstance.DeleteTab(w);
				}

				static class _TaskbarButton
				{
					[ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
					internal interface ITaskbarList3
					{
						// ITaskbarList
						[PreserveSig] int HrInit();
						[PreserveSig] int AddTab(Wnd hwnd);
						[PreserveSig] int DeleteTab(Wnd hwnd);
						[PreserveSig] int ActivateTab(Wnd hwnd);
						[PreserveSig] int SetActiveAlt(Wnd hwnd);

						// ITaskbarList2
						[PreserveSig] int MarkFullscreenWindow(Wnd hwnd, bool fFullscreen);

						// ITaskbarList3
						[PreserveSig] int SetProgressValue(Wnd hwnd, long ullCompleted, long ullTotal);
						[PreserveSig] int SetProgressState(Wnd hwnd, ProgressState state);
						[PreserveSig] int RegisterTab(Wnd hwndTab, Wnd hwndMDI);
						[PreserveSig] int UnregisterTab(Wnd hwndTab);
						[PreserveSig] int SetTabOrder(Wnd hwndTab, Wnd hwndInsertBefore);
						[PreserveSig] int SetTabActive(Wnd hwndTab, Wnd hwndMDI, uint dwReserved);
						[PreserveSig] int ThumbBarAddButtons(Wnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig] int ThumbBarUpdateButtons(Wnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig] int ThumbBarSetImageList(Wnd hwnd, IntPtr himl);
						[PreserveSig] int SetOverlayIcon(Wnd hwnd, IntPtr hIcon, string pszDescription);
						[PreserveSig] int SetThumbnailTooltip(Wnd hwnd, string pszTip);
						[PreserveSig] int SetThumbnailClip(Wnd hwnd, ref RECT prcClip);
					}

					[ComImport]
					[Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
					[ClassInterface(ClassInterfaceType.None)]
					class TaskbarInstance
					{
					}

					internal static ITaskbarList3 taskbarInstance = (ITaskbarList3)new TaskbarInstance();
				}
			}

			/// <summary>
			/// Arranges windows, shows/hides desktop.
			/// The same as the taskbar right-click menu commands.
			/// </summary>
			public static class Desktop
			{
				/// <summary>
				/// Shows or hides desktop.
				/// If there are non-minimized main windows, minimizes them. Else restores windows recently minimized by this function.
				/// </summary>
				public static void ToggleShowDesktop()
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
				public static void UndoMinimizeEtc()
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

						//The COM object does not do exactly the same as the true Explorer commands.
						//Eg MinimizeAll does not activete desktop. Then a minimized window is active.
						if(what == 1 && WndActive.IsMinimized) WndDesktop.ActivateLL();
					}
					catch { }

					WndShell._MinimalWaitIfOtherThread();
				}
			}

			//Better use Desktop.ToggleShowDesktop etc.
			///// <summary>
			///// Minimizes or restores main windows, to show or hide desktop.
			///// </summary>
			///// <param name="on">
			///// If omitted or null, calls <see cref="Misc.Desktop.ToggleShowDesktop"/>, which shows or hides desktop.
			///// If true, calls <see cref="Misc.Desktop.MinimizeWindows"/>, which minimizes main windows.
			///// If false, calls <see cref="Misc.Desktop.UndoMinimizeEtc"/>, which restores windows recently minimized by this function.
			///// </param>
			//public static void ShowDesktop(bool? on = null)
			//{
			//	if(on == null) Misc.Desktop.ToggleShowDesktop();
			//	else if(on.Value) Misc.Desktop.MinimizeWindows();
			//	else Misc.Desktop.UndoMinimizeEtc();
			//}

			//FUTURE: use IVirtualDesktopManager to manage virtual desktops.
			//Currently almost not useful, because its MoveWindowToDesktop does not work with windows of other processes.
			//But in the future, if we'll have a dll to inject into another process, eg to find accessible objects faster, then also can add this to it.
			//The inteface also has IsWindowOnCurrentVirtualDesktop and GetWindowDesktopId.
			//Also there are internal/undocumented interfaces to add/remove/switch desktops etc. There is a GitHub library. And another library that injects.

			/// <summary>
			/// Gets programming names of .NET Windows Forms controls.
			/// Usually each control has a unique name. It is useful to identify controls without a classic name/text.
			/// Control id of these controls is not constant and cannot be used.
			/// </summary>
			public class WinFormsControlNames :IDisposable
			{
				Process_.Memory _pm;
				Wnd _w;

				#region IDisposable Support

				void _Dispose()
				{
					if(_pm != null) { _pm.Dispose(); _pm = null; }
				}

				///
				~WinFormsControlNames() { _Dispose(); }
				///
				public void Dispose()
				{
					_Dispose();
					GC.SuppressFinalize(this);
				}

				#endregion

				static readonly uint WM_GETCONTROLNAME = Api.RegisterWindowMessage("WM_GETCONTROLNAME");

				/// <summary>
				/// Prepares to get control names.
				/// </summary>
				/// <param name="w">Any top-level or child window of that process.</param>
				/// <exception cref="WndException">w invalid.</exception>
				/// <exception cref="CatException">Failed to allocate process memory (see <see cref="Process_.Memory"/>) needed to get control names, usually because of UAC.</exception>
				public WinFormsControlNames(Wnd w)
				{
					_pm = new Process_.Memory(w, 4096); //throws
					_w = w;
				}

				/// <summary>
				/// Gets control name.
				/// Returns null if fails or the name is empty.
				/// </summary>
				/// <param name="c">The control. Can be a top-level window too. Must be of the same process as the window specified in the constructor.</param>
				public string GetControlName(Wnd c)
				{
					if(_pm == null) return null;
					if(!IsWinFormsControl(c)) return null;
					if(!c.SendTimeout(5000, out var R, WM_GETCONTROLNAME, 4096, _pm.Mem) || R < 1) return null;
					return _pm.ReadUnicodeString(R);
				}

				/// <summary>
				/// Returns true if window class name starts with "WindowsForms".
				/// Usually it means that we can get Windows Forms control name of w and its child controls.
				/// </summary>
				/// <param name="w">The window. Can be top-level or control.</param>
				public static bool IsWinFormsControl(Wnd w)
				{
					return w.ClassNameIs("WindowsForms*");
				}

				/// <summary>
				/// Gets the programming name of a Windows Forms control.
				/// Returns null if it is not a Windows Forms control or if fails.
				/// </summary>
				/// <param name="c">The control. Can be top-level window too.</param>
				/// <remarks>This function is easy to use and does not throw excaptions. However, when you need names of multiple controls of a single window, better create a WinFormsControlNames instance (once) and for each control call its GetControlNameOrText method, it will be faster.</remarks>
				public static string GetSingleControlName(Wnd c)
				{
					if(!IsWinFormsControl(c)) return null;
					try {
						using(var x = new WinFormsControlNames(c)) { return x.GetControlName(c); }
					}
					catch { }
					return null;
				}

				//Don't use this cached version, it does not make significantly faster. Also, keeping process handle in such a way is not good, would need to use other thread to close it after some time.
				///// <summary>
				///// Gets programming name of a Windows Forms control.
				///// Returns null if it is not a Windows Forms control or if fails.
				///// </summary>
				///// <param name="c">The control. Can be top-level window too.</param>
				///// <remarks>When need to get control names repeatedly or quite often, this function can be faster than creating WinFormsControlNames instance each time and calling its GetControlNameOrText method, because this function remembers the last used process etc. Also it is easier to use and does not throw exceptions.</remarks>
				//public static string GetSingleControlName(Wnd c)
				//{
				//	if(!IsWinFormsControl(c)) return null;
				//	uint pid = c.ProcessId; if(pid == 0) return null;
				//	lock (_prevLock) {
				//		if(pid != _prevPID || Time.Milliseconds - _prevTime > 1000) {
				//			if(_prev != null) { _prev.Dispose(); _prev = null; }
				//			try { _prev = new WinFormsControlNames(c); } catch { }
				//			//Print("new");
				//		} //else Print("cached");
				//		_prevPID = pid; _prevTime = Time.Milliseconds;
				//		if(_prev == null) return null;
				//		return _prev.GetControlNameOrText(c);
				//	}
				//}
				//static WinFormsControlNames _prev; static uint _prevPID; static long _prevTime; static object _prevLock = new object(); //cache
			}
		}
	}
}
