using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Compiler
{
public class Compiler
{
	struct _SHMEM {
	public IntPtr eventCompilerStartup;
	public IntPtr hwndCompiler;
	public Time.PerformanceCounter perf;
	}

	//We don't use MemoryMappedFile because it is very slow. Creating/writing is 1500, opening/reading is 5000.
	//With this class - 1300 and 600 (because of JIT). With ngen - 60 and 20 (same as in C++).
	unsafe class OurSharedMemory :SharedMemoryFast
	{
	public _SHMEM* x { get { return (_SHMEM*)_mem; } }
	}

	static OurSharedMemory _sm;

	//static IntPtr _hwndCompiler;
	static Api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	public static unsafe void Main()
	{
		//Msg("Compiler");

		long t1= Stopwatch.GetTimestamp();

		_sm=new OurSharedMemory();
		_sm.Open("Catkeys_SM_Tasks");

		_sm.x->perf.AddTicksNext(t1);
		_sm.x->perf.Next();

		//=AppDomain.CurrentDomain.GetData("hPipe")

		Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

		_sm.x->perf.Next();

		IntPtr w=Api.CreateWindowExW(0, "Catkeys_Compiler",
			null, WS_.POPUP, 0, 0, 0, 0, (IntPtr)(-3),
			NULL, Window.hInstModule, NULL);

		_sm.x->perf.Next();

		_SHMEM* x=_sm.x;
		x->hwndCompiler=w;
		Api.SetEvent(x->eventCompilerStartup);

		//message loop
		//Application.Run(); //By default would add several ms to the startup time. Same speed if Main() has the LoaderOptimization attribute. Also may be not completely compatible with native wndproc. Also in some cases adds several MB to the working set.
		Api.MSG m;
		while(Api.GetMessage(out m, NULL, 0, 0)>0) { Api.DispatchMessage(ref m); }
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
			//Out(Marshal.PtrToStringBSTR(lParam));
			Marshal.FreeBSTR(lParam);
			TestRoslyn();
			return NULL;
		}

		IntPtr R = Api.DefWindowProcW(hWnd, msg, wParam, lParam);

		switch(msg) {
		case WM_.NCDESTROY:
			Api.PostQuitMessage(0); //Application.Exit[Thread](); does not work
			break;
		}
		return R;

		//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	}

	static void TestRoslyn()
	{
		//Out("test");
		//TODO: auto-ngen compiler. Need admin.

		Time.First();
		//System.Runtime.ProfileOptimization.SetProfileRoot(@"C:\Test");
		//System.Runtime.ProfileOptimization.StartProfile("Startup.Profile"); //does not make jitting the C# compiler assemblies faster
		//Time.Next();

		//Assembly a = Assembly.LoadFile(@"Q:\Test\Csc\csc.exe"); //error
		//Assembly a = Assembly.LoadFile(@"C:\Program Files (x86)\MSBuild\14.0\Bin\csc.exe"); //error
		Assembly a = Assembly.LoadFile(Paths.CombineApp("csc.exe")); //ok
		//Assembly a = Assembly.Load("csc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"); //works, same speed as LoadFile, but VS shows many warnings if this project uses different .NET framework version than csc (which is added to project references). Also, possibly could make the app start slower, especially if HDD. Better to load on demand through reflection.
		MethodInfo m = a.EntryPoint;
		string[] g = new string[] {null, "/nologo", "/noconfig", @"C:\Test\test.cs"};
		object[] p = new object[1] { g };

		//g[0]="/?";
		Time.Next(); //16 ms
		for(int i=1; i<=4; i++)
		{
			g[0] = $@"/out:C:\Test\test{i}.exe";
			m.Invoke(0, p); //works, 22 ms, first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			Time.Next();
			//GC.Collect(); //4 ms, makes working set smaller 48 -> 33 MB
			//Time.Next();
		}
		Time.Write();

		Mes("now will GC.Collect");
		GC.Collect(); //releases a lot
	}
}
}
