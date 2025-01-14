﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace Au;

public unsafe partial class popupMenu {
	/// <summary>
	/// Sets some metrics, for example item padding.
	/// </summary>
	/// <seealso cref="DefaultMetrics"/>
	public PMMetrics Metrics { get; set; }

	/// <summary>
	/// Sets or gets default metrics.
	/// </summary>
	/// <seealso cref="Metrics"/>
	public static PMMetrics DefaultMetrics {
		get => s_defaultMetrics ??= new();
		set { s_defaultMetrics = value; }
	}
	static PMMetrics s_defaultMetrics;

	/// <summary>
	/// Sets or gets font.
	/// </summary>
	public FontNSS Font { get; set; }

	/// <summary>
	/// Sets or gets default font.
	/// </summary>
	public static FontNSS DefaultFont { get; set; }

	NativeFont_ _GetFont(bool bold = false) {
		var font = Font ?? DefaultFont;
		if (font == null) return bold ? NativeFont_.BoldCached(_dpi) : NativeFont_.RegularCached(_dpi);
		if(!bold) return _font ??= font.CreateFont(_dpi);
		return _fontBold ??= (font with { Bold = true }).CreateFont(_dpi);
	}
	NativeFont_ _font, _fontBold; //disposing in _WmNcdestroy or ~NativeFont_

	_Metrics _z;

	class _Metrics : IDisposable {
		public bool hasImages, hasCheck, hasSubmenus, hasSeparators, hasHotkeys;
		public int border, paddingY, paddingLeft, paddingRight, textPaddingX, textPaddingY, image, check, submenu, submenuMargin, separator, sepLine;
		public SIZE zCheck, zSubmenu;
		public IntPtr theme;
		public int xTextEnd, xHotkeyStart;

		public void Dispose() {
			if (theme != default) {
				Api.CloseThemeData(theme);
				theme = default;
			}
			GC.SuppressFinalize(this);
		}

		~_Metrics() => Dispose();

		public _Metrics(popupMenu m) {
			foreach (var b in m._a) {
				if (b.IsSeparator) hasSeparators = true;
				else {
					if (b.HasImage_) hasImages = true;
					if (b.checkType > 0) hasCheck = true;
					if (b.IsSubmenu) hasSubmenus = true;
					if (b.Hotkey != null) hasHotkeys = true;
				}
			}

			var k = m.Metrics;
			if (k == null) {
				for (var p = m._sub.parent; p != null; p = p._sub.parent) if ((k = p.Metrics) != null) break;
			}
			k ??= DefaultMetrics;

			int dpi = m._dpi;
			border = dpi / 96;
			paddingY = Dpi.Scale(k.ItemPaddingY, dpi);
			paddingLeft = Dpi.Scale(k.ItemPaddingLeft, dpi);
			paddingRight = Dpi.Scale(k.ItemPaddingRight, dpi);
			textPaddingX = Dpi.Scale(8, dpi);
			textPaddingY = Dpi.Scale(1, dpi);
			if (hasImages) image = Dpi.Scale(16, dpi);
			if (hasCheck) check = Dpi.Scale(18, dpi);
			if (hasSubmenus) submenu = Dpi.Scale(16, dpi);
			separator = Dpi.Scale(8, dpi);
			sepLine = 2;

			theme = Api.OpenThemeData(m._w, "Menu", dpi);
			if (theme != default) {
				using var dc = new ScreenDC_();
				if (hasSubmenus) {
					Api.GetThemePartSize(theme, dc, 16, 1, null, Api.THEMESIZE.TS_TRUE, out zSubmenu);
					submenu = Math.Max(submenu, zSubmenu.width + submenu / 4);
				}
				if (hasCheck) {
					Api.GetThemePartSize(theme, dc, 11, 1, null, Api.THEMESIZE.TS_TRUE, out zCheck);
					check = Math.Max(check, zCheck.width + 2);
				}
				if (hasSeparators && 0 == Api.GetThemePartSize(theme, dc, 15, 0, null, Api.THEMESIZE.TS_TRUE, out var z)) sepLine = z.height;
			}

			submenuMargin = hasHotkeys ? 0 : submenu;
		}
	}

