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
using System.Xml.Linq;

using Catkeys;
using static Catkeys.NoClass;

static class Program
{
	static void Test()
	{
		//Process.Start("https://referencesource.microsoft.com/#mscorlib/system/text/stringbuilder.cs,ec674e2123a44860");

		//var xml = "<x><a/><b/><!--comm--><c/><!--comm2--><d/><e/></x>";
		//XElement x = XElement.Parse(xml).Element("c");
		//Print(x.PreviousElement_());
		//Print(x.NextElement_());
		//Print(x.PreviousNode);
		//Print(x.NextNode);

		//var a1 = new Action(() => { x.PreviousElement_(); });
		//var a2 = new Action(() => { x.NextElement_(); });
		//var a3 = new Action(() => { });
		//var a4 = new Action(() => { });
		//Perf.ExecuteMulti(5, 1000, a1, a2, a3, a4);


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
	static void Main(string[] args)
	{
		try {
			//Test(); return;
			//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tv();
			//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tb();
			//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tb_big();

			Perf.First();

			if(CommandLine.OnProgramStarted(args)) return;

			OutputServer.NoNewline = true;
			OutputServer.Start();

			//Task.Run(() => { while(true) { Thread.Sleep(100); GC.Collect(); } });

			Api.SetErrorMode(Api.GetErrorMode() | Api.SEM_FAILCRITICALERRORS); //disable some error message boxes, eg when removable media not found; MSDN recommends too.
			Api.SetSearchPathMode(Api.BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE); //let SearchPath search in current directory after system directories

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Time.SetTimer(1000, false, t => Timer1s?.Invoke());

			Settings = new ProgramSettings();

			var form = new EForm();
			Application.Run(form);

			OutputServer.Stop();
		}
		catch(Exception e) { Print(e); }
	}

	public static Output.Server OutputServer = new Output.Server(true);
	public static ProgramSettings Settings;
	public static EForm MainForm;

	public struct Stock
	{
		static Stock()
		{
			FontNormal = SystemFonts.MessageBoxFont;
			FontBold = new Font(FontNormal, FontStyle.Bold);

		}

		public static Font FontNormal, FontBold;
	}

	public static event Action Timer1s;
}
