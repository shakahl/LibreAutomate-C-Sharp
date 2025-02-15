//CONSIDER: move snippets from XML to a workspace, like now Cookbook and Templates.
//	Maybe only code of some snippets, eg dialogs (big code).

using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;

static class CiSnippets {
	class _CiComplItemSnippet : CiComplItem {
		public readonly XElement x;
		public readonly _Context context;
		public readonly bool custom;

		public _CiComplItemSnippet(string name, XElement x, _Context context, bool custom) : base(CiComplProvider.Snippet, default, name, CiItemKind.Snippet) {
			this.x = x;
			this.context = context;
			this.custom = custom;
		}
	}

	static List<_CiComplItemSnippet> s_items;

	[Flags]
	enum _Context {
		None,
		Namespace = 1, //global, namespace{ }
		Type = 2, //class{ }, struct{ }, interface{ }
		Function = 4, //method{ }, lambda{ }
		Arrow = 8, //lambda=>, function=>
		Attributes = 16, //[Attributes]
		Unknown = 32,
		Any = 0xffff,
		Line = 0x10000, //at start of line
	}

	static _Context s_context;

	//static int s_test;
	public static void AddSnippets(List<CiComplItem> items, TextSpan span, CompilationUnitSyntax root, string code, CSharpSyntaxContext syncon) {
		//CSharpSyntaxContext was discovered later and therefore almost not used here.
		if (syncon.IsObjectCreationTypeContext) return;
		//CiUtil.GetContextType(syncon);

		//print.clear(); print.it(++s_test);

		//print.clear();
		//foreach (var v in root.ChildNodes()) {
		//	CiUtil.PrintNode(v);
		//}
		//print.it("---");

		_Context context = _Context.Unknown;
		int pos = span.Start;

		//get node from start
		var token = root.FindToken(pos);
		var node = token.Parent;
		//CiUtil.PrintNode(node); //print.it("--");
		//return;

		//find ancestor/self that contains pos inside
		while (node != null && !node.Span.ContainsInside(pos)) node = node.Parent;
		//CiUtil.PrintNode(node);
		//for(var v = node; v != null; v = v.Parent) print.it(v.GetType().Name, v is ExpressionSyntax, v is ExpressionStatementSyntax);

		//print.it(SyntaxFacts.IsTopLevelStatement);
		//print.it(SyntaxFacts.IsInNamespaceOrTypeContext); //not tested

		switch (node) {
		case BlockSyntax:
		case SwitchSectionSyntax: //between case: and break;
		case ElseClauseSyntax:
		case LabeledStatementSyntax:
		case IfStatementSyntax s1 when pos > s1.CloseParenToken.SpanStart:
		case WhileStatementSyntax s2 when pos > s2.CloseParenToken.SpanStart:
		case DoStatementSyntax s3 when pos < s3.WhileKeyword.SpanStart:
		case ForStatementSyntax s4 when pos > s4.CloseParenToken.SpanStart:
		case CommonForEachStatementSyntax s5 when pos > s5.CloseParenToken.SpanStart:
		case LockStatementSyntax s6 when pos > s6.CloseParenToken.SpanStart:
		case FixedStatementSyntax s7 when pos > s7.CloseParenToken.SpanStart:
		case UsingStatementSyntax s8 when pos > s8.CloseParenToken.SpanStart:
			context = _Context.Function;
			break;
		case TypeDeclarationSyntax td when pos > td.OpenBraceToken.Span.Start: //{ } of class, struct, interface
			context = _Context.Type;
			break;
		case NamespaceDeclarationSyntax ns when pos > ns.OpenBraceToken.Span.Start:
		case CompilationUnitSyntax:
		case null:
			context = _Context.Namespace | _Context.Function; //Function for C# 9 top-level statements. //FUTURE: only if in correct place.
			break;
		case LambdaExpressionSyntax:
		case ArrowExpressionClauseSyntax: //like void F() =>here
			context = _Context.Arrow;
			break;
		case AttributeListSyntax:
			context = _Context.Attributes;
			break;
		default:
			if (span.IsEmpty) { //if '=> here;' or '=> here)' etc, use =>
				var t2 = token.GetPreviousToken();
				if (t2.IsKind(SyntaxKind.EqualsGreaterThanToken) && t2.Parent is LambdaExpressionSyntax) context = _Context.Arrow;
			}
			break;
		}
		//print.it(context);
		s_context = context;

		if (s_items == null) {
			var a = new List<_CiComplItemSnippet>();
			if (!filesystem.exists(CustomFile).File) {
				try { filesystem.copy(folders.ThisAppBS + @"Default\Snippets2.xml", CustomFile); }
				catch { goto g1; }
			}
			_LoadFile(CustomFile, true);
		g1: _LoadFile(DefaultFile, false);
			if (a.Count == 0) return;
			s_items = a;

			void _LoadFile(string file, bool custom) {
				try {
					var xroot = XmlUtil.LoadElem(file);
					foreach (var xg in xroot.Elements("group")) {
						if (!xg.Attr(out string sc, "context")) continue;
						_Context con = default;
						if (sc == "Function") con = _Context.Function; //many
						else { //few, eg Type or Namespace|Type
							foreach (var seg in sc.Segments("|")) {
								switch (sc[seg.Range]) {
								case "Function": con |= _Context.Function; break;
								case "Type": con |= _Context.Type; break;
								case "Namespace": con |= _Context.Namespace; break;
								case "Arrow": con |= _Context.Arrow; break;
								case "Attributes": con |= _Context.Attributes; break;
								case "Any": con |= _Context.Any; break;
								case "Line": con |= _Context.Line; break;
								}
							}
						}
						if (con == default) continue;
						foreach (var xs in xg.Elements("snippet")) {
							a.Add(new _CiComplItemSnippet(xs.Attr("name"), xs, con, custom));
						}
					}
				}
				catch (Exception ex) { print.it("Failed to load snippets from " + file + "\r\n\t" + ex.ToStringWithoutStack()); }
			}
			//FUTURE: support $selection$. Add menu Edit -> Surround -> Snippet1|Snippet2|....
			//FUTURE: snippet editor, maybe like in Eclipse.
		}

		bool isLineStart = InsertCodeUtil.IsLineStart(code, pos);

		foreach (var v in s_items) {
			if (!v.context.HasAny(context)) continue;
			if (v.context.Has(_Context.Line) && !isLineStart) continue;
			v.group = 0; v.hidden = 0; v.hilite = 0; v.moveDown = 0;
			v.ci.Span = span;
			items.Add(v);
		}
	}

