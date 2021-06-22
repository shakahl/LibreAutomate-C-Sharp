//wnd.more nested classes.

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
//using System.Linq;

using Au.Types;
using Au.More;
using System.Buffers;

namespace Au
{
	public partial struct wnd
	{
		public static partial class more
		{
			/// <summary>
			/// Helps to save and restore window rectangle and state. Ensures in screen, per-monitor-DPI-aware, etc.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// class Form9 : Form {
			/// 	const string c_rkey = @"HKEY_CURRENT_USER\Software\Au\Test", c_rvalue = @"Form9.Rect";
			/// 	
			/// 	public Form9() {
			/// 		this.StartPosition = FormStartPosition.Manual;
			/// 		if (wnd.more.SavedRect.FromString(Registry.GetValue(c_rkey, c_rvalue, null) as string, out var v)) {
			/// 			this.Bounds = v.NormalizeRect();
			/// 			if(v.Maximize) this.WindowState = FormWindowState.Maximized;
			/// 		}
			/// 		//the same:
			/// 		//wnd.more.SavedRect.Restore(this, Registry.GetValue(c_rkey, c_rvalue, null) as string);
			/// 	}
			/// 
			/// 	protected override void OnFormClosing(FormClosingEventArgs e) {
			/// 		Registry.SetValue(c_rkey, c_rvalue, new wnd.more.SavedRect(this).ToString());
			/// 		base.OnFormClosing(e);
			/// 	}
			/// }
			/// ]]></code>
			/// The same in 2 lines.
			/// <code><![CDATA[
			/// class Form8 : Form {
			/// 	const string c_rkey = @"HKEY_CURRENT_USER\Software\Au\Test", c_rvalue = @"Form8.Rect";
			/// 	
			/// 	public Form8() {
			/// 		wnd.more.SavedRect.Restore(this, Registry.GetValue(c_rkey, c_rvalue, null) as string, s1 => Registry.SetValue(c_rkey, c_rvalue, s1.ToString()));
			/// 	}
			/// }
			/// ]]></code>
			/// WPF window created with <see cref="wpfBuilder"/>.
			/// <code><![CDATA[
			/// const string c_rkey = @"HKEY_CURRENT_USER\Software\Au\Test", c_rvalue = @"Wpf7.Rect";
			/// var b = new wpfBuilder("Window").WinSize(400).R.AddOkCancel().End();
			/// 	
			/// wnd.more.SavedRect.Restore(b.Window, Registry.GetValue(c_rkey, c_rvalue, null) as string, s1 => Registry.SetValue(c_rkey, c_rvalue, s1));
			/// 
			/// //the same
			/// //b.WinSaved(Registry.GetValue(c_rkey, c_rvalue, null) as string, s1 => Registry.SetValue(c_rkey, c_rvalue, s1));
			/// 
			/// if (!b.ShowDialog()) return;
			/// ]]></code>
			/// </example>
			public struct SavedRect
			{
				/// <summary>
				/// Window rectangle in normal state (not maximized/minimized), as retrieved by API <msdn>GetWindowPlacement</msdn>.
				/// </summary>
				public RECT RawRect { get => _r; set => _r = value; }
				RECT _r;

				/// <summary>
				/// <see cref="Dpi.OfWindow"/>.
				/// </summary>
				public int Dpi { get; set; }

				/// <summary>
				/// The window should be maximized.
				/// </summary>
				public bool Maximize { get; set; }

				/// <summary>
				/// <see cref="wnd.IsToolWindow"/>. If false, <see cref="RawRect"/> may have an offset that depends on work area.
				/// </summary>
				public bool IsToolWindow { get; set; }

				/// <summary>
				/// Converts this object to string for saving.
				/// The string is very simple, like "1 2 3 4 5 6".
				/// </summary>
				public override string ToString() {
					return $"{_r.left} {_r.top} {_r.Width} {_r.Height} {Dpi} {(Maximize ? 1 : 0) | (IsToolWindow ? 2 : 0)}";
				}

