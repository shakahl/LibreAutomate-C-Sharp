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
using Microsoft.Win32;
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
using System.Xml.Linq;
using System.Net;

using Au;
using Au.Types;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Completion;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Tags;

static class CiSnippets
{
	class _CiComplItemSnippet : CiComplItem
	{
		public readonly XElement x;
		public _CiComplItemSnippet(CompletionItem ci, XElement x) : base(ci)
		{
			this.x = x;
		}
	}

	static List<_CiComplItemSnippet> s_items;
	static ImmutableArray<string> s_tags = ImmutableArray.Create(WellKnownTags.Snippet);
	static DateTime s_customFileTime;

	//static int s_test;
	public static void AddSnippets(List<CiComplItem> items, TextSpan span, SyntaxNode node)
	{
		//don't add snippets if in arguments etc.
		//	Add only where can be a statement or declaration or likely a directive.
		//	The caller also does some simple filtering.
		//AOutput.Clear(); AOutput.Write(++s_test);
		int pos = span.Start;
		//find ancestor/self that contains pos inside
		while(node != null && !node.Span.ContainsInside(pos)) node = node.Parent;
		if(node is ArgumentListSyntax als) { //return unless => is at left (lambda expression)
			var left = node.FindToken(pos).GetPreviousToken().Parent;
			//CiUtil.PrintNode(left);
			if(!(left is SimpleLambdaExpressionSyntax)) return;
		} else {
			//for(var v = node; v != null; v = v.Parent) AOutput.Write(v.GetType().Name);
			switch(node) {
			case null:
			case CompilationUnitSyntax _:
			case BlockSyntax _:
			case SimpleLambdaExpressionSyntax _:
			case ArrowExpressionClauseSyntax _: //like void F() =>
			case SwitchSectionSyntax _: //between case: and break;
			case ElseClauseSyntax _:
			case IfStatementSyntax s1 when pos > s1.CloseParenToken.SpanStart:
			case WhileStatementSyntax s2 when pos > s2.CloseParenToken.SpanStart:
			case DoStatementSyntax s3 when pos < s3.WhileKeyword.SpanStart:
			case ForStatementSyntax s4 when pos > s4.CloseParenToken.SpanStart:
			case CommonForEachStatementSyntax s5 when pos > s5.CloseParenToken.SpanStart:
			case LockStatementSyntax s6 when pos > s6.CloseParenToken.SpanStart:
			case FixedStatementSyntax s7 when pos > s7.CloseParenToken.SpanStart:
			case UsingStatementSyntax s8 when pos > s8.CloseParenToken.SpanStart:
			case TypeDeclarationSyntax td when pos > td.OpenBraceToken.Span.Start: //{ } of class, struct, interface
			case NamespaceDeclarationSyntax ns when pos > ns.OpenBraceToken.Span.Start:
				break;
			default:
				return;
			}
		}

		DateTime fileTime = default;
		if(Program.Settings.ci_complCustomSnippets == 1 && AFile.GetProperties(CustomFile, out var fp, FAFlags.DontThrow | FAFlags.UseRawPath)) fileTime = fp.LastWriteTimeUtc;
		if(fileTime != s_customFileTime) { s_customFileTime = fileTime; s_items = null; }

		if(s_items == null) {
			string file = fileTime == default ? DefaultFile : CustomFile;
			try {
				var xroot = AExtXml.LoadElem(file);
				var a = new List<_CiComplItemSnippet>();
				foreach(var x in xroot.Elements("snippet")) {
					var ci = CompletionItem.Create(x.Attr("name"), tags: s_tags);
					a.Add(new _CiComplItemSnippet(ci, x));
				}
				s_items = a;
			}
			catch(Exception ex) { AOutput.Write("Failed to load snippets from " + file + "\r\n\t" + ex.ToStringWithoutStack()); return; }
		}

		foreach(var v in s_items) {
			v.group = 0; v.hidden = 0; v.hilite = 0; v.moveDown = 0;
			v.ci.Span = span;
			items.Add(v);
		}

		items.Sort((i1, i2) => {
			var r = string.Compare(i1.DisplayText, i2.DisplayText, StringComparison.OrdinalIgnoreCase);
			if(r == 0) r = i1.kind - i2.kind;
			return r;
		});
	}

