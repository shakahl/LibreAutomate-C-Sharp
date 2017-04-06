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

class PanelOutput :Control
{
	Scintilla _c;

	public PanelOutput()
	{
		_c = new Scintilla();
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Output";

		_c.HandleCreated += _c_HandleCreated;

		this.Controls.Add(_c);
	}

	private void _c_HandleCreated(object sender, EventArgs e)
	{
		_c.Margins[1].Width = 3;
		_c.Styles[Style.Default].BackColor = _c.Styles[0].BackColor = Color_.ColorFromRGB(0xF7F7F7);
		_c.Styles[0].Font = "Courier New"; _c.Styles[0].Size = 8;
		_c.ScrollWidth = 1;
		//_c.WrapVisualFlags = WrapVisualFlags.End;
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void Write(object text)
	{
		_c.AppendText(text.ToString() + "\r\n");
	}
}
