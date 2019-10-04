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

		this.AccessibleName = this.Name = "Status";
		_c = new AuScintilla { Name = "Status_text", AccessibleName = Name };
		_c.AccessibleRole = AccessibleRole.StatusBar;
		_c.Dock = DockStyle.Fill;

		_c.ZInitReadOnlyAlways = true;
		_c.ZInitTagsStyle = AuScintilla.ZTagsStyle.AutoWithPrefix;
		//_c.WrapLines = true;

		this.Controls.Add(_c);

		Program.MousePosChangedWhenProgramVisible += _MouseInfo;
	}

	protected override void OnLoad(EventArgs e)
	{
		var t = _c.Z;
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

	public void ZSetText(string text)
	{
		if(this.InvokeRequired) BeginInvoke(new Action(() => _SetText(text)));
		else _SetText(text);
	}

	void _SetText(string text)
	{
		_c.Z.SetText(text);
	}

	void _MouseInfo(POINT p)
	{
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

			_c.Z.SetText(b.ToString());
		}
	}

	//protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
