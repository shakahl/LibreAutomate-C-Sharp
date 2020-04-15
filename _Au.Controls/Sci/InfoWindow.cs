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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;

namespace Au.Controls
{
	/// <summary>
	/// An info window, similar to tooltips but persistent, normally at the right side of a form/control/rectangle.
	/// Supports output tags etc. Actually it is a floating <see cref="InfoBox"/>.
	/// You can set text, resize and show/hide/dispose it many times.
	/// User can middle-click to hide.
	/// </summary>
	public partial class InfoWindow /*: IMessageFilter*/
	{
		_Window _w;
		_Control _c, _c2;
		readonly int _twoControlsSplitPos;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="twoControlsSplitPos">If not 0 (default), creates second <see cref="InfoBox"/> control at this horizontal offset.</param>
		public InfoWindow(int twoControlsSplitPos = 0)
		{
			_twoControlsSplitPos = twoControlsSplitPos;
		}

		/// <summary>
		/// The top-level info window. Contains <see cref="Control1"/>.
		/// </summary>
		public InactiveWindow Window => _W;

		_Window _W {
			get {
				if(_w == null || _w.IsDisposed) {
					_w = new _Window(this) { Text = _caption };
					_w.Controls.Add(_c = new _Control(this, 0, _twoControlsSplitPos) { Name = "_c" });
					if(_twoControlsSplitPos > 0) _w.Controls.Add(_c2 = new _Control(this, _twoControlsSplitPos, _twoControlsSplitPos) { Name = "_c2" });
				}
				return _w;
			}
		}

		/// <summary>
		/// The child control of <see cref="Window"/>. Displays text.
		/// </summary>
		public InfoBox Control1 { get { _ = _W; return _c; } }

		/// <summary>
		/// The second child control of <see cref="Window"/>. Displays text.
		/// </summary>
		public InfoBox Control2 { get { _ = _W; return _c2; } }

		/// <summary>
		/// Text with output tags.
		/// </summary>
		public string Text {
			get => _c?.Text;
			set => Control1.Text = value;
		}

		/// <summary>
		/// Text of second control with output tags.
		/// </summary>
		public string Text2 {
			get => _c2?.Text;
			set => Control2.Text = value;
		}

		/// <summary>
		/// Window size.
		/// Default: (300, 200) dpi-scaled.
		/// </summary>
		/// <remarks>
		/// If visible, does not ensure correct position. Usually it's better to hide, set size and show again.
		/// </remarks>
		public SIZE Size {
			get => _size != default ? _size : Util.ADpi.ScaleSize((300, 200));
			set { _size = value; if(_w?.IsHandleCreated ?? false) _w.Size = _size; }
		}
		SIZE _size;

		/// <summary>
		/// Shows the info window by the control.
		/// </summary>
		/// <param name="c">Control. Its top-level parent window will own the info window.</param>
		/// <param name="align"></param>
		/// <exception cref="ArgumentException">c is null or its handle is not created.</exception>
		/// <exception cref="InvalidOperationException">Exceptions of <see cref="Form.Show(IWin32Window)"/>.</exception>
		public void Show(Control c, PopupAlignment align = default)
		{
			_ = c?.IsHandleCreated ?? throw new ArgumentException();
			_Show(c, ((AWnd)c).Rect, align);
		}

		/// <summary>
		/// Shows the info window by the control and rectangle.
		/// </summary>
		/// <param name="c">Control or form. The top-level window will own the info window.</param>
		/// <param name="r">Rectangle in control's client area or in screen.</param>
		/// <param name="screenRect">r is in screen.</param>
		/// <param name="align"></param>
		/// <exception cref="ArgumentException">c is null or its handle is not created.</exception>
		/// <exception cref="InvalidOperationException">Exceptions of <see cref="Form.Show(IWin32Window)"/>.</exception>
		public void Show(Control c, Rectangle r, bool screenRect, PopupAlignment align = 0)
		{
			_ = c?.IsHandleCreated ?? throw new ArgumentException();
			if(!screenRect) r = c.RectangleToScreen(r);
			_Show(c, r, align);
		}

