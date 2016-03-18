//#define NETSM

//mmm

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;
using System.Linq;

using System.Diagnostics;
using System.Threading;

using System.IO;
//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms; using K = System.Windows.Forms.Keys;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;
using Catkeys.Triggers;

//using Boo.Lang.Compiler;
//using Boo.Lang.Compiler.IO;
//using Boo.Lang.Compiler.Pipelines;

//using Cat = Catkeys.Automation.Input;
//using Meow = Catkeys.Show;


#pragma warning disable 162 //unreachable code

#if NETSM

class Test
{
	static MemoryMappedFile _mmf;
	static MemoryMappedViewAccessor _m;
	//0 event
	//8 _hwndCompiler

	static IntPtr _hwndCompiler;
	static api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	public static void _Main()
	{
	Time.First();
		IntPtr ev=api.CreateEvent(NULL, false, false, null);

		_mmf=MemoryMappedFile.CreateNew("Catkeys_SM_Tasks", 1024*1024);
		_m=_mmf.CreateViewAccessor();

		_m.Write(0, ref ev);
		
		var thr=new Thread(_AppDomainThread);
		thr.Start();

		api.WaitForSingleObject(ev, ~0U);
		//Thread.Sleep(100);
		api.CloseHandle(ev);

		_m.Read(8, out _hwndCompiler);
	Time.Next();
		
		api.SendMessage(_hwndCompiler, WM.USER, NULL, Marshal.StringToBSTR("test"));
	Time.NextOut();

		Msg("exit");
		
		api.SendMessage(_hwndCompiler, WM.CLOSE, NULL, NULL);
		//Environment.Exit(0);
	}

	static void _AppDomainThread()
	{
		var domain=AppDomain.CreateDomain("Compiler");
		//System.IO.Pipes.AnonymousPipeClientStream
		//childDomain.SetData("hPipe", handle.ToString());
		domain.DoCallBack(_DomainCallback);
		AppDomain.Unload(domain);
		//Out("_AppDomainThread() ended");
	}

	static void _DomainCallback()
	{
		//=AppDomain.CurrentDomain.GetData("hPipe")

		Util.Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

		IntPtr w=api.CreateWindowExW(0, "Catkeys_Compiler",
			null, WS.POPUP, 0, 0, 0, 0, (IntPtr)(-3),
			NULL, Window.hInst, NULL);

		_mmf=MemoryMappedFile.OpenExisting("Catkeys_SM_Tasks"); //1.5 ms
		_m=_mmf.CreateViewAccessor(); //3.5 ms. Why it is so slow? CreateOrOpen is even slower.

		_m.Write(8, ref w);
		IntPtr ev; _m.Read(0, out ev);
		api.SetEvent(ev);

		Application.Run(); //message loop
	}

	unsafe static IntPtr _WndProcCompiler(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam)
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
		case WM.USER:
			Out(Marshal.PtrToStringBSTR(lParam));
			Marshal.FreeBSTR(lParam);
			return NULL;
		}

		IntPtr R = api.DefWindowProcW(hWnd, msg, wParam, lParam);

		switch(msg) {
		case WM.NCDESTROY:
			api.PostQuitMessage(0); //Application.Exit[Thread](); does not work
			break;
		}
		return R;

		//tested: .NET class NativeWindow. It semms its purpose is different (to wrap/subclass an existing class).
	}
}





#else




#region

public struct Ptr
{
	public static implicit operator IntPtr(Ptr x) { return x._p; }
	public static implicit operator Ptr(IntPtr x) { unsafe { return *(Ptr*)(&x); } }
	//public static implicit operator int(Ptr x) { return (int)x._p; } //not useful
	public static implicit operator Ptr(int x) { unsafe { return (IntPtr)x; } } //for Ptr=0 or Ptr=HWND_MESSAGE etc

	#pragma warning disable 649 //_p never used
	IntPtr _p;
	#pragma warning restore 649
}