				/// <summary>
				/// Creates <b>SavedRect</b> from string created by <see cref="ToString"/>.
				/// Returns false if the string is null or invalid.
				/// </summary>
				/// <param name="saved">String created by <see cref="ToString"/>.</param>
				/// <param name="x">Result.</param>
				public static bool FromString(string saved, out SavedRect x) {
					x = default;
					if (saved == null) return false;
					var a = new int[6];
					for (int i = 0, j = 0; i < a.Length; i++) if (!saved.ToInt(out a[i], j, out j)) return false;
					x._r = (a[0], a[1], a[2], a[3]);
					x.Dpi = a[4];
					var flags = a[5];
					x.Maximize = 0 != (flags & 1);
					x.IsToolWindow = 0 != (flags & 2);
					return true;
				}

				/// <summary>
				/// Gets window rectangle and state for saving. Usually called when closing the window.
				/// See also <see cref="ToString"/>.
				/// </summary>
				/// <exception cref="AuWndException">Failed to get rectangle, probably invalid window handle.</exception>
				public SavedRect(wnd w) {
					if (!w.GetWindowPlacement_(out var p, false)) w.ThrowUseNative();
					_r = p.rcNormalPosition;
					Dpi = More.Dpi.OfWindow(w);
					Maximize = p.showCmd == Api.SW_SHOWMAXIMIZED || (p.showCmd == Api.SW_SHOWMINIMIZED && 0 != (p.flags & Api.WPF_RESTORETOMAXIMIZED));
					IsToolWindow = w.IsToolWindow;
				}

				/// <summary>
				/// Gets window rectangle and state for saving. Usually called when closing the window.
				/// See also <see cref="ToString"/>.
				/// </summary>
				/// <exception cref="AuWndException">Failed to get rectangle, probably invalid window handle.</exception>
				public SavedRect(System.Windows.Forms.Form form) : this(form.Hwnd()) { }
				//public SavedRect(Form form) { //rejected
				//	_r = form.RestoreBounds;
				//	Dpi = screen.of(RawRect).Dpi;
				//	Maximize = form.WindowState == FormWindowState.Maximized;
				//	IsToolWindow = form.FormBorderStyle == FormBorderStyle.;
				//}

				/// <summary>
				/// Gets window rectangle and state for saving. Usually called when closing the window.
				/// See also <see cref="ToString"/>.
				/// </summary>
				/// <exception cref="AuWndException">Failed to get rectangle, probably invalid window handle.</exception>
				public SavedRect(System.Windows.Window w) : this(w.Hwnd()) { }

				/// <summary>
				/// Gets real rectangle for restoring saved window rectangle.
				/// </summary>
				/// <remarks>
				/// It is recommended to call this before creating window, and create window with the returned rectangle. Also set maximized state if <see cref="Maximize"/>.
				/// If it is not possible, can be called later, for example when window is created but still invisible. However then possible various problems, for example may need to set window rectangle two times, because the window may be for example DPI-scaled when moving to another screen etc.
				/// 
				/// This function ensures the window is in screen, ensures correct size when screen DPI changed, etc.
				/// </remarks>
				public RECT NormalizeRect() {
					var r = _r;
					var scr = screen.of(r);
					int dpi = scr.Dpi;
					if (dpi != this.Dpi) {
						r.Width = Math2.MulDiv(r.Width, dpi, this.Dpi);
						r.Height = Math2.MulDiv(r.Height, dpi, this.Dpi);
						//don't change xy. Anyway we cannot cover all cases, eg changed DPI of another screen that could affect xy of the window in this screen.
					}
					if (!IsToolWindow) {
						var v = scr.Info;
						r.Offset(v.workArea.left - v.rect.left, v.workArea.top - v.rect.top);
					}
					r.EnsureInScreen(scr, !IsToolWindow); //SHOULDDO: use simple rect adjust. Or add EnsureInRect.
					return r;
				}

