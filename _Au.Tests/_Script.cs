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
using System.Windows;
using System.Drawing.Imaging;
using System.Collections;

using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using System.Diagnostics.CodeAnalysis;

using Au.Triggers;
using Au.Controls;
#endif

using Au;
using Au.Types;

[module: System.Runtime.InteropServices.DefaultCharSet(System.Runtime.InteropServices.CharSet.Unicode)]

class Script : AScript
{

	class TestGC
	{
		~TestGC()
		{
			if(Environment.HasShutdownStarted) return;
			if(AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			AOutput.Write("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
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

	//	public void AOutput.WriteVector()
	//	{
	//		AOutput.Write(_a);

	//		//for(int i=0; i < _v.Length; i++) {
	//		//	AOutput.Write(_v[i].GetElement(0), _v[i].GetElement(1), _v[i].GetElement(2), _v[i].GetElement(3));
	//		//}
	//	}
	//}

	//class JSettings
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

	//	AOutput.Write(v.OneTwo, v.ThreeFour, v.Five, v.Six, v.Seven, v.Eight);

	//	//JsonDocument d; d.RootElement.
	//}

	//[DllImport("CppE")]
	//static extern int Cpp_Install(int step, string dir);

	//[DllImport("CppE")]
	//static extern int Cpp_Uninstall();


	void TestMenu()
	{
		var m = new AMenu();
		m["One"] = o => AOutput.Write(o);
		m["Two"] = o => AOutput.Write(o);
		m.LazySubmenu("Submenu 1", _ => {
			AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
			m["Three"] = o => AOutput.Write(o);
			m["Four"] = o => AOutput.Write(o);
			m.LazySubmenu("Submenu 2", _ => {
				AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
				m["Five"] = o => AOutput.Write(o);
				m["Six"] = o => AOutput.Write(o);
			});
			m["Seven"] = o => AOutput.Write(o);
		});
		m["Eight"] = o => AOutput.Write(o);
		m.Show();

	}

	//void TestMenu2()
	//{
	//	var m = new AMenu();
	//	m["One"] = o => AOutput.Write(o);
	//	m["Two"] = o => AOutput.Write(o);
	//	m.LazySubmenu("Submenu 1").Fill = _ => {
	//		AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
	//		m["Three"] = o => AOutput.Write(o);
	//		m["Four"] = o => AOutput.Write(o);
	//		m.LazySubmenu("Submenu 2", _ => {
	//			AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
	//			m["Five"] = o => AOutput.Write(o);
	//			m["Six"] = o => AOutput.Write(o);
	//		});
	//		m["Seven"] = o => AOutput.Write(o);
	//	};
	//	m["Eight"] = o => AOutput.Write(o);
	//	m.Show();

	//}

	//void TestMenu2()
	//{
	//	var m = new AMenu();
	//	m["One"] = o => AOutput.Write(o);
	//	m["Two"] = o => AOutput.Write(o);
	//	m.LazySubmenu("Submenu 1");
	//	m.LazyFill = _ => {
	//		AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
	//		m["Three"] = o => AOutput.Write(o);
	//		m["Four"] = o => AOutput.Write(o);
	//		m.LazySubmenu("Submenu 2", _ => {
	//			AOutput.Write("adding items of " + m.CurrentAddMenu.OwnerItem);
	//			m["Five"] = o => AOutput.Write(o);
	//			m["Six"] = o => AOutput.Write(o);
	//		});
	//		m["Seven"] = o => AOutput.Write(o);
	//	};
	//	m["Eight"] = o => AOutput.Write(o);
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

			//t["Find", @"Q:\app\find.ico"] = o => AOutput.Write(o);
			//t["Copy", @"Q:\app\copy.ico"] = o => AOutput.Write(o);
			//t.Separator("Tpi group");
			//t["Delete", @"Q:\app\delete.ico"] = o => AOutput.Write(o);
			//t["No image"] = o => AOutput.Write(o);
			//t["TT", tooltip: "WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW WWWWWWWWWWWW "] = o => AOutput.Write(o);
			////t.LastButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			////t.LastButton.AutoToolTip = false;
			////t.LastButton.ToolTipText = "ggg";
			//t.Separator();
			//t["Run", @"Q:\app\run.ico"] = o => AOutput.Write(o);
			//t.Separator("");
			//t["Paste text", @"Q:\app\paste.ico"] = o => AOutput.Write(o);
			//t.LastButton.ToolTipText = "Toooooltip";

			//t.ExtractIconPathFromCode = true;
			//t["Auto icon"] = o => AOutput.Write("notepad.exe");
			//t["Failed icon", @"Q:\app\-.ico"] = o => AOutput.Write(o);
			////t.LastButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			////t.Separator("");
			////t.Add(new ToolStripTextBox { ToolTipText= "ToolStripTextBox", AutoSize=false, Width=50 });
			////t.Add(new ToolStripComboBox { ToolTipText= "ToolStripComboBox", AutoSize=false, Width=50 });
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripTextBox());
			////t.Add(new ToolStripButton("aaa"));
			////t.Add(new ToolStripButton("bbb"));
			////t["Multi\r\nline"] = o => AOutput.Write(o);

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
				m["one"] = o => AOutput.Write(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => AOutput.Write(o);
				}
			};
			var sb = new ToolStripSplitButton("SB");
			t.Add(sb, @"Q:\app\copy.ico", o => AOutput.Write(o));
#elif true
			//t.Control.Font = new Font("Courier New", 16);
			//t.Control.RightToLeft = RightToLeft.Yes;
			t.MenuButton("DD", m => {
				AOutput.Write("dd");
				//m.MultiShow = false;
				m["one"] = o => AOutput.Write(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => AOutput.Write(o);
				}
			}, @"Q:\app\find.ico", "MenuButton");
			t.SplitButton("SB", m => {
				m["one"] = o => AOutput.Write(o);
				//var sb = m.Control.OwnerItem as ToolStripSplitButton;
				//AOutput.Write(sb);
				//sb.DefaultItem = m.LastItem;
				using(m.Submenu("Sub")) {
					m["si"] = o => AOutput.Write(o);
				}
			}, @"Q:\app\copy.ico", "SplitButton", o => AOutput.Write(o));
			t.Separator("");
			t[true, "DD2", @"Q:\app\delete.ico"] = m => {
				AOutput.Write("create menu");
				//m.MultiShow = false;
				m["one"] = o => AOutput.Write(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => AOutput.Write(o);
				}
			};
			//t.SplitButton("SB", o => {
			//	AOutput.Write(o);
			//}, m => {
			//	m["one"] = o => AOutput.Write(o);
			//	using(m.Submenu("Sub")) {
			//		m["si"] = o => AOutput.Write(o);
			//	}
			//}, @"Q:\app\copy.ico", "SplitButton");
			//Action<AMenu> menu1 = m => {
			//	m["one"] = o => AOutput.Write(o);
			//	using(m.Submenu("Sub")) {
			//		m["si"] = o => AOutput.Write(o);
			//	}
			//};
			//t.MenuButton("DD", menu1, @"Q:\app\find.ico", "MenuButton");
#elif false
			t.MenuButton("DD", @"Q:\app\find.ico");
			t.Menu = m => {
				m["one"] = o => AOutput.Write(o);
				using(m.Submenu("Sub")) {
					m["si"] = o => AOutput.Write(o);
				}
			};
#else
			t.MenuButton("DD", @"Q:\app\find.ico").Menu = m => {
				AOutput.Write("dd");
				//m.MultiShow = false;
				m["one"] = o => AOutput.Write(o);
				using(m.Submenu("Sub")) {
					m["two"] = o => AOutput.Write(o);
				}
			};
			t.SplitButton("SB", o => AOutput.Write(o), @"Q:\app\copy.ico").Menu = m => {
				AOutput.Write("dd");
				m["one"] = o => AOutput.Write(o);
				using(m.Submenu("Sub")) {
					m["two"] = o => AOutput.Write(o);
				}
			};
#endif
			//t.Separator("");
			////t["GC"] = o => GC.Collect();

			//var dd = new ToolStripSplitButton("SB2", null, (unu,sed)=>AOutput.Write("click"));
			//t.Add(dd, @"Q:\app\delete.ico");
			//dd.DropDownOpening += (unu, sed) => {
			//	var m = new AMenu();
			//	dd.DropDown = m.Control;
			//	m["one"] = o => AOutput.Write(o);
			//};
			//dd.ButtonClick += (unu, sed) => AOutput.Write("button click");
			//dd.DoubleClickEnabled = true;
			//dd.ButtonDoubleClick += (unu, sed) => AOutput.Write("button double click");

			//ATimer.After(3000, _ => {
			//	var c = t.Control.Items[0];
			//	c.Select();
			//});

			//void _B(TBBorder b){
			//	t.Border = b;
			//	//AOutput.Write(AWnd.More.BorderWidth((AWnd)t.Control));
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
		//AOutput.Write((uint)c.ToArgb());

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
	//	AOutput.Write(so, ca);
	//}

	//internal static bool ClientToScreen4(AWnd w, ref POINT p)
	//{
	//	int x0=p.x;
	//	if(!Api.MapWindowPoints(w, default, ref p, out int k)) return false;
	//	AOutput.Write(p.x-x0, AMath.LoShort(k));
	//	//p.Offset(r.left, r.top);
	//	return true;
	//}

	//void TestClientToScreenRtlAware()
	//{

	//	ATimer.After(500, _ => {
	//		var w = AWnd.Find(null, "#32770").OrThrow();
	//		//RECT r;
	//		//AOutput.Write(w.GetClientRect(out r), r);
	//		//AOutput.Write(w.GetClientRect2(out r), r);
	//		//AOutput.Write(w.GetClientRect(out r, true), r);
	//		//AOutput.Write(w.GetClientRect2(out r, true), r);

	//		//POINT p = (1, 1);
	//		//Api.ClientToScreen(w, ref p);
	//		//AOutput.Write(p);


	//		//p = (1, 1);
	//		//AOutput.Write(Api.ClientToScreen2(w, ref p));
	//		//AOutput.Write(p);

	//		//p = (1, 1);
	//		//AOutput.Write(Api.ClientToScreen3(w, ref p));
	//		//AOutput.Write(p);

	//		////p = (1, 1);
	//		////AOutput.Write(ClientToScreen4(w, ref p));
	//		////AOutput.Write(p);
	//		////return;

	//		var a = AWnd.GetWnd.AllWindows(true); AOutput.Write(a.Length);
	//		//int ir = Array.FindIndex(a, o => o.HasExStyle(WS2.LAYOUTRTL)); //AOutput.Write(ir);
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

	static void Bug1(string s = null, Func<Screen, bool> f = null) { }

	[MethodImpl(MethodImplOptions.NoInlining)]
	void TestAScreen()
	{
		var w = AWnd.Active;
		var s = new AScreen(w);
		var d = s.GetScreenHandle();
		AOutput.Write(d);

		//var w = AWnd.Find("*Notepad", also: o => o.Screen.Index == 1);
		//Bug1("", f: o=>o.)
		//Bug1(f: o=>o.)

		//ThreadPool.QueueUserWorkItem(_ => AOutput.Write(1));
		//500.ms();
		//APerf.First();
		//var a = AScreen.AllScreens;
		//APerf.NW();
		//AOutput.Write(a);
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

		//AOutput.Write(HScreen.AllScreens);
		////AOutput.Write(Screen.AllScreens);
		//AOutput.Write(HScreen.FromIndex(0), HScreen.FromIndex(1), HScreen.FromIndex(2), HScreen.FromIndex(3));
		//foreach(var v in Screen.AllScreens) AOutput.Write(v.GetIndex(), (HScreen)v);
		//foreach(var v in HScreen.AllScreens) AOutput.Write(v.Index, (Screen)v);
		//bool two = AKeys.IsScrollLock;

		//var screenFromMouse = new HScreen(() => HScreen.FromPoint(AMouse.XY));
		//var screenFromActiveWindow = new HScreen(() => HScreen.FromWindow(AWnd.Active));
		//AOutput.Write(screenFromMouse, screenFromActiveWindow);

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
		//AOutput.Write(v, h.IsAlive);
		//AOutput.Write(h == HScreen.Primary);
	}

	//[DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", SetLastError =true)]
	//internal static extern IntPtr LoadLibraryEx(string lpLibFileName, IntPtr hFile, uint dwFlags);
	//void TestFailsWpf()
	//{
	//	//if(default==Api.LoadLibrary(@"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.0\wpfgfx_cor3.dll"))
	//	//	AOutput.Write(ALastError.Message);
	//	//if(default==LoadLibraryEx(@"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.0\wpfgfx_cor3.dll", default,
	//	//	0x1100))
	//	//	AOutput.Write(ALastError.Message);

	//	var f = new Window { Left = 500, Top = 1500 };
	//	f.Show();

	//	//AOutput.Write(Api.LoadLibrary(@"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.0\wpfgfx_cor3.dll"));

	//	ADialog.ShowEx(secondsTimeout: 2);
	//}

	//void TestAWndFromObject()
	//{
	//	//var f = new Form { StartPosition = FormStartPosition.Manual, Left = 500, Top = 1500 };
	//	var f = new Window { Left = 500, Top = 1500 };
	//	f.Show();

	//	var v = new AScreen(f);
	//	AOutput.Write(v.ToDevice());

	//	//ADialog.ShowEx(secondsTimeout: 2);
	//	ADialog.ShowEx(owner: f);
	//}

	class Se : Au.Util.JSettings
	{
		public static Se Load() => _Load<Se>(@"q:\test\JSettings.json");

		public string user { get => _user; set => Set(ref _user, value); }
		string _user;

	}

	void TestJSettings()
	{
		var se = Se.Load();
		AOutput.Write(se.user);
		//ATimer.After(1000, _ => se.user = AKeys.IsScrollLock ? "scroll" : "no");
		se.user = AKeys.IsScrollLock ? "scroll" : "no";
		//ADialog.Show("JSettings");
		Api.MessageBox(default, "JSettings", "test", 0x00040000);
	}

	void TestBitmapLockBitsDispose()
	{
		//Bitmap.FromFile(@"Q:\Test\qm small icon.png").Dispose();
		//AOutput.Write("start");
		//3.s();
		//ADebug.MemorySetAnchor_();
		//for(int i = 0; i < 1000; i++) {
		//	var b = Bitmap.FromFile(@"Q:\Test\qm small icon.png") as Bitmap;
		//	//var d=b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//	var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format64bppArgb);
		//	b.Dispose();
		//	b.UnlockBits(d);
		//}
		//ADebug.MemoryAOutput.Write_();
		//while(AKeys.IsScrollLock) 100.ms();


		var w = AWnd.Find("Au - Microsoft Visual Studio", "HwndWrapper[DefaultDomain;*").OrThrow();
		string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAA0AAAAQCAYAAADNo/U5AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAC2SURBVDhPYzhz9cN/DHzlw/+Hp1b8v7osHisGazoNVISOr6/N+39rZ+f/k6efY2C4Jmwm4sJDRdPNjSVgGobvH56BVQMIwzX1rz36nzd40n8Grx4w3bfh3P8Hxxfg1wRSuHzvZXCQgmgQ/+Hj8/g1gWxAjgvpmGlgcYKa0G0CiWPDDCDCo2krWBMMgzTM3XkZnJxAhqJjBpgGEI1sGjbFMAzWBNOATQE2DHYeKRpAGBwQpOEP/wF46o8knB4kYgAAAABJRU5ErkJggg==";
		//string image = @"Q:\Test\find.bmp";

		//for(int i = 0; i < 10; i++) {
		//	var im = AWinImage.Find(w, image, WIFlags.WindowDC).OrThrow();
		//	AOutput.Write(im);
		//}

		var im = AWinImage.Find(w, image, WIFlags.WindowDC).OrThrow();

		//var b = AWinImage.LoadImage(image);

		//var h = Api.LoadImage(default, image, Api.IMAGE_BITMAP, 0, 0, Api.LR_LOADFROMFILE);
		//var b = Bitmap.FromHbitmap(h);
		//var d = b.LockBits(new Rectangle(default, b.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//AOutput.Write(d.Stride);
		//b.UnlockBits(d);

		//var im = AWinImage.Find(w, b, WIFlags.WindowDC).OrThrow();
		AOutput.Write(im);

	}

	static HashSet<object> hs = new HashSet<object>();
	static HashSet<object> hs2 = new HashSet<object>();
	static System.Collections.Concurrent.ConcurrentDictionary<object, byte> cd = new System.Collections.Concurrent.ConcurrentDictionary<object, byte>();

	void TestConcurrentSpeed()
	{
		var a = new object[100];
		for(int i = 0; i < a.Length; i++) a[i] = new object();

		APerf.SpeedUpCpu();
		for(int i1 = 0; i1 < 7; i1++) {
			int n2 = a.Length;
			APerf.First();
			for(int i2 = 0; i2 < n2; i2++) { hs.Add(a[i2]); }
			for(int i2 = 0; i2 < n2; i2++) { hs.Remove(a[i2]); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { lock(hs2) hs2.Add(a[i2]); } //20% slower
			for(int i2 = 0; i2 < n2; i2++) { lock(hs2) hs2.Remove(a[i2]); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { cd.TryAdd(a[i2], default); } //50% slower
			for(int i2 = 0; i2 < n2; i2++) { cd.TryRemove(a[i2], out _); }
			APerf.Next();
			for(int i2 = 0; i2 < n2; i2++) { var gc = GCHandle.Alloc(a[i2]); gc.Free(); } //50% faster
			APerf.NW();
			Thread.Sleep(100);
		}
	}

	unsafe static class _Api
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool VirtualLock(void* lpAddress, LPARAM dwSize);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool VirtualUnlock(void* lpAddress, LPARAM dwSize);

	}

	//	unsafe void TestMemoryAllocSpeed()
	//	{
	//		byte[] a;
	//		byte* p1 = (byte*)Au.Util.AMemory.Alloc(1000); Au.Util.AMemory.Free(p1);
	//		byte* p2 = (byte*)Api.VirtualAlloc(default, 1000); Api.VirtualFree(p2);
	//		int n = 10_000_000;
	//		//n = 100_000_000;
	//		n = 0x10000;

	//		APerf.SpeedUpCpu();
	//		for(int i1 = 0; i1 < 10; i1++) {
	//			APerf.First();

	//			//	a = new byte[n];
	//			//	APerf.Next();
	//			//	fixed(byte* bp = a) {
	//			//		var ip = (long*)bp; for(int j = 0; j < n / 8; j++) ip[j] = j;
	//			//	}

	//			//APerf.Next();

	//			//	p1 = (byte*)Au.Util.AMemory.Alloc(n);
	//			//	APerf.Next();
	//			//	var ip = (long*)p1; for(int j = 0; j < n / 8; j++) ip[j] = j;
	//			//	APerf.Next();
	//			//	Au.Util.AMemory.Free(p1);

	//			//APerf.Next();

	//			//	//p2 = (byte*)Api.VirtualAlloc(default, n);
	//			//	p2 = (byte*)Api.VirtualAlloc(default, n, Api.MEM_COMMIT | Api.MEM_RESERVE, Api.PAGE_READWRITE);
	//			//	APerf.Next();
	//			//	var ip = (long*)p2; for(int j = 0; j < n / 8; j++) ip[j] = j;
	//			//	APerf.Next();
	//			//	Api.VirtualFree(p2);

	////			//test realloc speed
	////			p1 = (byte*)Au.Util.AMemory.Alloc(n);
	////			APerf.Next();
	////			var ip = (long*)p1; for(int j = 0; j < n / 8; j++) ip[j] = j;
	////			APerf.Next();
	////#if true
	////			Au.Util.AMemory.Free(p1);
	////			var p1c = (byte*)Au.Util.AMemory.Alloc(n + 20000);
	////#else
	////			var p1c = (byte*)Au.Util.AMemory.ReAlloc(p1, n + 20000);
	////#endif
	////			APerf.Next();
	////			Au.Util.AMemory.Free(p1c);

	//			APerf.NW();
	//			//AOutput.Write(p1c == p1);
	//			Thread.Sleep(100);
	//		}
	//	}

	void TestMenuDefaultIcon()
	{
		var m = new AMenu();
		var c = m.Control;

		//AOutput.Write(c.ShowItemToolTips);
		c.ShowItemToolTips = false;
		//c.BackColor = Color.Wheat;
		//c.Font = new Font("Courier New", 20);

		m.DefaultIcon = AIcon.GetFileIconImage(@"q:\app\macro.ico", 16);
		m.DefaultSubmenuIcon = AIcon.GetFileIconImage(@"q:\app\menu.ico", 16);
		m.ExtractIconPathFromCode = true;
		m["aa"] = null;
		//m.LastMenuItem.ToolTipText = "TT";
		using(m.Submenu("sub")) {
			m["bb"] = null;
			m["dd", @"q:\app\copy.ico"] = null;
			m["notepad"] = o=>AExec.Run("notepad.exe");
		}
		m.Separator();
		m["cc", ""] = null;
		m.Show();

	}

	class Ho : IDisposable
	{

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if(!disposedValue) {
				if(disposing) {
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~Ho()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
	bool TestArrayExt(string[] a)
	{
		return a.IsNE_();
	}
	//int TestArrayExt(string[] a)
	//{
	//	return a.Len_();
	//}

	unsafe void _Main()
	{
		//Application.SetCompatibleTextRenderingDefault(false);
		//AOutput.Write("before");
		//ADebug.AOutput.WriteLoadedAssemblies(true, true, true);


		//string s = "";
		//int n = s.Lenn();
		//if(s.IsNE())
	}

	void One(bool two) { }
	bool Two() => false;

	[STAThread] static void Main(string[] args) { new Script(args); }
	Script(string[] args)
	{
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();
		Au.Util.AssertListener_.Setup();

		//APerf.First();
		try {
			_Main();
		}
		catch(Exception ex) { AOutput.Write(ex); }
	}
}
