//#define TOPLEVEL

using System;
using System.Collections.Generic;
#if true
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime;
using Microsoft.Win32;
using System.Runtime.InteropServices.ComTypes;
using System.Numerics;
using System.Globalization;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Windows;
using System.Collections;

using Au.Triggers;
using Au.Controls;
#endif

using Au;
using Au.Types;
using System.Resources;
using Au.More;

[module: System.Runtime.InteropServices.DefaultCharSet(System.Runtime.InteropServices.CharSet.Unicode)]

#if TOPLEVEL

//// TEST C# 9 ////

// TOP-LEVEL PROGRAMS

//int i = 7;
//print.it("no class", LocFunc(), args.Length, Thread.CurrentThread.GetApartmentState());

//var v = new Abc();
//v.Test();

////[STAThread]
//int LocFunc() {
//	return i;
//}

//class Abc
//{
//	public void Test() {
//		print.it("test", typeof(Abc).FullName);
//	}
//}


// RECORDS

//var p = new Product { Name = "asd", CategoryId = 4 };
//p = p with { CategoryId = 10 };
//print.it(p.Name, p.CategoryId);

//public record Product
//{
//	public string Name { get; init; }
//	public int CategoryId { get; init; }
//}

//public record Product2
//{
//	string Name;
//	int CategoryId;
//}


// PATTERN-MATCHING

//object o = "asdf";
//switch (o) {
//	case int: print.it("int"); break;
//	case string: print.it("string"); break;
//}

//int i = 15;
//switch (i) {
//	case < 10: print.it("<10"); break;
//	case >= 10 and <=20: print.it("10..20"); break;
//}

//if (o is not int) print.it("not");
////if (o is not int and i is not double) print.it("not"); //no
////if(i>=10 and i<=20) //no


// TARGET-TYPING

//System.Drawing.Point p = new(3, 5);
//System.Drawing.Point pp = new();

//Control c1 = null; Form f1 = null;
//Control c = c1 ?? f1; // Shared base type
////int? result = false ? 0 : null; // nullable value type //error


// SKIP LOCALS-INIT

//Skiiip();

//[System.Runtime.CompilerServices.SkipLocalsInitAttribute]
//unsafe void Skiiip() {
//	var v = stackalloc long[1000];

//	print.it(v[0]);
//}


// NATIVE INT

//nint i = 5;
////print.it(sizeof(nint));


//////////////////////////////



#else

partial class TestScript
{

