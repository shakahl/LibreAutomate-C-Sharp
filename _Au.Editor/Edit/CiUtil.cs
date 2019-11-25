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
using System.Net;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using Au.Editor.Resources;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

static class CiUtil
{

	public static bool GetSymbolFromPos(out ISymbol sym, out CodeInfo.Context cd)
	{
		sym = null;
		var doc = Panels.Editor.ZActiveDoc; if(doc == null) { cd = default; return false; }
		if(!CodeInfo.GetContextAndDocument(out cd)) return false;
		int position = cd.pos16;

		//code like in Roslyn CommonQuickInfoProvider.GetQuickInfoAsync, but simplified
		var tree = cd.document.GetSyntaxTreeAsync().Result;
		var token = tree.GetTouchingTokenAsync(position, default, findInsideTrivia: true).Result;
		if(token == default) return false;
		if(!token.Span.IntersectsWith(position)) {
			if(token.Parent.IsKind(SyntaxKind.XmlCrefAttribute)) return false;
			token = token.GetPreviousToken();
			if(token == default || !token.Span.IntersectsWith(position)) return false;
		}

		var model = cd.document.GetSemanticModelAsync().Result;
		sym = model.GetSymbolInfo(token.Parent).Symbol;
		return sym != null && sym.Kind != SymbolKind.ErrorType;
	}

	public static void OpenSymbolFromPosHelp()
	{
		if(!GetSymbolFromPos(out var sym, out _)) return;
		var url = GetSymbolHelpUrl(sym); if(url == null) return;
		AExec.TryRun(url);
	}

	public static string GetSymbolHelpUrl(ISymbol sym)
	{
		//Print(sym.IsInSource(), sym.IsFromSource());
		string query;
		IModuleSymbol metadata = null;
		foreach(var loc in sym.Locations) {
			if((metadata = loc.MetadataModule) != null) break;
		}
		if(metadata != null) {
			query = sym.QualifiedName();
			if(metadata.Name == "Au.dll") return Au.Util.AHelp.AuHelpUrl(query);
			if(metadata.Name.Starts("Au.")) return null;
			string kind = (sym is INamedTypeSymbol ints) ? ints.TypeKind.ToString() : sym.Kind.ToString();
			query = query + " " + kind.Lower();
		} else if(sym.IsExtern) { //[DllImport]
			query = sym.Name + " function";
		} else if(sym is INamedTypeSymbol ints && ints.IsComImport) { //[ComImport]
			query = sym.Name + " " + ints.TypeKind.ToString().Lower();
		} else {
			return null;
		}

		return "http://www.google.com/search?q=" + query;
	}

	/// <summary>
	/// Gets rectangle of caret if it was at the specified UTF-16 position.
	/// If <i>pos16</i> less than 0, uses current caret position.
	/// </summary>
	public static Rectangle GetCaretRectFromPos(SciCode doc, int pos16 = -1)
	{
		if(pos16 < 0) pos16 = doc.Z.CurrentPos8; else pos16 = doc.Pos8(pos16);
		int x = doc.Call(Sci.SCI_POINTXFROMPOSITION, 0, pos16), y = doc.Call(Sci.SCI_POINTYFROMPOSITION, 0, pos16);
		return new Rectangle(x, y, 1, doc.Call(Sci.SCI_TEXTHEIGHT, doc.Z.LineFromPos(false, pos16)) + 4);
	}

