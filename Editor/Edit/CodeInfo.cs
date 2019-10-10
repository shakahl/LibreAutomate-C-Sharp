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
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Compiler;
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Completion;

static class CodeInfo
{
	static CiCompletion _compl = new CiCompletion();
	static CiSignature _signature = new CiSignature();
	static CiAutocorrect _correct = new CiAutocorrect();
	static CiQuickInfo _quickInfo = new CiQuickInfo();
	//static MetaComments _meta;
	static string _metaText;
	static Solution _solution;
	static ProjectId _projectId;
	static DocumentId _documentId;
	static Document _document;
	static bool _isWarm;
	static bool _isUI;
	static RECT _sciRect;

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

		Panels.Editor.ZActiveDocChanged += Stop;
		Program.Timer1sOr025s += _Program_Timer1sOr025s;
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
		if(!doc.ZFile.IsCodeFile) return false;
		if(doc != Panels.Editor.ZActiveDoc) { _Uncache(); return false; } //maybe changed an inactive file that participates in current compilation //TODO: what if isn't open?
		return true;
	}

	static void _Uncache()
	{
		//Print("_Uncache");
		CurrentWorkspace = null;
		_solution = null;
		_projectId = null;
		_documentId = null;
		_document = null;
		//_meta = null;
		_metaText = null;
	}

	public static void Stop()
	{
		Cancel();
		_Uncache();
	}

	public static void Cancel()
	{
		_compl.Cancel();
		_compl.HideTools();
		_signature.Cancel();
	}
	//TODO: now does not cancel when clicked empty space after all code.

	public static void SciKillFocus()
	{
#if DEBUG
		if(Debugger.IsAttached) return;
#endif
		//hide code info windows, except when a code info window is focused. Code info window names start with "Ci.".
		var aw = AWnd.ThisThread.Active;
		if(aw.Is0) Stop(); else if(!(Control.FromHandle(aw.Handle) is Form f && f.Name.Starts("Ci."))) Cancel();
	}

	public static bool SciCmdKey(SciCode doc, Keys keyData)
	{
		if(!_CanWork(doc)) return false;
		switch(keyData) {
		case Keys.Control | Keys.Space:
			ShowCompletionList(doc);
			return true;
		case Keys.Control | Keys.Shift | Keys.Space:
			ShowSignature(doc);
			return true;
		case Keys.Escape:
		case Keys.Down:
		case Keys.Up:
		case Keys.PageDown:
		case Keys.PageUp:
			if(_compl.OnCmdKey_SelectOrHide(keyData)) return true;
			if(_signature.OnCmdKey(keyData)) return true;
			break;
		case Keys.Tab:
		case Keys.Enter:
			return _compl.OnCmdKey_Commit(doc, keyData) != CiComplResult.None || _correct.SciBeforeKey(doc, keyData);
		case Keys.Enter | Keys.Shift:
		case Keys.Enter | Keys.Control:
		case Keys.OemSemicolon | Keys.Control:
			var complResult = _compl.OnCmdKey_Commit(doc, keyData);
			if(complResult == CiComplResult.Complex) return true;
			return _correct.SciBeforeKey(doc, keyData) | (complResult != CiComplResult.None);
		case Keys.Back:
		case Keys.Delete:
			return _correct.SciBeforeKey(doc, keyData);
		}
		return false;
	}

	public static bool SciBeforeCharAdded(SciCode doc, char ch)
	{
		if(!_CanWork(doc)) return false;

		if(_correct.SciBeforeCharAdded(doc, ch, out var b)) {
			if(_compl.IsVisibleUI) {
				int diff = b.newPosUtf8 - b.oldPosUtf8;
				_compl.SciCharAdding_Commit(doc, ch);
				b.newPosUtf8 = doc.Z.CurrentPos8 + diff;
			}

			doc.Z.CurrentPos8 = b.newPosUtf8;
			if(!b.dontSuppress) return true;
		} else if(_compl.IsVisibleUI) {
			if(CiComplResult.Complex == _compl.SciCharAdding_Commit(doc, ch)) return true;
		}
		return false;
	}

	public static void SciModified(SciCode doc, in Sci.SCNotification n)
	{
		if(!_CanWork(doc)) return;
		_document = null;
		_compl.SciModified(doc, in n);
	}

	public static void SciCharAdded(SciCode doc, char ch)
	{
		if(!_CanWork(doc)) return;

		using var c = new CharContext(doc, ch);
		_correct.SciCharAdded(c); //sync adds or removes ')' etc if need.
		if(!c.ignoreChar) {
			_compl.SciCharAdded_ShowList(c); //async gets completions and shows popup list. If already showing, filters/selects items.
			_signature.SciCharAdded(c.doc, c.ch); //sync shows signature help.
		}

		//Example: user types 'pri('.
		//	When typed 'p', _compl.SciCharAdded_ShowList shows popup list (async).
		//	While typing 'ri', _compl.SciModified in the list selects Print.
		//	When typed '(':
		//		_compl.SciCharAdded_Commit replaces 'pri(' with 'Print('. Caret is after '('.
		//		_correct adds ')'. Caret is still after '('.
		//		_signature shows signature help.
		//	If then user types 'tr)':
		//		_compl on 't' shows popup list and on ')' replaces 'tr)' with 'true)'.
		//		_correct deletes the ')' it added before.
		//		_signature not called because discardChar==true. To hide signature help are used temp ranges.
		//	Finally we have 'Print(true)', and caret is after it, and no double '))'.
		//	If instead types 'tr;':
		//		_correct on ';' moves caret after ')', and finally we have 'Print(true);', and caret after ';'.
	}

	public static void SciUpdateUI(SciCode doc, bool modified)
	{
		//Print("SciUpdateUI", modified, _tempNoAutoComplete);
		if(!_CanWork(doc)) return;
		_compl.SciUpdateUI(doc, modified);
		_signature.SciPositionChanged(doc);
	}

	public static void ShowCompletionList(SciCode doc)
	{
		if(!_CanWork(doc)) return;
		_compl.ShowList();
	}

	public static void ShowSignature(SciCode doc)
	{
		if(!_CanWork(doc)) return;
		_signature.ShowSignature(doc);
	}

	public static void SciMouseDwellStarted(SciCode doc, int positionUtf8)
	{
		if(!_CanWork(doc)) return;
		_quickInfo.SciMouseDwellStarted(doc, positionUtf8);
	}

	//public static void SciMouseDwellEnded(SciCode doc)
	//{
	//	if(!_CanWork(doc)) return;
	//	_quickInfo.SciMouseDwellEnded();
	//}

	//public static void SciMouseMoved(SciCode doc, int x, int y)
	//{
	//	if(!_CanWork(doc)) return;
	//	_quickInfo.SciMouseMoved(x, y);
	//}

	/// <summary>
	/// Called to show signature help after committing a completion item with mouse, Tab, Enter or ' ', when added '(' after method etc.
	/// </summary>
	public static void CompletionSignatureCharAdded(SciCode doc, char ch) => _signature.SciCharAdded(doc, ch);

	/// <summary>
	/// Called when added text containing { } etc and want the same behavior like when the user types { etc and it is corrected to { } etc.
	/// </summary>
	public static void BracesAdded(SciCode doc, int innerFrom, int innerTo, CiAutocorrect.EBraces operation) => _correct.BracesAdded(doc, innerFrom, innerTo, operation);

	//public static void CharSuppressedByCompletion(SciCode doc, char signatureChar, int bracesFromInner, int bracesToInner){
	//could join the above 2
	//}

	public struct Context
	{
		public Document document;
		public SciCode sciDoc;
		public string code;
		public int metaEnd;
		public int position;

		/// <summary>
		/// Initializes all fields except document.
		/// </summary>
		/// <param name="pos"></param>
		public Context(int pos)
		{
			Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1);

			document = null;
			sciDoc = Panels.Editor.ZActiveDoc;
			if(pos == -1) pos = sciDoc.Z.CurrentPos16; else if(pos == -2) pos = sciDoc.Z.SelectionStar16;
			position = pos;
			code = sciDoc.Text;
			metaEnd = MetaComments.FindMetaComments(code);
		}

		/// <summary>
		/// Initializes the document field.
		/// Creates or updates Solution if need.
		/// Returns false if fails, unlikely.
		/// </summary>
		public bool GetDocument()
		{
			if(_document != null) {
				document = _document;
				return true;
			}

			if(_solution != null && !code.Starts(_metaText)) _Uncache();
			if(_solution == null) _metaText = code.Remove(metaEnd);

			try {
				if(_solution == null) {
					_CreateSolution(sciDoc.ZFile);
				} else {
					_solution = _solution.WithDocumentText(_documentId, SourceText.From(code, Encoding.UTF8));
				}
			}
			catch(Exception ex) { //never seen
				ADebug.Print(ex);
				return false;
			}

			document = _document = _solution.GetDocument(_documentId);
			return document != null;
		}

		//public bool GetDocumentAndSyntaxRoot(out SyntaxNode root)
		//{
		//	if(!GetDocument()) { root = null; return false; }
		//	root = document.GetSyntaxRootAsync().Result;
		//	return true;
		//}

		//public bool GetDocumentAndFindNode(out SyntaxNode node)
		//{
		//	if(!GetDocument()) { node = null; return false; }
		//	node = document.GetSyntaxRootAsync().Result.FindToken(position).Parent;
		//	return true;
		//}
	}

	/// <summary>
	/// Creates new Context. Calls its GetContextAndDocument, unless position is in meta comments.
	/// Returns false if position is in meta comments or if fails to create/update Solution (unlikely). Then r.document is null.
	/// Else returns true. Then r.document is Document for Panels.Editor.ActiveDoc. If need, parses meta, creates Project, Solution, etc.
	/// Always sets other r fields.
	/// If position -1, gets current position. If position -2, gets selection start.
	/// </summary>
	public static bool GetContextAndDocument(out Context r, int position = -1)
	{
		if(!GetContextWithoutDocument(out r, position)) return false;
		return r.GetDocument();
	}

	/// <summary>
	/// Creates new Context with document=null.
	/// Returns false if position is in meta comments.
	/// If position -1, gets current position. If position -2, gets selection start.
	/// </summary>
	public static bool GetContextWithoutDocument(out Context r, int position = -1)
	{
		r = new Context(position);
		return r.position >= r.metaEnd;
	}

	/// <summary>
	/// Calls <see cref="GetContextAndDocument"/>, gets its syntax root and finds node.
	/// </summary>
	public static bool GetDocumentAndFindNode(out Context r, out SyntaxNode node, int position = -1)
	{
		if(!GetContextAndDocument(out r, position)) { node = null; return false; }
		node = r.document.GetSyntaxRootAsync().Result.FindToken(r.position).Parent;
		return true;
	}

	static void _CreateSolution(FileNode f)
	{
		//var p1 = APerf.Create();
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
			adi.Add(DocumentInfo.Create(docId, f1.f.Name, null, SourceCodeKind.Regular, TextLoader.From(tav), f1.f.ItemPath));
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

	private static void _Program_Timer1sOr025s()
	{
		//cancel if changed the screen rectangle of the document window
		if(_compl.IsVisibleUI || _signature.IsVisibleUI) {
			var r = ((AWnd)Panels.Editor.ZActiveDoc).Rect;
			if(!_isUI) {
				_isUI = true;
				_sciRect = r;
			} else if(r != _sciRect) { //moved/resized top-level window or eg moved some splitter
				_isUI = false;
				Cancel();
			}
		} else if(_isUI) {
			_isUI = false;
		}
	}

	public class CharContext : IDisposable
	{
		public readonly SciCode doc;
		public char ch;
		public bool ignoreChar;
		bool _undoStarted; //TODO: unused

		public CharContext(SciCode doc, char ch)
		{
			this.doc = doc;
			this.ch = ch;
		}

		public void BeginUndoAction()
		{
			if(!_undoStarted) {
				_undoStarted = true;
				doc.Call(Sci.SCI_BEGINUNDOACTION);
			}
		}

		public void Dispose()
		{
			if(_undoStarted) {
				_undoStarted = false;
				doc.Call(Sci.SCI_ENDUNDOACTION);
			}
		}
	}

	//public static void Test()
	//{

	//}
}
