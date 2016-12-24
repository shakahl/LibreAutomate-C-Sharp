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
	partial class GDockPanels
	{
		/// <summary>
		/// Floating parent form of a GPanel.
		/// </summary>
		class GFloat :Form
		{
			protected GDockPanels _manager;
			GContentNode _gc;

			internal GFloat(GDockPanels manager, GContentNode gc)
			{
				_manager = manager;
				_gc = gc;

				this.SuspendLayout();
				this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.ShowIcon = false;
				this.ShowInTaskbar = false;
				this.StartPosition = FormStartPosition.Manual;
				this.ControlBox = false;
				this.Text = "";
				this.ResumeLayout(false);
			}

			protected override void WndProc(ref Message m)
			{
				if(_manager._WndProcBefore_Common(this, ref m)) return;

				base.WndProc(ref m);

				//switch((uint)m.Msg) {
				////case Api.WM_:
				////	break;
				//}
			}

			protected override void OnClientSizeChanged(EventArgs e)
			{
				//OutFunc();
				var r = this.ClientRectangle;
				if(!r.IsEmpty) _gc.UpdateLayout(r);
				base.OnClientSizeChanged(e);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				_manager._OnPaint_Common(e);
				_gc.Paint(e.Graphics);

				base.OnPaint(e);
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				_manager._OnMouseDown_Common(this, e);
				base.OnMouseDown(e);
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				_manager._OnMouseUp_Common(this, e);
				base.OnMouseUp(e);
			}
		}
	}
}
