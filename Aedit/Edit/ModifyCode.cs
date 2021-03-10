using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;
using static Au.Controls.Sci;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Shared.Extensions;

partial class SciCode
{
	/// <summary>
	/// Replaces text without losing markers, expanding folded code, etc.
	/// </summary>
	public void ZReplaceTextGently(string s)
	{
		if(zIsReadonly) return;

		int len = s.Lenn(); if(len == 0) goto gRaw;
		string old = zText;
		if(len > 1_000_000 || old.Length > 1_000_000 || old.Length == 0) goto gRaw;
		var dmp = new DiffMatchPatch.diff_match_patch();
		var a = dmp.diff_main(old, s, true); //the slowest part. Timeout 1 s; then a valid but smaller.
		if(a.Count > 1000) goto gRaw;
		dmp.diff_cleanupEfficiency(a);
		Call(SCI_BEGINUNDOACTION);
		for(int i = a.Count - 1, j = old.Length; i >= 0; i--) {
			var d = a[i];
			if(d.operation == DiffMatchPatch.Operation.INSERT) {
				zInsertText(true, j, d.text);
			} else {
				j -= d.text.Length;
				if(d.operation == DiffMatchPatch.Operation.DELETE) zDeleteRange(true, j, j + d.text.Length);
			}
		}
		Call(SCI_ENDUNDOACTION);
		return;
		gRaw:
		this.zText = s;
	}

	/// <summary>
	/// Comments (adds // or /**/) or uncomments selected text or current line.
	/// </summary>
	/// <param name="comment">Comment (true), uncomment (false) or toggle (null).</param>
	public void ZCommentLines(bool? comment)
	{
		if(zIsReadonly) return;

		//how to comment/uncomment: // or /**/
		int selStart = zSelectionStar16, selEnd = zSelectionEnd16, replStart = selStart, replEnd = selEnd;
		bool com, slashStar = false, notSlashStar = false, isSelection = selEnd > selStart;
		if(!CodeInfo.GetContextAndDocument(out var cd, selStart)) return;
		string code = cd.code, s = null;
		var root = cd.document.GetSyntaxRootAsync().Result;
		var trivia = root.FindTrivia(selStart);
		if(trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)) {
			var span = trivia.Span;
			if(slashStar = comment != true && selEnd <= trivia.Span.End) {
				com = false;
				replStart = span.Start; replEnd = span.End;
				s = code[(replStart + 2)..(replEnd - 2)];
			} else notSlashStar = true;
		}
		if(!slashStar) {
			//get start and end of lines containing selection
			while(replStart > 0 && code[replStart - 1] != '\n') replStart--;
			if(!(replEnd > replStart && code[replEnd - 1] == '\n')) while(replEnd < code.Length && code[replEnd] != '\r' && code[replEnd] != '\n') replEnd++;
			if(replEnd == replStart) return;
			//is the first line //comments ?
			trivia = root.FindTrivia(replStart);
			if(trivia.IsKind(SyntaxKind.WhitespaceTrivia)) trivia = root.FindTrivia(trivia.Span.End);
			if(trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)) {
				com = comment ?? false;
			} else {
				if(comment == false) return;
				com = true;
				slashStar = isSelection && !notSlashStar && (selStart > replStart || selEnd < replEnd);
				if(slashStar) { replStart = selStart; replEnd = selEnd; }
			}
			s = code[replStart..replEnd];
			if(slashStar) {
				s = "/*" + s + "*/";
			} else {
				s = com ? s.RegexReplace(@"(?m)^", "//") : s.RegexReplace(@"(?m)^([ \t]*)//", "$1");
			}
		}
		bool caretAtEnd = isSelection && zCurrentPos16 == selEnd;
		zReplaceRange(true, replStart, replEnd, s);
		if(isSelection) {
			int i = replStart, j = replStart + s.Length;
			zSelect(true, caretAtEnd ? i : j, caretAtEnd ? j : i);
		}
	}

}
