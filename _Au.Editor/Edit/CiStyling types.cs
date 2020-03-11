//Code colors and folding.

//#if TRACE
//#define PRINT
//#endif

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

using Au;
using Au.Types;
using Au.Controls;
using static Au.Controls.Sci;

partial class CiStyling
{
	/// <summary>
	/// Scintilla style indices of token types.
	/// </summary>
	public enum EToken : byte
	{
		None,
		Comment,
		String,
		StringEscape,
		Number,
		Punctuation,
		Operator,
		Keyword,
		Namespace,
		Type,
		Function,
		LocalVar,
		Parameter,
		Field,
		Constant,
		EnumMember,
		Label,
		Preprocessor,
		Excluded,
		XmlDocText,
		XmlDocTag, //tags, CDATA, ///, etc

		countUserDefined,

		//STYLE_HIDDEN=31,
		//STYLE_DEFAULT=32,

		LineNumber = 33, //STYLE_LINENUMBER
	}

#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.Equals(object o)
	public struct TStyle
#pragma warning restore
	{
		public int color { get; set; }
		public bool bold { get; set; }

		public TStyle(int color, bool bold)
		{
			this.color = color;
			this.bold = bold;
		}

		public static implicit operator TStyle(int color) => new TStyle(color, false);
		public static implicit operator TStyle((int color, bool bold) v) => new TStyle(v.color, v.bold);

		public static bool operator ==(TStyle a, TStyle b) => a.color == b.color && a.bold == b.bold;
		public static bool operator !=(TStyle a, TStyle b) => !(a == b);
	}

	public class TStyles
	{
		public string FontName { get; set; }
		public int FontSize { get; set; }
		public int? BackgroundColor { get; set; }

		public TStyle Comment { get; set; }
		public TStyle String { get; set; }
		public TStyle StringEscape { get; set; }
		public TStyle Number { get; set; }
		public TStyle Punctuation { get; set; }
		public TStyle Operator { get; set; }
		public TStyle Keyword { get; set; }
		public TStyle Namespace { get; set; }
		public TStyle Type { get; set; }
		public TStyle Function { get; set; }
		public TStyle LocalVar { get; set; }
		public TStyle Parameter { get; set; }
		public TStyle Field { get; set; }
		public TStyle Constant { get; set; }
		public TStyle EnumMember { get; set; }
		public TStyle Label { get; set; }
		public TStyle Preprocessor { get; set; }
		public TStyle Excluded { get; set; }
		public TStyle XmlDocText { get; set; }
		public TStyle XmlDocTag { get; set; }

		public TStyle LineNumber { get; set; }

		public TStyles()
		{
			FontName = "Consolas";
			FontSize = 10;

			Comment = 0x408000; //green like in VS but towards yellow
			String = 0xA07040; //brown, more green
							   //0xc0c0c0; //good contrast with 0xA07040, but maybe not with white background
							   //0xc0e000; //light yellow-green. Too vivid.
			StringEscape = 0xB776FB; //pink-purple like in VS
			Number = 0x804000; //brown, more red
							   //Punctuation = 0; //black
			Operator = 0x0000ff; //blue like keyword
			Keyword = 0x0000ff; //blue like in VS
			Namespace = 0x808000; //dark yellow
			Type = 0x0080c0; //like in VS but more blue
			Function = (0, true);
			LocalVar = 0x204020; //dark green gray
			Parameter = 0x204020; //like variable
			Field = 0x204020; //like variable
			Constant = 0x204020; //like variable
			EnumMember = 0x204020; //like variable
			Label = 0xff00ff; //magenta
			Preprocessor = 0xff8000; //orange
			Excluded = 0x808080; //gray
			XmlDocText = 0x408000; //green like comment
			XmlDocTag = 0x808080; //gray

			LineNumber = 0x808080;
		}

		/// <summary>
		/// Gets colors, bold, but not font properties.
		/// </summary>
		public TStyles(AuScintilla sci)
		{
			BackgroundColor = ColorInt.SwapRB(sci.Call(SCI_STYLEGETBACK));

			TStyle _Get(EToken tok)
			{
				int color = ColorInt.SwapRB(sci.Call(SCI_STYLEGETFORE, (int)tok));
				bool bold = 0 != sci.Call(SCI_STYLEGETBOLD, (int)tok);
				return new TStyle(color, bold);
			}

			Comment = _Get(EToken.Comment);
			String = _Get(EToken.String);
			StringEscape = _Get(EToken.StringEscape);
			Number = _Get(EToken.Number);
			Punctuation = _Get(EToken.Punctuation);
			Operator = _Get(EToken.Operator);
			Keyword = _Get(EToken.Keyword);
			Namespace = _Get(EToken.Namespace);
			Type = _Get(EToken.Type);
			Function = _Get(EToken.Function);
			LocalVar = _Get(EToken.LocalVar);
			Parameter = _Get(EToken.Parameter);
			Field = _Get(EToken.Field);
			Constant = _Get(EToken.Constant);
			EnumMember = _Get(EToken.EnumMember);
			Label = _Get(EToken.Label);
			Preprocessor = _Get(EToken.Preprocessor);
			Excluded = _Get(EToken.Excluded);
			XmlDocText = _Get(EToken.XmlDocText);
			XmlDocTag = _Get(EToken.XmlDocTag);

			LineNumber = _Get(EToken.LineNumber);
		}

