using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Forms;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace WinForms
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Perf.First();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var f =new Form1();
			//f.FormBorderStyle = FormBorderStyle.FixedDialog;
			//f.UseWaitCursor = false;
			Application.Run(f);
		}
	}
}
