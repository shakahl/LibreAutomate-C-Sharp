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
using System.Windows.Forms; //SHOULDDO: avoid loading Forms dll. Now from it uses TextRenderer.
using System.Drawing;
//using System.Linq;

namespace Au
{
	/// <summary>
	/// Transparent window that can be used for on-screen display. Derived classes on it can draw non-transparent text, rectangle, image, anything.
	/// </summary>
	public abstract class AOsdWindow : IDisposable
	{
		AWnd _w;

		/// <summary>Destroys the OSD window.</summary>
		protected virtual void Dispose(bool disposing)
		{
			if(_w.Is0) return;
			_w.Close();
			_w = default;
		}

		/// <summary>Destroys the OSD window.</summary>
		public void Dispose() => Dispose(true);

		/// <summary>Destroys the OSD window.</summary>
		public void Close() => Dispose(true);

		/// <summary>OSD window handle or default(AWnd).</summary>
		public AWnd Handle => _w;

		/// <summary>
		/// Returns true if the OSD window is created.
		/// </summary>
		protected bool IsHandleCreated => !_w.Is0;

		/// <summary>
		/// Redraws the OSD window immediately.
		/// Does nothing it is not created or not visible.
		/// </summary>
		protected void Redraw()
		{
			if(!Visible) return;
			Api.InvalidateRect(_w, default, false);
			Api.UpdateWindow(_w);
		}

		/// <summary>
		/// Sets to redraw the OSD window later.
		/// Does nothing it is not created or not visible.
		/// </summary>
		protected void Invalidate()
		{
			if(!Visible) return;
			Api.InvalidateRect(_w, default, false);
		}

		/// <summary>
		/// Gets or sets the opacity of the OSD window, from 0 to 1.
		/// If 1 (default), completely opaque. If 0, pixels of <see cref="TransparentColor"/> are transparent, others opaque. If between 0 and 1, partially transparent.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// </remarks>
		public double Opacity {
			get => _opacity;
			set {
				var v = Math.Min(Math.Max(value, 0.0), 1.0);
				if(v == _opacity) return;
				bool was0 = _opacity == 0;
				_opacity = v;
				if(!IsHandleCreated) return;
				_SetOpacity();
				if((v == 0) != was0) Redraw();
			}
		}
		double _opacity = 1d;

		void _SetOpacity()
		{
			if(_opacity > 0) Api.SetLayeredWindowAttributes(_w, 0, (byte)(uint)(_opacity * 255), 2);
			else Api.SetLayeredWindowAttributes(_w, (uint)TransparentColor.ToBGR(), 0, 1);
		}

		/// <summary>
		/// Gets or sets transparent color, default 0xF5F4F5. Pixels of this color will be transparent, unless <see cref="Opacity"/> is not 0.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// Note: when used for transparent text, text edges are blended with this color, and it can become visible if the color is not chosen carefully.
		/// </remarks>
		public ColorInt TransparentColor { get; set; } = 0xFFF5F4F5;

		/// <summary>
		/// Gets or sets OSD window size and position in screen.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// </remarks>
		public RECT Rect {
			get => _r;
			set {
				if(value == _r) return;
				_r = value;
				if(IsHandleCreated) _w.SetWindowPos(Native.SWP.NOACTIVATE, _r.left, _r.top, _r.Width, _r.Height, Native.HWND.TOPMOST);
			}
		}
		RECT _r;

		/// <summary>
		/// Gets or sets whether the OSD window is visible.
		/// The 'set' function calls <see cref="Show"/> (it creates OSD window if need) or <see cref="Hide"/> (it does not destroy the OSD window).
		/// </summary>
		public bool Visible {
			get => _w.IsVisible;
			set { if(value) Show(); else Hide(); } //note: if overridden, calls the override func
		}