		public void ToScintilla(AuScintilla sci)
		{
			var z = sci.Z;

			z.StyleFont(STYLE_DEFAULT, FontName, FontSize);
			if(BackgroundColor != null) z.StyleBackColor(STYLE_DEFAULT, BackgroundColor.Value);
			z.StyleClearAll();

			void _Set(EToken tok, TStyle sty)
			{
				z.StyleForeColor((int)tok, sty.color);
				if(sty.bold) z.StyleBold((int)tok, true);
			}

			_Set(EToken.Comment, Comment);
			_Set(EToken.String, String);
			_Set(EToken.StringEscape, StringEscape);
			_Set(EToken.Number, Number);
			_Set(EToken.Punctuation, Punctuation);
			_Set(EToken.Operator, Operator);
			_Set(EToken.Keyword, Keyword);
			_Set(EToken.Namespace, Namespace);
			_Set(EToken.Type, Type);
			_Set(EToken.Function, Function);
			_Set(EToken.LocalVar, LocalVar);
			_Set(EToken.Parameter, Parameter);
			_Set(EToken.Field, Field);
			_Set(EToken.Constant, Constant);
			_Set(EToken.EnumMember, EnumMember);
			_Set(EToken.Label, Label);
			_Set(EToken.Preprocessor, Preprocessor);
			_Set(EToken.Excluded, Excluded);
			_Set(EToken.XmlDocText, XmlDocText);
			_Set(EToken.XmlDocTag, XmlDocTag);

			_Set((EToken)STYLE_LINENUMBER, LineNumber);
		}

		public TStyle GetStyle(EToken token)
		{
			switch(token) {
			case EToken.Comment: return Comment;
			case EToken.String: return String;
			case EToken.StringEscape: return StringEscape;
			case EToken.Number: return Number;
			case EToken.Punctuation: return Punctuation;
			case EToken.Operator: return Operator;
			case EToken.Keyword: return Keyword;
			case EToken.Namespace: return Namespace;
			case EToken.Type: return Type;
			case EToken.Function: return Function;
			case EToken.LocalVar: return LocalVar;
			case EToken.Parameter: return Parameter;
			case EToken.Field: return Field;
			case EToken.Constant: return Constant;
			case EToken.EnumMember: return EnumMember;
			case EToken.Label: return Label;
			case EToken.Preprocessor: return Preprocessor;
			case EToken.Excluded: return Excluded;
			case EToken.XmlDocText: return XmlDocText;
			case EToken.XmlDocTag: return XmlDocTag;
			case EToken.LineNumber: return LineNumber;
			}
			return default;
		}

		//public void SetStyle(EToken token, TStyle style)
		//{
		//	switch(token) {
		//	case EToken.Comment: Comment = style; break;
		//	case EToken.String: String = style; break;
		//	case EToken.StringEscape: StringEscape = style; break;
		//	case EToken.Number: Number = style; break;
		//	case EToken.Punctuation: Punctuation = style; break;
		//	case EToken.Operator: Operator = style; break;
		//	case EToken.Keyword: Keyword = style; break;
		//	case EToken.Namespace: Namespace = style; break;
		//	case EToken.Type: Type = style; break;
		//	case EToken.Function: Function = style; break;
		//	case EToken.LocalVar: LocalVar = style; break;
		//	case EToken.Parameter: Parameter = style; break;
		//	case EToken.Field: Field = style; break;
		//	case EToken.Constant: Constant = style; break;
		//	case EToken.EnumMember: EnumMember = style; break;
		//	case EToken.Label: Label = style; break;
		//	case EToken.Preprocessor: Preprocessor = style; break;
		//	case EToken.Excluded: Excluded = style; break;
		//	case EToken.XmlDocText: XmlDocText = style; break;
		//	case EToken.XmlDocTag: XmlDocTag = style; break;
		//	case EToken.LineNumber: LineNumber = style; break;
		//	}
		//}

		public unsafe bool Equals(TStyles s)
		{
			return FontName == s.FontName
				&& FontSize == s.FontSize
				&& BackgroundColor == s.BackgroundColor
				&& Comment == s.Comment
				&& String == s.String
				&& StringEscape == s.StringEscape
				&& Number == s.Number
				&& Punctuation == s.Punctuation
				&& Operator == s.Operator
				&& Keyword == s.Keyword
				&& Namespace == s.Namespace
				&& Type == s.Type
				&& Function == s.Function
				&& LocalVar == s.LocalVar
				&& Parameter == s.Parameter
				&& Field == s.Field
				&& Constant == s.Constant
				&& EnumMember == s.EnumMember
				&& Label == s.Label
				&& Preprocessor == s.Preprocessor
				&& Excluded == s.Excluded
				&& XmlDocText == s.XmlDocText
				&& XmlDocTag == s.XmlDocTag
				&& LineNumber == s.LineNumber;
		}
	}
}