	SIZE _Measure(int maxWidth) {
		SIZE R = default;

		_z?.Dispose();
		_z = new _Metrics(this);

		int buttonPlusX = (_z.border + _z.textPaddingX) * 2 + _z.check + _z.image + _z.submenu + _z.submenuMargin + _z.paddingLeft + _z.paddingRight;
		int maxTextWidth = maxWidth - buttonPlusX;

		var font = _GetFont();
		using var dc = new FontDC_(font);
		int textHeight = dc.MeasureEP(" ").height;

		int maxHotkey = 0;
		if (_z.hasHotkeys) {
			foreach (var b in _a) {
				if (b.Hotkey == null) continue;
				int wid = dc.MeasureDT(b.Hotkey, c_tffHotkey).width;
				maxHotkey = Math.Max(maxHotkey, Math.Min(wid, maxTextWidth / 2));
			}
		}
		int hotkeyPlus = textHeight * 3 / 2; //space between text and hotkey
		if (maxHotkey > 0) maxTextWidth -= maxHotkey += hotkeyPlus;

		int y = 0;
		for (int i = 0; i < _a.Count; i++) {
			//note: to support multiline, wrap, underlines and tabs we have to use DrawText, not TextOut.
			//	DrawText(DT_CALCRECT) is slow. Very slow compared with GetTextExtentPoint32. Eg 100-300 ms for 1000 items. Depends on text length.
			var b = _a[i];
			SIZE z;
			if (b.IsSeparator) {
				z = new(0, _z.separator);
			} else {
				var s = b.Text;
				if (!s.NE()) {
					if (b.FontBold) Api.SelectObject(dc, _GetFont(true));
					z = dc.MeasureDT(s, _TfFlags(b), maxTextWidth);
					z.width = Math.Min(z.width, maxTextWidth);
					if (b.FontBold) Api.SelectObject(dc, font);
					_z.xTextEnd = Math.Max(_z.xTextEnd, z.width);
					z.width += buttonPlusX;
				} else z = new(0, textHeight);
				z.height = Math.Max(z.height + _z.textPaddingY * 2 + 1, _z.image) + (_z.border + _z.paddingY) * 2;
			}
			b.rect = new(0, y, z.width, z.height);
			y += z.height;
			R.width = Math.Max(R.width, z.width);
			R.height = Math.Max(R.height, y);
		}

		if (maxHotkey > 0) {
			R.width += maxHotkey;
			_z.xHotkeyStart = _z.xTextEnd + hotkeyPlus;
		}
		foreach (var b in _a) b.rect.right = R.width;

		return R;
	}

	TFFlags _TfFlags(PMItem b) {
		var f = c_tff; if (b.rawText) f |= TFFlags.NOPREFIX; else if (!_flags.Has(PMFlags.Underline)) f |= TFFlags.HIDEPREFIX;
		return f;
	}
	const TFFlags c_tff = TFFlags.EXPANDTABS | TFFlags.WORDBREAK /*| TFFlags.PATH_ELLIPSIS*/;
	const TFFlags c_tffHotkey = TFFlags.NOPREFIX | TFFlags.SINGLELINE;

