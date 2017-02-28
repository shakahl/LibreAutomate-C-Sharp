using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Catkeys;
using static Catkeys.NoClass;

namespace WinForms
{
	public partial class Form1 :Form
	{
		public Form1()
		{
			//Perf.Next();
			InitializeComponent();
			//Perf.Next();

			//edit.LoadFile(@"C:\Users\G\Desktop\Program.cs");
		}

		//protected override void OnActivated(EventArgs e)
		//{
		//	base.OnActivated(e);
		//	OutFunc();
		//}

		//protected override void OnShown(EventArgs e)
		//{
		//	base.OnShown(e);
		//	OutFunc();
		//}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//	base.OnPaint(e);
		//	//OutFunc();
		//	Time.SetTimer(1, true, t => Perf.NW());
		//}
	}
}
