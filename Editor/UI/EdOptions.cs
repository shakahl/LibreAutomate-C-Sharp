using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;

partial class EdOptions : Form_
{
	public EdOptions()
	{
		InitializeComponent();
	}

	public static void ShowForm()
	{
		if(s_form == null) {
			s_form = new EdOptions();
			s_form.Show(MainForm);
		} else {
			s_form.Activate();
		}
	}
	static EdOptions s_form;

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);
		s_form = null;
	}
}
