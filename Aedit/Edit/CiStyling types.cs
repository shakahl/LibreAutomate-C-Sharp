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
		Variable,
		Constant,
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
		public int color;
		public bool bold;

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
		public string FontName;
		public int FontSize;
		public int BackgroundColor;

		public TStyle None;
		public TStyle Comment;
		public TStyle String;
		public TStyle StringEscape;
		public TStyle Number;
		public TStyle Punctuation;
		public TStyle Operator;
		public TStyle Keyword;
		public TStyle Namespace;
		public TStyle Type;
		public TStyle Function;
		public TStyle Variable;
		public TStyle Constant;
		public TStyle Label;
		public TStyle Preprocessor;
		public TStyle Excluded;
		public TStyle XmlDocText;
		public TStyle XmlDocTag;

		public TStyle LineNumber;

		public static TStyles Settings {
			get => s_styles ??= new TStyles();
			set {
				s_styles = value;
				if(value != null) value._Save();
				else AFile.Delete(s_settingsFile);
			}
		}
		static TStyles s_styles;
		internal static readonly string s_settingsFile = AppSettings.DirBS + "Font.csv";

		public TStyles()
		{
			FontName = "Consolas";
			FontSize = 10;
			BackgroundColor = 0xffffff;

			//None = 0;
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
			Variable = 0x204020; //dark green gray
			Constant = 0x204020; //like variable
			Label = 0xff00ff; //magenta
			Preprocessor = 0xff8000; //orange
			Excluded = 0x808080; //gray
			XmlDocText = 0x408000; //green like comment
			XmlDocTag = 0x808080; //gray

			LineNumber = 0x808080;

			_Load();
		}

		void _Load()
		{
			ACsv csv;
			if(!AFile.ExistsAsFile(s_settingsFile)) return;
			try { csv = ACsv.Load(s_settingsFile); }
			catch(Exception e1) { AOutput.Write(e1.ToStringWithoutStack()); return; }
			if(csv.ColumnCount < 2) return;

			foreach(var a in csv.Data) {
				switch(a[0]) {
				case "Font":
					if(!a[1].NE()) FontName = a[1];
					if(a.Length > 2) { int fs = a[2].ToInt(); if(fs >= 5 && fs <= 100) FontSize = fs; }
					break;
				case "Background":
					if(!a[1].NE()) BackgroundColor = a[1].ToInt();
					break;
				case nameof(None): _Style(ref None, a); break;
				case nameof(Comment): _Style(ref Comment, a); break;
				case nameof(String): _Style(ref String, a); break;
				case nameof(StringEscape): _Style(ref StringEscape, a); break;
				case nameof(Number): _Style(ref Number, a); break;
				case nameof(Punctuation): _Style(ref Punctuation, a); break;
				case nameof(Operator): _Style(ref Operator, a); break;
				case nameof(Keyword): _Style(ref Keyword, a); break;
				case nameof(Namespace): _Style(ref Namespace, a); break;
				case nameof(Type): _Style(ref Type, a); break;
				case nameof(Function): _Style(ref Function, a); break;
				case nameof(Variable): _Style(ref Variable, a); break;
				case nameof(Constant): _Style(ref Constant, a); break;
				case nameof(Label): _Style(ref Label, a); break;
				case nameof(Preprocessor): _Style(ref Preprocessor, a); break;
				case nameof(Excluded): _Style(ref Excluded, a); break;
				case nameof(XmlDocText): _Style(ref XmlDocText, a); break;
				case nameof(XmlDocTag): _Style(ref XmlDocTag, a); break;
				case nameof(LineNumber): _Style(ref LineNumber, a); break;
				}
			}

			static void _Style(ref TStyle r, string[] a)
			{
				if(!a[1].NE()) r.color = a[1].ToInt();
				if(a.Length > 2 && !a[2].NE()) r.bold = 0 != (1 & a[2].ToInt());
			}
		}

		void _Save()
		{
			var b = new StringBuilder(); //don't need ACsv for such simple values
			b.AppendFormat("Font, {0}, {1}\r\n", FontName, FontSize);
			b.Append("Background, 0x").AppendLine(BackgroundColor.ToString("X6"));
			_Style(nameof(None), None);
			_Style(nameof(Comment), Comment);
			_Style(nameof(String), String);
			_Style(nameof(StringEscape), StringEscape);
			_Style(nameof(Number), Number);
			_Style(nameof(Punctuation), Punctuation);
			_Style(nameof(Operator), Operator);
			_Style(nameof(Keyword), Keyword);
			_Style(nameof(Namespace), Namespace);
			_Style(nameof(Type), Type);
			_Style(nameof(Function), Function);
			_Style(nameof(Variable), Variable);
			_Style(nameof(Constant), Constant);
			_Style(nameof(Label), Label);
			_Style(nameof(Preprocessor), Preprocessor);
			_Style(nameof(Excluded), Excluded);
			_Style(nameof(XmlDocText), XmlDocText);
			_Style(nameof(XmlDocTag), XmlDocTag);
			_Style(nameof(LineNumber), LineNumber);

			void _Style(string name, TStyle r)
			{
				b.Append(name).Append(", 0x").Append(r.color.ToString("X6"));
				if(r.bold) b.Append(", 1");
				b.AppendLine();
			}

			AFile.SaveText(s_settingsFile, b.ToString());
		}

		/// <summary>
		/// Gets colors, bold, but not font properties.
		/// </summary>
		public TStyles(SciHost sci)
		{
			BackgroundColor = ColorInt.SwapRB(sci.Call(SCI_STYLEGETBACK));

			TStyle _Get(EToken tok)
			{
				int color = ColorInt.SwapRB(sci.Call(SCI_STYLEGETFORE, (int)tok));
				bool bold = 0 != sci.Call(SCI_STYLEGETBOLD, (int)tok);
				return new TStyle(color, bold);
			}

			None = _Get(EToken.None);
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
			Variable = _Get(EToken.Variable);
			Constant = _Get(EToken.Constant);
			Label = _Get(EToken.Label);
			Preprocessor = _Get(EToken.Preprocessor);
			Excluded = _Get(EToken.Excluded);
			XmlDocText = _Get(EToken.XmlDocText);
			XmlDocTag = _Get(EToken.XmlDocTag);

			LineNumber = _Get(EToken.LineNumber);
		}

		public void ToScintilla(SciHost sci)
		{
			var z = sci.Z;

			z.StyleFont(STYLE_DEFAULT, FontName, FontSize);
			z.StyleBackColor(STYLE_DEFAULT, BackgroundColor);
			//if(None.color != 0) z.StyleForeColor(STYLE_DEFAULT, None.color); //also would need bold and in ctor above
			z.StyleClearAll();

			void _Set(EToken tok, TStyle sty)
			{
				z.StyleForeColor((int)tok, sty.color);
				if(sty.bold) z.StyleBold((int)tok, true);
			}

			_Set(EToken.None, None);
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
			_Set(EToken.Variable, Variable);
			_Set(EToken.Constant, Constant);
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
			case EToken.None: return None;
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
			case EToken.Variable: return Variable;
			case EToken.Constant: return Constant;
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
		//	case EToken.None: None = style; break;
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
		//	case EToken.Variable: Variable = style; break;
		//	case EToken.Constant: Constant = style; break;
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
				&& None == s.None
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
				&& Variable == s.Variable
				&& Constant == s.Constant
				&& Label == s.Label
				&& Preprocessor == s.Preprocessor
				&& Excluded == s.Excluded
				&& XmlDocText == s.XmlDocText
				&& XmlDocTag == s.XmlDocTag
				&& LineNumber == s.LineNumber;
		}
	}
}