partial class Test
{
//[StructLayout(LayoutKind.Sequential)]
//public struct HWND
//{
//    public IntPtr Value;
//	public static readonly HWND Zero=default(HWND);
//}

//[DllImport("user32.dll",
//    SetLastError = true,
//    CallingConvention = CallingConvention.StdCall)]
//public extern static HWND FindWindowExW(
//    HWND hWndParent,
//    HWND hWndChildAfter,
//    [MarshalAs(UnmanagedType.LPWStr)] string lpszClass,
//    [MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);

//[DllImport("user32.dll",
//    SetLastError = true,
//    CallingConvention = CallingConvention.StdCall)]
//public extern static int SetForegroundWindow(
//    HWND hWnd);



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

	static IntPtr _hwndCompiler;
	static Api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	//class DialogVariables { public string lb3, c4; public string[] au; }
	//class DialogVariables { public object lb3, c4, au; }
	//here class is better than struct, because:
	//Don't need ref;
	//Can be used with modeless dialogs.

	//const string S1="one"+NL+"two"; //ok
	//const string S2=$"one{NL}two"; //error


	static void ShowDialog(object v)
	{
		FieldInfo[] a=v.GetType().GetFields();
		foreach(FieldInfo f in a)
		{
		Out(f.Name);
		//Out(f.FieldType.Equals(typeof(string)));
		switch(Type.GetTypeCode(f.FieldType)) {
		case TypeCode.String: Out("string"); break;
		case TypeCode.Object: Out("object"); break;
		}
		}
	}

	//delegate void Dee(GCHandle x);

	//static void Mee(GCHandle x)
	//{
	//Out("here"); return;
	////Out(x.IsAllocated);
	//Out(GCHandle.ToIntPtr(x));
	//if(GCHandle.ToIntPtr(x)==IntPtr.Zero) Out("null");
	//else {
	//string s=(x.Target as string);
	//Out(s);
	//}
	//}
	delegate void Dee(object x);

	static void Mee(object x)
	{
	Out(x);
	}

	[DllImport("UnmanagedDll", CallingConvention =CallingConvention.Cdecl)]
	static extern void TestUnmanaged();


	delegate void Del(int t);
	//delegate void Del0();

	class TestIndexers
	{
		//public int this[int i]
		//{
		//get { return i*i; }
		//set { Out($"{i} {value}"); }
		//}
		//public int this[int i, int j=1]
		//{
		//	get { return i*j; }
		//	set { Out($"{i} {j} {value}"); }
		//}
		public string this[string s]
		{
			get { return s+" ?"; }
			set { Out($"{s} {value}"); }
		}

		//static TestIndexers ti_=new TestIndexers();
		//public static TestIndexers Hotkey => ti_;
		//or
		public static readonly TestIndexers Hotkey=new TestIndexers();
	}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static void TestCallersVariables()
	//{
	//	Time.First();
	//	StackFrame frame = new StackFrame(1);
	//	var method = frame.GetMethod();
	//	MethodBody mb=method.GetMethodBody();
	//	int n=0;
	//	foreach(LocalVariableInfo v in mb.LocalVariables) {
	//		//Out(v.LocalType.ToString());
	//		if(v.LocalType.Name=="Hwnd") {
	//			n++;
	//			//v.
	//		}
	//	}
	//	Time.NextOut(); //400, 30, 18, 18
	//	Out(n);
	//}


#endregion

	[Trigger.Hotkey("Ctrl+K")]
	public static void Script(HotkeyTriggers.Message m) { Out("script"); }

