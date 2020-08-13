using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Au;
using Au.Types;

namespace Aedit
{
	public class Program
	{
		public static MainWindow wmain;

		[STAThread]
		static void Main(string[] args) {
			AOutput.QM2.UseQM2 = true;
			//ADebug.PrintLoadedAssemblies(true, true);

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
}
