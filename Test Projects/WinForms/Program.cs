using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Forms;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

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
			//MessageBox.Show("ff");
			//return;

			//Perf.First();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var f = new Form2();
			//f.FormBorderStyle = FormBorderStyle.FixedDialog;
			//f.UseWaitCursor = false;
			Application.Run(f);
		}
	}
}
