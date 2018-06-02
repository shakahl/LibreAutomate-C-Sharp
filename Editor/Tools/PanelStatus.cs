//#define MOUSE_INFO_FAST_RESPONSE

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
using static Au.NoClass;
using static Program;
using Au.Controls;

class PanelStatus :Control
{
	AuScintilla _c;
	POINT _p;
#if MOUSE_INFO_FAST_RESPONSE
	Timer_ _timer;
	bool _isMyTimer;
	int _timerStopCounter;
#endif

	public PanelStatus()
	{
		var z = TextRenderer.MeasureText("A\nj", MainForm.Font);
		this.Size = new Size(0, z.Height + 3);
		this.Dock = DockStyle.Bottom;

		_c = new AuScintilla();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = "Status bar";
		_c.AccessibleRole = AccessibleRole.StatusBar;

		_c.InitReadOnlyAlways = true;
		_c.WrapLines = true;
		_c.HandleCreated += _c_HandleCreated;

		_c.InitTagsStyle = AuScintilla.TagsStyle.AutoWithPrefix;

		this.Controls.Add(_c);

#if MOUSE_INFO_FAST_RESPONSE
		_timer = new Timer_(_MouseInfo);
#endif
		Program.Timer1s += Program_Timer1s;
	}

	private void _c_HandleCreated(object sender, EventArgs e)
	{
		var t = _c.ST;
		t.StyleBackColor(Sci.STYLE_DEFAULT, 0xF0F0F0);
		var font = MainForm.Font;
		t.StyleFont(Sci.STYLE_DEFAULT, font.Name, (int)font.Size);
		t.MarginWidth(1, 4);
		t.StyleClearAll();
		_c.Call(Sci.SCI_SETHSCROLLBAR); _c.Call(Sci.SCI_SETVSCROLLBAR);
	}

	protected override void Dispose(bool disposing)
	{
		//Print(disposing);
		Program.Timer1s -= Program_Timer1s;
		base.Dispose(disposing);
	}

	public void SetText(string text)
	{
		if(this.InvokeRequired) BeginInvoke(new Action(() => _SetText(text)));
		else _SetText(text);
	}

	void _SetText(string text)
	{
		_c.Text = text;
	}

	private void Program_Timer1s()
	{
#if MOUSE_INFO_FAST_RESPONSE
		if(_isMyTimer) return;
#endif
		if(!Visible) return;
		_MouseInfo(null);
	}

	unsafe void _MouseInfo(Timer_ timer)
	{
		var p = Mouse.XY;
		bool noChange = p == _p;
#if MOUSE_INFO_FAST_RESPONSE
		if(noChange) {
			if(_isMyTimer && ++_timerStopCounter >= 30) {
				_timer.Stop();
				_isMyTimer = false;
				//Print("stop");
			}
		} else {
			if(!_isMyTimer) {
				_timer.Start(100, false);
				_isMyTimer = true;
				//Print("start");
			}
			_timerStopCounter = 0;
		}
#endif
		if(noChange) return;
		_p = p;

		using(new Au.Util.LibStringBuilder(out var b, 1000)) {

			b.Append(p.x).Append("\n").Append(p.y);

			_c.ST.SetText(b.ToString());
		}
		//remember: limit long text, each line separatelly
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