		/// <summary>
		/// Shows the OSD window. Creates if need.
		/// </summary>
		/// <remarks>
		/// In any case, also moves the window to the top of the Z order.
		/// </remarks>
		public virtual void Show()
		{
			if(Visible) {
				_w.ZorderTopmost();
			} else {
				if(_w.Is0) _CreateWindow();
				_w.ShowLL(true);
				_w.ZorderTopmost();
				Api.UpdateWindow(_w);
			}
		}

		/// <summary>
		/// Hides the OSD window. Does not destroy; use <see cref="Close"/> or <see cref="Dispose"/> for it.
		/// Does nothing if not created or not visible.
		/// </summary>
		public virtual void Hide()
		{
			if(!Visible) return;
			_w.ShowLL(false);
		}

		void _CreateWindow()
		{
			//register window class if need. Need another class if shadow.
			string cn; byte regMask;
			if(Shadow) { cn = "Au.OSD2"; regMask = 2; } else { cn = "Au.OSD"; regMask = 1; }
			if((s_isWinClassRegistered & regMask) == 0) {
				var ce = new RWCEtc() { style = Api.CS_HREDRAW | Api.CS_VREDRAW, hbrBackground = default(IntPtr) };
				if(Shadow) ce.style |= Api.CS_DROPSHADOW;
				AWnd.More.RegisterWindowClass(cn, null, ce);
				s_isWinClassRegistered |= regMask;
			}

			var es = WS2.TOOLWINDOW | WS2.TOPMOST | WS2.LAYERED | WS2.TRANSPARENT | WS2.NOACTIVATE;
			if(ClickToClose) es &= ~WS2.TRANSPARENT;
			_w = AWnd.More.CreateWindow(WndProc, cn, Name, WS.POPUP, es); //note: don't set rect here: can be painting problems when resizing
			_SetOpacity();
			if(!_r.Is0) _w.SetWindowPos(Native.SWP.NOACTIVATE, _r.left, _r.top, _r.Width, _r.Height, Native.HWND.TOPMOST);
		}
		static byte s_isWinClassRegistered;

		/// <summary>
		/// Called when the OSD window receives a message.
		/// If your derived class overrides this function, it must call base.WndProc and return its return value, except when don't need default processing.
		/// </summary>
		protected virtual LPARAM WndProc(AWnd w, int message, LPARAM wParam, LPARAM lParam)
		{
			switch(message) {
			case Api.WM_ERASEBKGND:
				return 0;
			case Api.WM_PAINT:
				var dc = Api.BeginPaint(w, out var ps);
				try {
					var r = w.ClientRect;
					using var bg = BufferedGraphicsManager.Current.Allocate(dc, r);
					var g = bg.Graphics;
					if(_opacity == 0) g.Clear((Color)TransparentColor);
					OnPaint(g, r);
					bg.Render();
				}
				finally { Api.EndPaint(w, ps); }
				return 0;
			case Api.WM_MOUSEACTIVATE:
				return Api.MA_NOACTIVATE;
			case Api.WM_LBUTTONUP:
			case Api.WM_RBUTTONUP:
			case Api.WM_MBUTTONUP:
				if(ClickToClose) w.Post(Api.WM_CLOSE);
				break;
			}

			return Api.DefWindowProc(w, message, wParam, lParam);
		}

		/// <summary>
		/// Called when the OSD window must be drawn or redrawn.
		/// Derived classes should override this function and draw anything. Don't need to call base.OnPaint of <see cref="AOsdWindow"/>, it does nothing.
		/// </summary>
		/// <remarks>
		/// If <see cref="Opacity"/> is 0 (default), <i>g</i> is filled with <see cref="TransparentColor"/>. Pixels of this color will be transparent. The base class draws only non-transparent areas.
		/// </remarks>
		protected virtual void OnPaint(Graphics g, Rectangle r)
		{
		}

		/// <summary>
		/// If true, the OSD window will have shadow.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		protected bool Shadow { get; set; }

