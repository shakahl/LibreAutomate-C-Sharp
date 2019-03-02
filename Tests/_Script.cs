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
	struct _Struct
	{
		public int w, flags;
		public long time;
		public object o;
	}

	[STAThread] static void Main(string[] args) { new Script()._Main(args); }
	void _Main(string[] args)
	{ //}}//}}//}}//}}

		//Output.QM2.UseQM2 = true;
		//Output.Clear();

		var co = Control.DefaultBackColor;

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
		//	var w = g.wnd; var e = g.aEvent;
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

		//var qm2 = Wnd.FindFast(null, "QM_Editor").OrThrow();
		var h = new Au.Util.AccHook(ae, g => {
			//var h = new Au.Util.AccHook(AccEVENT.MIN, AccEVENT.MAX, g => {
				//return;
			//var h = new Au.Util.AccHook((AccEVENT)0x4E00, (AccEVENT)0x7fff, g => { //UI Automation. Nothing useful.
			var w = g.wnd; var e = g.aEvent;
			//if(g.idObject != 0 || g.idChild != 0 || !w.IsAlive) return;
			////if(w.HasStyle(WS.CHILD)) {
			//if(w.IsChild && e!= AccEVENT.OBJECT_PARENTCHANGE) {
			//	//if(e== AccEVENT.OBJECT_PARENTCHANGE) {
			//	//	Print(w.Get.DirectParentOrOwner);
			//	//}
			//	return;
			//}

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
			return;



			//if(e == AccEVENT.OBJECT_UNCLOAKED && !w.IsVisible) return; //eg IME
			//if(0 != w.ClassNameIs("*tooltip*", "SysShadow", "#32774", "QM_toolbar", "TaskList*", "ClassicShell.*", "IME", "MSCTFIME UI")) return;
			//if(w.HasExStyle(WS_EX.NOACTIVATE) && 0 != w.ProgramNameIs("devenv.exe")) return;
			//if(0 != w.ProgramNameIs("qm.exe")) return;
			//if(0 == w.ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow")) return;
			//if(!w.IsAlive) return;
			//if(Keyb.IsNumLock) return;
			//if(w.Window == qm2) return;
			//Print(e, g.idObject, g.idChild, w);

			string col = "0";
			switch(e) {
			case AccEVENT.SYSTEM_FOREGROUND:
				col = "0x0000ff";
				if(!w.IsActive) Print("inactive SYSTEM_FOREGROUND", w);
				break;
			case AccEVENT.OBJECT_NAMECHANGE: col = w == Wnd.GetWnd.Root ? "0xff0000" : "0x808000"; break;
			case AccEVENT.OBJECT_HIDE: col = "0x606060"; break;
			case AccEVENT.OBJECT_PARENTCHANGE: col = "0x00c0c0"; break;
			case AccEVENT.OBJECT_CREATE: col = "0xc000c0"; break;
			//case AccEVENT.OBJECT_REORDER: col = "0xc000c0"; break;
			}
			Print($"<><c {col}>{q.Count,2} {Time.WinMilliseconds64 % 10000,4}, {e,-17}, ({g.idObject}|{g.idChild}), {(w.IsVisible ? " " : "H")}{(w.IsCloaked ? "C" : " ")}, {w}, {w.Style}, {w.ExStyle}</c>");
			//
		}, flags: AccHookFlags.SKIPOWNPROCESS);
		//TODO: detect when some event is received too frequently, and temporarily unhook.


		//var h = new Au.Util.AccHook(AccEVENT.OBJECT_DESTROY, 0, g => {
		//	var w = g.wnd;
		//	Print(g.idObject, g.idChild, w);

		//}, flags: AccHookFlags.SKIPOWNPROCESS);


		//AuDialog.Show("hook");
		OutputForm.ShowForm();
		h.Dispose();




		return;

		var k = Triggers.Hotkey;
		var u = Triggers.Window;
		k["Ctrl+T"] = o => Print("Ctrl+T");
		Triggers.Last.Hotkey.EnabledAlways = true;
		Triggers.Of.Window("* Notepad");
		k["Ctrl+N"] = o => Print("Ctrl+N in Notepad");
		u.Active["Quick *"] = o => Print("active Quick *");
		u.NameChanged["Quick *"] = o => Print("name Quick *");
		u.Visible["* Notepad"] = o => Print("visible * Notepad");

		var f = new Form { Text = "Triggers", StartPosition = FormStartPosition.CenterScreen };
		f.FormClosed += (unu, sed) => Triggers.Stop();
		f.Show();

		Triggers.Run();
		//Print("stopped");
	}
}
