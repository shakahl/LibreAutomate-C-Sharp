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
using System.Xml;
using System.Xml.Linq;
using Au;
using Au.Types;
using static Au.AStatic;
using Au.Triggers;
using Au.Controls;
using System.Runtime.CompilerServices;
using System.Runtime.Caching;
using System.Reflection;
using System.Runtime;
using Microsoft.Win32;
//using CefSharp.WinForms;
//using CefSharp;

class Script : AScript
{
	//static void _Sleep(AWnd w)
	//{
	//	var c = w.Child("Save", "Button", WCFlags.HiddenToo).OrThrow();
	//	int n = 16;
	//	var a = new AWnd[n];
	//	var b = new bool[n];
	//	for(int i = 0; i < n; i++) {
	//		a[i] = AWnd.Focused;
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
	//	var ht = AThread.Handle;
	//	Api.SetThreadPriority(ht, -2);
	//	for(int i = 0; i < n; i++) {
	//		//Print(1 << (i & 3));
	//		SetThreadAffinityMask(ht, 1 << (i & 3));
	//		for(var t0 = ATime.PerfMicroseconds; ATime.PerfMicroseconds - t0 < 100;) { }
	//		a[i]=SwitchToThread();
	//		//a[i] = Thread.Yield();
	//		//Thread.Sleep(0);
	//		//1.ms();
	//	}
	//	SetThreadAffinityMask(ht, 15);
	//	Api.SetThreadPriority(ht, 0);
	//	Print(a);
	//}

	//[DllImport("kernel32.dll")]
	//internal static extern bool SwitchToThread();
	//[DllImport("kernel32.dll")]
	//internal static extern LPARAM SetThreadAffinityMask(IntPtr hThread, LPARAM dwThreadAffinityMask);

	void TestOptReset()
	{
		//Opt.Static.Debug.Verbose = false;
		//Opt.Debug.Verbose = true;
		//Opt.Debug.DisableWarnings("one", "two");
		//Print(Opt.Debug.Verbose);
		//Print(Opt.Debug.IsWarningDisabled("two"));
		////Opt.Reset();
		//Opt.Debug.Reset();
		//Print(Opt.Debug.Verbose);
		//Print(Opt.Debug.IsWarningDisabled("two"));

		//Opt.WaitFor.Period = 15;
		//Print(Opt.WaitFor.Period);
		////Opt.Reset();
		//Opt.WaitFor.Reset();
		//Print(Opt.WaitFor.Period);

		AOpt.Static.Mouse.MoveSpeed = 2;
		AOpt.Mouse.MoveSpeed = 15;
		Print(AOpt.Mouse.MoveSpeed);
		//Opt.Reset();
		AOpt.Mouse.Reset();
		Print(AOpt.Mouse.MoveSpeed);

		//Opt.Static.Key.TextSpeed = 2;
		//Opt.Key.TextSpeed = 15;
		//Print(Opt.Key.TextSpeed);
		////Opt.Reset();
		//Opt.Key.Reset();
		//Print(Opt.Key.TextSpeed);
	}

	unsafe void Test1()
	{
		//var w = AWnd.Find("Options").OrThrow();
		var w = AWnd.Find("Test", "Cabinet*").OrThrow();
		//var w = AWnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
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
		//var w = AWnd.Find("Options").OrThrow();
		var w = AWnd.Find("Test", "Cabinet*").OrThrow();
		//var w = AWnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		int n = 0, nv = 0;
		_EnumDirectChildren(w);

		void _EnumDirectChildren(AWnd parent)
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

	//void TestXmlNewlines()
	//{
	//	var s = "a\r\nb";
	//	var xml = "<x>" + s + "</x>";
	//	//XmlTextReader.
	//	var re=new XmlTextReader()
	//	XElement.ReadFrom
	//	var x = XElement.Parse(xml, LoadOptions.PreserveWhitespace);
	//	var ss = x.Value;
	//	Print(ss);
	//	Print(x);
	//	Print(ss == s, s.Length, ss.Length);
	//}
	//void TestXmlNewlines()
	//{
	//	var s = "a\r\nb";
	//	var xml = "<x><![CDATA[" + s + "]]></x>";
	//	var x = XElement.Parse(xml, LoadOptions.PreserveWhitespace);
	//	var ss = x.Value;
	//	Print(ss);
	//	Print(x);
	//	Print(ss == s, s.Length, ss.Length);
	//}

	unsafe void TestCharUpperNonBmp()
	{
		//for(int i='A'; i<0x10000; i++) {
		//	char c = (char)i;
		//	if(char.IsUpper(c)) Print((uint)i, c);
		//}

		//var s = new string('\0', 2);
		//fixed(char* p = s) {
		//	for(int i = 'A'; i < 0x10000; i++) {
		//		*(int*)p = i;
		//		if(char.IsUpper(s, 0)) Print((uint)i, s);
		//	}
		//}

		var e = Encoding.UTF32;
		for(int i = 0x10000; i < 0x10_0000; i++) {
			var c = (byte*)&i;
			var u = e.GetString(c, 4);
			//Print(u);
			//if(char.IsUpper(u, 0)) Print((uint)i, u, u.ToLowerInvariant());
			if(char.IsLower(u, 0)) Print((uint)i, u, u.ToUpperInvariant());
		}
		//Print("END");

		Print("a".Equals("A", StringComparison.OrdinalIgnoreCase));
		Print("𐐩".Equals("𐐁", StringComparison.OrdinalIgnoreCase));
		Print("𐐩".IndexOf("𐐁", StringComparison.OrdinalIgnoreCase));
		Print(char.IsUpper("𐐩", 0), char.IsLower("𐐩", 0), char.IsUpper("𐐁", 0), char.IsLower("𐐁", 0));
		Print("abc".Upper(true), "𐐨𐐩𐐪".Upper(true));
	}

	//class TIA
	//{
	//	public static implicit operator TIA(string s) => new TIA();
	//	public static implicit operator TIA(Action<string> a) => new TIA();
	//}

	//class TIO
	//{
	//	public TIA this[string s] {
	//		set {
	//			Print(s, value);
	//		}
	//		//get => null;
	//	}

	//	//void set_Item(string s, string va)
	//	//{
	//	//		Print(2);

	//	//}
	//}

	//void TestIndexerOverload()
	//{
	//	var t = new TIO();
	//	t["a"] = "text";
	//	t["b"] = o => Print(o);
	//}

	void TestMouseRelative()
	{
		var w = AWnd.Find("* Notepad");
		w.Activate();
		AMouse.Move(w, 20, 20);

		AOpt.Mouse.MoveSpeed = 10;
		for(int i = 0; i < 5; i++) {
			1.s();
			using(AMouse.LeftDown()) {
				AMouse.MoveRelative(30, 30);
				//var xy = AMouse.XY;
				//AMouse.Move(xy.x + 30, xy.y + 30);
			}
		}

		//Opt.Mouse.ClickSpeed = 1000;
		//AMouse.Click();

	}

	void TestBlockInputReliability()
	{
		AWnd.Find("* Notepad").Activate();
		//AWnd.Find("Quick *").Activate();

		//AThread.Start(() => {
		//	using(AHookWin.Keyboard(k => {
		//		if(!k.IsInjectedByAu) Print("----", k);

		//		return false;
		//	})) ATime.SleepDoEvents(5000);
		//});

		100.ms();
		Key("Ctrl+A Delete Ctrl+Shift+J");
		AOutput.Clear();
		//10.ms();
		for(int i = 0; i < 100; i++) {
			Text("---- ");
		}
	}

	void TestGetKeyState()
	{
		1.s();
		Print(AMouse.IsPressed(MButtons.Left), AMouse.IsPressed(MButtons.Right));

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 8; i1++) {
			int n2 = 1;
			APerf.First();
			for(int i2 = 0; i2 < n2; i2++) { AMouse.IsPressed(MButtons.Left); }
			//APerf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { AMouse.IsPressed(MButtons.Left| MButtons.Right); }
			//APerf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { AMouse.IsPressed(); }
			//APerf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { }
			APerf.NW();
			Thread.Sleep(200);
		}

		//for(int i = 0; i < 100; i++) {
		//	50.ms();
		//	APerf.First();
		//	//_ = Api.GetKeyState((int)KKey.Shift);
		//	_ = Api.GetAsyncKeyState((int)KKey.Shift);
		//	//_ = Api.GetKeyState((int)KKey.MouseLeft);
		//	//_ = Api.GetAsyncKeyState((int)KKey.MouseLeft);
		//	APerf.NW();
		//}
	}

	void TestCapsLock()
	{
		AWnd.Find("* Notepad").Activate(); 100.ms();
		AOpt.Key.TextOption = KTextOption.Keys;
		//Opt.Key.NoCapsOff = true;
		//Key("CapsLock*down");
		//Key("CapsLock*up");
		//Opt.Key.NoCapsOff = false;
		//return;
		//500.ms();
		Text("some Text");
	}

	void TestTurnOffCapsLockWithShift()
	{
		AWnd.Find("* Notepad").Activate();
		//Opt.Key.NoBlockInput = true;
		try {
			Key("a");
		}
		catch(Exception ex) { Print(ex); }
		Print("END");
	}

	void Test2CharKeysAndVK()
	{
		AWnd.Find("* Notepad").Activate();
		//AKeys.WaitForKey(0, "VK65", true);
		//Key("VK65 Vk0x42");
		//Print(AKeys.More.ParseKeysString("VK65 Vk0x42"));
		if(AKeys.More.ParseHotkeyString("Ctrl+VK65", out var mod, out var key)) Print(mod, key);
	}