				/// <summary>
				/// Calls <see cref="FromString"/>. If it returns true, sets <i>form</i> bounds = <see cref="NormalizeRect"/>, maximizes if need, StartPosition=Manual, and returns true.
				/// Call this function before showing form, for example in constructor.
				/// </summary>
				/// <param name="form"></param>
				/// <param name="saved">String created by <see cref="ToString"/>.</param>
				/// <param name="save">If not null, called when closing the window. Receives string for saving. Can save it in registry, file, anywhere.</param>
				public static bool Restore(System.Windows.Forms.Form form, string saved, Action<string> save = null) {
					bool ret = FromString(saved, out var v);
					if (ret) {
						form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
						form.Bounds = v.NormalizeRect();
						if (v.Maximize) form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
					}
					if (save != null) {
						form.FormClosing += (o, _) => save(new SavedRect(o as System.Windows.Forms.Form).ToString());
					}
					return ret;
				}

				/// <summary>
				/// Calls <see cref="FromString"/>. If it returns true, calls <see cref="NormalizeRect"/>, <see cref="ExtWpf.SetRect"/>, maximizes if need and returns true.
				/// Call this function before showing window.
				/// </summary>
				/// <param name="w"></param>
				/// <param name="saved">String created by <see cref="ToString"/>.</param>
				/// <param name="save">If not null, called when closing the window. Receives string for saving. Can save it in registry, file, anywhere.</param>
				/// <exception cref="InvalidOperationException">Window is loaded.</exception>
				public static bool Restore(System.Windows.Window w, string saved, Action<string> save = null) {
					if (w.IsLoaded) throw new InvalidOperationException("Window is loaded.");
					bool ret = FromString(saved, out var v);
					if (ret) {
						var r = v.NormalizeRect();
						if (v.Maximize) w.WindowState = System.Windows.WindowState.Maximized;
						w.SetRect(r);
					}
					if (save != null) {
						w.Closing += (o, _) => save(new SavedRect(o as System.Windows.Window).ToString());
					}
					return ret;
				}
			}

