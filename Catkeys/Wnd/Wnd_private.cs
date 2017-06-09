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

namespace Catkeys
{
	//[DebuggerStepThrough]
	public partial struct Wnd
	{
		/// <summary>
		/// if(!IsOfThisThread) { Thread.Sleep(15); SendTimeout(1000, 0); }
		/// </summary>
		void _MinimalWaitIfOtherThread()
		{
			if(!IsOfThisThread) _MinimalWaitNoCheckThread();
		}

		/// <summary>
		/// Thread.Sleep(15); SendTimeout(1000, 0);
		/// </summary>
		void _MinimalWaitNoCheckThread()
		{
			Debug.Assert(!IsOfThisThread);
			//Perf.First();
			Thread.Sleep(15);
			SendTimeout(1000, 0);
			//Perf.NW();
		}

		static void _ThrowIfStringEmptyNotNull(string s, string paramName)
		{
			if(s != null && s.Length == 0) throw new ArgumentException("Cannot be \"\". Can be null.", paramName);
		}

		static unsafe partial class _Api
		{
			[DllImport("kernel32.dll")]
			internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref int AppModelIDLength, [Out] char* sbAppUserModelID);
		}

		/// <summary>
		/// Gets window Windows Store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
		/// Returns 1 if gets user model id, 2 if gets path, 0 if fails.
		/// </summary>
		/// <param name="w">Window.</param>
		/// <param name="appId">Receives app ID.</param>
		/// <param name="prependShellAppsFolder">Prepend @"shell:AppsFolder\" (to run or get icon).</param>
		/// <param name="getExePathIfNotWinStoreApp">Get program path if it is not a Windows Store app.</param>
		static unsafe int _GetWindowsStoreAppId(Wnd w, out string appId, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
		{
			appId = null;

			if(Ver.MinWin8) {
				switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
				case 1:
					using(var p = Process_.LibProcessHandle.FromWnd(w)) {
						if(p!=null) {
							var b = Util.CharBuffer.LibCommon; int na = 1000;
							if(0 == _Api.GetApplicationUserModelId(p, ref na, b.Alloc(na))) appId = b.ToString();
						}
					}
					break;
				case 2:
					if(Ver.MinWin10) {
						Api.IPropertyStore ps; Api.PROPVARIANT_LPARAM v;
						if(0 == Api.SHGetPropertyStoreForWindow(w, ref Api.IID_IPropertyStore, out ps)) {
							if(0 == ps.GetValue(ref Api.PKEY_AppUserModel_ID, out v)) {
								if(v.vt == (ushort)Api.VARENUM.VT_LPWSTR) appId = Marshal.PtrToStringUni(v.value);
								Api.PropVariantClear(ref v);
							}
							Marshal.ReleaseComObject(ps);
						}
					}
					break;
				}

				if(appId != null) {
					if(prependShellAppsFolder) appId = @"shell:AppsFolder\" + appId;
					return 1;
				}
			}

			if(getExePathIfNotWinStoreApp) {
				appId = w.ProcessPath;
				if(appId != null) return 2;
			}

			return 0;
		}

		/// <summary>
		/// On Win10+, if w is "ApplicationFrameWindow", returns the real app window "Windows.UI.Core.CoreWindow" hosted by w.
		/// If w is minimized, cloaked (eg on other desktop) or the app is starting, the "Windows.UI.Core.CoreWindow" is not its child. Then searches for a top-level window with the same name as of w. It is unreliable, but MS does not provide API for this.
		/// Info: "Windows.UI.Core.CoreWindow" windows hosted by "ApplicationFrameWindow" belong to separate processes. All "ApplicationFrameWindow" windows belong to a single process.
		/// </summary>
		static Wnd _WindowsStoreAppFrameChild(Wnd w)
		{
			bool retry = false;
			string name = null;
			g1:
			if(!Ver.MinWin10 || !w.ClassNameIs("ApplicationFrameWindow")) return Wnd0;
			Wnd c = Api.FindWindowEx(w, Wnd0, "Windows.UI.Core.CoreWindow", null);
			if(!c.Is0) return c;
			if(retry) return Wnd0;

			name = w.GetText(false, false); if(Empty(name)) return Wnd0;

			for(;;) {
				c = Api.FindWindowEx(Wnd0, c, "Windows.UI.Core.CoreWindow", name); //I could not find API for it
				if(c.Is0) break;
				if(c.IsCloaked) return c; //else probably it is an unrelated window
			}

			retry = true;
			goto g1;
		}

		/// <summary>
		/// The reverse of _WindowsStoreAppFrameChild.
		/// </summary>
		static Wnd _WindowsStoreAppHost(Wnd w)
		{
			if(!Ver.MinWin10 || !w.ClassNameIs("Windows.UI.Core.CoreWindow")) return Wnd0;
			Wnd wo = w.WndDirectParent; if(!wo.Is0 && wo.ClassNameIs("ApplicationFrameWindow")) return wo;
			string s = w.GetText(false, false); if(Empty(s)) return Wnd0;
			return Api.FindWindow("ApplicationFrameWindow", s);
		}

		/// <summary>
		/// Normalizes coordinates or width/height (converts fraction to pixels, adds offsets, etc).
		/// </summary>
		[DebuggerStepThrough]
		static class _Coord
		{
			/// <summary>
			/// If c.type == 1, returns c.v1 + min.
			/// Else calculates and returns non-fraction coordinate.
			/// </summary>
			static int _Normalize(Types<int, float> c, int min, int max)
			{
				Debug.Assert(c.type != 0);
				return (c.type == 2 ? (int)((max - min) * c.v2) : c.v1) + min;
			}

			/// <summary>
			/// Returns new POINT(x, y), relative to the primary screen, where fractional x and/or y are converted to pixels.
			/// x or/and y .type can be 0, then the returned x or y value will be 0.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <param name="workArea">If false, coordinates are in primary screen, else in its work area.</param>
			/// <param name="widthHeight">Don't add work area offset. Use when x and y are width and height.</param>
			internal static POINT GetNormalizedInScreen(Types<int, float> x, Types<int, float> y, bool workArea = false, bool widthHeight = false)
			{
				var p = new POINT();
				if(x.type != 0 || y.type != 0) {
					RECT r;
					if(workArea) {
						r = Screen_.WorkArea;
						if(widthHeight) r.Offset(-r.left, -r.top);
					} else {
						r = new RECT(0, 0, Screen_.Width, Screen_.Height, false);
					}
					if(x.type != 0) p.x = _Normalize(x, r.left, r.right);
					if(y.type != 0) p.y = _Normalize(y, r.top, r.bottom);
				}
				return p;
			}

			/// <summary>
			/// Returns new POINT(x, y), relative to the window w client area, where fractional x and/or y are converted to pixels.
			/// x or/and y .type can be 0, then the returned x or y value will be 0.
			/// </summary>
			internal static POINT GetNormalizedInWindowClientArea(Types<int, float> x, Types<int, float> y, Wnd w)
			{
				var p = new POINT();
				if(x.type != 0 || y.type != 0) {
					if(x.type == 2 || y.type == 2) {
						RECT r = w.ClientRect;
						if(x.type != 0) p.x = _Normalize(x, 0, r.right);
						if(y.type != 0) p.y = _Normalize(y, 0, r.bottom);
					} else {
						if(x.type != 0) p.x = x.v1;
						if(y.type != 0) p.y = y.v1;
					}
				}
				return p;
			}
		}
	}
}