	public static unsafe void _Main()
	{

  //      Output.Clear();

		//double d = 3.5;
		//float f = 1.2F;
		//Out(d);
		//Out(f);
		//IntPtr p = (IntPtr)7;
		//Out(p);

		//Out(",  ", d, f, p, "end");

		//string[] a1 = { "aaa", "bbb", "ccc" };
		//List<string> a2 = new List<string> { "aaa", "bbb", "ccc" };
		//int[] a3 = { 4, 5, 6 };
		//char[] a4 = { 'A', 'B', 'C' };

		//Output.Write(a1);
		//Output.Write(a2);
		//Output.Write(a3);
		//Output.Write(a4);
		//Out("----");
		//Out(a1);
		//Out(a2);
		//Out(a3);
		//Out(a4);

		//Out(1);
		//Input.Key("Ctrl+K");
		//Cat.Key("Ctrl+K");

		//Show.MessageDialog("dddd");
		//Meow.MessageDialog("dddd");

		//Test_str();

		//Out($"{(int)Control.ModifierKeys:X}");
		//Out($"{(int)Keys.Control:X}");
		//Out($"{(int)K.Control:X}");
		//Keys("");
		//Key("");
		//SendKeys


		//Out((IntPtr)WndSpec.NoTopmost);
		//Out(Wnd.Find("Untitled - Notepad"));

		//TestUtil.Test_str();

		//string s = "file.txt";
		//Out(s.likeS("*.txt"));

		////Out(s.Reverse());

		//switch(s) {
		//case "*.txt":
		//	Out("txt");
		//	break;
		//case "*.moo":
		//	Out("moo");
		//	break;
		//default:
		//	Out("none");
		//	break;
		//}

		//if(s.likeI("*.txt")) {
		//	Out("txt");
		//} else if(s.likeI("*.moo")) {
		//	Out("moo");
		//} else {
		//	Out("none");
		//}

		//if(s.endsWithI(".txt")) {
		//	Out("txt");
		//} else if(s.endsWithI(".moo")) {
		//	Out("moo");
		//} else {
		//	Out("none");
		//}

		//if(Regex.IsMatch(s, "one")) {
		//	Out("txt");
		//} else if(Regex.IsMatch(s, "two")) {
		//	Out("moo");
		//} else {
		//	Out("none");
		//}


		//Out(K.A);
		//Keys("dd");
		//Text("uu");

		//Trigger.Hotkey["Ctrl+K"] =O=> { Out("lambda"); };
		//Trigger.Hotkey["Ctrl+K"] = delegate(HotkeyTriggers.Message m) { Out("delegate"); };

		//HotkeyTriggers.TestFireTrigger();

		//var k=new TestIndexers();
		//Out(k[3]); k[7]=5;
		//Out(k[3, 4]); k[7, 2]=5;
		//Out(k[3]); k[7]=5;
		//Out(k["AAA"]); k["BBB"]="C";
		//TestIndexers.Hotkey["test"]="moo";

		//Hwnd w=0;
		////w=new Hwnd("Untitled - Notepad");
		//Out(w.ToString());

		//TestCallersVariables();
		//TestCallersVariables();
		//TestCallersVariables();
		//TestCallersVariables();

		//TestOptions();
		//TestString();

		//Del0 ku=delegate() { Out(1); };
		//ku();
		//Del0 mu= ()=>{Out(2);Out(3);};
		//mu();

		//Del g=delegate(int t) { Out(t); };
		//g+= t => Out(t);
		//g(5);

		//Del gg= t => Out(t);
		//gg(6);

		//StringBuilder sb="hhh"; //error
		//var sbb=new StringBuilder("nnn");
		return;

	//TestUnmanaged();

	//StackTrace stackTrace = new StackTrace();
	//Out(stackTrace.GetFrame(1).GetMethod().Name);
	////Out(stackTrace.GetFrame(1).GetFileLineNumber()); //always 0, even in Debug build
	//Out(stackTrace.GetFrame(1).GetFileName()); //null
	////Out(stackTrace.GetFrame(1).GetMethod(). //nothing useful




	//Dee f=Mee;
	////f(GCHandle.Alloc("test"));
	////f(GCHandle.FromIntPtr(IntPtr.Zero));
	//f("test");
	//f(5);


	//return;

	//UIntPtr ki=1;

	//switch(2)
	//{
	//	case 1:
	//	int hdh=8;
	//	break;
	//	case 2:
	//	int koop=8;
	//Out(hdh);
	//	break;
	//}

	//Out(hdh);

	//Out($"one{_}two");
	//Out("three"+_+"four");
	//Out("one" RN "two"); //error
	//Out($"one{}two"); //error


		////str controls="3"
		////var d=new DialogVariables("3") { lb3="oooo" };
		////var d=new DialogVariables("3");
		//var d=new DialogVariables();
		//d.lb3="oooo";
		//d.c4=7;
		//d.au=new string[] { "one", "two" };
		////d.au={ "one", "two" }; //error
		//ShowDialog(d);
		//return;




		//Time.WakeCPU(); //does nothing
		long t1=Stopwatch.GetTimestamp();

		//TODO: instead could simply allocate unmanaged memory with Marshal methods and pass to domains with childDomain.SetData
		_sm=new OurSharedMemory();
		_sm.Create("Catkeys_SM_Tasks", 1024*1024);

		if(true) //compiler
		{
		_sm.x->perf.AddTicksFirst(t1);
		_sm.x->perf.Next();

		IntPtr ev=Api.CreateEvent(NULL, false, false, null);

		_sm.x->eventCompilerStartup=ev;

		//Mes("before");

		var thr=new Thread(_AppDomainThread);
		thr.Start();

		_sm.x->perf.Next();

		Api.WaitForSingleObject(ev, ~0U);
		//Thread.Sleep(100);
		Api.CloseHandle(ev);

		_sm.x->perf.NextWrite();

		_hwndCompiler=_sm.x->hwndCompiler;
		
		for(int i=0; i<1; i++) {
		Api.SendMessage(_hwndCompiler, WM_.USER, NULL, Marshal.StringToBSTR("test"));
		}

		//Mes("in");
		
		Api.SendMessage(_hwndCompiler, WM_.CLOSE, NULL, NULL);
		//Environment.Exit(0);

		//Mes("after");
		//return;
		}

		//Thread.Sleep(100);
		
		//for(int i = 0; i<5; i++) {
		//	var thr2 = new Thread(_AppDomainThread2);
		//	thr2.Start();
		//	Thread.Sleep(100);
		//	if(i%10!=9) continue;
		//	//Time.First();
		//	thr2=null;
		//	GC.Collect(); //releases a lot. Without it, GC runs when Task Manager shows 100 MB.
		//				  //Time.NextOut();
		//				  //GC.Collect(2, GCCollectionMode.Optimized); //collects at 26 MB; without - at 36 MB
		//}
		//Mes("exit");
	}

