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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Au.Controls;

class CiTools
{
#if DEBUG
	public static void RegexTest(int position)
	{
		var node = NodeAt(position);
		Print(IsInString(ref node, position));
	}
#endif

	/// <summary>
	/// Gets syntax node at position.
	/// If document==null, calls CodeInfo.GetDocument().
	/// </summary>
	public static SyntaxNode NodeAt(int position, Document document = null)
	{
		if(document == null) {
			if(!CodeInfo.GetContextAndDocument(out var cd, position)) return null; //returns false if position is in meta comments
			document = cd.document;
			position = cd.position;
		}
		var root = document.GetSyntaxRootAsync().Result;
		return root.FindToken(position).Parent;
	}

	/// <summary>
	/// Returns true if node is in a "string literal" value or in a literal part of an $"interpolated string".
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

	/// <summary>
	/// Returns 1 if node is a regex pattern string argument of a known ARegex function or is in a wildcard expression string.
	/// Returns 2 if node is a regex replacement string argument of a known ARegex function.
	/// Else returns 0.
	/// node must be StringLiteralExpression or InterpolatedStringExpression.
	/// </summary>
	public static int IsInRegexString(SemanticModel model, SyntaxNode node, string code, int position)
	{
		Debug.Assert(node.IsKind(SyntaxKind.StringLiteralExpression) || node.IsKind(SyntaxKind.InterpolatedStringExpression)); //caller should call IsInString before
		int R = 0;
		//is an ARegex function?
		if(node.Parent is ArgumentSyntax n1 && n1.Parent is ArgumentListSyntax n2) {
			switch(n2.Parent) {
			case ObjectCreationExpressionSyntax oce when n1 == n2.Arguments[0]:
				if(oce.Type.ToString() == "ARegex") R = 1;
				break;
			case InvocationExpressionSyntax ies when ies.Expression is MemberAccessExpressionSyntax maes:
				var args = n2.Arguments; bool arg0 = n1 == args[0];
				if(arg0 | (args.Count > 1 && n1 == args[1])) {
					var name = maes.Name.ToString();
					if(arg0) {
						switch(name.Like(false, "Regex*", "ExpandReplacement")) {
						case 1 when _IsType("String"): R = 1; break;
						case 2 when _IsType("RXMatch"): R = 2; break;
						}
					} else {
						switch(name.Eq(false, "RegexReplace", "Replace")) {
						case 1 when _IsType("String"): R = 2; break;
						case 2 when _IsType("ARegex"): R = 2; break;
						}
					}
					bool _IsType(string type) => model.GetTypeInfo(maes.Expression).Type?.Name == type;
				}
				break;
			}
		}
		//is wildcard expression containing regex?
		if(R == 0) {
			var ss = code.Substring(node.Span.Start, node.Span.Length);
			if(ss.RegexMatch(@"^[\$@]*""\*\*c?rc? ", 0, out RXGroup rg) && position >= node.Span.Start + rg.Length) R = 1;
		}
		//must be verbatim string, because the regex window does not escape backslashes, and it is good
		if(R == 1) {
			int i = node.SpanStart;
			bool verbatim = code[i] == '@' || (code[i] == '$' && code[i + 1] == '@');
			if(!verbatim) {
				Print("<>Regular expression string should be like <c brown>@\"text\"<>, not like <c brown>\"text\"<>.");
				return 0;
			}
		}
		return R;
	}

	RegexWindow _regexWindow;
	string _regexTopic;

	public bool RegexWindowIsVisible => _regexWindow?.Window.Visible ?? false;

	public void RegexWindowShow(SciCode doc, int position, bool replace)
	{
		if(_regexWindow == null) {
			_regexWindow = new RegexWindow();
			_regexWindow.Window.Name = "Ci.Regex"; //prevent hiding when activated
		}
		var r = CiUtil.GetCaretRectFromPos(doc, position);
		int i = Au.Util.ADpi.ScaleInt(100);
		r.Width = i;
		r.Inflate(i, 16);
		_regexWindow.Show(doc, r, false, PopupAlignment.TPM_CENTERALIGN | PopupAlignment.TPM_VERTICAL);
		_regexWindow.InsertInControl = doc;
		var s = _regexWindow.CurrentTopic;
		if(s == "replace") {
			if(!replace) _regexWindow.CurrentTopic = _regexTopic;
		} else if(replace) {
			_regexTopic = s;
			_regexWindow.CurrentTopic = "replace";
		}
	}

	public void RegexWindowHideIfNotInString(SciCode doc)
	{
		if(!RegexWindowIsVisible) return;
		int position = doc.Z.CurrentPos16;
		var node = NodeAt(position);
		if(!IsInString(ref node, position)) _regexWindow.Hide();
	}

	public void RegexWindowHide() => _regexWindow?.Hide();

	#endregion
}
