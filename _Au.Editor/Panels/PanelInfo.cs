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
using Au.Controls;
using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.Core.Entities;

class PanelInfo : AuUserControlBase
{
	HtmlPanel _html;
	Au.Controls.AuScintilla _sci;

	public PanelInfo()
	{
		this.AccessibleName = this.Name = "Info";

		//note: create _html async, not now. Now would make program startup slower. Loads 2 dlls. Setting text first time is even slower.

		//var z = TextRenderer.MeasureText("A\nj", Font);
		//<mouse info panel size> = new Size(0, z.Height + 3);

		_sci = new AuScintilla();
		_sci.AccessibleName = _sci.Name = "Info_mouse";
		_sci.Dock = DockStyle.Fill;
		_sci.ZInitReadOnlyAlways = true;
		_sci.ZInitTagsStyle = AuScintilla.ZTagsStyle.AutoAlways;
		this.Controls.Add(_sci);
	}

	protected override void OnLoad(EventArgs e)
	{
		var z = _sci.Z;
		z.StyleBackColor(Sci.STYLE_DEFAULT, 0xF0F0F0);
		z.StyleFont(Sci.STYLE_DEFAULT, Font);
		z.MarginWidth(1, 4);
		z.StyleClearAll();
		//_sci.Call(Sci.SCI_SETHSCROLLBAR);
		_sci.Call(Sci.SCI_SETVSCROLLBAR);

		Program.MousePosChangedWhenProgramVisible += _MouseInfo;
		ATimer.After(50, _ => { ZSetAboutInfo(); _html.Show(); });

		base.OnLoad(e);
	}

	void _LoadHtmlRenderer()
	{
		if(_html != null) return;
		_html = new HtmlPanel {
			AccessibleName = "Info_code",
			Dock = DockStyle.Fill,
			//BackColor = Color.LightYellow,
			BackColor = Color.FromArgb(unchecked((int)0xfff8fff0)),
			UseSystemCursors = true,
			BaseStylesheet = CiHtml.s_CSS
		};
		_html.ImageLoad += CodeInfo.OnHtmlImageLoad;
		_html.LinkClicked += _html_LinkClicked;
		this.Controls.Add(_html);

		_html.Hide(); //workaround for: HtmlPanel flickers (temp hscrollbar) when setting text first time. Now hide, then show after setting text.
	}

	void _html_LinkClicked(object sender, HtmlLinkClickedEventArgs e)
	{
		e.Handled = true;
		var s = e.Link;
		if(s == "?") {
			ZSetText(c_aboutHtml);
		} else if(s.Starts('#')) { //anchor
			e.Handled = false;
		} else {
			AFile.TryRun(s);
		}
	}

	const string c_aboutHtml = @"<body>This panel displays quick info about object from mouse position.<br>In code editor - function, class, etc.<br>In other windows - x, y, window, control.</body>";
	const string c_metaHtml = @"<body>The <span class='comment'>/*/ comments /*/</span> is file properties. Use the Properties dialog to edit and read about.</body>";

	public void ZSetAboutInfo(bool metaComents = false)
	{
		ZSetText(metaComents ? c_metaHtml : "<body><a href='?' style='color: #999'>?</a></body>");
	}

	public void ZSetText(string html)
	{
		if(html == _prevHtml && _isCodeInfo) return;
		_LoadHtmlRenderer();
		_html.Text = _prevHtml = html;

		if(!_isCodeInfo) {
			_isCodeInfo = true;
			_html.BringToFront();
		}
	}
	string _prevHtml;
	bool _isCodeInfo;

	//public void ZSetMouseInfoText(string text)
	//{
	//	if(this.InvokeRequired) BeginInvoke(new Action(() => _SetMouseInfoText(text)));
	//	else _SetText(text);
	//}

	//void _SetMouseInfoText(string text)
	//{
	//	_sci.Z.SetText(text);
	//}

	void _MouseInfo(POINT p)
	{
		if(!this.Visible) return;
		var c = AWnd.FromXY(p);
		var w = c.Window;
		if(_isCodeInfo) {
			if(w.IsOfThisProcess) return;
			_isCodeInfo = false;
			_sci.BringToFront();
		}
		using(new Au.Util.StringBuilder_(out var b, 1000)) {
			var cn = w.ClassName;
			if(cn != null) {
				var pc = p; w.MapScreenToClient(ref pc);
				b.AppendFormat("<b>xy</b> {0,5} {1,5},   <b>client</b> {2,5} {3,5}\r\n<b>Window</b>  {4}\r\n\t<b>cn</b>  {5}\r\n\t<b>program</b>  {6}",
					p.x, p.y, pc.x, pc.y,
					w.Name?.Escape(140), cn.Escape(70), w.ProgramName?.Escape(70));
				if(c != w) {
					b.AppendFormat("\r\n<b>Control   id</b>  {0}\r\n\t<b>cn</b>  {1}",
						c.ControlId, c.ClassName?.Escape(70));
				} else if(cn == "#32768") {
					var m = Au.Util.AMenuItemInfo.FromXY(p, w, 50);
					if(m != null) {
						b.AppendFormat("\r\n<b>Menu   id</b>  {0}", m.ItemId);
						if(m.IsSystem) b.Append(" (system)");
						//AOutput.Write(m.GetText(true, true));
					}
				}
			}

			_sci.Z.SetText(b.ToString());
		}
	}
}
