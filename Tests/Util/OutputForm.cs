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

class OutputForm : Form_
{
	AuScintilla _c;

	static Au.Util.OutputServer _os = new Au.Util.OutputServer(isGlobal: false);

	public OutputForm()
	{
		this.Text = "Tests";
		this.StartPosition = FormStartPosition.Manual;
		var r = Screen.AllScreens[1].Bounds; r.Inflate(-20, 0); r.Offset(20, 0);
		this.Bounds = r;

		_c = new SciOutput();
		_c.Dock = DockStyle.Fill;
		this.Controls.Add(_c);

		_os.NoNewline = true;
		_os.SetNotifications(_ProcessMessages, this);
	}

	void _ProcessMessages()
	{
		_c.Tags.OutputServerProcessMessages(_os);
	}

	public static void ShowForm()
	{
		_os.Start();
		Output.IgnoreConsole = true;
		Application.Run(new OutputForm());
		_os.Stop();
	}

	class SciOutput : AuScintilla
	{
		public SciOutput()
		{
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
			switch(e.Button) {
			case MouseButtons.Middle:
				ST.ClearText();
				break;
			}
			base.OnMouseDown(e);
		}
	}
}
