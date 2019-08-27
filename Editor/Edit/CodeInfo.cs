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
using Au.Editor.Properties;

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
		//warm up //TODO: 2000
		Task.Delay(100).ContinueWith(_ => {
			//return; //TODO
			//var p1 = APerf.Create();
			var code = @"using System; class C { static void Main() { } }";
			int position = code.IndexOf('}');
			//var refs = new MetaReferences().Refs;
			var refs = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
			ProjectId projectId = ProjectId.CreateNewId();
			DocumentId documentId = DocumentId.CreateNewId(projectId);
			var sol = new AdhocWorkspace().CurrentSolution
				.AddProject(projectId, "p", "p", LanguageNames.CSharp)
				.AddMetadataReferences(projectId, refs)
				.AddDocument(documentId, "f.cs", code);
			//p1.Next();
			//200.ms(); //avoid CPU fan (sometimes)
			var document = sol.GetDocument(documentId);
			var completionService = CompletionService.GetService(document);
			completionService.GetCompletionsAsync(document, position);
			//MetaReferences.CompactCache();
			//p1.NW(); //600-1000 ms when ngened
			//EdUtil.MinimizeProcessPhysicalMemory(500); //with this later significantly slower
			_isWarm = true;
		});

		Panels.Editor.ActiveDocChanged += Stop;
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
		_solution = null;
		_projectId = null;
		_documentId = null;
		//_meta = null;
		_metaText = null;
		_textChanged = false;
	}

	public static void Stop()
	{
		Cancel();
		_Uncache();
	}

	public static void Cancel()
	{
		//Print("Cancel");
		_compl.Cancel();
	}

	public static void SciTextChanged(SciCode doc, in Sci.SCNotification n, bool userTyping)
	{
		if(!_CanWork(doc)) { Debug.Assert(_solution == null); return; }
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

		_compl.SciTextChanged(n);
	}

	public static void SciUpdateUI(SciCode doc, bool modified)
	{
		//Print("SciUpdateUI", modified, _tempNoAutoComplete);
		if(!_CanWork(doc)) return;
		if(!modified) _compl.SciPositionChangedNotModified();
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

	static void _ChangeSolution(string code=null)
	{
		code ??= Panels.Editor.ActiveDoc.Text;
		_solution = _solution.WithDocumentText(_documentId, SourceText.From(code, Encoding.UTF8));
		_textChanged = false;
	}

	static void _CreateSolution()
	{
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
		APerf.Next('m');

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
		APerf.Next('p');

		_solution = new AdhocWorkspace().CurrentSolution.AddProject(pi);
	}

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

	static Bitmap[,] s_images;
	static string[,] s_imageNames;

	public static Bitmap GetImage(CiItemKind kind, CiItemAccess access = default)
	{
		if(s_images == null) {
			s_images = new Bitmap[18, 4];
			s_imageNames = new string[18, 4] {
				{ nameof(Resources.ciClass), nameof(Resources.ciClassPrivate), nameof(Resources.ciClassProtected), nameof(Resources.ciClassInternal)},
				{ nameof(Resources.ciStructure), nameof(Resources.ciStructurePrivate), nameof(Resources.ciStructureProtected), nameof(Resources.ciStructureInternal)},
				{ nameof(Resources.ciEnum), nameof(Resources.ciEnumPrivate), nameof(Resources.ciEnumProtected), nameof(Resources.ciEnumInternal)},
				{ nameof(Resources.ciDelegate), nameof(Resources.ciDelegatePrivate), nameof(Resources.ciDelegateProtected), nameof(Resources.ciDelegateInternal)},
				{ nameof(Resources.ciInterface), nameof(Resources.ciInterfacePrivate), nameof(Resources.ciInterfaceProtected), nameof(Resources.ciInterfaceInternal)},
				{ nameof(Resources.ciMethod), nameof(Resources.ciMethodPrivate), nameof(Resources.ciMethodProtected), nameof(Resources.ciMethodInternal)},
				{ nameof(Resources.ciExtensionMethod),null,null,null},
				{ nameof(Resources.ciProperty), nameof(Resources.ciPropertyPrivate), nameof(Resources.ciPropertyProtected), nameof(Resources.ciPropertyInternal)},
				{ nameof(Resources.ciEvent), nameof(Resources.ciEventPrivate), nameof(Resources.ciEventProtected), nameof(Resources.ciEventInternal)},
				{ nameof(Resources.ciField), nameof(Resources.ciFieldPrivate), nameof(Resources.ciFieldProtected), nameof(Resources.ciFieldInternal)},
				{ nameof(Resources.ciLocal),null,null,null},
				{ nameof(Resources.ciConstant), nameof(Resources.ciConstantPrivate), nameof(Resources.ciConstantProtected), nameof(Resources.ciConstantInternal)},
				{ nameof(Resources.ciEnumMember),null,null,null},
				{ nameof(Resources.ciKeyword),null,null,null},
				{ nameof(Resources.ciNamespace),null,null,null},
				{ nameof(Resources.ciLabel),null,null,null},
				{ nameof(Resources.ciSnippet),null,null,null},
				{ nameof(Resources.ciTypeParameter),null,null,null},
			};
		}
		return s_images[(int)kind, (int)access] ??= EdResources.GetImageNoCache(s_imageNames[(int)kind, (int)access]);
	}

	public static string[] ItemKindNames { get; } = new string[] { "Class", "Structure", "Enum", "Delegate", "Interface", "Method", "ExtensionMethod", "Property", "Event", "Field", "Local", "Constant", "EnumMember", "Keyword", "Namespace", "Label", "Snippet", "TypeParameter" }; //must match enum EItemKind
}

public enum CiItemKind :byte { Class, Structure, Enum, Delegate, Interface, Method, ExtensionMethod, Property, Event, Field, Local, Constant, EnumMember, Keyword, Namespace, Label, Snippet, TypeParameter, None }

public enum CiItemAccess :byte { Public, Private, Protected, Internal }

[Flags]
enum CiItemHiddenBy { Text = 1, Kind = 2, }

class DocCodeInfo
{
	public Solution solution;

	public DocCodeInfo(SciCode doc)
	{

	}
}
