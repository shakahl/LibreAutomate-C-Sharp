using Au.Types;
using Au.Util;
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

namespace Au.Controls
{
	using static Sci;

	unsafe partial class SciText
	{
		#region styles

		public void StyleFont(int style, string name) {
			SetString(SCI_STYLESETFONT, style, name);
		}

		//public string StyleFont(int style)
		//{
		//	return GetString(SCI_STYLEGETFONT, style, 100);
		//}

		public void StyleFont(int style, string name, int size) {
			StyleFont(style, name);
			StyleFontSize(style, size);
		}

		/// <remarks>Uses only font name and size. Not style etc.</remarks>
		public void StyleFont(int style, System.Windows.Controls.Control c) {
			StyleFont(style, c.FontFamily.ToString(), c.FontSize.ToInt() * 72 / 96);
		}

		public void StyleFontSize(int style, int value) {
			Call(SCI_STYLESETSIZE, style, value);
		}

		//public int StyleFontSize(int style)
		//{
		//	return Call(SCI_STYLEGETSIZE, style);
		//}

		public void StyleHidden(int style, bool value) {
			Call(SCI_STYLESETVISIBLE, style, !value);
		}

		//public bool StyleHidden(int style)
		//{
		//	return 0 == Call(SCI_STYLEGETVISIBLE, style);
		//}

		public void StyleBold(int style, bool value) {
			Call(SCI_STYLESETBOLD, style, value);
		}

		public void StyleItalic(int style, bool value) {
			Call(SCI_STYLESETITALIC, style, value);
		}

		public void StyleUnderline(int style, bool value) {
			Call(SCI_STYLESETUNDERLINE, style, value);
		}

		public void StyleEolFilled(int style, bool value) {
			Call(SCI_STYLESETEOLFILLED, style, value);
		}

		public void StyleHotspot(int style, bool value) {
			Call(SCI_STYLESETHOTSPOT, style, value);
		}

		public bool StyleHotspot(int style) {
			return 0 != Call(SCI_STYLEGETHOTSPOT, style);
		}

		public void StyleForeColor(int style, ColorInt color) {
			Call(SCI_STYLESETFORE, style, color.ToBGR());
		}

		public void StyleBackColor(int style, ColorInt color) {
			Call(SCI_STYLESETBACK, style, color.ToBGR());
		}

		/// <summary>
		/// Measures string width.
		/// </summary>
		public int StyleMeasureStringWidth(int style, string s) {
			return SetString(SCI_TEXTWIDTH, style, s);
		}

		/// <summary>
		/// Calls SCI_STYLECLEARALL, which sets all styles to be the same as STYLE_DEFAULT.
		/// Then also sets some special styles, eg STYLE_HIDDEN and hotspot color.
		/// </summary>
		public void StyleClearAll() {
			Call(SCI_STYLECLEARALL);
			StyleHidden(STYLE_HIDDEN, true);
			Call(SCI_SETHOTSPOTACTIVEFORE, true, 0xFF0080); //inactive 0x0080FF

			//STYLE_HOTSPOT currently unused
			//StyleHotspot(STYLE_HOTSPOT, true);
			//StyleForeColor(STYLE_HOTSPOT, 0xFF8000);
		}

		/// <summary>
		/// Calls SCI_STYLECLEARALL(styleFrom, styleToNotIncluding), which sets range of styles to be the same as STYLE_DEFAULT.
		/// If styleToNotIncluding is 0, clears all starting from styleFrom.
		/// </summary>
		public void StyleClearRange(int styleFrom, int styleToNotIncluding = 0) {
			Call(SCI_STYLECLEARALL, styleFrom, styleToNotIncluding);
		}

		/// <summary>
		/// Gets style at position.
		/// Uses SCI_GETSTYLEAT.
		/// Returns 0 if pos is invalid.
		/// </summary>
		public int GetStyleAt(int pos) {
			return Call(SCI_GETSTYLEAT, pos);
		}

