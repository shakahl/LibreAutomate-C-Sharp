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
	struct _WndStrW { public Wnd w; public string s; }
	struct _WndStrI { public int w; public string s; }

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
	bool _WndExistsInArray1(Wnd w, Wnd[] a, int n)
	{
		for(int i = 0; i < n; i++) if(a[i] == w) return true;
		return false;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
	bool _WndExistsInArray2(Wnd w, Wnd[] a, int n)
	{
		if((uint)n > a.Length) return false;
		for(int i = 0; i < n; i++) if(a[i] == w) return true;
		return false;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
	bool _WndExistsInArray3(Wnd w, Wnd[] a, int n)
	{
		fixed (Wnd* p = a) {
			for(int i = 0; i < n; i++) if(p[i] == w) return true;
		}
		return false;
	}

	[STAThread] static void Main(string[] args) { new Script()._Main(args); }
	void _Main(string[] args)
	{ //}}//}}//}}//}}

		//Output.QM2.UseQM2 = true;
		//Output.Clear();
		//100.ms();

		//var a = Wnd.GetWnd.AllWindows();
		//Print(a.Length);

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Thread.Sleep(200);
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { _WndExistsInArray1(default, a, a.Length); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _WndExistsInArray2(default, a, a.Length); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _WndExistsInArray3(default, a, a.Length); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}


		//return;

		////var s = "a\r\n\r\nb\r\n";
		////var rx = @"(?m)^";
		//////var rx = @"(?ms)$.";
		////var s2= s.RegexReplace_(rx, "//");

		////Print($"'{s}'\r\n'{s2}'");

		////foreach(var v in s.RegexFindAll_(rx)) { Print(v.Index, v.EndIndex); }

		////foreach(Match v in Regex.Matches(s, rx)) { Print(v.Index, v.Length); }

		//var s = "a mds a";
		//var rx = new Regex_("a");
		//var r = rx.Replace(s, "-");
		//Print(r);

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Thread.Sleep(200);
		//	int n2 = 100;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { r = rx.Replace(s, "-"); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}

		//return;

		//var dm = new Api.VirtualDesktopManager() as Api.IVirtualDesktopManager;
		////Print(dm);
		////var _atom = (int)Wnd.Misc.GetClassAtom("ApplicationFrameWindow");
		//////var _atom=(int)Api.GlobalAddAtom("ApplicationFrameWindow");
		////Print(_atom);

		//200.ms();
		////foreach(var w in Wnd.FindAll(cn: "ApplicationFrameWindow", flags: WFFlags.CloakedToo)) {
		//foreach(var w0 in Wnd.FindAll(flags: WFFlags.CloakedToo, also: o=>o.HasExStyle(WS_EX.NOREDIRECTIONBITMAP))) {
		//	Wnd w = w0;
		//	Perf.First();
		//	int cloaked = w.IsCloakedGetState;
		//	Perf.Next();
		//	bool afw = w.ClassNameIs("ApplicationFrameWindow");
		//	Perf.Next();
		//	int hr1 = dm.IsWindowOnCurrentVirtualDesktop(w.Get.RootOwnerOrThis(), out bool onCurrent);
		//	int hr2 = dm.GetWindowDesktopId(w.Get.RootOwnerOrThis(), out var guid); bool onCurrent2 = hr2 != 0 || guid == default;
		//	//Perf.NW();
		//	if(!afw) continue;
		//	var color = w.IsCloaked ? "0xC0C0" : "0";
		//	Print($"<><c {color}>{hr1} {hr2} {onCurrent} {onCurrent2}, {w},    {w.Style}, {w.ExStyle}</c>");
		//	if(hr2 == 0) Print(guid); else Print(WinError.MessageFor(hr2));
		//	//Print(cloaked);
		//	//if(0 == dm.GetWindowDesktopId(w.Get.RootOwnerOrThis(), out var guid)) Print(guid);
		//}

		//return;

		//var aw = Wnd.GetWnd.AllWindows();
		//int n = aw.Length;
		//Print(n);
		//var a = new int[n]; for(int i = 0; i < n; i++) a[i] = (int)aw[i];
		//var asw = new _WndStrW[n]; for(int i = 0; i < n; i++) asw[i].w = aw[i];
		//var asi = new _WndStrI[n]; for(int i = 0; i < n; i++) asi[i].w = a[i];
		//var alw = new List<Wnd>(n); for(int i = 0; i < n; i++) alw.Add(aw[i]);
		//var ali = new List<int>(n); for(int i = 0; i < n; i++) ali.Add(a[i]);

		//var hs=new HashSet<int>(n); for(int i = 0; i < n; i++) hs.Add(a[i]);
		//var dw=new Dictionary<int, string>(n); for(int i = 0; i < n; i++) dw.Add(a[i], "");

		//int findI = 12345678;
		//Wnd findW = (Wnd)findI;
		//int found=0;

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 8; i1++) {
		//	Thread.Sleep(200);
		//	Perf.First();
		//	for(int i = 0; i < n; i++) if(aw[i] == findW) { found++; }
		//	Perf.Next();
		//	for(int i = 0; i < n; i++) if(a[i] == findI) { found++; }
		//	Perf.Next();
		//	for(int i = 0; i < n; i++) if(asw[i].w == findW) { found++; }
		//	Perf.Next();
		//	for(int i = 0; i < n; i++) if(asi[i].w == findI) { found++; }
		//	Perf.Next();
		//	for(int i = 0; i < n; i++) if(alw[i] == findW) { found++; }
		//	Perf.Next();
		//	for(int i = 0; i < n; i++) if(ali[i] == findI) { found++; }
		//	Perf.Next();
		//	if(alw.IndexOf(findW) >= 0) found++;
		//	Perf.Next();
		//	if(ali.IndexOf(findI) >= 0) found++;
		//	Perf.Next();
		//	if(hs.Contains(findI)) found++;
		//	Perf.Next();
		//	if(dw.TryGetValue(findI, out var vv)) found++;
		//	Perf.NW();
		//}
		//Print(found);

		//return;

		//var co = Control.DefaultBackColor;

		//var w1 =Wnd.GetWnd.Shell;
		//int tid, pid;
		//tid=w1.GetThreadProcessId(out pid);
		//var a = new AccEVENT[] { AccEVENT.OBJECT_LOCATIONCHANGE, AccEVENT.OBJECT_REORDER, AccEVENT.SYSTEM_FOREGROUND,  };
		////using(var h = new Au.Util.AccHook(AccEVENT.MIN, AccEVENT.MAX, m => {
		//using(var h = new Au.Util.AccHook(a, m => {
		//	//return;
		//	//if(Keyb.IsNumLock) return;
		//	var w = m.wnd;
		//	switch(m.idObject) { case AccOBJID.CURSOR: case AccOBJID.CARET: return; }
		//	if(m.ev == AccEVENT.SYSTEM_FOREGROUND) { Print($"<><c blue>{w}<>"); return; }
		//	if(!w.Is0 && w.IsChild) return;
		//	Print(Time.PerfMilliseconds % 10000, m.ev, m.idObject, m.idChild, w);

		//}, flags: AccHookFlags.SKIPOWNPROCESS)) OutputForm.ShowForm();

		//return;

		//var _aVisible = new int[] { 1, 2, 3, 4 };
		//var _aVisible2 = new int[] { 1, 5, 2, 3 };

		//var h1 = new HashSet<int>(_aVisible);
		////var h2 = new HashSet<int>(_aVisible2);

		////h1.SymmetricExceptWith(_aVisible2);
		//h1.ExceptWith(_aVisible2);

		//Print(h1);
		//return;

		//var _aVisible = new int[] { 1,  3 };
		//var _aVisible2 = new int[] { 1, 2, 4, 3 };
		//int _nVisible = _aVisible.Length, n = _aVisible2.Length;

		//int to = Math.Min(_nVisible, n), diffFrom;
		//for(diffFrom = 0; diffFrom < to; diffFrom++) if(_aVisible2[diffFrom] != _aVisible[diffFrom]) break;
		////nTo=Math.Ma
		//if(n != _nVisible || diffFrom != n) {
		//	int diffTo1 = _nVisible, diffTo2 = n;
		//	while(diffTo1 > 0 && diffTo2 > 0) if(_aVisible2[--diffTo2] != _aVisible[--diffTo1]) { diffTo1++; diffTo2++; break; }
		//	Print(diffFrom, diffTo1, diffTo2, Math.Max(diffTo1-diffFrom, diffTo2-diffFrom));
		//} else Print("equal");

		//return;

		//#if false
		//		var a =Wnd.GetWnd.AllWindows(false, true);
		//		Print(a);
		//		Print(a.Length); //569 82
		//#else
		//		List<Wnd> a=null;
		//		Wnd.GetWnd.AllWindows(ref a, false, true);
		//		Print(a);
		//		Print(a.Count);
		//#endif

		//		return;

		//var w = Wnd.Find(null, "QM_editor");
		//Print(w);
		//var ww = Wnd.FindFast(null, "QM_editor");
		//Print(ww);

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Thread.Sleep(200);
		//	int n2 = 100;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { w = Wnd.Find(null, "QM_editor"); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { ww = Wnd.FindFast(null, "QM_editor"); }
		//	Perf.NW();
		//}

		//var f1 = new Au.Tools.Form_Wnd();
		//var w1 = Wnd.Find("*- Paint", flags: WFFlags.CloakedToo | WFFlags.HiddenToo).OrThrow();
		//var ac = Acc.Find(w1, "BUTTON", "Minimize", flags: AFFlags.HiddenToo).OrThrow();
		//Print(ac);
		//var f1 = new Au.Tools.Form_Acc(ac);
		//f1.ShowDialog();

		//var w1 = Wnd.Find("*- Paint", flags: WFFlags.CloakedToo | WFFlags.HiddenToo).OrThrow();
		////Mouse.Move(w1, 10, 10);
		//Mouse.Click(w1, 10, 10);

		//return;


		//#if true
		//		using(var a = Wnd.Lib.EnumWindows2(Wnd.Lib.EnumAPI.EnumWindows, false, true/*, predicate: (o, pp)=>o.ClassNameIs("*ui*")*/)) {
		//			for(int i=0;i<a.Count; i++) {
		//				var w = a[i];
		//				if(!w.IsVisible) break;
		//				if(w.ClassNameIs("QM_toolbar")) continue;
		//				if(0== w.ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow")) continue;
		//				var tcol = w.IsVisible ? "0" : "0x808080";
		//				var bcol = w.IsCloaked ? "0xc0f0f0" : "0xf8f8f8";
		//				Print($"<><z {bcol}><c {tcol}>{w}</c></z>");
		//			}
		//			Print(a.Count);
		//		}
		//#elif true
		//		int n = 0;
		//		for(int i = 0; i < 8; i++) {
		//			100.ms();
		//			Perf.First();
		//			var a = Wnd.GetWnd.AllWindows(true);
		//			Perf.NW();
		//			n = a.Length;
		//		}
		//		Print(n);
		//#else
		//		Debug_.LibMemorySetAnchor();
		//		for(int i = 0; i < 10000; i++) {
		//			//var a = Wnd.GetWnd.AllWindows();

		//			using(var a = Wnd.Lib.EnumWindows2(Wnd.Lib.EnumAPI.EnumWindows, false, false)) {

		//			}

		//			if(0 == (i % 1000)) Debug_.LibMemoryPrint();
		//		}
		//#endif



		//		return;

#if false

		////var aa = Wnd.GetWnd.AllWindows(false);
		////Print(aa.Length);
		////var a = new int[aa.Length];for(int i = 0; i < a.Length; i++) a[i] = (int)aa[i];
		//var rand = new Random();
		//var a = new _Struct[10000]; for(int i = 0; i < a.Length; i++) a[i].w = rand.Next();
		//int n = a.Length-10;
		//var d = new Dictionary<int, _Struct>();
		//for(int i = 0; i < a.Length; i++) d.Add(a[i].w, default);
		//int find = 1234567;

		//Timer_.Every(100, () => {
		//	//_ = a[0].IsVisible;
		//	//Perf.First();
		//	//for(int j = 0; j < 10; j++)
		//		for(int i = 0; i < n; i++) {
		//		if(a[i].w == find) { Print("a"); break; }
		//			//_ = Api.IsWindowVisible((Wnd)a[i]);
		//		}
		//	//Perf.Next();
		//	////for(int j = 0; j < 10; j++)
		//	//if(d.TryGetValue(find, out var v)) Print("b");
		//	//Perf.Next();
		//	//foreach(var vv in d) if(vv.Key==find) { Print("c"); break; }
		//	////Perf.Next();
		//	////var kk= Wnd.GetWnd.AllWindows(false);
		//	//Perf.NW();
		//	//foreach(var v in d) {
		//	//	_ = Api.IsWindowVisible((Wnd)v.Key);
		//	//}
		//});

		////var tim = new System.Windows.Forms.Timer();
		////tim.Interval = 10;
		////tim.Tick += (unu, sed) => {
		////	_ = a[0].IsVisible;
		////};
		////tim.Start();

		////Thread_.Start(() => {
		////	while(true) {
		////		Thread.Sleep(10);
		////		_ = a[0].IsVisible;
		////	}
		////});

		//MessageBox.Show("test");
		//return;

		//var events = new AccEVENT[] { AccEVENT.OBJECT_SHOW, AccEVENT.OBJECT_UNCLOAKED };
		//var h = new Au.Util.AccHook(events, g => {
		//	var w = g.wnd; var e = g.ev;
		//	if(g.idObject != 0 || g.idChild != 0 || !w.IsAlive) return;
		//	if(w.HasStyle(WS.CHILD) != w.IsChild) Print("!=", w.HasStyle(WS.CHILD), w.IsChild, e, w); //eg Windows.UI.Core.CoreWindow WS_CHILD but no parent
		//	if(w.HasStyle(WS.CHILD)) {
		//		if(e == AccEVENT.OBJECT_UNCLOAKED && !w.IsVisible) return;
		//		if(e == AccEVENT.OBJECT_SHOW && w.IsCloaked) { /*Print("child cloaked", e, w);*/ return; }
		//		Print("child", e, w, w.Style, w.ExStyle);
		//		return;
		//	}
		//	if(0 != w.ClassNameIs("tooltips_class32", "SysShadow", "#32774", "QM_toolbar", "TaskList*", "ClassicShell.*")) return;
		//	if(0 != w.ProgramName.Equals_(true, "devenv.exe", "Au.Tests.exe")) return;
		//	if(e == AccEVENT.OBJECT_UNCLOAKED && !w.IsVisible) return; //eg IME
		//	if(e == AccEVENT.OBJECT_SHOW && w.IsCloaked) { Print("cloaked", e, w); return; }

		//	Print(e, w);
		//});

		//const AccEVENT e0 = AccEVENT.OBJECT_SHOW;
		const AccEVENT e0 = AccEVENT.OBJECT_CREATE;
		var ae = new AccEVENT[] { e0, AccEVENT.OBJECT_SHOW, AccEVENT.OBJECT_NAMECHANGE, AccEVENT.SYSTEM_FOREGROUND, AccEVENT.OBJECT_UNCLOAKED, AccEVENT.OBJECT_PARENTCHANGE/*, AccEVENT.OBJECT_HIDE, AccEVENT.OBJECT_CLOAKED, AccEVENT.OBJECT_REORDER*/ };
		var q = new Queue<long>(30);

		var h = new Au.Util.AccHook(ae, g => {
			//var h = new Au.Util.AccHook(AccEVENT.MIN, AccEVENT.MAX, g => {
			//var h = new Au.Util.AccHook((AccEVENT)0x4E00, (AccEVENT)0x7fff, g => { //UI Automation. Nothing useful.
				//return;
			var w = g.wnd; var e = g.ev;
			if(g.idObject != 0 || g.idChild != 0 || !w.IsAlive) return;
			//if(w.HasStyle(WS.CHILD)) {
			if(w.IsChild /*&& e != AccEVENT.OBJECT_PARENTCHANGE*/) {
				//if(e== AccEVENT.OBJECT_PARENTCHANGE) {
				//	Print(w.Get.DirectParentOrOwner);
				//}
				return;
			}

			//if(e == e0) {
			//	if(ae[0] == 0) { Print("-"); return; }
			//	long t1 = Time.PerfMilliseconds, t2 = t1 - 100;
			//	while(q.Count > 0 && q.Peek() < t2) q.Dequeue();
			//	if(q.Count < 30) {
			//		q.Enqueue(t1);
			//	} else {
			//		Print("<pause>");
			//		q.Clear();
			//		g.hook.Unhook();
			//		ae[0] = 0;
			//		g.hook.Hook(ae, flags: AccHookFlags.SKIPOWNPROCESS);
			//		Timer_.After(1000, () => {
			//			g.hook.Unhook();
			//			ae[0] = e0;
			//			g.hook.Hook(ae, flags: AccHookFlags.SKIPOWNPROCESS);
			//		});
			//		return;
			//	}
			//}
			//return;



			if(e == AccEVENT.OBJECT_UNCLOAKED && !w.IsVisible) return; //eg IME
			if(0 != w.ClassNameIs("*tooltip*", "SysShadow", "#32774", "QM_toolbar", "TaskList*", "ClassicShell.*",
				"OleMainThreadWndClass", "CicMarshalWndClass", "UIRibbonStdCompMgr", "NetUI_Hidden", "UserAdapterWindowClass",
				"IME", "MSCTFIME UI"
				)) return;
			if(w.HasExStyle(WS_EX.NOACTIVATE) && 0 != w.ProgramNameIs("devenv.exe")) return;
			//if(0 == w.ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow")) return;
			//if(!w.IsAlive) return;
			//if(Keyb.IsNumLock) return;

			string col = "0";
			switch(e) {
			case AccEVENT.SYSTEM_FOREGROUND:
				col = "0x0000ff";
				if(!w.IsActive) Print("inactive SYSTEM_FOREGROUND", w);
				break;
			case AccEVENT.OBJECT_NAMECHANGE: col = w == Wnd.GetWnd.Root ? "0xff0000" : "0x808000"; break;
			case AccEVENT.OBJECT_HIDE: col = "0x606060"; break;
			case AccEVENT.OBJECT_PARENTCHANGE: col = "0x00c0c0"; break;
			case AccEVENT.OBJECT_CREATE: col = "0x00c000"; break;
			//case AccEVENT.OBJECT_REORDER: col = "0xc000c0"; break;
			}
			Print($"<><c {col}>{Time.WinMilliseconds64 % 10000,4}, {e,-17} {(w.IsVisible ? " " : "H")}{(w.IsCloaked ? "C" : " ")}, {w}, {w.Style}, {w.ExStyle}</c>");
			//, {q.Count,2}, ({g.idObject}|{g.idChild})
		}, flags: AccHookFlags.SKIPOWNPROCESS);


		//var h = new Au.Util.AccHook(AccEVENT.OBJECT_DESTROY, 0, g => {
		//	var w = g.wnd;
		//	Print(g.idObject, g.idChild, w);

		//}, flags: AccHookFlags.SKIPOWNPROCESS);


		OutputForm.ShowForm();
		h.Dispose();

		return;
#endif

		var k = Triggers.Hotkey;
		var u = Triggers.Window;
		Triggers.Window.LogEvents(true, o => !_DebugFilterEvent(o));
		Triggers.Options.RunActionInThread(0, 1000);

		//k["Ctrl+T"] = o => Print("Ctrl+T");
		k["Ctrl+T"] = o => { Print("Ctrl+T"); u.SimulateActiveNew(Wnd.Active); };
		//Triggers.Last.Hotkey.EnabledAlways = true;
		//Triggers.Of.Window("* Notepad");
		//k["Ctrl+N"] = o => Print("Ctrl+N in Notepad");

		//u.ActiveOnce["Quick *", thenEvents: TWEvents.Name] = o => Print("active once", o.Window);
		var te = TWEvents.Name;
		te |= TWEvents.Destroyed;
		te |= TWEvents.Active | TWEvents.Inactive;
		te |= TWEvents.Visible | TWEvents.Invisible;
		te |= TWEvents.Cloaked | TWEvents.Uncloaked;
		te |= TWEvents.Minimized | TWEvents.Unminimized;
		te |= TWEvents.MoveSizeStart | TWEvents.MoveSizeEnd;
		Triggers.FuncOf.NextTrigger = o => { var y = o as WindowTriggerArgs; Print("func", y.Later, y.Window); /*201.ms();*/ return true; };
		u.ActiveOnce["*- Notepad", later: te, flags: TWFlags.LaterCallFunc] = o => Print("active once", o.Later, o.Window);
		//u.ActiveNew["* WordPad"] = o => Print("active new", o.Window);
		//u.ActiveNew["* Notepad++"] = o => Print("active new", o.Window);
		//u.ActiveOnce["* Notepad++"] = o => Print("active once", o.Window);
		//u.Active["* Notepad++"] = o => Print("active always", o.Window);
		//u.ActiveNew["Calculator", "ApplicationFrameWindow"] = o => Print("active new", o.Window);
		u.Active["Calculator", "ApplicationFrameWindow"] = o => Print("active once", o.Window);
		//u.ActiveNew["Settings", "ApplicationFrameWindow"] = o => Print("active new", o.Window);
		//u.ActiveOnce["Settings", "ApplicationFrameWindow"] = o => Print("active once", o.Window);
		//u.ActiveOnce["*Studio ", flags: TWFlags.StartupToo] = o => Print("active once", o.Window);

		//u.Visible["* Notepad"] = o => Print("visible always", o.Window);
		//u.VisibleNew["* WordPad"] = o => Print("visible new", o.Window);
		u.VisibleOnce["QM SpamFilter"] = o => Print("visible once", o.Window);
		//u.VisibleOnce["*Studio ", flags: TWFlags.StartupToo] = o => Print("visible once", o.Window);

		//u.NameChanged["Quick *"] = o => Print("name action", o.Window);


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
