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

using System.Dynamic;

[module: DefaultCharSet(CharSet.Unicode)]
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
		//w.Activate(); Wait(.2);
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

			//var a =Acc.deb; for(int j = 0; j < a.Length; j++) PrintList(j, a[j]);
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

			//var a = Acc.Find(w, "PUSHBUTTON", "History");
			//var a = Acc.Find(w, "web:DOCUMENT/div/PUSHBUTTON", "Bookmarks");
			//var a = Acc.Find(w, "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/PUSHBUTTON", "History");

			//var a = Acc.Find(w, "LINK", "Programming", AFFlags.HiddenToo); //1126 (depends on hidden tabs)
			//var a = Acc.Find(w, "LINK", "Programming"); //225
			//var a = Acc.Find(w, "web:LINK", "Programming"); //120
			//var a = Acc.Find(w, "web:DOCUMENT/div/div/div/div/LIST/LISTITEM/LIST/LISTITEM/LINK", "Programming"); //74
			//var a = Acc.Find(w, "web:DOCUMENT/div/div[4]/div[5]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //47
			//var a = Acc.Find(w, "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/div/div/div/LIST/LISTITEM/LIST/LISTITEM/LINK", "Programming"); //94
			//var a = Acc.Find(w, "APPLICATION[4]/GROUPING[-4]/PROPERTYPAGE[-4]/browser/DOCUMENT/div/div[4]/div[5]/div/LIST[3]/LISTITEM[2]/LIST/LISTITEM/LINK", "Programming"); //58
			//var a = Acc.Find(w, "web://[4]/[5]//[3]/[2]///LINK", "Programming"); //47

			//var a = Acc.Find(w, "PUSHBUTTON", "Resources    Alt+F6"); //6.4 s
			//var a = Acc.Find(w, "class=ToolbarWindow32:PUSHBUTTON", "Resources    Alt+F6"); //120
			//var a = Acc.Find(w, "CLIENT/WINDOW/TOOLBAR/PUSHBUTTON", "Resources    Alt+F6"); //139

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
	//	////var a = Acc.Find(w, "PUSHBUTTON", "Search Control");
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
	//				if(aa == null) PrintList(s, a.WndContainer, a.WndContainer.WndWindow);
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
		//f.MaxLevel = 10;
		if(!f.Find(w)) return;
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
		//using(var a = Acc.Find(w, "PUSHBUTTON", "Paste*").OrThrow()) {
		//	Print(a);
		//}
		//using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) {
		//	a.EnumChildren(true, o =>
		//	{
		//		//PrintList(o.Level, o.Role);
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
		//using(var a = Acc.Find(w, "CHECKBUTTON", "\0 description=Maišymo veiksena").OrThrow()) Print(a);

		//var w = Wnd.Find("Welcome Guide — Atom", "Chrome_WidgetWin_1").OrThrow();
		//Print(w);
		//using(var a = Acc.Find(w, "web:PUSHBUTTON", "*Learn Keyboard Shortcuts").OrThrow()) Print(a);

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
				//	PrintList(!w1.Is0, !w2.Is0);
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
		//Acc.Find(Wnd.Find("Quick*"), "PUSHBUTTON", "Options*").MouseClick();

		//var w = Wnd.Find("**c|*Firefox").OrThrow();
		//Print(w);
		//var a = Acc.Find(w, "div", "Google").OrThrow();
		////a.MouseMove();
		//Perf.First();
		//for(int i = 0; i < 5; i++) {
		//	var si = ScreenImage.Find(@"Q:\app\Au\Tests\Images\google.bmp", a, SIFlags.WindowDC).OrThrow();
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

	static void TestAccToUIElem()
	{
		var w = Wnd.Find("Properties*").OrThrow();
		var a = Acc.Find(w, "PUSHBUTTON", "OK").OrThrow();
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
			//role = "PUSHBUTTON";

			//name = "Bug Reports";
			name = "???????*";
			//name = "Five";
			//name = "Board index";
			//name = "**p|-(Untitled";

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
			//p.role = "web:PUSHBUTTON";
			//p.role = "LISTITEM";
			//p.role = "id=-31772:LISTITEM";
			//p.role = "class=SysListView32:LISTITEM";
			//p.role = "id=7:LINK";
			//p.role = "id=0xa:LINK";
			//p.role = "class=Butt*:PUSHBUTTON";
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
			//p.name = "**p|^U.+d$";
			//p.name = "**g|Untitled";
			//p.prop = "value=XXX\0  a:href=YYY\0\r\n description=DDD";
			//p.prop = "maxLevel=15";
			//p.prop = "notin=ONE,TWO,THREE";
			//p.prop = "notin=PUSHBUTTON,MENUBAR,STATICTEXT";
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
				//var hr = Cpp.Cpp_AccFind(true, w, default, "PUSHBUTTON", "Infobar Container", 0, null, 0, out var r);
				var hr = Cpp.Cpp_AccFind(true, w, default, ref p, null, out var r, out var errStr);
				//var hr = Cpp.Cpp_AccFind(true, w, default, ref p, callback, out var r, out var errStr);
				//var hr = Cpp.Cpp_AccFind(true, default, iaccParent, ref p, null, out var r, out var errStr);
				//var hr = Cpp.Cpp_AccFind(true, w, default, null, null, 0, callback, 0, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, aParent._iacc, "LINK", "Bug Reports", 0, null, 0, out var r);
				//var hr = Cpp.Cpp_AccFromWindow(true, w, 0, out var r);
				//var hr = Cpp.Cpp_AccFromWindow(true, w, (int)AccOBJID.CLIENT, out var r);
				//var hr = Cpp.Cpp_AccFromPoint(true, Mouse.XY, out var r);
				//var hr = Cpp.Cpp_AccFind(true, w, default, "PUSHBUTTON", null, 0, null, 0, out var r);
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
		PrintList(1, a);
		_ClickWait(w, a, "*Finance*", "Trending Now");

		//Acc.Find(w, null, "Back", prop: "notin=DOCUMENT").DoAction();
		//return;

		Perf.First();
		a = Acc.Wait(10, w, "web:LINK", "Industries");
		Perf.NW();
		PrintList(2, a);
		_ClickWait(w, a, "*Industry*", "My Portfolio & Markets");

		Perf.First();
		a = Acc.Wait(10, w, "web:LINK", "Steel*", flags: flags);
		Perf.NW();
		PrintList(3, a);
		//var w1 = a.WndContainer;
		//PrintList(w1.Handle, a);
		_ClickWait(w, a, "*Stock Screener*", "Conglomerates");

		Perf.First();
		a = Acc.Wait(10, w, "web:LINK", "Results List");
		//Perf.NW();
		PrintList(4, a);

		bool stop = 2 == TaskDialog.ShowEx("back", null, "1 Continue|2 Cancel", secondsTimeout: 3);
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
		var f = new Acc.Finder("web:PUSHBUTTON", "Search");
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

		//var a = Acc.Find(w, "CHECKBUTTON", flags: flags);
		//var a = Acc.Find(w, "CHECKBUTTON", prop: "state=CHECKED, !DISABLED\0 notin=ONE,,TWO", flags: flags);
		var a = Acc.Find(w, "CHECKBUTTON", prop: "state=CHECKED,FOCUSABLE, !FOCUSED\0", flags: flags);
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
		//var a = Acc.Find(w, "PUSHBUTTON", "Compil*").OrThrow();
		////var a = Acc.Find(w, "OUTLINEITEM", "init").OrThrow();

		//var w = Wnd.Find("* Internet Explorer").OrThrow();
		////var a = Acc.Find(w, "web:LINK", "Test").OrThrow();
		//var a = Acc.Find(w, "web:TEXT", "Test").OrThrow();

		//var w = Wnd.Find("QM# - Q:*").OrThrow();
		////var a = Acc.Find(w, "LISTITEM", "Test*").OrThrow();
		////var a = Acc.Find(w, "PAGETAB", "Find").OrThrow();
		//var a = Acc.Find(w, "CLIENT", "Panels").OrThrow();

		//var w = Wnd.Find("Test", "CabinetWClass").OrThrow();
		////var a = Acc.Find(w, "PUSHBUTTON", "New folder").OrThrow();
		////var a = Acc.Find(w, "CLIENT", "Test").OrThrow();
		//var a = Acc.Find(w, "PUSHBUTTON", "Minimize").OrThrow();

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
		a = a.Navigate(nav, 0, true).OrThrow();
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
		//var f = new Acc.Finder("OUTLINEITEM");
		//f.ResultGetProperty = Acc.Finder.RProp.WndContainer;
		//if(f.Find(w1)) Print((Wnd)f.ResultProperty);
		//return;

		//var w = Wnd.Find("* Google Chrome").OrThrow();
		//var a = Acc.Find(w, "web:LINK", "Test").OrThrow();

		//var w = Wnd.Find(null, "QM_Editor").OrThrow();
		////var a = Acc.Find(w, "PUSHBUTTON", "Compil*").OrThrow();
		//var a = Acc.Find(w, "PUSHBUTTON", skip: 5, flags: AFFlags.NotInProc).OrThrow();

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
					//if(tt >= 2000) PrintList("<><c 0x8000>", tt, a, "</c>");

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
			if(tt >= 10000) PrintList(tt, a, a.WndTopLevel);
#endif
			//a._misc.flags = 0;
			//lenNIP += a.Name.Length;

		}
		Perf.NW();
		Print(sb);
		PrintList(k.Count, lenNIP, lenIP);
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
		PrintList(k.Count, g);
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

		//var a = Acc.Find(w, "PUSHBUTTON", "Prev*", flags: flags).OrThrow();
		//a.DoAction();

		//var a = Acc.Find(w, "TEXT", flags: flags).OrThrow();
		//a.Value = "TEST";

		//var a = Acc.Find(w, "CHECKBUTTON", "Unicode", flags: flags).OrThrow();
		//var a = Acc.Find(w, "LISTITEM", "Mouse", flags: flags).OrThrow();
		//var a = Acc.Find(w, "LIST", flags: flags).OrThrow();
		//var a = Acc.Find(w, "COMBOBOX", flags: flags).OrThrow();
		//var a = Acc.Find(w, "class=bosa_sdm_Microsoft Office Word 11.0:COMBOBOX", flags: flags).OrThrow();
		//var a = Acc.Find(w, "class=bosa_sdm_Microsoft Office Word 11.0:LIST", flags: flags).OrThrow();
		//var a = Acc.Find(w, "COMBOBOX", flags: flags).OrThrow().Navigate("fi");
		var a = Acc.Find(w, "LINK", "**p|^(T|G)", flags: flags).OrThrow();
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
		var a = Acc.Find(w, "PUSHBUTTON", "*PDF", flags: flags).OrThrow();
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
		//var a = Acc.Find(w, "CHECKBUTTON").OrThrow();
		var a = Acc.Find(w, "CHECKBUTTON", null, "state=DISABLED", flags: flags).OrThrow();
		//var a = Acc.Find(w, "CHECKBUTTON", null, "state=1", flags: flags).OrThrow();
		Print(a);
		//a = Acc.Find(w, "CHECKBUTTON", null, $"rect={a.Rect}", flags: flags).OrThrow();
		//a = Acc.Find(w, "CHECKBUTTON", null, "rect={L=1155 T=1182 W=132 H=13}", flags: flags).OrThrow();
		a = Acc.Find(w, "CHECKBUTTON", null, "rect={L=1155 T=1182 W=132 H=13}", flags: flags).OrThrow();
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
		var a = Acc.Find(w, "CHECKBUTTON", "C/C++").OrThrow();
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
			//a= Acc.Find(w, "PUSHBUTTON", prop: "elem=12", flags: flags).OrThrow();
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
		//Print(a.Find("PUSHBUTTON", "T*"));
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
		//var a = Acc.Find(w, "OUTLINEITEM", "Test Projects", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "OUTLINEITEM", "Downloads", flags: AFFlags.UIAutomation| AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "OUTLINEITEM", "Downloads", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "LISTITEM", "web", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "OUTLINE", flags: AFFlags.UIAutomation| AFFlags.NotInProc).OrThrow();
		//var a = Acc.Find(w, "OUTLINE", flags: AFFlags.UIAutomation).OrThrow();
		//var a = Acc.Find(w, "OUTLINEITEM").OrThrow();
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
		//var a = Acc.Find(w, "PUSHBUTTON", flags: AFFlags.UIA| AFFlags.NotInProc|AFFlags.HiddenToo| AFFlags.MenuToo);
		var a = Acc.Find(w, "PUSHBUTTON", flags: AFFlags.NotInProc | AFFlags.MenuToo);

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

		//PrintList(b1, b2, b3, b4);

		//var w = Wnd.Find("* Notepad").OrThrow();
		//w.SetTransparency(true, null, 0xF0F0F0);
		////w.SetTransparency(true, null, Color.Black);

		////int c = 0x0000ff; //blue
		////ColorInt c = 0x0000ff;
		////Color c = Color.FromArgb(0x0000ff);
		//int[] c = { 0x8000, 0x0000ff };
		////string[] c = { "one", "two" };
		//ScreenImage.Find(c, Wnd.Find("Quick *")).MouseMove();


	}

	//static void TestNativeWindow()
	//{
	//	var w = new NatWin();
	//	var c = new CreateParams() { Caption = "test", Height = 100, Width = 100, X = 100, Y = 100 };
	//	c.Style = unchecked((int)(Native.WS_POPUPWINDOW|Native.WS_VISIBLE));
	//	c.ExStyle = (int)Native.WS_EX_TOPMOST;

	//	w.CreateHandle(c);

	//	TaskDialog.Show("test");

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
	//	TaskDialog.Show("test");
	//}

	//static void _DomainCallback()
	//{
	//	Output.LibWriteToQM2 = true;

	//	var d =AppDomain.CurrentDomain;
	//	PrintList(d.Id, d.FriendlyName);
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
	//	TaskDialog.Show("--");
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
		TaskDialog.Show("--");
		x.Destroy();

		//var b = new AuToolbar();
		//b["one"] = o => Print(o);
		//b.Visible = true;
		//TaskDialog.Show("--");
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
		TaskDialog.Show("test");
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
		//var a1 = Acc.Find(w1, "web:PUSHBUTTON", "Search").OrThrow().Navigate("pa").OrThrow();
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
		PrintList(prop.one, prop.two, more);
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
		//var r1 = ScreenImage.Find("example", w).OrThrow();
		//var r2 = +ScreenImage.Find("example", w); //the same
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

	static void TestAccForm()
	{
#if true
		var w = Wnd.Find("* Chrome").OrThrow();
		var a = Acc.Find(w, "web:PUSHBUTTON", "Search").OrThrow();
		//var a = Acc.Find(w, "web:CELL", "Posts").OrThrow();

		//var w = Wnd.Find("* Firefox").OrThrow();
		//var a = Acc.Find(w, "web:TEXT", "Search the Web").OrThrow();

		//var w = Wnd.Find("Quick *").OrThrow();
		//var a = Acc.Find(w, "PUSHBUTTON", "Properties*").OrThrow();

		//foreach(var k in Wnd.Misc.AllWindows(false)) {
		//	//if(!k.ClassNameIs("Windows.UI.Core.CoreWindow")) continue;
		//	//if(!k.IsVisible || k.IsVisibleEx) continue;
		//	if(!k.IsCloaked) continue;
		//	var s = k.Name; if(Empty(s) || s=="Default IME" || s== "MSCTFIME UI") continue;
		//	Print(k);
		//	//PrintList(k.IsVisible, k.IsCloaked);
		//	//continue;
		//	foreach(var c in k.AllChildren()) {
		//		Print($"\t{c.ToString()}");
		//	}
		//}
		//return;

		//var w = Wnd.Find("Microsoft Edge", "Windows.UI.Core.CoreWindow", flags: WFFlags.HiddenToo).OrThrow();
		//var a = Acc.Find(w, "PANE", flags: AFFlags.HiddenToo).OrThrow();

		//a = a.Navigate("pr");
		Perf.First();
		var f = new Au.Tools.Form_Acc(a);
#else
		Perf.First();
		var f = new Acc_form();
#endif
		f.ShowDialog();
		//TaskDialog.Show("-");
	}


	[HandleProcessCorruptedStateExceptions]
	static unsafe void TestMain()
	{
#if DEBUG
		//Output.IgnoreConsole = true;
		//Output.LogFile=@"Q:\Test\Au"+IntPtr.Size*8+".log";
#endif
		Output.LibWriteToQM2 = true;
		Output.RedirectConsoleOutput = true;
		if(!Output.IsWritingToConsole) {
			Output.Clear();
			//100.ms();
		}

		try {

			try {

				TestAccForm();
				//TestAccThrowOperator();
			}
			finally {
				Cpp.Cpp_Unload();
			}
		}
		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }


		/*

		using static AuAlias;

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