	static void _AppDomainThread2()
	{
		Time.First();
		var domain=AppDomain.CreateDomain("Compiler");
		Time.Next();
		domain.ExecuteAssembly(@"C:\Test\test1.exe");
		Time.Next();
		AppDomain.Unload(domain);
		Time.NextWrite();
	}

	static void _AppDomainThread()
	{
		//_DomainCallback();

		var domain=AppDomain.CreateDomain("Compiler");
		//var domain=AppDomain.CreateDomain("Compiler", AppDomain.CurrentDomain.Evidence, new AppDomainSetup { LoaderOptimization = LoaderOptimization.MultiDomain }); //by default makes faster, but makes much slower when we use LoaderOptimization attribute on Main(). Assemblies will not be unloaded when appdomain unloaded (will use many MB of memory).
		//System.IO.Pipes.AnonymousPipeClientStream
		//childDomain.SetData("hPipe", handle.ToString());
		unsafe { _sm.x->perf.Next(); }

		domain.DoCallBack(_DomainCallback);
		//domain.ExecuteAssembly(Paths.CombineApp("Compiler.exe"));
		//domain.DoCallBack(Compiler.Compiler.Main); //faster than ExecuteAssembly by 3-4 ms
		AppDomain.Unload(domain);
		domain=null;
		//Out("_AppDomainThread() ended");
		GC.Collect(); //releases a lot
		//Mes("MinimizeMemory");
		//Misc.MinimizeMemory(); //does nothing

		//tested:
		//Currently speed and memory is similar in both cases - when compiler is in this assembly and when in another.
		//But will need to test later, when this assembly will be big.
		//Not using LoaderOptimization.MultiDomain, because then does not unload assemblies of unloaded domains (then uses much memory, and there is no sense to execute compiler in a separate domain).
	}

