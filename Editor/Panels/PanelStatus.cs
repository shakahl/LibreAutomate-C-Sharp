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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using static Program;
using Au.Controls;

class PanelStatus : AuUserControlBase
{
	AuScintilla _c;
	Font _font;

	public PanelStatus()
	{
		_font = Font;
		//_font = new Font(_font.FontFamily, _font.Size * 0.9f);
		var z = TextRenderer.MeasureText("A\nj", _font);
		this.Size = new Size(0, z.Height + 3);
		this.Dock = DockStyle.Bottom;

		_c = new AuScintilla();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = _c.Name = "Status_text";
		this.AccessibleName = this.Name = "Status";
		_c.AccessibleRole = AccessibleRole.StatusBar;

		_c.InitReadOnlyAlways = true;
		_c.InitTagsStyle = AuScintilla.TagsStyle.AutoWithPrefix;
		//_c.WrapLines = true;

		this.Controls.Add(_c);

		MousePosChangedWhenProgramVisible += _MouseInfo;
	}

	protected override void OnLoad(EventArgs e)
	{
		var t = _c.ST;
		t.StyleBackColor(Sci.STYLE_DEFAULT, 0xF0F0F0);
		t.StyleFont(Sci.STYLE_DEFAULT, _font);
		t.MarginWidth(1, 4);
		t.StyleClearAll();
		_c.Call(Sci.SCI_SETHSCROLLBAR); _c.Call(Sci.SCI_SETVSCROLLBAR);

		base.OnLoad(e);
	}

	//protected override void Dispose(bool disposing)
	//{
	//	//Print(disposing);
	//	MousePosChangedWhenProgramVisible -= _MouseInfo;
	//	base.Dispose(disposing);
	//}

	public void SetText(string text)
	{
		if(this.InvokeRequired) BeginInvoke(new Action(() => _SetText(text)));
		else _SetText(text);
	}

	void _SetText(string text)
	{
		_c.ST.SetText(text);
	}

	unsafe void _MouseInfo(POINT p)
	{
		using(new Au.Util.LibStringBuilder(out var b, 1000)) {
			var c = AWnd.FromXY(p);
			var w = c.Window;
			var pc = p; w.MapScreenToClient(ref pc);
			b.AppendFormat("Window  {0,5} {1,5}   .   {2}   .   cn={3}   .   styles=0x{4:X}, 0x{5:X}\r\nScreen     {6,5} {7,5}   ..   {8}",
				pc.x, pc.y, w.Name.Escape(100), w.ClassName.Escape(50), w.Style, w.ExStyle, p.x, p.y, w.ProgramName.Escape(50));
			if(c != w) {
				b.AppendFormat("   ..   Control   id={0}   .   cn={1}   .   styles=0x{2:X}, 0x{3:X}",
					c.ControlId, c.ClassName.Escape(50), c.Style, c.ExStyle);
			}

			_c.ST.SetText(b.ToString());
		}
	}

	//protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
