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
using System.Linq;

using Au;
using Au.Types;
using Au.Util;
using Au.Controls;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

static class CiUtil
{

	public static bool GetSymbolFromPos(out ISymbol sym, out CodeInfo.Context cd) {
		(sym, _, _, _) = GetSymbolEtcFromPos(out cd);
		return sym != null;
	}

	public static (ISymbol symbol, string keyword, HelpKind kind, SyntaxToken token) GetSymbolEtcFromPos(out CodeInfo.Context cd) {
		var doc = Panels.Editor.ZActiveDoc; if (doc == null) { cd = default; return default; }
		if (!CodeInfo.GetContextAndDocument(out cd)) return default;
		return GetSymbolOrKeywordFromPos(cd.document, cd.pos16, cd.code);
	}

	public static (ISymbol symbol, string keyword, HelpKind helpKind, SyntaxToken token)
		GetSymbolOrKeywordFromPos(Document document, int position, string code) {
		//using var p1 = APerf.Create();

		var tree = document.GetSyntaxTreeAsync().Result;
		var token = tree.GetTouchingTokenAsync(position, default, findInsideTrivia: true).Result;
		if (token == default) return default;

		var span = token.Span; string word = code[span.Start..span.End];
		//PrintNode(token);

		if (token.IsKind(SyntaxKind.IdentifierToken)) {
			switch (word) {
				case "var":
				case "dynamic":
				case "nameof":
				case "unmanaged": //tested cases
					return (null, word, HelpKind.ContextualKeyword, token);
			}
		} else {
			var k = token.Kind();

			//PrintNode(token.GetPreviousToken());
			//AOutput.Write(
			//	//token.IsKeyword(), //IsReservedKeyword||IsContextualKeyword, but not IsPreprocessorKeyword
			//	SyntaxFacts.IsReservedKeyword(k), //also true for eg #if
			//	SyntaxFacts.IsContextualKeyword(k)
			//	//SyntaxFacts.IsQueryContextualKeyword(k) //included in IsContextualKeyword
			//	//SyntaxFacts.IsAccessorDeclarationKeyword(k),
			//	//SyntaxFacts.IsPreprocessorKeyword(k), //true if #something or can be used in #something context. Also true for eg if without #.
			//	//SyntaxFacts.IsPreprocessorContextualKeyword(k) //badly named. True only if #something.
			//	);
			//return default;

			if (SyntaxFacts.IsReservedKeyword(k)) {
				bool preproc = (word == "if" || word == "else") && token.GetPreviousToken().IsKind(SyntaxKind.HashToken);
				if (preproc) word = "#" + word;
				return (null, word, preproc ? HelpKind.PreprocKeyword : HelpKind.ReservedKeyword, token);
			}
			if (SyntaxFacts.IsContextualKeyword(k)) {
				return (null, word, SyntaxFacts.IsAttributeTargetSpecifier(k) ? HelpKind.AttributeTarget : HelpKind.ContextualKeyword, token);
			}
			if (SyntaxFacts.IsPreprocessorKeyword(k)) {
				//if(SyntaxFacts.IsPreprocessorContextualKeyword(k)) word = "#" + word; //better don't use this internal func
				if (token.GetPreviousToken().IsKind(SyntaxKind.HashToken)) word = "#" + word;
				return (null, word, HelpKind.PreprocKeyword, token);
			}
			switch (k) {
				case SyntaxKind.StringLiteralToken:
				case SyntaxKind.InterpolatedStringTextToken:
				case SyntaxKind.InterpolatedStringEndToken:
				case SyntaxKind.InterpolatedStringStartToken:
					return (null, null, HelpKind.String, token);
			}
		}
		//note: don't pass contextual keywords to GetSemanticInfo. It may get info for something other, eg for 'new' gets the ctor method.

		ISymbol symbol = null;
		var model = document.GetSemanticModelAsync().Result;
		//p1.Next();
		bool preferGeneric = token.GetNextToken().IsKind(SyntaxKind.GreaterThanToken);
		var si = model.GetSemanticInfo(token, document.Project.Solution.Workspace, default); //works better than GetSymbolInfo, and slightly faster. Or could call GetSymbolInfo, and if its Symbol is null, use its CandidateSymbols; not tested here.
		foreach (var v in si.GetSymbols(includeType: true)) {
			bool gen = false;
			switch (v) {
				case IErrorTypeSymbol: continue;
				case INamedTypeSymbol ints when ints.IsGenericType:
				case IMethodSymbol ims when ims.IsGenericMethod:
					gen = true;
					break;
			}
			//AOutput.Write(v, gen, v.Kind);
			if (gen == preferGeneric) { symbol = v; break; }
			symbol ??= v;
		}

		return (symbol, null, default, token);
	}

