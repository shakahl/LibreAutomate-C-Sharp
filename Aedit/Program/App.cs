using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Au;
using Au.Types;

class Program
{
	internal static AOutputServer OutputServer;
	public static MainWindow Wnd;
	public static ProgramSettings Settings;
	public static string UserGuid;
	public const string AppName = "QM3";
	//public static FilesModel Model;
	//public static RunningTasks Tasks;

	[STAThread]
	static void Main(string[] args) {
		//#if !DEBUG
		AProcess.CultureIsInvariant = true;
		//#endif

#if true
		AppDomain.CurrentDomain.UnhandledException += (ad, e) => AOutput.Write(e.ExceptionObject);
		AOutput.QM2.UseQM2 = true;
		AOutput.Clear();
		//ADebug.PrintLoadedAssemblies(true, true);
#else
		AppDomain.CurrentDomain.UnhandledException += (ad, e) => ADialog.ShowError("Exception", e.ExceptionObject.ToString());
#endif

		AFolders.ThisAppDocuments = (FolderPath)(AFolders.Documents + "Aedit");

		//if (CommandLine.OnProgramStarted(args)) return;

		OutputServer = new AOutputServer(true) { NoNewline = true };
		OutputServer.Start();

		Api.SetErrorMode(Api.GetErrorMode() | Api.SEM_FAILCRITICALERRORS); //disable some error message boxes, eg when removable media not found; MSDN recommends too.
		Api.SetSearchPathMode(Api.BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE); //let SearchPath search in current directory after system directories

		//System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware); //no, we have manifest
		//System.Windows.Forms.Application.EnableVisualStyles(); //no, we have manifest
		//System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

		Settings = ProgramSettings.Load();

		UserGuid = Settings.user; if (UserGuid == null) Settings.user = UserGuid = Guid.NewGuid().ToString();

		//ATimer.Every(1000, t => _TimerProc(t));
		//note: timer can make Process Hacker/Explorer show CPU usage, even if we do nothing. Eg 0.02 if 250, 0.01 if 500, <0.01 if 1000.
		//Timer1s += () => AOutput.Write("1 s");
		//Timer1sOr025s += () => AOutput.Write("0.25 s");

		//ATime.SleepDoEvents(5000);
		_LoadUI();
		//ATimer.After(5000, _ => Wnd.Hide());
		//ATimer.After(7000, _ => Wnd.Show());

		OutputServer.Stop();
	}

	static void _LoadUI() {
		var app = new App();
		Wnd = new MainWindow();
		app.Run(Wnd);
	}
}

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
partial class App : Application
{
	//static MainWindow _mainWindow;

}