	class TestGC
	{
		~TestGC() {
			if (Environment.HasShutdownStarted) return;
			if (AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			print.it("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//timerm.after(1, _ => new TestGC());
			//var f = Program.MainForm; if(!f.IsHandleCreated) return;
			//f.BeginInvoke(new Action(() => new TestGC()));
			new TestGC();
		}
	}
	static bool s_debug2;

	void _MonitorGC() {
		//return;
		if (!s_debug2) {
			s_debug2 = true;
			new TestGC();

			//timerm.every(50, _ => {
			//	if(!s_debug) {
			//		s_debug = true;
			//		timerm.after(100, _ => new TestGC());
			//	}
			//});
		}
	}


	//class _Settings
	//{
	//	public string OneTwo { get; set; }
	//	public int ThreeFour { get; set; }
	//	public int Five { get; set; }
	//	public bool Six { get; set; }
	//	public string Seven { get; set; }
	//	public string Eight { get; set; } = "def";
	//}

	//void TestJson()
	//{
	//	var file = @"Q:\test\sett.json";
	//	var file2 = @"Q:\test\sett.xml";

	//	var v = new _Settings { OneTwo = "text ąčę", ThreeFour = 100 };

	//	for(int i = 0; i < 5; i++) {
	//		//100.ms();
	//		//perf.first();
	//		//var k1 = new JsonSerializerOptions { IgnoreNullValues = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };
	//		//var b1 = JsonSerializer.SerializeToUtf8Bytes(v, k1);
	//		//perf.next();
	//		//File.WriteAllBytes(file, b1);
	//		//perf.nw();

	//		100.ms();
	//		perf.first();
	//		var b2 = File.ReadAllBytes(file);
	//		perf.next();
	//		var k2 = new JsonSerializerOptions { IgnoreNullValues = true };
	//		perf.next();
	//		v = JsonSerializer.Deserialize<_Settings>(b2, k2);
	//		perf.nw('J');
	//	}

	//	for(int i = 0; i < 5; i++) {
	//		//100.ms();
	//		//perf.first();
	//		//var r1 = new XElement("r");
	//		//r1.Add(new XElement("OneTwo", v.OneTwo));
	//		//r1.Add(new XElement("ThreeFour", v.ThreeFour.ToString()));
	//		//perf.next();
	//		//r1.Save(file2);
	//		//perf.nw();

	//		100.ms();
	//		perf.first();
	//		var r2 = XElement.Load(file2);
	//		perf.next();
	//		v = new _Settings();
	//		v.OneTwo = r2.Element("OneTwo").Value;
	//		var s2 = r2.Element("ThreeFour").Value;
	//		perf.nw('X');
	//		v.ThreeFour = s2.ToInt();
	//	}

	//	print.it(v.OneTwo, v.ThreeFour, v.Five, v.Six, v.Seven, v.Eight);

	//	//JsonDocument d; d.RootElement.
	//}

	//[DllImport("CppE")]
	//static extern int Cpp_Install(int step, string dir);

	//[DllImport("CppE")]
	//static extern int Cpp_Uninstall();


#if false
	void TestToolbar()
	{
		for(int i = 0; i < 1; i++) {
			var t = new toolbar("123");
			//t.NoText = true;
			//t.Border= TBBorder.Sizable3;t.Control.Text = "Toolbar";
			//t.Border = TBBorder.SizableWithCaptionX;

			//t["Find", @"Q:\app\find.ico"] = o => print.it(o);
			//t["Copy", @"Q:\app\copy.ico"] = o => print.it(o);
			//t.Separator("Tpi group");
			//t["Delete", @"Q:\app\delete.ico"] = o => print.it(o);
			//t["No image"] = o => print.it(o);
			//t["TT", tooltip: "WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW "] = o => print.it(o);
			////t.LastButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			////t.LastButton.AutoToolTip = false;
			////t.LastButton.ToolTipText = "ggg";
			//t.Separator();
			//t["Run", @"Q:\app\run.ico"] = o => print.it(o);
			//t.Separator("");
			//t["Paste text", @"Q:\app\paste.ico"] = o => print.it(o);
			//t.LastButton.ToolTipText = "Toooooltip";

			//t.ExtractIconPathFromCode = true;
			//t["Auto icon"] = o => print.it("notepad.exe");
			//t["Failed icon", @"Q:\app\-.ico"] = o => print.it(o);
			////t.LastButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			////t.Separator("");
			////t.Add(new ToolStripTextBox { ToolTipText= "ToolStripTextBox", AutoSize=false, Width=50 });
			////t.Add(new ToolStripComboBox { ToolTipText= "ToolStripComboBox", AutoSize=false, Width=50 });
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripButton("aaa"));
			////t.Add(new ToolStripButton("bbb"));
			////t["Multi\r\nline"] = o => print.it(o);

			//t["None"] = o => _B(TBBorder.None);
			//t["SWC"] = o => _B(TBBorder.SizableWithCaption);
			//t["Sizable1"] = o => _B(TBBorder.Sizable1);
			//t["Sizable2"] = o => _B(TBBorder.Sizable2);
			//t["Sizable3"] = o => _B(TBBorder.Sizable3);
			//t["Sizable3D"] = o => _B(TBBorder.Sizable3D);
			//t["Sizable"] = o => _B(TBBorder.Sizable);
			//t["FixedWithCaption"] = o => _B(TBBorder.FixedWithCaption);
			//t["SizableWithCaption"] = o => _B(TBBorder.SizableWithCaption);
			//t["Close"] = o => t.Close();

#if false
			var dd = new ToolStripDropDownButton("DD");
			t.Add(dd, @"Q:\app\find.ico");
			dd.DropDownOpening += (_, _) => {
				var m = new popupMenu(dd);
				m["one"] = o => print.it(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => print.it(o);
				}
			};
			var sb = new ToolStripSplitButton("SB");
			t.Add(sb, @"Q:\app\copy.ico", o => print.it(o));
#elif true
			//t.Control.Font = new Font("Courier New", 16);
			//t.Control.RightToLeft = RightToLeft.Yes;
			t.MenuButton("DD", m => {
				print.it("dd");
				//m.MultiShow = false;
				m["one"] = o => print.it(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => print.it(o);
				}
			}, @"Q:\app\find.ico", "MenuButton");
			t.SplitButton("SB", m => {
				m["one"] = o => print.it(o);
				//var sb = m.Control.OwnerItem as ToolStripSplitButton;
				//print.it(sb);
				//sb.DefaultItem = m.LastItem;
				using(m.Submenu("Sub")) {
					m["si"] = o => print.it(o);
				}
			}, @"Q:\app\copy.ico", "SplitButton", o => print.it(o));
			t.Separator("");
			t[true, "DD2", @"Q:\app\delete.ico"] = m => {
				print.it("create menu");
				//m.MultiShow = false;
				m["one"] = o => print.it(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => print.it(o);
				}
			};
			//t.SplitButton("SB", o => {
			//	print.it(o);
			//}, m => {
			//	m["one"] = o => print.it(o);
			//	using(m.Submenu("Sub")) {
			//		m["si"] = o => print.it(o);
			//	}
			//}, @"Q:\app\copy.ico", "SplitButton");
			//Action<popupMenu> menu1 = m => {
			//	m["one"] = o => print.it(o);
			//	using(m.Submenu("Sub")) {
			//		m["si"] = o => print.it(o);
			//	}
			//};
			//t.MenuButton("DD", menu1, @"Q:\app\find.ico", "MenuButton");
#elif false
			t.MenuButton("DD", @"Q:\app\find.ico");
			t.Menu = m => {
				m["one"] = o => print.it(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => print.it(o);
				}
			};
#else
			t.MenuButton("DD", @"Q:\app\find.ico").Menu = m => {
				print.it("dd");
				//m.MultiShow = false;
				m["one"] = o => print.it(o);
				using(m.Submenu("Sub")) {
					m["two"] = o => print.it(o);
				}
			};
			t.SplitButton("SB", o => print.it(o), @"Q:\app\copy.ico").Menu = m => {
				print.it("dd");
				m["one"] = o => print.it(o);
				using(m.Submenu("Sub")) {
					m["two"] = o => print.it(o);
				}
			};
#endif
			//t.Separator("");
			////t["GC"] = o => GC.Collect();

			//var dd = new ToolStripSplitButton("SB2", null, (_, _)=>print.it("click"));
			//t.Add(dd, @"Q:\app\delete.ico");
			//dd.DropDownOpening += (_, _) => {
			//	var m = new popupMenu();
			//	dd.DropDown = m.Control;
			//	m["one"] = o => print.it(o);
			//};
			//dd.ButtonClick += (_, _) => print.it("button click");
			//dd.DoubleClickEnabled = true;
			//dd.ButtonDoubleClick += (_, _) => print.it("button double click");

			//timerm.after(3000, _ => {
			//	var c = t.Control.Items[0];
			//	c.Select();
			//});

			//void _B(TBBorder b){
			//	t.Border = b;
			//	//print.it(wnd.more.borderWidth((wnd)t.Control));
			//}

			//t.Bounds = new Rectangle(i * 300 + 700, 200, 200, 200);
			t.Show();
			//t.Window.ActivateL();
			wait.doEvents(200);

			//for(int j = 1; j <= (int)TBBorder.SizableWithCaptionX; j++) {
			//	wait.doEvents(1000);
			//	t.Border = (TBBorder)j;
			//}

			//wait.doEvents(1000);
			//t.Border = TBBorder.FixedWithCaption;
			//wait.doEvents(3000);
			//t.Border = TBBorder.SizableWithCaption;

			//var m = new popupMenu();
			//using(m.Submenu("Sub")) {

			//}
			//m.Show()
		}

		//var c = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(VisualStyleElement.Window.FrameLeft.Inactive).GetColor(ColorProperty.BorderColor);
		//print.it((uint)c.ToArgb());

		//timerm.after(500, _ => {
		//	var w = (wnd)t.Control;
		//	//w.SetStyle(WS.DLGFRAME, SetAddRemove.Add);
		//});

		dialog.options.topmostIfNoOwnerWindow = true;
		dialog.show();

		//timerm.after(10000, _ => Application.Exit());
		//Application.Run();
	}
#endif

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//void TestCallerArgumentExpression(string so, [CallerArgumentExpression("so")] string ca = null) //does not work
	//{
	//	print.it(so, ca);
	//}

