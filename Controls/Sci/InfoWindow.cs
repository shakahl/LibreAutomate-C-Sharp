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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

namespace Au.Controls
{
	/// <summary>
	/// An info window, similar to tooltips but persistent, normally at the right side of a form/control/rectangle.
	/// Supports print tags etc. Actually it is a floating <see cref="InfoBox"/>.
	/// You can set text, resize and show/hide it many times, and finally dispose.
	/// User can middle-click to hide.
	/// </summary>
	public partial class InfoWindow
	{
		_Window _w;
		_Control _c, _c2;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="twoControlsSplitPos">If not 0 (default), creates second <see cref="InfoBox"/> control at this horizontal offset.</param>
		public InfoWindow(int twoControlsSplitPos = 0)
		{
			_w = new _Window(this);
			_w.Controls.Add(_c = new _Control(this, 0, twoControlsSplitPos));
			if(twoControlsSplitPos > 0) _w.Controls.Add(_c2 = new _Control(this, twoControlsSplitPos, twoControlsSplitPos));
		}

		/// <summary>
		/// The top-level info window. Contains <see cref="Control"/>.
		/// </summary>
		public Form Window => _w;

		/// <summary>
		/// The child control of <see cref="Window"/>. Displays text.
		/// </summary>
		public InfoBox Control => _c;

		/// <summary>
		/// The second child control of <see cref="Window"/>. Displays text.
		/// </summary>
		public InfoBox Control2 => _c2;

		/// <summary>
		/// Text with print tags.
		/// </summary>
		public string Text {
			get => _c.Text;
			set => _c.Text = value;
		}

		/// <summary>
		/// Text of second control with print tags.
		/// </summary>
		public string Text2 {
			get => _c2.Text;
			set => _c2.Text = value;
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
			set { _size = value; if(_w.IsHandleCreated) _w.Size = _size; }
		}
		SIZE _size;

		/// <summary>
		/// Shows the info window below or above the anchor control.
		/// </summary>
		/// <param name="anchor">Control. Its top-level parent window will own the info window.</param>
		/// <exception cref="ArgumentException">anchor is null or its handle is not created.</exception>
		/// <exception cref="InvalidOperationException">Exceptions of <see cref="Form.Show(IWin32Window)"/>.</exception>
		public void Show(Control anchor)
		{
			_ = anchor?.IsHandleCreated ?? throw new ArgumentException();
			_Show(anchor, ((AWnd)anchor).Rect);
		}

		/// <summary>
		/// Shows the info window below or above the anchor rectangle relative to control.
		/// </summary>
		/// <param name="control">Control or form. The top-level window will own the info window.</param>
		/// <param name="anchor">Rectangle in control's client area.</param>
		/// <exception cref="ArgumentException">control is null or its handle is not created.</exception>
		/// <exception cref="InvalidOperationException">Exceptions of <see cref="Form.Show(IWin32Window)"/>.</exception>
		public void Show(Control control, Rectangle anchor)
		{
			_ = control?.IsHandleCreated ?? throw new ArgumentException();
			_Show(control, control.RectangleToScreen(anchor));
		}

		/// <summary>
		/// Shows the info window below or above the anchor rectangle.
		/// </summary>
		/// <param name="anchor">Rectangle in screen.</param>
		/// <remarks>
		/// The info window is top-most.
		/// </remarks>
		public void Show(Rectangle anchor)
		{
			_Show(null, anchor);
		}

		void _Show(Control anchor, Rectangle ra)
		{
			_w.SetRect(ra, (ra.Right, ra.Top), Size);
			_w.ShowAt(anchor);
		}

		/// <summary>
		/// Hides the info window.
		/// Does not dispose.
		/// </summary>
		public void Hide()
		{
			_w.Hide();
		}

		/// <summary>
		/// Destroys the info window.
		/// Then it cannot be shown again.
		/// </summary>
		public void Dispose()
		{
			_w.Dispose();
		}