		#endregion

		#region spec styles

		public void SelectionForeColor(bool use, ColorInt color) {
			Call(SCI_SETSELFORE, use, color.ToBGR());
		}

		public void SelectionBackColor(bool use, ColorInt color, int alpha = 256) {
			Call(SCI_SETSELBACK, use, color.ToBGR());
			Call(SCI_SETSELALPHA, alpha);
		}

		public void CaretLineColor(bool use, ColorInt color, int alpha = 256) {
			Call(SCI_SETCARETLINEBACK, color.ToBGR());
			Call(SCI_SETCARETLINEBACKALPHA, alpha);
			Call(SCI_SETCARETLINEVISIBLE, use);
		}

		//we set it in ctor and don't need to change
		//public void CaretWidth(int value)
		//{
		//	Call(SCI_SETCARETWIDTH, value);
		//}

		#endregion

		#region margins

		public void MarginType(int margin, int SC_MARGIN_) {
			Call(SCI_SETMARGINTYPEN, margin, SC_MARGIN_);
		}

		public void MarginWidth(int margin, int value, bool dpiScale = true, bool chars = false) {
			if (dpiScale && value > 0) {
				var a = _c._marginDpi ??= new int[Call(SCI_GETMARGINS)];
				if (chars) {
					value *= StyleMeasureStringWidth(STYLE_LINENUMBER, "8");
					a[margin] = ADpi.Unscale(value, _c._dpi).ToInt();
				} else {
					a[margin] = value;
					value = ADpi.Scale(value, _c._dpi);
				}
			} else {
				var a = _c._marginDpi;
				if (a != null) a[margin] = 0;
			}
			Call(SCI_SETMARGINWIDTHN, margin, value);
		}

		//public void MarginWidth(int margin, string textToMeasureWidth) {
		//	int n = StyleMeasureStringWidth(STYLE_LINENUMBER, textToMeasureWidth);
		//	Call(SCI_SETMARGINWIDTHN, margin, n + 4);
		//}

		//not used
		//public int MarginWidth(int margin, bool dpiUnscale) {
		//	int R = Call(SCI_GETMARGINWIDTHN, margin);
		//	if (dpiUnscale && R > 0) {
		//		var a = _c._marginDpi;
		//		var v = a?[margin] ?? 0;
		//		if (v > 0) R = v;
		//	}
		//	return R;
		//}

		internal void MarginWidthsDpiChanged_() {
			var a = _c._marginDpi; if (a == null) return;
			for (int i = a.Length; --i >= 0;) {
				if (a[i] > 0) Call(SCI_SETMARGINWIDTHN, i, ADpi.Scale(a[i], _c._dpi));
			}
		}

		public int MarginFromPoint(POINT p, bool screenCoord) {
			if (screenCoord) _c.Hwnd.MapScreenToClient(ref p);
			if (_c.Hwnd.ClientRect.Contains(p)) {
				for (int i = 0, n = Call(SCI_GETMARGINS), w = 0; i < n; i++) { w += Call(SCI_GETMARGINWIDTHN, i); if (w >= p.x) return i; }
			}
			return -1;
		}

		#endregion

		#region lexer