	void TestBitmapLockBitsDispose() {
		//Bitmap.FromFile(@"Q:\Test\qm small icon.png").Dispose();
		//print.it("start");
		//3.s();
		//Debug_.MemorySetAnchor_();
		//for(int i = 0; i < 1000; i++) {
		//	var b = Bitmap.FromFile(@"Q:\Test\qm small icon.png") as Bitmap;
		//	//var d=b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//	var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format64bppArgb);
		//	b.Dispose();
		//	b.UnlockBits(d);
		//}
		//Debug_.MemoryPrint_();
		//while(keys.isScrollLock) 100.ms();


		var w = +wnd.find("Au - Microsoft Visual Studio", "HwndWrapper[DefaultDomain;*");
		string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAA0AAAAQCAYAAADNo/U5AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAC2SURBVDhPYzhz9cN/DHzlw/+Hp1b8v7osHisGazoNVISOr6/N+39rZ+f/k6efY2C4Jmwm4sJDRdPNjSVgGobvH56BVQMIwzX1rz36nzd40n8Grx4w3bfh3P8Hxxfg1wRSuHzvZXCQgmgQ/+Hj8/g1gWxAjgvpmGlgcYKa0G0CiWPDDCDCo2krWBMMgzTM3XkZnJxAhqJjBpgGEI1sGjbFMAzWBNOATQE2DHYeKRpAGBwQpOEP/wF46o8knB4kYgAAAABJRU5ErkJggg==";
		//string image = @"Q:\Test\find.bmp";

		//for(int i = 0; i < 10; i++) {
		//	var im = +uiimage.find(w, image, IFFlags.WindowDC);
		//	print.it(im);
		//}

		var im = +uiimage.find(w, image, IFFlags.WindowDC);

		//var b = uiimage.more.loadImage(image);

		//var h = Api.LoadImage(default, image, Api.IMAGE_BITMAP, 0, 0, Api.LR_LOADFROMFILE);
		//var b = Bitmap.FromHbitmap(h);
		//var d = b.LockBits(new Rectangle(default, b.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//print.it(d.Stride);
		//b.UnlockBits(d);

		//var im = +uiimage.find(w, b, IFFlags.WindowDC);
		print.it(im);

	}

