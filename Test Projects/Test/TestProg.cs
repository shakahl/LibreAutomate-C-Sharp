#region copy-paste-from-Api.cs

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Wnd = System.IntPtr; //HWND (window handle)
using LPARAM = System.IntPtr; //LPARAM, WPARAM, LRESULT, X_PTR, SIZE_T, ... (non-pointer types that have different size in 64-bit and 32-bit process)

//add this to projects that will use these API
[module: DefaultCharSet(CharSet.Unicode)]

static unsafe class API
{
	//simplest Windows API function declaration  example
	[DllImport("user32.dll", EntryPoint = "FindWindowW")]
	public static extern Wnd FindWindow(string lpClassName, string lpWindowName);

	//example of a function that gets text using a caller-allocated buffer
	[DllImport("user32.dll")]
	public static extern int InternalGetWindowText(Wnd hWnd, [Out] StringBuilder pString, int cchMaxCount);

	//example of a function that uses a callback function (delegate)
	[DllImport("user32.dll")]
	public static extern bool EnumChildWindows(Wnd hWndParent, WNDENUMPROC lpEnumFunc, LPARAM lParam);

	//example delegate type declaration
	public delegate bool WNDENUMPROC(Wnd param1, LPARAM param2);

	//this is probably the most popular Windows API function, but .NET does not have a wrapper function or class that works with any window
	[DllImport("user32.dll", EntryPoint = "SendMessageW")]
	public static extern LPARAM SendMessage(Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);

	//overload example: lParam is string
	[DllImport("user32.dll", EntryPoint = "SendMessageW")]
	public static extern LPARAM SendMessage(Wnd hWnd, uint Msg, LPARAM wParam, string lParam);

	//constant example
	public const uint WM_SETTEXT = 0xC;

	//struct example
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		//of course here we can add member functions
		public override string ToString()
		{
			return string.Format("L={0} T={1} R={2} B={3}  W={4} H={5}", left, top, right, bottom, right - left, bottom - top);
		}
	}

	//and a function that uses the struct
	[DllImport("user32.dll")]
	public static extern bool GetWindowRect(Wnd hWnd, out RECT lpRect);

}

#endregion

#region my-console-program

class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		//find Notepad window
		Wnd hwnd = API.FindWindow("Notepad", null);
		Console.WriteLine(hwnd);

		//get window name
		var sb = new StringBuilder(1000);
		API.InternalGetWindowText(hwnd, sb, sb.Capacity);
		var s = sb.ToString();
		Console.WriteLine(s);

		//find first child window
		Wnd hctrl = default(Wnd);
		API.EnumChildWindows(hwnd, (h, p) => { hctrl = h; return false; }, default(LPARAM));

		//set child window text
		API.SendMessage(hctrl, API.WM_SETTEXT, default(LPARAM), "Notepad window name is " + s);

		//get window rectangle
		API.RECT r;
		API.GetWindowRect(hwnd, out r);
		Console.WriteLine(r);

		//don't close console immediately
		Console.ReadKey();
	}
}

#endregion

class TestInsertAPI
{
	//insert new declaration here

}

/*
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
*/

//Debug.Assert(false);
//Debug.Fail("fail"); //same as Debug.Assert(false, "fail");
//Debug.Write("write"); //only in debug mode (not the same as debug build); shows in VS output, not in console (unless you add a listener for it)
//Debug.WriteLine("writeline"); //the same as Debug.Write with \n
//Debug.Print("print"); //the same as Debug.WriteLine. Perhaps for VB.

//Trace.Assert(false);
//Trace.TraceError("eee"); //VS output shows "Error: 0 : eee"
//Trace.Write("write");



//static Wnd _hwndCompiler;
//static Api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate

//[STAThread]
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
//	//	Perf.Next();
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
