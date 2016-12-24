using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Drawing.Drawing2D;
using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	public partial class GPanel :Panel
	{
		protected GPanels _manager;
		protected Splitter _splitter;
		protected Floaty _floaty;

		const int _colorCaption = 0x7D5A46;
		//const int _colorCaptionMD = 0x608060;
		//const int _colorSplitter = 0xE5D6CF;

		public GPanel(GPanels manager, DockStyle dockStyle)
		{
			_manager = manager;

			_splitter = new Splitter();
			//_splitter.BackColor = ColorTranslator.FromWin32(_colorSplitter); //don't
			_splitter.Size = new Size(4, 4); //default is 3
			_splitter.MinExtra = _splitter.MinSize = 15; //default 25

			Dock = dockStyle;
			//OnX will set splitter properties

			_floaty = new Floaty(this);

			if(this is MultiGPanel) _manager.MultiPanels.Add(this as MultiGPanel); else _manager.Panels.Add(this);
		}

		//public Splitter DPSplitter { get { return _splitter; } }

		/// <summary>
		/// Is docked outside the main area, eg a toolstrip.
		/// </summary>
		public bool DockOutside { get; set; }

		/// <summary>
		/// Is currently floating.
		/// </summary>
		public bool IsFloating { get { return _floaty.IsFloating; } }

		/// <summary>
		/// Is currently caption vertical (depends on width/height).
		/// </summary>
		public bool IsVerticalCaption { get; private set; }

		/// <summary>
		/// Gets caption height if horizontal, width if vertical.
		/// </summary>
		public int CaptionHeight
		{
			get
			{
				if(_captionHeight == 0) {
					_captionHeight = (this is MultiGPanel) ? 10 : 19;
				}
				return _captionHeight;
			}
		}
		int _captionHeight;

		/// <summary>
		/// Adds this and splitter to parent Controls.
		/// </summary>
		/// <param name="parent"></param>
		public void AddToParent(Control parent)
		{
			if(_splitter != null) parent.Controls.Add(_splitter);
			parent.Controls.Add(this);
		}

		/// <summary>
		/// Shows this and, if floating, the floating parent.
		/// </summary>
		public new void Show()
		{
			_floaty.Show();
			base.Show();
			//OnX will set splitter

			//recursively show parent MultiGPanel
			var m = this.Parent as MultiGPanel;
			if(m != null && !m.Visible) m.Show();
		}

		/// <summary>
		/// Hides this and, if floating, the floating parent.
		/// </summary>
		public new void Hide()
		{
			_floaty.Hide();
			base.Hide();
			//OnX will set splitter

			//recursively hide parent MultiGPanel that don't have visible child panels
			var m = this.Parent as MultiGPanel;
			if(m != null && 0==m.VisibleChildPanelCount) m.Hide();
		}

		/// <summary>
		/// Makes this floating if docked.
		/// </summary>
		public void MakeFloating()
		{
			_floaty.MakeFloating(false);
		}

		/// <summary>
		/// Makes this docked if floating.
		/// </summary>
		public void MakeDocked()
		{
			_floaty.MakeDocked(false);
		}

		/// <summary>
		/// When state changed from floating to docked.
		/// </summary>
		public event EventHandler MadeDocked;

		/// <summary>
		/// When state changed from docked to floating.
		/// </summary>
		public event EventHandler MadeFloating;

		/// <summary>
		/// Shows the context menu.
		/// </summary>
		public void ShowContextMenu()
		{
			ContextMenu m = null;
			var mFloat = new MenuItem("Float\tDClick, Drag", (unu, sed) => MakeFloating());
			var mDock = new MenuItem("Dock\tDClick, Alt+Drag", (unu, sed) => MakeDocked());
			if(IsFloating) mFloat.Enabled = false; else mDock.Enabled = false;
			m = new ContextMenu(new MenuItem[]
				{
						mFloat,
						mDock,
						new MenuItem("test", (unu, sed) => Out("item"))
				});
			m.Show(this, this.PointToClient(Mouse.XY));
			Application.DoEvents(); m.Dispose();
		}

		protected override void OnParentChanged(EventArgs e)
		{
			if(_splitter != null && this.Parent != null) {
				//if(Name == "Output") OutList("OnParentChanged", Name, Visible);
				_splitter.Parent = this.Parent;
			}

			base.OnParentChanged(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			if(_splitter != null) {
				//if(Name == "Output") OutList("OnVisibleChanged", Name, Visible);
				if(this.Visible) {
					if(_IsDockStyleLTRB(this.Dock)) _splitter.Visible = true;
				} else {
					_splitter.Visible = false;
				}
			}

			base.OnVisibleChanged(e);
		}

		protected override void OnDockChanged(EventArgs e)
		{
			if(_splitter != null) {
				//if(Name=="Output") OutList("OnDockChanged", Name, Visible);
				if(_IsDockStyleLTRB(this.Dock)) {
					_splitter.Dock = this.Dock;
					_splitter.Visible = this.Visible;
				} else {
					_splitter.Visible = false;
					//_splitter.Dock = DockStyle.None;
				}
			}

			base.OnDockChanged(e);
		}

		void _OnZorderChanged()
		{
			if(_splitter != null && IsHandleCreated) {
				//if(Name == "Output") OutList("OnZorderChanged", Name, Visible);
				if(DockOutside) ((Wnd)_splitter).ZorderAfter((Wnd)this);
				else ((Wnd)_splitter).ZorderBefore((Wnd)this);
			}
		}

		protected override unsafe void WndProc(ref Message m)
		{
			//if(this.Name=="Open") Util.Debug_.OutMsg(ref m);

			switch((uint)m.Msg) {
			case Api.WM_NCCALCSIZE:
				base.WndProc(ref m);
				_OnNcCalcSize(ref *(RECT*)m.LParam);
				return;

			case Api.WM_NCPAINT:
				_OnNcPaint();
				return;

			case Api.WM_NCHITTEST:
				m.Result = (IntPtr)Api.HTCAPTION;
				return;

			case Api.WM_SYSCOMMAND:
				if(((uint)m.WParam & 0xfff0) == Api.SC_MOVE) {
					if(IsFloating || _Api.DragDetect((Wnd)this, Mouse.XY)) { //without DetectDrag starts immediately on mouse down, but only if child window
						_floaty.MakeFloating(true);
					}
					return;
				}
				break;

			case Api.WM_NCRBUTTONUP:
				ShowContextMenu();
				break;

			case Api.WM_NCLBUTTONDBLCLK:
				if(IsFloating) MakeDocked(); else MakeFloating();
				break;

			case Api.WM_WINDOWPOSCHANGED: {
					var u = (Api.WINDOWPOS*)m.LParam;
					if((u->flags & Api.SWP_NOZORDER) == 0) _OnZorderChanged();
				}
				break;
			}

			base.WndProc(ref m);
		}

		void _OnNcCalcSize(ref RECT r)
		{
			var h = CaptionHeight;
			if(h == 0) return; //MultiGPanel

			if(h > 10 && (r.Width <= h || r.Height <= h)) IsVerticalCaption = r.Width < r.Height; //show more caption
			else IsVerticalCaption = r.Width > r.Height + h; //show more content

			if(IsVerticalCaption) {
				r.left += h; if(r.left > r.right) r.left = r.right;
			} else {
				r.top += h; if(r.top > r.bottom) r.top = r.bottom;
			}
		}

		void _OnNcPaint()
		{
			var h = CaptionHeight;
			if(h == 0) return; //MultiGPanel

			//OutFunc();
			Wnd w = (Wnd)this;
			RECT r = w.Rect; r.Offset(-r.left, -r.top);
			bool vert = IsVerticalCaption;
			if(vert) r.right = h; else r.bottom = h;

			var dc = _Api.GetWindowDC(w); if(dc == Zero) return;
			var g = Graphics.FromHdc(dc);

			var bBack = new SolidBrush(ColorTranslator.FromWin32(/*(this is MultiGPanel) ? _colorCaptionMD :*/ _colorCaption));
			g.FillRectangle(bBack, r);
			bBack.Dispose();

			var s = this.Text;
			if(s.Length > 0) {
				//var bFore = new SolidBrush(Color.Black);
				var bFore = new SolidBrush(Color.White);
				var tff = StringFormatFlags.NoWrap; if(vert) tff |= StringFormatFlags.DirectionVertical;
				var txtFormat = new StringFormat(tff);
				g.DrawString(s, this.Font, bFore, 1, 1, txtFormat);
				bFore.Dispose();
				txtFormat.Dispose();
				//info: TextRenderer.DrawText cannot draw vertical.
				//tested: with "Segoe UI" font same quality as TextRenderer.DrawText or ExtTextOut.
				//note: don't use rectangle. Then does not draw partially clipped characters; result: no text after increasing window size.
			}

			g.Dispose();
			_Api.ReleaseDC(w, dc);
		}

		RECT _CaptionRectInScreen
		{
			get
			{
				RECT r = ((Wnd)this).Rect;
				bool vert = IsVerticalCaption;
				var h = CaptionHeight;
				if(vert) r.right = r.left + h; else r.bottom = r.top + h;
				return r;
			}
		}

		/// <summary>
		/// Returns true if DockStyle is Left, Top, Right or Bottom (not Fill or None).
		/// </summary>
		static bool _IsDockStyleLTRB(DockStyle ds)
		{
			return !(ds == DockStyle.Fill || ds == DockStyle.None);
		}

		//TODO: current code is just to debug
		public override string ToString()
		{
			var f = _floaty;
			return $"{Name}  Visible={Visible}, IsFloating={IsFloating}, Dock={Dock}, state=<{f._dockState.dockStyle}, {f._dockState.bounds}>";
		}
	}
}
