using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using static Au.Controls.Sci;
using Au.Compiler;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;

using DiffMatchPatch;
using System.Runtime;
using System.Globalization;

#if TRACE

#pragma warning disable 169

partial class ThisIsNotAFormFile { }

partial class FMain
{

	//void _TestGetItemText()
	//{
	//	int n = 0;
	//	APerf.First();
	//	foreach(var f in Program.Model.Root.Descendants()) {
	//		if(!f.IsCodeFile) continue;
	//		if(f.Name == "5 M lines.cs") continue;
	//		n++;
	//		//Print(f.Name);
	//		//var s = f.GetText();
	//		AFile.GetProperties(f.FilePath, out var p, FAFlags.UseRawPath);
	//	}
	//	APerf.NW();
	//	Print(n);
	//}

	//void TestFileNodeTextCache()
	//{
	//	AOutput.Clear();
	//	var f = Program.Model.CurrentFile;
	//	APerf.First();
	//	var s = f.GetText(saved: true, cache: true);
	//	APerf.NW();
	//	Print(s);
	//}

	void TestReplaceTextGently()
	{
		var doc = Panels.Editor.ZActiveDoc;
		var s1 = doc.Text;
		int i = s1.Find("//.");
		//var s2 = s1 + "added\r\n";
		var s2 = s1.Insert(i, "insert\r\n");
		doc.ZReplaceTextGently(s2);
	}

	void TestDiffMatchPatch()
	{
		var s1 = @"//.
using Au; using Au.Types; using static Au.AStatic; using System; using System.Collections.Generic;
class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;
	
	var s=""one"";
";
		var s2 = @"/*/ role exeProgram;		outputPath %AFolders.Workspace%\bin; console true; /*/ //.
using Au; using Au.Types; using static Au.AStatic; using System; using System.Collections.Generic;
using My.NS1; //ąčę îôû
using My.NS2;
class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;
	var i=2;
";

		var dmp = new diff_match_patch();
		List<Diff> diff = dmp.diff_main(s1, s2, true);
		dmp.diff_cleanupSemantic(diff);
		var delta = dmp.diff_toDelta(diff);
		Print(delta);
		Print("----");
		var d2 = dmp.diff_fromDelta(s1, delta);
		//Print(d2);
		Print(dmp.diff_text2(d2));
	}

