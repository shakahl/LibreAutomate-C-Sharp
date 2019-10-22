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

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

//To render rich text for code info can be used 3 controls: Scintilla, WeBrowser, HtmlRenderer.
//Scintilla is too limited. No images, all lines of same height, no separators, no custom layout etc.
//WebBrowser is good, except:
//	1. dangerous. In various places internally opens about:blank, waits and pumps messages. Then we have strange random exceptions. Too difficult to prevent it.
//	2. uses ~9 MB of memory. HtmlRenderer uses ~1 MB.
//	3. quite difficult to prevent window activation on click. Need to apply some dirty workaround eg on WM_MOUSEACTIVATE.
//	4. to use in-memory images need to Base64-encode and embed in HTML. But with small images it is easy and fast.
//	5. Something may depend on IE version and IE settings.
//HtmlRenderer is good, except:
//	1. Does not support many CSS things. Or draws incorrectly. Need workarounds.
//	2. Does not support accessibility and zoom. But it is not important.
//	3. GC is more frequent, eg every 7 times. With WebBrowser every 14 times. But it is not important.
//The best would be to create a control that can draw just what we need. Like AuListControl. But much work. Just started InfoControl class, then moved to the Unused project. Could not find in github etc.

using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.Core.Entities;

class CiPopupHtml
{
	InactiveWindow _w;
	_HtmlPanel _html;
	Action<bool> _onHiddenOrDestroyed;
	Func<int, string> _itemLinkHtml;
	bool _signature;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public InactiveWindow PopupWindow => _CreateOrGet();

	public CiPopupHtml(bool signature, Action<bool> onHiddenOrDestroyed = null)
	{
		_signature = signature;
		_onHiddenOrDestroyed = onHiddenOrDestroyed;
	}

	InactiveWindow _CreateOrGet()
	{
		if(_w == null || _w.IsDisposed) {
			_w = new InactiveWindow();
			_w.SuspendLayout();
			_w.Name = _w.Text = "Ci.PopupHtml";
			_w.MinimumSize = Au.Util.ADpi.ScaleSize((150, 150));
			_w.Size = new Size(Screen.FromControl(Program.MainForm).WorkingArea.Width / (_signature ? 2 : 3), Au.Util.ADpi.ScaleInt(_signature ? 300 : 360));

			_html = new _HtmlPanel();
			_html.SuspendLayout();
			_html.AccessibleName = _html.Name = "Codein_info";
			_html.Dock = DockStyle.Fill;
			_html.BackColor = Color.LightYellow;
			_w.BackColor = Color.LightYellow; //until the control loaded
			_html.UseSystemCursors = true;
			_html.BaseStylesheet = CiHtml.s_CSS;
			_html.LinkClicked += _html_LinkClicked;

			_w.Controls.Add(_html);
			_html.ResumeLayout();
			_w.ResumeLayout(true);

			if(_onHiddenOrDestroyed != null) _w.ZHiddenOrDestroyed += _onHiddenOrDestroyed;
		}
		return _w;
	}

	public void SetHtml(string html, Func<int, string> itemLinkHtml = null)
	{
		_itemLinkHtml = itemLinkHtml;
		UpdateHtml(html);
	}

	public void UpdateHtml(string html)
	{
		ADebug.PrintIf(!(Empty(html) || html.Starts("<body", true) || html.Starts("<html", true)), "no <body>");
		_CreateOrGet();
		_html.Text = html;
	}

	public void Show(SciCode doc, Rectangle anchorRect, PopupAlignment align = 0)
	{
		_CreateOrGet();
		_w.ZCalculateAndSetPosition(anchorRect, align);
		_w.ZShow(doc);
	}

	public void Show(SciCode doc, int position) //FUTURE: remove if unused
	{
		var r = CiUtil.GetCaretRectFromPos(doc, position);
		r.Inflate(100, 10);
		Show(doc, r, PopupAlignment.TPM_VERTICAL);
	}

	public void Hide()
	{
		PopupWindow.Hide();
		SetHtml(null);
	}

	private void _html_LinkClicked(object sender, HtmlLinkClickedEventArgs e)
	{
		e.Handled = true;
		var s = e.Link;
		if(s.Starts('^')) { //select another symbol (usually overload) in the list of symbols
			int i = s.ToInt(1);
			UpdateHtml(_itemLinkHtml(i));
		} else if(s.Starts('|')) { //go to symbol source file/position or web page
			CiGoTo.GoTo(s, _w);
		} else if(s.Starts('#')) { //anchor
			e.Handled = false;
		} else {
			AExec.TryRun(s);
		}
	}

	class _HtmlPanel : HtmlPanel
	{
		//CiPopupHtml _p;

		//public _HtmlPanel(/*CiPopupHtml p*/)
		//{
		//	_p = p;
		//	//this.SetStyle(ControlStyles.Selectable, false); //prevent focusing control and activating window on click. Does not work with this control. Activates on click.
		//}

		protected override void OnClick(EventArgs e)
		{
			//base.OnClick(e); //sets focus. Unwanted when link clicked. OnLinkClicked is after.
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(!Empty(this.SelectedText)) Focus(); //the user may want Ctrl+C
			base.OnMouseUp(e);
		}
	}
}