	static HashSet<object> hs = new HashSet<object>();
	static HashSet<object> hs2 = new HashSet<object>();
	static System.Collections.Concurrent.ConcurrentDictionary<object, byte> cd = new System.Collections.Concurrent.ConcurrentDictionary<object, byte>();

	void TestConcurrentSpeed() {
		var a = new object[100];
		for (int i = 0; i < a.Length; i++) a[i] = new object();

		perf.cpu();
		for (int i1 = 0; i1 < 7; i1++) {
			int n2 = a.Length;
			perf.first();
			for (int i2 = 0; i2 < n2; i2++) { hs.Add(a[i2]); }
			for (int i2 = 0; i2 < n2; i2++) { hs.Remove(a[i2]); }
			perf.next();
			for (int i2 = 0; i2 < n2; i2++) { lock (hs2) hs2.Add(a[i2]); } //20% slower
			for (int i2 = 0; i2 < n2; i2++) { lock (hs2) hs2.Remove(a[i2]); }
			perf.next();
			for (int i2 = 0; i2 < n2; i2++) { cd.TryAdd(a[i2], default); } //50% slower
			for (int i2 = 0; i2 < n2; i2++) { cd.TryRemove(a[i2], out _); }
			perf.next();
			for (int i2 = 0; i2 < n2; i2++) { var gc = GCHandle.Alloc(a[i2]); gc.Free(); } //50% faster
			perf.nw();
			Thread.Sleep(100);
		}
	}

