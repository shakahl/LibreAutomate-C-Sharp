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
using Au.Controls;
using static Au.Controls.Sci;
using Au.Compiler;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;

using DiffMatchPatch;
using System.Runtime;
using System.Globalization;

#if TEST

#pragma warning disable 169

partial class ThisIsNotAFormFile { }

partial class FMain
{
	//CaCompletion _compl;

	//NLog.Logger _log;

	//[MethodImpl(MethodImplOptions.NoInlining)]
	//void _TestNLog()
	//{
	//	APerf.Next();
	//	if(_log == null) {
	//		var config = new NLog.Config.LoggingConfiguration();
	//		var logfile = new NLog.Targets.FileTarget("logfile") { FileName = @"Q:\test\run.log" };
	//		config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);
	//		config.
	//		NLog.LogManager.Configuration = config;
	//		_log = NLog.LogManager.GetLogger("Au.Editor");
	//		APerf.Next();
	//	}

	//	_log.Info("one\r\ntwo");
	//	APerf.NW();

	//}

	//FileStream _log;


	private MetadataReference mscorlib;

	private MetadataReference Mscorlib {
		get {
			if(mscorlib == null) {
				mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			}

			return mscorlib;
		}
	}

	public void GetInScopeSymbols()
	{
		string source = @"
class C
{
}
class Program
{
    private static int i = 0;
    public static void Main()
    {
        int j = 0; j += i;
 
        // What symbols are in scope here?
    }
}";
		// Get position of the comment above.
		int position = source.IndexOf("//");

		var doc = Panels.Editor.ActiveDoc;
		source = doc.Text;
		position = doc.ST.CurrentPosChars;

		APerf.First();
		//var mr = new[] { Mscorlib };
		var mr = new Au.Compiler.MetaReferences().Refs;
		APerf.Next();
		SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(source);
		CSharpCompilation compilation = CSharpCompilation.Create("MyCompilation",
			syntaxTrees: new[] { tree }, references: mr);
		SemanticModel model = compilation.GetSemanticModel(tree);

		// Get 'all' symbols that are in scope at the above position. 
		ImmutableArray<ISymbol> symbols = model.LookupSymbols(position);
		APerf.NW();

		// Note: "Windows" only appears as a symbol at this location in Windows 8.1.
		//string results = string.Join("\r\n", symbols
		//	//.Where(symbol => symbol.Kind!= SymbolKind.NamedType)
		//	//.Select(symbol => symbol.ToDisplayString() + "    " + symbol.Kind)
		//	//.Select(symbol => symbol.Name + "    " + symbol.Kind + "    " + symbol.ContainingType?.Name)
		//	.Select(symbol => symbol.MetadataName)
		//	.OrderBy(s => s, StringComparer.Ordinal));

		var a = new List<string>();
		foreach(var v in symbols) {
			var s = v.MetadataName;
			int i = s.IndexOf('`');
			if(i > 0) {
				s = v.Name + "<>";
				if(a.Contains(s)) continue;
			}
			a.Add(s);
		}

		string results = string.Join("\r\n", a.OrderBy(s => s, StringComparer.OrdinalIgnoreCase));

		Print(results);
		return;
		//		Assert.Equal(@"C
		//j
		//Microsoft
		//object.~Object()
		//object.Equals(object)
		//object.Equals(object, object)
		//object.GetHashCode()
		//object.GetType()
		//object.MemberwiseClone()
		//object.ReferenceEquals(object, object)
		//object.ToString()
		//Program
		//Program.i
		//Program.Main()
		//System", results);

		// Filter results - get everything except instance members.
		symbols = model.LookupStaticMembers(position);

		// Note: "Windows" only appears as a symbol at this location in Windows 8.1.
		results = string.Join("\r\n", symbols.Select(symbol => symbol.ToDisplayString()).Where(s => s != "Windows").OrderBy(s => s));

		Print("----");
		Print(results);
		//		Assert.Equal(@"C
		//j
		//Microsoft
		//object.Equals(object, object)
		//object.ReferenceEquals(object, object)
		//Program
		//Program.i
		//Program.Main()
		//System", results);

		// Filter results by looking at Kind of returned symbols (only get locals and fields).
		results = string.Join("\r\n", symbols
			.Where(symbol => symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Field)
			.Select(symbol => symbol.ToDisplayString()).OrderBy(s => s));

		Print("----");
		Print(results);
		//		Assert.Equal(@"j
		//Program.i", results);
	}

