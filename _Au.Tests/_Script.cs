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
using System.Windows.Forms;
using System.Drawing;
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
//using AutoItX3Lib;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Windows.Forms.VisualStyles;

using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.Core.Entities;

using Au.Triggers;
using Au.Controls;
#endif

using Au;
using Au.Types;
using static Au.AStatic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

[module: System.Runtime.InteropServices.DefaultCharSet(System.Runtime.InteropServices.CharSet.Unicode)]

class Script : AScript
{

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
		//return;
		if(!s_debug2) {
			s_debug2 = true;
			new TestGC();

			//ATimer.Every(50, _ => {
			//	if(!s_debug) {
			//		s_debug = true;
			//		ATimer.After(100, _ => new TestGC());
			//	}
			//});
		}
	}

	//unsafe class MapArray
	//{
	//	public int[] _a;
	//	public Vector128<int>[] _v;

	//	public MapArray(int n)
	//	{
	//		_a = new int[n];
	//		for(int i = 0; i < _a.Length; i++) _a[i] = i;

	//		_v = new Vector128<int>[n];
	//	}

	//	public void Move(int i)
	//	{
	//		int n = _a.Length - i - 1;
	//		Array.Copy(_a, i, _a, i + 1, n);

	//		//fixed(int* p = _a) Api.memmove(p + i + 1, p + 1, n * 4); //same speed
	//	}

	//	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
	//	public void Inc(int i, int add)
	//	{
	//		//for(; i < _a.Length; i++) _a[i]+=add;

	//		long add2 = add; add2 = add2 << 32 | add2;
	//		fixed(int* ip = _a) {
	//			var p = (long*)ip;
	//			for(int n = _a.Length / 2; i < n; i++) {
	//				//var v = p[i];
	//				p[i] += add2;
	//			}

	//		}

	//		//var va = Vector128.Create(add);
	//		//for(;  i < _v.Length; i++) {
	//		//	_v[i]=Sse2.Add(_v[i], va);
	//		//}
	//	}

	//	//public void Insert(int i, int add)
	//	//{
	//	//	for(; i < _a.Length; i++) _a[i]+=add;
	//	//}

	//	public void PrintVector()
	//	{
	//		Print(_a);

	//		//for(int i=0; i < _v.Length; i++) {
	//		//	Print(_v[i].GetElement(0), _v[i].GetElement(1), _v[i].GetElement(2), _v[i].GetElement(3));
	//		//}
	//	}
	//}

	class JSettings
	{
		public string OneTwo { get; set; }
		public int ThreeFour { get; set; }
		public int Five { get; set; }
		public bool Six { get; set; }
		public string Seven { get; set; }
		public string Eight { get; set; } = "def";
	}

	//void TestJson()
	//{
	//	var file = @"Q:\test\sett.json";
	//	var file2 = @"Q:\test\sett.xml";

	//	var v = new JSettings { OneTwo = "text ąčę", ThreeFour = 100 };

	//	for(int i = 0; i < 5; i++) {
	//		//100.ms();
	//		//APerf.First();
	//		//var k1 = new JsonSerializerOptions { IgnoreNullValues = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };
	//		//var b1 = JsonSerializer.SerializeToUtf8Bytes(v, k1);
	//		//APerf.Next();
	//		//File.WriteAllBytes(file, b1);
	//		//APerf.NW();

	//		100.ms();
	//		APerf.First();
	//		var b2 = File.ReadAllBytes(file);
	//		APerf.Next();
	//		var k2 = new JsonSerializerOptions { IgnoreNullValues = true };
	//		APerf.Next();
	//		v = JsonSerializer.Deserialize<JSettings>(b2, k2);
	//		APerf.NW('J');
	//	}

	//	for(int i = 0; i < 5; i++) {
	//		//100.ms();
	//		//APerf.First();
	//		//var r1 = new XElement("r");
	//		//r1.Add(new XElement("OneTwo", v.OneTwo));
	//		//r1.Add(new XElement("ThreeFour", v.ThreeFour.ToString()));
	//		//APerf.Next();
	//		//r1.Save(file2);
	//		//APerf.NW();

	//		100.ms();
	//		APerf.First();
	//		var r2 = XElement.Load(file2);
	//		APerf.Next();
	//		v = new JSettings();
	//		v.OneTwo = r2.Element("OneTwo").Value;
	//		var s2 = r2.Element("ThreeFour").Value;
	//		APerf.NW('X');
	//		v.ThreeFour = s2.ToInt();
	//	}

	//	Print(v.OneTwo, v.ThreeFour, v.Five, v.Six, v.Seven, v.Eight);

	//	//JsonDocument d; d.RootElement.
	//}

	//[DllImport("CppE")]
	//static extern int Cpp_Install(int step, string dir);

	//[DllImport("CppE")]
	//static extern int Cpp_Uninstall();


	void TestMenu()
	{
		var m = new AMenu();
		m["One"] = o => Print(o);
		m["Two"] = o => Print(o);
		m.LazySubmenu("Submenu 1", _ => {
			Print("adding items of " + m.CurrentAddMenu.OwnerItem);
			m["Three"] = o => Print(o);
			m["Four"] = o => Print(o);
			m.LazySubmenu("Submenu 2", _ => {
				Print("adding items of " + m.CurrentAddMenu.OwnerItem);
				m["Five"] = o => Print(o);
				m["Six"] = o => Print(o);
			});
			m["Seven"] = o => Print(o);
		});
		m["Eight"] = o => Print(o);
		m.Show();

	}

	//void TestMenu2()
	//{
	//	var m = new AMenu();
	//	m["One"] = o => Print(o);
	//	m["Two"] = o => Print(o);
	//	m.LazySubmenu("Submenu 1").Fill = _ => {
	//		Print("adding items of " + m.CurrentAddMenu.OwnerItem);
	//		m["Three"] = o => Print(o);
	//		m["Four"] = o => Print(o);
	//		m.LazySubmenu("Submenu 2", _ => {
	//			Print("adding items of " + m.CurrentAddMenu.OwnerItem);
	//			m["Five"] = o => Print(o);
	//			m["Six"] = o => Print(o);
	//		});
	//		m["Seven"] = o => Print(o);
	//	};
	//	m["Eight"] = o => Print(o);
	//	m.Show();

	//}

	//void TestMenu2()
	//{
	//	var m = new AMenu();
	//	m["One"] = o => Print(o);
	//	m["Two"] = o => Print(o);
	//	m.LazySubmenu("Submenu 1");
	//	m.LazyFill = _ => {
	//		Print("adding items of " + m.CurrentAddMenu.OwnerItem);
	//		m["Three"] = o => Print(o);
	//		m["Four"] = o => Print(o);
	//		m.LazySubmenu("Submenu 2", _ => {
	//			Print("adding items of " + m.CurrentAddMenu.OwnerItem);
	//			m["Five"] = o => Print(o);
	//			m["Six"] = o => Print(o);
	//		});
	//		m["Seven"] = o => Print(o);
	//	};
	//	m["Eight"] = o => Print(o);
	//	m.Show();

	//}

