using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;
using static Program;
using G.Controls;

class PanelStatus :Control
{
	SciControl _c;
	StringBuilder _s;
	POINT _p;
	Time.Timer_ _timer;
	bool _isMyTimer;
	int _timerStopCounter;

	public PanelStatus()
	{
		var z = TextRenderer.MeasureText("A\nj", MainForm.Font);
		this.Size = new Size(0, z.Height + 3);
		this.Dock = DockStyle.Bottom;

		_c = new SciControl();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = "Status bar";
		_c.AccessibleRole = AccessibleRole.StatusBar;

		_c.InitReadOnlyAlways = true;
		_c.WrapLines = true;
		_c.HandleCreated += _c_HandleCreated;

		this.Controls.Add(_c);

		_s = new StringBuilder(1000);
		_timer = new Time.Timer_(_MouseInfo);
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
		if(_isMyTimer || !Visible) return;
		_MouseInfo(null);
	}

	void _MouseInfo(Time.Timer_ timer)
	{
		var p = Mouse.XY;
		bool noChange = p == _p;
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
		if(noChange) return;
		_p = p;

		_s.Clear();
		//Print(_s.Capacity);
		_s.AppendFormat("{0}\n{1} j", p.x, p.y);
		//remember: limit long text, each line separatelly

		_MouseInfo_SetText();
	}

	unsafe void _MouseInfo_SetText()
	{
		//a test-string, almost 300 length
		//_s.Append("Chars is the default property of the StringBuilder class. In C#, it is an indexer.\r\nThis means that individual characters can be retrieved from the Chars property as shown in the following example, which counts the number of alphabetic, white-space, and punctuation characters in a string.");
		//Print(_s.Length);

		//Perf.First();
#if false
		_SetText(_s.ToString());
		//This would make too much garbage: the string and a temporary byte[].
		//Let's say, this func is called at 10 ms period when moving the mouse, and average text size is 340.
		//Then need 1 KB. In second - 100 KB. In minute - 6 MB.
#else
		//_c.ST.SetText(_s);
#endif
		//Perf.Next();
		//_c.Update();
		//Perf.NW(); //~1.3 ms. Also tried to draw directly to this control (without scintilla), but it is slightly slower, even when using API.
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
