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
using Au.Editor.Properties;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;

static class CiUtil
{

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
	/// Returns a relative URL that can be passed to <see cref="OpenSymbolSourceUrl"/>.
	/// Returns null if symbol source is unavailable.
	/// May return non-null even if source unavailable. It happens until <see cref="OpenSymbolSourceUrl"/> called and downloaded all lists of available assemblies.
	/// This function is fast. Getting full URL would be slow, because may need to download files from source websites.
	/// Called eg when need to display a link to sym source code. No link if returns null. On link click called <see cref="OpenSymbolSourceUrl"/>.
	/// </summary>
	public static string GetSymbolSourceRelativeUrl(ISymbol sym)
	{
		if(sym.IsFromSource()) {
			var b = new StringBuilder("|");
			foreach(var loc in sym.Locations) {
				if(!loc.IsVisibleSourceLocation()) continue;
				var v = loc.GetLineSpan();
				if(b.Length > 1) b.Append('|');
				b.AppendFormat("{0}?{1},{2}", v.Path, v.StartLinePosition.Line + 1, v.StartLinePosition.Character + 1);
			}
			return b.ToString();
		} else {
			var assembly = sym.ContainingAssembly?.Name; if(assembly == null) return null;
			if(assembly == "Au" || assembly.Starts("Au.")) return null;
			if(s_sources.All(o => o.data != null) && _FindSourceSite(assembly, download: false) < 0) return null;

			//If wrong symbol, the site shows an error page.
			//Look how SourceBrowser (github) gets correct symbol and its hash. We don't use everything from there.
			//Methods:
			//	private HtmlElementInfo ProcessReference(Classification.Range range, ISymbol symbol, ReferenceKind kind, bool isLargeFile = false)
			//	private static string GetDocumentationCommentId(ISymbol symbol)
			//	private ISymbol GetSymbol(SyntaxNode node)
			if(sym is IMethodSymbol ims) sym = ims.ReducedFrom ?? sym; //extension method
			if(!sym.IsDefinition) sym = sym.OriginalDefinition; //generic
			string docId = sym.GetDocumentationCommentId().Replace("#ctor", "ctor");

			Au.Util.AHash.MD5 md5 = default;
			md5.Add(docId);
			docId = md5.Hash.ToString().Remove(16);

			return $"/{assembly}/a.html#{docId}";
		}
	}

	/// <summary>
	/// If symbol source is really available, gets full URL and opens in default web browser.
	/// Called eg when clicked a symbol source link. See <see cref="GetSymbolSourceRelativeUrl"/>.
	/// Runs async in a thread pool thread.
	/// </summary>
	public static void OpenSymbolSourceUrl(string relativeUrl)
	{
		Task.Run(() => {
			relativeUrl.RegexMatch(@"^/(.+?)/", 1, out string assembly);
			int i = _FindSourceSite(assembly, download: true);
			if(i >= 0) AExec.TryRun(s_sources[i].site + relativeUrl);
		});
	}

	static int _FindSourceSite(string assembly, bool download)
	{
		int R = -1;
		ARegex rx = null;
		for(int i = 0; i < s_sources.Length; i++) {
			if(download && s_sources[i].data == null) {
				try {
					using var client = new WebClient { CachePolicy = new System.Net.Cache.RequestCachePolicy() }; //get from cache if available and not too old
					s_sources[i].data = client.DownloadString(s_sources[i].site + "/assemblies.txt");
				}
				catch(WebException) { }
			}
			if(R >= 0) continue;
			rx ??= new ARegex($@"(?m)^{assembly};\d");
			if(rx.IsMatch(s_sources[i].data)) {
				R = i;
				if(!download) break;
			}
		}
		return R;
	}

	static readonly (string site, string data)[] s_sources = {
		("https://referencesource.microsoft.com", null), //framework
		("https://source.dot.net", null), //core
		("http://source.roslyn.io", null) //roslyn
	};

	public static void OpenSymbolSourceFile(string url, Control menuOwner)
	{
		if(url.RegexFindAll(@"\|(.+?)\?(\d+),(\d+)", out var a)) {
			if(a.Length == 1) {
				_GoTo(a[0]);
			} else {
				var m = new AMenu();
				foreach(var v in a) m[v[1].Value + ", line " + v[2].Value] = o => _GoTo(v);
				m.Show(menuOwner);
			}

			static void _GoTo(RXMatch v) => Program.Model.OpenAndGoTo(v[1].Value, v[2].Value, v[3].Value);
		}
	}

	public static Rectangle GetCaretRectFromPos(SciCode doc, int position = -1)
	{
		if(position < 0) position = doc.Z.CurrentPos8; else position = doc.Z.CountBytesFromChars(position);
		int x = doc.Call(Sci.SCI_POINTXFROMPOSITION, 0, position), y = doc.Call(Sci.SCI_POINTYFROMPOSITION, 0, position);
		return new Rectangle(x, y, 1, doc.Call(Sci.SCI_TEXTHEIGHT, doc.Z.LineIndexFromPos(false, position)) + 4);
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