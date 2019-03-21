/*/ runMode blue; ifRunning restart; /*/ //{{
										 //{{ using
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using Au;
using Au.Types;
using static Au.NoClass;
using Au.Triggers; //}}
				   //{{ main
unsafe partial class Script : AuScript
{
	void Test1()
	{
		//var w = Wnd.Find("Options").OrThrow();
		var w = Wnd.Find("Test", "Cabinet*").OrThrow();
		//var w = Wnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		int n = 0, nv = 0;
		Api.EnumChildWindows(w, (c, param) => {
			n++;
			//bool vis = c.IsVisible;
			bool vis = c.LibIsVisibleIn(w);
			if(vis) nv++;
			//bool isClass = w.ClassNameIs("Button");
			//int id = w.ControlId;
			//Print(vis, c);
			return 1;
		});
		Print(n, nv);
	}

	void Test2()
	{
		//var w = Wnd.Find("Options").OrThrow();
		var w = Wnd.Find("Test", "Cabinet*").OrThrow();
		//var w = Wnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		int n = 0, nv = 0;
		_EnumDirectChildren(w);

		void _EnumDirectChildren(Wnd parent)
		{
			for(var c = Api.GetWindow(parent, Api.GW_CHILD); !c.Is0; c = Api.GetWindow(c, Api.GW_HWNDNEXT)) {
				n++;
				if(!c.HasStyle(WS.VISIBLE)) continue;
				nv++;
				_EnumDirectChildren(c);
			}
		}

		Print(n, nv);
	}

