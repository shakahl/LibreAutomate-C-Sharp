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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
//using System.Linq;

namespace Au
{
	public partial class AWinImage
	{
		#region capture, etc

		/// <summary>
		/// Creates image from a rectangle of screen pixels.
		/// </summary>
		/// <param name="rect">A rectangle in screen coordinates.</param>
		/// <exception cref="ArgumentException">Empty rectangle.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of this size (with*height*4 bytes).</exception>
		/// <remarks>
		/// PixelFormat is always Format32bppRgb.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var file = AFolders.Temp + "notepad.png";
		/// AWnd w = AWnd.Find("* Notepad");
		/// w.Activate();
		/// using(var b = AWinImage.Capture(w.Rect)) { b.Save(file); }
		/// AFile.Run(file);
		/// ]]></code>
		/// </example>
		public static Bitmap Capture(RECT rect) {
			return _Capture(rect);
		}

		/// <summary>
		/// Creates image from a rectangle of window client area pixels.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="rect">A rectangle in w client area coordinates. Use <c>w.ClientRect</c> to get whole client area.</param>
		/// <param name="usePrintWindow">Use flag <see cref="WICFlags.PrintWindow"/>.</param>
		/// <exception cref="AuWndException">Invalid w.</exception>
		/// <exception cref="ArgumentException">Empty rectangle.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of this size (with*height*4 bytes).</exception>
		/// <remarks>
		/// How this is different from <see cref="Capture(RECT)"/>:
		/// 1. Gets pixels from window's device context (DC), not from screen DC, unless the Aero theme is turned off (on Windows 7). The window can be under other windows. The image can be different.
		/// 2. If the window is partially or completely transparent, gets non-transparent image.
		/// 3. Does not work with Windows Store app windows, Chrome and some other windows. Creates black image.
		/// 4. If the window is DPI-scaled, captures its non-scaled view. And <i>rect</i> must contain non-scaled coordinates.
		/// </remarks>
		public static Bitmap Capture(AWnd w, RECT rect, bool usePrintWindow = false) {
			w.ThrowIfInvalid();
			return _Capture(rect, w, usePrintWindow);
		}

		static unsafe Bitmap _Capture(RECT r, AWnd w = default, bool usePrintWindow = false, GraphicsPath path = null) {
			//Transfer from screen/window DC to memory DC (does not work without this) and get pixels.

			//rejected: parameter includeNonClient (GetWindowDC).
			//	Nothing good. If in background, captures incorrect caption etc.
			//	If need nonclient part, better activate window and capture window rectangle from screen.

			//FUTURE: if w is DWM-scaled...

			using var mb = new AMemoryBitmap(r.Width, r.Height);
			if (usePrintWindow && Api.PrintWindow(w, mb.Hdc, Api.PW_CLIENTONLY | (AVersion.MinWin8_1 ? Api.PW_RENDERFULLCONTENT : 0))) {
				//AOutput.Write("PrintWindow OK");
			} else {
				using (var dc = new WindowDC_(w)) {
					if (dc.Is0) w.ThrowNoNative("Failed");
					uint rop = !w.Is0 ? Api.SRCCOPY : Api.SRCCOPY | Api.CAPTUREBLT;
					bool ok = Api.BitBlt(mb.Hdc, 0, 0, r.Width, r.Height, dc, r.left, r.top, rop);
					Debug.Assert(ok); //the API fails only if a HDC is invalid
				}
			}

			var R = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppRgb);
			try {
				var bh = new Api.BITMAPINFOHEADER() {
					biSize = sizeof(Api.BITMAPINFOHEADER),
					biWidth = r.Width,
					biHeight = -r.Height, //use -height for top-down
					biPlanes = 1,
					biBitCount = 32,
					//biCompression = 0, //BI_RGB
				};
				var d = R.LockBits(new Rectangle(0, 0, r.Width, r.Height), ImageLockMode.ReadWrite, R.PixelFormat); //tested: fast, no copy
				try {
					var apiResult = Api.GetDIBits(mb.Hdc, mb.Hbitmap, 0, r.Height, (void*)d.Scan0, &bh, 0); //DIB_RGB_COLORS
					if (apiResult != r.Height) throw new AuException("GetDIBits");
					_SetAlpha(d, r, path);
				}
				finally { R.UnlockBits(d); } //tested: fast, no copy
				return R;
			}
			catch { R.Dispose(); throw; }
		}