#if false
	void TestToolbar()
	{
		for(int i = 0; i < 1; i++) {
			var t = new AToolbar("123");
			//t.NoText = true;
			//t.Border= TBBorder.Sizable3;t.Control.Text = "Toolbar";
			//t.Border = TBBorder.SizableWithCaptionX;

			//t["Find", @"Q:\app\find.ico"] = o => Print(o);
			//t["Copy", @"Q:\app\copy.ico"] = o => Print(o);
			//t.Separator("Tpi group");
			//t["Delete", @"Q:\app\delete.ico"] = o => Print(o);
			//t["No image"] = o => Print(o);
			//t["TT", tooltip: "WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW "] = o => Print(o);
			////t.LastButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			////t.LastButton.AutoToolTip = false;
			////t.LastButton.ToolTipText = "ggg";
			//t.Separator();
			//t["Run", @"Q:\app\run.ico"] = o => Print(o);
			//t.Separator("");
			//t["Paste text", @"Q:\app\paste.ico"] = o => Print(o);
			//t.LastButton.ToolTipText = "Toooooltip";

			//t.ExtractIconPathFromCode = true;
			//t["Auto icon"] = o => Print("notepad.exe");
			//t["Failed icon", @"Q:\app\-.ico"] = o => Print(o);
			////t.LastButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			////t.Separator("");
			////t.Add(new ToolStripTextBox { ToolTipText= "ToolStripTextBox", AutoSize=false, Width=50 });
			////t.Add(new ToolStripComboBox { ToolTipText= "ToolStripComboBox", AutoSize=false, Width=50 });
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripButton("aaa"));
			////t.Add(new ToolStripButton("bbb"));
			////t["Multi\r\nline"] = o => Print(o);

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
			dd.DropDownOpening += (unu, sed) => {
				var m = new AMenu(dd);
				m["one"] = o => Print(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => Print(o);
				}
			};
			var sb = new ToolStripSplitButton("SB");
			t.Add(sb, @"Q:\app\copy.ico", o => Print(o));
#elif true
			//t.Control.Font = new Font("Courier New", 16);
			//t.Control.RightToLeft = RightToLeft.Yes;
			t.MenuButton("DD", m => {
				Print("dd");
				//m.MultiShow = false;
				m["one"] = o => Print(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => Print(o);
				}
			}, @"Q:\app\find.ico", "MenuButton");
			t.SplitButton("SB", m => {
				m["one"] = o => Print(o);
				//var sb = m.Control.OwnerItem as ToolStripSplitButton;
				//Print(sb);
				//sb.DefaultItem = m.LastItem;
				using(m.Submenu("Sub")) {
					m["si"] = o => Print(o);
				}
			}, @"Q:\app\copy.ico", "SplitButton", o => Print(o));
			t.Separator("");
			t[true, "DD2", @"Q:\app\delete.ico"] = m => {
				Print("create menu");
				//m.MultiShow = false;
				m["one"] = o => Print(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => Print(o);
				}
			};
			//t.SplitButton("SB", o => {
			//	Print(o);
			//}, m => {
			//	m["one"] = o => Print(o);
			//	using(m.Submenu("Sub")) {
			//		m["si"] = o => Print(o);
			//	}
			//}, @"Q:\app\copy.ico", "SplitButton");
			//Action<AMenu> menu1 = m => {
			//	m["one"] = o => Print(o);
			//	using(m.Submenu("Sub")) {
			//		m["si"] = o => Print(o);
			//	}
			//};
			//t.MenuButton("DD", menu1, @"Q:\app\find.ico", "MenuButton");
#elif false
			t.MenuButton("DD", @"Q:\app\find.ico");
			t.Menu = m => {
				m["one"] = o => Print(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => Print(o);
				}
			};
#else
			t.MenuButton("DD", @"Q:\app\find.ico").Menu = m => {
				Print("dd");
				//m.MultiShow = false;
				m["one"] = o => Print(o);
				using(m.Submenu("Sub")) {
					m["two"] = o => Print(o);
				}
			};
			t.SplitButton("SB", o => Print(o), @"Q:\app\copy.ico").Menu = m => {
				Print("dd");
				m["one"] = o => Print(o);
				using(m.Submenu("Sub")) {
					m["two"] = o => Print(o);
				}
			};
#endif
			//t.Separator("");
			////t["GC"] = o => GC.Collect();

			//var dd = new ToolStripSplitButton("SB2", null, (unu,sed)=>Print("click"));
			//t.Add(dd, @"Q:\app\delete.ico");
			//dd.DropDownOpening += (unu, sed) => {
			//	var m = new AMenu();
			//	dd.DropDown = m.Control;
			//	m["one"] = o => Print(o);
			//};
			//dd.ButtonClick += (unu, sed) => Print("button click");
			//dd.DoubleClickEnabled = true;
			//dd.ButtonDoubleClick += (unu, sed) => Print("button double click");

			//ATimer.After(3000, _ => {
			//	var c = t.Control.Items[0];
			//	c.Select();
			//});

			//void _B(TBBorder b){
			//	t.Border = b;
			//	//Print(AWnd.More.BorderWidth((AWnd)t.Control));
			//}

			//t.Bounds = new Rectangle(i * 300 + 700, 200, 200, 200);
			t.Show();
			//t.Window.ActivateLL();
			ATime.SleepDoEvents(200);

			//for(int j = 1; j <= (int)TBBorder.SizableWithCaptionX; j++) {
			//	ATime.SleepDoEvents(1000);
			//	t.Border = (TBBorder)j;
			//}

			//ATime.SleepDoEvents(1000);
			//t.Border = TBBorder.FixedWithCaption;
			//ATime.SleepDoEvents(3000);
			//t.Border = TBBorder.SizableWithCaption;

			//var m = new AMenu();
			//using(m.Submenu("Sub")) {

			//}
			//m.Show()
		}

		//var c = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(VisualStyleElement.Window.FrameLeft.Inactive).GetColor(ColorProperty.BorderColor);
		//Print((uint)c.ToArgb());

		//ATimer.After(500, _ => {
		//	var w = (AWnd)t.Control;
		//	//w.SetStyle(WS.DLGFRAME, SetAddRemove.Add);
		//});

		ADialog.Options.TopmostIfNoOwnerWindow = true;
		ADialog.Show();

		//ATimer.After(10000, _ => Application.Exit());
		//Application.Run();
	}
