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
		public string code;
		public string info;
		public _CiComplItemSnippet(CompletionItem ci, XElement x) : base(ci)
		{
			this.code = x.Value;
			this.info = x.Attr("info");
		}
	}

	static List<_CiComplItemSnippet> s_items;
	static ImmutableArray<string> s_tags = ImmutableArray.Create(WellKnownTags.Snippet);

	public static void AddSnippets(List<CiComplItem> items, TextSpan span, SyntaxNode node)
	{
		//don't add snippets if in arguments etc
		//AOutput.Clear();
		//CiUtil.PrintNode(node);
		if(node is ArgumentListSyntax als) { //return unless => is at left (lambda expression)
			var left = node.FindToken(span.Start).GetPreviousToken().Parent;
			//CiUtil.PrintNode(left);
			if(!(left is SimpleLambdaExpressionSyntax)) return;
		} else {
#if false //TODO
			//find ancestor/self that contains span.Start inside
			while(node != null && !node.Span.ContainsInside(span.Start)) node = node.Parent;
			for(var v = node; v != null; v = v.Parent) AOutput.Write(v.GetType().Name);
			switch(node) {
			case null:
			case CompilationUnitSyntax _:
			case BlockSyntax _:
			case SwitchSectionSyntax _:
			case IfStatementSyntax _: //unfinished
			case ElseClauseSyntax _:
			case WhileStatementSyntax _: //unfinished
				//... unfinished
			case SimpleLambdaExpressionSyntax _:
			case ArrowExpressionClauseSyntax _: //like void F() =>
			case TypeDeclarationSyntax td when span.Start > td.OpenBraceToken.Span.Start: //{ } of class, struct, interface
			case NamespaceDeclarationSyntax ns when span.Start > ns.OpenBraceToken.Span.Start:
				break;
			default: return;
			}
#endif
		}

		if(s_items == null) {
			try {
				var xroot = XElement.Load(AFolders.ThisApp + @"Default\Snippets.xml");
				var a = new List<_CiComplItemSnippet>();
				foreach(var x in xroot.Elements("snippet")) {
					var ci = CompletionItem.Create(x.Attr("name"), tags: s_tags);
					a.Add(new _CiComplItemSnippet(ci, x));
				}
				s_items = a;
			}
			catch(Exception ex) { AOutput.Write(ex.ToStringWithoutStack()); return; }
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
		var s = snippet.code;
		s = s.Replace("$end$", "");
		s = WebUtility.HtmlEncode(s);
		var b = new StringBuilder("<body><div>Snippet <b>");
		b.Append(item.DisplayText).Append("</b>.");
		if(snippet.info != null) b.Append(' ').Append(snippet.info);
		b.Append("</div><code><pre>").Append(s);
		if(!s.Ends('\n')) b.AppendLine();
		b.Append("</pre></code></body>");
		return b.ToString();
	}

	public static CompletionChange GetCompletionChange(CiComplItem item, out int selectLength, out bool showSignature)
	{
		selectLength = 0; showSignature = false;
		var s = (item as _CiComplItemSnippet).code;

		//if multiline, add indentation
		if(s.Contains('\n')) {
			int indent = Panels.Editor.ZActiveDoc.Z.LineIndentationFromPos(true, item.ci.Span.Start);
			AOutput.Write(indent);
			if(indent > 0) s = s.RegexReplace(@"(?m)\n\K", new string('\t', indent));
		}

		//$end$ sets final position. Or $end$select_text$end$. Show signature if like Method($end$.
		int i = s.Find("$end$");
		if(i < 0) i = s.Length;
		else {
			s = s.Remove(i, 5);
			int j = s.Find("$end$", i);
			if(j >= i) { s = s.Remove(j, 5); selectLength = j - i; }

			showSignature = s.RegexIsMatch(@"\w ?\([^\)]*""?$", range: 0..i);
		}

		var ci = item.ci;
		return CompletionChange.Create(new TextChange(ci.Span, s), ci.Span.Start + i);
	}
}