		/// <summary>
		/// If true, the OSD window receive mouse messages. Only completely transparent areas don't. The user can click to close the OSD (left, right or middle button).
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		protected bool ClickToClose { get; set; }

		/// <summary>
		/// OSD window name. Optional, default null.
		/// </summary>
		/// <remarks>
		/// This text is invisible. Can be used to find OSD window. The class name is "Au.OSD"; if with shadow - "Au.OSD2".
		/// </remarks>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// Closes all OSD windows of this process.
		/// </summary>
		/// <param name="name">If not null, closes only OSD windows whose <see cref="Name"/> matches this [](xref:wildcard_expression).</param>
		public static void CloseAll([ParamString(PSFormat.AWildex)] string name = null)
		{
			foreach(var w in AWnd.FindAll(name, "**m Au.OSD||Au.OSD2", WOwner.Process(AProcess.ProcessId))) w.Close(noWait: true);
		}
	}

	/// <summary>
	/// Shows mouse-transparent rectangle on screen. Its interior can be visually transparent or opaque.
	/// </summary>
	/// <remarks>
	/// Creates a temporary partially transparent window, and draws rectangle in it.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// using(var x = new AOsdRect()) {
	/// 	x.Rect = (300, 300, 100, 100);
	/// 	x.Color = Color.SlateBlue;
	/// 	x.Thickness = 4;
	/// 	x.Show();
	/// 	for(int i = 0; i < 5; i++) {
	/// 		250.ms();
	/// 		x.Visible = !x.Visible;
	/// 	}
	/// }
	/// ]]></code>
	/// </example>
	public class AOsdRect : AOsdWindow
	{
		///
		public AOsdRect()
		{
			Opacity = 0d;
		}

		/// <summary>
		/// Gets or sets rectangle color.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// x.Color = 0xFF0000; //red
		/// x.Color = Color.Orange;
		/// ]]></code>
		/// </example>
		public ColorInt Color {
			get => _color;
			set { if(value != _color) { _color = value; Redraw(); } }
		}
		ColorInt _color = 0xFF000000;

		/// <summary>
		/// Gets or sets rectangle frame width.
		/// Used only if <see cref="AOsdWindow.Opacity"/> is 0 (default).
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// </remarks>
		public int Thickness {
			get => _thickness;
			set { if(value != _thickness) { _thickness = value; Redraw(); } }
		}
		int _thickness = 3;

		/// <summary>
		/// Called when the OSD window must be drawn or redrawn. Draws rectangle. More info: <see cref="AOsdWindow.OnPaint"/>.
		/// </summary>
		protected override void OnPaint(Graphics g, Rectangle r)
		{
			if(Opacity > 0) {
				g.Clear((Color)_color);
			} else {
				using var pen = new Pen((Color)_color, _thickness);
				pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
				g.DrawRectangle(pen, r);
			}
		}
	}

	/// <summary>
	/// Shows mouse-transparent text on screen. Its background can be visually transparent or opaque.
	/// </summary>
	/// <remarks>
	/// Creates a temporary partially transparent window, and draws text in it.
	/// Most properties cannot be changed after creating OSD window.
	/// </remarks>
	public class AOsd : AOsdWindow
	{
		/// <summary>
		/// Coordinates.
		/// Default: null. Screen center.
		/// </summary>
		/// <remarks>
		/// Not used if <see cref="Rect"/> is set.
		/// This property can be changed after creating OSD window.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var m = new AOsd { Text = "Text" };
		/// m.XY = new PopupXY(Coord.Center, Coord.Max); //bottom-center of the work area of the primary screen
		/// m.Show();
		/// ]]></code>
		/// </example>
		public PopupXY XY {
			get => _xy;
			set {
				_xy = value;
				if(value != null) _rectIsSet = false;
				if(IsHandleCreated) base.Rect = Measure();
			}
		}
		PopupXY _xy;

