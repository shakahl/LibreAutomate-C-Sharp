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
using TheArtOfDev.HtmlRenderer.WinForms;
using Au.Controls;
using TheArtOfDev.HtmlRenderer.Core.Entities;

class CiPopupHtml
{
	_Window _w;
	_HtmlPanel _html;
	CiCompletion _compl;
	CiComplItem _ci;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public Form PopupWindow => _w;

	public CiPopupHtml()
	{
		_w = new _Window(this);
		_w.SuspendLayout();
		_w.Size = new Size(Screen.FromControl(Program.MainForm).WorkingArea.Width / 3, Au.Util.ADpi.ScaleInt(300));

		_html = new _HtmlPanel();
		_html.SuspendLayout();
		_html.AccessibleName = _html.Name = "Codein_info";
		_html.Dock = DockStyle.Fill;
		//_html.Font = EdStock.FontRegular;
		_html.BaseStylesheet = CiUtil.TaggedPartsHtmlStyleSheet;
		_html.BackColor = Color.LightYellow;
		_html.LinkClicked += _html_LinkClicked;

		_w.Controls.Add(_html);
		_html.ResumeLayout();
		_w.ResumeLayout(true);
	}

	public void SetHtml(string html, CiCompletion compl = null, CiComplItem ci = null)
	{
		_html.Text = html;
		_compl = compl;
		_ci = ci;
	}

	public void Show(SciCode doc, int position)
	{
		var r = CiUtil.GetCaretRectFromPos(doc, position);
		r.Inflate(100, 10);
		_w.ShowAt(doc, new Point(r.Left, r.Bottom), r, 0);
	}

	public void Show(SciCode doc, Rectangle exclude)
	{
		_w.ShowAt(doc, new Point(exclude.Right, exclude.Top), exclude, 0);
	}

	public void Hide()
	{
		_w.Hide();
		_html.Text = null;
		_compl = null;
		_ci = null;
	}

	private void _html_LinkClicked(object sender, HtmlLinkClickedEventArgs e)
	{
		if(e.Link.Starts('^')) {
			e.Handled = true;
			int i = e.Link.ToInt(1);
			var html = _compl.GetDescriptionHtml(_ci, i);
			_html.Text = html;
		} else if(e.Link.Has("/a.html#")) {
			e.Handled = true;
			CiUtil.OpenSymbolSourceUrl(e.Link);
		}
	}

	class _Window : Form
	{
		CiPopupHtml _p;
		Control _owner;
		bool _showedOnce;

		public _Window(CiPopupHtml p)
		{
			_p = p;

			this.AutoScaleMode = AutoScaleMode.None;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Text = "Au.CiPopupHtml";
			this.MinimumSize = Au.Util.ADpi.ScaleSize((150, 150));
			this.Font = Au.Util.AFonts.Regular;
		}

		protected override CreateParams CreateParams {
			get {
				var p = base.CreateParams;
				p.Style = unchecked((int)(WS.POPUP | WS.THICKFRAME));
				p.ExStyle = (int)(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE);
				return p;
			}
		}

		protected override bool ShowWithoutActivation => true;

		public void ShowAt(Control c, Point pos, Rectangle exclude, PopupAlignment align)
		{
			Api.CalculatePopupWindowPosition(pos, this.Size, (uint)align, exclude, out var r);
			Bounds = r;

			var owner = c.TopLevelControl;
			bool changedOwner = false;
			if(_showedOnce) {
				changedOwner = owner != _owner;
				if(Visible) {
					if(!changedOwner) return;
					Visible = false;
				}
			}
			_owner = owner;

			Show(_owner);
			if(changedOwner) ((AWnd)this).ZorderAbove((AWnd)_owner);
			_showedOnce = true;
		}

		protected override void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m);

			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)Api.MA_NOACTIVATE;
				return;
				//case Api.WM_ACTIVATEAPP:
				//	if(m.WParam == default) _p.Hide();
				//	break;			}
			}

			base.WndProc(ref m);
		}
	}

	class _HtmlPanel : HtmlPanel
	{
		//CiPopupHtml _p;

		public _HtmlPanel(/*CiPopupHtml p*/)
		{
			//_p = p;
			//this.SetStyle(ControlStyles.Selectable, false); //prevent focusing control and activating window on click. Does not work with this control. Activates on click.
		}

		protected override void OnClick(EventArgs e)
		{
			//Print("OnClick");
			//base.OnClick(e); //sets focus. Unwanted when link clicked. OnLinkClicked is after.
		}

		//protected override void OnLinkClicked(HtmlLinkClickedEventArgs e)
		//{
		//	Print("OnLinkClicked");
		//	base.OnLinkClicked(e);
		//}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			//Print("OnMouseUp");
			if(!Empty(this.SelectedText)) Focus(); //the user may want Ctrl+C
			base.OnMouseUp(e);
		}
	}
}
