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
using static Au.NoClass;

public class PopupList2
{
	_Window _w;
	_Control _c;

	public PopupList2()
	{
		_w = new _Window(this);
		_c = new _Control(this);

		_w.Controls.Add(_c);
	}

	public void Show(Control anchor)
	{
		var ra = ((Wnd)anchor).Rect;
		var r = new Rectangle(0, 0, 250, 300);
		_c.Bounds = r;
		//var r2=new Rectangle(0, 0, 150, 100);_c.Bounds = r2; //test
		_w.ClientSize = r.Size;
		r.Offset(ra.left, ra.bottom);
		_w.Bounds = r;

		var owner = anchor.FindForm();

		_c.VirtualListSize = Items.Length;

		_w.Show(owner);
	}

	public string[] Items { get; set; }

	class _Window : Form
	{
		PopupList2 _p;

		public _Window(PopupList2 p)
		{
			_p = p;

			this.SuspendLayout();
			this.Font = SystemFonts.MessageBoxFont;
			this.AutoScaleMode = AutoScaleMode.None;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;

			//this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
			//this.MinimumSize = new Size(34, 30);
			this.ResumeLayout();
		}

		protected override CreateParams CreateParams {
			get {
				var p = base.CreateParams;
				p.Style = unchecked((int)(Native.WS.POPUP | Native.WS.CLIPCHILDREN));
				p.ExStyle = (int)(Native.WS_EX.TOOLWINDOW | Native.WS_EX.NOACTIVATE);
				p.ClassStyle |= (int)Api.CS_DROPSHADOW;
				return p;
			}
		}

		/// <summary>
		/// 1. Prevents activating window when showing. 2. Allows to show ToolTip for inactive window.
		/// </summary>
		protected override bool ShowWithoutActivation => true;

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)(((int)m.LParam >> 16 == Api.WM_LBUTTONDOWN) ? Api.MA_NOACTIVATE : Api.MA_NOACTIVATEANDEAT);
				return;
			}

			base.WndProc(ref m);
		}
	}

	class _Control : ListView
	{
		PopupList2 _p;
		ListViewItem _lvi;

		public _Control(PopupList2 p)
		{
			_p = p;

			this.BorderStyle = BorderStyle.FixedSingle;
			this.SetStyle(ControlStyles.Selectable, false); //does not work

			View = View.Details;
			HeaderStyle = ColumnHeaderStyle.None;
			VirtualMode = true;
			MultiSelect = false;
			FullRowSelect = true;
			AutoArrange = false;
			HotTracking = true;
			ShowItemToolTips = true;
			Columns.Add(null, 200);
			_lvi = new ListViewItem();


		}

		protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
		{
			_lvi.Text = _p.Items[e.ItemIndex];
			e.Item = _lvi;
			base.OnRetrieveVirtualItem(e);
		}

		protected override void WndProc(ref Message m)
		{
			//switch(m.Msg) {
			//case Api.
			//	return;
			//}

			base.WndProc(ref m);
		}

		//protected override void OnMouseDown(MouseEventArgs e)
		//{
		//	base.OnMouseDown(e);
		//}
	}
}