		/// <summary>
		/// Gets or sets OSD window size and position in screen.
		/// </summary>
		/// <remarks>
		/// Normally don't need to use this property. If not used, the OSD window size depends on text etc, and position on <see cref="XY"/>.
		/// This property can be changed after creating OSD window.
		/// </remarks>
		/// <seealso cref="Measure"/>
		public new RECT Rect {
			get => base.Rect;
			set { _rectIsSet = true; base.Rect = value; }
		}
		bool _rectIsSet;

		void _ResizeOrInvalidate()
		{
			if(ResizeWhenContentChanged) base.Rect = Measure(); //info: invalidates if resized, because of the CS_ styles
			Invalidate();
		}

		/// <summary>
		/// When changing text, resize/move the OSD window if need.
		/// Default: false.
		/// </summary>
		public bool ResizeWhenContentChanged { get; set; }

		/// <summary>
		/// Text in OSD window.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window; then the window is not moved/resized, unless <see cref="ResizeWhenContentChanged"/> is true.
		/// </remarks>
		public string Text {
			get => _text;
			set { if(value != _text) { _text = value; _ResizeOrInvalidate(); } }
		}
		string _text;

		/// <summary>
		/// Font.
		/// If not set, uses <see cref="DefaultSmallFont"/>.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public OsdFont Font { get; set; }

		/// <summary>
		/// Text color.
		/// Default: <see cref="DefaultTextColor"/>.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// </remarks>
		public ColorInt TextColor {
			get => _textColor;
			set { if(value != _textColor) { _textColor = value; Invalidate(); } }
		}
		ColorInt _textColor;

		/// <summary>
		/// Background color.
		/// Default: <see cref="DefaultBackColor"/>.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// Not used for completely transparent OSD.
		/// </remarks>
		public ColorInt BackColor {
			get => _backColor;
			set { if(value != _backColor) { _backColor = value; Invalidate(); } }
		}
		ColorInt _backColor;

		/// <summary>
		/// Border color.
		/// Default: <see cref="DefaultBorderColor"/>.
		/// </summary>
		/// <remarks>
		/// This property can be changed after creating OSD window.
		/// No border if <see cref="AOsdWindow.Opacity"/>==0 or <b>BorderColor</b>==<see cref="BackColor"/>.
		/// </remarks>
		public ColorInt BorderColor {
			get => _borderColor;
			set { if(value != _borderColor) { _borderColor = value; Invalidate(); } }
		}
		private ColorInt _borderColor;

		/// <summary>
		/// Background image.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public Image BackgroundImage { get; set; }

		//public ImageLayout BackgroundImageLayout { get; set; } //FUTURE

		/// <summary>
		/// When used <see cref="BackgroundImage"/>, the OSD window has the same size as the image, plus borders.
		/// Else OSD window size is calculated from sizes of text and icon. Then image is displayed scaled or clipped if need.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public bool IsOfImageSize { get; set; }

		/// <summary>
		/// Maximal text width.
		/// Default: 0 - no limit (depends on screen width etc).
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public int WrapWidth { get; set; }

		/// <summary>
		/// Gets or sets text format flags.
		/// Default: TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak | TextFormatFlags.ExpandTabs.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public TextFormatFlags TextFormatFlags { get; set; } = TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak | TextFormatFlags.ExpandTabs;

		/// <summary>
		/// Icon at the left of text.
		/// For example SystemIcons.Information.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public Icon Icon { get; set; }

		/// <summary>
		/// If true, the OSD window will have shadow.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// Window shadows can be disabled. See <msdn>SPI_SETDROPSHADOW</msdn>.
		/// </remarks>
		public new bool Shadow { get => base.Shadow; set => base.Shadow = value; }

		/// <summary>
		/// If true, the OSD window receive mouse messages. Only completely transparent areas don't. The user can click to close the OSD (left, right or middle button).
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public new bool ClickToClose { get => base.ClickToClose; set => base.ClickToClose = value; }

