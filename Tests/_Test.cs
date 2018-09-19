using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
//using Microsoft.Win32;
//using Registry = Microsoft.Win32.Registry;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

using Au;
using static Au.NoClass;

//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;

using Microsoft.VisualBasic.FileIO;
using System.Globalization;

//for LikeEx_
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using VB = Microsoft.VisualBasic.FileIO;

//using ImapX;
//using System.Data.SQLite;
using SQLite;

//using CsvHelper;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

using Microsoft.Win32.SafeHandles;

using System.IO.Compression;
using System.Reflection.Emit;
using System.Net;
using System.Net.NetworkInformation;

using System.Configuration;

using Au.Types;
using Au.Util;
using Au.Controls;

using System.Dynamic;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 169, 219, 649 //unreachable code, unused var/field

[System.Security.SuppressUnmanagedCodeSecurity]
static partial class Test
{
	//Why .NET creates so many threads?
	//Even simplest app has 6 threads.
	//9 threads if there is 'static Au SOMESTRUCT _var;' and [STAThread]. Why???
	//	Not if it's a class (even if '= new Class()'). Not if it's a non-library struct. Not if it's a .NET struct.
	//	Not if app is ngened (but lib can be non-ngened).
	//		But why only if STAThread? And why only if it's a struct (and not class) of a User.dll (and not eg of this assembly)?
	//		Tested with a simplest dll, not only with Au.dll.
	//	Also can depend on other things, eg handling some exception types, using Output.Clear etc. Only if [STAThread].
	//	With or without [STAThread], 1 call to Task.Run makes 12 (from 6 or 9), >=2 Task.Run makes 14.
	//The above numbers (6 and 9) are on Win10. On Win7 (virtual PC) the numbers are 4 and 7. Older .NET framework version.

	//static Point s_p;
	//static SimpleLib.Struct1 s_p;

	[STAThread]
	static void Main(string[] args)
	{
		//Application.EnableVisualStyles();
		//Application.SetCompatibleTextRenderingDefault(false);

		TestMain();
	}



