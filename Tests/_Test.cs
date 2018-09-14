#define TOOLS

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

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args)
	{
		//TestProcessStartSpeed(); return;

		//_EnableVisualStylesEtc();
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
		//			1.s();
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
		//10.s();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _EnableVisualStylesEtc()
	{
		//info: each of these load System.Windows.Forms.dll and System.Drawing.dll.
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
	}


	class AssemblyOptions
	{
		public int k;
	}
	static ConditionalWeakTable<Assembly, AssemblyOptions> s_ao = new ConditionalWeakTable<Assembly, AssemblyOptions>();

	static void TestAssemblySettings()
	{
		var a = Assembly.GetEntryAssembly();
		var ao = new AssemblyOptions() { k = 5 };
		s_ao.Add(a, ao);
		if(s_ao.TryGetValue(a, out var o)) Print(o.k);
	}

	static void TestFinalizersAndGC()
	{
		//Print(Marshal.SizeOf<HandleRef>());

		Task.Run(() => { for(; ; ) { 100.ms(); GC.Collect(); } });

		var w = Wnd.Find("Quick*").OrThrow();

		var a = Acc.Find(w, "CHECKBOX", "Annot*").OrThrow();
		//var atb = Acc.Find(w, "TOOLBAR", prop: "id=2053").OrThrow();
		//var a = atb.Find("CHECKBOX", "Annot*").OrThrow();

		a.DoAction();

		Print("after");
		0.3.s();
		Print("end");
	}

	class FormDoEvents :Form
	{
		public bool stop;

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Print("closing");
			stop = true;
			base.OnFormClosing(e);
		}

		protected override void WndProc(ref Message m)
		{
			//Print(m);
			base.WndProc(ref m);
		}
	}

	static void TestSleep2()
	{
		//Print(Time.Milliseconds, Time.MillisecondsWithoutComputerSleepTime);

		//for(int i = 0; i < 5; i++) {
		//	Perf.First();
		//	for(var t = Time.Microseconds; Time.Microseconds < t + 300;) { }
		//	Perf.Next();
		//	Time.SleepDoEvents(1);
		//	Perf.Next();
		//	Perf.Write();
		//}

		var f = new FormDoEvents();
		var b = new Button();
		b.Click += (unu, sed) =>
		{
			f.stop = true;
			//Perf.First();
			//Time.SleepDoEvents(5000);
			//Perf.NW();
		};
		f.Controls.Add(b);
		Timer_.After(500, () =>
		{
			Perf.First();
			//Time.SleepDoEvents(5000, true);
			//Time.SleepDoEvents(5000, ref f.stop);

			//for(var g=Time.Milliseconds; Time.Milliseconds < g+5000; ) {
			//	//Application.DoEvents();
			//	Time.DoEvents();
			//}

			//var m = new Au.Util.MessageLoop();
			//Timer_.After(1000, ty => m.Stop());
			//m.Loop();

			Perf.Next();
			Perf.NW();
		});

		f.ShowDialog();
		Print("END");
	}

	//static void TestSpeedThreadAndTask()
	//{
	//	//Perf.SpinCPU(100);
	//	100.ms();
	//	for(int i1 = 0; i1 < 5; i1++) {
	//		int n2 = 1;
	//		Perf.First();
	//		//for(int i2 = 0; i2 < n2; i2++) { var t = new Thread(()=> { /*Print(1);*/ }); t.Start(); }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { Task.Run(()=> { /*Print(2);*/ }); }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { }
	//		Perf.NW();
	//		200.ms();
	//	}

	//}

	static void TestAcc()
	{
		//Acc.Misc.WorkaroundToolbarButtonName = true;
		//Acc a = Acc.FromPoint(644, 1138); //BUTTON
		////Acc a = Acc.FromPoint(225, 1138); //SPLITBUTTON
		////Acc a = Acc.FromPoint(453, 1138); //SEPARATOR
		//Print(a);
		//0.1.s();
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
		//	//for(int i2 = 0; i2 < n2; i2++) { var k = a.RoleInt; }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { var k = a.Name; }
		//	//Perf.Next();
		//	//for(int i2 = 0; i2 < n2; i2++) { var k = a.RectInClientOf(cont); }
		//	Perf.NW();
		//}
		//return;

		//_StartMemoryMonitorThread();

		//Acc.Misc.WorkaroundToolbarButtonName = Keyb.IsScrollLock;
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
		//Wnd w = Wnd.Find("QM# -*");
		//Wnd w = Wnd.Find("WinDbg*");
		//Wnd w = Wnd.GetWnd.Root;
		//w.Activate(); .2.s();
		//w= w.ChildById(2216);
		//Wnd w = Wnd.Find(className: "QM_Editor").ChildById(2052);
		//Acc a = Acc.FromWindow(w);
		//Print(a);
		//a = a.Child(2);
		//Print(a);

		for(int i = 0; i < 1; i++) {
			Perf.First();
#if false
			x.TestAcc(w);

			//foreach(Wnd ww in Wnd.GetWnd.AllWindows(true)) x.TestAcc(ww);

			Perf.NW();
			x.PrintMemory();

			//var a =Acc.deb; for(int j = 0; j < a.Length; j++) Print(j, a[j]);
#else
			//var a = Acc.Find(w, "LINK", "Bug Reports");
			//var a = Acc.Find(w, "web:LINK", "Bug Reports");
			//var a = Acc.Find(w, "web:LINK", "Bug Reports", AFFlags.Reverse);
			//var a = Acc.Find(w, "web:DOCUMENT");
			//var a = Acc.Find(w, "web:PANE");
			//var a = Acc.Find(w, "TITLEBAR", "Solution Explorer", AFFlags.NonClientToo);
			//var a = Acc.Find(w, "LISTITEM", "Portable", 0);
			//var a = Acc.Find(w, "class=Internet Explorer_Server:LINK", "Bug Reports", 0);
			//var a = Acc.Find(w, "web:LINK", also: o => ++o.Counter == 4);
			//var a = Acc.Find(w, "web:LINK", also: o => ++o.Counter == 4); //find 4-th LINK

			//var a = Acc.Find(w, "BUTTON", "History");
			//var a = Acc.Find(w, "web:DOCUMENT/div/BUTTON", "Bookmarks");
			//var a = Acc.Find(w, "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/BUTTON", "History");

			//var a = Acc.Find(w, "LINK", "Programming", AFFlags.HiddenToo); //1126 (depends on hidden tabs)
			//var a = Acc.Find(w, "LINK", "Programming"); //225
			//var a = Acc.Find(w, "web:LINK", "Programming"); //120
			//var a = Acc.Find(w, "web:DOCUMENT/div/div/div/div/LIST/LISTITEM/LIST/LISTITEM/LINK", "Programming"); //74
			//var a = Acc.Find(w, "web:DOCUMENT/div/div[4]/div[5]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //47
			//var a = Acc.Find(w, "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/div/div/div/LIST/LISTITEM/LIST/LISTITEM/LINK", "Programming"); //94
			//var a = Acc.Find(w, "APPLICATION[4]/GROUPING[-4]/PROPERTYPAGE[-4]/browser/DOCUMENT/div/div[4]/div[5]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //58
			//var a = Acc.Find(w, "web://[4]/[5]//[3]/[2]///LINK", "Programming"); //47

			//var a = Acc.Find(w, "BUTTON", "Resources    Alt+F6"); //6.4 s
			//var a = Acc.Find(w, "class=ToolbarWindow32:BUTTON", "Resources    Alt+F6"); //120
			//var a = Acc.Find(w, "CLIENT/WINDOW/TOOLBAR/BUTTON", "Resources    Alt+F6"); //139

			//var f = new Acc.Finder("web:LINK", "Bug Reports");

			//f.FindIn(w); var a = f.Result;

			//var a = Acc.Find(w, "TEXT", "Address and*", AFFlags.Reverse);

			//var a = Acc.Find(w, "web:LINK", "Programming", AFFlags.NoThrow);
			var a = Acc.Find(w, "web:DOCUMENT/div/div[4!]/div[5!]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //47

			//Acc aa=null;
			////int n = 0;
			//AFFlags fl = 0;
			////fl |=AFFlags.HiddenToo;
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
			//	 // var r = o.Role;
			//	 // Perf.Next();
			//	 // var st = o.State;
			//	 // Perf.Next();
			//	 // Perf.Write();
			//	 //Print(o.ToString(o.Level));

			//	 //if(++o.Counter == 10) throw new AuException("TEST");

			//	 return false;
			// });
			//if(aa != null) { Print(aa); aa.Dispose(); }

			Perf.NW();

			Print(a);
			a?.Dispose();
#endif
		}

		//using(var a = Acc.FromWindow(w)) {
		//	Print(a);
		//	Print(a.Parent);
		//}
	}

	//static void TestAccNavigate()
	//{
	//	Wnd w;

	//	//w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
	//	//var a = Acc.Find(w, "web:LINK", "General", navig: "pa3 ne fi3");
	//	//Print(a);
	//	//a?.Dispose();

	//	//return;

	//	//w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
	//	//var a = Acc.Find(w, "web:LINK", "General");
	//	//Print(a);
	//	//var b = a.Navigate("parent3 next first3");
	//	////var b = a.Navigate("pa3 ne fi3");
	//	////var b = a.Navigate("p3 n f3");
	//	////var b = a.Navigate("#9,3 #5 #7,3");
	//	////var b = a.Navigate("parent,3 next first,3");

	//	////w = Wnd.Find("* Studio ");
	//	////Print(w);
	//	////var a = Acc.Find(w, "BUTTON", "Search Control");
	//	////Print(a);
	//	//////var b = a.Navigate("pr fi2 ne2");
	//	//////var b = a.Navigate("pr fi ch3");
	//	////var b = a.Navigate("pa fi2 ch3");

	//	////w = Wnd.Find("* - Mozilla Firefox", "MozillaWindowClass");
	//	////var a = Acc.FromWindow(w, AccOBJID.CLIENT);
	//	////Print(a);
	//	//////var b = a.Navigate((AccNAVDIR)0x1009);
	//	////var b = a.Navigate("#0x1009");

	//	//Print(b);
	//	//a.Dispose();
	//	//b?.Dispose();

	//	//return;

	//	//foreach(var v in Wnd.GetWnd.AllWindows()) {
	//	//	foreach(var a in )
	//	//}

	//	Wnd wSkip = Wnd.Find("Quick*").Child(2202);
	//	w = Wnd.GetWnd.Root;
	//	//w = Wnd.Find("app -*");
	//	//w = Wnd.Find("* - Notepad");
	//	//Perf.First();
	//	w = Wnd.Find("* Mozilla Firefox");
	//	//w = Wnd.Find("* Google Chrome");
	//	//w = Wnd.Find("* Internet Explorer");
	//	//w = Wnd.Find("Quick*").ChildById(2053);
	//	//w = (Wnd)(IntPtr)5965264;

	//	//w = Wnd.Find("QM# - Q*");

	//	//var a = Acc.Find(w, "TOOLBAR", "Help");
	//	//Print(a);
	//	//var p = a.Parent();
	//	//Print(p);
	//	//Print(p.Navigate(AccNAVDIR.FIRSTCHILD));

	//	////var a = Acc.Find(w, "CLIENT", "Panels");
	//	////Print(a);
	//	////Print(a.Navigate(AccNAVDIR.CHILD, 14));

	//	//return;

	//	//Perf.Incremental = true;
	//	using(var aRoot = Acc.FromWindow(w)) {
	//		aRoot.EnumChildren(true, a =>
	//		{
	//			if(a.WndContainer == wSkip) { a.SkipChildren(); return; }
	//			if(a.IsInvisible) { a.SkipChildren(); return; }
	//			//switch(a.Role) { case "TITLEBAR": case "SCROLLBAR": case "MENUBAR": case "GRIP": a.SkipChildren(); return; }
	//			var s = a.ToString(a.Level);
	//			//Perf.First();
	//			using(var aa = a.Navigate(AccNAVDIR.PARENT)) {
	//				//Perf.Next();
	//				if(aa == null) Print(s, a.WndContainer, a.WndTopLevel);
	//				//if(aa == null) Print(s);
	//				//else Print("<><c 0x8000>" + s + "</c>\r\n<c 0xff0000>" + aa.ToString(a.Level+1) + "</c>");
	//			}
	//		});
	//		//_TestAccNavigate(aRoot, 0, out _, out _, out _);
	//	}
	//	//Perf.Write();
	//	//Perf.NW();
	//	Print("END");
	//}

	//static void _TestAccNavigate(Acc parent, int level, out int nChildren, out Acc firstChild, out Acc lastChild)
	//{
	//	nChildren = 0; firstChild = lastChild = null;
	//	var w = parent.WndContainer;
	//	if(!w.IsVisible || w.ControlId == 2202) return;

	//	var k = parent.Children(false); if(k.Length == 0) return;
	//	bool testFirstLast = false;
	//	for(int i = 0; i < k.Length; i++) {
	//		Acc a = k[i];
	//		if(a.IsInvisible) continue;
	//		if(!testFirstLast) {
	//			if(i > 0 || i < k.Length - 1) {
	//				Print(a.ToString(level));
	//				Native.ClearError();
	//				bool checkInvisible = false;
	//				if(i > 0) {
	//					using(var g = a.Navigate(AccNAVDIR.PREVIOUS)) {
	//						if(g == null && !(checkInvisible && k[i - 1].IsInvisible)) Print("<><c 0xff>previous  </c>" + Native.GetErrorMessage());
	//					}
	//				}
	//				if(i < k.Length - 1) {
	//					using(var g = a.Navigate(AccNAVDIR.NEXT)) {
	//						if(g == null && !(checkInvisible && k[i + 1].IsInvisible)) Print("<><c 0xff>next  </c>" + Native.GetErrorMessage());
	//					}
	//				}
	//			}
	//		}

	//		_TestAccNavigate(a, level + 1, out var nc, out var fc, out var lc);

	//		if(testFirstLast && nc != 0) {
	//			Print(a.ToString(level));
	//			Native.ClearError();
	//			bool checkInvisible = false;
	//			using(var g = a.Navigate(AccNAVDIR.FIRSTCHILD)) {
	//				if(g == null && !(checkInvisible && fc.IsInvisible)) Print("<><c 0xff>first  </c>" + Native.GetErrorMessage());
	//			}
	//			using(var g = a.Navigate(AccNAVDIR.LASTCHILD)) {
	//				if(g == null && !(checkInvisible && lc.IsInvisible)) Print("<><c 0xff>last  </c>" + Native.GetErrorMessage());
	//			}
	//			fc.Dispose(); if(lc != fc) lc.Dispose();
	//		}
	//	}
	//	if(testFirstLast) {
	//		nChildren = k.Length; firstChild = k[0]; lastChild = k[k.Length - 1];
	//		for(int i = 1; i < k.Length - 1; i++) k[i].Dispose();
	//	} else {
	//		for(int i = 0; i < k.Length; i++) k[i].Dispose();
	//	}
	//}

	static void TestAccExamples()
	{
		//var f = new Acc.Finder("BUTTON", "Apply"); //object properties
		//Wnd w = Wnd.Find(className: "#32770", also: t => f.FindIn(t));
		//Print(w);
		//Print(f.Result);

		//var w = Wnd.Find("Find");
		////var a = Acc.Find(w, "BUTTON", also: o => o.GetRect(out var r, o.WndWindow) && r.Contains(266, 33));
		////var a = Acc.Find(w, "BUTTON", also: o => o.GetRect(out var r, o.WndWindow) && r.left==234);
		////var a = Acc.Find(w, "BUTTON", also: o => ++o.Counter == 2, navig: "pa pr2");
		//var a = Acc.Find(w, "BUTTON", also: o => o.Level == 2);

		var w = Wnd.Find("*Mozilla Firefox");
		//var a = Acc.Find(w, "LINK", also: o => o.Value == "http://www.quickmacros.com/forum/viewforum.php?f=3&sid=720fc3129e6c70e07042b446be23a646");
		//var a = Acc.Find(w, "LINK", also: o => o.Value.Like_("http://www.example.com/x.php?*"));
		//var a = Acc.Find(w, "LINK", also: o => o.Value?.Like_("http://www.example.com/x.php?*") ?? false);
		//var a = Acc.Find(w, "web:LINK", "General");
		//var a = Acc.Find(w, "web:LINK", "**m Untitled||General");

		var f = new Acc.Finder("web:LINK", "General");
		//f.MaxLevel = 10;
		if(!f.Find(w)) return;
		var a = f.Result;

		//var w = Wnd.Find("*Sandcastle*");
		////var a = Acc.Find(w, "web:LINK", "**m Untitled||General");
		//Print(Acc.FromWindow(w).Children(true).Where(o=>!o.IsInvisible));

		//var w = Wnd.Find("Find");
		////var a = Acc.Find(w, "class=button:BUTTON");
		////var a = Acc.Find(w, "class=button:");
		//var a = Acc.Find(w, "id=1132:BUTTON");

		Print(a);
		a?.Dispose();

		//Print(AuDialog.ShowEx(buttons: "One|Two|50 Three|51Four", flags: DFlags.CommandLinks));
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
		using(var a = Acc.Find(w, "web:LINK", "name=P*\0 a:href=**r forum").OrThrow()) {
			Print(a);
		}
#elif false
		var a = Acc.Find(w, "web:LINK", "Board index").OrThrow();
		//var a = Acc.Find(w, "web:LISTITEM", "FAQ").OrThrow();
		//var a = Acc.Find(w, "web:BUTTON", "Search").OrThrow();
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

			var p = Perf.StartNew();
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

	static void Test1()
	{
		var w = Wnd.Find("Quick*");

		//var cf = new Wnd.ChildFinder("name");
		//var a = Acc.Find(w, "role", controls:cf);

		var af = new Acc.Finder("role");
		w.Child("name", also: o => af.Find(o));
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
		//using(var a = Acc.Find(w, "BUTTON", "Paste*").OrThrow()) {
		//	Print(a);
		//}
		//using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
		//	a.EnumChildren(true, o =>
		//	{
		//		//Print(o.Level, o.Role);
		//		Print(o);
		//		//100.ms();
		//	});
		//}
		Print("END");
	}

	static void TestAccVariousApps()
	{
		//var w = Wnd.Find("VLC leistuvė", "QWidget").OrThrow();
		//Print(w);
		//using(var a = Acc.Find(w, "CHECKBOX", "\0 description=Maišymo veiksena").OrThrow()) Print(a);

		//var w = Wnd.Find("Welcome Guide — Atom", "Chrome_WidgetWin_1").OrThrow();
		//Print(w);
		//using(var a = Acc.Find(w, "web:BUTTON", "*Learn Keyboard Shortcuts").OrThrow()) Print(a);

		//Acc.Misc.MaxChildren *= 2;
		var w = Wnd.Find("Quick*");
		//using(var a = Acc.FromWindow(w)) {
		//	a.EnumChildren(true, o =>
		//	 {
		//		 Print(o);
		//	 });
		//}
	}

	static void TestAccPreferLink()
	{
		//using(var a = Acc.FromXY(1150, 1469, preferLINK: true)) Print(a);
		for(int i = 0; i < 30; i++) {
			1.s();
			using(var a = Acc.FromMouse(AXYFlags.PreferLink)) {
				Print(a);
			}
		}
	}

	//static void TestAccFromXY()
	//{
	//	Wnd w;
	//	w = Wnd.Find("Quick*", "QM_Editor");
	//	//w = Wnd.Find("* Internet Explorer");
	//	//w = Wnd.Find("* Mozilla Firefox");
	//	w = Wnd.Find("* Google Chrome");
	//	//w = Wnd.Find("* OpenOffice*");

	//	//using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
	//	//	Print(a);
	//	//	using(var b = a.ChildFromXY(65, 65, screenCoord: false)) {
	//	//		Print(b);
	//	//	}
	//	//}
	//	int x, y;
	//	//x = 65; y = 65;
	//	//x = 277; y = 536;
	//	//x = 65; y = -10;
	//	//x = -1; y = 0;
	//	//x = 94; y = 365;
	//	//x = 60; y = 32;
	//	//x = 428; y = 337;
	//	x = 77; y = 178;
	//	//x = 516; y = 39;
	//	using(var a = Acc.FromXY(w, x, y)) {
	//		//using(var a = Acc.FromXY(1160, 1467)) {
	//		//using(var a=Acc.FromXY(w, 65, 65)) {
	//		//using(var a=Acc.FromXY(w, 277, 536)) {
	//		//using(var a=Acc.FromXY(w, 65, -10)) {
	//		//using(var a = Acc.FromXY(w, -1, 0)) {
	//		Print(a);
	//		//Print(a.Navigate("pa"));
	//	}
	//}

	static void TestAccFromFocus()
	{
		Wnd w;
		w = Wnd.Find("Quick*", "QM_Editor");
		//w = Wnd.Find("* Internet Explorer");
		//w = Wnd.Find("*Mozilla Firefox");
		//w = Wnd.Find("* Google Chrome");
		//w = Wnd.Find("* Opera");
		//w = Wnd.Find("* OpenOffice*");
		//w = Wnd.Find("Options");
		//w = Wnd.Find("Java *");
		//w = Wnd.Find("Settings");
		//w = Wnd.Find("QM# - Q*", "WindowsForms*");
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
		using(new AccHook(AccEVENT.OBJECT_FOCUS, 0, x =>
		{
			Print(x.wnd);
			var a = x.GetAcc();
			if(a == null) Print(Native.GetErrorMessage());
			else Print(a);
		})) MessageBox.Show("hook");
	}

	//static void TestAccFromComObject()
	//{
	//	Wnd w = Wnd.Find("* Internet Explorer").Child(null, "Internet Explorer_Server");
	//	Print(w);
	//	var res = w.Send(WM_HTML_GETOBJECT);
	//	var guid = typeof(MSHTML.IHTMLDocument2).GUID;
	//	if(0 != ObjectFromLresult(res, ref guid, 0, out IntPtr ip)) return;

	//	var doc = Marshal.GetTypedObjectForIUnknown(ip, typeof(MSHTML.IHTMLDocument2)) as MSHTML.IHTMLDocument2;
	//	var body = doc.body;
	//	//Print(body.outerHTML);
	//	Print(Acc.FromComObject(body));

	//	Marshal.ReleaseComObject(body);
	//	Marshal.ReleaseComObject(doc);
	//	Print(Marshal.Release(ip));
	//}
	//static uint WM_HTML_GETOBJECT = Api.RegisterWindowMessage("WM_HTML_GETOBJECT");

	[DllImport("oleacc.dll", PreserveSig = true)]
	internal static extern int ObjectFromLresult(LPARAM lResult, in Guid riid, LPARAM wParam, out IntPtr ppvObject);

	static void TestAccMiscMethods()
	{
		//var w = Wnd.FromMouse();
		//w.MouseClick();
		//return;

		//var w = Wnd.Find("Options");
		//w.Activate();
		Output.Clear();
		//using(var a = Acc.Find(w, null, "Mouse").OrThrow()) {
		//using(var a = Acc.Find(w, "CHECKBOX", "Mouse").OrThrow()) {
		//using(var a = Acc.Find(w, nameof(AccROLE.CHECKBOX), "Mouse").OrThrow()) {
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
				//a.DoAction();
				//Print(a.Rect);
				//a.MouseMove();
				//a.MouseClick();
				//a.MouseClick(1, 1, MButton.Right);

				Perf.Next();
				var w1 = a.WndContainer;
				Perf.Next();
				//using(var ap = a.Navigate("pa")) {
				//	Perf.Next();
				//	var w2 = ap.WndContainer;
				//	Perf.NW();

				//	//Print(a);
				//	//Print(ap);
				//	Print(!w1.Is0, !w2.Is0);
				//}
				if(i == 0) Perf.Cpu(100);
			}
		}
		//100.ms();
		//Print(Wnd.Active);
	}

	static void TestAccWebPageProp()
	{
		//var w = Wnd.Find("* Mozilla Firefox");
		var w = Wnd.Find("* Google Chrome");
		//var w = Wnd.Find("* Internet Explorer");
		//var w = Wnd.Find("FileZilla");
		//var w = Wnd.Find("Au - Microsoft Visual Studio ", "HwndWrapper[DefaultDomain;*");
		//using(var a = Acc.Find(w, "web:")) {
		//using(var a = Acc.Find(w, "web:LINK", "\0 a:href=*forum*")) {
		using(var a = Acc.Find(w, "web:LINK", "Programming", null, AFFlags.HiddenToo).OrThrow()) {
			//using(var a = Acc.Find(w, "TREEITEM", "Other Bookmarks", AFFlags.HiddenToo).OrThrow()) { //no scroll
			//using(var a = Acc.Find(w, "TREEITEM", "Temp", AFFlags.HiddenToo).OrThrow()) { //no scroll
			//using(var a = Acc.Find(w, "TREEITEM", "Structs.cs", AFFlags.HiddenToo).OrThrow()) { //no scroll
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
		//using(var a=Acc.Find(w, "TREE").OrThrow()) {
		//using(var a=Acc.Find(w, "LIST").OrThrow()) {
		//using(var a=Acc.Find(w, "LIST", also:o=>++o.Counter==2).OrThrow()) {
		//using(var a=Acc.Find(w, "web:LIST", "\0 a:name=cars").OrThrow()) {
		//using(var a=Acc.Find(w, "web:COMBOBOX", "\0 a:name=cars").OrThrow()) {
		using(var a = Acc.Find(w, "web:ALERT", "\0 a:name=cars").OrThrow()) { //IE
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
			using(var a = Acc.FromMouse(AXYFlags.PreferLink)) {
				Print(a);
			}
		}

		//Print(Acc.Focused());
	}

	static void TestAccInOtherFunctions()
	{
		//Acc.Find(Wnd.Find("Quick*"), "BUTTON", "Options*").MouseClick();

		//var w = Wnd.Find("**c *Firefox").OrThrow();
		//Print(w);
		//var a = Acc.Find(w, "div", "Google").OrThrow();
		////a.MouseMove();
		//Perf.First();
		//for(int i = 0; i < 5; i++) {
		//	var si = WinImage.Find(a, @"Q:\app\Au\Tests\Images\google.bmp", WIFlags.WindowDC).OrThrow();
		//	Perf.Next();
		//}
		//Perf.Write();
		////si.MouseMove();

		//var w = Wnd.Find("Options*").ChildById(1571);
		//Print(w.NameAcc);

		//var w = Wnd.Find("Options*").Child("***accName Run as", "combo*").OrThrow();
		//Print(w);

		//var w = Wnd.Find("Options*");
		//var af = new Acc.Finder("COMBOBOX", "Run as");
		//Print(w.HasAcc(af));

		//var w = Wnd.Find("Options*").ChildById(1099);
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
		//var a = Acc.Find(w, "CHECKBOX", skip:2).OrThrow();
		//var a = Acc.WaitFor(0, w, "CHECKBOX", "Mouse").OrThrow();
		var f = new Acc.Finder("CHECKBOX", "Mouse");
		Print(f.Wait(-2, w));
		var a = f.Result;
		Print(a);
	}

#if false
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
			var a = Acc.Find(w, "LINK", "Bug Reports").OrThrow();
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
		//	var cond = u.CreatePropertyCondition(UIA.PropertyId.Name, "Bug Reports");
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
		//Print(e != null, e?.Name);

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
			//e = Acc.Find(w, "LINK", "Bug Reports"); //FF 300, Chr 230
			//e = Acc.Find(w, "web:LINK", "Bug Reports"); //FF 190, Chr 230
			//e = Acc.Find(w, "web:LINK", "Untitled-"); //FF 230, Chr 300

			//e = AElement.Find(w, "Bug Reports");
			//e = AElement.Find(w, "Bug Reports", UIA.TypeId.Hyperlink); //FF 260, Chr 60
			//e = AElement.Find(w, "Untitled-", UIA.TypeId.Hyperlink); //FF 390, Chr 130
			e = AElement.WebFind(w, "Bug Reports", UIA.TypeId.Hyperlink); //FF 100, Chr 60
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
			Print(e != null, e?.Name);
			//if(e != null) break;
		}

		//Wnd.Find(null, "QM_Editor").Activate();

		new ACondition().Add(UIA.PropertyId.AutomationId, "hhhh").AddOr(UIA.PropertyId.ClassName, "cn1", "cn2");
	}

	static void TestAElementFromPoint()
	{
		for(; !Keyb.IsCtrl; 1.s()) {
			UIA.IElement e = null;
			try { e = AElement.FromMouse(true); } catch(Exception ex) { Print(ex); }

			//var p = Mouse.XY;
			//var w = Wnd.FromXY(p, WXYFlags.NeedWindow);
			//var e = AElement.FromPoint(w, p);

			Print(e.LibToString_());
		}
	}

	static void TestAccToUIElem()
	{
		var w = Wnd.Find("Properties*").OrThrow();
		var a = Acc.Find(w, "BUTTON", "OK").OrThrow();
		var e = AElement.FromAcc(a, false);
		if(e == null) { Print("failed"); return; }
		Print(e.Name);
		Print(e.AutomationId);
	}
