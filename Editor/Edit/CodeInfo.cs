using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Compiler;
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Completion;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.SignatureHelp;
using Microsoft.CodeAnalysis.CSharp.SignatureHelp;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.CSharp.QuickInfo;
using TheArtOfDev.HtmlRenderer.WinForms;


static class CodeInfo
{
	static CiCompletion _compl = new CiCompletion();
	static CiSignature _signature = new CiSignature();
	static CiQuickInfo _quickInfo = new CiQuickInfo();
	//static MetaComments _meta;
	static string _metaText;
	static Solution _solution;
	static ProjectId _projectId;
	static DocumentId _documentId;
	static bool _isWarm;
	static bool _textChanged;

	public static void UiLoaded()
	{
		//warm up
		Task.Delay(500).ContinueWith(_1 => {
			var p1 = APerf.Create();
			//_isWarm = true;
			//return;
#if DEBUG
			if(Debugger.IsAttached) { _isWarm = true; return; }
#endif
			_Warmup(ref p1);
		});

		Panels.Editor.ActiveDocChanged += Stop;
	}

	static void _Warmup(ref APerf.Inst p1)
	{
		p1.Next();
		try {
#if true
			var code = @"//{{
using Au; using Au.Types; using static Au.AStatic; using System; using System.Collections.Generic;
class Script :AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) {
Print(1);
} }";
			var refs = new MetaReferences().Refs;
#else
			var code = @"using System; class C { static void Main() { } }";
			var refs = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
#endif
			int position = code.IndexOf('}');
			ProjectId projectId = ProjectId.CreateNewId();
			DocumentId documentId = DocumentId.CreateNewId(projectId);
			using var ws = new AdhocWorkspace();
			var sol = ws.CurrentSolution
				.AddProject(projectId, "p", "p", LanguageNames.CSharp)
				.AddMetadataReferences(projectId, refs)
				.AddDocument(documentId, "f.cs", code);
			var document = sol.GetDocument(documentId);
			p1.Next();
			Task.Run(() => { Compiler.Warmup(document); /*p1.NW('c');*/ }); //if no ProfileOptimization (PO), makes GetCompletionsAsync faster, eg 3 -> 2.5 s, and total faster eg 4 -> 3.4. If with PO, same speed or slower.
			var completionService = CompletionService.GetService(document);
			var cr = completionService.GetCompletionsAsync(document, position).Result;
			//MetaReferences.CompactCache();
			_isWarm = true; //TODO: instead use cancellation token
							//p1.Next(); //600-1000 ms when ngened, 2.1 s with ProfileOptimization, else 3 s (2.5 s with Compiler.Warmup above)
							//Compiler.Warmup(document);
			p1.NW('w');
			if(cr == null) ADebug.Print("null"); //else Print(cr.Items.Length);

