using Au.Types;
using Au.More;
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
	/// <summary>
	/// Draws text using fastest GDI API such as TextOut and standard UI font.
	/// Can easily draw string parts with different colors/styles without measuring.
	/// Must be disposed.
	/// </summary>
	public unsafe class GdiTextRenderer : IDisposable
	{
		IntPtr _dc, _oldFont;
		uint _oldAlign;
		int _color, _oldColor;
		int _dpi;
		bool _releaseDC;

		/// <summary>Object created with this ctor can draw and measure.</summary>
		/// <param name="hdc">Device context handle. <b>Dispose</b> will not release it.</param>
		/// <param name="dpi"></param>
		public GdiTextRenderer(IntPtr hdc, int dpi) {
			_dpi = dpi;
			_dc = hdc;
			_oldFont = Api.SelectObject(_dc, NativeFont_.RegularCached(_dpi));
			_oldAlign = 0xffffffff;
			Api.SetBkMode(_dc, 1);
			_oldColor = Api.SetTextColor(_dc, _color = 0);
		}

		/// <summary>Object created with this ctor can measure only. Uses screen DC.</summary>
		public GdiTextRenderer(int dpi) {
			_dpi = dpi;
			_releaseDC = true;
			_dc = Api.GetDC(default);
			_oldFont = Api.SelectObject(_dc, NativeFont_.RegularCached(_dpi));
		}

		public void Dispose() {
			if (_dc != default) {
				Api.SelectObject(_dc, _oldFont);
				if (_releaseDC) Api.ReleaseDC(default, _dc);
				else {
					if (_oldAlign != 0xffffffff) Api.SetTextAlign(_dc, _oldAlign);
					if (_oldColor != _color) Api.SetTextColor(_dc, _oldColor);
				}
				_dc = default;
			}
			//never mind: we don't restore the old current position. Nobody later would draw at current position without movetoex. As well as bkmode.
		}

		/// <summary>
		/// Sets the current drawing position of the DC.
		/// Returns previous position.
		/// </summary>
		public POINT MoveTo(int x, int y) {
			Api.MoveToEx(_dc, x, y, out var p);
			return p;
		}

		/// <summary>
		/// Gets the current drawing position of the DC.
		/// </summary>
		public POINT GetCurrentPosition() {
			Api.GetCurrentPositionEx(_dc, out var p);
			return p;
		}

		public void FontNormal() => Api.SelectObject(_dc, NativeFont_.RegularCached(_dpi));

		public void FontBold() => Api.SelectObject(_dc, NativeFont_.BoldCached(_dpi));

		//public void FontItalic() => Api.SelectObject(_dc, NativeFont_.ItalicCached(_dpi));

		//public void FontBoldItalic() => Api.SelectObject(_dc, NativeFont_.BoldItalicCached(_dpi));

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <summary>
		/// Draws text at the current drawing position of the DC, and updates it.
		/// </summary>
		/// <param name="color">0xBBGGRR</param>
		public void DrawText(string s, int color = 0, Range? range = null) {
			var (from, len) = range.GetOffsetAndLength(s.Length); if (len == 0) return;
			if (_oldAlign == 0xffffffff) _oldAlign = Api.SetTextAlign(_dc, 1); //TA_UPDATECP
			_DrawText(s, 0, 0, color, from, len);
		}

		/// <summary>
		/// Draws text at specified position. Does not use/update the current drawing position of the DC.
		/// </summary>
		/// <param name="color">0xBBGGRR</param>
		public void DrawText(string s, POINT p, int color = 0, Range? range = null) {
			var (from, len) = range.GetOffsetAndLength(s.Length); if (len == 0) return;
			if (_oldAlign != 0xffffffff) { Api.SetTextAlign(_dc, _oldAlign); _oldAlign = 0xffffffff; }
			_DrawText(s, p.x, p.y, color, from, len);
		}

		/// <summary>
		/// Draws text clipped in specified rectangle. Does not use/update the current drawing position of the DC.
		/// </summary>
		/// <param name="color">0xBBGGRR</param>
		public void DrawText(string s, in RECT r, int color = 0, Range? range = null) {
			var (from, len) = range.GetOffsetAndLength(s.Length); if (len == 0) return;
			if (_oldAlign != 0xffffffff) { Api.SetTextAlign(_dc, _oldAlign); _oldAlign = 0xffffffff; }
			_DrawText(s, r, color, from, len);
		}
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

		unsafe void _DrawText(string s, int x, int y, int color, int from, int len) {
			if (color != _color) Api.SetTextColor(_dc, _color = color);
			fixed (char* p = s) Api.TextOut(_dc, x, y, p + from, len);
		}

		unsafe void _DrawText(string s, in RECT r, int color, int from, int len) {
			if (color != _color) Api.SetTextColor(_dc, _color = color);
			fixed (char* p = s) Api.ExtTextOut(_dc, r.left, r.top, Api.ETO_CLIPPED, r, p + from, len);
		}

		public SIZE MeasureText(string s, Range? range = null) {
			var (from, len) = range.GetOffsetAndLength(s.Length); if (len == 0) return default;
			fixed (char* p = s) {
				Api.GetTextExtentPoint32(_dc, p + from, len, out var z);
				return z;
			}
		}
	}
}
