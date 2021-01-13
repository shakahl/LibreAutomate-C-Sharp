using Au.Types;
using System;
using System.Collections.Generic;
using Au.Util;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Au.Controls
{
	/// <summary>
	/// WPF control for selecting color.
	/// </summary>
	public class KColorPicker : UserControl
	{
		TextBox _tColor;
		_Palette _pal;
		bool _hlsChanging, _palSelecting;

		public KColorPicker() {
			StackPanel p1 = new();
			StackPanel p2 = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 3) };
			p1.Children.Add(p2);

			TextBox tH = null, tL = null, tS = null;

			var fColor = new TextBlock { Text = "Color", ToolTip = "Color 0xRRGGBB", Foreground = Brushes.Black };
			p2.Children.Add(fColor);

			var bColor = new Rectangle { Width = 9, Height = 9, Fill = Brushes.Black, Margin = new Thickness(4, 2, 4, 0) };
			p2.Children.Add(bColor);

			_tColor = new() { Width = 68, Text = "0x000000" };
			_tColor.TextChanged += (o, e) => {
				int col = _GetColor(bgr: true);
				if (!_hlsChanging) {
					var (H, L, S) = _ColorRGBToHLS(col);
					_hlsChanging = true;
					if (S != 0) tH.Text = H.ToString();
					tL.Text = L.ToString();
					tS.Text = S.ToString();
					_hlsChanging = false;
				}

				if (!_palSelecting) _pal.SelectColor(col);

				var brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)col, (byte)(col >> 8), (byte)(col >> 16)));
				fColor.Foreground = brush;
				bColor.Fill = brush;

				if (ColorChanged != null) {
					if (!BGR) col = ColorInt.SwapRB(col);
					ColorChanged(col);
				}
			};
			p2.Children.Add(_tColor);

			//hls: 0 H, 1 L, 2 S.
			TextBox _AddHLS(int hls, string label, string tooltip) {
				p2.Children.Add(new TextBlock { Text = label, Margin = new Thickness(hls == 0 ? 18 : 12, 0, 4, 0), ToolTip = tooltip });
				TextBox t = new() { Width = 34, Text = "0" };
				t.TextChanged += (o, e) => {
					if (_hlsChanging) return;
					var tb = o as TextBox;
					int i = tb.Text.ToInt();
					if (i < 0 || i > 240) {
						_hlsChanging = true;
						tb.Text = Math.Clamp(i, 0, 240).ToStringInvariant();
						_hlsChanging = false;
					}

					int H = tH.Text.ToInt(), L = tL.Text.ToInt(), S = tS.Text.ToInt();
					int col = _ColorHLSToRGB(H, L, S);
					_hlsChanging = true;
					_SetColor(col, bgr: true);
					_hlsChanging = false;
				};
				t.MouseWheel += (_, e) => {
					int d = e.Delta / 15; //+-8
					int i = t.Text.ToInt() + d;
					if (hls == 0) { if (i < 0) i += 240; else if (i > 240) i -= 240; }
					int v = Math.Clamp(i, 0, 240);
					t.Text = v.ToStringInvariant();
				};
				p2.Children.Add(t);
				return t;
			}

			tH = _AddHLS(0, "H", "Hue 0-240. Change with mouse wheel.");
			tL = _AddHLS(1, "L", "Luminance 0-240. Change with mouse wheel.");
			tS = _AddHLS(2, "S", "Saturation 0-240. Change with mouse wheel.");

			_pal = new _Palette(this);
			p1.Children.Add(_pal);

			base.Content = p1;
		}

		class _Palette : HwndHost
		{
			const string c_winClassName = "KColorPicker";
			KColorPicker _cp;
			AWnd _w;
			int _dpi;
			int _cellSize;
			const int c_nHue = 30; //number of hue columns. Must be 240-divisible.
			const int c_nLum = 7; //number of luminance rows. Must be 240-divisible minus 1.

			public AWnd Hwnd => _w;

			static _Palette() {
				AWnd.More.RegisterWindowClass(c_winClassName);
			}

			public _Palette(KColorPicker cp) {
				_cp = cp;
			}

			protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
				var wParent = (AWnd)hwndParent.Handle;
				AWnd.More.CreateWindow(_WndProc, c_winClassName, null, WS.CHILD | WS.CLIPCHILDREN, 0, 0, 0, 10, 10, wParent);

				return new HandleRef(this, _w.Handle);
			}

			protected override void DestroyWindowCore(HandleRef hwnd) {
				Api.DestroyWindow(_w);
			}

			protected override Size MeasureOverride(Size constraint) => _Measure();

			LPARAM _WndProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam) {
				//var pmo = new PrintMsgOptions(Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_NCMOUSEMOVE, 0x10c1);
				//if (AWnd.More.PrintMsg(out string s, _w, msg, wParam, lParam, pmo)) AOutput.Write("<><c green>" + s + "<>");

				switch (msg) {
				case Api.WM_NCCREATE:
					_w = w;
					BufferedPaint_.Init();
					break;
				case Api.WM_NCDESTROY:
					_w = default;
					BufferedPaint_.Uninit();
					break;
				//			case Api.WM_NCHITTEST: //SHOULDDO: test in Popup, probably click closes. Currently not using in popups.
				//				return Api.HTTRANSPARENT;
				case Api.WM_LBUTTONDOWN:
					_WmLbuttondown(lParam);
					break;
				case Api.WM_PAINT:
					using (var bp = new BufferedPaint_(w, true)) _Paint(bp.DC);
					return default;
				}

				return Api.DefWindowProc(w, msg, wParam, lParam);
			}

			Size _Measure() {
				_dpi = ADpi.OfWindow(_w);
				int i = _cellSize = ADpi.Scale(10, _dpi);
				var z = new SIZE(i, i);
				z.width *= c_nHue; z.width++;
				z.height *= c_nLum + 1; z.height++; z.height += _cellSize / 4;
				return ADpi.Unscale(z, _dpi);
			}

			void _Paint(nint dc) {
				using var g = System.Drawing.Graphics.FromHdc(dc);
				using var penSel = new System.Drawing.Pen(System.Drawing.Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
				g.Clear(System.Drawing.Color.White);

				var z = _cellSize;

				//colors. Hue in x axis, luminance in y axis.
				int y = 0;
				for (int i = 0, lum = 240; (lum -= 240 / (c_nLum + 1)) > 0; i++) {
					for (int j = 0, hue = 0; hue < 240; j++, hue += 240 / c_nHue) {
						//					var col=_ColorHLSToRGB(hue, lum, 240);

						int lum2 = lum;
						if (lum >= 120) {
							int d = Math.Abs(80 - hue); //diff from green
							if (d <= 64) {
								d /= 4;
								//							if(lum==120) AOutput.Write(Math.Max(d, 10), (240-lum)/Math.Max(d, 10));
								lum2 -= (240 - lum) / Math.Max(d, 8);
							}
						}
						var col = _ColorHLSToRGB(hue, lum2, 240);

						_ac[j, i] = col;

						//					if(lum==120 /*&& hue<=180*/) {
						//						ColorInt k=ColorInt.FromBGR(col, true);
						//						var b=k.GetPerceivedBrightness();
						//						AOutput.Write(col.ToString("X6"), hue, lum, b);
						//					}

						_Draw(col, j * z);
					}
					y += z;
				}

				//black...white
				y += _cellSize / 4;
				for (int j = 0, gray = 0; j < c_nHue; j++, gray += gray < 120 ? 10 : 8) {
					//				AOutput.Write(gray);
					if (gray > 255) gray = 255;
					var col = (gray << 16) | (gray << 8) | gray;
					_ac[j, c_nLum] = col;
					_Draw(col, j * z);
				}

				//selection
				y = _select.lum * z; if (_select.lum == c_nLum) y += _cellSize / 4;
				g.DrawRectangle(penSel, _select.hue * z, y, z, z);

				void _Draw(int col, int x) {
					RECT r = (x + 1, y + 1, z - 1, z - 1);
					var brush = Api.CreateSolidBrush(col);
					Api.FillRect(dc, r, brush);
					Api.DeleteObject(brush);
				}
			}

			int[,] _ac = new int[c_nHue, c_nLum + 1];

			(int hue, int lum) _select = (0, c_nLum);

			//		public int SelectedColor => _ac[_select.hue, _select.lum];

			//		public bool SelectedGray => _select.lum==c_nLum;

			public void SelectColor(int col) {
				//			AOutput.Write((uint)col);
				var (H, L, S) = _ColorRGBToHLS(col);
				if (S == 0) { //gray
					int lum = L * 255 / 240;
					for (int j = 0; j < c_nHue; j++) {
						int gray = _ac[j, c_nLum] & 255;
						if (gray >= lum) { _select = (j, c_nLum); break; }
					}
				} else {
					const int nLum = c_nLum + 1, hueStep = 240 / c_nHue, lumStep = 240 / nLum;
					double dif = 1000000;
					for (int hue = 0; hue < 240; hue += hueStep) {
						for (int lum = 240; (lum -= lumStep) > 0;) {
							int dH = hue - H, dL = lum - L;
							double d = Math.Sqrt(dH * dH + dL * dL);
							if (d < dif) { dif = d; _select = (hue / hueStep, c_nLum - lum / lumStep); }
						}
					}
				}
				Invalidate();
			}

			void _WmLbuttondown(LPARAM lp) {
				int x = AMath.LoShort(lp), y = AMath.HiShort(lp);
				x--; x /= _cellSize;
				y--; y /= _cellSize;
				x = Math.Clamp(x, 0, c_nHue - 1);
				y = Math.Clamp(y, 0, c_nLum);
				_select = (x, y);
				_cp._palSelecting = true;
				int col = _ac[x, y];
				_cp._SetColor(col, bgr: true);
				_cp._palSelecting = false;
				Invalidate();
			}

			public unsafe void Invalidate() {
				var w = Hwnd;
				if (w.IsVisible) Api.InvalidateRect(w, null, false);
			}
		}

		static int _ColorHLSToRGB(int H, int L, int S) {
			if (S == 0) { //ColorHLSToRGB bug: returns 0 if S 0
				int i = L * 255 / 240;
				return i | (i << 8) | (i << 16);
			}
			return KApi.ColorHLSToRGB((ushort)H, (ushort)L, (ushort)S);
		}

		static (int H, int L, int S) _ColorRGBToHLS(int col) {
			KApi.ColorRGBToHLS(col, out var H, out var L, out var S);
			return (H, L, S);
		}

		void _SetColor(int color, bool bgr) {
			if (bgr) color = ColorInt.SwapRB(color);
			var s = "0x" + color.ToStringInvariant("X6");
			_tColor.Text = s;
		}

		int _GetColor(bool bgr) {
			int col = _tColor.Text.ToInt();
			if (bgr) col = ColorInt.SwapRB(col);
			return col;
		}

		/// <summary>
		/// With public functions and events use color format 0xBBGGRR. If false (default), uses 0xRRGGBB.
		/// </summary>
		public bool BGR { get; set; }

		public int Color {
			get => _GetColor(BGR);
			set => _SetColor(value, BGR);
		}

		public event Action<int> ColorChanged;
	}
}