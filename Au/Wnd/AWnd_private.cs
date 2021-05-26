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
//using System.Linq;

namespace Au
{
	public unsafe partial struct AWnd
	{
		/// <summary>
		/// if(!IsOfThisThread) { Thread.Sleep(15); SendTimeout(1000, 0); }
		/// </summary>
		internal void MinimalSleepIfOtherThread_() {
			if (!IsOfThisThread) MinimalSleepNoCheckThread_();
		}

		/// <summary>
		/// Thread.Sleep(15); SendTimeout(1000, 0);
		/// </summary>
		internal void MinimalSleepNoCheckThread_() {
			Debug.Assert(!IsOfThisThread);
			//APerf.First();
			Thread.Sleep(15);
			SendTimeout(1000, out _, 0);
			//APerf.NW();
		}

		/// <summary>
		/// On Win10+, if w is "ApplicationFrameWindow", returns the real app window "Windows.UI.Core.CoreWindow" hosted by w.
		/// If w is minimized, cloaked (eg on other desktop) or the app is starting, the "Windows.UI.Core.CoreWindow" is not its child. Then searches for a top-level window named like w. It is unreliable, but MS does not provide API for this.
		/// Info: "Windows.UI.Core.CoreWindow" windows hosted by "ApplicationFrameWindow" belong to separate processes. All "ApplicationFrameWindow" windows belong to a single process.
		/// </summary>
		static AWnd _WindowsStoreAppFrameChild(AWnd w) {
			bool retry = false;
			string name = null;
			g1:
			if (!AVersion.MinWin10 || !w.ClassNameIs("ApplicationFrameWindow")) return default;
			AWnd c = Api.FindWindowEx(w, default, "Windows.UI.Core.CoreWindow", null);
			if (!c.Is0) return c;
			if (retry) return default;

			name = w.NameTL_; if (name.NE()) return default;

			for (; ; ) {
				c = Api.FindWindowEx(default, c, "Windows.UI.Core.CoreWindow", name); //I could not find API for it
				if (c.Is0) break;
				if (c.IsCloaked) return c; //else probably it is an unrelated window
			}

			retry = true;
			goto g1;
		}

		//not used
		///// <summary>
		///// The reverse of _WindowsStoreAppFrameChild.
		///// </summary>
		//static AWnd _WindowsStoreAppHost(AWnd w)
		//{
		//	if(!AVersion.MinWin10 || !w.ClassNameIs("Windows.UI.Core.CoreWindow")) return default;
		//	AWnd wo = w.Get.DirectParent; if(!wo.Is0 && wo.ClassNameIs("ApplicationFrameWindow")) return wo;
		//	string s = w.GetText(false, false); if(s.NE()) return default;
		//	return Api.FindWindow("ApplicationFrameWindow", s);
		//}

		internal static partial class Internal_
		{
			/// <summary>
			/// Gets window Windows Store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
			/// Returns 1 if gets user model id, 2 if gets path, 0 if fails.
			/// </summary>
			/// <param name="w">Window.</param>
			/// <param name="appId">Receives app ID.</param>
			/// <param name="prependShellAppsFolder">Prepend <c>@"shell:AppsFolder\"</c> (to run or get icon).</param>
			/// <param name="getExePathIfNotWinStoreApp">Get program path if it is not a Windows Store app.</param>
			[SkipLocalsInit]
			internal static int GetWindowsStoreAppId(AWnd w, out string appId, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false) {
				appId = null;

				if (AVersion.MinWin8) {
					switch (w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
					case 1:
						using (var p = Handle_.OpenProcess(w)) {
							if (!p.Is0) {
								int na = 1024; var b = stackalloc char[na];
								if (0 == Api.GetApplicationUserModelId(p, ref na, b) && na > 1) appId = new(b, 0, na - 1);
							}
						}
						break;
					case 2 when AVersion.MinWin10:
						if (0 == Api.SHGetPropertyStoreForWindow(w, Api.IID_IPropertyStore, out Api.IPropertyStore ps)) {
							if (0 == ps.GetValue(Api.PKEY_AppUserModel_ID, out var v)) {
								if (v.vt == Api.VARENUM.VT_LPWSTR) appId = Marshal.PtrToStringUni(v.value);
								v.Dispose();
							}
							Marshal.ReleaseComObject(ps);
						}
						break;
					}

					if (appId != null) {
						if (prependShellAppsFolder) appId = @"shell:AppsFolder\" + appId;
						return 1;
					}
				}

				if (getExePathIfNotWinStoreApp) {
					appId = w.ProgramPath;
					if (appId != null) return 2;
				}

				return 0;
			}

			[Flags]
			internal enum WFlags
			{
				//these were used by AElement.
				//ChromeYes = 1,
				//ChromeNo = 2,
			}

			/// <summary>
			/// Calls API SetProp/GetProp to set/get window flags <see cref="WFlags"/>.
			/// </summary>
			internal static class WinFlags
			{
				static readonly ushort s_atom = Api.GlobalAddAtom("Au.WFlags"); //atom is much faster than string
																				//note: cannot delete atom, eg in static dtor. Deletes even if currently used by a window prop, making the prop useless.

				internal static bool Set(AWnd w, WFlags flags, bool? setAddRem = null) {
					switch (setAddRem) {
					case true: flags = Get(w) | flags; break;
					case false: flags = Get(w) & ~flags; break;
					}
					return w.Prop.Set(s_atom, (int)flags);
				}

				internal static WFlags Get(AWnd w) {
					return (WFlags)(int)w.Prop[s_atom];
				}

