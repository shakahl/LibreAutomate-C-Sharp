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
using static Au.AStatic;
using static Program;
using Au.Controls;
using static Au.Controls.Sci;
//using Au.Intellisense;
//using DiffMatchPatch;

#if TEST

#pragma warning disable 169

partial class ThisIsNotAFormFile { }

partial class FMain
{
	//CaCompletion _compl;

	internal unsafe void TestEditor()
	{

		AOutput.Clear();

		var doc = Panels.Editor.ActiveDoc;
		//var doc = Panels.Output.Controls[0] as AuScintilla;
		var t = doc.ST;
		var s = doc.Text;

		//var position = t.CountBytesToChars(0, t.CurrentPos);

		//APerf.First();
		////var _compl = new CaCompletion(null);
		//if(_compl == null) _compl = new CaCompletion(null);
		//APerf.Next();

		//var a = _compl.GetCompletions(s, position, true);
		//APerf.NW();
		//Print(a);

		//Print(Control.FromHandle(Api.GetFocus().Handle));
		//Print(MainForm.AcceptButton);
		//return;

		//TestMC();
		////TestDragDrop();
		//return;

		//var th = new Thread(()=>
		//{
		//	Thread.Sleep(-1);
		//});
		//th.SetApartmentState(ApartmentState.STA);
		//th.IsBackground = true;
		//th.Start();


		//Settings.Set("test.multiline", "a\r\nb");
		//Print(Settings.GetString("test.multiline").Length);

		//var file = @"Q:\Test\test1.xml";
		//var s = "<x>a\r\nb</x>";
		//File.WriteAllText(file, s);

		////var x = XElement.Load(file);
		////var x = AExtXml.LoadElem(file);
		////var x = XDocument.Load(file).Root;
		//var x = AExtXml.LoadDoc(file).Root;
		//Print(x.Value.Length);


		//return;


		//int n = t.Call(SCI_GETWORDCHARS, 0, 0);
		//var s = t.GetString(SCI_GETWORDCHARS, 0);
		//Print(n);
		//Print(s);


		//AOutput.QM2.Write(s);

		//int len=t.TextLengthBytes;
		//var b = stackalloc byte[len * 2 + 2];

		//Sci.Sci_TextRange tr = default;
		//tr.chrg.cpMax = len;
		//tr.lpstrText = b;
		//t.Call(Sci.SCI_GETSTYLEDTEXT, 0, &tr);
		//for(int i = 0; i < len*2; i++) {
		//	Print(b[i]);
		//}

		//t.Call(SCI_FOLDALL);
		//for(int i = 0; i < 3; i++) Print((uint)t.Call(SCI_GETFOLDLEVEL, i));
		//return;

		//AOutput.Clear();
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


		//var a = s.SegLines();
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

		//for(int i = 0; i < 5; i++) Print((uint)doc.Call(Sci.SCI_GETFOLDLEVEL, i));
		//return;

		//Panels.Status.SetText("same thread\r\nline2\r\nline3");
		//Task.Run(() => { 2.s(); Panels.Status.SetText("other thread, WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM"); });

		//TestTools();
		//TestReplaceFile();

		//doc.CommentLines(!AKeys.IsShift);

		//Au.Triggers.ActionTriggers.DisabledEverywhere ^= true;

		//		var s1 = @"//{{
		////{{ using
		//using Au; using static Au.AStatic; using Au.Types; using System; using System.Collections.Generic; //}}
		////{{ main
		//class Script :AScript { [STAThread] static void Main(string[] args) { new Script()._Main(args); } void _Main(string[] args) { //}}//}}//}}//}}
		//";
		//		var s2 = @"/*/ role exeProgram; outputPath %AFolders.Workspace%\bin; console true; /*/ //{{
		////{{ using
		//using Au; using static Au.AStatic; using Au.Types; using System; using System.Collections.Generic; //}}
		//using My.NS1; //ąčę îôû
		//using My.NS2;
		////{{ main
		//class Script :AScript { [STAThread] static void Main(string[] args) { new Script()._Main(args); } void _Main(string[] args) { //}}//}}//}}//}}
		//";

		//		var dmp = new diff_match_patch();
		//		List<Diff> diff = dmp.diff_main(s1, s2, true);
		//		dmp.diff_cleanupSemantic(diff);
		//		var delta = dmp.diff_toDelta(diff);
		//		Print(delta);
		//		Print("----");
		//		var d2 = dmp.diff_fromDelta(s1, delta);
		//		//Print(d2);
		//		Print(dmp.diff_text2(d2));

		//Print(t.CountBytesFromChars(2), t.CountBytesFromChars(4, 2));
		//Print(t.CountBytesToChars(0, 4));

	}
	//static bool s_test1;