	static unsafe void TestWinHook()
	{
		////using Au.Util;
		//var stop = false;
		//using(WinHook.Keyboard(x =>
		//{
		//	Print(x);
		//	if(x.vkCode == KKey.Escape) { stop = true; return true; } //return true to cancel the event
		//	return false;
		//})) {
		//	MessageBox.Show("Low-level keyboard hook.", "Test");
		//	//or
		//	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for Esc key
		//	//Print("the end");
		//}

		////using Au.Util;
		//var stop = false;
		//using(WinHook.Mouse(x =>
		//{
		//	Print(x);
		//	if(x.Event == HookData.MouseEvent.RightButton) { stop = x.IsButtonUp; return true; } //return true to cancel the event
		//	return false;
		//})) {
		//	MessageBox.Show("Low-level mouse hook.", "Test");
		//	//or
		//	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for right-click
		//	//Print("the end");
		//}

		////using Au.Util;
		//using(WinHook.ThreadCbt(x =>
		//{
		//	Print(x.code);
		//	switch(x.code) {
		//	case HookData.CbtEvent.ACTIVATE:
		//		Print(x.ActivationInfo(out _, out _));
		//		break;
		//	case HookData.CbtEvent.CREATEWND:
		//		Print(x.CreationInfo(out var c, out _), c->x, c->lpszName);
		//		break;
		//	case HookData.CbtEvent.CLICKSKIPPED:
		//		Print(x.MouseInfo(out var m), m->pt, m->hwnd);
		//		break;
		//	case HookData.CbtEvent.KEYSKIPPED:
		//		Print(x.KeyInfo(out _));
		//		break;
		//	case HookData.CbtEvent.SETFOCUS:
		//		Print(x.FocusInfo(out Wnd wPrev), wPrev);
		//		break;
		//	case HookData.CbtEvent.MOVESIZE:
		//		Print(x.MoveSizeInfo(out var r), r->ToString());
		//		break;
		//	case HookData.CbtEvent.MINMAX:
		//		Print(x.MinMaxInfo(out var state), state);
		//		break;
		//	case HookData.CbtEvent.DESTROYWND:
		//		Print((Wnd)x.wParam);
		//		break;
		//	}
		//	return false;
		//})) {
		//	MessageBox.Show("CBT hook.", "Test", MessageBoxButtons.OKCancel);
		//	//new Form().ShowDialog(); //to test MINMAX
		//}

		//Timer_.After(1000, t => { Wnd.Misc.PostThreadMessage(Api.WM_APP); Api.PeekMessage(out var mk, default, 0, 0, Api.PM_NOREMOVE); Api.PeekMessage(out var m, default, 0, 0, Api.PM_REMOVE); });

		////using Au.Util;
		//using(WinHook.ThreadGetMessage(x =>
		//{
		//	Print(x.msg->ToString(), x.PM_NOREMOVE);
		//})) MessageBox.Show("hook");

		////using Au.Util;
		//using(WinHook.ThreadKeyboard(x =>
		//{
		//	Print(x.key, 0 != (x.lParam & 0x80000000) ? "up" : "", x.lParam, x.PM_NOREMOVE);
		//	return false;
		//})) MessageBox.Show("hook");

		////using Au.Util;
		//using(WinHook.ThreadMouse(x =>
		//{
		//	Print(x.message, x.m->pt, x.m->hwnd, x.PM_NOREMOVE);
		//	return false;
		//})) MessageBox.Show("hook");

		//Task.Run(() => { 1.s(); Wnd.Find(className: "#32770").Send(Api.WM_APP + 87); });

		////using Au.Util;
		//using(WinHook.ThreadCallWndProc(x =>
		//{
		//	ref var m = ref *x.msg;
		//	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam);
		//	Print(mm, x.sentByOtherThread);
		//})) MessageBox.Show("hook");

		////using Au.Util;
		//using(WinHook.ThreadCallWndProcRet(x =>
		//{
		//	ref var m = ref *x.msg;
		//	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam); mm.Result = m.lResult;
		//	Print(mm, x.sentByOtherThread);
		//})) MessageBox.Show("hook");

		//Print(WaitForKey2(-5, KKey.Left));

		//using(var x = new OnScreenRect()) {
		//	x.Rect = new RECT(100, 100, 200, 200, true);
		//	x.Color = Color.SlateBlue;
		//	x.Thickness = 4;
		//	x.Show(true);
		//	for(int i = 0; i < 6; i++) {
		//		300.ms();
		//		x.Visible = !x.Visible;
		//	}
		//}

		//var h = WinHook.ThreadCbt(x =>
		// {

		//	 return false;
		// });
		//MessageBox.Show("hook");

		//AuDialog.ShowEx("test", secondsTimeout: 5);

		//using(var b = new BlockUserInput(BIEvents.MouseClicks)) {
		//	//AuDialog.Show(buttons: "OK|Cancel");
		//	AuDialog.ShowTextInput(out var s, editType: DEdit.Multiline);
		//}
	}

	static void _TestGetAccFromHook()
	{
		try {
			//Api.ReplyMessage(0);
			Print(Acc.FromMouse());
		}
		catch(Exception e) { Print(e.Message); }
	}

	static void TestAccHook()
	{
		//using Au.Util;
		bool stop = false;
		using(new AccHook(AccEVENT.SYSTEM_FOREGROUND, 0, x =>
		{
			Print(x.wnd);
			var a = x.GetAcc();
			Print(a);
			if(x.wnd.ClassNameIs("Shell_TrayWnd")) stop = true;
		})) {
			MessageBox.Show("hook");
			//or
			//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for activated taskbar
			//Print("the end");
		}

		//using(var b = new BlockUserInput(BIEvents.Keys)) {
		//	//AuDialog.Show(buttons: "OK|Cancel");
		//	AuDialog.ShowTextInput(out var s, editType: DEdit.Multiline);
		//}
	}

