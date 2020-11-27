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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Util;
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
	public enum UsedBy { PopupList, Signature, Info }

	InactiveWindow _w;
	_HtmlPanel _c;
	Action<bool> _onHiddenOrDestroyed;
	string _html;
	UsedBy _usedBy;

	public CiPopupHtml(UsedBy usedy, Action<bool> onHiddenOrDestroyed = null)
	{
		_usedBy = usedy;
		_onHiddenOrDestroyed = onHiddenOrDestroyed;
	}

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public InactiveWindow PopupWindow => _CreateOrGet();

	///// <summary>
	///// The HTML control.
	///// </summary>
	//public HtmlPanel HtmlControl {
	//	get {
	//		_CreateOrGet();
	//		return _c;
	//	}
	//}

	/// <summary>
	/// Called when clicked a link with href prefix "^".
	/// </summary>
	public Action<CiPopupHtml, HtmlLinkClickedEventArgs> OnLinkClick { get; set; }

	/// <summary>
	/// Called to get an image before displaying the HTML or calculating its rectangle.
	/// </summary>
	public EventHandler<HtmlImageLoadEventArgs> OnLoadImage { get; set; }

	/// <summary>
	/// HTML text to show. Set before calling Show.
	/// </summary>
	public string Html {
		get => _html;
		set {
			ADebug.PrintIf(!(value.NE() || value.Starts("<body", true) || value.Starts("<html", true)), "no <body>");
			if(value != _html) {
				_html = value;
				if(IsVisible) {
					if(_usedBy == UsedBy.Info) _w.Size = _MeasureHtml(_html);
					_c.Text = _html;
				}
			}
		}
	}

	InactiveWindow _CreateOrGet()
	{
		if(_w == null || _w.IsDisposed) {
			_w = new InactiveWindow(_usedBy == UsedBy.Info ? WS.POPUP | WS.BORDER : WS.POPUP | WS.THICKFRAME, shadow: _usedBy == UsedBy.Info);
			_w.SuspendLayout();
			_w.Name = "Ci.Info";
			_w.Text = _usedBy switch { UsedBy.PopupList => "Au list item info", UsedBy.Signature => "Au parameters info", _ => "Au quick info" };
			if(_usedBy == UsedBy.Info) {

			} else {
				var owner = Program.MainForm;
				int dpi = ADpi.OfWindow(owner);
				_w.MinimumSize = ADpi.Scale((150, 150), dpi);
				_w.Size = new Size(
					AScreen.Of(Program.MainForm).WorkArea.Width / (_usedBy == UsedBy.PopupList ? 3 : 2),
					ADpi.Scale(_usedBy switch { UsedBy.PopupList => 360, UsedBy.Signature => 300, _ => 100 }, dpi)
					);
			}

			_c = new _HtmlPanel();
			_c.SuspendLayout();
			_c.Dock = DockStyle.Fill;
			var color = Color.FromArgb(0xff, 0xff, 0xf0);
			_c.BackColor = color;
			_w.BackColor = color; //until the control loaded
			_c.UseSystemCursors = true;
			_c.BaseStylesheet = CiHtml.s_CSS;

			_w.Controls.Add(_c);
			_c.ResumeLayout();
			_w.ResumeLayout(true);

			if(_onHiddenOrDestroyed != null) _w.ZHiddenOrDestroyed += _onHiddenOrDestroyed;
			_c.ImageLoad += (sender, e) => OnLoadImage?.Invoke(sender, e);
			_c.LinkClicked += (sender, e) => {
				var s = e.Link;
				if(s.Starts('#')) return; //anchor
				e.Handled = true;
				if(s.Starts('^')) {
					OnLinkClick?.Invoke(this, e);
				} else if(s.Starts('|')) { //go to symbol source file/position or web page
					CiGoTo.LinkGoTo(s, _w);
				} else {
					AFile.TryRun(s);
				}
			};
		}
		return _w;
	}

	Size _MeasureHtml(string html)
	{
		if(html.NE()) return default;
		int sbWid = SystemInformation.VerticalScrollBarWidth;
		using var g = _c.CreateGraphics();
		var zf = HtmlRender.Measure(g, html, 0, _c.BaseCssData, imageLoad: OnLoadImage);
		int wid = (int)zf.Width;
		int waWid = AScreen.Of(Program.MainForm).WorkArea.Width * 2 / 3 - sbWid;
		if(wid > waWid) { //remeasure, because HtmlRender.Measure returns maxWidth parameter if it is > text width
			zf = HtmlRender.Measure(g, html, waWid, _c.BaseCssData, imageLoad: OnLoadImage);
			wid = (int)zf.Width;
		}
		return new Size(wid + sbWid, (int)zf.Height + 3);
	}

	/// <summary>
	/// Shows by anchorRect, owned by ownerControl.
	/// </summary>
	/// <param name="ownerControl"></param>
	/// <param name="anchorRect">Rectangle in screen coord. The popup window will be by it but not in it.</param>
	/// <param name="align"></param>
	/// <param name="hideIfOutside">Hide when mouse moved outside anchorRect unless is in the popup window.</param>
	public void Show(SciCode ownerControl, Rectangle anchorRect, PopupAlignment align = 0, bool hideIfOutside = false)
	{
		_CreateOrGet();
		_w.ZCalculateAndSetPosition(anchorRect, align, _usedBy == UsedBy.Info ? _MeasureHtml(_html) : default(Size?));
		_c.Text = _html;
		_w.ZShow(ownerControl);

		if(hideIfOutside) {
			ATimer.Every(100, t => {
				if(IsVisible) {
					Point p = AMouse.XY;
					if(anchorRect.Contains(p) || _w.Bounds.Contains(p) || _w.Capture || _c.Capture) return;
					Hide();
				}
				t.Stop();
			});
		}
	}

	public void Show(SciCode ownerControl, int pos16, bool hideIfOutside = false, bool above = false)
	{
		var r = CiUtil.GetCaretRectFromPos(ownerControl, pos16);
		r.X -= 50; r.Width += 100;
		PopupAlignment pa = PopupAlignment.TPM_VERTICAL; if(above) pa |= PopupAlignment.TPM_BOTTOMALIGN;
		Show(ownerControl, ownerControl.RectangleToScreen(r), pa, hideIfOutside);
	}

	public void Hide()
	{
		if(!IsVisible) return;
		_w.Hide();
		_c.Text = _html = null;
	}

	public bool IsVisible => _w?.Visible ?? false;

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
			if(Visible && !this.SelectedText.NE()) Focus(); //the user may want Ctrl+C
			base.OnMouseUp(e);
		}
	}
}
