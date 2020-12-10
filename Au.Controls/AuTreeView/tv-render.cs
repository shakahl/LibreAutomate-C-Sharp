using Au;
using Au.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Au.Util;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace Au.Controls
{
	unsafe partial class AuTreeView
	{
		//Called on resize, scroll, set visible items (replace, clear, expand/collapse, add/remove/move).
		void _Measure(bool onScroll = false) {
			EndEditLabel();
			_labeltip?.Hide();

			double width = _grid.ActualWidth * _dpi / 96, height = _grid.ActualHeight * _dpi / 96;
			if (width < 1 || height < 1 || _avi.NE_()) {
				_scrollTopIndex = 0;
				_HideSB(_hscroll);
				_HideSB(_vscroll);
				return;
			}
			var range = _GetViewRange(onScroll ? _height : (int)Math.Ceiling(height));

			//AOutput.Write("_Measure", range.from, range.to, _scrollTopIndex);
			int maxWidth = _itemsW;
			GdiTextRenderer tr = null;
			for (int i = range.from; i < range.to; i++) {
				if (_avi[i].measured > 0) continue;
				tr ??= new GdiTextRenderer(_dpi);
				bool bold = _avi[i].item.IsBold; if (bold) tr.FontBold();
				var z = tr.MeasureText(_avi[i].item.DisplayText);
				if (bold) tr.FontNormal();
				int wid = ++z.width + _imageSize * (_avi[i].level + 1) + _imageMarginX * 2 + _marginLeft + _marginRight; if (HasCheckboxes) wid += _itemH;
				if (wid > maxWidth) maxWidth = wid;
				_avi[i].measured = (ushort)Math.Clamp(z.width, 1, ushort.MaxValue);
			}
			tr?.Dispose();
			if (maxWidth > _itemsW) _itemsW = maxWidth; else if (onScroll) return;

			//set scrollbars
			double sb = _vscroll.Width * _dpi / 96;
			double itemsH = _avi.Length * _itemH;
			bool needH = _itemsW > width && height >= _imageSize + sb; if (needH) height -= sb;
			bool needV = itemsH > height && _avi.Length > 1 && width > sb; if (needV) { width -= sb; if (!needH) if (needH = _itemsW > width && height >= _imageSize + sb) height -= sb; }
			//AOutput.Write(needH, needV);
			if (needH) {
				_hscroll.Visibility = Visibility.Visible;
				_hscroll.ViewportSize = width;
				_hscroll.LargeChange = width;
				_hscroll.SmallChange = _imageSize;
				_hscroll.Maximum = _itemsW - width;
			} else _HideSB(_hscroll);
			if (needV) {
				_vscroll.Visibility = Visibility.Visible;
				int u = (int)height / _itemH;
				_vscroll.ViewportSize = u;
				_vscroll.LargeChange = u;
				_vscroll.Maximum = _avi.Length - u;
			} else {
				_scrollTopIndex = 0;
				_HideSB(_vscroll);
			}

			static void _HideSB(ScrollBar sb) {
				if (!sb.IsVisible) return;
				sb.Visibility = Visibility.Collapsed;
				sb.Value = 0;
			}
		}

		void _MeasureClear(bool updateNow) {
			if (_grid == null) return;
			if (_avi != null) for (int i = 0; i < _avi.Length; i++) _avi[i].measured = 0;
			_itemsW = 0;
			if (updateNow) {
				_Measure();
				_Invalidate();
			}
		}

		(int from, int to) _GetViewRange(int height) {
			int len = _avi.Lenn_(); if (len == 0) return default;
			int max = (height + _itemH - 1) / _itemH, to = Math.Min(_scrollTopIndex + max, len + 1), from = Math.Max(0, to - max);
			if (_scrollTopIndex > from) _scrollTopIndex = from;
			return (_scrollTopIndex, Math.Min(to, len));
		}

		(int from, int to) _GetViewRange() => _GetViewRange(_height);

		void _Invalidate(RECT* r = null) {
			if(_hh!=null) Api.InvalidateRect(_hh.Hwnd, r, false);
		}

		void _Invalidate(int index) {
			if (_hh == null) return;
			var r = GetRectPhysical(index, clampX: true);
			if (r.bottom > 0 && r.top < _height) Api.InvalidateRect(_hh.Hwnd, &r, false);
		}

		/// <summary>
		/// Asynchronously redraws item.
		/// Does nothing if the control is not created.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="remeasure">Remeasure item width. My need this when changed text or text style.</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public void Redraw(int index, bool remeasure = false) {
			if (_grid == null) return;
			if (!_IsValid(index)) throw new IndexOutOfRangeException();
			if (remeasure) {
				//if this was the widest measured item, need to remeasure all, else would not update horz scrollbar if this item become narrower
				int max = 0; for (int i = 0; i < _avi.Length; i++) max = Math.Max(max, _avi[i].measured);
				if (max == _avi[index].measured) {
					_MeasureClear(updateNow: true);
					return;
				}
				_avi[index].measured = 0;
				_Measure();
			}
			_Invalidate(index);
		}

		/// <summary>
		/// Asynchronously redraws item.
		/// Does nothing if the control is not created or <i>item</i> is not a visible item in this control.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remeasure">Remeasure item width. My need this when changed text or text style.</param>
		public void Redraw(ITreeViewItem item, bool remeasure = false) {
			if (_grid == null) return;
			int i = IndexOf(item);
			if (i >= 0) Redraw(i, remeasure);
		}

		/// <summary>
		/// Asynchronously redraws all items.
		/// Does nothing if the control is not created.
		/// </summary>
		/// <param name="remeasure">Remeasure item width. My need this when changed text or text style.</param>
		public void Redraw(bool remeasure = false) {
			if (_grid == null) return;
			if(remeasure) _MeasureClear(updateNow: true);
			else _Invalidate();
		}

		//	
		//	/// <summary>
		//	/// Remeasure item widths and redraws. My need this when changed text or text style.
		//	/// Does nothing if the control is not created.
		//	/// </summary>
		//	/// <param name="indices"></param>
		//	/// <exception cref="ArgumentOutOfRangeException"></exception>
		//	public void Remeasure(Range indices) {
		//		if(_grid==null) return;
		//		var (i, to)=indices.GetStartEnd(_avi.Lenn_());
		//		while(i<to) _avi[i++].measured=0;
		//		_Measure();
		//		_Invalidate();
		//	}

		void _Render(IntPtr dc, RECT rect) {
			//		using var p1 = APerf.Create();
			//		AOutput.Write("_Render");
			//never mind: should draw only invalidated part. With async images redraws several times at startup. But then 2-3 ms, while first time 20-30 ms.
			Api.FillRect(dc, rect, (IntPtr)(Api.COLOR_WINDOW + 1));
			if (_avi.NE_()) return;

			var range = _GetViewRange();
			int nDraw = range.to - range.from;
			if (nDraw > 0) {
				int backColor = Api.GetSysColor(Api.COLOR_WINDOW), textColor = Api.GetSysColor(Api.COLOR_WINDOWTEXT);
				bool isFocusedControl = this.IsKeyboardFocused;
				int xLefts = -_scrollX; if (HasCheckboxes) xLefts += _itemH;
				int xImages = xLefts + _imageMarginX + _marginLeft;
				int yyImages = (_itemH + 1 - _imageSize) / 2, yyText = _dpi == 96 ? 1 : (_dpi >= 144 ? -1 : 0);

				var graphics = System.Drawing.Graphics.FromHdc(dc);
				var tr = new GdiTextRenderer(dc, _dpi);
				IntPtr checkTheme = HasCheckboxes ? api2.OpenThemeData(_hh.Hwnd, "Button") : default;
				try {
					graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					SIZE cSize = default; if (HasCheckboxes) if (checkTheme == default || 0 != api2.GetThemePartSize(checkTheme, dc, 3, 1, null, api2.THEMESIZE.TS_TRUE, out cSize)) cSize.width = cSize.height = ADpi.Scale(13, _dpi);

					var cd = CustomDraw;
					var cdi = cd == null ? null : new TVDrawInfo(this, dc, graphics, _dpi) {
						isFocusedControl = isFocusedControl,
						marginLeft = _marginLeft,
						marginRight = _marginRight,
						checkSize = cSize
					};
					cd?.Begin(cdi, tr);

					//		p1.Next();
					for (int i = 0; i < nDraw; i++) {
						int index = i + range.from;
						var v = _avi[index];
						var item = v.item;
						int y = i * _itemH;
						var r = new RECT(0, y, _width, _itemH);
						int indent = _imageSize * v.level, xLeft = indent + xLefts;
						int xImage = indent + xImages, yImage = y + yyImages;
						int xText = xImage + _imageSize + _imageMarginX, yText = y + yyText;

						if (cdi != null) {
							cdi.index = index;
							cdi.item = item;
							cdi.rect = r;
							cdi.imageRect = new RECT(xImage, yImage, _imageSize, _imageSize);
							cdi.xText = xText;
							cdi.yText = yText;
							cdi.xLeft = xLeft;
							cdi.xRight = xText + v.measured;
							cdi.isFocusedItem = index == _focusedIndex;
							cdi.isHot = index == _hotIndex;
							cdi.isSelected = v.isSelected;
						}

						//background
						if (cd == null || !cd.DrawBackground()) {
							int color = item.Color;
							if (color != -1 || backColor == 0xffffff) { //custom or white
								if (color == -1) {
									if (v.isSelected) color = isFocusedControl ? 0xffd5c4 : 0xe0e0e0;
									else color = (HotTrack && index == _hotIndex) ? 0xfff0e8 : backColor;
								}
								if (color != backColor) {
									var brush = Api.CreateSolidBrush(color);
									Api.FillRect(dc, r, brush);
									Api.DeleteObject(brush);
								}
							} else { //probably high contrast
								if (v.isSelected) Api.FillRect(dc, r, (IntPtr)(Api.COLOR_HIGHLIGHT + 1));
							}
						}

						//checkboxes
						if (HasCheckboxes && item.CheckState != TVCheck.None) {
							if (cd == null || !cd.DrawCheckbox()) {
								//							if(1==(i&3)) item.CheckState=TVCheck.Checked; if(2==(i&3)) item.CheckState=TVCheck.Mixed; if(3==(i&3)) item.CheckState=TVCheck.Excluded; if(0!=(i&4)) v.IsDisabled=true;
								int k = (_itemH - cSize.height) / 2;
								var rr = new RECT(xLeft - _itemH + k, y + k, cSize.width, cSize.height);
								var ch = item.CheckState;
								if (checkTheme != default) {
									int state = ch switch { TVCheck.Checked => 5, TVCheck.RadioChecked => 5, TVCheck.Mixed => 9, TVCheck.Excluded => 17, _ => 1 }; //CBS_x,RBS_x
									if (item.IsDisabled) state += 3; else if (index == _hotIndex) state += 1;
									api2.DrawThemeBackground(checkTheme, dc, (ch == TVCheck.RadioChecked || ch == TVCheck.RadioUnchecked) ? 2 : 3, state, rr); //BP_RADIOBUTTON,BP_CHECKBOX
								} else if (ch != TVCheck.Excluded) {
									int state = ch switch { TVCheck.Checked => 0x400, TVCheck.Mixed => 0x408, TVCheck.RadioUnchecked => 0x4, TVCheck.RadioChecked => 0x404, _ => 0 }; //DFCS_x
									if (item.IsDisabled) state |= 0x100; else if (index == _hotIndex) state |= 0x1000;
									api2.DrawFrameControl(dc, rr, 4, state); //DFC_BUTTON
								}
								//cannot use .NET CheckBoxRenderer etc because no per-monitor DPI.
							}
						}

						//image
						var b = item.Image;
						if (b == null) {
							var imageSource = item.ImageSource;
							if (!imageSource.NE()) {
								bool isImage = AResources.HasResourcePrefix(imageSource) || 0 != imageSource.Starts(false, "imagefile:", "image:", "~:");
								bool mayNeedAsync = !isImage;
								if (mayNeedAsync) _imageAsyncCompletion ??= _ImageAsyncCompletion;
								b = ImageCache.Get(imageSource, _dpi, isImage, mayNeedAsync ? _imageAsyncCompletion : null, item);
							}
						}
						if (b != null) {
							if (cd == null || !cd.DrawImage(b)) {
								graphics.DrawImage(b, new System.Drawing.Rectangle(xImage, yImage, _imageSize, _imageSize));
							}
						}

						//text
						if (cd == null || !cd.DrawText()) {
							bool bold = item.IsBold; if (bold) tr.FontBold();
							int color = item.TextColor; if (color == -1) color = item.IsDisabled ? Api.GetSysColor(Api.COLOR_GRAYTEXT) : textColor;
							tr.DrawText(item.DisplayText, (xText, yText), color);
							if (bold) tr.FontNormal();
						}

						if (cd != null) {
							cd.DrawMarginLeft();
							cd.DrawMarginRight();
						}

						//drag & drop insertion mark
						if (_dd != null && _dd.insertIndex == index) {
							int thick = ADpi.Scale(3, _dpi);
							using var pen = new System.Drawing.Pen(System.Drawing.SystemColors.WindowText, thick);
							y += thick / 2; int h1 = _itemH - thick;
							if (_dd.insertFolder) {
								graphics.DrawRectangle(pen, xImage, y, _imageSize, h1);
							} else {
								if (_dd.insertAfter) y += h1;
								graphics.DrawLine(pen, indent, y, _width, y);
							}
						}
					}
					//		p1.Next();

					cd?.End();
				}
				finally {
					graphics.Dispose();
					tr.Dispose();
					if (checkTheme != default) api2.CloseThemeData(checkTheme);
				}
			}
		}

		Action<System.Drawing.Bitmap, object> _imageAsyncCompletion;
		void _ImageAsyncCompletion(System.Drawing.Bitmap b, object o) => Redraw(o as ITreeViewItem);

		/// <summary>
		/// Gets or sets image cache.
		/// For example you can use single cache for all controls.
		/// If not set, the 'get' function auto-creates new instance when called first time.
		/// </summary>
		public AIconImageCache ImageCache {
			get => _imageCache ??= new AIconImageCache();
			set { _imageCache = value; }
		}
		AIconImageCache _imageCache;

		/// <summary>
		/// Width of custom-draw area before image. For example for state images.
		/// Use WPF logical units, not physical pixels.
		/// </summary>
		public int ItemMarginLeft {
			get => _itemMarginLeft_;
			set {
				if (value != _itemMarginLeft_) {
					_itemMarginLeft_ = value;
					_marginLeft = _DpiScale(value);
					_MeasureClear(true);
				}
			}
		}
		int _itemMarginLeft_;

		/// <summary>
		/// Width of custom-draw area after item text.
		/// Use WPF logical units, not physical pixels.
		/// </summary>
		public int ItemMarginRight {
			get => _itemMarginRight_;
			set {
				if (value != _itemMarginRight_) {
					_itemMarginRight_ = value;
					_marginRight = _DpiScale(value);
					_MeasureClear(true);
				}
			}
		}
		int _itemMarginRight_;

		/// <summary>
		/// Custom-draw interface.
		/// </summary>
		public ITVCustomDraw CustomDraw {
			get => _customDraw_;
			set {
				if (value != _customDraw_) {
					_customDraw_ = value;
					if (_grid != null) _Invalidate();
				}
			}
		}
		ITVCustomDraw _customDraw_;
#pragma warning restore 1591
	}
}