	public static PSFormat GetParameterStringFormat(SyntaxNode node, SemanticModel semo, bool isString)
	{
		var kind = node.Kind();
		//Print(kind);
		SyntaxNode parent;
		if(isString || kind == SyntaxKind.StringLiteralExpression) parent = node.Parent;
		else if(kind == SyntaxKind.InterpolatedStringText) parent = node.Parent.Parent;
		else return PSFormat.None;

		PSFormat format = PSFormat.None;
		if(parent is ArgumentSyntax asy && parent.Parent is ArgumentListSyntax alis) {
			switch(alis.Parent) {
			case ObjectCreationExpressionSyntax oce:
				format = _GetFormat(oce);
				if(format == PSFormat.None) {
					switch(oce.Type.ToString()) { //fast if single word
					case "Regex":
					case "System.Text.RegularExpressions.Regex":
					case "RegexCompilationInfo":
					case "System.Text.RegularExpressions.RegexCompilationInfo":
						if((object)asy == alis.Arguments[0]) format = PSFormat.Regex;
						break;
					}
				}
				break;
			case InvocationExpressionSyntax ies:
				format = _GetFormat(ies);
				if(format == PSFormat.None) {
					switch(ies.Expression.ToString()) {
					case "Regex.IsMatch":
					case "Regex.Match":
					case "Regex.Matches":
					case "Regex.Replace":
					case "Regex.Split":
						var aa = alis.Arguments;
						if(aa.Count >= 2 && (object)asy == aa[1]) format = PSFormat.Regex;
						break;
					}
				}
				break;
			//default:
			//	CiUtil.PrintNode(alis.Parent);
			//	break;
			}

			PSFormat _GetFormat(ExpressionSyntax es)
			{
				var sym = semo.GetSymbolInfo(es).Symbol;
				if(sym is IMethodSymbol ims) {
					IParameterSymbol p = null;
					var pa = ims.Parameters;
					var nc = asy.NameColon;
					if(nc != null) {
						var name = nc.Name.Identifier.Text;
						foreach(var v in pa) if(v.Name == name) { p = v; break; }
					} else {
						int i; var aa = alis.Arguments;
						for(i = 0; i < aa.Count; i++) if((object)aa[i] == asy) break;
						if(i >= pa.Length && pa[^1].IsParams) i = pa.Length - 1;
						if(i < pa.Length) p = pa[i];
					}
					if(p != null) {
						var fa = p.GetAttributes().FirstOrDefault(o => o.AttributeClass.Name == "ParamStringAttribute");
						if(fa != null) return fa.GetConstructorArgument<PSFormat>(0, SpecialType.None);
					}
				}
				return PSFormat.None;
			}
		}
		return format;
	}

#if DEBUG
	public static void PrintNode(SyntaxNode x, int pos = 0, bool printNode = true, bool printErrors = false)
	{
		if(x == null) { Print("null"); return; }
		if(printNode) Print($"<><c blue>{pos}, {x.Span}, {x.Kind()}, {x.GetType().Name},<> '<c green>{x}<>'");
		if(printErrors) foreach(var d in x.GetDiagnostics()) Print(d.Code, d.Location.SourceSpan, d);
	}

	public static void PrintNode(SyntaxToken x, int pos = 0, bool printNode = true, bool printErrors = false)
	{
		if(printNode) Print($"<><c blue>{pos}, {x.Span}, {x.Kind()},<> '<c green>{x}<>'");
		if(printErrors) foreach(var d in x.GetDiagnostics()) Print(d.Code, d.Location.SourceSpan, d);
	}

	public static void PrintNode(SyntaxTrivia x, int pos = 0, bool printNode = true, bool printErrors = false)
	{
		if(printNode) Print($"<><c blue>{pos}, {x.Span}, {x.Kind()},<> '<c green>{x}<>'");
		if(printErrors) foreach(var d in x.GetDiagnostics()) Print(d.Code, d.Location.SourceSpan, d);
	}
#endif