	//[System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.NoOptimization)]
	static unsafe void _DomainCallback()
	{
		//if(AppDomain.CurrentDomain.FriendlyName!="Compiler") return;
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
		string[] g = new string[] {null, "/nologo", "/noconfig", "/target:winexe",
		"/r:System.dll", "/r:System.Core.dll", "/r:System.Windows.Forms.dll",
		  @"C:\Test\test.cs"};
		object[] p = new object[1] { g };

		//g[0]="/?";
		Time.Next(); //16 ms
		for(int i=1; i<=4; i++)
		{
			g[0] = $@"/out:C:\Test\test{i}.exe";
			int r=(int)m.Invoke(0, p); //works, 22 ms, first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			if(r!=0) Out(r);
			Time.Next();
			//GC.Collect(); //4 ms, makes working set smaller 48 -> 33 MB
			//Time.Next();
		}
		Time.Write();

		//Mes("now will GC.Collect");
		GC.Collect(); //releases a lot

		OutLoadedAssemblies();
		Mes("exit");
	}

	static void OutLoadedAssemblies()
	{
    AppDomain currentDomain = AppDomain.CurrentDomain;
    Assembly[] assems = currentDomain.GetAssemblies();
    foreach (Assembly assem in assems){
      Out(assem.ToString());
	  Out(assem.CodeBase);
	  Out(assem.Location);
	  Out("");
    }
	}
}



#endif















		////var thr=new Thread(AppDomainThread);
		////thr.Start();
		//AppDomainThread();
		////Uuoo(1);
		////Uuoo(2);
		////Uuoo(3);
		//MessageBox.Show("main domain, tid="+Thread.CurrentThread.ManagedThreadId.ToString());


	//System.AppDomain.CreateDomain(
	//System.Collections.ArrayList k=new System.Collections.ArrayList();
	//k.Add(object
	//System.Collections.Hashtable t=new System.Collections.Hashtable();
	//t.Add(
	//System.Collections.Generic.HashSet<

//		return;
//			//Out(OptParam(b:5));

//			//try { Out(1); }catch {}

//			//for(int j=0; j<5; j++)
//			//{
//			//	TestLocal();
//			//	//Out("returned");
//			//	//GC.Collect(0, GCCollectionMode.Forced, true);
//			//	Time.First();
//			//	GC.Collect();
//			//	Time.Next();
//			//	//Out("collected");
//			//	GC.WaitForFullGCComplete();
//			//	Time.NextOut();
//			//	//Out("waited");
//			//}

//			//long g1, g2;
//			//g1=Stopwatch.GetTimestamp();
//			//Time.First();
//			////Thread.Sleep(1000);
//			//Time.NextOut();
//			//g2=Stopwatch.GetTimestamp();

//			//Out(g2-g1);
//			//return;

//			string script = @"
//import System
////import System.Runtime.InteropServices
//import Moo
//import Catkeys.Winapi

////[DllImport(""user32.dll"")]
////def MessageBox(hWnd as int, text as string, caption as string, type as int) as int:
////	pass

//static def Main():
//	i =8
//	print ""Hello, World!""
//	api.MessageBox(0, ""text $(i)"", ""cap"", 0);

//	print Class1.Add(1, 2);