	public static string GetDescriptionHtml(CiComplItem item)
	{
		var snippet = item as _CiComplItemSnippet;
		var b = new StringBuilder("<body><div>Snippet <b>");
		b.Append(item.DisplayText).Append("</b>.");
		_AppendInfo(snippet.x);
		bool isList = snippet.x.HasElements;
		if(isList) {
			foreach(var v in snippet.x.Elements("list")) {
				b.Append("<hr><div>").Append(v.Attr("item"));
				_AppendInfo(v);
				_AppendCode(v);
			}
		} else {
			_AppendCode(snippet.x);
		}
		if(snippet.x.Attr(out string more, "more")) {
			if(isList) b.Append("<hr>");
			b.Append("<p>").Append(more).Append("</p>");
		}
		b.Append("</body>");
		return b.ToString();

		void _AppendInfo(XElement x)
		{
			if(x.Attr(out string info, "info")) b.Append(' ').Append(info);
			b.Append("</div>");
		}

		void _AppendCode(XElement x)
		{
			var s = x.Value;
			s = s.Replace("$end$", "");
			s = WebUtility.HtmlEncode(s);
			b.Append("<code><pre>").Append(s);
			if(!s.Ends('\n')) b.AppendLine();
			b.Append("</pre></code>");
		}
	}

	public static CompletionChange GetCompletionChange(SciCode doc, CiComplItem item, out int selectLength, out bool showSignature, out string usingDir)
	{
		selectLength = 0; showSignature = false; usingDir = null;
		var snippet = item as _CiComplItemSnippet;
		string s;
		var ci = item.ci;
		int pos = ci.Span.Start;

		//if helpSnippet above method, add parameters
		if(snippet.DisplayText == "helpSnippet") {
			s = snippet.x.Value;
			int j = s.Find("$signature$");
			if(j >= 0) {
				string sig = null;
				if(CodeInfo.GetContextAndDocument(out var cd, pos)) {
					var code2 = cd.code.RegexReplace(@"\w+\s+", "", 1, RXFlags.ANCHORED, pos..);
					var doc2 = cd.document.WithText(SourceText.From(code2));
					var node = doc2.GetSyntaxRootAsync().Result.FindToken(pos).Parent;
					for(; node != null && node.Span.Start >= pos; node = node.Parent) {
						if(node is MethodDeclarationSyntax md) {
							var b = new StringBuilder();
							foreach(var p in md.ParameterList.Parameters) {
								b.Append("\r\n/// <param name=\"").Append(p.Identifier.Text).Append("\"></param>");
							}
							var rt = md.ReturnType;
							if(!code2.Eq(rt.Span.Start..rt.Span.End, "void")) b.Append("\r\n/// <returns></returns>");
							sig = b.ToString();
							break;
						}
					}
				}
				s = s.ReplaceAt(j, 11, sig);
			}
		} else {
			//list of snippets?
			var x = snippet.x;
			if(x.HasElements) {
				x = null;
				var m = new AMenu { Modal = true };
				foreach(var v in snippet.x.Elements("list")) {
					m[v.Attr("item")] = o => x = v;
				}
				m.Control.Items[0].Select();
				m.Show(doc, byCaret: true);
				if(x == null) return null;
			}
			s = x.Value;

			//##directive -> #directive
			if(s.Starts('#') && doc.Text.Eq(pos - 1, '#')) s = s[1..]; 

			//maybe need more code before
				if(x.Attr(out string before, "before") && doc.Text.Find(before, 0..pos) < 0) {
				s = before + "\r\n" + s;
			}

			//replace $guid$ with new GUID. Note: can be in the 'before' code.
			int j = s.Find("$guid$");
			if(j >= 0) s = s.ReplaceAt(j, 6, Guid.NewGuid().ToString());
		}

		//maybe need a using directive
		usingDir = snippet.x.Attr("using");

		//if multiline, add indentation
		if(s.Contains('\n')) {
			int indent = doc.Z.LineIndentationFromPos(true, pos);
			if(indent > 0) s = s.RegexReplace(@"(?m)\n\K", new string('\t', indent));
		}

		//$end$ sets final position. Or $end$select_text$end$. Show signature if like Method($end$.
		int i = s.Find("$end$");
		if(i < 0) i = s.Length;
		else {
			s = s.Remove(i, 5);
			int j = s.Find("$end$", i);
			if(j >= i) { s = s.Remove(j, 5); selectLength = j - i; }

			showSignature = s.RegexIsMatch(@"\w ?[([][^)\]]*""?$", range: 0..i);
		}

		return CompletionChange.Create(new TextChange(ci.Span, s), ci.Span.Start + i);
	}

	public static readonly string DefaultFile = AFolders.ThisApp + @"Default\Snippets.xml";
	public static readonly string CustomFile = ProgramSettings.DirBS + "Snippets.xml";
}