		/// <summary>
		/// Close the OSD window after this time, seconds.
		/// If 0 (default), depends on text length. Can be <see cref="Timeout.Infinite"/> (-1).
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public int SecondsTimeout { get; set; }

		/// <summary>
		/// See <see cref="OsdShowMode"/>.
		/// </summary>
		/// <remarks>
		/// This property cannot be changed after creating OSD window.
		/// </remarks>
		public OsdShowMode ShowMode { get; set; }

		//CONSIDER: public AnyWnd Owner { get; set; }

		///
		public AOsd()
		{
			Font = DefaultSmallFont;
			_textColor = DefaultTextColor;
			_backColor = DefaultBackColor;
			_borderColor = DefaultBorderColor;
		}

		/// <summary>
		/// Shows the OSD window. Creates if need.
		/// By default does not wait; the window will be closed after <see cref="SecondsTimeout"/>.
		/// </summary>
		/// <remarks>
		/// Depending on <see cref="ShowMode"/>, creates the OSD window in this or new thread.
		/// If the OSD window is already created, just shows it if hidden. Many properties can be changed only before creating OSD window; call <see cref="AOsdWindow.Close"/> if need.
		/// </remarks>
		public override void Show()
		{
			if(!Handle.Is0) {
				base.Show();
				return;
			}
			bool thisThread = false, wait = false;
			switch(ShowMode) {
			case OsdShowMode.Auto: thisThread = AThread.HasMessageLoop(); break;
			case OsdShowMode.ThisThread: thisThread = true; break;
			case OsdShowMode.Wait: thisThread = wait = true; break;
			}
			if(thisThread) {
				_Show(wait);
			} else {
				//Task.Run(() => ShowWait()); //works too, but cannot use StrongThread
				AThread.Start(() => _Show(true), ShowMode == OsdShowMode.WeakThread);
				AWaitFor.Condition(30, () => IsHandleCreated);

				//CONSIDER: make smaller timeout when main thread ended if OsdShowMode.Auto
			}
		}

		void _Show(bool sync)
		{
			if(!_rectIsSet) base.Rect = Measure();

			int t = SecondsTimeout;
			if(t == 0) t = Math.Min(Text.Lenn(), 1000) / 10 + 3; //calc time from text length
			base.Show();
			if(sync) {
				if(t < 0) t = 0; else t = -t;
				if(!AWaitFor.MessagesAndCondition(t, () => !IsHandleCreated)) Close();
			} else if(t > 0) {
				t = Math.Min(t, int.MaxValue / 1000) * 1000; //s -> ms
				ATimer.After(t, _ => Close());
			}
		}

		/// <summary>
		/// Draws OSD text etc.
		/// </summary>
		protected override void OnPaint(Graphics g, Rectangle r)
		{
			//AOutput.Write(AThread.NativeId);
			if(Opacity != 0) {
				g.Clear((Color)BackColor); //else AOsdWindow cleared with TransparentColor

				if(BorderColor != BackColor) { //border
					using var pen = new Pen((Color)BorderColor);
					Rectangle rr = r; rr.Width--; rr.Height--;
					g.DrawRectangle(pen, rr);
					r.Inflate(-1, -1);
				}
			}

			if(BackgroundImage != null) {
				g.DrawImageUnscaledAndClipped(BackgroundImage, r); //info: if image smaller - scales; if bigger - clips.
			}

			r.Inflate(-1, -1); //margins

			if(Icon != null) {
				var ri = new Rectangle(r.X + c_iconPadding, r.Y + c_iconPadding, Icon.Width, Icon.Height);
				g.DrawIconUnstretched(Icon, ri);
				int k = Icon.Width + c_iconPadding * 2; r.X += k; r.Width -= k;
			}

			if(!Text.NE()) {
				var tff = TextFormatFlags; if(WrapWidth > 0) tff |= TextFormatFlags.WordBreak;
				TextRenderer.DrawText(g, Text, _font, r, (Color)TextColor, tff);
				//TextRenderer.DrawText(g, Text, _font, r, (Color)TextColor, Opacity==0 ? (Color)TransparentColor : (Color)BackColor, _textFormatFlags);
			}
		}

