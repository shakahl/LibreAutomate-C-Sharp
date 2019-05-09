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
		/// Creates image from a rectangle of screen pixels.
		/// </summary>
		/// <param name="rect">A rectangle in screen coordinates.</param>
		/// <exception cref="ArgumentException">Empty rectangle.</exception>
		/// <exception cref="AException">Failed. Probably there is not enough memory for bitmap of this size (with*height*4 bytes).</exception>
		/// <remarks>
		/// PixelFormat is always Format32bppRgb.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var file = Folders.Temp + "notepad.png";
		/// Wnd w = Wnd.Find("* Notepad");
		/// w.Activate();
		/// using(var b = WinImage.Capture(w.Rect)) { b.Save(file); }
		/// Exec.Run(file);
		/// ]]></code>
		/// </example>
		public static Bitmap Capture(RECT rect)
		{
			return _Capture(rect);
		}

		/// <summary>
		/// Creates image from a rectangle of window client area pixels.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="rect">A rectangle in w client area coordinates. Use <c>w.ClientRect</c> to get whole client area.</param>
		/// <exception cref="WndException">Invalid w.</exception>
		/// <exception cref="ArgumentException">Empty rectangle.</exception>
		/// <exception cref="AException">Failed. Probably there is not enough memory for bitmap of this size (with*height*4 bytes).</exception>
		/// <remarks>
		/// How this is different from <see cref="Capture(RECT)"/>:
		/// 1. Gets pixels from window's device context (DC), not from screen DC, unless the Aero theme is turned off (on Windows 7). The window can be under other windows. The image can be different.
		/// 2. If the window is partially or completely transparent, gets non-transparent image.
		/// 3. Does not work with Windows Store app windows, Chrome and some other windows. Creates black image.
		/// 4. If the window is DPI-scaled, captures its non-scaled view. And <i>rect</i> must contain non-scaled coordinates.
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

			using var mb = new Util.MemoryBitmap(r.Width, r.Height);
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
					if(apiResult != r.Height) throw new AException("GetDIBits");
					_SetAlpha(d, r, path);
				}
				finally { R.UnlockBits(d); } //tested: fast, no copy
				return R;
			}
			catch { R.Dispose(); throw; }
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

			using var path = _CreatePath(outline);
			RECT r = path.GetBounds();
			if(r.IsEmpty) {
				path.Widen(Pens.Black); //will be transparent, but no exception. Difficult to make non-transparent line.
				r = path.GetBounds();
			}
			return _Capture(r, w, path);
		}

		/// <summary>
		/// Creates image from a non-rectangular area of screen pixels.
		/// </summary>
		/// <param name="outline">The outline (shape) of the area in screen. If single element, captures single pixel.</param>
		/// <exception cref="ArgumentException"><i>outline</i> is null or has 0 elements.</exception>
		/// <exception cref="AException">Failed. Probably there is not enough memory for bitmap of this size.</exception>
		/// <remarks>
		/// PixelFormat is always Format32bppRgb.
		/// </remarks>
		public static Bitmap Capture(List<POINT> outline)
		{
			return _Capture(outline);
		}

		/// <summary>
		/// Creates image from a non-rectangular area of window client area pixels.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="outline">The outline (shape) of the area in w client area coordinates. If single element, captures single pixel.</param>
		/// <exception cref="WndException">Invalid <i>w</i>.</exception>
		/// <exception cref="ArgumentException"><i>outline</i> is null or has 0 elements.</exception>
		/// <exception cref="AException">Failed. Probably there is not enough memory for bitmap of this size.</exception>
		/// <remarks>More info: <see cref="Capture(Wnd, RECT)"/>.</remarks>
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
		/// <exception cref="AException">Failed. For example hbitmap is default(IntPtr).</exception>
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
			throw new AException();
		}

		#endregion

		#region capture UI

		/// <summary>
		/// Creates image from a user-selected area of screen pixels. Or gets single pixel color, or just rectangle.
		/// Returns false if cancelled.
		/// </summary>
		/// <param name="result">Receives results.</param>
		/// <param name="flags"></param>
		/// <param name="toolWindow">Owner window. Temporarily hides it and its owner windows.</param>
		/// <remarks>
		/// Gets all screen pixels and shows in a full-screen topmost window, where the user can select an area.
		/// </remarks>
		public static bool CaptureUI(out WICResult result, WICFlags flags = 0, AnyWnd toolWindow = default)
		{
			result = default;

			switch(flags & (WICFlags.Image | WICFlags.Color | WICFlags.Rectangle)) {
			case 0: case WICFlags.Image: case WICFlags.Color: case WICFlags.Rectangle: break;
			default: throw new ArgumentException();
			}

			Wnd[] aw = null; Wnd wTool = default;
			try {
				if(!toolWindow.IsEmpty) {
					wTool = toolWindow.Wnd;
					aw = wTool.Get.OwnersAndThis(true);
					foreach(var w in aw) w.ShowLL(false);
					using(new InputBlocker(BIEvents.MouseClicks)) Time.SleepDoEvents(300); //time for animations
				}

				g1:
				RECT rs = SystemInformation.VirtualScreen;
				//RECT rs = Screen.PrimaryScreen.Bounds; //for testing, to see Print output in other screen
				Bitmap bs;
				bool windowDC = flags.Has(WICFlags.WindowDC);
				if(windowDC) {
					if(!_WaitForHotkey("Press F3 to select window from mouse pointer.")) return false;
					var w = Wnd.FromMouse(WXYFlags.NeedWindow);
					w.GetClientRect(out var rc, inScreen: true);
					using var bw = Capture(w, w.ClientRect);
					bs = new Bitmap(rs.Width, rs.Height);
					using var g = Graphics.FromImage(bs);
					g.Clear(Color.Black);
					g.DrawImage(bw, rc.left, rc.top);
				} else {
					bs = Capture(rs);
				}

				var f = new _Form(bs, flags);
				f.Bounds = rs;
				switch(f.ShowDialog()) {
				case DialogResult.OK: break;
				case DialogResult.Retry:
					if(!windowDC && !_WaitForHotkey("Press F3 when ready for new screenshot.")) return false;
					goto g1;
				default: return false;
				}

				var r = f.Result;
				r.wnd = _WindowFromRect(r);
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

		static Wnd _WindowFromRect(WICResult r)
		{
			Thread.Sleep(25); //after the form is closed, sometimes need several ms until OS sets correct Z order. Until that may get different w1 and w2.
			Wnd w1 = Wnd.FromXY((r.rect.left, r.rect.top));
			Wnd w2 = (r.image == null) ? w1 : Wnd.FromXY((r.rect.right - 1, r.rect.bottom - 1));
			if(w2 != w1 || !_IsInClientArea(w1)) {
				Wnd w3 = w1.Window, w4 = w2.Window;
				w1 = (w4 == w3 && _IsInClientArea(w3)) ? w3 : default;
			}
			return w1;

			bool _IsInClientArea(Wnd w) => w.GetClientRect(out var rc, true) && rc.Contains(r.rect);
		}

		static bool _WaitForHotkey(string info)
		{
			using(Osd.ShowText(info, Timeout.Infinite, icon: SystemIcons.Information)) {
				//try { Keyb.WaitForHotkey(0, KKey.F3); }
				//catch(AException) { ADialog.ShowError("Failed to register hotkey F3"); return false; }

				Keyb.WaitForKey(0, KKey.F3, up: true, block: true);
			}
			return true;
		}

		class _Form : Form
		{
			Bitmap _img;
			Graphics _gMagn;
			Bitmap _bMagn;
			bool _paintedOnce;
			bool _magnMoved;
			bool _capturing;
			WICFlags _flags;
			Cursor _cursor;

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
				Cursor = _cursor = Util.ACursor.LoadCursorFromMemory(Properties.Resources.red_cross_cursor, 32);
			}

			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					p.Style = unchecked((int)(WS.POPUP));
					p.ExStyle = (int)(WS_EX.TOOLWINDOW | WS_EX.TOPMOST);
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
				using var gr = this.CreateGraphics();
				var pc = e.Location; //cursor position

				//format text to draw below magnifier
				string text;
				using(new Util.LibStringBuilder(out var s)) {
					var ic = _flags & (WICFlags.Image | WICFlags.Color | WICFlags.Rectangle);
					if(ic == 0) ic = WICFlags.Image | WICFlags.Color;
					bool canColor = ic.Has(WICFlags.Color);
					if(canColor) {
						var color = _img.GetPixel(pc.X, pc.Y).ToArgb() & 0xffffff;
						s.Append("Color  0x").Append(color.ToString("X6")).Append('\n');
					}
					if(ic == WICFlags.Color) {
						s.Append("Click to capture color.\n");
					} else if(ic == WICFlags.Rectangle) {
						s.Append("Mouse-drag to capture rectangle.\n");
					} else {
						s.Append("How to capture\n");
						s.Append("  rectangle:  mouse-drag\n  any shape:  Shift+drag\n");
						if(canColor) s.Append("  color:  Ctrl+click\n");
					}
					s.Append("More:  right-click"); //"  cancel:  key Esc\n  retry:  key F3 ... F3"
					text = s.ToString();
				}

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
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				if(e.Button != MouseButtons.Left) return;

				bool isColor = false, isAnyShape = false;
				var ic = _flags & (WICFlags.Image | WICFlags.Color | WICFlags.Rectangle);
				if(ic == WICFlags.Color) {
					isColor = true;
				} else {
					var mod = ModifierKeys;
					if(mod != 0 && ic == WICFlags.Rectangle) return;
					switch(mod) {
					case Keys.None: break;
					case Keys.Shift: isAnyShape = true; break;
					case Keys.Control when ic != WICFlags.Image: isColor = true; break;
					default: return;
					}
				}

				Result = new WICResult();
				POINT p0 = e.Location;
				if(isColor) {
					Result.color = _img.GetPixel(p0.x, p0.y).ToArgb();
					Result.rect = (p0.x, p0.y, 1, 1);
				} else {
					RECT r = (p0.x, p0.y, 0, 0);
					var a = isAnyShape ? new List<POINT>() { p0 } : null;
					var pen = Pens.Red;
					bool notFirstMove = false;
					_capturing = true;
					try {
						if(!Util.DragDrop.SimpleDragDrop(this, MButtons.Left, m => {
							if(m.Msg.message != Api.WM_MOUSEMOVE) return;
							POINT p = m.Msg.pt;
							p.x -= Left; p.y -= Top; //screen to client
							using var g = this.CreateGraphics();
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

					if(ic != WICFlags.Rectangle) {
						var b = _img.Clone(r, PixelFormat.Format32bppArgb);
						var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
						try {
							_SetAlpha(d, r, path);
						}
						finally { b.UnlockBits(d); path?.Dispose(); }
						Result.image = b;
					}

					r.Offset(Left, Top); //client to screen
					Result.rect = r;
				}

				this.DialogResult = DialogResult.OK;
				this.Close();
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				if(e.Button != MouseButtons.Right) return;
				var m = new AMenu();
				m["Retry\tF3"] = o => { this.DialogResult = DialogResult.Retry; this.Close(); };
				m["Cancel\tEsc"] = o => this.Close();
				m.Show(this);
			}

			//note: the OSD window is not always activated. Then can use context menu.
			protected override void OnKeyUp(KeyEventArgs e) //note: not Down, because on F3 will wait for F3 up
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
	}

	/// <summary>
	/// Results of <see cref="WinImage.CaptureUI"/>.
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
		public Wnd wnd;
	}
}
