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
		Print("abc".ToUpper_(true), "𐐨𐐩𐐪".ToUpper_(true));
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
		var w = Wnd.Find("* Notepad");
		w.Activate();
		Mouse.Move(w, 20, 20);

		Opt.Mouse.MoveSpeed = 10;
		for(int i = 0; i < 5; i++) {
			1.s();
			using(Mouse.LeftDown()) {
				Mouse.MoveRelative(30, 30);
				//var xy = Mouse.XY;
				//Mouse.Move(xy.x + 30, xy.y + 30);
			}
		}

		//Opt.Mouse.ClickSpeed = 1000;
		//Mouse.Click();

	}

	void TestBlockInputReliability()
	{
		Wnd.Find("* Notepad").Activate();
		//Wnd.Find("Quick *").Activate();

		//Thread_.Start(() => {
		//	using(Au.Util.WinHook.Keyboard(k => {
		//		if(!k.IsInjectedByAu) Print("----", k);

		//		return false;
		//	})) Time.SleepDoEvents(5000);
		//});

		100.ms();
		Key("Ctrl+A Delete Ctrl+Shift+J");
		Output.Clear();
		//10.ms();
		for(int i = 0; i < 100; i++) {
			Text("---- ");
		}
	}

	void TestGetKeyState()
	{
		1.s();
		Print(Mouse.IsPressed(MButtons.Left), Mouse.IsPressed(MButtons.Right));

		Perf.Cpu();
		for(int i1 = 0; i1 < 8; i1++) {
			int n2 = 1;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { Mouse.IsPressed(MButtons.Left); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { Mouse.IsPressed(MButtons.Left| MButtons.Right); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { Mouse.IsPressed(); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { }
			Perf.NW();
			Thread.Sleep(200);
		}

		//for(int i = 0; i < 100; i++) {
		//	50.ms();
		//	Perf.First();
		//	//_ = Api.GetKeyState((int)KKey.Shift);
		//	_ = Api.GetAsyncKeyState((int)KKey.Shift);
		//	//_ = Api.GetKeyState((int)KKey.MouseLeft);
		//	//_ = Api.GetAsyncKeyState((int)KKey.MouseLeft);
		//	Perf.NW();
		//}
	}

	void TestCapsLock()
	{
		Wnd.Find("* Notepad").Activate(); 100.ms();
		Opt.Key.TextOption = KTextOption.Keys;
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
		Wnd.Find("* Notepad").Activate();
		//Opt.Key.NoBlockInput = true;
		try {
			Key("a");
		}
		catch(Exception ex) { Print(ex); }
		Print("END");
	}

	void Test2CharKeysAndVK()
	{
		Wnd.Find("* Notepad").Activate();
		//Keyb.WaitForKey(0, "VK65", true);
		//Key("VK65 Vk0x42");
		//Print(Keyb.Misc.ParseKeysString("VK65 Vk0x42"));
		if(Keyb.Misc.ParseHotkeyString("Ctrl+VK65", out var mod, out var key)) Print(mod, key);
	}

	void TestAcc()
	{
		//var w = Wnd.Find("*paint.net*", "*.Window.*").OrThrow();
		//var a = Acc.Find(w, "BUTTON", prop: "id=commonActionsStrip").OrThrow();
		////var a = Acc.Find(w, "BUTTON", flags: AFFlags.NotInProc, prop: "id=commonActionsStrip").OrThrow();
		//Print(a);

#if true
		//string s;
		//s = @"var w = Wnd.Find(""Quick Macros - ok - [Macro329]"", ""QM_Editor"").OrThrow();";
		////s = @"var w = Wnd.Find(""Quick Macros \""- ok - [Macro329]"", ""QM_Editor"").OrThrow();";
		////s = @"var w = Wnd.Wait(5, ""Quick Macros - ok - [Macro329]"", ""QM_Editor"").OrThrow();";
		////s = @"var w = Wnd.Find(@""**r Document - WordPad\*?"", ""QM_Editor"").OrThrow();";
		////s = @"var w = Wnd.Find(@""**r Document """"- WordPad\*?"", ""QM_Editor"").OrThrow();";

		//if(s.RegexMatch_(@"^(?:var|Wnd) \w+ ?= ?Wnd\.(?:Find\(|Wait\(.+?, )(?s)(?:""((?:[^""\\]|\\.)*)""|(@(?:""[^""]*"")+))", out var k)) {

		//	Print(k[0]);
		//	Print(k[1]);
		//	Print(k[2]);
		//}

		//return;

		//Task.Run(() => { for(; ; ) { 1.s(); GC.Collect(); } });

		//var w = Wnd.Find("FileZilla", "wxWindowNR").OrThrow();
		//var a = Acc.Find(w, "STATICTEXT", "Local site:", "id=-31741").OrThrow();

		var fa = new Au.Tools.Form_Acc();
		//var fa = new Au.Tools.Form_Wnd();
		//var fa = new Au.Tools.Form_WinImage();
		fa.ShowDialog();

		//AuDialog.Show("unload dll");
#else
		//var w = Wnd.Find("FileZilla", "wxWindowNR").OrThrow();
		////var a = Acc.Find(w, "LISTITEM").OrThrow();
		////Print(a.RoleInt, a);

		////using(new Au.Util.AccHook(AccEVENT.OBJECT_FOCUS, 0, k => { var a = k.GetAcc(); Print(a.RoleInt, a); }, idThread: w.ThreadId)) {

		Triggers.Hotkey["F3"] = o => {
			var a = Acc.FromMouse(AXYFlags.PreferLink | AXYFlags.NoThrow);
			//var a = Acc.FromMouse();
			//var a = Acc.FromMouse(AXYFlags.NotInProc);
			//var a = Acc.Focused();
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
		//var r = Wnd.Find("Notepad", "#32770", contains: "a 'BUTTON' Save").OrThrow();
		//Print(r);

		//Wnd.Finder x; x.Props.DoesNotMatch

		//var f = new Wnd.Finder("Notepad", "#32770", "notepad.exe", WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//var f = new Wnd.Finder("Notepad", "#32770", WF3.Owner(Wnd.Find("Quick*")), WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//var f = new Wnd.Finder("Notepad", "#32770", WF3.Thread(5), WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//var p = f.Props;
		//Print(p.name, p.cn, p.program, p.processId, p.threadId, p.owner.Handle, p.flags, p.also, p.contains);

		//var w1 = Wnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		//var f = new Wnd.Finder("Notepad", "#32770", "notepad.exe", WFFlags.CloakedToo, also: o => true, contains: "a 'BUTTON' Save");
		//if(f.IsMatch(w1)) Print("MATCH"); else Print(f.Props.DoesNotMatch);

		//Print(f);

		//var w1 = Wnd.Find("*- Notepad").OrThrow();
		//var c1 = w1.Child(null, "Edit").OrThrow();
		//Print(c1.IsEnabled(false));
		//Print(c1.IsEnabled(true));
		//c1.Focus();

		//var w1 = Wnd.Find("Notepad", "#32770", contains: new object());
		//string image1 = @"image:iVBORw0KGgoAAAANSUhEUgAAAJEAAAATCAYAAACQoO/wAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAALLSURBVGhD7VbBbeswDO1O/9ZRMkDWyAq59pZ77znlkgW6QIACXaArqCJFKSRFSbSDGj+FHmAgdijy8fFJ9kuYmHgQ00QTD2OaaOJhFBPdTu/h5d9bdb2eviliYsKGNNHuI9zoPuEz7NFMl3CmJxNj2FpKeGKeBQMTAb7DcReNdPik+4kRponoXuB6iafRezh+0X0ExvNXX8dk54P9v67Zz5nMvL/SLQHXNGpbdfGZ6lPEfX2EV85BncIQC694wbXkow3H11ea9mOW6IrQfHm9Ti+tmWh9JB/pgQyfiYhMHiAWEuIOTis0oX4lplelP+dyE9V18+uZiyF53E4XIZQWO/Hk34qUk8V0tSRYMYt1VdwB5xM3QKcX70w4R3ON10Q8uTJUAT63nZrFEB/pQCjXc+VcYSItMtU8RnEKF6xRC1OghNOmAmjtVploja69/yyIXjwz0bntGSw4iSihEvWOeldw6PwwjNKAK+caE8mhQyzWZGJZ6/FZrHu/pImE8ADFX/dqoYpZqSv2Bxwb9Xq9aA71TPi6+6X7X/BNRMVXNpv+z87mvyNcOdeZCHNjX7CeauKmgHo6Z6rXG+7/ZqKMYqaixbgXcTgUTQhNPjUcJkpkinBYzGiKE2qgDAAI8sG7cqaB6wGieD0TIf+Y4xpzlf6yeei/jplRF/bs10z0gK4FPNbRCyD3g/95ZmKgbyJKpIeUXM/JpKH0hxkBje0uYW+cKJ6clQgo1KgumS+eQnz4kAuedQeZ+2c1PSayBljBiFmsa+S351wwJ5nI0QsC1uBMaqOafA715pAmwiL8au8AHV8Ja4JEaezScU5an2OiuLhmYN6UV/VCouoaggPwVMN2mYjzbPTailmkazFGvmSPo14S+jNJRrpfFp9iom2QCHeFmXg6bGsi3Dl6J0w8OzY00TyF/io2MVF5Nw++XSaeExt/E038PYTwA9lvzmPkr4YtAAAAAElFTkSuQmCC";
		//var w1 = Wnd.Find("Notepad", "#32770", contains: image1);
		//var w1 = Wnd.Find("Notepad", "#32770", contains: Color.Black);
		//var w1 = Wnd.Find("Notepad", "#32770", also: o => { o.Close(); o.WaitForClosed(5); return true; }, contains: "Save");
		//var w1 = Wnd.Find("Notepad", "#32770", also: o => { o.Close(); o.WaitForClosed(5); var uu = o.Child("kkkk"); return true; });
		//Print(w1);

		//return;

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

		////var w = Wnd.Find("Notepad", "#32770", flags: WFFlags.HiddenToo).OrThrow();
		//////var a = Acc.Find(w, "BUTTON", "Save", "class=Button").OrThrow();
		//////var a = Acc.Find(w, "BUTTON", "Save", "class=Button", AFFlags.HiddenToo).OrThrow();
		////var a = Acc.Find(w, "BUTTON", "Save", "class=Button", AFFlags.UIA).OrThrow();
		////Print(a.Name);
	}

	void TestToolForms()
	{
		//var fa = new Au.Tools.Form_Acc();
		//var fa = new Au.Tools.Form_Wnd();
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
		//if you want to set options for all or some triggers, do it before adding these triggers
		Triggers.Options.RunActionInThread(0, 500);

		//you can use variables if don't want to type "Triggers.Hotkey" etc for each trigger
		var hk = Triggers.Hotkey;
		var mouse = Triggers.Mouse;
		var window = Triggers.Window;
		var tt = Triggers.Autotext;

		//hotkey triggers

		hk["Ctrl+K"] = o => Print(o.Trigger);
		hk["Ctrl+Shift+F11"] = o => {
			Print(o.Trigger);
			var w1 = Wnd.FindOrRun("* Notepad", run: () => Shell.Run(Folders.System + "notepad.exe"));
			Text("text");
			w1.Close();
		};

		//triggers that work only with some windows

		Triggers.Of.Window("* WordPad", "WordPadClass"); //let the following triggers work only when a WordPad window is active
		hk["Ctrl+F5"] = o => Print(o.Trigger, o.Window);
		hk["Ctrl+F6"] = o => Print(o.Trigger, o.Window);

		var notepad = Triggers.Of.Window("* Notepad"); //let the following triggers work only when a Notepad window is active
		hk["Ctrl+F5"] = o => Print(o.Trigger, o.Window);
		hk["Ctrl+F6"] = o => Print(o.Trigger, o.Window);

		Triggers.Of.AllWindows(); //let the following triggers work with all windows

		//mouse triggers

		mouse[TMClick.Right, "Ctrl+Shift", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		mouse[TMEdge.RightInCenter50] = o => { Print(o.Trigger); AuDialog.ShowEx("Bang!", x: Coord.Max); };
		mouse[TMMove.LeftRightInCenter50] = o => Wnd.SwitchActiveWindow();

		Triggers.FuncOf.NextTrigger = o => Keyb.IsScrollLock; //example of a custom scope (aka context, condition)
		mouse[TMWheel.Forward] = o => Print($"{o.Trigger} while ScrollLock is on");

		Triggers.Of.Again(notepad); //let the following triggers work only when a Notepad window is active
		mouse[TMMove.LeftRightInBottom25] = o => { Print(o.Trigger); o.Window.Close(); };
		Triggers.Of.AllWindows();

		//window triggers. Note: window triggers don't depend on Triggers.Of window.

		window.ActiveNew["* Notepad", "Notepad"] = o => Print("opened Notepad window");
		window.ActiveNew["Notepad", "#32770", contains: "Do you want to save *"] = o => {
			Print("opened Notepad's 'Do you want to save' dialog");
			Key("Alt+S");
		};

		//autotext triggers

		tt["los"] = o => o.Replace("Los Angeles");
		tt["WIndows", TAFlags.MatchCase] = o => o.Replace("Windows");
		tt.DefaultPostfixType = TAPostfix.None;
		tt["<b>"] = o => o.Replace("<b>[[|]]</b>");
		Triggers.Options.BeforeAction = o => { Opt.Key.TextOption = KTextOption.Paste; };
		tt["#file"] = o => {
			o.Replace("");
			var fd = new OpenFileDialog();
			if(fd.ShowDialog() == DialogResult.OK) Text(fd.FileName);
		};
		Triggers.Options.BeforeAction = null;
		tt.DefaultPostfixType = default;

		var ts = Triggers.Autotext.Simple;
		ts["#su"] = "Sunday"; //the same as tt["#su"] = o => o.Replace("Sunday");
		ts["#mo"] = "Monday";

		//how to stop and disable/enable triggers

		hk["Ctrl+Alt+Q"] = o => Triggers.Stop(); //let Triggers.Run() end its work and return
		hk.Last.EnabledAlways = true;

		hk["Ctrl+Alt+D"] = o => Triggers.Disabled ^= true; //disable/enable triggers here
		hk.Last.EnabledAlways = true;

		hk["Ctrl+Alt+Win+D"] = o => ActionTriggers.DisabledEverywhere ^= true; //disable/enable triggers in all processes
		hk.Last.EnabledAlways = true;

		hk["Ctrl+F7"] = o => Print("This trigger can be disabled/enabled with Ctrl+F8.");
		var t1 = hk.Last;
		hk["Ctrl+F8"] = o => t1.Disabled ^= true; //disable/enable a trigger

		//finally call Triggers.Run(). Without it the triggers won't work.
		Triggers.Run();
		//Triggers.Run returns when is called Triggers.Stop (see the "Ctrl+Alt+Q" trigger above).
		Print("called Triggers.Stop");
		//Recommended properties for scripts containg triggers: 'runMode'='blue' and 'ifRunning'='restart'. You can set it in the Properties dialog.
		//The first property allows other scripts to start while this script is running.
		//The second property makes easy to restart the script after editing: just click the Run button.
	}

	void TestWindowTriggers()
	{
		//Triggers.Window.ActiveOnce["Notepad", "#32770"] = null;
		//var t1 = Triggers.Window.Last;
		//Print(t1.TypeString);
		//Print(t1.ParamsString);
		//Print(t1);

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
		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "c 'Button' Save"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "c 'Button' Save", also: also] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "a 'STATICTEXT' * save *"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.VisibleOnce["Notepad", "#32770", contains: "c 'Button' Save"] = o => Print("TRIGGER", o.Window);
		//Triggers.Window.VisibleOnce["Notepad", "#32770", contains: "a 'STATICTEXT' * save *"] = o => Print("TRIGGER", o.Window);

		//Triggers.Window.VisibleOnce["Notepad", "#32770"] = o => { Print("TRIGGER", o.Window); o.Window.Close(); };
		//Triggers.Window.ActiveOnce["Notepad", "#32770"] = o => { Print("TRIGGER", o.Window); o.Window.Close(); };

		//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "c 'Button' Save"] = o => {
		//Triggers.Window.ActiveOnce["Save As", "#32770", "notepad.exe", contains: "c 'Button' Save"] = o => {
		//Triggers.Window.ActiveOnce["Save As", "#32770", "notepad.exe", contains: "a 'BUTTON' Save"] = o => {
		//Triggers.Window.ActiveOnce["Notepad", "#32770"] = o => {
		Triggers.Window.ActiveOnce["Notepad", "#32770", contains: "Do you want to save*"] = o => {
			//string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAJEAAAATCAYAAACQoO/wAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAALLSURBVGhD7VbBbeswDO1O/9ZRMkDWyAq59pZ77znlkgW6QIACXaArqCJFKSRFSbSDGj+FHmAgdijy8fFJ9kuYmHgQ00QTD2OaaOJhFBPdTu/h5d9bdb2eviliYsKGNNHuI9zoPuEz7NFMl3CmJxNj2FpKeGKeBQMTAb7DcReNdPik+4kRponoXuB6iafRezh+0X0ExvNXX8dk54P9v67Zz5nMvL/SLQHXNGpbdfGZ6lPEfX2EV85BncIQC694wbXkow3H11ea9mOW6IrQfHm9Ti+tmWh9JB/pgQyfiYhMHiAWEuIOTis0oX4lplelP+dyE9V18+uZiyF53E4XIZQWO/Hk34qUk8V0tSRYMYt1VdwB5xM3QKcX70w4R3ON10Q8uTJUAT63nZrFEB/pQCjXc+VcYSItMtU8RnEKF6xRC1OghNOmAmjtVploja69/yyIXjwz0bntGSw4iSihEvWOeldw6PwwjNKAK+caE8mhQyzWZGJZ6/FZrHu/pImE8ADFX/dqoYpZqSv2Bxwb9Xq9aA71TPi6+6X7X/BNRMVXNpv+z87mvyNcOdeZCHNjX7CeauKmgHo6Z6rXG+7/ZqKMYqaixbgXcTgUTQhNPjUcJkpkinBYzGiKE2qgDAAI8sG7cqaB6wGieD0TIf+Y4xpzlf6yeei/jplRF/bs10z0gK4FPNbRCyD3g/95ZmKgbyJKpIeUXM/JpKH0hxkBje0uYW+cKJ6clQgo1KgumS+eQnz4kAuedQeZ+2c1PSayBljBiFmsa+S351wwJ5nI0QsC1uBMaqOafA715pAmwiL8au8AHV8Ja4JEaezScU5an2OiuLhmYN6UV/VCouoaggPwVMN2mYjzbPTailmkazFGvmSPo14S+jNJRrpfFp9iom2QCHeFmXg6bGsi3Dl6J0w8OzY00TyF/io2MVF5Nw++XSaeExt/E038PYTwA9lvzmPkr4YtAAAAAElFTkSuQmCC";
			//Triggers.Window.ActiveOnce["Notepad", "#32770", contains: image] = o => {
			//Triggers.Window.ActiveOnce["Notepad", "#32770", also: o => WinImage.Wait(-1, o, image, WIFlags.WindowDC) != null] = o => {
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

		Triggers.Run();
		//Print("stopped");

		//Triggers.Options.RunActionInNewThread(true);
		//Triggers.Window.ActiveNew["* Notepad"] = o => o.Window.Resize(500, 200);
		//Triggers.Hotkey["Ctrl+T"] = o => Triggers.Window.SimulateActiveNew();
		//Triggers.Hotkey["Ctrl+Alt+T"] = o => Triggers.Stop();
		//Triggers.Run();
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
		tm[TMClick.Right, null, TMFlags.ButtonModUp] = o => Print(o.Trigger);
		tm[TMClick.Right, "Alt"] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Alt", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Alt", TMFlags.ShareEvent] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Alt", TMFlags.ButtonModUp | TMFlags.ShareEvent] = o => Print(o.Trigger);
		Triggers.Options.BeforeAction = o => { Opt.Key.KeySpeed = 50; /*Opt.Key.NoBlockInput = true;*/ };
		//tm[TMClick.Right, "Alt"] = o => Key("some Spa text Spa for Spa testing Spa Enter");
		//tm[TMClick.Right, "Alt"] = o => Print("wait", Keyb.WaitForKey(0, true, true));
		//tm[TMClick.Right, "Win"] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Shift"] = o => Print(o.Trigger);
		tm[TMClick.Right, "Win", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		tm[TMClick.Right, "Win+Shift"] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Shift", TMFlags.ButtonModUp] = o => Print(o.Trigger);
		//tm[TMClick.Right, "Shift", TMFlags.ButtonModUp | TMFlags.ShareEvent] = o => Print(o.Trigger);
		//tm[TMClick.Left, "Shift"] = o => {
		//	//2.s();
		//	Opt.Mouse.MoveSpeed = 50;
		//	using(Mouse.LeftDown()) {
		//		//Mouse.MoveRelative(30, 30);
		//		var xy = Mouse.XY;
		//		Mouse.Move(xy.x+30, xy.y+30);
		//	}
		//	Print("done");
		//};


		//Triggers.Hotkey["Ctrl+K"] = o => Print(o.Trigger);
		////Triggers.Autotext["mmpp"] = o => Print(o.Trigger);
		//Triggers.Mouse[TMClick.Right] = o => Print(o.Trigger);
		//Triggers.Mouse[TMWheel.Right] = o => Print(o.Trigger);
		////Triggers.Mouse[TMEdge.Right] = o => Print(o.Trigger);
		//Triggers.Mouse[TMMove.DownUp] = o => Print(o.Trigger);

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

		//tk["Shift+K"] = o => { Print(Keyb.IsShift); Mouse.Click(); };
		//tk["Shift+K", TKFlags.NoModOff] = o => { Print(Keyb.IsShift); Mouse.Click(); };
		//tk["Shift+K", TKFlags.NoModOff] = o => { Print(Keyb.IsShift); Key("keys", "some Text"); };
		//tk["Shift+K", TKFlags.NoModOff] = o => { Print(Keyb.IsShift); Opt.Key.NoModOff = true; Key("keys", "some Text"); };
		//tk["Shift+K"] = o => { Print(Keyb.IsShift); Opt.Key.NoModOff = true; Key("keys", "some Text"); };
		tk["Ctrl+K"] = o => { Print(o.Trigger); };
		//tk["Ctrl+K", TKFlags.NoModOff] = o => { Print(o.Trigger); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, Keyb.IsCtrl); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, Keyb.IsCtrl); Text(" ...."); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, Keyb.IsCtrl); Text(" ..", 30, "", ".."); };
		//tk["Ctrl+K"] = o => { Opt.Key.NoBlockInput = true; Print(o.Trigger, Keyb.IsCtrl); Text(" ...."); };
		//tk["Ctrl+K"] = o => { Print(o.Trigger, Keyb.IsCtrl); 1.s(); Print(Keyb.IsCtrl); };
		tk["Ctrl+L", TKFlags.LeftMod] = o => { Print(o.Trigger); };
		tk["Ctrl+R", TKFlags.RightMod] = o => { Print(o.Trigger); };
		//tk["Alt+F"] = o => { Print(o.Trigger); };
		//tk["Win+R"] = o => { Print(o.Trigger); };
		//tk["Alt+F", TKFlags.NoModOff] = o => { Print(o.Trigger, Keyb.IsAlt); };
		//tk["Win+R", TKFlags.NoModOff] = o => { Print(o.Trigger, Keyb.IsWin); };
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

		//Triggers.Hotkey["F11"] = o => { Print(o.Trigger); Mouse.Click(); };
		//Triggers.Hotkey["F11"] = o => { Print(o.Trigger); Mouse.Click(); };
		//Triggers.Hotkey["F11"] = o => { Text("text"); };
		//Triggers.Hotkey["F11"] = o => { Key("text"); };
		Triggers.Hotkey["F11"] = o => { Paste("text"); };
		//Triggers.Hotkey["F11"] = o => { Opt.Key.TextSpeed = 500; Text("kaa kaa "); };
		//Triggers.Hotkey["F11"] = o => {
		//	2.s();
		//	Wnd.Find("* Notepad").Activate();
		//};
		//Triggers.Hotkey["u", TKFlags.ShareEvent] = o => { Print(o.Trigger); };

		//Triggers.FuncOf.NextTrigger = o => { var ta = o as AutotextTriggerArgs; ta.Replace("dramblys"); return false; };
		//tt["kut"] = null;

		tt["mee"] = o => {
			//100.ms();
			var m = new AuMenu();
			m["one"] = u => Text("One");
			m["two"] = u => Text("Two");
			m.Show(true);
		};

		//tt["mii", TAFlags.Confirm] = o => { o.Replace("dramblys"); };
		tt["mii", TAFlags.Confirm] = o => { o.Replace(@"1>------ Build started: Project: Au, Configuration: Debug Any CPU ------
1>  Au -> Q:\app\Au\Au\bin\Debug\Au.dll
2>------ Build started: Project: TreeList, Configuration: Debug Any CPU ------
3>------ Build started: Project: Au.Compiler, Configuration: Debug Any CPU ------
"); };
		tt["con1", TAFlags.Confirm] = o => o.Replace("Flag Confirm");
		tt["con2"] = o => { if(o.Confirm("Example")) o.Replace("Function Confirm"); };

		var ts = Triggers.Autotext.Simple;
		ts["#su"] = "Sunday"; //the same as Triggers.Autotext["#su"] = o => o.Replace("Sunday");
		ts["#mo"] = "Monday";

		Triggers.Run();
	}

	[STAThread] static void Main(string[] args) { new Script()._Main(args); }
	void _Main(string[] args)
	{ //}}//}}//}}//}}
#if false
		Output.QM2.UseQM2 = true;
		Output.Clear();
		//100.ms();

		//TestIndexerOverload();
		//TestCharUpperNonBmp();
		//TestBlockInputReliability();
		//TestGetKeyState();
		//TestCapsLock();
		//Test2CharKeysAndVK();
		//TestTurnOffCapsLockWithShift();
		//TestMouseRelative();
#else
		Thread_.Start(() => {
			OutputForm.ShowForm();
			//AuDialog.Show("triggers");
			Triggers.Stop();
		}, false);
		300.ms();

		TestAutotextTriggers();
		//TestHotkeyTriggers();
		//TestMouseTriggers();
		//TriggersExamples();
#endif
		return;

		//try { TestAcc(); }
		//finally {
		//	GC.Collect();
		//	GC.WaitForPendingFinalizers();
		//	Cpp.Unload();
		//}
		////finally {  }
		////AuDialog.Show("dll unloaded");
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
