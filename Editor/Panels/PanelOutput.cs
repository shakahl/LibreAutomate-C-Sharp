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
using G.Controls;

using static G.Controls.Sci;

class PanelOutput :Control
{
	SciOutput _c;

	public SciOutput Output { get => _c; }

	public PanelOutput()
	{
		_c = new SciOutput();
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Output";
		this.Controls.Add(_c);

		//_c.Deb = true;
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }
}

class SciOutput :SciControl
{
	public SciOutput()
	{
		InitReadOnlyAlways = true;
		InitImagesStyle = ImagesStyle.ImageTag;
		InitTagsStyle = TagsStyle.AutoWithPrefix;

	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		ST.MarginWidth(1, 3);
		ST.StyleBackColor(STYLE_DEFAULT, 0xF7F7F7);
		ST.StyleFont(STYLE_DEFAULT, "Courier New", 8);
		ST.StyleClearAll();
	}

	public void Write(object text)
	{
		var s = text.ToString();
		ST.AppendText(s, true, true);
	}

	public void Clear()
	{
		ST.ClearText();
	}
}
