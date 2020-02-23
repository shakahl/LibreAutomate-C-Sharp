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
//using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	/// <summary>
	/// Draws text using GDI API such as TextOut using standard UI font.
	/// Can easily draw string parts with different colors/styles without measuring.
	/// Faster than TextRenderer.
	/// Must be disposed.
	/// </summary>
	public struct GdiTextRenderer : IDisposable //SHOULDDO: use everywhere
	{
		Graphics _g;
		IntPtr _hdc, _oldFont;
		uint _oldAlign;
		int _color, _oldColor;

		public GdiTextRenderer(Graphics g) : this()
		{
			_g = g;
			_hdc = g.GetHdc();
			_oldFont = Api.SelectObject(_hdc, Util.NativeFont_.RegularCached);
			_oldAlign = 0xffffffff;
			Api.SetBkMode(_hdc, 1);
			_oldColor = Api.SetTextColor(_hdc, 0);
		}

		public void Dispose()
		{
			if(_oldAlign != 0xffffffff) Api.SetTextAlign(_hdc, _oldAlign);
			if(_oldColor != _color) Api.SetTextColor(_hdc, _oldColor);
			Api.SelectObject(_hdc, _oldFont);
			//never mind: we don't restore the old current position. Nobody later would draw at current position without movetoex. As well as bkmode.
			_g.ReleaseHdc();
		}

		/// <summary>
		/// Sets the current drawing position of the DC.
		/// Returns previous position.
		/// </summary>
		public POINT MoveTo(int x, int y)
		{
			Api.MoveToEx(_hdc, x, y, out var p);
			return p;
		}

		/// <summary>
		/// Gets the current drawing position of the DC.
		/// </summary>
		public POINT GetCurrentPosition()
		{
			Api.GetCurrentPositionEx(_hdc, out var p);
			return p;
		}

		public void FontNormal() => Api.SelectObject(_hdc, Util.NativeFont_.RegularCached);

		public void FontBold() => Api.SelectObject(_hdc, Util.NativeFont_.BoldCached);

		//public void FontItalic() => Api.SelectObject(_hdc, Au.Util.NativeFont_.ItalicCached);

		//public void FontBoldItalic() => Api.SelectObject(_hdc, Au.Util.NativeFont_.BoldItalicCached);

		/// <summary>
		/// Draws text at specified position. Does not use/update the current drawing position of the DC.
		/// </summary>
		public void DrawText(string s, int x, int y, ColorInt color = default, int from = 0, int to = -1)
		{
			int len = _Len(s, from, to); if(len == 0) return;
			if(_oldAlign != 0xffffffff) { Api.SetTextAlign(_hdc, _oldAlign); _oldAlign = 0xffffffff; }
			_DrawText(s, x, y, color, from, len);
		}

		/// <summary>
		/// Draws text at the current drawing position of the DC, and updates it.
		/// </summary>
		public void DrawText(string s, ColorInt color = default, int from = 0, int to = -1)
		{
			int len = _Len(s, from, to); if(len == 0) return;
			if(_oldAlign == 0xffffffff) _oldAlign = Api.SetTextAlign(_hdc, 1); //TA_UPDATECP
			_DrawText(s, 0, 0, color, from, len);
		}

		static int _Len(string s, int from, int to)
		{
			if((uint)from >= s.Length || to > s.Length) return 0;
			if(to < 0) to = s.Length;
			return to - from;
		}

		unsafe void _DrawText(string s, int x, int y, ColorInt color, int from, int len)
		{
			var col = color.ToBGR(); if(col != _color) Api.SetTextColor(_hdc, _color = col);
			fixed (char* p = s) Api.TextOut(_hdc, x, y, p + from, len);
		}
	}
}