	void TestAcc()
	{
		//var w = AWnd.Find("*paint.net*", "*.Window.*").OrThrow();
		//var a = AAcc.Find(w, "BUTTON", prop: "id=commonActionsStrip").OrThrow();
		////var a = AAcc.Find(w, "BUTTON", flags: AFFlags.NotInProc, prop: "id=commonActionsStrip").OrThrow();
		//Print(a);

#if true
		//string s;
		//s = @"var w = AWnd.Find(""Quick Macros - ok - [Macro329]"", ""QM_Editor"").OrThrow();";
		////s = @"var w = AWnd.Find(""Quick Macros \""- ok - [Macro329]"", ""QM_Editor"").OrThrow();";
		////s = @"var w = AWnd.Wait(5, ""Quick Macros - ok - [Macro329]"", ""QM_Editor"").OrThrow();";
		////s = @"var w = AWnd.Find(@""**r Document - WordPad\*?"", ""QM_Editor"").OrThrow();";
		////s = @"var w = AWnd.Find(@""**r Document """"- WordPad\*?"", ""QM_Editor"").OrThrow();";

		//if(s.RegexMatch(@"^(?:var|AWnd) \w+ ?= ?AWnd\.(?:Find\(|Wait\(.+?, )(?s)(?:""((?:[^""\\]|\\.)*)""|(@(?:""[^""]*"")+))", out var k)) {

		//	Print(k[0]);
		//	Print(k[1]);
		//	Print(k[2]);
		//}

		//return;

		//Task.Run(() => { for(; ; ) { 1.s(); GC.Collect(); } });

		//var w = AWnd.Find("FileZilla", "wxWindowNR").OrThrow();
		//var a = AAcc.Find(w, "STATICTEXT", "Local site:", "id=-31741").OrThrow();

		var fa = new Au.Tools.FormAAcc();
		//var fa = new Au.Tools.FormAWnd();
		//var fa = new Au.Tools.FormAWinImage();
		fa.ShowDialog();

		//ADialog.Show("unload dll");

		//var w = AWnd.Find("", "Shell_TrayWnd").OrThrow();
		//var a = AAcc.Find(w, "BUTTON", "Au - Microsoft Visual Studio - 1 running window", "class=MSTaskListWClass").OrThrow();
		//var w = AWnd.Find("Calculator", "ApplicationFrameWindow").OrThrow();
		//var a = AAcc.Find(w, "BUTTON", "Seven", "class=Windows.UI.Core.CoreWindow").OrThrow();
		//var w = AWnd.Find("Calculator", "ApplicationFrameWindow").OrThrow();
		//var c = w.Child("Calculator", "Windows.UI.Core.CoreWindow").OrThrow();
		//var a = AAcc.Find(c, "BUTTON", "Seven").OrThrow();
		//Print(a);
		//Print(a.MiscFlags);

#else
		//var w = AWnd.Find("FileZilla", "wxWindowNR").OrThrow();
		////var a = AAcc.Find(w, "LISTITEM").OrThrow();
		////Print(a.RoleInt, a);

		////using(new AHookAcc(AccEVENT.OBJECT_FOCUS, 0, k => { var a = k.GetAcc(); Print(a.RoleInt, a); }, idThread: w.ThreadId)) {

		Triggers.Hotkey["F3"] = o => {
			var a = AAcc.FromMouse(AXYFlags.PreferLink | AXYFlags.NoThrow);
			//var a = AAcc.FromMouse();
			//var a = AAcc.FromMouse(AXYFlags.NotInProc);
			//var a = AAcc.Focused();
			//Print(a.RoleInt, a);
			//Print(a.RoleInt);

			Print(a.WndContainer);
			//if(a.GetProperties("w", out var p)) Print(p.WndContainer);
		};
		Triggers.Run();
		////}
#endif
	}

	void TestWndFindEtc()
	{
		//var r = AWnd.Find("Notepad", "#32770", contains: "a 'BUTTON' Save").OrThrow();
		//Print(r);

		//AWnd.Finder x; x.Props.DoesNotMatch

		//var f = new AWnd.Finder("Notepad", "#32770", "notepad.exe", WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//var f = new AWnd.Finder("Notepad", "#32770", WF3.Owner(AWnd.Find("Quick*")), WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//var f = new AWnd.Finder("Notepad", "#32770", WF3.Thread(5), WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//var p = f.Props;
		//Print(p.name, p.cn, p.program, p.processId, p.threadId, p.owner.Handle, p.flags, p.also, p.contains);

		//var w1 = AWnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		//var f = new AWnd.Finder("Notepad", "#32770", "notepad.exe", WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//if(f.IsMatch(w1)) Print("MATCH"); else Print(f.Props.DoesNotMatch);

		//Print(f);

		//var w1 = AWnd.Find("*- Notepad").OrThrow();
		//var c1 = w1.Child(null, "Edit").OrThrow();
		//Print(c1.IsEnabled(false));
		//Print(c1.IsEnabled(true));
		//c1.Focus();

		//var w1 = AWnd.Find("Notepad", "#32770", contains: new object());
		//string image1 = @"image:iVBORw0KGgoAAAANSUhEUgAAAJEAAAATCAYAAACQoO/wAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAALLSURBVGhD7VbBbeswDO1O/9ZRMkDWyAq59pZ77znlkgW6QIACXaArqCJFKSRFSbSDGj+FHmAgdijy8fFJ9kuYmHgQ00QTD2OaaOJhFBPdTu/h5d9bdb2eviliYsKGNNHuI9zoPuEz7NFMl3CmJxNj2FpKeGKeBQMTAb7DcReNdPik+4kRponoXuB6iafRezh+0X0ExvNXX8dk54P9v67Zz5nMvL/SLQHXNGpbdfGZ6lPEfX2EV85BncIQC694wbXkow3H11ea9mOW6IrQfHm9Ti+tmWh9JB/pgQyfiYhMHiAWEuIOTis0oX4lplelP+dyE9V18+uZiyF53E4XIZQWO/Hk34qUk8V0tSRYMYt1VdwB5xM3QKcX70w4R3ON10Q8uTJUAT63nZrFEB/pQCjXc+VcYSItMtU8RnEKF6xRC1OghNOmAmjtVploja69/yyIXjwz0bntGSw4iSihEvWOeldw6PwwjNKAK+caE8mhQyzWZGJZ6/FZrHu/pImE8ADFX/dqoYpZqSv2Bxwb9Xq9aA71TPi6+6X7X/BNRMVXNpv+z87mvyNcOdeZCHNjX7CeauKmgHo6Z6rXG+7/ZqKMYqaixbgXcTgUTQhNPjUcJkpkinBYzGiKE2qgDAAI8sG7cqaB6wGieD0TIf+Y4xpzlf6yeei/jplRF/bs10z0gK4FPNbRCyD3g/95ZmKgbyJKpIeUXM/JpKH0hxkBje0uYW+cKJ6clQgo1KgumS+eQnz4kAuedQeZ+2c1PSayBljBiFmsa+S351wwJ5nI0QsC1uBMaqOafA715pAmwiL8au8AHV8Ja4JEaezScU5an2OiuLhmYN6UV/VCouoaggPwVMN2mYjzbPTailmkazFGvmSPo14S+jNJRrpfFp9iom2QCHeFmXg6bGsi3Dl6J0w8OzY00TyF/io2MVF5Nw++XSaeExt/E038PYTwA9lvzmPkr4YtAAAAAElFTkSuQmCC";
		//var w1 = AWnd.Find("Notepad", "#32770", contains: image1);
		//var w1 = AWnd.Find("Notepad", "#32770", contains: Color.Black);
		//var w1 = AWnd.Find("Notepad", "#32770", also: o => { o.Close(); o.WaitForClosed(5); return true; }, contains: "Save");
		//var w1 = AWnd.Find("Notepad", "#32770", also: o => { o.Close(); o.WaitForClosed(5); var uu = o.Child("kkkk"); return true; });
		//Print(w1);

		//return;

		//		var w = AWnd.Find("Test", "Cabinet*").OrThrow();
		//#if false
		//		//var a = AWnd.GetWnd.AllWindows(false, true);
		//		var a = AWnd.GetWnd.ThreadWindows(w.ThreadId, false, true);
		//		//var a = w.Get.Children(true, true);
		//		Print(a);
		//		Print(a.Length);
		//#else
		//		List<AWnd> a=null;
		//		//AWnd.GetWnd.AllWindows(ref a, false, true);
		//		//AWnd.GetWnd.ThreadWindows(ref a, w.ThreadId, false, true);
		//		w.Get.Children(ref a, false, true);
		//		Print(a);
		//		Print(a.Count);
		//#endif

		//		return;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 7; i1++) {
		//	APerf.First();
		//	Test2();
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}

		//return;

