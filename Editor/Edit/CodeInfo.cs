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
//using System.Windows.Forms;
//using System.Drawing;
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

class CodeInfo
{
	public void UiLoaded()
	{
		//warm up
		Task.Delay(2000).ContinueWith(_ => {
			//return; //TODO
			//500.ms();
			//APerf.Cpu();
			//var p1 = APerf.Create();
			var code = @"using System; class C { static void Main() { } }";
			int position = code.IndexOf('}');
			var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			ProjectId projectId = ProjectId.CreateNewId();
			DocumentId documentId = DocumentId.CreateNewId(projectId);
			var sol = new AdhocWorkspace().CurrentSolution
				.AddProject(projectId, "p", "p", LanguageNames.CSharp)
				.AddMetadataReferences(projectId, new MetadataReference[] { mscorlib })
				.AddDocument(documentId, "f.cs", code);
			//p1.Next();
			200.ms(); //avoid CPU fan
			var document = sol.GetDocument(documentId);
			var completionService = CompletionService.GetService(document);
			completionService.GetCompletionsAsync(document, position);
			//p1.NW(); //600-1000 ms when ngened
			//EdUtil.MinimizeProcessPhysicalMemory(500); //36 MB -> 1 MB. Later code info will make 36 again, but without this would make 54; but with this then significantly slower.
		});

		Panels.Editor.ActiveDocChanged += Cancel;
	}

	public void TextChanged(PanelEdit.SciCode doc, in Sci.SCNotification n)
	{
		//bool added = 0 != (n.modificationType & Sci.MOD.SC_MOD_INSERTTEXT);
		_ComplCancel();
	}

	public void Cancel()
	{
		//Print("Cancel");
		_ComplCancel();
		CancelCompletionList();
	}

	void _ComplCancel()
	{
		_complCTS?.Cancel();
		_complTimer?.Stop();

	}

	public void CharAdded(PanelEdit.SciCode doc, int chi)
	{
		//return; //TODO
		if(chi > char.MaxValue) return;
		var ch = (char)chi;
		if(!(ch == '.' || ch == '_' || char.IsLetterOrDigit(ch))) return;
		_ShowCompletionList(ch);
	}

	public void ShowCompletionList()
	{
		_ShowCompletionList();
	}

	ATimer _complTimer;
	CancellationTokenSource _complCTS;
	char _complChar;

	void _ShowCompletionList(char ch = default)
	{
		_complCTS?.Cancel();
		_complCTS = new CancellationTokenSource();
		_complChar = ch;
		if(ch == default || ch == '.') {
			_ShowCompletionList2(_complCTS);
		} else {
			if(_complTimer == null) _complTimer = new ATimer(() => _ShowCompletionList2(_complCTS, _complChar));
			_complTimer.Start(100, true);
			_complTimer.Tag = _complCTS;
		}
	}

