//AWnd.More nested classes.

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
using static Au.AStatic;

namespace Au
{
	public partial struct AWnd
	{
		public static partial class More
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
				public static void Flash(AWnd w, int count)
				{
					//const uint FLASHW_STOP = 0;
					//const uint FLASHW_CAPTION = 0x00000001;
					const uint FLASHW_TRAY = 0x00000002;
					//const uint FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY;
					//const uint FLASHW_TIMER = 0x00000004;
					//const uint FLASHW_TIMERNOFG = 0x0000000C;

					var fi = new Api.FLASHWINFO(); fi.cbSize = Api.SizeOf(fi); fi.hwnd = w;
					if(count > 0) {
						fi.uCount = count;
						//fi.dwTimeout = (uint)periodMS; //not useful
						fi.dwFlags = FLASHW_TRAY;
					}
					Api.FlashWindowEx(ref fi);

					//tested. FlashWindow is easier but does not work for taskbar button, only for caption when no taskbar button.
				}

				/// <summary>
				/// Sets the state of the progress indicator displayed on the taskbar button.
				/// Calls <msdn>ITaskbarList3.SetProgressState</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				/// <param name="state">Progress indicator state and color.</param>
				public static void SetProgressState(AWnd w, TBProgressState state)
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
				public static void SetProgressValue(AWnd w, int progressValue, int progressTotal = 100)
				{
					_TaskbarButton.taskbarInstance.SetProgressValue(w, progressValue, progressTotal);
				}

				/// <summary>
				/// Adds taskbar button.
				/// Calls <msdn>ITaskbarList.AddTab</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				public static void Add(AWnd w)
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
				public static void Delete(AWnd w)
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
						[PreserveSig] int AddTab(AWnd hwnd);
						[PreserveSig] int DeleteTab(AWnd hwnd);
						[PreserveSig] int ActivateTab(AWnd hwnd);
						[PreserveSig] int SetActiveAlt(AWnd hwnd);

						// ITaskbarList2
						[PreserveSig] int MarkFullscreenWindow(AWnd hwnd, bool fFullscreen);

						// ITaskbarList3
						[PreserveSig] int SetProgressValue(AWnd hwnd, long ullCompleted, long ullTotal);
						[PreserveSig] int SetProgressState(AWnd hwnd, TBProgressState state);
						[PreserveSig] int RegisterTab(AWnd hwndTab, AWnd hwndMDI);
						[PreserveSig] int UnregisterTab(AWnd hwndTab);
						[PreserveSig] int SetTabOrder(AWnd hwndTab, AWnd hwndInsertBefore);
						[PreserveSig] int SetTabActive(AWnd hwndTab, AWnd hwndMDI, uint dwReserved);
						[PreserveSig] int ThumbBarAddButtons(AWnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig] int ThumbBarUpdateButtons(AWnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig] int ThumbBarSetImageList(AWnd hwnd, IntPtr himl);
						[PreserveSig] int SetOverlayIcon(AWnd hwnd, IntPtr hIcon, string pszDescription);
						[PreserveSig] int SetThumbnailTooltip(AWnd hwnd, string pszTip);
						[PreserveSig] int SetThumbnailClip(AWnd hwnd, ref RECT prcClip);
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
						if(what == 1 && Active.IsMinimized) GetWnd.Desktop.ActivateLL();
					}
					catch { }