	void _TestWorkspaces()
	{
		var doc = Panels.Editor.ActiveDoc;
		var source = doc.Text;
		var position = doc.ST.CurrentPosChars;

		APerf.First();
		//var mr = new[] { Mscorlib };
		var mr = new MetaReferences().Refs;
		APerf.Next();

		ProjectId project1Id = ProjectId.CreateNewId();
		DocumentId document1Id = DocumentId.CreateNewId(project1Id);

		Solution solution = new AdhocWorkspace().CurrentSolution
			.AddProject(project1Id, "Project1", "Project1", LanguageNames.CSharp)
			.AddMetadataReferences(project1Id, mr)
			.AddDocument(document1Id, "File1.cs", source);
		APerf.Next();

		var document = solution.GetDocument(document1Id);
		var completionService = CompletionService.GetService(document);
		APerf.Next();
		var all = completionService.GetCompletionsAsync(document, position).Result;
		APerf.NW();
		if(all == null) return;
		Print(all.Items);
	}

	void _TestWorkspaces2()
	{
		var doc = Panels.Editor.ActiveDoc;
		var source = doc.Text;
		var position = doc.ST.CurrentPosChars;

		var mr = new MetaReferences();
		APerf.Next();

		ProjectId project1Id = ProjectId.CreateNewId();
		DocumentId document1Id = DocumentId.CreateNewId(project1Id);

		Solution solution = new AdhocWorkspace().CurrentSolution
			.AddProject(project1Id, "Project1", "Project1", LanguageNames.CSharp)
			.AddMetadataReferences(project1Id, mr.Refs)
			.AddDocument(document1Id, "File1.cs", source);
		APerf.Next();

		var document = solution.GetDocument(document1Id);
		var completionService = CompletionService.GetService(document);
		APerf.Next();
		var all = completionService.GetCompletionsAsync(document, position).Result;
		APerf.NW();
		if(all == null) return;
		Print(all.Items);
	}

	void _TestWorkspaces3()
	{
		//GC.Collect(); GC.WaitForPendingFinalizers();

		APerf.First();
		var sol = new AdhocWorkspace().CurrentSolution;
		APerf.NW();

		APerf.Incremental = true;
		var hsDone = new HashSet<FileNode>();
		var adi = new List<DocumentInfo>();
		foreach(var f0 in Program.Model.Root.Descendants()) {
			var f = f0;
			if(!f.IsCodeFile) continue;
			if(f.Name == "5 M lines.cs") continue;
			if(f.FindProject(out var projFolder, out var projMain)) f = projMain;
			if(hsDone.Contains(f)) continue;
			hsDone.Add(f);
			//Print(f.ItemPath);

			APerf.First();
			var m = new MetaComments();
			if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo | EMPFlags.PrintErrors)) continue;
			var err = m.Errors;
			APerf.Next();

			ProjectId projectId = ProjectId.CreateNewId();
			adi.Clear();
			foreach(var f1 in m.CodeFiles) {
				var tav = TextAndVersion.Create(Microsoft.CodeAnalysis.Text.SourceText.From(f1.code, Encoding.UTF8), VersionStamp.Default, f1.f.FilePath);
				var di = DocumentInfo.Create(DocumentId.CreateNewId(projectId), f1.f.Name, null, SourceCodeKind.Regular, TextLoader.From(tav));
				adi.Add(di);
			}
			var pi = ProjectInfo.Create(projectId, VersionStamp.Default, f.Name, f.Name, LanguageNames.CSharp, null, null,
				m.CreateCompilationOptions(), m.CreateParseOptions(), adi,
				projectReferences: null, //TODO: create from m.ProjectReferences?
				m.References.Refs); //TODO: set outputRefFilePath if library?
			sol = sol.AddProject(pi);

