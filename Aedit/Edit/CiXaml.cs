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
using System.Xml.Linq;

using Au;
using Au.Types;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.DocumentationComments;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.SignatureHelp;
using System.Windows.Documents;
using System.Windows.Markup;
using Au.Util;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;

//TODO: instead of formatting/parsing XAML, append objects directly. Now errors sometimes. Eg even type name can contain <> and need to escape.
//SHOULDDO: test whether are displayed ref and readonly modifiers of types, functions and fields. Now functions can be readonly, which means they don't modify state.

class CiXaml
{
	readonly StringBuilder _b;
	public StringBuilder SB => _b;

	public CiXaml() {
		_b = new StringBuilder("<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'"
			+ " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'"
			+ " xml:space=\"preserve\">");
	}

	public string End() {
		_b.Append("\n</Section>");
		return _b.ToString();
	}

	public void Append(string text, bool escape = false) {
		if (escape) text = Escape(text);
		_b.Append(text);
	}

	public static string Escape(string text) => new XElement("a", text).ToString()[3..^4];

	//public void Append(char c) => _b.Append(c);

	//public void AppendFormat(string text, params object[] a) => _b.AppendFormat(text, a);

	public void Style(string style, bool gt) {
		_b.Append(" Style='{StaticResource ").Append(style).Append("}'");
		if (gt) _b.Append('>');
	}

	public void StartParagraph(string style = null, bool gt = true) {
		_b.Append("\n<Paragraph");
		if (style != null) Style(style, false);
		if (gt) _b.Append('>');
	}

	public void EndParagraph() => _b.Append("</Paragraph>");

	/// <summary>
	/// <c>StartParagraph("div");</c>
	/// </summary>
	public void StartDiv() => StartParagraph("div");

	public void EndDiv() => _b.Append("</Paragraph>");

	public void Header(string text, bool escape = false) {
		StartParagraph("header");
		Append(text, escape);
		EndParagraph();
	}

	public void Separator() {
		_b.Append("\n<BlockUIContainer><Separator Margin='4'></Separator></BlockUIContainer>");
	}

	public void LineBreak(string append = null, bool escape = false) {
		int i = LineBreakIfLonger;
		if (!(i > 0 && _b.Length <= i)) _b.Append('\n');
		if (append != null) Append(append, escape);
	}

	/// <summary>
	/// Disables <see cref="LineBreak"/> if text length is not more than this value.
	/// </summary>
	public int LineBreakIfLonger { get; set; }

	public void StartOverload(bool selected, int index) {
		_b.Append("\n<Paragraph");
		Style(selected ? "overloadSelected" : "overload", true);
		if (!selected) { StartHyperlink($"^{index}", false); Style("divLink", true); }
		Run(selected ? "dotSelected" : "dot", "● ");
	}

	public void EndOverload(bool selected) {
		if (!selected) EndHyperlink();
		_b.Append("</Paragraph>");
	}

	/// <summary>
	/// Appends codeBlock paragraph with text.
	/// </summary>
	/// <param name="code">Non-escaped text.</param>
	public void CodeBlock(string code) {
		StartParagraph("codeBlock");
		Append(code, escape: true);
		EndParagraph();
	}

	/// <summary>
	/// Appends Span with a defined style (like HTML class) or custom attributes.
	/// </summary>
	/// <param name="style">A style defined in app resources, like "keyword". Or space and Style attributes, like " Foreground=\"Green\"".</param>
	/// <param name="gt">Append ">" at the end. Default true.</param>
	public void StartSpan(string style, bool gt = true) {
		_b.Append("<Span");
		if (style.Starts(' ')) _b.Append(style); else Style(style, false);
		if (gt) _b.Append('>');
	}

	public void EndSpan() => _b.Append("</Span>");

	//not used
	///// <summary>
	///// Calls <see cref="StartSpan"/>, <see cref="Append"/>, <see cref="EndSpan"/> and optionally <b>Append</b>.
	///// </summary>
	//public void Span(string style, string text, string append = null, bool escapeText = false, bool escapeAppend = false) {
	//	StartSpan(style);
	//	Append(text, escapeText);
	//	EndSpan();
	//	if (append != null) Append(append, escapeAppend);
	//}

