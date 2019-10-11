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
using static Au.AStatic;
using Au.Controls;

using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.Core.Entities;

class PanelInfo : AuUserControlBase
{
	HtmlPanel _html;
	AuScintilla _mouseInfo;

	public PanelInfo()
	{
		InitializeComponent();
		this.AccessibleName = this.Name = "Info";

		//note: create _html async, not now. Now would make program startup slower. Loads 2 dlls. Setting text first time is even slower.

		//var z = TextRenderer.MeasureText("A\nj", Font);
		//<mouse info panel size> = new Size(0, z.Height + 3);

		_mouseInfo = new AuScintilla {
			AccessibleName = "MouseInfo",
			Dock = DockStyle.Fill,
			//WrapLines = true,
			ZInitReadOnlyAlways = true,
			ZInitTagsStyle = AuScintilla.ZTagsStyle.AutoWithPrefix
		};

		this.splitContainer1.Panel2.Controls.Add(_mouseInfo);
	}

	protected override void OnLoad(EventArgs e)
	{
		var z = _mouseInfo.Z;
		z.StyleBackColor(Sci.STYLE_DEFAULT, 0xF0F0F0);
		z.StyleFont(Sci.STYLE_DEFAULT, Font);
		z.MarginWidth(1, 4);
		z.StyleClearAll();
		_mouseInfo.Call(Sci.SCI_SETHSCROLLBAR);
		_mouseInfo.Call(Sci.SCI_SETVSCROLLBAR);

		Program.MousePosChangedWhenProgramVisible += _MouseInfo;
		ATimer.After(50, () => { ZSetAboutInfo(); _html.Show(); });

		base.OnLoad(e);
	}

	void _LoadHtmlRenderer()
	{
		if(_html != null) return;
		_html = new HtmlPanel {
			AccessibleName = "CodeInfo",
			Dock = DockStyle.Fill,
			//BackColor = Color.LightYellow,
			BackColor = Color.FromArgb(unchecked((int)0xfff8fff0)),
			UseSystemCursors = true,
			BaseStylesheet = CiHtml.s_CSS
		};
		_html.ImageLoad += _html_ImageLoad;
		_html.LinkClicked += _html_LinkClicked;
		this.splitContainer1.Panel1.Controls.Add(_html);

		_html.Hide(); //workaround for: HtmlPanel flickers (temp hscrollbar) when setting text first time. Now hide, then show after setting text.
	}

	private void _html_ImageLoad(object sender, HtmlImageLoadEventArgs e)
	{
		var s = e.Src;
		//Print(s);
		if(s.Starts("@")) {
			e.Handled = true;
			int i = s.ToInt(2);
			var b = s[1] switch { 'k' => CiUtil.GetKindImage((CiItemKind)i), 'a' => CiUtil.GetAccessImage((CiItemAccess)i), _ => null };
			e.Callback(b);
		}
	}

	private void _html_LinkClicked(object sender, HtmlLinkClickedEventArgs e)
	{
		e.Handled = true;
		var s = e.Link;
		if(s=="?") {
			ZSetText(c_aboutHtml);
			//} else if(s.Starts('|')) { //open symbol source file and go to
			//	CiUtil.OpenSymbolSourceFile(s, _w);
			//} else if(s.Has("/a.html#")) { //open symbol source web page
			//	CiUtil.OpenSymbolSourceUrl(s);
		} else if(s.Starts('#')) { //anchor
			e.Handled = false;
		} else {
			AExec.TryRun(s);
		}
	}

	const string c_metaHtml = @"<body>The <span class='comment'>/*/ comments /*/</span> is file properties. Use the Properties dialog to edit and read about.</body>";
	const string c_aboutHtml = @"<body>
This pane displays some info for a function, class or other symbol from mouse in the code editor.
<p>Below: mouse x y in window client area, window info, mouse x y in screen, program, control info.</p>
<p>Other features that help to write code:</p>
<ul style='margin-top: 0'>
<li>Show online help for a symbol or C# keyword: click it in the code editor and press F1.</li>
<li>Go to symbol source code if available: click it in the code editor and press F12.</li>
<li>List of symbols etc with info and autocompletion, function parameters info, auto <code>)]}>""'</code> and <code>;</code>, autoindent. <a href='http://3.quickmacros.com/help/editor/Code%20editor.html'>Read more</a>.</li>
<li>Get window/control/object info and create code: use dialogs 'Find window or control' and 'Find accessible object' (menu Code).</li>
</ul>
</body>";

	public void ZSetAboutInfo(bool metaComents = false)
	{
		ZSetText(metaComents ? c_metaHtml : "<body><a href='?' style='color: #999'>?</a></body>");
	}

	public void ZSetText(string html)
	{
		if(html == _prevHtml) return;
		_LoadHtmlRenderer();
		_html.Text = _prevHtml = html;
	}
	string _prevHtml;

	//public void ZSetMouseInfoText(string text)
	//{
	//	if(this.InvokeRequired) BeginInvoke(new Action(() => _SetMouseInfoText(text)));
	//	else _SetText(text);
	//}

	//void _SetMouseInfoText(string text)
	//{
	//	_mouseInfo.Z.SetText(text);
	//}

	void _MouseInfo(POINT p)
	{
		if(!this.Visible) return;
		using(new Au.Util.LibStringBuilder(out var b, 1000)) {
			var c = AWnd.FromXY(p);
			var w = c.Window;
			var cn = w.ClassName;
			if(cn != null) {
				var pc = p; w.MapScreenToClient(ref pc);
				b.AppendFormat("Window  {0,5} {1,5}   .   {2}   .   cn={3}   .   styles=0x{4:X}, 0x{5:X}\r\nScreen     {6,5} {7,5}   ..   {8}",
					pc.x, pc.y, w.Name?.Escape(140), cn.Escape(70), w.Style, w.ExStyle, p.x, p.y, w.ProgramName?.Escape(70));
				if(c != w) {
					b.AppendFormat("   ..   Control   id={0}   .   cn={1}   .   styles=0x{2:X}, 0x{3:X}",
						c.ControlId, c.ClassName?.Escape(70), c.Style, c.ExStyle);
				} else if(cn == "#32768") {
					var m = Au.Util.AMenuItemInfo.FromXY(p, w, 50);
					if(m != null) {
						b.AppendFormat("   ..   Menu   id={0}", m.ItemId);
						if(m.IsSystem) b.Append(" (system)");
						//Print(m.GetText(true, true));
					}
				}
			}

			_mouseInfo.Z.SetText(b.ToString());
		}
	}

	/// <summary> 
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary> 
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if(disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Component Designer generated code

	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
		this.splitContainer1.SuspendLayout();
		this.SuspendLayout();
		// 
		// splitContainer1
		// 
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Size = new System.Drawing.Size(501, 150);
		this.splitContainer1.SplitterDistance = 114;
		this.splitContainer1.TabIndex = 0;
		this.splitContainer1.Panel2MinSize = 30;
		// 
		// PanelInfo
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.Controls.Add(this.splitContainer1);
		this.Name = "PanelInfo";
		this.Size = new System.Drawing.Size(501, 150);
		((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
		this.splitContainer1.ResumeLayout(false);
		this.ResumeLayout(false);

	}

	#endregion

	private System.Windows.Forms.SplitContainer splitContainer1;
}
