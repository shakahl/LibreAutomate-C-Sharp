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
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	using static Sci;

	unsafe partial class SciText
	{
		#region styles

		public void StyleFont(int style, string name)
		{
			SetString(SCI_STYLESETFONT, style, name);
		}

		//public string StyleFont(int style)
		//{
		//	return GetString(SCI_STYLEGETFONT, style, 100);
		//}

		public void StyleFont(int style, string name, int size)
		{
			StyleFont(style, name);
			StyleFontSize(style, size);
		}

		public void StyleFontSize(int style, int value)
		{
			Call(SCI_STYLESETSIZE, style, value);
		}

		//public int StyleFontSize(int style)
		//{
		//	return Call(SCI_STYLEGETSIZE, style);
		//}

		public void StyleHidden(int style, bool value)
		{
			Call(SCI_STYLESETVISIBLE, style, !value);
		}

		//public bool StyleHidden(int style)
		//{
		//	return 0 == Call(SCI_STYLEGETVISIBLE, style);
		//}

		public void StyleBold(int style, bool value)
		{
			Call(SCI_STYLESETBOLD, style, value);
		}

		public void StyleItalic(int style, bool value)
		{
			Call(SCI_STYLESETITALIC, style, value);
		}

		public void StyleUnderline(int style, bool value)
		{
			Call(SCI_STYLESETUNDERLINE, style, value);
		}

		public void StyleEolFilled(int style, bool value)
		{
			Call(SCI_STYLESETEOLFILLED, style, value);
		}

		public void StyleHotspot(int style, bool value)
		{
			Call(SCI_STYLESETHOTSPOT, style, value);
		}

		public bool StyleHotspot(int style)
		{
			return 0 != Call(SCI_STYLEGETHOTSPOT, style);
		}

		public void StyleForeColor(int style, ColorInt colorRGB)
		{
			Call(SCI_STYLESETFORE, style, colorRGB.ToBGR());
		}

		public void StyleBackColor(int style, ColorInt colorRGB)
		{
			Call(SCI_STYLESETBACK, style, colorRGB.ToBGR());
		}

		/// <summary>
		/// Measures string width.
		/// </summary>
		public int StyleMeasureStringWidth(int style, string s)
		{
			return SetString(SCI_TEXTWIDTH, style, s);
		}

		/// <summary>
		/// Calls SCI_STYLECLEARALL, which sets all styles to be the same as STYLE_DEFAULT.
		/// Then also sets some special styles, eg STYLE_HIDDEN.
		/// </summary>
		public void StyleClearAll()
		{
			Call(SCI_STYLECLEARALL);
			StyleHidden(STYLE_HIDDEN, true);
			Call(SCI_SETHOTSPOTACTIVEFORE, true, 0xFF0080); //or 0x80FF

			//STYLE_HOTSPOT currently unused
			//StyleHotspot(STYLE_HOTSPOT, true);
			//StyleForeColor(STYLE_HOTSPOT, 0xFF8000);
		}

		/// <summary>
		/// Calls SCI_STYLECLEARALL(styleFrom, styleToNotIncluding), which sets range of styles to be the same as STYLE_DEFAULT.
		/// If styleToNotIncluding is 0, clears all starting from styleFrom.
		/// </summary>
		public void StyleClearRange(int styleFrom, int styleToNotIncluding = 0)
		{
			Call(SCI_STYLECLEARALL, styleFrom, styleToNotIncluding);
		}

		/// <summary>
		/// Gets style at position.
		/// Uses SCI_GETSTYLEAT.
		/// Returns 0 if pos is invalid.
		/// </summary>
		public int GetStyleAt(int pos)
		{
			return Call(SCI_GETSTYLEAT, pos);
		}

		#endregion

		#region spec styles

		public void SelectionForeColor(bool use, ColorInt colorRGB)
		{
			Call(SCI_SETSELFORE, use, colorRGB.ToBGR());
		}

		public void SelectionBackColor(bool use, ColorInt colorRGB, int alpha = 256)
		{
			Call(SCI_SETSELBACK, use, colorRGB.ToBGR());
			Call(SCI_SETSELALPHA, alpha);
		}

		public void CaretLineColor(bool use, ColorInt colorRGB, int alpha = 256)
		{
			Call(SCI_SETCARETLINEBACK, colorRGB.ToBGR());
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

		public void MarginType(int margin, int SC_MARGIN_)
		{
			Call(SCI_SETMARGINTYPEN, margin, SC_MARGIN_);
		}

		public void MarginWidth(int margin, int value)
		{
			Call(SCI_SETMARGINWIDTHN, margin, value);
		}

		public void MarginWidth(int margin, string textToMeasureWidth)
		{
			int n = StyleMeasureStringWidth(STYLE_LINENUMBER, textToMeasureWidth);
			Call(SCI_SETMARGINWIDTHN, margin, n + 4);
		}

		public int MarginWidth(int margin)
		{
			return Call(SCI_GETMARGINWIDTHN, margin);
		}

		#endregion
	}
}
