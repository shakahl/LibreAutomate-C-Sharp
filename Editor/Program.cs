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
using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Editor
{
	static class Program
	{
		static void Test()
		{
			var a = new object[] { "ggg", 5, 2.5 };
			foreach(var s in a.OfType<string>()) {
				Out(s);
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			try {
				Output.Clear();
				//Test(); return;
				//Editor.Test.DevTools.CreatePngImagelistFileFromIconFiles_il_tb();
				//Editor.Test.DevTools.CreatePngImagelistFileFromIconFiles_il_tv();
				Perf.First();
				var p = new Perf.Inst(true);

				//Run some code in other thread, to make it faster later in main thread.
				if(Environment.ProcessorCount > 1) {
				//if(false) { //TODO: currently the init code is too fast...
					var t = new Thread(() =>
						{
							//Control.BackColor first time takes 15 ms here, 9 ms in main thread where actually used.
							//However this does not make the app load faster. Maybe slightly slower.
							//var c = new Splitter(); c.BackColor = Color.DarkBlue; c.Dispose();
							//p.Next(); //15

							//Accessing .NET resources and creating imagelist first time takes 50-70 ms...
							EImageList.LoadImageLists();
							p.Next(); //60 with resources (now 45 *), 30 with imagelist png file (now 13 *), 14 if c.BackColor used above
							//* after updating Windows 10 to 14986 and ngening all used .NET assemblies everything starts much faster, and program starts almost 2 times faster!

							//new WeifenLuo.WinFormsUI.Docking.DockPanel().Dispose(); //ok if ngened, else spends too long, main thread has to wait anyway. Assume will always be ngened; then saves not much time.
							//Assembly.LoadFile(Folders.App + "WeifenLuo.WinFormsUI.Docking.dll");
							//p.Next(); //57 ms, ngen 16 ms

							//var m = new ToolStrip();
							//m.Items.Add("a");
							////m.Show();
							//var h = m.Handle;
							//m.Dispose();

							//p.Write();
							//Out(1);
						});
					//t.SetApartmentState(ApartmentState.STA);
					t.Start();

					//WaitMS(50);
				} else {
					EImageList.LoadImageLists();
					//p.NW();
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
#if FORM2
			Application.Run(new Form2());
#else
				Application.Run(new Form4());
#endif

			}
			catch(Exception e) { Out(e); }
		}
	}
}
