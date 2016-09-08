using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	public partial struct Wnd
	{
		static partial class _Api
		{
			[DllImport("kernel32.dll")]
			internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref uint AppModelIDLength, [Out] StringBuilder sbAppUserModelID);
		}

		/// <summary>
		/// Gets window Windows Store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
		/// Returns 1 if gets user model id, 2 if gets path, 0 if fails.
		/// </summary>
		/// <param name="w">Window.</param>
		/// <param name="appId">Receives app ID.</param>
		/// <param name="prependShellAppsFolder">Prepend @"shell:AppsFolder\" (to run or get icon).</param>
		/// <param name="getExePathIfNotWinStoreApp">Get exe full path if hwnd is not a Windows Store app.</param>
		static int _WindowsStoreAppId(Wnd w, out string appId, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
		{
			appId = null;

			if(WinVer >= Win8) {
				switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
				case 1:
					using(var p = new Process_.LibProcessHandle(w)) {
						if(!p.Is0) {
							uint u = 1000; var sb = new StringBuilder((int)u);
							if(0 == _Api.GetApplicationUserModelId(p, ref u, sb)) appId = sb.ToString();
						}
					}
					break;
				case 2:
					if(WinVer >= Win10) {
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

		//static int _WindowsStoreAppId2(Wnd w, out string appID, bool prependShellAppsFolder = false, bool getExePathIfNotWinStoreApp = false)
		//{
		//	appID = null;

		//	if(getExePathIfNotWinStoreApp) {
		//		if(WinVer >= Win8) {
		//			bool isApp = false;
		//			switch(w.ClassNameIs("Windows.UI.Core.CoreWindow", "ApplicationFrameWindow")) {
		//			case 1:
		//				isApp = true;
		//				break;
		//			case 2:
		//				if(WinVer >= Win10) {
		//					Wnd t = _WindowsStoreAppFrameChild(w);
		//					if(!t.Is0) { w = t; isApp = true; }
		//				}
		//				break;
		//			}
		//			if(!isApp) {
		//				appID = w.ProcessPath;
		//				return (appID == null) ? 0 : 2;
		//			}
		//		}

		//	}

		//	using(var p = new Process_.LibProcessHandle(w)) {
		//		if(p.Is0) return 0;
		//		uint u = 1000;
		//		var sb = new StringBuilder((int)u);
		//		if(0 != _Api.GetApplicationUserModelId(p, ref u, sb)) return 0;
		//		if(prependShellAppsFolder) appID = @"shell:AppsFolder\" + sb.ToString(); else appID = sb.ToString();
		//		return 1;
		//	}
		//}

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
			if(WinVer < Win10 || !w.ClassNameIs("ApplicationFrameWindow")) return Wnd0;
			Wnd c = Api.FindWindowEx(w, Wnd0, "Windows.UI.Core.CoreWindow", null);
			if(!c.Is0) return c;
			if(retry) return Wnd0;

			name = w.Name; if(Empty(name)) return Wnd0;

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
			if(WinVer < Win10 || !w.ClassNameIs("Windows.UI.Core.CoreWindow")) return Wnd0;
			Wnd wo = w.DirectParent; if(!wo.Is0 && wo.ClassNameIs("ApplicationFrameWindow")) return wo;
			string s = w.Name; if(Empty(s)) return Wnd0;
			return Api.FindWindow("ApplicationFrameWindow", s);
		}
	}


	//This can be used, but not much simpler than calling ATI directly and using try/finally.
	//internal struct LibAttachThreadInput :IDisposable
	//{
	//	uint _tid;

	//	public bool Attach(uint tid)
	//	{
	//		if(!Api.AttachThreadInput(Api.GetCurrentThreadId(), _tid, true)) return false;
	//		_tid = tid; return true;
	//	}

	//	public bool Attach(Wnd w) { return Attach(w.ThreadId); }

	//	public void Dispose() { if(_tid != 0) Api.AttachThreadInput(Api.GetCurrentThreadId(), _tid, false); }
	//}

}