			APerf.Next();

		}

		APerf.Write();

		//foreach(var proj in sol.Projects) {
		//	Print(proj.Name);
		//	var a = proj.Documents.ToArray();
		//	if(a.Length != 1) {
		//		Print("-- docs --");
		//		foreach(var v in a) Print("\t" + v.Name);
		//	}
		//}

		//MetaReferences.DebugPrintCachedRefs();
	}

	class CiWorkspace : Workspace
	{
		//public CaWorkspace(HostServices host) : base(host, WorkspaceKind.Host)
		public CiWorkspace() : base(MefHostServices.DefaultHost, WorkspaceKind.Host) //TODO: Roslyn throws/handles 2 exceptions because does not find Workspaces and Features assemblies for VB
		{

		}

		public Project AddProject(ProjectInfo projectInfo)
		{
			if(projectInfo == null) {
				throw new ArgumentNullException(nameof(projectInfo));
			}

			this.OnProjectAdded(projectInfo);

			this.UpdateReferencesAfterAdd();

			return this.CurrentSolution.GetProject(projectInfo.Id);
		}
	}

	void _TestWorkspaces4()
	{
		//if(!s_debug2) {
		//	_MonitorGC();
		//	return;
		//}

		200.ms();
		if(s_sol != null && AKeys.IsScrollLock) {
			_TestWorkspaces4_2();
			return;
		}

		//AOutput.Clear();
		APerf.First();
		var doc = Panels.Editor.ActiveDoc;
		var f = doc.FN;
		var f0 = f;
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		var m = new MetaComments();
		if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo)) {
			var err = m.Errors;
			err.PrintAll();
			return;
		}
		APerf.Next('M');
		//return;

		string code = null;
		DocumentId documentId = null;
		ProjectId projectId = ProjectId.CreateNewId();
		var adi = new List<DocumentInfo>();
		foreach(var f1 in m.CodeFiles) {
			var docId = DocumentId.CreateNewId(projectId);
			var tav = TextAndVersion.Create(SourceText.From(f1.code, Encoding.UTF8), VersionStamp.Default, f1.f.FilePath);
			adi.Add(DocumentInfo.Create(docId, f1.f.Name, null, SourceCodeKind.Regular, TextLoader.From(tav)));
			if(f1.f == f0) {
				documentId = docId;
				code = f1.code;
			}
		}
		var pi = ProjectInfo.Create(projectId, VersionStamp.Default, f.Name, f.Name, LanguageNames.CSharp, null, null,
			m.CreateCompilationOptions(), m.CreateParseOptions(), adi,
			projectReferences: null, //TODO: create from m.ProjectReferences?
			m.References.Refs); //TODO: set outputRefFilePath if library?
		APerf.Next('p');
		var ws = new AdhocWorkspace();
		//var ws = new CiWorkspace();
		//s_ws = ws;
		s_docId = documentId;
#if true
		var sol = ws.CurrentSolution;
		sol = sol.AddProject(pi); //does not change ws.CurrentSolution, which then is invalid
		s_sol = sol;
#else
		ws.AddProject(pi);
		var sol = ws.CurrentSolution;
#endif
		APerf.NW('P');
		var document = sol.GetDocument(documentId);
		var completionService = CompletionService.GetService(document);
		APerf.Next('s');

#if true
		Action act = () => {
			//APerf.SpeedUpCpu(); //55 -> 20 ms
			APerf.First();
			var position = doc.ST.CurrentPosChars;
			var r = completionService.GetCompletionsAsync(document, position).Result;
			if(r == null) { Print("null"); return; }
			//APerf.Next();
			//var span = r.Span;
			//var r2 = completionService.FilterItems(document, r.Items, code.Substring(span.Start, span.Length)); //returns r.Items
			//var k = completionService.GetChangeAsync(document, r.Items[0]).Result;

			APerf.NW();
			Print(r.Items.Length);
			//Print(r.Items.Length, k.TextChange);
			//Print(r.Items.Length, r2.Length);

			//var a = r.Items;
			//foreach(var v in a) {
			//	Print($"<><Z green>{v.DisplayText}<>");
			//	var d = completionService.GetDescriptionAsync(document, v).Result;
			//	Print(d.Text);
			//}
		};
