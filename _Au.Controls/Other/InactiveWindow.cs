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

using Au;
using Au.Types;

namespace Au.Controls
{
	/// <summary>
	/// Form that is used as an inactive popup window.
	/// </summary>
	/// <remarks>
	/// Ctor sets AutoScaleMode=None, StartPosition=Manual, FormBorderStyle=None.
	/// Hides on mouse middle click.
	/// </remarks>
	public class InactiveWindow : Form
	{
		WS _style;
		WS2 _exStyle;
		bool _shadow;
		Control _owner;
		bool _wasCtor, _wasShow;

		public InactiveWindow(WS style = WS.POPUP | WS.THICKFRAME, WS2 exStyle = WS2.TOOLWINDOW | WS2.NOACTIVATE, bool shadow = false)
		{
			_style = style;
			_exStyle = exStyle;
			_shadow = shadow;

			this.SuspendLayout();
			this.AutoScaleMode = AutoScaleMode.None;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.ResumeLayout();

			_wasCtor = true;
		}

		protected override CreateParams CreateParams {
			get {
				var p = base.CreateParams;
				if(_wasCtor) {
					//AOutput.Write((WS)p.Style, (WS2)p.ExStyle);
					p.Style = (int)_style;
					var es = _exStyle;
					if(_owner == null) es |= WS2.TOPMOST;
					p.ExStyle = (int)es;
					if(_shadow && !_style.Has(WS.THICKFRAME)) p.ClassStyle |= (int)Api.CS_DROPSHADOW;
				}
				return p;

				//note: if WS_CLIPCHILDREN, often at startup briefly black until control finished painting
			}
		}

		protected override bool ShowWithoutActivation => true;

		protected override unsafe void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_NCMOUSEMOVE);
			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				switch(AMath.HiShort(m.LParam)) {
				case Api.WM_MBUTTONDOWN:
					Hide(); //never mind: we probably don't receive this message if our thread is inactive
					m.Result = (IntPtr)Api.MA_NOACTIVATEANDEAT;
					break;
				default:
					m.Result = (IntPtr)Api.MA_NOACTIVATE;
					break;
				}
				return;
			case Api.WM_NCLBUTTONDOWN:
				var wa = AWnd.ThisThread.Active;
				if(wa != default && wa.Handle != m.HWnd) {
					var h = m.HWnd;
					using(AHookWin.ThreadCbt(d => d.code == HookData.CbtEvent.ACTIVATE && d.ActivationInfo(out _, out _).Handle == h))
						base.WndProc(ref m);
					return;
				}
				break;
			}
			//Somehow OS ignores WS_EX_NOACTIVATE if the active window is of this thread. Workaround: on WM_MOUSEACTIVATE return MA_NOACTIVATE.
			//Also then activates when clicked in non-client area, eg when moving or resizing. Workaround: on WM_NCLBUTTONDOWN suppress activation with a CBT hook.
			//When moving or resizing, WM_NCLBUTTONDOWN returns when moving/resizing ends. On resizing would activate on mouse button up.

			base.WndProc(ref m);

			switch(m.Msg) {
			case Api.WM_SHOWWINDOW:
				if(m.WParam==default) ZHiddenOrDestroyed?.Invoke(false);
				break;
			case Api.WM_DESTROY:
				ZHiddenOrDestroyed?.Invoke(true);
				break;
			}
		}

		/// <summary>
		/// Calls API <msdn>CalculatePopupWindowPosition</msdn> and sets <b>Bounds</b>.
		/// </summary>
		public void ZCalculateAndSetPosition(int x, int y, PopupAlignment align, Rectangle excludeRect = default, Size? size = null)
		{
			Api.CalculatePopupWindowPosition((x, y), size ?? this.Size, (uint)align, excludeRect, out var r);
			Bounds = r;
		}

		/// <summary>
		/// Calls API <msdn>CalculatePopupWindowPosition</msdn> and sets <b>Bounds</b>.
		/// The window by default will be at the right or left side of <i>anchorRect</i>. Use <b>PopupAlignment.TPM_VERTICAL</b> if need below or above.
		/// </summary>
		public void ZCalculateAndSetPosition(Rectangle anchorRect, PopupAlignment align, Size? size = null)
		{
			int x = align.HasAny(PopupAlignment.TPM_VERTICAL | PopupAlignment.TPM_RIGHTALIGN) ? anchorRect.Left : anchorRect.Right;
			int y = align.Has(PopupAlignment.TPM_VERTICAL) && !align.Has(PopupAlignment.TPM_BOTTOMALIGN) ? anchorRect.Bottom : anchorRect.Top;
			ZCalculateAndSetPosition(x, y, align, anchorRect, size);
		}

		/// <summary>
		/// Shows the window.
		/// </summary>
		/// <param name="ownerControl">Owner form or control or null.</param>
		public virtual void ZShow(Control ownerControl)
		{
			var owner = ownerControl?.TopLevelControl;
			bool changedOwner = false;
			if(_wasShow) {
				changedOwner = owner != _owner;
				if(Visible) {
					if(!changedOwner) return;
					Visible = false;
				}
			}

			_wasShow = true;
			_owner = owner;

			if(_owner != null) {
				var zorderAbove = ((AWnd)_owner).Get.EnabledOwned(orThis: changedOwner);
				Show(_owner);
				if(!zorderAbove.Is0) ((AWnd)this).ZorderAbove(zorderAbove);
			} else {
				Show(); //note: not the same as Show(null)
				if(changedOwner) ((AWnd)this).ZorderTopmost();
			}
		}

#if DEBUG
		protected override void CreateHandle()
		{
			Debug.Assert(_wasShow); //some child controls may create handle before ZShow if used incorrectly, eg WebBrowser control if navigated before creating handle
			base.CreateHandle();
		}
#endif

		/// <summary>
		/// When the window is hidden or destroyed.
		/// The bool parameter is true if destroyed.
		/// </summary>
		public event Action<bool> ZHiddenOrDestroyed;
	}

	[Flags]
	public enum PopupAlignment
	{
		TPM_CENTERALIGN = 0x4,
		TPM_RIGHTALIGN = 0x8,
		TPM_VCENTERALIGN = 0x10,
		TPM_BOTTOMALIGN = 0x20,
		TPM_VERTICAL = 0x40,
		TPM_WORKAREA = 0x10000,
	}
}
