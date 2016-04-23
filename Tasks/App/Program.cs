using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Tasks
{
	static class Program
	{
		static Wnd _hwndAM;
		static Api.WNDPROC _wndProcKeepAlive = WndProcAppsManager; //use static member to prevent GC from collecting the delegate

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
			Application.EnableVisualStyles();
			Test._Main();
#if NOTEST

		//Out($"{Time.Microseconds-Convert.ToInt64(args[0])}"); //startup time: 17 ms. In QM2 use: CreateProcessSimple(F"Q:\app\Catkeys\Tasks\Catkeys Tasks.exe {perf}")

		//Application.EnableVisualStyles(); //don't need for this domain; do it for script domains; 600 mcs
		//Application.SetCompatibleTextRenderingDefault(false); //90 mcs
		//Speed.First();
		ushort atom = Window.RegWinClassHidden("CatkeysTasks", _wndProcKeepAlive);

		api.CreateWindowExW(0, "CatkeysTasks",
			//null, WS.POPUP, 0, 0, 0, 0, Wnd.Spec.Message,
			"Catkeys Tasks", WS.OVERLAPPEDWINDOW|WS.VISIBLE, 400, 300, 400, 200, Zero,
			Zero, Zero, Zero);
		//Out($"{_hwndAM}");

		//GC.Collect(); //does nothing here
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

		unsafe static LPARAM WndProcAppsManager(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
		{
			switch(msg) {
			case Api.WM_NCCREATE:
				_hwndAM=hWnd;
				break;
			//case WM.CREATE:
			//	Speed.Next();
			//	break;
			case Api.WM_COPYDATA: //TODO: ChangeWindowMessageFilter
				_OnCopyData((Wnd)wParam, (Api.COPYDATASTRUCT*)lParam);
				break;
				//case WM.DESTROY:
				//	Out("destroy");
				//	break;
			}

			LPARAM R = Api.DefWindowProc(hWnd, msg, wParam, lParam);

			switch(msg) {
			case Api.WM_NCDESTROY:
				//Out("ncdestroy");
				Application.Exit();
				break;
			}
			return R;

			//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
		}

		unsafe static int _OnCopyData(Wnd hwndSender, Api.COPYDATASTRUCT* c)
		{
			Out($"{c->dwData}");
			string s = Marshal.PtrToStringUni(c->lpData, c->cbData/2-1);
			Out(s);
			return 0;
		}
	}
}
