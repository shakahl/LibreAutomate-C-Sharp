using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Shared.Extensions;

partial class SciCode
{
	/// <summary>
	/// Replaces text without losing markers, expanding folded code, etc.
	/// </summary>
	public void ZReplaceTextGently(string s) {
		if (zIsReadonly) return;

		int len = s.Lenn(); if (len == 0) goto gRaw;
		string old = zText;
		if (len > 1_000_000 || old.Length > 1_000_000 || old.Length == 0) goto gRaw;
		var dmp = new DiffMatchPatch.diff_match_patch();
		var a = dmp.diff_main(old, s, true); //the slowest part. Timeout 1 s; then a valid but smaller.
		if (a.Count > 1000) goto gRaw;
		dmp.diff_cleanupEfficiency(a);
		using (new UndoAction(this)) {
			for (int i = a.Count - 1, j = old.Length; i >= 0; i--) {
				var d = a[i];
				if (d.operation == DiffMatchPatch.Operation.INSERT) {
					zInsertText(true, j, d.text);
				} else {
					j -= d.text.Length;
					if (d.operation == DiffMatchPatch.Operation.DELETE) zDeleteRange(true, j, j + d.text.Length);
				}
			}
		}
		return;
		gRaw:
		this.zText = s;
	}

	/// <summary>
	/// Comments out (adds // or /**/) or uncomments selected text or current line.
	/// </summary>
	/// <param name="comment">Comment out (true), uncomment (false) or toggle (null).</param>
	/// <param name="notSlashStar">Comment out lines, even if there is nut-full-line selection.</param>
	public void ZCommentLines(bool? comment, bool notSlashStar = false) {
		if (zIsReadonly) return;

		//how to comment/uncomment: // or /**/
		int selStart = zSelectionStart16, selEnd = zSelectionEnd16, replStart = selStart, replEnd = selEnd;
		bool com, slashStar = false, isSelection = selEnd > selStart;
		if (!CodeInfo.GetContextAndDocument(out var cd, selStart)) return;
		string code = cd.code, s = null;
		var root = cd.document.GetSyntaxRootAsync().Result;

		if (!notSlashStar) {
			var trivia = root.FindTrivia(selStart);
			if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)) {
				var span = trivia.Span;
				if (slashStar = comment != true && selEnd <= trivia.Span.End) {
					com = false;
					replStart = span.Start; replEnd = span.End;
					s = code[(replStart + 2)..(replEnd - 2)];
				} else notSlashStar = true;
			}
		}

		if (!slashStar) {
			//get the start and end of lines containing selection
			while (replStart > 0 && code[replStart - 1] != '\n') replStart--;
			if (!(replEnd > replStart && code[replEnd - 1] == '\n')) {
				while (replEnd < code.Length && code[replEnd] is not ('\r' or '\n')) replEnd++;
				if (replEnd > replStart && code.Eq(replEnd, "\r\n")) replEnd += 2; //prevent on Undo moving the caret to the end of line and possibly hscrolling
			}
			if (replEnd == replStart) return;
			//is the first line //comments ?
			var trivia = root.FindTrivia(replStart);
			if (trivia.IsKind(SyntaxKind.WhitespaceTrivia)) trivia = root.FindTrivia(trivia.Span.End);
			if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)) {
				com = comment ?? false;
			} else {
				if (comment == false) return;
				com = true;
				slashStar = isSelection && !notSlashStar && (selStart > replStart || selEnd < replEnd);
				if (slashStar) { replStart = selStart; replEnd = selEnd; }
			}

			s = code[replStart..replEnd];
			if (slashStar) {
				s = "/*" + s + "*/";
			} else {
				if (com) {
					s.RxFindAll(@"(?m)^[\t ]*(.*)\R?", out RXMatch[] a);
					//find smallest common indent
					int indent = 0; //tabs*4 or spaces*1
					foreach (var m in a) {
						if (m[1].Length == 0) continue;
						int n = 0; for (int i = m.Start; i < m[1].Start; i++) if (s[i] == ' ') n++; else n = (n & ~3) + 4;
						indent = indent == 0 ? n : Math.Min(indent, n);
						if (indent == 0) break;
					}
					//insert // in lines containing code
					var b = new StringBuilder();
					foreach (var m in a) {
						if (m[1].Length == 0) {
							b.Append(s, m.Start, m.Length);
						} else {
							int i = m.Start; for (int n = 0; n < indent; i++) if (s[i] == ' ') n++; else n = (n & ~3) + 4;
							b.Append(s, m.Start, i - m.Start).Append("//").Append(s, i, m.End - i);
						}
					}
					s = b.ToString();
				} else { //remove single // from all lines
					s = s.RxReplace(@"(?m)^([ \t]*)//", "$1");
				}
			}
		}

		bool caretAtEnd = isSelection && zCurrentPos16 == selEnd;
		zReplaceRange(true, replStart, replEnd, s);
		if (isSelection) {
			int i = replStart, j = replStart + s.Length;
			zSelect(true, caretAtEnd ? i : j, caretAtEnd ? j : i);
		}
	}

}
