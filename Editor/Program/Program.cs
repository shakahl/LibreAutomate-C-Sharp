using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Runtime;

using Au;
using Au.Types;
using static Au.NoClass;

static class Program
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	//[LoaderOptimization(LoaderOptimization.MultiDomain)] //shares all assemblies; does not unload assemblies; very slow assembly startup.
	//[LoaderOptimization(LoaderOptimization.MultiDomainHost)] //shares only GAC assemblies; unloads non-GAC assemblies. Makes loading some assemblies slightly faster, but eg Forms much slower (60 -> 100 ms).
	static void Main(string[] args)
	{
		//try {

		//_Test(); return;
		//Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1; //test how works with 1 CPU

		Perf.First();

#if !DEBUG
		var fProfile = Folders.ThisAppDataLocal + "ProfileOptimization";
		File_.CreateDirectory(fProfile);
		ProfileOptimization.SetProfileRoot(fProfile);
		ProfileOptimization.StartProfile("Editor.speed"); //makes startup faster eg 640 -> 470 ms (ngen 267). Makes compiler startup faster 4000 -> 2500 (ngen 670).
		Perf.Next();
#endif

		if(CommandLine.OnProgramStarted(args)) return;

		OutputServer.NoNewline = true;
		OutputServer.Start();

		//Task.Run(() => { while(true) { Thread.Sleep(1000); GC.Collect(); } });

		Api.SetErrorMode(Api.GetErrorMode() | Api.SEM_FAILCRITICALERRORS); //disable some error message boxes, eg when removable media not found; MSDN recommends too.
		Api.SetSearchPathMode(Api.BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE); //let SearchPath search in current directory after system directories

		//Application.EnableVisualStyles(); //we have manifest
		Application.SetCompatibleTextRenderingDefault(false);

		Settings = new ProgramSettings();

		Timer_.Every(1000, () => Timer1s?.Invoke());

		EForm.RunApplication();

		OutputServer.Stop();
		//}
		//catch(Exception e) { Print(e); }
	}

	internal static Au.Util.OutputServer OutputServer = new Au.Util.OutputServer(true);
	internal static ProgramSettings Settings;
	internal static EForm MainForm;
	internal static FilesModel Model;
	internal static RunningTasks Tasks;

	internal static event Action Timer1s;

	internal struct Stock
	{
		static Stock()
		{
			FontNormal = SystemFonts.MessageBoxFont;
			FontBold = new Font(FontNormal, FontStyle.Bold);

		}

		public static Font FontNormal, FontBold;
	}

	static void _Test()
	{
		//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tv();
		//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tb();
		//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tb_big();

		//RunUac.Test();

	}
}
