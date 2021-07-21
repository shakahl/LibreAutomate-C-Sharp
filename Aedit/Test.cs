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
using System.Drawing;

#if TRACE

#pragma warning disable 169

class Cty
{
	int i;
	int i3;

	void Vo() {

	}

	int i2;
	int i4;

	void Vo2() { }
	int i5;

	void Vo3() { }
	void Vo4() { }

}

static unsafe class Test
{
	//static void _ColorQuantizer() {
	//	for (int i = 0; i < 8; i++) {
	//		var r = new RECT(30, i * 100, 32, 32);
	//		var a = ColorQuantizer.MakeScreenshotComment(r);
	//	}
	//}

	//static void ModelFind() {
	//	print.clear();
	//	//var names = new string[] { "using.cs", "Class2.cs", "email.ico" };
	//	var names = Panels.Editor.ZActiveDoc.zText.Lines();
	//	var kind = FNFind.Any;
	//	foreach (var name in names) {
	//		if (name.NE()) break;
	//		if (name.Starts('*')) {
	//			kind = Enum.Parse<FNFind>(name[1..]);
	//			continue;
	//		}
	//		if (name.Starts('/')) continue;
	//		var p1 = perf.local();
	//		var v1 = App.Model.Find(name, kind);
	//		p1.Next();
	//		//p1.Write();
	//		print.it(name, v1?.ItemPath);
	//	}

	//	//FileNode.test = keys.isScrollLock;
	//	//string path = @"\Classes\global.cs", name = "global.cs";
	//	//string path = @"\old\Class@.cs", name = "Class@.cs";
	//	//FNFind kind = FNFind.Class;
	//	//kind = FNFind.Any;

	//	//var v1 = App.Model.Find(path, kind);
	//	//var v2 = App.Model.Find(name, kind);
	//	//print.it(v1, v2);

	//	//perf.cpu();
	//	//for (int i1 = 0; i1 < 7; i1++) {
	//	//	int n2 = 1000;
	//	//	perf.first();
	//	//	for (int i2 = 0; i2 < n2; i2++) { App.Model.Find(path, kind); }
	//	//	perf.next();
	//	//	for (int i2 = 0; i2 < n2; i2++) { App.Model.Find(name, kind); }
	//	//	perf.next();
	//	//	for (int i2 = 0; i2 < n2; i2++) { }
	//	//	perf.next();
	//	//	for (int i2 = 0; i2 < n2; i2++) { }
	//	//	perf.nw();
	//	//	//100.ms();
	//	//}

	//	//Debug_.MemorySetAnchor_();
	//	//for(int i2 = 0; i2 < 1000; i2++) {App.Model.Find(path, kind); }
	//	//Debug_.MemoryPrint_();
	//	//Debug_.MemorySetAnchor_();
	//	//for(int i2 = 0; i2 < 1000; i2++) {App.Model.Find(name, kind); }
	//	//Debug_.MemoryPrint_();
	//}

	//static void CompressBrotli() {
	//	var dir = @"Q:\Test\bmp";
	//	var dir2 = dir + "2";
	//	filesystem.createDirectory(dir2);
	//	foreach (var f in Directory.GetFiles(dir, "*.bmp")) {
	//		var b = Image.FromFile(f) as Bitmap;
	//		var a = ColorQuantizer.Quantize(b, 16);
	//		File.WriteAllBytes(dir2 + "\\" + pathname.getName(f), a);
	//	}
	//}

	//static void _SpeedIEnumerableToListVsToArray() {
	//	using var pi = perf.local();
	//	var e = App.Model.Root.Descendants();

	//	perf.cpu();
	//	//Debug_.MemorySetAnchor_();
	//	long n = 0;
	//	using var nogc = new Debug_.NoGcRegion(100_000_000);
	//	for (int i1 = 0; i1 < 7; i1++) {
	//		int n2 = 10;
	//		perf.first();
	//		for (int i2 = 0; i2 < n2; i2++) { var a = e.ToList(); n += a.Count; } //ToList 2.217. ToArray 1.877. ToList.ToArray 2.958.
	//		perf.next();
	//		//for (int i2 = 0; i2 < n2; i2++) { var a = e.ToArray(); n += a.Length; } //ToList 2.217. ToArray 1.877. ToList.ToArray 2.958.
	//		//perf.next();
	//		//for (int i2 = 0; i2 < n2; i2++) { n += e.Count(); }
	//		//perf.next();
	//		for (int i2 = 0; i2 < n2; i2++) { }
	//		perf.next();
	//		for (int i2 = 0; i2 < n2; i2++) { }
	//		perf.nw();
	//		//100.ms();
	//	}
	//	//Debug_.MemoryPrint_();
	//	print.it(n);
	//}

	public static void FromMenubar() {
		//byte[] a=new byte[1]
		//print.it()
		//List<>


		//_ColorQuantizer();
		//ModelFind();
		//CompressBrotli();
		//_SpeedIEnumerableToListVsToArray();
		//return;

		//MetaReferences.DebugPrintCachedRefs();

		var doc = Panels.Editor.ZActiveDoc;
		//doc.test_ = true;
		print.it(doc.zCurrentPos16);
		//doc.TestHidden();
		//doc.TestIndicators();
		//doc.Call(SCI_SETSCROLLWIDTH, 1);

		//Sci_GetVisibleRange(doc.ZSciPtr, 1, out var si); //fast
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

		//WndUtil.CreateWindow("Edit", null, WS.CHILD | WS.VISIBLE, 0, 0, 0, 50, 20, Api.GetFocus()).Focus();
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