	public static void TagsToKindAndAccess(ImmutableArray<string> tags, out CiItemKind kind, out CiItemAccess access)
	{
		kind = CiItemKind.None;
		access = default;
		if(tags.IsDefaultOrEmpty) return;
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
			WellKnownTags.Snippet => CiItemKind.Snippet,
			WellKnownTags.TypeParameter => CiItemKind.TypeParameter,
			_ => CiItemKind.None
		};
		if(tags.Length > 1) {
			access = tags[1] switch
			{
				WellKnownTags.Private => CiItemAccess.Private,
				WellKnownTags.Protected => CiItemAccess.Protected,
				WellKnownTags.Internal => CiItemAccess.Internal,
				_ => default
			};
		}
	}

	static Bitmap[] s_images;
	static string[] s_imageNames;
	const int c_nKinds = 18;

	static void _InitImages()
	{
		if(s_images == null) {
			s_images = new Bitmap[c_nKinds + 3];
			s_imageNames = new string[c_nKinds + 3] {
				nameof(Resources.ciClass),
				nameof(Resources.ciStructure),
				nameof(Resources.ciEnum),
				nameof(Resources.ciDelegate),
				nameof(Resources.ciInterface),
				nameof(Resources.ciMethod),
				nameof(Resources.ciExtensionMethod),
				nameof(Resources.ciProperty),
				nameof(Resources.ciEvent),
				nameof(Resources.ciField),
				nameof(Resources.ciLocalVariable),
				nameof(Resources.ciConstant),
				nameof(Resources.ciEnumMember),
				nameof(Resources.ciNamespace),
				nameof(Resources.ciKeyword),
				nameof(Resources.ciLabel),
				nameof(Resources.ciSnippet),
				nameof(Resources.ciTypeParameter),
				nameof(Resources.ciOverlayPrivate),
				nameof(Resources.ciOverlayProtected),
				nameof(Resources.ciOverlayInternal),
			};
		}
	}

	static Bitmap _ResImage(int i) => EdResources.GetImageNoCacheDpi(s_imageNames[i]);

	public static Bitmap GetKindImage(CiItemKind kind)
	{
		if(kind == CiItemKind.None) return null;
		_InitImages();
		return s_images[(int)kind] ??= _ResImage((int)kind);
	}

	public static Bitmap GetAccessImage(CiItemAccess access)
	{
		if(access == default) return null;
		_InitImages();
		int i = c_nKinds - 1 + (int)access;
		return s_images[i] ??= _ResImage(i);
	}

	public static string[] ItemKindNames { get; } = new string[] { "Class", "Structure", "Enum", "Delegate", "Interface", "Method", "ExtensionMethod", "Property", "Event", "Field", "LocalVariable", "Constant", "EnumMember", "Namespace", "Keyword", "Label", "Snippet", "TypeParameter" }; //must match enum CiItemKind
}

enum CiItemKind : sbyte { Class, Structure, Enum, Delegate, Interface, Method, ExtensionMethod, Property, Event, Field, LocalVariable, Constant, EnumMember, Namespace, Keyword, Label, Snippet, TypeParameter, None }

enum CiItemAccess : sbyte { Public, Private, Protected, Internal }
static class CiExt
{
	[Conditional("DEBUG")]
	public static void DebugPrint(this CompletionItem t, string color = "blue")
	{
		Print($"<><c {color}>{t.DisplayText},    {string.Join("|", t.Tags)},    prefix={t.DisplayTextPrefix},    suffix={t.DisplayTextSuffix},    filter={t.FilterText},    sort={t.SortText},    inline={t.InlineDescription},    automation={t.AutomationText},    provider={t.ProviderName}<>");
		Print(string.Join("\n", t.Properties));
	}

	[Conditional("DEBUG")]
	public static void DebugPrintIf(this CompletionItem t, bool condition, string color = "blue")
	{
		if(condition) DebugPrint(t, color);
	}

	public static string QualifiedName(this ISymbol t, bool onlyNamespace = false, bool noDirectName = false)
	{
		var g = s_qnStack ??= new Stack<string>();
		g.Clear();
		if(noDirectName) t = t.ContainingType ?? t.ContainingNamespace as ISymbol;
		if(!onlyNamespace) for(var k = t; k != null; k = k.ContainingType) g.Push(k.Name);
		for(var n = t.ContainingNamespace; n != null && !n.IsGlobalNamespace; n = n.ContainingNamespace) g.Push(n.Name);
		return string.Join(".", g);
	}
	[ThreadStatic] static Stack<string> s_qnStack;
}