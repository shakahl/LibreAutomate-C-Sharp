//#define NETSM

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Linq;

using System.Diagnostics;
using System.Threading;

using System.IO;
//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using K = System.Windows.Forms.Keys;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;
using Catkeys.Triggers;


//using Boo.Lang.Compiler;
//using Boo.Lang.Compiler.IO;
//using Boo.Lang.Compiler.Pipelines;

//using Cat = Catkeys.Automation.Input;
//using Meow = Catkeys.Show;

//using static Catkeys.Show;

#pragma warning disable 162 //unreachable code

#if NETSM

class Test
{
	static MemoryMappedFile _mmf;
	static MemoryMappedViewAccessor _m;
	//0 event
	//8 _hwndCompiler

	static Wnd _hwndCompiler;
	static api.WndProc _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	public static void _Main()
	{
	Time.First();
		IntPtr ev=api.CreateEvent(Wnd0, false, false, null);

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
		
		_hwndCompiler.Send(WM.USER, Zero, Marshal.StringToBSTR("test"));
	Time.NextOut();

		Msg("exit");
		
		_hwndCompiler.Send(WM.CLOSE);
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

		Wnd w=api.CreateWindowExW(0, "Catkeys_Compiler",
			null, WS.POPUP, 0, 0, 0, 0, Wnd.Spec.Message,
			Zero, Zero, Zero);

		_mmf=MemoryMappedFile.OpenExisting("Catkeys_SM_Tasks"); //1.5 ms
		_m=_mmf.CreateViewAccessor(); //3.5 ms. Why it is so slow? CreateOrOpen is even slower.

		_m.Write(8, ref w);
		IntPtr ev; _m.Read(0, out ev);
		api.SetEvent(ev);

		Application.Run(); //message loop
	}

	unsafe static LPARAM _WndProcCompiler(Wnd hWnd, WM msg, LPARAM wParam, LPARAM lParam)
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
			return Zero;
		}

		LPARAM R = api.DefWindowProcW(hWnd, msg, wParam, lParam);

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



public partial class Test
{

	#region

	struct _SHMEM
	{
		public IntPtr eventCompilerStartup;
		public Wnd hwndCompiler;
		public Time.PerformanceCounter perf;
	}

	//We don't use MemoryMappedFile because it is very slow. Creating/writing is 1500, opening/reading is 5000.
	//With this class - 1300 and 600 (because of JIT). With ngen - 60 and 20 (same as in C++).
	unsafe class OurSharedMemory :Util.SharedMemoryFast
	{
		public _SHMEM* x { get { return (_SHMEM*)_mem; } }
	}

	static OurSharedMemory _sm;

	static Wnd _hwndCompiler;
	static Api.WNDPROC _wndProcCompiler = _WndProcCompiler; //use static member to prevent GC from collecting the delegate


	//class DialogVariables { public string lb3, c4; public string[] au; }
	//class DialogVariables { public object lb3, c4, au; }
	//here class is better than struct, because:
	//Don't need ref;
	//Can be used with modeless dialogs.

	//const string S1="one"+NL+"two"; //ok
	//const string S2=$"one{NL}two"; //error


