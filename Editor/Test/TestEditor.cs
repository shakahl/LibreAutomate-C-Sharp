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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

#if TEST
partial class ThisIsNotAFormFile { }

partial class EForm
{
	internal void TestEditor()
	{
		var f = new Au.Tools.Form_Wnd();
		//var f = new Au.Tools.Form_Acc();
		//var f = new Au.Tools.Form_WinImage();
		//Wnd.GetWnd.Root.Activate();100.ms();
		f.Show(this);
		//f.ShowDialog();
		//f.Dispose();
		f.FormClosed += (unu, sed) => Print(f.DialogResult, f.ResultCode);


		//Panels.Status.SetText("same thread\r\nline2\r\nline3");
		//Task.Run(() => { 2.s(); Panels.Status.SetText("other thread, WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM"); });
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

	public static void TestParsing()
	{
		var code = Panels.Editor.ActiveDoc.Text;

		var sRef = new string[] { typeof(object).Assembly.Location, Folders.ThisApp + "Au.dll" };
		//var sRef = new string[] { typeof(object).Assembly.Location };

		var references = new List<PortableExecutableReference>();
		foreach(var s in sRef) {
			//references.Add(MetadataReference.CreateFromFile())
			references.Add(MetadataReference.CreateFromFile(s));
		}

		//Microsoft.CodeAnalysis.Text.SourceText.From()
		//var po=new CSharpParseOptions(LanguageVersion.)
		var tree = CSharpSyntaxTree.ParseText(code);

		//Print(tree.);



		//var options = new CSharpCompilationOptions(OutputKind.WindowsApplication, allowUnsafe: true);
		//var compilation = CSharpCompilation.Create(name, new[] { tree }, references, options);
	}
}
#endif