	void TestNoGcRegion()
	{
		for(int i = 0; i < 2; i++) {
			ADebug.LibMemorySetAnchor();
			bool noGC = GC.TryStartNoGCRegion(10_000_000);
			var a = new byte[50_000_000];
			for(int j = 0; j < a.Length; j++) a[j] = 1;
			Print(noGC, GCSettings.LatencyMode == GCLatencyMode.NoGCRegion);
			if(noGC && GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) try { GC.EndNoGCRegion(); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
			ADebug.LibMemoryPrint();
			GC.Collect();
			if(!ADialog.ShowYesNo("Continue?")) break;
		}

	}

	class TestGC
	{
		~TestGC()
		{
			if(Environment.HasShutdownStarted) return;
			if(AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			Print("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//ATimer.After(1, _ => new TestGC());
			//var f = Program.MainForm; if(!f.IsHandleCreated) return;
			//f.BeginInvoke(new Action(() => new TestGC()));
			new TestGC();
		}
	}
	static bool s_debug2;

	void _MonitorGC()
	{
		//if(!s_debug2) {
		//	s_debug2 = true;
		//	new TestGC();

		//	//ATimer.Every(50, _ => {
		//	//	if(!s_debug) {
		//	//		s_debug = true;
		//	//		ATimer.After(100, _ => new TestGC());
		//	//	}
		//	//});
		//}
	}

	//static ushort atom = AWnd.More.RegisterWindowClass("uuuuuuu", (w, m, wp, lp) => {
	//	AWnd.More.PrintMsg(w, m, wp, lp);
	//	return Api.DefWindowProc(w, m, wp, lp);
	//});

	//static bool classRegistered;

	//static LPARAM _Wndproc(AWnd w, int m, LPARAM wp, LPARAM lp)
	//{
	//	AWnd.More.PrintMsg(w, m, wp, lp);
	//	return Api.DefWindowProc(w, m, wp, lp);
	//}


	public unsafe void TestEditor()
	{
		if(AKeys.IsShift) {
			AWnd.Find("*Notepad").Activate();
			ATime.SleepDoEvents(100);
		}

		var m = new AMenu("name");
		m["A"] = o => Print(o);
		m.Add(new ToolStripTextBox());
		var cb = new ToolStripComboBox();
		for(int i=0;i<60;i++) cb.Items.Add("aaa");
		m.Add(cb);
		m["B"] = o => Print(o);
		using(m.Submenu("sub")) {
			m["C"] = o => Print(o);
			m.Add(new ToolStripTextBox());
		}
		m.Show(Program.MainForm);

		//var task = "_Au.Editor";
		//bool exists = WinTaskScheduler.TaskExists("Au", task);
		//Print(exists);
		//if(exists) WinTaskScheduler.DeleteTask("Au", task);
		//else WinTaskScheduler.CreateTaskToRunProgramOnDemand("Au", task, UacIL.System, AFolders.ThisAppBS + "Au.CL.exe", "/s $(Arg0)");
		//Print("ok");

		//AOutput.Clear();

		//EdDatabases.CreateRefAndDoc();
		//EdDatabases.CreateWinapi();

		var doc = Panels.Editor.ZActiveDoc;
		var Z = doc.Z;
		var s = doc.Text;


		//z.Select(false, 300, 295);
		//z.ReplaceRange(false, 295, 300, "RE");
		//z.ReplaceRange(false, 300, 295, "RE");
		//z.ReplaceRange(false, 295, 300, "RE", true);
		//z.ReplaceRange(false, 300, 295, "RE", true);
		//z.InsertText(false, 300, "INS");

		//Debug.Assert(false);

		//CodeInfo.Stop();

		//Print(z.CurrentPos16);

		//int i = z.SelectionStart8, j = z.SelectionEnd8;
		//doc.Call(SCI_STARTSTYLING, i);
		//doc.Call(SCI_SETSTYLING, j - i, 31);

		//int i = z.LineFromPos(false, z.CurrentPos8);
		//Print(i + 1, (uint)doc.Call(SCI_GETFOLDLEVEL, i));
		//Print(AWnd.ThisThread.FocusedControl);

		//int line = doc.Call(SCI_GETLINECOUNT);
		////Print(doc.Len8, doc.Call(SCI_POSITIONFROMLINE, line), doc.Call(SCI_POSITIONFROMLINE, line+1), doc.Call(SCI_POSITIONFROMLINE, -1));
		//int len = doc.Len8;
		////Print(len);
		////Print(z.LineStart(false, -1));
		////Print(doc.Call(SCI_GETLINECOUNT), doc.Call(SCI_LINEFROMPOSITION, len), doc.Call(SCI_LINEFROMPOSITION, len + 100));
		////Print(len, z.LineEnd(false, line + 1), z.LineEnd(false, line + 1, true));
		////Print(len, z.LineStart(false, line + 1), z.LineStartFromPos(false, len+1));
		////Print(len, z.LineEndFromPos(false, len+1), z.LineEndFromPos(false, len+1, true));
		//Print(len, doc.Pos8(len), doc.Pos16(len));
		////Print(doc.Pos8(len+100000));
		//Print(doc.Pos16(len+1));

		//int pos8 = z.CurrentPos8, pos16= Encoding.UTF8.GetCharCount(Encoding.UTF8.GetBytes(s), 0, pos8);
		//Print(pos8, pos16, doc.Pos8(pos16), doc.Pos16(pos8));
		//return;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 7; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { doc.Pos8(pos16); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { doc.Pos16(pos8); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { doc.TestCreatePosMap(); }
		//	APerf.NW();
		//	Thread.Sleep(100);
		//}

	}
	//SciCode.ITempRange _tr;
	//bool _testOvertype;

	unsafe void SetHookToMonitorCreatedWindowsOfThisThread()
	{
		_hook = AHookWin.ThreadCallWndProcRet(x => {
			if(x.msg->message == Api.WM_CREATE) {
				if(Program.MainForm.Visible) return;
				var w = x.msg->hwnd;
				var p = w.Get.DirectParent; if(p.Is0) p = w.Owner;
				var c = Control.FromHandle(w.Handle); //always null in CBT hook proc
				var s = c?.ToString() ?? "";
				Print($"<><c 0xcc00>{w} ({s}), {p.Handle}</c>");

				//if(c is Au.Controls.AToolStrip) { //never mind: .NET bug: if toolstrip Custom1 has overflow and window is maximized, creates parked handle
				//	int stop = 0;
				//}
			}
		});
		Application.ApplicationExit += (unu, sed) => _hook.Dispose(); //without it at exit crashes (tested with raw API and not with AHookWin) 
	}
	static AHookWin _hook;
}
#endif
