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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Controls;
using static Au.Controls.Sci;

class OutputForm : AFormBase
{
	AuScintilla _c;

	static Au.Util.OutputServer _os = new Au.Util.OutputServer(isGlobal: false);

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
		Output.IgnoreConsole = true;
	}

	void _ProcessMessages()
	{
		if(_paused) return;
		_c.Tags.OutputServerProcessMessages(_os);
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
			InitReadOnlyAlways = true;
			InitTagsStyle = TagsStyle.AutoWithPrefix;
			InitImagesStyle = ImagesStyle.ImageTag;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			ST.MarginWidth(1, 3);
			ST.StyleBackColor(STYLE_DEFAULT, 0xF7F7F7);
			ST.StyleFont(STYLE_DEFAULT, "Courier New", 8);
			ST.StyleClearAll();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(ModifierKeys != 0) return;
			switch(e.Button) {
			case MouseButtons.Middle:
				ST.ClearText();
				break;
			case MouseButtons.Right:
				if(!_f._paused) { Osd.ShowText("OutputForm info: right-click pauses output. Right click again to resume.", xy: PopupXY.Mouse); }
				_f._paused ^= true;
				break;
			}
			base.OnMouseDown(e);
		}
	}
}