	void _Render(IntPtr dc, RECT rUpdate) {
		Api.FillRect(dc, rUpdate, (IntPtr)(Api.COLOR_BTNFACE + 1));

		using var g = Graphics.FromHdc(dc);
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		var font = _GetFont();
		using var soFont = new GdiSelectObject_(dc, font);
		Api.SetBkMode(dc, 1);
		int textColor = Api.GetSysColor(Api.COLOR_BTNTEXT), textColorDisabled = Api.GetSysColor(Api.COLOR_GRAYTEXT);

		rUpdate.Offset(0, _scroll.Offset);
		for (int i = _scroll.Pos; i < _a.Count; i++) {
			var b = _a[i];

			if (b.rect.bottom <= rUpdate.top) continue;
			if (b.rect.top >= rUpdate.bottom) break;

			var r = _ItemRect(b);

			//g.DrawRectangleInset(Pens.BlueViolet, r);

			if (b.IsSeparator) {
				r.Inflate(-_z.textPaddingX / 4, 0);
				//r.left+=_z.check+_z.image;
				r.top += (r.Height - _z.sepLine) / 2;
				r.Height = _z.sepLine;
				if (_z.theme != default) Api.DrawThemeBackground(_z.theme, dc, 15, 0, r);
				else Api.DrawEdge(dc, ref r, Api.EDGE_ETCHED, Api.BF_TOP);
			} else {
				if (b.BackgroundColor != default) using (var brush = new SolidBrush((Color)b.BackgroundColor)) g.FillRectangle(brush, r);
				if (i == _iHot) {
					if ((textColor == 0 || b.TextColor != default) && b.BackgroundColor == default)
						using (var brush = new SolidBrush(0xC0DCF3.ToColor_())) g.FillRectangle(brush, r);
					using (var pen = new Pen(0x90C8F6.ToColor_(), _z.border)) g.DrawRectangleInset(pen, r);
				}

				r.Inflate(-_z.border, -_z.border - _z.paddingY);
				r.left += _z.paddingLeft; r.right -= _z.paddingRight;
				var r2 = r;

				r.left += _z.check;

				if (b.image2 != null) {
					g.DrawImage(b.image2, r.left + _z.textPaddingY, r.top + (r.Height - _z.image) / 2, _z.image, _z.image);
				}

				r.left += _z.image + _z.textPaddingX; r.right -= _z.textPaddingX + _z.submenu + _z.submenuMargin;
				r.top += _z.textPaddingY; r.bottom -= _z.textPaddingY;

				if (b.Hotkey != null) {
					Api.SetTextColor(dc, textColorDisabled);
					var rh = r; rh.left += _z.xHotkeyStart;
					Api.DrawText(dc, b.Hotkey, ref rh, c_tffHotkey);
				}

				Api.SetTextColor(dc, b.TextColor != default ? b.TextColor.ToBGR() : (b.IsDisabled ? textColorDisabled : textColor));
				if (!b.Text.NE()) {
					if (b.FontBold) Api.SelectObject(dc, _GetFont(true));
					r.Width = _z.xTextEnd;
					Api.DrawText(dc, b.Text, ref r, _TfFlags(b));
					if (b.FontBold) Api.SelectObject(dc, font);
				}

				if (b.IsSubmenu) {
					_DrawControl(_z.zSubmenu, 16, b.IsDisabled ? 2 : 1, "➜", r2.right - _z.submenu, r.top, _z.submenu, r.Height);
				}
				if (b.IsChecked) {
					_DrawControl(_z.zCheck, 11, b.checkType == 1 ? (b.IsDisabled ? 2 : 1) : (b.IsDisabled ? 4 : 3), b.checkType == 1 ? "✔" : "●", r2.left, r.top, _z.check, r.Height);
				}

				void _DrawControl(SIZE z, int part, int state, string c, int x, int y, int width, int height) {
					if (_z.theme != default && z != default) {
						RECT r = new(x, r2.top + (r2.Height - z.height) / 2, z.width, z.height);
						Api.DrawThemeBackground(_z.theme, dc, part, state, r);
					} else {
						RECT r = new(x, y, width, height);
						Api.DrawText(dc, c, ref r, TFFlags.CENTER);
						//cannot use DrawFrameControl(DFC_MENU, DFCS_MENUARROW etc), it draws with white background and small when high DPI
					}
				}
			}
		}
	}

	void _WmNcpaint() {
		using var dc = new WindowDC_(Api.GetWindowDC(_w), _w);
		RECT r = new(0, 0, _size.window.width, _size.window.height);
		for (int i = 0; i < _size.border; i++) {
			Api.FrameRect(dc, r, Api.GetSysColorBrush(i == 0 ? Api.COLOR_BTNSHADOW : Api.COLOR_BTNFACE));
			r.Inflate(-1, -1);
		}
	}

	internal void Invalidate_(PMItem k = null) {
		_ThreadTrap();
		if (_w.Is0) return;
		if (k != null) Api.InvalidateRect(_w, _ItemRect(k));
		else Api.InvalidateRect(_w);
	}

	void _Invalidate(int i) => Invalidate_(_a[i]);

	void _Images() {
		foreach (var v in _a) {
			v.image2 = _GetImage(v).image;
		}
	}

	//not used
	//internal void ChangeImage_(PMItem ti, Bitmap b) {
	//	ti.image2 = b;
	//	_Invalidate(ti);
	//}
}
