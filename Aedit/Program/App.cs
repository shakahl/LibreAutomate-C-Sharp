using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Au;
using Au.Types;

public class Program
{
	public static MainWindow wmain;

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

		//ATime.SleepDoEvents(5000);
		_LoadUI();
		//ATimer.After(5000, _ => wmain.Hide());
		//ATimer.After(7000, _ => wmain.Show());
	}

	static void _LoadUI() {
		var app = new App();
		wmain = new MainWindow();
		app.Run(wmain);
	}
}

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	//static MainWindow _mainWindow;

}