		const int c_iconPadding = 5;

		/// <summary>
		/// Calculates OSD window size and position.
		/// Can be called before showing OSD.
		/// </summary>
		public RECT Measure()
		{
			Size z = default;
			if(IsOfImageSize && BackgroundImage != null) {
				z = BackgroundImage.Size;
			} else {
				Size zi = default;
				if(Icon != null) {
					zi = Icon.Size;
					zi.Width += c_iconPadding * 2; zi.Height += c_iconPadding * 2;
				}

				if(!Text.NE()) {
					var screen = XY?.GetScreen() ?? DefaultScreen.Now;
					int dpi = screen.Dpi;
					_font = (Font ?? DefaultSmallFont).CreateFont(dpi);
					var rs = screen.WorkArea; z = new Size(rs.Width - zi.Width - 10, rs.Height - 14);
					var tff = TextFormatFlags;
					int ww = WrapWidth; if(ww > 0) { tff |= TextFormatFlags.WordBreak; if(ww < z.Width) z.Width = ww; }
					z = TextRenderer.MeasureText(Text, _font, z, tff);
					z.Height += AMath.MulDiv(3, dpi, 96);
				}

				z.Width += zi.Width;
				z.Height = Math.Max(z.Height, zi.Height);

				z.Width += 2; z.Height += 2; //margins
			}

			if(Opacity != 0 && BorderColor != BackColor) { //border
				z.Width += 2; z.Height += 2;
			}

			var r = new RECT(0, 0, z.Width, z.Height);
			//AOutput.Write(r);

			if(XY != null) {
				if(XY.inRect) r.MoveInRect(XY.rect, XY.x, XY.y, false);
				else r.MoveInScreen(XY.x, XY.y, XY.screen, XY.workArea, ensureInScreen: false);
				r.EnsureInScreen(workArea: true);
			} else {
				r.MoveInScreen(Coord.Center, Coord.Center, DefaultScreen, workArea: true, ensureInScreen: true);
			}

			return r;
		}
		Font _font;

		/// <summary>
		/// Shows a tooltip-like OSD window with text and optionally icon.
		/// </summary>
		/// <param name="text"><see cref="Text"/></param>
		/// <param name="secondsTimeout"><see cref="SecondsTimeout"/></param>
		/// <param name="xy"><see cref="XY"/></param>
		/// <param name="icon"><see cref="Icon"/></param>
		/// <param name="textColor"><see cref="TextColor"/></param>
		/// <param name="backColor"><see cref="BackColor"/></param>
		/// <param name="name"><see cref="AOsdWindow.Name"/></param>
		/// <param name="showMode"><see cref="ShowMode"/></param>
		/// <param name="doNotShow">Don't call <see cref="Show"/>. The caller can use the return value to set some other properties and call <b>Show</b>.</param>
		/// <returns>Returns an <see cref="AOsd"/> object that can be used to change properties or close the OSD window.</returns>
		/// <remarks>
		/// Also sets these properties: <see cref="ClickToClose"/>=true, <see cref="Shadow"/>=true.
		/// </remarks>
		public static AOsd ShowText(string text,
			int secondsTimeout = 0, PopupXY xy = null,
			Icon icon = null, ColorInt textColor = default, ColorInt backColor = default,
			string name = null, OsdShowMode showMode = default, bool doNotShow = false)
		{
			var o = new AOsd();
			o._text = text;
			o.SecondsTimeout = secondsTimeout;
			o._xy = xy;
			o.Icon = icon;
			if(textColor != default) o._textColor = textColor;
			if(backColor != default) o._backColor = backColor;
			o.Name = name;
			o.ShowMode = showMode;

			o.ClickToClose = true;
			o.Shadow = true;

			if(!doNotShow) o.Show();
			return o;
		}