				internal static WFlags Remove(AWnd w) {
					return (WFlags)(int)w.Prop.Remove(s_atom);
				}
			}

			//internal class LastWndProps
			//{
			//	AWnd _w;
			//	long _time;
			//	string _class, _programName, _programPath;
			//	int _tid, _pid;

			//	void _GetCommon(AWnd w)
			//	{
			//		var t = ATime.PerfMilliseconds;
			//		if(w != _w || t - _time > 100) { _w = w; _class = _programName= _programPath = null; _tid = _pid = 0; }
			//		_time = t;
			//	}

			//	//internal string GetName(AWnd w) { _GetCommon(w); return _name; }

			//	internal string GetClass(AWnd w) { _GetCommon(w); return _class; }

			//	internal string GetProgram(AWnd w, bool fullPath) { _GetCommon(w); return fullPath ? _programPath : _programName; }

			//	internal int GetTidPid(AWnd w, out int pid) { _GetCommon(w); pid = _pid; return _tid; }

			//	//internal void SetName(string s) => _name = s;

			//	internal void SetClass(string s) => _class = s;

			//	internal void SetProgram(string s, bool fullPath) { if(fullPath) _programPath = s; else _programName = s; }

			//	internal void SetTidPid(int tid, int pid) { _tid = tid; _pid = pid; }

			//	[ThreadStatic] static LastWndProps _ofThread;
			//	internal static LastWndProps OfThread => _ofThread ??= new LastWndProps();
			//}

			/// <summary>
			/// Auto-registers window class "Au.DWP" with wndproc = DefWindowProc and creates hidden window.
			/// </summary>
			/// <param name="messageOnly"></param>
			/// <param name="wndProcUnsafe">If not null, replaces window procedure (SetWindowLongPtr). The caller must protect the delegate from GC.</param>
			public static AWnd CreateWindowDWP(bool messageOnly, WNDPROC wndProcUnsafe = null) {
				var cn = WindowClassDWP;
				var w = messageOnly ? More.CreateMessageOnlyWindow(cn) : More.CreateWindow(cn);
				if (wndProcUnsafe != null) Api.SetWindowLongPtr(w, GWLong.WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProcUnsafe));
				return w;
			}
			static int s_registeredDWP;
			const string c_wndClassDWP = "Au.DWP";

			/// <summary>
			/// Auto-registers window class "Au.DWP" with wndproc = DefWindowProc and returns "Au.DWP".
			/// </summary>
			public static string WindowClassDWP {
				get {
					if (0 == Interlocked.CompareExchange(ref s_registeredDWP, 1, 0)) {
						var x = new Api.WNDCLASSEX { cbSize = sizeof(Api.WNDCLASSEX), style = Api.CS_GLOBALCLASS };
						fixed (char* pCN = c_wndClassDWP) {
							x.lpszClassName = pCN;
							x.lpfnWndProc = Api.GetProcAddress("user32.dll", "DefWindowProcW");
							if (0 == Api.RegisterClassEx(x)) throw new Win32Exception();
						}
					}
					return c_wndClassDWP;
				}
			}

			/// <summary>
			/// Replaces window procedure (SetWindowLongPtr). Returns previous window procedure.
			/// The caller must protect the delegate from GC.
			/// </summary>
			public static IntPtr SubclassUnsafe(AWnd w, WNDPROC wndProc) {
				return Api.SetWindowLongPtr(w, GWLong.WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProc));
			}

			/// <summary>
			/// Returns true if w contains a non-zero special handle value (<see cref="SpecHWND"/>).
			/// Note: <b>SpecHWND.TOP</b> is 0.
			/// </summary>
			public static bool IsSpecHwnd(AWnd w) {
				int i = (int)w;
				return (i <= 1 && i >= -3) || i == 0xffff;
			}

			/// <summary>
			/// Converts object to AWnd.
			/// Object can contain null, AWnd, Control, or System.Windows.DependencyObject (must be in element 0 of object[]).
			/// Avoids loading Forms and WPF dlls when not used.
			/// </summary>
			public static AWnd FromObject(object o) => o switch {
				null => default,
				AWnd w => w,
				object[] a => _Wpf(a[0]),
				_ => _Control(o)
			};

			[MethodImpl(MethodImplOptions.NoInlining)] //prevents loading Forms dlls when don't need
			static AWnd _Control(object o) => (o as System.Windows.Forms.Control).Hwnd();

			[MethodImpl(MethodImplOptions.NoInlining)] //prevents loading WPF dlls when don't need
			static AWnd _Wpf(object o) => (o as System.Windows.DependencyObject).Hwnd();

			/// <summary>
			/// If w is handle of a WPF element (Window, Popup, HwndHost-ed control, HwndSource.RootVisual), returns that element, else null.
			/// Slow if HwndHost-ed control.
			/// w can be default.
			/// </summary>
			public static System.Windows.FrameworkElement ToWpfElement(AWnd w) {
				if (!w.Is0) {
					if (System.Windows.Interop.HwndSource.FromHwnd(w.Handle) is System.Windows.Interop.HwndSource hs) return hs.RootVisual as System.Windows.FrameworkElement;
					for (var p = w; !(p = p.Get.DirectParent).Is0; w = p) {
						if (System.Windows.Interop.HwndSource.FromHwnd(p.Handle)?.RootVisual is System.Windows.Media.Visual v) {
							return v.FindVisualDescendant(d => d is System.Windows.Interop.HwndHost hh && hh.Handle == w.Handle, orSelf: true) as System.Windows.FrameworkElement; //speed: 200 mcs
						}
					}
				}
				return null;
			}

		}
	}
}