	public enum HelpKind
	{
		None, ReservedKeyword, ContextualKeyword, AttributeTarget, PreprocKeyword, String
	}

	public static void OpenSymbolOrKeywordFromPosHelp() {
		string url = null;
		var (symbol, keyword, helpKind, _) = GetSymbolEtcFromPos(out _);
		if (symbol != null) {
			url = GetSymbolHelpUrl(symbol);
		} else if (keyword != null) {
			var s = helpKind switch
			{
				HelpKind.PreprocKeyword => "preprocessor directive",
				HelpKind.AttributeTarget => "attributes, ",
				_ => "keyword"
			};
			s = $"C# {s} \"{keyword}\"";
			//AOutput.Write(s); return;
			url = _GoogleURL(s);
		} else if (helpKind == HelpKind.String) {
			int i = ClassicMenu_.ShowSimple("1 C# strings|2 String formatting|3 Wildcard expression|11 Regex tool (Ctrl+Space)|12 Keys tool (Ctrl+Space)", owner: Panels.Editor.ZActiveDoc, byCaret: true);
			switch (i) {
			case 1: url = "C# strings"; break;
			case 2: url = "C# string formatting"; break;
			case 3: AHelp.AuHelp("articles/Wildcard expression"); break;
			case 11: CiTools.CmdShowRegexWindow(); break;
			case 12: CiTools.CmdShowKeysWindow(); break;
			}
			if (url != null) url = _GoogleURL(url);
		}
		if (url != null) AFile.TryRun(url);
	}

	static string _GoogleURL(string query) => "https://www.google.com/search?q=" + Uri.EscapeDataString(query);

	public static string GetSymbolHelpUrl(ISymbol sym) {
		//AOutput.Write(sym);
		//AOutput.Write(sym.IsInSource(), sym.IsFromSource());
		string query;
		IModuleSymbol metadata = null;
		foreach (var loc in sym.Locations) {
			if ((metadata = loc.MetadataModule) != null) break;
		}
		if (metadata != null) {
			query = sym.QualifiedName();
			bool au = metadata.Name == "Au.dll";
			query = query.Replace("..ctor", au ? ".-ctor" : null);
			if (au) return AHelp.AuHelpUrl(query);
			if (metadata.Name.Starts("Au.")) return null;
			string kind = (sym is INamedTypeSymbol ints) ? ints.TypeKind.ToString() : sym.Kind.ToString();
			query = query + " " + kind.Lower();
		} else if (!sym.IsInSource()) { //eg an operator of string etc
			if (!(sym is IMethodSymbol me && me.MethodKind == MethodKind.BuiltinOperator)) return null;
			//AOutput.Write(sym, sym.Kind, sym.QualifiedName());
			//query = "C# " + sym.ToString(); //eg "string.operator +(string, string)", and Google finds just Equality
			//query = "C# " + sym.QualifiedName(); //eg "System.String.op_Addition", and Google finds nothing
			query = "C# " + sym.ToString().RegexReplace(@"\(.+\)$", "", 1).Replace('.', ' '); //eg C# string operator +, not bad
		} else if (sym.IsExtern) { //[DllImport]
			query = sym.Name + " function";
		} else if (sym is INamedTypeSymbol ints && ints.IsComImport) { //[ComImport]
			query = sym.Name + " " + ints.TypeKind.ToString().Lower();
		} else {
			return null;
		}

		return _GoogleURL(query);
	}

