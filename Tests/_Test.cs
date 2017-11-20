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

using Catkeys.Types;
using Catkeys.Util;
//using static Test.CatAlias;

[module: DefaultCharSet(CharSet.Unicode)]
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 169, 219, 649 //unreachable code, unused var/field


[System.Security.SuppressUnmanagedCodeSecurity]
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

		//foreach(var f in Files.EnumDirectory(Folders.ProgramFiles, FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
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
			//var u=ScreenImage.Find(bmp, area, colorDiff: colorDiff).OrThrow();
			var u = ScreenImage.Find(bmp, area, f, colorDiff: colorDiff).OrThrow();

			//var u = ScreenImage.Find("resource:test", area, f, colorDiff: colorDiff).OrThrow();

			//SIResult u = null;
			//using(var b = Tests.Properties.Resources.test) {
			//	u = ScreenImage.Find(b, area, f, colorDiff: colorDiff).OrThrow();
			//}

			Perf.Next(); sp += Perf.Times; sp += "\r\n";
			PrintList(u.Rect);
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
			var u = ScreenImage.Find(bmp, area, 0).OrThrow();
			PrintList(u.Rect);
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

			if(r == null) { Print("not found"); return; }
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

	#region memory monitor

	static void _StartMemoryMonitorThread()
	{
		new PrintGcCollect();
		Task.Run(() =>
		{
			for(; ; ) {
				_PrintMemory();
				Wait(1);
			}
		});
		Wait(0.1);
	}

	static long s_mem0;
	static void _PrintMemory()
	{
		var mem = GC.GetTotalMemory(false);
		if(s_mem0 == 0) s_mem0 = mem;
		Print(((mem - s_mem0) / 1024d / 1024d).ToString_("F3"));
	}

	#endregion

	static void TestGarbageOfWaitWindowEtc()
	{
		var o = Wnd.Misc.WndTop;

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Find(null, null, o, flags: WFFlags.HiddenToo); }); //40
		//var a2 = new Action(() => { Wnd.Find(null, null, o); });
		//var a3 = new Action(() => { Wnd.Find("name", null, flags: WFFlags.HiddenToo); }); //115
		//var a4 = new Action(() => { Wnd.Find(null, "class", flags: WFFlags.HiddenToo); }); //130
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);
		//return;

		_StartMemoryMonitorThread();

		//var w = Wnd.Find("Quick*", flags: WFFlags.HiddenToo); Print(w); return;
		//var w = Wnd.Find(className:"Notepad", flags: WFFlags.HiddenToo); Print(w); return;
		//var w = WaitFor.WindowExists(20, className: "Notepad", flags: WFFlags.HiddenToo); Print(w); return;
		//var w = WaitFor.WindowExists(20, programEtc: "Notepad", flags: WFFlags.HiddenToo); Print(w); return;
		//var w = Wnd.Find(programEtc:"qm", flags: WFFlags.HiddenToo); Print(w); return;
		//var w = Wnd.Find("Catkeys *", "WindowsForms*"); //w=w.Child("**wfName:Edit"); Print(w); return;
		//foreach(var v in w.AllChildren()) PrintList(v.ClassName, v.NameWinForms);
		//return;

		//var w = Wnd.Find("Quick*");
		//Wnd w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
		//Wnd w = Wnd.Find("*- Google Chrome");
		//Wnd w = Wnd.Find("* Internet Explorer");
		//Wnd w = Wnd.Find("Site Manager");

		for(int i = 0; i < 1; i++) {
			//WaitFor.WindowExists(-10, null, null, o, flags: WFFlags.HiddenToo); //31-47 or 23-39 (16)
			//WaitFor.WindowExists(-10, "nnname");
			//WaitFor.WindowExists(-10, null, "ccclass");
			//WaitFor.WindowExists(-10, null, "ccclass", flags: WFFlags.HiddenToo);
			//WaitFor.WindowExists(-10, "nnname", flags: WFFlags.HiddenToo);
			//WaitFor.WindowExists(-10, programEtc: "ppprogram", flags: WFFlags.HiddenToo);
			//WaitFor.WindowExists(-10, programEtc: "ppprogram");
			//WaitFor.WindowExists(-20, also: v => v.HasChild(className: "moo"));
			//WaitFor.WindowExists(-20, also: v => v.HasChild("moo"), flags: WFFlags.HiddenToo);
			//var cf = new Wnd.ChildFinder("moo"); WaitFor.WindowExists(-20, also: v => v.HasChild(cf), flags: WFFlags.HiddenToo);

			//var f = new Wnd.ChildFinder("**wfName:no");
			//WaitFor.WindowCondition(w, v => f.FindIn(v), -10);

			//var f = new Wnd.Finder("jhsjdhjshdjsh");
			//WaitFor.Condition(-10, v => f.Find(), null, 10, 10);

			//var f = new Wnd.ChildFinder("jhsjdhjshdjsh");
			//WaitFor.Condition(-10, v => f.FindIn(w), null, 10, 10);

			//WaitFor.Condition(-10, v => !w.ChildById(7654).Is0, null, 10, 10);

			//var f = new Acc.Finder("LINK", "noo Untitled", 0);
			//var f = new Acc.Finder("web:LINK", "noo Untitled");

			//WaitFor.Condition(-10, v => f.FindIn(w), null, 10, 10);

			//WaitFor.WindowExists(-1, null, "**r|.+(a)", flags: WFFlags.HiddenToo, also:t=>false);
			//ScreenImage.Wait(-30, "qm info.png", w);
			//GC.Collect();

			//string s = "abc \\ \".\r\n\t\u0000\f";
			//WaitFor.Condition(-10, v => s.Escape_() == null, null, 10, 10);

			string s = "pare next firs";
			//s = "two * threefour * two * threefour * two * threefour * two * threefour * two * threefour * two * threefour * ";
			//s = "a b";
			//WaitFor.Condition(-10, v => { TestStringSegment(); return false; }, null, 10, 10);
			//WaitFor.Condition(-10, v => { foreach(var t in s.Split(s_spaces)) if(t.Length>1000) Print(t); return false; }, null, 10, 10);
			WaitFor.Condition(-10, v => { foreach(var t in s.Segments_(" ")) if(t.Length > 1000) Print(t); return false; }, null, 10, 10);
		}

		//var a = new Dictionary<Wnd, string>();
		//var b = Wnd.Misc.AllWindows();
		//for(int i = 0; i < b.Length; i++) a.Add(b[i], "stringggggggggggggggggggggggggggggg");
		//for(var t0=Time.Milliseconds; Time.Milliseconds < t0 + 10_000;) {
		//	for(int i = 0; i < b.Length; i++) a.TryGetValue(b[i], out var s);
		//	Time.Sleep(10);
		//}

		//var w = Wnd.FindFast(null, "QM_Editor");
		//Print(w);
		//int pid = w.ProcessId;
		//Print(pid);
		//Print(Process_.GetProcessName(pid, false, true));

		////string s1 = Process_.GetProcessName(pid, false, true);
		////string s2 = Process_.GetProcessName(pid, false, true);
		////Print(ReferenceEquals(s1, s2)); return;

		////_PrintMemory();
		////string s1 = "gggggg";
		////var p = stackalloc char[10];
		////for(int i = 0; i < 9; i++) p[i] = (char)('A' + i);

		//var t = Time.Milliseconds;
		//for(int i = 0; Time.Milliseconds < t + 10_000; i++) {
		//	//var a = Wnd.Misc.AllWindows();
		//	//foreach(var v in a) {
		//	//	//var s = v.Name;
		//	//	var s = v.ClassName;
		//	//}
		//	//var s = new string(p, 0, 9);
		//	////var s = s1 + "kkkkkk";
		//	////String.Intern(s);

		//	string pname = Process_.GetProcessName(pid, false, true);

		//	//if((i % 50) == 0) Wait(0.001);
		//	if((i % 50) == 0) Time.Sleep(1);
		//	//if((i % 50) == 0) Thread.Sleep(1);
		//}

		GC.Collect(); Wait(1.5);
	}

	class PrintGcCollect
	{
		int k;
		~PrintGcCollect()
		{
			Print("GC");
			Task.Delay(100).ContinueWith(t => new PrintGcCollect());
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

	static void TestDictionarySpeed()
	{
		var a = Wnd.Misc.AllWindows();
		Print(a.Length);

		var htable = new Hashtable();
		var dict = new Dictionary<Wnd, string>();
		var hset = new HashSet<Wnd>();
		var cdict = new ConcurrentDictionary<Wnd, string>();

		for(int i = 0; i < a.Length; i++) {
			var w = a[i];
			htable.Add(w, null);
			dict.Add(w, null);
			hset.Add(w);
		}

		for(int j = 0; j < 17; j++) {
			Perf.First();
			for(int k = 0; k < 10; k++) for(int i = 0; i < a.Length; i++) htable.ContainsKey(a[i]); //several times slower (need boxing)
			Perf.Next();
			for(int k = 0; k < 10; k++) for(int i = 0; i < a.Length; i++) dict.ContainsKey(a[i]);
			Perf.Next();
			for(int k = 0; k < 10; k++) for(int i = 0; i < a.Length; i++) hset.Contains(a[i]); //same speed
			Perf.Next();
			for(int k = 0; k < 10; k++) for(int i = 0; i < a.Length; i++) cdict.ContainsKey(a[i]); //same speed
			Perf.NW();
		}
	}

	static void TestHwndReusing()
	{
		string cn1, cn2;
		cn1 = "jooooo1";
		cn2 = "jooooo2";
		Wnd.Misc.WindowClass.InterDomainRegister(cn1, null);
		Wnd.Misc.WindowClass.InterDomainRegister(cn2, null);

		var d = new HashSet<Wnd>();
		Perf.First();
		for(int i = 1; ; i++) {
			var cn = ((i & 1) != 0) ? cn1 : cn2;
			//var w = Wnd.Misc.CreateWindow(0, "#32770", null, Native.WS_POPUP); //75000, 4 minutes
			////var w = Wnd.Misc.CreateMessageWindow("#32770"); //65000, 12 seconds
			//var w = Wnd.Misc.WindowClass.InterDomainCreateWindow(0, cn, null, Native.WS_POPUP); //75000, 4 minutes
			var w = Wnd.Misc.WindowClass.InterDomainCreateMessageWindow(cn); //65000, 9 seconds
			if(w.Is0) throw new Exception();
			Wnd.Misc.DestroyWindow(w);
			if(d.Contains(w)) break;
			d.Add(w);
			if(i % 1000 == 0) Print(i);
			Time.Sleep(1);
		}
		Perf.NW();
		Print(d.Count);
	}

	static void TestWindowClass()
	{
		var hset = new HashSet<string>();
		int n = 0, nn = 0;
		foreach(var w in Wnd.Misc.AllWindows()) {
			var s = w.ClassName;
			//Print(s);
			hset.Add(s); n++; nn++;
			foreach(var c in w.AllChildren()) { hset.Add(c.ClassName); nn++; }
		}
		PrintList(n, nn, hset.Count);

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { var w = Wnd.FindFast(null, "Notepad"); });
		//var a2 = new Action(() => { var w = Wnd.Find(null, "Notepad"); });
		//var a3 = new Action(() => { var w = Wnd.Find(null, "Notepad", flags:WFFlags.HiddenToo); });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);

		//var w = Wnd.FindFast(null, "Notepad");
		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { var s = w.ClassName; });
		//var a2 = new Action(() => { var s = w.Name; });
		//var a3 = new Action(() => { var s = w.ProcessName; });
		//var a4 = new Action(() => { var s = w.ProcessPath; });
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);

	}

	static void TestFindThreadWindow()
	{
		//var w = Wnd.FindFast(null, "Notepad");
		//int tid = w.ThreadId;
		//Print(Wnd.Find(null, "Notepad", WFOwner.ThreadId(tid)));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { w = Wnd.FindFast(null, "Notepad"); });
		//var a2 = new Action(() => { w = Wnd.Find(null, "Notepad", WFOwner.ThreadId(tid)); });
		//var a3 = new Action(() => { w = Wnd.Find(null, "Notepad"); });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(8, 1000, a1, a2, a3, a4);

		Print(Wnd.Misc.WndDesktop);
		Perf.SpinCPU(100);
		var a1 = new Action(() => { var w = Wnd.Misc.WndDesktop; });
		var a2 = new Action(() => { });
		var a3 = new Action(() => { });
		var a4 = new Action(() => { });
		Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

	}

	static void TestWndFindAgain()
	{
		//var w = Wnd.Find(also: v => v.HasChild(className: "msctls_statusbar32"));
		//var w = WaitFor.WindowExists(also: v => v.HasChild(className: "msctls_statusbar32"));
		//Print(w);

		//Print(Wnd.FindAll(programEtc: "qm"));
		Wnd w = Wnd.Find("Quick *");
		//Print(Wnd.FindAll(programEtc: w.ProcessId));
		//Print(Wnd.FindAll(programEtc: WFOwner.ProcessId(w.ProcessId)));
		//Print(Wnd.FindAll(programEtc: WFOwner.ThreadId(w.ThreadId)));
		//Print(Wnd.FindAll(programEtc: w));
		//Print(Wnd.FindAll(programEtc: WFOwner.OwnerWindow(w)));
		//Print(Wnd.Misc.AllWindows(false, true));
		//Print(Wnd.Misc.MainWindows());
		//Print(Wnd.Misc.ThreadWindows(w.ThreadId, sortFirstVisible:true));
		//w=w.Child
		//Print(w.AllChildren(true));
		//Print(w.ChildAll(null, "*s*"));
		//Print(w.WndChild(5));

	}

	static void TestStringCache()
	{
		//int n = 14;
		//var p = stackalloc char[n + 1];
		//for(int i = 0; i < n; i++) p[i] = (char)('A' + i);

		////var s1 = _stringCache.Add(p, n);
		////var s2 = _stringCache.Add(p, n);
		////var s3 = _stringCache.Add(p, n-1);
		////Print(s1);
		////Print(s2);
		////Print(s3);
		////Print(ReferenceEquals(s1, s2));
		////Print(ReferenceEquals(s1, s3));

		var hset = new HashSet<string>();
		foreach(var w in Wnd.Misc.AllWindows()) {
			hset.Add(w.ClassName); hset.Add(w.Name); hset.Add(w.ProcessName);
			foreach(var c in w.AllChildren()) { hset.Add(c.ClassName); hset.Add(c.Name); hset.Add(c.ProcessName); }
		}
		var a = new string[hset.Count];
		hset.CopyTo(a);
		Print(hset.Count);

		//var d = new Dictionary<int, string>();
		//foreach(var s in a) {
		//	//Print(s); continue;
		//	fixed (char* u = s) {
		//		var x = _Hash(u, s.Length);
		//		if(d.ContainsKey(x)) PrintList(s, s.Length, d[x]);
		//		else d.Add(x, s);
		//	}
		//}

		Wait(0.1);
		Perf.SpinCPU(100);
		for(int i1 = 0; i1 < 8; i1++) {
			Perf.First();
			for(int i2 = 0; i2 < a.Length; i2++) { var v = a[i2]; fixed (char* p = v) { var s = new string(p, 0, v.Length); } }
			Perf.Next();
			for(int i2 = 0; i2 < a.Length; i2++) { var v = a[i2]; fixed (char* p = v) { var s = _stringCache.Add(p, v.Length); } }
			Perf.Next();
			for(int i2 = 0; i2 < a.Length; i2++) { var v = a[i2]; fixed (char* p = v) { lock(_stringCache) { var s = _stringCache.Add(p, v.Length); } } }
			Perf.Next();
			for(int i2 = 0; i2 < a.Length; i2++) {
				var v = a[i2]; fixed (char* p = v) {
					lock(_stringCache) {
						if(!_stringCacheWR.TryGetTarget(out var cache)) _stringCacheWR.SetTarget(cache = new Catkeys.Util.StringCache());
						var s = cache.Add(p, v.Length);
					}
				}
			}
			Perf.NW();
		}
		//return;
		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 8; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { int x = _HashFnv1(p, n); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { int x = _Hash(p, n); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { int x = _HashFnv1(p, n); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { int x = _Hash(p, n); }
		//	Perf.NW();
		//}

	}

	static int _HashFnv1(char* data, int lengthChars)
	{
		uint hash = 2166136261;
		int i = 0;

		//To make faster with long strings, we take every n-th character. If length is >=18, takes 10 to 17 characters, including the last 2.

		lengthChars -= 2; //always take the last 2 characters (in the second loop) because often there are several strings like Chrome_WidgetWin_0, Chrome_WidgetWin_1...
		if(lengthChars >= 16) {
			int every = lengthChars / 16 + 1; //(8 to 15) + 2 samples for strings of length >= 8 + 2

			for(; i < lengthChars; i += every)
				hash = (hash * 16777619) ^ data[i];
			i = lengthChars;
		}
		lengthChars += 2;

		for(; i < lengthChars; i++)
			hash = (hash * 16777619) ^ data[i];

		return (int)hash;
	}

	static int _Hash(char* data, int lengthChars)
	{
		uint hash = 2166136261;
		int i = 0;

		//To make faster with long strings, we take every n-th character.
		//Also the last 1-2 characters (in the second loop), because often there are several strings like Chrome_WidgetWin_0, Chrome_WidgetWin_1...
		//Also we hash uints, not chars, unless the string is very short.

		if(lengthChars > 8) {
			int lc = lengthChars--;
			lengthChars /= 2; //we'll has uints, not chars
			int every = lengthChars / 8 + 1;

			for(; i < lengthChars; i += every)
				hash = (hash * 16777619) ^ ((uint*)data)[i];

			i = lengthChars * 2;
			lengthChars = lc;
		}

		for(; i < lengthChars; i++)
			hash = (hash * 16777619) ^ data[i];

		return (int)hash;
	}


	static Catkeys.Util.StringCache _stringCache = new Catkeys.Util.StringCache();
	static WeakReference<Catkeys.Util.StringCache> _stringCacheWR = new WeakReference<Catkeys.Util.StringCache>(null);

	static void TestCharPtrEndWith()
	{
		var s = "notepad.exe";
		fixed (char* p = s) {
			Print(Catkeys.Util.LibCharPtr.EndsWith(p, s.Length, ".Exe", true));
		}
	}

	static void TestProcesses()
	{
		//var a = Process_.GetProcesses();
		//Print(a);
		//Print(a.Length);

		//var w = Wnd.Find("Inbox *");
		////Print(w.ProcessName);
		////Print(w.ProcessPath);
		//int pid = w.ProcessId;
		//Print(Process_.GetProcessName(pid));
		//Print(Process_.GetProcessName(pid, true));
		//Print(Process_.GetProcessesByName("**m|notepad[]qm"));
		//Print(Process_.

		//var a2 = Process.GetProcesses();
		//Print(a2.Length);
		//foreach(var p in a2) {

		//	Print(p.ProcessName);
		//	try { Print(p.MainModule.FileName); } catch { Print("-"); }
		//	//try { Print(p.); } catch { Print("-"); }
		//}

		//Perf.SpinCPU(100);
		//_PrintMemory();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1;
		//	Perf.First();
		//	//for(int i2 = 0; i2 < n2; i2++) { Process.GetProcesses(); } //5 times calling this creates 2.2 MB of garbage
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { Process_.GetProcesses(); } //5 times calling this creates 0.031 MB of garbage
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}
		//_PrintMemory();

		//Wait(10);

		//var pid = Wnd.Find("* Notepad").ProcessId;
		//Print(pid);
		//Wait(0.1);
		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { var s = Process.GetProcessById(pid).ProcessName; });
		//var a2 = new Action(() => { var s = Process.GetProcessesByName("notepad"); });
		//var a3 = new Action(() => { var s = Process_.GetProcessName(pid); });
		//var a4 = new Action(() => { var s = Process_.GetProcessesByName("notepad"); });
		//Perf.ExecuteMulti(5, 1, a1, a2, a3, a4);

	}

	static void TestWndButton()
	{
		var w = Wnd.Find("Dialog");
		//var w = Wnd.Find("Properties -*");
		//w.Activate();
		//var b = w.Child("Normal").AsButton;
		var b = w.Child("3-state").AsButton;
		//var b = w.Child("icon").AsButton;
		//var b = w.Child("Option next").AsButton;
		Print(b);
		//b.Click();
		PrintList(b.GetCheckState(), b.GetCheckState(true), b.IsChecked(), b.IsChecked(true));
		//b.Check(true);
		//b.SetCheckState(1);

		//var w = Wnd.Find("* Notepad");
		//w.MouseClick(20, -8); w = WaitFor.WindowExists(2, null, "#32768", w.ProcessId);
		//w.MenuClick(2);
		//w.Activate(); w.MenuClick(61456, true); //Move
	}

	static void TestWildex2()
	{
		//Options.DisableWarnings("To match \"\",*");
		//Print(Wnd.Find("", "Notepad"));
		//Print(Wnd.Find(null, "Notepad"));
		//Print(Wnd.Find("*Notepad", ""));
		//Print(Wnd.Find("*Notepad", null));
		//Print(Wnd.Find("*Notepad", null, "notepad"));
		//Print(Wnd.Find("*Notepad", null, "notepad.exe"));
		//Print(Wnd.Find("*Notepad", null, ""));
		//Print(Wnd.Find("*Notepad", null, "**empty"));
		//Print(Wnd.Find("", "shell_trayWnd"));
		//Print(Wnd.Find("**empty", "shell_trayWnd"));
		//Print(Wnd.Find("**t|", "shell_trayWnd"));
		//Print(Wnd.FindFast(null, "shell_trayWnd"));
		//Print(Wnd.FindFast("", "shell_trayWnd"));

		//for(int i = 0; i < 1000; i++) {
		//	Wnd.Find("", "Notepad");
		//	Wait(0.010);
		//}

	}

	static void TestAddControl()
	{
		var f = new Form();
		//   1 Button 0x54030001 0x4 116 116 48 14 "OK" "toooooool"
		f.Add_<Button>(10, 10, 100, 30, "Test", "toooooool"); //.Enabled = false
		f.Add_<TextBox>(10, 50, 100, 30, anchor: AnchorStyles.Right | AnchorStyles.Left, tooltip: "tt2");
		f.ShowDialog();
		//f.Dispose();
		//MessageBox.Show("ddd");
	}
	//static extern void CreateTestInterface2(out IUnknown t); //more QI etc
	#region COM

	[ComImport, Guid("3AB5235E-2768-47A2-909A-B5852A9D1868"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface ITest
	{
		[PreserveSig]
		int Test1(int i);
		[PreserveSig]
		int TestOL(ref int i);
		[PreserveSig]
		int TestOL(string s);
		[PreserveSig]
		int TestNext(ref sbyte p);
	};

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern ITest CreateTestInterface();

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void CreateTestInterface2(out ITest t);
	//static extern void CreateTestInterface2(out IntPtr t); //with Marshal.GetTypedObjectForIUnknown is missing 1 Release
	//static extern void CreateTestInterface2([MarshalAs(UnmanagedType.IUnknown)] out object t); //the same number of QI etc as with 'out ITest t'

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", EntryPoint = "CreateTestInterface2", CallingConvention = CallingConvention.Cdecl)]
	static extern void CreateTestInterface3(out IntPtr t); //with Marshal.GetTypedObjectForIUnknown is missing 1 Release

	[DllImport(@"Q:\app\Catkeys\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void DllTestAcc();

	[ComImport, Guid("00000000-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IUnknown
	{
		//[PreserveSig]
		//IntPtr QueryInterface(ref Guid riid, out IntPtr pVoid);

		//[PreserveSig]
		//IntPtr AddRef();

		//[PreserveSig]
		//IntPtr Release();
	}

	static void TestCOM2()
	{
		CreateTestInterface3(out var ip);
		var vtbl = *(IntPtr**)ip;
		var f1 = vtbl[3];

		var d = (ITestVtbl.Test1)Marshal.GetDelegateForFunctionPointer(f1, typeof(ITestVtbl.Test1));
		Print(d.Invoke(ip, 9));

		Marshal.Release(ip);
	}

	struct ITestVtbl
	{
		public delegate int Test1(IntPtr ip, int i);
	}

	static void TestCOM()
	{
		try {
			CreateTestInterface2(out ITest t);

			//CreateTestInterface3(out var ip);
			//t = Unsafe.As<ITest>(Marshal.GetObjectForIUnknown(ip));

			Print("---- created");
			t.Test1(4);

			//var unknown = Marshal.GetIUnknownForObject(t);
			//Print("---");
			////var obj = Marshal.GetObjectForIUnknown(unknown);
			////var obj = Marshal.GetObjectForIUnknown(unknown) as ITest;
			//var obj = Unsafe.As<ITest>(Marshal.GetObjectForIUnknown(unknown)); //less AddRef/Release
			////var obj = Unsafe.As<ITest>(Marshal.GetUniqueObjectForIUnknown(unknown)); //less AddRef/Release
			//Print("---");
			//Marshal.Release(unknown);
			////object o = t; var obj = (ITest)o; //no

			//Print(ReferenceEquals(t, obj));


			//Print(Marshal.ReleaseComObject(t));
			//Print(Marshal.ReleaseComObject(obj));
			t = null;
		}
		finally {
			//Marshal.CleanupUnusedObjectsInCurrentContext(); //does nothing
			//GC.Collect(); //does nothing here, but works if in caller
		}
		Wait(0.1);
		Print("---- return");
	}

	class Acc2
	{
		IntPtr p;
		int u, uu, uuu;
	}

	static void TestSizeOfObjectsWithOverhead()
	{
		var o = new object();
		var s = new string('A', 1);
		object oi = 5;
		object od = 5.5;
		object or = new RECT();
		//ITest t; CreateTestInterface2(out t);
		Wnd w = Wnd.Find("Quick*"); Acc a = Acc.FromWindow(w);
		var a2 = new Acc2();

		long m0 = GC.GetTotalMemory(false);

		for(int i = 0; i < 1000; i++) {
			//o = new object(); //24 bytes (0 in Release)
			//s = new string('A', 1); //32 bytes
			//s = new string('A', 3); //32 bytes
			//s = new string('A', 4); //40 bytes
			//s = new string('A', 10); //49 bytes
			//oi = i; //24 bytes
			//od = 7.5; //24 bytes
			//or = new RECT(); //32 bytes
			//CreateTestInterface2(out t); //32 bytes
			//CreateTestInterface2(out t); t.Test1(4); //32 bytes
			//a = Acc.FromWindow(w); //64 bytes
			//a = new Acc(null); //32 bytes
			a2 = new Acc2();
		}

		Print(GC.GetTotalMemory(false) - m0);
	}

	#endregion

	static void TestCoord()
	{
		var w = Wnd.Find("Quick*");
		w.Activate();
		//w.Move(100, 100);
		//w.Move(100, 100, new CoordOptions(false, 1));
		//w.Move(300, Coord.Center);
		//w.Move(100, 100, new CoordOptions(false, 1));
		w.Move(100, 100, Coord.Reverse(200), Coord.Reverse(200), new CoordOptions(false, 1));
		//w.Resize(100, Coord.Center);
		//w.Resize(500, Coord.Max, new CoordOptions(false, w));
		//w.Move(0.5f, 1);
		//Mouse.Move(w, 100, 100);
		//Mouse.Move(w);
		//Mouse.Move(100, 100, new CoordOptions(false, 1));
		//Mouse.Move(500, Coord.MaxInside, new CoordOptions(true));
		//var a = Acc.FromXY(100, 100);
		//var a1 = Acc.Find(w, "MENUITEM", "Help");
		//var e1 = AElement.FromAcc(a1);
		//Print(e1.Name);
		//var e2 = (UIA.IElement2)e1;
		//Print(e2.Name);
		//Mouse.Move(10, 10, new CoordOptions(false, e2));
		//var a = Acc.FromXY(100, 100, new CoordOptions(false, w));
		//Print(a);

		//Print(Screen_.GetRect(1, true));
	}

	static void TestAcc()
	{
		//Acc.Misc.WorkaroundToolbarButtonName = true;
		//Acc a = Acc.FromPoint(644, 1138); //PUSHBUTTON
		////Acc a = Acc.FromPoint(225, 1138); //SPLITBUTTON
		////Acc a = Acc.FromPoint(453, 1138); //SEPARATOR
		//Print(a);
		//Wait(0.1);
		////return;
		//var cont = a.WndContainer;
		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 8; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { var k = a.WndContainer; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var k = cont.ClassNameIs("ToolbarWindow32"); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var k = cont.Is64Bit; }
		//	//Perf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { var k = a.RoleEnum; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var k = a.Name; }
		//	//Perf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { var k = a.RectInClientOf(cont); }
		//	Perf.NW();
		//}
		//return;

		//_StartMemoryMonitorThread();

		//Acc.Misc.WorkaroundToolbarButtonName = Input.IsScrollLock;
		//Print(Acc.Misc.WorkaroundToolbarButtonName);
		//Print(Acc.FromPoint(257, 1138)); return; //QM toolbar button Properties
		//Print(Acc.FromPoint(1147, 1092)); return; //QM floating toolbar button Mouse

		//Wnd w = Wnd.Find(className: "Shell_TrayWnd");
		//Wnd w = Wnd.Find(className: "QM_Editor");
		//Wnd w = Wnd.Find("QM TOOLBAR");
		//Wnd w = Wnd.Find("Options");
		//Wnd w = Wnd.Find("Dialog Editor");
		Wnd w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
		//Wnd w = Wnd.Find("*- Google Chrome");
		//Wnd w = Wnd.Find("* Internet Explorer");
		//Wnd w = Wnd.Find(null, "CabinetWClass");
		//Wnd w = Wnd.Find("FileZilla");
		//Wnd w = Wnd.Find("ILspy");
		//Wnd w = Wnd.Find("app -*");
		//Wnd w = Wnd.Find("Catkeys -*");
		//Wnd w = Wnd.Find("WinDbg*");
		//Wnd w = Wnd.Misc.WndRoot;
		//w.Activate(); Wait(.2);
		//w= w.ChildById(2216);
		//Wnd w = Wnd.Find(className: "QM_Editor").ChildById(2052);
		//Acc a = Acc.FromWindow(w);
		//Print(a);
		//a = a.Child(2);
		//Print(a);

		for(int i = 0; i < 1; i++) {
			var x = new TestAccClass();
			Perf.First();
#if false
			x.TestAcc(w);

			//foreach(Wnd ww in Wnd.Misc.AllWindows(true)) x.TestAcc(ww);

			Perf.NW();
			x.PrintMemory();

			//var a =Acc.deb; for(int j = 0; j < a.Length; j++) PrintList(j, a[j]);
#else
			//var a = Acc.Find(w, "LINK", "Untitled");
			//var a = Acc.Find(w, "LINK", "Untitled", AFFlags.SkipLists);
			//var a = Acc.Find(w, "web:LINK", "Untitled");
			//var a = Acc.Find(w, "web:LINK", "Untitled", AFFlags.Reverse);
			//var a = Acc.Find(w, "web:DOCUMENT");
			//var a = Acc.Find(w, "web:PANE");
			//var a = Acc.Find(w, "TITLEBAR", "Solution Explorer", AFFlags.NonClientToo);
			//var a = Acc.Find(w, "LISTITEM", "Portable", 0);
			//var a = Acc.Find(w, "class=Internet Explorer_Server:LINK", "Untitled", 0);
			//var a = Acc.Find(w, "web:LINK", also: o => ++o.Counter == 4);
			//var a = Acc.Find(w, "web:LINK", also: o => ++o.Counter == 4); //find 4-th LINK

			//var a = Acc.Find(w, "PUSHBUTTON", "History");
			//var a = Acc.Find(w, "web:DOCUMENT/div/PUSHBUTTON", "Bookmarks");
			//var a = Acc.Find(w, "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/PUSHBUTTON", "History");

			//var a = Acc.Find(w, "LINK", "Programming", AFFlags.HiddenToo); //1126 (depends on hidden tabs)
			//var a = Acc.Find(w, "LINK", "Programming"); //225
			//var a = Acc.Find(w, "LINK", "Programming", AFFlags.SkipLists); //184
			//var a = Acc.Find(w, "web:LINK", "Programming"); //120
			//var a = Acc.Find(w, "web:DOCUMENT/div/div/div/div/LIST/LISTITEM/LIST/LISTITEM/LINK", "Programming"); //74
			//var a = Acc.Find(w, "web:DOCUMENT/div/div[4]/div[5]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //47
			//var a = Acc.Find(w, "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/div/div/div/LIST/LISTITEM/LIST/LISTITEM/LINK", "Programming"); //94
			//var a = Acc.Find(w, "APPLICATION[4]/GROUPING[-4]/PROPERTYPAGE[-4]/browser/DOCUMENT/div/div[4]/div[5]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //58
			//var a = Acc.Find(w, "web://[4]/[5]//[3]/[2]///LINK", "Programming"); //47

			//var a = Acc.Find(w, "PUSHBUTTON", "Resources    Alt+F6"); //6.4 s
			//var a = Acc.Find(w, "class=ToolbarWindow32:PUSHBUTTON", "Resources    Alt+F6"); //120
			//var a = Acc.Find(w, "PUSHBUTTON", "Resources    Alt+F6", AFFlags.SkipLists); //193
			//var a = Acc.Find(w, "CLIENT/WINDOW/TOOLBAR/PUSHBUTTON", "Resources    Alt+F6"); //139

			//var f = new Acc.Finder("web:LINK", "Untitled");

			//f.FindIn(w); var a = f.Result;

			//var a = Acc.Find(w, "TEXT", "Address and*", AFFlags.SkipWeb);
			//var a = Acc.Find(w, "TEXT", "Address and*", AFFlags.Reverse);
			//var a = Acc.Find(w, "PUSHBUTTON", "Resources *", AFFlags.SkipLists);
			//var a = Acc.Find(w, "/////PUSHBUTTON", "Undo *", AFFlags.Reverse| AFFlags.SkipLists);

			//var a = Acc.Find(w, "web:LINK", "Programming", AFFlags.NoThrow);
			var a = Acc.Find(w, "web:DOCUMENT/div/div[4!]/div[5!]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //47

			//Acc aa=null;
			////int n = 0;
			//AFFlags fl = 0;
			////fl |=AFFlags.HiddenToo;
			////fl |= AFFlags.SkipLists;
			//var a = Acc.Find(w, "web:", null, fl, also: o =>
			// {
			//	 //n++;
			//	 //var s = o.ToString(v.Level);
			//	 //Print(s);
			//	 //aa = o;
			//	 //aa = o.ToAcc();
			//	 //o.Stop();
			//	 //o.SkipChildren();

			//	 //Perf.First();
			//	 // var r = o.RoleString;
			//	 // Perf.Next();
			//	 // var st = o.State;
			//	 // Perf.Next();
			//	 // Perf.Write();
			//	 //Print(o.ToString(o.Level));

			//	 //if(++o.Counter == 10) throw new CatException("TEST");

			//	 return false;
			// });
			//if(aa != null) { Print(aa); aa.Dispose(); }

			Perf.NW();

			Print(a);
			a?.Dispose();

			x.PrintMemory();
#endif
		}

		//using(var a = Acc.FromWindow(w)) {
		//	Print(a);
		//	Print(a.Parent);
		//}
	}

	class TestAccClass
	{
		int count;
		long mem0;
		//List<Acc> _a;

		public TestAccClass()
		{
			_accObj = _AccObj;
			//_a = new List<Acc>(100);
			mem0 = GC.GetTotalMemory(false);
		}

		public void TestAcc(Wnd w)
		{
			using(var a = Acc.FromWindow(w)) {
				//count++;
				//using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
				//using(var a = Acc.FromWindow(w, AccOBJID.TITLEBAR)) {
				//using(var a = Acc.FromPoint(853, 1603)) {
				//using(var a = Acc.FromPoint(310, 1605)) {
				//Print(a.WndContainer);
				//a.Elem = 2; //PUSHBUTTON Minimize (if a was AccOBJID.TITLEBAR)
				//PrintList(a.Elem, a.Role, a.Name);
				//PrintList(a.Role, a.Name);
				//var p = a.Parent;
				//PrintList(p.Role, p.Name, p.WndContainer); //CLIENT, Desktop 1, 65552 #32769 "" //exception if used p.Value or p.Description
				//p = p.Parent;
				//PrintList(p.Role, p.Name, p.WndContainer); //WINDOW, Desktop 1, 65552 #32769 ""
				////p.PropGetDoNotThrow = true;
				//p = p.Parent;
				//Print(p.Is0);

				//Print(a.Parent.Role);
				////Print(a.ParentAndDispose.Role);
				//Print(a.Role);

				//p.Dispose();
				//Print(a.State);
				//Print(a.RoleAsEnum);
				//Print(a.Role);
				//Print(a.Name);
				//Print(a.Value);
				//Print(a.Description);
				//Print(a.DefaultAction);
				//a.Value = "NEW";

				Print(w);
				//Print(a);
				a.EnumChildren(true, _accObj);
				return;

				//for(var t0 = Time.Milliseconds; Time.Milliseconds - t0 < 5000;) {
				//	//var r = a.Role;
				//	//var s = a.State;
				//	//var s = a.Name;
				//	var s = a.Value;
				//	Time.Sleep(1);
				//}
			}

		}

		Action<AFAcc> _accObj;
		void _AccObj(AFAcc a)
		{
			count++;
			//if(count % 100 == 0) {
			//	PrintMemory();
			//	//Thread.Sleep(10);
			//}

			//try {
			//	if(a.State.HasAny_(AccSTATE.INVISIBLE)) { a.SkipChildren(); return; }
			//}
			//catch { }

			//var role = a.RoleEnum;
			//if(role == AccROLE.SCROLLBAR) { a.SkipChildren(); return; }
			//if(role == AccROLE.OUTLINE || role == AccROLE.TEXT) a.SkipChildren(); //many OUTLINEITEM in QM2, many TEXT child TEXT in VS 2008

			//var s = a.ToString(a.Level);
			//Print(s);
			//if(isTB) { Print(s); continue; }

			//var s=a.State;
			//var s = a.RoleEnum;
			//var s = a.RoleString;
			//var s = a.Name;
			//var s = a.Value;
			//var s = a.Description;
			//var s=a.KeyboardShortcut;
			//var s=a.DefaultAction;
			//var s=a.ChildCount;
			//a.GetRect(out var s);
			//var s = a.Parent();
			//Print(s.Name);
			//s.Dispose();

			//Perf.First();
			////var r = a.RoleString;
			////Perf.Next();
			////var n = a.Name;
			////Perf.Next();
			//var k = a.State;
			////a.GetRect(out var s);
			////Perf.Next();
			//Perf.NW();
			////if(k.HasAny_(AccSTATE.INVISIBLE) || r=="OUTLINE" || r=="TEXT") a.SkipChildren();
			//if(k.HasAny_(AccSTATE.INVISIBLE)) a.SkipChildren();
			//Print(a);
		}

		public void PrintMemory()
		{
			long mem = GC.GetTotalMemory(false) - mem0;
			long memObj = (count > 0) ? (mem / count) : 0;
			//PrintList(count, Catkeys.Util.StringCache.LibDebugStringCount, Math.Round(mem / 1024 / 1024d, 3).ToString_(), memObj);
			PrintList(count, Math.Round(mem / 1024 / 1024d, 3).ToString_(), memObj);
		}
	}

	static void TestStringEscape()
	{
		string s = "abc \\ \".\r\n\t\u0000\f";
		string r = s.Escape_();
		Print(r);
		Print(ReferenceEquals(s, r));

	}

	static void TestNewWndFind()
	{
		var w = Wnd.Find("Quick*");
		//Print(w);

		//Print(Wnd.FindAll(null, "QM_*"));
		//Print(Wnd.FindAll("*k*"));


		//var f = new Wnd.Finder(null, "QM_*");
		//Print(f.Find());
		//Print(f.FindAll());
		////Print(f.IsMatch(w));
		//var a = Wnd.Misc.AllWindows(true);
		//Print(f.FindInList(a));
		////Print(f.FindAllInList(a));

		//Print(w.Child(className: "*List*"));
		//Print(w.ChildAll(className: "*Tree*"));

		//Wnd c = w.Child(className: "QM_Code");
		//Print(c);
		//var f = new Wnd.ChildFinder(null, "QM_*");
		////Print(f.FindIn(w));
		////Print(f.FindAllIn(w));
		////Print(f.IsMatch(c));
		////Print(f.IsMatch(c, w));
		//var a = w.AllChildren();
		////Print(f.FindInList(a));
		//Print(f.FindAllInList(a));

		//Print(w.ChildById(2212));
	}

	static void TestOutputWriteOverloads()
	{
		var s = "stringgg";
		object o = s;
		var sa = new string[] { "one", "two" };
		Hashtable col = new Hashtable(); col.Add(1, "a"); col.Add(2, "b");
		var d = new Dictionary<int, string>() { { 1, "a" }, { 2, "b" } };

		Output.Write(s);
		Print("---");
		Output.Write(o);
		Print("---");
		Output.Write(sa);
		Print("---");
		Output.Write(col);
		//Output.Write(col.Cast<KeyValuePair<object,object>>());
		Print("---");
		Output.Write(d);
		Print("---");

		//Print(String.Join("|", d));
		//Print(String.Join("|", col));
		//Output.WriteList("one", 2);
		//Output.Write("one", 2);
		//Output.Write(d, "SEP");
	}

	static void TestSBCache()
	{
		//StringBuilder sb;

		//Perf.First();
		//Time.Sleep(1);
		//Perf.Next();
		//Time.Sleep(1);
		//Perf.NW();

		//Print(Acc.FromMouse());

		//var csv = new CsvTable("one,two\nthree,four");
		//Print(csv.ToString());

		//var d = new TaskDialog("aaa", footerText: "FOOTER");
		////d.FlagRtlLayout = true;
		//d.SetTimeout(5, "bang!");
		//d.ShowDialog();

		//this code executes maybe one minute. The server gets all text in one time. Then its StringBuilder's capacity is > 870 MB, and total memory should be several GB.
		//for(int i = 0; i < 10000000; i++) {
		//	Output.Write("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq");
		//}
		//this code is OK. Server gets text maybe 10 times.
		//for(int i = 0; i < 1000; i++) {
		//	Output.Write("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq");
		//	Time.Sleep(1);
		//}
		//MessageBox.Show("");
	}

	static void TestRepeat()
	{
		//Repeat(5, o =>
		//{
		//	Print(o);
		//});

		//5.Times(o =>
		//{
		//	Print(o);
		//});

		//for(int i = 0; i < 5; i++) {
		//	Print(i);
		//}

		//foreach(int i in I(5)) {
		//	Print(i);
		//}

		//var l1 = new Loop(5);
		//while(l1.Do) {
		//	Print("loop");
		//}

		//int n = 0;

		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 10; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { for(int i = 0; i < 5; i++) n += i / 2; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { 5.Times(i => n += i / 2 ); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { for(int i = 0; i < 5; i++) n += i / 2; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { 5.Times(i => n += i / 2 ); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { foreach(int i in I(5)) n += i / 2; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { foreach(var i in Enumerable.Range(0, 5)) n += i / 2; }
		//	Perf.NW();
		//}

		//Print(n);

		//Wait(5);
		//5.s();

		////5.S();
		////5.sec();

		//Wait(0.5);
		//0.5.s();

		//Time.Sleep(10);
		//10.ms();

		//10.MS();
	}

	//public struct Loop
	//{
	//	int _n;

	//	public Loop(int n) { _n=n;  }

	//	public bool Do { get => --_n >= 0; }
	//}

	//public static IEnumerable<int> I(int count)
	//{
	//	for(int i = 0; i < count; i++) yield return i;
	//}

	//public static void Repeat(int nTimes, Action<int> code)
	//{
	//	for(int i = 0; i < nTimes; i++) code(i);
	//}

	//public static void Times(this int count, Action<int> code)
	//{
	//	for(int i = 0; i < count; i++) code(i);
	//}

	static void TestIEnumerableWnd()
	{
		var w = Wnd.Find(null, "QM_Editor");
		//Print(w.ChildAll(null, "*Tree*"));

		//var a = Acc.Find(w.ChildAll(null, "*Tree*"), null, "Time*");
		//Print(a);

		//var b = Acc.Find(w, new Wnd.ChildFinder(null, "*Tree*"), null, "Time*");
		//Print(b);

		//var cf =new Wnd.ChildFinder(null, "*Tree*");
		////if(cf.FindIn(w)) Print(cf.Result);
		//var b = Acc.Find(w, cf, null, "Time*");
		//Print(b);
		//Print(cf.Result);

		var af = new Acc.Finder("PUSHBUTTON", "OK");
		//Print(Wnd.Find(className: "#32770", also: o => af.FindIn(o)));
		Print(Wnd.Find(className: "#32770", also: t => t.HasAcc(af)));
		Print(af.Result);
	}

	static void TestStringSegment()
	{
		//string s ="parent3 next first3";
		//string s = "one * two\r\nthree ";
		//string s = "A B C D";
		//string s = "AAAAAA BBBBBBBBBBBB CCCCC DDDDDDD EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE F";
		string s = "AA AAAA BBB BBB BB BBBB CCCCC DDDDDDD E EEEE EE E E EEE EEE EEEE EE EE EE EEEE EEEEEEEEEEEE EEEEEE EEEEE EEEE EEEEE EEE F";
		//string s = "Aa\r\nBa\nCa\rDa\r\nEa";
		var u = new string[] { "one", "two three four", "five six seven eight\r\nten eleven twelve", "two * threefour * two * threefour * two * threefour * " };

		//var aa = s.Split(' ');
		//Print("".Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length); return;

		//var seg = s.Segments_(" ");
		//seg.ToStringArray();
		//Print(seg.Count());
		//Print(seg.Count());
		//foreach(var v in seg) Print(v);
		//foreach(var v in seg) Print(v);
		//return;

		Print(s.Split_(" ")); //return;

		//s = "";
		//foreach(var t in s.Segments_(" ")) Print(t);
		//foreach(var t in s.Segments_(Separators.Word, SegFlags.NoEmpty)) Print(t);
		//foreach(var t in s.Segments_(Separators.Line)) Print(t);
		//Print(s.Segments_(" ").ToStringArray());

		//using(var reader = new StringReader(s)) { for(string t; (t = reader.ReadLine()) != null;) Print($"'{t}'"); }
		//return;

		//Print("----");
		//return;

		//var g = new Segmenter(s, " ");
		////Print(g.Count());
		////Print(g.ElementAt(1));
		////Print(g.Reverse());
		////Print(g.ToArray());
		//return;

		//g_n++;

		100.ms();
		var mem0 = GC.GetTotalMemory(false);
		int k1 = 0, k2 = 0, k3 = 0, k4 = 0, k5 = 0, k6 = 0, k7 = 0, k8 = 0, k9 = 0;
		Perf.SpinCPU(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 100;
			Perf.First();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { var a = v.Split(s_spaces); foreach(var t in a) k1 += t.Length; } }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { foreach(var t in v.Segments_(" ")) k2 += t.Length; } }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { foreach(var t in v.Segments_(" \t")) k3 += t.Length; } }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { foreach(var t in v.Segments_(Separators.Whitespace)) k4 += t.Length; } }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { foreach(var t in v.Segments_(Separators.Word)) k5 += t.Length; } }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { foreach(var t in v.Segments_(0, v.Length, " ")) k6 += t.Length; } }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { foreach(var v in u) { foreach(var t in v.Segments_(Separators.Word, SegFlags.NoEmpty)) k7 += t.Length; } }
			//Perf.Next();
			////for(int i2 = 0; i2 < n2; i2++) { foreach(var t in s.Segments_(Separators.Line)) k8 += t.Length; }
			////Perf.Next();
			////for(int i2 = 0; i2 < n2; i2++) { using(var reader = new StringReader(s)) { for(string t; (t = reader.ReadLine()) != null;) k9 += t.Length; } }
			////Perf.Next();

#if !true
			for(int i2 = 0; i2 < n2; i2++) { var a = s.Split(s_spaces); }
			Perf.Next();
#else
			for(int i2 = 0; i2 < n2; i2++) { var a = s.Split_(" "); }
			Perf.Next();
#endif

			Perf.Write();
		}
		PrintList(k1, k2, k3, k4, k5, k6, k7, k8, k9);
		Print((GC.GetTotalMemory(false) - mem0) / 1024);
	}
	static int g_n;
	static char[] s_spaces = new char[] { ' ' };
	//static char[] s_spaces = new char[] { ' ', '\t' };

	static void TestLineCount()
	{
		string s = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\r\nBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB\nCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC\rDDDDDDDDDD";
		Print(s.CountLines_());

		Perf.SpinCPU(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { int n = s.CountLines_(); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { int n = s.LineCount_2(); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { }
			Perf.NW();
		}

	}

	static void TestFastAllocateString()
	{
		//results: FastAllocateString is just few % faster than new string('\0', n). Both depend on length, probably both fill with '\0' (once). But new string('not \0', n) is much slower.

		var fastAllocate = typeof(string).GetMethod("FastAllocateString", BindingFlags.NonPublic | BindingFlags.Static)
			?.CreateDelegate(typeof(FastAllocateStringT)) as FastAllocateStringT;
		Print(fastAllocate);

		//string s = fastAllocate(100);
		//Print(s.Length);
		//fixed(char* p = s) {
		//	for(int i = 0; i <= s.Length; i++) Print((int)p[i]);

		//}

		100.ms();
		Perf.SpinCPU(200);
		for(int i1 = 0; i1 < 10; i1++) {
			int n2 = 10000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { var s = new string('\0', 1000); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var s = fastAllocate(1000); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			Perf.NW();
		}

	}
	delegate string FastAllocateStringT(int length);

	static void TestStringCompareSpeed()
	{
		string s = "qwertyuio", ss = (s + " ").Remove(s.Length);
		//Print(ReferenceEquals(s, ss));
		Perf.SpinCPU(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 10000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { s.Equals(ss); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s.Equals(ss, StringComparison.Ordinal); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { }
			Perf.NW();
		}

	}

	static void TestAccNavigate()
	{
		Wnd w;

		//w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
		//var a = Acc.Find(w, "web:LINK", "General", navig: "pa3 ne fi3");
		//Print(a);
		//a?.Dispose();

		//return;

		//w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
		//var a = Acc.Find(w, "web:LINK", "General");
		//Print(a);
		//var b = a.Navigate("parent3 next first3");
		////var b = a.Navigate("pa3 ne fi3");
		////var b = a.Navigate("p3 n f3");
		////var b = a.Navigate("#9,3 #5 #7,3");
		////var b = a.Navigate("parent,3 next first,3");

		////w = Wnd.Find("* Studio ");
		////Print(w);
		////var a = Acc.Find(w, "PUSHBUTTON", "Search Control");
		////Print(a);
		//////var b = a.Navigate("pr fi2 ne2");
		//////var b = a.Navigate("pr fi ch3");
		////var b = a.Navigate("pa fi2 ch3");

		////w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
		////var a = Acc.FromWindow(w, AccOBJID.CLIENT);
		////Print(a);
		//////var b = a.Navigate((AccNAVDIR)0x1009);
		////var b = a.Navigate("#0x1009");

		//Print(b);
		//a.Dispose();
		//b?.Dispose();

		//return;

		//foreach(var v in Wnd.Misc.AllWindows()) {
		//	foreach(var a in )
		//}

		Wnd wSkip = Wnd.Find("Quick*").Kid(2202);
		w = Wnd.Misc.WndRoot;
		//w = Wnd.Find("app -*");
		//w = Wnd.Find("* - Notepad");
		//Perf.First();
		w = Wnd.Find("* Mozilla Firefox");
		//w = Wnd.Find("* Google Chrome");
		//w = Wnd.Find("* Internet Explorer");
		//w = Wnd.Find("Quick*").ChildById(2053);
		//w = (Wnd)(IntPtr)5965264;

		//w = Wnd.Find("Catkeys - Q*");

		//var a = Acc.Find(w, "TOOLBAR", "Help");
		//Print(a);
		//var p = a.Parent();
		//Print(p);
		//Print(p.Navigate(AccNAVDIR.FIRSTCHILD));

		////var a = Acc.Find(w, "CLIENT", "Panels");
		////Print(a);
		////Print(a.Navigate(AccNAVDIR.CHILD, 14));

		//return;

		//Perf.Incremental = true;
		using(var aRoot = Acc.FromWindow(w)) {
			aRoot.EnumChildren(true, a =>
			{
				if(a.WndContainer == wSkip) { a.SkipChildren(); return; }
				if(a.IsInvisible) { a.SkipChildren(); return; }
				//switch(a.RoleString) { case "TITLEBAR": case "SCROLLBAR": case "MENUBAR": case "GRIP": a.SkipChildren(); return; }
				var s = a.ToString(a.Level);
				//Perf.First();
				using(var aa = a.Navigate(AccNAVDIR.PARENT)) {
					//Perf.Next();
					if(aa == null) PrintList(s, a.WndContainer, a.WndContainer.WndWindow);
					//if(aa == null) Print(s);
					//else Print("<><c 0x8000>" + s + "</c>\r\n<c 0xff0000>" + aa.ToString(a.Level+1) + "</c>");
				}
			});
			//_TestAccNavigate(aRoot, 0, out _, out _, out _);
		}
		//Perf.Write();
		//Perf.NW();
		Print("END");
	}

	static void _TestAccNavigate(Acc parent, int level, out int nChildren, out Acc firstChild, out Acc lastChild)
	{
		nChildren = 0; firstChild = lastChild = null;
		var w = parent.WndContainer;
		if(!w.IsVisible || w.ControlId == 2202) return;

		var k = parent.Children(false); if(k.Length == 0) return;
		bool testFirstLast = false;
		for(int i = 0; i < k.Length; i++) {
			Acc a = k[i];
			if(a.IsInvisible) continue;
			if(!testFirstLast) {
				if(i > 0 || i < k.Length - 1) {
					Print(a.ToString(level));
					Native.ClearError();
					bool checkInvisible = false;
					if(i > 0) {
						using(var g = a.Navigate(AccNAVDIR.PREVIOUS)) {
							if(g == null && !(checkInvisible && k[i - 1].IsInvisible)) Print("<><c 0xff>previous  </c>" + Native.GetErrorMessage());
						}
					}
					if(i < k.Length - 1) {
						using(var g = a.Navigate(AccNAVDIR.NEXT)) {
							if(g == null && !(checkInvisible && k[i + 1].IsInvisible)) Print("<><c 0xff>next  </c>" + Native.GetErrorMessage());
						}
					}
				}
			}

			_TestAccNavigate(a, level + 1, out var nc, out var fc, out var lc);

			if(testFirstLast && nc != 0) {
				Print(a.ToString(level));
				Native.ClearError();
				bool checkInvisible = false;
				using(var g = a.Navigate(AccNAVDIR.FIRSTCHILD)) {
					if(g == null && !(checkInvisible && fc.IsInvisible)) Print("<><c 0xff>first  </c>" + Native.GetErrorMessage());
				}
				using(var g = a.Navigate(AccNAVDIR.LASTCHILD)) {
					if(g == null && !(checkInvisible && lc.IsInvisible)) Print("<><c 0xff>last  </c>" + Native.GetErrorMessage());
				}
				fc.Dispose(); if(lc != fc) lc.Dispose();
			}
		}
		if(testFirstLast) {
			nChildren = k.Length; firstChild = k[0]; lastChild = k[k.Length - 1];
			for(int i = 1; i < k.Length - 1; i++) k[i].Dispose();
		} else {
			for(int i = 0; i < k.Length; i++) k[i].Dispose();
		}
	}

	static void TestNetFindObjectFunctions()
	{
		//var v = Process.GetProcessesById

	}

	static void TestOrThrow()
	{
		//var w = Wnd.Find("*Notepad").OrThrow();
		////var a = Acc.Find(w, "MENUITEM", "Edit").OrThrow();
		//var a = Acc.Find(w, "MENUITEM", "Edit") ?? throw new NotFoundException();
		//Print(a);
		////var s = a.Name.OrThrow();
		////Print(s);

		//Wnd.Find("*Notepad").MouseClick(100, 100);
		//Wnd.Find("*Notepad").OrThrow().MouseClick(100, 100);

		//Wnd w = Wnd.Find("Quick*");
		//var u = ScreenImage.Find(Folders.ThisAppImages + "qm info.png", w, SIFlags.WindowDC).OrThrow();
		//var u = ScreenImage.Wait(-2, Folders.ThisAppImages + "qm info.png", w, SIFlags.WindowDC).OrThrow();
		//PrintList(u.Rect);
		//Print( ScreenImage.WaitNot(-2, Folders.ThisAppImages + "qm info.png", w, SIFlags.WindowDC));
		//Print( ScreenImage.WaitChanged(-2, w, SIFlags.WindowDC));

		//var w = Wnd.Find("Catkeys - Q*");
		//var a = Acc.Find(w, "LISTITEM", "test.cs", AFFlags.SkipWeb);
		//var w = Wnd.Find("*Internet Explorer");
		//Perf.First();
		//var a = Acc.Find(w, "PUSHBUTTON", "Home", AFFlags.Reverse| AFFlags.SkipWeb);
		//Perf.NW();
		//Print(a);
	}

	static void TestAccExamples()
	{
		//var f = new Acc.Finder("PUSHBUTTON", "Apply"); //object properties
		//Wnd w = Wnd.Find(className: "#32770", also: t => f.FindIn(t));
		//Print(w);
		//Print(f.Result);

		//var w = Wnd.Find("Find");
		////var a = Acc.Find(w, "PUSHBUTTON", also: o => o.GetRect(out var r, o.WndWindow) && r.Contains(266, 33));
		////var a = Acc.Find(w, "PUSHBUTTON", also: o => o.GetRect(out var r, o.WndWindow) && r.left==234);
		////var a = Acc.Find(w, "PUSHBUTTON", also: o => ++o.Counter == 2, navig: "pa pr2");
		//var a = Acc.Find(w, "PUSHBUTTON", also: o => o.Level == 2);

		var w = Wnd.Find("*Mozilla Firefox");
		//var a = Acc.Find(w, "LINK", also: o => o.Value == "http://www.quickmacros.com/forum/viewforum.php?f=3&sid=720fc3129e6c70e07042b446be23a646");
		//var a = Acc.Find(w, "LINK", also: o => o.Value.Like_("http://www.example.com/x.php?*"));
		//var a = Acc.Find(w, "LINK", also: o => o.Value?.Like_("http://www.example.com/x.php?*") ?? false);
		//var a = Acc.Find(w, "web:LINK", "General");
		//var a = Acc.Find(w, "web:LINK", "**m|Untitled[]General");

		var f = new Acc.Finder("web:LINK", "General");
		f.MaxLevel = 10;
		if(!f.FindIn(w)) return;
		var a = f.Result;

		//var w = Wnd.Find("*Sandcastle*");
		////var a = Acc.Find(w, "web:LINK", "**m|Untitled[]General");
		//Print(Acc.FromWindow(w).Children(true).Where(o=>!o.IsInvisible));

		//var w = Wnd.Find("Find");
		////var a = Acc.Find(w, "class=button:PUSHBUTTON");
		////var a = Acc.Find(w, "class=button:");
		//var a = Acc.Find(w, "id=1132:PUSHBUTTON");

		Print(a);
		a?.Dispose();

		//Print(TaskDialog.ShowEx(buttons: "One|Two|50 Three|51Four", flags: TDFlags.CommandLinks));
	}

	static void TestAccWeb()
	{
		Debug_.TextPrefix = "<><Z 0xffff>"; Debug_.TextSuffix = "</Z>";

		//Print(Ver.Is64BitProcess);
		//var w = Wnd.Find("*Mozilla Firefox");
		var w = Wnd.Find("*Google Chrome");
		//var w = Wnd.Find("*Internet Explorer");

#if true
		//using(var a = Acc.Find(w, "web:LINK", also: o => o.Match("href", "*forum*")).OrThrow()) {
		//using(var a = Acc.Find(w, "web:LINK", "\0 a:href=*forum*").OrThrow()) {
		//using(var a = Acc.Find(w, "web:LINK", "name=P*\0 a:href=*forum*").OrThrow()) {
		using(var a = Acc.Find(w, "web:LINK", "name=P*\0 a:href=**r|forum").OrThrow()) {
			Print(a);
		}
#elif false
		var a = Acc.Find(w, "web:LINK", "Board index").OrThrow();
		//var a = Acc.Find(w, "web:LISTITEM", "FAQ").OrThrow();
		//var a = Acc.Find(w, "web:PUSHBUTTON", "Search").OrThrow();
		//var a = Acc.Find(w, "web:TEXT", "Search for keywords").OrThrow();
		//var a = Acc.Find(w, "web:LIST").OrThrow();
		Print(a);
		string attr;
		attr = "href";
		//attr = "HREF";
		//attr = "class";
		//attr = "type";
		//attr = "value";
		//attr = "id";
		//attr = "ID";
		//attr = "name";
		//attr = "title";
		//attr = "onclick";

		Perf.First();
		for(int i = 0; i < 5; i++) {
			//var s = a.HtmlAttribute(attr, interpolated: false);
			var s = a.HtmlAttribute(attr, AccBrowser.InternetExplorer);
			//var s = a.HtmlAttributes();
			//var s = a.Html(true);
			Perf.Next();
			Print(s);
		}
		Perf.Write();

		//Print(a.HtmlAttributes());
		a.Dispose();
#else
		//var a = Acc.FromWindow(w, AccOBJID.CLIENT);
		//Perf.First();
		var a = Acc.Find(w, "web:").OrThrow();
		//var a = Acc.Find(w, "web:", flags: AFFlags.WebBusy).OrThrow();
		//Perf.NW(); return;

		//Print(a.Html(true)); return;

		//a.EnumChildren(true, o =>
		//{
		//	if(o.IsInvisible) return;
		//	Print("<><c 0xE00000>" + o.ToString(o.Level) + "</c>");

		//	//var d = o.HtmlAttributes();
		//	//if(d.Count > 0) {
		//	//	if(d.Count >= 16) Print("<><Z 0xff00>" + d.Count + "</Z>");
		//	//	Print(d);
		//	//}

		//	Print(o.Html(true));
		//});

		Perf.First();
		var t = a.Children(true);
		Perf.Next();
		foreach(var k in t) {
			//var d = k.HtmlAttributes();
			//if(d.Count > 15) Print(d.Count);

			var p = new Perf.Inst(true);
			var s = k.HtmlAttribute("href");
			p.NW();
			if(!Empty(s)) Print(s);

			//if(k.Match("a:href", "*forum*")) {
			if(k.Match("a:class", "forumtitle", "a:href", "*forum*")) {
				//Print(k);
				Print(k.HtmlAttributes());
			}

			//Print(k);
			k.Dispose();
		}
		Perf.NW();
		a.Dispose();
#endif
	}

	static void TestVariantToString()
	{
		VARIANT v = default;
		//v.vt = Api.VARENUM.VT_I4; v.value = 5;
		//v.vt = Api.VARENUM.VT_I8; v.value = 12345678912345;
		v.vt = Api.VARENUM.VT_BOOL; v.value = -1;
		Print(v.ToStringAndDispose());
	}

	static void TestDebugPrintColor()
	{
		Debug_.TextPrefix = "<><c 0xff0000>"; Debug_.TextSuffix = "</c>";
		Debug_.Print("Blue");
	}

	static void TestSegmentChangeProps()
	{
		string g;
		//g = "one ,, , two";
		g = "one,two";
		foreach(var s in g.Segments_(",")) {
			//s.TrimStart();
			//s.TrimEnd();
			//s.Trim();
			//s.Length++;
			//s.LengthAdd(1);
			//s.LengthSub(1);
			//s.LengthSet(1);
			//var s2 = s.Subsegment(s.Offset, s.Length - 1);
			//s.EndAdd(1);
			//s.EndSub(1);
			//s.EndSet(5);
			//s.OffsetAdd(1);
			//if(s.Offset>0) s.OffsetSub(1);
			s.OffsetSet(2);

			Print($"'{s}' {s.Offset} {s.Length}");
		}
	}

	static void Test1()
	{
		var w = Wnd.Find("Quick*");

		//var cf = new Wnd.ChildFinder("name");
		//var a = Acc.Find(w, "role", controls:cf);

		var af = new Acc.Finder("role");
		w.Child("name", also: o => af.FindIn(o));
		var b = af.Result;

		var k = Acc.Find(w.Child("name"), "role");

		//var a1 = Acc.Find(w, "role", waitS: 5);
		//var a2 = Acc.WaitFor(5, w, "role");
	}

	static void TestAccOpenLibreOffice()
	{
		//int r = 0;
		//Api.SystemParametersInfo(Api.SPI_GETSCREENREADER, 0, &r, 0);
		//Print(r);
		//Api.SystemParametersInfo(Api.SPI_SETSCREENREADER, 0, 0, 0);
		////Api.SystemParametersInfo(Api.SPI_SETSCREENREADER, 1, 0, 0);
		////Api.SystemParametersInfo(Api.SPI_SETSCREENREADER, 0, 0, 0);
		//Api.SystemParametersInfo(Api.SPI_GETSCREENREADER, 0, &r, 0);
		//Print(r);

		//return;

		//var w = Wnd.Find("*LibreOffice *", "SALFRAME").OrThrow();
		var w = Wnd.Find("*OpenOffice *", "SALFRAME").OrThrow();
		Print(w);
		//using(var a= Acc.FromWindow(w, AccOBJID.CLIENT)) {
		//	//Print(a.Children(true));
		//	Print(a);
		//	Print(a.ChildCount);
		//	//Print(a.Navigate("first"));
		//}
		//using(var a = Acc.Find(w, "PUSHBUTTON", "Paste*").OrThrow()) {
		//	Print(a);
		//}
		using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
			a.EnumChildren(true, o =>
			{
				//PrintList(o.Level, o.RoleString);
				Print(o);
				//100.ms();
			});
		}
		Print("END");
	}

	static void TestAccVariousApps()
	{
		//var w = Wnd.Find("VLC leistuvė", "QWidget").OrThrow();
		//Print(w);
		//using(var a = Acc.Find(w, "CHECKBUTTON", "\0 description=Maišymo veiksena").OrThrow()) Print(a);

		//var w = Wnd.Find("Welcome Guide — Atom", "Chrome_WidgetWin_1").OrThrow();
		//Print(w);
		//using(var a = Acc.Find(w, "web:PUSHBUTTON", "*Learn Keyboard Shortcuts").OrThrow()) Print(a);

		//Acc.Misc.MaxChildren *= 2;
		var w = Wnd.Find("Quick*");
		using(var a = Acc.FromWindow(w)) {
			a.EnumChildren(true, o =>
			 {
				 Print(o);
			 });
		}
	}

	static void TestWndKid()
	{
		var w = Wnd.Find("Options");
		//Print(w.Kid(1103));

		//_PrintMemory();
		//for(int i = 0; i < 10000; i++) w.Kid(1103);
		//_PrintMemory();
		//for(int i = 0; i < 10000; i++) w.Kid(1103);
		//_PrintMemory();
		//for(int i = 0; i < 100000; i++) { object k = 5; }
		//_PrintMemory();

		//Print(Acc.Find(w, "class=button:PUSHBUTTON"));
		//Print(Acc.Find(w, "id=1103:"));
		//Print(Acc.Find(w, "class=button:CHECKBUTTON"));
		//Print(Acc.Find(Wnd.Find("* Internet Explorer"), "ie:LINK"));

		w = Wnd.Find("* Internet Explorer");
		//w = Wnd.Find("Settings");
		var f = new Acc.Finder("rrrrr");
		_PrintMemory();
		for(int j = 0; j < 1; j++) {
			Perf.First();
			for(int i = 0; i < 10; i++) f.FindIn(w);
			Perf.NW();
			_PrintMemory();
		}
		Print(f.Result);
	}

	static void TestWinFlags()
	{
		//var w = Wnd.Find("Quick*");
		//Wnd.Lib.WinFlags.Set(w, Wnd.Lib.WFlags.Test);
		//Print(Wnd.Lib.WinFlags.Get(w));

		//Perf.SpinCPU(100);
		//var a1 = new Action(() => { Wnd.Lib.WinFlags.Set(w, Wnd.Lib.WFlags.Test); });
		//var a2 = new Action(() => { Wnd.Lib.WinFlags.Get(w); });
		//var a3 = new Action(() => { w.Prop.Set("hdhdhdh", 1); });
		//var a4 = new Action(() => { var k=w.Prop["hdhdhdh"]; });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);

		//Print(w.Prop);

	}

	static void TestAccPreferLink()
	{
		//using(var a = Acc.FromXY(1150, 1469, preferLINK: true)) Print(a);
		for(int i = 0; i < 30; i++) {
			1.s();
			using(var a = Acc.FromMouse(preferLINK: true)) {
				Print(a);
			}
		}
	}

	static void TestAccFromXY()
	{
		Wnd w;
		w = Wnd.Find("Quick*", "QM_Editor");
		//w = Wnd.Find("* Internet Explorer");
		//w = Wnd.Find("* Mozilla Firefox");
		w = Wnd.Find("* Google Chrome");
		//w = Wnd.Find("* OpenOffice*");

		//using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
		//	Print(a);
		//	using(var b = a.ChildFromXY(65, 65, screenCoord: false)) {
		//		Print(b);
		//	}
		//}
		int x, y;
		//x = 65; y = 65;
		//x = 277; y = 536;
		//x = 65; y = -10;
		//x = -1; y = 0;
		//x = 94; y = 365;
		//x = 60; y = 32;
		//x = 428; y = 337;
		x = 77; y = 178;
		//x = 516; y = 39;
		using(var a = Acc.FromXY(w, x, y)) {
			//using(var a = Acc.FromXY(1160, 1467)) {
			//using(var a=Acc.FromXY(w, 65, 65)) {
			//using(var a=Acc.FromXY(w, 277, 536)) {
			//using(var a=Acc.FromXY(w, 65, -10)) {
			//using(var a = Acc.FromXY(w, -1, 0)) {
			Print(a);
			//Print(a.Navigate("pa"));
		}
	}

	static void TestAccFromFocus()
	{
		Wnd w;
		w = Wnd.Find("Quick*", "QM_Editor");
		//w = Wnd.Find("* Internet Explorer");
		//w = Wnd.Find("* Mozilla Firefox");
		//w = Wnd.Find("* Google Chrome");
		//w = Wnd.Find("* OpenOffice*");
		//w = Wnd.Find("Options");
		//w = Wnd.Find("Settings");
		//w = Wnd.Find("catkeys - Q*", "WindowsForms*");
		w.Activate();

		//using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
		//	Print(a);
		//	using(var b = a.ChildFromXY(65, 65, screenCoord: false)) {
		//		Print(b);
		//	}
		//}
		using(var a = Acc.Focused()) {
			Print(a);
			//Print(a.Navigate("pa"));
		}
	}

	static void TestAccFromEvent()
	{
		AccEVENT ev = AccEVENT.OBJECT_FOCUS;
		var hh = SetWinEventHook(ev, ev, default, _testWinEventProc, 0, 0, 0);
		//Print(hh);
		TaskDialog.Show();
		//MessageBox.Show("");
		//new Form().ShowDialog();
		//Time.SleepDoEvents(10000);
		UnhookWinEvent(hh);
		Print("END");
	}

	static WINEVENTPROC _testWinEventProc = _TestWinEventProc;
	static void _TestWinEventProc(IntPtr hHook, AccEVENT event_, Wnd w, int idObject, int idChild, int idThread, int eventTime)
	{
		//Print(w);
		using(var a = Acc.FromEvent(w, idObject, idChild)) {
			if(a == null) Print(Native.GetErrorMessage());
			else Print(a);
		}
	}

	internal delegate void WINEVENTPROC(IntPtr hHook, AccEVENT event_, Wnd w, int idObject, int idChild, int idThread, int eventTime);

	[DllImport("user32.dll")]
	internal static extern IntPtr SetWinEventHook(AccEVENT eventMin, AccEVENT eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

	[DllImport("user32.dll")]
	internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

	static void TestAccFromComObject()
	{
		Wnd w = Wnd.Find("* Internet Explorer").Child(null, "Internet Explorer_Server");
		Print(w);
		var res = w.Send(WM_HTML_GETOBJECT);
		var guid = typeof(MSHTML.IHTMLDocument2).GUID;
		if(0 != ObjectFromLresult(res, ref guid, 0, out IntPtr ip)) return;

		var doc = Marshal.GetTypedObjectForIUnknown(ip, typeof(MSHTML.IHTMLDocument2)) as MSHTML.IHTMLDocument2;
		var body = doc.body;
		//Print(body.outerHTML);
		Print(Acc.FromComObject(body));

		Marshal.ReleaseComObject(body);
		Marshal.ReleaseComObject(doc);
		Print(Marshal.Release(ip));
	}
	static uint WM_HTML_GETOBJECT = Api.RegisterWindowMessage("WM_HTML_GETOBJECT");

	[DllImport("oleacc.dll", PreserveSig = true)]
	internal static extern int ObjectFromLresult(LPARAM lResult, ref Guid riid, LPARAM wParam, out IntPtr ppvObject);

	#region UIA old

	//static void TestUIA_managed()
	//{
	//	//System.Windows.Automation is very slow (finds 4-10 times slower). Uses deprecated API (not COM).

	//	//var w = Wnd.Find("* Internet Explorer").OrThrow();
	//	//var w = Wnd.Find("* Google Chrome").OrThrow();
	//	var w = Wnd.Find("* Mozilla Firefox").OrThrow();

	//	//var er = AutomationElement.FromHandle(w.Handle);
	//	//Perf.First();
	//	//AutomationElement e = null;
	//	//for(int i = 0; i < 5; i++) {
	//	//	e =er.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "General"));
	//	//	Perf.Next();
	//	//}
	//	//Perf.Write();
	//	//Print(e.Current.Name);
	//}

	//class Acc3
	//{
	//	[MethodImpl(MethodImplOptions.NoInlining)]
	//	public static int Func1()
	//	{
	//		return 1;
	//	}

	//	[MethodImpl(MethodImplOptions.NoInlining)]
	//	public static UIA.IUIAutomation Func2()
	//	{
	//		return _lazy.Value;
	//	}

	//	//static CUIAutomation _au = new CUIAutomation();
	//	//static CUIAutomation _au = _Create();
	//	//static Lazy<CUIAutomation> _lazy = new Lazy<CUIAutomation>(_Create, true);
	//	static Lazy<UIA.IUIAutomation> _lazy = new Lazy<UIA.IUIAutomation>(()=> new UIA.CUIAutomation() as UIA.IUIAutomation, true);

	//	[MethodImpl(MethodImplOptions.NoInlining)]
	//	static UIA.IUIAutomation _Create()
	//	{
	//		Print("create");
	//		return new UIA.CUIAutomation() as UIA.IUIAutomation;
	//	}
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static void TestUIA_COM()
	//{
	//	Perf.Next();
	//	//var w = Wnd.Find("* Internet Explorer").OrThrow();
	//	var w = Wnd.FindFast(null, "ieframe").OrThrow();
	//	//w = w.Child(null, "Internet Explorer_Server").OrThrow();
	//	//var w = Wnd.Find("* Google Chrome").OrThrow();
	//	//var w = Wnd.Find("* Mozilla Firefox").OrThrow();

	//	//Print(2);
	//	Perf.Next();
	//	var m = Acc3.Func1();
	//	//Print(3);
	//	Perf.Next();
	//	Print("Uia");
	//	//var x = new CUIAutomation();
	//	var x = Acc3.Func2();
	//	//Print(4);
	//	Perf.Next();
	//	var ew = x.ElementFromHandle(w);
	//	Perf.NW();
	//	Print(ew.Name);
	//	var e =ew.FindFirst(UIA.TreeScope.Descendants, x.CreatePropertyCondition(UIA.PropertyId.Name, "General"));
	//	Print(e.Name);
	//	Print(Acc.FromComObject(e));
	//	//Print("Acc");
	//	//Print(Acc.FromWindow(w).Name);
	//	//Print(Acc.Find(w, "LINK", "General").Name);

	//	////var t=new Thread(() =>
	//	////{
	//	////	//var x2 = Acc3.Func2();
	//	////	var x2 = new CUIAutomation();
	//	////	var w2 = Wnd.FindFast(null, "ieframe").OrThrow();
	//	////	var e2 = x2.ElementFromHandle(w2.Handle);
	//	////	Print(e2.Name);
	//	////	Print(Acc.FromWindow(w2).Name);
	//	////});
	//	////t.Start();

	//	//Task.Run(() =>
	//	//{
	//	//	Print("thread Uia");
	//	//	var x2 = Acc3.Func2();
	//	//	//var x2 = new CUIAutomation();
	//	//	var w2 = Wnd.FindFast(null, "ieframe").OrThrow();
	//	//	var e2 = x2.ElementFromHandle(w2.Handle);
	//	//	Print(e2.Name);
	//	//	Print(ew.FindFirst(TreeScope.TreeScope_Descendants, x.CreatePropertyCondition(30005, "General")).Name);
	//	//	Print("thread Acc");
	//	//	Print(Acc.FromWindow(w2).Name);
	//	//	Print(Acc.Find(w, "LINK", "General").Name);
	//	//});

	//	//1.s();

	//	//for(int j= 0; j < 5; j++) {
	//	//	Perf.First();
	//	//	var x = new CUIAutomation();
	//	//	Perf.Next();
	//	//	var ew = x.ElementFromHandle(w.Handle);
	//	//	//Print(ew.Name);
	//	//	Perf.Next();
	//	//	IUIAutomationElement e = null;
	//	//	for(int i = 0; i < 1; i++) {
	//	//		var cond = x.CreatePropertyCondition(30005, "General"); //UIA_PropertyIds.UIA_NamePropertyId
	//	//		e = ew.FindFirst(TreeScope.TreeScope_Descendants, cond);
	//	//		Perf.Next();
	//	//	}
	//	//	Perf.NW();
	//	//	Print(e.Name);
	//	//}
	//}

	//static void TestWhite()
	//{
	//	//White uses the slow and obsolete .NET classes that wrap deprecated API. It seems it's possible to use COM, but not easy.
	//	//Also does not have an easy way to find descendants.

	//	//var app=TestStack.White.Application.Attach("iexplore");
	//	var app=TestStack.White.Application.Attach(11168);
	//	//Print(app.Name);
	//	//foreach(var t in app.GetWindows()) {
	//	//	Print(t.Name);
	//	//}

	//	//var w = app.GetWindow("Quick Macros Forum • Index page - Internet Explorer");
	//	//Print(w.Name);

	//	//Print(TestStack.White.Desktop.Instance.Windows());
	//	//var w=TestStack.White.Desktop.Instance.Get<TestStack.White.UIItems.WindowItems.Window>("Quick Macros Forum • Index page - Internet Explorer"); //error
	//	var w = app.GetWindow("Quick Macros Forum • Index page - Internet Explorer");
	//	Print(w);

	//	//w.LogStructure();
	//	//var link = w.GetElement(TestStack.White.UIItems.Finders.SearchCriteria.ByText("General")); //error
	//	//var link = w.Get(TestStack.White.UIItems.Finders.SearchCriteria.ByText("General")); //fails (does not search descendants)
	//	//var f=new TestStack.White.AutomationElementSearch.DescendantFinder()

	//	Print("END");
	//}

	//static void TestFlaUI()
	//{
	//	//Reviewed FlaUI code. It does not add something new and probably does not make simpler. Use only as a UIA sample.
	//}

	//	static void TestMainWindowsWithUIA()
	//	{
	//		//Print(Wnd.Misc.MainWindows()); return;

	//		//MSAA does not filter windows.
	//		//using(var desktop = Acc.FromWindow(Wnd.Misc.WndRoot, AccOBJID.CLIENT)) {
	//		//	//Print(desktop.Children(false));
	//		//	desktop.Find(flags: AFFlags.DirectChild, also: o => { Print(o); return false; });
	//		//}

	//		//var x = new CUIAutomation();
	//		////var ca = x.CreateCacheRequest(); Print(x.GetRootElementBuildCache(ca).GetCachedChildren().Length); //0
	//		////var a=x.GetRootElement().FindAll(TreeScope.TreeScope_Children, x.RawViewCondition);
	//		//var a=x.GetRootElement().FindAll(TreeScope.TreeScope_Children, x.CreatePropertyCondition(30003, 0xC370)); //UIA_ControlTypePropertyId, UIA_WindowControlTypeId
	//		//PrintList("window", a.Length);
	//		////for(int i = 0; i < a.Length; i++) PrintList(a.GetElement(i).ClassName, a.GetElement(i).Name);
	//		//for(int i = 0; i < a.Length; i++) {
	//		//	var e =a.GetElement(i);
	//		//	Print((Wnd)e.NativeWindowHandle);
	//		//	//PrintList(e.ClassName, e.Name);
	//		//}
	//		//a=x.GetRootElement().FindAll(TreeScope.TreeScope_Children, x.CreatePropertyCondition(30003, 0xC371)); //UIA_ControlTypePropertyId, UIA_PaneControlTypeId
	//		//PrintList("pane", a.Length);
	//		//for(int i = 0; i < a.Length; i++) {
	//		//	var e = a.GetElement(i);
	//		//	Print((Wnd)e.NativeWindowHandle);
	//		//	//PrintList(e.ClassName, e.Name);
	//		//}


	//		var k = new List<Wnd>();
	//		Perf.First();
	//		for(int j=0; j<5; j++) {
	//#if false
	//			k=Wnd.Misc.MainWindows().ToList();
	//#else
	//			k.Clear();
	//			var x = Acc3.Func2();
	//			var a=x.GetRootElement().FindAll(UIA.TreeScope.Children, x.CreatePropertyCondition(UIA.PropertyId.TypeId, UIA.TypeId.Window));
	//			for(int i = 0; i < a.Length; i++) k.Add(a.GetElement(i).NativeWindowHandle);
	//#endif
	//			Perf.Next();
	//		}
	//		Perf.Write();
	//		Print(k);
	//	}

	#endregion

	static void TestAccMiscMethods()
	{
		//var w = Wnd.FromMouse();
		//w.MouseClick();
		//return;

		//var w = Wnd.Find("Options");
		//w.Activate();
		Output.Clear();
		//using(var a = Acc.Find(w, null, "Mouse").OrThrow()) {
		//using(var a = Acc.Find(w, "CHECKBUTTON", "Mouse").OrThrow()) {
		//using(var a = Acc.Find(w, nameof(AccROLE.CHECKBUTTON), "Mouse").OrThrow()) {
		//5.s();
		for(int i = 0; i < 1; i++) {
			Perf.First();
			//using(var a = Acc.FromMouse(preferLINK: true)) {
			using(var a = Acc.FromMouse()) {
				//Print(a);
				//a.Select(AccSELFLAG.TAKESELECTION);
				//a.Select(AccSELFLAG.ADDSELECTION);
				//a.Select(AccSELFLAG.EXTENDSELECTION);
				//a.Select(AccSELFLAG.REMOVESELECTION);
				//a.Focus();
				//Print(a.DefaultAction);
				//a.DoDefaultAction();
				//Print(a.Rect);
				//a.MouseMove();
				//a.MouseClick();
				//a.MouseClick(1, 1, MButton.Right);

				Perf.Next();
				var w1 = a.WndContainer;
				Perf.Next();
				using(var ap = a.Navigate("pa")) {
					Perf.Next();
					var w2 = ap.WndContainer;
					Perf.NW();

					//Print(a);
					//Print(ap);
					PrintList(!w1.Is0, !w2.Is0);
				}
				if(i == 0) Perf.SpinCPU(100);
			}
		}
		//100.ms();
		//Print(Wnd.WndActive);
	}

	static void TestAccBstrWithNullChars()
	{
		int n = 0;
		Acc.FromWindow(Wnd.Misc.WndRoot).EnumChildren(true, a =>
		{
			var s = a.ToString();
			n++;
		});
		Print(n);
	}

	static void TestWndFocus()
	{
		//var f = new Form();
		//var b = new Button();
		//var c = new ComboBox();
		//c.Location = new Point(0, 100);
		////c.Enabled = false;

		//f.Controls.Add(b);
		//f.Controls.Add(c);
		//f.Click += (unu, sed) =>
		//{
		//	//Wnd.Find("Options").ActivateLL();
		//	//Time.SleepDoEvents(200);
		//	//Api.SetFocus(default);
		//	//Print(Api.GetFocus());
		//	var w = (Wnd)c;
		//	//c.Dispose();
		//	//Print(w);
		//	//w.Focus();
		//	Native.ClearError();
		//	bool ok = w.FocusLocal();
		//	PrintList(ok, ok?"":Native.GetErrorMessage());
		//	//Print(w.IsFocused);
		//};
		//f.ShowDialog();

		var w = Wnd.Find("Options").Kid(3089);
		//Print(w.FocusLocal());
		w.Focus();
	}

	static void TestManifest_disableWindowFiltering()
	{
		//var a = Wnd.Misc.AllWindows();
		//Print(a);
		//Print(a.Length);
		Print(Wnd.Find("Search", "Windows.UI.Core.CoreWindow"));
	}

	//static void TestIAccessible2()
	//{
	//	var w = Wnd.Find("* Mozilla Firefox");
	//	using(var a = Acc.Find(w, "web:")) {
	//		Print(a);
	//		int hr = Api.IUnknown_QueryService(a.LibIAccessibleDebug, ref IID_IAccessible, ref IID_IAccessible2, out var ip);
	//		PrintHex(hr);
	//		Print(ip);
	//	}
	//}
	//internal static Guid IID_IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
	//internal static Guid IID_IAccessible2 = new Guid(0xE89F726E, 0xC4F4, 0x4c19, 0xBB, 0x19, 0xB6, 0x47, 0xD7, 0xFA, 0x84, 0x78);

	static void TestAccWebPageProp()
	{
		//var w = Wnd.Find("* Mozilla Firefox");
		var w = Wnd.Find("* Google Chrome");
		//var w = Wnd.Find("* Internet Explorer");
		//var w = Wnd.Find("FileZilla");
		//var w = Wnd.Find("Catkeys - Microsoft Visual Studio ", "HwndWrapper[DefaultDomain;*");
		//using(var a = Acc.Find(w, "web:")) {
		//using(var a = Acc.Find(w, "web:LINK", "\0 a:href=*forum*")) {
		using(var a = Acc.Find(w, "web:LINK", "Programming", AFFlags.HiddenToo).OrThrow()) {
			//using(var a = Acc.Find(w, "OUTLINEITEM", "Other Bookmarks", AFFlags.HiddenToo).OrThrow()) { //no scroll
			//using(var a = Acc.Find(w, "OUTLINEITEM", "Temp", AFFlags.HiddenToo).OrThrow()) { //no scroll
			//using(var a = Acc.Find(w, "OUTLINEITEM", "Structs.cs", AFFlags.HiddenToo).OrThrow()) { //no scroll
			Print(a);

			//Print(a.Name);
			//Print(a.Value);

			//var x = a;
			////var x = a.Navigate("first");
			////Print(x.WebPage.URL);
			//Print(x.Html(true));

			a.ScrollTo(); //works with Firefox, Chrome, IE. Not with .
		}
	}

	static void TestAccSelectedChildren()
	{
		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		//var w = Wnd.Find("Options").OrThrow();
		//var w = Wnd.Find("*Mozilla Firefox").OrThrow();
		//var w = Wnd.Find("*Google Chrome").OrThrow();
		var w = Wnd.Find("*Internet Explorer").OrThrow();
		//using(var a=Acc.Find(w, "OUTLINE").OrThrow()) {
		//using(var a=Acc.Find(w, "LIST").OrThrow()) {
		//using(var a=Acc.Find(w, "LIST", also:o=>++o.Counter==2).OrThrow()) {
		//using(var a=Acc.Find(w, "web:LIST", "\0 a:name=cars").OrThrow()) {
		//using(var a=Acc.Find(w, "web:COMBOBOX", "\0 a:name=cars", navig:"first").OrThrow()) {
		using(var a = Acc.Find(w, "web:ALERT", "\0 a:name=cars", navig: "child3").OrThrow()) { //IE
			Print(a);
			Print("---- selected children ----");
			Print(a.SelectedChildren);
		}
	}

	static void TestChromeEnableAcc()
	{
		var w = Wnd.Find("* Google Chrome").OrThrow();
		w.Activate();

		//w.GetWindowAndClientRectInScreen(out var rw, out var r);
		//Print(Acc.FromXY(w, -1, 0));
		//Print(Acc.FromXY(w, 1, 1));
		//Print(Acc.FromXY(w, 88, 178));

		//for(int i = 0; i < 10; i++) {
		//	1.s();
		//	using(var a = Acc.FromMouse(preferLINK: true)) {
		//		Print(a);
		//	}
		//}

		Print(Acc.Focused());
	}

	static void TestJava()
	{
		var w = Wnd.Find("Java Control Panel").OrThrow();
		w.Activate();

		//Print(Acc.Find(w, "java:push button", "Netw*"));

		//using(var a = Acc.FromXY(1440, 1308)) {
		//	Print(a);
		//}

		for(int i = 0; i < 10; i++) {
			1.s();
			using(var a = Acc.FromMouse(preferLINK: true)) {
				Print(a);
			}
		}

		//Print(Acc.Focused());
	}

	static void TestAccInOtherFunctions()
	{
		//Acc.Find(Wnd.Find("Quick*"), "PUSHBUTTON", "Options*").MouseClick();

		//var w = Wnd.Find("**c|*Firefox").OrThrow();
		//Print(w);
		//var a = Acc.Find(w, "div", "Google").OrThrow();
		////a.MouseMove();
		//Perf.First();
		//for(int i = 0; i < 5; i++) {
		//	var si = ScreenImage.Find(@"Q:\app\Catkeys\Tests\Images\google.bmp", a, SIFlags.WindowDC).OrThrow();
		//	Perf.Next();
		//}
		//Perf.Write();
		////si.MouseMove();

		//var w = Wnd.Find("Options*").Kid(1571);
		//Print(w.NameAcc);

		//var w = Wnd.Find("Options*").Child("**accName:Run as", "combo*").OrThrow();
		//Print(w);

		//var w = Wnd.Find("Options*");
		//var af = new Acc.Finder("COMBOBOX", "Run as");
		//Print(w.HasAcc(af));

		//var w = Wnd.Find("Options*").Kid(1099);
		//w.AsButton.Click(true);
	}

	static void TestAccTODO()
	{
		var w = Wnd.Find("Quick*");
		Acc a = null;
		Perf.First();
		for(int i = 0; i < 9; i++) {
			a = Acc.Find(w, "LISTITEM", "ngen");
			Perf.Next();
		}
		Perf.Write();
		Print(a);
	}

	static void TestAccSkipAndWait()
	{
		var w = Wnd.Find("Options");
		//var a = Acc.Find(w, "CHECKBUTTON", skip:2).OrThrow();
		//var a = Acc.WaitFor(0, w, "CHECKBUTTON", "Mouse").OrThrow();
		var f = new Acc.Finder("CHECKBUTTON", "Mouse");
		Print(f.WaitIn(-2, w));
		var a = f.Result;
		Print(a);
	}

	static void TestAccFromUIA()
	{
		var w = Wnd.Find("*Firefox");
		Print(w);
		var a = Acc.Find(w, "web:LINK", "Programming").OrThrow();
		Print(a);
		var e1 = AElement.FromAcc(a);

		var e = e1 as UIA.IElement4;

		Print(e.Name);
		var a2 = Acc.FromComObject(e);
		Print(a2);

	}

	static void TestUiaSpeed()
	{
		Wnd w = Wnd.Find("*a Firefox").OrThrow();
		for(int i = 0; i < 5; i++) {
			Perf.First();
			var a = Acc.Find(w, "LINK", "Untitled").OrThrow();
			Perf.NW();
			Print(a.Name);
			a.Dispose();
			1.s();
		}
	}

	static void TestUia2()
	{
		//ComMemberType mt = 0;
		//var m = Marshal.GetMethodInfoForComSlot(typeof(U2.IUIAutomationProxyFactoryEntry), 5, ref mt);
		//Print(m);
		//Print(mt);


		//Wnd w = Wnd.Find("* Firefox").OrThrow();
		//Perf.First();
		//var u = new UIA.CUIAutomation8() as UIA.IUIAutomation2;
		//Perf.Next();
		//var ew = u.ElementFromHandle(w);
		//Perf.Next();
		//string s1 = ew.Name, s2 = null;
		//for(int i = 0; i < 5; i++) {
		//	var cond = u.CreatePropertyCondition(UIA.PropertyId.Name, "Untitled");
		//	var e = ew.FindFirst(UIA.TreeScope.Descendants, cond);
		//	Perf.Next();
		//	s2 = e.Name;
		//}
		//Perf.Write();
		//Print(s1);
		//Print(s2);

		//Wnd w = Wnd.Find("*- Notepad").OrThrow();
		//var ew = AElement.Factory.ElementFromHandle(w);
		//var e = ew.FindFirst(UIA.TreeScope.Descendants, new ACondition().Name("Edit").Type(UIA.TypeId.MenuItem).Offscreen(false).Condition);
		//Print(e.Name);
	}

	static void TestAElementFromAcc()
	{
		var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
		var a = Acc.Find(w, "web:LINK", "General").OrThrow();
		Print(a);
		var e1 = AElement.FromAcc(a, true);
		Print(AElement.LibToString_(e1));
	}

	static void TestAElementFind()
	{
		//var w = WaitFor.WindowActive(15, "* Internet Explorer", "Windows.UI.Core.CoreWindow");
		////var w = Wnd.Find("* Internet Explorer", "Windows.UI.Core.CoreWindow").OrThrow();
		//Print(w);
		////w.Activate();
		//1.s();

		//Acc.Find()
		var w = Wnd.Find("* Mozilla Firefox").OrThrow();
		//var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, Api.s_IES).OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").OrThrow();
		//var w = Wnd.Find("* Opera").OrThrow();
		//var w = Wnd.Find("Options").OrThrow();
		//var w = Wnd.Find("*- OpenOffice Writer").OrThrow();
		Print(w);
		//w.Activate(); 100.ms();

		//var ew = AElement.FromWindow(w);
		//var a = ew.FindAll(UIA.TreeScope.Descendants, new ACondition().Type(UIA.TypeId.Hyperlink).Name("RSS").Condition);
		//Print(a.Length);

		//UIA.IElement e = null;
		//for(int i = 0; i < 5; i++) {
		//	//if(i > 0) {
		//	//	e = null;
		//	//	GC.Collect();
		//	//	GC.WaitForPendingFinalizers();
		//	//	100.ms();
		//	//}
		//	Perf.First();
		//	e = AElement.FindCurrentWebPage(w);
		//	Perf.NW();
		//	100.ms();
		//}
		//PrintList(e != null, e?.Name);

		AElement.s_testMethod = AElement.TestMethod.Find;
		//AElement.s_testMethod = AElement.TestMethod.FindAll;
		//AElement.s_testMethod = AElement.TestMethod.FindAllCache;
		//AElement.s_testMethod = AElement.TestMethod.Walker;
		//AElement.s_testMethod = AElement.TestMethod.WalkerCache;
		UIA.IElement e = null;
		//Acc e = null;
		for(int i = 0; i < 7; i++) {
			100.ms();
			Perf.First();
			//e = Acc.Find(w, "LINK", "Untitled"); //FF 300, Chr 230
			//e = Acc.Find(w, "web:LINK", "Untitled"); //FF 190, Chr 230
			//e = Acc.Find(w, "web:LINK", "Untitled-"); //FF 230, Chr 300

			//e = AElement.Find(w, "Untitled");
			//e = AElement.Find(w, "Untitled", UIA.TypeId.Hyperlink); //FF 260, Chr 60
			//e = AElement.Find(w, "Untitled-", UIA.TypeId.Hyperlink); //FF 390, Chr 130
			e = AElement.WebFind(w, "Untitled", UIA.TypeId.Hyperlink); //FF 100, Chr 60
			//e = AElement.WebFind(w, "SHOW MORE", UIA.TypeId.Button); //FF 100, Chr 60
																	   //e = AElement.WebFind(w, "Untitled-", UIA.TypeId.Hyperlink); //FF 110, Chr 70

			//e = AElement.WebFind(w, "partial", UIA.TypeId.Hyperlink);
			//e = AElement.WebFind(w, "Sheppy", UIA.TypeId.Hyperlink);
			//e = AElement.Find(w, "Advanced search", UIA.TypeId.Hyperlink);
			//e = AElement.WebFind(w, "Advanced search", UIA.TypeId.Hyperlink);
			//e = AElement.Find(w, null, UIA.TypeId.Pane);
			//e = AElement.Find(w, "Debug...");
			//e = AElement.Find(w, "Debug...", UIA.TypeId.Button);
			//e = AElement.Find(w, "Bold");
			Perf.NW();
			PrintList(e != null, e?.Name);
			//if(e != null) break;
		}

		//Wnd.Find(null, "QM_Editor").Activate();

		new ACondition().Add(UIA.PropertyId.AutomationId, "hhhh").AddOr(UIA.PropertyId.ClassName, "cn1", "cn2");
	}

	static void TestAElementFromPoint()
	{
		for(; !Input.IsCtrl; 1.s()) {
			UIA.IElement e = null;
			try { e = AElement.FromMouse(true); } catch(Exception ex) { Print(ex); }

			//var p = Mouse.XY;
			//var w = Wnd.FromXY(p, WXYFlags.NeedWindow);
			//var e = AElement.FromPoint(w, p);

			Print(e.LibToString_());
		}
	}

	static void TestDllInProc()
	{
		var w = Wnd.Find(null, "MozillaWindowClass").OrThrow();
		//var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, Api.s_IES).OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, "TabWindowClass").OrThrow().WndDirectParent;
		//var w = Wnd.Find("* Internet Explorer").Child(null, "Shell DocObject View").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").Child(null, "Windows.UI.Core.CoreWindow").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").Child(null, "ApplicationFrameInputSinkWindow").OrThrow();
		//var w = Wnd.Find("* Opera").OrThrow();
		//var w = Wnd.Find("Options").OrThrow();
		//var w = Wnd.Find("ipc_server").OrThrow();
		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		//var w = Wnd.Find(null, "QM_Editor").Kid(2051).OrThrow();
		//var w = Wnd.Find("QM Dialog").OrThrow();
		//var w = Wnd.Find("Catkeys - Q*").OrThrow();
		//var w = Wnd.Find("Calculator").OrThrow();
		//var w = Wnd.Find("Calculator").Child(null, "Windows.UI.Core.CoreWindow").OrThrow();
		//var w = Wnd.Find("ILSpy").OrThrow();
		//var w = Wnd.Find("Java *").OrThrow();
		//var w = Wnd.Find("* Sandcastle *").OrThrow();
		//var w = Wnd.Find("* C# Converter *").OrThrow();
		//var w = Wnd.Find("* OpenOffice *").OrThrow();
		//var w = Wnd.Find("* - Notepad").OrThrow();
		//var w = Wnd.Find("* - Paint").OrThrow();
		//var w = Wnd.Find("* Help Viewer *").OrThrow();
		//var w = Wnd.Find("*Visual Studio ").OrThrow();
		//var w = Wnd.Find("app -*").OrThrow();
		//var w = Wnd.Find(null, "QM_Editor").Kid(2053).OrThrow();
		//var w = Wnd.Find("FileZilla").Kid(-31801).OrThrow();
		Print(w);
		//Print(w.ProcessId);

		//var a1=Acc.Find(w, "LINK", "Untitled");
		//Print(a1);
		//return;

		try {
			//using(var aParent = Acc.FromWindow(w)) {

			var k = new List<Acc>();
			Cpp.AccCallbackT callback = (ref Cpp.Cpp_Acc r) =>
			 {
#if true
				 var a = new Acc(r.iacc, r.elem);
				 r.iacc = default;
				 //Print(a);
				 //Print(a.RoleString);
				 k.Add(a);
#else
		using(var a = new Acc(r.iacc, r.elem)) {
			r.iacc = default;
			//Print(a);
		}
#endif
				 return 0;
			 };

			for(int i = 0; i < 7; i++) {
				k.Clear();
				100.ms();
				Perf.First();
				//var hr = Cpp.Cpp_AccFind(true, w, default, "LINK", "Untitled", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "GROUPING", "Extensions", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "PUSHBUTTON", "Infobar Container", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "LINK", null, 0, callback, 0, out var r);
				var hr = Cpp.Cpp_AccFind(true, w, default, null, null, 0, callback, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, aParent._iacc, "LINK", "Untitled", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFromWindow(true, w, 0, out var r);
				//var hr = Cpp.Cpp_AccFromWindow(true, w, (int)AccOBJID.CLIENT, out var r);
				//var hr = Cpp.Cpp_AccFromPoint(true, Mouse.XY, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "PUSHBUTTON", null, 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, "New macro    Ctrl+N", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, "Record    Ctrl+K", 0, null, 0, out var r);
				Perf.NW();
				Print(hr);
				if(hr != 0) continue;
				//continue;
				//var aa = Acc.Find(w, "PUSHBUTTON");
				using(var a = new Acc(r.iacc, r.elem)) {
				//using(var a=aa) {
					Print(a);
					//111.ms();
					//string s = null;
					//Perf.First();
					//for(int j=0; j<7; j++) {
					//	s = a.Name;
					//	Perf.Next();
					//}
					//Perf.Write();
					//Print(s);
					////speed:
					////found out-proc: 2260  1139  1072  1721  1640  932  678
					////found in-proc: 1219  482  478  473  489  472  478

					//var e = AElement.FromAcc(a, false);
					//Print(e != null);
				}
			}

			Print(k.Count);
			//Print(k);
			//foreach(var v in k) {
			//	Print($"{(IntPtr)v._iacc} {v._elem},  {v.ToString()}");
			//}
			//}
		}
		finally { Cpp.Cpp_Unload(); }
	}

	static void TestDllInProcManyWindows()
	{
		try {
			var aw = Wnd.Misc.AllWindows(true);
			foreach(var w in aw) {
				if(!w.ClassNameIs("Mozilla*")) continue;
				Print($"<><c 0xFF8000>{w}</c>");
				//continue;
				var a = new List<Acc>();
				Perf.First();
				Cpp.Cpp_AccFind(true, w, default, "LINK", null, 0, null, 0, out _);
				Perf.Next();
				var hr = Cpp.Cpp_AccFind(true, w, default, "LINK", null, 0, (ref Cpp.Cpp_Acc r) =>
				{
					a.Add(new Acc(r.iacc, r.elem));
					r.iacc = default;
					return 0;
				}, 0, out _);
				Perf.Next();
				if(hr != 1) { Print($"<><c 0xFF>{hr}</c>"); if(a.Count==0) continue; }
				Print(a.Count);
				Print(a);
				Perf.NW();
			}
		}
		finally { Cpp.Cpp_Unload(); }
		Print("END");
	}


	static void TestCppLike()
	{
		string s = "one two three four five--one two three four five--one two three four five--one two three four five--one two three four five--";
		//Print(s.Like_("*three*"));
		s = "one";

		string w = " one two three four five";
		w = "one two *--*four *";
		w = " one";
		w = w.Substring(1);
		Print(s.Like_(w));

		var x1 = new Regex(w);
		var x2 = new Regex_(w);

		//100.ms();
		Perf.SpinCPU(200);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 10000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { s.Like_("*three*"); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s.Like_(w); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s.Equals_(w); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s.Like_(w, true); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s.Equals_(w, true); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { x1.IsMatch(s); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { x2.Match(s); }
			Perf.NW();
		}

	}

	static void TestPcre()
	{
		string s, p;
		s = "one two three";
		p = @"(?i) (t|h).";

		for(int i = 0; i < 5; i++) {
			100.ms();
#if true
			Cpp.Cpp_TestPCRE(s, p);

#else
			Perf.First();
			var rx = new Regex(p, RegexOptions.CultureInvariant);
			Perf.Next();
			for(int j = 0; j < 1000; j++) {
				rx.IsMatch(s);
				//Regex.IsMatch(s, p, RegexOptions.CultureInvariant);
			}
			Perf.NW();
#endif
		}
	}

	//slower than DllImport
	//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	//[System.Security.SuppressUnmanagedCodeSecurity]
	//delegate bool Cpp_StringEqualsT(char* s, LPARAM lenS, char* w, LPARAM lenW, bool ignoreCase = false);
	//static Cpp_StringEqualsT Cpp_StringEqualsF = Cpp.Cpp_StringEquals;

	static void TestCppWildex()
	{
		string s, w;
		//s = "one two three";
		s = "one two three ąčę";
		//s = "one * three";

		//w = "**c|one two three";
		//w = s;
		//w = "one";
		//w = "one*";
		//w = "ONE TWO THREE";
		//w = "**c|ONE TWO THREE";
		//w = "one * three";
		//w = "**t|one * three";
		w = @"**r|^one \w+ threE$";
		//w = @"**p|^one \w+ threE$";
		//w = @"**rc|^one \w+ three$";
		//w = @"**rc|^one \w+ three ąčę$";
		//w = @"**r|^one \w+ three ąčĘ$";
		//w = @"**r|^one \w+ three \w+$";
		//w = @"**r|^one \w+ three ...$";
		//w = @"**r|(*UCP)^one \w+ three \w+$";
		//w = @"**n|one";
		//w = @"**n|one*";
		//w = @"**m|one*";
		//w = @"**m|kuku[]one*";
		w = @"**m|ku[]**p|ku[]one*";

		//		for(int i = 0; i < 1; i++) {
		//			100.ms();
		//#if true
		//				Cpp.Cpp_TestWildex(s, w);
		//#else
		//			var x = new Wildex2(w);
		//			Print(x.Match(s));
		//#endif
		//		}

		s = "modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Auisuidu - hdjshhdsjhdj jdskjdks Auisuidu - hdjshhdsjhdj jdskjdkM ė";
		w = "**c|modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Auisuidu - hdjshhdsjhdj jdskjdks Auisuidu - hdjshhdsjhdj jdskjdkM ė";

		s = "short678";
		w = "**c|short678";

		w = w.Substring(4);
		//w = w.ToUpper_();

		Wildex x = w;

		bool ignoreCase = true;
		fixed (char* p1 = s, p2 = w) {
			//PrintHex((long)p);
			Print(ignoreCase ? w.Equals(s, StringComparison.OrdinalIgnoreCase) : w.Equals(s));
			//Print(Cpp.Cpp_StringEquals(p1, s.Length, p2, w.Length, ignoreCase));
			//Print(Cpp.Cpp_StringEquals(p1 + 1, s.Length - 1, p2 + 1, w.Length - 1, ignoreCase));
			//Print(Cpp_StringEqualsF(p1, s.Length, p2, w.Length, ignoreCase));
			Print(w.Equals_(s, ignoreCase));
			Print(x.Match(s));

			Perf.SpinCPU(200);
			//200.ms();
			for(int i1 = 0; i1 < 5; i1++) {
				int n2 = 10000;
				Perf.First();
				for(int i2 = 0; i2 < n2; i2++) { /*x.Match(s);*/ }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { bool yes = ignoreCase ? w.Equals(s, StringComparison.OrdinalIgnoreCase) : w.Equals(s); }
				//Perf.Next();
				//for(int i2 = 0; i2 < n2; i2++) { bool yes = Cpp.Cpp_StringEquals(p1, s.Length, p2, w.Length, ignoreCase); }
				//Perf.Next();
				//for(int i2 = 0; i2 < n2; i2++) { bool yes = Cpp.Cpp_StringEquals(p1+1, s.Length-1, p2+1, w.Length-1, ignoreCase); }
				//Perf.Next();
				//for(int i2 = 0; i2 < n2; i2++) { bool yes = Cpp_StringEqualsF(p1, s.Length, p2, w.Length, ignoreCase); }
				Perf.Next();
				for(int i2 = 0; i2 < n2; i2++) { bool yes = w.Equals_(s, ignoreCase); }
				//Perf.Next();
				//for(int i2 = 0; i2 < n2; i2++) { bool yes = x.Match(s); }
				Perf.NW();
			}

		}
		return;

		//var x = new Wildex(w);
		////Print(x.Match(s));

		//Print(w.Equals_(s, true));
		////Print(w.Equals(s, StringComparison.OrdinalIgnoreCase));
		//return;

		//Perf.SpinCPU(200);
		////200.ms();
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { /*x.Match(s);*/ }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { bool yes = w.Equals_(s, true); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { bool yes = w.Equals(s, StringComparison.OrdinalIgnoreCase); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}

	}

	//[StructLayout(LayoutKind.Sequential)]
	//class Wildex2
	//{
	//	LPARAM _1, _2, _3;

	//	~Wildex2() => Cpp.Cpp_WildexDtor(this);

	//	public Wildex2(string w)
	//	{
	//		char* es = null;
	//		if(!Cpp.Cpp_WildexParse(this, w, w.Length, &es))
	//			throw new CatException(new string(es));
	//	}

	//	public bool Match(string s)
	//	{
	//		if(s == null) return false;
	//		return Cpp.Cpp_WildexMatch(this, s, s.Length);
	//	}

	//	public bool HasValue => _1 != default;
	//}

	static void TestWildexStruct()
	{
		var x = new WildexStruct("**r|gg");
		Print(x.Value);
		//x.Value = "k";
		Print(x.Match("gg"));
	}

	static void TestCppRegex()
	{
		string s, w;
		//s = "one two three";
		//s = "one two three ishdalkhi ajdsaiudpahfauhfiahfhhus gausgusahi usfhaisaisgfiagasgfiugaiufgiahruihaiuhuia";
		//s = "one two three ąčę";
		//s = "k";
		s = @"kk one 125 three";

		w = s;
		w = @"one (two|\d+) three";
		//w = "a ) b";
		//w = @^one \w+ threE$";
		//w = @"^one \w+ three$";
		//w = @"^one \w+ three ąčę$";
		//w = @"^one \w+ three ąčĘ$";
		//w = @"^one \w+ three \w+$";
		//w = @"^one \w+ three ...$";
		//w = @"(*UCP)^one \w+ three \w+$";

		//var x = new Regex_(w, RXFlags.EXTRA_MATCH_WORD);
		//Print(x.Match(s));

		//var xn = new Regex(w);
		//Print(xn.IsMatch(s));

		//Perf.SpinCPU(100);
		//100.ms();
		//var a1 = new Action(() => { xn = new Regex(w); });
		//var a2 = new Action(() => { x = new Cpp.Cpp_Regex(w); });
		////var a1 = new Action(() => { xn.IsMatch(s); });
		////var a2 = new Action(() => { x.Match(s); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 100, a1, a2, a3, a4);


		//MessageBox.Show("ddd");
		//Perf.First();
		//for(int i = 0; i < 100; i++) {
		//	//if((i % 10) == 0) Print(i);
		//	var x = new Cpp.Cpp_Regex(w);
		//	if((i % 1000) == 0) {
		//		//_PrintMemory();
		//		1.ms();
		//	}
		//}
		//Perf.NW();
		//Print("END");

		Print(Wnd.Find(@"**p|\QQuick\E \bMacros\b"));
	}

	//static Cpp.ICppTest s_icpptest;

	//static void TestUnmanagedCallSpeed()
	//{
	//	string s="aa", w="nn";

	//	s_icpptest = Cpp.Cpp_Interface();

	//	fixed (char* p1 = s, p2 = w) {

	//		Perf.SpinCPU(100);
	//		for(int i1 = 0; i1 < 5; i1++) {
	//			int n2 = 1000;
	//			Perf.First();
	//			for(int i2 = 0; i2 < n2; i2++) { var x = _TestAdd(s, w); }
	//			Perf.Next();
	//			for(int i2 = 0; i2 < n2; i2++) { var x = _TestAdd(s, w); }
	//			Perf.NW();
	//		}
	//	}
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static byte _TestAdd(string a, string b)
	//{
	//	fixed (char* pa = a, pb = b)
	//		return _TestAdd2(pa, pb);
	//		//return Cpp.Cpp_TestAdd2(pa, pb);
	//		//return s_icpptest.One(pa, pb);
	//}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//static byte _TestAdd2(char* a, char* b)
	//{
	//	return (byte)(a == b ? 1 : 0);
	//}


	static void TestIPC()
	{
		//var w = Wnd.Find("* Mozilla Firefox").OrThrow();
		//var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, Api.s_IES).OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, "TabWindowClass").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, "Shell DocObject View").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").OrThrow();
		//var w = Wnd.Find("* Opera").OrThrow();
		//var w = Wnd.Find("Options").OrThrow();
		//var w = Wnd.Find("*- OpenOffice Writer").OrThrow();
		//var w = Wnd.Find("Keys").OrThrow();
		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		var w = Wnd.Misc.FindMessageWindow(null, "QM_testIPC").OrThrow();
		//var w = Wnd.Find("QM Dialog").OrThrow();
		//var w = Wnd.Find("Catkeys - Q*").OrThrow();
		//var w = Wnd.Find("Calculator").OrThrow();
		//var w = Wnd.Find("ILSpy").OrThrow();
		//var w = Wnd.Find("Java *").OrThrow();
		//var w = Wnd.Find("* Sandcastle *").OrThrow();
		//var w = Wnd.Find("* C# Converter *").OrThrow();
		//var w = Wnd.Find("* OpenOffice *").OrThrow();
		//w = w.WndDirectParent;
		//Print(w);

		//Cpp.Cpp_Test();

		//var hp = Api.GetCurrentProcess();
		////using(var pm=new Process_.Memory(w, 0)) {
		////var hp = pm.ProcessHandle;
		//const int KB = 1024;
		//const int MB = 1024 * 1024;
		//for(uint i = 64 * KB; i < 0x7fffffff; i += 64 * KB) {
		//	var m = Api.VirtualAllocEx(hp, (IntPtr)i, 4000);
		//	//var m2 = Api.VirtualAllocEx(hp, (IntPtr)i, 4000);
		//	PrintHex(m);
		//	//PrintList(m.ToInt64().ToString("X"), m2.ToInt64().ToString("X"));
		//	Api.VirtualFreeEx(hp, m);
		//	//if(m2 != default) Api.VirtualFreeEx(hp, m2);
		//}

		//for(int i = 0; i < 5; i++) {
		//	100.ms();
		//	Perf.First();
		//	var m = AllocInThisProcessForOtherProcess(64 * 1024, false);
		//	Perf.Next();
		//	Api.VirtualFreeEx(Api.GetCurrentProcess(), m);
		//	Perf.NW();
		//	PrintHex(m.ToInt64());
		//}

		for(int i = 0; i < 10; i++) {
			100.ms();
			Perf.First();
#if false
			var k = Wnd.Misc.WindowClass.InterDomainCreateMessageWindow("class_test589");
			Perf.Next();
			try {
				var c = (char*)NativeHeap.Alloc(6);
				c[0] = 'A'; c[1] = 'B'; c[2] = '\0';
				Api.COPYDATASTRUCT x;
				x.dwData = 0;
				x.cbData = 6;
				x.lpData = (IntPtr)c;
				w.Send(Api.WM_COPYDATA, (LPARAM)k, &x);
				NativeHeap.Free(c);
				Perf.Next();

				for(; ; ) {
					var o = Api.MsgWaitForMultipleObjectsEx(0, null, 1000);
					//Print(o);
					break;
				}
				//Api.PeekMessage(out var ms, default, 0, 0, Api.PM_REMOVE);
				Perf.Next();
			}
			finally { Api.DestroyWindow(k); }
#elif true
			var c = (char*)NativeHeap.Alloc(6);
			c[0] = 'A'; c[1] = 'B'; c[2] = '\0';
			Api.COPYDATASTRUCT x;
			x.dwData = 0;
			x.cbData = 6;
			x.lpData = (IntPtr)c;
			w.Send(Api.WM_COPYDATA, 0, &x);
			NativeHeap.Free(c);
			Perf.Next();
			if(0 != Api.WaitForSingleObject(s_event.SafeWaitHandle.DangerousGetHandle(), 10000)) throw new CatException(0, "*wait for event");
			Perf.Next();

#elif true
			if(0 != Api.WaitForSingleObject(s_mutex.SafeWaitHandle.DangerousGetHandle(), 10000)) throw new CatException(0, "*wait for mutex");
			try {
				Perf.Next();
				var c = (char*)s_sm;
				c[0] = 'A'; c[1] = 'B'; c[2] = '\0';
				Perf.Next();
				w.SendNotify(Api.WM_APP + 2, 0, 0);
				Perf.Next();
				if(0 != Api.WaitForSingleObject(s_event.SafeWaitHandle.DangerousGetHandle(), 10000)) throw new CatException(0, "*wait for event");
				Perf.Next();
				//Print(new string(c, 0, 2));
			}
			finally { s_mutex.ReleaseMutex(); }
#else
			using(var hpTarget = Process_.LibProcessHandle.FromWnd(w, Api.PROCESS_DUP_HANDLE)) {
				Perf.Next();
				//Print(hpTarget.Handle);
				var hpThis = Api.GetCurrentProcess();
				if(!DuplicateHandle(hpThis, hpThis, hpTarget, out var hpDup, 0, false, DUPLICATE_SAME_ACCESS)) throw new CatException(0, "*DuplicateHandle");
				Perf.Next();
				bool is64 = Api.IsWow64Process(hpTarget, out var is32bit) && !is32bit;
				Perf.Next();
				//Print(is64);
				var mem = AllocInThisProcessForOtherProcess(64 * 1024, is64);
				Perf.Next();
				try {
					if(mem == default) throw new CatException(0, "*allocate memory");

					var c = (char*)mem;
					c[0] = 'A'; c[1] = 'B'; c[2] = '\0';

					Perf.Next();
					w.Send(Api.WM_APP + 1, hpDup, mem);
					Perf.Next();

					//Print(new string(c, 0, 2));
				}
				finally { Api.VirtualFreeEx(hpThis, mem); Api.CloseHandle(hpDup); }
			}
#endif
			Perf.NW();
		}
	}
	[DllImport("kernel32.dll")]
	internal static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, bool bInheritHandle, uint dwOptions);
	internal const uint DUPLICATE_SAME_ACCESS = 0x2;

	static void* s_sm = SharedMemory.CreateOrGet("sm_test589", 1 * 1024 * 1024, out _);
	static Mutex s_mutex = new Mutex(false, "mutex_test589");
	static EventWaitHandle s_event = new EventWaitHandle(false, EventResetMode.AutoReset, "event_test589");

	static ushort s_wcIPC = Wnd.Misc.WindowClass.InterDomainRegister("class_test589", _WndProcIPC);

	static LPARAM _WndProcIPC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	{
		//Wnd.Misc.PrintMsg(w, msg, wParam, lParam);
		if(msg == Api.WM_COPYDATA) {

			//w.Post(0);
		}

		return Api.DefWindowProc(w, msg, wParam, lParam);
	}

	static IntPtr AllocInThisProcessForOtherProcess(int size, bool otherProcessIs64bit)
	{
		var hp = Api.GetCurrentProcess();

		if(otherProcessIs64bit || !Ver.Is64BitProcess)
			return Api.VirtualAllocEx(hp, default, size);

		const int KB = 1024;
		for(long i = 64 * KB; i < 0x80000000 - size; i += 64 * KB) {
			var m = Api.VirtualAllocEx(hp, (IntPtr)i, size);
			if(m != default) return m;
		}
		return default;
	}

	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
#if DEBUG
		//Output.IgnoreConsole = true;
		//Output.LogFile=@"Q:\Test\catkeys"+IntPtr.Size*8+".log";
#endif
		Output.LibWriteToQM2 = true;
		Output.RedirectConsoleOutput = true;
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			Thread.Sleep(100);
		}

		//TaskDialog.Show(Debug_.IsCatkeysDebugConfiguration?"DEBUG":"RELEASE"); return;

		try {
			//TestIPC();
			TestDllInProc();
			//TestDllInProcManyWindows();

			//TestUnmanagedCallSpeed();
			//Cpp.Cpp_TestSimpleStringBuilder();
			//TestCppRegex();
			//TestWildexStruct();
			//TestCppWildex();
			//TestPcre();
			//TestCppLike();
			//Cpp_TestClrHost();
			//Cpp.Cpp_Test();

			//TestAElementFromPoint();
			//TestAElementFind();
			//TestUia2();
			//TestUiaSpeed();

			//TestAccFromUIA();
			//TestAccSkipAndWait();
			//TestAccTODO();
			//TestAccInOtherFunctions();
			//TestJava();
			//Acc.TestJava();
			//TestChromeEnableAcc();
			//TestAccSelectedChildren();
			//TestAccWebPageProp();
			//TestIAccessible2();
			//TestManifest_disableWindowFiltering();
			//TestAccMiscMethods();
			//TestWndFocus();
			//TestAccBstrWithNullChars();
			//TestWhite();
			//TestMainWindowsWithUIA();
			//TestUIA_COM();
			//TestAccFromComObject();
			//TestAccFromEvent();
			//TestAccFromFocus();
			//TestAccFromXY();
			//TestAccPreferLink();
			//TestWinFlags();
			//TestWndKid();
			//TestAccVariousApps();
			//TestAccOpenLibreOffice();
			//TestAccWeb();
			//TestAccExamples();
			//TestAcc();
			//TestAccNavigate();
			//TestOrThrow();
			//TestVariantToString();
			//TestDebugPrintColor();

			//TestGarbageOfWaitWindowEtc();
		}
		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }


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

		//l.Perf.First();
		//l.TaskDialog.Show("f");
		//l.Util.LibDebug_.PrintLoadedAssemblies();
		//Print(l.TDIcon.Info);

	}
}

