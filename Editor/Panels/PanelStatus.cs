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

using ScintillaNET;

class PanelStatus :Control
{
	Scintilla _c;
	StringBuilder _s;
	POINT _p;
	Time.Timer_ _timer;
	bool _isMyTimer;
	int _timerStopCounter;

	public PanelStatus()
	{
		var z = TextRenderer.MeasureText("A\nj", EForm.MainForm.Font);
		this.Size = new Size(0, z.Height + 3);
		this.Dock = DockStyle.Bottom;

		_c = new Scintilla();
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = "Status bar";
		_c.AccessibleRole = AccessibleRole.StatusBar;

		_c.HandleCreated += _c_HandleCreated;

		this.Controls.Add(_c);

		_s = new StringBuilder(1000);
		_timer = new Time.Timer_(_MouseInfo);
		Program.Timer1s += Program_Timer1s;
	}

	private void _c_HandleCreated(object sender, EventArgs e)
	{
		var font = EForm.MainForm.Font; //Print(font);
		_c.Margins[1].Width = 2;
		_c.Styles[Style.Default].BackColor = _c.Styles[0].BackColor = Color_.ColorFromRGB(0xF0F0F0);
		_c.Styles[0].Font = font.Name; _c.Styles[0].SizeF = font.Size;
		_c.HScrollBar = false; _c.VScrollBar = false; //_c.ScrollWidth = 1;
		_c.WrapVisualFlags = WrapVisualFlags.End;
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
		//This makes no garbage.
		var encoder = Encoding.UTF8;
		int i, n1 = Math.Min(_s.Length, 2000);
		var s1 = stackalloc char[n1 + 1]; s1[n1] = '\0';
		for(i = 0; i < n1; i++) s1[i] = _s[i]; //the slowest part, about 2 mcs for 300 length
		int n2 = encoder.GetByteCount(s1, n1 + 1);
		var s2 = stackalloc byte[n2];
		if(encoder.GetBytes(s1, n1 + 1, s2, n2) != n2) return;
		//Perf.Next();
		_c.DirectMessage(2181, Zero, (IntPtr)s2); //SCI_SETTEXT
#endif
		//Perf.Next();
		//_c.Update();
		//Perf.NW(); //~1.3 ms. Also tried to draw directly to this control (without scintilla), but it is slightly slower, even when using API.
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
