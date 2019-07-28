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
using Au.Controls;
using Microsoft.CodeAnalysis.Completion;

class CodeAssist
{
	CaWorkspace _ws;

	public void WorkspaceOpened()
	{
		//_ws = new CaWorkspace();
	}

	public void WorkspaceClosed()
	{
		//_ws.Dispose();
		//_ws = null;
	}

	public void FileOpened(PanelEdit.SciCode doc)
	{
		//var f = doc.FN;
		//if(!f.IsCodeFile) return;
		//bool isProject = _FileNodeIsProject(f);
		////Print("open", isProject, f);

		//if(isProject) {
		//	if(!_ws.AddProject(f)) return;

		//}

		//doc.CodeaData = new SciCodeData { oldText = doc.Text };
	}

	public void FileClosed(PanelEdit.SciCode doc)
	{
		if(doc.CodeaData == null) return;
		var f = doc.FN;
		if(!f.IsCodeFile) return;
		bool isProject = _FileNodeIsProject(f);
		//Print("close", isProject, f);

		doc.CodeaData = null;

		if(isProject) {
			_ws.RemoveProject(f);

		}
	}

	bool _FileNodeIsProject(FileNode f) => f.IsScript || f.GetClassFileRole() switch { FileNode.EClassFileRole.App => true, FileNode.EClassFileRole.Library => true, _ => false };

	public void TextChanged(PanelEdit.SciCode doc, in Sci.SCNotification n)
	{
		if(doc.CodeaData == null) return;
		bool added = 0 != (n.modificationType & Sci.MOD.SC_MOD_INSERTTEXT);
		var text = doc.Text;
		//var posBytes = doc.ST.CurrentPos;
		//var posChars = doc.ST.CountBytesToChars(posBytes);

		//Print(n.position, n.length, n.modificationType);

		try {_ws.UpdateText(doc.FN, text); }
		catch(Exception ex) { ADebug.Print(ex); }
	}

	public void CharAdded(PanelEdit.SciCode doc, int chi)
	{
		if(doc.CodeaData == null) return;
		if(chi > char.MaxValue) return;
		var ch = (char)chi;
		if(!(ch == '.' || ch == '_' || char.IsLetterOrDigit(ch))) return;
		//Print(chi, ch);

		//_ShowCompletions(doc, ch, false); //TODO
	}

	void _ShowCompletions(PanelEdit.SciCode doc, char ch, bool ctrlSpace)
	{
		var text = doc.Text;
		var position = doc.ST.CurrentPosChars;
		var f = doc.FN;
		var document = _ws.CurrentSolution.GetDocument(f.CaDocumentId);

		var completionService = CompletionService.GetService(document);
		var trigger = ctrlSpace ? default : CompletionTrigger.CreateInsertionTrigger(ch);
		var all = completionService.GetCompletionsAsync(document, position, trigger).Result;
		//var all = completionService.GetCompletionsAsync(_document, posChars).Result;

		if(all == null) return;
		Print(all.Items.Length);
		string spanText = all.Span.IsEmpty ? null : text.Substring(all.Span.Start, all.Span.Length);
		var r = new List<_CompletionItem>(spanText == null ? all.Items.Length : 0);
		foreach(var v in all.Items) {
			string s = v.DisplayText;
			if(spanText != null) {
				if(!s.Has(spanText, true)) continue;

			}
			//Print($"prefix: '{v.DisplayTextPrefix}'  suffix: '{v.DisplayTextSuffix}'");
			r.Add(new _CompletionItem { text = s });
		}
		Print(r);
	}

	struct _CompletionItem
	{
		public string text;

		public override string ToString()
		{
			return text;
		}
	}

	public class SciCodeData
	{
		public string oldText;
	}
}
