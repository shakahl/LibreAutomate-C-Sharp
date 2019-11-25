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
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Au.Controls;
using Microsoft.CodeAnalysis.Text;

class CiTools
{
	//#if DEBUG
	//	public static void RegexTest(int position)
	//	{
	//		var node = NodeAt(position);
	//		Print(IsInString(ref node, position));
	//	}
	//#endif

	//FUTURE: remove if unused
	/// <summary>
	/// Gets syntax node at position.
	/// If document==null, calls CodeInfo.GetDocument().
	/// </summary>
	public static SyntaxNode NodeAt(int position, Document document = null)
	{
		if(document == null) {
			if(!CodeInfo.GetContextAndDocument(out var cd, position)) return null; //returns false if position is in meta comments
			document = cd.document;
			position = cd.pos16;
		}
		var root = document.GetSyntaxRootAsync().Result;
		return root.FindToken(position).Parent;
	}

	/// <summary>
	/// Returns true if node is in a "string literal" between "" or in a text part of an $"interpolated string".
	/// </summary>
	/// <param name="node">Any node. If returns true, finally its kind is StringLiteralExpression or InterpolatedStringExpression.</param>
	/// <param name="position"></param>
	public static bool IsInString(ref SyntaxNode node, int position)
	{
		if(node == null) return false;
		var nk = node.Kind();
		//Print(nk, position, node.Span, node.GetType(), node);
		switch(nk) {
		case SyntaxKind.StringLiteralExpression:
			//return true only if position is in the string value.
			//false if <= the first " or >= the last ".
			//true if position is at the end of span and the last " is missing (error CS1010).
			var span = node.Span;
			int i = position - span.Start;
			if(i <= 0 || (i == 1 && node.ToString().Starts('@'))) return false;
			i = position - span.End;
			if(i > 0 || (i == 0 && !_NoClosingQuote(node))) return false;
			return true;
		case SyntaxKind.InterpolatedStringExpression:
			int j = node.Span.End - position;
			if(j != 1 && !(j == 0 && _NoClosingQuote(node))) return false;
			return true;
		case SyntaxKind.InterpolatedStringText:
		case SyntaxKind.Interpolation when position == node.SpanStart:
			node = node.Parent;
			nk = node.Kind();
			return nk == SyntaxKind.InterpolatedStringExpression;
		}
		return false;

		static bool _NoClosingQuote(SyntaxNode n) => n.ContainsDiagnostics && n.GetDiagnostics().Any(o => o.Id == "CS1010"); //Newline in constant
	}

	#region regex

	RegexWindow _regexWindow;
	string _regexTopic;

	public void RegexWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan, bool replace)
	{
		int j = stringSpan.Start, vi = _StringPrefixLength(code, j);

		if(!replace && (vi == 0 || !(code[j] == '@' || code[j + 1] == '@')))
			ADialog.ShowInfo(null, "Regular expression string should be like @\"text\", not like \"text\". The Regex tool will not escape \\ when inserting text.");

		if(_regexWindow == null) {
			_regexWindow = new RegexWindow();
			_regexWindow.Window.Name = "Ci.Regex"; //prevent hiding when activated
		}

		var r = CiUtil.GetCaretRectFromPos(doc, pos16);
		int i = Au.Util.ADpi.ScaleInt(100);
		r.Width = i;
		r.Inflate(i, 0);
		_regexWindow.Show(doc, r, false, PopupAlignment.TPM_CENTERALIGN | PopupAlignment.TPM_VERTICAL);
		_regexWindow.InsertInControl = doc;
		var s = _regexWindow.CurrentTopic;
		if(s == "replace") {
			if(!replace) _regexWindow.CurrentTopic = _regexTopic;
		} else if(replace) {
			_regexTopic = s;
			_regexWindow.CurrentTopic = "replace";
		}
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _regexWindow.Hide());
	}

	public void RegexWindowHide() => _regexWindow?.Hide();

	//public bool RegexWindowIsVisible => _regexWindow?.Window.Visible ?? false;

	#endregion

	static int _StringPrefixLength(string s, int j)
	{
		int R = 0;
		if(s[j] == '@') R = s[j + 1] == '$' ? 2 : 1; else if(s[j] == '$') R = s[j + 1] == '@' ? 2 : 1;
		return R;
	}

	#region keys

	KeysWindow _keysWindow;

	public void KeysWindowShow(SciCode doc, string code, int pos16, TextSpan stringSpan)
	{
		if(_keysWindow == null) {
			_keysWindow = new KeysWindow();
			_keysWindow.Window.Name = "Ci.Keys"; //prevent hiding when activated
		}
		var r = CiUtil.GetCaretRectFromPos(doc, pos16);
		int i = Au.Util.ADpi.ScaleInt(100);
		r.Width = i;
		r.Inflate(i, 0);
		_keysWindow.Show(doc, r, false, PopupAlignment.TPM_CENTERALIGN | PopupAlignment.TPM_VERTICAL);
		_keysWindow.InsertInControl = doc;
		int vi = _StringPrefixLength(code, stringSpan.Start);
		doc.ZTempRanges_Add(this, stringSpan.Start + vi + 1, stringSpan.End - 1, onLeave: () => _keysWindow.Hide());
	}

	public void KeysWindowHide() => _keysWindow?.Hide();

	#endregion

	public static void CmdShowRegexOrKeysWindow(bool regex)
	{
		if(!CodeInfo.GetDocumentAndFindNode(out var cd, out var node)) return;
		var pos16 = cd.pos16;
		if(!IsInString(ref node, pos16)) { ADialog.ShowInfo("Text cursor must be in string."); return; }
		var doc = cd.sciDoc;
		var stringSpan = node.Span;

		var t = CodeInfo._compl._tools;
		if(regex) t.RegexWindowShow(doc, cd.code, pos16, stringSpan, replace: false);
		else t.KeysWindowShow(doc, cd.code, pos16, stringSpan);
	}
}
