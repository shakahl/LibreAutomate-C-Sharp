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

using Au.Types;
using Au.Util;

static unsafe partial class Test
{
	#region ScreenImage

	static void TestScreenImage()
	{
		//var b = new Au.Util.MemoryBitmap(-1, -1);
		//var b = new Au.Util.MemoryBitmap(60000, 10000);
		//PrintList(b.Hdc, b.Hbitmap);

		//Wnd.Find("app -*").Activate();Sleep(100);
		//var w = Wnd.Find("app -*");
		//var w = Wnd.Find("QM# -*");

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
		//AuException.ThrowIfFailed(Api.AccessibleObjectFromWindow(w, Api.OBJID_WINDOW, ref Api.IID_IAccessible, out var iacc));
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

		//var s = Au.Controls.ImageUtil.ImageToString(Folders.ThisAppImages + "qm info.bmp");
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
			Print(Au.Controls.ImageUtil.ImageToString(path));
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
		//var w = Wnd.Find("QM# *", "WindowsForms*"); //w=w.Child("**wfName:Edit"); Print(w); return;
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
		//int k;
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

	//static void TestHwndReusing()
	//{
	//	string cn1, cn2;
	//	cn1 = "jooooo1";
	//	cn2 = "jooooo2";
	//	Wnd.Misc.MyWindowClass.InterDomainRegister(cn1, null);
	//	Wnd.Misc.MyWindowClass.InterDomainRegister(cn2, null);

	//	var d = new HashSet<Wnd>();
	//	Perf.First();
	//	for(int i = 1; ; i++) {
	//		var cn = ((i & 1) != 0) ? cn1 : cn2;
	//		//var w = Wnd.Misc.CreateWindow(0, "#32770", null, Native.WS_POPUP); //75000, 4 minutes
	//		////var w = Wnd.Misc.CreateMessageWindow("#32770"); //65000, 12 seconds
	//		//var w = Wnd.Misc.MyWindowClass.InterDomainCreateWindow(0, cn, null, Native.WS_POPUP); //75000, 4 minutes
	//		var w = Wnd.Misc.MyWindowClass.InterDomainCreateMessageWindow(cn); //65000, 9 seconds
	//		if(w.Is0) throw new Exception();
	//		Wnd.Misc.DestroyWindow(w);
	//		if(d.Contains(w)) break;
	//		d.Add(w);
	//		if(i % 1000 == 0) Print(i);
	//		Time.Sleep(1);
	//	}
	//	Perf.NW();
	//	Print(d.Count);
	//}

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
						if(!_stringCacheWR.TryGetTarget(out var cache)) _stringCacheWR.SetTarget(cache = new Au.Util.StringCache());
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


	static Au.Util.StringCache _stringCache = new Au.Util.StringCache();
	static WeakReference<Au.Util.StringCache> _stringCacheWR = new WeakReference<Au.Util.StringCache>(null);

	static void TestCharPtrEndWith()
	{
		var s = "notepad.exe";
		fixed (char* p = s) {
			Print(Au.Util.LibCharPtr.EndsWith(p, s.Length, ".Exe", true));
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

	[DllImport(@"Q:\app\Au\Dll\64bit\AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern ITest CreateTestInterface();

	[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
	static extern void CreateTestInterface2(out ITest t);
	//static extern void CreateTestInterface2(out IntPtr t); //with Marshal.GetTypedObjectForIUnknown is missing 1 Release
	//static extern void CreateTestInterface2([MarshalAs(UnmanagedType.IUnknown)] out object t); //the same number of QI etc as with 'out ITest t'

	[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", EntryPoint = "CreateTestInterface2", CallingConvention = CallingConvention.Cdecl)]
	static extern void CreateTestInterface3(out IntPtr t); //with Marshal.GetTypedObjectForIUnknown is missing 1 Release

	[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
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
		//IntPtr p;
		//int u, uu, uuu;
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
	//static int g_n;
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

		//var w = Wnd.Find("QM# - Q*");
		//var w = Wnd.Find("*Internet Explorer");
		//Perf.First();
		//var a = Acc.Find(w, "PUSHBUTTON", "Home", AFFlags.Reverse);
		//Perf.NW();
		//Print(a);
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
			for(int i = 0; i < 10; i++) f.Find(w);
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

	//static void TestAccBstrWithNullChars()
	//{
	//	int n = 0;
	//	Acc.FromWindow(Wnd.Misc.WndRoot).EnumChildren(true, a =>
	//	{
	//		var s = a.ToString();
	//		n++;
	//	});
	//	Print(n);
	//}

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
		//var w = Wnd.Find("QM# - Q*").OrThrow();
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
			if(0 != Api.WaitForSingleObject(s_event.SafeWaitHandle.DangerousGetHandle(), 10000)) throw new AuException(0, "*wait for event");
			Perf.Next();

#elif true
			if(0 != Api.WaitForSingleObject(s_mutex.SafeWaitHandle.DangerousGetHandle(), 10000)) throw new AuException(0, "*wait for mutex");
			try {
				Perf.Next();
				var c = (char*)s_sm;
				c[0] = 'A'; c[1] = 'B'; c[2] = '\0';
				Perf.Next();
				w.SendNotify(Api.WM_APP + 2, 0, 0);
				Perf.Next();
				if(0 != Api.WaitForSingleObject(s_event.SafeWaitHandle.DangerousGetHandle(), 10000)) throw new AuException(0, "*wait for event");
				Perf.Next();
				//Print(new string(c, 0, 2));
			}
			finally { s_mutex.ReleaseMutex(); }
#else
			using(var hpTarget = Process_.LibProcessHandle.FromWnd(w, Api.PROCESS_DUP_HANDLE)) {
				Perf.Next();
				//Print(hpTarget.Handle);
				var hpThis = Api.GetCurrentProcess();
				if(!DuplicateHandle(hpThis, hpThis, hpTarget, out var hpDup, 0, false, DUPLICATE_SAME_ACCESS)) throw new AuException(0, "*DuplicateHandle");
				Perf.Next();
				bool is64 = Api.IsWow64Process(hpTarget, out var is32bit) && !is32bit;
				Perf.Next();
				//Print(is64);
				var mem = AllocInThisProcessForOtherProcess(64 * 1024, is64);
				Perf.Next();
				try {
					if(mem == default) throw new AuException(0, "*allocate memory");

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

	//static ushort s_wcIPC = Wnd.Misc.MyWindowClass.InterDomainRegister("class_test589", _WndProcIPC);

	//static LPARAM _WndProcIPC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	//{
	//	//Wnd.Misc.PrintMsg(w, msg, wParam, lParam);
	//	if(msg == Api.WM_COPYDATA) {

	//		//w.Post(0);
	//	}

	//	return Api.DefWindowProc(w, msg, wParam, lParam);
	//}

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

	//static void TestBstrInterop()
	//{
	//	string s = "kkk", w = "kk*";

	//	fixed (char* p = s) {
	//		Print((LPARAM)p);
	//	}

	//	Cpp.Cpp_TestWildex(s, w);
	//}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestIR()
	{
		ITest x = CreateTestInterface();
		x = null;
		Acc a = Acc.FromWindow(Wnd.Find("*Notepad"));
		a = null;
	}

	static void TestComInterfaceReleaseThread()
	{
		//results: if [STAThread], GC calls Release in same thread where created. Else in GC thread.

		Task.Run(() => { Thread.Sleep(50); GC.Collect(); });
		Print(Api.GetCurrentThreadId());
		Perf.First();
		for(int i = 0; i < 1; i++) {
			TestIR();
			Perf.Next();
			Thread.Sleep(300);
		}
		Perf.Write();
	}

}
