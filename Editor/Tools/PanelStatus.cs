//#define MOUSE_INFO_FAST_RESPONSE

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
	StringBuilder _sb;
	//WeakReference<char[]> _wrca=new WeakReference<char[]>(null);
	//WeakReference<string> _wrs = new WeakReference<string>(null);
	Point _p;
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

		_c = new SciControl();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = "Status bar";
		_c.AccessibleRole = AccessibleRole.StatusBar;

		_c.InitReadOnlyAlways = true;
		_c.WrapLines = true;
		_c.HandleCreated += _c_HandleCreated;

		_c.InitTagsStyle = SciControl.TagsStyle.AutoWithPrefix;

		_sb = new StringBuilder(1000);

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

		_sb.Clear();

		_sb.Append(p.X);
		_sb.Append("\n");
		_sb.Append(p.Y);

		_c.ST.SetText(_sb.ToString());

		//remember: limit long text, each line separatelly

		//possible alternatives:

		//var b = Catkeys.Util.Buffers.Get(1024, ref _wrca);
		//fixed (char* line1 = b) {
		//	Api.wsprintfW(line1, "%i\n%i", __arglist(p.X, p.Y));
		//	_c.ST.SetText(new string(line1));
		//}
		//no advantages. Creates garbage too, harder to use, slower than StringBuilder.Append (same speed as StringBuilder.AppendFormat).

		//if(!_wrs.TryGetTarget(out var b)) _wrs.SetTarget(b = new string('\0', 1000));
		//fixed (char* line1 = b) {
		//	//Api.wsprintfW(line1, "%i\n%i", __arglist(p.X, p.Y));
		//	Api.wsprintfW(line1, "<>%i\n<c 0xff>%i</c>", __arglist(p.X, p.Y));
		//	int len = CharPtr.Length(line1);
		//	Unsafe.Write(line1 - 2, len);
		//	_c.ST.SetText(b);
		//}
		//does not create garbage, but too dangerous, may not work with other .NET versions etc.
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