	unsafe static class _Api
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool VirtualLock(void* lpAddress, nint dwSize);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool VirtualUnlock(void* lpAddress, nint dwSize);

	}

	//	unsafe void TestMemoryAllocSpeed()
	//	{
	//		byte[] a;
	//		byte* p1 = (byte*)Au.More.MemoryUtil.Alloc(1000); Au.More.MemoryUtil.Free(p1);
	//		byte* p2 = (byte*)Api.VirtualAlloc(default, 1000); Api.VirtualFree(p2);
	//		int n = 10_000_000;
	//		//n = 100_000_000;
	//		n = 0x10000;

	//		perf.cpu();
	//		for(int i1 = 0; i1 < 10; i1++) {
	//			perf.first();

	//			//	a = new byte[n];
	//			//	perf.next();
	//			//	fixed(byte* bp = a) {
	//			//		var ip = (long*)bp; for(int j = 0; j < n / 8; j++) ip[j] = j;
	//			//	}

	//			//perf.next();

	//			//	p1 = (byte*)Au.More.MemoryUtil.Alloc(n);
	//			//	perf.next();
	//			//	var ip = (long*)p1; for(int j = 0; j < n / 8; j++) ip[j] = j;
	//			//	perf.next();
	//			//	Au.More.MemoryUtil.Free(p1);

	//			//perf.next();

	//			//	//p2 = (byte*)Api.VirtualAlloc(default, n);
	//			//	p2 = (byte*)Api.VirtualAlloc(default, n, Api.MEM_COMMIT | Api.MEM_RESERVE, Api.PAGE_READWRITE);
	//			//	perf.next();
	//			//	var ip = (long*)p2; for(int j = 0; j < n / 8; j++) ip[j] = j;
	//			//	perf.next();
	//			//	Api.VirtualFree(p2);

	////			//test realloc speed
	////			p1 = (byte*)Au.More.MemoryUtil.Alloc(n);
	////			perf.next();
	////			var ip = (long*)p1; for(int j = 0; j < n / 8; j++) ip[j] = j;
	////			perf.next();
	////#if true
	////			Au.More.MemoryUtil.Free(p1);
	////			var p1c = (byte*)Au.More.MemoryUtil.Alloc(n + 20000);
	////#else
	////			var p1c = (byte*)Au.More.MemoryUtil.ReAlloc(p1, n + 20000);
	////#endif
	////			perf.next();
	////			Au.More.MemoryUtil.Free(p1c);

	//			perf.nw();
	//			//print.it(p1c == p1);
	//			Thread.Sleep(100);
	//		}
	//	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
	bool TestArrayExt(string[] a) {
		return a.NE_();
	}
	//int TestArrayExt(string[] a)
	//{
	//	return a.Len_();
	//}

	/// <summary>
	/// <para>List:</para>
	/// <list type="bullet">
	/// <item>ONE.</item>
	/// <item>TWO.</item>
	/// </list>
	/// 
	/// <p>List:</p>
	/// <ul>
	/// <li>one.</li>
	/// <li>two.</li>
	/// </ul>
	/// </summary>
	void TestMarkdownXmlDocComments() {
		string markdown = @"List:
- one.
- two.

<para>List:</para>
<list type=""bullet"">
<item>ONE.</item>
<item>TWO.</item>
</list>

<p>List:</p>
<ul>
<li>one.</li>
<li>two.</li>
</ul>

";

		string html = Markdig.Markdown.ToHtml(markdown);
		html = html.RegexReplace(@"(?m)^", "/// ");
		print.it(html);
	}

