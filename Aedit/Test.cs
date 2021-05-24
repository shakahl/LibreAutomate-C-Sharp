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
//using System.Linq;
//using System.Xml.Linq;
//using System.Runtime;

//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Interop;
//using System.Windows.Input;

using Au;
using Au.Types;
using Au.Util;
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
//using Microsoft.CodeAnalysis.Host.Mef;

//using DiffMatchPatch;
//using System.Globalization;
//using System.Windows.Media.Imaging;
//using System.Resources;

#if TRACE

#pragma warning disable 169

static unsafe class Test
{

	public static void FromMenubar() {
		//Cpp.Cpp_Test();

		//var s = Panels.Editor.ZActiveDoc.zGetString0Terminated(Sci.SCI_GETSELTEXT, 0, 1000);
		//AOutput.Write(s);
		//var s2 = Panels.Editor.ZActiveDoc.zGetStringGetLength(Sci.SCI_GETSELTEXT, 0);
		//AOutput.Write(s2);
		//var s3 = Panels.Editor.ZActiveDoc.zGetStringOfLength(Sci.SCI_GETSELTEXT, 0, 5);
		//AOutput.Write(s2);

		//InsertCode.UsingDirective("Test.Usings");

		//EdDatabases.CreateWinapi();

		//var code = App.Settings.ci_usings.RegexReplace(@"(?m)^.+$", "using $0;");
		////var code = "using Au.Types;";
		////AOutput.Write(code);

		//var tree = CSharpSyntaxTree.ParseText(code, encoding: Encoding.UTF8);
		//var comp = CSharpCompilation.Create("f", new SyntaxTree[] { tree }, new MetaReferences().Refs);
		//var m = comp.GetSemanticModel(tree, false) as SyntaxTreeSemanticModel;
		//APerf.First();
		//foreach (var v in m.LookupNamespacesAndTypes(code.Length)) {
		//	//AOutput.Write(v.Kind, v);
		//	if (v.Kind != SymbolKind.NamedType) continue;
		//	AOutput.Write(v);

		//}

		//foreach (var v in m.LookupStaticMembers(code.Length)) {
		//	if (v.Kind == SymbolKind.Namespace) continue;
		//	//AOutput.Write(v.Kind, v);

		//}
		//foreach (var v in m.LookupSymbols(code.Length)) {
		//	if (v.Kind == SymbolKind.Namespace) continue;
		//	//AOutput.Write(v.Kind, v);

		//}



		//for (int i = 0; i < 30; i++) {
		//	Menus.Run.Compile();
		//	Menus.Run.Start();
		//}
		//AOutput.Write("done");

		//AOutput.W
		//AKeys.Key("");

		//AThread.Start(() => {
		//	Cpp.Cpp_Test();

		////	POINT p = (936, 392);
		////	p = (1468, 1653);
		////	var a = AAcc.FromXY(p);
		////	//var a=AAcc.FromXY(p, AXYFlags.NotInProc);
		////	//AOutput.Write(a.Role);
		////	a.Dispose();
		//});


		//KScintilla k; k.

		//AWnd.More.CreateWindow("Edit", null, WS.CHILD | WS.VISIBLE, 0, 0, 0, 50, 20, Api.GetFocus()).Focus();
		//var v = App.FocusedElement;
		//AOutput.Write(v);
	}

	class TestGC
	{
		~TestGC() {
			if (Environment.HasShutdownStarted) return;
			if (AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			AOutput.Write("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//ATimer.After(1, _ => new TestGC());
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

		//	//ATimer.Every(50, _ => {
		//	//	if(!s_debug) {
		//	//		s_debug = true;
		//	//		ATimer.After(100, _ => new TestGC());
		//	//	}
		//	//});
		//}
	}
}
#endif