#endif

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
		Perf.Cpu(200);
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

	//	static void TestPcre()
	//	{
	//		string s, p;
	//		s = "one two three";
	//		p = @"(?i) (t|h).";

	//		for(int i = 0; i < 5; i++) {
	//			100.ms();
	//#if true
	//			Cpp.Cpp_TestPCRE(s, p);

	//#else
	//			Perf.First();
	//			var rx = new Regex(p, RegexOptions.CultureInvariant);
	//			Perf.Next();
	//			for(int j = 0; j < 1000; j++) {
	//				rx.IsMatch(s);
	//				//Regex.IsMatch(s, p, RegexOptions.CultureInvariant);
	//			}
	//			Perf.NW();
	//#endif
	//		}
	//	}

	//slower than DllImport
	//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	//[System.Security.SuppressUnmanagedCodeSecurity]
	//delegate bool Cpp_StringEqualsT(char* s, LPARAM lenS, char* w, LPARAM lenW, bool ignoreCase = false);
	//static Cpp_StringEqualsT Cpp_StringEqualsF = Cpp.Cpp_StringEquals;

	static unsafe void TestCppWildex()
	{
		string s, w;
		//s = "one two three";
		s = "one two three ąčę";
		//s = "one * three";

		//w = "**c one two three";
		//w = s;
		//w = "one";
		//w = "one*";
		//w = "ONE TWO THREE";
		//w = "**c ONE TWO THREE";
		//w = "one * three";
		//w = "**t one * three";
		w = @"**r ^one \w+ threE$";
		//w = @"**r ^one \w+ threE$";
		//w = @"**rc ^one \w+ three$";
		//w = @"**rc ^one \w+ three ąčę$";
		//w = @"**r ^one \w+ three ąčĘ$";
		//w = @"**r ^one \w+ three \w+$";
		//w = @"**r ^one \w+ three ...$";
		//w = @"**r (*UCP)^one \w+ three \w+$";
		//w = @"**n one";
		//w = @"**n one*";
		//w = @"**m one*";
		//w = @"**m kuku||one*";
		w = @"**m ku||**r ku||one*";

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
		w = "**c modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Modjkajdkjka ajdkaj ja.ldskkdskofkoskfo sfosfjsigjf ijfisjfsoi soifoi  Auisuidu - hdjshhdsjhdj jdskjdks Auisuidu - hdjshhdsjhdj jdskjdkM ė";

		s = "short678";
		w = "**c short678";

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

			Perf.Cpu(200);
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
	//			throw new AuException(new string(es));
	//	}

	//	public bool Match(string s)
	//	{
	//		if(s == null) return false;
	//		return Cpp.Cpp_WildexMatch(this, s, s.Length);
	//	}

	//	public bool HasValue => _1 != default;
	//}

	//static void TestWildexStruct()
	//{
	//	var x = new WildexStruct("**r gg");
	//	Print(x.Value);
	//	//x.Value = "k";
	//	Print(x.Match("gg"));
	//}

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
		//		//Debug_.LibPrintMemory();
		//		1.ms();
		//	}
		//}
		//Perf.NW();
		//Print("END");

		Print(Wnd.Find(@"**r \QQuick\E \bMacros\b"));
	}


	//static void TestDllInProcThisThread()
	//{
	//	var f = new Form();
	//	f.Text = "Form55";
	//	var b = f.Add_<Button>(4, 4, 100, 30, "Five");
	//	f.Click += (unu, sed) => { TestDllInProc(); };
	//	f.ShowDialog();
	//}

	static void TestDllInProc()
	{
		//var w = Wnd.Find(null, "MozillaWindowClass").OrThrow();
		var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
		//var w = Wnd.Find("* Google Chrome", "Chrome*").Child(null, "Chrome_Render*").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, Api.s_IES).OrThrow();
		//var w = Wnd.Find("* Internet Explorer").Child(null, "TabWindowClass").OrThrow().Get.DirectParent;
		//var w = Wnd.Find("* Internet Explorer").Child(null, "Shell DocObject View").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").Child(null, "Windows.UI.Core.CoreWindow").OrThrow();
		//var w = Wnd.Find("* Microsoft Edge").Child(null, "ApplicationFrameInputSinkWindow").OrThrow();
		//var w = Wnd.Find("* Opera").OrThrow();
		//var w = Wnd.Find("Options").OrThrow();
		//var w = Wnd.Find("ipc_server").OrThrow();
		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		//var w = Wnd.Find(null, "QM_Editor").ChildById(2051).OrThrow();
		//var w = Wnd.Find("QM Dialog").OrThrow();
		//var w = Wnd.Find("QM# - Q*").OrThrow();
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
		//var w = Wnd.Find(null, "QM_Editor").ChildById(2053).OrThrow();
		//var w = Wnd.Find("FileZilla").OrThrow();
		//var w = Wnd.Find("FileZilla").ChildById(-31801).OrThrow(); //toolbar
		//var w = Wnd.Find("*cmd.exe").OrThrow(); //fails to inject
		//var w = Wnd.Find("Character Map").OrThrow();
		//var w = Wnd.Find("Form55").OrThrow();
		//var w = Wnd.Find("Registry Editor").OrThrow();
		Print(w);
		//Print(w.ProcessId);

		//var a1=Acc.Find(w, "LINK", "Bug Reports");
		//Print(a1);
		//return;

		try {
#if true
			AFFlags flags = 0;
			//flags |= AFFlags.NotInProc;

			string role = null, name = null, prop = null;

			role = "web:LINK";
			//role = "LINK";
			//role = "id=:LINK";
			//role = "BUTTON";

			//name = "Bug Reports";
			name = "???????*";
			//name = "Five";
			//name = "Board index";
			//name = "**r -(Untitled";

			//Acc ap = Acc.FromWindow(w);
			//if(0!=Cpp.Cpp_AccFromWindow(true, w, 0, out var iacc)) return; Acc ap = new Acc(iacc);
			//Print(ap);

			for(int i = 0; i < 1; i++) {
				100.ms();
				Perf.First();
				var a = Acc.Find(w, role, name, prop, flags);
				Perf.NW();
				if(a != null) {
					Print(a);
				} else {
					Print("not found");
					break;
				}
			}
#else
			//using(var aParent = Acc.FromWindow(w)) {

			//Perf.First(); Perf.NW();

			var k = new List<Acc>();
			Cpp.AccCallbackT callback = (ref Cpp.Cpp_Acc r) =>
			 {
				 //Perf.First(); int n=Marshal.Release(r.iacc); Perf.NW('R');
				 ////Print(n);
				 //return 0;
//#if true
				// Perf.First(); Marshal.AddRef(r.iacc); Perf.NW();
				 var a = new Acc(r.iacc, r.elem);
				 //var a = new Acc(r.iacc, r.elem, addRef: true);
				 //Print(a);
				 //Print(a.Role);
				 k.Add(a);
//#else
//		using(var a = new Acc(r.iacc, r.elem)) {
//			r.iacc = default;
//			//Print(a);
//		}
//#endif
				// if(a.Name == "RSS") return 1;
				 if(a.Name.Contains("i")) return 1;
				 return 0;
			 };

			//Wnd wAgent = default;
			Acc.IAccessible iaccParent = default;
			//int hr1 = Cpp.Cpp_AccFromWindow(true, w, 0, out iaccParent);
			//if(hr1 != 0) { PrintHex(hr1); return; }
			//Print(iaccParent); return;

			Cpp.Cpp_AccParams p = default;

			//p.role = "LINK";
			p.role = "web:LINK";
			//p.role = "web:BUTTON";
			//p.role = "LISTITEM";
			//p.role = "id=-31772:LISTITEM";
			//p.role = "class=SysListView32:LISTITEM";
			//p.role = "id=7:LINK";
			//p.role = "id=0xa:LINK";
			//p.role = "class=Butt*:BUTTON";
			//p.role = "id=bad:LINK";
			//p.role = "ONE/TWO[5]/THREE[7!]/FOUR[-0xa]//SEVEN";
			//p.role = "web:/ONE/TWO";
			//p.role = "WINDOW[4!]/CLIENT/CLIENT/CLIENT/DOCUMENT";
			//p.role = "TEXT";

			p.name = "Bug Reports";
			//p.name = "Show more";
			//p.name = "Board index";
			//p.name = "scintilla";
			//p.name = "?*";
			//p.name = "**r ^U.+d$";
			//p.name = "**g Untitled";
			//p.prop = "value=XXX\0  a:href=YYY\0\r\n description=DDD";
			//p.prop = "maxLevel=15";
			//p.prop = "notin=ONE,TWO,THREE";
			//p.prop = "notin=BUTTON,MENUBAR,STATICTEXT";
			//p.name = "Select";
			//p.prop = "value=test\r";
			//p.prop = "description=Used*";
			//p.flags |= AFFlags.HiddenToo;
			//p.prop = "action=press";
			//p.prop = "key=alt+s";
			//p.prop = "help=* notification";

			//p.skip = 2;

			for(int i = 0; i < 1; i++) {
				//100.ms();
				//Perf.First();
				//foreach(var v in k) v.Dispose();
				//Perf.NW();

				k.Clear();
				100.ms();
				Perf.First();
				//var hr = Cpp.Cpp_AccFind(true, w, default, "LINK", "Bug Reports", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, default, iaccParent, "LINK", "Bug Reports", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, null, 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, default, iaccParent, null, null, 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "GROUPING", "Extensions", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "BUTTON", "Infobar Container", 0, null, 0, out var r);
				var hr = Cpp.Cpp_AccFind(true, w, default, ref p, null, out var r, out var errStr);
				//var hr = Cpp.Cpp_AccFind(true, w, default, ref p, callback, out var r, out var errStr);
				//var hr = Cpp.Cpp_AccFind(true, default, iaccParent, ref p, null, out var r, out var errStr);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, null, 0, callback, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, aParent._iacc, "LINK", "Bug Reports", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFromWindow(true, w, 0, out var r);
				//var hr = Cpp.Cpp_AccFromWindow(true, w, (int)AccOBJID.CLIENT, out var r);
				//var hr = Cpp.Cpp_AccFromPoint(true, Mouse.XY, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "BUTTON", null, 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, "New macro    Ctrl+N", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, "Record    Ctrl+K", 0, null, 0, out var r);

				//var hr = Cpp.Cpp_AccFind2(true, w, ref wAgent, ref iaccParent, null, null, 0, null, 0, out var r);

				//Cpp.Cpp_Acc r = default;
				//var hr=Cpp.Cpp_AccFromWindow(true, w, 0, out r.iacc);

				Perf.NW();
				if(hr != 0) {
					//Perf.NW();
					PrintHex(hr);
					if(errStr!=null) Print(errStr);
					continue;
				}
				//continue;
				//var aa = Acc.Find(w, "BUTTON");
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
			iaccParent.Dispose();

			Print(k.Count);
			Print(k);
			//foreach(var v in k) {
			//	Print($"{(IntPtr)v._iacc} {v._elem},  {v.ToString()}");
			//}
			//}
#endif
		}
		finally { Cpp.Cpp_Unload(); }
	}

	//static void TestDllInProcManyWindows()
	//{
	//	try {
	//		var aw = Wnd.GetWnd.AllWindows(true);
	//		foreach(var w in aw) {
	//			if(!w.ClassNameIs("Mozilla*")) continue;
	//			Print($"<><c 0xFF8000>{w}</c>");
	//			//continue;
	//			var a = new List<Acc>();
	//			Perf.First();
	//			Cpp.Cpp_AccFind(true, w, default, "LINK", null, 0, null, 0, out _);
	//			Perf.Next();
	//			var hr = Cpp.Cpp_AccFind(true, w, default, "LINK", null, 0, (ref Cpp.Cpp_Acc r) =>
	//			{
	//				a.Add(new Acc(r.iacc, r.elem));
	//				r.iacc = default;
	//				return 0;
	//			}, 0, out _);
	//			Perf.Next();
	//			if(hr != 1) { Print($"<><c 0xFF>{hr}</c>"); if(a.Count==0) continue; }
	//			Print(a.Count);
	//			Print(a);
	//			Perf.NW();
	//		}
	//	}
	//	finally { Cpp.Cpp_Unload(); }
	//	Print("END");
	//}

	//static void _ClickWait(Acc a, string name)
	//{
	//	Perf.First();
	//	Wnd w = a.WndTopLevel;
	//	string docRole = w.ClassNameIs("IEFrame") ? "web:PANE" : "web:DOCUMENT";
	//	var doc0 = Acc.Find(w, docRole); Print(doc0);
	//	bool checkIsValid = true;
	//	_Print();
	//	a.DoAction();
	//	Wildex x = name;
	//	var to = new WaitFor.LibTimeout(15);
	//	for(; ; ) {
	//		_Print();
	//		if(x.Match(w.Name)) break;
	//		//if(a.RoleInt == 0) break;
	//		to.Sleep();
	//	}
	//	Perf.NW();

	//	void _Print()
	//	{
	//		//if(checkIsValid && doc0.State==0) { checkIsValid=false; Print("old DOCUMENT invalidated"); }
	//		if(checkIsValid && a.RoleInt==0) { checkIsValid=false; Print("LINK invalidated"); }
	//		var doc = Acc.Find(w, docRole);
	//		Print($"'{w.Name}', '{doc.Name}', '{doc.Value}', ({doc.State})");
	//	}
	//}

	static void _ClickWait(Wnd w, Acc a, string name, string old)
	{
		//_TODO: test what Chrome web page objects exist when window name changed before invalidating old DOCUMENT.

		//var w = a.WndTopLevel;
		////Wnd w = default; WaitFor.Condition(3, ()=> { bool ok = !(w = a.WndTopLevel).Is0; if(!ok) Print("wnd 0"); return ok; });
		//Acc.Find(w, "web:", old).OrThrow();

		//var docOld = Acc.Find(w, "web:", secondsTimeout: 0);

		//a.DoAction();
		//for(int i = 0; i < 100; i++) {
		//	//var atest = Acc.Find(w, "web:").OrThrow();
		//	//Cpp.Cpp_InProcTest(atest._iacc);
		//	Print("-----");
		//	Print(Acc.Find(w, "web:").Name);
		//	Print(w.Name);
		//	50.ms();
		//}


		//return;


		Perf.First();
		//a.DoAction();
		//a.DoDefaultActionAndWaitForWindowName(w, name, 10);
		//a.DoDefaultActionAndWaitForInvalid(10);
		//a.DoDefaultActionAndWaitForNewPage(10, w);
		//a.DoActionAndWaitForWebPageLoaded(15);
		a.DoActionAndWaitForNewWebPage(10);

		//a.DoAction();
		//WaitFor.WindowCondition(w, o => o.Name.Like_(name), 10);
		//Perf.Next();
		//Acc.Wait(10, w, "web:", name).Dispose();

		//Print(docOld);
		//Print(docOld.ChildCount);
		//int i;
		//for(i=0; i<10; i++) {
		//	Print(a);
		//	//if(!docOld.GetRect(out var re)) break;
		//	//w.Send(0);
		//	Time.Sleep(14);
		//}
		//Print(i);

		Perf.NW('d');

		//1.s();
		//if(null!=Acc.Find(w, "web:", old)) Print("  old still found");
	}

	enum EBrowser { Chrome, Firefox, IE, Opera }

	static void TestAccScript()
	{
		EBrowser browser;
		browser = EBrowser.Chrome;
		//browser = EBrowser.Firefox;
		//browser = EBrowser.IE;
		//browser = EBrowser.Opera;

		string appName, menuClass;
		switch(browser) {
		case EBrowser.Chrome: appName = "Google Chrome"; menuClass = "Chrome_WidgetWin_2"; break;
		case EBrowser.Firefox: appName = "Mozilla Firefox"; menuClass = "MozillaDropShadowWindowClass"; break;
		case EBrowser.Opera: appName = "Opera"; menuClass = "Chrome_WidgetWin_1"; break;
		default: appName = "Internet Explorer"; menuClass = "#32768"; break;
		}

		string page, link;
		page = "Yahoo - "; link = "Finance";
		//page ="Test - "; link ="Test2";
		//page ="Quick Macros - Support - "; link ="Help online";

		appName = page + appName;
		var w = Wnd.Find(appName).OrThrow();
		w.Activate();

		//Print(Acc.Find(w, "web:")); return;
		//Perf.First(); Acc.Wait(-0.001, w, "web:NN"); Perf.NW(); return;
		//Perf.First(); Acc.Find(w, "web:", secondsTimeout: 3); Perf.NW(); return;
		//Perf.First(); Acc.Find(w, "web:", null, AFFlags.NotInProc, secondsTimeout: 1); Perf.NW(); return;

		AFFlags flags = 0;
		//flags = AFFlags.NotInProc;
		g1:
		Perf.First();
		var a = Acc.Find(w, "web:LINK", link).OrThrow();
		Perf.NW();
		Print(1, a);
		_ClickWait(w, a, "*Finance*", "Trending Now");

		//Acc.Find(w, null, "Back", prop: "notin=DOCUMENT").DoAction();
		//return;

		Perf.First();
		a = Acc.Wait(10, w, "web:LINK", "Industries");
		Perf.NW();
		Print(2, a);
		_ClickWait(w, a, "*Industry*", "My Portfolio & Markets");

		Perf.First();
		a = Acc.Wait(10, w, "web:LINK", "Steel*", flags: flags);
		Perf.NW();
		Print(3, a);
		//var w1 = a.WndContainer;
		//Print(w1.Handle, a);
		_ClickWait(w, a, "*Stock Screener*", "Conglomerates");

		Perf.First();
		a = Acc.Wait(10, w, "web:LINK", "Results List");
		//Perf.NW();
		Print(4, a);

		bool stop = 2 == AuDialog.ShowEx("back", null, "1 Continue|2 Cancel", secondsTimeout: 3);
		//for(int i = 0; i < 3; i++) { //does not work well with Firefox
		//	back.DoAction();
		//	0.1.s();
		//}
		Acc.Find(w, null, "Back", "notin=DOCUMENT").VirtualRightClick();
		var wBackMenu = Wnd.Wait(10, false, null, menuClass, also: o => o.HasStyle(Native.WS.POPUP));
		//Print(wBackMenu);
		a = Acc.Wait(10, wBackMenu, "MENUITEM", "Yahoo");
		//Print(a);
		switch(browser) {
		case EBrowser.Chrome: case EBrowser.Opera: a.VirtualClick(); break; //VirtualClick does not work with Firefox
		default: 100.ms(); a.DoAction(); break; //DoAction does not work with Chrome
		}

		//return;
		if(stop) return;
		Wnd.Wait(30, false, appName);
		3.s();
		Print("---------");
		goto g1;
	}

	static void TestAccFindGetProp()
	{
		var w = Wnd.Find("*- Google Chrome").OrThrow();

		//var f = new Acc.Finder("web:");
		//var f = new Acc.Finder("web:", flags: AFFlags.NotInProc);
		var f = new Acc.Finder("web:BUTTON", "Search");
		//f.ResultGetProperty = 'R';
		//f.ResultGetProperty = 'n';
		//f.ResultGetProperty = 'v';
		//f.ResultGetProperty = 's';
		//f.ResultGetProperty = 'r';
		//f.ResultGetProperty = 'w';
		f.ResultGetProperty = '@';
		if(!f.Find(w)) { Print("not found"); return; }
		Print(f.ResultProperty.GetType());
		//Print(f.ResultProperty);
		Print(f.ResultProperty as Dictionary<string, string>);
		//Print(f.Result);
	}

	static void TestAccCallback()
	{
		var w = Wnd.Find("*- Google Chrome");

		AFFlags flags = 0;
		//flags |= AFFlags.NotInProc;

		var a = Acc.Find(w, "web:LINK", flags: flags, also: o => { Print(o); return false; });
		//var a = Acc.Find(w, "web:LINK", flags: flags, also: o => { Print(o); return o.Name == "Support"; });

		//Perf.First();
		//var a = Acc.Find(w, "web:LINK");
		////var a = Acc.Wait(-0.001, w, "web:LINK");
		//Perf.NW();

		Print(a);
		Print("END");
	}

	static void TestAccFindState()
	{
		var w = Wnd.Find("Options");
		w.Activate();

		AFFlags flags = 0;
		//flags |= AFFlags.NotInProc;

		//var a = Acc.Find(w, "CHECKBOX", flags: flags);
		//var a = Acc.Find(w, "CHECKBOX", prop: "state=CHECKED, !DISABLED\0 notin=ONE,,TWO", flags: flags);
		var a = Acc.Find(w, "CHECKBOX", prop: "state=CHECKED,FOCUSABLE, !FOCUSED\0", flags: flags);
		//var a = Acc.Find(w, "TEXT", prop: "state=PROTECTED", flags: flags);

		Print(a);
		Print("END");
	}

	//static void TestAccHtml()
	//{
	//	//var w = Wnd.Find("* Mozilla Firefox").OrThrow();
	//	var w = Wnd.Find("* Google Chrome").OrThrow();
	//	//var w = Wnd.Find("* Internet Explorer").OrThrow();
	//	//w.Activate();

	//	//var a = Acc.Find(w, "web:").OrThrow();
	//	//var a = Acc.Find(w, "web:", "Support").OrThrow();
	//	//var a = Acc.Find(w, "web:", null, "@href=*f=4*").OrThrow();
	//	//var a = Acc.Find(w, "web:LIST").OrThrow();
	//	var a = Acc.Find(w, "web:LINK", "Test").OrThrow();
	//	//var a = Acc.Find(w, "web:LINK", "Resources").OrThrow();

	//	Print(a);

	//	string attr = "href";
	//	//attr = "class";
	//	//attr = "'o"; //outer HTML
	//	//attr = "'i"; //inner HTML
	//	attr = "'a"; //attributes
	//	//attr = "'s"; //scroll
	//	for(int i = 0; i < 1; i++) {
	//		var hr = Cpp.Cpp_AccWeb(a, attr, out var s);
	//		if(hr != 0) { PrintHex(hr); return; }
	//		if(s != null) Print(s.Replace('\0', ';'));
	//	}
	//}

	static void TestAccNavigate()
	{
		var w = Wnd.Find("* Google Chrome").OrThrow();
		var a = Acc.Find(w, "web:LINK", "Test").OrThrow();

		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		//var a = Acc.Find(w, "BUTTON", "Compil*").OrThrow();
		////var a = Acc.Find(w, "TREEITEM", "init").OrThrow();

		//var w = Wnd.Find("* Internet Explorer").OrThrow();
		////var a = Acc.Find(w, "web:LINK", "Test").OrThrow();
		//var a = Acc.Find(w, "web:TEXT", "Test").OrThrow();

		//var w = Wnd.Find("QM# - Q:*").OrThrow();
		////var a = Acc.Find(w, "LISTITEM", "Test*").OrThrow();
		////var a = Acc.Find(w, "PAGETAB", "Find").OrThrow();
		//var a = Acc.Find(w, "CLIENT", "Panels").OrThrow();

		//var w = Wnd.Find("Test", "CabinetWClass").OrThrow();
		////var a = Acc.Find(w, "BUTTON", "New folder").OrThrow();
		////var a = Acc.Find(w, "CLIENT", "Test").OrThrow();
		//var a = Acc.Find(w, "BUTTON", "Minimize").OrThrow();

		//var w = Wnd.Find("* Notepad").OrThrow();
		//var a = Acc.Find(w, "TITLEBAR").OrThrow();

		//var w = Wnd.Find("* Google Chrome").OrThrow();
		//if(0 != Cpp.Cpp_AccFromWindow(true, w, 0, out var iaccW)) throw new Exception();
		//var aw =new Acc(iaccW);
		//var a = aw.Find("LINK", "Test").OrThrow();

		//var a = Acc.Find(w, "web:LINK", "Test").OrThrow();
		//var w = Wnd.Find("* Google Chrome").OrThrow();
		//var a = Acc.Find(w, "web:LINK", "Test", navig:"pa").OrThrow();

		//Marshal.AddRef(a._iacc); Print(Marshal.Release(a._iacc));
		Print(a);

		string nav;
		//nav = "pa2 child,4 #2,3 ne next pr previous fi first la last pa parent ch child";
		nav = "pa2 ch5";
		nav = "fi pa3 ch5";
		nav = "pa ch2 la";
		nav = "pr2 pr ne2";
		nav = "pa la pr2 ne";
		//nav = "pr10";
		nav = "ch2 ne";
		nav = "pa ne3";
		//nav="pa";
		Perf.First();
		//int hr = Cpp.Cpp_Navigate(a, nav, out var r);
		a = a.Navigate(nav).OrThrow();
		//a = a.Navigate("parent next ch3", 0, true).OrThrow();
		Perf.NW();
		//if(hr != 0) { PrintHex(hr); return; }

		//a.Dispose();
		//a = new Acc(ref r);
		Print(a);
		//Marshal.AddRef(a._iacc); Print(Marshal.Release(a._iacc));

		//Print(a.Html(true));
		//Print(a.HtmlAttribute("no"));
		//Print(Native.GetErrorMessage());

		//string role, name;
		////role = "CLIENT";
		//role = "PROPERTYPAGE";
		//name = "Ribbon";
		////name = "Quick Access Toolbar";

		//var w = Wnd.Find("Test", "CabinetWClass").OrThrow();
		////var w = Wnd.Find("Options").OrThrow();
		////var w = Wnd.Find(null, "QM_Editor").OrThrow();
		//var a = Acc.Find(w, role, name).OrThrow();
		////return;

		//AFFlags flags = 0;
		////flags |= AFFlags.NotInProc;
		//Perf.First();
		//for(int i = 0; i < 4; i++) {
		//	a = Acc.Find(w, role, name, flags: flags).OrThrow();
		//	Perf.Next();
		//}
		//Perf.Write();
		//Print(a);
		//return;
	}

	static void TestAccGetProp()
	{
		//var w1 = Wnd.Find(null, "QM_Editor").OrThrow();
		//var f = new Acc.Finder("TREEITEM");
		//f.ResultGetProperty = Acc.Finder.RProp.WndContainer;
		//if(f.Find(w1)) Print((Wnd)f.ResultProperty);
		//return;

		//var w = Wnd.Find("* Google Chrome").OrThrow();
		//var a = Acc.Find(w, "web:LINK", "Test").OrThrow();

		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		////var a = Acc.Find(w, "BUTTON", "Compil*").OrThrow();
		//var a = Acc.Find(w, "BUTTON", skip: 5, flags: AFFlags.NotInProc).OrThrow();

		//Print(a);


		//test speed inproc and outproc

		var flags = AFFlags.MenuToo;
		//flags |= AFFlags.NotInProc;
		var aw = Wnd.GetWnd.AllWindows(true);
		var k = new List<Acc>(100000);
		foreach(var w_ in aw) {
			var w = w_;
			//if(!w.ClassNameIs("Mozilla*")) continue;
			if(w.ClassNameIs("Internet Explorer_Hidden")) continue;
			if(w.ClassNameIs("Ieframe")) w = w.Child(null, "Internet Explorer_Server");
			//if(!w.ClassNameIs("QM_Editor")) continue;
			//Print(w);
			try {
				Acc.Find(w, flags: flags, also: a =>
				{
					//if(a.RoleInt == AccROLE.WINDOW) return false;

					//a = a.Navigate("pa", false); if(a==null) return false;
					k.Add(a);

					//var p = Perf.StartNew();
					////a._misc.flags = 0;
					//var nl = a.Name.Length;
					//p.Next();
					//var tt = p.TimeTotal;
					//if(tt >= 2000) Print("<><c 0x8000>", tt, a, "</c>");

					//Print(a);
					//Print(a.WndContainer);
					return false;
				});
			}
			catch(Exception ex) { Print($"<><c 0xff>{ex.Message}</c>"); }
			//break;
		}
		//return;
		Print("");
		long lenIP = 0, lenNIP = 0;
		Perf.Cpu(500);
		Perf.First();
		var sb = new StringBuilder(1000000);
		foreach(var a in k) {
			//var p = Perf.StartNew();
			//Print(a._misc.flags);
			//a._misc.flags = 0;

			//lenIP += a.Name.Length;
			//lenIP += a.Value.Length;
			//lenIP += a.Description.Length;
			//lenIP += a.Help.Length;
			//lenIP += a.KeyboardShortcut.Length;
			//lenIP += a.DefaultAction.Length;
			//var r = a.Rect; lenIP += r.Width + r.Height;
			//lenIP += (long)a.State;
			//lenIP += (LPARAM)a.WndContainer;

			sb.AppendLine(a.ToString());
#if true
			//p.NW();
#else
			p.Next();
			var tt = p.TimeTotal;
			Print(tt);
			if(tt >= 10000) Print(tt, a, a.WndTopLevel);
#endif
			//a._misc.flags = 0;
			//lenNIP += a.Name.Length;

		}
		Perf.NW();
		Print(sb);
		Print(k.Count, lenNIP, lenIP);
	}

	static void TestWndAccName()
	{
		var k = new List<Wnd>(10000);
		foreach(var w in Wnd.GetWnd.AllWindows(true)) {
			k.AddRange(w.Get.Children());
		}
		Perf.Cpu(200);
		Perf.First();
		long g = 0;
		for(int i = 0; i < 5; i++) {
			g = 0;
			foreach(var w in k) {
				//Perf.First();
				var s = w.NameAcc;
				//Perf.NW();
				//Print(s);
				g += s?.Length ?? 0;
			}
			Perf.Next();
		}
		Perf.Write();
		Print(k.Count, g);
	}

	static void TestAccIsInvisible()
	{
		var w = Wnd.Find("*Mozilla Firefox").OrThrow();
		Acc.Find(w, flags: AFFlags.HiddenToo, also: a =>
		{
			if(a.IsInvisible) Print($"<><c 0x808080>{a}</c>"); else Print(a);
			return false;
		});

		//AFFlags flags = 0;
		////flags |= AFFlags.HiddenToo;
		//var w = Wnd.Find("* Notepad").OrThrow();
		////var w = Wnd.Find("* Word").OrThrow();
		//Print(w);
		//Print(Acc.Find(w, "MENUITEM", "Paste*", flags: flags));
	}

	static void TestAccAction()
	{
		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		//var w = Wnd.Find("Options").OrThrow();
		var w = Wnd.Find("* Google Chrome").OrThrow();
		//Print(w);
		//w.Activate(); 100.ms();

		AFFlags flags = 0;
		//flags |= AFFlags.NotInProc;

		//var a = Acc.Find(w, "BUTTON", "Prev*", flags: flags).OrThrow();
		//a.DoAction();

		//var a = Acc.Find(w, "TEXT", flags: flags).OrThrow();
		//a.Value = "TEST";

		//var a = Acc.Find(w, "CHECKBOX", "Unicode", flags: flags).OrThrow();
		//var a = Acc.Find(w, "LISTITEM", "Mouse", flags: flags).OrThrow();
		//var a = Acc.Find(w, "LIST", flags: flags).OrThrow();
		//var a = Acc.Find(w, "COMBOBOX", flags: flags).OrThrow();
		//var a = Acc.Find(w, "class=bosa_sdm_Microsoft Office Word 11.0:COMBOBOX", flags: flags).OrThrow();
		//var a = Acc.Find(w, "class=bosa_sdm_Microsoft Office Word 11.0:LIST", flags: flags).OrThrow();
		//var a = Acc.Find(w, "COMBOBOX", flags: flags).OrThrow().Navigate("fi");
		var a = Acc.Find(w, "LINK", "**r ^(T|G)", flags: flags).OrThrow();
		//a = a.Find("LIST", flags: AFFlags.HiddenToo);
		Print(a);
		//Print(a.Html(true));
		//Print(a.Html(false));
		//Print(a.HtmlAttribute("href"));
		//Print(a.HtmlAttribute("style"));
		//Print(a.HtmlAttribute("class"));
		//Print(a.HtmlAttribute("no"));
		//Print(a.HtmlAttributes());
		//Print(a.HtmlAttributes()["href"]);

		//foreach(var v in a.GetMultipleProperties("rnvdhaksRwoi@")) {
		//	if(v is Dictionary<string, string> d) Print(d); //HTML attributes
		//	else Print(v);
		//}

		//a.WndContainer.Focus();
		//PrintHex(Cpp.Cpp_AccSelect(a, AccSELFLAG.TAKEFOCUS));
		//PrintHex(Cpp.Cpp_AccSelect(a, AccSELFLAG.TAKEFOCUS| AccSELFLAG.TAKESELECTION));
		//PrintHex(Cpp.Cpp_AccSelect(a, AccSELFLAG.TAKESELECTION));

		//a.Focus();
		//a.Focus(true);

		//Print(a.SelectedChildren);
	}

	static void TestAccRectHighDPI()
	{
		var w = Wnd.Find("IrfanView");
		Acc.Find(w, also: o => { Print(o); return false; });
	}

	static void TestAccOpenOffice()
	{
		var w = Wnd.Find("*- OpenOffice*").OrThrow();
		//var w = Wnd.Find("*- LibreOffice*").OrThrow(); //need NotInProc and must be process of different bitness
		var flags = AFFlags.NotInProc;
		flags = 0;
		//Acc.PrintAll(w, flags: flags);
		//Print("----");
		var a = Acc.Find(w, "BUTTON", "*PDF", flags: flags).OrThrow();
		//var a = Acc.Find(w, "CLIENT", flags: flags).OrThrow();
		Print(a);
	}

	static void TestAccJava()
	{
		var w = Wnd.Find("Java Control Panel").OrThrow();
		var a = Acc.Find(w, "java:push button", "About*", flags: AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "java:label", "to add items*", flags: AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "java:panel", "enable logging*", flags: AFFlags.NotInProc).OrThrow();

		//var w = Wnd.Find("Network Settings").OrThrow();
		//var a = Acc.Find(w, "java:text", "Automatic proxy script location", flags: AFFlags.NotInProc).OrThrow();

		w.Activate();

		//a = a.Navigate("pa");
		//a = a.Navigate("pa2 ch8 fi");
		//a = a.Navigate("pa ne3");
		//a = a.Navigate("pa2 la fi");

		Print(a);

		//Acc.PrintAll(w, "java:", AFFlags.NotInProc);

		//Print(a.Navigate("pa2").ChildCount);
		//Print(a.Navigate("pa2").GetChildCount(false));
		//Print(a.Navigate("pa2").GetChildCount(true));		//Print(a.DefaultAction);
		//Print(a.Description);
		//Print(a.SimpleElementId);
		//Print(a.Help);
		//Print(a.IsReadonly);
		//Print(a.KeyboardShortcut);
		//Print(a.Name);
		//Print(a.Rect);
		//Print(a.Role);
		//Print(a.SelectedChildren);
		//Print(a.State);
		//Print(a.Value);
		//Print(a.WndContainer);
		//a.Value = "55"; Print(a.Value);

		//a.DoAction();
		//a.DoJavaAction();
		//a.DoJavaAction("moo");
		//a.DoJavaAction("click");
		//a.DoJavaAction("select-line");

		//a.Focus();
		//a.VirtualClick();
		//a.Select(AccSELFLAG.TAKESELECTION);
		//a.MouseClick();

		Print("END");
	}

	static void TestAccFromPoint()
	{
		//var w = Wnd.FromMouse();
		//var a = Acc.FromWindow(w);
		//Print(a);
		//Print(a.MiscFlags);

		//2.s();
		Acc a = null;
		AXYFlags flags = 0;
		flags |= AXYFlags.PreferLink;
		//flags |= AXYFlags.UIA;
		if(Keyb.IsScrollLock) flags |= AXYFlags.NotInProc;

		Perf.First();
		for(int i = 0; i < 5; i++) {
			//a = Acc.FromXY(1270, 1290);
			a = Acc.FromMouse(flags);
			Perf.Next();
		}
		Perf.Write();
		//Print(a!=null);
		Print(a);
		Print(a._misc.flags);
		a.Dispose();
	}

	static void TestAccChildCount()
	{
		//var kk =Wnd.GetWnd.AllWindows(true);
		//Print("----");
		//foreach( var v in kk) {
		//	Print(v);
		//	if(v.IsCloaked) Print($"0x{v.Style:X} 0x{v.ExStyle:X}");
		//}
		//return;

		Wnd w;
		//w = Wnd.Find("Quick*", "QM_Editor");
		//w = Wnd.Find("* Internet Explorer");
		//w = Wnd.Find("*Mozilla Firefox");
		//w = Wnd.Find("* Google Chrome");
		//w = Wnd.Find("* Opera");
		//w = Wnd.Find("* OpenOffice*");
		//w = Wnd.Find("Options");
		w = Wnd.Find("Java *");
		//w = Wnd.Find("Settings");
		//w = Wnd.Find("QM# - Q*", "WindowsForms*");

		Print(w);
		//Acc.PrintAll(w, flags: AFFlags.NotInProc);

		//var a = Acc.Find(w, "TOOLBAR", skip:1).OrThrow();
		//var a = Acc.Find(w, "TOOLBAR", flags: AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "PAGETABLIST").OrThrow();
		//var a = Acc.Find(w, "LINK", skip:2).OrThrow();
		var a = Acc.Find(w, "java:tree").OrThrow();
		Print(a);
		Print(a.State);
		Print(a.ChildCount);
	}

	static void TestAccFromFlags()
	{
		//var a = Acc.FromMouse(AXYFlags.PreferLink);
		////var a = Acc.FromMouse(AXYFlags.NoThrow);
		//Print(a);
		//Print(a.InProc);

		//var w = Wnd.Find(null, "QM_Editor");
		var w = Wnd.Find("* Chrome");

		//w.Activate();
		//Mouse.Move(1000, 1500);

		//var a = Acc.FromWindow(w);
		var a = Acc.FromWindow(w, flags: AWFlags.NotInProc);
		Print(a);
		Print(a.MiscFlags);

		//foreach(var c in w.Get.Children()) {
		//	Print(c);
		//	//var a2 = Acc.FromWindow(c, AccOBJID.CLIENT);
		//	var a2 = Acc.FromWindow(c, AccOBJID.CLIENT, flags: AWFlags.NoThrow);
		//	Print(a2);
		//	Print(a2.InProc);
		//}

		//Print(Acc.Find(w, "TOOLBAR"));
	}

	static void TestAccWnd()
	{
		//var w = Wnd.Find("*Chrome").OrThrow();
		//var a = Acc.Find(w, "web:LINK", "General").OrThrow();
		////var a = Acc.Find(w, "web:LINK", "General", flags: AFFlags.NotInProc).OrThrow();
		//a = a.Navigate("pa");
		//Wnd w2 = default;
		//Perf.First();
		//for(int i = 0; i < 5; i++) {
		//	w2 = a.WndContainer;
		//	Perf.Next();
		//}
		//Perf.Write();
		//Print(w2);

		//var w = Wnd.Find(null, "QM_Editor");
		////var w = Wnd.Find("* Chrome");
		//var a = Acc.Find(w, "CLIENT");
		//Print(a);
		//Print(!a.NotInProc);
		//Print(w.NameAcc);

		AFFlags flags = 0;
		flags |= AFFlags.NotInProc;

		var w = Wnd.Find("Options");
		//var a = Acc.Find(w, "CHECKBOX").OrThrow();
		var a = Acc.Find(w, "CHECKBOX", null, "state=DISABLED", flags: flags).OrThrow();
		//var a = Acc.Find(w, "CHECKBOX", null, "state=1", flags: flags).OrThrow();
		Print(a);
		//a = Acc.Find(w, "CHECKBOX", null, $"rect={a.Rect}", flags: flags).OrThrow();
		//a = Acc.Find(w, "CHECKBOX", null, "rect={L=1155 T=1182 W=132 H=13}", flags: flags).OrThrow();
		a = Acc.Find(w, "CHECKBOX", null, "rect={L=1155 T=1182 W=132 H=13}", flags: flags).OrThrow();
		Print(a);
	}

	static void TestAccMisc1()
	{
		//var w = Wnd.Find("* Chrome");
		//w = w.Child(null, "*render*").OrThrow();
		//var a = Acc.Find(w, "web:LINK", "General");
		//w.Activate();
		//var a = Acc.Focused();
		//var a = Acc.FromMouse(AXYFlags.PreferLink);
		//Print(a);

		var w = Wnd.Find("Java *");
		//var w = Wnd.Find("*Android Studio*");
		Acc.PrintAll(w);
		//Acc.PrintAll(w, "web:LINK");
		//Print(Acc.Find(w, "web:LINK"));
		//var a = Acc.Find(w, "root pane").OrThrow();
		//Print(a);
		//Print(a.Navigate("pa"));
	}

	static void TestAccVariousApps2()
	{
		var w = Wnd.Find("* Eclipse").OrThrow();
		Acc.PrintAll(w);
		//Perf.First();
		//var a = Acc.Find(w, "web:LINK", "Tutorials*").OrThrow();
		var a = Acc.Find(w, "CHECKBOX", "C/C++").OrThrow();
		//Perf.NW();
		Print(a);
	}

	static void TestAccUia()
	{
		var w = Wnd.Find("* Microsoft Edge").OrThrow();
		//var w = Wnd.Find("Ensemble").OrThrow(); //JavaFX
		//var w = Wnd.Find("* Chrome").OrThrow();
		//var w = Wnd.Find("*Sandcastle*").OrThrow();
		//var w = Wnd.Find("* Notepad").OrThrow();
		//var w = Wnd.Find(null, "CabinetWClass").OrThrow();
		//var w = Wnd.Find("FileZilla").OrThrow();
		//var w = Wnd.Find("Java *").OrThrow();

		AFFlags flags = 0;
		flags |= AFFlags.UIA;
		//flags |= AFFlags.HiddenToo;
		//flags |= AFFlags.NotInProc;
		//flags |= AFFlags.MenuToo;

		//Acc.PrintAll(w, flags: flags); return;

		Perf.First();
		Acc a = null;
		//UIA.IElement e = null;
		for(int i = 0; i < 1; i++) {
			//a= Acc.Find(w, "LINK", flags: flags).OrThrow();
			a = Acc.Find(w, "LINK", "Bug Reports", flags: flags).OrThrow();
			//a= Acc.Find(w, "LINK", "Board index", flags: flags).OrThrow();
			//a= Acc.Find(w, prop:"help=YouTube Home", flags: flags).OrThrow();
			//a= Acc.Find(w, "LINK", "HBox", flags: flags).OrThrow();
			//a= Acc.Find(w, prop: "uiaid=JavaFX403", flags: flags).OrThrow();
			//a = Acc.Find(w, "LISTITEM", flags: flags).OrThrow();
			//a= Acc.Find(w, "BUTTON", prop: "elem=12", flags: flags).OrThrow();
			//a= Acc.Find(w, name:"About...", flags: flags).OrThrow();
			//a= Acc.Find(w, "filler", flags: flags).OrThrow();
			//a= Acc.Find(w, name: "Orientation", flags: flags).OrThrow();
			//a= Acc.Find(w, prop:"uiaid=sb_form_q", flags: flags).OrThrow();

			//a = Acc.FromMouse();
			//a = Acc.FromMouse(AXYFlags.UIAutomation| AXYFlags.PreferLink);
			//a = Acc.Focused(true);

			//e = AElement.Find(w, "Bug Reports", UIA.TypeId.Hyperlink);
			//e = AElement.Find(w, "\"Psichologo komentaras\": Apie meditaciją biure", UIA.TypeId.Hyperlink);
			//e = AElement.Find(w, "VBox", UIA.TypeId.Hyperlink);
			Perf.Next();

			//if(a != null) {
			//	//Print("-->");
			//	var e = AElement.FromAcc(a, false);
			//	//Print("<--");
			//	Perf.Next();
			//	Print(e != null);
			//	Print(e?.Name);
			//}

			//foreach(var link in Acc.FindAll(w, "LINK", flags: flags)) {
			//	//foreach(var link in Acc.FindAll(w, "LINK", "*a*", flags: flags)) {
			//	//foreach(var link in Acc.FindAll(w, "LINK", prop: "value=*forumdisplay.php*", flags: flags)) {
			//	//foreach(var link in Acc.FindAll(w, "LINK", flags: flags, also: o => o.Value.Like_("*forumdisplay.php*"))) {
			//	//foreach(var link in Acc.FindAll(w, "////////LINK", flags: flags)) {
			//	//foreach(var link in Acc.FindAll(w, "LINK", prop: "level=8 1000", flags: flags)) {
			//	//foreach(var link in Acc.FindAll(w, prop: "level=2 5", flags: flags)) {
			//	Print(link);
			//	Print(link._misc.flags);
			//}
		}
		Perf.Write();
		Print(a);
		//Print(e?.Name);

		Print(a._misc.flags);
		//a = a.Navigate("pa9");
		//a = a.Navigate("pa fi");
		////a = a.Find();
		//Print(a);
		//Print(a._misc.flags);

		//Print(a.WndContainer);
		//Print(a.WndTopLevel);

		//Print(a.UiaId);
		//Print(a.GetMultipleProperties("u"));

		//Print(a.ChildCount);
		//Print(a.Help);
		//Print(a.KeyboardShortcut);
		////Print(a.Html(true));
		//Print(a.HtmlAttribute("href"));

		//w.Activate();
		//Print(Wnd.Active);
		//a.DoAction(); //yes, but Edge bug: scrolls and calls SetForegroundWindow(direct container control).
		//a.ScrollTo(); //yes
		//a.VirtualClick(); //no
		//a.VirtualRightClick(); //no
		//a.MouseClick();
		//a.DoJavaAction(); //yes in JavaFX (but activates window). In Edge just scrolls.
		//Print(a.Find("BUTTON", "T*"));
		//Print(a.Find("STATICTEXT"));
		//a.Focus(); //yes
		//Print(a.Navigate("pa2 pr2 ne ch2")); //yes
		//a.Select(AccSELFLAG.REMOVESELECTION); //yes

		//a = a.Navigate("pa");
		////Print(a.SelectedChildren);

		//for(int i = 0; i < 5; i++) {
		//	Perf.First();
		//	var sa =a.SelectedChildren;
		//	//var sa = a.FindAll(prop: "level=0\0 state=SELECTED"); //much slower
		//	Perf.NW();
		//	foreach(var v in sa) {
		//		Print(v);
		//		Print(v._misc.flags);
		//	}
		//}

		//1.s();
		//Print(Wnd.Active);
		Print("-- END --");
	}

	static void TestAccFindNavig()
	{
		var w = Wnd.Find("* Chrome").OrThrow();
		var a = Acc.Find(w, "web:LINK", "Bug*").Navigate("ch1", 2);
		Print(a);
		Print("---");
	}

	static void TestAccFromWindowJavaUIA()
	{
		var w = Wnd.Find("Java*");
		//var w = Wnd.Find("Ensemble");
		//var a = Acc.FromWindow(w);
		var a = Acc.FromWindow(w, AccOBJID.Java);
		//var a = Acc.FromWindow(w, AccOBJID.UIAutomation);
		Print(a);
		Print(a.Navigate("fi"));
		//Print(a.Find(name:"Pause"));
		Print(a.Find(name: "View..."));
	}

	//static void TestSpeedDllAndCOM()
	//{
	//	var x = Cpp.Cpp_Interface();

	//	Perf.SpinCPU(100);
	//	for(int i1 = 0; i1 < 5; i1++) {
	//		int n2 = 1000;
	//		Perf.First();
	//		for(int i2 = 0; i2 < n2; i2++) { Cpp.Cpp_TestInt(1, 2, 3); }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { Cpp.Cpp_TestString("fffffffffffffffffffffffffffffffffffffffffffffff", 2, 3); }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { x.TestInt(1, 2, 3); }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { x.TestString("fffffffffffffffffffffffffffffffffffffffffffffff", 2, 3); }
	//		Perf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { x.TestBSTR("fffffffffffffffffffffffffffffffffffffffffffffff", 2, 3); }
	//		Perf.NW();
	//	}

	//}

	static void TestAccScrollTo()
	{
		//Print(Acc.Focused()); return;

		//var w = Wnd.Find("Quick Macros Forum - Mozilla Firefox", "Moz*").OrThrow();
		//var w = Wnd.Find("Quick Macros Forum - Google Chrome", "Chrome*").OrThrow();
		//var w = Wnd.Find("Au - Microsoft Visual Studio ").OrThrow();
		//var w = Wnd.Find("FileZilla").OrThrow();
		var w = Wnd.Find("Quick*").OrThrow();
		//var w = Wnd.Find("* Edge").OrThrow();

		//Acc.PrintAll(w, "web:LINK"); return;
		//var a = Acc.Find(w, "web:LINK", "Bug*").OrThrow();
		//var a = Acc.Find(w, "TREEITEM", "Test Projects", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "TREEITEM", "Downloads", flags: AFFlags.UIAutomation| AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "TREEITEM", "Downloads", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "LISTITEM", "web", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "TREE", flags: AFFlags.UIAutomation| AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "TREE", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "TREEITEM").OrThrow();
		//var a = Acc.Find(w, "LINK", "Bug*", flags: AFFlags.UIAutomation).OrThrow();
		Perf.First();
		var a = Acc.FromWindow(w);
		Perf.NW();

		Print(a);
		Print(a._misc.flags);

		//a.ScrollTo();
		//Print(a.UiaId);
	}

	static void TestAccRoleString()
	{
		var w = Wnd.Find("* Chrome").OrThrow();
		////var w = Wnd.Find("* Edge").OrThrow();
		////var w = Wnd.Find("Ensemble").OrThrow();
		////var w = Wnd.Find("Java *").OrThrow();
		//var w = Wnd.Find("* OpenOffice *").OrThrow();
		//var w = Wnd.Find("Task Scheduler").OrThrow();
		Print(w);

		AFFlags flags = 0;
		//flags |= AFFlags.UIAutomation;
		flags |= AFFlags.HiddenToo | AFFlags.MenuToo;

		Acc.PrintAll(w, flags: flags);
		//foreach(var a in Acc.FindAll(w, flags: flags)) Print(a.Role);

		//Print(Acc.Find(w, "web:div", flags: flags, skip: 1));

		//foreach(var w in Wnd.GetWnd.AllWindows()) {
		//	Print($"<><c 0xff0000>{w.ToString()}</c>");
		//	string name = w.Name;
		//	if(name.EndsWith_(" Edge") || name == "Ensemble") flags |= AFFlags.UIAutomation; else flags &= ~AFFlags.UIAutomation;
		//	Acc.PrintAll(w, flags: flags);
		//}
	}

	static void TestAccEverything()
	{
		//var w = Wnd.Find("* Chrome");
		var w = Wnd.Find("* Notepad");

		//var a = Acc.Find(w, flags: AFFlags.UIA);
		//var a = Acc.Find(w, "BUTTON", flags: AFFlags.UIA| AFFlags.NotInProc|AFFlags.HiddenToo| AFFlags.MenuToo);
		var a = Acc.Find(w, "BUTTON", flags: AFFlags.NotInProc | AFFlags.MenuToo);

		//var a = Acc.FromWindow(w, AccOBJID.UIA, flags: AWFlags.InProc);
		////var a = Acc.FromWindow(w, AccOBJID.UIA);

		Print(a);
		//a.Dispose();
	}

	//ref struct Re2
	//{
	//	public int i, j;

	//	public void Met()
	//	{
	//		var p = &j;

	//	}
	//}

	//static void TestInParam(in RECT r, ref Re2 k)
	//{
	//	Print(r);
	//	var p = &k;
	//	Print(p);
	//}

	static void TestCSharp72()
	{
		//Span<char> span = default;
		//RECT r=default;
		//Re2 k = default;
		//TestInParam(r, ref k);
	}

	public static int LevenshteinDistance(string s, string t)
	{
		int n = s.Length;
		int m = t.Length;
		int[,] d = new int[n + 1, m + 1];

		// Step 1
		if(n == 0) {
			return m;
		}

		if(m == 0) {
			return n;
		}

		// Step 2
		for(int i = 0; i <= n; d[i, 0] = i++) {
		}

		for(int j = 0; j <= m; d[0, j] = j++) {
		}

		// Step 3
		for(int i = 1; i <= n; i++) {
			//Step 4
			for(int j = 1; j <= m; j++) {
				// Step 5
				int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

				// Step 6
				d[i, j] = Math.Min(
					Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
					d[i - 1, j - 1] + cost);
			}
		}
		// Step 7
		return d[n, m];
	}

	static void TestLevenshteinDistance()
	{
		Print(LevenshteinDistance("kitten", "sitting"));
		0.1.s();
		int n = 0;
		Perf.Cpu(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { n += LevenshteinDistance("kitten", "sitting"); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			Perf.NW();
		}
		Print(n);
	}

	static void TestNewColorTypes()
	{
		//var c = new ColorInt(0xff, false);
		//c = new ColorInt(Color.Blue);
		//c = Color.Blue;
		//c=0xff;
		////Print(ColorInt.FromString("orange", out c));
		////Print(ColorInt.FromString("0xFF", out c));
		////Print(ColorInt.FromString("0x123456", out c));
		////Print(ColorInt.FromString("0x1234567", out c));
		////Print(ColorInt.FromString("#123456", out c));
		////Print(ColorInt.FromString("#1234567", out c));
		////Print(ColorInt.FromString("#w", out c));
		////Print(ColorInt.FromString("0x", out c));
		////Print(ColorInt.FromString("0xm", out c));
		////Print(ColorInt.FromString("0xA", out c));
		////Print(ColorInt.FromString("melyna", out c));
		////c=ColorInt.FromBGR(0xff, false);
		////c=ColorInt.FromBGR(0x80ff, true);
		////Print(c);
		////PrintHex(c.ToBGR());
		//////Print(c.AdjustLuminance(500));
		//////Print(c.AdjustLuminance(-500));
		////Print(c.Brightness0to255());

		//var k = c;
		//bool b1 = (c == k);
		//bool b2 = (c == 0xff);
		//bool b3 = (c == unchecked((int)0xff0000ff));
		//bool b4 = (c == Color.Blue);
		////bool b5 = (Color.Blue==c); //error

		//Print(b1, b2, b3, b4);

		//var w = Wnd.Find("* Notepad").OrThrow();
		//w.SetTransparency(true, null, 0xF0F0F0);
		////w.SetTransparency(true, null, Color.Black);

		////int c = 0x0000ff; //blue
		////ColorInt c = 0x0000ff;
		////Color c = Color.FromArgb(0x0000ff);
		//int[] c = { 0x8000, 0x0000ff };
		////string[] c = { "one", "two" };
		//WinImage.Find(Wnd.Find("Quick *"), c).MouseMove();


	}

	//static void TestNativeWindow()
	//{
	//	var w = new NatWin();
	//	var c = new CreateParams() { Caption = "test", Height = 100, Width = 100, X = 100, Y = 100 };
	//	c.Style = unchecked((int)(Native.WS.POPUPWINDOW|Native.WS.VISIBLE));
	//	c.ExStyle = (int)Native.WS_EX.TOPMOST;

	//	w.CreateHandle(c);

	//	AuDialog.Show("test");

	//	w.DestroyHandle();
	//}

	//public class NatWin :NativeWindow
	//{
	//	protected override void WndProc(ref Message m)
	//	{
	//		Print(m);
	//		base.WndProc(ref m);
	//	}
	//}

	//class ThreadExitEvent
	//{
	//	public ThreadExitEvent()
	//	{
	//		Print(Api.GetCurrentThreadId());
	//	}
	//	~ThreadExitEvent()
	//	{
	//		Print(Api.GetCurrentThreadId());
	//	}
	//}

	//static void TestThreadExitEvent()
	//{
	//	Print(Api.GetCurrentThreadId());

	//	t_ttt = 9;

	//	var k = Cpp.Cpp_ThreadExitEvent(Marshal.GetFunctionPointerForDelegate(_onThreadExit));
	//	//var k = Cpp.Cpp_ThreadExitEvent(Marshal.GetFunctionPointerForDelegate(new Action(()=> { Print($"t_ttt={t_ttt}"); })));
	//	//Cpp.Cpp_ThreadExitEvent2(Marshal.GetFunctionPointerForDelegate(_onThreadExit));

	//	//var h = GCHandle.Alloc(k, GCHandleType.Pinned);

	//	//Marshal.ReleaseComObject(k);
	//	Print("END");
	//}

	//static Action _onThreadExit = _OnThreadExit;
	//static void _OnThreadExit()
	//{
	//	Print($"t_ttt={t_ttt}");
	//}

	//[ThreadStatic] static int t_ttt;
	//[ThreadStatic] static ThreadExitEvent t_tee;


	//class FormTEE : Form
	//{
	//	protected override void WndProc(ref Message m)
	//	{
	//		Print(m);
	//		base.WndProc(ref m);
	//	}
	//}

	//static void TestThreadExitEvent2()
	//{
	//	var f = new FormTEE();
	//	f.Show();
	//	AuDialog.Show("test");
	//}

	//static void _DomainCallback()
	//{
	//	Output.LibUseQM2 = true;

	//	var d =AppDomain.CurrentDomain;
	//	Print(d.Id, d.FriendlyName);
	//}

	//static void TestNativeCallbackInMultipleAppDomains()
	//{
	//	for(int i = 0; i < 3; i++) {
	//		var t = new Thread((o) =>
	//		  {
	//			  var d = AppDomain.CreateDomain("ad" + o);
	//			  d.DoCallBack(_DomainCallback);
	//			  AppDomain.Unload(d);
	//		  });
	//		t.Start(i);
	//	}
	//	1.s();
	//}

	//static LPARAM _WndProc1(Wnd w, uint msg, LPARAM wParam, LPARAM lParam)
	//{
	//	Wnd.Misc.PrintMsg(w, msg, wParam, lParam);

	//	var R = Wnd.Misc.DefWindowProc(w, msg, wParam, lParam);

	//	return R;
	//}

	//static void TestRegisterWndclassWithCbtHook()
	//{
	//	var atom = Wnd.Misc.MyWindowClass.InterDomainRegister("aa test", _WndProc1);
	//	var style =Native.WS.OVERLAPPEDWINDOW |Native.WS.VISIBLE;
	//	var w = Wnd.Misc.MyWindowClass.InterDomainCreateWindow("aa test", "Test1", style, 0, 200, 200, 200, 200);
	//	AuDialog.Show("--");
	//	Wnd.Misc.DestroyWindow(w);
	//}

	class MyWindow2 :Wnd.Misc.MyWindow
	{
		protected override LPARAM WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
		{
			Wnd.Misc.PrintMsg(w, message, wParam, lParam);

			//switch(message) {
			//case Api.WM_NCCREATE:

			//	return 0;
			//}

			return base.WndProc(w, message, wParam, lParam);
		}
	}

	static void TestMyWindow()
	{
		//var e = new Wnd.Misc.MyWindow.WndClassEx() { };
		var e = new Wnd.Misc.MyWindow.WndClassEx() { hbrBackground = (IntPtr)(Api.COLOR_INFOBK + 1) };

		Wnd.Misc.MyWindow.RegisterClass("MyWindow", ex: e);
		var style = Native.WS.CAPTION | Native.WS.SYSMENU | Native.WS.VISIBLE;

		var x = new MyWindow2();
		if(!x.Create("MyWindow", "MyWindow", style, 0, 200, 200, 200, 200)) { Print("failed"); return; }
		Timer_.After(1000, () => { GC.Collect(); });
		AuDialog.Show("--");
		x.Destroy();

		//var b = new AuToolbar();
		//b["one"] = o => Print(o);
		//b.Visible = true;
		//AuDialog.Show("--");
	}

	static void TestOnScreenRect()
	{
		var r = new RECT(100, 100, 100, 100, true);
		var x = new OsdRect();
		x.Rect = r;
		//x.Color = 0xFF0000; //red
		x.Color = Color.Orange;
		x.Thickness = 2;
		//x.Opacity = 0.3;
		x.Show();

		//for(int i = 0; i < 6; i++) {
		//	0.3.s();
		//	//x.Visible = !x.Visible;
		//	//r = x.Rect; r.Offset(20, 20); x.Rect = r;
		//	//x.Thickness += 1;
		//	//x.Opacity += 0.1;
		//	x.Color = x.Color.AdjustLuminance(-100);
		//}
		Timer_.Every(300, t =>
		{
			//x.Visible = !x.Visible;
			//r = x.Rect; r.Offset(20, 20); x.Rect = r;
			//x.Opacity += 0.03;
			x.Thickness += 1;
		});
		AuDialog.Show("test");
		//var f = new Form(); f.ShowDialog();
		//2.s();
		//x.Dispose();
	}

	public class AccEtc
	{
		string _s;
		public string value, description, help, action, key, uiaid, attributes, notin;
		public object state, level, rect, elem, maxcc;

		public AccEtc() { }

		public AccEtc(
			string value, string description, string help, string action, string key, string uiaid, string attributes,
			string notin, object state, object level, object rect, object elem, object maxcc)
		{

		}

		public static implicit operator AccEtc(string s) => new AccEtc() { _s = s };
		public static implicit operator string(AccEtc x)
		{
			string s = x._s;
			if(s == null) {
				var b = new StringBuilder();
				_Add(x.value);
				_Add(x.description);
				_Add(x.help);
				_Add(x.action);
				_Add(x.key);
				_Add(x.uiaid);
				_Add(x.attributes);
				_Add(x.notin);
				_Add(x.state);
				_Add(x.level);
				_Add(x.rect);
				_Add(x.elem);
				_Add(x.maxcc);

				void _Add(object t)
				{
					if(t == null) return;
					if(b.Length != 0) b.Append("\0 ");
					b.Append(t);
				}

				s = b.ToString();
			}
			return s;
		}
	}

	static void TestAccPropFormat()
	{
		//var w1 = Wnd.Find("* Chrome").OrThrow();
		//var a1 = Acc.Find(w1, "web:BUTTON", "Search").OrThrow().Navigate("pa").OrThrow();
		//Print(a1 != null);
		//return;

		//Print("one" + @"\two" + $"{3}" + $@"f{0}\ur"); return;
		//Print(@"one" + "\0" + @"two"); return;
		//Print(@"one\n" + @"two"); return;
		//Print(new string[] { @"one", @"two" });

		//AccFind(default, "role", more: "ffff", more: "gggg");

		//RECT r = new RECT(1, 2, 3, 4, true);
		//Tyyp(r);
		////Tyyp(in r);
		//Tyyp();

		//Kuu("one=ONE\0 two=TWO\0 three=THREE");
		//Kuu(one: "ONE", two: "TWO", three: "THREE");
		Wnd ww = default;
		Acc.Find(ww, "role", prop: "one=ONE\0 two=TWO\0 three=THREE");
		Acc.Find(ww, "role", prop: "one=ONE\n two=TWO\n three=`TH\r\nREE`");
		Acc.Find(ww, "role", prop: "one=ONE\n" + @"two=TWO\n" + "three=`TH\r\nREE`");
		Acc.Find(ww, "role", prop: "one=ONE\0" + @"two=TWO" + "\0" + "three=`TH\r\nREE`");
		Acc.Find(ww, "role", prop: new AccEtc() { value = "ONE", uiaid = "TWO", state = "STATE" });
		Acc.Find(ww, "role", prop: @"
one=ONE
two=TWO
three=`TH
REE`");
	}

	static void TestDynamicExpandoObject()
	{
		dynamic d = new ExpandoObject();
		d.Moo = 5;
		Print(d.Moo);
	}

	static void _TestTupleParams((int one, string two) prop = default, int more = 0)
	{
		Print(prop.one, prop.two, more);
	}

	static void TestTupleParams()
	{
		//_TestTupleParams(more: 3);
		_TestTupleParams((1, "two"));
		_TestTupleParams((one: 8, two: "two"));
		//_TestTupleParams((two: "two", one: 8)); //error
		//_TestTupleParams((two: "two")); //error
	}

	static void TestAccThrowOperator()
	{
		////Wnd.Find("Quick Macros Forum - Google Chrome-", "Chrome_WidgetWin_1").MouseMove();
		////Print((+Wnd.Find("Quick Macros Forum - Google Chrome-", "Chrome_WidgetWin_1")).ThreadId);
		//Print(Wnd.Find("Quick Macros Forum - Google Chrome-", "Chrome_WidgetWin_1").OrThrow().ThreadId);
		//return;

		//var w = Wnd.Find("Quick Macros Forum - Google Chrome", "Chrome_WidgetWin_1").OrThrow();
		////Acc a = null;
		////a = Acc.Find(w, "web:LINK", "Example"); //if not found, sets a = null
		////a = -Acc.Find(w, "web:LINK", "Example"); //if not found, throws exception

		////a = Acc.Find(w, "web:LINK") + "next";
		////a = -Acc.Find(w, "web:UUU") + "next";
		////a = (-Acc.Find(w, "web:UUU")).Navigate("next");
		////a = -(-Acc.Find(w, "web:LINK")).Navigate("next100");
		////a = Acc.Find(w, "web:LINK")["next"];
		////a = -Acc.Find(w, "web:LINK")["ne100"];
		////a = -(-Acc.Find(w, "web:UUU"))["ne100"];
		////a = Acc.Find(w, "web:UUU").OrThrow()["ne100"];
		////a = Acc.Find(w, "web:LINK").OrThrow()["ne"].OrThrow();
		////a = Acc.Find(w, "web:LINK")?["ne"];
		//////a = (Acc.Find(w, "web:LINK-")?["ne"]).OrThrow();
		////a = (Acc.Find(w, "web:LINK")?["ne100"]).OrThrow();
		//////a = Acc.Find(w, "web:UUU")["ne100"];
		////a = -Acc.Find(w, "web:LINK")?["ne100"];
		////var a = Acc.Find(w, "web:LINK-").SimpleElementId.OrThrow();
		//Acc.Find(w, "web:LINK", "Portal-").MouseMove();

		////a = Acc.Find(w, "web:UUU").OrThrow();
		////Print(a);

		//var w1 = Wnd.Find("Example").OrThrow();
		//var w2 = Wnd.Find("Example").OrThrow(); //the same

		//var w3 = Wnd.Find("Example").OrThrow().Child("Example").OrThrow();
		//var w4 = Wnd.Find("Example").Child("Example").OrThrow();

		//var w5 = Wnd.Find("Example").Child("Example");

		//var w = Wnd.Find("Example").OrThrow();

		//var a1 = Acc.Find(w, "web:LINK", "Example").OrThrow();
		//var a2 = Acc.Find(w, "web:LINK", "Example").OrThrow(); //the same

		//var a3 = (Acc.Find(w, "web:LINK", "Example")?.Navigate("example")).OrThrow();

		//var a5 = (Acc.Wait(3, w, "Example")?.Navigate("example")).OrThrow();

		//var w = Wnd.Find("Example").OrThrow();
		//var r1 = WinImage.Find(w, "example").OrThrow();
		//var r2 = WinImage.Find(w, "example").OrThrow(); //the same
	}

	static void TestAccEdgeNoUIA()
	{
		for(int i = 0; i < 5; i++) {

			Perf.First();
#if true
			var w = Wnd.Find("* Edge", "Applic*").OrThrow();
			//w = w.Child(null, "Windows.UI.Core.CoreWindow").OrThrow(); //same speed etc
			var a = Acc.Find(w, "LINK", "Bug*", flags: AFFlags.UIA).OrThrow();
#else //30% faster
			var w = Wnd.Find("Microsoft Edge", "Windows.UI.Core.CoreWindow", flags: WFFlags.HiddenToo).OrThrow();
			//w = w.Child("CoreInput", "Windows.UI.Core.CoreComponentInputSource").OrThrow(); //same speed etc
			var a = Acc.Find(w, "LINK", "Bug*", flags: AFFlags.HiddenToo).OrThrow();
#endif
			Perf.NW();
			Print(a);
			Print(a.MiscFlags);
			//Print(a.WndContainer);
		}
	}

	static unsafe void TestStrtoiOverflow()
	{
		//if(!ColorInt.FromString("0xFF", out var c)) return;
		//if(!ColorInt.FromString("#FF", out var c)) return;
		//Print(c);

		//Print("0x1A".ToInt_());
		//Print("1A".ToInt_());
		//Print("0x1A".ToInt_(0, true));
		//Print("1A".ToInt_(0, true));
		//Print("0x1A".ToInt_(0, out _, true));
		//Print("1A".ToInt_(0, out _, true));

		string s = "0x10";
		s = "0xffffffff";
		s = "0x80000000";
		s = "-0xffffffff";
		//s = "0xffffffffffffffff";
		//Print(s.ToInt_());
		//Print(s.ToLong_());
		//Print("-----");
		//Print(Api.strtoi(s));
		//Print(Api.strtoi64(s));
		//Print(Api.strtoui64(s));

		//Print(0xffffffffU);
		//Print(-0xffffffffU);

		fixed (char* p = s) {
			Print((uint)Api.strtoi(p));
		}
	}

	//	static void TestCompiler()
	//	{
	//		var source =
	//			@"
	//using System;
	//using Catkeys;
	//using Catkeys.Types;
	//using static Catkeys.NoClass;

	//public class Test
	//{
	//public static void Main()
	//{
	//Print(1);
	//}
	//}
	//";



	//		var sRef = new string[] { typeof(object).Assembly.Location, Folders.ThisApp + "Au.dll" };
	//		//var sRef = new string[] { typeof(object).Assembly.Location };

	//		var references = new List<PortableExecutableReference>();
	//		foreach(var s in sRef) {
	//			references.Add(MetadataReference.CreateFromFile(s));
	//		}

	//		var tree = CSharpSyntaxTree.ParseText(source);

	//		var options = new CSharpCompilationOptions(OutputKind.WindowsApplication, allowUnsafe: true);

	//		var compilation = CSharpCompilation.Create("A", new[] { tree }, references, options);

	//		var ms = new MemoryStream();

	//		//Emitting to file is available through an extension method in the Microsoft.CodeAnalysis namespace
	//		var emitResult = compilation.Emit(ms);

	//		//If our compilation failed, we can discover exactly why.
	//		if(!emitResult.Success) {
	//			foreach(var diagnostic in emitResult.Diagnostics) {
	//				Print(diagnostic.ToString());
	//			}
	//			return;
	//		}

	//	}

	static unsafe void TestUnsafe()
	{
		int x = 5, y = 0;
		Unsafe.Copy(ref y, &x);
		Print(y);
	}

	static void TestAccFirefoxNoSuchInterface()
	{
		//var w = Wnd.Find("Quick Macros Forum - Mozilla Firefox", "MozillaWindowClass").OrThrow();
		////var w = Wnd.Find("* Chrome").OrThrow();
		//AFFlags f = 0;
		////f |= AFFlags.NotInProc;

		////for(int i = 0; i < 10; i++) {
		////	//var aa=Acc.Find(w, "web:", null, null, f);
		////	//var aa=Acc.Wait(0.1, w, "web:", null, null, f);
		////	var aa=Acc.Find(w, "LINK", null, null, f, skip: 1);
		////	if(aa == null) Print("no"); else Print(aa);
		////}
		////return;

		//////Acc.PrintAll(w, flags: f);
		////Acc.PrintAll(w, "web:", flags: f);
		////return;

		//var a = Acc.Find(w, "web:LINK", "Bug Reports", null, f).OrThrow();
		//Print(a.MiscFlags);
		//Print(a);
		////Print(a.Role);
		////Print(a.Name);
		////Print(a.State);
		////Print(a.Rect);
		////Print(a.WndContainer);

		//var w = Wnd.Find("Settings", "ApplicationFrameWindow").OrThrow();
		//var a = Acc.Find(w, "LISTITEM", "Devices", "class=Windows.UI.Core.CoreWindow").OrThrow();

		var w = Wnd.Find("Microsoft Edge", "Windows.UI.Core.CoreWindow", flags: WFFlags.HiddenToo).OrThrow();
		Acc.PrintAll(w, flags: AFFlags.UIA);
		var a = Acc.Find(w, "TEXT", "this exact word or phrase:", flags: AFFlags.UIA).OrThrow();
		Print(a);
		Print(a.MiscFlags);
	}

	static void TestRunConsole()
	{
		//Shell.Run("notepad.exe");
		//Shell.Run("http://www.quickmacros.com");
		//Shell.Run(@"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
		//Shell.Run(@"%Folders.System%\notepad.exe");

		//return;

		Directory.SetCurrentDirectory(@"C:\");
		string cl = null;
		var enc = Encoding.UTF8;
		//Print(Directory.GetCurrentDirectory());
		//var con = @"Q:\app\Au\Test Projects\Simple\bin\Debug\Simple.exe";
		//var con = @"..\Test Projects\Simple\bin\Debug\Simple.exe";
		//var con = @"%Folders.ThisApp%\..\Test Projects\Simple\bin\Debug\Simple.exe";
		//var con = @"Simple.exe";
		//var con = @"Test\Simple.exe";
		var con = @".\Test\Simple.exe";
		//var con = @"ping.exe"; cl = "/?";
		//var con = Folders.System + @"ping.exe"; cl = "/?";
		//con = @"\\?\" + con;
		//var con = @"http://www.quickmacros.com";

		int ec = Shell.RunConsole(out var s, con, cl, null, enc);
		Print(s);
	}

	static void TestMainWindows2()
	{
		//var a =Wnd.GetWnd.MainWindows(likeAltTab:true);
		//Print("---");
		//Print(a);

		//Print(Wnd.Active);
		//Wnd.SwitchActiveWindow();
		//Print(Wnd.Active);

		//Print(Api.GetDesktopWindow());
		//Print("----");
		//var a =Wnd.GetWnd.AllWindows();

		////Perf.SpinCPU(100);
		////for(int i1 = 0; i1 < 5; i1++) {
		////	int n2 = 1000;
		////	Perf.First();
		////	for(int i2 = 0; i2 < n2; i2++) { foreach(var w in a) { var ro = w.Get.RootOwnerOrThis(); } }
		////	Perf.Next();
		////	for(int i2 = 0; i2 < n2; i2++) { foreach(var w in a) { var ro = w.Owner; } }
		////	Perf.Next();
		////	//for(int i2 = 0; i2 < n2; i2++) { foreach(var w in a) { var ro = w.Get.RootOwnerOrThis2(); } }
		////	//Perf.Next();
		////	for(int i2 = 0; i2 < n2; i2++) { }
		////	Perf.NW();
		////}

		//foreach(var w in a) {
		//	var ro = w.Get.RootOwnerOrThis();
		//	//if(ro== Api.GetDesktopWindow()) {
		//	//if(ro!= w) {
		//	//	Print("----");
		//	//	Print(w);
		//	//	Print(ro);
		//	//	Print(w.Owner);
		//	//	Print(w.Get.DirectParentOrOwner );
		//	//}
		//	//Print();
		//	var ro2 = w.Get.RootOwnerOrThis(true);
		//	if(ro2 != ro) {
		//		Print("----");
		//		Print(w);
		//		Print(ro);
		//		Print(ro2);
		//	}
		//}

		//var w = Wnd.Find("Options");
		//Print(w);
		//Print(w.Get.LastActiveOwnedOrThis());
		//Print(w.Get.LastActiveOwnedOrThis(true));

		//Print(Wnd.GetWnd.MainWindows());
		////Print(Wnd.GetWnd.NextMain());
		//var w = Wnd.Find("* Chrome").OrThrow();
		//Print(Wnd.GetWnd.NextMain(w, retryFromTop: true));
		Print(Wnd.SwitchActiveWindow());
		Print(Wnd.Active);
	}

	static void TestAccProcessDoesNotExit()
	{
		//Print(Api.GetCurrentThreadId());

		//var a = Acc.FromXY(20, 20);
		//var a = Acc.FromXY(20, 20, flags: AXYFlags.NotInProc);

		Wnd w = Wnd.Find("*Notepad");
		//return;

		AFFlags f = 0;
		////f |= AFFlags.NotInProc;
		var a = Acc.Find(w, "TEXT", flags: f);
		//var a = Acc.Find(w, "NOTFOUND");
		if(a == null) return;

		Print(a);
		a.Dispose();

		//Perf.SpinCPU(100);
		//for(int i = 0; i < 10; i++) {
		//	var a = Acc.Find(w, "TEXT");
		//	Print(a);
		//	200.ms();
		//}
	}

	static void TestAccProcessDoesNotExit3()
	{
		//Wnd w = Wnd.Find("*Notepad");
		//var x=new

		var t = new Thread(() => { Print(Api.GetCurrentThreadId()); TestAccProcessDoesNotExit(); });
		t.SetApartmentState(ApartmentState.STA);
		t.Start();
		MessageBox.Show("");
		Task.Run(() => { Print(Api.GetCurrentThreadId()); TestAccProcessDoesNotExit(); });
		MessageBox.Show("");
		var tt = new Thread(() =>
		{
			var ad = AppDomain.CreateDomain("qwerty");
			ad.DoCallBack(() => { Output.LibUseQM2 = true; Print(Api.GetCurrentThreadId()); TestAccProcessDoesNotExit(); });
			AppDomain.Unload(ad);
		});
		tt.SetApartmentState(ApartmentState.STA);
		tt.Start();
		MessageBox.Show("");
	}

	//static void TestIpcWithWmCopydataAndAnonymousPipe()
	//{
	//	Cpp.Cpp_Test();
	//}

	static void TestWndFindProgramEtc()
	{
		//var w = Wnd.Find("*pad", null, "program=notepa?\0 pid=10708\0 tid=0x1A50\0 owner=2622450");
		//var w = Wnd.Find("*pad", programEtc: "program=notepa?");
		//var w = Wnd.Find(programEtc: "program=notepa?");
		//var w = Wnd.Find("*pad", programEtc: $"pid={10708}");
		var w1 = Wnd.Find("*- Notepad");
		var w = Wnd.Find("*pad", programEtc: $"owner={w1.Handle}");
		Print(w);
	}

	static void TestTaskDialogOwnerWpf()
	{
		var w = new System.Windows.Window() { Title = "Test" };
		var k0 = (Wnd)w;
		Print(k0);
		w.MouseLeftButtonUp += (unu, sed) =>
		{
			//AuDialog.Show("dialog", owner: w);
			//System.Windows.MessageBox.Show(w, "message");
			var k = (Wnd)w;
			Print(k);

			//var m = new AuMenu();
			//m["test"] = o => Print(o);
			//m.Show();
		};
		w.Show();
		var app = new System.Windows.Application();
		app.Run();

		//AuDialog.Show(owner: Wnd.Find("Quick*"));
	}

#if TOOLS
	static void TestAccForm()
	{
		Acc a = null;
#if true
		//Cpp.Cpp_Test(); return;

		var w = Wnd.Find(null, "QM_Editor").OrThrow();
		a = Acc.Find(w, "BUTTON", "Paste*").OrThrow();

		//var w = Wnd.Find(null, "IEFrame").OrThrow();
		//a = Acc.Find(w, "web:LINK", "QM and*").OrThrow();

		//var w = Wnd.Find(null, "Shell_TrayWnd").OrThrow();
		//a = Acc.Find(w, "BUTTON").OrThrow();

		//var w = Wnd.Find("* Chrome").OrThrow();
		//a = Acc.Find(w, "web:BUTTON", "Search").OrThrow();
		//a = Acc.Find(w, "web:BUTTON", "Search", "class=moo").OrThrow();


		//Print(a);
		//return;
		//Print(a.MiscFlags);

		//Print(w);
		//Print(a);

		//Print(Ver.Is64BitProcess);
		//Print(w.Is64Bit);
		//return;

		//a = Acc.Find(w, "web:CELL", "Posts").OrThrow();

		//var w = Wnd.Find("* Firefox").OrThrow();
		//a = Acc.Find(w, "web:TEXT", "Search the Web").OrThrow();

		//var w = Wnd.Find("Quick *").OrThrow();
		//a = Acc.Find(w, "BUTTON", "Properties*").OrThrow();

		//foreach(var k in Wnd.GetWnd.AllWindows(false)) {
		//	//if(!k.ClassNameIs("Windows.UI.Core.CoreWindow")) continue;
		//	//if(!k.IsVisible || k.IsVisibleEx) continue;
		//	if(!k.IsCloaked) continue;
		//	var s = k.Name; if(Empty(s) || s=="Default IME" || s== "MSCTFIME UI") continue;
		//	Print(k);
		//	//Print(k.IsVisible, k.IsCloaked);
		//	//continue;
		//	foreach(var c in k.Get.Children()) {
		//		Print($"\t{c.ToString()}");
		//	}
		//}
		//return;

		//var w = Wnd.Find("Microsoft Edge", "Windows.UI.Core.CoreWindow", flags: WFFlags.HiddenToo).OrThrow();
		//a = Acc.Find(w, "PANE", flags: AFFlags.HiddenToo).OrThrow();

		//a = a.Navigate("pr");
#endif

		//Task.Run(() => { for(int i = 0; i < 10; i++) { 2.s(); GC.Collect(); } });

		Perf.First();
		var f = new Au.Tools.Form_Acc(a);
		f.ShowDialog();
		//Application.Run(f);
		f.Close();
		f.Dispose();
		//AuDialog.Show("-");
	}
#endif

	//static void TestScreenCaptureSpeedWithCaptureblt()
	//{
	//	Perf.SpinCPU(1000);
	//	var file = Folders.Temp + "test.png";
	//	var r=Screen_.Rect;
	//	Print(r);
	//	for(int i = 0, n=16; i < n; i++) {
	//		10.ms();
	//		Perf.SpinCPU(100);
	//		Perf.First();
	//		using(var b = WinImage.Capture((i&1)!=0, r)) {
	//			Perf.NW();
	//			if(i == n-1) {
	//				b.Save(file);
	//				Shell.Run(file);
	//			}
	//		}
	//	}

	//}

	static void TestWndForm()
	{

	}

	static void TestWndImage()
	{
		//var w = Wnd.Find("Icons").OrThrow();
		//var r = WinImage.Find(w, @"Q:\My QM\copy.bmp", WIFlags.WindowDC).OrThrow();
		//r.MouseMove();

		//var w = Wnd.Find("Icons").OrThrow();
		//var _image = Au.Controls.ImageUtil.ImageToString(@"Q:\My QM\copy.bmp");
		//Print(_image);
		//var r = WinImage.Find(w, _image, WIFlags.WindowDC).OrThrow();
		//r.MouseMove();



		//Perf.First();
		////var im = Image.FromFile(@"Q:\My QM\copy.bmp");
		////var w = Wnd.Find(also: o=> null!=WinImage.Find(o, im, WIFlags.WindowDC)).OrThrow();
		//var w = Wnd.Find(also: o => null != WinImage.Find(o, @"Q:\My QM\copy.bmp", WIFlags.WindowDC)).OrThrow();
		////var w = Wnd.Find(also: o=> { Print(o); return null != WinImage.Find(o, @"Q:\My QM\copy.bmp", WIFlags.WindowDC); }).OrThrow();
		////var w = Wnd.Find(also: o => { Print(o); Perf.First(); var ok = null != WinImage.Find(o, @"Q:\My QM\copy.bmp", WIFlags.WindowDC); Perf.NW(); return ok; }).OrThrow();
		//Perf.NW();
		//Print(w);

		//1.5.s();
		////Task.Run(() => { for(int i = 0; i < 10; i++) { 2.s(); GC.Collect(); } });
		//var path = @"Q:\My QM\copy.bmp"; //memsize 1 KB
		//path = @"C:\Program Files\Android\Android Studio\plugins\android\lib\layoutlib\data\res\drawable-sw720dp-nodpi\default_wallpaper.png"; //memsize 28 MB
		////path = @"C:\Program Files\Android\Android Studio\plugins\android\lib\device-art-resources\nexus_4\land_back.png"; //memsize 6 MB
		////path = @"C:\Program Files\LibreOffice 5\share\gallery\education\Notebook.png"; //memsize 2 MB
		////path = @"C:\Program Files\LibreOffice 5\share\gallery\finance\GoldBar.png"; //memsize 500 KB
		////path = @"C:\Program Files\Android\Android Studio\plugins\android\lib\layoutlib\data\res\drawable-ldpi\jog_tab_right_confirm_red.png"; //memsize 22 KB
		////path = @"C:\Program Files\Android\Android Studio\plugins\android\lib\layoutlib\data\res\drawable-mdpi\picture_emergency.png"; //memsize 36 KB
		////path = @"C:\Program Files\Android\Android Studio\plugins\android\lib\device-art-resources\nexus_4\thumb.png"; //memsize 93 KB
		////path = @"C:\Program Files\LibreOffice 5\share\gallery\computers\Database-Download.png"; //memsize 299 KB
		////path = @"C:\Program Files\Android\Android Studio\plugins\android\lib\layoutlib\data\res\drawable-hdpi\ic_menu_recent_history.png"; //memsize 9 KB
		//for(int i = 0; i < 150; i++) {
		//	Debug_.LibPrintMemory();
		//	var k = new WinImage._Finder._Image(path);
		//	//k.Dispose();
		//	//var k = Image.FromFile(path);
		//	100.ms();
		//}
	}

	static void TestWndFindContains()
	{
		Perf.First();
		var w = Wnd.Find(contains: "Do you want to save*").OrThrow();
		//var w = Wnd.Find(contains: "Untitled - Notepad").OrThrow();
		//var w = Wnd.Find(contains: "Personalization").OrThrow();
		//var w = Wnd.Find(contains: new Acc.Finder("STATICTEXT", "Do you want to save*")).OrThrow();
		//var w = Wnd.Find(contains: new Wnd.ChildFinder("Save", "Button")).OrThrow();
		//var w = Wnd.Find(contains: WinImage.LoadImage(@"Q:\My QM\copy.bmp")).OrThrow();
		Perf.NW();
		Print(w);

		//var w = Wnd.Find("* Notepad").OrThrow();
		//Acc.PrintAll(w);
		////Acc.PrintAll(w, flags: AFFlags.ClientArea);
		//////Acc.PrintAll(w, flags: AFFlags.ClientArea | AFFlags.UIA);
	}

	static void TestAccFindWithChildFinder()
	{
		var w = Wnd.Find("* Internet Explorer").OrThrow();
		for(int i = 0; i < 5; i++) {
			100.ms();
			Perf.First();
			//var a = Acc.Find(w, "web:LINK", "Videos").OrThrow();
			//var a = Acc.Find(w, "LINK", "Videos").OrThrow();
			//var a = Acc.Find(w, "LINK", "Videos", "class=*Server").OrThrow();
			var a = Acc.Find(w, "LINK", "Videos", controls: new Wnd.ChildFinder(null, "*Server")).OrThrow();
			Perf.NW();
			Print(a);
		}
	}

	static void TestToIntWithFlags()
	{
		Print("15".ToInt_());
		Print("0xA".ToInt_());
		Print("C".ToInt_());
		Print("C".ToInt_(0, STIFlags.IsHexWithout0x));
		Print("0xA".ToInt_(0, STIFlags.NoHex));
	}

	//static void TestNewWildexSyntax()
	//{
	//	string s, w;
	//	s = "two";
	//	//s = "ONE";

	//	w = "two";
	//	w = "**m one||two";
	//	//w = "**m(^^^) one^^^two";
	//	//w = "**m(^^ one^^two";
	//	//w = "**m() one^^two";
	//	//w = "**m(^^)one^^two";

	//	//var x = new Wildex(w);
	//	//Print(x.Match(s));

	//	Cpp.Cpp_TestWildex(s, w);
	//}

	static void TestPcreSpeed()
	{
		//Print(Ver.Is64BitProcess);

		string s = "test name-m7354().gge";
		s = "var w1 = Wnd.Find('jdjdjsjhahdjhsdjahjdhj', 'sijdiairuiru')";

		//var rx = new string[1000];
		//for(int i = 0; i < 1000; i++) rx[i] = _rxTest + i.ToString();

		int k1 = 0, k2 = 0, k3 = 0, k4 = 0, k5 = 0, k6 = 0, k7 = 0, k8 = 0, k9 = 0;
		Perf.Cpu(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			Perf.First();
			//for(int i2 = 0; i2 < n2; i2++) { if(_RxNet1(s, rx[i2])) k1++; }
			for(int i2 = 0; i2 < n2; i2++) { if(_RxNet1(s, _rxTest)) k1++; }
			Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { if(_RxNet2(s, rx[i2])) k2++; }
			for(int i2 = 0; i2 < n2; i2++) { if(_RxNet2(s, _rxTest)) k2++; }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { if(_RxNet3(s)) k3++; }
			Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { if(_RxPcre1(s, rx[i2])) k4++; }
			for(int i2 = 0; i2 < n2; i2++) { if(_RxPcre1(s, _rxTest)) k4++; }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { if(_RxPcre2(s, _rxTest)) k5++; }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { if(_RxPcre3(s)) k6++; }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { if(_RxPcre4(s)) k7++; }
			Perf.NW();
		}
		Print(k1, k2, k3, k4, k5, k6, k7);
	}

	public static bool _RxNet1(string s, string rx)
	{
		var r1 = new Regex(rx, RegexOptions.CultureInvariant);
		return r1.Match(s).Success;
	}

	public static bool _RxNet2(string s, string rx)
	{
		//return false;
		return Regex.IsMatch(s, rx, RegexOptions.CultureInvariant);
	}

	public static bool _RxPcre1(string s, string rx)
	{
		var r1 = new Regex_(rx, c_rxFlags);
		//var r1 = new Regex_(rx, 0);
		return r1.IsMatch(s);
	}

	public static bool _RxPcre2(string s, string rx)
	{
		return s.RegexIsMatch_(rx, c_rxFlags);
	}

	public static bool _RxPcre3(string s)
	{
		return _rxTestCompiledPcre.IsMatch(s);
	}

	public static bool _RxPcre4(string s)
	{
		return _rxTestCompiledPcre.Match(s, out RXMatch m);
	}

	public static bool _RxNet3(string s)
	{
		return _rxTestCompiledNet.Match(s).Success;
	}

	//const string _rxTest = @"\.$|[\\/|<>?*:""\x00-\x1f]";
	//const string _rxTest = @"(?i)^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)";
	//const string _rxTest = @"(?i)^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)";
	//const string _rxTest = @".";
	//const string _rxTest = @"^(?:Wnd|var) +(\w+) *=";
	//const string _rxTest = @"(?i)^(?:Wnd|var) +(\w+) *=";
	const string _rxTest = @"^var";
	//const string _rxTest = @"^var(.)";
	static Regex_ _rxTestCompiledPcre = new Regex_(_rxTest, c_rxFlags);
	static Regex _rxTestCompiledNet = new Regex(_rxTest, RegexOptions.CultureInvariant);

	const RXFlags c_rxFlags = 0;
	//const RXFlags c_rxFlags = RXFlags.UTF; //slower by 20-50 %, regardless of (?i)
	//const RXFlags c_rxFlags = RXFlags.UTF | RXFlags.NO_UTF_CHECK; //same speed

	static void TestPcreRegexStatic()
	{
		string s;
		s = "-10";
		//Print(Regex_.Match(s, @"^-(\w+)"));
		//Print(Regex_.Match(s, @"^-(\w+)"));
		//Print(Regex_.Match(s, @"(\d\d)"));
		//Print(Regex_.Match(s, @"(\d\d)", from: 1, to: -1));
		//Print(Regex_.Match(s, @"(\d\d)", matchFlags: RXMatchFlags.ANCHORED, from: 0, to: -1));
		//Print(Regex_.Match(s, @"^(\d\d)", from: 1, to: -1));
		//Print(Regex_.Match(s, @"\G(\d\d)", from: 1, to: -1));
		//Print(Regex_.Match(s, @"\A(\d\d)", from: 1, to: -1));

		s = "aĄ";
		Print(s.RegexIsMatch_(@"(?i)aą"));
		Print(s.RegexIsMatch_(@"(?i)aą", RXFlags.NEVER_UTF));
		Print(s.RegexIsMatch_(@"(?i)aĄ", RXFlags.NEVER_UTF));
	}

	static void TestRegexCulture()
	{
		//var x = new Regex("(?i)ĄąΣσ");
		//Print(x.IsMatch("ąĄσΣ"));

		RXFlags f = 0;
		//f |= RXFlags.UTF;
		//f |= RXFlags.NEVER_UTF;
		string s, r;

		s = "ąĄσΣ";
		r = "(?i)ĄąΣσ";
		//r ="(*UTF)(?i)ĄąΣσ";

		//s ="ąĄ";
		//r ="(?i)Ąą";

		var x = new Regex_(r, f);
		Print(x.IsMatch(s));
	}

	static int s_sep;

	[MethodImpl(MethodImplOptions.NoInlining)]
	//static RXMatch.Group TestRegex_()
	static void TestRegex_()
	{
		string rx, s; RXFlags f = 0; RXMatchFlags f2 = 0;
		rx = @"\d(\w)";
		//rx = @"\d\K(\w)";
		//rx = @"\d(?=\w)";
		//rx = @"\d(?=\w\K)";
		//rx = @"\d(&)?(?=\w\K)";
		//rx = @"\d(-)?\w";
		//rx = @"\d(-)?\w(o)";
		//rx = @"\G\d(\w)";
		s = "1wo 2k";

		//rx = @"\d\d(\d\d)-\d\d-\d\d";
		//rx = @"\d\d(\d\d)(*MARK:moo)-\d\d-\d\d";
		//s = "2018-";
		////s = "2018-11-11";
		////s = "2018-11-kk"; //use long enough, else ignores MARK
		////f2 |= RXMatchFlags.PARTIAL_SOFT;
		////f |= RXFlags.NO_START_OPTIMIZE;

		////rx = "a(b)?(c?)";
		////s = "a";

		//rx = @"(\w+) (\w+) (\w+)";
		//s = "one two three";

		//rx = "(?<=')(.*?)(?=')";
		//s = "= 'testname'k";

		var x = new Regex_(rx);
		//Print(x.Match(s, out var m, f2));
		//Print(x.Match(s, out var m, f2, new RXLimits(4)));
		//Print(x.Match(s, out var m, f2, new RXLimits(4, 6)));

		//Print(m[0].DebugToString(), m.IndexNoK);

		//Print(x.IsMatch(s));
		//Print(x.IsMatch(s, fromTo: new RXFromTo(2)));

		//Print(x.MatchG(s, out var g));
		//Print(x.MatchG(s, out var g, fromTo: new RXFromTo(2)));
		//Print(g.DebugToString());

		//Print(x.MatchS(s, out var ss));
		//Print(x.MatchS(s, out var ss, fromTo: new RXFromTo(2)));
		//Print(x.MatchS(s, out var ss, 1, fromTo: new RXFromTo(2)));
		//Print(ss);

		////s_sep = 1;
		////int i = m[0].Index;
		//////s_sep = 2;
		////i += m[0].Length;
		////s_sep = 3;
		////var v = m[0].Value;
		////s_sep = 4;
		////var h = i.ToString();

		////s_sep = 1;
		////var g = m[0];
		////s_sep = 2;
		////int i = g.Index;
		////s_sep = 3;
		////i += g.Length;
		////s_sep = 4;
		////var v = g.Value;
		////s_sep = 5;
		////var h = i.ToString();

		//if(m == null) Print("null");
		//else if(!m.Exists) Print(m.Mark);
		//else if(m.GroupCount == 1) Print(m[0].DebugToString(), m.Exists, m.IsPartial, m.IndexNoK, m.Mark);
		//else Print(m[0].DebugToString(), m[1].DebugToString(), m.Exists, m.IsPartial, m.IndexNoK, m.Mark);

		//Print(m.GroupIL(0));
		//Print(m.GroupIV(0));

		//Print(m[0].Length);
		//Print(m[0].Value);
		//Print(m[0].Length);
		//Print(m[0].Value);

		//Print($"{m[0].Value}, {m[1].Value}, {m[0].Value}, {m[1].Value}");
		//Print($"{m[0]}, {m[1]}, {m[0]}, {m[1]}");
		//Print($"{m[0].ToString()}, {m[1].ToString()}, {m[0].ToString()}, {m[1].ToString()}");

		//return m[0];

		//var g = m[0];
		//Print(g.Length);
		//Print(g.Value);
		//Print(g.Length);
		//Print(g.Value);

		//var g1 = m[0];
		//Print(g1.Length);
		//Print(g1.Value);
		//var g2 = m[0];
		//Print(g2.Length);
		//Print(g2.Value);

		//ref var g1 = ref m[0];
		//Print(g1.Length);
		//Print(g1.Value);
		//ref var g2 = ref m[0];
		//Print(g2.Length);
		//Print(g2.Value);

		//return g;
	}

	static void TestRegexGroupNumberFromName()
	{
		var r = "(one)|(two)";
		r = "(?<AM>one)|(?<AM>two)";
		r = "((?<AM>mo)|(?<AM>ko))?--$";
		var s = "--one--";
		s = "--two--";

		var x = new Regex_(r, RXFlags.DUPNAMES);
		if(!x.Match(s, out var m)) { Print("no match"); return; }
		int i = m.GroupNumberFromName("AM", out var notUnique);
		Print(i, notUnique);
	}

	static void TestRegexCallout()
	{
		var r = @"--(?C1)(?:(\w+) (?C2))+--";
		//r = "(*MARK:M1)(?C'cow')on(*MARK:M2)e(?C'dog')";
		//r = @"(?C1)\w+(?C2)";
		//var s = "--one--";
		var s = "--one MONE do --";
		//r = @"--(?C1)(\w+(?C2) )+--";

		var x = new Regex_(r);
		//var x = new Regex_(r, RXFlags.NO_AUTO_POSSESS|RXFlags.NO_DOTSTAR_ANCHOR| RXFlags.NO_START_OPTIMIZE);
#if true
		//x.SetCallout((in RXCalloutData d) =>
		x.Callout = d =>
		{
			//Print(d.current_position, d.callout_number);
			//Print(d.mark, d.callout_string);
			Print($"last={d.capture_last}, top={d.capture_top}, pos={d.current_position}, start={d.start_match}, pat_pos={d.pattern_position}, next_len={d.next_item_length}");
			int i = d.capture_last;
			if(i > 0) Print(d.LastGroup, d.LastGroupValue);

			d.Result = -1;

			//Print(i);
			//if(i > 0) {

			//	var (offs, len) = d.Group(i);
			//	var k = d.GroupValue(i);
			//	if(offs > 1000 || len > 1000) Print(k);
			//}
		};
#endif
		if(!x.Match(s, out var m)) { Print("no match"); return; }

		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { x.Match(s, out m); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}

		Print(m);
	}

	static void TestRegexGetRepeatedGroupInstances()
	{
		var s = "BEGIN 111 2222 333 END";
		var x = new Regex_(@"^(\w+) (?:(\d+) (?C1))+(\w+)$");
		var a = new List<string>();
		x.Callout = o => a.Add(o.LastGroupValue);
		if(!x.Match(s, out var m)) { Print("no match"); return; }
		Print(m[1]);
		Print(a); //all numbers. m[2] contains only the last number.
		Print(m[3]);
	}

	public static string DebugToString(this RXGroup t) => $"{t.Index.ToString()} {t.Length.ToString()} '{t.Value}'";

	static void TestRegexMatch()
	{
		var s = "--333--";
		var x = new Regex_(@"\d+(-)");

		//Print(x.IsMatch(s));

		//if(!x.Match(s, out var m)) { Print("no match"); return; }
		//Print(m);

		//if(!x.MatchS(s, out var m)) { Print("no match"); return; }
		//Print(m);

		if(!x.MatchG(s, out var m)) { Print("no match"); return; }
		Print(m.DebugToString());

		//s = "--aaa--333--";
		//x = new Regex_(@"(?<n1>\w+)--(?'n2'\d+)");

		//if(!x.MatchS(s, out var m, x.GroupNumberFromName("n2"))) { Print("no match"); return; }
		//Print(m);
	}

	static void TestRegexFindAll()
	{
		var s = "BEGIN 111 2222 333 END";
		var x = new Regex_(@"\w(\w+)");
		//var x = new Regex_(@"\w\w(?=\w\w\K)");
		//var x = new Regex_(@"\b");
		//if(!x.FindAll(s, out var a)) { Print("no match"); return; }
		//if(!x.FindAllG(s, out var a)) { Print("no match"); return; }
		//if(!x.FindAllS(s, out string[] a)) { Print("no match"); return; }
		//Print(a);
		//Print(x.FindAll(s));
		//Print(x.FindAllG(s));
		//Print(x.FindAllS(s));
		//Print(x.FindAllS(s, 1));
		//Print(x.FindAllS(s, limits: new RXLimits(10)));
		//Print(x.FindAllS(s, limits: new RXLimits(0, 10)));
		//Print(x.FindAllS(s, limits: new RXLimits(maxCount: 2)));
		foreach(var g in x.FindAllG(s)) Print(g.DebugToString());

		//var x = new Regex(@"\w+ \w+");
		//var a = x.Matches(s);
		//Print(a.Count);
	}

	static void TestRegexReplace()
	{
		string s, rx, re;
		s = "BEGIN 111 2222 333 END";
		//s = "𑀠𑀡𑀢𑀣";
#if true
		//rx =@"\w(\w+)";
		//rx =@"\b";
		//rx=@"\w\K\w+";
		//rx=@"\w+\K";
		//rx=@"(?s)(?=.)";
		//rx = @"\d+(*:number)|\w+(*:text)"; re = "-$*-";
		rx = @"(\w+) (\w+)"; re = "$2-$1"; //re = "$22-$11";
		rx = @"(?<one>\w+) (?<two>\w+)"; re = "${two}-${one}"; //re = "${2}-${1}"; //re = "${k$1}"; //re = "$4000000000-"; //re = "${4000000000}";
															   //rx = @"(?<5>\w+) (?<2>\w+)"; re = "${2}-${5}"; //PCRE does not allow group names starting wih digit
															   //rx = @"";
															   //rx = @"";
															   //rx = @"(*UTF)."; re = "'$0";
															   //rx = @"[𑀠𑀡𑀢𑀣]"; re = "'$0";
															   //rx = @"(*UTF)\b"; re = "'";
															   //rx = @"(*UTF)(?=.)"; re = "'";
		var x = new Regex_(rx);

		//re = "moo";
		//re = "'";
		//re = "($0)";
		//re = "($&)";
		//re = "($`)";
		//re = "($')";
		//re = "($+)";
		//re = "($_)";
		//re = "($?)";
		//re = "-$";
		//re = "-$$-";
		//re = "-$$";
		//re = "$$-";
		//re = "";
		//re = "";

		//Print(x.IsMatch(s));
		Print(x.Replace(s, re));
		//Print(x.ReplaceAll(s, re, 0, new RXLimits(10)));
		//Print(x.ReplaceAll(s, re, 0, new RXLimits(10, 20)));
		//Print(x.ReplaceOne(s, re));
		//Print(x.ReplaceAll(s, m=>"moo"));
		//Print(x.ReplaceAll(s, m=>m.ExpandReplacement("moo")));
		//Print(x.ReplaceAll(s, m=>m.ExpandReplacement("$2 $1")));
#else
		//var x = new Regex(@"\w(?<5>\w+)");
		//var x = new Regex(@"\w(\w+)(-)?");
		//var x = new Regex(@"\b");
		var x = new Regex(@".");
		//var x = new Regex(@"(?s)(?=.)");

		//Print(x.Replace(s, "'"));
		//Print(x.Replace(s, m => m.Result("$1")));
		//Print(x.Replace(s, "${5}"));
		Print(x.Replace(s, "'$0"));

		//var x = new Regex(@"\w+ \w+");
		//var a = x.Matches(s);
		//Print(a.Count);
#endif

	}

	static void TestRegexFindAllE()
	{
		string s, rx;
		s = "BEGIN 111 2222 333 END";
		rx = @"\w+";
		var x = new Regex_(rx);
		//foreach(var m in x.FindAllE(s)) {
		//	Print(m);
		//}

		foreach(var m in x.FindAll(s)) Print(m);
		foreach(var m in x.FindAllG(s)) Print(m);
		foreach(var m in x.FindAllS(s)) Print(m);

		if(x.FindAll(s, out var am1)) Print(am1);
		if(x.FindAllG(s, out var ag1)) Print(ag1);
		if(x.FindAllS(s, out var as1)) Print(as1);

		int k1 = 0, k2 = 0, k3 = 0, k4 = 0;
		Perf.Cpu(100);
		Debug_.LibMemoryPrint();
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 100;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { foreach(var m in x.FindAllS(s)) k1++; }
			//for(int i2 = 0; i2 < n2; i2++) { if(x.FindAllS(s, out var a2)) k2++; }
			Perf.NW();
			Debug_.LibMemoryPrint();
		}
		Print(k1, k2, k3, k4);
	}

	static void TestRegexSplit()
	{
		string s, rx;
		s = "BEGIN 111 2222 333 END";
		rx = @"\w+";
		rx = @" ";
		var x = new Regex_(rx);
		//Print(x.Replace(s, "-", -1));
		//Print(x.Split(s, 0));
		foreach(var g in x.SplitG(s, 0)) Print(g.DebugToString());

		//Print(s.Split(new char[] { ' ' }, 0));
	}

	static void TestRegexCalloutWithFindAll()
	{
		string s, rx;
		s = "BEGIN 111 2222 333 and KKK 4 55 6 END";
		rx = @"\w+ (?:(\d+) (?C1))+";
		//s = "ABC111 DEF222 GHK333";
		//rx = @"\w\K\w\w(?C1)\d+";
		RXFlags f = 0;
		//f |=RXFlags.AUTO_CALLOUT;
		var x = new Regex_(rx, f);
		x.Callout = o =>
		{
			Print(o.start_match, o.GroupValue(1));
			//Print(o.start_match, o.capture_top);
		};

		foreach(var m in x.FindAll(s)) Print(m);

	}

	static void TestRegexAndGC()
	{
		Task.Run(() => { for(; ; ) { 100.ms(); GC.Collect(); } });

		string s, rx;
		s = "BEGIN 111 2222 END";
		rx = @"\w+ (?:(\d+) (?C))+";
		var x = new Regex_(rx);
		x.Callout = o =>
		{
			MessageBox.Show(o.GroupValue(1));
		};
		x.IsMatch(s);
		Print("---");
		x.Callout = null;
		Print(x.IsMatch(s));
		Print("end");
		MessageBox.Show("end");

		//string s, r;
		//s = "-- one555 -- two2 --";
		//r = @"\b(three|four|five|one|two)\d+\b";
		//var x = new Regex_(r);
		//var x2 = new Regex(r);
		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 5; i1++) {
		//	int n2 = 1000;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { x2.IsMatch(s); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { x.IsMatch(s); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { }
		//	Perf.NW();
		//}

	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static int TestAccLeaks3(Wnd w)
	{
		var a = Acc.FindAll(w);
		//foreach(var v in a) v.Dispose();
		return a.Length;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestAccLeaks2(Wnd w)
	{
		//return;
		int n = 0;
		Debug_.LibMemoryPrint();
		for(int i = 0; i < 15; i++) {
			for(int j = 0; j < 40; j++) {
				n = TestAccLeaks3(w);
			}
			Debug_.LibMemoryPrint();
			100.ms();
		}
		Print(n);
		//Print($"max {(double)Acc.DebugMaxMemoryPressure/(1024*1024)}, sum {(double)Acc.DebugMemorySum/(1024*1024)}");
	}

	static void TestAccLeaks()
	{
		//var w = Wnd.Find(className: "FM").OrThrow();
		//var w = Wnd.Find("QM Help").OrThrow();
		//var w = Wnd.Find("* Firefox").OrThrow();
		var w = Wnd.Find("Quick M*").OrThrow();
		//var w = Wnd.Find("*Chrome").OrThrow();
		Acc.FindAll(w);
		GC.Collect();
		MessageBox.Show("continue");
		g1:
		Output.Clear();
		TestAccLeaks2(w);
		MessageBox.Show("before GC");
		GC.Collect();
		100.ms();
		GC.Collect();
		100.ms();
		GC.Collect();
		100.ms();
		GC.Collect();
		100.ms();
		GC.Collect();
		if(MessageBox.Show("after GC\nContinue?", "", MessageBoxButtons.YesNo) == DialogResult.Yes) goto g1;
	}

	//public static void Write<T>(T x)
	//{
	//	Print(1);

	//	//switch(x) {
	//	//case IEnumerable<T> t:
	//	//	Print(t);
	//	//	break;
	//	//}
	//}

	public static void Write(string x)
	{
		Print("string");
	}

	public static void Write(object x)
	{
		if(x is IEnumerable e) {
			Print("IEnumerable");
		} else {
			Print("other");
		}
	}

	public static void Write<T>(IEnumerable<T> x)
	{
		Print("IEnumerable<T>");
	}

	public static void Write<T>(T[] x)
	{
		Print("T[]");
	}

	//public static void Write<T>(IEnumerable<T> x, string format = "{v}\r\n")
	//{
	//	Print("IEnumerable<T>");
	//	//Print(x is string);
	//}

	//public static void Write(object first, params object[] other)
	//{
	//	Print("params");
	//	//Print(other != null);
	//}

	public static void Write(params object[] values)
	{
		Print("params");
		//Print(other != null);
	}

	//public static void Write(string x1, object x2, params object[] xn)
	//{
	//	Print("params");
	//	Print(xn != null);
	//}

	static void Func1(in int k)
	{

	}

	static void Func2(ref int k)
	{
		Func1(k);
	}

	static void TestPrintListEx()
	{
		var a = new string[] { "one", null, "thr\t\"ee" };
		var b = new List<string> { "one", null, "thr\t\"ee" };
		//var a = new int[] { 55, 88};
		//var a = new object[] { "te\txt", null, 55, b};
		//a = null;
		//a = new string[0];
		//a = new string[] { "" };
		var nongen = new System.Collections.ArrayList(a);

		//PrintListEx(a);
		//PrintListEx(a, "{0}. '{3}'\r\n");
		//PrintListEx(a, "{0}. {4}\r\n");

		//Print("aaa", "b\r\n\t\"c", 4, 1.4, a);

		//Print("----");

		//Print(a);
		//PrintListEx(a);
		PrintListEx(a, "{1}. {3}\r\n");
		//Print(a, "', '");
		//Print(a, "', '", "'", "'.");
		//Print(a, "'\r\n'", "'", "'");
		//Print(a, indices: true);
		//Print(nongen);
		//Print(new char[] { 'a', 'b' });
		//Print(4, "ff");
		//Print("one", "two");
		//Print("one", true, null, 5, a);

		////var c = "string";
		//////Print(c.AsEnumerable());
		////Print(c as IEnumerable<char>);

		//var d = new Dictionary<string, int>() { { "one", 1 }, { "two", 2 } };
		////Print(d);
		//PrintListEx(d, "d[{0}] = {2}\r\n");
	}

	static void TestNewEscape()
	{
		Perf.First();
		string s = "\t234567890";
		//s = "ab";
		//Print(s.Limit_(10));
		//Print(s.Limit_(9));
		//Print(s.Limit_(4));
		//Print(s.Limit_(3));
		//Print(s.Limit_(2));
		//Print(s.Limit_(1));
		//Print(s.Limit_(0));

		Print(s.Escape_());
		Print(s.Escape_(quote: true));
		Print(s.Escape_(7, false));
		Print(s.Escape_(7, true));

		Print(s, "mm", s);
		//Print(s, "mm");
		//Print(s, "mm");
		//Print(s, "mm");
		PrintListEx(new string[] { s, "kk", s }, "{3}\r\n");

		//Acc.PrintAll(Wnd.Find("Quick*").OrThrow());

		Perf.Next();
		//var w = Wnd.Find(null, "Notepad");
		//w.SetText("one\ttwo");
		//Print(w);

		//Print(new CsvTable("one,two\r\nthree"));
		Perf.NW();
	}

	static void TestLibDC()
	{
		using(var dcs = new LibScreenDC(0)) {
			Print((IntPtr)dcs);
			using(var dcm = new LibCompatibleDC(dcs)) {
				Print((IntPtr)dcm);
			}
		}
	}

	static void TestSpan()
	{
		//Span<char> span;

		//byte* pointerToStack = stackalloc byte[256];
		//Span<byte> stackMemory = new Span<byte>(pointerToStack, 256);

		//IntPtr unmanagedHandle = Marshal.AllocHGlobal(256);
		//Span<byte> unmanaged = new Span<byte>(unmanagedHandle.ToPointer(), 256);
		//Marshal.FreeHGlobal(unmanagedHandle);


		//string input = "123,456";
		//ReadOnlySpan<char> inputSpan = input.AsReadOnlySpan();
		//int commaPos = input.IndexOf(',');
		//int first = int.Parse(inputSpan.Slice(0, commaPos));
		//int second = int.Parse(inputSpan.Slice(commaPos + 1));

		//var s = "one two";
		//var k = s.AsReadOnlySpan(3);
		////Print(k.);
		//for(int i = 0; i < k.Length; i++) Print(k[i]);

		//foreach(var v in s.Segments_(Separators.Whitespace, SegFlags.NoEmpty)) {
		//	Print(v);
		//}

	}

	static void TestAuDialogRenamed()
	{
		//Print(Dpi.BaseDPI);
		var s = "More text";
		//s = "WWWWWWWWWWWWW WWWWWWWWWWWW MMMMMMMMMMM MMMMMMMMMMMM WWWWWWWWWWWW WWWWWWWWWWWW MMMMMMMMMMM MMMMMMMMMMM";
		//Print(AuDialog.Show("Main instruction", s, "OK|Cancel|100 C1|C2", DIcon.App, DFlags.CommandLinks, expandedText: "Exp"));
		//Print(AuDialog.Show("Main instruction", s, "OK|Cancel|100 C1|C2", DIcon.App, DFlags.CommandLinks | DFlags.Wider, expandedText: "Exp"));
		//Print(AuDialog.Show("Main instruction", s, "OK|Cancel|100 C1|C2", default, DFlags.CommandLinks, expandedText: "Exp"));
		//Print(AuDialog.Show("Main instruction", s, "OK|Cancel|100 C1|C2", default, DFlags.CommandLinks | DFlags.Wider, expandedText: "Exp"));
		//Print(AuDialog.Show("Main instruction wide wide wide wide wide wide wide", s, "OK|Cancel|100 C1|C2", DIcon.App, DFlags.CommandLinks, expandedText: "Exp"));
		//Print(AuDialog.Show("Main instruction wide wide wide wide wide wide wide", s, "OK|Cancel|100 C1|C2", DIcon.App, DFlags.CommandLinks | DFlags.Wider, expandedText: "Exp"));

		////MessageBox.Show(s, "cap", default, MessageBoxIcon.Asterisk);

		//AuDialog.ShowList("One wide wide wide wide wide wide wide and even more wide|Two");
		AuDialog.ShowList("One wide wide wide wide wide wide wide and even more wide|Two", flags: DFlags.Wider);

		//if(!AuDialog.ShowTextInput(out var s)) return;
		//if(!AuDialog.ShowNumberInput(out var i)) return;

	}

	static void TestRegexExamples()
	{
		var s = "one, two,three , four";
		var x = new Regex_(@" *, *");
		var a = x.SplitG(s);
		foreach(var v in a) Print(v.Index, v.Value);

		//Print(s.RegexIndexOf_)


		//var s = "one obertone";
		//var r = @"\w\w\w(?C)\w+";
		//var x = new Regex_(r);
		//x.Callout = o => Print(o.current_position);
		//if(x.Match(s, out var m)) Print(m); else Print("not found");

		//var s = "one two22 three-333 four 55 -77";
		////var x = new Regex_(@"\d+", RXFlags.MATCH_WORD);
		////var x = new Regex_(@"\b(?:\d+)\b");
		////var x = new Regex_(@"[A-Z]+", RXFlags.CASELESS);
		//var x = new Regex_(@"^\w{3}");
		////var x = new Regex_(@"^\w{3}", RXFlags.PARTIAL_SOFT);
		////if(!x.FindAll(s, out var a)) { Print("not found"); return; }
		////foreach(var m in a) Print(m.Value);

		//if(x.Match("fg", out var m)) Print(m, m.IsPartial); else Print("no match");
		//if(x.Match("fg", out var m2, new RXMore(matchFlags: RXMatchFlags.PARTIAL_SOFT))) Print(m2, m2.IsPartial); else Print("no match");

		//var s = "one two22 three333 four";
		//var x = new Regex_(@"\b(\w+?)(\d+)\b");
		//if(!x.FindAll(s, out var a)) { Print("not found"); return; }
		//foreach(var m in a) Print(m.Value, m[1].Value, m[2].Value);

		//var s = "one two22 three333 four";
		//var x = new Regex_(@"\b(\w+?)(\d+)\b");
		//Print(x.IsMatch(s));
		//if(x.Match(s, out var m)) Print(m.Value, m[1].Value, m[2].Value);
		//Print(x.FindAllS(s, 2));
		//Print(x.ReplaceAll(s, "'$2$1'"));
		//Print(x.ReplaceAll(s, o => o.Value.ToUpper_()));


		//var s = "text <a href='url'>link</a> text";
		//var rx =@"(?C1)<a (?C2)href='.+?'>(?C3)[^<]*(?C4)</a>";
		//var x = new Regex_(rx);
		//x.Callout = o => { Print(o.callout_number, o.current_position, s.Substring(o.start_match, o.current_position), rx.Substring(o.pattern_position, o.next_item_length)); };
		//Print(x.IsMatch(s));

		//var s = "one 'two' three";
		//var rx = @"'(.+?)'";
		//var x = new Regex_(rx, RXFlags.AUTO_CALLOUT);
		//x.Callout = o => Print(o.current_position, o.pattern_position, rx.Substring(o.pattern_position, o.next_item_length));
		//Print(x.IsMatch(s));

		//var s = "one 123-5 two 12-456 three 1-34 four";
		//var x = new Regex_(@"\b\d+-\d+\b(?C1)");
		//x.Callout = o => { int len = o.current_position - o.start_match; /*Print(len);*/ if(len > 5) o.Result = 1; };
		//Print(x.FindAllS(s));
	}

	static void TestRegexStatic()
	{
		//var s = "ab cd-45-ef gh";
		//if(s.RegexMatch_(@"\b([a-z]+)-(\d+)\b", out RXMatch m))
		//	Print(
		//		m.GroupCount, //3 (whole match and 2 groups)
		//		m.Index, //3, same as m[0].Index
		//		m.Value, //"cd-45-ef", same as m[0].Value
		//		m[1].Index, //3
		//		m[1].Value, //"cd"
		//		m[2].Index, //6
		//		m[2].Value //"45"
		//		);

		//var s = "ab cd--ef gh";
		//if(s.RegexMatch_(@"\b([a-z]+)-(\d+)?-([a-z]+)\b", out RXMatch m))
		//	Print(
		//		m.GroupCountPlusOne, //4 (whole match and 3 groups)
		//		m[2].Exists, //false
		//		m[2].Index, //-1
		//		m[2].Length, //0
		//		m[2].Value //null
		//		);


		//var s = "one two22, three333,four";
		//var x = new Regex_(@"\b(\w+?)(\d+)\b");

		// Print("//IsMatch:");
		//Print(x.IsMatch(s));

		// Print("//Match:");
		//if(x.Match(s, out var m)) Print(m.Value, m[1].Value, m[2].Value);

		// Print("//FindAll with foreach:");
		//foreach(var v in x.FindAll(s)) Print(v.Value, v[1].Value, v[2].Value);
		// Print("//FindAllS, get only strings of group 2:");
		//Print(x.FindAllS(s, 2));

		// Print("//Replace:");
		//Print(x.Replace(s, "'$2$1'"));
		// Print("//Replace with callback:");
		//Print(x.Replace(s, o => o.Value.ToUpper_()));
		// Print("//Replace with callback and ExpandReplacement:");
		//Print(x.Replace(s, o => { if(o.Length > 5) return o.ExpandReplacement("'$2$1'"); else return o[1].Value; }));

		// Print("//Split:");
		//Print(new Regex_(@" *, *").Split(s));

		//var s = "one two22, three333,four";
		//var rx = @"\b(\w+?)(\d+)\b";
		//Print("//RegexIsMatch_:");
		//Print(s.RegexIsMatch_(rx));
		//Print("//RegexMatch_:");
		//if(s.RegexMatch_(rx, out var m)) Print(m.Value, m[1].Value, m[2].Value);
		//Print("//RegexFindAll_ with foreach:");
		//foreach(var v in s.RegexFindAll_(rx)) Print(v.Value, v[1].Value, v[2].Value);
		//Print("//RegexFindAll_, get only strings:");
		//Print(s.RegexFindAll_(rx, 2));
		//Print("//RegexReplace_:");
		//Print(s.RegexReplace_(rx, "'$2$1'"));
		//Print("//RegexReplace_ with callback:");
		//Print(s.RegexReplace_(rx, o => o.Value.ToUpper_()));
		//Print("//RegexSplit_:");
		//Print(s.RegexSplit_(@" *, *"));

		//var s = "one two22, three333,four";
		//var rx = @"\b(\w+?)(\d+)\b";

		// Print("//RegexIsMatch_:");
		//Print(s.RegexIsMatch_(rx));

		// Print("//RegexMatch_:");
		//if(s.RegexMatch_(rx, out var m)) Print(m.Value, m[1].Value, m[2].Value);

		// Print("//RegexMatch_, get only string:");
		//if(s.RegexMatch_(rx, 0, out var s0)) Print(s0);
		// Print("//RegexMatch_, get only string of group 1:");
		//if(s.RegexMatch_(rx, 1, out var s1)) Print(s1);

		// Print("//RegexFindAll_ with foreach:");
		//foreach(var v in s.RegexFindAll_(rx)) Print(v.Value, v[1].Value, v[2].Value);

		// Print("//RegexFindAll_ with foreach, get only strings:");
		//foreach(var v in s.RegexFindAll_(rx, 0)) Print(v);
		// Print("//RegexFindAll_ with foreach, get only strings of group 2:");
		//foreach(var v in s.RegexFindAll_(rx, 2)) Print(v);

		// Print("//RegexFindAll_, get array:");
		//if(s.RegexFindAll_(rx, out var am)) foreach(var k in am) Print(k.Value, k[1].Value, k[2].Value);

		// Print("//RegexFindAll_, get array of strings:");
		//if(s.RegexFindAll_(rx, 0, out var av)) Print(av);
		// Print("//RegexFindAll_, get array of group 2 strings:");
		//if(s.RegexFindAll_(rx, 2, out var ag)) Print(ag);

		// Print("//RegexReplace_:");
		//Print(s.RegexReplace_(rx, "'$2$1'"));

		// Print("//RegexReplace_ with callback:");
		//Print(s.RegexReplace_(rx, o => o.Value.ToUpper_()));
		// Print("//RegexReplace_ with callback and ExpandReplacement:");
		//Print(s.RegexReplace_(rx, o => { if(o.Length > 5) return o.ExpandReplacement("'$2$1'"); else return o[1].Value; }));

		// Print("//RegexReplace_, get replacement count:");
		//if(0 != s.RegexReplace_(rx, "'$2$1'", out var s2)) Print(s2);

		// Print("//RegexReplace_ with callback, get replacement count:");
		//if(0 != s.RegexReplace_(rx, o => o.Value.ToUpper_(), out var s3)) Print(s3);

		// Print("//RegexSplit_:");
		//Print(s.RegexSplit_(@" *, *"));


		//var s = "one two,three , four";

		//var s = "one, two,three , four";
		////Print(s.RegexSplit_(@" *, *"));
		////Print(s.RegexSplit_(@" *, *", 2));

		////Print(s.RegexIsMatch_(@"\w+,\w+"));
		////Print(s.RegexIsMatch_(@"\w+,\w+", RXFlags.ANCHORED));

		//if(s.RegexMatch_(@"\w+", out var m)) Print(m); else Print("no match");
		//if(s.RegexMatch_(@"\w+,(\w+)", 1, out var k)) Print(k); else Print("no match");
		//if(s.RegexMatch_(@"\w+,\K\w+", 0, out var kk)) Print(kk); else Print("no match");
	}

	//public static (string result, int count) RReplace()
	//{
	//	return ("test", 1);
	//}

	//static void TestTupleReturn()
	//{
	//	var s = "subject";
	//	int n;
	//	//var r = RReplace();
	//	//var (r, n) = RReplace();
	//	(s, n) = RReplace();
	//}

	static void TestWildexRegex()
	{
		//Wildex w = @"**r \ba\Qbc\E";
		//Print(w.Match("abc"));

		Print(Wnd.Find(@"**r Q\Quick\E"));
	}

	static void TestRegexGroupByName()
	{
		string s = "one two222 three";
		if(s.RegexMatch_(@"(?<A>[a-z]+)(?<B>\d+)", out var m)) Print(m[1], m[2], m["A"], m["B"]);
	}

	static void TestPrintHex()
	{
		//for(int i=0; i<1000; i++) {
		//	char c = (char)i;
		//	Print(c.ToString());
		//}
		//return;

		//string s = "ab{s}cd{1}ef{gh\r\nij";
		//s = "{s}\r\n";
		////string rx;

		////Print(s.RegexFindAll_(@"(?s)\{[sr01]\}|.+?(?=\{|$)"));
		////Print(s.RegexFindAll_(@"\{[sr01]\}|[^{]+"));
		////foreach(var m in s.RegexFindAll_(@"(?s)(\{[sr01]\})|.+?(?=(?1)|\z)")) Print($"{m.Index} {m.Length} '{m.Value}'");
		//foreach(var m in s.RegexFindAll_(@"(?s)(\{[sr01]\})|.+?(?=(?1)|$)")) Print($"{m.Index} {m.Length} '{m.Value}'");

		//return;

		//int i = 10;
		//Print(i);
		//Print((uint)i);
		//string s = "kk";
		//var ca = new char[] { 'a', 'b' };
		//var ca = new int[] { 'a', 'b', '\0', '\r', '\n', '\t', (char)20, (char)130, 'c' };
		//var ca = new string[] { "one", "two\r\n\t\0mmm", null };
		//var ca = new uint[] { 'a', 'b', '\0', '\r', '\n', '\t', (char)20, (char)130, 'c' };
		//var ca = new int?[] { 'a', 'b', '\0', '\r', '\n', '\t', (char)20, null };
		//var ca = new object[] { 'a', 10, 10u, "one", "two\r\n\tmmm", null };
		//Print(i, (uint)i, s, ca);
		//Print(s);
		//Print((object)s);
		//s = null;
		//Print(i, (uint)i, s);
		//Print(s);
		//Print((object)s);
		//Print(new StringBuilder("SB"));
		//Print(new int[] { 4, 5 });

		//Print("Print(ca):");
		//Print(ca);
		//Print("Print((object)ca):");
		//Print((object)ca);
		//Print(@"PrintListEx(ca, ""{s}\r\n""):");
		//PrintListEx(ca);
		//PrintListEx(ca, "{s}\r\n");
		//PrintListEx(ca, "{0}. '{s}'\r\n");
		//PrintListEx(ca, "{s}", 0);
	}

	//static void TestWaitPrecision()
	//{
	//	//Print(Time.LibSleepPrecision.Current);
	//	//var t = new WaitFor.Loop(-0.1, 1);
	//	//for(; ; ) {
	//	//	Perf.First();
	//	//	if(!t.Sleep()) break;
	//	//	Perf.NW();
	//	//}

	//	Print(WaitForMouseLeftButtonDown(3));

	//	//Print(WaitFor.Condition(3, () => Mouse.IsPressed(MouseButtons.Left)));
	//}

	//static bool WaitForMouseLeftButtonDown(double secondsTimeout)
	//{
	//	var x = new WaitFor.Loop(secondsTimeout);
	//	for(; ; ) {
	//		if(Mouse.IsPressed(MouseButtons.Left)) return true;
	//		if(!x.Sleep()) return false;
	//	}
	//}

	//static bool WaitForMouseLeftButtonDown2(double secondsTimeout)
	//{
	//	return WaitFor.Condition(secondsTimeout, () => Mouse.IsPressed(MouseButtons.Left));
	//}

	[DllImport("user32.dll")]
	internal static unsafe extern bool GetKeyboardState(sbyte* lpKeyState);

	public static unsafe KMod GetMod(KMod modifierKeys = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
	{
		var a = stackalloc sbyte[256];
		GetKeyboardState(a);
		if(a[17] >= 0) modifierKeys &= ~KMod.Ctrl;
		if(a[16] >= 0) modifierKeys &= ~KMod.Shift;
		if(a[18] >= 0) modifierKeys &= ~KMod.Alt;
		if((a[91] >= 0 && a[92] >= 0)) modifierKeys &= ~KMod.Win;
		return modifierKeys;
	}

	static void TestGetMod()
	{
		//Print(GetMod());
		//Print(Keyb.IsMod());
		Print(Keyb.GetMod());

		Perf.Cpu(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 1000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { Keyb.IsMod(); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { GetMod(); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { Keyb.GetMod(); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { }
			Perf.NW();
		}

	}

	//static void TestAttachThreadInput()
	//{
	//	var w = Wnd.Find("FileZilla").ChildById(-31791).OrThrow();
	//	//using(new LibAttachThreadInput(w.ThreadId, out bool ok)) {
	//	//	Print(ok);
	//	//}
	//	w.Focus();
	//}

	//static void TestLibThreadCpuSwitcher()
	//{
	//	//Task.Run(() => Perf.SpinCPU(300));
	//	//Task.Run(() => Perf.SpinCPU(300));
	//	//Task.Run(() => Perf.SpinCPU(300));
	//	//Task.Run(() => Perf.SpinCPU(300));
	//	10.ms();
	//	using(var ts = new LibThreadSwitcher()) {
	//		for(int i = 0; i < 15; i++) {
	//			Perf.First();
	//			if(!ts.Switch()) Print("failed");
	//			Perf.NW();
	//		}
	//	}
	//}

	static void TestBlockUserInput()
	{
		//using(new BlockUserInput(BIEvents.All)) {
		//	Print("blocked");
		//	5.s();
		//}
		//Print("unblocked");
		//return;

		//100.ms();
		//using(var bi = new BlockUserInput()) {
		//	for(int i = 0; i < 5; i++) {
		//		bi.Start(BIEvents.Keys);
		//		100.ms();
		//		bi.Stop();
		//		100.ms();
		//	}
		//}
		//for(int i = 0; i < 5; i++) {
		//	using(var bi = new BlockUserInput(BIEvents.Keys)) 100.ms();
		//	100.ms();
		//}

		//Task.Run(() => {
		//	for(int i = 0; i < 100; i++) {
		//		100.ms();
		//		GC.Collect();
		//	}
		//});

		//BlockUserInput.DoNotBlockKeys = new Keys[] { Keys.A, Keys.B };

		//using(var bi = new BlockUserInput(BIEvents.Keys)) {
		//using(var bi = new BlockUserInput(BIEvents.MouseClicks|BIEvents.MouseMoving)) {
		using(var bi = new BlockUserInput(BIEvents.All)) {
			//bi.Dispose();
			//bi.Stop();
			//bi.Start(BIEvents.Keys);
			bi.ResendBlockedKeys = true;
			//bi.ResumeAfterCtrlAltDelete = true;
			//15.s();

			5.s();
			//Shell.Run(@"C:\Program Files\Process Hacker 2\ProcessHacker.exe");
			//AuDialog.ShowEx(secondsTimeout: 5);
			//Print("> inner");
			//using(var bk = new BlockUserInput()) {
			//	bk.ResendBlockedKeys = true;
			//	bk.Start(BIEvents.Keys);
			//	5.s();
			//}
			//Print("< inner");
			//5.s();

			//3.s();
			//bi.Pause = true;
			//3.s();
			//bi.Pause = false;
			//3.s();

			//AuDialog.Show("started");
			//bi.Stop();
			//AuDialog.Show("stopped");
		}

		//var t = new Thread(() =>
		//  {
		//	  var bi = new BlockUserInput(BIEvents.Keys);
		//	  500.ms();
		//  });
		//t.Start();
		//AuDialog.Show("started");

		Print("END");
	}

	static void TestIsKey()
	{
		for(int i = 1; i < 255; i++) {
			var k = (KKey)i;
			var s = k.ToString(); if(Empty(s) || s.Contains(",")) continue;
			//Print($"{k,-20} {Keyb.IsKeyPressed(k)}, {Keyb.IsKeyToggled(k)}");
			Print($"{k,-20} {Keyb.GetKeyState(k):X4} {Keyb.GetAsyncKeyState(k):X4}");
		}
	}

	static string _CreateLongText(int len)
	{
		var b = new StringBuilder(len + 30);
		while(b.Length < len) {
			if(b.Length > 0) b.Append(' ');
			b.Append(b.Length);
		}
		return b.ToString();
	}

	static void TestKey()
	{
		//Key("RShift*down");
		//100.ms();
		//Print(Keyb.GetKeyState(Keys.ShiftKey), Keyb.GetKeyState(Keys.LShiftKey), Keyb.GetKeyState(Keys.RShiftKey));
		//Key("RShift*up");
		//Print(Keyb.GetKeyState(Keys.ShiftKey), Keyb.GetKeyState(Keys.LShiftKey), Keyb.GetKeyState(Keys.RShiftKey));

		//Key("RCtrl*down");
		//100.ms();
		//Print(Keyb.GetKeyState(Keys.ControlKey), Keyb.GetKeyState(Keys.LControlKey), Keyb.GetKeyState(Keys.RControlKey));
		//Key("Ctrl*up");
		//Print(Keyb.GetKeyState(Keys.ControlKey), Keyb.GetKeyState(Keys.LControlKey), Keyb.GetKeyState(Keys.RControlKey));

		//Opt.Key.NoModOff = true;
		////Key("Alt*down");
		//1100.ms();
		//Print(Keyb.GetKeyState(Keys.Menu), Keyb.GetKeyState(Keys.LMenu), Keyb.GetKeyState(Keys.RMenu));
		////Key("Alt*up");
		////Print(Keyb.GetKeyState(Keys.Menu), Keyb.GetKeyState(Keys.LMenu), Keyb.GetKeyState(Keys.RMenu));

		//return;

		//Key("RWin*down");
		//100.ms();
		//Print(Keyb.GetKeyState(Keys.LWin), Keyb.GetKeyState(Keys.RWin));
		//Key("RWin*up");
		//Print(Keyb.GetKeyState(Keys.LWin), Keyb.GetKeyState(Keys.RWin));

		//return;

		//TestKParam(() => Print(1));

		//if(Time.LibSleepPrecision.Current < 15) throw new Exception("s");
		//AuDialog.Show(""+Time.LibSleepPrecision.Current);
		////Print(System.Configuration.ConfigurationSettings.AppSettings.AllKeys);
		//return;

		//PrintHex(Keyb._GetKeyboardLayoutOfActiveThread()); return;

		//var k = new Keyb();
		//k.Key("a");

		var wa = Wnd.Active;
		//var w = Wnd.Find("Quick*").OrThrow();
		//var w = Wnd.Find("* Writer").OrThrow();
		//var w = Wnd.Find("* Word*").OrThrow();
		//var w = Wnd.Find("Dialog").OrThrow();
		//var w = Wnd.Find("*Notepad").OrThrow();
		//w.Activate();
		if(wa.Name.Like_("*Studio ")) Wnd.SwitchActiveWindow();
		100.ms();

		//new Keyb(Opt.Key).AddKeys("Tab Ctrl+V Alt+E+P Alt+(E P) Ctrl+Shift+Left Left*3 Space a , 5 #5 $abc Enter").Send();
		//new Keyb(Opt.Key).AddKeys("Tab Ctrl+V Alt+(E P) Left*3 Space a , 5 #5 $abc Enter").AddText("Abc ").Send();

		//Key("ab", 1000, "cd", 2000);
		//Opt.Key.TextOption = KTextOption.Keys;
		//Text("aą2");

		//Key("Alt+Tab", 100, "", "text");
		//Key("Alt+Tab", "text");

		////Opt.Key.SleepFinally = 20;
		//Opt.Key.KeySpeed = 100;
		////Key("ab");
		////Key("a Ctrl+b c d");
		//Key("Ctrl+c");
		////Key("Ctrl+Alt+c");
		////Key("Alt+a+b+c z");
		////Key("Alt+a+b+c");
		////Key("Alt+(a b)");
		////Key("Alt+(a b) c");
		////Key("A*down A*up");
		////Key("A*down");
		////Key("A*up");
		////Key("A*3");
		////Key("A+*3");
		////Key("A", 10, "B");
		////Key("Ctrl+", 20, "B");
		////Key("Ctrl*down A*down Ctrl*up A*up");
		////Key("Shift+", "text");

		////Opt.Key.TextOption = KTextOption.Keys;
		////for(int i = 0; i < 5; i++) {
		////	//Text("One Two Three Four Five Six Seven Eight Nine Ten\r\n");
		////	Text("OneTwoThreeFourFive ");
		////}
		//return;

		//Text("abCD$,ąĄ", "keys Enter");
		//Key(1000, "", "abCD$,ąĄ", "keys Enter");

		//Opt.Key.SleepFinally = 0;
		////Opt.Key.TextSpeed=0;
		//var s =_CreateLongText(100);
		//Print(s.Length);
		//Perf.First();
		//Text(s, "Enter");
		//Perf.NW();

		//Key(Keys.BrowserBack);
		//Key("BrowserBack");

		//for(int i = 0; i < 5; i++) {
		//	Perf.First();
		//	Clipb.Clear();
		//	//Clipb.SetText("fff");
		//	//300.ms();
		//	Perf.Next();
		//	//20.ms();
		//	Key("Ctrl+C");
		//	Perf.NW();
		//	300.ms();
		//}
		//return;

		//var b = new Keyb(null);
		//b.SleepFinally = 0;
		//b.KeySpeed = 0;
		//for(int i = 0; i < 5; i++) {
		//	Clipb.Clear();
		//	20.ms();
		//	b.Add("Ctrl+C");
		//	//int t = 1;
		//	////b.Add("Ctrl*down", 1, "C*down", 1, "C*up", 0, "Ctrl*up");
		//	//b.Add("Ctrl*down", 1, "C*down", 1, "Ctrl*up", 0, "C*up");
		//	//b.AddKey(Keys.ControlKey).AddKeys("+").AddKey(Keys.A);
		//	//Perf.First();
		//	b.Send();
		//	//Perf.NW();
		//	200.ms();
		//	Print(Clipb.GetText());
		//}
		Print("END");

		//Opt.Key.KeySpeed = 1;
		//Text("Some Text, Some Text, Some Text, Some Text, Some Text, Some Text, Some Text, Some Text\r\n");
		//Key("$some Spa $text, $some Spa $text, $some Spa $text, $some Spa $text, $some Spa $text, $some Spa $text, $some Spa $text, Enter");
		return;

		//Opt.Key.SleepFinally = 100;
		//Opt.Key.Hook = () => new OptKey() { SleepFinally = 100 };
		//Opt.Key.Hook = o =>
		//{
		//	Print(o.w);
		//	Wnd w = o.w.Window;
		//	if(w.Name?.StartsWith_("Unti") ?? false) o.opt.KeySpeed = 500;
		//	//opt.TextOption = KTextOption.Keys;
		//	//opt.SleepFinally = 1000;
		//	//opt.TextSpeed = 500;
		//};

		var x = new Keyb(Opt.Key);

		//2.s();
		//Action actNotepad = () => Wnd.Find("*Notepad").Activate();
		//Action actQm = () => Wnd.Find("Quick M*").Activate();
		//x.Add("abc Space", actNotepad, "some text", actQm, "Enter");
		//x.Add("abc Space", "some text", 1d, "Enter", 2.5);
		//x.AddText("a"); x.AddText("b"); x.AddText("c");
		x.AddText("a"); x.AddCallback(() => Print(1)); x.AddText("c");
		//x.Add("Alt+Tab");
		//x.Add("Ctrl+S a", 1000, "Esc*2");
		Perf.First();
		for(int i = 0; i < 1; i++) {
			x.Send();
			//x.Send(true);
			Perf.Next();
		}
		Perf.Write();
		//x.Key("Enter", "Tab", "Enter");
		//x.Text("Left");

		return;

		//x.AddChar('a').AddChar('B');
		//x.AddText("Test");
		//x.AddKeys("Ctrl+A Tab*33 T * 33k b* Num* ab c Alt+(A B) Alt + (A B) #- #+ #* # #hk h#*#4 #**55 F12 FF kNow **");
		//x.AddKeys("Shift+A");
		//x.AddKeys("Shift+A qwertyuiopasdfghjklzxcvbnm");
		//x.AddKeys("Ctrl+Shift+A");
		//x.AddKeys("B*3");
		//x.AddKeys("Shift+C*3");
		//x.AddKeys("Shift+M"); x.Send(); x.Send(); x.Send();
		//x.AddKeys("Shift+M"); for(int i=0; i<3; i++) x.Send(true);
		//x.AddKeys("Shift+"); x.Send(); Print(1); x.AddKeys("k"); x.Send();
		//x.AddEvents("Shift+", Keys.K, (45, false));
		//x.AddKeys("Ctrl LCtrl RCtrl Alt+a LAlt+a RAlt+a Shift LShift RShift");
		//x.AddKeys("ab Esc 1 2 RCtrl Right Shift+A Alt+(fe)");
		//x.AddText("one\rtwo\nthree\r\nfour ąč");
		//x.AddKeys("Ctrl+A Del");
		//x.AddKeys(@"Tab qwertyuiop Enter asdfghjkl Enter zxcvbnm Enter `1234567890-= Enter []\;',./ Enter #/#*#-#+#.#0#1#2#3#4#5#6#7#8#9");
		//x.AddKeys("F1 F12 F24 Back");
		//x.AddKeys("F1 F12 F24 Back Backspace BAC NumLock NUM");
		//x.AddKeys("Alt AltG App Add Bac Ctr Con Cap Del Dec Div Dow Ent End Esc Hom Ins");
		//x.AddKeys("Lef LSh LCt LCo LAl LWi Rig RSh RCt RCo RAl RWi Ret");
		//x.AddKeys("Men Mul Num PgU PageU PgD PageD Pau Pri PRT Shi Spa Scr Sub Tab Up Win");
		//x.AddKeys("$send$input $4");
		//x.AddKeys("$send$(input)");
		//x.AddText("𑀠-𑀡-𑀢-𑀣-𑀤-𑀥-𑀦-𑀧-𑀨-𑀩-𑀪-𑀫-𑀬-𑀭-𑀮-𑀯");
		//x.AddKeysText("", "𑀠-𑀡-𑀢-𑀣", 1000, "", "-𑀤-𑀥");
		//Keyb.Common.TextSpeed = 100;
		//Keyb.Common.TimeTextCharSent = 1;
		//x.AddText("123 β, 𑀨-𑀪-𑀫.");
		//x.AddText("123456789").AddSleep(400).AddText(" 123456789\r\n");
		//x.AddText("a β 123 ƱʍЧ.");
		//x.AddKeys("Enter", 10);
		//Keyb.Common.TextOption = KTextOption.Keys;
		//x.AddKeys("-");
		//x.AddText("abc");
		//x.AddKeys("Shift+(");

		//var s = new StringBuilder();
		//for(int i = 0; ; i++) {
		//	int len = s.Length; if(len > 1000) break;
		//	s.Append(len).Append(" 123456789 123456789 123456789 123456789 123456789 βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ\r\n");
		//	//s+="123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 1234567-\r\n"
		//	//	+ "βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱʍЧ βƱ-\r\n";
		//}
		//x.AddText(s.ToString());

		////x.AddText("a\r\nBcDE\n\r");
		//x.AddText("aąAĄβƱʍЧ\r\n");
		//x.AddText("abc");

		//x.Add("Tab", 1000, Keys.A, (Action)(() => Print(1)), "Tab-");

		Action click = () => Mouse.Click();
		//x.Add("Shift+", click);
		//x.Add("Shift+(", click);
		//x.Add("Shift+(", click, ")");
		//x.Add("Shift+", (Action)(() => Mouse.Click()));
		//x.Add("Shift+", click, "a");
		//x.Add("Shift+", Keys.A, "a");
		//x.Add("Shift+", 500, "a");
		//x.Add("Shift+", "text", "a");
		//x.Add("Shift+", 500);
		//x.Add(Keys.A, "*3");
		x.Add("Shift*down", click, "*up");


		Perf.First();
		x.Send();
		//Keyb.Common.KeySpeed = 0;
		//for(int i = 0; i < 8; i++) {
		//	x.Send(true);
		//}
		//SendKeys.SendWait(s);
		Perf.NW();

		//100.ms();
		//wa.Activate();
	}

	static void TestKeybExamples()
	{

		//var k = new Keyb(Opt.Static.Key);
		//k.KeySpeed = 50;
		//k.AddKeys("Tab // Space").AddRepeat(3).AddText("text").AddKey(Keys.Enter).AddSleep(500);
		//k.Send(); //sends and clears the variable
		//k.Add("Tab // Space*3", "text", Keys.Enter, 500); //does the same as the above k.Add... line
		//for(int i = 0; i < 5; i++) k.Send(true); //does not clear the variable

		//Keyb.Key("Ctrl+Shift+Left"); //press Ctrl+Shift+Left

		//Opt.Key.KeySpeed = 300; //set options for static functions
		//Keyb.Key("Ctrl+A Del Tab*3", "text", "Enter", 500); //press Ctrl+A, press Del, press Tab 3 times, send text, press Enter, wait 100 ms

		//Keyb.Text("text\r\n"); //send text that ends with newline
		//Keyb.Text("text", "Enter", 300); //send text, press Enter, wait 300 ms

		Text("Key and Text can be used without the \"Keyb.\" prefix.");
		Key("Enter");
	}

	static void TestClipb()
	{
		//Clipb.SetText("qwe");

		//var s = Clipb.GetText();
		//Print((object)s);


		//Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Perf.First();
		//	string s = Clipb.GetText();
		//	Perf.NW();
		//	Print((object)s);
		//	100.ms();
		//}

	}

	static void TestPaste()
	{
		var wa = Wnd.Active;
		if(wa.Name.Like_("*Studio ")) Wnd.SwitchActiveWindow();
		100.ms();

		Opt.Key.TextOption = KTextOption.Paste;
		Opt.Key.PasteEnter = true;
		Opt.Key.SleepFinally = 0;
		//Opt.Key.RestoreClipboard = true;
		OptKey.RestoreClipboardAllFormats = true;
		//OptKey.RestoreClipboardExceptFormats = new string[] { "Rich Text Format" };

		//Opt.Key.Hook = o =>
		//{
		//	Print(o.w);
		//	o.opt.PasteEnter = true;
		//};

		//Text("a\r\n");
		//return;

		var a = new string[] { "One ", "Two ", "Three\r\n", };
		//var a = new string[] { "One\r\n", "Two\r\n", "Three\r\n", };
		Perf.First();
		for(int i = 0; i < a.Length; i++) {
			//Paste("Paste\r\n");
			//Paste(a[i]);
			Text(a[i]);
			Perf.Next();
			//100.ms();
			//Wnd.Focused.Send(0);
			//Perf.Next();
			//break;
		}
		Perf.Write();

		//Paste("Paste\r\n");
		//Text("Text\r\n");
		//Paste("Text\r\n", "Tab keys");
		//Text("Text\r\n", "Tab keys");
		//Key("a");
		Print("END");
	}

	static void TestCopy()
	{
		//Api.OpenClipboard(default);
		//for(int format = 0; 0 != (format = Api.EnumClipboardFormats(format));) {
		//	Print(Clipb.LibGetFormatName(format));
		//}
		//Api.CloseClipboard();
		//return;

		//Print(Clipb.GetText());
		//return;

		var wa = Wnd.Active;
		//if(wa.Name.Like_("*Studio ")) Wnd.Misc.SwitchActiveWindow();
		100.ms();

		//Opt.Key.SleepFinally = 0;
		//Opt.Key.Hook = o =>
		//{
		//	Print(o.w);
		//	o.opt.PasteEnter = true;
		//};

		//Text("a\r\n");
		//return;

		//Opt.Key.RestoreClipboard = false;
		OptKey.RestoreClipboardAllFormats = true;
		//OptKey.RestoreClipboardExceptFormats = new string[] { "Rich Text Format" };

		//OptKey.PrintClipboard();
		////return;
		//Print("---");

		//for(int i = 0; i < 1; i++) {
		//	Perf.First();
		//	var s = Clipb.CopyText();
		//	Perf.NW();
		//	Print(s);
		//	100.ms();
		//	//break;
		//}

		//Print("---");
		//OptKey.PrintClipboard();
		//Print(Clipb.GetText());

		var af = new object[] { 0, 1, 7, 13, "Rich Text Format", "HTML Format", "text/html", "FileName", "FileNameW", "DwHt" };
		foreach(var v in af) {
			int f = 0; if(v is int ii) f = ii; else f = ClipFormats.Register(v as string);
			Clipb.CopyData(() => { var s = Clipb.Data.GetText(f); Print(v, s ?? "<NO>"); });
		}

		//Print(Clipb.CopyText());
		//Clipb.CopyData(() => Print(Clipb.GetText()));
		//Clipb.CopyData(() => Print(Clipboard.GetText()));

		Print("END");
	}

	static void TestPasteFormat()
	{
		//var g = Image.FromFile(@"q:\app\il_qm.bmp");
		//Clipboard.SetImage(g);

		//return;

		//		var html=@"Version:0.9
		//StartHTML:00000146
		//EndHTML:00000281
		//StartFragment:00000180
		//EndFragment:00000245
		//SourceURL:http://www.quickmacros.com/index.html
		//<html><body>
		//<!--StartFragment--><a href=""http://www.quickmacros.com/features.html"">Screenshot</a><!--EndFragment-->
		//</body>
		//</html>";
		var html = "<i>italy</i>";
		html = "[<i>ąčę</i>]";
		html = "<html><body>[<i>ą č ę</i>]</body></html>";
		html = "<html><body>[<i>ą č ę</i>]</body></html>";

		//Clipb.SetText("Text");
		//Clipb.SetData(new(int, object)[] { (0, "Text2") });
		//Clipb.SetData(new(int, object)[] { (Api.CF_TEXT, "ASCII") });
		//Clipb.SetData(new(int, object)[] { (Clipb.Misc.RtfFormat, @"{\rtf1 rtf\par}"), (0, "Text") });
		//Clipb.SetData(new(int, object)[] { (Clipb.Misc.HtmlFormat, html), (0, "Text") });
		//Clipb.SetData(new(int, object)[] { (Clipb.Misc.HtmlFormat, html), (0, "Text") });
		//Clipb.SetData(new(int, object)[] { (Clipb.Misc.RtfFormat, @"{\rtf1 rtf\par}"), (Clipb.Misc.HtmlFormat, "<html><body>text <a href='http://www.quickmacros.com'>Quick Macros</a> text</body></html>"), (0, "Text") });
		//Clipb.SetData(new(int, object)[] { (1, "Text ą č ę") });
		//Clipb.SetData(new(int, object)[] { (1, ("Text ąčę", Encoding.GetEncoding(1257))) });
		//Clipb.SetData(new(int, object)[] { (0, g) });
		//new Clipb.Data().AddText("text").AddImage(g).SetClipboard();
		//new Clipb.Data().AddCsvTable(new CsvTable("onę,\"t\"\"w\"\"\r\no\"\r\nth     r\tee,<four>")).SetClipboard();

		//Clipb.Paste("text");
		//Clipb.PasteData(new Clipb.Data().AddText("text2"));
		//Clipb.PasteData(new Clipb.Data().AddImage(g));

		//var b = WinImage.Capture(new RECT(100, 100, 100, 100, true));
		//Wnd.Misc.SwitchActiveWindow();
		//Clipb.PasteData(new Clipb.Data().AddImage(b));

		//	new Clipb.Data().AddFiles(@"C:\Users\G\Documents\Book1.xls", @"C:\Users\G\Documents\dictionary.xls").SetClipboard();

		return;

		var wa = Wnd.Active;
		if(wa.Name.Like_("*Studio ")) Wnd.SwitchActiveWindow();
		100.ms();

		Opt.Key.PasteEnter = true;
		Opt.Key.SleepFinally = 0;
		OptKey.RestoreClipboardAllFormats = true;

		//Opt.Key.Hook = o =>
		//{
		//	Print(o.w);
		//	o.opt.SleepFinally=1000;
		//};

		Clipb.PasteText("Paste\r\n");

		Print("END");
	}

	static void TestCreateHtmlFormatData()
	{
		Clipb.Data.LibCreateHtmlFormatData("<i>italy</i>");
		Clipb.Data.LibCreateHtmlFormatData("<html><body><i>italy</i></body></html>");
		Clipb.Data.LibCreateHtmlFormatData("<html><body><!--StartFragment--><i>italy</i><!--EndFragment--></body></html>");

		//Clipb.Data.CreateHtmlFormatData("ac");
		//Clipb.Data.CreateHtmlFormatData("ač");
		//Clipb.Data.CreateHtmlFormatData("<html><body><!--StartFragment-->ac<!--EndFragment--></body></html>");
		//Clipb.Data.CreateHtmlFormatData("<html><body><!--StartFragment-->ač<!--EndFragment--></body></html>");
		//Clipb.Data.CreateHtmlFormatData("ac<html><body><!--StartFragment-->ac<!--EndFragment--></body></html>");
		//Clipb.Data.CreateHtmlFormatData("ač<html><body><!--StartFragment-->ač<!--EndFragment--></body></html>");
	}

	static void TestClipbGetText()
	{
		int ubo = ClipFormats.Register("Ubo", Encoding.UTF32);

		//var af = new object[] {0, 1, 7, 13, "Rich Text Format", "HTML Format", "text/html", "FileName", "FileNameW", "DwHt", "Ubo"};
		//foreach(var v in af) {
		//	int f = 0; if(v is int ii) f = ii; else f = ClipFormats.Register(v as string);
		//	var s = Clipb.GetText(f);
		//	Print(v, s ?? "<NO>");
		//}

		int th = ClipFormats.Register("text/html");
		int fn = ClipFormats.Register("FileName", Encoding.Default);

		new Clipb.Data().AddText("text0").AddText("text1", 1).AddText("text7", 7)//.AddText("text13", 13)
			.AddRtf("rtf").AddHtml("html").AddText("text/html", th).AddText("FileName", fn).AddText("Ubo", ubo)
			.SetClipboard();
	}

	static void TestClipbDataGet()
	{
		//Print(Clipb.Text);
		//Clipb.Text = "moo";

		//var b = Clipb.Data.GetImage();
		//if(b == null) { Print("null"); return; }

		////var f2 = Folders.Temp + "test.png";
		////b.Save(f2);
		////Shell.Run(f2);

		//var f = new Form();
		//var k = new PictureBox();
		//k.SizeMode = PictureBoxSizeMode.AutoSize;
		//k.Image = b;
		//f.Controls.Add(k);
		//f.ShowDialog();

		//Print(Clipb.Data.GetBinary(1).Length);
		//Print(Clipb.Data.GetFiles());
		//Print(Clipb.Data.GetRtf());

		//var s = Clipb.Data.GetHtml(out int fs, out int fl, out string sourceURL); if(s == null) return;
		//Print(s); Print(fs, fl, sourceURL); Print(s.Substring(fs, fl));

		//		var csv = @"onę 𑀱,""t""""Q"""" 'A'
		//o""
		//th     r	ee,<four>";
		//		var t = new CsvTable(csv);
		//var s = t.ToHtmlTable();

		//var s = t.ToXml();
		//Print(s);
		//var f = ClipFormats.Register("XML Spreadsheet", Encoding.UTF8);
		//new Clipb.Data().AddText(s, f).SetClipboard();

		//Print(t.ToHtmlTable2());
		//Print("----");
		//Print(XmlToHtml(s));

		//new Clipb.Data().AddCsvTable(t).SetClipboard();

		//Clipb.Data.
		//new Clipb.Data().

	}

	static void TestClipbDataContains()
	{
		//Print(Clipb.Data.Contains(ClipFormats.Rtf));
		//Print(Clipb.Data.Contains(ClipFormats.Rtf, ClipFormats.Text));

		//string text = null; Bitmap image = null; string[] files = null;
		//Clipb.CopyData(() => { text = Clipb.Data.GetText(); image = Clipb.Data.GetImage(); files = Clipb.Data.GetFiles(); });
		//if(text == null) Print("no text in clipboard"); else Print(text);
		//if(image == null) Print("no image in clipboard"); else Print(image.Size);
		//if(files == null) Print("no files in clipboard"); else Print(files);

		//Clipb.PasteData(new Clipb.Data().AddHtml("<b>text</b>").AddText("text"));
		//string html = null, text = null;
		//Clipb.CopyData(() => { html = Clipb.Data.GetHtml(); text = Clipb.Data.GetText(); });
		//Print(text); Print(html);
		//var image = Clipb.Data.GetImage();
		//if(image == null) Print("no image in clipboard"); else Print(image.Size);

	}

	static void TestPrintNoQuot()
	{
		//Print("aaa\r\nbbb");
		//Print("simple", "aaa\r\nbbb", null);
		Print(new string[] { "simple", "aaa\r\nbbb", null });
	}

	static void TestKeybFinally()
	{
		var wa = Wnd.Active;
		if(wa.Name.Like_("*Studio ")) Wnd.SwitchActiveWindow();
		100.ms();

		//Key("Tab abc Enter*2 Back");

		//Opt.Key.PasteEnter = true;
		//Paste("text\r\n");

		//Keyb.Key("Tab", 70000);
		//Key("#1#+#2", (Keys.Enter, 0, true));
		//Key("Shift+(A Ctrl+B)");
		//Key("Shift+(A*5)");
		//Key("Shift+", Keys.Left, "*3");
		//Key(null, "key F1 using scan code:", (0x3B, false));
		//Key("keys", 500, "", "text");

		//Action click = () => Mouse.Click(); Key("Shift+", click);
		//Key("Left", 500, "Right");
		//Key("", "numpad Enter:", (Keys.Enter, 0, true));
		//Key("Ctrl+Alt+Del");
		//Key("Win+L");

		//Key("A F2 Ctrl+Shift+A Enter*2"); //keys A, F2, Ctrl+Shift+A, Enter Enter

		//Key("Shift+A*3"); //Shift down, A 3 times, Shift up
		////Key("Shift+", Keys.A, Keys.B); //press Shift+A, B
		////Key("Shift+(", Keys.A, 500, Keys.B, ")");
		new Keyb(null).Add("keys", "text").Send();
		return;

		//Press key Enter.
		Keyb.Key("Enter");

		//The same as above. The "Keyb." prefix is optional.
		Key("Enter");

		//Press keys Ctrl+A.
		Key("Ctrl+A");

		//Ctrl+Alt+Shift+Win+A.
		Key("Ctrl+Alt+Shift+Win+A");

		//Alt down, E, P, Alt up.
		Key("Alt+(E P)");

		//Alt down, E, P, Alt up.
		Key("Alt*down E P Alt*up");

		//Press key End, key Backspace 3 times, send text "Text".
		Key("End Back*3", "Text");

		//Press Tab n times, send text "user", press Tab, send text "password", press Enter.
		int n = 5;
		Key($"Tab*{n}", "user", "Tab", "password", "Enter");

		//Send text "Text".
		Text("Text");

		//Send text "user", press Tab, send text "password", press Enter.
		Text("user", "Tab", "password", "Enter");

		//Press Ctrl+V, wait 500 ms, press Enter.
		Key("Ctrl+V", 500, "Enter");

		//Press Ctrl+V, wait 500 ms, send text "Text".
		Key("Ctrl+V", 500, "", "Text");

		//F2, Ctrl+K, Left 3 times, Space, A, comma, 5, numpad 5, Shift+A, B, C, BrowserBack.
		Key("F2 Ctrl+K Left*3 Space a , 5 #5 $abc", Keys.BrowserBack);

		//Shift down, A 3 times, Shift up.
		Key("Shift+A*3");

		//Shift down, A 3 times, Shift up.
		Key("Shift+", Keys.A, "*3");

		//Shift down, A, wait 500 ms, B, Shift up.
		Key("Shift+(", Keys.A, 500, Keys.B, ")");

		//Send keys and text slowly.
		Opt.Key.KeySpeed = Opt.Key.TextSpeed = 50;
		Key("keys$:Space 123456789 Space 123456789 ,Space", "text: 123456789 123456789\n");

		//Ctrl+click
		Action click = () => Mouse.Click();
		Key("Ctrl+", click);

		//Ctrl+drag
		Action drag = () => { using(Mouse.LeftDown()) Mouse.MoveRelative(0, 50); };
		Key("Ctrl+", drag);

		//Ctrl+drag, poor man's version
		Key("Ctrl*down");
		using(Mouse.LeftDown()) Mouse.MoveRelative(0, 50);
		Key("Ctrl*up");
	}

	//static void TestTimeKeyPressedInt()
	//{
	//	Wnd.Find("Quick*").Activate();
	//	100.ms();
	//	Opt.Key.SleepFinally = 0;
	//	Opt.Key.KeySpeed = Opt.Key.TextSpeed = 50;

	//	Perf.First();
	//	for(int i = 0; i < 1; i++) {
	//		Key("keys$:Space 123456789 Space 123456789 ,Space", "text: 123456789 123456789\n");
	//		Perf.Next();
	//	}
	//	Perf.Write();
	//}

	static void TestKeyOwnThread()
	{
		var f = new Form();
		var b = new Button() { Text = "Key" };
		var t = new TextBox() { Top = 100 };
		var c = new Button() { Text = "Close", Left = 100 };
		f.Controls.Add(b);
		f.Controls.Add(t);
		f.Controls.Add(c); f.CancelButton = c;

		b.Click += async (unu, sed) =>
		{
			//Key("Tab", "text", 2000, "Esc"); //incorrect; may work or not; the form does not respond until Key returns
			await Task.Run(() => { Key("Tab", "text", 2000, "Esc"); }); //correct
		};

		f.ShowDialog();
	}

	//public static class Options
	//{
	//	public static OptKey Key { get { return _key; } }
	//	[ThreadStatic] static OptKey _key;
	//}

	static void TestAuOptions()
	{
		//Opt.Static.Key.KeySpeed = 7;

		//Opt.Mouse.ClickSpeed = 1000;
		//for(int i = 0; i < 1; i++) {
		//	Perf.First();
		//	//Mouse.Click();
		//	Mouse.DoubleClick();
		//	Perf.NW();
		//	600.ms();
		//}

		//var w = Wnd.Find("*Notepad").OrThrow();
		////Opt.Mouse.Relaxed = true;
		//Mouse.Move(w, 200, 200);

		//Opt.Static.Mouse.ClickSpeed = 5;
		//Print(Opt.Static.Mouse.ClickSpeed);
		//Print("----");
		//Print(Opt.Mouse.ClickSpeed);
		//using(Opt.Temp.Mouse) {
		//	Opt.Mouse.ClickSpeed = 100;
		//	Print(Opt.Mouse.ClickSpeed);
		//} //here restored automatically
		//Print(Opt.Mouse.ClickSpeed);

		//Opt.Static.Key.KeySpeed = 10;
		//Print(Opt.Key.KeySpeed);
		//Opt.Key.KeySpeed = 22;
		//Print(Opt.Key.KeySpeed);
		//using(Opt.Temp.Key) {
		//	Opt.Key.KeySpeed = 5;
		//	Print(Opt.Key.KeySpeed);
		//} //here restored automatically
		//Print(Opt.Key.KeySpeed);

		//Opt.Debug.Verbose = false;
		//Opt.Debug.DisableWarnings("Exam*");
		//PrintWarning("Example");
		//PrintWarning("Example");

		//Opt.Debug.Verbose = true;
		//PrintWarning("one");
		//using(Opt.Debug.DisableWarnings("*")) {
		//	PrintWarning("two");
		//}
		//PrintWarning("three");

		//Opt.Mouse.MoveSpeed = 30;
		//Opt.Mouse.ClickSpeed = 100;
		//Opt.Mouse.ClickSleepFinally = 100;
		//Opt.Mouse.MoveSleepFinally = 100;
		//Perf.First();
		////Mouse.MoveRelative(100, 1000);
		////Mouse.Move(Coord.Center, Coord.MaxInside);
		//Mouse.Click(Coord.Center, Coord.Center);
		////Mouse.Click();
		//Perf.NW();

		//Wnd.Find("Quick*").Activate();
		////Opt.Key.KeySpeed = 10;
		//Opt.Key.KeySpeed = 0;
		//Opt.Key.SleepFinally = 500;
		//Key("$coord.$center, $coord.$max$inside", "Coord.Center, Coord.MaxInside\n");
		//Print("END");

		//Clipb.CopyText(false, )
	}

	static void _TestKeybClipbMouseOwnWindow()
	{
		//Opt.Key.SleepFinally = 3000;
		Opt.Key.KeySpeed = 1000;
		Perf.First();
		//Time.Sleep(2000);
		//Time.SleepDoEvents(2000);
		//Paste("text");
		//Key("one", 3000, "two");
		//Key("one");
		//Opt.Mouse.MoveSpeed = 100;
		//Mouse.Click(Wnd.Active, 100, 100);
		Perf.NW();
	}

	static void TestKeybClipbMouseOwnWindow()
	{
		var f = new Form();
		var t = new TextBox();
		f.Controls.Add(t);
		//f.Click += (unu, sed) => _TestKeybClipbMouseOwnWindow();
		Timer_.After(500, () => _TestKeybClipbMouseOwnWindow());
		f.ShowDialog();
	}

	static void TestAuDialogMessaging()
	{
		var d = new AuDialog("text1", "text2");
		d.HelpF1 += e =>
		{
			e.dialog.Send.ChangeText2("moo", false);
			//e.dialog.Send.Close();
		};
		d.ShowDialog();
	}

	static unsafe void _WriteToQM2(string s)
	{
		if(!_hwndQM2.IsAlive) {
			_hwndQM2 = Api.FindWindow("QM_Editor", null);
			if(_hwndQM2.Is0) return;
		}
		//_hwndQM2.SendS(Api.WM_SETTEXT, -1, s);
		//_hwndQM2.Send(Api.WM_APP+88, -1, 0);
		//_hwndQM2.SendS(Api.WM_APP+88, -1, s);

		fixed (char* p = s) {
			Api.COPYDATASTRUCT d = default;
			d.cbData = (s.Length + 1) * 2;
			d.lpData = p;
			_hwndQM2.Send(Api.WM_COPYDATA, 72598, &d);
		}
	}
	static Wnd _hwndQM2;

	static unsafe void _AddMessage(ConcurrentQueue<Au.Util.OutputServer.Message> q)
	{
		char c = '\0';
		var m = new Au.Util.OutputServer.Message(Au.Util.OutputServer.MessageType.Write, new string(&c), 0, null);
	}

	static void TestOutputServer()
	{

		//var q = new ConcurrentQueue<Au.Util.OutputServer.Message>();
		//_AddMessage(q);
		//var m1 = GC.GetTotalMemory(false);
		//int n = 10000;
		//for(int i = 0; i < n; i++) {
		//	_AddMessage(q);
		//}
		//var m2 = GC.GetTotalMemory(false);
		//Print((m2 - m1) / n);
		//return;

		//Output.IgnoreConsole = true;
		//100.ms();
		////Perf.SpinCPU(100);
		//for(int i1 = 0; i1 < 5; i1++) {
		//	Output.LogFile = Folders.Temp + "test_log.txt";
		//	int n2 = 100;
		//	Perf.First();
		//	for(int i2 = 0; i2 < n2; i2++) { Print("123456789."); }
		//	Perf.Next();
		//	Output.LogFile = null;
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { Print("123456789."); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { _WriteToQM2("123456789."); }
		//	Perf.Next();
		//	for(int i2 = 0; i2 < n2; i2++) { Console.WriteLine("123456789."); }
		//	Perf.Next();
		//	Perf.Write();
		//}
		//Console.ReadKey();

		Output.LibUseQM2 = true;
		Output.Clear();
		Output.LibUseQM2 = false;
		Print("warm");
		_WriteToQM2("warm");

		var s = "123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.";
		//s = new string('a', 11_0000);

		100.ms();
		Perf.Cpu(100);
		for(int i1 = 0; i1 < 1; i1++) {
			int n2 = 100;
			n2 = 1000000;
			n2 = 1000;
			Perf.First();
			//for(int i2 = 0; i2 < n2; i2++) { _WriteToQM2(s); }
			for(int i2 = 0; i2 < n2; i2++) { Print(s); }
			Perf.Next();
			Perf.Write();
			Output.LibUseQM2 = true;
			Print("OK", (double)Perf.TimeTotal / n2);
			Output.LibUseQM2 = false;
			1.ms();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static bool _TestLparamOperators(LPARAM a, LPARAM b)
	{
		return (a < b);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static LPARAM _TestLparamOperators2(LPARAM a, LPARAM b)
	{
		a += 1;
		return (a + (int)b);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void TestLparamOperators()
	{
		//Print(_TestLparamOperators(10, (long)int.MaxValue+1));
		Print(_TestLparamOperators2((long)int.MaxValue + 1, 10));

		//LPARAM k = (long)int.MaxValue + 1;
		//int i = k;
		//long g = k;
		//Print(i, g);
	}

	static void TestTimer()
	{
		var t = new Timer_(k => { Print(1); /*k.Stop();*/ k.Start(200, true); });
		t.Start(1000, false);
		AuDialog.Show();
	}

	static void TestWinImage1()
	{
		var f = Folders.Temp + "test.png";
		//WinImage.Capture(new RECT(693, 1578, 12, 12, true)).Save(f);

		var w = Wnd.Find("Programs");
		if(true) {
			//WinImage.Find(w, f).MouseMove();
			WinImage.Find(w, f, WIFlags.WindowDC).MouseMove();
			//WinImage.Wait(10, w, f).MouseMove();
		} else {
			var a = Acc.Find(w, "TREE");
			//WinImage.Find(a, f).MouseMove();
			WinImage.Find(a, f, WIFlags.WindowDC).MouseMove();
			//WinImage.Wait(10, w, f).MouseMove();
		}
	}

	static void TestShellRunResult()
	{
		//var r = Shell.Run(@"notepad.exe", flags: SRFlags.NeedProcessHandle);
		//using(var h = r.ProcessHandle) h?.WaitOne();
		//Print(r.ProcessId, r.ProcessExitCode);

		//Shell.Run("notepad.exe");
		//Wnd w = WaitFor.WindowActive(10, "*- Notepad", "Notepad");

		//Wnd w = Wnd.Find("*- Notepad", "Notepad");
		//if(w.Is0) { Shell.Run("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind); }
		//w.Activate();

		Wnd w = Wnd.FindOrRun("* Notepad", run: () => Shell.Run("notepad.exe"));

		//Wnd w = WaitFor.WindowActive(10, "*- Notepad", "Notepad", "pid=" + Shell.Run("notepad.exe"));
		//var r = Shell.Run("notepad.exe");
		//500.ms();
		////var w = Wnd.Find("*- Notepad", "Notepad", r);
		//Print("pid: " + r);
		//var w = Wnd.Find("*- Notepad", "Notepad", "pid=" + r);
		Print(w);
	}

	static void TestBase64()
	{
		//var a = new byte[] { 1, 2, 3, 4 };
		//var s = Convert.ToBase64String(a);
		//Print(Convert_.Base64Decode(s));

		//Print(Au.Controls.ImageUtil.ImageToString(@"Q:\Test\image.png"));
		//Print(Au.Controls.ImageUtil.ImageToString(@"Q:\app\il_qm.bmp"));

		var s = Au.Controls.ImageUtil.ImageToString(@"Q:\Test\image.png");
		WinImage.Find(new RECT(0, 0, 2000, 2000), s).MouseMove();
	}

	static void TestStringCompare()
	{
		string s1 = "123456789i123456789i123456789i123456789i123456789i";
		string s2 = "123456789i123456789i123456789i123456789i123456789j";
		string s3 = "23456789i123456789i123456789i123456789i123456789i";
		string s4 = "123456789i123456789i123456789i123456789i123456789";
		Print(s1.Equals(s2));
		Print(s1.Equals(s2, StringComparison.OrdinalIgnoreCase));
		//Print(string.Compare(s1, s2, StringComparison.Ordinal));
		//Print(string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase));
		Print(s1.Equals_(s2));
		Print(s1.Equals_(s2, true));
		//Print(s1.EqualsAt_(1, s3));
		//Print(s1.EqualsAt_(1, s3, true));
		//Print(s1.EndsWith_(s3));
		//Print(s1.EndsWith_(s3, true));
		//Print(s1.StartsWith_(s4));
		//Print(s1.StartsWith_(s4, true));
		Print(s1.IndexOfAny("ai".ToCharArray()));
		Print(s1.IndexOfAny(s_ca1));

		Perf.Cpu(100);
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 10000;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { s1.Equals(s2); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s1.Equals(s2); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.Equals(s2, StringComparison.OrdinalIgnoreCase); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { string.Compare(s1, s2, StringComparison.Ordinal); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s1.Equals_(s2); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s1.Equals_(s2, true); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.EqualsAt_(0, s2); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.EqualsAt_(0, s2, true); }
			Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.EndsWith_(s3); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.EndsWith_(s3, true); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.StartsWith_(s4); }
			//Perf.Next();
			//for(int i2 = 0; i2 < n2; i2++) { s1.StartsWith_(s4, true); }
			//Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s1.IndexOfAny("ai".ToCharArray()); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { s1.IndexOfAny(s_ca1); }
			Perf.Next();
			Perf.Write();
		}

	}
	static char[] s_ca1 = new char[] { 'a', 'i' };

	static void TestWaitFor()
	{
		//WaitFor.WindowVisible
		//Wnd.
		//new Acc.Finder().Wait
		//new Wnd.Finder().

		//Print(Wnd.Wait(10, true, "* Notepad"));
		//Wnd w = Wnd.WaitAny(10, true, new Wnd.Finder("* Notepad"), new Wnd.Finder("* Word"));
		//Print(w);

		//Print(Wnd.WaitNot(-5, "* Notepad"));

		//Wnd w = Wnd.FindOrRun("* Notepad", run: () => Shell.Run("notepad.exe"));
		//Print(w);

		//Wnd w = Wnd.Find("* Notepad");

		//w.WaitForCondition(30, t => t.IsActive);
		//Print("active");

		//w.WaitForCondition(0, t => t.IsMinimized || !t.IsAlive, true);
		//if(!w.IsAlive) { Print("closed"); return; }
		//Print("minimized");

		//var c = new Wnd.ChildFinder(className: "Edit");
		//w.WaitForCondition(10, t => c.Find(t) && c.Result.IsFocused);
		//Print("child focused");

		//Keyb.WaitForNoModifierKeys();
		//Keyb.WaitForNoModifierKeysAndMouseButtons();
		//Mouse.WaitForNoButtonsPressed();
		//WaitFor.Condition(0, () => Keyb.IsScrollLock);

		//Acc a;
		//a.WaitForState(0, o => !o.IsDisabled);
		//WaitFor.Condition(0, () => !a.IsDisabled);

		//Shell.Run("notepad.exe");
		//Wnd w = Wnd.Wait(10, true, "*- Notepad", "Notepad");

		//Wnd w = Wnd.Find("*- Notepad", "Notepad");
		//if(w.Is0) { Shell.Run("notepad.exe"); w = Wnd.WaitAny(60, true, Wnd.LastFind); }
		//w.Activate();

		//var s = @"Q:\Test\image.png";
		//var area =new WIArea(200, 0, 1000, 2000);
		////WinImage.Find(area, s).MouseMove();
		////WinImage.Wait(5, area, s).MouseMove();
		//Print(WinImage.WaitNot(-5, area, s));

		//Wnd w = Wnd.Find("Quick*");
		////w.WaitForCondition(10, t => t.Name.Like_("*Auto*"));
		//w.WaitForName(10, "*Auto*");
		////w.WaitForName(10, "**r (Auto*");

		//Print(Mouse.WaitForCursor(-10, MCursor.IBeam));

		//Au.Util.Cursor_.GetCurrentCursor(out var hcur);
		//var hash = Au.Util.Cursor_.HashCursor(hcur);
		////3.s();
		////Print(Mouse.WaitForCursor(-10, hash));
		//Print(Mouse.WaitForCursor(-10, hash, true));

		//Time.SleepDoEventsVar()

		//WaitFor.Condition(-0.1, () => false, (1,1));
		Perf.First();
		Perf.NW();
		Output.Clear();

		//var o = new OptWaitFor();
		//o = true;
		//Print(o.Period);
		//return;

		//Timer_.Every(500, t => Print("timer"));
		//Opt.WaitFor.DoEvents = true;
		//Opt.WaitFor.Period = 25;
		//for(int i = 0; i < 1; i++) {
		//	Perf.First();
		//	//WaitFor.Condition(-15, () => { Perf.NW(); Perf.First(); return false; });
		//	WaitFor.Condition(-2.5, () => { Perf.NW(); Perf.First(); return false; }, 20);
		//}

		//WaitFor.Condition(-0.1, () => false, (1,1));
		//Perf.First();
		//Perf.NW();
		//Output.Clear();

		//WaitFor.OptionFasterResponse = true;
		//for(int i = 0; i < 1; i++) {
		//	Perf.First();
		//	//WaitFor.Condition(-1.5, () => { Perf.NW(); Perf.First(); return false; });
		//	//WaitFor.Condition(-5.5, () => { Perf.NW(); Perf.First(); return false; }, (30, 100));
		//	//WaitFor.Condition(-5.5, () => { Perf.NW(); Perf.First(); return false; }, new OptWaitFor(30, 100));
		//	WaitFor.Condition(-0.5, () => { Perf.NW(); Perf.First(); return false; }, new OptWaitFor(30, 100));
		//}

		//var w = Wnd.Find("*Notepad");
		////var w = Wnd.Find("Registry Editor");
		//var w = Wnd.Find("Font");
		//////w.ShowMinimized();
		//////Api.SetForegroundWindow(w);
		//////Print(Wnd.Wait(30, true, "*Notepad"));
		////Print(Wnd.WaitAny(30, true, new Wnd.Finder("*Notepad")));

		////Print(w.WaitForCondition(10, t => !t.IsAlive, true));
		//Opt.WaitFor.DoEvents = true;
		//Timer_.Every(1000, t => Print("timer"));
		////Print(w.WaitForClosed());
		////Print(w.WaitForClosed(0, true));
		//Print(w.WaitForClosed(5, true));
		//Print(w.WaitForClosed(-5, true));

		var keys = "Ctrl F1  End \ta b #* End-=`~[{]}\\|;:'\",<.>/? cde";
		//var keys = "Ctrl F1  End \ta b #* Ctrl+(k)";
		//Print(Keyb.Misc.ParseKeysString(keys));
		//Print(Keyb.Misc.ParseKeyName("Enter"));
		//Print(Keyb.Misc.ParseKeyName("F5"));
		//Print(Keyb.Misc.ParseKeyName("F0x5"));
		//Print(Keyb.Misc.ParseKeyName("F 5"));
		//Print(Keyb.Misc.ParseKeyName("F5k"));
		//Print(Keyb.Misc.ParseKeyName("End"));
		//Print(Keyb.Misc.ParseKeyName("Ends"));
		//Print(Keyb.Misc.ParseKeyName("Ends5"));
		//Print(Keyb.Misc.ParseKeyName("End", 2, 1));
		//Print(Keyb.Misc.ParseKeyName("End", 3, 1));
		//Print(Keyb.Misc.ParseKeyName("End", 1, -1));
		//Print(Keyb.Misc.ParseHotkeyString("End", out var k), k);
		//Print(Keyb.Misc.ParseHotkeyString(" Shift + * ", out var k), k);


		//2.s();
		////Wnd.Misc.SwitchActiveWindow();
		//Wnd.Find("Quick*").Activate();

		//Print(Keyb.WaitForReleased(-5, KKey.Left));
		//Print(Keyb.WaitForReleased(-5, KKey.Shift, KKey.Escape));
		//Print(Keyb.WaitForReleased(-5, "Left"));
		//Print(Keyb.WaitForReleased(-5, "Left Shift"));
		//Print(Keyb.WaitForReleased(-5, "MouseLeft"));

		//Key("Ctrl+A Del", 100, "", "text", KKey.Enter);
		//Key("F5");

		//Key("Shift*down");
		//Key("a");
		//Mouse.Click();
		//Key("Shift*up");

		//Keyb.WaitForHotkey(0, "F11");
		//Keyb.WaitForHotkey(0, KKey.F11);
		//Keyb.WaitForHotkey(0, "Shift+A", true);
		//Keyb.WaitForHotkey(0, (KMod.Ctrl | KMod.Shift, KKey.P)); //Ctrl+Shift+P
		//Keyb.WaitForHotkey(0, Keys.Control | Keys.Alt | Keys.H); //Ctrl+Alt+H
		//Keyb.WaitForHotkey(5, "Ctrl+Win+K"); //exception after 5 s
		//if(!Keyb.WaitForHotkey(-5, "Left")) Print("timeout"); //returns false after 5 s

		//Print(Keyb.WaitForKey(-5));
		//Print(Keyb.WaitForKey(-5, true));
		//Print(Keyb.WaitForKey(-5, false, true));
		//Print(Keyb.WaitForKey(-5, true, true));
		//Print(Keyb.WaitForKey(-5, KKey.Ctrl));
		//Print(Keyb.WaitForKey(-5, KKey.RCtrl));
		//Print(Keyb.WaitForKey(-5, "Left"));
		//Print(Keyb.WaitForKey(-5, "Left", true));
		//Print(Keyb.WaitForKey(-5, "Left", true, true));
		//Print(Keyb.WaitForKey(-5, "Shift", true, true));

		//Mouse.WaitForClick(0, MouseButtons.Left, up: true, block: false);
		//Print("click");

		//Print(Mouse.WaitForClick(-5));
		//Print(Mouse.WaitForClick(-5, discard: true));
		//Print(Mouse.WaitForClick(-5, true, true));
		//Print(Mouse.WaitForClick(-5, 0));
		//Print(Mouse.WaitForClick(-5, MouseButtons.Left));
		//Print(Mouse.WaitForClick(-5, MouseButtons.Left | MouseButtons.Right));
		//Print(Mouse.WaitForClick(-5, MouseButtons.Left, true));
		//Print(Mouse.WaitForClick(-5, MouseButtons.Left, true, true));

		//Print(Keyb.WaitForHotkey(0, "#5"));
		//Print(Keyb.WaitForHotkey(0, 5));
		//Print(Keyb.IsShift);

		//Wnd.Find("Quick*").Activate();
		//Print(CopyText());
		//Paste("test");

		//bool stop = false;
		//Task.Run(() => { 2.s(); Print("task"); stop = true; });
		//WaitFor.Variable(5, ref stop, options: 1);
		//Print(stop);

		//bool stop = false;
		//Timer_.After(2000, t => { Print("timer"); stop = true; });
		//WaitFor.MessagesAndCondition(5, () => stop);
		//Print(stop);

		//Timer_.After(2000, t => { Print("timer"); });
		//WaitFor.PostedMessage(5, (ref Native.MSG m) => { Print(m); return m.message == 0x113; }); //WM_TIMER
		//Print("finished");

		//using(var b = new BlockUserInput(BIEvents.All)) {
		//	b.ResendBlockedKeys = true;
		//	5.s();
		//}


		//var e1 = Api.CreateEvent(default, false, false, null);
		//var e2 = Api.CreateEvent(default, false, false, null);
		//////Timer_.After(1000, t => { Print("timer"); Api.SetEvent(e1); Api.SetEvent(e2); });
		////Timer_.After(1000, t => { Print("timer"); Api.SetEvent(e2); });
		//////Task.Run(()=> { 2000.ms(); Print("task"); Api.SetEvent(e1); });
		////var f = WFHFlags.All | WFHFlags.DoEvents;
		//////Print(Time.LibWait(3000, e1, e2));
		//////Print(Time.LibWait(3000, f, e1, e2));
		////Print(WaitFor.Handles(-3, f, e1, e2));

		//bool stop = false, stop2=false;
		////Timer_.After(1000, t => { Print("timer"); stop = true; });
		//Timer_.After(1000, t => { Print("timer"); stop2 = true; });
		////Print(Time.LibWait(3000, WFHFlags.DoEvents, (ref Native.MSG m)=> { Print(m); return true; }, ref stop));
		//Print(Time.LibWait(3000, WFHFlags.DoEvents| WFHFlags.CallbackAfter, (ref Native.MSG m)=> { Print(m); return stop2; }, ref stop));

		//bool stop = false;
		//var f = new Form();
		//f.KeyUp += (unu, sed) =>
		//{
		//	Print("start");

		//	////Timer_.After(1000, t => { Print("timer"); Api.SetEvent(e1); Api.SetEvent(e2); });
		//	Timer_.After(1000, t => { Print("timer"); Api.SetEvent(e2); stop = true; });
		//	Task.Run(()=> { 2000.ms(); Print("task"); Api.SetEvent(e1); });
		//	//Task.Run(()=> { 2000.ms(); Print("task"); Api.SetEvent(e2); Api.SetEvent(e1); });

		//	//Time.LibWait(3000, WFHFlags.DoEvents);
		//	//WaitFor.Handles(-3, 0);
		//	//WaitFor.Handles(-3, WFHFlags.DoEvents);
		//	//Print(WaitFor.Handles(-4, 0, e1, e2));
		//	//Print(WaitFor.Handles(-4, WFHFlags.DoEvents, e1, e2));
		//	//Print(WaitFor.Handles(-4, WFHFlags.DoEvents | WFHFlags.All, e1, e2));
		//	//Print(WaitFor.Handles(-4, WFHFlags.All, e1, e2));
		//	//Thread.CurrentThread.Join(3000);
		//	//Time.SleepDoEvents(3000);
		//	Time.SleepDoEventsVar(3000, ref stop);
		//	Print("end");
		//};
		//f.ShowDialog();

		Print("ok");
	}

	static void TestRegisterHotkey()
	{
		var f = new FormRegisterHotkey();
		f.ShowDialog();
	}

	class FormRegisterHotkey :Form
	{
		RegisterHotkey _hk1, _hk2;

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_CREATE: //0x1
				bool r1 = _hk2.Register(1, "Ctrl+Alt+F10", this);
				bool r2 = _hk1.Register(2, (KMod.Ctrl | KMod.Shift, KKey.D), this); //Ctrl+Shift+D
				Print(r1, r2);
				break;
			case Api.WM_DESTROY: //0x2
				_hk1.Unregister();
				_hk2.Unregister();
				break;
			case RegisterHotkey.WM_HOTKEY:
				Print(m.WParam);
				break;
			}
			base.WndProc(ref m);
		}
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

	//public static bool WaitForKey(double secondsTimeout, KKey key)
	//{
	//	bool ok = false;
	//	using(WinHook.Keyboard(x => ok = x.IsKey(key))) {
	//		return WaitFor.MessagesAndCondition(secondsTimeout, () => ok);
	//	}
	//}

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

	static void TestWaitWithHook()
	{

	}

	static void TestWndChild()
	{
		//var w = Wnd.Find("Quick*");

		//var s = "***tid 88";
		//Print(Au.Util.StringMisc.ParseParam3Stars(ref s, "pid", "tid", "owner"), s);

		//Print(w.Child("Open items"));
		//Print(w.Child("***id 2214"));
		//w = Wnd.Find("Options", programEtc: WFEtc.Owner(w));
		////Print(w.Child("***accName Run as", "combobox"));
		//Print(w.Child("moo"));
		//Print(w.Child("***text moo"));
		////Print(w.Child("***bad bad"));

		//w = Wnd.Find("Options").Child("***id " + 11030).OrThrow();

		var w = Wnd.Find("Quick*", programEtc: "qm.exe");
		//Print(w.ProgramName, w.ProgramFilePath);
		//Print(Process_.GetProcessIds("notepad.exe"));
		//Print(Process_.AllProcesses(true));

		////int pid = Process_.GetProcessIds("winlogon.exe")[0];
		//int pid = Process_.GetProcessIds("qmserv.exe")[0];
		////pid = Process_.GetProcessId("qm.exe");
		//Print(pid);
		//Print(Process_.GetName(pid, true));
		//Print(Process_.GetName(pid, false));
		//Print(Process_.GetName(pid, false, true));

		//Print(Process_.GetProcessId(@"q:\app\qm.exe", fullPath: true));
		//Print(Process_.GetProcessId("qmserv.exe", ofThisSession: true));

		//foreach(var v in Process_.AllProcesses()) Print(v.SessionId, v.ProcessId, v.Name);

		//Print(Process_.CurrentSessionId);

		//var a = Process.GetProcesses();
		//foreach(var p in a) {
		//	//Print(p.ProcessM)
		//	try { Print(p.ProcessName, p.MainModule.FileVersionInfo.FileDescription); } catch { }
		//}
		//Print(w.ProgramDescription);
		//Print(Process_.GetVersionInfo(w.ProcessId).CompanyName);

		w = w.ChildById(2052);
		Print(w);
	}

	static void TestScreen()
	{
		//Print(Screen_.PrimaryWidth, Screen_.PrimaryHeight, Screen_.PrimaryRect, Screen_.PrimaryWorkArea);
		//Print(Screen_.GetRect());
		//Print(Screen_.GetRect(1));
		//Print(Screen_.GetRect(2));
		//Print(Screen_.GetRect(Screen_.Primary));
		//Print(Screen_.GetRect(Screen_.OfActiveWindow));
		//Print(Screen_.GetRect(Screen_.OfMouse));

		var w = Wnd.Find("*Notepad");
		//w.MoveToScreenCenter();
		//w.MoveInScreen(Coord.Reverse(1), Coord.Reverse(1));
		//w.MoveInScreen(Coord.Center, Coord.Center);
		//w.EnsureInScreen();
		//w.EnsureInScreen(Screen_.OfMouse);
		//w.MoveInScreen(default, default);
		//w.MoveInScreen(Coord.Center, Coord.Center);
		//w.MoveToScreenCenter(1);

		//AuDialog.Options.DefaultScreen = Screen.PrimaryScreen;
		//AuDialog.Options.DefaultScreen = 1;
		//var s = "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\nWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW\n";
		//AuDialog.Show(s);
		//AuDialog.ShowEx(s, x: 10, y: -10);
		//AuDialog.ShowEx(s, x: 0, y: 0);
		//AuDialog.ShowEx(s, x: 10, y: Coord.Reverse(10));
		//AuDialog.ShowEx(null, s, x: Coord.Reverse(10));
		//AuDialog.ShowEx(null, s, x: Coord.Fraction(0.3));
		//var d = new AuDialog(s);
		//d.Screen = Screen_.Primary;
		//d.Screen = Screen_.OfMouse;
		//d.Screen = Screen_.OfActiveWindow;
		//d.Screen = 1;
		//d.Screen = 2;
		//d.Screen = w;
		//d.Screen = Acc.Find(w, "TEXT");
		//d.Screen = new POINT(1000, 1500);
		//d.Screen = new RECT(1000, 1500, 10, 10, true);
		//d.Screen =Screen.PrimaryScreen;
		//d.Screen = Screen.AllScreens[1];
		//d.ShowDialog();

		//Print(Wnd.FromXY(10, 10));
		//Print(Wnd.FromXY(10, 10, co: new CoordOptions(false, 1)));
		//Print(w.ContainsScreenXY(10, 10));
		//Print(w.ContainsScreenXY(10, 10, new CoordOptions(false, 1)));
		//w.Move(10, 10);
		//w.Move(10, 10, new CoordOptions(false, 1));
		//w.Move(10, 10, 500, 400);
		//w.Move(10, 10, 500, 400, new CoordOptions(false, 1));
		//w.Move(10, Coord.Reverse(0));
		//w.Move(10, Coord.Reverse(200), new CoordOptions(false, 1));
		//w.Resize(Coord.Reverse(40), Coord.Reverse(40));
		//w.Resize(Coord.Reverse(20), Coord.Reverse(20), new CoordOptions(false, 0));

		//var r = new RECT(0, 0, 100, 100, true);
		////r.MoveInScreen(10, 10, 1);
		//r.left -= 100; r.EnsureInScreen(1);
		//Print(r);

		//Mouse.Move(10, 10);
		//Mouse.Move(10, 10, new CoordOptions(false, 1));

		//var k = new System.Windows.Window();
		////Timer_.Every(1000, t => { Screen_ j = k; Print(j.GetScreen()); });
		//Timer_.After(1000, t => { AnyWnd j = k; Print(j.Wnd); });
		//k.ShowDialog();

		//Wnd.Lib.WinFlags.Set(w, (Wnd.Lib.WFlags)1);
		//Print(Wnd.Lib.WinFlags.Get(w));
	}

	static void TestLibIsPossiblyDos()
	{
		//Print(Path_.LibIsPossiblyDos("file.txt"));
		//Print(Path_.LibIsPossiblyDos("~file.txt"));
		//Print(Path_.LibIsPossiblyDos("file~.txt"));
		//Print(Path_.LibIsPossiblyDos("file~8.txt"));
		//Print(Path_.LibIsPossiblyDos("file12~8.txt"));
		//Print(Path_.LibIsPossiblyDos("file1~12.txt"));
		//Print(Path_.LibIsPossiblyDos("file~123.txt"));
		//Print(Path_.LibIsPossiblyDos("fil~1234.txt"));
		//Print(Path_.LibIsPossiblyDos("/i~12345.txt"));
		//Print(Path_.LibIsPossiblyDos("f~123456.txt"));
		//Print(Path_.LibIsPossiblyDos("f~1234567.txt"));
		//Print(Path_.LibIsPossiblyDos("file12~8.txtt"));
		//Print(Path_.LibIsPossiblyDos("file~123"));
		//Print(Path_.LibIsPossiblyDos("ile~123"));
		//Print(Path_.LibIsPossiblyDos("ffile~123.txt"));
		//Print(Path_.LibIsPossiblyDos(@"c:\file~123"));
		//Print(Path_.LibIsPossiblyDos(@"c:\ile~123"));
		//Print(Path_.LibIsPossiblyDos(@"c:\ffile~123"));
		//Print(Path_.LibIsPossiblyDos(@"c:\file~123\more"));
		//Print(Path_.LibIsPossiblyDos(@"c:\ile~123\more"));
		//Print(Path_.LibIsPossiblyDos(@"c:\ffile~123\more"));

		Print(Path_.Normalize(@"c:\progra~1"));
	}

	static void TestConvert()
	{
		//var b = new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t', };
		//var s1 = Convert.ToBase64String(b);
		//var s2 = Convert_.HexEncode(b);
		//Print(s1, s2);

		//Print(Convert_.HexDecode(s2));
		//Print(Convert_.Base64Decode(s1));

		//b = File.ReadAllBytes(@"Q:\app\Au\Tests\App.config");
	}

	static void TestMultiStringAndColor()
	{
		//Print(AuDialog.ShowList("One|Two|Three"));
		//Print(AuDialog.ShowList(new string[] { "A", "One|Two|Three", "C" }));
		//Print(AuDialog.ShowList(new List<string> { "A", "One|Two|Three", "C" }));

		//var d = new AuDialog("test");
		////d.SetButtons("-1 OK|-2 Cancel");
		////d.SetButtons("-1 OK|-2 Cancel", false, "One|Two");
		//d.SetButtons("-1 OK|-2 Cancel", false, new string[] { "A", "One|Two|Three", "C" });
		//Print(d.ShowDialog());

		//ColorInt c = 0xff00;
		//var c1 = (Color)c; Print(c1);
		//var c2 = (System.Windows.Media.Color)c; Print(c2);
		//c = c1;
		//Print(c);
		//c = c2;
		//Print(c);
		//Print(c == 0xFF00ff00, c == 0xFF00ff01, c==0xff00, c==0xff01);
		////c= 0xff;
		//Print(c == c1, c== c2);
		//c.color &= 0xffffff;
		//Print(c);
		//c = 0x10000ff; Print(c);
	}

	static void TestPasteBluestacks()
	{
		//var d = new Clipb.Data(); d.AddText("moo").SetClipboard();
		Wnd.Find("BlueStacks").Activate();
		//Wnd.Find("*Edge").Activate();
		//Wnd.Find("*Notepad").Activate();
		//Wnd.Find("*VMware*").Activate(); Mouse.Click(1079, 445);
		//Print(Wnd.Active);
		//for(int i = 0; i < 12; i++) {
		//	Task.Run(() => Perf.Cpu(500));
		//}
		100.ms();
		//Print(Wnd.Focused);
		Key("Back*20", 100);
		//Opt.Key.KeySpeed = 1000; Key("ab"); return;
#if true
		//Opt.Key.RestoreClipboard = false;
		//Opt.Key.KeySpeedClipboard = 100;
		Opt.Key.Hook = o => { if(o.w.ProgramName.Equals_("HD-Player.exe", true)) o.opt.KeySpeedClipboard = 100; };
		Paste("one ");
		Paste("two ");
		Paste("three ");
		//1000.ms();
		//var d = new Clipb.Data(); d.AddText("OLD").SetClipboard();
		//Key("Ctrl+A"); Print(CopyText());
#else
		var d = new Clipb.Data(); d.AddText("moo").SetClipboard();
		Key("Ctrl+V");
		//Key("Ctrl+", 260, "V");
		//Key("Ctrl+V*down", 700, "V*up");
#endif
		//Text("Text");
		Print("ok");
	}

	static void TestWinImageCapture()
	{
		//var w = Wnd.Find("*Chrome");
		//var b = WinImage.Capture(w, w.Rect);

		//var b = WinImage.Capture(new RECT(0, 0, 1, 1, true));
		//_ShowImage(b);

		WICFlags f = 0;
		//f|=WICFlags.WindowDC;
		//f |= WICFlags.Color;
		//f |= WICFlags.Image;
		f |= WICFlags.Rectangle;
		if(!WinImage.CaptureUI(out var r, f)) return;
		Print(r.rect, r.color, r.wnd);
		_ShowImage(r.image);

		//var f = new Form();
		////f.SizeGripStyle = SizeGripStyle.Hide;
		////f.FormBorderStyle = FormBorderStyle.FixedDialog;
		//f.StartPosition = FormStartPosition.Manual;
		//var r= SystemInformation.VirtualScreen;
		//f.Top = r.Top;
		//f.Left = r.Left;
		//f.ShowDialog();

		//var f = new Form();
		//var p = new PictureBox();
		//p.ImageLocation = @"Q:\My QM\C8375087-NET.png";
		//p.SizeMode = PictureBoxSizeMode.AutoSize;
		//f.Controls.Add(p);
		//p.MouseDown += (unu, sed) =>
		//  {
		//	  var a = new List<POINT>() { Mouse.XY };
		//	  if(!Au.Util.DragDrop.SimpleDragDrop(p, MButtons.Left, m =>
		//	  {
		//		  //Print(m.Msg.pt);
		//		  a.Add(m.Msg.pt);
		//	  })) return;

		//	  //a = new List<POINT>() { (0,0),(2,0),(2,2),(0,2) };

		//	  //var w =(Wnd)f;
		//	  //w.Owner = Wnd.GetWnd.Root;
		//	  //Print(Wnd.Misc.OwnerWindowsAndThis(w)); return;

		//	  var b = WinImage.Capture(a);
		//	  _ShowImage(b);
		//  };
		//f.ShowDialog();
	}

	static void _ShowImage(Bitmap b)
	{
		if(b == null) return;
		string file = Folders.Temp + "WinImage.png";
		b.Save(file);
		Shell.Run(file);
	}

	static void TestLoadCursor()
	{
		Print(Api.GetSystemMetrics(Api.SM_CXCURSOR));
		var f = new Form();
		var s = @"C:\WINDOWS\Cursors\aero_busy.ani";
		//var s = @"C:\WINDOWS\Cursors\aero_busy_xl.ani";
		s = @"Q:\app\IDC_CROSS_RED.cur";
		//f.Cursor = Au.Util.Cursor_.LoadCursorFromFile(s);
		f.Cursor = Au.Util.Cursor_.LoadCursorFromMemory(File.ReadAllBytes(s));
		//f.Cursor = new Cursor(s);
		f.ShowDialog();
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

	static void _TestIconMemory()
	{
		//var k = Icon_.GetStockIcon(StockIcon.DESKTOPPC, 32);
		var k = new _TestHookFinalizer();
		//GCInterceptor.RegisterGCEvent(k, hash => Print(hash), k);
		Au.Util.GC_.AddObjectMemoryPressure(k, 100);
		k.Dispose();
	}

	static void TestIconMemory()
	{
		//_TestIconMemory();
		//Timer_.After(1000, () =>
		//{
		//	Print("collect");
		//	GC.Collect();
		//	GC.WaitForPendingFinalizers();
		//	Print("collected");
		//});

		//AuDialog.Show();

		for(int i = 0; i < 400; i++) {
			if((i % 10) == 0) {
				//1.ms();
			}
			if((i % 10) == 0) {
				Print(i, Debug_.LibMemoryGet(),
					GetGuiResources(Process_.CurrentProcessHandle, 1),
					GetGuiResources(Process_.CurrentProcessHandle, 4),
					GetGuiResources(Process_.CurrentProcessHandle, 0),
					GetGuiResources(Process_.CurrentProcessHandle, 2)
					);
			}
			int size = 32;
			//Perf.First();
			//var k = Icon_.GetStockIcon(StockIcon.DESKTOPPC, size);
			var k = Icon_.GetFileIcon(@"q:\app\qm.exe", size);
			//var k = Icon_.GetFileIcon(@"q:\app\macro.ico", size);
			//Perf.Next();
			//size/=2;
			//Au.Util.GCMemoryPressure.Add(k, 1000+ size * size);
			//Perf.NW();
			if(k == null) throw new AuException(i.ToString() + ",  " + Native.GetErrorMessage());
		}
	}
	[DllImport("user32.dll")]
	internal static extern int GetGuiResources(IntPtr hProcess, uint uiFlags);

	static void TestIconCacheEtc()
	{
		//var sf = @"q:\app\app.cpp";
		var sf = @"Q:\My QM\icon.exe";
		var c = new Icon_.ImageCache(Folders.Temp + "Au.icon.cache", 16);
		//c.ClearCache();
		//Print(k);

		var f = new Form();
		f.BackgroundImageLayout = ImageLayout.None;

		var k = c.GetImage(sf, true, 0, (im, ob) => { Print(im, ob); f.BackgroundImage = im; }, 5);
		//var k = c.GetImage("DESKTOPPC", () => Icon_.GetStockIconHandle(StockIcon.DESKTOPPC, 16));
		if(k == null) return;

		f.BackgroundImage = k;
		f.ShowDialog();

		//var so = Folders.Temp + "Au.icon.png";
		//k.Save(so);
		//Shell.Run(so);
	}

	static void TestOsd()
	{
		//Print(Thread_.IsUI);
		//Timer_.After(1000, () => Print(Thread_.IsUI));
		//var f = new Form();
		//f.ShowDialog();
		////var w = new System.Windows.Window();
		////w.ShowDialog();

		//Print(Thread_.NativeId);
		_TestOsd();
		AuDialog.Show("Test OSD");

		//var f = new Form();
		//f.Click += (unu, sed) => _TestOsd();
		//f.ShowDialog();

		//var r = new OnScreenRect();
		//r.Color = Color.Blue;
		//r.Rect = (300, 300, 100, 100);
		//for(int i = 0; i < 10; i++) {
		//	r.Show((i & 1) == 0);
		//	100.ms();
		//}

		//TestOnScreenRect(); return;

		//var r = new OsdRect();
		//r.Color = Color.Blue;
		////r.Opacity = 0.3;
		////r.Thickness = 8;
		//r.Rect = (300, 300, 100, 100);
		//for(int i = 0; i < 10; i++) {
		//	r.Visible = !r.Visible;
		//	200.ms();
		//}

		//var k = new Osd();
		//k.Text = "Test";
		//k.SecondsTimeout = 5;
		//k.ShowMode = OsdShowMode.Wait;
		//k.ClickToClose = true;
		//k.Show();
		//1.s();
		//k.Text = "Moo";
		//k.Show();
	}
	//static int s_uytr;

	static void _TestOsd()
	{
		string s = null;
		s = "W Test OSD JjW";
		//s = "&OSD qwertyuiop\nasdfghjkl\nzxcvbnm WWWW WWWWWW WWWWWWWW WWWWWWWW WWWWWWW WWWWWW WWWWWWW WWWWWW WWWWWWWWWW";
		//s = "&OSD qwertyuiop WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW qwertyuiop WWWWWWWWWWWWWWWWWWWWWW WWWWWWWWWWWWWWWWWWWWWW asdfghjkl zxcvbnm A B C D E F G H I J K L M N O P Q R S T U V W Z X Y mmmm nnnn";
		//s = "one\n\ttwo";

		//Osd.DefaultScreen = 1;
		var w = Wnd.Find("*Notepad");
#if false

		var o = new Osd();
		o.Text = s;
		o.ShowMode = OsdShowMode.ThisThread;

		o.SecondsTimeout = 30;
		//o.TransparentColor = 0xff00ff;
		//o.Opacity = 0.8;
		//o.BorderColor = o.BackColor;
		o.ClickToClose = true;
		//o.Shadow = true;
		//o.Rect = (100, 100, 200, 400);
		//o.Y = Coord.Fraction(0.9);
		//o.XY = Mouse.XY;
		//o.XY = (Coord.Center, Coord.Fraction(0.4));
		//o.XY = (Coord.Center, Coord.Center, 1);
		//o.XY = (default, default, 1);
		//o.XY = true;
		//o.XY = (Screen_)1;
		//o.XY = (Screen_)Screen.AllScreens[1];
		//o.XY = (1, true);
		//o.XY = Mouse.XY + (0, 30);
		//o.XY = PopupXY.Mouse;
		//o.XY = (Coord.Fraction(0.9), Coord.Reverse(0));

		//o.XY = w.Rect;
		//o.XY = (Screen_)w.Rect;
		//o.XY = (w.Rect, 0, 0);
		//o.XY = (w.Rect, default, 0);
		//o.XY = (w.Rect, Coord.Reverse(0), Coord.Reverse(0));
		//o.XY = (w.Rect, Coord.Fraction(-1), Coord.Fraction(-1));

		//o.ForeColor = 0x80;
		//o.WrapWidth = 200;
		//o.Icon = SystemIcons.Information;
		//o.BackgroundImage = Image.FromFile(@"Q:\My QM\C8375087-NET.png");
		//o.IsOfImageSize = true;
		//o.WrapWidth = 80;

		o.Show();
		//o.Visible = true;
		//o.X = 400;
		//Timer_.After(4000, ()=>o.Visible = true);

		//Timer_.Every(1000, () => Print(o.Handle));

		o.ResizeWhenContentChanged = true;
		Timer_.After(1000, () =>
		{
			o.Text = "New text";
			//o.Text = "WW Test OSD Jj";
			//o.XY=(100, 200);
			//o.Rect=(100, 100, 100, 100);

			//o.TextColor = Color.Red;
			//o.BackColor = Color.Orange;
			//o.BorderColor = Color.Blue;

		});

		//var k = new OsdRect() { Rect = (300, 300, 100, 100) };
		//k.Show();
		////Timer_.After(1000, () => k.Rect = (300, 300, 150, 150));
		//Timer_.After(1000, () => k.Rect = (300, 300, 50, 50));
#else
		//Osd.DefaultFont = new Font("Comic Sans MS", 12);
		//Osd.DefaultBackColor=Color.Wheat;
		//Osd.DefaultBorderColor = Color.Tomato;
		//Osd.DefaultTextColor = Color.IndianRed;
		//Osd.DefaultTextFormatFlags |= TextFormatFlags.HidePrefix|  TextFormatFlags.EndEllipsis;
		//Osd.DefaultTransparentTextColor=Color.Green;
		//Osd.DefaultTransparentTextFont=new Font("Comic Sans MS", 24);

		//Osd.ShowTransparentText(s, 30);
		//Osd.ShowTransparentText(s, 30, color: Color.Green);
		//Osd.ShowTransparentText(s, 30, xy: (Screen_)w);
		//Osd.ShowTransparentText(s, 30, xy: w);

		//Osd.ShowText(s, 30);
		//Osd.ShowText(s, 30, textColor: Color.Honeydew, backColor: Color.DarkBlue);
		//Osd.ShowText("Test OSD", 30, PopupXY.Mouse, SystemIcons.Information);
		//Osd.ShowText(s, 30, icon: SystemIcons.Information);
		//Osd.ShowText("Test OSD", 30, icon: Icon_.GetStockIcon(StockIcon.HELP, 16));
		//Osd.ShowText("Test OSD", 30, icon: Icon_.GetAppIcon(16));
		//Osd.ShowText("Test OSD", 30, icon: Icon_.LoadIcon(@"q:\app\qm.exe", 1, 16));
		//Osd.ShowText("Test OSD", 30, icon: Icon_.GetFileIcon(@"q:\app\qm.exe,1", 16));
		//Osd.ShowText("Test OSD", 30, icon: Icon_.GetPidlIcon(Folders.VirtualPidl.AddNewPrograms, 16));

		//var ico = Icon_.CreateIcon(32, 32);
		//var ic = Icon_.CreateIcon(32, 32, g =>
		//{
		//	g.Clear(Color.Bisque);
		//	g.SmoothingMode = SmoothingMode.HighQuality;
		//	g.DrawEllipse(Pens.Blue, 1, 1, 30, 30);
		//});
		//Osd.ShowText("Test OSD", 30, icon: ic);

		//var im = Image.FromFile(@"Q:\My QM\C8375087-NET.png");
		////Print(im);
		//Osd.ShowImage(im, 60);
		////Osd.ShowImage(im, 60, transparentColor: 0xF5F8FC);

		//var o= Osd.ShowImage(im, 60, doNotShow: true);
		//o.ClickToHide = false;
		//o.Show();

		//var o= Osd.ShowText(s, 30, doNotShow: true);
		//o.Opacity = 0.5;
		//o.ClickToClose = false;
		//o.Show();

		//Timer_.After(3000, () => Osd.CloseAll());

		//var m = new Osd { Text = "Text", ShowMode = OsdShowMode.StrongThread };
		//m.XY = (Coord.Center, Coord.Max); //bottom-center of the work area of the primary screen
		//m.Show();
#endif

		//var r = new OsdRect();
		//r.


		//o.SecondsTimeout = 2;
		//o.ShowWait();
		//o.Dispose();
		//o.Shadow = false;
		//o.ShowWait();

		//o.SecondsTimeout = 1;
		//for(int i = 0; i < 4; i++) {
		//	o.ShowWait();
		//	200.ms();
		//}

		//POINT p = (Coord.Center, default, true);
		//p = (default, default, default, true);
		////RECT r = (100, 100, 100, 100);
		////p = (r, default, default);
		//Print(p);

		//Osd.ShowText("Test OSD", 5, showMode: OsdShowMode.StrongThread);
		//Osd.ShowText("Test OSD", 5, showMode: OsdShowMode.WeakThread);
		//Osd.ShowText("Test OSD", 5, showMode: OsdShowMode.Wait);
		//Osd.ShowText("Test OSD", 5, showMode: OsdShowMode.ThisThread);
		//AuDialog.Show();
	}

#if TOOLS
	static void TestToolWinImage()
	{
		using(var f = new Au.Tools.Form_WinImage()) f.ShowDialog();
	}

#endif
	static void TestToolWinImageCode()
	{
#if true
		//var w = Wnd.Find("Quick*");


		//WinImage.Find(w, @"q:\test\function icon.bmp").MouseMove();
		//var r = WinImage.Find(w, @"q:\test\function icon.bmp");
		//Print(r);

		//var a = new List<WinImage>();
		//var img = @"q:\test\function icon.bmp";
		////var img = new string[] { @"q:\test\function icon.bmp", @"q:\test\autotext icon.bmp"};
		//WinImage.Find(w, img, also: o => { a.Add(o); return false; });
		////Print("----");
		////Print(a);
		//foreach(var k in a) { Print(k.MatchIndex, k.ListIndex); k.MouseMove(); 0.3.s(); }

		//Print("ok");

		var w = Wnd.Find("Au - Microsoft Visual Studio ", "HwndWrapper[DefaultDomain;*").OrThrow();
		object[] images = {
			"image:iVBORw0KGgoAAAANSUhEUgAAABwAAAAQCAYAAAAFzx/vAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADqSURBVEhLY/hPZzD4Lfz37x+URR4gyUKYZZP2Tvhv1W8Kx/OOzAGLg9jINDaAYuHVV1//+2x5/l916eP/Hpue/T/7/DNUBgFglgXN9QNbBOJvv7wVbjkyxgZQLHTd+Oz/vCvv/n/6/vP/kmvv/1uuffr/77+/UFkIABnkNs3p/9evX6AiCACzBJdlIEByHIIMy1uRDeUhAEgcHWMDKBYqLnmCgdEByCCq+RBmyY8fP7Ba+OfPH7DvQAYix+Hq0yuhKggDkiwEAZDPYJbCMMhCYrMLyUGKDYB8TiwgOdFQCkYtpDqgs4X//wMAud2NhZbZMbsAAAAASUVORK5CYII=",
			"image:iVBORw0KGgoAAAANSUhEUgAAAA0AAAALCAYAAACksgdhAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABiSURBVChTY/iPBD6/vv//5bVd/3/8+IEV39vZBFbHcH9/939k/O3rV6waQBiuCZskLjwAmkAMfPjr54+ka3p6aiHpmkD448v7pGu6v7eLdE0g/OrWwf+Pjs2AaAKTJIH//wEF2/8fchW91wAAAABJRU5ErkJggg==",
		};
		var all = new List<WinImage>();
		//all.Add(WinImage.Find(w, images));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.FindOther; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.OkFindMore; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.FindOtherOfList; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.OkFindMoreOfList; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.FindOtherOfThis; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.OkFindMoreOfThis; }));
		//bool found = false; Print(WinImage.Find(w, images, also: t => { all.Add(t); if(!found) { found = true; return WIAlso.OkFindMoreOfThis; } return WIAlso.FindOtherOfThis; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.OkReturn; }));
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return WIAlso.NotFound; }));

		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.FindOther; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.OkFindMore; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.FindOtherOfList; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.OkFindMoreOfList; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.FindOtherOfThis; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.OkFindMoreOfThis; }));
		//bool found = false; Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); if(!found) { found = true; return WIAlso.OkFindMoreOfThis; } return WIAlso.FindOtherOfThis; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.OkReturn; }));
		//Print(WinImage.Wait(-1, w, images, also: t => { all.Add(t); return WIAlso.NotFound; }));

		//var found = new BitArray(images.Length); WinImage.Find(w, images, also: t => { found[t.ListIndex] = true; return WIAlso.OkFindMoreOfList; }); if(found[0]) Print(0); if(found[1]) Print(1);
		//Print(WinImage.Find(w, images, also: t => { all.Add(t); return true ? WIAlso.OkReturn : WIAlso.FindOther; }));
		Print(WinImage.Find(w, images, also: o => { all.Add(o); return o.Skip(1); }));
		Print("---");
		foreach(var wi in all) { Print(wi); }

#else
		//var w = Wnd.Find("Au - Microsoft Visual Studio ", "HwndWrapper[DefaultDomain;*").OrThrow();
		//string image = "image:iVBORw0KGgoAAAANSUhEUgAAABYAAAANCAYAAACtpZ5jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADZSURBVDhPY/hPIzACDP737x+UhR+QZDDM0El7J/y36jeF43lH5oDFQWwYjWLw1Vdf//tsef5fdenj/x6bnv0/+/wzVAYBYIYGzfUDGwjib7+8FW4JDKMY7Lrx2f95V979//T95/8l197/t1z79P/ff3+hshAA0uQ2zen/169foCIIAJKD0SSHMUhT3opsKA8BQOLIGMVgxSVPMDA6AGki2cUww378+IHV4D9//oBdC9KIHMarT6+EqkAAkgwGAZBLYYbDMMhg9GRIclBgAyCfoAOSI4848P8/AFdhC+1vmMLqAAAAAElFTkSuQmCC";
		//var wi = WinImage.Find(w, image).OrThrow();
		//wi.MouseMove();

		//var w = Wnd.Find("Quick Macros - ok - [Macro10]", "QM_Editor").OrThrow();
		//string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAAoAAAANCAYAAACQN/8FAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACDSURBVChTY/j//8B/YjCKQo81ViiSyBhDoewUbhQFMIyhcPGzPqyKURRaLdP5H3nNDatiFIUmi5XBCkHYaq8mimIUhSrzRP57HTcHK5JZyPRfZAoDGGMoFJ/DiqJIoB+iCEOh0EwGrIpAGEMhNkUgjKIQlyIQRlGISxEIoyjEjQ/8BwCB76UzQz6WoQAAAABJRU5ErkJggg==";
		//var wi = WinImage.Find(w, image).OrThrow();
		//using(wi.MouseClick(button: MButton.Left | MButton.Down)) Mouse.MoveRelative(0, 50);


#endif

	}

	static void TestThrowAndWait()
	{
		//Print(WaitFor.Condition(1, () => false));

	}

	static void TestAccFindParamNavig()
	{
		var w = Wnd.Find("Quick Macros - ok - [Macro10]", "QM_Editor").OrThrow();
		//var a = (Acc.Find(w, "BUTTON", "Copy*", "id=2053")?.Navigate("ne")).OrThrow();
		//var a = Acc.Find(w, "BUTTON", "Copy*", "id=2053", navig: "ne").OrThrow();
		//var a = Acc.Wait(2, w, "BUTTON", "Copy*", "id=2053");
		var a = Acc.Wait(2, w, "BUTTON", "Copy*", "id=2053", navig: "ne");
		Print(a);
	}

	static void TestWndFindContainsRoleName()
	{
		//var w = Wnd.Find("Options", contains: "Unicode");
		var w = Wnd.Find("Options", contains: "'CHECKBOX' Unicode");
		//var w = Wnd.Find("Options", contains: "'' Unicode");
		//var w = Wnd.Find("Options", contains: "'CHECKBOX' ");
		Print(w);
	}

	static void TestWFEtc()
	{
		//var w = Wnd.Find("Options", null, null);
		//var w = Wnd.Find("Options", null, "qm.exe");
		var wo = Wnd.Find(null, "QM_Editor");
		//var wo = Wnd.Find(null, "Shell_TrayWnd");
		var w = Wnd.Find("Options", null, WFEtc.Owner(wo));
		//var w = Wnd.Find("Options", null, WFEtc.Process(wo.ProcessId));
		//var w = Wnd.Find("Options", null, WFEtc.Thread(wo.ThreadId));
		Print(w);

		//Print(Wnd.GetWnd.Desktop);

	}

#if TOOLS
	static void TestToolWnd()
	{
		Wnd w = default;

		w = Wnd.Find(null, "QM_Editor");
		//w = Wnd.Find(className: "Shell_TrayWnd");
		//w = Wnd.Find("Notepad");
		//w = Wnd.Find("Options");
		//w = Wnd.Find("Administrator:*");
		w.OrThrow();

		//w = w.Child(null, "Button").OrThrow();
		w = w.Child(null, "SysListView32").OrThrow();
		//w = w.Child(null, "ComboBox").OrThrow();
		////w = w.Child("***label Check").OrThrow();
		//w = w.Child("***label Text", "Edit").OrThrow();
		////w.Child(also: o => { Print(o.ClassName, o.NameLabel); return false; });
		////w.Child("***id 1001", also: o => { Print(o.ClassName, o.NameLabel); return false; });
		//Print(w); return;

		//var w = Wnd.Find("*Sandcastle*").OrThrow();
		//w = w.Child(null, "*.SysTree*").OrThrow();

		using(var f = new Au.Tools.Form_Wnd(w)) {
			f.ShowDialog();
			//Print("form closed");
		}
		//Print("form disposed");
	}

#endif
	static void TestFormClose()
	{
		//		var f = new Form();
		//		var ed = new TextBox() { Top = 100, TabStop = true }; f.Controls.Add(ed);
		//		var ok = new Au.Controls.ButtonOK() { Text = "OK", TabStop=true, TabIndex=1 }; f.Controls.Add(ok);
		//		var can = new Au.Controls.ButtonCancel() { Text = "Cancel", Left=100, TabStop = true, TabIndex=2 }; f.Controls.Add(can);

		//#if false
		//		f.ShowDialog();
		//#else
		//		var f2=new Form();
		//		Timer_.After(100, () =>f.Show(f2));
		//		f2.ShowDialog();
		//#endif

		//		Print(f.DialogResult);

		var main = new Form() { Text = "Main" };

		var f = new FormTest() { Text = "Popup" };
		f.IsPopup = true;
		//f.ShowInTaskbar = false;

		Timer_.After(500, () => f.Show(main));
		//main.ShowDialog();
		Application.Run(main);
	}

	class FormTest :Au.Controls.Form_
	{
		bool _closing;

		//protected override void WndProc(ref Message m)
		//{
		//	switch(m.Msg) {
		//	case Api.WM_CLOSE:
		//		_closing = true;
		//		break;
		//	//case Api.WM_ACTIVATE when _closing && (int)m.WParam == 0:
		//	//	Print("deact");
		//	//	break;
		//	case Api.WM_WINDOWPOSCHANGING when _closing:
		//		Print("WM_WINDOWPOSCHANGING");
		//		break;
		//	}

		//	if(_closing)
		//	Print($"{m.ToString().Split(')')[0]}\r\n\tact={Wnd.Active}\r\n\town={((Wnd)this).Owner}");

		//	base.WndProc(ref m);
		//}


	}

	static void TestWndGet()
	{
		var w = Wnd.Find("Options", "#32770").OrThrow();
		w = w.ChildById(11).OrThrow();
		//Print(w);

		Wnd r;
		//object r;
		//r = w.Get.Next(0);
		//r = w.Get.Previous(1);
		//r = w.Get.Child(1);
		//r = w.Get.FirstChild;
		//r = w.Get.LastChild;
		//r = w.Get.FirstSibling;
		//r = w.Get.LastSibling;
		//r = w.Get.Window;
		//r = w.Get.Owner;
		//r = w.Get.DirectParent;
		//r = w.Get.DirectParentOrOwner;
		//r = w.Get.LastActiveOwnedOrThis();
		//r = w.Get.LastActiveOwnedOrThis(true);
		//r = w.Get.RootOwnerOrThis(true);
		//r = w.Get.RootOwnerOrThis();
		//r = w.Get.OwnersAndThis();
		//r = w.Get.RootOwnerOrThis();
		//r = w.Get.Children();
		//r = w.Get.Children(true);
		//r = w.Get.Above();
		//r = w.Get.Above(80);
		//r = w.Get.Below();
		//r = w.Get.Below(topChild: true);
		//r = w.Get.Left();
		//r = w.Get.Right();
		//r = w.Get.Right(yOffset: 60);
		//r = w.Get.Right(50, 60);
		//r = w.Get.Right(-50);
		r = w.Get.Right(-50, topChild: true);

		//r = Wnd.GetWnd.Top;
		//r = Wnd.GetWnd.Desktop;
		//r = Wnd.GetWnd.DesktopControl;
		//r = Wnd.GetWnd.AllWindows();
		//r = Wnd.GetWnd.AllWindows(true);
		//r = Wnd.GetWnd.AllWindows(false, true);
		//r = Wnd.GetWnd.Root;
		//r = Wnd.GetWnd.Shell;
		//r = Wnd.GetWnd.ThreadWindows(w.ThreadId);
		//r = Wnd.GetWnd.ThreadWindows(w.ThreadId, true);
		//r = Wnd.GetWnd.ThreadWindows(w.ThreadId, false, true);
		//r = Wnd.GetWnd.IsMainWindow(w);
		//r = Wnd.GetWnd.IsMainWindow(w.Owner);
		//r = Wnd.GetWnd.MainWindows();
		//r = Wnd.GetWnd.MainWindows(true);
		//r = Wnd.GetWnd.NextMain();
		//r = Wnd.GetWnd.NextMain(r);

		Print(r);

		//w = w.Get.Right();
		//Print(w);
		//w = w.Get.Next();
		//Print(w);
	}

	static void TestSciSetText()
	{
		var f = new Form();

		var x = new SciTest();
		x.Size = new Size(200, 150);
		//x.InitReadOnlyAlways = true;

		f.Controls.Add(x);
		f.Click += (unu, sed) =>
		{
			//x.ST.ClearText();
			//x.ST.ClearText(noUndo: true, noNotif: true);

			x.ST.SetText("moo");
			//x.ST.SetText("moo", noUndo: true, noNotif: true);

			//x.Call(Sci.SCI_SETREADONLY, 0);
		};
		Application.Run(f);
	}

	class SciTest :AuScintilla
	{

		protected override void OnSciNotify(ref Sci.SCNotification n)
		{
			switch(n.nmhdr.code) {
			case Sci.NOTIF.SCN_PAINTED: case Sci.NOTIF.SCN_UPDATEUI: case Sci.NOTIF.SCN_STYLENEEDED: break;
			default: Print(n.nmhdr.code, n.modificationType); break;
			}

			base.OnSciNotify(ref n);
		}
	}

	static void TestMenuAutoIcons()
	{
		var m = new AuMenu() { ExtractIconPathFromCode = true };
		//m.IconSize = 32;
		//m.ItemThread = MTThread.ThreadPool;
		//m.ItemThread = MTThread.StaThread;
		//m.ItemThread = MTThread.StaBackgroundThread;
		//m.ExceptionHandling = MTExcept.Warning;

		m["'notepad.exe'"] = o => Shell.TryRun("notepad.exe");
		m["'%Folders.System%\\notepad.exe'"] = o => Shell.TryRun("%Folders.System%\\notepad.exe");
		//m.ExtractIconPathFromCode=false;
		m["Folders.System + 'notepad.exe'"] = o => Shell.TryRun(Folders.System + "notepad.exe");
		//m["notepad"] = o => Shell.TryRun(Folders.Test() + "notepad.exe");
		//m["notepad"] = o => Shell.TryRun(Folders.System + @"c:\windows\system32\notepad.exe");
		m["'quickmacros'"] = o => Shell.TryRun("http://www.quickmacros.com");
		m["Folders.System"] = o => Shell.TryRun(Folders.System);
		m["Folders.Virtual.RecycleBin"] = o => Shell.TryRun(Folders.Virtual.RecycleBin);
		m["':: 14001F7840F05F6481501B109F0800AA002F954E'"] = o => Shell.TryRun(":: 14001F7840F05F6481501B109F0800AA002F954E");
		m[@"'shell: AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App'"] = o => Shell.TryRun(@"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
		m["'notepad.exe', ''", ""] = o => Shell.TryRun("notepad.exe");
		m["'notepad.exe', 'notepad.exe'", Folders.System + "notepad.exe"] = o => Shell.TryRun("notepad.exe");
		m["TryRun('no.exe')"] = o => Shell.TryRun("no.exe");
		m["Run('no.exe')"] = o => Shell.Run("no.exe");
		//m["'bad'"] = o => Shell.TryRun("one", "two", 0, new SRMore() { Verb = "verb", WorkingDirectory = "dir" });
		m["Task.Run"] = o => Task.Run(() => Shell.Run("notepad.exe"));
		m["Task.Run"] = o => Task.Run(() => Shell.Run(Folders.System + "notepad.exe"));
		m.Separator();
		//m["Task.Run"] = o => Task.Run(()=> AuDialog.Show("text"));
		//Print(Thread_.NativeId);
		m["Task.Run"] = o => { var t = Thread.CurrentThread; Print(Thread_.NativeId, t.GetApartmentState(), t.IsThreadPoolThread, t.IsBackground); AuDialog.Show("test"); };
		m["Run('no.exe')"] = o => Shell.Run("no.exe");
		m["Form"] = o => new Form().ShowDialog();
		m["OpenFileDialog"] = o => new System.Windows.Forms.OpenFileDialog().ShowDialog();
		m["Clipboard"] = o => Print(Clipboard.GetText());
		m.Separator();
		m.CMS.Items.Add("external", null, (sender, sed) => Print(sender));

		//m.MultiShow = true;
		m.Show();
		1000.ms();
		//AuDialog.Show("after");
		//var i = "moo";
		//Shell.TryRun("notepad.exe");
	}

	public class Autotext :IDisposable
	{

		public void Dispose()
		{

		}

		//public void Add(string text, ATValue replacement)
		//{

		//}

		public void Add(string text, string replacement)
		{
			Print(1);
		}

		public void Add(string text, Func<string, string> replacement)
		{
			Print(2);
		}

		public void Add(string text, Action<string> replacement)
		{
			Print(3);
		}

		public object this[string text]
		{
			set
			{
				Print(4);
			}
		}

		//public ATValue this[string text]
		//{
		//	set
		//	{

		//	}
		//}

		//public int this[string text]
		//{
		//	set
		//	{

		//	}
		//}

		public void Run()
		{

			//Time.SleepDoEvents(Timeout.Infinite);
		}
	}

	//public struct ATValue
	//{
	//	object _o;
	//	ATValue(object o) => _o = o;

	//	public static implicit operator ATValue(string s) => new ATValue(s);
	//	public static implicit operator ATValue(Func<string, string> s) => new ATValue(s);
	//	public static implicit operator ATValue(Action<string> s) => new ATValue(s);
	//}

	static void TestAutotext()
	{
		using(var a = new Autotext()) {
			a.Add("one", "ONE");
			a.Add("two", o => "TWO");
			a.Add("three", o => Print("THREE"));
			//a.Add("two", new Func<string, string>(o => "TWO"));
			a["one"] = "ONE";
			//a["two"] = o => "TWO";
			//a["two"] = new Func<string, string>(o => "TWO");
			//a["two"] = new FuncSS(o => "TWO");
			a.Run();
		}
	}

	//public delegate string FuncSS(string s);

	static void TestProcessStartSpeed()
	{
		using(var e = EventWaitHandle.OpenExisting("Au.TestEv")) {
			e.Set();

			100.ms();
			Output.LibUseQM2 = true;
			Print(2);
		}
	}

	static unsafe void TestMD5Hash()
	{
		//Print(sizeof(decimal));
		//Print(sizeof(GOYTR));
		//Print(sizeof(Convert_.MD5Hash));

		//string s = "one two ąčę Ə";
		//Convert_.MD5Hash h = default;
		var h = new Convert_.MD5Hash();
		//fixed(char* p = s) {

		//	h.Add(p, s.Length);
		//	var r = h.Hash;
		//	Print(r);
		//}

		//var b = Encoding.UTF8.GetBytes(s);
		//fixed (byte* p = b) {

		//	h.Add(p, b.Length);
		//	var r = h.Hash;
		//	Print(r);

		//	var a = r.ToArray();
		//	Print(Convert_.HexEncode(a));

		//}

		//h.Add(s);

		var g1 = Guid.NewGuid();
		var g2 = Guid.NewGuid();

		h.Add(&g1, sizeof(Guid));
		h.Add(&g2, sizeof(Guid));
		Print(h.Hash);

		h.Add(&g1, sizeof(Guid));
		h.Add(&g2, sizeof(Guid));
		Print(h.Hash);
	}

	[StructLayout(LayoutKind.Explicit)]
	struct GOYTR
	{
		[FieldOffset(0)] long k;
		[FieldOffset(40)] public decimal d;
	}


	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _TestTaskExceptions()
	{
		var t = Task.Run(() => { Print("task"); throw new InvalidOperationException("test"); });
		//100.ms();
		//t.Dispose();
	}
	static void TestTaskExceptions()
	{
		//TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		for(int i = 0; i < 5; i++) _TestTaskExceptions();
		//for(int i=0; i<5; i++) {
		//	//Thread.Sleep(100);
		//	Time.SleepDoEvents(100);
		//	GC.Collect();
		//	GC.WaitForPendingFinalizers();
		//}
		Task.Run(() =>
		{
			500.ms();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		});
		AuDialog.Show();
		//MessageBox.Show("");
		Print("fin");
	}

	private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
	{
		Print(e.Observed, e.Exception);
	}

	static void _TestFileWriteLineSpeed()
	{
		var path = @"Q:\Test\ok\test.log";
		string s1 = "ffskjfdshfjshdfjkdshfj", s2 = "dahshdkahfjdfjhgjshdfahfiodjfijifjkjsglkjdkgljskljgkldjg";

		Perf.First();
		//using(var b = File.CreateText(path)) {
		//	for(int i = 0; i < 1000; i++) {
		//		if(false) b.WriteLine(s1);
		//		else { b.Write(s1); b.WriteLine(s2); }
		//		//else b.WriteLine(s1+s2);
		//	}
		//}

		var k = new StringBuilder();
		for(int i = 0; i < 1000; i++) {
			if(false) k.AppendLine(s1);
			else { k.Append(s1); k.AppendLine(s2); }
			//else b.WriteLine(s1+s2);
		}
		File.WriteAllText(path, k.ToString());

		Perf.NW();
	}

	static void TestFileWriteLineSpeed()
	{
		Perf.Cpu();
		for(int i1 = 0; i1 < 5; i1++) {
			_TestFileWriteLineSpeed();
		}

	}

	static object koooo;
	static unsafe void TestKeySpeedWithEnum()
	{
		Perf.Next();
		koooo = "dd";

		var k = new Keyb(null);
		Perf.Cpu();
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 100;
			Perf.First();
			//for(int i2 = 0; i2 < n2; i2++) { k.AddKeys("Down"); }
			for(int i2 = 0; i2 < n2; i2++) { if(0 == Keyb.Misc.ParseKeyName("Down")) throw new ArgumentException(); }
			Perf.Next();
			for(int i2 = 0; i2 < n2; i2++) { if(0 == Keyb.Misc.ParseKeyName("VolumeUp")) throw new ArgumentException(); }
			//for(int i2 = 0; i2 < n2; i2++) { if(0 == Keyb.Misc.ParseKeyName("Attn")) throw new ArgumentException(); }
			Perf.NW();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static unsafe void TestPerfWithoutSM(long t1)
	{
		//var t = Time.Microseconds;
		//var t2 = Time.Microseconds;
		//var t3 = Time.Microseconds;
		//Print(t2 - t, t3-t2);

		//var p = Perf.StartNew();
		//var pp = new Perf.Inst();
		//1.ms();
		//p.Next();
		//1.ms();
		//p.Next();
		////Print(sizeof(Perf.Inst));

		//Perf.First();
		//string s=null;
		//for(int i = 0; i < 5; i++) {
		//	//s=Convert_.HexEncode(&p, sizeof(Perf.Inst));
		//	//var si = sizeof(Perf.Inst);
		//	//var b = new byte[si]; Marshal.Copy((IntPtr)(&p), b, 0, si);
		//	//s = Convert.ToBase64String(b);
		//	s = p.Serialize();
		//	Perf.Next();
		//	//b = Convert.FromBase64String(s);
		//	//pp.Deserialize(s);
		//	pp = new Perf.Inst(s);
		//	Perf.Next();
		//}
		//Perf.Write();
		//Print(s);
		//p.Write();
		//pp.Write();

		var t2 = Time.Microseconds;
		//200.ms();
		//Perf.Cpu();

		Perf.First();

		var t3 = Time.Microseconds;

		Perf.Next();

		var t4 = Time.Microseconds;

		Perf.Next();
		Perf.Next();
		Perf.NW();

		Print("t", t2 - t1, t3 - t2, t4 - t3);
	}

	static void Cpu(int timeMilliseconds = 200)
	{
		int n = 0;
		for(long t0 = Time.Microseconds; Time.Microseconds - t0 < timeMilliseconds * 1000L; n++) { }
		//Print(n);
	}

	static int s_mfgytr = 8;

	static ref int Bure()
	{
		return ref s_mfgytr;
	}

	//static void TestWebBrowserLeaks()
	//{
	//	for(int i = 0; i < 5; i++) {
	//		var t = new Thread(() =>
	//		  {
	//			  var f = new Tests.Form1();
	//			  Application.Run(f);
	//		  });
	//		t.SetApartmentState(ApartmentState.STA);
	//		t.Start();
	//		t.Join();
	//		AuDialog.Show();
	//	}
	//}

	static void TestCompiler2()
	{
		//var f = new Au.Tools.Form_Wnd(Wnd.Find("Quick*"));
		//f.ShowDialog();

		string code =
@"static int Hoo() { return 3; }";

		Au.Compiler.Scripting.Result r = null;
		Perf.First();
		for(int i = 0; i < 5; i++) {
			if(!Au.Compiler.Scripting.Compile(code, out r, true, true)) { Print(r.errors); return; }
			Perf.Next();

		}
		Perf.Write();
		Print(r.method.Invoke(null, null));
	}

	static void TestArrayExtensions()
	{
		//throw new Exception();

		var a = new int[] { 100, 101, 102 };
		//a = a.RemoveAt_(0);
		//a = a.Insert_(1, -1);
		a = a.Insert_(1, -1, -2);
		Print(a);
		Print("---");
		a = a.RemoveAt_(0, 2);
		Print(a);

		//var a = new string[] { "", "one", "two ooo", "thr \"ee\"", @"four\",  @"fi ve\",  @"fi ve\\", @"si ""x""\", @"si \""x""", @"si \\""x""", "se\r\nven"};
		//Print(a);

		//var s = Au.Util.StringMisc.CommandLineFromArray(a);
		//Print($"<><c 0xff0000>{s}</c>");

		//Print(Au.Util.StringMisc.CommandLineToArray(s));

		//var si = new ProcessStartInfo(@"q:\app\au\_\Au.Task.exe") { UseShellExecute = false, Arguments = s };
		//Process.Start(si);
	}

	static void TestResources()
	{
		//var v = Project.Properties.Resources.SciLexer;
		//var v = Au.Util.Resources_.GetAppResource("SciLexer");
		//var v = Project.Properties.Resources.tips;
		//var v = Au.Util.Resources_.GetAppResource("tips");
		//var v = Project.Properties.Resources.il_icons;
		//Print(v?.GetType());

		//Print(File_.Misc.CalculateDirectorySize(Folders.ThisApp));
		100.ms();

		//var file = @"Q:\Test\copy.txt";
		////file = @"Q:\Test\qm small icon.png";
		////file = @".jpg";
		////file = @"Q:\Test\x.qml";
		//file = @"Q:\Test\catlog.txt-";
		//file = ".cur";
		//if(File_.Misc.GetMimeContentType(file, out var mime, true)) Print(mime);
		//else Print("FAILED");

		//Print(System.Web.MimeMapping.GetMimeMapping(file));

		//var h = new HashSet<string>();
		//foreach(var v in File_.EnumDirectory(Folders.ProgramFilesX86, FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
		//	if(v.IsDirectory) continue;
		//	if(!h.Add(Path_.GetExtension(v.Name).ToLower_())) continue;
		//	var file = v.FullPath;
		//	if(!File_.Misc.GetMimeContentType(file, out var mime, false)) mime = "FAILED";
		//	var mime2 =System.Web.MimeMapping.GetMimeMapping(file);
		//	if(mime == mime2 ||(mime=="FAILED" && mime2== "application/octet-stream")) continue;
		//	Print($"{Path_.GetExtension(file),-15}   {mime,-30}, {mime2}");
		//}

		//var x = Project.Properties.Resources.macro;
		//x = new Icon(x, 16, 16);
		//Osd.ShowText("text", icon: x, showMode: OsdShowMode.Wait);
	}

	static void TestConfigSettings()
	{
		//var s = Project.Properties.Settings.Default.Moo;
		//Print(s);
		//Project.Properties.Settings.Default.Moo = "NNNew";
		//s = Project.Properties.Settings.Default.Moo;
		//Print(s);
		//Project.Properties.Settings.Default.Save();

		//ConfigurationManager.

		//var x = new Project.Properties.Settings();
		//Print(x["Moo"]);
		//Print(s_sett["Moo"]);

		//var config = @"Q:\app\Au\Tests\Unused currently\App2.config";
		//AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);
		//Print(ConfigurationManager.AppSettings["name"]);
	}

	//static Project.Properties.Settings s_sett = (Project.Properties.Settings)System.Configuration.ApplicationSettingsBase.Synchronized(new Project.Properties.Settings());

	static void TestStartProcessFromShell()
	{
		string exe = Folders.System + "notepad.exe";

		//int hr = Cpp.Cpp_StartProcess(exe, null, null, null, out BSTR bResult);
		//Print(hr, bResult);
	}

	static void TestGCHandle()
	{
		object o1 = new object(), o2 = new object();

		for(int i = 0; i < 20; i++) {
			var h = GCHandle.Alloc((0 == (i & 1)) ? o1 : o2);
			Print((IntPtr)h);
			h.Free();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static bool TestEnumHas(DFlags f)
	{
		return f.Has_(DFlags.CommandLinks);
	}

	static void TestEnumHas2()
	{
		Print(1);
		var w = Wnd.Find("Quick*");
		//var v = InterDomainVariables.GetVariable(<string>("test");
	}

	static void TestFileOpenWaitLocked()
	{
		var file = Folders.Temp + "test.txt";

		Task.Run(() =>
		{
			try {
				for(int i = 0; i < 100; i++) {
					using(var f = File.OpenWrite(file)) {
						f.WriteByte(1);
						5.ms();
					}
					5.ms();
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); }
		});

		Task.Run(() =>
		{
			10.ms();
			try {
				for(int i = 0; i < 100; i++) {
					using(var f = File.OpenRead(file)) {
						f.ReadByte();
					}
					12.ms();
				}
			}
			catch(Exception e) { Debug_.Print(e.ToString()); }
		}).Wait();

		Print("OK");
	}

	delegate int _tIsWindow(Wnd w);

	[Flags] enum EInt { one = 1, two = 2 };
	[Flags] enum ELong :long { one = 1, two = 2 };
	[Flags] enum EByte :byte { one = 1, two = 2 };
	[Flags] enum EShort :short { one = 1, two = 2 };

	static void TestEnumExtMethodsAndArrayBuilder()
	{
		//var k = new LibArrayBuilder<int>();
		//k.Add(1);
		//k.Add(2);
		//k.Add() = 3;
		//k.Add() = 4;
		//int i = 5;
		//k.AddR(in i);
		//k.AddR(in i);
		//Print(k.ToArray());
		//Print("---");
		//Print(k[0], k[1], k[2]);

		//Print("--");
		//EInt i = EInt.two;
		//if(i.Has_(EInt.two))
		//	Print("int");

		//ELong l = ELong.two;
		//if(l.Has_(ELong.two))
		//	Print("long");

		//EShort s = EShort.two;
		//if(s.Has_(EShort.two))
		//	Print("short");

		//EByte b = EByte.two;
		//if(b.Has_(EByte.two))
		//	Print("byte");

		//AFFlags e = 0;
		//e.SetFlag_(AFFlags.ClientArea | AFFlags.MenuToo, true);
		//Print(e);
		//e.SetFlag_(AFFlags.MenuToo, false);
		//Print(e);
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

	static void TestFoldersSetOnce()
	{
		Print(Folders.ThisAppTemp);
		//Folders.ThisAppTemp = @"C:\Users\G\AppData\Local\Temp\Au2";
		//Folders.ThisAppTemp = @"C:\Users\G\AppData\Local\Temp\Au2";
		//Print(Folders.ThisAppTemp);

		Print(Folders.ThisAppDocuments);
		Print(Folders.ThisAppData);
		Print(Folders.ThisAppDataLocal);
		Print(Folders.ThisAppDataCommon);
		Print(Folders.ThisAppImages);
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

			//TestAuLoadingFormsAssembly();
			//TestFoldersSetOnce();
			//TestExceptionInInteropCallback();
			//TestFileOpenWaitLocked();
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