	//void TestMenuDropdownBug()
	//{
	//	bool test = keys.isScrollLock;
	//	{
	//		var m = new ToolStripDropDownMenu();
	//		var k = m.Items;
	//		k.Add("0");
	//	}
	//	for(int j = 0; j < 1; j++) {
	//		Debug_.MemorySetAnchor_();

	//		perf.first();
	//		var m = new ToolStripDropDownMenu();
	//		var k = m.Items;
	//		if(test) m.ShowItemToolTips = false;

	//		for(int i = 0; i < 100; i++) {
	//			k.Add("0");
	//		}
	//		perf.next();
	//		//foreach(ToolStripMenuItem v in m.Items) {
	//		//	if(v.HasDropDown) print.it("has");
	//		//}
	//		//perf.next();
	//		//if(test) {
	//		//	m.ShowItemToolTips = true;
	//		//	perf.next();
	//		//}

	//		Debug_.MemoryPrint_();
	//		perf.write();

	//		//m.Show();

	//		//Debug_.MemoryPrint_();

	//		100.ms();
	//	}
	//}

	//class Implement : IDisposable
	//{
	//	int i;
	//}

	//class Implement2 : IEnumerable<int>
	//{

	//}

	//public interface ITest : ITestBase
	//{
	//	bool PropDef => true;
	//	bool Prop { get; set; }
	//	bool PropGet { get; }
	//	bool PropSet { set; }
	//	bool Meth(int i, out RECT r);
	//	void MethDef(int j) { }
	//	event EventHandler Event;
	//	int this[int index] { get; set; }
	//	bool Prop2 { get; protected set; }
	//	ref int Ref();
	//	internal bool PropInternal { get; set; }
	//	//	protected bool PropInternal { get; set; }

	//	const int Const = 5;
	//	static ITest() { }
	//	public static ITest operator +(ITest a, ITest b) => default;
	//	public struct Struct { int i; }
	//	static int s_i;
	//	public static void StatMeth() { }
	//	//	void ITestBase.OfBase() {  }
	//}

	//public interface ITestBase
	//{
	//	void OfBase();
	//}

	//class Implement3 : ITest
	//{
	//}

	//void One(bool two) { }
	//bool Two() => false;