		static unsafe void _SetAlpha(BitmapData d, RECT r, GraphicsPath path = null) {
			//remove alpha. Will compress better.
			//APerf.First();
			byte* p = (byte*)d.Scan0, pe = p + r.Width * r.Height * 4;
			for (p += 3; p < pe; p += 4) *p = 0xff;
			//APerf.NW(); //1100 for max window

			//if path used, set alpha=0 for outer points
			if (path != null) {
				int* k = (int*)d.Scan0;
				for (int y = r.top; y < r.bottom; y++)
					for (int x = r.left; x < r.right; x++, k++) {
						//AOutput.Write(x, y, path.IsVisible(x, y));
						if (!path.IsVisible(x, y)) *k = 0xFFFFFF; //white, 0 alpha
					}
			}
		}

		static GraphicsPath _CreatePath(List<POINT> outline) {
			int n = outline.Count;
			Debug.Assert(n > 1);

			var p = new Point[n];
			for (int i = 0; i < n; i++) p[i] = outline[i];

			var t = new byte[n];
			//t[0] = (byte)PathPointType.Start; //0
			for (int i = 1; i < n; i++) t[i] = (byte)PathPointType.Line;
			t[n - 1] |= (byte)PathPointType.CloseSubpath;

			return new GraphicsPath(p, t);
		}

		static Bitmap _Capture(List<POINT> outline, AWnd w = default, bool usePrintWindow = false) {
			int n = outline?.Count ?? 0;
			if (n == 0) throw new ArgumentException();
			if (n == 1) return _Capture(new RECT(outline[0].x, outline[0].y, 1, 1));

			using var path = _CreatePath(outline);
			RECT r = RECT.From(path.GetBounds());
			if (r.NoArea) {
				path.Widen(Pens.Black); //will be transparent, but no exception. Difficult to make non-transparent line.
				r = RECT.From(path.GetBounds());
			}
			return _Capture(r, w, usePrintWindow, path);
		}

		/// <summary>
		/// Creates image from a non-rectangular area of screen pixels.
		/// </summary>
		/// <param name="outline">The outline (shape) of the area in screen. If single element, captures single pixel.</param>
		/// <exception cref="ArgumentException"><i>outline</i> is null or has 0 elements.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of this size.</exception>
		/// <remarks>
		/// PixelFormat is always Format32bppRgb.
		/// </remarks>
		public static Bitmap Capture(List<POINT> outline) {
			return _Capture(outline);
		}

		/// <summary>
		/// Creates image from a non-rectangular area of window client area pixels.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="outline">The outline (shape) of the area in w client area coordinates. If single element, captures single pixel.</param>
		/// <param name="usePrintWindow">Use flag <see cref="WICFlags.PrintWindow"/>.</param>
		/// <exception cref="AuWndException">Invalid <i>w</i>.</exception>
		/// <exception cref="ArgumentException"><i>outline</i> is null or has 0 elements.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of this size.</exception>
		/// <remarks>More info: <see cref="Capture(AWnd, RECT, bool)"/>.</remarks>
		public static Bitmap Capture(AWnd w, List<POINT> outline, bool usePrintWindow = false) {
			w.ThrowIfInvalid();
			return _Capture(outline, w, usePrintWindow);
		}

		#endregion

		#region misc

