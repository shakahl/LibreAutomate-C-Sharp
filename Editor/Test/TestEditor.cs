using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;

#if TEST
partial class ThisIsNotAFormFile { }

partial class EForm
{
	internal void TestEditor()
	{

		//var th = new Thread(()=>
		//{
		//	Thread.Sleep(-1);
		//});
		//th.SetApartmentState(ApartmentState.STA);
		//th.IsBackground = true;
		//th.Start();

		//return;


		var doc = Panels.Editor.ActiveDoc;
		var t = doc.ST;
		var s = doc.Text;

		//Output.Clear();
		//Model.Save.TextNowIfNeed();
		//Compiler.ConvertCodeScriptToApp(Model.CurrentFile);

		//t.PositionBytes = 8;
		//Print(t.PositionBytes);
		//return;

		

		//Print("<><code>" + s + "</code>");

		//if(0!=t.Call(Sci.SCI_GETLINEVISIBLE, 2)) {
		//	t.Call(Sci.SCI_HIDELINES, 1, 3);
		//} else {
		//	t.Call(Sci.SCI_SHOWLINES, 1, 3);
		//}

		//t.Call(Sci.SCI_FOLDALL, 2);

		//t.StyleHidden(22, true);
		//t.StyleBackColor(22, Color.BlueViolet);
		//doc.Call(Sci.SCI_STARTSTYLING, 0);
		//doc.Call(Sci.SCI_SETSTYLING, 10, 22);


		var a = s.SplitLines_();
		//bool? folder = default; switch(a[1]) { case "1": folder = true; break; case "0": folder = false; break; }
		////Print(Model.Find(a[0], folder));
		////var fn = Model.Find("test scripts", true);
		////var fn = Model.Find("folder1", true);
		//var fn = Model.Root;
		//Print(fn.FindRelative(a[0], folder));

		//Print(Model.FindByFilePath(a[0]));
		//Print(Model.FindAll(a[0]));

		//if(!s_test1) {
		//	s_test1 = true;
		//	AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		//	AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		//}

		//Print(AppDomain.CurrentDomain.GetAssemblies());
		//return;

		var f = new Au.Tools.Form_Wnd(Wnd.Find("Quick*"));
		//var f = new Au.Tools.Form_Acc();
		//var f = new Au.Tools.Form_WinImage();
		//Wnd.GetWnd.Root.Activate();100.ms();
		f.Show(this);
		//f.ShowDialog();
		//f.Dispose();
		//f.FormClosed += (unu, sed) => Print(f.DialogResult, f.ResultCode);


		//Panels.Status.SetText("same thread\r\nline2\r\nline3");
		//Task.Run(() => { 2.s(); Panels.Status.SetText("other thread, WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM"); });
	}
	static bool s_test1;

	private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	{
		Print("resolve", args.Name, args.RequestingAssembly);
		foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) if(v.FullName == args.Name) { Print("already loaded"); return v; }


		return null;
	}

	private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
	{
		Print("load", args.LoadedAssembly);
	}

	void SetHookToMonitorCreatedWindowsOfThisThread()
	{
		_hook = Au.Util.WinHook.ThreadCbt(x =>
		{
			if(x.code == HookData.CbtEvent.CREATEWND) Print((Wnd)x.wParam);
			return false;
		});
		Application.ApplicationExit += (unu, sed) => _hook.Dispose(); //without it at exit crashes (tested with raw API and not with WinHook) 
	}
	static Au.Util.WinHook _hook;

	//public static void TestParsing()
	//{
	//	var code = Panels.Editor.ActiveDoc.Text;

	//	var sRef = new string[] { typeof(object).Assembly.Location, Folders.ThisApp + "Au.dll" };
	//	//var sRef = new string[] { typeof(object).Assembly.Location };

	//	var references = new List<PortableExecutableReference>();
	//	foreach(var s in sRef) {
	//		//references.Add(MetadataReference.CreateFromFile())
	//		references.Add(MetadataReference.CreateFromFile(s));
	//	}

	//	//Microsoft.CodeAnalysis.Text.SourceText.From()
	//	//var po=new CSharpParseOptions(LanguageVersion.)
	//	var tree = CSharpSyntaxTree.ParseText(code);

	//	//Print(tree.);



	//	//var options = new CSharpCompilationOptions(OutputKind.WindowsApplication, allowUnsafe: true);
	//	//var compilation = CSharpCompilation.Create(name, new[] { tree }, references, options);
	//}
}
#endif
