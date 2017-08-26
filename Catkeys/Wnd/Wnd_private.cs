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
			//[DllImport("kernel32.dll")]
			//internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref int AppModelIDLength, char* sbAppUserModelID);
			[DllImport("kernel32.dll")]
			internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref int AppModelIDLength, [Out] char[] sbAppUserModelID);
		}

		/// <summary>
		/// Gets window Windows Store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
		/// Returns 1 if gets user model id, 2 if gets path, 0 if fails.
		/// </summary>
		/// <param name="w">Window.</param>
		/// <param name="appId">Receives app ID.</param>
		/// <param name="prependShellAppsFolder">Prepend @"shell:AppsFolder\" (to run or get icon).</param>
		/// <param name="getExePathIfNotWinStoreApp">Get program path if it is not a Windows Store app.</param>
		static int _GetWindowsStoreAppId(Wnd w, out string appId, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
		{
			appId = null;

			if(Ver.MinWin8) {
				switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
				case 1:
					using(var p = Process_.LibProcessHandle.FromWnd(w)) {
						if(p != null) {
							var b = Util.Buffers.LibChar(1000, out int na);
							if(0 == _Api.GetApplicationUserModelId(p, ref na, b)) appId = b.ToString(na);
						}
					}
					break;
				case 2:
					if(Ver.MinWin10) {
						Api.IPropertyStore ps; Api.PROPVARIANT v;
						if(0 == Api.SHGetPropertyStoreForWindow(w, ref Api.IID_IPropertyStore, out ps)) {
							if(0 == ps.GetValue(ref Api.PKEY_AppUserModel_ID, out v)) {
								if(v.vt == Api.VARENUM.VT_LPWSTR) appId = Marshal.PtrToStringUni(v.value);
								v.Dispose();
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
			if(!Ver.MinWin10 || !w.ClassNameIs("ApplicationFrameWindow")) return default(Wnd);
			Wnd c = Api.FindWindowEx(w, default(Wnd), "Windows.UI.Core.CoreWindow", null);
			if(!c.Is0) return c;
			if(retry) return default(Wnd);

			name = w.GetText(false, false); if(Empty(name)) return default(Wnd);

			for(;;) {
				c = Api.FindWindowEx(default(Wnd), c, "Windows.UI.Core.CoreWindow", name); //I could not find API for it
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
			if(!Ver.MinWin10 || !w.ClassNameIs("Windows.UI.Core.CoreWindow")) return default(Wnd);
			Wnd wo = w.WndDirectParent; if(!wo.Is0 && wo.ClassNameIs("ApplicationFrameWindow")) return wo;
			string s = w.GetText(false, false); if(Empty(s)) return default(Wnd);
			return Api.FindWindow("ApplicationFrameWindow", s);
		}
	}
}