			/// <summary>
			/// Taskbar button flash, progress, add/delete.
			/// </summary>
			public static class taskbarButton
			{
				/// <summary>
				/// Starts or stops flashing the taskbar button.
				/// </summary>
				/// <param name="w">Window.</param>
				/// <param name="count">The number of times to flash. If 0, stops flashing.</param>
				public static void flash(wnd w, int count) {
					//const uint FLASHW_STOP = 0;
					//const uint FLASHW_CAPTION = 0x00000001;
					const uint FLASHW_TRAY = 0x00000002;
					//const uint FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY;
					//const uint FLASHW_TIMER = 0x00000004;
					//const uint FLASHW_TIMERNOFG = 0x0000000C;

					var fi = new Api.FLASHWINFO(); fi.cbSize = Api.SizeOf(fi); fi.hwnd = w;
					if (count > 0) {
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
				public static void setProgressState(wnd w, WTBProgressState state) {
					_TaskbarButton.taskbarInstance.SetProgressState(w, state);
				}

				/// <summary>
				/// Sets the value of the progress indicator displayed on the taskbar button.
				/// Calls <msdn>ITaskbarList3.SetProgressValue</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				/// <param name="progressValue">Progress indicator value, 0 to progressTotal.</param>
				/// <param name="progressTotal">Max progress indicator value.</param>
				public static void setProgressValue(wnd w, int progressValue, int progressTotal = 100) {
					_TaskbarButton.taskbarInstance.SetProgressValue(w, progressValue, progressTotal);
				}

				/// <summary>
				/// Adds taskbar button.
				/// Calls <msdn>ITaskbarList.AddTab</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				public static void add(wnd w) {
					_TaskbarButton.taskbarInstance.AddTab(w);
					//info: succeeds without HrInit(), tested on Win10 and 7.
					//info: always returns 0, even if w is 0. Did not test ITaskbarList3 methods.
				}

				/// <summary>
				/// Deletes taskbar button.
				/// Calls <msdn>ITaskbarList.DeleteTab</msdn>.
				/// </summary>
				/// <param name="w">Button's window.</param>
				public static void delete(wnd w) {
					_TaskbarButton.taskbarInstance.DeleteTab(w);
				}

				static class _TaskbarButton
				{
					[ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
					internal interface ITaskbarList3
					{
						// ITaskbarList
						[PreserveSig] int HrInit();
						[PreserveSig] int AddTab(wnd hwnd);
						[PreserveSig] int DeleteTab(wnd hwnd);
						[PreserveSig] int ActivateTab(wnd hwnd);
						[PreserveSig] int SetActiveAlt(wnd hwnd);

						// ITaskbarList2
						[PreserveSig] int MarkFullscreenWindow(wnd hwnd, bool fFullscreen);

						// ITaskbarList3
						[PreserveSig] int SetProgressValue(wnd hwnd, long ullCompleted, long ullTotal);
						[PreserveSig] int SetProgressState(wnd hwnd, WTBProgressState state);
						[PreserveSig] int RegisterTab(wnd hwndTab, wnd hwndMDI);
						[PreserveSig] int UnregisterTab(wnd hwndTab);
						[PreserveSig] int SetTabOrder(wnd hwndTab, wnd hwndInsertBefore);
						[PreserveSig] int SetTabActive(wnd hwndTab, wnd hwndMDI, uint dwReserved);
						[PreserveSig] int ThumbBarAddButtons(wnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig] int ThumbBarUpdateButtons(wnd hwnd, uint cButtons, IntPtr pButton); //LPTHUMBBUTTON
						[PreserveSig] int ThumbBarSetImageList(wnd hwnd, IntPtr himl);
						[PreserveSig] int SetOverlayIcon(wnd hwnd, IntPtr hIcon, string pszDescription);
						[PreserveSig] int SetThumbnailTooltip(wnd hwnd, string pszTip);
						[PreserveSig] int SetThumbnailClip(wnd hwnd, ref RECT prcClip);
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
			public static class desktop
			{
				/// <summary>
				/// Shows or hides desktop.
				/// If there are non-minimized main windows, minimizes them. Else restores windows recently minimized by this function.
				/// </summary>
				public static void toggleShowDesktop() => _Do(0);

				/// <summary>
				/// Minimizes main windows.
				/// </summary>
				public static void minimizeWindows() => _Do(1);

				/// <summary>
				/// Cascades non-minimized main windows.
				/// </summary>
				public static void cascadeWindows() => _Do(3);

				/// <summary>
				/// Arranges non-minimized main windows horizontally or vertically.
				/// </summary>
				public static void tileWindows(bool vertically) => _Do(vertically ? 5 : 4);

				/// <summary>
				/// Restores windows recently minimized, cascaded or tiled with other functions of this class.
				/// </summary>
				public static void undoMinimizeEtc() => _Do(2);

				static void _Do(int what) {
					try {
						dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application")); //speed: faster than calling a method
						switch (what) {
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
						if (what == 1 && active.IsMinimized && getwnd.desktop(out var wd, out _)) wd.ActivateL();
					}
					catch { }

					getwnd.shellWindow.MinimalSleepIfOtherThread_();
				}
			}

			//FUTURE: use IVirtualDesktopManager to manage virtual desktops.
			//Currently almost not useful, because its MoveWindowToDesktop does not work with windows of other processes.
			//But in the future, if we'll have a dll to inject into another process, eg to find UI elements faster, then also can add this to it.
			//The inteface also has IsWindowOnCurrentVirtualDesktop and GetWindowDesktopId.
			//Also there are internal/undocumented interfaces to add/remove/switch desktops etc. There is a GitHub library. And another library that injects.

			/// <summary>
			/// Send/receive data to/from other process using message <msdn>WM_COPYDATA</msdn>.
			/// </summary>
			/// <remarks>
			/// This struct is <msdn>COPYDATASTRUCT</msdn>.
			/// <note>By default [](xref:uac) blocks messages sent from processes of lower integrity level. Call <see cref="EnableReceivingWM_COPYDATA"/> if need.</note>
			/// </remarks>
			/// <seealso cref="System.IO.MemoryMappedFiles.MemoryMappedFile"/>
			/// <seealso cref="System.IO.Pipes.NamedPipeServerStream"/>
			public unsafe struct CopyData
			{
				//COPYDATASTRUCT fields
				nint _dwData;
				int _cbData;
				byte* _lpData;

				#region receive

				/// <summary>
				/// Initializes this variable from <i>lParam</i> of a received <msdn>WM_COPYDATA</msdn> message.
				/// Then you can call functions of this variable to get data in managed format.
				/// </summary>
				/// <param name="lParam"><i>lParam</i> of a <msdn>WM_COPYDATA</msdn> message received in a window procedure. It is <msdn>COPYDATASTRUCT</msdn> pointer.</param>
				public CopyData(nint lParam) {
					var p = (CopyData*)lParam;
					_dwData = p->_dwData; _cbData = p->_cbData; _lpData = p->_lpData;
				}

				/// <summary>
				/// Data id. It is <msdn>COPYDATASTRUCT.dwData</msdn>.
				/// </summary>
				public int DataId { get => (int)_dwData; set => _dwData = value; }

				/// <summary>
				/// Unmanaged data pointer. It is <msdn>COPYDATASTRUCT.lpData</msdn>.
				/// </summary>
				public byte* RawData { get => _lpData; set => _lpData = value; }

				/// <summary>
				/// Unmanaged data size. It is <msdn>COPYDATASTRUCT.cbData</msdn>.
				/// </summary>
				public int RawDataSize { get => _cbData; set => _cbData = value; }

				/// <summary>
				/// Gets received data as string.
				/// </summary>
				public string GetString() {
					return new string((char*)_lpData, 0, _cbData / 2);
				}

				/// <summary>
				/// Gets received data as byte[].
				/// </summary>
				public byte[] GetBytes() {
					var a = new byte[_cbData];
					Marshal.Copy((IntPtr)_lpData, a, 0, a.Length);
					return a;
				}

				/// <summary>
				/// Calls API <msdn>ChangeWindowMessageFilter</msdn>(<b>WM_COPYDATA</b>). Then windows of this process can receive this message from lower [](xref:uac) integrity level processes.
				/// </summary>
				public static void EnableReceivingWM_COPYDATA() {
					Api.ChangeWindowMessageFilter(Api.WM_COPYDATA, 1);
				}

				#endregion

				#region send

				/// <summary>
				/// Sends string or other data to a window of any process. Uses API <msdn>SendMessage</msdn> <msdn>WM_COPYDATA</msdn>.
				/// </summary>
				/// <typeparam name="T">Type of data elements. For example, char for string, byte for byte[].</typeparam>
				/// <param name="w">The window.</param>
				/// <param name="dataId">Data id. It is <msdn>COPYDATASTRUCT.dwData</msdn>.</param>
				/// <param name="data">Data. For example string or byte[]. String can contain '\0' characters.</param>
				/// <param name="wParam">wParam. Can be any value. Optional.</param>
				/// <returns><b>SendMessage</b>'s return value.</returns>
				public static unsafe nint Send<T>(wnd w, int dataId, ReadOnlySpan<T> data, nint wParam = 0) where T : unmanaged {
					fixed (T* p = data) {
						var c = new CopyData { _dwData = dataId, _cbData = data.Length * sizeof(T), _lpData = (byte*)p };
						return w.Send(Api.WM_COPYDATA, wParam, &c);
					}
				}

				/// <summary>
				/// Type of <see cref="SendReceive{TSend, TReceive}(wnd, int, ReadOnlySpan{TSend}, ResultReader{TReceive})"/> callback function.
				/// </summary>
				/// <param name="span">Received data buffer. The callback function can convert it to array, string, etc.</param>
				public delegate void ResultReader<TReceive>(ReadOnlySpan<TReceive> span) where TReceive : unmanaged;
				//compiler error if Action<ReadOnlySpan<TReceive>>.
				//could instead use System.Buffers.ReadOnlySpanAction, but then need TState, which is difficult to use for return, and nobody would use, and would not make faster etc.

				static readonly Lazy<IntPtr> s_mutex = new(Api.CreateMutex(null, false, "Au-mutex-wnd.more.Data"));
				//TODO: maybe need Api.SECURITY_ATTRIBUTES.ForLowIL

				/// <summary>
				/// Sends string or other data to a window of any process. Uses API <msdn>SendMessage</msdn> <msdn>WM_COPYDATA</msdn>.
				/// Receives string or other data returned by that window with <see cref="Return"/>.
				/// </summary>
				/// <typeparam name="TSend">Type of data elements. For example, char for string, byte for byte[]</typeparam>
				/// <typeparam name="TReceive">Type of received data elements. For example, char for string, byte for byte[].</typeparam>
				/// <param name="w">The window.</param>
				/// <param name="dataId">Data id. It is <msdn>COPYDATASTRUCT.dwData</msdn>.</param>
				/// <param name="send">Data to send. For example string or byte[]. String can contain '\0' characters.</param>
				/// <param name="receive">Callback function that can convert the received data to desired format.</param>
				/// <returns>false if failed.</returns>
				public static unsafe bool SendReceive<TSend, TReceive>(wnd w, int dataId, ReadOnlySpan<TSend> send, ResultReader<TReceive> receive) where TSend : unmanaged where TReceive : unmanaged {
					var mutex = s_mutex.Value;
					if (Api.WaitForSingleObject(mutex, -1) is not (0 or Api.WAIT_ABANDONED_0)) return false;
					try {
						int len = (int)Send(w, dataId, send, Api.GetCurrentProcessId());
						if (len == 0) return false;
						var sm = SharedMemory_.ReturnDataPtr;
						if (len > 0) { //shared memory
							if (len <= SharedMemory_.ReturnDataSize) {
								receive(new ReadOnlySpan<TReceive>((TReceive*)sm, len / sizeof(TReceive)));
							} else {
								using var m2 = SharedMemory_.Mapping.CreateOrOpen(new((char*)sm), len);
								receive(new ReadOnlySpan<TReceive>((TReceive*)m2.Mem, len / sizeof(TReceive)));
							}
						} else { //process memory
							var pm = (void*)*(long*)sm;
							receive(new ReadOnlySpan<TReceive>((TReceive*)pm, -len / sizeof(TReceive)));
							bool ok = Api.VirtualFree(pm);
							Debug_.PrintIf(!ok, "VirtualFree");
						}
						return true;
					}
					finally { Api.ReleaseMutex(mutex); }
				}

				/// <summary>
				/// Calls <see cref="SendReceive{TSend, TReceive}(wnd, int, ReadOnlySpan{TSend}, ResultReader{TReceive})"/> and gets the returned data as byte[].
				/// </summary>
				public static bool SendReceive<T>(wnd w, int dataId, ReadOnlySpan<T> send, out byte[] receive) where T : unmanaged {
					byte[] r = null;
					bool R = SendReceive<T, byte>(w, dataId, send, span => r = span.ToArray());
					receive = r;
					return R;
				}

				/// <summary>
				/// Calls <see cref="SendReceive{TSend, TReceive}(wnd, int, ReadOnlySpan{TSend}, ResultReader{TReceive})"/> and gets the returned string.
				/// </summary>
				public static bool SendReceive<T>(wnd w, int dataId, ReadOnlySpan<T> send, out string receive) where T : unmanaged {
					string r = null;
					bool R = SendReceive<T, char>(w, dataId, send, span => r = span.ToString());
					receive = r;
					return R;
				}
				//public static bool SendReceive_<T>(wnd w, int dataId, ReadOnlySpan<T> send, out string receive, bool utf8) where T : unmanaged {
				//	string r = null;
				//	bool R = utf8
				//		? SendReceive_<T, byte>(w, dataId, send, span => r = Encoding.UTF8.GetString(span))
				//		: SendReceive_<T, char>(w, dataId, send, span => r = span.ToString());
				//	receive = r;
				//	return R;
				//}

				/// <summary>
				/// Returns data to <see cref="SendReceive"/>.
				/// </summary>
				/// <param name="data"></param>
				/// <param name="length"></param>
				/// <param name="wParam"><i>wParam</i> of the received <b>WM_COPYDATA</b> message. Important, pass unchanged.</param>
				/// <returns>Your window procedure must return this value.</returns>
				public static unsafe int Return(void* data, int length, nint wParam) {
					var sm = SharedMemory_.ReturnDataPtr;

					//use shared memory of this library. Max 1 MB.
					if (length <= SharedMemory_.ReturnDataSize) {
						MemoryUtil.Copy(data, sm, length);
						return length;
					}

					//allocate memory in caller process
					try {
						using var pm = new ProcessMemory((int)wParam, length);
						pm.Write(data, length);
						*(long*)sm = (long)pm.Mem;
						pm.ForgetMem();
						return -length;
					}
					catch { } //fails if that process has higher UAC IL. Rare.

					//allocate new shared memory
					try {
						var smname = "Au-memory-" + Guid.NewGuid().ToString();
						fixed (char* p = smname) MemoryUtil.Copy(p, sm, smname.Length * 2 + 2);
						var m2 = SharedMemory_.Mapping.CreateOrOpen(smname, length);
						MemoryUtil.Copy(data, m2.Mem, length);
						Task.Run(() => { //wait until caller returns and then close the shared memory in this process
							var mutex = s_mutex.Value;
							if (Api.WaitForSingleObject(mutex, -1) is not (0 or Api.WAIT_ABANDONED_0)) { Debug_.Print("WaitForSingleObject"); return; }
							Api.ReleaseMutex(mutex);
							m2.Dispose();
						});
						return length;
					}
					catch { return 0; }

					//speed when size 1 MB and hot CPU:
					//	shared memory: 1000 mcs
					//	process memory: 1500 mcs
					//	shared memory 2: 2500 mcs
				}

				/// <summary>
				/// Returns string or other data to <see cref="SendReceive"/>.
				/// </summary>
				/// <typeparam name="T">Type of data elements. For example, char for string, byte for byte[]</typeparam>
				/// <param name="data"></param>
				/// <param name="wParam"><i>wParam</i> of the received <b>WM_COPYDATA</b> message. Important, pass unchanged.</param>
				/// <returns>Your window procedure must return this value.</returns>
				public static unsafe int Return<T>(ReadOnlySpan<T> data, nint wParam) where T : unmanaged {
					fixed (T* f = data) return Return(f, data.Length * sizeof(T), wParam);
				}

				//rejected. Don't need too many not important overloads. Good: in most cases data size is 2 times smaller. Same: speed.
				//[SkipLocalsInit]
				//public static unsafe int ReturnStringUtf8_(ReadOnlySpan<char> data, nint wParam) {
				//	var e = Encoding.UTF8;
				//	using var b = new FastBuffer<byte>(e.GetByteCount(data));
				//	int len = e.GetBytes(data, new Span<byte>(b.p, b.n));
				//	return ReturnData_(b.p, len, wParam);
				//}

				#endregion
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Used by <see cref="wnd.more.taskbarButton.setProgressState"/>.
	/// </summary>
	[NoDoc]
	public enum WTBProgressState
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
