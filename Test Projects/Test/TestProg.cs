using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;

class TestProg
{
	static void Main(string[] args)
	{
		//Debug.Assert(false);
		//Debug.Fail("fail"); //same as Debug.Assert(false, "fail");
		//Debug.Write("write"); //only in debug mode (not the same as debug build); shows in VS output, not in console (unless you add a listener for it)
		//Debug.WriteLine("writeline"); //the same as Debug.Write with \n
		//Debug.Print("print"); //the same as Debug.WriteLine. Perhaps for VB.

		//Trace.Assert(false);
		//Trace.TraceError("eee"); //VS output shows "Error: 0 : eee"
		//Trace.Write("write");



		Console.WriteLine("etc");

		//Console.ReadKey();
	}

	//static Wnd _hwndCompiler;
	//static Api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate

	//static void Main(string[] args)
	//{
	//	Catkeys.Util.Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

	//	var thr=new Thread(_AppDomainThread);
	//	thr.Start();

	//	Thread.Sleep(100);
	//	_hwndCompiler=Api.FindWindow("Catkeys_Compiler", null);
	//	Console.WriteLine($"_hwndCompiler={_hwndCompiler}");

	//	_hwndCompiler.Send(Api.WM_USER, Zero, Marshal.StringToBSTR("test"));

	//	Console.WriteLine("key...");
	//	Console.ReadKey();
	//}

	//static void _AppDomainThread()
	//{
	//	var domain=AppDomain.CreateDomain("Compiler");
	//	//System.IO.Pipes.AnonymousPipeClientStream
	//	//childDomain.SetData("hPipe", handle.ToString());
	//	domain.DoCallBack(_DomainCallback);
	//	AppDomain.Unload(domain);
	//}

	//static void _DomainCallback()
	//{
	//	//=AppDomain.CurrentDomain.GetData("hPipe")

	//	Api.CreateWindowExW(0, "Catkeys_Compiler",
	//		null, WS_.POPUP, 0, 0, 0, 0, Wnd.Spec.Message,
	//		Zero, Window.hInstModule, Zero);

	//	Application.Run(); //message loop
	//}

	//unsafe static LPARAM _WndProcCompiler(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
	//{
	//	switch(msg) {
	//	//case WM.NCCREATE:
	//	//	_hwndAM=hWnd;
	//	//	break;
	//	//case WM.CREATE:
	//	//	Time.Next();
	//	//	break;
	//	//case WM.COPYDATA: //TODO: ChangeWindowMessageFilter
	//	//	_OnCopyData(wParam, (api.COPYDATASTRUCT*)lParam);
	//	//	break;
	//	//case WM.DESTROY:
	//	//	Out("destroy");
	//	//	break;
	//	case Api.WM_USER:
	//		Console.WriteLine(Marshal.PtrToStringBSTR(lParam));
	//		Marshal.FreeBSTR(lParam);
	//		return Zero;
	//	}

	//	LPARAM R = Api.DefWindowProcW(hWnd, msg, wParam, lParam);

	//	//switch(msg) {
	//	//case WM.NCDESTROY:
	//	//	//Out("ncdestroy");
	//	//	Application.Exit();
	//	//	break;
	//	//}
	//	return R;

	//	//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	//}
}
