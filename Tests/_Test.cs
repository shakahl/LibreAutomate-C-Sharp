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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

using Catkeys;
using static Catkeys.NoClass;


//using System.IO.MemoryMappedFiles;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

//using Catkeys.Triggers;


//using Cat = Catkeys.Input;
//using Meow = Catkeys.Show;

//using static Catkeys.Show;

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

//using static Test.CatAlias;

[module: DefaultCharSet(CharSet.Unicode)]
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field


static partial class Test
{
	//Why .NET creates so many threads?
	//Even simplest app has 6 threads.
	//9 threads if there is 'static Catkeys SOMESTRUCT _var;' and [STAThread]. Why???
	//	Not if it's a class (even if '= new Class()'). Not if it's a non-library struct. Not if it's a .NET struct.
	//	Not if app is is ngened (but lib can be non-ngened).
	//		But why only if STAThread? And why only if it's a struct (and not class) of a User.dll (and not eg of this assembly)?
	//		Tested with a simplest dll, not only with Catkeys.dll.
	//	Also can depend on other things, eg handling some exception types, using Output.Clear etc. Only if [STAThread].
	//	With or without [STAThread], 1 call to Task.Run makes 12 (from 6 or 9), >=2 Task.Run makes 14.
	//The above numbers (6 and 9) are on Win10. On Win7 (virtual PC) the numbers are 4 and 7. Older .NET framework version.

