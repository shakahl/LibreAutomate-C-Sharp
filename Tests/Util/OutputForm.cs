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
using static Au.Controls.Sci;

class OutputForm : AuForm
{
	AuScintilla _c;

	static AOutputServer _os = new AOutputServer(isGlobal: false);

	public OutputForm()
	{
		this.Text = "Tests";
		this.StartPosition = FormStartPosition.Manual;
		int x = 300;
		var r = Screen.AllScreens[1].Bounds; r.Inflate(-x, 0); r.Offset(x, 0);
		this.Bounds = r;

		_c = new SciOutput(this);
		_c.Dock = DockStyle.Fill;
		this.Controls.Add(_c);

		_os.NoNewline = true;
		_os.SetNotifications(_ProcessMessages, this);
		_os.Start();
		AOutput.IgnoreConsole = true;
	}

	void _ProcessMessages()
	{
		if(_paused) return;
		_c.ZTags.OutputServerProcessMessages(_os);
	}
	bool _paused;

	public static void ShowForm()
	{
		Application.Run(new OutputForm());
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		_os.Stop();
		base.OnFormClosed(e);
	}

	protected override bool ShowWithoutActivation => true;

	class SciOutput : AuScintilla
	{
		OutputForm _f;

		public SciOutput(OutputForm f)
		{
			_f = f;
			ZInitReadOnlyAlways = true;
			ZInitTagsStyle = ZTagsStyle.AutoWithPrefix;
			ZInitImagesStyle = ZImagesStyle.ImageTag;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			Z.MarginWidth(1, 3);
			Z.StyleBackColor(STYLE_DEFAULT, 0xF7F7F7);
			Z.StyleFont(STYLE_DEFAULT, "Courier New", 8);
			Z.StyleClearAll();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(ModifierKeys != 0) return;
			switch(e.Button) {
			case MouseButtons.Middle:
				Z.ClearText();
				break;
			case MouseButtons.Right:
				if(!_f._paused) { AOsd.ShowText("OutputForm info: right-click pauses output. Right click again to resume.", xy: PopupXY.Mouse); }
				_f._paused ^= true;
				break;
			}
			base.OnMouseDown(e);
		}
	}
}
