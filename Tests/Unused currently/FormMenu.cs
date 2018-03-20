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
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Au;
using static Au.NoClass;

public partial class FormMenu :Form
{
	public FormMenu()
	{
		//InitializeComponent();

		var ms = new MenuStrip();
		var ddi = new ToolStripMenuItem("File");
		var k = new ToolStripMenuItem("Test");

		ms.Items.Add(ddi);
		//Print(ddi.DropDown);
		//Print(ddi.HasDropDown);
		ddi.DropDown.Items.Add(k);
		//Print(ddi.HasDropDown);
		//Print(ddi.DropDown);

		//var dd = new ToolStripDropDownMenu();
		//ddi.DropDown = dd;
		//dd.Items.Add(k);

		Print(k.OwnerItem);

		k.Click += K_Click;

		k.ShortcutKeys = Keys.Control | Keys.Z;

		this.Controls.Add(ms);
		this.MainMenuStrip = ms;

	}

	private void K_Click(object sender, EventArgs e)
	{
		Print("test");
	}

	private void testToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Print("test");
	}
}