//	//print ""Press a key . . . mm""; Console.ReadKey(true)
//";
////static def stringManip(item as string):
////	return ""'${item}' ? What the hell are you talking about ? ""
////";

//			for (int i = 0; i < 1; i++)
//			{
//				Stopwatch sw = new Stopwatch();
//				long t1, t2 = 0, t3 = 0, t4 = 0, t5 = 0;

//				sw.Start();
//				BooCompiler compiler = new BooCompiler();
//				//compiler.Parameters.Input.Add(new StringInput("_script_", "print('Hello!')"));
//				compiler.Parameters.Input.Add(new StringInput("Script", script + "//" + i.ToString()));
//				compiler.Parameters.Pipeline = new CompileToMemory();
//				//compiler.Parameters.Pipeline = new CompileToFile();
//				//compiler.Parameters.Ducky = true;
//				//Out(compiler.Parameters.BooAssembly.FullName);
//				//Out(compiler.Parameters.Debug);
//				compiler.Parameters.Debug = false; //default true; 20% faster when Release
//				//compiler.Parameters.Environment.Provide.
//				//compiler.Parameters.GenerateInMemory=false; //default is true even if new CompileToFile()
//				//Out(compiler.Parameters.GenerateInMemory);
//				//Out(compiler.Parameters.OutputAssembly);
//				//compiler.Parameters.OutputAssembly=@"q:\test\boo.dll";

//				compiler.Parameters.AddAssembly(Assembly.LoadFile(@"Q:\test\Moo.dll")); //ok
//				//compiler.Parameters.LoadAssembly(@"Q:\test\Moo.dll", true); //no effect
//				//compiler.Parameters.LoadReferencesFromPackage(@"Q:\test\Moo.dll"); //error
//				//compiler.Parameters.References.Add(Assembly.LoadFile(@"Q:\test\Moo.dll")); //ok
//				compiler.Parameters.AddAssembly(Assembly.LoadFile(@"C:\Users\G\Documents\SharpDevelop Projects\Test\Winapi\bin\Release\Winapi.dll"));

//				CompilerContext context = compiler.Run();
//				t1 = sw.ElapsedTicks;
//				//Note that the following code might throw an error if the Boo script had bugs.
//				//Poke context.Errors to make sure.
//				if (context.GeneratedAssembly != null)
//				{
//					//SaveAssembly(context.GeneratedAssembly, @"q:\test\boo.exe");
//					//Out(context.GeneratedAssembly.FullName);
//					//Out(context.GeneratedAssembly.EntryPoint.ToString()); //void Main()
//					//Out(context.GeneratedAssembly.);

//					Type scriptModule = context.GeneratedAssembly.GetType("ScriptModule");
//					//Out(scriptModule == null);
//					MethodInfo met = scriptModule.GetMethod("Main");

//					//MethodInfo[] a = scriptModule.GetMethods();
//					//foreach(MethodInfo m in a)
//					//{
//					//Out(m.Name);
//					//}

//					met.Invoke(null, null);

//					//string output = (string)stringManip.Invoke(null, new object[] { "Tag" });
//					//Out(output);
//				}
//				else
//				{
//					foreach (CompilerError error in context.Errors)
//						Out(error);
//				}

//				double f = Stopwatch.Frequency / 1000000.0;
//				Out("speed: {0} {1} {2} {3} {4}", (long)(t1 / f), (long)((t2 - t1) / f), (long)((t3 - t2) / f), (long)((t4 - t3) / f), (long)((t5 - t4) / f));
//			}
//			Out("Press a key . . . ");
//			Console.ReadKey(true);
	//}

	//static void SaveAssembly(Assembly a, string file)
	//{
	//	using (FileStream stream = new FileStream(file, FileMode.Create))
	//	{
	//		BinaryFormatter formatter = new BinaryFormatter();

	//		formatter.Serialize(stream, a); //error, assembly not marked as serializable
	//	}
	//}

