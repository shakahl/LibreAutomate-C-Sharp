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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

class PanelFound : AuUserControlBase
{
	AuScintilla _c;

	public AuScintilla ZControl => _c;

	public PanelFound()
	{
		this.AccessibleName = this.Name = "Found";
		_c = new AuScintilla();
		_c.AccessibleName = _c.Name = "Found_list";
		_c.Dock = DockStyle.Fill;
		_c.ZInitReadOnlyAlways = true;
		_c.ZInitTagsStyle = AuScintilla.ZTagsStyle.AutoAlways;
		_c.ZAcceptsReturn = true;
		_c.HandleCreated += _c_HandleCreated;

		this.Controls.Add(_c);
	}

	private void _c_HandleCreated(object sender, EventArgs e)
	{
		var z = _c.Z;
		z.MarginWidth(1, 0);
		z.StyleFont(Sci.STYLE_DEFAULT, Font);
		z.StyleClearAll();
		_c.ZTags.SetLinkStyle(new SciTags.UserDefinedStyle(), (false, default), false);
	}

	//protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}
