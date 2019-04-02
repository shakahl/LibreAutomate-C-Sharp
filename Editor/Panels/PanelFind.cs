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

class PanelFind :Control
{
	ListView _c;

	public PanelFind()
	{
		_c = new ListView();
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = _c.Name = "Find_list";
		this.AccessibleName = this.Name = "Find";
		this.Controls.Add(_c);

	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
