using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Xml.Linq;
//using System.Xml.XPath;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	//[DebuggerStepThrough]
	public class ParamGridTLP
	{
		TableLayoutPanel _panel;

		public ParamGridTLP(TableLayoutPanel panel, bool isFlags=false)
		{
			_panel = panel;
			_panel.RowCount = 0;
			_panel.ColumnCount = isFlags?1:2;
			_panel.AutoScroll = true;
			_panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			//_panel.BorderStyle = BorderStyle.FixedSingle;

			var cs =_panel.ColumnStyles;
			cs.Clear(); //designer adds
			cs.Add(new ColumnStyle(SizeType.Absolute, 70));
			cs.Add(new ColumnStyle(SizeType.AutoSize));
		}

		public void AddOptional(string name, string value, bool check = false)
		{
			_panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));

			var c = new CheckBox() { Text = name, Checked = check };
			c.Padding = new Padding(2, 0, 2, 0);
			//c.AutoEllipsis=true; //does not work well
			//Print();
			//c.AutoSize = true;
			//c.Dock = DockStyle.Fill;
			c.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

			var t = new TextBox() { Multiline = true, BorderStyle= BorderStyle.None, Text = value };
			//t.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right| AnchorStyles.Bottom;
			t.Dock = DockStyle.Fill;

			_AddControl(c);
			_AddControl(t);
		}

		void _AddControl(Control c)
		{
			c.Margin = new Padding(0, 0, 0, 0);
			_panel.Controls.Add(c);
		}

		public void Clear()
		{
			var a = new Control[_panel.Controls.Count]; _panel.Controls.CopyTo(a, 0);
			_panel.Controls.Clear();
			_panel.RowStyles.Clear();
			foreach(Control v in a) v.Dispose();
		}

		public void AutoSizeAfterAddingAll()
		{
			//workaround for: the last row is wide, and its checkbox is incorrectly aligned
			_AddControl(new Label() { Text = "", Height = 0, Width = 0 });

			//workaround for control bug: vertical scrollbar adds horizontal scrollbar too
			//_panel.Padding = new Padding(0, 0, 0, 0);
		}
	}
}
