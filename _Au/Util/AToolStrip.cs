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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;

#pragma warning disable 1591 //XML doc

namespace Au.Util
{
	/// <summary>
	/// Adds to ToolStrip:
	/// 1. A click works when the window is not the active window.
	/// 2. A click does not activate the window if it has WS_EX_NOACTIVATE style.
	/// 3. Show tooltips when the window is not the active window if it has WS_EX_NOACTIVATE style.
	/// </summary>
	public class AToolStrip : ToolStrip
	{
		//these are used for tooltip
		AWnd _tt;
		ToolStripItem _ttItem;
		ATimer _ttTimer;
		bool _ttUpdate;
		bool _ttHide;
		int _ttX, _ttY;

		[Browsable(false)]
		protected override Size DefaultSize => ADpi.ScaleSize((500, 25));
		//because of default width 100, child text boxes (default width 100) are temporarily moved to the overflow,
		//	then after setting normal size are moved back, making everything slower etc

		[Browsable(false)]
		protected override bool DefaultShowItemToolTips => false;
		//tested: We will not have 2 tooltip windows created. ToolStrip does not create its own tooltip window until it shows a tooltip.

		void _HideTooltip()
		{
			if(_ttItem == null) return;
			_tt.Send(TTM_TRACKACTIVATE);
			_ttItem = null;
		}

		void _ShowTooltip(MouseEventArgs e, Control inControl)
		{
			int x = e.X, y = e.Y;
			if(x == _ttX && y == _ttY) return;
			_ttX = x; _ttY = y;

			var w = (AWnd)TopLevelControl;
			if(!w.HasExStyle(WS2.NOACTIVATE) && !w.IsActive) return;

			if(inControl != null) (x, y) = this.MouseClientXY();
			ToolStripItem b1 = this.GetItemAt(x, y);
			if(b1 == _ttItem) return;

			string text = b1?.ToolTipText;
			if(text.NE()) {
				_HideTooltip();
				return;
			}

			if(_tt.Is0) {
				_tt = AWnd.More.CreateWindow("tooltips_class32", null, WS.POPUP | TTS_NOPREFIX | TTS_ALWAYSTIP, WS2.TOOLWINDOW | WS2.NOACTIVATE | WS2.TOPMOST, parent: (AWnd)this);
				_tt.Send(TTM_SETMAXTIPWIDTH, 0, 1000);
			}

			int delay = (int)_tt.Send(TTM_GETDELAYTIME, _ttItem == null ? TTDT_INITIAL : TTDT_RESHOW);
			_HideTooltip();
			_ttItem = b1;
			_ttHide = false;
			_ttTimer ??= new ATimer(t => {
				if(_ttItem == null) return;
				if(_ttHide ^= true) {
					_ShowTooltipNow();
					t.After((int)_tt.Send(TTM_GETDELAYTIME, TTDT_AUTOPOP));
				} else if(_ttItem != null) {
					_tt.Send(TTM_TRACKACTIVATE);
				}
			});
			_ttTimer.After(delay);
		}

		unsafe void _ShowTooltipNow()
		{
			var p = AMouse.XY;
			fixed(char* ptext = _ttItem.ToolTipText) {
				var k = new TTTOOLINFO { cbSize = sizeof(TTTOOLINFO), uFlags = TTF_TRACK, lpszText = ptext };
				_tt.Send(_ttUpdate ? TTM_UPDATETIPTEXT : TTM_ADDTOOL, 0, &k); _ttUpdate = true;
				_tt.Send(TTM_TRACKPOSITION, 0, AMath.MakeUint(p.x, p.y + 20));
				_tt.Send(TTM_TRACKACTIVATE, 1, &k);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_ShowTooltip(e, null);
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_HideTooltip();
			base.OnMouseLeave(e);
		}

		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			_ShowTooltip(e, sender as Control);
		}

		private void Control_MouseLeave(object sender, EventArgs e)
		{
			_HideTooltip();
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			//need this to show tooltip for child controls, eg TextBox hosted by ToolStripTextBox
			e.Control.MouseMove += Control_MouseMove;
			e.Control.MouseLeave += Control_MouseLeave;
			base.OnControlAdded(e);
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			e.Control.MouseMove -= Control_MouseMove;
			e.Control.MouseLeave -= Control_MouseLeave;
			base.OnControlRemoved(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			_HideTooltip();
			base.OnVisibleChanged(e);
		}

		//protected override void OnParentVisibleChanged(EventArgs e)
		//{
		//	//.NET bug: called when parent becomes visible, but not when becomes hidden. OnVisibleChanged too.
		//	_HideTooltip();
		//	base.OnParentVisibleChanged(e);
		//}

		internal const int TTM_SETMAXTIPWIDTH = 0x418;
		internal const int TTM_GETDELAYTIME = 0x415;
		internal const int TTM_ADDTOOL = 0x432;
		internal const int TTM_UPDATETIPTEXT = 0x439;
		internal const int TTM_TRACKPOSITION = 0x412;
		internal const int TTM_TRACKACTIVATE = 0x411;

		internal const WS TTS_NOPREFIX = (WS)0x2;
		internal const WS TTS_ALWAYSTIP = (WS)0x1;
		internal const int TTDT_RESHOW = 1;
		internal const int TTDT_AUTOPOP = 2;
		internal const int TTDT_INITIAL = 3;
		internal const uint TTF_TRACK = 0x20;

		internal unsafe struct TTTOOLINFO
		{
			public int cbSize;
			public uint uFlags;
			public AWnd hwnd;
			public LPARAM uId;
			public RECT rect;
			public IntPtr hinst;
			public char* lpszText;
			public LPARAM lParam;
			public IntPtr lpReserved;
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)(((AWnd)TopLevelControl).HasExStyle(WS2.NOACTIVATE) ? Api.MA_NOACTIVATE : Api.MA_ACTIVATE);
				return;
			case Api.WM_DESTROY:
				//dispose tooltip and prepare to work again if recreating handle
				if(!_tt.Is0) { Api.DestroyWindow(_tt); _tt = default; }
				_ttTimer?.Stop(); _ttTimer = null;
				_ttItem = null;
				_ttUpdate = false;
				break;
			}

			base.WndProc(ref m);
		}
	}

	/// <summary>
	/// Adds to MenuStrip:
	/// 1. User can click an item when the parent form is not the active window.
	/// 2. A click does not activate the parent form if it has WS_EX_NOACTIVATE style.
	/// 3. Fixes some MenuStrip bugs.
	/// </summary>
	public class AMenuStrip : MenuStrip
	{
		//MenuStrip bug workaround.
		//If parent uses non-default font, this is called before creating control handle.
		//Then base.OnFontChanged creates parked control + drop-down etc.
		protected override void OnFontChanged(EventArgs e)
		{
			//ADebug.PrintFunc();
			if(!IsHandleCreated) return;
			base.OnFontChanged(e);
		}

		protected override void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m);
			//LPARAM WP = m.WParam, LP = m.LParam;

			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)(((AWnd)this.TopLevelControl).HasExStyle(WS2.NOACTIVATE) ? Api.MA_NOACTIVATE : Api.MA_ACTIVATE);
				return;
			}

			base.WndProc(ref m);
		}
	}
}