	public unsafe void TestOld()
	{
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\any.dll,-85"));
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\a.bmp"));
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\.bmp"));
		//Print(EImageUtil.ImageTypeFromString(true, @"C:\any.ico"));
		//Print(EImageUtil.ImageTypeFromString(true, @"\\a\b\any.png"));
		//Print(EImageUtil.ImageTypeFromString(true, @"~:123456"));
		//Print(EImageUtil.ImageTypeFromString(true, @"resource:mmm"));

		//_img.ClearCache();

		//for(int i = 0; i < 10; i++) Print($"{i}: '{_t.AnnotationText(i)}'");
		//for(int i = 0; i < 10; i++) _t.AnnotationText(i, "||||new text");
		//for(int i = 0; i < 10; i++) _t.AnnotationText(i, null);

		//_t.AnnotationText(0, "Test\nAnnotations");
		//_t.AnnotationText(0, Empty(_t.AnnotationText(0)) ? "Test\nAnnotations" : "");
		//_t.AnnotationText(0, (_t.AnnotationText(0).Length<5) ? "Test\nAnnotations" : "abc");

		//Print(_c.Images.Visible);

		//switch(_c.Images.Visible) {
		//case Sci.AnnotationsVisible.ANNOTATION_HIDDEN:
		//	_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_STANDARD;
		//	//_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_BOXED;
		//	break;
		//default:
		//	_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_HIDDEN;
		//	//_c.Images.Visible = Sci.AnnotationsVisible.ANNOTATION_BOXED;
		//	break;
		//}

		//switch((Sci.AnnotationsVisible)(int)_c.Call(Sci.SCI_ANNOTATIONGETVISIBLE)) {
		//case Sci.AnnotationsVisible.ANNOTATION_HIDDEN:
		//	_c.Call(Sci.SCI_ANNOTATIONSETVISIBLE, (int)Sci.AnnotationsVisible.ANNOTATION_STANDARD);
		//	break;
		//default:
		//	_c.Call(Sci.SCI_ANNOTATIONSETVISIBLE, (int)Sci.AnnotationsVisible.ANNOTATION_HIDDEN);
		//	break;
		//}

		//var o = Panels.Output;
		//o.Write(@"Three green strips: <image ""C:\Users\G\Documents\Untitled.bmp"">");
		//Print(_c.Text);
		AOutput.Clear();
		//Print(_activeDoc?.Text);
		//_c.Text = "";

		//Print("one\0two");
		//Print("<><c 0x8000>one\0two</c>");


		//foreach(var f in AFile.EnumDirectory(AFolders.ProgramFiles, FEFlags.AndSubdirectories | FEFlags.IgnoreAccessDeniedErrors)) {
		//	if(f.IsDirectory) continue;
		//	if(0 == f.Name.Ends(true, ".png", ".bmp", ".jpg", ".gif", ".ico")) continue;
		//	//Print(f.FullPath);
		//	MainForm.Panels.AOutput.Write($"<image \"{f.FullPath}\">");
		//	ATime.DoEvents();
		//}
	}
	//static bool _debugOnce;

