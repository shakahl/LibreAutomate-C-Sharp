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
using System.Net;
using System.Net.NetworkInformation;

//using static Test.CatAlias;

[module: DefaultCharSet(CharSet.Unicode)]
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 219, 649 //unreachable code, unused var/field


static unsafe partial class Test
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

	#region ScreenImage

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
		object image;
		//image = "yellow macro.png";
		//image = "qm info.png";
		image = new string[] { "yellow macro.png", "qm info.png", "white macro.png" };
		SIFlags flags = 0;
		//flags = SIFlags.WindowDC;

		SIArea area;

		var w = Wnd.Find("Quick*");
		area = w;
		//area = new SIArea(400, 1600, 200, 200);
		//area = new SIArea(w, 1200, 0, 200, 200);

		//w = w.ChildById(2207); //vertical toolbar
		//w = w.ChildById(2212); //open items
		//var acc = Acc.FromWindowClientArea(w);
		////Print(acc.Rect);
		//area = acc;
		//area = new SIArea(acc, 2, 2, 20, 16);
		//area.SetRect(2, 2, 20, 16);

		//var r = ScreenImage.Find(image, area, flags);
		//var r = ScreenImage.Wait(10, image, area, flags);
		//var r = ScreenImage.Wait(0.1, image, area, flags, 0, t=> { Print($"{t.ListIndex} {t.MatchIndex} {t.Rect}"); return false; });
		//Print(ScreenImage.WaitNot(-5, image, area, flags));
		//Print(ScreenImage.WaitChanged(-5, area, flags));
		//var r = ScreenImage.Find(image, area, flags, 0, also: t => t.MatchIndex == 1);
		//var r = ScreenImage.Find(image, area, flags, 0, also: t => { t.MouseClick(); Wait(0.5); return false; });
		//var r = ScreenImage.Find(image, area, flags|SIFlags.AllMustExist);

		//var rectList = new List<RECT>();
		////var r = ScreenImage.Find(image, area, flags, 0, also: t => { rectList.Add(t.Rect); return false; });
		//var r = ScreenImage.Find(image, area, flags|SIFlags.AllMustExist, 0, also: t => { rectList.Add(t.Rect); return true; });
		//Print(rectList);

		SIResult r = null;
		for(int i = 0; i < 1; i++) {
			r = ScreenImage.Find(
				//"~:zZLPaxNBFMcrePfiwT9AbBXbVHE9CQueVKihp9rTgj8QT7nmVPakEXIYECsWLYtWmhqJS2vTH0I6VIUIohPbYrRaxxTXtdpkjEmTdXcnvt2NumyIV/3y5rE/5sP3vTdztE/b3uHqMKwuWDuba1vHLve72vzvF/svRV/mVEmKC0K8R5geisJr656ZW1dOHzsUiKG+g3ggRBWZZVWGFYIiKLRjKX2vla19fAv5dyyMXiYRkROVc0azGIkhrhM2iwAPuMNm43Ohrr2rFfLVtaXK6nM6qzAVcVeUYKIgrlN4AvfpaDTIFvX6xnpdW6utv66+X6Fy2DV1xRgKizgWARyKj/d0BlizvJm/cICc7X0s7Z85tY/F/rBMp45hWCQJxLEyckQIsNbWd7NcNEsbxhfN0D8wRYZtAOKYrAyIwELGCqIJGZ+X/Oz8nWHb2HJ8z4U836nBbp6IcJiwTj0cMgwcBqivvvGzmYkRbpm2UbdrFavCzG+bP4r6p0zKwcGdEiewQmPhwmI6cEaPUqNQ3i/fbvAd799z/cTumye7GPSIJBgWHHSJvmq9G9nJ2w1PtmWbBtRvVcvN9r9q3s1ppTw9S48D5+t3b6K/05lepeRdlTPHhXZs7mGy4Re3uem071FetGNXFu43AoL+LXNu7Oryk3ko+O7wxXZsfnEyyLo8Tt4AMHntUjvQYwOxnEm9mJt4OjWWww/+Av4T/QQ="
				"image:iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAIAAAAC64paAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAIxSURBVDhPrdLPaxNBFAdw/wP/CbEqtlExNyFXFTT01Hpa8AfiKdecyp40Qg4DYsWiZdFKt42kobVJWyFdaoUIohPbYrRaxxTjWm0yxqTJuruTvvjGbEhRD/XLY9jD+8ybmWQP30X+B55cyHVUPL08NvtyZOrZIyOLPTvj4caOiEbjTsw4fvL8tVsxbOuIxBPzK1L8jhAN2xE3R2bnni6Dvzo4jp3tkTj2OCvRr7ggbVG33Bv3Uv5TF7Cwsz0Sjyafgzl6Jee7RA8riwf7U129+o8tp1SxweMW2Nkeie9PZnCm4zYs292y3HLVKZbtjZJd+GYBhpNjZ3skHo4/EULg5G5l8VB/an/v6L7Ttw+cvUt0rhARJFzV2GtWwn6MxENjaXgeuGSl5vKKs/ndNos/4+nPIV1ohqCsWfARjLDkQh4JROLBB3Nw1NadYXL3uSmQiYxgJlcjRqBPgzWR4YEQfbtqopIYbgXPIy/51fpoWqrGYRQEJaUMVqIZqs6UywYqD2+WbZh85KKcHIzwBG1iCAxv4iAhOoUd/SeGUHnYLFrrG/W1Qv3Nem3lQzWoshbmXIAMRQxmws15V08UlYfzX6z3hXouX1taq75YrWgzjCQ4YoMyolGQ8GwhQsPhaVQefvepBmurrg/Pw9vAcNjAyDBfgFBTkBm+10eyrxgqD8PfoKOOnRnw9Rnw88Ijw2lhJsiHySUkEIn/FBiiKAm/P+rviYYHplszMf/Af88uMOfbZ5LV4Uf3+sYAAAAASUVORK5CYII="
				, area, flags);

			if(!r) { Print("not found"); return; }
		}
		Print(r.Rect);
		r.MouseMove();
		//r.MouseMove(1, 1);
		//r.MouseMove(Coord.Reverse(1), Coord.Fraction(0.9));
		//r.MouseClick();
		//Wnd.FromMouse().Activate();
		//r.MouseClick();

		//ScreenImage.Find("qm info.png", area, flags)
		//	.MouseClick();

		//Mouse.

		//var s = G.Controls.ImageUtil.ImageToString(Folders.ThisAppImages + "qm info.bmp");
		//Print(s);

		//ScreenImage.Find(
		//#region .   image
		//	"~:zZLPaxNBFMcjePfiwT9AbBXbVHE9CQueVKihp9rTglXEU645lT1phBwGxIpFy6KVpqakobXpDyEdqkIE0Y1tMVqtY4rrWm0yxqTJursT3+5UGzasV/3y3rAL78P3vTdzslvbHXB1HLIdcu927grsC/iJ/pcir3IpSYoJQqxTmB6IwG9rzczd6+dPHfPEQPdR3BskikyzKYoVFYVRcM9SeryVrX16B+efWBi+poZFpqYYoySLkRhkukpnEeAedyg2vhTq2vtaIV9dW6qsviCzCk0h5oqoWFUQ0wl8gft0JOJli3p9Y72urdXW31Q/rBA55Jq6ohSFRBwNAw7NxzrbPKxZ3sxfPqJe6HoiHZ45d4hGd1iqE8cwJKpxxLAydELwsNbWD7NcNEsbxlfN0D9SRYYyAHFUVnpFYOHECiJxGV+Smtn5+4O2seX4Xgxy36m+DhYPM9iwTjgOJywcFqivvm1mM2NDzDJto27XKlaFmt83fxb1z5mkg4M7UZ3AComGCotpzx09Tg5De799O8B3tOfArTP775xtpzAjkmBZcNEl8rr1bWQn7zW4bMs2Dejfqpa3x/+m8ZfTSnE9T48C1zTvwXhPm7O9Sok/lf7Tgh+be5RoNIvZzHTG5xQPP3ZlYaLhEcxvmXMjN5afzkPDDwav+LH5xUkv6/I4cRvAxM2rfiBnPbGcSb6cG3s2NZLDD/8C/hP9Ag=="
		//#endregion
		//	, area, flags);
	}

	static void ConvertBmpToPng()
	{
		//Output.DebugWriteToQM2 = true;
		//Output.IgnoreConsole = true;

		//foreach(var f in Files.EnumDirectory(Folders.ThisAppImages)) {
		//	if(f.IsDirectory) continue;
		//	var path = f.FullPath; if(!path.EndsWith_(".bmp", true)) continue;
		//	Print(path);
		//	using(var im = Image.FromFile(path)) {
		//		var path2 = Path.ChangeExtension(path, ".png");
		//		im.Save(path2, ImageFormat.Png);
		//	}
		//}
		foreach(var f in Files.EnumDirectory(Folders.ThisAppImages)) {
			if(f.IsDirectory) continue;
			var path = f.FullPath;
			Print(path);
			Print(G.Controls.ImageUtil.ImageToString(path));
		}
	}

	#endregion

	static void TestGarbageOfWaitWindowEtc()
	{
		object o = Wnd.Misc.WndTop;

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Find(null, null, o, flags: WFFlags.HiddenToo); });
		//var a2 = new Action(() => { Wnd.Find(null, null, o);});
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);
		//return;

		new PrintGcCollect();
		Task.Run(() =>
		{
			for(;;) {
				Print((GC.GetTotalMemory(false) / 1024 / 1024d).ToString_("F3"));
				Wait(1);
			}
			//var t = Time.Microseconds;
			//for(;;) {
			//	var tt = Time.Microseconds;
			//	var d = tt - t;
			//	if(d >= 200) { Print(d); Wait(0.1); tt = Time.Microseconds; }
			//	t = tt;
			//}
		});
		//WaitFor.WindowExists(-0.001, null, null, o, flags: WFFlags.HiddenToo);
		//Print(0.55555555.ToString_("F3"));
		Wait(0.2);
		//Print((GC.GetTotalMemory(false) / 1024 / 1024d).ToString_("F3"));
		//TaskDialog.Show("wait"); return;
		var w = Wnd.Find("Quick*", flags: WFFlags.HiddenToo); Print(w); return;
		for(int i = 0; i < 10; i++) {
			//WaitFor.WindowExists(-30, "nnname");
			//WaitFor.WindowExists(-30, null, "nnname");
			WaitFor.WindowExists(-1, null, "nnname", flags: WFFlags.HiddenToo);
			//WaitFor.WindowExists(-1, "nnname", flags: WFFlags.HiddenToo);
			//WaitFor.WindowExists(-1, null, null, o, flags: WFFlags.HiddenToo);
			//ScreenImage.Wait(-30, "qm info.png", w);
			//GC.Collect();
		}
		//Print((GC.GetTotalMemory(false) / 1024 / 1024d).ToString_("F3"));
		//var t = Time.Milliseconds;
		//for(int i = 0; Time.Milliseconds<t+30_000; i++) {
		//	var a = Wnd.Misc.AllWindows();
		//	//foreach(var v in a) {
		//	//	//var s = v.Name;
		//	//	var s = v.ClassName;
		//	//}
		//	Wait(0.01);
		//}
	}

	class PrintGcCollect
	{
		int k;
		~PrintGcCollect()
		{
			Print("GC");
			Task.Delay(500).ContinueWith(t => new PrintGcCollect());
		}
	}

	static void TestWndArray()
	{
		//var a = Wnd.FindAll("*u*");
		//Print(a);
		//Print("------");
		//var f = new Wnd.Finder("*m*");
		//Print(f.FindAllInList(a));

		//var w = Wnd.Find("Quick*");
		//var a = w.ChildAll(null, "QM_*");
		//Print(a);

		var a = Wnd.Misc.AllWindows();
		Print(a);
	}

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
			//TestWndArray();
			TestGarbageOfWaitWindowEtc();
			//ConvertBmpToPng();
			//TestScreenImage2();
			//TestBitmapFromHbitmap();
			//TestScreenImage_BottomUp();
			//TestScreenImage_InBitmap();
			//TestScreenImage_CaptureExample();

			//TestFtpUploadWithProgress(@"q:\app\ok - Copy.qml", "ftp://ftp.quickmacros.com/public_html/test/", "quickmac", "jucabA8~/ke+f4");
			//TestInterDomainVariables();

			//Example.Test();
			//TestLibBuffers();
			//TestWeakReference();
			//TestFormatSpeed();
			//TestSetStringLength();
			//TestArrayBuilder();
			//TestUnsafeLibrary(MButton.Right|MButton.Down, true);

			//TestFindOrRun();

			//TestMouseDragDrop();
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