#endif

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//void TestCallerArgumentExpression(string so, [CallerArgumentExpression("so")] string ca = null) //does not work
	//{
	//	Print(so, ca);
	//}

	//internal static bool ClientToScreen4(AWnd w, ref POINT p)
	//{
	//	int x0=p.x;
	//	if(!Api.MapWindowPoints(w, default, ref p, out int k)) return false;
	//	Print(p.x-x0, AMath.LoShort(k));
	//	//p.Offset(r.left, r.top);
	//	return true;
	//}

	//void TestClientToScreenRtlAware()
	//{

	//	ATimer.After(500, _ => {
	//		var w = AWnd.Find(null, "#32770").OrThrow();
	//		//RECT r;
	//		//Print(w.GetClientRect(out r), r);
	//		//Print(w.GetClientRect2(out r), r);
	//		//Print(w.GetClientRect(out r, true), r);
	//		//Print(w.GetClientRect2(out r, true), r);

	//		//POINT p = (1, 1);
	//		//Api.ClientToScreen(w, ref p);
	//		//Print(p);


	//		//p = (1, 1);
	//		//Print(Api.ClientToScreen2(w, ref p));
	//		//Print(p);

	//		//p = (1, 1);
	//		//Print(Api.ClientToScreen3(w, ref p));
	//		//Print(p);

	//		////p = (1, 1);
	//		////Print(ClientToScreen4(w, ref p));
	//		////Print(p);
	//		////return;

	//		var a = AWnd.GetWnd.AllWindows(true); Print(a.Length);
	//		//int ir = Array.FindIndex(a, o => o.HasExStyle(WS2.LAYOUTRTL)); //Print(ir);
	//		APerf.SpeedUpCpu();
	//		for(int i1 = 0; i1 < 7; i1++) {
	//			//p = (1, 1); Api.ClientToScreen(w, ref p);
	//			//p = (1, 1); Api.ClientToScreen2(w, ref p);
	//			//p = (1, 1); Api.ClientToScreen3(w, ref p);
	//			//foreach(var k in a) { p = (1, 1); Api.ClientToScreen(k, ref p); }
	//			foreach(var k in a) k.GetClientRect(out r, false);
	//			APerf.First();
	//			//foreach(var k in a) { p = (1, 1); Api.ClientToScreen(k, ref p); }
	//			//APerf.Next();
	//			//foreach(var k in a) { p = (1, 1); Api.ClientToScreen2(k, ref p); }
	//			//APerf.Next();
	//			//foreach(var k in a) { p = (1, 1); Api.ClientToScreen3(k, ref p); }
	//			//foreach(var k in a) k.GetClientRect(out r, false);
	//			//APerf.Next();
	//			//foreach(var k in a) k.GetClientRect2(out r, false);
	//			//APerf.Next();
	//			//foreach(var k in a) k.GetClientRect(out r, true);
	//			//APerf.Next();
	//			//foreach(var k in a) k.GetClientRect2(out r, true);
	//			APerf.NW();
	//			Thread.Sleep(100);
	//		}


	//	});


	//	ADialog.Options.TopmostIfNoOwnerWindow = true;
	//	ADialog.Options.RtlLayout = true;
	//	ADialog.Show("text");

	//}

	static void Bug1(string s=null, Func<Screen, bool> f = null) { }

	[MethodImpl(MethodImplOptions.NoInlining)]
	void TestAScreen()
	{
		var w = AWnd.Active;
		var s = new AScreen(w);
		var d = s.GetScreenHandle();
		Print(d);

		//var w = AWnd.Find("*Notepad", also: o => o.Screen.Index == 1);
		//Bug1("", f: o=>o.)
		//Bug1(f: o=>o.)

		//ThreadPool.QueueUserWorkItem(_ => Print(1));
		//500.ms();
		//APerf.First();
		//var a = AScreen.AllScreens;
		//APerf.NW();
		//Print(a);
		//500.ms();
		////while(!AKeys.IsCtrl) 100.ms();

		////AScreen.Of(w).

		//var w = AWnd.Active;
		//AWnd.Active.Move(0, 0, 0, 0, false, screen: 1);
		////AWnd.Active.Move(0, 0, 0, 0, false, screen: w);
		////AWnd.Active.Move(0, 0, 0, 0, false, (AScreen)w);
		//AWnd.Active.Move(0, 0, 0, 0, false, new AScreen(w));
		//AWnd.Active.Move(0, 0, 0, 0, false, AScreen.Of(w));

		//new AScreen(() => AScreen.Of(AMouse.XY));
		//new AScreen(() => AScreen.Of(AWnd.Active));

		//Screen sco = null; new AScreen(sco);
		//AWnd.Active.Move(0, 0, 0, 0, false, screen: sco);

		//RECT r = AScreen.Of(AWnd.Active).Bounds;

		//Print(HScreen.AllScreens);
		////Print(Screen.AllScreens);
		//Print(HScreen.FromIndex(0), HScreen.FromIndex(1), HScreen.FromIndex(2), HScreen.FromIndex(3));
		//foreach(var v in Screen.AllScreens) Print(v.GetIndex(), (HScreen)v);
		//foreach(var v in HScreen.AllScreens) Print(v.Index, (Screen)v);
		//bool two = AKeys.IsScrollLock;

		//var screenFromMouse = new HScreen(() => HScreen.FromPoint(AMouse.XY));
		//var screenFromActiveWindow = new HScreen(() => HScreen.FromWindow(AWnd.Active));
		//Print(screenFromMouse, screenFromActiveWindow);

		//while(ADialog.ShowOKCancel("test")) {
		//	two = AKeys.IsScrollLock;
		//	200.ms();
		//	//APerf.SpeedUpCpu();
		//	APerf.First();
		//	if(two) { _ = HScreen.AllScreens; APerf.Next(); _ = HScreen.AllScreens; }
		//	else { _ = Screen.AllScreens; APerf.Next(); _ = Screen.AllScreens; }
		//	APerf.NW();
		//}

		//APerf.SpeedUpCpu();
		//for(int i1 = 0; i1 < 7; i1++) {
		//	int n2 = 1;
		//	APerf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = HScreen.AllScreens; }
		//	APerf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _ = two ? HScreen.Primary.Info.bounds : AScreen2.PrimaryRect; }
		//	APerf.NW();
		//	Thread.Sleep(100);
		//}

		//var h = HScreen.Primary;
		////h = default;
		////h = new HScreen((IntPtr)12345678);
		//h = HScreen.FromWindow(AWnd.Active, SDefault.Nearest);
		//var v = h.GetInfo();
		//Print(v, h.IsAlive);
		//Print(h == HScreen.Primary);
	}

	//[DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", SetLastError =true)]
	//internal static extern IntPtr LoadLibraryEx(string lpLibFileName, IntPtr hFile, uint dwFlags);
	//void TestFailsWpf()
	//{
	//	//if(default==Api.LoadLibrary(@"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.0\wpfgfx_cor3.dll"))
	//	//	Print(ALastError.Message);
	//	//if(default==LoadLibraryEx(@"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.0\wpfgfx_cor3.dll", default,
	//	//	0x1100))
	//	//	Print(ALastError.Message);

	//	var f = new Window { Left = 500, Top = 1500 };
	//	f.Show();

	//	//Print(Api.LoadLibrary(@"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.0\wpfgfx_cor3.dll"));

	//	ADialog.ShowEx(secondsTimeout: 2);
	//}

	//void TestAWndFromObject()
	//{
	//	//var f = new Form { StartPosition = FormStartPosition.Manual, Left = 500, Top = 1500 };
	//	var f = new Window { Left = 500, Top = 1500 };
	//	f.Show();

	//	var v = new AScreen(f);
	//	Print(v.ToDevice());

	//	//ADialog.ShowEx(secondsTimeout: 2);
	//	ADialog.ShowEx(owner: f);
	//}

	unsafe void _Main()
	{
		//Application.SetCompatibleTextRenderingDefault(false);
		//Print("before");
		//ADebug.PrintLoadedAssemblies(true, true, true);

		//Triggers.Window[Au.Triggers.TWEvent.ActiveOnce, "*Notepad"]
		//One()

		//TestAScreen();
		//TestClientToScreenRtlAware();
		//TestToolbar();
		//TestMenu();
		//TestCallerArgumentExpression("FF" + 5); var v = "gg"; TestCallerArgumentExpression(v);
	}

	void One(bool two) { }
	bool Two() => false;

	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();
		Au.Util.LibAssertListener.Setup();

		//APerf.First();
		try {
			_Main();
		}
		catch(Exception ex) { Print(ex); }
	}
}