					GetWnd.Shell.LibMinimalSleepIfOtherThread();
				}
			}

			//FUTURE: use IVirtualDesktopManager to manage virtual desktops.
			//Currently almost not useful, because its MoveWindowToDesktop does not work with windows of other processes.
			//But in the future, if we'll have a dll to inject into another process, eg to find accessible objects faster, then also can add this to it.
			//The inteface also has IsWindowOnCurrentVirtualDesktop and GetWindowDesktopId.
			//Also there are internal/undocumented interfaces to add/remove/switch desktops etc. There is a GitHub library. And another library that injects.

			/// <summary>
			/// Makes easier to send and receive data to/from other processes using message <msdn>WM_COPYDATA</msdn>.
			/// </summary>
			/// <remarks>
			/// This struct is <msdn>COPYDATASTRUCT</msdn>.
			/// <note>By default [](xref:uac) blocks messages sent from processes of lower integrity level. Call <see cref="EnableReceivingWM_COPYDATA"/> if need.</note>
			/// </remarks>
			/// <seealso cref="System.IO.MemoryMappedFiles.MemoryMappedFile"/>
			/// <seealso cref="System.IO.Pipes.NamedPipeServerStream"/>
			public unsafe struct CopyDataStruct
			{
				//COPYDATASTRUCT fields
				LPARAM _dwData;
				int _cbData;
				void* _lpData;

				#region send

				/// <summary>
				/// Sends string to a window of another process using API <msdn>SendMessage</msdn>(<msdn>WM_COPYDATA</msdn>).
				/// </summary>
				/// <returns><b>SendMessage</b>'s return value.</returns>
				/// <param name="w">The window.</param>
				/// <param name="dataId">Data id. It is <msdn>COPYDATASTRUCT.dwData</msdn>.</param>
				/// <param name="s">Data. Can contain '\0' characters.</param>
				/// <param name="wParam">wParam of WM_COPYDATA. Optional.</param>
				public static LPARAM SendString(AWnd w, int dataId, string s, LPARAM wParam = default)
				{
					fixed (char* p = s) {
						var c = new CopyDataStruct { _dwData = dataId, _cbData = s.Length * 2, _lpData = p };
						return w.Send(Api.WM_COPYDATA, wParam, &c);
					}
				}

				/// <summary>
				/// Sends byte[] to a window of another process using API <msdn>SendMessage</msdn>(<msdn>WM_COPYDATA</msdn>).
				/// More info: <see cref="SendString"/>.
				/// </summary>
				public static unsafe LPARAM SendBytes(AWnd w, int dataId, byte[] a, LPARAM wParam = default)
				{
					fixed (byte* p = a) {
						var c = new CopyDataStruct { _dwData = dataId, _cbData = a.Length, _lpData = p };
						return w.Send(Api.WM_COPYDATA, wParam, &c);
					}
				}

				#endregion

				#region receive

				/// <summary>
				/// Initializes this variable from <i>lParam</i> of a received <msdn>WM_COPYDATA</msdn> message.
				/// Then you can call methods and properties of this variable to get data in managed format.
				/// </summary>
				/// <param name="lParam"><i>lParam</i> of a <msdn>WM_COPYDATA</msdn> message received in a window procedure. It is <msdn>COPYDATASTRUCT</msdn> pointer.</param>
				public CopyDataStruct(LPARAM lParam)
				{
					var p = (CopyDataStruct*)lParam;
					_dwData = p->_dwData; _cbData = p->_cbData; _lpData = p->_lpData;
				}

				/// <summary>
				/// Data id. It is <msdn>COPYDATASTRUCT.dwData</msdn>.
				/// </summary>
				public int DataId { get => (int)_dwData; set => _dwData = value; }

				/// <summary>
				/// Unmanaged data pointer. It is <msdn>COPYDATASTRUCT.lpData</msdn>.
				/// </summary>
				public void* RawData { get => _lpData; set => _lpData = value; }

				/// <summary>
				/// Unmanaged data size. It is <msdn>COPYDATASTRUCT.cbData</msdn>.
				/// </summary>
				public int RawDataSize { get => _cbData; set => _cbData = value; }

				/// <summary>
				/// Gets received data as string.
				/// </summary>
				public string GetString()
				{
					return new string((char*)_lpData, 0, _cbData / 2);
				}

				/// <summary>
				/// Gets received data as byte[].
				/// </summary>
				public byte[] GetBytes()
				{
					var a = new byte[_cbData];
					Marshal.Copy((IntPtr)_lpData, a, 0, a.Length);
					return a;
				}

				/// <summary>
				/// Calls API <msdn>ChangeWindowMessageFilter</msdn>(<b>WM_COPYDATA</b>). Then windows of this process can receive this message from lower [](xref:uac) integrity level processes.
				/// </summary>
				public static void EnableReceivingWM_COPYDATA()
				{
					Api.ChangeWindowMessageFilter(Api.WM_COPYDATA, 1);
				}

				#endregion
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Used by <see cref="AWnd.More.TaskbarButton.SetProgressState"/>.
	/// </summary>
	[NoDoc]
	public enum TBProgressState
	{
#pragma warning disable 1591 //XML doc
		NoProgress = 0,
		Indeterminate = 0x1,
		Normal = 0x2,
		Error = 0x4,
		Paused = 0x8
#pragma warning restore 1591 //XML doc
	}
}