	//static Point s_p;
	//static SimpleLib.Struct1 s_p;

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args)
	{
#if false
		Console.WriteLine(GetThreadCount());
#else
		//TODO: temporarily disabled
		//avoid loading System.Windows.Forms.dll when debugging, because sometimes want to find where is loaded System.Windows.Forms.dll
		if(!Debugger.IsAttached) _EnableVisualStylesEtc();
		//Perf.Next();
		//Perf.Write();

#if true
		TestMain();
#elif true
		var d = AppDomain.CreateDomain("AppDomain");
		TestAppDomainDoCallback(d);
		TestAppDomainUnload(d);
		//#else
		//		var d = AppDomain.CreateDomain("AppDomain");

		//		new Thread(() =>
		//		{
		//			Wait(1);
		//			Print("unload");
		//			TestAppDomainUnload(d);
		//		}).Start();

		//		TestAppDomainDoCallback(d);
#endif

		//for(int i = 0; i < 1; i++) {
		//	var t = new Thread(() =>
		//	  {
		//		  var d = AppDomain.CreateDomain("AppDomain" + i);
		//		  TestAppDomainDoCallback(d);
		//		  TestAppDomainUnload(d);
		//	  });
		//	t.SetApartmentState(ApartmentState.STA);
		//	t.Start();
		//}
		//Wait(10);
#endif
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _EnableVisualStylesEtc()
	{
		//info: each of these load System.Windows.Forms.dll and System.Drawing.dll.
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
	}

	static void TestScreenImage()
	{
		//var b = new Catkeys.Util.MemoryBitmap(-1, -1);
		//var b = new Catkeys.Util.MemoryBitmap(60000, 10000);
		//PrintList(b.Hdc, b.Hbitmap);

		//Wnd.Find("app -*").Activate();Sleep(100);
		//var w = Wnd.Find("app -*");
		//var w = Wnd.Find("Catkeys -*");

		//var s1 = Folders.Temp + "test.bmp";
		//var s2 = Folders.Temp + "test.png";
		////ScreenImage.Capture(new RECT(100, 30, 100, 100, true), s);
		////using(var b = ScreenImage.Capture(new RECT(100, 30, 100, 100, true))) {
		//using(var b = ScreenImage.Capture(w, new RECT(100, 30, 100, 100, true))) {
		//	//var b = b0.Clone(new Rectangle(0, 0, b0.Width, b0.Height), PixelFormat.Format24bppRgb);
		//	Print(b.PixelFormat);
		//	//b.Save()
		//	b.Save(s1, ImageFormat.Bmp);
		//	b.Save(s2, ImageFormat.Png);
		//	//b.Dispose();
		//}
		//Files.GetProperties(s1, out var p1);
		//Files.GetProperties(s2, out var p2);
		//PrintList(p1.Size, p2.Size);
		////return;
		//Shell.Run(s1);
		//Shell.Run(s2);

		//var r = new RECT(30, 30, 16, 16, true);
		//using(var b = ScreenImage.Capture(w, r)) {
		//	//using(var b = Image.FromFile(s) as Bitmap) {
		//	Print(b.PixelFormat);

		//	r.Offset(-r.left, -r.top);
		//	var k = b.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

		//	PrintList(k.Height, k.Width, k.Stride, k.PixelFormat, k.Scan0);

		//	b.UnlockBits(k);

		//	k = b.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

		//	PrintList(k.Height, k.Width, k.Stride, k.PixelFormat, k.Scan0);

		//	b.UnlockBits(k);
		//}

		//foreach(var f in Files.EnumDirectory(Folders.ProgramFiles, Files.EDFlags.AndSubdirectories | Files.EDFlags.IgnoreAccessDeniedErrors)) {
		//	if(f.IsDirectory || !(f.Name.EndsWith_(".png", true))) continue;
		//	//Print(f.FullPath);

		//	PixelFormat pf = 0;
		//	Perf.First();
		//	using(var b = Image.FromFile(f.FullPath) as Bitmap) {
		//		//PrintList(b.PixelFormat, f.Name);

		//		Perf.Next();
		//		pf = b.PixelFormat;
		//		//if(pf != PixelFormat.Format24bppRgb) continue;

		//		var t = b;
		//		//if(pf != PixelFormat.Format24bppRgb) {
		//		//	t = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format24bppRgb);
		//		//	Perf.Next();
		//		//}
		//		var k = t.LockBits(new Rectangle(0, 0, t.Width, t.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//		t.UnlockBits(k);
		//		Perf.Next();
		//		if(t != b) t.Dispose();
		//	}
		//	Perf.Next();
		//	Print(Perf.Times + "     " + pf);
		//}

		Perf.SpinCPU(100);

		var bmp = Folders.ThisAppImages + "test_r.png";
		//var bmp = new string[] { Folders.ThisAppImages + "test.png", Folders.ThisApp + "test.png-" };
		//var bmp = new object[] { Folders.ThisAppImages + "test.png", 0xff5555 };

		SIArea area;
		Wnd w;
		//w = Wnd.Misc.WndRoot;
		//w = Wnd.Find("* Word");
		//w = Wnd.Find("*- Notepad");
		//w = Wnd.Find("Quick *");
		w = Wnd.Find("app - Microsoft Visual Studio", "wndclass_desked_gsk");
		//w = w.Child("Class View", "GenericPane");
		w.ThrowIf0();
		area = w;
		//w.GetWindowAndClientRectInScreen(out var r1, out var r2); Print(r1); Print(r2); return;
		//CatException.ThrowIfFailed(Api.AccessibleObjectFromWindow(w, Api.OBJID_WINDOW, ref Api.IID_IAccessible, out var iacc));
		//var acc = new Acc() { a = iacc, elem = 0 };
		//area=acc;
		SIFlags f = 0;
		f |= SIFlags.WindowDC;
		int colorDiff = 0;
		//area.SetRect(665, 693, 11, 15);
		//area.SetRect(1691, 28, 203, 238);
		//area.SetRect(714, 68, 230, 250);
		//area.SetRect(2, 20, 230, 250);

		string sp = null;
		for(int i = 0; i < 1; i++) {
			//Perf.SpinCPU(100);
			Perf.First();
			//var u=ScreenImage.Find(bmp, area, colorDiff: colorDiff);
			var u = ScreenImage.Find(bmp, area, f, colorDiff: colorDiff);

			//var u = ScreenImage.Find("resource:test", area, f, colorDiff: colorDiff);

			//SIResult u = null;
			//using(var b = Tests.Properties.Resources.test) {
			//	u = ScreenImage.Find(b, area, f, colorDiff: colorDiff);
			//}

			Perf.Next(); sp += Perf.Times; sp += "\r\n";
			PrintList(u.Found, u.Rect);
			//ScreenImage.Test(bmp, area);
			//ScreenImage.Test(bmp, area, SIFlags.WindowDC);
			//Sleep(100);
		}
		Print(sp);

		//using(var b = ScreenImage.Capture(new RECT(100, 30, 100, 100, true))) {
		//	Print(b.PixelFormat);
		//}
		//using(var b = ScreenImage.Capture(Wnd.Find("Calculator"), new RECT(100, 30, 100, 100, true))) {
		//	Print(b.PixelFormat);
		//}
	}

	static void ShowBitmap(Bitmap b)
	{
		var testFile = Folders.Temp + "ScreenImage.png";
		b.Save(testFile);
		Shell.Run(testFile);
		Wait(0.5);
	}

	static void TestScreenImage_InBitmap()
	{
		var bmp = Folders.ThisAppImages + "test.png";
		//var bmp = Image.FromFile(Folders.ThisAppImages + "test.png") as Bitmap;
		//Shell.Run(bmp);
		Wnd w;
		w = Wnd.Find("app - Microsoft Visual Studio", "wndclass_desked_gsk");
		w.ThrowIf0();
		using(var area = ScreenImage.Capture(w, w.ClientRect)) {
			//ShowBitmap(area);
			var u = ScreenImage.Find(bmp, area, 0);
			PrintList(u.Found, u.Rect);
		}
	}

	static void TestScreenImage_BottomUp()
	{
		//using(var b = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
		//							   Screen.PrimaryScreen.Bounds.Height,
		//							   PixelFormat.Format32bppArgb)) {

		//	using(var g = Graphics.FromImage(b)) {
		//		g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

		//		_PrintBitmapStride(b);
		//	}
		//}
		//return;

		//var file = Folders.Temp + "test.bmp";
		//var r = new RECT(0, 0, 1, 4, true);
		//using(var b = ScreenImage.Capture(r)) {
		//	_PrintBitmapStride(b);
		//	//b.Save(file);
		//	b.Save(file,ImageFormat.Bmp);
		//}
		//using(var b = Bitmap.FromFile(file) as Bitmap) {
		//	_PrintBitmapStride(b);
		//}

		//Wnd w;
		//w = Wnd.Find("app - Microsoft Visual Studio", "wndclass_desked_gsk");
		//w.ThrowIf0();
		//var r = w.ClientRect;
		//Perf.First();
		//for(int i=0; i<7; i++) {
		//	var b = ScreenImage.Capture(w, r);
		//	Perf.Next();
		//}
		//Perf.Write();
	}

	static void _PrintBitmapStride(Bitmap b)
	{
		var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
		PrintList(d.Stride, b.PixelFormat);
		b.UnlockBits(d);
	}

	static void TestScreenImage_CaptureExample()
	{
		//var file = Folders.Temp + "notepad.png";
		//Wnd w = Wnd.Find("* Notepad");
		//w.Activate();
		//using(var b = ScreenImage.Capture(w.Rect)) { b.Save(file); }
		//Shell.Run(file);

		var file = Folders.Temp + "notepad.png";
		Wnd w = Wnd.Find("* Notepad");
		using(var b = ScreenImage.Capture(w, w.ClientRect)) { b.Save(file); }
		Shell.Run(file);

		//var file = Folders.Temp + "notepad.png";
		//Wnd w = Wnd.Find("* Notepad");
		////w.Activate();
		//var r = w.Rect; r.Offset(-r.left, -r.top);
		//using(var b = ScreenImage.Capture(w, r, true)) { b.Save(file); }
		//Shell.Run(file);
	}

	static void TestBitmapFromHbitmap()
	{
		var file = Folders.Documents + "untitled.bmp";
		var hb = Api.LoadImage(Zero, file, 0, 0, 0, Api.LR_LOADFROMFILE | Api.LR_DEFAULTSIZE);
		Print(hb);

		using(var b = Image.FromHbitmap(hb)) {
			_PrintBitmapStride(b);
		}

		using(var b = ScreenImage.BitmapFromHbitmap(hb)) {
			_PrintBitmapStride(b);
		}

		Api.DeleteObject(hb);
	}

	static void TestScreenImage2()
	{
		SIFlags flags = 0;
		//flags = SIFlags.WindowDC;

		SIArea area;

		var w = Wnd.Find("Quick*");
		area = w;
		area = new SIArea(400, 1600, 200, 200);

		//w = w.ChildById(2207); //vertical toolbar
		//CatException.ThrowIfFailed(Api.AccessibleObjectFromWindow(w, Api.OBJID_CLIENT, ref Api.IID_IAccessible, out var iacc));
		//var acc = new Acc() { a = iacc, elem = 3 };
		//area = acc;

		//var r = ScreenImage.Find("qm info.png", area, flags);
		var r = ScreenImage.Wait(0, "qm info.png", area, flags);
		Print(r.Rect);
		//r.MouseMove();
		//r.MouseMove(1, 1);
		//r.MouseMove(Coord.Reverse(1), Coord.Fraction(0.9));
		r.MouseClick();

		//ScreenImage.Find("qm info.png", area, flags)
		//	.MouseClick();

		//Mouse.
	}

	static void TestWndClick()
	{
		//Options.Relaxed = true;

		//Wnd.Find("Quick*").ChildById(2207).Click(10, 10);
		//Wnd.Find("Quick*").ChildById(2207).MouseMove(10, 10);
		//WaitFor.WindowActive(0, "Quick*").MouseMove(100, 100);

		//Print(WaitFor.ModifierKeysReleased(-5));
		//Print(WaitFor.MouseButtonsReleased(-5));
		//WaitFor.MouseButtonsReleased(out var wasPressed); Print(wasPressed);

		//Wnd.Find("Quick*").Click(10, 10);
		//Wnd.Find("Quick*").Click(20, 10, mmFlags:MFlags.NonClientXY|MFlags.WaitForButtonsReleased);

		//var w = Wnd.Find("Quick*");
		var w = Wnd.Find("* Notepad");
		//w.Click(100, 100);
		//w.Click(100, 100, flags:MFlags.Relaxed);
		//w.Click(10, 10, flags:MFlags.NonClientXY);
		//w.Click(10, 10, flags:MFlags.WorkAreaXY);
		//w.Click(400, 90, MButton.Down);
		//Wait(0.1);
		//Wait(3);
		//w.MouseMove(350, 70);
		//w.MouseMove(350, 70, mmFlags: MFlags.Relaxed);
		//Wait(0.1);
		//Mouse.LeftUp();

		//Mouse.Move(-100, 400);

		//Mouse.LeftDown(w, 1, 100, true);
		//Mouse.LeftDown(w, -1, 100);
		//Mouse.LeftUp(w, 200, w.MouseClientXY.Y);

	}

	static void TestOptionsMouseMoveSpeed()
	{
		//Wnd.Find("app -*").Activate(); Wait(0.5);
		Options.MouseMoveSpeed = 50;
		//Mouse.LeftDown(176, 910);
		//Mouse.MoveRelative(50, 0);
		//Mouse.LeftUp();

		Mouse.MoveRelative(50, 0);
		Mouse.MoveRelative(50, 200);
		Mouse.MoveRelative(-150, -100);
		Mouse.MoveRelative(500, 500);

		//test crossing an offscreen area
		//Mouse.Move(100, 200, screen: 1);
		//Options.MouseMoveSpeed = 500;
		//Mouse.Move(11, 962);

	}

	static void TestRecordMouseMove()
	{
		var xy0 = Mouse.XY;
		s_withSleepTimes = false;
		s_recptime = Time.Milliseconds;
		s_recMoves = new List<uint>();

		var hh = Api.SetWindowsHookEx(Api.WH_MOUSE_LL, _testRecordMouseMove_Proc, Catkeys.Util.ModuleHandle.OfProcessExe(), 0);
		Thread.CurrentThread.Join(7000);
		Api.UnhookWindowsHookEx(hh);

		var s = Catkeys.Util.Recording.MouseToString(s_recMoves, s_withSleepTimes);
		Print(s);

		Wait(1);
		Mouse.Move(xy0);
		Mouse.MoveRecorded(s);
	}

	static Api.HOOKPROC _testRecordMouseMove_Proc = _TestRecordMouseMove_Proc;
	static unsafe LPARAM _TestRecordMouseMove_Proc(int code, LPARAM msg, LPARAM lParam)
	{
		//var m = (Api.MSLLHOOKSTRUCT*)lParam;
		ref Api.MSLLHOOKSTRUCT m = ref *(Api.MSLLHOOKSTRUCT*)lParam;

		//PrintList(m.pt.X, m.pt.Y);
		//PrintList(m.pt.X - s_pp.X, m.pt.Y - s_pp.Y);

		var d = new Point(m.pt.X - s_pp.X, m.pt.Y - s_pp.Y);
		int ddx = d.X - s_ppd.X, ddy = d.Y - s_ppd.Y;
		//string s = null;
		//if(ddx < -16 || ddx > 15 || ddy < -16 || ddy > 15) s = ">15";
		//else if(ddx < -8 || ddx > 7 || ddy < -8 || ddy > 7) s = ">7";
		//else if(ddx < -4 || ddx > 3 || ddy < -4 || ddy > 3) s = ">3";
		//Print($"{d.X}, {d.Y}      {ddx}, {ddy}     {s}");
		s_pp = m.pt;
		s_ppd = d;

		if(s_withSleepTimes) {
			var t = Time.Milliseconds;
			s_recMoves.Add((uint)(t - s_recptime));
			s_recptime = t;
		}
		s_recMoves.Add(Calc.MakeUint(d.X, d.Y));

		return Api.CallNextHookEx(Zero, code, msg, lParam);
	}
	static Point s_pp = Mouse.XY;
	static Point s_ppd;
	static bool s_withSleepTimes;
	static long s_recptime;
	static List<uint> s_recMoves;

	static void TestHowLooksRecordedMouseScript()
	{
		Wnd w1 = Wnd.Find("Untitled - Notepad", "Notepad");

		//Mouse.Click(w1, 100, 100);
		//w1.MouseClick(100, 100);

		//Mouse.RightClick(w1, 100, 100);
		//w1.MouseClick(100, 100, MButton.Right);

		//Mouse.MoveRecorded("sjdksjdkjskdjksjdksjkdsjdks");

		//var speed = 0.5;
		//Mouse.MoveRecorded("sjdksjdkjskdjksjdksjkdsjdks", speed);
		//Mouse.Click(w1, 100, 100); Wait(0.5 * speed);
		////Mouse.Click(w1, 100, 100); WaitSF(0.5); //=Wait(0.5 * Options.SpeedFactor);
		////Mouse.ClickWait(w1, 100, 100, 0.5);

		//Mouse.RightClick()
		//Mouse.Move(
		Mouse.Click(w1, 1, 2);
		Mouse.ClickEx(MButton.Middle, w1, 1, 2);
		w1.MouseClick(1, 2);
		w1.MouseClick(1, 2, MButton.Right);
		Mouse.LeftDown(w1, 100, 200);
		Mouse.ClickEx(MButton.Left | MButton.Down, w1, 100, 200);

		//var f=new FileStream()
	}

	static void TestFindOrRun()
	{
		Wnd w = Wnd.FindOrRun("* Notepad", run: () => Shell.Run("notepad.exe"));
		Print(w);
	}

	static void TestMouseDragDrop()
	{
		var w = Wnd.Find("* Notepad");
		//w.Activate();
		//Wait(0.5);

		//Mouse.LeftDown(w, 8, 8);
		//Mouse.LeftUp(w, 28, 8);

		//Mouse.LeftDown(w, 8, 8);
		//Mouse.MoveRelative(20, 0, drop: true);

		//Mouse.LeftDown(w, 8, 8);
		//Mouse.MoveRelative(20, 0);
		////Mouse.Drop();
		//Wait(3);
		//Mouse.LeftUp(true);

		//using(Mouse.LeftDown(w, 8, 8)) Mouse.MoveRelative(20, 0);
		//using(Mouse.ClickEx(MButton.Left|MButton.Down, w, 8, 8)) Mouse.MoveRelative(20, 0);

		//Mouse.Move(w, 100, 100);
		//Mouse.Wheel(-5);
		//Wait(0.5);
		//Mouse.Wheel(1);

		//Wnd w2 = Wnd.Find("* WordPad");
		//w2.ZorderTopmost();
		//w2.ZorderNoTopmost();
		//Wait(0.5);

		//w = Wnd.Find("", "WorkerW");

		Mouse.Click(w, 20, 20);
	}

	//static int s_four = 4;

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestUnsafeLibrary(MButton b, bool add)
	{
		//int i =Unsafe.SizeOf<RECT>();
		//if(i== s_four) Print("4");

		//bool y = b.Has_(MButton.Right|MButton.Left);
		//bool y = b.Has_(MButton.Right);
		//Print(y ? "yes" : "no");

		//var p = new Point();
		//p.

		//Calc.SetFlag(ref b, MButton.Left, add);
		////Calc.SetFlag(ref b, MButton.Down, false);
		////Print(b);
		//Print(b==(MButton.Left|MButton.Right) ? "yes" : "no");

		//Debug_.CheckFlagsOpt(b, MButton.Down|MButton.Up);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestArrayBuilder()
	{
		//var a = new Catkeys.Util.LibArrayBuilder<long>();
		//a.Add() = 8;
		//var v=a[i];
		//Print(v==8 ? "8" : "no");

		var a = Wnd.Misc.AllWindows(sortFirstVisible: true);
		Print(a);
		Print(a.Length);
		//Print(Wnd.Misc.AllWindows(onlyVisible:true));
	}
	//static void TestArrayBuilder()
	//{
	//	var a = new Catkeys.Util.LibArrayBuilder<long>();
	//	a.Add() = 8;
	//	PrintList(a.Count, a.Capacity);
	//	var v=a[0];
	//	Print(v);
	//	a[0] = 9;
	//	Print(a[0]);
	//	long i = 10;
	//	//a.Add(ref i);
	//	a.AddV(10); Print(a[1]);
	//	//Print(a[2]);

	//	var r = new RECT(1, 2, 3, 4, false);
	//	var aa = new Catkeys.Util.LibArrayBuilder<RECT>();
	//	//aa.Add(ref r);
	//	aa.AddV(r);
	//	Print(aa[0]);
	//	aa.Add()=r;
	//}

	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
#if DEBUG
		//Output.IgnoreConsole = true;
		//Output.DebugWriteToQM2 = true;
#endif
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			Thread.Sleep(100);
		}

		try {
			//TestArrayBuilder();
			//TestUnsafeLibrary(MButton.Right|MButton.Down, true);

			//TestFindOrRun();

			//TestScreenImage2();
			//TestBitmapFromHbitmap();
			//TestScreenImage_BottomUp();
			//TestScreenImage_InBitmap();
			//TestScreenImage_CaptureExample();

			TestMouseDragDrop();
			//TestRecordMouseMove();
			//TestOptionsMouseMoveSpeed();
			//TestWndClick();
			//TestRejectTypes();
			//TestMoveInScreenWithCoord();
			//TestScreen_();
			//TestDebug_();
			//TestScriptOptionsDebug();
			//TestCoord();
			//TestMouseMoveClick();
			//TestDisableWarnings();
			//TestOptionsExample();
			//TestWndFromXY();

			//TestThreadStaticDtor();
			//TestBitArray();
			//TestRefVariable();
			//TestIntNullable2();
			//TestEnumToInt(MFlags.WorkAreaXY);
			//TestTimer_();
			//TestThreadPoolPerf();
			//TestSystemWaitPeriod();

			//var flags = MFlags.WorkAreaXY;
			//LibCheckEnumFlags<MFlags>((int)flags, (int)(MFlags.ForClick | MFlags.WaitForButtonsReleased));

			//Print(WaitFor.NoModifierKeysAndMouseButtons());

			//TestRect();
			//TestLibProcessHandle();
			//TestInterDomainLock();
			//TestOutputAllCases();
			//TestOutputServer();
			//TestFloatConvert();
			//TesOutputServerExternal();
			//OutputFormExample.Main();
			//TestCatMenuRun();
			//TestGetStringFromMethod();
			//TestWTimer();
			//TestOutputDestinations();
			//TestVariadicDllImport();
			//TestTuple();
			//TestDateFormat();
			//TestConsoleSpeed();
			//TestFnv64();
			//TestHexEncode();
			//TestBase64();
			//TestAsciiStartsWithI();
			//TestFindUtf8();
			//TestSciControl();
			//TestReadTextFile();
			//TestConvertUtf8();
			//TestStackallocSpeed();
			//TestPngSize();
			//Print(_big.Length);
			//Print(Convert.FromBase64String(_big).Length);
			//TestDictionary();
		}
		catch(CatException ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(ArgumentException ex) { PrintList(ex); }
		catch(Win32Exception ex) { PrintList(ex.NativeErrorCode, ex); }
		catch(Exception ex) when(!(ex is ThreadAbortException)) { PrintList(ex.GetType(), ex); }
		//catch(Exception ex) { }
		//Why try/catch creates 3 more threads? Does not do it if there is only catch(Exception ex). Only if STA thread.


		/*

		using static CatAlias;

		say(...); //Output.Write(...); //or print
		key(...); //Input.Keys(...);
		tkey(...); //Input.TextKeys(...); //or txt
		paste(...); //Input.Paste(...);
		msgbox(...); //TaskDialog.Show(...);
		wait(...); //Time.Wait(...);
		click(...); //Mouse.Click(...);
		mmove(...); //Mouse.Move(...);
		run(...); //Shell.Run(...);
		act(...); //Wnd.Activate(...);
		win(...); //Wnd.Find(...);
		speed=...; //Script.Speed=...;

		using(Script.TempOptions(speed

		*/

		//[SysCompil.MethodImpl(SysCompil.MethodImplOptions.NoOptimization)]

		//SysColl.List<int> a;
		//SysText.StringBuilder sb;
		//SysRX.Regex rx;
		//SysDiag.Debug.Assert(true);
		//SysInterop.SafeHandle se;
		//SysIO.File.Create("");
		//SysThread.Thread th;
		//SysTask.Task task;
		//SysReg.RegistryKey k;
		//SysForm.Form form;
		//SysDraw.Rectangle rect;
		//System.Runtime.CompilerServices



		//l.Perf.First();
		//l.TaskDialog.Show("f");
		//l.Util.LibDebug_.PrintLoadedAssemblies();
		//Print(l.TDIcon.Info);

	}
}

