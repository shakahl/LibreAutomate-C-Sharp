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
using System.Linq;
//using System.Xml.Linq;
//using System.Runtime;

//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Interop;
//using System.Windows.Input;

using Au;
using Au.Types;
using Au.More;
using Au.Tools;
//using Au.Controls;
//using static Au.Controls.Sci;
using Au.Compiler;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using Au.Controls;
using System.Windows.Forms;
//using Microsoft.CodeAnalysis.Host.Mef;

//using DiffMatchPatch;
//using System.Globalization;
//using System.Windows.Media.Imaging;
//using System.Resources;

using static Au.Controls.Sci;

#if TRACE

#pragma warning disable 169

//namespace Au.More
//{
//class uConvert {  }
//class Uconvert {  }
//class UConvert {  }
//class convertutil {  }
//}

static unsafe class Test
{

	public static void FromMenubar() {





		//var doc = Panels.Editor.ZActiveDoc;

		//Sci_GetStylingInfo(doc.ZSciPtr, 15, out var si); //fast
		//print.it($"pos={doc.zCurrentPos8}, endStyled={si.endStyled}, endStyledLineStart={si.endStyledLineStart}, endStyledLine={si.endStyledLine+1}, visibleFrom={si.visibleFrom}, visibleFromLine={si.visibleFromLine+1}, visibleTo={si.visibleTo}, visibleToLine={si.visibleToLine+1}");

		//Cpp.Cpp_Test();

		//run.it("fffffffffffffffff.exe");

		//InsertCode.UsingDirective("Test.Usings");

		//EdDatabases.CreateWinapi();

		//var code = App.Settings.ci_usings.RegexReplace(@"(?m)^.+$", "using $0;");
		////var code = "using Au.Types;";
		////print.it(code);

		//var tree = CSharpSyntaxTree.ParseText(code, encoding: Encoding.UTF8);
		//var comp = CSharpCompilation.Create("f", new SyntaxTree[] { tree }, new MetaReferences().Refs);
		//var m = comp.GetSemanticModel(tree, false) as SyntaxTreeSemanticModel;
		//perf.first();
		//foreach (var v in m.LookupNamespacesAndTypes(code.Length)) {
		//	//print.it(v.Kind, v);
		//	if (v.Kind != SymbolKind.NamedType) continue;
		//	print.it(v);

		//}

		//foreach (var v in m.LookupStaticMembers(code.Length)) {
		//	if (v.Kind == SymbolKind.Namespace) continue;
		//	//print.it(v.Kind, v);

		//}
		//foreach (var v in m.LookupSymbols(code.Length)) {
		//	if (v.Kind == SymbolKind.Namespace) continue;
		//	//print.it(v.Kind, v);

		//}



		//for (int i = 0; i < 30; i++) {
		//	Menus.Run.Compile();
		//	Menus.Run.Start();
		//}
		//print.it("done");


		//run.thread(() => {
		//	Cpp.Cpp_Test();

		////	POINT p = (936, 392);
		////	p = (1468, 1653);
		////	var a = elm.FromXY(p);
		////	//var a=elm.FromXY(p, EXYFlags.NotInProc);
		////	//print.it(a.Role);
		////	a.Dispose();
		//});


		//KScintilla k; k.

		//wnd.more.createWindow("Edit", null, WS.CHILD | WS.VISIBLE, 0, 0, 0, 50, 20, Api.GetFocus()).Focus();
		//var v = App.FocusedElement;
		//print.it(v);
	}

	class TestGC
	{
		~TestGC() {
			if (Environment.HasShutdownStarted) return;
			if (AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			print.it("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//timerm.after(1, _ => new TestGC());
			//var f = App.Wmain; if(!f.IsHandleCreated) return;
			//f.BeginInvoke(new Action(() => new TestGC()));
			new TestGC();
		}
	}
	static bool s_debug2;

	public static void MonitorGC() {
		//if(!s_debug2) {
		//	s_debug2 = true;
		//	new TestGC();

		//	//timerm.every(50, _ => {
		//	//	if(!s_debug) {
		//	//		s_debug = true;
		//	//		timerm.after(100, _ => new TestGC());
		//	//	}
		//	//});
		//}
	}
}
#endif