	void TestFirefoxElm() {
		try {
			//var w = +wnd.find("*- Mozilla Firefox", "MozillaWindowClass");
			//var a = elm.fromWindow(w);

			var w = +wnd.find("Au automation library and editor | Au - Mozilla Firefox", "MozillaWindowClass");
			//var a = +elm.find(w, "web:LINK", "Library");
			var a = +elm.find(w, "web:DOCUMENT");
			a = +a.Navigate("ch2");
			//a = +a.Navigate("fi ne fi4 ne fi ne fi2");
			//a=a.Find("LINK", "Library");

			print.it(a.MiscFlags, a);

			if (a.GetProperties("@", out var p)) {
				print.it(p.HtmlAttributes);
			}

		}
		finally {
#if DEBUG
			Cpp.DebugUnload();
#endif
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	void TestUnsafe() {
		//var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
		//ulong k = Unsafe.As<byte, ulong>(ref a[0]);
		//print.it(k);

		//a[2] = Unsafe.As<ulong, byte>(ref k);
		//print.it(a);

		var f = keys.isScrollLock ? FAFlags.DontThrow : FAFlags.UseRawPath;
		//f.SetFlag2(EFFlags.Reverse, true);
		//print.it(f);
		//f.SetFlag2(EFFlags.Reverse, false);
		//print.it(f);

		int k1 = 0, k2 = 0, k3 = 0, k4 = 0, k5 = 0;
		perf.cpu();
		//if(f.HasAny(EFFlags.HiddenToo)) k3++;
		for (int i1 = 0; i1 < 7; i1++) {
			int n2 = 10000;
			perf.first();
			for (int i2 = 0; i2 < n2; i2++) { f.SetFlag(FAFlags.DontThrow, true); f.SetFlag(FAFlags.DontThrow, false); }
			perf.next();
			for (int i2 = 0; i2 < n2; i2++) { if (f.HasFlag(FAFlags.UseRawPath)) k1++; }
			perf.next();
			for (int i2 = 0; i2 < n2; i2++) { if (f.Has(FAFlags.UseRawPath)) k2++; }
			perf.next();
			for (int i2 = 0; i2 < n2; i2++) { if (f.HasAny(FAFlags.UseRawPath)) k3++; }
			//perf.next();
			//for(int i2 = 0; i2 < n2; i2++) { if(f.HasAny5(EFFlags.HiddenToo)) k5++; }
			perf.nw();
			Thread.Sleep(200);
		}
		print.it(f, k1, k2, k3, k4, k5);
	}

	void TestNetCoreVersion() {
		print.it(folders.NetRuntime);
	}

	[StructLayout(LayoutKind.Explicit, Size = 500000)]
	struct BIGBIG { public override string ToString() => "TEST"; }
	void TestStringInterpolationBoxing() {
		BIGBIG r = default;
		Debug_.MemorySetAnchor_();
		var s = $"a {r}?"; //result: boxes, although .ToString() causes an IDE suggestion to remove it
		Debug_.MemoryPrint_();
		print.it(s);
	}

	void TestMinusSign() {
		print.clear();
		var v = -5;
		var s = v.ToString();
		print.it((uint)s[0]);
		print.it(Convert.ToInt32(s), int.Parse(s), s.ToInt());
		s = "-6";
		print.it((uint)s[0]);
		print.it(Convert.ToInt32(s), int.Parse(s), s.ToInt());

	}

	unsafe void TestNint() {
		//int i = -100;
		//nint n = i;
		//var s1 = n.ToString();
		//var s2 = n.ToStringInvariant();
		//print.it(n, s1, s2, (uint)s1[0], (uint)s2[0]);

		////int i =unchecked((int) uint.MaxValue);
		long i = long.MaxValue;
		//nuint n = (nuint)i;

		//IntPtr n = (IntPtr)i;
		nint n = (nint)i;
		int k = (int)n;
		print.it(n, k);

		//nint n = (uint)8;
		//nint n = (IntPtr)5;
		//IntPtr p = n;
		//n = (nint)(void*)null;

		//Coord c = 6;
	}

	void TestWpfWindow() {
		//var w = new Au.Tests.MainWindow();
		var w = new Au.Tests.Window1();
		//var w = new Window();
		w.ShowDialog();
	}

	//System.PlatformNotSupportedException: System.Management currently is only supported for Windows desktop applications.
//	void TestWMI() {
//print.clear();

//var interval = new TimeSpan( 0, 0, 1 );
//const string isWin32Process = "TargetInstance isa \"Win32_Process\"";

//// Listen for started processes.
//WqlEventQuery startQuery
//    = new WqlEventQuery( "__InstanceCreationEvent", interval, isWin32Process );
//var _startWatcher = new ManagementEventWatcher( startQuery );
//_startWatcher.Start();
//_startWatcher.EventArrived += (_,e)=> { print.it("start", e.Context, e.NewEvent); };

//// Listen for closed processes.
//WqlEventQuery stopQuery
//    = new WqlEventQuery( "__InstanceDeletionEvent", interval, isWin32Process );
//var _stopWatcher = new ManagementEventWatcher( stopQuery );
//_stopWatcher.Start();
//_stopWatcher.EventArrived += (_,e)=> { print.it("end", e.Context, e.NewEvent); };


//dialog.show("");
//	}

	unsafe void _Main() {
		//Application.SetCompatibleTextRenderingDefault(false);
		//print.it("before");
		//Debug_.WriteLoadedAssemblies(true, true, true);


		//TestWMI();
		//new Script().Test();
		//TestSvg();
		//TestWpfWindow();
		//TestNint();
		//TestMinusSign();
		//TestStringInterpolationBoxing();
		//TestNetCoreVersion();
	}

	[STAThread] static void Main(string[] args) { new TestScript(args); }
	TestScript(string[] args) {
		print.qm2.use = true;
		//print.clear();

		//perf.first();
		try {
			_Main();
		}
		catch (Exception ex) { print.it(ex); }
	}
}
#endif
