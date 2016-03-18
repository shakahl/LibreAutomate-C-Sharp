using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;

class TestProg
{
	static IntPtr _hwndCompiler;
	static Api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate

	static void Main(string[] args)
	{
		Catkeys.Util.Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

		var thr=new Thread(_AppDomainThread);
		thr.Start();

		Thread.Sleep(100);
		_hwndCompiler=Api.FindWindow("Catkeys_Compiler", null);
		Console.WriteLine($"_hwndCompiler={_hwndCompiler}");
		
		Api.SendMessage(_hwndCompiler, WM_.USER, NULL, Marshal.StringToBSTR("test"));

		Console.WriteLine("key...");
		Console.ReadKey();
	}

	static void _AppDomainThread()
	{
		var domain=AppDomain.CreateDomain("Compiler");
		//System.IO.Pipes.AnonymousPipeClientStream
		//childDomain.SetData("hPipe", handle.ToString());
		domain.DoCallBack(_DomainCallback);
		AppDomain.Unload(domain);
	}

	static void _DomainCallback()
	{
		//=AppDomain.CurrentDomain.GetData("hPipe")

		Api.CreateWindowExW(0, "Catkeys_Compiler",
			null, WS_.POPUP, 0, 0, 0, 0, (IntPtr)(-3),
			NULL, Window.hInstModule, NULL);

		Application.Run(); //message loop
	}

	unsafe static IntPtr _WndProcCompiler(IntPtr hWnd, WM_ msg, IntPtr wParam, IntPtr lParam)
	{
		switch(msg) {
		//case WM.NCCREATE:
		//	_hwndAM=hWnd;
		//	break;
		//case WM.CREATE:
		//	Time.Next();
		//	break;
		//case WM.COPYDATA: //TODO: ChangeWindowMessageFilter
		//	_OnCopyData(wParam, (api.COPYDATASTRUCT*)lParam);
		//	break;
		//case WM.DESTROY:
		//	Out("destroy");
		//	break;
		case WM_.USER:
			Console.WriteLine(Marshal.PtrToStringBSTR(lParam));
			Marshal.FreeBSTR(lParam);
			return NULL;
		}

		IntPtr R = Api.DefWindowProcW(hWnd, msg, wParam, lParam);

		//switch(msg) {
		//case WM.NCDESTROY:
		//	//Out("ncdestroy");
		//	Application.Exit();
		//	break;
		//}
		return R;

		//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	}
}
