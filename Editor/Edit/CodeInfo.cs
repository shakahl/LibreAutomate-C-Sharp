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
using Microsoft.CodeAnalysis.Completion;

class CodeInfo
{
	public void UiLoaded()
	{
		//warm up
		new Timer(_ => {
			//500.ms();
			//APerf.Cpu();
			//var p1 = APerf.StartNew();
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
		}, null, 2000, -1);

		Panels.Editor.ActiveDocChanged += Cancel;
	}

	public void TextChanged(PanelEdit.SciCode doc, in Sci.SCNotification n)
	{
		//bool added = 0 != (n.modificationType & Sci.MOD.SC_MOD_INSERTTEXT);
		_ComplCancel();
	}

	public void Cancel()
	{
		Print("Cancel");
		_ComplCancel();
	}

	void _ComplCancel()
	{
		_complCTS?.Cancel();
		_complTimer?.Stop();

	}

	public void CharAdded(PanelEdit.SciCode doc, int chi)
	{
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
		if(ch == default) {
			_ShowCompletionList2(_complCTS);
		} else {
			if(_complTimer == null) _complTimer = new ATimer(() => _ShowCompletionList2(_complCTS, _complChar));
			_complTimer.Start(200, true);
			_complTimer.Tag = _complCTS;
		}
	}

	async void _ShowCompletionList2(CancellationTokenSource cancel, char ch = default)
	{
		//AOutput.Clear(); //TODO
		var doc = Panels.Editor.ActiveDoc;
		var f = doc.FN;
		if(!f.IsCodeFile) return;
		var f0 = f;
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		APerf.First();
		var m = new MetaComments();
		if(!m.Parse(f, projFolder, EMPFlags.ForCodeInfo)) {
			var err = m.Errors;
			err.PrintAll();
			return;
		}
		APerf.Next('M');

		DocumentId documentId = null;
		ProjectId projectId = ProjectId.CreateNewId();
		var adi = new List<DocumentInfo>();
		foreach(var f1 in m.CodeFiles) {
			var docId = DocumentId.CreateNewId(projectId);
			var tav = TextAndVersion.Create(Microsoft.CodeAnalysis.Text.SourceText.From(f1.code, Encoding.UTF8), VersionStamp.Default, f1.f.FilePath);
			adi.Add(DocumentInfo.Create(docId, f1.f.Name, null, SourceCodeKind.Regular, TextLoader.From(tav)));
			if(f1.f == f0) {
				documentId = docId;
			}
		}
		var pi = ProjectInfo.Create(projectId, VersionStamp.Default, f.Name, f.Name, LanguageNames.CSharp, null, null,
			m.CreateCompilationOptions(), m.CreateParseOptions(), adi,
			projectReferences: null, //TODO: create from m.ProjectReferences?
			m.References.Refs); //TODO: set outputRefFilePath if library?
		APerf.Next('P');

		var position = doc.ST.CurrentPosChars;
		APerf.Next();
		var cancelToken = cancel.Token;
		var all = await Task.Run(() => {
			var sol = new AdhocWorkspace().CurrentSolution;
			sol = sol.AddProject(pi);
			APerf.Next('S');
			var document = sol.GetDocument(documentId);
			var completionService = CompletionService.GetService(document);
			APerf.Next('s');
			if(cancelToken.IsCancellationRequested) return null;
			var trigger = ch == default ? default : CompletionTrigger.CreateInsertionTrigger(ch);
			return completionService.GetCompletionsAsync(document, position, trigger, cancellationToken: cancelToken).Result; //info: somehow GetCompletionsAsync is not async
		});
		APerf.NW();
		if(all == null) return;
		//Print(all.Items);
		Print(all.Items.Length);

	}
}