		public void SetLexerCpp(bool noClear = false) {
			if (!noClear) StyleClearRange(0, STYLE_HIDDEN); //STYLE_DEFAULT - 1
			Call(SCI_SETLEXER, (int)LexLanguage.SCLEX_CPP);

			const int colorComment = 0x8000;
			const int colorString = 0xA07040;
			const int colorNumber = 0xA04000;
			const int colorDoc = 0x606060;
			StyleForeColor((int)LexCppStyles.SCE_C_COMMENT, colorComment); //  /*...*/
			StyleForeColor((int)LexCppStyles.SCE_C_COMMENTLINE, colorComment); //  //...
			StyleForeColor((int)LexCppStyles.SCE_C_COMMENTLINEDOC, colorDoc); //  ///...
			StyleForeColor((int)LexCppStyles.SCE_C_COMMENTDOC, colorDoc); //  /**...*/
			StyleForeColor((int)LexCppStyles.SCE_C_CHARACTER, colorNumber);
			StyleForeColor((int)LexCppStyles.SCE_C_NUMBER, colorNumber);
			StyleForeColor((int)LexCppStyles.SCE_C_STRING, colorString);
			StyleForeColor((int)LexCppStyles.SCE_C_VERBATIM, colorString); //@"string"
			StyleForeColor((int)LexCppStyles.SCE_C_ESCAPESEQUENCE, 0xC0C0C0);
			//StyleForeColor((int)LexCppStyles.SCE_C_OPERATOR, 0x80); //+,;( etc. Let it be black.
			StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSOR, 0xFF8000);
			StyleForeColor((int)LexCppStyles.SCE_C_WORD, 0xFF); //keywords
			StyleForeColor((int)LexCppStyles.SCE_C_TASKMARKER, 0x00C000);
			StyleBackColor((int)LexCppStyles.SCE_C_TASKMARKER, 0x0);
			//StyleForeColor((int)LexCppStyles.SCE_C_WORD2, 0x80F0); //functions. Not using here.
			//StyleForeColor((int)LexCppStyles.SCE_C_GLOBALCLASS, 0xC000C0); //types. Not using here.

			//StyleForeColor((int)LexCppStyles.SCE_C_USERLITERAL, ); //C++, like 10_km
			//StyleForeColor((int)LexCppStyles.SCE_C_STRINGRAW, ); //R"string"
			//StyleForeColor((int)LexCppStyles.SCE_C_COMMENTDOCKEYWORD, ); //supports only JavaDoc and Doxygen
			//StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSORCOMMENT, ); //?
			//StyleForeColor((int)LexCppStyles.SCE_C_PREPROCESSORCOMMENTDOC, ); //?

			SetStringString(SCI_SETPROPERTY, "styling.within.preprocessor\0" + "1");
			SetStringString(SCI_SETPROPERTY, "lexer.cpp.allow.dollars\0" + "0");
			SetStringString(SCI_SETPROPERTY, "lexer.cpp.track.preprocessor\0" + "0"); //default 1
			SetStringString(SCI_SETPROPERTY, "lexer.cpp.escape.sequence\0" + "1");
			//SetStringString(SCI_SETPROPERTY, "lexer.cpp.verbatim.strings.allow.escapes\0" + "1"); //expected to style "", but it does nothing


			//AOutput.Write(GetString(SCI_DESCRIBEKEYWORDSETS, 0, -1));
			//Primary keywords and identifiers
			//Secondary keywords and identifiers
			//Documentation comment keywords
			//Global classes and typedefs
			//Preprocessor definitions
			//Task marker and error marker keywords
			SetString(SCI_SETKEYWORDS, 0, "abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while add alias ascending async await by descending dynamic equals from get global group into join let nameof on orderby partial remove select set value var when where yield unmanaged notnull record init nint nuint not and or");
			//SetString(SCI_SETKEYWORDS, 1, "Function"); //functions. Not using here.
			//SetString(SCI_SETKEYWORDS, 2, "summary <summary>"); //supports only JavaDoc and Doxygen
			//SetString(SCI_SETKEYWORDS, 3, "Au"); //types. Not using here.
			//SetString(SCI_SETKEYWORDS, 4, "DEBUG TRACE"); //if used with #if, lexer knows which #if/#else branch to style. Not using here (see "lexer.cpp.track.preprocessor").
			SetString(SCI_SETKEYWORDS, 5, "TO" + "DO SHOULD" + "DO CON" + "SIDER FU" + "TURE B" + "UG HA" + "CK");
		}

		#endregion
	}
}
