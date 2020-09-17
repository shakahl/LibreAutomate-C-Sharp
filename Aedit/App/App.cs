using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Au;
using Au.Controls;
using Au.Types;

class App
{
	public const string AppName = "QM3";
	public static string UserGuid;
	internal static AOutputServer OutputServer;
	public static AppSettings Settings;
	public static MainWindow Wnd;
	public static AuPanels Panels;
	public static AuMenuCommands Commands;
	public static ToolBar[] Toolbars;
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

		//OutputServer = new AOutputServer(true) { NoNewline = true };
		//OutputServer.Start();

		Api.SetErrorMode(Api.GetErrorMode() | Api.SEM_FAILCRITICALERRORS); //disable some error message boxes, eg when removable media not found; MSDN recommends too.
		Api.SetSearchPathMode(Api.BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE); //let SearchPath search in current directory after system directories

		//System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware); //no, we have manifest
		//System.Windows.Forms.Application.EnableVisualStyles(); //no, we have manifest
		//System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

		Settings = AppSettings.Load();

		UserGuid = Settings.user; if (UserGuid == null) Settings.user = UserGuid = Guid.NewGuid().ToString();

		//ATimer.Every(1000, t => _TimerProc(t));
		//note: timer can make Process Hacker/Explorer show CPU usage, even if we do nothing. Eg 0.02 if 250, 0.01 if 500, <0.01 if 1000.
		//Timer1s += () => AOutput.Write("1 s");
		//Timer1sOr025s += () => AOutput.Write("0.25 s");

		//ATime.SleepDoEvents(5000);
		_LoadUI();
		//ATimer.After(5000, _ => Wnd.Hide());
		//ATimer.After(7000, _ => Wnd.Show());

		//OutputServer.Stop();
	}

	static void _LoadUI() {
		var app = new WpfApp();
		app.InitializeComponent();
		Wnd = new MainWindow();
		app.DispatcherUnhandledException += (_, e) => {
			e.Handled = 1 == ADialog.ShowError("Exception", e.Exception.ToStringWithoutStack(), "1 Continue|2 Exit", DFlags.Wider, Wnd, e.Exception.ToString());
		};
		app.Run(Wnd);
	}

	private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
		AOutput.Write(e.Exception);
		e.Handled = true;
	}
}

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
partial class WpfApp : Application
{
}
