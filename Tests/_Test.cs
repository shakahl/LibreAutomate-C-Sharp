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
//using System.Linq;
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

using System.Dynamic;

//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

#pragma warning disable 162, 168, 169, 219, 649 //unreachable code, unused var/field


[System.Security.SuppressUnmanagedCodeSecurity]
static unsafe partial class Test
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
		_EnableVisualStylesEtc();
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

		var w = +Wnd.Find("Quick*");

		var a = +Acc.Find(w, "CHECKBOX", "Annot*");
		//var atb = +Acc.Find(w, "TOOLBAR", prop: "id=2053");
		//var a = +atb.Find("CHECKBOX", "Annot*");

		a.DoAction();

		Print("after");
		0.3.s();
		Print("end");
	}

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
		//Wnd w = Wnd.Find("QM# -*");
		//Wnd w = Wnd.Find("WinDbg*");
		//Wnd w = Wnd.Misc.WndRoot;
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

			//foreach(Wnd ww in Wnd.Misc.AllWindows(true)) x.TestAcc(ww);

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

	//	//foreach(var v in Wnd.Misc.AllWindows()) {
	//	//	foreach(var a in )
	//	//}

	//	Wnd wSkip = Wnd.Find("Quick*").Kid(2202);
	//	w = Wnd.Misc.WndRoot;
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
	//				if(aa == null) Print(s, a.WndContainer, a.WndContainer.WndWindow);
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
		AccEVENT ev = AccEVENT.OBJECT_FOCUS;
		var hh = SetWinEventHook(ev, ev, default, _testWinEventProc, 0, 0, 0);
		//Print(hh);
		AuDialog.Show();
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
	internal static extern int ObjectFromLresult(LPARAM lResult, ref Guid riid, LPARAM wParam, out IntPtr ppvObject);

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
				if(i == 0) Perf.SpinCPU(100);
			}
		}
		//100.ms();
		//Print(Wnd.WndActive);
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
		for(; !Input.IsCtrl; 1.s()) {
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

	static void TestCppWildex()
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
	//			throw new AuException(new string(es));
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
		var x = new WildexStruct("**r gg");
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
		//		//Debug_.LibPrintMemory();
		//		1.ms();
		//	}
		//}
		//Perf.NW();
		//Print("END");

		Print(Wnd.Find(@"**r \QQuick\E \bMacros\b"));
	}


	static void TestDllInProcThisThread()
	{
		var f = new Form();
		f.Text = "Form55";
		var b = f.Add_<Button>(4, 4, 100, 30, "Five");
		f.Click += (unu, sed) => { TestDllInProc(); };
		f.ShowDialog();
	}

	static void TestDllInProc()
	{
		//var w = Wnd.Find(null, "MozillaWindowClass").OrThrow();
		var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
		//var w = Wnd.Find("* Google Chrome", "Chrome*").Child(null, "Chrome_Render*").OrThrow();
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
		//var w = Wnd.Find(null, "QM_Editor").Kid(2053).OrThrow();
		//var w = Wnd.Find("FileZilla").OrThrow();
		//var w = Wnd.Find("FileZilla").Kid(-31801).OrThrow(); //toolbar
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
	//		var aw = Wnd.Misc.AllWindows(true);
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
		//TODO: test what Chrome web page objects exist when window name changed before invalidating old DOCUMENT.

		//var w = a.WndTopLevel;
		////Wnd w = default; WaitFor.Condition(3, o=> { bool ok = !(w = a.WndTopLevel).Is0; if(!ok) Print("wnd 0"); return ok; });
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
		var wBackMenu = WaitFor.WindowExists(10, null, menuClass, also: o => o.HasStyle(Native.WS_POPUP));
		//Print(wBackMenu);
		a = Acc.Wait(10, wBackMenu, "MENUITEM", "Yahoo");
		//Print(a);
		switch(browser) {
		case EBrowser.Chrome: case EBrowser.Opera: a.VirtualClick(); break; //VirtualClick does not work with Firefox
		default: 100.ms(); a.DoAction(); break; //DoAction does not work with Chrome
		}

		//return;
		if(stop) return;
		WaitFor.WindowExists(30, appName);
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
		var aw = Wnd.Misc.AllWindows(true);
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
		Perf.SpinCPU(500);
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
		foreach(var w in Wnd.Misc.AllWindows(true)) {
			k.AddRange(w.AllChildren());
		}
		Perf.SpinCPU(200);
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
		if(Input.IsScrollLock) flags |= AXYFlags.NotInProc;

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
		//var kk =Wnd.Misc.AllWindows(true);
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

		//foreach(var c in w.AllChildren()) {
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
		//Print(Wnd.WndActive);
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
		//Print(Wnd.WndActive);
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

		//foreach(var w in Wnd.Misc.AllWindows()) {
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
		Perf.SpinCPU(100);
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
	//	c.Style = unchecked((int)(Native.WS_POPUPWINDOW|Native.WS_VISIBLE));
	//	c.ExStyle = (int)Native.WS_EX_TOPMOST;

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
	//	Output.LibWriteToQM2 = true;

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
	//	var style =Native.WS_OVERLAPPEDWINDOW |Native.WS_VISIBLE;
	//	var w = Wnd.Misc.MyWindowClass.InterDomainCreateWindow("aa test", "Test1", style, 0, 200, 200, 200, 200);
	//	AuDialog.Show("--");
	//	Wnd.Misc.DestroyWindow(w);
	//}

	class MyWindow2 :Wnd.Misc.MyWindow
	{
		public override LPARAM WndProc(Wnd w, uint message, LPARAM wParam, LPARAM lParam)
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
		var style = Native.WS_OVERLAPPEDWINDOW | Native.WS_VISIBLE;

		var x = new MyWindow2();
		if(!x.Create("MyWindow", "MyWindow", style, 0, 200, 200, 200, 200)) { Print("failed"); return; }
		Timer_.After(1000, t => { GC.Collect(); });
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
		var x = new OnScreenRect();
		x.Rect = r;
		//x.Color = 0xFF0000; //red
		x.Color = Color.Orange;
		x.Thickness = 2;
		//x.Opacity = 0.3;
		x.Show(true);

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
			x.Opacity += 0.03;
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
		////var a = +Acc.Find(w, "web:LINK")?["ne"];
		////var a = +Acc.Find(w, "web:LINK-").SimpleElementId;
		//Acc.Find(w, "web:LINK", "Portal-").MouseMove();

		////a = Acc.Find(w, "web:UUU").OrThrow();
		////Print(a);

		//var w1 = Wnd.Find("Example").OrThrow();
		//var w2 = +Wnd.Find("Example"); //the same

		//var w3 = Wnd.Find("Example").OrThrow().Child("Example").OrThrow();
		//var w4 = Wnd.Find("Example").Child("Example").OrThrow();

		//var w5 = Wnd.Find("Example").Child("Example");

		//var w = Wnd.Find("Example").OrThrow();

		//var a1 = Acc.Find(w, "web:LINK", "Example").OrThrow();
		//var a2 = +Acc.Find(w, "web:LINK", "Example"); //the same

		//var a3 = (Acc.Find(w, "web:LINK", "Example")?.Navigate("example")).OrThrow();
		//var a4 = +Acc.Find(w, "web:LINK", "Example")?.Navigate("example"); //the same

		//var a5 = (Acc.Wait(3, w, "Example")?.Navigate("example")).OrThrow();
		//var a6 = +Acc.Wait(3, w, "Example")?.Navigate("example");

		//var w = Wnd.Find("Example").OrThrow();
		//var r1 = WinImage.Find(w, "example").OrThrow();
		//var r2 = +WinImage.Find(w, "example"); //the same
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

	static void TestStrtoiOverflow()
	{
		//if(!ColorInt.FromString("0xFF", out var c)) return;
		//if(!ColorInt.FromString("#FF", out var c)) return;
		//Print(c);

		//Print("0x1A".ToInt32_());
		//Print("1A".ToInt32_());
		//Print("0x1A".ToInt32_(0, true));
		//Print("1A".ToInt32_(0, true));
		//Print("0x1A".ToInt32_(0, out _, true));
		//Print("1A".ToInt32_(0, out _, true));

		string s = "0x10";
		s = "0xffffffff";
		s = "0x80000000";
		s = "-0xffffffff";
		//s = "0xffffffffffffffff";
		//Print(s.ToInt32_());
		//Print(s.ToInt64_());
		//Print("-----");
		//Print(Api.strtoi(s));
		//Print(Api.strtoi64(s));
		//Print(Api.strtoui64(s));

		//Print(0xffffffffU);
		//Print(-0xffffffffU);

		fixed (char* p = s) {
			PrintHex(Api.strtoi(p));
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

	static void TestUnsafe()
	{
		int x = 5, y = 0;
		Unsafe.Copy(ref y, &x);
		Print(y);
	}

	static void TestAccFirefoxNoSuchInterface()
	{
		//var w = +Wnd.Find("Quick Macros Forum - Mozilla Firefox", "MozillaWindowClass");
		////var w = +Wnd.Find("* Chrome");
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

		//var a = +Acc.Find(w, "web:LINK", "Bug Reports", null, f);
		//Print(a.MiscFlags);
		//Print(a);
		////Print(a.Role);
		////Print(a.Name);
		////Print(a.State);
		////Print(a.Rect);
		////Print(a.WndContainer);

		//var w = +Wnd.Find("Settings", "ApplicationFrameWindow");
		//var a = +Acc.Find(w, "LISTITEM", "Devices", "class=Windows.UI.Core.CoreWindow");

		var w = +Wnd.Find("Microsoft Edge", "Windows.UI.Core.CoreWindow", flags: WFFlags.HiddenToo);
		Acc.PrintAll(w, flags: AFFlags.UIA);
		var a = +Acc.Find(w, "TEXT", "this exact word or phrase:", flags: AFFlags.UIA);
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
		//var a =Wnd.Misc.MainWindows(likeAltTab:true);
		//Print("---");
		//Print(a);

		//Print(Wnd.WndActive);
		//Wnd.Misc.SwitchActiveWindow();
		//Print(Wnd.WndActive);

		//Print(Api.GetDesktopWindow());
		//Print("----");
		//var a =Wnd.Misc.AllWindows();

		////Perf.SpinCPU(100);
		////for(int i1 = 0; i1 < 5; i1++) {
		////	int n2 = 1000;
		////	Perf.First();
		////	for(int i2 = 0; i2 < n2; i2++) { foreach(var w in a) { var ro = Wnd.Misc.WndRootOwnerOrThis(w); } }
		////	Perf.Next();
		////	for(int i2 = 0; i2 < n2; i2++) { foreach(var w in a) { var ro = w.WndOwner; } }
		////	Perf.Next();
		////	//for(int i2 = 0; i2 < n2; i2++) { foreach(var w in a) { var ro = Wnd.Misc.WndRootOwnerOrThis2(w); } }
		////	//Perf.Next();
		////	for(int i2 = 0; i2 < n2; i2++) { }
		////	Perf.NW();
		////}

		//foreach(var w in a) {
		//	var ro = Wnd.Misc.WndRootOwnerOrThis(w);
		//	//if(ro== Api.GetDesktopWindow()) {
		//	//if(ro!= w) {
		//	//	Print("----");
		//	//	Print(w);
		//	//	Print(ro);
		//	//	Print(w.WndOwner);
		//	//	Print(w.WndDirectParentOrOwner );
		//	//}
		//	//Print();
		//	var ro2 = Wnd.Misc.WndRootOwnerOrThis(w, true);
		//	if(ro2 != ro) {
		//		Print("----");
		//		Print(w);
		//		Print(ro);
		//		Print(ro2);
		//	}
		//}

		//var w = Wnd.Find("Options");
		//Print(w);
		//Print(Wnd.Misc.WndLastActiveOwnedOrThis(w));
		//Print(Wnd.Misc.WndLastActiveOwnedOrThis(w, true));

		//Print(Wnd.Misc.MainWindows());
		////Print(Wnd.Misc.WndNextMain());
		//var w = +Wnd.Find("* Chrome");
		//Print(Wnd.Misc.WndNextMain(w, retryFromTop: true));
		Print(Wnd.Misc.SwitchActiveWindow());
		Print(Wnd.WndActive);
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
			ad.DoCallBack(() => { Output.LibWriteToQM2 = true; Print(Api.GetCurrentThreadId()); TestAccProcessDoesNotExit(); });
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
		};
		w.Show();
		var app = new System.Windows.Application();
		app.Run();

		//AuDialog.Show(owner: Wnd.Find("Quick*"));
	}

	static void TestAccForm()
	{
		Acc a = null;
#if false
		//Cpp.Cpp_Test(); return;

		var w = Wnd.Find("* Chrome").OrThrow();
		a = Acc.Find(w, "web:BUTTON", "Search").OrThrow();
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

		//foreach(var k in Wnd.Misc.AllWindows(false)) {
		//	//if(!k.ClassNameIs("Windows.UI.Core.CoreWindow")) continue;
		//	//if(!k.IsVisible || k.IsVisibleEx) continue;
		//	if(!k.IsCloaked) continue;
		//	var s = k.Name; if(Empty(s) || s=="Default IME" || s== "MSCTFIME UI") continue;
		//	Print(k);
		//	//Print(k.IsVisible, k.IsCloaked);
		//	//continue;
		//	foreach(var c in k.AllChildren()) {
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
		//var w = +Wnd.Find("Icons");
		//var r = +WinImage.Find(w, @"Q:\My QM\copy.bmp", WIFlags.WindowDC);
		//r.MouseMove();

		//var w = +Wnd.Find("Icons");
		//var _image = Au.Controls.ImageUtil.ImageToString(@"Q:\My QM\copy.bmp");
		//Print(_image);
		//var r = +WinImage.Find(w, _image, WIFlags.WindowDC);
		//r.MouseMove();



		//Perf.First();
		////var im = Image.FromFile(@"Q:\My QM\copy.bmp");
		////var w = +Wnd.Find(also: o=> null!=WinImage.Find(o, im, WIFlags.WindowDC));
		//var w = +Wnd.Find(also: o => null != WinImage.Find(o, @"Q:\My QM\copy.bmp", WIFlags.WindowDC));
		////var w = +Wnd.Find(also: o=> { Print(o); return null != WinImage.Find(o, @"Q:\My QM\copy.bmp", WIFlags.WindowDC); });
		////var w = +Wnd.Find(also: o => { Print(o); Perf.First(); var ok = null != WinImage.Find(o, @"Q:\My QM\copy.bmp", WIFlags.WindowDC); Perf.NW(); return ok; });
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
		var w = +Wnd.Find(contains: "Do you want to save*");
		//var w = +Wnd.Find(contains: "Untitled - Notepad");
		//var w = +Wnd.Find(contains: "Personalization");
		//var w = +Wnd.Find(contains: new Acc.Finder("STATICTEXT", "Do you want to save*"));
		//var w = +Wnd.Find(contains: new Wnd.ChildFinder("Save", "Button"));
		//var w = +Wnd.Find(contains: WinImage.LoadImage(@"Q:\My QM\copy.bmp"));
		Perf.NW();
		Print(w);

		//var w = +Wnd.Find("* Notepad");
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
			//var a = +Acc.Find(w, "web:LINK", "Videos");
			//var a = +Acc.Find(w, "LINK", "Videos");
			//var a = +Acc.Find(w, "LINK", "Videos", "class=*Server");
			var a = +Acc.Find(w, "LINK", "Videos", controls: new Wnd.ChildFinder(null, "*Server"));
			Perf.NW();
			Print(a);
		}
	}

	static void TestToIntWithFlags()
	{
		Print("15".ToInt32_());
		Print("0xA".ToInt32_());
		Print("C".ToInt32_());
		Print("C".ToInt32_(0, STIFlags.IsHexWithout0x));
		Print("0xA".ToInt32_(0, STIFlags.NoHex));
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
		Perf.SpinCPU(100);
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
		Perf.SpinCPU(100);
		Debug_.LibPrintMemory();
		for(int i1 = 0; i1 < 5; i1++) {
			int n2 = 100;
			Perf.First();
			for(int i2 = 0; i2 < n2; i2++) { foreach(var m in x.FindAllS(s)) k1++; }
			//for(int i2 = 0; i2 < n2; i2++) { if(x.FindAllS(s, out var a2)) k2++; }
			Perf.NW();
			Debug_.LibPrintMemory();
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
		Debug_.LibPrintMemory();
		for(int i = 0; i < 15; i++) {
			for(int j = 0; j < 40; j++) {
				n = TestAccLeaks3(w);
			}
			Debug_.LibPrintMemory();
			100.ms();
		}
		Print(n);
		//Print($"max {(double)Acc.DebugMaxMemoryPressure/(1024*1024)}, sum {(double)Acc.DebugMemorySum/(1024*1024)}");
	}

	static void TestAccLeaks()
	{
		//var w = +Wnd.Find(className: "FM");
		//var w = +Wnd.Find("QM Help");
		//var w = +Wnd.Find("* Firefox");
		var w = +Wnd.Find("Quick M*");
		//var w = +Wnd.Find("*Chrome");
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


	static void TestKeySyntax()
	{

		Input.Common.SleepAfter = 100;
		Key("Tab Ctrl+V");

		var k = new Input() { SleepAfter = 200 };
		k.Key("Tab Ctrl+V");

		//using(var k = new Input()) {
		//	k.Key("Tab Ctrl+V");
		//}

		//Input.PasteKeyText = true; //or Key(true, "keys", "text");
		//Key("Ctrl+C", "text", "F2", 500, Keys.Back, "", "text", (Keys)8, (KScan)8);
		//Key("Tab*2", "user", "Tab", "password", 200, "Enter");
		//Text("user", "Tab");
		//Paste("user");
		//var w = +Wnd.Find("Quick*").Kid(2216);
		//Key(w, "Ctrl+V");
		//Text(w, "Ctrl+V");

		//var k = new Input();
		//k.Prop1 = 3;
		//k.SendKeys("Enter");


		//Part types depend on argument type:
		//string - keys or text, in alternating order: Key("keys", "text", "keys", "text").
		//	After arguments of other types again starts with keys: Key("keys", 10, 20, "keys", "text").
		//	Keys can be empty string: Key("keys", 10, "", "text").
		//int - milliseconds to sleep.
		//enum Keys (or KCode?) - virtual key code: Key(Keys.Back, (Keys)8).
		//enum KScan - scan code. Example: Key((KScan)10).
		//bool - true to use clipboard (paste text), false (default) to use keyboard.
		//	For Text(), the default depends on Input.TextOptions, which can specify to use paste with all or some windows, maybe using a predicate.
		//some options/flags types.

		//rejected
		//Key("Ctrl+C", KText, "text", KSleep, 500, "F2", KCode, Keys.Back, KCode, Api.VK_APPS, KScan, 10);
		//Key("Ctrl+C", "text", "F2", 500, "F2", Keys.Back, KCode, Api.VK_APPS, KScan, 10);
		//Key("Ctrl+C 'text' F2", 500, "F2", Keys.Back, KCode, Api.VK_APPS, KScan, 10);
		//Key("Ctrl+C 'text' F2", 500, "F2", Keys.Back, (Keys)8, (byte)10);

		//consider
		//Key(K.Ctrl.Alt.F.Plus.O.Text("text").Tab.Paste("text").Sleep(100).Enter);

		//or don't use Text(params object[] text). Rarely used. Instead:
		//Text(string text, string keys = null, TOptions options = null)
		//rejected:
		//Key(string keys, string text = null, TOptions options = null)

		//rejected: use clipboard for text. Rarely need. Unclear parameters. Better Key("keys"); Paste("keys");
		//KeyPaste("keys", "paste")

		//SELECTED:
		//Key(params object[] keys) //Keys("keys", "text", "keys", "text", 100, "keys", "text", Keys.Back, new KeyOptions(...));
		//Text(string text, string keys = null)
		//Text(KeyOptions options, string text, string keys = null)
		//Paste(string text, string keys = null)
		//Input.Paste(string text, string keys = null)
		//Input.PasteFormat(string text, string format)
		//Input.PasteTo(Wnd w, string text, string format = null)
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


	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
		//MessageBox.Show(""); return;

#if DEBUG
		//Output.IgnoreConsole = true;
		//Output.LogFile=@"Q:\Test\Au"+IntPtr.Size*8+".log";
#endif
		Output.LibWriteToQM2 = true;
		Output.RedirectConsoleOutput = true;
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			100.ms();
		}

		try {
#if true

			TestRegexGroupByName();
			//TestWildexRegex();
			//TestRegexStatic();
			//TestRegexExamples();
			//TestKeySyntax();
			//TestAuDialogRenamed();
			//TestSpan();
			//TestLibDC();
			//TestNewEscape();
			//TestPrintListEx();
			//TestRegexAndGC();
			//TestRegexCalloutWithFindAll();
			//TestRegexSplit();
			//TestRegexFindAllE();
			//TestRegexReplace();
			//TestRegexFindAll();
			//TestRegex_();
			//TestRegexCulture();
			//TestRegexMatch();
			//TestRegexGetRepeatedGroupInstances();
			//TestRegexCallout();
			//TestRegexGroupNumberFromName();
			//var g=TestRegex_(); //Print(g.Value); Print(g.Value);
			//TestRegex_();
			//TestPcreSpeed();
			//TestPcreRegexStatic();
			//TestNewWildexSyntax();
			//TestWndImage();
			//TestWndForm();
			//TestTaskDialogOwnerWpf();
			//TestWndFindProgramEtc();
			//TestMainWindows2();
			//TestRunConsole();
			//TestStrtoiOverflow();
			//TestCompiler();
			//TestToIntWithFlags();
			//TestFinalizersAndGC();
			//return;
#else
			try {

				//TestAccLeaks();
				TestAccForm();
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
		key(...); //Input.Keys(...);
		tkey(...); //Input.TextKeys(...); //or txt
		paste(...); //Input.Paste(...);
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