		/// <summary>
		/// If set to a non-null string before creating the info window, the window will be with caption and resizable.
		/// </summary>
		public string Caption {
			get => _caption;
			set => _w.Text = _caption = value;
		}
		string _caption;

		class _Window : Form
		{
			InfoWindow _t;
			Control _owner;
			Font _font;
			bool _showedOnce;

			public _Window(InfoWindow t)
			{
				_t = t;

				this.SuspendLayout();
				this.AutoScaleMode = AutoScaleMode.None;
				this.Font = _font = Util.AFonts.Regular;
				this.StartPosition = FormStartPosition.Manual;
				this.FormBorderStyle = FormBorderStyle.None;
				this.Text = "Au.InfoWindow";
				this.ResumeLayout();
			}

			protected override void Dispose(bool disposing)
			{
				if(disposing) {
					_font.Dispose();
				}
				base.Dispose(disposing);
			}

			public void ShowAt(Control anchor)
			{
				var owner = anchor?.TopLevelControl;
				bool changedOwner = false;
				if(_showedOnce) {
					changedOwner = owner != _owner;
					if(Visible) {
						if(!changedOwner) return;
						Visible = false;
					}
				}
				_owner = owner;

				if(_owner != null) {
					Show(_owner);
					if(changedOwner) ((AWnd)this).ZorderAbove((AWnd)_owner);
				} else {
					Show(); //note: not the same as Show(null)
					if(changedOwner) ((AWnd)this).ZorderTopmost();
				}
				_showedOnce = true;
			}

			public void SetRect(RECT exclude, POINT pos, SIZE size)
			{
				Api.CalculatePopupWindowPosition(pos, size, 0, exclude, out var r);
				Bounds = r;
			}

			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					var st = WS.POPUP;
					var es = WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE;
					bool noShadow = false;
					if(_t != null) {
						if(_owner == null) es |= WS_EX.TOPMOST;
						if(_t.Caption != null) {
							noShadow = true;
							st |= WS.CAPTION | WS.SYSMENU | WS.THICKFRAME;
						}
					}
					p.Style = unchecked((int)st);
					p.ExStyle = (int)es;
					if(!noShadow) p.ClassStyle |= (int)Api.CS_DROPSHADOW;
					return p;
				}
			}

			protected override bool ShowWithoutActivation => true;

			protected override void WndProc(ref Message m)
			{
				//AWnd.More.PrintMsg(m);

				switch(m.Msg) {
				case Api.WM_MOUSEACTIVATE:
					m.Result = (IntPtr)Api.MA_NOACTIVATE;
					if(AMath.HiShort(m.LParam) == Api.WM_MBUTTONDOWN) Hide();
					return;
					//case Api.WM_ACTIVATEAPP:
					//	if(m.WParam == default && !_t.DontCloseWhenAppDeactivated) _t._Close();
					//	break;
				}

				base.WndProc(ref m);
			}

			protected override void OnFormClosing(FormClosingEventArgs e)
			{
				if(e.CloseReason == CloseReason.UserClosing) {
					e.Cancel = true;
					Hide();
				}
				base.OnFormClosing(e);
			}

			//protected override void OnClientSizeChanged(EventArgs e)
			//{
			//	Print("OnClientSizeChanged");
			//	base.OnClientSizeChanged(e);
			//}
		}

		class _Control : InfoBox
		{
			InfoWindow _t;

			public _Control(InfoWindow t, int left, int splitPos)
			{
				_t = t;
				this.InitUseControlFont = true;
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
				if(_t.Caption == null) this.InitBorderStyle = BorderStyle.FixedSingle;
				base.OnHandleCreated(e);
				Call(Sci.SCI_SETMARGINLEFT, 0, 4);
				Call(Sci.SCI_SETMARGINRIGHT, 0, 4);
				//base.NoMouseLeftSetFocus = true; //no, then cannot scroll with wheel on Win7-8.1
				//base.NoMouseRightSetFocus = true;
			}
		}
	}
}