	class _TestHookFinalizer :IDisposable
	{
		public int x = 8;
		public override string ToString()
		{
			return "x=" + x;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		~_TestHookFinalizer() => Print("~_TestHookFinalizer");


	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestAuLoadingFormsAssembly()
	{
		_ = typeof(Stopwatch).Assembly; //System, +17 ms
		_ = typeof(System.Linq.Enumerable).Assembly; //System.Core, +18 ms
		Print("NEW");

		//Perf.Cpu();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Perf.First();
		//	//Thread_.LibIsLoadedFormsWpf();
		//	"fffff".StartsWith_("ff", true);
		//	//var s = Convert_.HexEncode(new byte[] { 1, 2 });
		//	//Perf.First();
		//	//var b = Convert_.HexDecode(s);
		//	//Print(b);

		//	Perf.NW();
		//}


		//return;

		AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		//if(Keyb.IsCtrl) Print("ctrl");
		//AuDialog.ShowEx(secondsTimeout: 1);

		//Print(Thread_.LibIsLoadedFormsWpf());

		//var t = typeof(Application);
		//bool u = Thread_.IsUI;
		//Print(u);

		//var f = new Form();
		//f.Click += (unu, sed) => Print(Thread_.IsUI);
		////Application.Run(f);
		//f.ShowDialog(); f.Dispose();

		//Print(Thread_.LibIsLoadedFormsWpf());

		//var m = new Au.Util.MessageLoop();
		//Timer_.After(2000, () => m.Stop());
		////Timer_.After(2000, () => Api.PostQuitMessage(0));
		////Timer_.After(2000, () => Wnd.Misc.PostThreadMessage(Api.WM_QUIT));
		//m.Loop();

		//var m = new AuMenu();
		//m["one"] = o => Print(o);
		//m.Show();

		//Osd.ShowText("TEST", showMode: OsdShowMode.Wait);
		//var m = new Osd();

		//Perf.First();
		//var k = new Keyb(null);
		//Perf.Next();
		//for(int i = 0; i < 5; i++) {
		//	k.AddKeys("Left");
		//	//k.AddKeys("VolumeUp");
		//	Perf.Next();
		//}
		//Perf.NW();

		Print("FINALLY");
		foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v);
	}

	private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
	{
		var a = args.LoadedAssembly;
		Print(a);
		if(a.FullName.StartsWith_("System.Windows.Forms")) {
			//Print(1);
		}
	}

	static unsafe void _TestExceptionInInteropCallback()
	{
		using(Au.Util.WinHook.ThreadGetMessage(x =>
		{
			Print(x.msg->ToString(), x.PM_NOREMOVE);
			//throw new AuException("TEST");
		})) {
			Timer_.Every(1000, () =>
		{
			Print(1);
			//throw new AuException("TEST");
			//Thread.CurrentThread.Abort();
		});
			MessageBox.Show("");
			//AuDialog.Show();
			//AuDialog.ShowEx(secondsTimeout: 10);
			Print("thread OK");
		}

		//EnumWindows((w, param) =>
		//{
		//	//Thread.Sleep(100);
		//	//Thread.CurrentThread.Abort();
		//	throw new AuException("TEST");
		//	Print(w);
		//	return true;
		//}, 0);
	}
	[DllImport("user32.dll")]
	internal static extern bool EnumWindows(Api.WNDENUMPROC lpEnumFunc, LPARAM lParam);

	static unsafe void TestExceptionInInteropCallback()
	{
		AppDomain.CurrentDomain.UnhandledException += (_, __) => { Print("UE", __.ExceptionObject); };

		var t = new Thread(_TestExceptionInInteropCallback);
		t.SetApartmentState(ApartmentState.STA);
		t.Start();
		1500.ms();
		t.Abort();
		t.Join();
		Print("main OK");
	}

	static void TestFileOpenWaitLocked()
	{
		var file = Folders.Temp + "test.txt";

		Task.Run(() =>
		{
			try {
				var t = Time.Milliseconds;
				while(Time.Milliseconds - t < 1200) {
					//using(var f = File.Create(file)) {
					using(var f = File_.OpenWithFunc(() => File.Create(file))) {
						f.WriteByte(1);
						50.ms();
					}
					//File.WriteAllText(file, "TEXT"); //unsafe. Exception if the file is locked.
					//File_.OpenWithAction(() => File.WriteAllText(file, "TEXT")); //safe. Waits while the file is locked.
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); Print((uint)e.HResult); }
		});

		Task.Run(() =>
		{
			10.ms();
			try {
				var t = Time.Milliseconds;
				while(Time.Milliseconds - t < 1200) {
					//using(var f = File.OpenRead(file)) {
					using(var f = File_.OpenWithFunc(() => File.OpenRead(file))) {
						f.ReadByte();
					}
					//var s1 = File.ReadAllText(file); //unsafe. Exception if the file is locked.
					//var s2 = File_.OpenWithFunc(() => File.ReadAllText(file)); //safe. Waits while the file is locked.
					//using(var f = File_.OpenWithFunc(() => File.OpenText(file))) { //safe. Waits while the file is locked.
					//	var s3 = f.ReadToEnd();
					//}
					//using(var f = File_.OpenWithFunc(() => File.Create(file))) { //safe. Waits while the file is locked.
					//	f.WriteByte(1);
					//}
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); Print((uint)e.HResult); }
		}).Wait();

		Print("OK");
	}

	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		//MessageBox.Show(""); return;
		//OutputFormExample.Main(); return;
		//Output.IgnoreConsole = true;
#if DEBUG
		//Output.IgnoreConsole = true;
		//Output.LogFile=@"Q:\Test\Au"+IntPtr.Size*8+".log";
#endif
		Output.LibUseQM2 = true;
		Output.RedirectConsoleOutput = true;
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			100.ms();
		}