	public static int Compare(CiComplItem i1, CiComplItem i2) {
		if (i1 is _CiComplItemSnippet s1 && i2 is _CiComplItemSnippet s2) {
			if (!s1.custom) return 1; else if (!s2.custom) return -1; //sort custom first
		}
		return 0;
	}

	public static System.Windows.Documents.Section GetDescription(CiComplItem item) {
		var snippet = item as _CiComplItemSnippet;
		var m = new CiText();
		m.StartParagraph();
		m.Append("Snippet "); m.Bold(item.Text); m.Append(".");
		_AppendInfo(snippet.x);
		bool isList = snippet.x.HasElements;
		if (isList) {
			foreach (var v in snippet.x.Elements("list")) {
				m.Separator();
				m.StartParagraph();
				m.Append(StringUtil.RemoveUnderlineChar(v.Attr("item")));
				_AppendInfo(v);
				_AppendCode(v);
			}
		} else {
			_AppendCode(snippet.x);
		}
		if (snippet.x.Attr(out string more, "more")) {
			if (isList) m.Separator();
			m.StartParagraph(); m.Append(more); m.EndParagraph();
		}
		return m.Result;

		void _AppendInfo(XElement x) {
			if (x.Attr(out string info, "info")) m.Append(" " + info);
			m.EndParagraph();
		}

		void _AppendCode(XElement x) {
			m.CodeBlock(x.Value.Replace("$end$", ""));
		}
	}

