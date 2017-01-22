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
//using System.Configuration;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;

namespace Editor
{

	static class Program
	{
		static void Test()
		{
			//TODO: why Output.Write causes Xml assembly to load?
			//TODO: in TaskDialog edit field use correct font.


			//TaskDialog.Show("test");


			//Print(Application.CompanyName);
			//Print(Application.ProductName);
			//Print(Application.ProductVersion);
			//Print(Application.UserAppDataPath);
			//Print(Application.CommonAppDataPath);

			//PrintList("ThisApp", Folders.ThisApp);
			//PrintList("ThisAppData", Folders.ThisAppData);
			//PrintList("ThisAppDataCommon", Folders.ThisAppDataCommon);
			//PrintList("ThisAppDataLocal", Folders.ThisAppDataLocal);
			//PrintList("ThisAppDocuments", Folders.ThisAppDocuments);
			//PrintList("ThisAppTemp", Folders.ThisAppTemp);
			//PrintList("ThisProcess", Folders.ThisProcess);

			//Perf.Next();
			////var s = Properties.Settings.Default.Setting;
			////Perf.Next();
			//var xml = Properties.Settings.Default.PanelsXML;
			//Perf.Next();
			//XmlElement xFirstSplit = xml.SelectSingleNode("panels/split") as XmlElement;
			//Perf.NW();
			//Print(xml.InnerXml);
			//doc.DocumentElement.InnerText = "neew";
			//Properties.Settings.Default.Save();

			//Print(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath);
			//Print(Properties.Settings.Default.Setting);
			//Properties.Settings.Default.Setting = "kkk";
			//Properties.Settings.Default.Save();

			//Print(Folders.RoamingAppData);
			//Print(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

			//Print(Folders.ThisApp);
			//Print(Folders.ThisAppTemp);
			//Print(Folders.ThisAppDocuments);
			//Print(Folders.ThisAppData);
			//Print(Folders.ThisAppDataLocal);
			//Print(Folders.ThisAppDataCommon);


			//Application.Run(new Form1());

			//var f = new Form();
			//var t = new TextBox();
			//t.Font = new Font("Segoe UI", 9);
			//PrintList(t.MaximumSize, t.MinimumSize, t.AutoSize, t.Size);
			//t.AutoSize = false;
			////t.MaximumSize = new Size(10000, 10);
			//f.Height -=3;
			//PrintList(t.MaximumSize, t.MinimumSize, t.AutoSize, t.Size);

			//f.Controls.Add(t);
			//f.ShowDialog();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try {
				//Task.Run(() => { while(true) { WaitMS(100); GC.Collect(); } });

				Output.Clear();
				Test(); return;
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

							//var m = new ToolStrip();
							//m.Items.Add("a");
							////m.Show();
							//var h = m.Handle;
							//m.Dispose();

							//p.Write();
							//Print(1);
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

				Application.Run(new MainForm());
			}
			catch(Exception e) { Print(e); }
		}
	}
}