		/// <summary>
		/// Shows a big-font text with transparent background.
		/// </summary>
		/// <param name="text"><see cref="Text"/></param>
		/// <param name="secondsTimeout"><see cref="SecondsTimeout"/></param>
		/// <param name="xy"><see cref="XY"/></param>
		/// <param name="color"><see cref="TextColor"/>. Default: <see cref="DefaultTransparentTextColor"/>.</param>
		/// <param name="name"><see cref="AOsdWindow.Name"/></param>
		/// <param name="showMode"><see cref="ShowMode"/></param>
		/// <param name="doNotShow">See <see cref="ShowText"/>.</param>
		/// <returns>Returns an <see cref="AOsd"/> object that can be used to change properties or close the OSD window.</returns>
		/// <remarks>
		/// Also sets these properties: <see cref="Font"/>=<see cref="DefaultBigFont"/>, <see cref="AOsdWindow.Opacity"/>=0.
		/// </remarks>
		public static AOsd ShowTransparentText(string text,
			int secondsTimeout = 0, PopupXY xy = null,
			ColorInt color = default,
			string name = null, OsdShowMode showMode = default, bool doNotShow = false)
		{
			var o = new AOsd();
			o._text = text;
			o.SecondsTimeout = secondsTimeout;
			o._xy = xy;
			o._textColor = color == default ? DefaultTransparentTextColor : color;
			o.Name = name;
			o.ShowMode = showMode;

			o.Font = s_bigFont;
			o.Opacity = 0d;

			if(!doNotShow) o.Show();
			return o;
		}

		/// <summary>
		/// Shows on-screen image.
		/// </summary>
		/// <param name="image"><see cref="BackgroundImage"/></param>
		/// <param name="secondsTimeout"><see cref="SecondsTimeout"/></param>
		/// <param name="xy"><see cref="XY"/></param>
		/// <param name="name"><see cref="AOsdWindow.Name"/></param>
		/// <param name="showMode"><see cref="ShowMode"/></param>
		/// <param name="doNotShow">See <see cref="ShowText"/>.</param>
		/// <returns>Returns an <see cref="AOsd"/> object that can be used to change properties or close the OSD window.</returns>
		/// <remarks>
		/// Also sets these properties: <see cref="IsOfImageSize"/>=true, <see cref="AOsdWindow.Opacity"/>=0, <see cref="ClickToClose"/>=true.
		/// </remarks>
		public static AOsd ShowImage(Image image,
			int secondsTimeout = 0, PopupXY xy = null,
			string name = null, OsdShowMode showMode = default, bool doNotShow = false)
		{
			var o = new AOsd();
			o.BackgroundImage = image;
			o.SecondsTimeout = secondsTimeout;
			o._xy = xy;
			o.Name = name;
			o.ShowMode = showMode;

			o.IsOfImageSize = true;
			o.Opacity = 0d;
			o.ClickToClose = true;
			o._backColor = o.TransparentColor;

			if(!doNotShow) o.Show();
			return o;
		}

		/// <summary>Default font for <see cref="ShowText"/> and <b>AOsd</b>. Default: Segoe UI 12.</summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static OsdFont DefaultSmallFont { get => s_smallFont; set => s_smallFont = value ?? throw new ArgumentNullException(); }
		static OsdFont s_smallFont = new OsdFont(12);

		/// <summary>Default font for <see cref="ShowTransparentText"/>. Default: Default: Segoe UI 20.</summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static OsdFont DefaultBigFont { get => s_bigFont; set => s_bigFont = value ?? throw new ArgumentNullException(); }
		static OsdFont s_bigFont = new OsdFont(20);

