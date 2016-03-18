using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Tasks
{
static class Program
{
	static IntPtr _hwndAM;
	static Api.WndProc _wndProcKeepAlive = WndProcAppsManager; //use static member to prevent GC from collecting the delegate

	//static Program()
	//{
	//}

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	//[LoaderOptimization(LoaderOptimization.MultiDomain)] //makes new AppDomain creation faster by 10 ms. But then does not unload assemblies of unloaded domains (uses many more MB of memory).
	[LoaderOptimization(LoaderOptimization.MultiDomainHost)] //makes new AppDomain creation faster, and unloads non-GAC assemblies, eg frees many MB of compiler memory.
	static void Main(string[] args)
	{
		Test._Main();
#if NOTEST

		//Out($"{Time.TimeNowMCS()-Convert.ToInt64(args[0])}"); //startup time: 17 ms. In QM2 use: CreateProcessSimple(F"Q:\app\Catkeys\Tasks\Catkeys Tasks.exe {perf}")

		//Application.EnableVisualStyles(); //don't need for this domain; do it for script domains; 600 mcs
		//Application.SetCompatibleTextRenderingDefault(false); //90 mcs
		//Time.First();
		ushort atom = Window.RegWinClassHidden("CatkeysTasks", _wndProcKeepAlive);

		api.CreateWindowExW(0, "CatkeysTasks",
			//null, WS.POPUP, 0, 0, 0, 0, (IntPtr)(-3),
			"Catkeys Tasks", WS.OVERLAPPEDWINDOW|WS.VISIBLE, 400, 300, 400, 200, NULL,
			NULL, Window.hInst, NULL);
		//Out($"{_hwndAM}");

		//GC.Collect(); //does nothing here
		//Time.NextOut();
		Application.Run();

		//api.DestroyWindow(_hwndAM);
		//Application.Run(new ManagerWin());
		return;

		//Test._Main();

		//Compile.Init();
		//Thread.Sleep(2000);
		//Msg("Main");
#endif
		}

		unsafe static IntPtr WndProcAppsManager(IntPtr hWnd, WM_ msg, IntPtr wParam, IntPtr lParam)
	{
		switch(msg) {
		case WM_.NCCREATE:
			_hwndAM=hWnd;
			break;
		//case WM.CREATE:
		//	Time.Next();
		//	break;
		case WM_.COPYDATA: //TODO: ChangeWindowMessageFilter
			_OnCopyData(wParam, (Api.COPYDATASTRUCT*)lParam);
			break;
		//case WM.DESTROY:
		//	Out("destroy");
		//	break;
		}

		IntPtr R = Api.DefWindowProcW(hWnd, msg, wParam, lParam);

		switch(msg) {
		case WM_.NCDESTROY:
			//Out("ncdestroy");
			Application.Exit();
			break;
		}
		return R;

		//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	}

	unsafe static int _OnCopyData(IntPtr hwndSender, Api.COPYDATASTRUCT* c)
	{
		Out($"{c->dwData}");
		string s=Marshal.PtrToStringUni(c->lpData, c->cbData/2-1);
		Out(s);
		return 0;
	}
}
}
