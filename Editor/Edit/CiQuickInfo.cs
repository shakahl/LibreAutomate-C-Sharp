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
using System.Linq;
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

class CiQuickInfo
{
#if true
	public void SciMouseDwellStarted(int positionUtf8, int x, int y)
	{
	}
#else

	public void SciMouseDwellStarted(int positionUtf8, int x, int y)
	{
		return;
		//_htmlToolTip?.Hide(doc);

		//Print("dwell");
		APerf.First();
		var document = CodeInfo.CreateTestDocumentForEditorCode(out string code, out _);

		//var workspace = document.Project.Solution.Workspace;
		//var descriptionService = workspace.Services.GetLanguageServices("C#").GetService<Microsoft.CodeAnalysis.LanguageServices.ISymbolDisplayService>();
		//Print(descriptionService);
		//var sections = descriptionService.ToDescriptionGroupsAsync(workspace, document.GetSemanticModelAsync().Result, position, symbols.AsImmutable()).Result;

		APerf.Next();
		var doc = Panels.Editor.ActiveDoc;
		int position = doc.ST.CountBytesToChars(positionUtf8);
		Print("dwell", position);
		var context = new QuickInfoContext(document, position, default);

		APerf.Next();
		var provider = new CSharpSemanticQuickInfoProvider();
		var r = provider.GetQuickInfoAsync(context).Result;
		//APerf.NW();
		if(r == null) return;

		//Print("-----");
		//Print(r.Span, r.RelatedSpans, r.Sections, r.Tags);
		//Print(r.Tags);
		//Print(r.Sections);

		var b = new StringBuilder();

		string descr = null, docComm = null;
		foreach(var se in r.Sections) {
			//Print(se.Kind, se.Text);
			switch(se.Kind) {
			case QuickInfoSectionKinds.Description: descr = se.Text; break;
			case QuickInfoSectionKinds.DocumentationComments: docComm = se.Text; break;
			}
			//Print(se.TaggedParts);
			//foreach(var tp in se.TaggedParts) {
			//	Print(tp.Text, tp.Tag);
			//}
		}
		if(docComm == null && r.Tags[0] == "Namespace" && descr != null && descr.RegexMatch(@"^namespace ([\w\.]+)", 1, out string ns)) {
			docComm = MetaReferences.GetNamespaceDoc(ns);
			if(docComm != null) Print("DocumentationComments", docComm);
		}

		if(_tt == null) {
			_tt = new HtmlToolTip();
			this._tt.AllowLinksHandling = true;
			this._tt.BaseStylesheet = null;
			this._tt.MaximumSize = new System.Drawing.Size(0, 0);
			this._tt.OwnerDraw = true;
			this._tt.TooltipCssClass = "htmltooltip";
		}
		var xy = doc.MouseClientXY(); xy.x -= 10; xy.y -= 1;
		_ttSpan = r.Span;
		_tt.Show("tooltip <b>bold</b>", doc, xy);
		//ATimer.After(600, () => _htmlToolTip.Hide(doc));

		var fi = typeof(ToolTip).GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
		var nw = fi.GetValue(_tt) as NativeWindow;
		_ttWnd = (AWnd)nw.Handle;
		//Print(_ttWnd);
	}
#endif

	HtmlToolTip _tt;
	AWnd _ttWnd;
	TextSpan _ttSpan;

	public void SciMouseDwellEnded()
	{
		//Print("end", AWnd.FromMouse());
		//_htmlToolTip?.Hide(doc);
		if(_tt != null) {
			//Print(_htmlToolTip.Visible);
			//Print((AWnd)_htmlToolTip.)
		}
	}

	public void SciMouseMoved(int x, int y)
	{
		if(!_ttSpan.IsEmpty) {
			var doc = Panels.Editor.ActiveDoc;
			int pos = doc.ST.PosFromXY((x, y), minusOneIfFar: true);
			if(pos >= 0) pos = doc.ST.CountBytesToChars(pos);
			Print(pos);
			if(!_ttSpan.Contains(pos)) {
				_ttSpan = default;
				_tt.Hide(doc);
			}
		}
	}
}