	static void ShowDialog(object v)
	{
		FieldInfo[] a = v.GetType().GetFields();
		foreach(FieldInfo f in a) {
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
	//if(GCHandle.ToIntPtr(x)==Zero) Out("null");
	//else {
	//string s=(x.Target as string);
	//Out(s);
	//}
	//}
	delegate void Dee(object x);

	[DllImport("UnmanagedDll", CallingConvention = CallingConvention.Cdecl)]
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
			get { return s + " ?"; }
			set { Out($"{s} {value}"); }
		}

		//static TestIndexers ti_=new TestIndexers();
		//public static TestIndexers Hotkey => ti_;
		//or
		public static readonly TestIndexers Hotkey = new TestIndexers();
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


	[Trigger.Hotkey("Ctrl+K")]
	public static void Function1(HotkeyTriggers.Message m) { Out("script"); }

	#endregion

	//static void AnotherThread()
	//{
	//	//Show.TaskDialog("another thread", "", "", x:1);
	//	MessageBox.Show("another thread");
	//	Out("after msgbox in another thread");
	//}

	//[DllImport("comctl32.dll", EntryPoint = "TaskDialog")]
	//static extern int _TaskDialog(Wnd hWndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, TDButton dwCommonButtons, LPARAM pszIcon, out int pnButton);

	//static int TD(string s, bool asy)
	//{
	//	int r = 0;
	//	for(int i = 0; i < 100; i++) {
	//		int hr = _TaskDialog(Wnd0, Zero, "Test", s, null, TDButton.Cancel, 0, out r);
	//		//OutList(hr, r, asy);
	//		if(hr == 0 || hr == Api.E_INVALIDARG) break;
	//		Time.WaitMS(20);
	//	}
	//	return r;
	//}

	public static async Task<TDResult> TaskDialogAsync(string text1)
	{
		return await Task.Run(() => Show.TaskDialog(text1, "", "OC"));
	}

	public static TaskDialogObject TaskDialogNoWait(string text1, Action<TDResult> onClose = null)
	{
		var d = new TaskDialogObject(text1);
		d.ShowAsync(onClose);
		return d;
	}

	public static TaskDialogObject TaskProgressDialog(string text1)
	{
		var d = new TaskDialogObject(text1);
		d.FlagShowProgressBar = true;
		d.ShowAsync();
		return d;
	}

	//	static System.Drawing.Font _dialogFont;
	//	public static System.Drawing.Font DefaultDialogFont
	//	{
	//		get
	//		{
	//			if(_dialogFont == null) {
	//				Form.DefaultFont
	//				System.Drawing.Font.
	//_dialogFont =new System.Drawing.Font()
	//            }
	//			return _dialogFont;
	//		}
	//	}


	//[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
	static void TestX()
	{
		#region commented
		//Out(AppDomain.CurrentDomain.FriendlyName);

		//new Thread(AnotherThread).Start();

		//Show.TaskDialog("appdomain primary thread", "", "e");

		////Thread.CurrentThread.Abort();
		////AppDomain.Unload(AppDomain.CurrentDomain);
		//Out("after TaskDialog");

		//return;

		//Show.TaskDialog("", "<a href=\"test\">test</a>", onLinkClick: ed =>
		//{
		//	Wnd z = ed.Hwnd;
		//	string s = null;
		//	Time.First(true);
		//	for(int j = 0; j<8; j++) {
		//		for(int i=0; i<1000; i++) s= z.ClassName;
		//		//for(int i=0; i<1000; i++) s= z.Name;
		//		//for(int i = 0; i<1000; i++) s= z.ControlText;
		//		//for(int i = 0; i<1000; i++) s= z.ControlTextLength;
		//		Time.Next();
		//	}
		//	Time.Write();
		//	Out(s);
		//}
		//);

		//return;

		////Time.Wait(1);
		//Wnd z = Wnd.Find("Untitled - Notepad");
		////z=(Wnd)(IntPtr)2098486; //Inno
		////z=(Wnd)(IntPtr)395896; //Static
		////z=(Wnd)(IntPtr)1510052; //Edit

		////z.Name = "MMMMMMMMGGGG"; return;

		////string m = z.Name;
		////string m = z.GetControlText();
		////OutList(m==null, m=="", m);
		////return;

		//Out(z);

		//string cn = null;
		////cn= z.ControlText; OutList(cn.Length, cn); return;
		//int nt = 0;

		//Time.First(true);
		//for(int j = 0; j<8; j++) {
		//	//for(int i=0; i<1000; i++) cn= z.ClassName;
		//	//for(int i=0; i<1000; i++) cn= z.Name;
		//	for(int i = 0; i<1000; i++) cn= z.GetControlText();
		//	Time.Next();
		//}
		//Time.Write();
		//Out(cn);
		//Out(nt);

		//Wnd ww = Wnd.Find("Untitled - Notepad");
		//ww.MoveInScreen(100, 100);
		//return;

		//OutFunc();
		//Out(FunctionName());
		//Output.WriteHex((sbyte)(-10));
		//OutList(1, "mmm", true, null, 5.6);
		//Out("ff");
		//Print(5);
		//PrintList(1, 2, 3);
		//return;

		//Output.Clear();
		//Screen s = null;
		////Output.Write(s);
		////Console.WriteLine(s);
		//Info("stri");
		//Info(5);
		//Info(true);
		//Info(new char[] { 'a', 'b' });
		//Info(new string[] { "aaa", "bbb" });
		//Info(new int[] { 'a', 'b' });
		//Info(new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } });
		//Out("----------");
		//Print("stri");
		//Print(5);
		//Print(true);
		//Print(new char[] { 'a', 'b' });
		//Print(new string[] { "aaa", "bbb" });
		//Print(new int[] { 'a', 'b' });
		//Print(new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } });

		////Time.First(true);
		////for(int j=0; j<5; j++) {
		////	for(int i = 0; i<1000; i++) Info2("ff");
		////	Time.Next();
		////}
		////Time.Write();

		//return;




		//Wait(2);
		//Info("bla");
		//Out("bla");
		//Say("bla");
		//Print("bla");
		//OW("bla");

		//Info("bla"); Warning("bla"); Error("bla");



		//Wnd w = Wnd.Find("Untitled - Notepad");
		////Wnd w2 = Wnd.Find("Registry Editor");

		////w.MoveInScreen(0, 0, null, limitSize:true, rawXY:false);
		////w.EnsureInScreen(null, limitSize:true, workArea:true);

		//RECT k=new RECT(0, 1700, 5000, 400, true);
		////RECT k=new RECT(-1, -1, 500, 400, true);

		////Wnd.RectMoveInScreen(ref k, limitSize:true);
		//Wnd.RectEnsureInScreen(ref k, limitSize:true);
		//Out(k);

		////w=Wnd.Spec.NoTopmost;
		//Out(Screen_.FromObject(w));

		////Screen s1 = Screen_.FromObject(w);
		////Screen s2 = Screen_.FromObject(w);
		//Screen s1 = Screen.PrimaryScreen;
		//Screen s2 = Screen.PrimaryScreen;
		//Out(s1==s2);
		//Out(s1.Equals(s2));


		//IntPtr hm = DisplayMonitor.GetHandle(w);
		////hm=DisplayMonitor.GetHandle(2);
		////hm=DisplayMonitor.GetHandle(DisplayMonitor.OfMouse);
		////hm=DisplayMonitor.GetHandle(new POINT(2000, 2000));
		////hm=DisplayMonitor.GetHandle(new RECT(2000, 2000, 100, 100, true));
		//Out(hm);

		//for(int z=0; z<2; z++) {
		//	Screen[] ad = Screen.AllScreens;
		//	foreach(Screen k in ad) {
		//		RECT rr = k.Bounds;
		//		Out(rr);
		//	}
		//	Show.MessageDialog("aaa");
		//}

		//Time.First(true);
		//for(int rep1=0; rep1<5; rep1++) {
		//	//for(int rep2=0; rep2<100; rep2++) { RECT u1 = DisplayMonitor.GetRectangle(2); }
		//	for(int rep2=0; rep2<100; rep2++) { RECT u2 = ScreenFromIndex(2).Bounds; }
		//	Time.Next();
		//}
		//Time.Write();

		//Screen k = ScreenFromIndex(1);
		//Out(k.Bounds);


		//return;

		//RECT r1; System.Drawing.Rectangle r2;

		//Api.GetWindowRect(w, out r1);
		//GetWindowRect(w, out r2);

		//OutList(r1, r2);

		//return;


		//var r1 = new RECT();
		//var r2 = r1;
		//var r3 = RECT.LTRB(1, 8, 10, 50);
		//var r4 = RECT.LTWH(1, 8, 10, 50);
		//var r5 = new RECT() { left=2, top=20, Width=2, Height=10 };

		//Out(r2==r1);

		//Out(r3);
		//Out(r4);
		//Out(r5);

		//return;

		//Wnd w = Wnd.Spec.Bottom;
		//Out(w.Equals(Wnd.Spec.Bottom));
		//Wnd w2 = w;
		//Out(w.Equals(w2));
		////Wnd w = Wnd0;
		////Out(w.Equals(Wnd0));

		//Wnd wg = Wnd.Get.FirstToplevel();

		//return;

		//int eon, x = "ab 99 hjk".ToInt_(2, out eon);
		//OutList(x, eon);
		//int x = "ab 99 hjk".ToInt_(2);
		//int x = " 99 hjk".ToInt_();
		//OutList(x);
		//int eon, x = " 99 hjk".ToInt_(out eon);
		//OutList(x, eon);

		//return;

		//#if NEWRESULT
		//try {
		//Thread.Sleep(5000);

		//Api.MessageBox(Wnd0, "dd", "ggg", 0x40000);
		//return;

		//Script.Option.dialogRtlLayout=true;
		//Script.Option.dialogTopmostIfNoOwner=true;

		//var asm = Assembly.GetEntryAssembly(); //fails if DoCallBack or CreateInstance, OK if ExecuteAssembly
		//var asm = Assembly.GetExecutingAssembly(); //OK
		//Out(asm!=null);
		//Out(asm.Location);
		////var rm = new System.Resources.ResourceManager("", asm);
		////Out(rm);
		//return;

		//ScriptOptions.DisplayName="Script display name";
		//Out(Assembly.GetEntryAssembly().FullName); //exception

		//Wnd ko = Wnd0;
		////ko = Wnd.Spec.Topmost;
		//Out(ko == null);
		//Out(null==ko);
		//Wnd? mo = null, mo2=null;
		//Out(ko == mo);
		//Out(mo == mo2);
		//POINT po = new POINT();
		//Out(po == null);
		////int io = 0;
		////Out(io == null);
		//IntPtr pi = Zero;
		//Out(pi == null);
		//return;

		#endregion


		//Show.TaskListDialog("1|2|3|4|5|6|7|8|9|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30");
		Show.TaskListDialog("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA BBBBBBBBBBBBBBBBBBBBBBBBB");
		return;

		//var f = new Form();
		//f.Show();

		string s; int i;
		//if(!Show.InputDialog(out s)) return;
		//if(!Show.InputDialog(out s, "Text.", owner: Wnd.Find("Untitled - Notepad"))) return;
		if(!Show.InputDialog(out s, "Text\ntext\ntext.")) return;
		//if(!Show.InputDialog(out s, "Text.", "Default")) return;
		//if(!Show.InputDialogEx(out s, "Text.", "0", editType: TDEdit.Number, expandedText:"exp")) return;
		//if(!Show.InputDialog(out i, "Text.", 5)) return; Out(i); return;
		//if(!Show.InputDialogEx(out s, "Text.", "pas", editType: TDEdit.Password)) return;
		//if(!Show.InputDialog(out s, "Text.", "one\r\ntwo\r\nthree", editType: TDEdit.Multiline)) return;
		//if(!Show.InputDialog(out s, "Text.", "def\none\r\ntwo\nthree", editType: TDEdit.Combo)) return;
		//if(!Show.InputDialogEx(out s, "Text.", "def\none\r\ntwo\nthree", editType: TDEdit.Combo, expandedText:"exp")) return;
		//if(!Show.InputDialogEx(out s, "Text.")) return;
		bool ch;
		//if(!Show.InputDialogWithCheckboxEx(out s, out ch, "Check", "Text.", "one\r\ntwo\r\nthree", editType:TDEdit.Multiline, expandedText:"More\ntext.", timeoutS: 60)) return;
		//if(!Show.InputDialogWithCheckbox(out s, out ch, "Check", "Text.", "txt", editType:TDEdit.Multiline)) return;
		Out(s);
		//Out(ch);

		//if(false) {
		//	d.SetCustomButtons("1 Browse...", true, true);
		//	d.ButtonClicked += e => { if(e.WParam == 1) { Out("Browse"); e.Return = 1; } };
		//}

		//MessageBox.Show("ddddddddddddddddddd");
		//Show.MessageDialog("fffffffffff");

		return;


		//TaskDialogAsync("async"); //warning "consider applying await"
		//TDResult rr=await TaskDialogAsync("async"); //error, the caller must be marked with async. But then fails to run altogether.

		//Task<TDResult> t=TaskDialogAsync("async");
		//Show.TaskDialog("continue");
		//t.Wait();
		//Out(t.Result);

		//var pd = TaskDialogNoWait("async", y=>Out(y));
		//var pd = TaskDialogNoWait("async");
		//Wait(2);
		//if(pd.IsOpen) pd.Send.Close();
		//Out(pd.Result);

		//var pd = TaskProgressDialog("progress");
		//for(int i = 1; i <= 100; i++) {
		//	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		//	pd.Send.Progress(i);
		//	WaitMS(50); //do next part of your work in the loop
		//}
		//pd.Send.Close();

		//var td = new TaskDialogObject("dddd");
		//Task.Run(() => td.Show());
		////Time.First();
		////td.ThreadWaitOpen();
		////Time.NextWrite();
		//td.ThreadWaitClosed();
		////Task.Run(() => td.Show());
		////td.ThreadWaitOpen();
		////td.ThreadWaitClosed();
		//Out(td.Result);

		//Show.TaskDialog("continue", y:300);


		//Out("finished");

		//Task t = Task.Run(() =>
		//{
		//	//Thread.Sleep(100);

		//	//Out("run");
		//	Out(Show.TaskDialog("async", style: "OC", x: 1));
		//	//MessageBox.Show("another thread");
		//	//Show.MessageDialog("async",style:"OC");
		//	//TD("async", true);
		//}
		//);

		////Thread.Sleep(7);

		//Out(Show.TaskDialog("continue", style: "OC"));
		////TD("continue", false);
		////Show.MessageDialog("continue",style:"OC");
		////Thread.Sleep(1000);
		//t.Wait();
		//Out("after all");

		//for(int i=0; i<5; i++) TD("continue", false);

		//Out(GetThemeAppProperties());
		//MessageBox.Show("sss");
		//Out(Show.MessageDialog("test", MDButtons.OKCancel, MDIcon.App, MDFlag.DefaultButton2));
		//Out(Show.MessageDialog("One\ntwooooooooooooo."));
		//Out(Show.MessageDialog("One\ntwooooooooooooo.", "YNC!t2"));
		//Show.MessageDialog("One\ntwooooooooooooo.");
		//Out(Wnd.ActiveWindow);

		//Out(Show.TaskDialog(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButton.OKCancel, TDIcon.App, TDFlag.CommandLinks, TDResult.Cancel, "1 one|2 two", new string[] { "101 r1|||", "102 r2" }, "Chick|check", "expanded", "", "TTT", 0, 0, 20).ToString());
		//Out(Show.TaskDialog("Head1\nHead2.", "Text1\nText2.", "OCd2!t", Wnd0, "1 one|2 two", null, null, "expanded", "foo", 60, "TTT").ToString());
		//Out(Show.TaskDialog(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButton.OKCancel|TDButton.YesNo|TDButton.Retry|TDButton.Close, TDIcon.Info).ToString());
		//Out(Show.TaskDialog(Wnd0, "Head1\nHead2.", "Text1\nText2.", TDButton.OKCancel|TDButton.YesNo|TDButton.Retry|TDButton.Close, (TDIcon)0xfff0).ToString());
		//Out(Show.TaskDialog("head", "content", "OCYNLRio", owner: Wnd.Find("Untitled - Notepad")).ToString());
		//Out(Show.TaskDialog("head", "content", "OCYNLRi", x:100, y:-11, timeoutS:15).ToString());
		//Out(Show.TaskDialog("", "<a href=\"example\">link</a>.", onLinkClick: ed => { Out(ed.LinkHref); }).ToString());
		Out(Show.TaskDialog("head", "content", "i", customButtons: "-1 Mo OK|-2 My Cancel").ToString());

		//Out(Show.TaskListDialog("1 one| 2 two| 3three|4 four\nnnn|5 five|6 six|7 seven|8 eight|9 nine|10Ten|0Cancel|1 one|2 two|3three|4 four\nnnn|5 five|6 six|7 seven|8 eight|9 nine|10Ten", "Main", "More."));
		//Out(Show.TaskListDialog(new string[] { "1 one", "2 two", "Cancel" }, "Main", "More").ToString());
		//Out(Show.TaskListDialog(new List<string> { "1 one", "2 two", "Cancel" }, "Main", "More").ToString());
		////		Out(Show.TaskListDialog(@"
		////|1 one
		////|2 two
		////comments
		////|3 three
		////" , "Main", "More\r\nmore"));

		////		Out(Show.TaskListDialog("1 one|2 two\nN|3 three\r\nRN|4 four"));
		return;

		//var d = new TaskDialogObject("Head", "Text <A HREF=\"xxx\">link</A>.", TDButton.OKCancel|TDButton.Retry, TDIcon.Shield, "Title");
		//var d = new TaskDialogObject("Head", "Text <A HREF=\"xxx\">link</A>.", TDButton.OKCancel|TDButton.Retry, (TDIcon)0xfff0, "Title");
		//var d = new TaskDialogObject("Head", "Text <A HREF=\"xxx\">link</A>.", (TDButton)111);
		//var d = new TaskDialogObject("Head Text.", null, 0, TDIcon.Shield);
		//var d = new TaskDialogObject("", "More text.", 0, TDIcon.Shield);
		//var d = new TaskDialogObject();
		var d = new TaskDialogObject();

		d.SetTitleBarText("MOO");

		d.SetText("Main text.", "More text.\nSupports <A HREF=\"link data\">links</A> if you subscribe to HyperlinkClick event.");

		d.SetButtons(TDButton.OKCancel | TDButton.Retry);

		//d.SetIcon(TDIcon.Warning);
		//d.SetIcon(new System.Drawing.Icon(@"Q:\app\copy.ico", 32, 32)); //OK
		//d.SetIcon(Catkeys.Tasks.Properties.Resources.output); //OK
		//d.SetIcon(new System.Drawing.Icon(Catkeys.Tasks.Properties.Resources.output, 16, 16)); //OK
		//d.SetIcon(new System.Drawing.Icon("Resources/output.ico")); //OK
		//d.SetIcon(new System.Drawing.Icon("Resources/output.ico", 16, 16)); //OK
		//d.SetIcon(new System.Drawing.Icon(typeof(Test), "output.ico")); //exception
		//Out(Catkeys.Tasks.Properties.Resources.output.Width);
		//d.SetIcon(new System.Drawing.Icon(Show.Resources.AppIconHandle32));
		//d.SetIcon(TDIcon.App);

		d.SetStyle("YNCa");

		Wnd w = Wnd.Find("Untitled - Notepad");
		//d.SetOwnerWindow(w);

		//Script.Option.dialogScreenIfNoOwner=2;

		d.SetXY(100, 100);

		d.SetCheckbox("Checkbox", false);

		//Script.Option.dialogTopmostIfNoOwner=true;
		//d.FlagTopmost=false;
		d.FlagAllowCancel = true;
		d.FlagCanBeMinimized = true;
		//d.FlagRtlLayout=true;
		//d.FlagPositionRelativeToWindow=true;
		d.FlagNoTaskbarButton = true;
		d.FlagNeverActivate = true;

		//Script.Option.dialogScreenIfNoOwner=2;
		//d.Screen=2;

		d.SetExpandedText("Expanded info\nand more info.", true);

		d.SetExpandControl(true, "Show more info", "Hide more info");

		d.SetFooterText("Footer text.", TDIcon.Warning);
		//d.SetFooterText("Footer.");
		//d.SetFooterText("Footer.", Catkeys.Tasks.Properties.Resources.output); //icon 32x32, srinked
		//d.SetFooterText("Footer.", new System.Drawing.Icon(Catkeys.Tasks.Properties.Resources.output, 16, 16)); //icon OK

		//d.Width=700;

		//d.SetCustomButtons(new string[] { "101 one", "102 two" });
		d.SetCustomButtons("1 one|2 two\nzzz", true);
		//d.SetCustomButtons(new string[] { "5", "102 two" }, true);
		//d.SetCustomButtons("101|102 two\nzzz", true);
		d.DefaultButton = TDResult.No;
		//d.SetDefaultButton(2);
		//d.SetDefaultButton(TDResult.Cancel);
		//d.SetDefaultButton(TDResult.Retry);

		//d.SetRadioButtons(new string[] { "1001 r1", "1002 r2" }, 1002);
		d.SetRadioButtons("1001 r1|1002 r2");

		//d.SetTimeout(10, "Cancel");
		//d.SetTimeout(10, null, true);

		//d.Created += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); };
		d.Created += ed => { ed.Obj.Send.EnableButton(TDResult.Yes, false); };
		//d.Created += ed => { ed.OwnerWindow.Enabled=true; };
		//d.Created += ed => { ed.Hwnd.Owner.Enabled=true; };
		//d.Created += ed => { Wnd.Get.Owner(ed.Hwnd).Enabled=true; };
		//d.Created += ed => { w.Enabled=true; };
		//d.Destroyed += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); };
		d.ButtonClicked += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); if(ed.WParam == TDResult.No) ed.Return = 1; };
		d.HyperlinkClicked += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); };
		//d.OtherEvents += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); };
		//d.Timer += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); };
		//d.HelpF1 += ed => { Out($"{ed.Message} {ed.WParam} {ed.LinkHref}"); };

		d.FlagShowProgressBar = true; d.Timer += ed => ed.Obj.Send.Progress(ed.WParam / 100);

		//Time.First();
		TDResult r = d.Show();
		//Time.NextWrite();

		Out(r.ToString());

		//} catch(ArgumentException e) { Out($"ArgumentException: {e.ParamName}, '{e.Message}'"); } catch(Exception e) { Out($"Exception: '{e.Message}'"); }
		//#endif
	}

	//static void ScriptDomainThread()
	//{

	//}

	public Test()
	{
		TestX();
	}

	static void TestInScriptDomain()
	{
		var domain = AppDomain.CreateDomain("Test");

		try {

			//domain.ExecuteAssembly(@"..\Test Projects\ScriptClass\bin\Debug\ScriptClass.exe");

			//domain.DoCallBack(TestX);
			//Out("after domain.DoCallBack");

			domain.CreateInstance("CatkeysTasks", "Test");
			//domain.CreateInstanceFrom("CatkeysTasks.exe", "Test");
			//Out("after domain.CreateInstance");

		} finally {
			AppDomain.Unload(domain);
			//Out("after AppDomain.Unload(domain)");
		}
	}
	//static void TestInScriptDomain()
	//{
	//	var domain = AppDomain.CreateDomain("Test");
	//	//domain.ExecuteAssembly(@"..\Test Projects\ScriptClass\bin\Debug\ScriptClass.exe");
	//	domain.DoCallBack(TestX); //Assembly.GetEntryAssembly() fails. OK if ExecuteAssembly.
	//	Out("after domain.DoCallBack");
	//	AppDomain.Unload(domain);
	//	Out("after AppDomain.Unload(domain)");
	//}


	static unsafe void TestAny()
	{
		Output.Clear();

		//TestX();
		//TestInScriptDomain();
		var t = new Thread(TestInScriptDomain);
		t.SetApartmentState(ApartmentState.STA); //must be STA, or something will not work, eg some COM components, MSAA in TaskDialog.
		t.Start();
		t.Join();
		//Show.TaskDialog("after all");
		//Out("after all");

		//Out(sizeof(WPARAM));

		////void* b = (void*)1000000;
		////IntPtr b = (IntPtr)(-1);
		////UIntPtr b = (UIntPtr)(0xffffffff);
		//int b = -1;
		////uint b = 0xffffffff;
		////byte b = 5;
		////sbyte b = -5;
		////ushort b = 5;
		////short b = -5;
		////char b = 'A';
		////WPARAM b = -1;

		////IntPtr b=(IntPtr)(-1);
		////UIntPtr b=(UIntPtr)(0xffffffff);
		////uint b = 5;
		////char b = 'A';

		//LPARAM x;
		//x=b;
		////x=1000;
		//b=x;

		////uint u =0xffffffff;
		////int x = (int)u;
		////WPARAM y = u;

		//Out("OK");
		//Out($"{x} {b}");
		////Out($"{x} {((int)b).ToString()}");
		////Out(x==-1);
		////Out(x+4);


		////string s = " 10 more";
		//string s = "-10 more";
		////string s = "0x10 more";
		////string s = "-0x10";
		//s=" 15 text";

		////Out(Convert.ToInt32(s));
		////Out(int.Parse(s));
		////Out(SplitNumberString(ref s));
		////Out($"'{s}'");

		//int len, r = s.ToInt_(out len);
		//Out($"{r} 0x{r:X} {len}");

		//string tail;
		//r=s.ToInt_(out tail);
		//Out($"{r} 0x{r:X} '{tail}' {tail==null}");

		//Time.SpinCPU();
		//int i, j, n1=0, n2=0;
		//for(j=0; j<4; j++) {
		//	Time.First();
		//	for(i=0; i<1000; i++) n1+=int.Parse(s);
		//	Time.Next();
		//	for(i=0; i<1000; i++) n2+=ToInt_(s, out len);
		//	Time.NextWrite();
		//}
		//Out($"{n1} {n2} {len}");

		//string[] a = { "one", "two" };
		//Out(a);

		//var d = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } };
		//Out(d);

		//var k = new Dictionary<string, string>() { { "A", "B" }, { "C", "D" } };
		////Out(k);
		//Output.Write(k);

		//Redirect();
		//Thread.Sleep(100);

		//Output.Writer=new MooWriter();

		//Time.SpinCPU();
		//int i, n=10;
		//for(int j=0; j<3; j++) {

		//	Time.First();
		//	for(i=0; i<n; i++) Out("out");
		//	Time.Next();
		//	for(i=0; i<n; i++) Console.WriteLine("con");
		//	//Time.Next();
		//	//for(i=0; i<n; i++) Trace.WriteLine("tra");
		//	Time.NextWrite();
		//}
		//speed: Write unbuffered 35, Console.WriteLine 30, Trace.WriteLine (debug mode) 900

		////Time.First(true);
		////Output.AlwaysOutput=true;
		//Output.RedirectConsoleOutput();
		//Output.RedirectDebugOutput();
		////Time.NextWrite();

		////Console.Write("{0} {1}", 1, true);
		////return;
		//Out("out");
		//Console.WriteLine("con");
		//Trace.WriteLine("tra");
		////Thread.Sleep(1000); try { Console.Clear(); } catch { Out("exc"); }
		//Debug.WriteLine("deb");

		////Output.Clear();


		//var e = new Exception("failed");
		//var e = new ArgumentException(null, "paramName");
		//var e = new FormatException();
		//var e = new InvalidOperationException();
		//var e = new NotImplementedException();
		//var e = new NotSupportedException();
		//var e = new OperationCanceledException();
		//var e = new TimeoutException();
		//var e = new WaitTimeoutException();
		//var e = new WaitTimeoutException(null, new Exception("inner"));
		//var e = new WaitTimeoutException();
		//Out(e.Message);


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
	}

	#region old

	public static unsafe void _Main()
	{
		TestAny();
		return;

		//TestUnmanaged();

		//StackTrace stackTrace = new StackTrace();
		//Out(stackTrace.GetFrame(1).GetMethod().Name);
		////Out(stackTrace.GetFrame(1).GetFileLineNumber()); //always 0, even in Debug build
		//Out(stackTrace.GetFrame(1).GetFileName()); //null
		////Out(stackTrace.GetFrame(1).GetMethod(). //nothing useful




		//Dee f=Mee;
		////f(GCHandle.Alloc("test"));
		////f(GCHandle.FromIntPtr(Zero));
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




		//Time.SpinCPU(); //does nothing
		long t1 = Stopwatch.GetTimestamp();

		//TODO: instead could simply allocate unmanaged memory with Marshal methods and pass to domains with childDomain.SetData
		_sm = new OurSharedMemory();
		_sm.Create("Catkeys_SM_Tasks", 1024 * 1024);

		if(true) //compiler
		{
			_sm.x->perf.AddTicksFirst(t1);
			_sm.x->perf.Next();

			IntPtr ev = Api.CreateEvent(Zero, false, false, null);

			_sm.x->eventCompilerStartup = ev;

			//Mes("before");

			var thr = new Thread(_AppDomainThread);
			thr.Start();

			_sm.x->perf.Next();

			Api.WaitForSingleObject(ev, ~0U);
			//Thread.Sleep(100);
			Api.CloseHandle(ev);

			_sm.x->perf.NextWrite();

			_hwndCompiler = _sm.x->hwndCompiler;

			for(int i = 0; i < 1; i++) {
				_hwndCompiler.Send(Api.WM_USER, Zero, Marshal.StringToBSTR("test"));
			}

			//Mes("in");

			_hwndCompiler.Send(Api.WM_CLOSE);
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
		var domain = AppDomain.CreateDomain("Compiler");
		Time.Next();
		domain.ExecuteAssembly(@"C:\Test\test1.exe");
		Time.Next();
		AppDomain.Unload(domain);
		Time.NextWrite();
	}

	static void _AppDomainThread()
	{
		//_DomainCallback();

		var domain = AppDomain.CreateDomain("Compiler");
		//var domain=AppDomain.CreateDomain("Compiler", AppDomain.CurrentDomain.Evidence, new AppDomainSetup { LoaderOptimization = LoaderOptimization.MultiDomain }); //by default makes faster, but makes much slower when we use LoaderOptimization attribute on Main(). Assemblies will not be unloaded when appdomain unloaded (will use many MB of memory).
		//System.IO.Pipes.AnonymousPipeClientStream
		//childDomain.SetData("hPipe", handle.ToString());
		unsafe { _sm.x->perf.Next(); }

		domain.DoCallBack(_DomainCallback);
		//domain.ExecuteAssembly(Paths.CombineApp("Compiler.exe"));
		//domain.DoCallBack(Compiler.Compiler.Main); //faster than ExecuteAssembly by 3-4 ms
		AppDomain.Unload(domain);
		domain = null;
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
		long t1 = Stopwatch.GetTimestamp();

		_sm = new OurSharedMemory();
		_sm.Open("Catkeys_SM_Tasks");

		_sm.x->perf.AddTicksNext(t1);
		_sm.x->perf.Next();

		//=AppDomain.CurrentDomain.GetData("hPipe")

		Util.Window.RegWinClassHidden("Catkeys_Compiler", _wndProcCompiler);

		_sm.x->perf.Next();

		Wnd w = Api.CreateWindowEx(0, "Catkeys_Compiler", null, Api.WS_POPUP, 0, 0, 0, 0, Wnd.Spec.Message, Zero, Zero, Zero);

		_sm.x->perf.Next();

		_SHMEM* x = _sm.x;
		x->hwndCompiler = w;
		Api.SetEvent(x->eventCompilerStartup);

		//message loop
		//Application.Run(); //By default would add several ms to the startup time. Same speed if Main() has the LoaderOptimization attribute. Also may be not completely compatible with native wndproc. Also in some cases adds several MB to the working set.
		Api.MSG m;
		while(Api.GetMessage(out m, Wnd0, 0, 0) > 0) { Api.DispatchMessage(ref m); }
	}

	unsafe static LPARAM _WndProcCompiler(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
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
		case Api.WM_USER:
			//Out(Marshal.PtrToStringBSTR(lParam));
			Marshal.FreeBSTR(lParam);
			TestRoslyn();
			return Zero;
		}

		LPARAM R = Api.DefWindowProc(hWnd, msg, wParam, lParam);

		switch(msg) {
		case Api.WM_NCDESTROY:
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
		Assembly a = Assembly.LoadFile(Util.Paths.CombineApp("csc.exe")); //ok
																		  //Assembly a = Assembly.Load("csc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"); //works, same speed as LoadFile, but VS shows many warnings if this project uses different .NET framework version than csc (which is added to project references). Also, possibly could make the app start slower, especially if HDD. Better to load on demand through reflection.
		MethodInfo m = a.EntryPoint;
		string[] g = new string[] {null, "/nologo", "/noconfig", "/target:winexe",
		"/r:System.dll", "/r:System.Core.dll", "/r:System.Windows.Forms.dll",
		  @"C:\Test\test.cs"};
		object[] p = new object[1] { g };

		//g[0]="/?";
		Time.Next(); //16 ms
		for(int i = 1; i <= 4; i++) {
			g[0] = $@"/out:C:\Test\test{i}.exe";
			int r = (int)m.Invoke(0, p); //works, 22 ms, first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			if(r != 0) Out(r);
			Time.Next();
			//GC.Collect(); //4 ms, makes working set smaller 48 -> 33 MB
			//Time.Next();
		}
		Time.Write();

		//Mes("now will GC.Collect");
		GC.Collect(); //releases a lot

		OutLoadedAssemblies();
		Show.TaskDialog("exit");
	}

	static void OutLoadedAssemblies()
	{
		AppDomain currentDomain = AppDomain.CurrentDomain;
		Assembly[] assems = currentDomain.GetAssemblies();
		foreach(Assembly assem in assems) {
			Out(assem.ToString());
			Out(assem.CodeBase);
			Out(assem.Location);
			Out("");
		}
	}
}


#endregion


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