	[STAThread] static void Main(string[] args) { new Script()._Main(args); }
	void _Main(string[] args)
	{ //}}//}}//}}//}}

//		Output.QM2.UseQM2 = true;
//		Output.Clear();
//		100.ms();

//		var w = Wnd.Find("Test", "Cabinet*").OrThrow();
//#if false
//		//var a = Wnd.GetWnd.AllWindows(false, true);
//		var a = Wnd.GetWnd.ThreadWindows(w.ThreadId, false, true);
//		//var a = w.Get.Children(true, true);
//		Print(a);
//		Print(a.Length);
//#else
//		List<Wnd> a=null;
//		//Wnd.GetWnd.AllWindows(ref a, false, true);
//		//Wnd.GetWnd.ThreadWindows(ref a, w.ThreadId, false, true);
//		w.Get.Children(ref a, false, true);
//		Print(a);
//		Print(a.Count);
//#endif

//		return;

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 7; i1++) {
		//	Perf.First();
		//	Test2();
		//	Perf.NW();
		//	Thread.Sleep(200);
		//}

		//return;

		//var fa = new Au.Tools.Form_Acc();
		//var fa = new Au.Tools.Form_Wnd();
		//try {
		//	fa.ShowDialog();
		//}
		//finally {
		//	Cpp.Cpp_Unload();
		//}

		////Cpp.Cpp_Test();

		////var w = Wnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		//////var a = Acc.Find(w, "BUTTON", "Save", "class=Button").OrThrow();
		//////var a = Acc.Find(w, "BUTTON", "Save", "class=Button", AFFlags.HiddenToo).OrThrow();
		////var a = Acc.Find(w, "BUTTON", "Save", "class=Button", AFFlags.UIA).OrThrow();
		////Print(a.Name);

		//return;

		var k = Triggers.Hotkey;
		var u = Triggers.Window;
		Triggers.Window.LogEvents(true, o => !_DebugFilterEvent(o));
		//Triggers.Options.RunActionInThread(0, 1000);

		k["Ctrl+T"] = o => Print("Ctrl+T");
		//k["Ctrl+T"] = o => { Print("Ctrl+T"); u.SimulateActiveNew(Wnd.Active); };
		//Triggers.Hotkey.Last.EnabledAlways = true;
		//Triggers.Of.Window("* Notepad");
		//k["Ctrl+N"] = o => Print("Ctrl+N in Notepad");

		//u.ActiveOnce["Quick *", thenEvents: TWEvents.Name] = o => Print(o.Window);
		var te = TWEvents.Name;
		te |= TWEvents.Destroyed;
		te |= TWEvents.Active | TWEvents.Inactive;
		te |= TWEvents.Visible | TWEvents.Invisible;
		te |= TWEvents.Cloaked | TWEvents.Uncloaked;
		te |= TWEvents.Minimized | TWEvents.Unminimized;
		//te |= TWEvents.MoveSizeStart | TWEvents.MoveSizeEnd;
		//Triggers.FuncOf.NextTrigger = o => { var y = o as WindowTriggerArgs; Print("func", y.Later, y.Window); /*201.ms();*/ return true; };
		//u.ActiveOnce["*- Notepad", later: te, flags: TWFlags.LaterCallFunc] = o => Print(o.Later, o.Window);
		//u.ActiveNew["* WordPad"] = o => Print(o.Window);
		//u.ActiveNew["* Notepad++"] = o => Print(o.Window);
		//u.ActiveOnce["* Notepad++"] = o => Print(o.Window);
		//u.Active["* Notepad++"] = o => Print("active always", o.Window);
		//u.ActiveNew["Calculator", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u.Active["Calculator", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u.ActiveNew["Settings", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u.ActiveOnce["Settings", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u.ActiveOnce["*Studio ", flags: TWFlags.StartupToo] = o => Print(o.Window);

		//u.Visible["* Notepad"] = o => Print(o.Window);
		//u.VisibleNew["* WordPad"] = o => Print(o.Window);
		u.VisibleOnce["QM SpamFilter"] = o => Print("visible once", o.Window);
		//u.VisibleOnce["*Studio ", flags: TWFlags.StartupToo] = o => Print(o.Window);

		//u.VisibleOnce["mmmmmmm"] = o => Print("mmmmmmm", Thread_.NativeId);
		//var t1 = Triggers.Window.Last;
		////k["Ctrl+K"] = o => { Print("Ctrl+K", Thread_.NativeId); t1.RunAction(null); 100.ms(); };
		////Triggers.FuncOf.NextTrigger = o => { t1.RunAction(null); return false; };
		//Triggers.FuncOf.NextTrigger = o => { (o as HotkeyTriggerArgs).Trigger.RunAction(null); return false; };
		//k["Ctrl+K"] = o => Print("Ctrl+K");
		////Triggers.Options.RunActionInThread(0, 0, noWarning: true);
		//k["Ctrl+M"] = o => { Print("Ctrl+M"); 300.ms(); };

		//u.NameChanged["Quick *"] = o => Print(o.Window);

		//Triggers.Mouse[TMMove.DownUp] = o => Print(o);

		//Triggers.Stopping += (unu, sed) => { Print("stopping"); };
		//k["Ctrl+Q"] = o => { Print("Ctrl+Q"); Triggers.Stop(); };

		//k["Ctrl+K"] = o => Print("Ctrl+K");
		//Triggers.Hotkey.Last.Disabled = true;

		//Triggers.Hotkey["Ctrl+D"] = o => { Print("Ctrl+D (disable/enable)"); Triggers.Disabled ^= true; }; //toggle
		//Triggers.Hotkey.Last.EnabledAlways = true;
		//Triggers.Hotkey["Ctrl+Q"] = o => { Print("Ctrl+Q (stop)"); Triggers.Stop(); };
		//Triggers.Hotkey.Last.EnabledAlways = true;

		//TODO: maybe better Triggers.Window.FuncOfNextTrigger. Then args can be WindowTriggerArgs and don't need to cast.
		//	Or let func be a parameter. Use FuncOf only for common functions; move it into Options: Options.AfterOf/BeforeOf.

		//Triggers.FuncOf.NextTrigger = o => { var v = o as WindowTriggerArgs; var ac = v.Window.Get.Children(); Print(ac); Print(ac.Length); return true; };
		Func<Wnd, bool> also = o => { var ac = o.Get.Children(); foreach(var v in ac) Print(v, v.Style & (WS)0xffff0000); Print(ac.Length); return true; };
		//Func<Wnd, bool> also = o => { /*100.ms();*/ Print(o.Get.Children().Length); return true; };
		//Triggers.Window.ActiveOnce["Notepad", "#32770"]=o=>Print("TRIGGER", o.Window);
		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "'!Button' Save"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "'!Button' Save", also: also] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "'STATICTEXT' * save *"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.VisibleOnce["Notepad", "#32770", contains: "'!Button' Save"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.VisibleOnce["Notepad", "#32770", contains: "'STATICTEXT' * save *"] = o => Print("TRIGGER", o.Window);

		//Triggers.Window.VisibleOnce["Notepad", "#32770"] = o => { Print("TRIGGER", o.Window); o.Window.Close(); };
		//Triggers.Window.ActiveOnce["Notepad", "#32770"] = o => { Print("TRIGGER", o.Window); o.Window.Close(); };

		Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "'!Button' Save"] = o => {
		//Triggers.Window.ActiveOnce["Notepad", "#32770"] = o => {
			//Triggers.Window.ActiveOnce["Options", "#32770"] = o => {
			//Triggers.Window.ActiveOnce["* WordPad"] = o => {
			//Triggers.Window.ActiveOnce["* Word"] = o => {
			//Triggers.Window.ActiveOnce["* Chrome"] = o => {
			//Triggers.Window.ActiveOnce["Calculator"] = o => {
			//Triggers.Window.ActiveOnce["*paint.net*"] = o => {
			var w = o.Window;
			//Print(w.Child("Save"));
			//Perf.First();
			//w.Send(0);
			////_Sleep(w);
			//Perf.Next();
			////Print(w.Child("Save"));
			//Perf.Write();
			w.Close();
		};

