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
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

#pragma warning disable 1591 //XML doc

namespace Au.Controls
{
	/// <summary>
	/// Adds to ToolStrip:
	/// 1. User can click a button when the parent form is not the active window.
	/// 2. A click does not activate the parent form if it has WS_EX_NOACTIVATE style.
	/// 3. Can show tooltips when parent form is not the active window. To enable it, the form's ShowWithoutActivation override must return true (it is called when need to show a tooltip).
	/// </summary>
	//[DebuggerStepThrough]
	public class AuToolStrip :ToolStrip
	{
		//these are used for tooltip
		ToolTip _ttToolTip;
		ToolStripItem _ttItem; //the last mouse move event hit test result
		Timer_ _ttTimer;

		public AuToolStrip()
		{
			this.ShowItemToolTips = false;
			this.Width = 500; //because of default width 100, child text boxes (default width 100) are temporarily moved to the overflow, then after setting normal size are moved back, making everything slower etc

			//tested: We will not have 2 tooltip windows created. ToolStrip does not create its own tooltip window until it shows a tooltip.
		}

		protected override void Dispose(bool disposing)
		{
			if(_ttTimer != null) { _ttTimer.Stop(); _ttTimer = null; }
			if(_ttToolTip != null) { _ttToolTip.Dispose(); _ttToolTip = null; }
			base.Dispose(disposing);
		}

		Control _TopLevelParent { get { var R = this.TopLevelControl; Debug.Assert(R != null); return R; } }

		void _HideTooltip()
		{
			if(_ttItem == null) return;
			_ttToolTip.Hide(_TopLevelParent);
			_ttItem = null;
		}

		bool _ShowTooltip()
		{
			ToolStripItem b1 = this.GetItemAt(this.MouseClientXY_()); if(b1 == null) return false;
			var s1 = b1.ToolTipText; if(Empty(s1)) return false; //null if separator
			if(b1 == _ttItem) return true;
			if(_ttToolTip == null) _ttToolTip = new ToolTip();
			int delay = (_ttItem == null) ? _ttToolTip.InitialDelay : _ttToolTip.ReshowDelay;
			_HideTooltip();
			_ttItem = b1;
			if(_ttTimer == null) _ttTimer = new Timer_(t =>
			{
				if(_ttItem == null) return;
				_ttToolTip.Hide(_TopLevelParent);
				var par = _TopLevelParent;
				var p = par.MouseWindowXY_();
				_ttToolTip.Show(_ttItem.ToolTipText, par, p.x, p.y + 20, 5000);
				//info: why here we use _TopLevelParent (not this ToolStrip Control):
				//	Shortly: to enable tooltip in inactive form.
				//	Show() shows tooltip only when 1 or 2 are true:
				//		1. window.ShowWithoutActivation (override) returns true. Only Form has this override, Control doesn't.
				//		2. window.TopLevelControl is the active window.
				//	See ToolTip.Show source code.
				//	Could instaed use native tooltip control directly, but much more work.
			});
			_ttTimer.Start(delay, true);
			return true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(!_ShowTooltip()) _HideTooltip();
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_HideTooltip();
			base.OnMouseLeave(e);
		}

		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			if(!_ShowTooltip()) _HideTooltip();
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

		protected override void WndProc(ref Message m)
		{
			//Wnd.Misc.PrintMsg(ref m);
			//LPARAM WP = m.WParam, LP = m.LParam;

			switch((uint)m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)(((Wnd)_TopLevelParent).HasExStyle(Native.WS_EX.NOACTIVATE) ? Api.MA_NOACTIVATE : Api.MA_ACTIVATE);
				return;
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
	public class AuMenuStrip :MenuStrip
	{

		//MenuStrip bug workaround.
		//If parent uses non-default font, this is called before creating control handle.
		//Then base.OnFontChanged creates parked control + drop-down etc.
		protected override void OnFontChanged(EventArgs e)
		{
			//DebugPrintFunc();
			if(!IsHandleCreated) return;
			base.OnFontChanged(e);
		}

		protected override void WndProc(ref Message m)
		{
			//Wnd.Misc.PrintMsg(ref m);
			//LPARAM WP = m.WParam, LP = m.LParam;

			switch((uint)m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)(((Wnd)this.TopLevelControl).HasExStyle(Native.WS_EX.NOACTIVATE) ? Api.MA_NOACTIVATE : Api.MA_ACTIVATE);
				return;
			}

			base.WndProc(ref m);
		}
	}
}
