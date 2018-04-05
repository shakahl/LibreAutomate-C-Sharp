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
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

#if TEST
partial class ThisIsNotAFormFile { }

//[DebuggerStepThrough]
partial class EForm
{
	internal void TestEditor()
	{
		var f = new Au.Tools.Form_Acc();
		f.Show(this);



		//Panels.Status.SetText("same thread\r\nline2\r\nline3");
		//Task.Run(() => { 2.s(); Panels.Status.SetText("other thread, WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM"); });
	}

	void SetHookToMonitorCreatedWindowsOfThisThread()
	{
		var hh = Api.SetWindowsHookEx(Api.WH_CBT, _cbtHookProc, default, Api.GetCurrentThreadId());
		Application.ApplicationExit += (unu, sed) => Api.UnhookWindowsHookEx(hh); //without it at exit crashes, and process exits with a delay
	}

	static Api.HOOKPROC _cbtHookProc = _CbtHookProc;
	static unsafe LPARAM _CbtHookProc(int code, LPARAM wParam, LPARAM lParam)
	{
		if(code == Api.HCBT_CREATEWND) {
			Wnd w = (Wnd)wParam;
			var s = w.ClassName;
			Print(w);
		}

		return Api.CallNextHookEx(default, code, wParam, lParam);
	}
}
#endif