	/// <summary>
	/// Gets rectangle of caret if it was at the specified UTF-16 position.
	/// If <i>pos16</i> less than 0, uses current caret position.
	/// </summary>
	public static RECT GetCaretRectFromPos(SciCode doc, int pos16 = -1, bool inScreen = false) {
		if (pos16 < 0) pos16 = doc.Z.CurrentPos8; else pos16 = doc.Pos8(pos16);
		int x = doc.Call(Sci.SCI_POINTXFROMPOSITION, 0, pos16), y = doc.Call(Sci.SCI_POINTYFROMPOSITION, 0, pos16);
		var r = new RECT(x, y, 1, doc.Call(Sci.SCI_TEXTHEIGHT, doc.Z.LineFromPos(false, pos16)) + 2);
		if (inScreen) doc.Hwnd.MapClientToScreen(ref r);
		return r;
	}

	public static PSFormat GetParameterStringFormat(SyntaxNode node, SemanticModel semo, bool isString) {
		var kind = node.Kind();
		//AOutput.Write(kind);
		SyntaxNode parent;
		if (isString || kind == SyntaxKind.StringLiteralExpression) parent = node.Parent;
		else if (kind == SyntaxKind.InterpolatedStringText) parent = node.Parent.Parent;
		else return PSFormat.None;

		PSFormat format = PSFormat.None;
		if (parent is ArgumentSyntax asy && parent.Parent is ArgumentListSyntax alis) {
			switch (alis.Parent) {
				case ObjectCreationExpressionSyntax oce:
					format = _GetFormat(oce);
					if (format == PSFormat.None) {
						switch (oce.Type.ToString()) { //fast if single word
							case "Regex":
							case "System.Text.RegularExpressions.Regex":
							case "RegexCompilationInfo":
							case "System.Text.RegularExpressions.RegexCompilationInfo":
								if ((object)asy == alis.Arguments[0]) format = PSFormat.Regex;
								break;
						}
					}
					break;
				case InvocationExpressionSyntax ies:
					format = _GetFormat(ies);
					if (format == PSFormat.None) {
						switch (ies.Expression.ToString()) {
							case "Regex.IsMatch":
							case "Regex.Match":
							case "Regex.Matches":
							case "Regex.Replace":
							case "Regex.Split":
								var aa = alis.Arguments;
								if (aa.Count >= 2 && (object)asy == aa[1]) format = PSFormat.Regex;
								break;
						}
					}
					break;
					//default:
					//	CiUtil.PrintNode(alis.Parent);
					//	break;
			}

			PSFormat _GetFormat(ExpressionSyntax es) {
				if (semo.GetSymbolInfo(es).Symbol is IMethodSymbol ims) {
					IParameterSymbol p = null;
					var pa = ims.Parameters;
					var nc = asy.NameColon;
					if (nc != null) {
						var name = nc.Name.Identifier.Text;
						foreach (var v in pa) if (v.Name == name) { p = v; break; }
					} else {
						int i; var aa = alis.Arguments;
						for (i = 0; i < aa.Count; i++) if ((object)aa[i] == asy) break;
						if (i >= pa.Length && pa[^1].IsParams) i = pa.Length - 1;
						if (i < pa.Length) p = pa[i];
					}
					if (p != null) {
						var fa = p.GetAttributes().FirstOrDefault(o => o.AttributeClass.Name == "ParamStringAttribute");
						if (fa != null) return fa.GetConstructorArgument<PSFormat>(0, SpecialType.None);
					}
				}
				return PSFormat.None;
			}
		}
		return format;
	}