		/// <summary>
		/// Shows the info window by the rectangle.
		/// </summary>
		/// <param name="r">Rectangle in screen.</param>
		/// <param name="align"></param>
		/// <remarks>
		/// The info window is top-most.
		/// </remarks>
		public void Show(Rectangle r, PopupAlignment align = 0)
		{
			_Show(null, r, align);
		}

		void _Show(Control c, Rectangle r, PopupAlignment align)
		{
			_W.ZCalculateAndSetPosition(r.Right, r.Top, align, r, _size);
			_w.ZShow(c);
		}

		/// <summary>
		/// Hides the info window.
		/// Does not dispose.
		/// </summary>
		public void Hide()
		{
			_w?.Hide();
		}

		/// <summary>
		/// Destroys the info window.
		/// Later can be shown again.
		/// </summary>
		public void Dispose()
		{
			_w?.Dispose(); //sets our _w = null
		}

		/// <summary>
		/// If set to a non-null string before creating the info window, the window will be with caption and resizable.
		/// </summary>
		public string Caption {
			get => _caption;
			set {
				_caption = value;
				Debug.Assert(_w == null || (_w.Text == null) == (_caption == null)); //we don't support adding/removing caption later
				if(_w != null) _w.Text = _caption;
			}
		}
		string _caption;

		/// <summary>
		/// true if hidden when the user clicked the x button.
		/// </summary>
		public bool UserClosed { get; set; }

		/// <summary>
		/// Called when window loaded but still invisible.
		/// InfoWindow.OnLoad does nothing. This function is just for overriding.
		/// </summary>
		protected virtual void OnLoad(EventArgs e) { }

		//rejected, unfinished
		//public bool CloseOnEsc {
		//	get => _closeOnEsc;
		//	set {
		//		if(value!= _closeOnEsc) {
		//			if(_closeOnEsc = value) Application.AddMessageFilter(this);
		//			else Application.RemoveMessageFilter(this);
		//		}
		//	}
		//}
		//bool _closeOnEsc;

		//bool IMessageFilter.PreFilterMessage(ref Message m)
		//{
		//	AOutput.Write(m);
		//	return false;
		//}

		class _Window : InactiveWindow
		{
			InfoWindow _t;

			public _Window(InfoWindow t) : base(t.Caption == null ? WS.POPUP : WS.POPUP | WS.CAPTION | WS.SYSMENU | WS.THICKFRAME, shadow: t.Caption == null)
			{
				_t = t;

				this.Text = "Au.InfoWindow";
			}

			protected override void Dispose(bool disposing)
			{
				if(disposing) {
					_t._w = null;
				}
				base.Dispose(disposing);
			}

			public override void ZShow(Control ownerControl)
			{
				base.ZShow(ownerControl);
				_t.UserClosed = false;
			}

			protected override void OnFormClosing(FormClosingEventArgs e)
			{
				if(e.CloseReason == CloseReason.UserClosing) {
					e.Cancel = true;
					Hide();
					_t.UserClosed = true;
				}
				base.OnFormClosing(e);
			}

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				_t.OnLoad(e);
			}

			protected override void OnSizeChanged(EventArgs e)
			{
				_t._size = Size;
				base.OnSizeChanged(e);
			}
		}

		class _Control : InfoBox
		{
			InfoWindow _t;

			public _Control(InfoWindow t, int left, int splitPos)
			{
				_t = t;
				this.ZInitUseControlFont = true;
				if(splitPos == 0) { //single control
					this.Dock = DockStyle.Fill;
				} else {
					var z = t._w.ClientSize;
					this.Height = z.Height;
					if(left == 0) { //first control of two
						this.Width = splitPos;
						this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
					} else { //second control
						this.Left = splitPos + 2;
						this.Width = z.Width - splitPos - 2;
						this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
					}
				}
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				if(_t.Caption == null) this.ZInitBorderStyle = BorderStyle.FixedSingle;
				base.OnHandleCreated(e);
				Call(Sci.SCI_SETMARGINLEFT, 0, 4);
				Call(Sci.SCI_SETMARGINRIGHT, 0, 4);
				//base.NoMouseLeftSetFocus = true; //no, then cannot scroll with wheel on Win7-8.1
				//base.NoMouseRightSetFocus = true;
			}
		}
	}
}
