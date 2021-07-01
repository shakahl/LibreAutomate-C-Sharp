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

		Image,

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
		public bool hidden;
		//public bool small;

		public TStyle(int color, bool bold, bool hidden = false) {
			this.color = color;
			this.bold = bold;
			this.hidden = hidden;
			//this.small = small;
		}

		public static implicit operator TStyle(int color) => new(color, false);

		public static bool operator ==(TStyle a, TStyle b) => a.color == b.color && a.bold == b.bold;
		public static bool operator !=(TStyle a, TStyle b) => !(a == b);
	}

	public record TStyles //note: must be record, because uses synthesized ==
	{
		public string FontName = "Consolas";
		public int FontSize = 10;
		public int BackgroundColor = 0xffffff;

		public TStyle None; //black
		public TStyle Comment = 0x408000; //green like in VS but towards yellow
		public TStyle String = 0xA07040; //brown, more green
										 //0xc0c0c0; //good contrast with 0xA07040, but maybe not with white background
										 //0xc0e000; //light yellow-green. Too vivid.
		public TStyle StringEscape = 0xB776FB; //pink-purple like in VS
		public TStyle Number = 0x804000; //brown, more red
		public TStyle Punctuation; //black
		public TStyle Operator = 0x0000ff; //blue like keyword
		public TStyle Keyword = 0x0000ff; //blue like in VS
		public TStyle Namespace = 0x808000; //dark yellow
		public TStyle Type = 0x0080c0; //like in VS but more blue
		public TStyle Function = new(0, true);
		public TStyle Variable = 0x204020; //dark green gray
		public TStyle Constant = 0x204020; //like variable
		public TStyle Label = 0xff00ff; //magenta
		public TStyle Preprocessor = 0xff8000; //orange
		public TStyle Excluded = 0x808080; //gray
		public TStyle XmlDocText = 0x408000; //green like comment
		public TStyle XmlDocTag = 0x808080; //gray

		public TStyle Image = new(0xf0f0f0, false, true); //hidden
		//public TStyle Image = 0xffffff; //visible only when selected or if dark theme
		//public TStyle Image = 0xf0f0f0; //barely visible, unless selected or if dark theme
		//public TStyle Image = 0xe0e0e0;

		public TStyle LineNumber = 0x808080;

		public static TStyles Settings {
			get => s_styles ??= new TStyles();
			set {
				s_styles = value;
				if (value != null) value._Save();
				else filesystem.delete(s_settingsFile);
			}
		}
		static TStyles s_styles;
		internal static readonly string s_settingsFile = AppSettings.DirBS + "Font.csv";

		public TStyles() {
			csvTable csv;
			if (!filesystem.exists(s_settingsFile).isFile) return;
			try { csv = csvTable.load(s_settingsFile); }
			catch (Exception e1) { print.it(e1.ToStringWithoutStack()); return; }
			if (csv.ColumnCount < 2) return;

			foreach (var a in csv.Rows) {
				switch (a[0]) {
				case "Font":
					if (!a[1].NE()) FontName = a[1];
					if (a.Length > 2) { int fs = a[2].ToInt(); if (fs >= 5 && fs <= 100) FontSize = fs; }
					break;
				case "Background":
					if (!a[1].NE()) BackgroundColor = a[1].ToInt();
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

			static void _Style(ref TStyle r, string[] a) {
				if (!a[1].NE()) r.color = a[1].ToInt();
				if (a.Length > 2 && !a[2].NE()) r.bold = 0 != (1 & a[2].ToInt()); else r.bold = false;
			}
		}

		void _Save() {
			var b = new StringBuilder(); //don't need csvTable for such simple values
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

			void _Style(string name, TStyle r) {
				b.Append(name).Append(", 0x").Append(r.color.ToString("X6"));
				if (r.bold) b.Append(", 1");
				b.AppendLine();
			}

			filesystem.saveText(s_settingsFile, b.ToString());
		}

		/// <summary>
		/// Gets colors, bold, but not font properties.
		/// </summary>
		public TStyles(KScintilla sci) {
			BackgroundColor = ColorInt.SwapRB(sci.Call(SCI_STYLEGETBACK));

			TStyle _Get(EToken tok) {
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

		public void ToScintilla(KScintilla sci) {
			sci.zStyleFont(STYLE_DEFAULT, FontName, FontSize);
			sci.zStyleBackColor(STYLE_DEFAULT, BackgroundColor);
			//if(None.color != 0) sci.zStyleForeColor(STYLE_DEFAULT, None.color); //also would need bold and in ctor above
			sci.zStyleClearAll();

			void _Set(EToken tok, TStyle sty) {
				sci.zStyleForeColor((int)tok, sty.color);
				if (sty.bold) sci.zStyleBold((int)tok, true);
				//cannot use hidden style or small font because of scintilla bug:
				//	1. In wrap mode draws as many lines as with big font. Even caret is large and spans all lines.
				//		Plus other anomalies, eg when scrolling.
				//		I could not find a workaround. Tried SCI_SETLAYOUTCACHE, SCI_SETPOSITIONCACHE, SCI_SETHOTSPOTSINGLELINE, etc.
				//	2. User cannot delete text containing hidden text.
				//		Need to modify scintilla source; maybe just simply modify IsProtected() in Style.h.
				//if (sty.hidden) sci.zStyleHidden((int)tok, true);
				//if (sty.small) { sci.zStyleFont((int)tok, "Gabriola", 1); sci.Call(SCI_STYLESETCASE, (int)tok, 2); } //smallest font available on Win7 too

				//if (sty.hidden) { sci.zStyleHidden((int)tok, true); }
				//if (sty.hidden) { /*sci.zStyleHidden((int)tok, true);*/ sci.zStyleHotspot((int)tok, true); }
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

			_Set(EToken.Image, Image);

			_Set((EToken)STYLE_LINENUMBER, LineNumber);
		}

		//not used
		//public TStyle GetStyle(EToken token) {
		//	return token switch {
		//		EToken.None => None,
		//		EToken.Comment => Comment,
		//		EToken.String => String,
		//		EToken.StringEscape => StringEscape,
		//		EToken.Number => Number,
		//		EToken.Punctuation => Punctuation,
		//		EToken.Operator => Operator,
		//		EToken.Keyword => Keyword,
		//		EToken.Namespace => Namespace,
		//		EToken.Type => Type,
		//		EToken.Function => Function,
		//		EToken.Variable => Variable,
		//		EToken.Constant => Constant,
		//		EToken.Label => Label,
		//		EToken.Preprocessor => Preprocessor,
		//		EToken.Excluded => Excluded,
		//		EToken.XmlDocText => XmlDocText,
		//		EToken.XmlDocTag => XmlDocTag,
		//		EToken.LineNumber => LineNumber,
		//		_ => default,
		//	};
		//}

		//public void SetStyle(EToken token, TStyle style) {
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
	}
}