			//EdUtil.MinimizeProcessPhysicalMemory(500); //with this later significantly slower
		}
		//catch(ReflectionTypeLoadException ex) {
		//	ADebug.Print(ex.LoaderExceptions);
		//	ADebug.Print(ex);
		//}
		catch(Exception ex) {
			ADebug.Print(ex);
		}
	}

	static bool _CanWork(SciCode doc)
	{
		if(!_isWarm) return false;
		if(doc == null) return false;
		if(!doc.FN.IsCodeFile) return false;
		if(doc != Panels.Editor.ActiveDoc) { _Uncache(); return false; } //maybe changed an inactive file that participates in current compilation //TODO: what if isn't open?
		return true;
	}

	static void _Uncache()
	{
		CurrentWorkspace = null;
		_solution = null;
		_projectId = null;
		_documentId = null;
		//_meta = null;
		_metaText = null;
		_textChanged = false;
	}

	public static void Stop()
	{
		Cancel(hideTools: true);
		_Uncache();
	}

	public static void Cancel(bool hideTools = false)
	{
		//Print("Cancel");
		_compl.Cancel();
		if(hideTools) {
			_compl.HideTools();
		}
	}

	public static void SciCharAdded(SciCode doc)
	{
		if(!_CanWork(doc)) return;
		_compl.SciCharAdded(doc);
	}

	public static void SciTextChanged(SciCode doc, in Sci.SCNotification n, bool userTyping)
	{
		if(!_CanWork(doc)) return;
		_textChanged = true;
		if(!userTyping) { Cancel(); return; }

		var code = doc.Text;
		int endOfMeta = MetaComments.FindMetaComments(code);
		if(n.position < endOfMeta) {
			_Uncache();
			_compl.SciTextChangedInMeta(n);
			return;
		}
		if(_solution != null) {
			bool uncache = endOfMeta != _metaText.Length || !code.Starts(_metaText);
			if(uncache) _Uncache();
			//else _ChangeSolution(code); //later
		}

		_compl.SciTextChanged(doc, n);
	}

	public static void SciUpdateUI(SciCode doc, bool modified)
	{
		//Print("SciUpdateUI", modified, _tempNoAutoComplete);
		if(!_CanWork(doc)) return;
		_compl.SciPositionChanged(doc, modified);
	}

	public static void ShowCompletionList(SciCode doc)
	{
		if(!_CanWork(doc)) return;
		_compl.ShowList();
	}

	public static void ShowSignature(SciCode doc)
	{
		if(!_CanWork(doc)) return;
		_signature.ShowSignature();
	}

	public static void SciMouseDwellStarted(SciCode doc, int positionUtf8, int x, int y)
	{
		if(!_CanWork(doc)) return;
		_quickInfo.SciMouseDwellStarted(positionUtf8, x, y);
	}

	public static void SciMouseDwellEnded(SciCode doc)
	{
		if(!_CanWork(doc)) return;
		_quickInfo.SciMouseDwellEnded();
	}

	public static void SciMouseMoved(SciCode doc, int x, int y)
	{
		if(!_CanWork(doc)) return;
		_quickInfo.SciMouseMoved(x, y);
	}

	public static Document GetDocument()
	{
		Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1);
		if(_solution == null) _CreateSolution();
		else if(_textChanged) _ChangeSolution();
		return _solution.GetDocument(_documentId);
	}

	static void _ChangeSolution(string code = null)
	{
		code ??= Panels.Editor.ActiveDoc.Text;
		_solution = _solution.WithDocumentText(_documentId, SourceText.From(code, Encoding.UTF8));
		_textChanged = false;
	}

	static void _CreateSolution()
	{
		//var p1 = APerf.Create();
		var doc = Panels.Editor.ActiveDoc;
		var code = doc.Text;
		_metaText = code.Remove(MetaComments.FindMetaComments(code));

		var f = doc.FN;
		var f0 = f;
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		var m = new MetaComments();
		if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo)) {
			var err = m.Errors;
			err.PrintAll();
		}
		//p1.Next('m');

		_projectId = ProjectId.CreateNewId();
		var adi = new List<DocumentInfo>();
		foreach(var f1 in m.CodeFiles) {
			var docId = DocumentId.CreateNewId(_projectId);
			var tav = TextAndVersion.Create(SourceText.From(f1.code, Encoding.UTF8), VersionStamp.Default, f1.f.FilePath);
			adi.Add(DocumentInfo.Create(docId, f1.f.Name, null, SourceCodeKind.Regular, TextLoader.From(tav)));
			if(f1.f == f0) {
				_documentId = docId;
			}
		}
		var pi = ProjectInfo.Create(_projectId, VersionStamp.Default, f.Name, f.Name, LanguageNames.CSharp, null, null,
			m.CreateCompilationOptions(),
			m.CreateParseOptions(),
			adi,
			projectReferences: null, //TODO: create from m.ProjectReferences?
			m.References.Refs); //TODO: set outputRefFilePath if library?
								//p1.Next('p');

		CurrentWorkspace = new AdhocWorkspace();
		_solution = CurrentWorkspace.CurrentSolution.AddProject(pi);
		//p1.NW('s');
	}

	public static Workspace CurrentWorkspace { get; private set; }

	public static Document CreateTestDocumentForEditorCode(out string code, out int position)
	{
		var doc = Panels.Editor.ActiveDoc;
		code = null;
		position = doc.ST.CurrentPosChars;
		var f = doc.FN;
		if(!f.IsCodeFile) return null;
		var f0 = f;
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		var m = new MetaComments();
		if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo)) {
			var err = m.Errors;
			err.PrintAll();
			return null;
		}

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
			m.References.Refs //TODO: set outputRefFilePath if library?
			);

		var sol = new AdhocWorkspace().CurrentSolution;
		sol = sol.AddProject(pi);
		return sol.GetDocument(documentId);
	}

	public static void Test()
	{

	}
}

//class DocCodeInfo
//{
//	public Solution solution;

//	public DocCodeInfo(SciCode doc)
//	{

//	}
//}
