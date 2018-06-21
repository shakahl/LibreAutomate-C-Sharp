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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	public partial class WinImage
	{
		#region capture, etc

		/// <summary>
		/// Copies a rectangle of screen pixels to a new Bitmap object.
		/// </summary>
		/// <param name="rect">A rectangle in screen coordinates.</param>
		/// <exception cref="ArgumentException">Empty rectangle.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of this size (with*height*4 bytes).</exception>
		/// <remarks>
		/// PixelFormat is always Format32bppRgb.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var file = Folders.Temp + "notepad.png";
		/// Wnd w = Wnd.Find("* Notepad");
		/// w.Activate();
		/// using(var b = WinImage.Capture(w.Rect)) { b.Save(file); }
		/// Shell.Run(file);
		/// ]]></code>
		/// </example>
		public static Bitmap Capture(RECT rect)
		{
			return _Capture(rect);
		}

		/// <summary>
		/// Copies a rectangle of window client area pixels to a new Bitmap object.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="rect">A rectangle in w client area coordinates. Use <c>w.ClientRect</c> to get whole client area.</param>
		/// <exception cref="WndException">Invalid w.</exception>
		/// <exception cref="ArgumentException">Empty rectangle.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of this size (with*height*4 bytes).</exception>
		/// <remarks>
		/// How this is different from <see cref="Capture(RECT)"/>:
		/// 1. Gets pixels from window's device context (DC), not from screen DC, unless the Aero theme is turned off (on Windows 7). The window can be under other windows. The image can be different.
		/// 2. If the window is partially or completely transparent, gets non-transparent image.
		/// 3. Does not work with Windows Store app windows, Chrome and some other windows. Creates black image.
		/// 4. If the window is DPI-scaled, captures its non-scaled view. And <paramref name="rect"/> must contain non-scaled coordinates.
		/// </remarks>
		public static Bitmap Capture(Wnd w, RECT rect)
		{
			w.ThrowIfInvalid();
			return _Capture(rect, w);
		}

		static unsafe Bitmap _Capture(RECT r, Wnd w = default, GraphicsPath path = null)
		{
			//Transfer from screen/window DC to memory DC (does not work without this) and get pixels.

			//rejected: parameter includeNonClient (GetWindowDC).
			//	Nothing good. If in background, captures incorrect caption etc.
			//	If need nonclient part, better activate window and capture window rectangle from screen.

			//FUTURE: if w is DWM-scaled...

			using(var mb = new Util.MemoryBitmap(r.Width, r.Height)) {
				using(var dc = new Util.LibWindowDC(w)) {
					if(dc.Is0 && !w.Is0) w.ThrowNoNative("Failed");
					uint rop = !w.Is0 ? Api.SRCCOPY : Api.SRCCOPY | Api.CAPTUREBLT;
					bool ok = Api.BitBlt(mb.Hdc, 0, 0, r.Width, r.Height, dc, r.left, r.top, rop);
					Debug.Assert(ok); //the API fails only if a HDC is invalid
				}
				var R = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppRgb);
				try {
					var bh = new Api.BITMAPINFOHEADER() {
						biSize = sizeof(Api.BITMAPINFOHEADER),
						biWidth = r.Width, biHeight = -r.Height, //use -height for top-down
						biPlanes = 1, biBitCount = 32,
						//biCompression = 0, //BI_RGB
					};
					var d = R.LockBits(new Rectangle(0, 0, r.Width, r.Height), ImageLockMode.ReadWrite, R.PixelFormat); //tested: fast, no copy
					try {
						var apiResult = Api.GetDIBits(mb.Hdc, mb.Hbitmap, 0, r.Height, (void*)d.Scan0, &bh, 0); //DIB_RGB_COLORS
						if(apiResult != r.Height) throw new AuException("GetDIBits");
						_SetAlpha(d, r, path);
					}
					finally { R.UnlockBits(d); } //tested: fast, no copy
					return R;
				}
				catch { R.Dispose(); throw; }
			}
		}

		static unsafe void _SetAlpha(BitmapData d, RECT r, GraphicsPath path = null)
		{
			//remove alpha. Will compress better.
			//Perf.First();
			byte* p = (byte*)d.Scan0, pe = p + r.Width * r.Height * 4;
			for(p += 3; p < pe; p += 4) *p = 0xff;
			//Perf.NW(); //1100 for max window

			//if path used, set alpha=0 for outer points
			if(path != null) {
				int* k = (int*)d.Scan0;
				for(int y = r.top; y < r.bottom; y++)
					for(int x = r.left; x < r.right; x++, k++) {
						//Print(x, y, path.IsVisible(x, y));
						if(!path.IsVisible(x, y)) *k = 0xFFFFFF; //white, 0 alpha
					}
			}
		}

		static GraphicsPath _CreatePath(List<POINT> outline)
		{
			int n = outline.Count;
			Debug.Assert(n > 1);

			var p = new Point[n];
			for(int i = 0; i < n; i++) p[i] = outline[i];

			var t = new byte[n];
			//t[0] = (byte)PathPointType.Start; //0
			for(int i = 1; i < n; i++) t[i] = (byte)PathPointType.Line;
			t[n - 1] |= (byte)PathPointType.CloseSubpath;

			return new GraphicsPath(p, t);
		}

		static Bitmap _Capture(List<POINT> outline, Wnd w = default)
		{
			int n = outline?.Count ?? 0;
			if(n == 0) throw new ArgumentException();
			if(n == 1) return _Capture((outline[0].x, outline[0].y, 1, 1));

			using(var path = _CreatePath(outline)) {
				RECT r = path.GetBounds();
				if(r.IsEmpty) {
					path.Widen(Pens.Black); //will be transparent, but no exception. Difficult to make non-transparent line.
					r = path.GetBounds();
				}
				return _Capture(r, w, path);
			}
		}

		public static Bitmap Capture(List<POINT> outline)
		{
			return _Capture(outline);
		}

		public static Bitmap Capture(Wnd w, List<POINT> outline)
		{
			w.ThrowIfInvalid();
			return _Capture(outline, w);
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
		public static unsafe Bitmap BitmapFromHbitmap(IntPtr hbitmap)
		{
			var bh = new Api.BITMAPINFOHEADER() { biSize = sizeof(Api.BITMAPINFOHEADER) };
			using(var dcs = new Util.LibScreenDC(0)) {
				if(0 == Api.GetDIBits(dcs, hbitmap, 0, 0, null, &bh, 0)) goto ge;
				int wid = bh.biWidth, hei = bh.biHeight;
				if(hei > 0) bh.biHeight = -bh.biHeight; else hei = -hei;
				bh.biBitCount = 32;

				var R = new Bitmap(wid, hei, PixelFormat.Format32bppRgb);
				var d = R.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadWrite, R.PixelFormat);
				bool ok = hei == Api.GetDIBits(dcs, hbitmap, 0, hei, (void*)d.Scan0, &bh, 0);
				R.UnlockBits(d);
				if(!ok) { R.Dispose(); goto ge; }
				return R;
			}
			ge:
			throw new AuException();
		}

		#endregion

		#region capture UI

		public static bool CaptureUI(out WICResult result, WICFlags flags = 0, AnyWnd toolWindow = default)
		{
			result = default;
			Wnd[] aw = null; Wnd wTool = default;
			try {
				if(!toolWindow.IsEmpty) {
					aw = Wnd.Misc.OwnerWindowsAndThis(wTool = toolWindow.Wnd, true);
					foreach(var w in aw) w.ShowLL(false);
					Time.SleepDoEvents(550);
				}

				g1:
				RECT rs = SystemInformation.VirtualScreen;
				//RECT rs = Screen.PrimaryScreen.Bounds;
				Bitmap bs;
				bool windowDC = flags.Has_(WICFlags.WindowDC);
				//windowDC = true;
				if(windowDC) {
					if(!_WaitForHotkey("Press F3 to select window")) return false;
					var w = Wnd.FromMouse(WXYFlags.NeedWindow);
					w.GetClientRect(out var rc, inScreen: true);
					using(var bw = Capture(w, w.ClientRect)) {
						bs = new Bitmap(rs.Width, rs.Height);
						using(var g = Graphics.FromImage(bs)) {
							g.Clear(Color.Black);
							g.DrawImage(bw, rc.left, rc.top);
						}
					}
				} else {
					bs = Capture(rs);
				}

				var f = new _Form(bs, flags);
				f.Bounds = rs;
				switch(f.ShowDialog()) {
				case DialogResult.OK: break;
				case DialogResult.Retry:
					if(!_WaitForHotkey("Press F3 when ready for new screenshot")) return false;
					goto g1;
				default: return false;
				}

				//window from rect
				var r = f.Result;
				Wnd w1 = Wnd.FromXY((r.rect.left, r.rect.top));
				Wnd w2 = (r.image == null) ? w1 : Wnd.FromXY((r.rect.right, r.rect.bottom));
				if(w2 != w1 || !_IsInClientArea(w1)) {
					Wnd w3 = w1.WndWindow, w4 = w2.WndWindow;
					w1 = (w4 == w3 && _IsInClientArea(w3)) ? w3 : default;
				}
				r.wnd = w1;
				bool _IsInClientArea(Wnd w) => w.GetClientRect(out var rc, true) && rc.Contains(r.rect);

				result = r;
			}
			finally {
				if(aw != null) {
					foreach(var w in aw) w.ShowLL(true);
					if(wTool.IsAlive) {
						wTool.ShowNotMinimized();
						wTool.ActivateLL();
					}
				}
			}
			return true;
		}
		
		static bool _WaitForHotkey(string info)
		{
			//if(1 != AuDialog.ShowInfo(info, buttons: "OK|Cancel")) return false;
			AuDialog.ShowEx(info, icon: DIcon.Info, secondsTimeout: 3);
			//FUTURE: OSD

			//try { Keyb.WaitForHotkey(0, KKey.F3); }
			//catch(AuException) { AuDialog.ShowError("Failed to register hotkey F3"); return false; }

			Keyb.WaitForKey(0, KKey.F3, up: true, block: true);
			return true;
		}

		class _Form :Form
		{
			Bitmap _img;
			Graphics _gMagn;
			Bitmap _bMagn;
			bool _paintedOnce;
			bool _magnMoved;
			bool _capturing;
			WICFlags _flags;
			Cursor _cursor;
			//POINT _p, _p0;

			public WICResult Result;

			public _Form(Bitmap img, WICFlags flags)
			{
				_img = img;
				_flags = flags;
				SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
				AutoScaleMode = AutoScaleMode.None;
				FormBorderStyle = FormBorderStyle.None; //important
				ShowInTaskbar = false; //optional
				TopLevel = true; //optional
				StartPosition = FormStartPosition.Manual;
				Text = "Au.WinImage.CaptureUI";
				Cursor = _cursor = Util.Cursors_.LoadCursorFromMemory(Properties.Resources.red_cross_cursor, 32);

				//Font = new Font("Tahoma", 16); //test
			}

			protected override CreateParams CreateParams
			{
				get
				{
					var p = base.CreateParams;
					p.Style = unchecked((int)(Native.WS.POPUP));
					p.ExStyle = (int)(Native.WS_EX.TOOLWINDOW | Native.WS_EX.TOPMOST);
					return p;
				}
			}

			protected override void OnFormClosed(FormClosedEventArgs e)
			{
				_img.Dispose();
				_bMagn?.Dispose();
				_gMagn?.Dispose();
				_clip?.Dispose();
				_cursor?.Dispose();
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				e.Graphics.DrawImageUnscaled(_img, 0, 0);
				_paintedOnce = true;
			}

			Size _textSize;
			Region _clip;

			protected override void OnMouseMove(MouseEventArgs e)
			{
				if(!_paintedOnce) return;
				using(var gr = this.CreateGraphics()) {
					var pc = e.Location; //cursor position

					//format text to draw below magnifier
					//const TextFormatFlags textFlags = TextFormatFlags.Left| TextFormatFlags.WordBreak;
					var color = _img.GetPixel(pc.X, pc.Y).ToArgb() & 0xffffff;
					string text = $@"Color  0x{color:X6}
How to capture
  rectangle: mouse drag
  any shape: Shift+drag
  color: Ctrl+click
  cancel: key Esc
  retry: key F3 ... F3";

					const int magnWH = 200; //width and height of the magnified image without borders etc

					var m = _gMagn;
					if(m == null) {
						_textSize = TextRenderer.MeasureText(gr, text, Font);
						_bMagn = new Bitmap(Math.Max(magnWH, _textSize.Width) + 2, magnWH + 4 + _textSize.Height);
						_gMagn = m = Graphics.FromImage(_bMagn);
						m.InterpolationMode = InterpolationMode.NearestNeighbor; //no interpolation
						m.PixelOffsetMode = PixelOffsetMode.Half; //no half-pixel offset
					}

					//draw frames and color background. Also erase magnifier, need when near screen edges.
					m.Clear(Color.Black);

					//copy from captured screen image to magnifier image. Magnify 5 times.
					int k = magnWH / 10;
					var rFrom = new Rectangle(pc.X - k, pc.Y - k, k * 2, k * 2);
					var rTo = new Rectangle(1, 1, magnWH, magnWH);
					m.DrawImage(_img, rTo, rFrom, GraphicsUnit.Pixel);
					//draw red crosshair
					var redPen = Pens.Red;
					k = magnWH / 2 + 4;
					m.SetClip(_clip ?? (_clip = new Region(new Rectangle(k - 4, k - 4, 7, 7))), CombineMode.Exclude);
					m.DrawLine(redPen, k, 1, k, magnWH + 1);
					m.DrawLine(redPen, 1, k, magnWH + 1, k);
					m.ResetClip();
					//draw text below magnifier
					var rc = new Rectangle(1, magnWH + 2, _textSize.Width, _textSize.Height);
					TextRenderer.DrawText(m, text, Font, rc, Color.YellowGreen, Color.Transparent, TextFormatFlags.Left);

					//set maginifier position far from cursor
					var pm = new Point(-Left + 4, -Top + 4);
					const int xMove = 600;
					if(_magnMoved) pm.Offset(xMove, 0);
					var rm = new Rectangle(pm.X, pm.Y, _bMagn.Width, _bMagn.Height); rm.Inflate(100, 100);
					if(rm.Contains(pc)) {
						rm = new Rectangle(pm.X, pm.Y, _bMagn.Width, _bMagn.Height);
						gr.DrawImage(_img, rm, rm, GraphicsUnit.Pixel);
						_magnMoved ^= true;
						pm.Offset(_magnMoved ? xMove : -xMove, 0);
					}

					//m -> gr
					gr.DrawImageUnscaled(_bMagn, pm);

					//_p = p;
				}
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				bool isColor = false, isAnyShape = false;
				switch(ModifierKeys) {
				case Keys.Control: isColor = true; break;
				case Keys.Shift: isAnyShape = true; break;
				case Keys.None: break;
				default: return;
				}

				Result = new WICResult();
				POINT p0 = e.Location;
				if(isColor) {
					Result.color = _img.GetPixel(p0.x, p0.y).ToArgb();
					Result.rect = (p0.x, p0.y, 1, 1);
				} else {
					RECT r = default;
					var a = isAnyShape ? new List<POINT>() { p0 } : null;
					var pen = Pens.Red;
					bool notFirstMove = false;
					_capturing = true;
					try {
						if(!Au.Util.DragDrop.SimpleDragDrop(this, MButtons.Left, m =>
						{
							if(m.Msg.message != Api.WM_MOUSEMOVE) return;
							POINT p = m.Msg.pt;
							p.x -= Left; p.y -= Top; //screen to client
							using(var g = this.CreateGraphics()) {
								if(isAnyShape) {
									a.Add(p);
									g.DrawLine(pen, p0, p);
									p0 = p;
								} else {
									if(notFirstMove) { //erase prev rect
										r.right++; r.bottom++;
										g.DrawImage(_img, r, r, GraphicsUnit.Pixel);
										//FUTURE: prevent flickering. Also don't draw under magnifier.
									} else notFirstMove = true;
									r = (p0.x, p0.y, p.x, p.y, false);
									r.Normalize(true);
									g.DrawRectangle(pen, r);
								}
							}
						})) { //Esc key etc
							this.Invalidate();
							return;
						}
					}
					finally { _capturing = false; }

					GraphicsPath path = null;
					if(isAnyShape && a.Count > 1) {
						path = _CreatePath(a);
						r = path.GetBounds();
					} else {
						r.right++; r.bottom++;
					}
					if(r.IsEmpty) {
						this.Close();
						return;
					}

					var b = _img.Clone(r, PixelFormat.Format32bppArgb);
					var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
					try {
						_SetAlpha(d, r, path);
					}
					finally { b.UnlockBits(d); path?.Dispose(); }
					Result.image = b;
					r.Offset(Left, Top); //client to screen
					Result.rect = r;
				}

				this.DialogResult = DialogResult.OK;
				this.Close();
			}

			protected override void OnKeyDown(KeyEventArgs e)
			{
				if(_capturing) return;
				switch(e.KeyCode) {
				case Keys.Escape:
					this.Close();
					break;
				case Keys.F3:
					this.DialogResult = DialogResult.Retry;
					this.Close();
					break;
				}
			}
		}

		#endregion
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="WinImage.CaptureUI"/>.
	/// </summary>
	[Flags]
	public enum WICFlags
	{
		/// <summary>Can capture only color, not image.</summary>
		Color = 1,

		/// <summary>Can capture only image, not color.</summary>
		Image = 2,

		///
		WindowDC = 4,

		//TODO
	}

	/// <summary>
	/// Results of <see cref="WinImage.CaptureUI"/>.
	/// </summary>
	public class WICResult
	{
		/// <summary>
		/// Captured image.
		/// </summary>
		public Bitmap image;

		/// <summary>
		/// Captured color.
		/// </summary>
		public ColorInt color;

		/// <summary>
		/// Rectangle of the captured image, in screen coordinates.
		/// </summary>
		public RECT rect;

		/// <summary>
		/// Window or control containing the captured image, if whole image is in its client area.
		/// In some cases may be incorrect, for example if windows moved/opened/closed/etc while capturing.
		/// </summary>
		public Wnd wnd;
	}
}
