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
using Au.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class CiAutocorrect
{
	public class BeforeCharContext
	{
		public int oldPosUtf8, newPosUtf8;
		public bool dontSuppress;
	}

	/// <summary>
	/// Call when added text with { } etc and want it ehave like when the user types { etc.
	/// </summary>
	public void BracesAdded(SciCode doc, int innerFrom, int innerTo)
	{
		doc.ZTempRanges_Add(innerFrom, innerTo, tag: this);
	}

	/// <summary>
	/// Called on Enter, Shift+Enter, Ctrl+Enter, Ctrl+; Tab and Backspace, before passing it to Scintilla. Won't pass if returns true.
	/// Enter: If before ')' or ']' and not after ',': leaves the argument list etc and adds newline, maybe semicolon, braces, indentation, and returns true.
	/// Shift+Enter or Ctrl+Enter: The same as above, but anywhere.
	/// Ctrl+;: Like SciBeforeCharAdded(';'), but anywhere; and inserts semicolon now.
	/// Tab: not impl. Returns false.
	/// Backspace: If inside an empty temp range, selects the '()' etc to erase and returns false.
	/// </summary>
	public bool SciBeforeKey(SciCode doc, Keys keyData, bool afterCompletion)
	{
		switch(keyData) {
		case Keys.Enter:
			return _OnEnterOrSemicolon(afterCompletion, anywhere: false, onSemicolon: false, out _);
		case Keys.Enter | Keys.Shift:
		case Keys.Enter | Keys.Control:
			_OnEnterOrSemicolon(afterCompletion, anywhere: true, onSemicolon: false, out _);
			return true;
		case Keys.OemSemicolon | Keys.Control:
			_OnEnterOrSemicolon(afterCompletion, anywhere: true, onSemicolon: true, out _);
			return true;
		case Keys.Tab:
			return false; //TODO: if in argument list, add or select next argument. Even if in string or comment.
		case Keys.Back:
			return SciBeforeCharAdded(doc, (char)8, out _);
		default:
			Debug.Assert(false);
			return false;
		}
	}

	/// <summary>
	/// Called on WM_CHAR, before passing it to Scintilla. Won't pass if returns true, unless ch is ';'.
	/// If ch is ')' etc, and at current position is ')' etc previously added on '(' etc, clears the temp range, sets the out vars and returns true.
	/// If ch is ';' inside '(...)' and the terminating ';' is missing, sets newPosUtf8 = where ';' should be and returns true.
	/// </summary>
	public bool SciBeforeCharAdded(SciCode doc, char ch, out BeforeCharContext c)
	{
		c = null;
		bool isBackspace = false;

		switch(ch) {
		case ';':
			return _OnEnterOrSemicolon(afterCompletion: false, anywhere: false, onSemicolon: true, out c);
		case '\"': case '\'': case ')': case ']': case '}': case '>': break;
		case (char)8: isBackspace = true; break;
		default: return false;
		}

		int pos = doc.Z.CurrentPos8;
		var r = doc.ZTempRanges_Enum(pos, this, endPosition: (ch == '\"' || ch == '\''), utf8: true).FirstOrDefault();
		if(r == null) return false;
		r.GetCurrentFromTo(out int from, out int to, utf8: true);

		if(isBackspace) {
			if(pos != from) return false;
		} else {
			if(ch != (char)doc.Call(Sci.SCI_GETCHARAT, to)) return false; //'\0' if posUtf8 invalid
		}
		for(int i = pos; i < to; i++) switch((char)doc.Call(Sci.SCI_GETCHARAT, i)) { case ' ': case '\r': case '\n': case '\t': break; default: return false; } //eg space before '}'

		r.Remove();

		if(isBackspace) {
			doc.Call(Sci.SCI_SETSEL, pos - 1, to + 1); //select and pass to Scintilla, let it delete
			return false;
		}

		c = new BeforeCharContext { oldPosUtf8 = pos, newPosUtf8 = to + 1 };
		return true;
	}

	/// <summary>
	/// Called on SCN_CHARADDED. If ch is '(' etc, adds ')' etc.
	/// </summary>
	public void SciCharAdded(CodeInfo.CharContext c)
	{
		char ch = c.ch;
		string replaceText = ch switch { '\"' => "\"", '\'' => "'", '(' => ")", '[' => "]", '{' => "}", '<' => ">", '*' => "*/", _ => null };
		if(replaceText == null) return;

		if(!CodeInfo.GetContextAndDocument(out var cd)) return;
		string code = cd.code;
		int pos = cd.position - 1; if(pos < 0) return;

		Debug.Assert(code[pos] == ch);
		if(code[pos] != ch) return;

		bool isBeforeWord = cd.position < code.Length && char.IsLetterOrDigit(code[cd.position]); //usually user wants to enclose the word manually, unless typed '{' in interpolated string
		if(isBeforeWord && ch != '{') return;

		var root = cd.document.GetSyntaxRootAsync().Result;
		//if(!root.ContainsDiagnostics) return; //no. Don't use errors. It can do more bad than good. Tested.

		int replaceLength = 0, tempRangeFrom = cd.position, tempRangeTo = cd.position, newPos = 0;

		if(ch == '*') { /**/
			var trivia = root.FindTrivia(pos);
			if(!trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)) return;
			if(trivia.SpanStart != --pos) return;
			tempRangeFrom = 0;
		} else {
			var node = root.FindToken(pos).Parent;
			var kind = node.Kind();
			if(kind == SyntaxKind.InterpolatedStringText) {
				node = node.Parent;
				kind = node.Kind();
			}

			if(isBeforeWord && kind != SyntaxKind.Interpolation) return;

			var span = node.Span;
			if(span.Start > pos) return; // > if pos is in node's leading trivia, eg comments or #if-disabled block

			//CiUtil.PrintNode(node);
			//Print("ALL");
			//CiUtil.PrintNode(root, false);

			if(ch == '\"' || ch == '\'') {
				bool isVerbatim = false, isInterpolated = false;
				switch(kind) {
				case SyntaxKind.CharacterLiteralExpression when ch == '\'':
					break;
				case SyntaxKind.StringLiteralExpression when ch == '\"':
					isVerbatim = code[span.Start] == '@';
					break;
				case SyntaxKind.InterpolatedStringExpression when ch == '\"':
					isInterpolated = true;
					isVerbatim = code[span.Start] == '@' || code[span.Start + 1] == '@';
					break;
				default: return;
				}

				if(span.Start != pos - (isVerbatim ? 1 : 0) - (isInterpolated ? 1 : 0)) {
					if(!isVerbatim) return;
					//inside verbatim string replace " with ""
					cd.position--; //insert " before ", and let caret be after ""
					tempRangeFrom = 0;
				}
			} else {
				//Print(kind);
				if(ch == '<' && !(kind == SyntaxKind.TypeParameterList || kind == SyntaxKind.TypeArgumentList)) return; //can be operators
				switch(kind) {
				case SyntaxKind.CompilationUnit:
				case SyntaxKind.CharacterLiteralExpression:
				case SyntaxKind.StringLiteralExpression:
					return;
				case SyntaxKind.InterpolatedStringExpression:
					//after next typed { in interpolated string remove } added after first {
					if(ch == '{' && code.Eq(pos - 1, "{{}") && c.doc.ZTempRanges_Enum(cd.position, this, endPosition: true).Any()) {
						replaceLength = 1;
						replaceText = null;
						tempRangeFrom = 0;
						break;
					}
					return;
				default:
					if(_IsInNonblankTrivia(node, pos)) return;
					if(ch == '{' && kind != SyntaxKind.Interpolation) {
						replaceText = "  }";
						if(pos > 0 && !char.IsWhiteSpace(code[pos - 1])) {
							replaceText = " {  }";
							tempRangeFrom++;
							cd.position--; replaceLength = 1; //replace the '{' too
						}
						tempRangeTo = tempRangeFrom + 2;
						newPos = tempRangeFrom + 1;
					}
					break;
				}
			}
		}

		c.doc.Z.ReplaceRange(true, cd.position, replaceLength, replaceText, true, ch == ';' ? SciFinalCurrentPos.AtEnd : default);
		if(newPos > 0) c.doc.Z.CurrentPos16 = newPos;

		if(tempRangeFrom > 0) c.doc.ZTempRanges_Add(tempRangeFrom, tempRangeTo, tag: this);
		else c.ignoreChar = true;
	}

	bool _OnEnterOrSemicolon(bool afterCompletion, bool anywhere, bool onSemicolon, out BeforeCharContext bcc)
	{
		bcc = null;

		if(!CodeInfo.GetContextWithoutDocument(out var cd)) return false;
		var code = cd.code;
		int pos = cd.position;

		if(!(pos > 0 && pos < code.Length)) return false;
		if(!anywhere) {
			char ch = code[pos]; if(!(ch == ')' || ch == ']')) return false;
			ch = code[pos - 1]; if(ch == ',') return false;
		}
		//TODO: if in argument list but not before ')' etc, need to indent.

		if(!cd.GetDocument()) return false;

		var root = cd.document.GetSyntaxRootAsync().Result;
		var tok1 = root.FindToken(pos);
		//CiUtil.PrintNode(tok1, printErrors: true);
		if(!anywhere) {
			var tokKind = tok1.Kind();
			if(!(tokKind == SyntaxKind.CloseParenToken || tokKind == SyntaxKind.CloseBracketToken) || tok1.SpanStart != cd.position) return false;
		}
		var nodeFromPos = tok1.Parent;

		SyntaxNode node = null, indentNode = null;
		bool needSemicolon = false, needBlock = false, canBeEmptyParentheses = false;
		SyntaxToken token = default; BlockSyntax block = null;
		foreach(var v in nodeFromPos.AncestorsAndSelf()) {
			if(v is StatementSyntax ss) {
				node = v;
				switch(ss) {
				case IfStatementSyntax k:
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case ForStatementSyntax k:
					if(onSemicolon && !anywhere) return false;
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case ForEachStatementSyntax k:
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case WhileStatementSyntax k:
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case FixedStatementSyntax k:
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case LockStatementSyntax k:
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case UsingStatementSyntax k:
					token = k.CloseParenToken;
					block = k.Statement as BlockSyntax;
					break;
				case SwitchStatementSyntax k:
					if(!k.OpenBraceToken.IsMissing) return false;
					token = k.CloseParenToken;
					break;
				case LocalFunctionStatementSyntax k:
					token = k.ParameterList.CloseParenToken;
					block = k.Body;
					canBeEmptyParentheses = true;
					break;
				case DoStatementSyntax k:
					needSemicolon = true;
					break;
				case BlockSyntax _:
					return false;
				default: //method invocation, assignment expression, return, throw, etc
					needSemicolon = true;
					canBeEmptyParentheses = true;
					break;
				}

				//if autocompleted with empty parentheses, don't add { } now
				if(afterCompletion && !canBeEmptyParentheses && code[pos - 1] == '(') return false;

				//if eg 'if(...) ThisNodeNotInBraces', next line must have indent of 'if'
				if(needSemicolon && node.Parent is StatementSyntax pa && !(pa is BlockSyntax)) indentNode = pa;

			} else if(v is MemberDeclarationSyntax mds) {
				node = v;
				switch(mds) {
				case MethodDeclarationSyntax k: //is MemberDeclarationSyntax
					if(node.Parent is InterfaceDeclarationSyntax || node.ChildTokens().Any(o => o.IsKind(SyntaxKind.ExternKeyword) || o.IsKind(SyntaxKind.AbstractKeyword))) {
						needSemicolon = true;
						//also may need semicolon if partial, but we don't know which part this is.
					} else {
						token = k.ParameterList.CloseParenToken;
						block = k.Body;
					}
					break;
				case BaseTypeDeclarationSyntax _: //pos is directly in the body of a class, struct, interface, enum
					return false;
				default: //field, property, delegate
					needSemicolon = true;
					break;
				}
			} else if(v is CatchClauseSyntax css) {
				if(afterCompletion && code[pos - 1] == '(') return false;
				node = v;
				block = css.Block;
				if(block != null && block.OpenBraceToken.IsMissing) block = null;
				if(block == null) token = css.Filter?.CloseParenToken ?? css.Declaration.CloseParenToken;
			} else if(v is AttributeListSyntax als) {
				node = v;
				token = als.CloseBracketToken;
				if(token.IsMissing) return false;
				break;
			} else continue;

			if(needSemicolon) {
				token = node.GetLastToken();
				needSemicolon = !token.IsKind(SyntaxKind.SemicolonToken);
			} else if(block != null) {
				token = block.OpenBraceToken;
			} else {
				needBlock = true;
				if(token.IsMissing) token = node.GetLastToken();
			}

			break;
		}
		Print("----");
		if(node == null) return false;

		CiUtil.PrintNode(node);
		//Print($"Span={ node.Span}, Span.End={node.Span.End}, FullSpan={ node.FullSpan}, SpanStart={ node.SpanStart}, EndPosition={ node.EndPosition}, FullWidth={ node.FullWidth}, HasTrailingTrivia={ node.HasTrailingTrivia}, Position={ node.Position}, Width={ node.Width}");
		//return true;

		var doc = cd.sciDoc;
		int endOfSpan = token.Span.End, endOfFullSpan = token.FullSpan.End;
		//Print(endOfSpan, endOfFullSpan);

		if(onSemicolon) {
			if(anywhere) {
				doc.Z.GoToPos(true, (needSemicolon || endOfSpan > pos) ? endOfSpan : endOfFullSpan);
				if(needSemicolon) doc.Z.ReplaceSel(";");
			} else {
				bcc = new BeforeCharContext { oldPosUtf8 = pos, newPosUtf8 = endOfSpan, dontSuppress = needSemicolon };
			}
		} else {
			int indent = doc.Z.LineIndentationFromPos(true, (indentNode ?? node).SpanStart);
			if(needBlock || block != null) indent++;
			bool indentNext = indent > 0 && code[endOfFullSpan - 1] != '\n' && endOfFullSpan < code.Length; //indent next statement (or whatever) that was in the same line

			var b = new StringBuilder();
			if(needBlock) b.Append(" {"); else if(needSemicolon) b.Append(';');

			int replaceLen = endOfFullSpan - endOfSpan;
			int endOfFullTrimmed = endOfFullSpan; while(code[endOfFullTrimmed - 1] <= ' ') endOfFullTrimmed--; //remove newline and spaces
			if(endOfFullTrimmed > endOfSpan) b.Append(code, endOfSpan, endOfFullTrimmed - endOfSpan);
			b.AppendLine();

			if(indent > 0) b.Append('\t', indent);
			b.AppendLine();

			int finalPos = endOfSpan + b.Length - 2;
			if(needBlock) {
				if(--indent > 0) b.Append('\t', indent);
				b.AppendLine("}");
			}

			int endOfBlock = block?.CloseBraceToken.SpanStart ?? 0;
			if(indentNext) {
				if(endOfFullSpan == endOfBlock) indent--;
				if(indent > 0) b.Append('\t', indent);
			}

			if(endOfBlock > endOfFullSpan) { //if block contains statements, move the closing '}' down
				bool hasNewline = false;
				while(code[endOfBlock - 1] <= ' ') if(code[--endOfBlock] == '\n') hasNewline = true;
				if(endOfBlock > endOfFullSpan) {
					replaceLen += endOfBlock - endOfFullSpan;
					b.Append(code, endOfFullSpan, endOfBlock - endOfFullSpan);
					if(!hasNewline) b.AppendLine();
					if(--indent > 0) b.Append('\t', indent);
				}
			}

			var s = b.ToString();
			Print($"'{s}'");
			doc.Z.ReplaceRange(true, endOfSpan, replaceLen, s, true);
			doc.Z.GoToPos(true, finalPos);
		}

		return true;
	}

	#region util

	//currently unused
	//static bool _GetNodeIfNotInNonblankTriviaOrStringOrChar(out SyntaxNode node, CodeInfo.Context cd)
	//{
	//	int pos = cd.position;

	//	var root = cd.document.GetSyntaxRootAsync().Result;
	//	node = root.FindToken(pos).Parent;
	//	if(node.Parent is InterpolatedStringExpressionSyntax iss) node = iss;
	//	//CiUtil.PrintNode(node, true, true);

	//	var span = node.Span;
	//	if(pos > span.Start && pos < span.End) {
	//		switch(node.Kind()) {
	//		case SyntaxKind.CharacterLiteralExpression:
	//		case SyntaxKind.StringLiteralExpression:
	//		case SyntaxKind.InterpolatedStringExpression:
	//		case SyntaxKind.CompilationUnit:
	//		case SyntaxKind.Block:
	//		case SyntaxKind.ClassDeclaration:
	//		case SyntaxKind.StructDeclaration:
	//		case SyntaxKind.InterfaceDeclaration:
	//			return false;
	//		}
	//	} else {
	//		if(pos == span.End) {
	//			switch(node.Kind()) {
	//			case SyntaxKind.CharacterLiteralExpression:
	//			case SyntaxKind.StringLiteralExpression:
	//			case SyntaxKind.InterpolatedStringExpression:
	//				if(node.ContainsDiagnostics && node.GetDiagnostics().Any(o => o.Id == "CS1010")) return false; //newline in constant
	//				break;
	//			}
	//		}
	//		if(_IsInNonblankTrivia(node, pos)) return false;
	//	}

	//	return true;
	//}

	static bool _IsInNonblankTrivia(SyntaxNode node, int pos)
	{
		var trivia = node.FindTrivia(pos);
		if(trivia.RawKind != 0) {
			//Print($"{trivia.Kind()}, {pos}, {trivia.FullSpan}, '{trivia}'");
			var ts = trivia.Span;
			if(!(pos > ts.Start && pos < ts.End)) { //pos is not inside trivia; possibly at start or end.
				bool lookBefore = pos == ts.Start && trivia.IsKind(SyntaxKind.EndOfLineTrivia) && node.FullSpan.Start < pos;
				if(!lookBefore) return false;
				trivia = node.FindTrivia(pos - 1); //can be eg single-line comment
				switch(trivia.Kind()) {
				case SyntaxKind.MultiLineCommentTrivia:
				case SyntaxKind.MultiLineDocumentationCommentTrivia:
					return false;
				}
				//CiUtil.PrintNode(trivia);
			}
			switch(trivia.Kind()) {
			case SyntaxKind.None:
			case SyntaxKind.WhitespaceTrivia:
			case SyntaxKind.EndOfLineTrivia:
				break;
			default:
				return true; //mostly comments, directives and #if-disabled text
			}
		}
		return false;
	}

	#endregion
}