		Thread_.Start(() => {
			var f = new OutputForm();
			f.FormClosed += (unu, sed) => Triggers.Stop();
			f.ShowDialog();
		}, false);
		100.ms();
		Triggers.Run();
		//Print("stopped");

		//Triggers.Options.RunActionInNewThread(true);
		//Triggers.Window.ActiveNew["* Notepad"] = o => o.Window.Resize(500, 200);
		//Triggers.Hotkey["Ctrl+T"] = o => Triggers.Window.SimulateActiveNew();
		//Triggers.Hotkey["Ctrl+Alt+T"] = o => Triggers.Stop();
		//Triggers.Run();
	}

	//static void _Sleep(Wnd w)
	//{
	//	var c = w.Child("Save", "Button", WCFlags.HiddenToo).OrThrow();
	//	int n = 16;
	//	var a = new Wnd[n];
	//	var b = new bool[n];
	//	for(int i = 0; i < n; i++) {
	//		a[i] = Wnd.Focused;
	//		b[i] = c.LibIsVisibleIn(w);
	//		1.ms();
	//	}
	//	//Print(a);
	//	for(int i = 0; i < n; i++) Print(b[i], a[i]);
	//}

	//static void _Sleep()
	//{
	//	int n = 16;
	//	var a = new bool[n];
	//	var ht = Thread_.Handle;
	//	Api.SetThreadPriority(ht, -2);
	//	for(int i = 0; i < n; i++) {
	//		//Print(1 << (i & 3));
	//		SetThreadAffinityMask(ht, 1 << (i & 3));
	//		for(var t0 = Time.PerfMicroseconds; Time.PerfMicroseconds - t0 < 100;) { }
	//		a[i]=SwitchToThread();
	//		//a[i] = Thread.Yield();
	//		//Thread.Sleep(0);
	//		//1.ms();
	//	}
	//	SetThreadAffinityMask(ht, 15);
	//	Api.SetThreadPriority(ht, 0);
	//	Print(a);
	//}

	[DllImport("kernel32.dll")]
	internal static extern bool SwitchToThread();
	[DllImport("kernel32.dll")]
	internal static extern LPARAM SetThreadAffinityMask(IntPtr hThread, LPARAM dwThreadAffinityMask);


	bool _DebugFilterEvent(Wnd w)
	{
		if(0 != w.ClassNameIs("*tooltip*", "SysShadow", "#32774", "QM_toolbar", "TaskList*", "ClassicShell.*"
			, "IME", "MSCTFIME UI", "OleDde*", ".NET-Broadcast*", "GDI+ Hook*", "UIRibbonStdCompMgr"
			)) return false;
		//if(w.HasExStyle(WS_EX.NOACTIVATE) && 0 != w.ProgramNameIs("devenv.exe")) return;
		var es = w.ExStyle;
		bool ret = es.Has_(WS_EX.TRANSPARENT | WS_EX.LAYERED) && es.HasAny_(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE); //tooltips etc
		if(!ret) ret = es.Has_(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE) && es.Has_(WS_EX.LAYERED); //eg VS code tips
		if(ret) { /*Print($"<><c 0x80e0e0>    {e}, {w}, {es}</c>");*/ return false; }
		if(Debugger.IsAttached && w.LibNameTL.Like_("*Visual Studio")) return false;
		return true;
	}
}