	public void Run(string style, string text, string append = null, bool escapeText = false, bool escapeAppend = false) {
		_b.Append("<Run");
		Style(style, true);
		Append(text, escapeText);
		_b.Append("</Run>");
		if (append != null) Append(append, escapeAppend);
	}

	public void StartBold() => _b.Append("<Bold>");

	public void EndBold() => _b.Append("</Bold>");

	/// <param name="text">Escaped text.</param>
	public void Bold(string text) => _b.Append("<Bold>").Append(text).Append("</Bold>");

	public void StartItalic() => _b.Append("<Italic>");

	public void EndItalic() => _b.Append("</Italic>");

	//not used
	///// <param name="text">Escaped text.</param>
	//public void Italic(string text) {
	//	_b.Append("<Italic>").Append(text).Append("</Italic>");
	//}

	public void StartHyperlink(string uri, bool gt = true) {
		_b.Append("<Hyperlink NavigateUri=\"").Append(uri).Append('\"');
		if (gt) _b.Append('>');
	}

	public void EndHyperlink() => _b.Append("</Hyperlink>");

	public void Hyperlink(string uri, string text, string append = null, bool escapeText = false, bool escapeAppend = false) {
		StartHyperlink(uri);
		Append(text, escapeText);
		EndHyperlink();
		if (append != null) Append(append, escapeAppend);
	}

	/// <summary>
	/// Adds xaml or png etc image from app resources.
	/// </summary>
	/// <param name="source">Image resource, like "resources/a.xaml" or png etc.</param>
	public void Image(string source) {
		if (source.Starts('@')) {
			int i = source.ToInt(2);
			source = source[1] switch { 'k' => CiComplItem.ImageResource((CiItemKind)i), 'a' => CiComplItem.AccessImageResource((CiItemAccess)i), _ => null };
		}
		if (source.Ends(".xaml")) {
			//_b.Append("<InlineUIContainer BaselineAlignment=\"Center\" Tag=\"").Append(source).Append("\"");
			_b.Append("<InlineUIContainer BaselineAlignment=\"Center\"><ContentControl Content=\"{Binding Source=")
				.Append(source)
				.Append(", Converter={StaticResource imageConverter}}\"/></InlineUIContainer>");
			XamlImageResourceConverter.AddToAppResources();
		} else {
			_b.Append("<Image Source=\"").Append(source).Append("\" Stretch=\"None\"/>");
		}
	}

	//public override string ToString() => _b.ToString();

	//public Section Parse() => Parse(_b.ToString());

	public static Section Parse(string xaml) {
		//AOutput.Write(xaml);

		//var a = new List<(InlineUIContainer uc, UIElement e)>(); //need if used for images, because cannot modify collection while enumerating
#if DEBUG
		Section sec;
		try { sec = XamlReader.Parse(xaml) as Section; }
		catch { AOutput.Write(xaml); throw; }
#else
		var sec = XamlReader.Parse(xaml) as Section;
#endif

		foreach (var v in sec.LogicalDescendants()) {
			if (v is Hyperlink h) {
				h.RequestNavigate += (o, e) => FlowDocumentControl.OnLinkClicked_(o as Hyperlink, e);
			}
			//else if(v is InlineUIContainer u && u.Tag is string s) {
			//	var e = AResources.GetWpfImageElement(s);
			//	//u.Child = e; //no
			//	a.Add((u, e));
			//}
		}
		//foreach (var (u, e) in a) {
		//	u.Child = e;
		//}

		return sec;
	}

