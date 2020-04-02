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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Net;

using Au;
using Au.Types;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.DocumentationComments;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.SignatureHelp;

//SHOULDDO: test whether are displayed ref and readonly modifiers of types, functions and fields. Now functions can be readonly, which means they don't modify state.

static class CiHtml
{
	public const string s_CSS = @"
body { font: 10.5pt Calibri; margin: 0 4px 4px 4px; }
span.type { color: #088 }
span.keyword { color: #00f }
span.string { color: #a74 }
span.number { color: #a40 }
span.namespace { color: #777 }
span.comment { color: #080 }
span.dot { color: #ccc }
span.dotSelected { color: #c0f }
span.hilite { background-color: #fca }
p { margin: 0.5em 0 0.5em 0 }
p.parameter { background-color: #dec; margin-bottom: 0; }
div.selected, div.link { padding: 0 0 1px 3px; }
div.selected { background-color: #f8f0a0; }
div.link a { color: #000; text-decoration: none; }
code { background-color: #f0f0f0; font: 100% Consolas; }
hr { border: none; border-top: 1px solid #ccc; margin: 0.5em 0 0.5em 0; }
div.br2 { margin: 0.3em 0 0.3em 0; }

/* workarounds for HtmlRenderer's bugs and limitations */
div.dashline { border-top: 1px dashed #ccc; } /* cannot use div border-bottom because draws too high */
";
	//HtmlRenderer bug: text-decoration underline is randomly too high or low if using 'Segoe UI' font of good size.
	//	Workaround: body { font: 10.5pt Calibri; }. Or could use it only for links, and use body { font: 9.8pt 'Segoe UI'; }. But Calibri is better.
	//HtmlRenderer's default font is 'Segoe UI' 11pt. WebBrowser's - 12pt (16px).

	public static void TaggedPartsToHtml(StringBuilder b, IEnumerable<TaggedText> tags)
	{
		if(tags == null) return;
		int i = -1, iBr = -2;
		foreach(var v in tags) {
			i++;
			//AOutput.Write($"{v.Tag}, '{v.Text}', {v.Style}");
			//AOutput.Write($"{v.Tag}, '{v.Text}', {v.Style}, navHint='{v.NavigationHint}', navTarget='{v.NavigationTarget}'");
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
				b.Append(i == iBr + 1 ? "<div class='br2'></div>" : "<br>");
				iBr = i;
				continue;
			case TextTags.Punctuation:
			case TextTags.Method: //eg operator <
			case TextTags.Operator:
				if(s.Length > 0 && (s[0] == '<' || s[0] == '>' || s[0] == '&')) s = WebUtility.HtmlEncode(s); //eg < in X<T>
				break;
			case TextTags.Text:
				s = WebUtility.HtmlEncode(s);
				//SHOULDDO: parse [][xref:topic_id]
				break;
#if DEBUG
			case TextTags.Space:
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
			case TextTags.Label:
			case TextTags.ContainerStart:
			case TextTags.ContainerEnd:
			case TextTags.ErrorType:
				break;
			default:
				ADebug.Print($"{v.Tag}, '{v.Text}', {v.Style}");
				break;
#endif
			}

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

	public static void TaggedPartsToHtml(StringBuilder b, ImmutableArray<SymbolDisplayPart> parts)
		=> TaggedPartsToHtml(b, parts.ToTaggedText());

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
		ISymbol sym = null;
		int i = -1;
		foreach(var v in symbols) {
			if(++i == iSelect) sym = v;
			using var li = new HtmlListItem(b, i == iSelect);
			if(i != iSelect) b.AppendFormat("<a href='^{0}'>", i);
			var parts = v.ToDisplayParts(s_symbolFullFormat);
			TaggedPartsToHtml(b, parts);
			if(v.Kind == SymbolKind.Parameter) _AppendComment("parameter");
			if(i != iSelect) b.Append("</a>");
		}
		//APerf.Next();

		if(sym != null) {
			//AOutput.Write($"<><c blue>{sym}<>");
			//AOutput.Write(sym.GetType().GetInterfaces());

			var parts = GetSymbolDescription(sym, model, position);
			//AOutput.Write(parts);
			if(parts.Any()) {
				b.Append("<p>");
				TaggedPartsToHtml(b, parts);
				b.Append("</p>");
			}

			//AOutput.Write(sym.GetDocumentationCommentXml());

			ISymbol enclosing = null;
			switch(sym.Kind) {
			case SymbolKind.NamedType:
			case SymbolKind.Event:
			case SymbolKind.Field:
			case SymbolKind.Method:
			case SymbolKind.Property:
				b.Append("<hr>");

				//HELP AND SOURCE LINKS
				SymbolLinksToHtml(b, sym, "<div>", "</div>");

				//CONTAINER namespace/type and assembly
				b.Append("<div>In ");
				var ctn = sym.ContainingType ?? sym.ContainingNamespace as INamespaceOrTypeSymbol;
				if(!(ctn is INamespaceSymbol ins && ins.IsGlobalNamespace)) {
					b.Append((ctn is ITypeSymbol its) ? its.TypeKind.ToString().Lower() : "namespace").Append(' ');
					TaggedPartsToHtml(b, ctn.ToDisplayParts(s_qualifiedTypeFormat));
					b.Append(", ");
				}
				b.Append(sym.IsFromSource() ? "file " : "assembly ").Append(sym.ContainingAssembly.Name);
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
					_AppendSpaceKeyword(sym.DeclaredAccessibility switch
					{
						Microsoft.CodeAnalysis.Accessibility.Public => "public",
						Microsoft.CodeAnalysis.Accessibility.Private => "private",
						Microsoft.CodeAnalysis.Accessibility.Internal => "internal",
						Microsoft.CodeAnalysis.Accessibility.Protected => "protected",
						Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => "private protected",
						Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal => "protected internal",
						_ => ""
					});

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
							case MethodKind.Destructor:
								continue;
								//case MethodKind.Destructor: goto g1; //don't check is accessible etc
							}
							if(m.IsImplicitlyDeclared) continue; //eg default ctor of struct
																 //AOutput.Write(m.MethodKind, m);
							break;
						case IPropertySymbol p when p.IsIndexer:
							break;
						default: continue;
						}
						if(!_IsAccessible(v)) continue;
						if(v.GetAttributes().Any(o => o.AttributeClass.Name == "ObsoleteAttribute")) continue;
						//g1:
						(a ??= new List<ISymbol>()).Add(v);
					}
					if(a != null) {
						b.Append("<hr>");
						//string header = tKind switch { TypeKind.Class => "Constructors, finalizers, operators, indexers", TypeKind.Struct => "Constructors, operators, indexers", _ => "Indexers" };
						string header = tKind == TypeKind.Interface ? "Indexers" : "Constructors, operators, indexers";
						b.Append("<b>").Append(header).Append("</b>");
						a.Sort((v1, v2) => {
							var diff = v1.Kind - v2.Kind; //indexer is property, others are methods
							if(diff != 0) return diff;
							if(v1 is IMethodSymbol m1 && v2 is IMethodSymbol m2) return _Sort(m1) - _Sort(m2);
							static int _Sort(IMethodSymbol sy) => sy.MethodKind switch { MethodKind.StaticConstructor => 0, MethodKind.Constructor => 1, MethodKind.Destructor => 2, MethodKind.Conversion => 3, MethodKind.UserDefinedOperator => 4, _ => 0 };
							return 0;
						});
						foreach(var v in a) {
							b.Append("<div><span class='dot'>•</span> ");
							if(v is IMethodSymbol m && m.MethodKind == MethodKind.StaticConstructor) { _AppendKeyword("static"); b.Append(' '); }
							TaggedPartsToHtml(b, v.ToDisplayParts(s_symbolFullFormat));
							b.Append("</div>");
						}
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
				if(t.IsGenericType) TaggedPartsToHtml(b, t.ToDisplayParts(s_unqualifiedTypeFormat));
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
					string aname = ac.Name.RemoveSuffix("Attribute");
					if(aname == "CompilerGenerated") continue;
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
							b.Append(v.ToCSharpString()); //never mind: enum with namespace
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
		void _AppendComment(string s) => b.Append(" &nbsp; <span class='comment'>//").Append(s).Append("</span>");

		b.Append("</body>");
		return b.ToString();
	}

	public static IEnumerable<TaggedText> GetSymbolDescription(ISymbol sym, SemanticModel model, int position)
	{
		return sym.GetDocumentationParts(model, position, _Formatter, default);
	}

	public static IEnumerable<TaggedText> GetTaggedTextForXml(string xml, SemanticModel model, int position)
	{
		if(xml == null) return Enumerable.Empty<TaggedText>();
		return _Formatter.Format(xml, model, position, ISymbolExtensions2.CrefFormat);
	}

	static IDocumentationCommentFormattingService _Formatter => s_formatter ??= CodeInfo.CurrentWorkspace.Services.GetLanguageServices("C#").GetService<IDocumentationCommentFormattingService>();
	static IDocumentationCommentFormattingService s_formatter;

	const SymbolDisplayMiscellaneousOptions s_miscDisplayOptions =
		SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral
		| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
		//| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier //?
		//| SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier //!
		| SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix
		| SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName
		| SymbolDisplayMiscellaneousOptions.UseSpecialTypes;
	const SymbolDisplayParameterOptions s_parameterDisplayOptions =
		SymbolDisplayParameterOptions.IncludeType
		| SymbolDisplayParameterOptions.IncludeName
		| SymbolDisplayParameterOptions.IncludeParamsRefOut
		| SymbolDisplayParameterOptions.IncludeOptionalBrackets
		| SymbolDisplayParameterOptions.IncludeDefaultValue
		| SymbolDisplayParameterOptions.IncludeExtensionThis;

	static SymbolDisplayFormat s_symbolFullFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
				SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
				SymbolDisplayDelegateStyle.NameAndSignature,
				SymbolDisplayExtensionMethodStyle.InstanceMethod,
				s_parameterDisplayOptions,
				SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
				SymbolDisplayLocalOptions.IncludeType | SymbolDisplayLocalOptions.IncludeRef | SymbolDisplayLocalOptions.IncludeConstantValue,
				SymbolDisplayKindOptions.IncludeMemberKeyword | SymbolDisplayKindOptions.IncludeNamespaceKeyword | SymbolDisplayKindOptions.IncludeTypeKeyword,
				s_miscDisplayOptions
				);
	static SymbolDisplayFormat s_qualifiedTypeFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
				miscellaneousOptions: s_miscDisplayOptions
				);
	static SymbolDisplayFormat s_unqualifiedTypeFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.Omitted,
				SymbolDisplayTypeQualificationStyle.NameOnly,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
				miscellaneousOptions: s_miscDisplayOptions
				);
	static SymbolDisplayFormat s_parameterFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.Omitted,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
				genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
				parameterOptions: s_parameterDisplayOptions,
				miscellaneousOptions: s_miscDisplayOptions
		);
	static SymbolDisplayFormat s_symbolWithoutParametersFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.Omitted,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
				SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
				miscellaneousOptions: s_miscDisplayOptions
				);
	static SymbolDisplayFormat s_nameWithoutTypeParametersFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.Omitted,
				SymbolDisplayTypeQualificationStyle.NameOnly,
				miscellaneousOptions: s_miscDisplayOptions
				);

	public static void SymbolWithoutParametersToHtml(StringBuilder b, ISymbol sym)
	{
		TaggedPartsToHtml(b, sym.ToDisplayParts(sym is INamedTypeSymbol ? s_nameWithoutTypeParametersFormat : s_symbolWithoutParametersFormat));
	}

	public static void ParametersToHtml(StringBuilder b, ISymbol sym, int iSel = -1, SignatureHelpItem sh = null)
	{
		ImmutableArray<IParameterSymbol> ap;
		switch(sym) {
		case IMethodSymbol sy:
			ap = sy.Parameters;
			break;
		case IPropertySymbol sy: //indexer
			ap = sy.Parameters;
			break;
		case INamedTypeSymbol sy:
			if(sy.IsTupleType && !sy.IsDefinition) {
				var te = sy.TupleElements;
				for(int i = 0; i < te.Length; i++) {
					_Param(te[i], i, isField: true);
				}
			} else if(sy.IsGenericType) {
				var tp = sy.TypeParameters;
				for(int i = 0; i < tp.Length; i++) {
					_Param(sy.TypeParameters[i], i);
				}
			} else {
				ADebug.Print(sy);
			}
			return;
		default:
			ADebug.Print(sym);
			return;
		}

		for(int i = 0; i < ap.Length; i++) _Param(ap[i], i);
		if(sh != null) { //eg properties in [Attribute(params, properties)]. Can be used for params too, but formats not as I like.
			var ap2 = sh.Parameters;
			for(int i = ap.Length; i < ap2.Length; i++) {
				if(i > 0) b.Append(", ");
				if(i == iSel) b.Append("<b>");
				TaggedPartsToHtml(b, ap2[i].DisplayParts);
				if(i == iSel) b.Append("</b>");
			}
		}

		void _Param(ISymbol p, int i, bool isField = false)
		{
			if(i > 0) b.Append(", ");
			if(i == iSel) b.Append("<b>");
			TaggedPartsToHtml(b, p.ToDisplayParts(isField ? s_symbolWithoutParametersFormat : s_parameterFormat));
			//info: s_symbolWithoutParametersFormat here maybe is not the best, but it worked well with all tested tuple parameter types.
			if(i == iSel) b.Append("</b>");
		}
	}

	//public static void ParametersToHtml(StringBuilder b, SignatureHelpItem sh, int iSel = -1)
	//{
	//	var ap = sh.Parameters;
	//	for(int i = 0; i < ap.Length; i++) {
	//		if(i > 0) b.Append(", ");
	//		if(i == iSel) b.Append("<b>");
	//		TaggedPartsToHtml(b, ap[i].DisplayParts);
	//		if(i == iSel) b.Append("</b>");
	//	}
	//}

	public static bool SymbolLinksToHtml(StringBuilder b, ISymbol sym, string prefix, string suffix)
	{
		string helpUrl = CiUtil.GetSymbolHelpUrl(sym);
		string sourceUrl = CiGoTo.GetLinkData(sym);
		if(helpUrl == null && sourceUrl == null) return false;
		SymbolLinksToHtml(b, helpUrl, sourceUrl, prefix, suffix);
		return true;
	}

	public static void SymbolLinksToHtml(StringBuilder b, string helpUrl, string sourceUrl, string prefix, string suffix)
	{
		b.Append(prefix).Append(helpUrl != null && sourceUrl != null ? "Links: " : "Link: ");
		if(helpUrl != null) b.AppendFormat("<a href='{0}'>more info</a>", helpUrl);
		if(sourceUrl != null) b.AppendFormat("{0}<a href='{1}'>source code</a>", helpUrl != null ? " , " : null, sourceUrl);
		b.Append(suffix);
	}

	public static string KeywordToHtml(string name)
	{
		//var url = "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/";
		var url = "https://www.google.com/search?q=C%23+keyword";
		return $@"<body>
<div>Keyword <span class='keyword'>{name}</span></div>
<hr>
<p>Links: <a href='{url} {name}'>more info</a> , <a href='{url}s'>C# keywords</a>.</p>
</body>";
	}

	public static string LabelToHtml(string name)
	{
		return $@"<body><div>Label <b>{name}</b></div></body>";
	}

	public struct HtmlListItem : IDisposable
	{
		StringBuilder _b;

		public HtmlListItem(StringBuilder b, bool selected)
		{
			_b = b;
			b.AppendFormat("<div class='{0}'><span class='dot{1}'>●</span> ", selected ? "selected" : "link", selected ? "Selected" : "");
		}

		public void Dispose()
		{
			_b.Append("</div><div class='dashline'></div>");
		}
	}
}