	async void _ShowCompletionList2(CancellationTokenSource cancel, char ch = default)
	{
		//AOutput.Clear(); //TODO
		APerf.First();
		var doc = Panels.Editor.ActiveDoc;
		var f = doc.FN;
		if(!f.IsCodeFile) { CancelCompletionList(); return; }
		var f0 = f;
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		var m = new MetaComments();
		if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo)) {
			var err = m.Errors;
			err.PrintAll();
			CancelCompletionList(); return;
		}
		APerf.Next('M');

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
		APerf.Next('P');

		var position = doc.ST.CurrentPosChars;
		APerf.Next();
		Document document = null;
		var cancelToken = cancel.Token;
		try {
			var r = await Task.Run(() => {
				var sol = new AdhocWorkspace().CurrentSolution;
				sol = sol.AddProject(pi);
				APerf.Next('S');
				document = sol.GetDocument(documentId);

		//_=document.GetSemanticModelAsync().Result;
		//APerf.Next('k');

				var completionService = CompletionService.GetService(document);
				APerf.Next('s');
				if(cancelToken.IsCancellationRequested) return null;
				//var trigger = ch == default ? default : CompletionTrigger.CreateInsertionTrigger(ch);
				return completionService.GetCompletionsAsync(document, position, /*trigger,*/ cancellationToken: cancelToken); //info: somehow GetCompletionsAsync is not async
			});
			APerf.Next('C');
			if(r == null || cancelToken.IsCancellationRequested) { CancelCompletionList(); APerf.NW('z'); return; }
			//Print(r.Items);
			//Print(r.Items.Length);

			if(!document.TryGetSemanticModel(out var model)) model = document.GetSemanticModelAsync().Result; //fast
			var symbols = model.LookupSymbols(position); //1-2 ms
			APerf.Next('M');

			var d = new Dictionary<string, object>(symbols.Length);
			foreach(var sy in symbols) {
				string name = sy.Name;
				if(d.TryGetValue(name, out var o)) {
					switch(o) {
					case ISymbol sy2: d[name] = new List<ISymbol> { sy2, sy }; break;
					case List<ISymbol> list: list.Add(sy); break;
					}
				} else d.Add(name, sy);
			}
			APerf.Next('m');

			var span = r.Span;
			//Print(span, code.Substring(span.Start, span.Length));
			List<CodeinItem> b;
			if(span.Length > 0) {
				var sub = code.Substring(span.Start, span.Length);
				var subUpper = sub.Upper();
				b = new List<CodeinItem>();
				var ah = new List<int>();
				foreach(var v in r.Items) {
					ADebug.PrintIf(v.FilterText != v.DisplayText, $"{v.FilterText}, {v.DisplayText}");
					//Print(v.DisplayText, v.FilterText, v.SortText, v.ToString());
					int[] hilite;
					var s = v.DisplayText;
					int iSub = s.Find(sub, true);
					if(iSub >= 0) {
						hilite = new int[] { iSub, iSub + sub.Length };
						//TODO: VS adds only if the substring starts with uppercase
					} else if(sub.Length > 1) { //has all uppercase chars? Eg add OneTwoThree if sub is "ott" or "ot" or "tt".
						ah.Clear();
						bool no = false;
						for(int i = 0, j = 0; i < subUpper.Length; i++, j++) {
							j = s.IndexOf(subUpper[i], j);
							if(j < 0) { no = true; break; }
							ah.Add(j); ah.Add(j + 1);
						}
						if(no) continue;
						hilite = ah.ToArray();
					} else continue;
					//TODO: support _ and all ucase, like WM_PAINT

					b.Add(new CodeinItem(v, hilite));
				}
			} else {
				b = new List<CodeinItem>(r.Items.Length);
#if true
				foreach(var v in r.Items) b.Add(new CodeinItem(v));
#else
				foreach(var v in r.Items) {
					bool noFilter = false;
					switch(v.Tags[0]) {
					case WellKnownTags.Keyword:
					case WellKnownTags.TypeParameter:
					case WellKnownTags.EnumMember:
					case WellKnownTags.Local:
					case WellKnownTags.Parameter:
					case WellKnownTags.RangeVariable:
					case WellKnownTags.Label:
					case WellKnownTags.Snippet:
						noFilter = true;
						break;
					}
					if(!noFilter) {
						var name = v.DisplayText;
						if(d.TryGetValue(name, out var o)) {
							ISymbol sym = null;
							switch(o) {
							case ISymbol sy2: sym=sy2; break;
							case List<ISymbol> list: sym=list[0]; break;
							}

							Print(name, sym);
						} else {
							Print($"<><c red>{name}, {v.Tags[0]}<>");
						}
					}

					b.Add(new CodeinItem(v));
				}
#endif
			}
			APerf.Next('f');
			//Print(b.Count);
			Panels.Codein.SetListItems(b);
			//ATimer.Every(100, () => Panels.Codein.SetListItems(b));
			//APerf.Next();
			APerf.NW();
		}
		catch(OperationCanceledException) { /*ADebug.Print("canceled");*/ CancelCompletionList(); }
	}

	public void CancelCompletionList()
	{
		Panels.Codein.SetListItems(null);
	}
}
