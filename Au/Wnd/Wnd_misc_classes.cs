//Wnd.Misc nested classes.

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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

#pragma warning disable 282 //intellisense bug: it thinks that Wnd has multiple fields.

namespace Au
{
	public partial struct Wnd
	{
		public static partial class Misc
		{
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

				/// <summary>
				/// Used by <see cref="SetProgressState"/>.
				/// </summary>
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
			/// Arranges windows, shows/hides desktop. The same as the taskbar right-click menu commands.
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

					WndShell.LibMinimalSleepIfOtherThread();
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
			/// </summary>
			/// <remarks>
			/// Usually each control has a unique name. It is useful to identify controls without a classic name/text.
			/// The control id of these controls is not useful, it is not constant.
			/// </remarks>
			public sealed class WinFormsControlNames :IDisposable
			{
				Process_.Memory _pm;
				Wnd _w;

				///
				public void Dispose()
				{
					if(_pm != null) { _pm.Dispose(); _pm = null; }
					GC.SuppressFinalize(this);
				}

				static readonly uint WM_GETCONTROLNAME = Api.RegisterWindowMessage("WM_GETCONTROLNAME");

				/// <summary>
				/// Prepares to get control names.
				/// </summary>
				/// <param name="w">Any top-level or child window of that process.</param>
				/// <exception cref="WndException">w invalid.</exception>
				/// <exception cref="AuException">Failed to allocate process memory (see <see cref="Process_.Memory"/>) needed to get control names, usually because of <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink>.</exception>
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
					if(!c.SendTimeout(5000, out var R, WM_GETCONTROLNAME, 4096, _pm.Mem) || (int)R < 1) return null;
					int len = (int)R - 1;
					if(len == 0) return "";
					return _pm.LibReadUnicodeStringCached(len);
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