		/// <summary>
		/// Creates Bitmap from a GDI bitmap.
		/// </summary>
		/// <param name="hbitmap">GDI bitmap handle. This function makes its copy.</param>
		/// <remarks>
		/// How this function is different from <see cref="Image.FromHbitmap"/>:
		/// 1. Image.FromHbitmap usually creates bottom-up bitmap, which is incompatible with <see cref="Find"/>. This function creates normal top-down bitmap, like <c>new Bitmap(...)</c>, <c>Bitmap.FromFile(...)</c> etc do.
		/// 2. This function always creates bitmap of PixelFormat Format32bppRgb.
		/// </remarks>
		/// <exception cref="AuException">Failed. For example hbitmap is default(IntPtr).</exception>
		/// <exception cref="Exception">Exceptions of Bitmap(int, int, PixelFormat) constructor.</exception>
		public static unsafe Bitmap BitmapFromHbitmap(IntPtr hbitmap) {
			var bh = new Api.BITMAPINFOHEADER() { biSize = sizeof(Api.BITMAPINFOHEADER) };
			using (var dcs = new ScreenDC_()) {
				if (0 == Api.GetDIBits(dcs, hbitmap, 0, 0, null, &bh, 0)) goto ge;
				int wid = bh.biWidth, hei = bh.biHeight;
				if (hei > 0) bh.biHeight = -bh.biHeight; else hei = -hei;
				bh.biBitCount = 32;

				var R = new Bitmap(wid, hei, PixelFormat.Format32bppRgb);
				var d = R.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadWrite, R.PixelFormat);
				bool ok = hei == Api.GetDIBits(dcs, hbitmap, 0, hei, (void*)d.Scan0, &bh, 0);
				R.UnlockBits(d);
				if (!ok) { R.Dispose(); goto ge; }
				return R;
			}
			ge:
			throw new AuException();
		}

		#endregion

		#region capture UI

		/// <summary>
		/// Creates image from a user-selected area of screen pixels. Or gets single pixel color, or just rectangle.
		/// Returns false if cancelled.
		/// </summary>
		/// <param name="result">Receives results.</param>
		/// <param name="flags"></param>
		/// <param name="owner">Owner window. Temporarily hides it and its owner windows.</param>
		/// <remarks>
		/// Gets all screen pixels and shows in a full-screen topmost window, where the user can select an area.
		/// </remarks>
		public static bool CaptureUI(out WICResult result, WICFlags flags = 0, AnyWnd owner = default) {
			result = default;

			switch (flags & (WICFlags.Image | WICFlags.Color | WICFlags.Rectangle)) {
			case 0: case WICFlags.Image: case WICFlags.Color: case WICFlags.Rectangle: break;
			default: throw new ArgumentException();
			}

			List<AWnd> aw = null; AWnd wTool = default;
			try {
				if (!owner.IsEmpty) {
					wTool = owner.Hwnd;
					aw = wTool.Get.Owners(andThisWindow: true, onlyVisible: true);
					foreach (var w in aw) w.ShowL(false);
					using (new AInputBlocker(BIEvents.MouseClicks)) ATime.SleepDoEvents(300); //time for animations
				}

				g1:
				RECT rs = AScreen.VirtualScreen;
				//RECT rs = AScreen.Primary.Rect; //for testing, to see Write output in other screen
				Bitmap bs;
				bool windowPixels = flags.HasAny(WICFlags.WindowDC | WICFlags.PrintWindow);
				if (windowPixels) {
					if (!_WaitForHotkey("Press F3 to select window from mouse pointer. Or Esc.")) return false;
					var w = AWnd.FromMouse(WXYFlags.NeedWindow);
					var rc = w.ClientRect;
					using var bw = Capture(w, rc, flags.Has(WICFlags.PrintWindow));
					bs = new Bitmap(rs.Width, rs.Height);
					using var g = Graphics.FromImage(bs);
					g.Clear(Color.Black);
					w.MapClientToScreen(ref rc);
					g.DrawImage(bw, rc.left - rs.left, rc.top - rs.top);
				} else {
					bs = Capture(rs);
				}

				var cw = new _CapturingWindow();
				switch (cw.Show(bs, flags, rs)) {
				case 1: break;
				case 2:
					if (!windowPixels && !_WaitForHotkey("Press F3 when ready for new screenshot. Or Esc.")) return false;
					goto g1;
				default: return false;
				}

				var r = cw.Result;
				r.wnd = _WindowFromRect(r);
				result = r;
			}
			finally {
				if (aw != null) {
					foreach (var w in aw) w.ShowL(true);
					if (wTool.IsAlive) {
						wTool.ShowNotMinimized();
						wTool.ActivateL();
					}
				}
			}
			return true;

			static bool _WaitForHotkey(string info) {
				using (AOsd.ShowText(info, Timeout.Infinite)) {
					//try { AKeys.WaitForHotkey(0, KKey.F3); }
					//catch(AuException) { ADialog.ShowError("Failed to register hotkey F3"); return false; }

					return KKey.F3 == AKeys.WaitForKeys(0, k => !k.IsUp && k.Key is KKey.F3 or KKey.Escape, block: true);
				}
			}

			static AWnd _WindowFromRect(WICResult r) {
				Thread.Sleep(25); //after the window is closed, sometimes need several ms until OS sets correct Z order. Until that may get different w1 and w2.
				AWnd w1 = AWnd.FromXY((r.rect.left, r.rect.top));
				AWnd w2 = (r.image == null) ? w1 : AWnd.FromXY((r.rect.right - 1, r.rect.bottom - 1));
				if (w2 != w1 || !_IsInClientArea(w1)) {
					AWnd w3 = w1.Window, w4 = w2.Window;
					w1 = (w4 == w3 && _IsInClientArea(w3)) ? w3 : default;
				}
				return w1;

				bool _IsInClientArea(AWnd w) => w.GetClientRect(out var rc, true) && rc.Contains(r.rect);
			}
		}

		class _CapturingWindow
		{
			AWnd _w;
			Bitmap _img;
			bool _paintedOnce;
			bool _magnMoved;
			bool _capturing;
			WICFlags _flags;
			ACursor _cursor;
			SIZE _textSize;
			int _dpi;
			int _res;

			public WICResult Result;

			/// <returns>0 Cancel, 1 OK, 2 Retry.</returns>
			public int Show(Bitmap img, WICFlags flags, RECT r) {
				_img = img;
				_flags = flags;
				_cursor = ACursor.Load(AResources.GetBytes("<Au>resources/red_cross_cursor.cur"), 32);
				_dpi = AScreen.Primary.Dpi;
				_w = AWnd.More.CreateWindow(_WndProc, true, "#32770", "Au.AWinImage.CaptureUI", WS.POPUP | WS.VISIBLE, WSE.TOOLWINDOW | WSE.TOPMOST, r.left, r.top, r.Width, r.Height);
				_w.ActivateL();

				try {
					while (Api.GetMessage(out var m) > 0 && m.message != Api.WM_APP) {
						switch (m.message) {
						case Api.WM_KEYDOWN when !_capturing:
							switch ((KKey)(int)m.wParam) {
							case KKey.Escape: return 0;
							case KKey.F3: return 2;
							}
							break;
						case Api.WM_RBUTTONUP when m.hwnd == _w:
							switch (AMenu.ShowSimple("1 Retry\tF3|2 Cancel\tEsc", owner: _w)) {
							case 1: return 2;
							case 2: return 0;
							}
							break;
						}
						Api.DispatchMessage(m);
					}
				}
				finally {
					var w = _w; _w = default;
					Api.DestroyWindow(w);
				}
				return _res;
			}

			nint _WndProc(AWnd w, int msg, nint wParam, nint lParam) {
				//AWnd.More.PrintMsg(w, msg, wParam, lParam);

				switch (msg) {
				case Api.WM_NCDESTROY:
					_img.Dispose();
					_cursor?.Dispose();
					if (_w != default) {
						_w = default;
						_w.Post(Api.WM_APP);
					}
					break;
				case Api.WM_SETCURSOR:
					Api.SetCursor(_cursor.Handle);
					return 1;
				case Api.WM_ERASEBKGND:
					return default;
				case Api.WM_PAINT:
					var dc = Api.BeginPaint(w, out var ps);
					_WmPaint(dc);
					Api.EndPaint(w, ps);
					return default;
				case Api.WM_MOUSEMOVE:
					_WmMousemove(AMath.NintToPOINT(lParam));
					break;
				case Api.WM_LBUTTONDOWN:
					_WmLbuttondown(AMath.NintToPOINT(lParam));
					break;
				}

				return Api.DefWindowProc(w, msg, wParam, lParam);
			}

			void _WmPaint(IntPtr dc) {
				using var g = Graphics.FromHdc(dc);
				g.DrawImageUnscaled(_img, 0, 0);
				_paintedOnce = true;
			}

			void _WmMousemove(POINT pc) {
				if (!_paintedOnce) return;

				//format text to draw below magnifier
				string text;
				using (new StringBuilder_(out var s)) {
					var ic = _flags & (WICFlags.Image | WICFlags.Color | WICFlags.Rectangle);
					if (ic == 0) ic = WICFlags.Image | WICFlags.Color;
					bool canColor = ic.Has(WICFlags.Color);
					if (canColor) {
						var color = _img.GetPixel(pc.x, pc.y).ToArgb() & 0xffffff;
						s.Append("Color  0x").Append(color.ToString("X6")).Append('\n');
					}
					if (ic == WICFlags.Color) {
						s.Append("Click to capture color.\n");
					} else if (ic == WICFlags.Rectangle) {
						s.Append("Mouse-drag to capture rectangle.\n");
					} else {
						s.Append("How to capture\n");
						s.Append("  rectangle:  mouse-drag\n  any shape:  Shift+drag\n");
						if (canColor) s.Append("  color:  Ctrl+click\n");
					}
					s.Append("More:  right-click"); //"  cancel:  key Esc\n  retry:  key F3 ... F3"
					text = s.ToString();
				}

				var font = NativeFont_.RegularCached(_dpi);
				int magnWH = ADpi.Scale(200, _dpi) / 10 * 10; //width and height of the magnified image without borders etc
				if (_textSize == default) using (var tr = new FontDC_(font)) _textSize = tr.Measure(text, TFFlags.NOPREFIX);
				int width = Math.Max(magnWH, _textSize.width) + 2, height = magnWH + 4 + _textSize.height;
				using var mb = new AMemoryBitmap(width, height);
				var dc = mb.Hdc;
				using var wdc = new WindowDC_(_w);

				//draw frames and color background. Also erase magnifier, need when near screen edges.
				Api.FillRect(dc, (0, 0, width, height), Api.GetStockObject(4)); //BLACK_BRUSH

				//copy from captured screen image to magnifier image. Magnify 5 times.
				int k = magnWH / 10;
				Api.StretchBlt(dc, 1, 1, magnWH, magnWH, wdc, pc.x - k, pc.y - k, k * 2, k * 2, Api.SRCCOPY);

				//draw red crosshair
				k = magnWH / 2;
				using (var pen = new Pen_(0xff)) {
					pen.DrawLine(dc, (k, 1), (k, magnWH + 1));
					pen.DrawLine(dc, (1, k), (magnWH + 1, k));
				}

				//draw text below magnifier
				var rc = new RECT(1, magnWH + 2, _textSize.width, _textSize.height);
				Api.SetTextColor(dc, 0x32CD9A); //Color.YellowGreen
				Api.SetBkMode(dc, 1);
				var oldFont = Api.SelectObject(dc, font);
				Api.DrawText(dc, text, text.Length, ref rc, TFFlags.NOPREFIX);
				Api.SelectObject(dc, oldFont);

				//set magninifier position far from cursor
				var pm = new POINT(4, 4); _w.MapScreenToClient(ref pm);
				int xMove = magnWH * 3;
				if (_magnMoved) pm.Offset(xMove, 0);
				var rm = new RECT(pm.x, pm.y, width, height); rm.Inflate(magnWH / 2, magnWH / 2);
				if (rm.Contains(pc)) {
					Api.InvalidateRect(_w, (pm.x, pm.y, width, height));
					_magnMoved ^= true;
					pm.Offset(_magnMoved ? xMove : -xMove, 0);
				}

				Api.BitBlt(wdc, pm.x, pm.y, width, height, dc, 0, 0, Api.SRCCOPY);
			}

			void _WmLbuttondown(POINT p0) {
				bool isColor = false, isAnyShape = false;
				var ic = _flags & (WICFlags.Image | WICFlags.Color | WICFlags.Rectangle);
				if (ic == WICFlags.Color) {
					isColor = true;
				} else {
					var mod = AKeys.UI.GetMod();
					if (mod != 0 && ic == WICFlags.Rectangle) return;
					switch (mod) {
					case 0: break;
					case KMod.Shift: isAnyShape = true; break;
					case KMod.Ctrl when ic != WICFlags.Image: isColor = true; break;
					default: return;
					}
				}

				Result = new WICResult();
				var r = new RECT(p0.x, p0.y, 0, 0);
				if (isColor) {
					Result.color = _img.GetPixel(p0.x, p0.y).ToArgb();
					r.right++; r.bottom++;
				} else {
					var a = isAnyShape ? new List<POINT>() { p0 } : null;
					var pen = Pens.Red;
					bool notFirstMove = false;
					_capturing = true;
					try {
						if (!ADragDrop.SimpleDragDrop(_w, MButtons.Left, m => {
							if (m.Msg.message != Api.WM_MOUSEMOVE) return;
							POINT p = m.Msg.pt; _w.MapScreenToClient(ref p);
							using var g = Graphics.FromHwnd(_w.Handle);
							if (isAnyShape) {
								a.Add(p);
								g.DrawLine(pen, p0, p);
								p0 = p;
							} else {
								if (notFirstMove) { //erase prev rect
									r.right++; r.bottom++;
									g.DrawImage(_img, r, r, GraphicsUnit.Pixel);
									//FUTURE: prevent flickering. Also don't draw under magnifier.
								} else notFirstMove = true;
								r = RECT.FromLTRB(p0.x, p0.y, p.x, p.y);
								r.Normalize(true);
								g.DrawRectangle(pen, r);
							}
						})) { //Esc key etc
							Api.InvalidateRect(_w);
							return;
						}
					}
					finally { _capturing = false; }

					GraphicsPath path = null;
					if (isAnyShape && a.Count > 1) {
						path = _CreatePath(a);
						r = RECT.From(path.GetBounds());
					} else {
						r.right++; r.bottom++;
					}
					if (r.NoArea) {
						Api.DestroyWindow(_w);
						return;
					}

					if (ic != WICFlags.Rectangle) {
						var b = _img.Clone(r, PixelFormat.Format32bppArgb);
						var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
						try {
							_SetAlpha(d, r, path);
						}
						finally { b.UnlockBits(d); path?.Dispose(); }
						Result.image = b;
					}

				}
				_w.MapClientToScreen(ref r);
				Result.rect = r;

				_res = 1;
				Api.DestroyWindow(_w);
			}
		}

		#endregion
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AWinImage.CaptureUI"/>.
	/// </summary>
	/// <remarks>
	/// Only one of flags <b>Image</b>, <b>Color</b> and <b>Rectangle</b> can be used. If none, can capture image or color.
	/// </remarks>
	[Flags]
	public enum WICFlags
	{
		/// <summary>Can capture only image, not color.</summary>
		Image = 1,

		/// <summary>Can capture only color, not image.</summary>
		Color = 2,

		/// <summary>Capture only rectangle, not image/color.</summary>
		Rectangle = 4,

		/// <summary>
		/// Get pixels from the client area device context (DC) of a user-selected window, not from screen DC.
		/// More info: <see cref="WIFlags.WindowDC"/>.
		/// </summary>
		WindowDC = 8,

		/// <summary>
		/// Get pixels from the client area of a user-selected window using API <msdn>PrintWindow</msdn>.
		/// More info: <see cref="WIFlags.PrintWindow"/>.
		/// </summary>
		PrintWindow = 16,
	}

	/// <summary>
	/// Results of <see cref="AWinImage.CaptureUI"/>.
	/// </summary>
	public class WICResult
	{
		/// <summary>
		/// Captured image.
		/// null if captured single pixel color or used flag <see cref="WICFlags.Rectangle"/>.
		/// </summary>
		public Bitmap image;

		/// <summary>
		/// Captured color.
		/// </summary>
		public ColorInt color;

		/// <summary>
		/// Location of the captured image or rectangle, in screen coordinates.
		/// </summary>
		public RECT rect;

		/// <summary>
		/// Window or control containing the captured image or rectangle, if whole image is in its client area.
		/// In some cases may be incorrect, for example if windows moved/opened/closed/etc while capturing.
		/// </summary>
		public AWnd wnd;
	}
}