	void TestMC()
	{
		//var code = Model.CurrentFile.GetText();

		var t=Panels.Editor.ActiveDoc.ST;
		//APerf.First();
		//t.ReplaceRange(2, 4, "NEW TEXT");
		//t.ReplaceRange(2, 4, "NEW TEXT", true, true);
		//t.DeleteRange(2, 3);
		//t.InsertText(2, "TEXT");
		//APerf.NW();

		//var s=t.GetText();
		//Print(s.Length, s, t.Call(SCI_POSITIONRELATIVE, 0, 1), t.Call(SCI_COUNTCHARACTERS, 0, 4));

		foreach(var f in Model.Root.Descendants()) {
			var k = f.GetClassFileRole();
			if(k!= default) Print(k, f.ItemPath);
		}
	}

	void TestDragDrop()
	{
		//var f = new DDForm();
		//f.Show();

		AThread.Start(() => {
			var f = new DDForm();
			var c = new TextBox(); f.Controls.Add(c);
			f.ShowDialog();
			f.Dispose();
		});
	}

	class DDForm : Form
	{
		public DDForm()
		{
			AllowDrop = true;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(1250, 1350);
		}

		protected override void OnDragEnter(DragEventArgs d)
		{
			Print("enter", d.AllowedEffect);
			d.Effect = d.AllowedEffect;
			base.OnDragEnter(d);
		}

		protected override void OnDragOver(DragEventArgs d)
		{
			Print("over", d.AllowedEffect);
			d.Effect = d.AllowedEffect;
			base.OnDragOver(d);
		}

		protected override void OnDragDrop(DragEventArgs d)
		{
			Print("drop", d.AllowedEffect);
			d.Effect = d.AllowedEffect;
			base.OnDragDrop(d);
		}
	}

	void TestTools()
	{
		var f = new Au.Tools.Form_Wnd(AWnd.Find("Quick*"));
		//var f = new Au.Tools.Form_Acc();
		//var f = new Au.Tools.Form_WinImage();
		//AWnd.GetWnd.Root.Activate();100.ms();
		f.Show(this);
		//f.ShowDialog();
		//f.Dispose();
		//f.FormClosed += (unu, sed) => Print(f.DialogResult, f.ResultCode);

	}

	//void TestReplaceFile()
	//{
	//	var settFile = AFolders.ThisAppDocuments + @"!Settings\Settings2.xml";
	//	lock(Settings) {
	//		for(int i = 0; i < 300; i++) {
	//			try {
	//				Settings.Set("test", i);
	//				Settings.Xml.SaveElem(settFile);
	//			}catch(Exception e) {
	//				Print(e.ToStringWithoutStack(), (uint)e.HResult, i);
	//				break;
	//			}
	//			1.ms();
	//		}
	//	}
	//	Print("OK");
	//}

	//private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	//{
	//	Print("resolve", args.Name, args.RequestingAssembly);
	//	foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) if(v.FullName == args.Name) { Print("already loaded"); return v; }


	//	return null;
	//}

	//private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
	//{
	//	Print("load", args.LoadedAssembly);
	//}

	//public static void TestParsing()
	//{
	//	var code = Panels.Editor.ActiveDoc.Text;

	//	var sRef = new string[] { typeof(object).Assembly.Location, AFolders.ThisApp + "Au.dll" };
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

	void SetHookToMonitorCreatedWindowsOfThisThread()
	{
		_hook = AHookWin.ThreadCbt(x => {
			if(x.code == HookData.CbtEvent.CREATEWND) {
				var w = (AWnd)x.wParam;
				Print(w);
				//var c = Control.FromHandle(w.Handle); //always null
				//if(c != null) Print(c); else Print(w);
			}
			return false;
		});
		Application.ApplicationExit += (unu, sed) => _hook.Dispose(); //without it at exit crashes (tested with raw API and not with AHookWin) 
	}
	static AHookWin _hook;
}
#endif