		/// <summary>Default text color for <see cref="ShowText"/> and <b>AOsd</b>. Default: 0xFF404040 (dark gray).</summary>
		public static ColorInt DefaultTextColor { get; set; } = 0xFF404040;

		/// <summary>Default border color for <see cref="ShowText"/> and <b>AOsd</b>. Default: 0xFF404040 (dark gray).</summary>
		public static ColorInt DefaultBorderColor { get; set; } = 0xFF404040;

		/// <summary>Default background color for <see cref="ShowText"/> and <b>AOsd</b>. Default: 0xFFFFFFF0 (light yellow).</summary>
		public static ColorInt DefaultBackColor { get; set; } = 0xFFFFFFF0;

		/// <summary>Default text color for <see cref="ShowTransparentText"/>. Default: 0xFF8A2BE2 (Color.BlueViolet).</summary>
		public static ColorInt DefaultTransparentTextColor { get; set; } = 0xFF8A2BE2;

		/// <summary>
		/// Default screen when <see cref="XY"/> is not set.
		/// The <b>AScreen</b> must be lazy or empty.
		/// </summary>
		/// <exception cref="ArgumentException"><b>AScreen</b> with <b>Handle</b>. Must be lazy (with <b>LazyFunc</b>) or empty.</exception>
		/// <example>
		/// <code><![CDATA[
		/// AOsd.DefaultScreen = AScreen.Index(1, lazy: true);
		/// ]]></code>
		/// </example>
		public static AScreen DefaultScreen {
			get => _defaultScreen;
			set => _defaultScreen = value.ThrowIfWithHandle_;
		}
		static AScreen _defaultScreen;
	}
}

namespace Au.Types
{
	/// <summary>
	/// Whether <see cref="AOsd.Show"/> waits or shows the OSD window in this or new thread.
	/// </summary>
	/// <remarks>
	/// If this thread has windows, any value can be used, but usually <b>Auto</b> (default) or <b>ThisThread</b> is the best.
	/// </remarks>
	public enum OsdShowMode
	{
		/// <summary>Depends on <see cref="AThread.HasMessageLoop"/>. If it is true, uses <b>ThisThread</b>, else <b>StrongThread</b>. Does not wait.</summary>
		Auto,

		/// <summary>
		/// Show the OSD window in this thread and don't wait.
		/// Don't use if this thread does not process messages and therefore cannot have windows.
		/// </summary>
		ThisThread,

		/// <summary>Show the OSD window in new thread and don't wait. Set <see cref="Thread.IsBackground"/>=true, so that the OSD is closed when other threads of this app end.</summary>
		WeakThread,

		/// <summary>Show the OSD window in new thread and don't wait. Set <see cref="Thread.IsBackground"/>=false, so that the OSD is not closed when other threads of this app end.</summary>
		StrongThread,

		/// <summary>
		/// Show the OSD window in this thread and wait until it disappears.
		/// Waits <see cref="AOsd.SecondsTimeout"/> seconds. While waiting, dispatches messages etc; see <see cref="ATime.SleepDoEvents"/>.
		/// </summary>
		Wait,
	}

	/// <summary>
	/// Font size and family name for OSD classes.
	/// </summary>
	public class OsdFont { //FUTURE: could be record
		/// <summary>
		/// Family name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Size.
		/// On high-DPI screen the font size will be scaled.
		/// </summary>
		public int Size { get; }

		/// <summary>
		/// Creates font.
		/// </summary>
		/// <param name="dpi">DPI for scaling.</param>
		public Font CreateFont(int dpi) => new Font(Name, AMath.MulDiv(Size, dpi, 96));

		/// <param name="size">Sets <see cref="Size"/>.</param>
		/// <param name="name">Sets <see cref="Name"/>. Default "Segoe UI".</param>
		public OsdFont(int size, string name = "Segoe UI") {
			Size = size;
			Name = name ?? "Segoe UI";
		}
	}
}