#else
		Action act = () => {
			//APerf.SpeedUpCpu(); //7 -> 7 ms
			APerf.First();
			var position = doc.ST.CurrentPosChars;
			var model = document.GetSemanticModelAsync().Result;
			APerf.Next();
			var symbols = model.LookupSymbols(position);
			//var symbols = model.LookupLabels(position);
			//var symbols = model.LookupNamespacesAndTypes(position);
			//var symbols = model.LookupStaticMembers(position);
			//var symbols = model.LookupBaseMembers(position);
			//APerf.Next();
			//var r = symbols.OrderBy(sy => sy.ToDisplayString(), StringComparer.OrdinalIgnoreCase);
			APerf.NW();
			Print(symbols.Length);
		};
#endif

		act();
		int n = 0;
		//ATimer.Every(1000, tim => { if(n++ > 2) tim.Stop(); act(); });
	}
	//CiWorkspace s_ws;
	Solution s_sol;
	DocumentId s_docId;

	void _TestWorkspaces4_2()
	{
		var doc = Panels.Editor.ActiveDoc;
		var position = doc.ST.CurrentPosChars;
		APerf.First();
		s_sol = s_sol.WithDocumentText(s_docId, SourceText.From(doc.Text, Encoding.UTF8));
		var document = s_sol.GetDocument(s_docId);
		APerf.Next();
		int n = 0;
#if true
		var completionService = CompletionService.GetService(document);
		APerf.Next('s');
		var r = completionService.GetCompletionsAsync(document, position).Result;
		n = r?.Items.Length ?? 0;
#else
		if(!document.TryGetSemanticModel(out var model)) model = document.GetSemanticModelAsync().Result;
		APerf.Next();
		var symbols = model.LookupSymbols(position);
		n=symbols.Length;
#endif
		APerf.NW();
		Print(n);
	}

	//void _TestWorkspaces4_2()
	//{
	//	var doc = Panels.Editor.ActiveDoc;
	//	APerf.First();
	//	var sol = s_ws.CurrentSolution;
	//	var document = sol.GetDocument(s_docId);

	//	var position = doc.ST.CurrentPosChars;
	//	var model = document.GetSemanticModelAsync().Result;
	//	APerf.Next();
	//	var symbols = model.LookupSymbols(position);
	//	APerf.NW();
	//	Print(symbols.Length);
	//}

	//class _TextLoader : TextLoader
	//{
	//	FileNode _f;
	//	public _TextLoader(FileNode f) => _f = f;
	//	public override Task<TextAndVersion> LoadTextAndVersionAsync(Workspace workspace, DocumentId documentId, CancellationToken cancellationToken)
	//	{
	//		return new Task<TextAndVersion>(() => TextAndVersion.Create(Microsoft.CodeAnalysis.Text.SourceText.From(_f.GetText(), Encoding.UTF8), VersionStamp.Default, _f.FilePath));
	//	}
	//}

	//void _TestResources()
	//{
	//	//Print(Au.Editor.Properties.Resources.back);
	//	//Print(EdResources.GetObjectUseCache(nameof(Au.Editor.Properties.Resources.back)));
	//	//Print(EdResources.GetObjectUseCache2(nameof(Au.Editor.Properties.Resources.back)));

	//	//APerf.SpeedUpCpu();
	//	for(int i1 = 0; i1 < 5; i1++) {
	//		Thread.Sleep(200);
	//		int n2 = 100;
	//		APerf.First();
	//		//for(int i2 = 0; i2 < n2; i2++) { var v = Au.Editor.Properties.Resources.back; }
	//		APerf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { var v = EdResources.GetObjectUseCache(nameof(Au.Editor.Properties.Resources.back)); }
	//		APerf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { var v = EdResources.GetObjectUseCache2(nameof(Au.Editor.Properties.Resources.back)); }
	//		APerf.Next();
	//		for(int i2 = 0; i2 < n2; i2++) { }
	//		APerf.NW();
	//	}
	//}

	//public class AObjectCache<T> where T : class
	//{
	//	WeakReference<T> _wr;
	//	T _keeper;
	//	System.Threading.Timer _timer;

	//	public bool TryGet(out T x)
	//	{
	//		x = null;



	//		return true;
	//	}

	//	public void Add(T x, int timeoutMS)
	//	{

	//	}
	//}


	//public class SimpleListCache<T> where T : class
	//{
	//	WeakReference<List<T>> _wr;
	//	List<T> _a;

	//	public SimpleListCache()
	//	{
	//		Program.Timer1s += _Timer1s;
	//	}

	//	private void _Timer1s()
	//	{

	//	}

	//	public bool TryGet(out T x)
	//	{
	//		x = null;



	//		return true;
	//	}

	//	public void Add(T x, int secondsTimeout)
	//	{

	//	}
	//}


	void _TestGetItemText()
	{
		int n = 0;
		APerf.First();
		foreach(var f in Program.Model.Root.Descendants()) {
			if(!f.IsCodeFile) continue;
			if(f.Name == "5 M lines.cs") continue;
			n++;
			//Print(f.Name);
			//var s = f.GetText();
			AFile.GetProperties(f.FilePath, out var p, FAFlags.UseRawPath);
		}
		APerf.NW();
		Print(n);
	}

	void TestFileNodeTextCache()
	{
		AOutput.Clear();
		var f = Program.Model.CurrentFile;
		APerf.First();
		var s = f.GetText(saved: true, cache: true);
		APerf.NW();
		Print(s);
	}

	void TestReplaceTextGently()
	{
		var doc = Panels.Editor.ActiveDoc;
		var s1 = doc.Text;
		int i = s1.Find("//.");
		//var s2 = s1 + "added\r\n";
		var s2 = s1.Insert(i, "insert\r\n");
		doc.ReplaceTextGently(s2);
	}

	void TestDiffMatchPatch()
	{
		var s1 = @"//{{
using Au; using Au.Types; using static Au.AStatic; using System; using System.Collections.Generic;
class Script :AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //}}//}}//}}
	
	var s=""one"";
";
		var s2 = @"/*/ role exeProgram;		outputPath %AFolders.Workspace%\bin; console true; /*/ //{{
using Au; using Au.Types; using static Au.AStatic; using System; using System.Collections.Generic;
using My.NS1; //ąčę îôû
using My.NS2;
class Script :AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //}}//}}//}}
	var i=2;
";

		var dmp = new diff_match_patch();
		List<Diff> diff = dmp.diff_main(s1, s2, true);
		dmp.diff_cleanupSemantic(diff);
		var delta = dmp.diff_toDelta(diff);
		Print(delta);
		Print("----");
		var d2 = dmp.diff_fromDelta(s1, delta);
		//Print(d2);
		Print(dmp.diff_text2(d2));
	}

	void TestNoGcRegion()
	{
		for(int i = 0; i < 2; i++) {
			ADebug.LibMemorySetAnchor();
			bool noGC = GC.TryStartNoGCRegion(10_000_000);
			var a = new byte[50_000_000];
			for(int j = 0; j < a.Length; j++) a[j] = 1;
			Print(noGC, GCSettings.LatencyMode == GCLatencyMode.NoGCRegion);
			if(noGC && GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) try { GC.EndNoGCRegion(); } catch(InvalidOperationException ex) { ADebug.Print(ex.Message); }
			ADebug.LibMemoryPrint();
			GC.Collect();
			if(!ADialog.ShowYesNo("Continue?")) break;
		}

	}

	class TestGC
	{
		~TestGC()
		{
			if(Environment.HasShutdownStarted) return;
			if(AppDomain.CurrentDomain.IsFinalizingForUnload()) return;
			Print("GC", GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
			//ATimer.After(1, () => new TestGC());
			//var f = Program.MainForm; if(!f.IsHandleCreated) return;
			//f.BeginInvoke(new Action(() => new TestGC()));
			new TestGC();
		}
	}
	static bool s_debug2;

	void _MonitorGC()
	{
		return;
		if(!s_debug2) {
			s_debug2 = true;
			new TestGC();

			//ATimer.Every(50, () => {
			//	if(!s_debug) {
			//		s_debug = true;
			//		ATimer.After(100, () => new TestGC());
			//	}
			//});
		}
	}

	public unsafe void TestEditor()
	{


		//for(int i = 1; i < 0x10000; i++) {
		//	char c = (char)i;
		//	var k = char.GetUnicodeCategory(c);
		//	switch(k) {
		//	//case UnicodeCategory.LetterNumber:
		//	//case UnicodeCategory.LowercaseLetter:
		//	//case UnicodeCategory.ModifierLetter:
		//	//case UnicodeCategory.OtherLetter:
		//	//case UnicodeCategory.TitlecaseLetter:
		//	//case UnicodeCategory.UppercaseLetter:
		//	//case UnicodeCategory.SpacingCombiningMark:
		//	//case UnicodeCategory.NonSpacingMark:
		//	//case UnicodeCategory.DecimalDigitNumber:
		//	case UnicodeCategory.ConnectorPunctuation:
		//	//case UnicodeCategory.Format:
		//		Print($"u{i:x4} '{c}' {k}");
		//		break;
		//	}
		//	//Print($"u{i:x4} '{c}' {k}");
		//}



		//AOutput.Clear();
		//_TestGetItemText();
		//_TestResources();
		//_TestWorkspaces4();
		//_MonitorGC();
		//TestFileNodeTextCache();
		//TestReplaceTextGently();
		//TestDiffMatchPatch();
		//CodeInfo.Test();
		//MetaReferences.DebugPrintCachedRefs();
		//TestNoGcRegion();

		//for(int i = 0; i < 5; i++) {
		//	_ = new byte[1_000_000];
		//	GC.Collect();
		//	GC.WaitForPendingFinalizers();
		//	//1.ms();
		//}

		//return;


		//APerf.First();
		//_TestNLog();

		//var file = AFolders.ThisAppTemp + "run.log";
		//_log?.Close();
		////_log = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
		//_log = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
		////_log = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.Write);
		////_log.Seek(0, SeekOrigin.End);
		////_log.Flush()

		//using var mutex = new Mutex(false, "testLogFile");
		//	mutex.WaitOne();


		//	mutex.ReleaseMutex();

		//return;


		AOutput.Clear();

		var doc = Panels.Editor.ActiveDoc;
		//var doc = Panels.Output.Controls[0] as AuScintilla;
		var t = doc.ST;
		var s = doc.Text;

		//doc.DebugPrintMessages = true;

		//int i = s.Find("Print");
		//t.ReplaceRange(i, 5, "PRINT", SciFromTo.BothChars | SciFromTo.ToIsLength);

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

		var t = Panels.Editor.ActiveDoc.ST;
		//APerf.First();
		//t.ReplaceRange(2, 4, "NEW TEXT");
		//t.ReplaceRange(2, 4, "NEW TEXT", true, true);
		//t.DeleteRange(2, 3);
		//t.InsertText(2, "TEXT");
		//APerf.NW();

		//var s=t.GetText();
		//Print(s.Length, s, t.Call(SCI_POSITIONRELATIVE, 0, 1), t.Call(SCI_COUNTCHARACTERS, 0, 4));

		foreach(var f in Program.Model.Root.Descendants()) {
			var k = f.GetClassFileRole();
			if(k != default) Print(k, f.ItemPath);
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
		var f = new Au.Tools.FormAWnd(AWnd.Find("Quick*"));
		//var f = new Au.Tools.FormAAcc();
		//var f = new Au.Tools.FormAWinImage();
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

	unsafe void SetHookToMonitorCreatedWindowsOfThisThread()
	{
		_hook = AHookWin.ThreadCallWndProcRet(x => {
			if(x.msg->message == Api.WM_CREATE) {
				if(Program.MainForm.Visible) return;
				var w = x.msg->hwnd;
				var p = w.Get.DirectParent; if(p.Is0) p = w.Owner;
				var c = Control.FromHandle(w.Handle); //always null in CBT hook proc
				var s = c?.ToString() ?? "";
				Print($"<><c 0xcc00>{w} ({s}), {p.Handle}</c>");

				//if(c is Au.Controls.AuToolStrip) { //never mind: .NET bug: if toolstrip Custom1 has overflow and window is maximized, creates parked handle
				//	int stop = 0;
				//}
			}
		});
		Application.ApplicationExit += (unu, sed) => _hook.Dispose(); //without it at exit crashes (tested with raw API and not with AHookWin) 
	}
	static AHookWin _hook;
}
#endif
