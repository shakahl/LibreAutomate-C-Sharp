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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;
using Au.Editor.Properties;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

static class CiUtil
{
	public const string TaggedPartsHtmlStyleSheet = @"
body { font: 0.8em ""Segoe UI""; margin: 2px; }
span.type { color: #088 }
span.keyword { color: #00f }
span.string { color: #a74 }
span.number { color: #a40 }
span.namespace { color: #444 }
code { background-color: #f0f0f0 }
h4 { margin-bottom: 0.2em }
";

	//public static void TaggedPartsToHtml(StringBuilder b, ImmutableArray<TaggedText> tags)
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
				//if(i == 0 && s == "(" && tags.Length > 4 && tags[1].Text == "extension") { i = 3; continue; } //remove prefix "(extension) "
				if(s.Length > 0 || s[0] == '<' || s[0] == '>' || s[0] == '&') s = System.Net.WebUtility.HtmlEncode(s); //eg < in X<T>
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
			case TextTags.Method:
			case TextTags.Operator:
			case TextTags.Parameter:
			case TextTags.Property:
			case TextTags.RangeVariable:
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