	/// <summary>
	/// Returns true if node is in a "string literal" between "" or in a text part of an $"interpolated string".
	/// </summary>
	/// <param name="node">Any node. If returns true, finally its kind is StringLiteralExpression or InterpolatedStringExpression.</param>
	/// <param name="position"></param>
	public static bool IsInString(ref SyntaxNode node, int position) {
		if (node == null) return false;
		var nk = node.Kind();
		//AOutput.Write(nk, position, node.Span, node.GetType(), node);
		switch (nk) {
			case SyntaxKind.StringLiteralExpression:
				//return true only if position is in the string value.
				//false if <= the first " or >= the last ".
				//true if position is at the end of span and the last " is missing (error CS1010).
				var span = node.Span;
				int i = position - span.Start;
				if (i <= 0 || (i == 1 && node.ToString().Starts('@'))) return false;
				i = position - span.End;
				if (i > 0 || (i == 0 && !_NoClosingQuote(node))) return false;
				return true;
			case SyntaxKind.InterpolatedStringExpression:
				int j = node.Span.End - position;
				if (j != 1 && !(j == 0 && _NoClosingQuote(node))) return false;
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

	//FUTURE: remove if unused
	/// <summary>
	/// Gets syntax node at position.
	/// If document==null, calls CodeInfo.GetDocument().
	/// </summary>
	public static SyntaxNode NodeAt(int position, Document document = null) {
		if (document == null) {
			if (!CodeInfo.GetContextAndDocument(out var cd, position)) return null; //returns false if position is in meta comments
			document = cd.document;
			position = cd.pos16;
		}
		var root = document.GetSyntaxRootAsync().Result;
		return root.FindToken(position).Parent;
	}

	public static string GetTextWithoutUnusedUsingDirectives() {
		if (!CodeInfo.GetContextAndDocument(out var cd, 0, metaToo: true)) return cd.code;
		var code = cd.code;
		var semo = cd.document.GetSemanticModelAsync().Result;
		var a = semo.GetDiagnostics(null)
			.Where(d => d.Severity == DiagnosticSeverity.Hidden && d.Code == 8019)
			.Select(d => d.Location.SourceSpan)
			.OrderBy(span => span.Start);
		if (!a.Any()) return code;
		var b = new StringBuilder();
		int i = 0;
		foreach (var span in a) {
			int start = span.Start;
			if (start > i && code[start - 1] == ' ') start--;
			if (start > i) b.Append(code, i, start - i);
			i = span.End;
			if (b.Length == 0 || b[^1] == '\n') {
				if (code.Eq(i, "\r\n")) i += 2;
				else if (code.Eq(i, ' ')) i++;
			}
		}
		b.Append(code, i, code.Length - i);
		return b.ToString();
	}

	public static string FormatSignatureXmlDoc(MethodDeclarationSyntax md, string code) {
		var b = new StringBuilder();
		foreach (var p in md.ParameterList.Parameters) {
			b.Append("\r\n/// <param name=\"").Append(p.Identifier.Text).Append("\"></param>");
		}
		//rejected. Rarely used. VS intellisense ignores it.
		//var rt = md.ReturnType;
		//if(!code.Eq(rt.Span.Start..rt.Span.End, "void")) b.Append("\r\n/// <returns></returns>");
		return b.ToString();
	}

#if DEBUG
	public static void PrintNode(SyntaxNode x, int pos = 0, bool printNode = true, bool printErrors = false) {
		if (x == null) { AOutput.Write("null"); return; }
		if (printNode) AOutput.Write($"<><c blue>{pos}, {x.Span}, k={x.Kind()}, t={x.GetType().Name},<> '<c green>{x}<>'");
		if (printErrors) foreach (var d in x.GetDiagnostics()) AOutput.Write(d.Code, d.Location.SourceSpan, d);
	}

	public static void PrintNode(SyntaxToken x, int pos = 0, bool printNode = true, bool printErrors = false) {
		if (printNode) AOutput.Write($"<><c blue>{pos}, {x.Span}, {x.Kind()},<> '<c green>{x}<>'");
		if (printErrors) foreach (var d in x.GetDiagnostics()) AOutput.Write(d.Code, d.Location.SourceSpan, d);
	}

	public static void PrintNode(SyntaxTrivia x, int pos = 0, bool printNode = true, bool printErrors = false) {
		if (printNode) AOutput.Write($"<><c blue>{pos}, {x.Span}, {x.Kind()},<> '<c green>{x}<>'");
		if (printErrors) foreach (var d in x.GetDiagnostics()) AOutput.Write(d.Code, d.Location.SourceSpan, d);
	}

	public static void HiliteRange(int start, int end) {
		var doc = Panels.Editor.ZActiveDoc;
		doc.InicatorsFind_(null);
		doc.InicatorsFind_(new List<Range> { start..end });
	}

	public static void HiliteRange(TextSpan span) => HiliteRange(span.Start, span.End);
#endif

	public static void TagsToKindAndAccess(ImmutableArray<string> tags, out CiItemKind kind, out CiItemAccess access) {
		kind = CiItemKind.None;
		access = default;
		if (tags.IsDefaultOrEmpty) return;
		kind = tags[0] switch
		{
			WellKnownTags.Class => CiItemKind.Class,
			WellKnownTags.Structure => CiItemKind.Structure,
			WellKnownTags.Enum => CiItemKind.Enum,
			WellKnownTags.Delegate => CiItemKind.Delegate,
			WellKnownTags.Interface => CiItemKind.Interface,
			WellKnownTags.Method => CiItemKind.Method,
			WellKnownTags.ExtensionMethod => CiItemKind.ExtensionMethod,
			WellKnownTags.Property => CiItemKind.Property,
			WellKnownTags.Event => CiItemKind.Event,
			WellKnownTags.Field => CiItemKind.Field,
			WellKnownTags.Local => CiItemKind.LocalVariable,
			WellKnownTags.Parameter => CiItemKind.LocalVariable,
			WellKnownTags.RangeVariable => CiItemKind.LocalVariable,
			WellKnownTags.Constant => CiItemKind.Constant,
			WellKnownTags.EnumMember => CiItemKind.EnumMember,
			WellKnownTags.Keyword => CiItemKind.Keyword,
			WellKnownTags.Namespace => CiItemKind.Namespace,
			WellKnownTags.Label => CiItemKind.Label,
			WellKnownTags.TypeParameter => CiItemKind.TypeParameter,
			WellKnownTags.Snippet => CiItemKind.Snippet,
			_ => CiItemKind.None
		};
		if (tags.Length > 1) {
			access = tags[1] switch
			{
				WellKnownTags.Private => CiItemAccess.Private,
				WellKnownTags.Protected => CiItemAccess.Protected,
				WellKnownTags.Internal => CiItemAccess.Internal,
				_ => default
			};
		}
	}

	public static string[] ItemKindNames { get; } = new string[] { "Class", "Structure", "Enum", "Delegate", "Interface", "Method", "ExtensionMethod", "Property", "Event", "Field", "LocalVariable", "Constant", "EnumMember", "Namespace", "Keyword", "Label", "Snippet", "TypeParameter" }; //must match enum CiItemKind
}

enum CiItemKind : sbyte { Class, Structure, Enum, Delegate, Interface, Method, ExtensionMethod, Property, Event, Field, LocalVariable, Constant, EnumMember, Namespace, Keyword, Label, Snippet, TypeParameter, None }

enum CiItemAccess : sbyte { Public, Private, Protected, Internal }

static class CiExt
{
	[Conditional("DEBUG")]
	public static void DebugPrint(this CompletionItem t, string color = "blue") {
		AOutput.Write($"<><c {color}>{t.DisplayText},    {string.Join("|", t.Tags)},    prefix={t.DisplayTextPrefix},    suffix={t.DisplayTextSuffix},    filter={t.FilterText},    sort={t.SortText},    inline={t.InlineDescription},    automation={t.AutomationText},    provider={t.ProviderName}<>");
		AOutput.Write(string.Join("\n", t.Properties));
	}

	[Conditional("DEBUG")]
	public static void DebugPrintIf(this CompletionItem t, bool condition, string color = "blue") {
		if (condition) DebugPrint(t, color);
	}

	public static string QualifiedName(this ISymbol t, bool onlyNamespace = false, bool noDirectName = false) {
		var g = t_qnStack ??= new Stack<string>();
		g.Clear();
		if (noDirectName) t = t.ContainingType ?? t.ContainingNamespace as ISymbol;
		if (!onlyNamespace) for (var k = t; k != null; k = k.ContainingType) g.Push(k.Name);
		for (var n = t.ContainingNamespace; n != null && !n.IsGlobalNamespace; n = n.ContainingNamespace) g.Push(n.Name);
		return string.Join(".", g);
	}
	[ThreadStatic] static Stack<string> t_qnStack;

	/// <summary>
	/// <c>position &gt; t.Start &amp;&amp; position &lt; t.End;</c>
	/// </summary>
	public static bool ContainsInside(this TextSpan t, int position) => position > t.Start && position < t.End;
}