	public void AppendTaggedParts(IEnumerable<TaggedText> tags) {
		if (tags == null) return;
		//int i = -1, iBr = -2;
		foreach (var v in tags) {
			//i++;
			//AOutput.Write($"{v.Tag}, '{v.Text}', {v.Style}");
			//AOutput.Write($"{v.Tag}, '{v.Text}', {v.Style}, navHint='{v.NavigationHint}', navTarget='{v.NavigationTarget}'");
			string s = v.Text, c = null;
			switch (v.Tag) {
			case TextTags.Class:
			case TextTags.Struct:
			case TextTags.Enum:
			case TextTags.Interface:
			case TextTags.Delegate:
			case TextTags.TypeParameter:
			case TextTags.Record:
				c = "type";
				break;
			case TextTags.Keyword:
				c = "keyword";
				break;
			case TextTags.StringLiteral:
				c = "string";
				s = Escape(s);
				break;
			case TextTags.NumericLiteral:
				c = "number";
				break;
			case TextTags.Namespace:
				c = "namespace";
				break;
			case TextTags.LineBreak:
				//_b.Append(i == iBr + 1 ? "<div class='br2'></div>" : "<br>"); //this was for HtmlRenderer. Now it seems don't need.
				_b.Append('\n');
				//iBr = i;
				continue;
			case TextTags.Punctuation:
			case TextTags.Method: //eg operator <
			case TextTags.Operator:
				if (s.Length > 0 && (s[0] == '<' || s[0] == '>' || s[0] == '&')) s = Escape(s); //eg < in X<T>
				break;
			case TextTags.Text:
				s = Escape(s);
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

			switch (v.Style) {
			case TaggedTextStyle.Strong: StartBold(); break;
			case TaggedTextStyle.Emphasis: StartItalic(); break;
			case TaggedTextStyle.Code: StartSpan("codeSpan"); break;
			}

			if (c != null) Run(c, s); else _b.Append(s);

			switch (v.Style) {
			case TaggedTextStyle.Code: EndSpan(); break;
			case TaggedTextStyle.Emphasis: EndItalic(); break;
			case TaggedTextStyle.Strong: EndBold(); break;
			}
		}
	}

	public void AppendTaggedParts(ImmutableArray<SymbolDisplayPart> parts)
		=> AppendTaggedParts(parts.ToTaggedText());

	public static string FromTaggedParts(IEnumerable<TaggedText> tags) {
		var x = new CiXaml();
		x.StartParagraph();
		x.AppendTaggedParts(tags);
		x.EndParagraph();
		return x.End();
	}

	public static string FromSymbols(IEnumerable<ISymbol> symbols, int iSelect, SemanticModel model, int position) {
		//APerf.First();
		var x = new CiXaml();
		x.AppendSymbols(symbols, iSelect, model, position);
		return x.End();
	}

	public void AppendSymbols(IEnumerable<ISymbol> symbols, int iSelect, SemanticModel model, int position) {
		//APerf.First();
		ISymbol sym = null;
		int i = -1;
		foreach (var v in symbols) {
			if (++i == iSelect) sym = v;
			StartOverload(i == iSelect, i);
			var parts = v.ToDisplayParts(s_symbolFullFormat);
			AppendTaggedParts(parts);
			if (v.Kind == SymbolKind.Parameter) _AppendComment("parameter");
			EndOverload(i == iSelect);
		}
		//APerf.Next();

		if (sym != null) {
			//AOutput.Write($"<><c blue>{sym}<>");
			//AOutput.Write(sym.GetType().GetInterfaces());

			var parts = GetSymbolDescription(sym, model, position);
			//AOutput.Write(parts);
			if (parts.Any()) {
				StartParagraph();
				AppendTaggedParts(parts);
				EndParagraph();
			}

			//AOutput.Write(sym.GetDocumentationCommentXml());

			ISymbol enclosing = null;
			switch (sym.Kind) {
			case SymbolKind.NamedType:
			case SymbolKind.Event:
			case SymbolKind.Field:
			case SymbolKind.Method:
			case SymbolKind.Property:
				Separator();

				//HELP AND SOURCE LINKS
				StartDiv();
				AppendSymbolLinks(sym);
				EndDiv();

				//CONTAINER namespace/type and assembly
				StartDiv();
				_b.Append("In ");
				var ctn = sym.ContainingType ?? sym.ContainingNamespace as INamespaceOrTypeSymbol;
				if (!(ctn is INamespaceSymbol ins && ins.IsGlobalNamespace)) {
					_b.Append((ctn is ITypeSymbol its) ? its.TypeKind.ToString().Lower() : "namespace").Append(' ');
					AppendTaggedParts(ctn.ToDisplayParts(s_qualifiedTypeFormat));
					_b.Append(", ");
				}
				_b.Append(sym.IsFromSource() ? "file " : "assembly ").Append(sym.ContainingAssembly.Name);
				EndDiv();

				//MODIFIERS
				bool isReadonly = false, isConst = false, isVolatile = false, isAsync = false, isEnumMember = false;
				var tKind = TypeKind.Unknown; //0
				INamedTypeSymbol baseType = null; string enumBaseType = null; ImmutableArray<INamedTypeSymbol> interfaces = default;
				switch (sym) {
				case IFieldSymbol ifs:
					if (isEnumMember = ifs.IsEnumMember()) {
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
					switch (tKind) {
					case TypeKind.Class:
						var bt = ints.BaseType;
						if (bt != null && bt.BaseType != null) baseType = bt; //not object
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
				if (!isEnumMember) {
					StartDiv();
					int i1 = _b.Length;
					string s1 = sym.DeclaredAccessibility switch {
						Microsoft.CodeAnalysis.Accessibility.Public => "public",
						Microsoft.CodeAnalysis.Accessibility.Private => "private",
						Microsoft.CodeAnalysis.Accessibility.Internal => "internal",
						Microsoft.CodeAnalysis.Accessibility.Protected => "protected",
						Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => "private protected",
						Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal => "protected internal",
						_ => null
					};
					if (s1 != null) _AppendKeyword(s1);

					if (isConst) _AppendSpaceKeyword("const"); else if (sym.IsStatic) _AppendSpaceKeyword("static");
					if (sym.IsAbstract && tKind != TypeKind.Interface) _AppendSpaceKeyword("abstract");
					if (sym.IsSealed && (tKind == TypeKind.Class || tKind == TypeKind.Unknown)) _AppendSpaceKeyword("sealed");
					if (sym.IsVirtual) _AppendSpaceKeyword("virtual");
					if (sym.IsOverride) _AppendSpaceKeyword("override");
					if (sym.IsExtern) _AppendSpaceKeyword("extern");
					if (isReadonly) _AppendSpaceKeyword("readonly");
					if (isAsync) _AppendSpaceKeyword("async");
					if (isVolatile) _AppendSpaceKeyword("volatile");
					EndDiv();
					void _AppendSpaceKeyword(string name) {
						if (_b.Length > i1) _b.Append(' ');
						Run("keyword", name);
					}
				}

				//BASE TYPES AND IMPLEMENTED INTERFACES
				if (baseType != null || enumBaseType != null || !interfaces.IsDefaultOrEmpty) {
					StartDiv();
					_b.Append(": ");
					if (enumBaseType != null) {
						_AppendTypeName(enumBaseType, "keyword");
					} else if (baseType != null) {
						_AppendType(baseType);
						for (baseType = baseType.BaseType; baseType.BaseType != null; baseType = baseType.BaseType) {
							_b.Append(" : ");
							_AppendType(baseType);
						}
					}
					if (!interfaces.IsDefaultOrEmpty) {
						bool comma = baseType != null || enumBaseType != null;
						foreach (var v in interfaces) {
							_AppendComma(ref comma);
							bool hilite = v.Name == "IDisposable";
							if (hilite) StartSpan("hilite");
							_AppendType(v);
							if (hilite) EndSpan();
						}
					}
					EndDiv();
				}

				//ATTRIBUTES
				_AppendAttributes(sym.GetAttributes());
				switch (sym) {
				case IMethodSymbol ims:
					_AppendAttributes(ims.GetReturnTypeAttributes(), "return");
					foreach (var v in ims.Parameters) _AppendAttributes(v.GetAttributes(), v.Name, isParameter: true);
					break;
				case IPropertySymbol ips:
					var ipgs = ips.GetMethod;
					if (ipgs != null) {
						_AppendAttributes(ipgs.GetAttributes(), "get");
						_AppendAttributes(ipgs.GetReturnTypeAttributes(), "return");
					}
					var ipss = ips.SetMethod;
					if (ipss != null) _AppendAttributes(ipss.GetAttributes(), "set");
					var ipfs = ips.GetBackingFieldIfAny();
					if (ipfs != null) _AppendAttributes(ipfs.GetAttributes(), "field");
					break;
				}

				//TYPE MEMBERS NOT SHOWN IN THE COMPLETION LIST: constructors, finalizers, operators, indexers
				if ((tKind == TypeKind.Class || tKind == TypeKind.Struct || tKind == TypeKind.Interface) && sym is INamespaceOrTypeSymbol t) {
					List<ISymbol> a = null;
					foreach (var v in t.GetMembers()) {
						switch (v) {
						case IMethodSymbol m:
							switch (m.MethodKind) {
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
							if (m.IsImplicitlyDeclared) continue; //eg default ctor of struct
																  //AOutput.Write(m.MethodKind, m);
							break;
						case IPropertySymbol p when p.IsIndexer:
							break;
						default: continue;
						}
						if (!_IsAccessible(v)) continue;
						if (v.GetAttributes().Any(o => o.AttributeClass.Name == "ObsoleteAttribute")) continue;
						//g1:
						(a ??= new List<ISymbol>()).Add(v);
					}
					if (a != null) {
						Separator();
						//string header = tKind switch { TypeKind.Class => "Constructors, finalizers, operators, indexers", TypeKind.Struct => "Constructors, operators, indexers", _ => "Indexers" };
						string header = tKind == TypeKind.Interface ? "Indexers" : "Constructors, operators, indexers";
						Header(header);
						a.Sort((v1, v2) => {
							var diff = v1.Kind - v2.Kind; //indexer is property, others are methods
							if (diff != 0) return diff;
							if (v1 is IMethodSymbol m1 && v2 is IMethodSymbol m2) return _Sort(m1) - _Sort(m2);
							static int _Sort(IMethodSymbol sy) => sy.MethodKind switch { MethodKind.StaticConstructor => 0, MethodKind.Constructor => 1, MethodKind.Destructor => 2, MethodKind.Conversion => 3, MethodKind.UserDefinedOperator => 4, _ => 0 };
							return 0;
						});
						foreach (var v in a) {
							StartDiv();
							Run("dot", "• ");
							if (v is IMethodSymbol m && m.MethodKind == MethodKind.StaticConstructor) _AppendKeyword("static ");
							AppendTaggedParts(v.ToDisplayParts(s_symbolFullFormat));
							EndDiv();
						}
					}
					//never mind: also should add those of base types.
				}
				break;
			}
			//APerf.NW();

			bool _IsAccessible(ISymbol symbol) {
				enclosing ??= model.GetEnclosingNamedTypeOrAssembly(position, default);
				return enclosing != null && symbol.IsAccessibleWithin(enclosing);
			}
			void _AppendComma(ref bool isComma) { if (!isComma) isComma = true; else _b.Append(", "); }
			void _AppendKeyword(string name) { Run("keyword", name); }
			void _AppendTypeName(string name, string cssClass = "type") => Run(cssClass, name);
			void _AppendType(INamedTypeSymbol t) {
				if (t.IsGenericType) AppendTaggedParts(t.ToDisplayParts(s_unqualifiedTypeFormat));
				else _AppendTypeName(t.Name);
			}
			void _AppendAttributes(ImmutableArray<AttributeData> a, string target = null, bool isParameter = false) {
				if (a.IsDefaultOrEmpty) return;
				bool attrComma = false, paramAdded = false;
				foreach (var att in a) {
					var ac = att.AttributeClass; if (ac == null) continue;
					//if(!_IsAccessible(ac)) continue; //no, would not show attributes if code does not have 'using' for the attribute
					string aname = ac.Name.RemoveSuffix("Attribute");
					if (aname == "CompilerGenerated" || aname == "IteratorStateMachine") continue;
					//att.ToString(); //similar, but too much noise
					if (isParameter) {
						if (!paramAdded) {
							paramAdded = true;
							StartDiv();
							_b.Append(target).Append(": [");
						}
						_AppendComma(ref attrComma);
					} else {
						StartDiv();
						_b.Append('[');
						if (target != null) Run("keyword", target, ": ");
					}
					bool hilite = aname == "Obsolete";
					if (hilite) StartSpan("hilite");
					_AppendTypeName(aname);
					if (hilite) EndSpan();
					var ca = att.CommonConstructorArguments;
					var na = att.CommonNamedArguments;
					if (ca.Length + na.Length > 0) {
						_b.Append('(');
						bool paramComma = false;
						foreach (var v in ca) {
							_AppendComma(ref paramComma);
							Append(v.ToCSharpString(), escape: true); //never mind: enum with namespace
						}
						foreach (var v in na) {
							_AppendComma(ref paramComma);
							_b.Append(v.Key).Append(" = ");
							Append(v.Value.ToCSharpString(), escape: true);
						}
						_b.Append(')');
					}
					if (!isParameter) { _b.Append(']'); EndDiv(); }
				}
				if (isParameter && paramAdded) { _b.Append(']'); EndDiv(); }
			}
		}
		void _AppendComment(string s) {
			_b.Append("   ");
			Run("comment", "//" + s);
		}
	}

	public static IEnumerable<TaggedText> GetSymbolDescription(ISymbol sym, SemanticModel model, int position) {
		return sym.GetDocumentationParts(model, position, _Formatter, default);
	}

	//public static IEnumerable<TaggedText> GetTaggedTextForXml(string xml, SemanticModel model, int position)
	//{
	//	if(xml == null) return Enumerable.Empty<TaggedText>();
	//	return _Formatter.Format(xml, model, position, ISymbolExtensions2.CrefFormat); //error in new roslyn
	//}

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

	internal static readonly SymbolDisplayFormat s_symbolFullFormat = new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
				SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
				SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
				SymbolDisplayDelegateStyle.NameAndSignature,
				SymbolDisplayExtensionMethodStyle.Default,
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

	public void AppendSymbolWithoutParameters(ISymbol sym) {
		AppendTaggedParts(sym.ToDisplayParts(sym is INamedTypeSymbol ? s_nameWithoutTypeParametersFormat : s_symbolWithoutParametersFormat));
	}

	public void AppendParameters(ISymbol sym, int iSel = -1, SignatureHelpItem sh = null) {
		ImmutableArray<IParameterSymbol> ap;
		switch (sym) {
		case IMethodSymbol sy:
			ap = sy.Parameters;
			break;
		case IPropertySymbol sy: //indexer
			ap = sy.Parameters;
			break;
		case INamedTypeSymbol sy:
			if (sy.IsTupleType && !sy.IsDefinition) {
				var te = sy.TupleElements;
				for (int i = 0; i < te.Length; i++) {
					_Param(te[i], i, isField: true);
				}
			} else if (sy.IsGenericType) {
				var tp = sy.TypeParameters;
				for (int i = 0; i < tp.Length; i++) {
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

		for (int i = 0; i < ap.Length; i++) _Param(ap[i], i);
		if (sh != null) { //eg properties in [Attribute(params, properties)]. Can be used for params too, but formats not as I like.
			var ap2 = sh.Parameters;
			for (int i = ap.Length; i < ap2.Length; i++) {
				if (i > 0) _b.Append(", ");
				if (i == iSel) StartBold();
				AppendTaggedParts(ap2[i].DisplayParts);
				if (i == iSel) EndBold();
			}
		}

		void _Param(ISymbol p, int i, bool isField = false) {
			if (i > 0) _b.Append(", ");
			if (i == iSel) StartBold();
			AppendTaggedParts(p.ToDisplayParts(isField ? s_symbolWithoutParametersFormat : s_parameterFormat));
			//info: s_symbolWithoutParametersFormat here maybe is not the best, but it worked well with all tested tuple parameter types.
			if (i == iSel) EndBold();
		}
	}

	//public void AppendParameters(SignatureHelpItem sh, int iSel = -1)
	//{
	//	var ap = sh.Parameters;
	//	for(int i = 0; i < ap.Length; i++) {
	//		if(i > 0) _b.Append(", ");
	//		if(i == iSel) StartBold();
	//		AppendTaggedParts(ap[i].DisplayParts);
	//		if(i == iSel) EndBold();
	//	}
	//}

	public bool AppendSymbolLinks(ISymbol sym) {
		string helpUrl = CiUtil.GetSymbolHelpUrl(sym);
		string sourceUrl = CiGoTo.GetLinkData(sym);
		if (helpUrl == null && sourceUrl == null) return false;
		AppendSymbolLinks(helpUrl, sourceUrl);
		return true;
	}

	public void AppendSymbolLinks(string helpUrl, string sourceUrl) {
		_b.Append(helpUrl != null && sourceUrl != null ? "Links: " : "Link: ");
		if (helpUrl != null) Hyperlink(helpUrl, "more info");
		if (sourceUrl != null) {
			if (helpUrl != null) _b.Append(" , ");
			Hyperlink(sourceUrl, "source code");
		}
	}

	public static string FromKeyword(string name) {
		//var url = "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/";
		var url = "https://www.google.com/search?q=C%23+keyword";
		var x = new CiXaml();
		x.StartParagraph();
		x.Append("Keyword ");
		x.Run("keyword", name);
		x.EndParagraph();
		x.Separator();
		x.StartParagraph();
		x.Append("Links: ");
		x.Hyperlink($"{url} {name}", "more info", " , ");
		x.Hyperlink($"{url}s", "C# keywords", ".");
		x.EndParagraph();
		return x.End();
	}

	public static string FromLabel(string name) {
		var x = new CiXaml();
		x.StartParagraph();
		x.Append("Label "); x.Bold(name);
		x.EndParagraph();
		return x.End();
	}

	public static FlowDocumentControl CreateControl() {
		var d = new FlowDocument {
			//FontFamily = App.Wmain.FontFamily,
			//FontSize = App.Wmain.FontSize,
			FontFamily = new FontFamily("Calibri"),
			FontSize = App.Wmain.FontSize + 2,
			Background = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xf0)),
			PagePadding = default,
			TextAlignment = TextAlignment.Left,
		};
		var c = new FlowDocumentControl {
			Document = d,
			FocusVisualStyle = null,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			IsInactiveSelectionHighlightEnabled = true,
		};
		return c;
	}
}

class FlowDocumentControl : FlowDocumentScrollViewer
{
	public event Action<FlowDocumentControl, string> LinkClicked;

	internal static void OnLinkClicked_(Hyperlink h, RequestNavigateEventArgs e) {
		var s = e.Uri.OriginalString;
		//AOutput.Write(s);
		DependencyObject d = h;
		while (null != (d = LogicalTreeHelper.GetParent(d))) if (d is FlowDocumentControl c) {
				c.LinkClicked?.Invoke(c, s);
				break;
			}
	}
}

//[ValueConversion(typeof(string), typeof(UIElement))]
public class XamlImageResourceConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		return AResources.GetWpfImageElement(value as string);
		//CONSIDER: cache? Speed 1500 mcs. Not frequent. GC is frequent.
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
		return DependencyProperty.UnsetValue;
	}

	/// <summary>
	/// If not already done, creates XamlImageResourceConverter instance and adds to Application.Current.Resources with key "imageConverter".
	/// Why not to do it in XAML? Because then VS compiles project twice.
	/// </summary>
	public static void AddToAppResources() {
		if (s_added) return; s_added = true;
		Application.Current.Resources["imageConverter"] = new XamlImageResourceConverter();
	}
	static bool s_added;
}
