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
using System.Windows;
using System.Windows.Controls;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

class PanelFound : DockPanel
{
	KScintilla _c;

	public KScintilla ZControl => _c;

	public PanelFound()
	{
		_c = new KScintilla { Name = "Found_list" };
		_c.ZInitReadOnlyAlways = true;
		_c.ZInitTagsStyle = KScintilla.ZTagsStyle.AutoAlways;
		_c.ZAcceptsEnter = true;
		_c.ZHandleCreated += _c_ZHandleCreated;

		this.Children.Add(_c);
	}

	private void _c_ZHandleCreated() {
		_c.zSetMarginWidth(1, 0);
		_c.zStyleFont(Sci.STYLE_DEFAULT, App.Wmain);
		_c.zStyleClearAll();
		_c.ZTags.SetLinkStyle(new SciTags.UserDefinedStyle(), (false, default), false);
	}
}