	public static void Commit(SciCode doc, CiComplItem item, int codeLenDiff) {
		var snippet = item as _CiComplItemSnippet;
		string s, usingDir;
		var ci = item.ci;
		int pos = ci.Span.Start, endPos = pos + ci.Span.Length + codeLenDiff;

		//list of snippets?
		var x = snippet.x;
		if (x.HasElements) {
			var a = snippet.x.Elements("list").ToArray();
			var m = new popupMenu();
			foreach (var v in a) m.Add(v.Attr("item"));
			m.FocusedItem = m.Items.First();
			int g = m.Show(PMFlags.ByCaret | PMFlags.Underline);
			if (g == 0) return;
			x = a[g - 1];
		}
		s = x.Value;

		//##directive -> #directive
		if (s.Starts('#') && doc.zText.Eq(pos - 1, '#')) s = s[1..];

		//maybe need more code before
		if (x.Attr(out string before, "before") || snippet.x.Attr(out before, "before")) {
			if (doc.zText.Find(before, 0..pos) < 0) s = before + "\r\n" + s;
		}

		//replace $guid$ and $random$. Note: can be in the 'before' code.
		int j = s.Find("$guid$");
		if (j >= 0) s = s.ReplaceAt(j, 6, Guid.NewGuid().ToString());
		j = s.Find("$random$");
		if (j >= 0) s = s.ReplaceAt(j, 8, new Random().Next().ToString());

		//get variable name from code
		if (x.Attr(out string avar, "var") || snippet.x.Attr(out avar, "var")) {
			if (avar.RxMatch(@"^(.+?),(.+)$", out var m)) {
				var t = InsertCodeUtil.GetNearestLocalVariableOfType(m[1].Value);
				s = s.Replace("$var$", t?.Name ?? m[2].Value);
			}
		}

		//remove ';' if in =>
		if (s.Ends(';') && s_context == _Context.Arrow) {
			if (doc.zText.RxIsMatch(@"\s*[;,)\]]", RXFlags.ANCHORED, endPos..)) s = s[..^1];
		}

		usingDir = x.Attr("using") ?? snippet.x.Attr("using");

		//if multiline, add indentation
		s = InsertCodeUtil.IndentStringForInsertSimple(s, doc, pos);

		//$end$ sets final position. Or $end$select_text$end$. Show signature if like Method($end$.
		int selectLength = 0;
		bool showSignature = false;
		(int from, int to) tempRange = default;
		int i = s.Find("$end$");
		if (i >= 0) {
			s = s.Remove(i, 5);
			j = s.Find("$end$", i);
			if (j >= i) { s = s.Remove(j, 5); selectLength = j - i; }

			showSignature = s.RxIsMatch(@"\w[([][^)\]]*""?$", range: ..i);
			if (selectLength == 0) {
				if (s.Eq(i - 1, "()") || s.Eq(i - 1, "[]") || s.Eq(i - 1, "\"\"")) tempRange = (i, i);
				else if (s.Eq(i - 2, "{  }")) tempRange = (i - 1, i + 1);
			}
		}

		//maybe need using directives
		if (usingDir != null) {
			int len1 = doc.zLen16;
			if (InsertCode.UsingDirective(usingDir)) {
				int lenDiff = doc.zLen16 - len1;
				pos += lenDiff;
				endPos += lenDiff;
			}
		}

		CodeInfo.Pasting(doc, silent: true);
		doc.zReplaceRange(true, pos, endPos, s, moveCurrentPos: i < 0);

		if (i >= 0) {
			int newPos = pos + i;
			doc.zSelect(true, newPos, newPos + selectLength, makeVisible: true);
			if (tempRange != default) CodeInfo._correct.BracketsAdded(doc, pos + tempRange.from, pos + tempRange.to, default);
			if (showSignature) CodeInfo.ShowSignature();
		}
	}

	public static readonly string DefaultFile = folders.ThisApp + @"Default\Snippets.xml";
	public static readonly string CustomFile = AppSettings.DirBS + "Snippets.xml";
}
