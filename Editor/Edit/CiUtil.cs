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
//using System.Xml.Linq;
using System.Net;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using Au.Editor.Properties;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.DocumentationComments;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;

static class CiUtil
{
	public const string TaggedPartsHtmlStyleSheet = @"
body { font: 0.8em ""Segoe UI""; margin: 4px; }
span.type { color: #088 }
span.keyword { color: #00f }
span.string { color: #a74 }
span.number { color: #a40 }
span.namespace { color: #777 }
span.comment { color: #080 }
span.dotSelected { color: #6c0 }
span.dot { color: #ccc }
span.hilite { background-color: #ed0 }
code { background-color: #f0f0f0 }
h4 { margin: 0.2em 0 0.2em 0 }
ul { margin-left: 0; margin-top: 0; list-style-type: none; }
div.hr { border-top: 1px solid #ccc; margin: 0.5em 0 0.5em 0; }
li a { color: #000; text-decoration: none; }
";
	//hr { border-top: 1px solid #ccc; height: 0; box-sizing: content-box; } //does not work. Use div.hr instead.
	//body { background-gradient: #ccc; height: 100% } //does not work 'height: 100%'

	public static void TaggedPartsToHtml(StringBuilder b, IEnumerable<TaggedText> tags)
	{
		int i = -1, iBr = -2;
		foreach(var v in tags) {
			i++;
			//Print($"{v.Tag}, '{v.Text}', {v.Style}");
			//Print($"{v.Tag}, '{v.Text}', {v.Style}, navHint='{v.NavigationHint}', navTarget='{v.NavigationTarget}'");
			string s = v.Text, c = null;
			switch(v.Tag) {
			case TextTags.Class:
			case TextTags.Struct:
			case TextTags.Enum:
			case TextTags.Interface:
			case TextTags.Delegate:
			case TextTags.TypeParameter:
				c = "type";
				break;
			case TextTags.Keyword:
				c = "keyword";
				break;
			case TextTags.StringLiteral:
				c = "string";
				break;
			case TextTags.NumericLiteral:
				c = "number";
				break;
			case TextTags.Namespace:
				c = "namespace";
				break;
			case TextTags.LineBreak:
				b.Append("<br/>");
				if(iBr < 0) iBr = i;
				continue;
			case TextTags.Punctuation:
			case TextTags.Method: //eg operator <
			case TextTags.Operator:
				if(s.Length > 0 || s[0] == '<' || s[0] == '>' || s[0] == '&') s = WebUtility.HtmlEncode(s); //eg < in X<T>
				break;
#if DEBUG
			case TextTags.Space:
			case TextTags.Text:
			case TextTags.Constant:
			case TextTags.EnumMember:
			case TextTags.Event:
			case TextTags.ExtensionMethod:
			case TextTags.Field:
			case TextTags.Local:
			case TextTags.Parameter:
			case TextTags.Property:
			case TextTags.RangeVariable:
			case TextTags.Alias:
			case TextTags.ErrorType:
				break;
			default:
				ADebug.Print($"{v.Tag}, '{v.Text}', {v.Style}");
				break;
#endif
			}

			if(iBr == i - 1) b.Append("<br/>");

			switch(v.Style) {
			case TaggedTextStyle.Strong: b.Append("<b>"); break;
			case TaggedTextStyle.Emphasis: b.Append("<i>"); break;
			case TaggedTextStyle.Code: b.Append("<code>"); break;
			}

			if(c != null) b.AppendFormat("<span class='{0}'>{1}</span>", c, s); else b.Append(s);

			switch(v.Style) {
			case TaggedTextStyle.Code: b.Append("</code>"); break;
			case TaggedTextStyle.Emphasis: b.Append("</i>"); break;
			case TaggedTextStyle.Strong: b.Append("</b>"); break;
			}
		}
	}

	public static string TaggedPartsToHtml(IEnumerable<TaggedText> tags)
	{
		var b = new StringBuilder("<body>");
		TaggedPartsToHtml(b, tags);
		b.Append("</body>");
		return b.ToString();
	}

	public static string SymbolsToHtml(IEnumerable<ISymbol> symbols, int iSelect, SemanticModel model, int position)
	{
		//APerf.First();
		var b = new StringBuilder("<body>");
		b.Append("<ul>");
		ISymbol sym = null;
		int i = -1;
		foreach(var v in symbols) {
			if(++i == iSelect) sym = v;
			b.AppendFormat("<li><span class='dot{0}'>•</span> ", i == iSelect ? "Selected" : "");
			if(i != iSelect) b.AppendFormat("<a href='^{0}'>", i);
			var parts = v.ToDisplayParts(s_symbolFormat);
			TaggedPartsToHtml(b, parts.ToTaggedText());
			if(v.Kind == SymbolKind.Parameter) _AppendComment("parameter");
			if(i != iSelect) b.Append("</a>");
			b.Append("</li>");
		}
		b.Append("</ul>");
		//APerf.Next();

		if(sym != null) {
			//Print($"<><c blue>{sym}<>");
			//Print(sym.GetType().GetInterfaces());

			var parts = GetSymbolDescription(sym, model, position);
			//Print(parts);
			if(parts.Any()) {
				b.Append("<p>");
				TaggedPartsToHtml(b, parts);
				b.Append("</p>");
			}

			//Print(sym.GetDocumentationCommentXml());

			ISymbol enclosing = null;
			switch(sym.Kind) {
			case SymbolKind.NamedType:
			case SymbolKind.Event:
			case SymbolKind.Field:
			case SymbolKind.Method:
			case SymbolKind.Property:
				_AppendHr();

				//HELP AND SOURCE LINKS
				string helpUrl = GetSymbolHelpUrl(sym);
				string sourceUrl = GetSymbolSourceRelativeUrl(sym);
				if(helpUrl != null || sourceUrl != null) {
					b.Append("<div>");
					if(helpUrl != null) b.AppendFormat("<a href='{0}'>Help</a>", helpUrl);
					if(sourceUrl != null) b.AppendFormat("{0}<a href='{1}'>Source</a>", helpUrl != null ? ", " : null, sourceUrl);
					b.Append("</div>");
				}

				//CONTAINER namespace/type and assembly
				var ctn = sym.ContainingType ?? sym.ContainingNamespace as ISymbol;
				var ctParts = ctn.ToDisplayParts(s_qualifiedTypeFormat);
				b.Append("<div>");
				TaggedPartsToHtml(b, ctParts.ToTaggedText());
				if(!sym.IsFromSource()) b.Append(", assembly ").Append(sym.ContainingAssembly.Name);
				b.Append("</div>");

				//MODIFIERS
				bool isReadonly = false, isConst = false, isVolatile = false, isAsync = false, isEnumMember = false;
				var tKind = TypeKind.Unknown; //0
				INamedTypeSymbol baseType = null; string enumBaseType = null; ImmutableArray<INamedTypeSymbol> interfaces = default;
				switch(sym) {
				case IFieldSymbol ifs:
					if(isEnumMember = ifs.IsEnumMember()) {
						enumBaseType = ifs.ContainingType.EnumUnderlyingType.ToNameDisplayString();
					} else {
						isReadonly = ifs.IsReadOnly;
						isConst = ifs.IsConst;
						isVolatile = ifs.IsVolatile;
					}
					break;
				case INamedTypeSymbol ints:
					isReadonly = ints.IsReadOnly;
					tKind = ints.TypeKind;
					switch(tKind) {
					case TypeKind.Class:
						var bt = ints.BaseType;
						if(bt != null && bt.BaseType != null) baseType = bt; //not object
						goto case TypeKind.Interface;
					case TypeKind.Interface:
					case TypeKind.Struct:
						interfaces = ints.AllInterfaces;
						break;
					case TypeKind.Enum:
						enumBaseType = ints.EnumUnderlyingType.ToNameDisplayString();
						break;
					}
					break;
				case IMethodSymbol ims:
					isAsync = ims.IsAsync;
					break;
				}
				if(!isEnumMember) {
					b.Append("<div>");
					_AppendSpaceKeyword(sym.DeclaredAccessibility switch { Accessibility.Public => "public", Accessibility.Private => "private", Accessibility.Internal => "internal", Accessibility.Protected => "protected", Accessibility.ProtectedAndInternal => "private protected", Accessibility.ProtectedOrInternal => "protected internal", _ => "" });

					if(isConst) _AppendSpaceKeyword("const"); else if(sym.IsStatic) _AppendSpaceKeyword("static");
					if(sym.IsAbstract && tKind != TypeKind.Interface) _AppendSpaceKeyword("abstract");
					if(sym.IsSealed && (tKind == TypeKind.Class || tKind == TypeKind.Unknown)) _AppendSpaceKeyword("sealed");
					if(sym.IsVirtual) _AppendSpaceKeyword("virtual");
					if(sym.IsOverride) _AppendSpaceKeyword("override");
					if(sym.IsExtern) _AppendSpaceKeyword("extern");
					if(isReadonly) _AppendSpaceKeyword("readonly");
					if(isAsync) _AppendSpaceKeyword("async");
					if(isVolatile) _AppendSpaceKeyword("volatile");
					b.Append("</div>");
				}

				//BASE TYPES AND IMPLEMENTED INTERFACES
				if(baseType != null || enumBaseType != null || !interfaces.IsDefaultOrEmpty) {
					b.Append("<div>: ");
					if(enumBaseType != null) {
						_AppendTypeName(enumBaseType, "keyword");
					} else if(baseType != null) {
						_AppendType(baseType);
						for(baseType = baseType.BaseType; baseType.BaseType != null; baseType = baseType.BaseType) {
							b.Append(" : ");
							_AppendType(baseType);
						}
					}
					if(!interfaces.IsDefaultOrEmpty) {
						bool comma = baseType != null || enumBaseType != null;
						foreach(var v in interfaces) {
							_AppendComma(ref comma);
							bool hilite = v.Name == "IDisposable";
							if(hilite) b.Append("<span class='hilite'>");
							_AppendType(v);
							if(hilite) b.Append("</span>");
						}
					}
					b.Append("</div>");
				}

				//ATTRIBUTES
				_AppendAttributes(sym.GetAttributes());
				switch(sym) {
				case IMethodSymbol ims:
					_AppendAttributes(ims.GetReturnTypeAttributes(), "return");
					foreach(var v in ims.Parameters) _AppendAttributes(v.GetAttributes(), v.Name, isParameter: true);
					break;
				case IPropertySymbol ips:
					var ipgs = ips.GetMethod;
					if(ipgs != null) {
						_AppendAttributes(ipgs.GetAttributes(), "get");
						_AppendAttributes(ipgs.GetReturnTypeAttributes(), "return");
					}
					var ipss = ips.SetMethod;
					if(ipss != null) _AppendAttributes(ipss.GetAttributes(), "set");
					var ipfs = ips.GetBackingFieldIfAny();
					if(ipfs != null) _AppendAttributes(ipfs.GetAttributes(), "field");
					break;
				}

				//TYPE MEMBERS NOT SHOWN IN THE COMPLETION LIST: constructors, finalizers, operators, indexers
				if((tKind == TypeKind.Class || tKind == TypeKind.Struct || tKind == TypeKind.Interface) && sym is INamespaceOrTypeSymbol t) {
					List<ISymbol> a = null;
					foreach(var v in t.GetMembers()) {
						switch(v) {
						case IMethodSymbol m:
							switch(m.MethodKind) {
							case MethodKind.Ordinary:
							case MethodKind.PropertyGet:
							case MethodKind.PropertySet:
							case MethodKind.EventAdd:
							case MethodKind.EventRemove:
							case MethodKind.ExplicitInterfaceImplementation:
								continue;
							case MethodKind.Destructor: goto g1; //don't check is accessible etc
							}
							if(m.IsImplicitlyDeclared) continue; //eg default ctor of struct
							//Print(m.MethodKind, m);
							break;
						case IPropertySymbol p when p.IsIndexer:
							break;
						default: continue;
						}
						if(!_IsAccessible(v)) continue;
						if(v.GetAttributes().Any(o => o.AttributeClass.Name == "ObsoleteAttribute")) continue;
						g1:
						(a ??= new List<ISymbol>()).Add(v);
					}
					if(a != null) {
						_AppendHr();
						string header = tKind switch { TypeKind.Class => "Constructors, finalizers, operators, indexers", TypeKind.Struct => "Constructors, operators, indexers", _ => "Indexers" };
						b.Append("<h4>").Append(header).Append("</h4><ul>");
						a.Sort((v1, v2) => {
							var diff = v1.Kind - v2.Kind; //indexer is property, others are methods
							if(diff != 0) return diff;
							static int _Sort(ISymbol sy) => (sy as IMethodSymbol).MethodKind switch { MethodKind.StaticConstructor => 0, MethodKind.Constructor => 1, MethodKind.Destructor => 2, MethodKind.Conversion => 3, MethodKind.UserDefinedOperator => 4, _ => 0 };
							return _Sort(v1) - _Sort(v2);
						});
						foreach(var v in a) {
							b.Append("<li><span class='dot'>•</span> ");
							if(v is IMethodSymbol m && m.MethodKind == MethodKind.StaticConstructor) { _AppendKeyword("static"); b.Append(' '); }
							TaggedPartsToHtml(b, v.ToDisplayParts(s_symbolFormat).ToTaggedText());
							b.Append("</li>");
						}
						b.Append("</ul>");
					}
					//never mind: also should add those of base types.
				}
				break;
			}
			//APerf.NW();

			bool _IsAccessible(ISymbol symbol)
			{
				enclosing ??= model.GetEnclosingNamedTypeOrAssembly(position, default);
				return enclosing != null && symbol.IsAccessibleWithin(enclosing);
			}
			void _AppendComma(ref bool isComma) { if(!isComma) isComma = true; else b.Append(", "); }
			void _AppendKeyword(string name) => b.Append("<span class='keyword'>").Append(name).Append("</span>");
			void _AppendSpaceKeyword(string name) { b.Append(' '); _AppendKeyword(name); }
			void _AppendTypeName(string name, string cssClass = "type") => b.AppendFormat("<span class='{0}'>{1}</span>", cssClass, name);
			void _AppendType(INamedTypeSymbol t)
			{
				if(t.IsGenericType) TaggedPartsToHtml(b, t.ToDisplayParts(s_unqualifiedTypeFormat).ToTaggedText());
				else _AppendTypeName(t.Name);
			}
			void _AppendAttributes(ImmutableArray<AttributeData> a, string target = null, bool isParameter = false)
			{
				if(a.IsDefaultOrEmpty) return;
				if(isParameter) {
					b.Append("<div>").Append(target).Append(": [");
				}
				bool attrComma = false;
				foreach(var att in a) {
					var ac = att.AttributeClass; if(ac == null) continue;
					if(!_IsAccessible(ac)) continue;
					//att.ToString(); //similar, but too much noise
					if(isParameter) {
						_AppendComma(ref attrComma);
					} else {
						b.Append("<div>[");
						if(target != null) {
							_AppendKeyword(target);
							b.Append(": ");
						}
					}
					string aname = ac.Name.RemoveSuffix("Attribute");
					bool hilite = aname == "Obsolete";
					if(hilite) b.Append("<span class='hilite'>");
					_AppendTypeName(aname);
					if(hilite) b.Append("</span>");
					var ca = att.CommonConstructorArguments;
					var na = att.CommonNamedArguments;
					if(ca.Length + na.Length > 0) {
						b.Append('(');
						bool paramComma = false;
						foreach(var v in ca) {
							_AppendComma(ref paramComma);
							b.Append(v.ToCSharpString());
						}
						foreach(var v in na) {
							_AppendComma(ref paramComma);
							b.Append(v.Key).Append(" = ").Append(v.Value.ToCSharpString());
						}
						b.Append(')');
					}
					if(!isParameter) b.Append("]</div>");
				}
				if(isParameter) b.Append("]</div>");
			}
		}
		void _AppendHr() { b.Append("<div class='hr'/>"); } //HtmlRenderer does not support CSS for <hr/>
		void _AppendComment(string s) => b.Append(" &nbsp; <span class='comment'>//").Append(s).Append("</span>");

		b.Append("</body>");
		//Print(b);
		return b.ToString();
	}

	public static IEnumerable<TaggedText> GetSymbolDescription(ISymbol sym, SemanticModel model, int position)
	{
		s_formatter ??= CodeInfo.CurrentWorkspace.Services.GetLanguageServices("C#").GetService<IDocumentationCommentFormattingService>();
		if(sym.Kind == SymbolKind.Namespace) { //INamespaceSymbol does not give XML doc
			string xml = Au.Compiler.MetaReferences.GetNamespaceDocXml(sym.QualifiedName());
			if(xml == null) return Enumerable.Empty<TaggedText>();
			return s_formatter.Format(xml, model, position, ISymbolExtensions2.CrefFormat);
		} else {
			return sym.GetDocumentationParts(model, position, s_formatter, default);
		}
	}
	static IDocumentationCommentFormattingService s_formatter;

	static SymbolDisplayFormat s_symbolFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
				SymbolDisplayTypeQualificationStyle.NameOnly,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
				SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
				SymbolDisplayDelegateStyle.NameAndSignature,
				SymbolDisplayExtensionMethodStyle.InstanceMethod,
				SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut | SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeExtensionThis,
				SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
				SymbolDisplayLocalOptions.IncludeType | SymbolDisplayLocalOptions.IncludeRef | SymbolDisplayLocalOptions.IncludeConstantValue,
				SymbolDisplayKindOptions.IncludeMemberKeyword | SymbolDisplayKindOptions.IncludeNamespaceKeyword | SymbolDisplayKindOptions.IncludeTypeKeyword,
				SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				);
	static SymbolDisplayFormat s_qualifiedTypeFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance
				);
	static SymbolDisplayFormat s_unqualifiedTypeFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.Omitted,
				SymbolDisplayTypeQualificationStyle.NameOnly,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance
				);

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
		if(sym.IsFromSource()) return null;
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

	public static string KeywordToHtml(string name)
	{
		//var url = "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/";
		var url = "https://www.google.com/search?q=C%23+keyword";
		return $@"<body>
<ul><li>Keyword <span class='keyword'>{name}</span></li></ul>
<div class='hr'/>
<p><a href='{url} {name}'>Help</a>, <a href='{url}s'>C# keywords</a></p>
</body>";
	}

	public static Rectangle GetCaretRectFromPos(SciCode doc, int position = -1)
	{
		if(position < 0) position = doc.ST.CurrentPos;
		else position = doc.ST.CountBytesFromChars(position);
		int x = doc.Call(Sci.SCI_POINTXFROMPOSITION, 0, position), y = doc.Call(Sci.SCI_POINTYFROMPOSITION, 0, position);
		return new Rectangle(x, y - 2, 1, doc.Call(Sci.SCI_TEXTHEIGHT, doc.ST.LineIndexFromPos(position)) + 4);
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
		if(kind == CiItemKind.None) return default;
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