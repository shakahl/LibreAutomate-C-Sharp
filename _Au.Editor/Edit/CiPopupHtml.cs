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
//	1. Does not support many CSS things. Or draws incorrectly. Need to apply HTML/CSS workarounds.
//	2. Does not support accessibility and zoom. But it is not important.
//	3. GC is more frequent, eg every 7 times. With WebBrowser every 14 times. But it is not important.
//The best would be to create a control that can draw just what we need. Like AuListControl. But much work. Just started InfoControl class, then moved to the Unused project. Could not find in github etc.

#if true //use HtmlPanel control from HtmlRenderer assembly

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
		} else if(s.Starts('|')) { //open symbol source file and go to
			CiUtil.OpenSymbolSourceFile(s, _w);
		} else if(s.Has("/a.html#")) { //open symbol source web page
			CiUtil.OpenSymbolSourceUrl(s);
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
#elif true //use WebBrowser control
class CiPopupHtml
{
	InactiveWindow _w;
	_WebBrowser _html; //never mind: should destroy windows containing WebBrowser when hiding the main window. Tested: it frees some memory, but not much.
	Action<bool> _onHiddenOrDestroyed;
	bool _signature;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public InactiveWindow PopupWindow => _CreateOrGet();

	InactiveWindow _CreateOrGet()
	{
		if(_w == null || _w.IsDisposed) {
			_w = new InactiveWindow();
			_w.SuspendLayout();
			_w.Name = _w.Text = "Ci.PopupHtml";
			_w.MinimumSize = Au.Util.ADpi.ScaleSize((150, 150));
			_w.Size = new Size(Screen.FromControl(Program.MainForm).WorkingArea.Width / (_signature ? 2 : 3), Au.Util.ADpi.ScaleInt(_signature ? 300 : 360));
			_w.BackColor = Color.LightYellow; //until the control loaded

			_html = new _WebBrowser();
			_html.SuspendLayout();
			_html.AccessibleName = _html.Name = "Codein_info";
			_html.Dock = DockStyle.Fill;
			_html.AllowWebBrowserDrop = false;
			_html.IsWebBrowserContextMenuEnabled = false;
			_html.ResumeLayout();
			_w.Controls.Add(_html);
			_w.ResumeLayout(true);

			if(_onHiddenOrDestroyed != null) _w.ZHiddenOrDestroyed += _onHiddenOrDestroyed;
		}
		return _w;
	}

	public CiPopupHtml(bool signature, Action<bool> onHiddenOrDestroyed = null)
	{
		_signature = signature;
		_onHiddenOrDestroyed = onHiddenOrDestroyed;
	}

	public void SetHtml(string html, Func<int, string> itemLinkHtml = null)
	{
		_CreateOrGet();
		_html.SetHtml(html, itemLinkHtml);
	}

	public void UpdateHtml(string html)
	{
		_CreateOrGet();
		_html.UpdateHtml(html);
	}

	public void Show(SciCode doc, Rectangle anchorRect, PopupAlignment align = 0)
	{
		_CreateOrGet();
		_w.ZCalculateAndSetPosition(anchorRect, align);
		_w.ZShow(doc);
		_html.InitHtml();
	}

	public void Show(SciCode doc, int position)
	{
		var r = CiUtil.GetCaretRectFromPos(doc, position);
		r.Inflate(100, 10);
		Show(doc, r, PopupAlignment.TPM_VERTICAL);
	}

	public void Hide()
	{
		_CreateOrGet();
		_w.Hide();
		SetHtml(null);
	}

	class _WebBrowser : WebBrowser
	{
		//CiPopupHtml _p;
		Func<int, string> _itemLinkHtml;

		//public _WebBrowser(CiPopupHtml p) { _p = p; }

		protected override void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m, Api.WM_SETCURSOR, Api.WM_NCHITTEST, Api.WM_NCMOUSEMOVE);
			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				//AStatic.Print("WM_MOUSEACTIVATE");
				if(AMath.HiShort(m.LParam) == Api.WM_LBUTTONDOWN) {
					var e = this.Document.GetElementFromPoint(this.MouseClientXY());
					while(e != null && !e.TagName.Eqi("a")) e = e.Parent;
					if(e != null) {
						string s = e.GetAttribute("href");
						//_LinkClicked(s); //now not a good time for it. May generate multiple WM_MOUSEACTIVATE.
						BeginInvoke(new Action(() => _LinkClicked(s)));
					}
					if(e != null || !this.Focused) {
						m.Result = (IntPtr)Api.MA_NOACTIVATEANDEAT;
						return;
					}
				}
				break;
			}
			base.WndProc(ref m);
		}

		//on click this is not called, because we receive WM_MOUSEACTIVATE even when the window is active and control focused. But safer with this.
		protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
		{
			//AStatic.Print("OnNavigating");
			if(_inited < 2) return;
			e.Cancel = true;
			_LinkClicked(e.Url.ToString());
			//base.OnNavigating(e);
		}

		public void SetHtml(string html, Func<int, string> itemLinkHtml)
		{
			UpdateHtml(html);
			_itemLinkHtml = itemLinkHtml;
		}

		public void UpdateHtml(string html)
		{
			html ??= "";
			if(_inited == 2) {
				this.Document.Body.InnerHtml = html;
			} else {
				_initHtml = html;
			}
		}
		int _inited;
		string _initHtml;

		public void InitHtml()
		{
			if(_inited == 0) { //now set empty HTML with CSS. Later just change HTML of the body element. Faster and no 'wait' cursor.
				_inited = 1;
				var html = @"<html><head><style>
html { background-color: #ffffe0 }
body { font: x-small 'Segoe UI'; margin: 2px 4px 4px 4px; overflow: auto; }
span.type { color: #088 }
span.keyword { color: #00f }
span.string { color: #a74 }
span.number { color: #a40 }
span.namespace { color: #777 }
span.comment { color: #080 }
span.dot { color: #ccc }
span.dotSelected { color: #c0f }
span.hilite { background-color: #fca }
p { margin: 0.6em 0 0.6em 0 }
p.parameter { background-color: #dec; }
div.selected, div.link { padding: 0 0 2px 2px; border-bottom: 1px dashed #ccc; }
div.selected { background-color: #f8f0a0; }
div.link a { color: #000; text-decoration: none; }
code { background-color: #f0f0f0; font: 100% Consolas; }
hr { height: 1px; border-top: 1px solid #ccc; }
</style>
</head><body></body></html>";
				this.DocumentText = html;
			}
		}

		protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
		{
			if(_inited < 2) {
				_inited = 2;
				if(_initHtml != null) {
					var html = _initHtml; _initHtml = null;
					this.Document.Body.InnerHtml = html;
				}
			}
			base.OnDocumentCompleted(e);
		}

		void _LinkClicked(string s)
		{
			if(s.Starts("about:")) s = s.Substring(6);
			if(s.Starts('^')) { //select another symbol (usually overload) in the list of symbols
				int i = s.ToInt(1);
				this.Document.Body.InnerHtml = _itemLinkHtml(i);
			} else if(s.Starts('|')) { //open symbol source file and go to
				CiUtil.OpenSymbolSourceFile(s, this);
			} else if(s.Has("/a.html#")) { //open symbol source web page
				CiUtil.OpenSymbolSourceUrl(s);
			} else {
				AExec.TryRun(s);
			}
		}
	}
}
#else //use InfoControl. The control and this code are unfinished.
class CiPopupHtml
{
	InactiveWindow _w;
	InfoControl _html;
	bool _signature;
	Action<bool> _onHiddenOrDestroyed;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public InactiveWindow PopupWindow => _CreateOrGet();

	InactiveWindow _CreateOrGet()
	{
		if(_w == null || _w.IsDisposed) {
			_w = new InactiveWindow();
			_w.SuspendLayout();
			_w.Name = _w.Text = "Ci.PopupHtml";
			_w.MinimumSize = Au.Util.ADpi.ScaleSize((150, 150));
			_w.Size = new Size(Screen.FromControl(Program.MainForm).WorkingArea.Width / (_signature ? 2 : 3), Au.Util.ADpi.ScaleInt(_signature ? 300 : 360));
			//_w.BackColor = Color.LightYellow;

			_html = new InfoControl();
			_html.SuspendLayout();
			_html.AccessibleName = _html.Name = "Codein_info";
			_html.Dock = DockStyle.Fill;
			_html.ResumeLayout();
			_w.Controls.Add(_html);
			_w.ResumeLayout(true);

			if(_onHiddenOrDestroyed != null) _w.ZHiddenOrDestroyed += _onHiddenOrDestroyed;
		}
		return _w;
	}

	public CiPopupHtml(bool signature, Action<bool> onHiddenOrDestroyed = null)
	{
		_signature = signature;
		_onHiddenOrDestroyed = onHiddenOrDestroyed;
	}

	public void SetHtml(string html, Func<int, string> itemLinkHtml = null)
	{
		_CreateOrGet();
		//_html.SetHtml(html, itemLinkHtml);
		_html.SetText(html);
	}

	public void UpdateHtml(string html)
	{
		_CreateOrGet();
		//_html.UpdateHtml(html);
		_html.SetText(html);
	}

	public void Show(SciCode doc, Rectangle anchorRect, PopupAlignment align = 0)
	{
		_CreateOrGet();
		_w.ZCalculateAndSetPosition(anchorRect, align);
		_w.ZShow(doc);
	}

	public void Show(SciCode doc, int position)
	{
		var r = CiUtil.GetCaretRectFromPos(doc, position);
		r.Inflate(100, 10);
		Show(doc, r, PopupAlignment.TPM_VERTICAL);
	}

	public void Hide()
	{
		_CreateOrGet();
		_w.Hide();
		SetHtml(null);
	}

}
#endif