		////var w = AWnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		//////var a = AAcc.Find(w, "BUTTON", "Save", "class=Button").OrThrow();
		//////var a = AAcc.Find(w, "BUTTON", "Save", "class=Button", AFFlags.HiddenToo).OrThrow();
		////var a = AAcc.Find(w, "BUTTON", "Save", "class=Button", AFFlags.UIA).OrThrow();
		////Print(a.Name);
	}

	void TestToolForms()
	{
		//var fa = new Au.Tools.FormAAcc();
		//var fa = new Au.Tools.FormAWnd();
		//try {
		//	fa.ShowDialog();
		//}
		//finally {
		//	Cpp.Unload();
		//}

		////Cpp.Cpp_Test();
	}

	void TriggersExamples()
	{
	}

	void TestWindowTriggers()
	{
		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770"] = null;
		//var t1 = Triggers.Window.Last;
		//Print(t1.TypeString);
		//Print(t1.ParamsString);
		//Print(t1);

		var k = Triggers.Hotkey;
		var u = Triggers.Window;
		Triggers.Window.LogEvents(true, o => !_DebugFilterEvent(o));
		//Triggers.Options.RunActionInThread(0, 1000);

		k["Ctrl+T"] = o => Print("Ctrl+T");
		//k["Ctrl+T"] = o => { Print("Ctrl+T"); u.SimulateActiveNew(AWnd.Active); };
		//Triggers.Hotkey.Last.EnabledAlways = true;
		//Triggers.Of.Window("* Notepad");
		//k["Ctrl+N"] = o => Print("Ctrl+N in Notepad");

		//u[TWEvent.ActiveOnce, "Quick *", thenEvents: TWLater.Name] = o => Print(o.Window);
		var te = TWLater.Name;
		te |= TWLater.Destroyed;
		te |= TWLater.Active | TWLater.Inactive;
		te |= TWLater.Visible | TWLater.Invisible;
		te |= TWLater.Cloaked | TWLater.Uncloaked;
		te |= TWLater.Minimized | TWLater.Unminimized;
		//te |= TWLater.MoveSizeStart | TWEvents.MoveSizeEnd;
		//Triggers.FuncOf.NextTrigger = o => { var y = o as WindowTriggerArgs; Print("func", y.Later, y.Window); /*201.ms();*/ return true; };
		//u[TWEvent.ActiveOnce, "*- Notepad", later: te, flags: TWFlags.LaterCallFunc] = o => Print(o.Later, o.Window);
		//u[TWEvent.ActiveNew, "* WordPad"] = o => Print(o.Window);
		//u[TWEvent.ActiveNew, "* Notepad++"] = o => Print(o.Window);
		//u[TWEvent.ActiveOnce, "* Notepad++"] = o => Print(o.Window);
		//u[TWEvent.Active, "* Notepad++"] = o => Print("active always", o.Window);
		//u[TWEvent.ActiveNew, "Calculator", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u[TWEvent.Active, "Calculator", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u[TWEvent.ActiveNew, "Settings", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u[TWEvent.ActiveOnce, "Settings", "ApplicationFrameWindow"] = o => Print(o.Window);
		//u[TWEvent.ActiveOnce, "*Studio ", flags: TWFlags.StartupToo] = o => Print(o.Window);

		//u[TWEvent.Visible, "* Notepad"] = o => Print(o.Window);
		//u[TWEvent.VisibleNew, "* WordPad"] = o => Print(o.Window);
		u[TWEvent.VisibleOnce, "QM SpamFilter"] = o => Print("visible once", o.Window);
		//u[TWEvent.VisibleOnce, "*Studio ", flags: TWFlags.StartupToo] = o => Print(o.Window);

		//u[TWEvent.VisibleOnce, "mmmmmmm"] = o => Print("mmmmmmm", AThread.NativeId);
		//var t1 = Triggers.Window.Last;
		////k["Ctrl+K"] = o => { Print("Ctrl+K", AThread.NativeId); t1.RunAction(null); 100.ms(); };
		////Triggers.FuncOf.NextTrigger = o => { t1.RunAction(null); return false; };
		//Triggers.FuncOf.NextTrigger = o => { (o as HotkeyTriggerArgs).Trigger.RunAction(null); return false; };
		//k["Ctrl+K"] = o => Print("Ctrl+K");
		////Triggers.Options.RunActionInThread(0, 0, noWarning: true);
		//k["Ctrl+M"] = o => { Print("Ctrl+M"); 300.ms(); };

		//u.NameChanged["Quick *"] = o => Print(o.Window);

		//Triggers.Stopping += (unu, sed) => { Print("stopping"); };
		//k["Ctrl+Q"] = o => { Print("Ctrl+Q"); Triggers.Stop(); };

		//k["Ctrl+K"] = o => Print("Ctrl+K");
		//Triggers.Hotkey.Last.Disabled = true;

		//Triggers.Hotkey["Ctrl+D"] = o => { Print("Ctrl+D (disable/enable)"); Triggers.Disabled ^= true; }; //toggle
		//Triggers.Hotkey.Last.EnabledAlways = true;
		//Triggers.Hotkey["Ctrl+Q"] = o => { Print("Ctrl+Q (stop)"); Triggers.Stop(); };
		//Triggers.Hotkey.Last.EnabledAlways = true;

		//Triggers.FuncOf.NextTrigger = o => { var v = o as WindowTriggerArgs; var ac = v.Window.Get.Children(); Print(ac); Print(ac.Length); return true; };
		Func<AWnd, bool> also = o => { var ac = o.Get.Children(); foreach(var v in ac) Print(v, v.Style & (WS)0xffff0000); Print(ac.Length); return true; };
		//Func<AWnd, bool> also = o => { /*100.ms();*/ Print(o.Get.Children().Length); return true; };
		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770"]=o=>Print("TRIGGER", o.Window);
		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", contains: "c 'Button' Save"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", contains: "c 'Button' Save", also: also] = o => Print("TRIGGER", o.Window);
		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", contains: "a 'STATICTEXT' * save *"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window[TWEvent.VisibleOnce, "Notepad", "#32770", contains: "c 'Button' Save"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window[TWEvent.VisibleOnce, "Notepad", "#32770", contains: "a 'STATICTEXT' * save *"] = o => Print("TRIGGER", o.Window);

		Triggers.Window[TWEvent.VisibleOnce, "Notepad", "#32770"] = o => { Print("TRIGGER", o.Window); o.Window.Close(); };
		Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770"] = o => { Print("TRIGGER", o.Window); o.Window.Close(); };

		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", contains: "c 'Button' Save"] = o => {
		//Triggers.Window[TWEvent.ActiveOnce, "Save As", "#32770", "notepad.exe", contains: "c 'Button' Save"] = o => {
		//Triggers.Window[TWEvent.ActiveOnce, "Save As", "#32770", "notepad.exe", contains: "a 'BUTTON' Save"] = o => {
		//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770"] = o => {
		Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", contains: "Do you want to save*"] = o => {
			//string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAJEAAAATCAYAAACQoO/wAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAALLSURBVGhD7VbBbeswDO1O/9ZRMkDWyAq59pZ77znlkgW6QIACXaArqCJFKSRFSbSDGj+FHmAgdijy8fFJ9kuYmHgQ00QTD2OaaOJhFBPdTu/h5d9bdb2eviliYsKGNNHuI9zoPuEz7NFMl3CmJxNj2FpKeGKeBQMTAb7DcReNdPik+4kRponoXuB6iafRezh+0X0ExvNXX8dk54P9v67Zz5nMvL/SLQHXNGpbdfGZ6lPEfX2EV85BncIQC694wbXkow3H11ea9mOW6IrQfHm9Ti+tmWh9JB/pgQyfiYhMHiAWEuIOTis0oX4lplelP+dyE9V18+uZiyF53E4XIZQWO/Hk34qUk8V0tSRYMYt1VdwB5xM3QKcX70w4R3ON10Q8uTJUAT63nZrFEB/pQCjXc+VcYSItMtU8RnEKF6xRC1OghNOmAmjtVploja69/yyIXjwz0bntGSw4iSihEvWOeldw6PwwjNKAK+caE8mhQyzWZGJZ6/FZrHu/pImE8ADFX/dqoYpZqSv2Bxwb9Xq9aA71TPi6+6X7X/BNRMVXNpv+z87mvyNcOdeZCHNjX7CeauKmgHo6Z6rXG+7/ZqKMYqaixbgXcTgUTQhNPjUcJkpkinBYzGiKE2qgDAAI8sG7cqaB6wGieD0TIf+Y4xpzlf6yeei/jplRF/bs10z0gK4FPNbRCyD3g/95ZmKgbyJKpIeUXM/JpKH0hxkBje0uYW+cKJ6clQgo1KgumS+eQnz4kAuedQeZ+2c1PSayBljBiFmsa+S351wwJ5nI0QsC1uBMaqOafA715pAmwiL8au8AHV8Ja4JEaezScU5an2OiuLhmYN6UV/VCouoaggPwVMN2mYjzbPTailmkazFGvmSPo14S+jNJRrpfFp9iom2QCHeFmXg6bGsi3Dl6J0w8OzY00TyF/io2MVF5Nw++XSaeExt/E038PYTwA9lvzmPkr4YtAAAAAElFTkSuQmCC";
			//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", contains: image] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "Notepad", "#32770", also: o => AWinImage.Wait(-1, o, image, WIFlags.WindowDC) != null] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "Options", "#32770"] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "* WordPad"] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "* Word"] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "* Chrome"] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "Calculator"] = o => {
			//Triggers.Window[TWEvent.ActiveOnce, "*paint.net*"] = o => {
			var w = o.Window;
			//Print(w.Child("Save"));
			//APerf.First();
			//w.Send(0);
			////_Sleep(w);
			//APerf.Next();
			////Print(w.Child("Save"));
			//APerf.Write();
			w.Close();
		};

		//int n = 0;
		//foreach(var v in Triggers.Window) {
		//	Print(v);
		//	n++;
		//}
		//Print(n);

		Triggers.Run();
		//Print("stopped");

		//Triggers.Options.RunActionInNewThread(true);
		//Triggers.Window[TWEvent.ActiveNew, "* Notepad"] = o => o.Window.Resize(500, 200);
		//Triggers.Hotkey["Ctrl+T"] = o => Triggers.Window.SimulateActiveNew();
		//Triggers.Hotkey["Ctrl+Alt+T"] = o => Triggers.Stop();
		//Triggers.Run();
	}

	bool _DebugFilterEvent(AWnd w)
	{
		if(0 != w.ClassNameIs("*tooltip*", "SysShadow", "#32774", "QM_toolbar", "TaskList*", "ClassicShell.*"
			, "IME", "MSCTFIME UI", "OleDde*", ".NET-Broadcast*", "GDI+ Hook*", "UIRibbonStdCompMgr"
			)) return false;
		//if(w.HasExStyle(WS_EX.NOACTIVATE) && 0 != w.ProgramNameIs("devenv.exe")) return;
		var es = w.ExStyle;
		bool ret = es.Has(WS_EX.TRANSPARENT | WS_EX.LAYERED) && es.HasAny(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE); //tooltips etc
		if(!ret) ret = es.Has(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE) && es.Has(WS_EX.LAYERED); //eg VS code tips
		if(ret) { /*Print($"<><c 0x80e0e0>    {e}, {w}, {es}</c>");*/ return false; }
		if(Debugger.IsAttached && w.LibNameTL.Like("*Visual Studio")) return false;
		return true;
	}

	void TestMouseTriggers()
	{
		Triggers.Options.RunActionInThread(0, 500);
		var tm = Triggers.Mouse;

		//tm.MoveSensitivity = 0;
		//tm[TMClick.Right, flags: TMFlags.ShareEvent] = o => Print(o.Trigger);
		tm[TMClick.X1, "Ctrl", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		//tm[TMClick.X1, null, TMFlags.Up] = o => Print(o.Trigger, o);
		//tm[TMClick.Right, "Ctrl"] = o => { Opt.Key.NoModOff = true; Key("F22"); Print(o.Trigger); };
		//tm[TMClick.Middle, "Win"] = o => { Print(o.Trigger); };
		//tm[TMClick.Middle, "Win", TMFlags.ShareEvent] = o => { Print(o.Trigger); };
		tm[TMWheel.Forward, "Ctrl", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		tm[TMEdge.RightInTop25] = o => Print(o.Trigger);
		//tm[TMEdge.Right] = o => Print(o.Trigger);
		tm[TMEdge.Right, "Ctrl", TMFlags.RightMod] = o => Print(o.Trigger);
		tm[TMEdge.Left, "Ctrl", TMFlags.LeftMod] = o => Print(o.Trigger);
		tm[TMEdge.BottomInRight25, "Ctrl", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		tm[TMEdge.Top, "?"] = o => Print(o.Trigger);
		tm[TMEdge.Right, "Shift+Alt"] = o => Print(o.Trigger);
		Triggers.Mouse[TMMove.DownUp] = o => Print(o.Trigger);
		//Triggers.Mouse[TMMove.RightLeftInBottom25, "Alt"] = o => Print(o.Trigger);
		//Triggers.Mouse[TMMove.RightLeftInBottom25, "Alt", screenIndex: -1] = o => Print(o.Trigger);
		//Triggers.Mouse[TMMove.RightLeftInBottom25, "Alt", screenIndex: 1] = o => Print(o.Trigger);
		Triggers.Mouse[TMMove.RightLeftInBottom25, "Alt", screen: TMScreen.Any] = o => Print(o.Trigger);

		tm[TMEdge.BottomInLeft25] = o => Print(o.Trigger);
		tm[TMEdge.LeftInTop25, screen: TMScreen.NonPrimary1] = o => Print(o.Trigger);
		tm[TMEdge.LeftInBottom25] = o => Print(o.Trigger);

		//Triggers.Hotkey["Alt+O"] = o => Print(o.Trigger);
		//Triggers.Hotkey["Ctrl+Shift+O"] = o => Print(o.Trigger);

		//tm[TMClick.Right] = o => Print(o.Trigger);
		//tm[TMClick.Right, null, TMFlags.ButtonModUp] = o => Print(o.Trigger);
		tm[TMClick.Right, "Alt"] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Alt", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Alt", TMFlags.ShareEvent] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Alt", TMFlags.ButtonModUp | TMFlags.ShareEvent] = o => Print(o.Trigger);
		Triggers.Options.BeforeAction = o => { AOpt.Key.KeySpeed = 50; /*Opt.Key.NoBlockInput = true;*/ };
		//tm[TMClick.Right, "Alt"] = o => Key("some Spa text Spa for Spa testing Spa Enter");
		//tm[TMClick.Right, "Alt"] = o => Print("wait", AKeys.WaitForKey(0, true, true));
		//tm[TMClick.Right, "Win"] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Shift"] = o => Print(o.Trigger);
		tm[TMClick.Right, "Win", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		tm[TMClick.Right, "Win+Shift"] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Shift", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Shift", TMFlags.ButtonModUp | TMFlags.ShareEvent] = o => Print(o.Trigger);
		//tm[TMClick.Left, "Shift"] = o => {
		//	//2.s();
		//	Opt.Mouse.MoveSpeed = 50;
		//	using(AMouse.LeftDown()) {
		//		//AMouse.MoveRelative(30, 30);
		//		var xy = AMouse.XY;
		//		AMouse.Move(xy.x+30, xy.y+30);
		//	}
		//	Print("done");
		//};


		//Triggers.Hotkey["Ctrl+K"] = o => Print(o.Trigger);
		////Triggers.Autotext["mmpp"] = o => Print(o.Trigger);
		//Triggers.Mouse[TMClick.Right] = o => Print(o.Trigger);
		//Triggers.Mouse[TMWheel.Right] = o => Print(o.Trigger);
		////Triggers.Mouse[TMEdge.Right] = o => Print(o.Trigger);
		//Triggers.Mouse[TMMove.DownUp] = o => Print(o.Trigger);

		//int n = 0;
		//foreach(var v in tm) {
		//	Print(v);
		//	n++;
		//}
		//Print(n);

		//Print("----");
		//Triggers.Hotkey["Ctrl+Q"] = o => Triggers.Stop();
		Triggers.Run();
		//Print("stopped 1");
		//Triggers.Run();
		//Print("stopped 2");
	}

	void TestHotkeyTriggers()
	{
		Triggers.Options.RunActionInThread(0, 500);
		var tk = Triggers.Hotkey;

#if true

		//Triggers.Options.BeforeAction = o => { Opt.Key.KeySpeed = 100; };
		//Opt.Key.KeySpeed = 100;
		//Opt.Static.Key.KeySpeed = 10;
		//Triggers.Options.BeforeAction = o => { Opt.Key.Hook = h => { Print(h.w); if(h.w.Window.ClassNameIs("Notepad")) h.opt.KeySpeed = 100; }; };
		//Opt.Key.Hook = h => { Print(h.w); if(h.w.Window.ClassNameIs("Notepad")) h.opt.KeySpeed = 100; };
		//Opt.Static.Key.Hook = h => { Print(h.w); if(h.w.Window.ClassNameIs("Notepad")) h.opt.KeySpeed = 100; };
		//tk["F11"] = o => { Key("abcdef"); };

		tk["F11"] = o => { AOpt.Key.KeySpeed = 200; Key("abcdef"); };
		tk["F12"] = o => { Key("ghijk"); };

#elif true
		//tk["Shift+K"] = o => { Print(AKeys.IsShift); AMouse.Click(); };
		//tk["Shift+K", TKFlags.NoModOff] = o => { Print(AKeys.IsShift); AMouse.Click(); };
		//tk["Shift+K", TKFlags.NoModOff] = o => { Print(AKeys.IsShift); Key("keys", "some Text"); };
		//tk["Shift+K", TKFlags.NoModOff] = o => { Print(AKeys.IsShift); Opt.Key.NoModOff = true; Key("keys", "some Text"); };
		//tk["Shift+K"] = o => { Print(AKeys.IsShift); Opt.Key.NoModOff = true; Key("keys", "some Text"); };
		tk["Ctrl+K"] = o => { Print(o.Trigger); };
		//tk["Ctrl+K", TKFlags.NoModOff] = o => { Print(o.Trigger); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, AKeys.IsCtrl); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, AKeys.IsCtrl); Text(" ...."); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, AKeys.IsCtrl); Text(" ..", 30, "", ".."); };
		//tk["Ctrl+K"] = o => { Opt.Key.NoBlockInput = true; Print(o.Trigger, AKeys.IsCtrl); Text(" ...."); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, AKeys.IsCtrl); 1.s(); Print(AKeys.IsCtrl); };
		tk["Ctrl+L", TKFlags.LeftMod] = o => { Print(o.Trigger); };
		tk["Ctrl+R", TKFlags.RightMod] = o => { Print(o.Trigger); };
		//tk["Alt+F"] = o => { Print(o.Trigger); };
		//tk["Win+R"] = o => { Print(o.Trigger); };
		//tk["Alt+F", TKFlags.NoModOff] = o => { Print(o.Trigger, AKeys.IsAlt); };
		//tk["Win+R", TKFlags.NoModOff] = o => { Print(o.Trigger, AKeys.IsWin); };
		//tk["Shift+T", TKFlags.NoModOff] = o => { Opt.Key.NoModOff = true; Opt.Key.TextOption = KTextOption.Keys; Text("some Text"); };
		//tk["Alt+F", TKFlags.ShareEvent] = o => { Print(o.Trigger); };
		//tk["Win+R", TKFlags.ShareEvent] = o => { Print(o.Trigger); };
		tk["Win+R", TKFlags.RightMod] = o => { Print(o.Trigger); };
		tk["F11", TKFlags.KeyModUp] = o => { Print(o.Trigger); };
		//tk["F11", TKFlags.KeyModUp| TKFlags.ShareEvent] = o => { Print(o.Trigger); };

		//tk["Alt+E"] = o => { Print(o.Trigger); };
		//tk["Alt+E", TKFlags.NoModOff] = o => { Print(o.Trigger); };
		//tk["Alt+E", TKFlags.ShareEvent] = o => { Print(o.Trigger); };
		//tk["Alt+E", TKFlags.NoModOff|TKFlags.ShareEvent] = o => { Print(o.Trigger); };
		//tk["Alt+E", TKFlags.KeyModUp] = o => { Print(o.Trigger); };
		tk["Alt+E", TKFlags.KeyModUp | TKFlags.ShareEvent] = o => { Print(o.Trigger); };

		tk["Ctrl+Shift+Alt+Win+U", TKFlags.KeyModUp] = o => { Print(o.Trigger); };
		tk["Win+R", TKFlags.NoModOff] = o => { Print(o.Trigger); };

		tk[KKey.Clear, null] = o => { Print(o.Trigger); };
		tk[KKey.Clear, "Ctrl"] = o => { Print(o.Trigger); };
		tk[(KKey)250, null] = o => { Print(o.Trigger); };
		tk[(KKey)250, "Ctrl"] = o => { Print(o.Trigger); };

		tk[KKey.F12, "?"] = o => { Print(o.Trigger, o.Key, o.Mod); };

		tk["Ctrl+VK66"] = o => { Print(o.Trigger); };

		tk["Shift+F11"] = o => { Print(o.Trigger); };
		tk["?+F11"] = o => { Print(o.Trigger); };
#else
		tk["Ctrl+K"] = o => { Print(o.Trigger); };
		tk["Ctrl+Alt+K"] = o => { Print(o.Trigger); };
		tk[KKey.F12, "?"] = o => { Print(o.Trigger, o.Key, o.Mod); };
		tk["Ctrl?+L"] = o => { Print(o.Trigger); };
		tk["Ctrl?+Shift?+M"] = o => { Print(o.Trigger); };
		tk["Ctrl?+Shift+N"] = o => { Print(o.Trigger); };

#endif

		//int n = 0;
		//foreach(var v in tk) {
		//	Print(v);
		//	n++;
		//}
		//Print(n);

		Triggers.Run();
	}

	void TestAutotextTriggers()
	{
		var tt = Triggers.Autotext;
		//tt.PostfixKey = KKey.RShift;

		Triggers.Options.RunActionInThread(0, 500);
		//Triggers.Of.Window("* Notepad");
		//Triggers.Options.BeforeAction = o => { Opt.Key.NoBlockInput=true; };
		//tt["ABCDE"] = o => Print(o.Trigger);
		//tt["A"] = o => Print(o.Trigger);
		tt["kuu"] = o => { o.Replace("dramblys"); };
		tt["#hor"] = o => { o.Replace("horisont"); };
		tt["si#"] = o => { o.Replace("sit down"); };
		tt[".dot"] = o => { o.Replace("density"); };
		tt[".d"] = o => { o.Replace("dynamite"); };
		tt["LL\r\nKK"] = o => { o.Replace("puff"); };
		//tt["kuu", postfix: TAPostfix.None] = o => Print(o.Trigger);
		//tt["kuu", postfix: TAPostfix.Delimiter] = o => Print(o.Trigger);
		tt["ąčę"] = o => { o.Replace("acetilenas"); };
		tt["na;", 0, TAPostfix.None] = o => { o.Replace("naujienos"); };
		//tt["?"] = o => { o.Replace("acetilenas"); };
		//tt["!", 0, TAPostfix.None] = o => { o.Replace("acetilenas"); };
		//tt["<a ", TAFlags.DontErase, TAPostfix.None] = o => { o.Replace("href=\"\"></a>"); };
		tt["<a "] = o => { o.Replace("<a href=\"\">[[|]]</a>"); };
		//tt["<a ", TAFlags.DontErase] = o => { o.Replace("href=\"\">[[|]]</a>"); };
		Triggers.Autotext["#exa"] = o => o.Replace("<example>[[|]]</example>");
		tt["#kh"] = o => { o.Replace("one[[|]]\r\ntwo\r\nthree"); };
		tt["#ki"] = o => { o.Replace("one[[|]]a-𐐨𐐩𐐪-b"); };
		tt["hee"] = o => { o.Replace("𐐩ko"); };
		//Triggers.Options.BeforeAction = o => { Opt.Key.TextOption = KTextOption.Paste; };
		//tt["hee"] = o => { o.Replace("one\r\ntwo\r\nthree"); };
		//tt["tuu", TAFlags.DontErase] = o => { o.Replace("dramblys"); };
		//tt["tuu", TAFlags.RemovePostfix] = o => { o.Replace("dramblys"); };
		tt["tuu", TAFlags.DontErase | TAFlags.RemovePostfix, postfixChars: ",;"] = o => { o.Replace("dramblys"); };
		//tt["kup", postfixChars: ",\r"] = o => { o.Replace("dramblys"); };
		//tt.DefaultPostfixChars = ",\r#_";
		//tt.DefaultPostfixChars = ",\r";
		tt.WordCharsPlus = "_#";
		tt["kup"] = o => { o.Replace("dramblys"); };

		//Triggers.Hotkey["F11"] = o => { Print(o.Trigger); AMouse.Click(); };
		//Triggers.Hotkey["F11"] = o => { Print(o.Trigger); AMouse.Click(); };
		//Triggers.Hotkey["F11"] = o => { Text("text"); };
		//Triggers.Hotkey["F11"] = o => { Key("text"); };
		Triggers.Hotkey["F11"] = o => { Paste("text"); };
		//Triggers.Hotkey["F11"] = o => { Opt.Key.TextSpeed = 500; Text("kaa kaa "); };
		//Triggers.Hotkey["F11"] = o => {
		//	2.s();
		//	AWnd.Find("* Notepad").Activate();
		//};
		//Triggers.Hotkey["u", TKFlags.ShareEvent] = o => { Print(o.Trigger); };

		//Triggers.FuncOf.NextTrigger = o => { var ta = o as AutotextTriggerArgs; ta.Replace("dramblys"); return false; };
		//tt["kut"] = null;

		tt["mee"] = o => {
			//100.ms();
			var m = new AMenu();
			m["one"] = u => Text("One");
			m["two"] = u => Text("Two");
			m.Show(true);
		};

		tt["mii", TAFlags.Confirm] = o => { o.Replace("dramblys"); };
		//		tt["mii", TAFlags.Confirm] = o => { o.Replace(@"1>------ Build started: Project: Au, Configuration: Debug Any CPU ------
		//1>  Au -> Q:\app\Au\Au\bin\Debug\Au.dll
		//2>------ Build started: Project: TreeList, Configuration: Debug Any CPU ------
		//3>------ Build started: Project: Au.Compiler, Configuration: Debug Any CPU ------
		//"); };
		tt["con1", TAFlags.Confirm] = o => o.Replace("Flag Confirm");
		tt["con2"] = o => { if(o.Confirm("Example")) o.Replace("Function Confirm"); };

		var ts = Triggers.Autotext.SimpleReplace;
		ts["#su"] = "Sunday"; //the same as Triggers.Autotext["#su"] = o => o.Replace("Sunday");
		ts["#mo"] = "Monday";
		ts["#mokkkk"] = "Monday";
		ts["#sukkkk"] = "Sunday";

		Triggers.Of.Window("* Notepad");

		Triggers.Mouse[TMEdge.RightInBottom25] = o => Print(o.Trigger);
		Triggers.Window[TWEvent.ActiveNew, "* Notepad"] = o => Print(o.Trigger);

		//int n = 0;
		//foreach(var v in tt) {
		//	Print(v);
		//	n++;
		//}
		//Print(n);

		Triggers.Run();
	}

	void TestProcessStarter()
	{
		//var exe = @"Q:\My QM\console4.exe";

		//var ps = new Au.Util.LibProcessStarter(exe, "COMMAND", "", "moo=KKK");
		//ps.Start();
		//ps.StartUserIL();
		//ps.Start(0, true);

		//var r = ps.Start(Au.Util.LibProcessStarter.Result.Need.WaitHandle);
		//r.waitHandle.WaitOne(-1);

		//var r = ps.Start(Au.Util.LibProcessStarter.Result.Need.NetProcess);
		////var r = ps.StartUserIL(Au.Util.LibProcessStarter.Result.Need.NetProcess);
		//r.netProcess.WaitForExit();

		//Print(AExec.RunConsole(s => Print(s.Length, s), exe));
		//Print(AExec.RunConsole(exe));
		//Print(AExec.RunConsole(out var s, exe)); Print(s);
		//Print(AExec.RunConsole(s => Print(s.Length, s), exe, rawText: true));
		//Print(AExec.RunConsole(exe, "cmd", "q:\\programs", encoding: Encoding.UTF8));

		//string v = "example";
		//int r1 = AExec.RunConsole(@"Q:\Test\console1.exe", $@"/an ""{v}"" /etc");

		//int r2 = AExec.RunConsole(s => Print(s), @"Q:\Test\console2.exe");

		//int r3 = AExec.RunConsole(out var text, @"Q:\Test\console3.exe", encoding: Encoding.UTF8);
		//Print(text);

		//Print("exit");

		//if(AKeys.IsCtrl);
		//if(AKeys.IsCtrl);
	}

	void TestRunConsole()
	{
		//var exe = @"Q:\My QM\console4.exe";
		var exe = @"Q:\Test\ok\bin\console5.exe";

#if true
		int ec = AExec.RunConsole(s => {
			Print(s);
			//Print(s.Length, (int)s[s.Length - 1]);
			Print(s.Length);
		},
		exe,
		//flags: RCFlags.RawText,
		encoding: Encoding.UTF8);
		//Print(ec);
#elif true
		AExec.RunConsole(out var s, exe, encoding: Encoding.UTF8);
		Print(s);
#else
		using(var p = new Process()) {
			var ps = p.StartInfo;
			ps.FileName = exe;
			ps.UseShellExecute = false;
			ps.CreateNoWindow = true;
			ps.RedirectStandardOutput = true;
			p.OutputDataReceived += (sen, e) => {
				var s = e.Data;
				Print(s);
			};
			p.Start();
			p.BeginOutputReadLine();
			p.WaitForExit();
		}
#endif
	}

	void TestManyTriggerActionThreads()
	{
		//AWnd.Find("* Notepad").Activate();

		Triggers.Options.RunActionInNewThread(false);
		var t = Triggers.Hotkey;
		int n = 0;
		t["F11"] = o => { Print(++n); 30.s(); n--; };

		try { Triggers.Run(); }
		catch(Exception ex) { Print(ex); }

	}

	//class DebugListener :TraceListener
	//{
	//	public override void Write(string message) => AOutput.Write(message);

	//	public override void WriteLine(string message) => AOutput.Write(message);
	//}

	//void TestCs8()
	//{
	//	int i = 1, j = 2;
	//	switch(i, j) {
	//	case (1, 2):
	//		Print("yess");
	//		break;
	//	}

	//	int k = j switch { 1 => 10, 2 => 20, _ => 0 };
	//	Print(k);

	//	POINT p = (3, 4);
	//	if(p is POINT { x: 3, y: 4 }) Print("POINT(3,4)");

	//	//Range range = 1..5; //no
	//	//var s = "one"; Print(s[^1]); //no

	//	//using var _ = new TestUsing();
	//	//using new TestUsing(); //no
	//	//TestUsing v = null; using v; //no

	//	Loc(i);
	//	static void Loc(int u)
	//	{
	//		Print(u);
	//	}


	//	switch(ADialog.Show("test", "mmm", "1 OK|2 Cancel")) {

	//	}

	//	int dr = ADialog.Show("test", "mmm", "1 OK|2 Cancel") switch
	//	{
	//		1 => 10,
	//		_ => 20
	//	};
	//}

	class TestCs8Using : IDisposable
	{
		public void Dispose()
		{
			Print("Dispose");
		}
	}

	//	void TestIronPython()
	//	{
	//		var py = IronPython.Hosting.Python.CreateEngine();
	//		//var r = py.Execute(@"print 5");

	//		py.SetSearchPaths(new string[] { @"Q:\Downloads\IronPython.2.7.9\Lib" });

	//		var code = @"
	//def Script():
	//	print(""test"")
	//	print("""") # empty line
	//	print(""one\ntwo"") # multiline
	//	print(2) # number


	//# ---------------------------------------------------------------------------------
	//# Put your Python script in the Script() function. Don't need to change other code.

	//import sys
	//import ctypes
	//import os
	//import win32gui

	//class PrintRedirector(object):
	//	def __init__(self):
	//		self.hwndQM=win32gui.FindWindow(""QM_Editor"", None)

	//	def write(self, message):
	//		if(message=='\n'): return
	//		ctypes.windll.user32.SendMessageW(self.hwndQM, 12, -1, message)

	//sys.stdout = PrintRedirector()
	//try: Script()
	//finally: sys.stdout = sys.__stdout__";

	//		var r = py.Execute(code);
	//		Print(r);

	//		//ADialog.Show("");
	//	}

	//void TestDiffMatchpatch()
	//{
	//	var s1 = "using System; using Au; using Au.Types;";
	//	var s2 = "using System; using MyLib; using Au;";

	//	//var dmp = new DiffMatchPatch.DiffMatchPatch(1f, 0.5);
	//	var dmp = new diff_match_patch();
	//	List<Diff> diff = dmp.diff_main(s1, s2);

	//	//dmp.DiffCleanupSemantic(diff);
	//	//dmp.DiffCleanupEfficiency(diff);
	//	//dmp.DiffCleanupMerge(diff);
	//	//dmp.DiffCleanupSemanticLossless(diff);

	//	foreach(var v in diff) {
	//		Print(v.operation, v.text);
	//	}

	//	//Print(dmp.DiffText1(diff));
	//	//Print(dmp.DiffText2(diff));
	//	//Print(dmp.DiffPrettyHtml(diff));
	//	var delta = dmp.diff_toDelta(diff);
	//	Print(delta);
	//	var d2 = dmp.diff_fromDelta(s1, delta);
	//	Print(d2);
	//	Print(dmp.diff_text2(d2));

	//	//Print("prefix, suffix");
	//	//Print(dmp.DiffCommonPrefix(s1, s2));
	//	//Print(dmp.DiffCommonSuffix(s1, s2));

	//	//Print(dmp.DiffXIndex(diff, 30));



	//	//var la = new List<string>(diff.Count);
	//	//for(int i = 0; i < diff.Count*100; i++) la.Add(null);
	//	//dmp.DiffCharsToLines(diff, la);
	//	//Print(la);
	//}

	class FormRegisterHotkey : Form
	{
		ARegisteredHotkey[] _a = new ARegisteredHotkey[7];

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case 1: //0x1
				bool r1 = _a[0].Register(1, "F1", this);
				bool r2 = _a[1].Register(2, "Shift+F1", this);
				bool r3 = _a[2].Register(3, "Ctrl+F1", this);
				bool r4 = _a[3].Register(4, "Alt+F1", this);
				bool r5 = _a[4].Register(5, "Win+F2", this);
				bool r6 = _a[5].Register(6, "Pause", this);
				bool r7 = _a[6].Register(7, "Alt+Pause", this);
				Print(r1, r2, r3, r4, r5, r6, r7);
				break;
			case 2: //0x2
				foreach(var v in _a) v.Unregister();
				break;
			case ARegisteredHotkey.WM_HOTKEY:
				Print(m.WParam);
				break;
			}
			base.WndProc(ref m);
		}
	}

	void TestTodo()
	{
		//Action<MTClickArgs> f=o => Au.Util.AHelp.AuHelp(o.ToString());

		//var m = new AMenu();
		//m[""] = o => Au.Util.AHelp.AuHelp("");
		//m["api/"] = f;
		//m["Au.AAcc.Find"] = f;
		//m["AWnd.Find"] = f;
		//m["Au.Types.RECT"] = f;
		//m["Types.Coord"] = f;
		//m["articles/Wildcard expression"] = f;
		//m.Show();

		//string s = "abcdefghijklmnoprstuv";
		//Print(s.Starts("ab"));
		//Print(s.Starts("Ab"));
		//Print(s.Starts("Ab", true));
		//Print(s.Has("bc", true));
		//s.Starts()
		//s.Lower
		//s.Has("", true)

		//Debug.Listeners.Add(new DebugListener());
		//Trace.Listeners.Add(new DebugListener());
		//Trace.Listeners.Add(new ConsoleTraceListener());
		//AOutput.RedirectDebugOutput = true;
		//Debug.Indent();
		//Debug.Print("Debug.Print");
		//Debug.Write("Debug.Write\n");
		////Debug.WriteLine("Debug.WriteLine");
		//Trace.Write("Trace.Write\r\n");
		//Trace.Write("Trace.Write2\r\n");
		////Trace.WriteLine("Trace.WriteLine");
		//Print("END");

		//Debug.Write("one");
		//Debug.Write("two");
		////Debug.Flush();
		//Debug.Print("|");

		//Debug.WriteLine("List of errors:");
		//Debug.Indent();
		//Debug.WriteLine("Error 1: File not found");
		//Debug.WriteLine("Error 2: Directory not found");
		//Debug.Unindent();
		//Debug.WriteLine("End of list of errors");

		//for(int i = 1; i <= 17; i++) Print(AClipboard.LibGetFormatName(i));

		//Print(s.RemoveSuffix("CD", true));
		//Print(s.RemoveSuffix('d'));

		//int limit = 8;
		//Print(s.Limit(limit));
		//Print(s.Limit(limit, true));

		//if(s.Regex(""))

		//AExec.SelectInExplorer(@"q:\app\au\au\_doc");

		//s = "  abcdefghijklmnoprstuv";
		////s = "                                                                                                                                                                        abcdefghijklmnoprstuv";

		//var c = new char[] { 't', 'u' };
		//int i = 0, j = 0, k = 0, m = 0, n = 0, last=0;
		//i = s.IndexOfAny(new char[] { 't', 'u' });
		//Print(i);
		//k = s.FindChars("tu");
		//Print(k);
		//m = s.FindChars("tu", 1, s.Length - 2);
		//Print(m);
		//Print(s.FindChars(" \r\n", not: true));
		////return;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { i = s.IndexOfAny(new char[] { 't', 'u' }); }
		//	//APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { j = s.IndexOfAny(c); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { k = s.FindChars("tu"); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { m = s.FindChars("tu", 1, s.Length - 2); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { n = s.FindChars(" \r\n", not: true); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { last = s.FindLastChars(" \n", not: false); }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
		//Print(i, j, k, m, n, last);

		//s = "//one\\\\";
		//Print(s.Trim('/', '\\'));
		//Print(s.TrimChars("/\\"));

		//string s1 = null, s2 = null, s3 = null;
		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { s1=s.Trim('/', '\\'); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { s2=s.Trim(AExtString.Lib.pathSep); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { s3=s.TrimChars("/\\"); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
		//Print(s1, s2, s3);

		//s = "\u1234";
		//Print(s.Length);

		//s = "a \\ \" \t \r \n \0 \u0001 b";
		//var e = s.Escape();
		//var e = AClipboardData.GetText();
		//Print(e);
		//if(e.Unescape(out var u)) {
		//	var a = u.ToCharArray();
		//	foreach(var c in a) Print((uint)c);
		//} else Print("FAILED");

		//Print(",𝄎bcd𐊁 bcd,".FindWord("bcd𐊁"));
		//Print(",bcd, bcd,".FindWord("bcd,"));
		//Print("a_moo, _moo_, _moo,".FindWord("_moo", otherWordChars: "_"));

		//s = "    dshjdhsj    dskdlskdl         fsfhsfsyyf           uyuyuyu needle dshdjshdjhs";
		//var f = "needle";
		//var rx = @"\bneedle\b";
		//int i = 0, j = 0, k = 0, n=0;

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { i = s.Find(f); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { j = s.FindWord(f); }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { k = s.RegexMatch(rx, 0, out RXGroup g) ? g.Index : -1; }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var m = Regex.Match(s, rx, RegexOptions.CultureInvariant); n = m.Success ? m.Index : -1; }
		//	APerf.NW();
		//	Thread.Sleep(200);
		//}
		//Print(i, j, k, n);

		//foreach(var f in AFile.EnumDirectory(@"C:\Users\G\AppData\Roaming", FEFlags.AndSubdirectories, filter: k => k.IsDirectory || k.Name.Ends(".png", true))) {
		//	if(f.IsDirectory) continue;
		//	Print(f.FullPath);
		//}

		//foreach(var f in AFile.EnumDirectory(@"C:\Users", FEFlags.AndSubdirectories, filter: k => !k.Name.Eqi("G"), errorHandler: p => Print("failed", p))) {
		//	//if(f.IsDirectory) continue;
		//	Print(f.FullPath);
		//}

		//AWnd.Find("* Notepad").Activate();
		//2.s();
		//Print("start");
		//using var bi = new AInputBlocker(BIEvents.Keys);
		//bi.ResendBlockedKeys = true;
		//2.s();
		//Print("stop");
		//bi.Stop(true);

		//var f = new Au.Tools.FormAWinImage();
		//f.ShowDialog();

		//string s1 = "abcdefgh";
		//string s2 = null;
		//Print(s1.Eq(null), s2.Eq(null), s1.Eq("abcdefgh"), s2.Eq("abcdefgh"));
		//Print(s1.Eqi(null), s2.Eqi(null), s1.Eqi("abcdefgh"), s2.Eqi("abcdefgh"));
		//Print(s1.Eq(false, null, "abcdefgh"), s2.Eq(false, null, "abcdefgh"), s1.Eq(false, "abcdefgh", null), s2.Eq(false, "abcdefgh", null));
		//Print(s1.Eq(2, "cd"), s2.Eq(2, "cd"), s1.Eq(20, "cd"), s1.Eq(2, false, "cd"), s2.Eq(2, false, "cd"));
		//Print(s1.Ends("abcdefgh"), s2.Ends("abcdefgh"));
		//Print(s1.Starts("abcdefgh"), s2.Starts("abcdefgh"));
		//Print(s1.Like("abcdefgh"), s2.Like("abcdefgh"));
		//Print(s1.Find("cd"), s2.Find("cd"));
		//Print(s1.Has("cd"), s2.Has("cd"));
		//Print(s1.FindChars("cd"), s2.FindChars("cd"));
		//Print(s1.FindLastChars("cd"), s2.FindLastChars("cd"));
		//Print(s1.FindWord("abcdefgh"), s2.FindWord("abcdefgh"));
		//s1 = "12 kb"; Print(s1.ToInt(), s2.ToInt());
		//Print(s1.Regex("cd"), s2.Regex("cd"));
		//Print(s1.RegexMatch("cd", out var r1), s2.RegexMatch("cd", out var r2));
		//Print(s1.RegexMatch("cd", 0, out string r3), s2.RegexMatch("cd", 0, out string r4));
		//Print(s1.RegexMatch("cd", 0, out RXGroup r5), s2.RegexMatch("cd", 0, out RXGroup r6));

		//var (i, w) = AWnd.WaitAny(10, true, "* Notepad", new AWnd.Finder("* Word"));
		//Print(i, w);

		//AOutput.RedirectConsoleOutput = true;

		//#if true
		//		var w = AWnd.Find("Quick *").OrThrow();
		//#if true
		//		var f = new Au.Tools.FormAWnd(w);
		//		f.ShowDialog();
		//#else
		//		var f2 = new Au.Tools.FormAWinImage();
		//		f2.ShowDialog();
		//#endif
		//#else
		//		var f = new Form();
		//		var t1 = new TextBox { Location = new Point(10, 10), Text = "one" };
		//		var t2 = new TextBox { Location = new Point(10, 50), Text = "two" };
		//		f.Controls.Add(t1);
		//		f.Controls.Add(t2);
		//		f.ShowDialog();
		//#endif

		//Print(APath.ExpandEnvVar("%AFolders.Documents%"));

		//var f = new Form();
		//f.StartPosition = FormStartPosition.CenterScreen;
		//var e = new TextBox();
		//f.Controls.Add(e);
		//f.Click += (unu, sed) => {
		//	var k = new Au.Controls.PopupList();
		//	k.Items = AWnd.GetWnd.AllWindows().Select(o=>o.ToString()).ToArray();
		//	k.Show(e);
		//};
		//f.ShowDialog();

		//var f = new FormRegisterHotkey();
		//f.ShowDialog();

		//using(new AInputBlocker(BIEvents.Keys)) {
		//	ADialog.ShowYesNo();
		//}

		//var w = AWnd.Find("* Notepad");
		//for(int i = 0; i < 3; i++) {
		//	2.s();
		//	w.Activate();
		//}

		//for(int i = 0; i < 10; i++) {
		//	Print(i);
		//}

		//foreach(int i in Times(10)) {
		//	Print(i);
		//}

		//for(_i = 0; _i < 10; _i++) {
		//	Print(_i);
		//}

		//foreach(_i in Times(10)) {
		//	Print(_i);
		//}

		//10.Times(() => {
		//	Print(1);
		//});

		//10.Times(i => {
		//	Print(i);
		//});

		//3.Times(i => {
		//	3.Times(i => {
		//		Print(i);
		//	});
		//	Print("outer", i);
		//});


		//for(int i = 0; i < 3; i++) {
		//	Print(1);
		//}

		//3.Times(() => {
		//	Print(2);
		//});

		//for(int i = 0; i < 3; i++) {
		//	Print(i);
		//}

		//3.Times(i => {
		//	Print(i);
		//});

		//3.Times(i => Print(i));

		//for(int i = 0; i < 3; i++) Print(i);

		//Repeat(3, i => Print(i));

		//for(int i = 0; i < 3; i++) {
		//	Print(i);
		//}

		//foreach(int i in Repeat(3)) {
		//	Print(i);
		//}

		//var f = new Form();
		//var n = new NotifyIcon();
		//n.Text = "test";
		//n.Icon = AIcon.GetWindowIcon(AWnd.Find("* Firefox"));
		//n.MouseClick += (se, e) => {
		//	var m = new AMenu();
		//	m["test"] = o => Print(o);
		//	m.Show();
		//};
		//f.Load += (unu, sed) => { n.Visible = true;};
		//f.ShowDialog();
		//n.Dispose();

		//Print("ab\r\n".Regex("^", RXFlags.MULTILINE | RXFlags.ALT_CIRCUMFLEX, new RXMore(1)));

		//string s;
		//using(StreamReader sr = new StreamReader("", Encoding.UTF8)) {
		//	_=(sr.BaseStream as FileStream).Length;
		//	s = sr.ReadToEnd();
		//}

		//var s = "one\r\ntwo\r\nthree\r\n\four\r\n";
		//Print(s.LineCount());

	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	void _TestComboWrapper(Control c)
	{
		var x = new ComboWrapper(c);
		x.ArrowButtonPressed += (unu, sed) => Print("arrow");
		x.ButtonImage = AIcon.GetFileIconImage(@"c:\", 16);
		x.ImageButtonClicked += (unu, sed) => Print("image");
	}

	void TestComboWrapper()
	{
		Task.Run(() => { for(; ; ) { 1.s(); GC.Collect(); } });
		var f = new AuForm();
#if true
		var c = new TextBox();
		//var c = new RichTextBox();
		//var c = new AuScintilla { InitBorderStyle = BorderStyle.FixedSingle };
		//var c = new Button { Text = "OK", Width = 100 };
		_TestComboWrapper(c);
		//ATimer.After(2000, () => _TestComboWrapper(c));

		//var x = new ComboWrapper(c);
		//x.ArrowButtonPressed += (unu, sed) => Print("arrow");
		//x.ButtonImage=AIcon.GetFileIconImage(@"c:\", 16);
		//x.ImageButtonClicked += (unu, sed) => Print("image");

		//ATimer.After(2000, () => c.Dispose());
		//ATimer.After(2000, () => { c.Multiline = true; });
#else
		var c = new AuComboBox();
#endif
		f.Controls.Add(c);
		f.ShowDialog();
		f.Dispose();

		//ADialog.Show();
	}

	void TestSciTagsRegisteredStyles()
	{
		var f = new AuForm();
		var t = new InfoBox();
		t.ZInitUseControlFont = true;

		t.HandleCreated += (unu, sed) => {
			t.ZTags.AddStyleTag(".k", new SciTags.UserDefinedStyle { textColor = 0xff, italic = true, backColor = 0x00ff00 });
			t.ZTags.AddStyleTag(".r", new SciTags.UserDefinedStyle { textColor = 0xf08080 });
		};
		//ATimer.After(2000, () => t.Text = "<.k>user<> tag <c green>green<>");
		ATimer.After(2000, () => {
			t.ZTags.AddStyleTag(".late", new SciTags.UserDefinedStyle { backColor = 0x00ffff });
			t.Text = "<.late>user<> tag <c green>green<>";
		});

		string s;
		s = "aa <c green>green<>\n<.k>user<> tag <.k>user<> tag <c green>green<>";
		//s = "aa <size 15>big <.k>user<> big<> bb";
		//s = "<.r>(?m)<> - multiline";

		t.Text = s;
		f.Controls.Add(t);
		f.ShowDialog();
	}

	void TestCalculatePopupWindowPosition()
	{
		var w = AWnd.Find("* Notepad").OrThrow();
		w.Activate();
		var er = w.Rect;

		SIZE z = (200, 200);
		POINT p = (er.right, er.top);

		if(!Api.CalculatePopupWindowPosition(p, z, 0, er, out var r)) throw new AuException(0);
		var osd = new AOsdRect();
		osd.Rect = r;
		osd.Show();
		1.s();
	}

	void TestAuInfoWindow()
	{
		var f = new Form();
		var t = new InfoWindow();
		t.Text = "My <c green>green<>\n<link http://www.quickmacros.com>QM2<>\nfold <fold>one\ntwo</fold>";
		//t.Text = AClipboard.Text;
		//t.Size = (200, 100);
		t.Caption = "Regex info";
		f.Click += (unu, sed) => {
			t.Show(f);
			//t.Show(new Rectangle(100, 100, 100, 100));
			//t.Show(f, new Rectangle(100, 100, 100, 100));
			//ATimer.After(2000, () => t.Hide());
			////ATimer.After(2000, () => t.Dispose());
			//ATimer.After(4000, () => t.Show(f));
			//ATimer.After(2000, () => t.Show(f)); //during that time move the owner form
			//ATimer.After(2000, () => t.Size=(500,500));
			//ATimer.After(2000, () => { t.Hide(); t.Size = (500, 500); t.Show(f); });
		};
		f.ShowDialog();
	}

	void TestRegexInfoWindow()
	{
		var f = new AuForm { StartPosition = FormStartPosition.Manual, Location = new Point(400, 1200) };
		var t = new TextBox();
		f.Controls.Add(t);
		var b = new Button { Text = "Test", Top = 50 };
		f.Controls.Add(b);

		var r = new Au.Tools.RegexWindow { InsertInControl = t };
		f.Load += (unu, sed) => {
			//r.ContentText = @"Q:\app\Au\Tools\Regex.txt";
			r.Show(f);
			//b.Click+=(unu,sed)=> r.Show(f);
			b.Click += (unu, sed) => { r.Dispose(); r.Show(f); };
		};
		f.Click += (unu, sed) => {
			r.ContentText = @"Q:\app\Au\Tools\Regex.txt";
			r.Refresh();
		};
		f.ShowDialog();
	}

	void TestXmlFormatting()
	{
		var x = AExtXml.LoadElem(@"C:\Users\G\Documents\Au\!Settings\Settings.xml");
		Print(x.Element("n").Value.Length, x.Element("s").Value);
		//var x = XElement.Load(@"C:\Users\G\Documents\Au\!Settings\Settings.xml");
		x.SetElementValue("test", "value");
		var f2 = @"C:\Users\G\Documents\Au\!Settings\Settings2.xml";
		//x.SaveElem(f2);

		x.Save(f2);


		Print(File.ReadAllText(f2));
	}

	void TestRegexReplaceEx()
	{
		var s = "yyn!";
		var rx = "(y)?|n";
		var rep = "${1:+yes:no}";

		//var x = new Regex(rx);
		//Print(x.Replace(s, rep));

		//s = "/*/ meta /*/\nmore";
		////s = "more";
		//rx = @"(?s-m)^(/\*/.*?/\*/(*MARK: ))?+\R?+";
		//rep = "$1$*//{{ using\n";

		//Print(s.RegexReplace(rx, rep));

		rx = @"(\d)\d+";
		//rx = @"(\d)(?C)\d+";
		s = "one 12 two 34 three";
		rep = @"$1<R>$>";//rep = null;

		var x = new ARegex(rx);
		//x.Callout = o => { Print(o.current_position); };

		//Print(x.ReplaceEx(s, rep, out s, false), s);
		Print(x.Replace(s, rep, out s), s);
	}

	void TestRegexMatchDataStack()
	{
		var x = new ARegex(@"(\d+)(\s+)!");
		var s = "aadjksjdksjdk 123  ! 87 !";

		if(x.MatchG(s, out var g, 1)) Print(g);

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			APerf.First();
			for(int i2 = 0; i2 < n2; i2++) { x.MatchG(s, out g, 1); }
			APerf.NW();
			Thread.Sleep(200);
		}
	}

	public static class Caching
	{
		/// <summary>
		/// A generic method for getting and setting objects to the memory cache.
		/// </summary>
		/// <typeparam name="T">The type of the object to be returned.</typeparam>
		/// <param name="cacheItemName">The name to be used when storing this object in the cache.</param>
		/// <param name="cacheTimeMS">The cache entry will be evicted if it has not been accessed in this time (ms).</param>
		/// <param name="objectSettingFunction">A parameterless function to call if the object isn't in the cache and you need to set it.</param>
		/// <returns>An object of the type you asked for</returns>
		public static T GetObjectFromCache<T>(string cacheItemName, int cacheTimeMS, Func<T> objectSettingFunction, CacheEntryRemovedCallback removedCallback = null)
		{
			APerf.Next();
			ObjectCache cache = MemoryCache.Default;
			var cachedObject = (T)cache[cacheItemName]; //~30 mcs. Why so slow?
			if(cachedObject == null) {
				APerf.Next();
				CacheItemPolicy policy = new CacheItemPolicy();
				policy.SlidingExpiration = TimeSpan.FromMilliseconds(cacheTimeMS); //bug: if eg 2000 ms, and we access every 1000 ms, expires anyway
				policy.RemovedCallback = removedCallback;
				cachedObject = objectSettingFunction();
				cache.Set(cacheItemName, cachedObject, policy);
			}
			APerf.NW();
			return cachedObject;
		}
	}

	class Cached : IDisposable
	{
		public Cached()
		{
			Print("ctor");
		}

		~Cached() => Dispose(false);

		public void Dispose() => Dispose(true);

		protected void Dispose(bool disposing)
		{
			Print("dispose", disposing);
		}
	}

	void TestMemoryCache()
	{
		Task.Run(() => { for(; ; ) { 200.ms(); GC.Collect(); } });
		//_TestMemoryCache();
		ATimer.Every(1000, () => _TestMemoryCache()); //why removes if 1000? Not if 500 or 1500.
		ADialog.Show();
	}

	void _TestMemoryCache()
	{
		APerf.First();
		var c = Caching.GetObjectFromCache("test", 2000, () => new Cached(), o => Print("removed", o.RemovedReason));
	}

	void TestWeakReferenceWithTimeout()
	{
		Task.Run(() => { for(; ; ) { 1000.ms(); GC.Collect(); } });
		var w = new WeakReference<Cached>(null);
		_TestWeakReferenceWithTimeout(w);
		ADialog.Show();
	}

	void _TestWeakReferenceWithTimeout(WeakReference<Cached> w)
	{
		//w.SetTarget(new Cached());
		w.SetTargetWithTimeout(new Cached(), 3000);
	}

	void TestNetTimer()
	{
		var t = new System.Threading.Timer(o => { var k = Thread.CurrentThread; Print("timer", k.ManagedThreadId, k.GetApartmentState(), k.IsBackground, k.IsThreadPoolThread); });
		//t.Change(2000, -1);
		//ADialog.Show();
		var m = new ATimer(() => Print("A"));
		var f = new Form();
		f.Click += (unu, sed) => { APerf.First(); t.Change(2000, -1); APerf.NW(); };
		//f.Click += (unu, sed) => { APerf.First(); m.Start(1000, true); APerf.NW(); };
		f.ShowDialog();
	}

	void TestCalcellationToken()
	{
		var cancel = new CancellationTokenSource();
		var token = cancel.Token;
		Task.Run(() => {
			for(int i = 0; i < 1000; i++) {
				10.ms();
				if(token.IsCancellationRequested) { Print(i); break; }
			}
		});
		1000.ms();
		cancel.Cancel();
		cancel.Cancel();
		100.ms();
		Print(cancel.IsCancellationRequested, token.IsCancellationRequested);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	string _Initf(int b)
	{
		Print(b);
		return "A" + b;
	}

	string _sku1;
	string Sku1 => _sku1 ?? (_sku1 = _Initf(1));

	string _sku2;
	string Sku2 => _sku2 ??= _Initf(2);

	void TestNullOperator()
	{
		//Print(Sku1, Sku2);
		//Print(Sku1, Sku2);
		Print(Sku2);
	}

	void TestWR()
	{
		var f = new Form();
		var b1 = new Button { Text = "Get" };
		b1.Click += (unu, sed) => {
			if(s_wr.TryGetTarget(out var s)) Print("cached");
			else {
				Print("new");
				var n = new _Dtor();
				//s_var = n;
				s_wr.SetTarget(new List<_Dtor> { n });
			}
		};
		var b2 = new Button { Text = "GC", Location = new Point(80, 0) };
		b2.Click += (unu, sed) => {
			GC.Collect();
		};
		var b3 = new Button { Text = "=null", Location = new Point(160, 0) };
		b3.Click += (unu, sed) => {
			s_var = null;
		};
		f.Controls.Add(b1);
		f.Controls.Add(b2);
		f.Controls.Add(b3);
		f.ShowDialog();
	}
	static WeakReference<List<_Dtor>> s_wr = new WeakReference<List<_Dtor>>(null);
	static _Dtor s_var;
	class _Dtor { ~_Dtor() => Print("dtor"); }

	//void TestWR()
	//{
	//	var f = new Form();
	//	var b1 = new Button { Text = "Get" };
	//	b1.Click += (unu, sed) => {
	//		if(s_wr.TryGetTarget(out var s)) Print("cached");
	//		else {
	//			Print("new");
	//			var n = new _Dtor();
	//			s_var = n;
	//			s_wr.SetTarget(n);
	//		}
	//	};
	//	var b2 = new Button { Text = "GC", Location=new Point(80,0) };
	//	b2.Click += (unu, sed) => {
	//		GC.Collect();
	//	};
	//	var b3 = new Button { Text = "=null", Location=new Point(160,0) };
	//	b3.Click += (unu, sed) => {
	//		s_var = null;
	//	};
	//	f.Controls.Add(b1);
	//	f.Controls.Add(b2);
	//	f.Controls.Add(b3);
	//	f.ShowDialog();
	//}
	//static WeakReference<_Dtor> s_wr=new WeakReference<_Dtor>(null);
	//static _Dtor s_var;
	//class _Dtor { ~_Dtor() => Print("dtor"); }

	struct _KV //:IEquatable<_KV>
	{
		public string k, v;
		//public bool Equals(_KV other) => k.Eq(other.k);
	}

	void TestDictionaryAndListSearchSpeed()
	{
		var d = new Dictionary<string, string>();
		//var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		var k = new List<_KV>();
		foreach(var v in AFile.EnumDirectory(AFolders.NetFrameworkRuntime)) {
			if(!v.Name.Ends(".dll", true)) continue;
			//Print(v.Name);
			d.Add(v.Name, v.FullPath);
			k.Add(new _KV { k = v.Name, v = v.FullPath });
			if(k.Count == 100) break;
		}
		Print(k.Count);

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 7; i1++) {
			APerf.First();
			foreach(var g in k) {
				if(!d.TryGetValue(g.k, out var v)) Print("no");
			}
			APerf.Next();
			foreach(var g in k) {
				var s = g.k;
				//if(!k.Exists(x => x.k.Eq(s))) Print("no");

				bool found = false;
				for(int i = 0; i < k.Count; i++) if(k[i].k == s) { found = true; break; }
				if(!found) Print("no");

				//if(!k.Contains(g)) Print("no");
			}
			APerf.NW();
			Thread.Sleep(200);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	void _TestTimers()
	{
		//new System.Threading.Timer(o => { Print("tim"); }, null, 1000, -1);

		//var t = new System.Timers.Timer();
		//t.Elapsed += (_, e) => Print("tim");
		////t.Disposed += (unu, sed) => Print("disposed");
		//t.AutoReset = false;
		//t.Interval = 1000;
		//t.Start();

		//ATimer.After(1000, () => Print("tim"));

		Task.Delay(1000).ContinueWith(_ => Print("tim"));
	}

	void TestTimers()
	{
		APerf.First();
		_TestTimers();
		APerf.NW();
		//GC.Collect();
		//ATimer.After(5000, () => GC.Collect());
		ADialog.Show();
		100.ms();
	}

	void TestTimersCpu()
	{
		ATimer.Every(100, () => { int i = 7; }); //0.045 %
												 //new System.Threading.Timer(_ => { int i = 7; }, null, 0, 100); //0.095 %
		ADialog.Show();
	}

	void TestDecompressInvalidZipData()
	{
		APerf.First();
		try {
			var s = "123456";
			var b = ImageUtil.BmpFileDataFromString(s, ImageUtil.ImageType.Base64CompressedBmp);

		}
		finally { APerf.NW(); }
	}

	void BuildDocumentationProviderDatabase()
	{
		var dbPath = AFolders.ThisAppBS + "CiDoc.db";
		AFile.Delete(dbPath);
		using var d = new ASqlite(dbPath, sql: "PRAGMA page_size = 8192;"); //8192 makes file smaller by 2-3 MB.
		using var trans = d.Transaction();
		d.Execute("CREATE TABLE data (name TEXT PRIMARY KEY, xml TEXT)");
		using var statInsert = d.Statement("INSERT INTO data VALUES (?, ?)");
		using var statDupl = d.Statement("SELECT xml FROM data WHERE name=?");
		var haveRefs = new List<string>();
		var uniq = new Dictionary<string, string>(); //name -> asmName

		_AddFile("Au.dll", AFolders.ThisAppBS + "Au.xml");

		var srcDir = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2";
		string netDir = AFolders.NetFrameworkRuntime + @"\", wpfDir = netDir + @"\WPF\";
		foreach(var f in AFile.EnumDirectory(srcDir)) {
			if(f.IsDirectory) continue;
			if(!f.Name.Ends(".xml", true)) continue;
			var refName = APath.GetFileName(f.Name, withoutExtension: true);
			var xmlPath = f.FullPath;
			if(!f.Name.Eqi("namespaces.xml")) {
				var dllName = Path.ChangeExtension(f.Name, "dll");
				if(!(AFile.ExistsAsFile(netDir + dllName) || AFile.ExistsAsFile(wpfDir + dllName))) {
					Print("<><c 0x808080>" + f.Name + "</c>");
					continue;
				}
			}
			_AddFile(refName, xmlPath, isNET: true);
			//break;
		}

		statInsert.BindAll(".", string.Join("\n", haveRefs)).Step();

		trans.Commit();
		d.Execute("VACUUM");

		Print("DONE");

		void _AddFile(string refName, string xmlPath, bool isNET = false)
		{
			Print(refName);
			haveRefs.Add(refName);
			var xr = AExtXml.LoadElem(xmlPath);
			foreach(var e in xr.Descendants("member")) {
				var name = e.Attr("name");

				//remove <remarks> and <example>. Does not save much space, because .NET xmls don't have it.
				foreach(var v in e.Descendants("remarks").ToArray()) v.Remove();
				foreach(var v in e.Descendants("example").ToArray()) v.Remove();

				using var reader = e.CreateReader();
				reader.MoveToContent();
				var xml = reader.ReadInnerXml();
				//Print(name, xml);

				//remove some texts
				xml = xml.Replace("To browse the .NET Framework source code for this type, see the Reference Source.", null);

				if(uniq.TryGetValue(name, out var prevRef)) {
					if(!statDupl.Bind(1, name).Step()) throw new Exception();
					var prev = statDupl.GetText(0);
					if(xml != prev && refName != "System.Linq") Print($"<>\t{name} already defined in {prevRef}\r\n<c 0xc000>{prev}</c>\r\n<c 0xff0000>{xml}</c>");
					statDupl.Reset();
				} else {
					statInsert.BindAll(name, xml).Step();
					uniq.Add(name, refName);
				}
				statInsert.Reset();
			}
		}
	}

	//Successfully compiles and starts. Did not try to create a web browser instance. Adds 334 MB of files to the output folder.
	//void TestCefSharp()
	//{
	//	AppDomain.CurrentDomain.AssemblyResolve += _CefSharpResolver;
	//	_TestCefSharp();
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//void _TestCefSharp()
	//{
	//	var settings = new CefSettings();

	//	// Set BrowserSubProcessPath based on app bitness at runtime
	//	settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
	//										   Environment.Is64BitProcess ? "x64" : "x86",
	//										   "CefSharp.BrowserSubprocess.exe");

	//	// Make sure you set performDependencyCheck false
	//	Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

	//	var browser = new _CefSharpForm();
	//	Application.Run(browser);
	//}

	//// Will attempt to load missing assembly from either x86 or x64 subdir
	//private static Assembly _CefSharpResolver(object sender, ResolveEventArgs args)
	//{
	//	if(args.Name.StartsWith("CefSharp")) {
	//		string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
	//		string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
	//											   Environment.Is64BitProcess ? "x64" : "x86",
	//											   assemblyName);

	//		return File.Exists(archSpecificPath)
	//				   ? Assembly.LoadFile(archSpecificPath)
	//				   : null;
	//	}

	//	return null;
	//}

	//internal class _CefSharpForm :Form
	//{
	//	public _CefSharpForm()
	//	{
	//	}
	//}

	class _UniIdent
	{
		//public static void \u0066or() { }
		//public static void a\ufe4db() { }
		//public static void c\u0300d() { }
		//public static void e\u0600f() { }
		//public static void g\u0903h() { }
		public static void Az() { }
		public static void ak() { }
		public static void _ac() { }
		public static void ta() { }
		public static void @abstract() { }
		public static void AcBc() { }
		public static void _bc() { }
		//public static void @base() { }
		public static void Bc() { }
		public static void bd() { }
		public static void CokaNoka() { }
		public static void Coka_noka() { }
		public static void KO_NGO() { }
	}

	//void TestScreenDC()
	//{
	//	string s = "One Two";
	//	using var dc = new Au.Util.LibScreenDC(0);
	//	var font = Au.Util.AFonts.Regular.ToHfont();
	//	var oldFont = Api.SelectObject(dc, font);
	//	if(Api.GetTextExtentPoint32(dc, s, s.Length, out var z)) Print(z);
	//	Task.Run(() => {
	//		using var dc = new Au.Util.LibScreenDC(0);
	//		if(Api.GetTextExtentPoint32(dc, s, s.Length, out var z)) Print(z);
	//	}).Wait();
	//	Api.SelectObject(dc, oldFont);
	//	Api.DeleteObject(font);
	//}

	void TestKnownFolders()
	{
		Print(AFolders.GetKnownFolders());

		//AFolders.GetKnownFolders();
		//3.s();
		//var t1 = ATime.WinMilliseconds;
		//while(ATime.WinMilliseconds-t1<10000) AFolders.GetKnownFolders();
	}

	void TestFont()
	{
		ADebug.LibMemorySetAnchor();
		for(int i = 0; i < 10000; i++) {
			//var f = new Font("Segoe UI", 8f+(i/100f));
			var f = Au.Util.AFonts.Regular;
			var z = f.Size;
			//f.Dispose();
			if(0 == (i % 1000)) ADebug.LibMemoryPrint();
			1.ms();
		}
	}

	[STAThread] static void Main(string[] args) { new Script()._Main(args); }
	void _Main(string[] args)
	{ //}}//}}//}}//}}
#if true
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();
		//100.ms();

#if false
		//g1:
		//AWnd k = AWnd.Find("dkdkdkdk", null, null, );
		AWnd.Find("", "", default, WFFlags.);
		//this.
		//Form f = null; f.
		//Print(1);
		var s = "";
		int somegome = 1;
		s.Remove(0);
		//_Dtor d; d.
		//_UniIdent.no
		
		//AWnd.Find("", "", "", 

		var r1 = new Regex(@"j");
		//r1.Replace("", "")
		var r = new ARegex("^(.+)");
		//r.Replace() - 2 repl
		//"".RegexReplace() - 2 repl
		//RXMatch m;m.ExpandReplacement() - 1 repl

		//var f = new Au.Tools.FormAWinImage();
		//f.ShowDialog();
		string swwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww = "";
		//s.
		//this.TestA
		//s.Insert(0, "f");
		//List<int> a; a.ConvertAll
		//var d = new Dictionary<int, int>(); d.
		//AWnd w; w.
		swwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww.Find("d");

		Print(new string(' ', 5), new Dictionary<int, int>(2, null), 7, );

		//AWnd.Find(
		//	"",
		//	""

		//Microsoft.CodeAnalysis.
		//ImmutableArray<int> k; k.
		//System.Collections.Immutable.ImmutableList
		//"".Ends(  );
		int kk = 7;
#endif
		//Print(Empty(), 1)
		//var k = (1 + 2);

		//if(true)
		//	Print(1);
		//k = (1 + 2);
		//if(true) { }

		//if(true) if(true) if(true) {

		//		}
#if true
#else
#endif


#else
		AThread.Start(() => {
#if true
			OutputForm.ShowForm();
#else
			AOutput.QM2.UseQM2 = true;
			AOutput.Clear();
			ADialog.Show("triggers");
#endif
			Triggers.Stop();
		}, false);
		300.ms();

		//TestManyTriggerActionThreads();
		//TestAutotextTriggers();
		TestHotkeyTriggers();
		//TestMouseTriggers();
		//TestWindowTriggers();
		//TriggersExamples();
#endif

		//try { TestAcc(); }
		//finally {
		//	GC.Collect();
		//	GC.WaitForPendingFinalizers();
		//	Cpp.Unload();
		//}
		////finally {  }
		////ADialog.Show("dll unloaded");
	}
	//async Task<int> TestAsync() { }

	public int Pub2 {
		[return: MarshalAs(UnmanagedType.Bool)]
		get => 0;
		//[param: DefaultParameterValue(5)]
		[CompilerGenerated]
		set { }
	}

	[field: ThreadStatic]
	public int Pub3 { get; set; }
}

partial class PartClass{
	partial void Part();

	partial void Part( ) { }
}

abstract class AbstractClass{
	public abstract void Abs();
}

static class TestExt
{
	public static void SetTargetWithTimeout<T>(this WeakReference<T> t, T target, int timeoutMS) where T : class
	{
		t.SetTarget(target);
		//Task.Delay(timeoutMS).ContinueWith(_ => GC.KeepAlive(target));
		ATimer.After(timeoutMS, () => GC.KeepAlive(target));
	}
	public static void inda(this string t) { }
}