		try {
#if true

			TestFileOpenWaitLocked();
			//TestAuLoadingFormsAssembly();
			//TestFoldersSetOnce();
			//TestExceptionInInteropCallback();
			//TestEnumHas2();
			//TestGCHandle();
			//TestStartProcessFromShell();
			//TestConfigSettings();
			//TestResources();
			//TestArrayExtensions();
			//TestCompiler2();
			//TestWebBrowserLeaks();
			//Cpu();
			//var t1 = Time.Microseconds;
			//TestPerfWithoutSM(t1);
			//TestKeySpeedWithEnum();
			//TestFileWriteLineSpeed();
			//TestTaskExceptions();
			//TestAutotext();
			//TestCompiler();
			//TestMenuAutoIcons();
			//TestToolWnd();
			//TestSciSetText();
			//TestWndGet();
			//TestToolWinImage();
			//TestWFEtc();
			//TestWndFindContainsRoleName();
			//TestAccFindParamNavig();
			//TestThrowAndWait();
			//TestToolWinImageCode();
			//TestWinImageCapture();
			//Au.Tools.Test.OsdRect();
			//TestOsd();
			//TestFormClose();
#else
			try {

				//var w8 = Wnd.Find("*Firefox*", "MozillaWindowClass").OrThrow();
				////Print(w8);
				//var a = Acc.FindAll(w8, "web:TEXT", "??*");
				////var a = Acc.FindAll(w8, "web:");
				////var a = Acc.FindAll(w8, "web:TEXT", "??*", flags: AFFlags.NotInProc);
				////var a = Acc.Wait(3, w8, "web:TEXT", "Search the Web", flags: AFFlags.NotInProc);
				////var a = Acc.Wait(3, w8, "TEXT", "Search the Web", flags: AFFlags.UIA);
				//Print(a);
				//Print("---");
				//Print(a[0].MiscFlags);
				//return;

				//var w = Wnd.Find("Java Control Panel", "SunAwtFrame").OrThrow();
				////var a = Acc.Find(w, "push button", "Settings...").OrThrow();
				//var a = Acc.Find(w, "push button", "Settings...", flags: AFFlags.ClientArea).OrThrow();
				//Print(a);

				TestAccForm();
				//TestAccLeaks();
				//TestWndFindContains();
				//TestAccFindWithChildFinder();
				//TestIpcWithWmCopydataAndAnonymousPipe();
				//TestAccProcessDoesNotExit3();
				//TestAccFirefoxNoSuchInterface();
				//TestAccThrowOperator();
			}
			finally {
				Cpp.Cpp_Unload();
			}
#endif
		}
		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }


		/*

		using static AuAlias;

		say(...); //Output.Write(...); //or print
		key(...); //Keyb.Key(...);
		tkey(...); //Keyb.Text(...); //or txt
		paste(...); //Keyb.Paste(...);
		msgbox(...); //AuDialog.Show(...);
		wait(...); //Time.Wait(...);
		click(...); //Mouse.Click(...);
		mmove(...); //Mouse.Move(...);
		run(...); //Shell.Run(...);
		act(...); //Wnd.Activate(...);
		win(...); //Wnd.Find(...);
		speed=...; //Script.Speed=...;

		using(Script.TempOptions(speed

		*/

		//l.Perf.First();
		//l.AuDialog.Show("f");
		//l.Util.LibDebug_.PrintLoadedAssemblies();
		//Print(l.DIcon.Info);

	}
}
