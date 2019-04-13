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
		if(args.Length > 0 && args[0] == "/d") {
			UacDragDrop.NonAdminProcess.MainDD(args);
			return;
		}

		//restart as admin if started as non-admin on admin user account
		if(args.Length > 0 && args[0] == "/n") {
			args = args.RemoveAt_(0);
		} else if(!Uac.IsAdmin && Uac.OfThisProcess.Elevation == UacElevation.Limited) {
			if(_RestartAsAdmin(args)) return;
		}
		//speed with restarting is the same as when runs as non-admin. The fastest is when started as admin. Because faster when runs as admin.

		_Main(args);
	}

	static void _Main(string[] args)
	{
		Perf.First();

		//Test(); return;
		//Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1; //test how works with 1 CPU
		//Task.Run(() => { while(true) { Thread.Sleep(1000); GC.Collect(); } });
		//AppDomain.CurrentDomain.AssemblyLoad += (object sender, AssemblyLoadEventArgs ala) => Print(ala.LoadedAssembly);

		if(CommandLine.OnProgramStarted(args)) return;

#if !DEBUG
		var fProfile = Folders.ThisAppDataLocal + "ProfileOptimization";
		File_.CreateDirectory(fProfile);
		ProfileOptimization.SetProfileRoot(fProfile);
		ProfileOptimization.StartProfile("Editor.speed"); //makes startup faster eg 680 -> 560 ms. Makes compiler startup faster 4000 -> 2500 (ngen 670).
		Perf.Next();
#endif

		OutputServer.NoNewline = true;
		OutputServer.Start();

		Api.SetErrorMode(Api.GetErrorMode() | Api.SEM_FAILCRITICALERRORS); //disable some error message boxes, eg when removable media not found; MSDN recommends too.
		Api.SetSearchPathMode(Api.BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE); //let SearchPath search in current directory after system directories

		//Application.EnableVisualStyles(); //we have manifest
		Application.SetCompatibleTextRenderingDefault(false);

		Settings = new ProgramSettings();

		if(!Settings.Get("user", out UserGuid)) Settings.Set("user", Guid.NewGuid().ToString());

		Timer_.Every(1000, t => _TimerProc(t));
		//note: timer can make Process Hacker show constant CPU, even if we do nothing. Eg 0.02 if 250, 0.01 if 500, 0 of 1000.
		//Timer1s += () => Print("1 s");
		//Timer1sOr025s += () => Print("0.25 s");

		EdForm.RunApplication();

		OutputServer.Stop();
	}

	internal static Au.Util.OutputServer OutputServer = new Au.Util.OutputServer(true);
	internal static ProgramSettings Settings;
	internal static EdForm MainForm;
	internal static FilesModel Model;
	internal static RunningTasks Tasks;
	internal static string UserGuid;

	/// <summary>
	/// Timer with 1 s period.
	/// </summary>
	internal static event Action Timer1s;

	/// <summary>
	/// Timer with 1 s period when main window hidden and 0.25 s period when visible.
	/// </summary>
	internal static event Action Timer1sOr025s;

	/// <summary>
	/// True if Timer1sOr025s fires every 0.25 s (when main window visible), false if every 1 s (when hidden).
	/// </summary>
	internal static bool IsTimer025 => s_timerCounter > 0;
	static uint s_timerCounter;

	static void _TimerProc(Timer_ t)
	{
		bool needFast = (MainForm?.IsLoaded ?? false) && MainForm.Visible;
		if(needFast != (s_timerCounter > 0)) t.Start(needFast ? 250 : 1000, false);
		if(needFast) {
			Timer1sOr025s?.Invoke();
			s_timerCounter++;
			if(MousePosChangedWhenProgramVisible != null) {
				var p = Mouse.XY;
				if(p != s_mousePos) {
					s_mousePos = p;
					MousePosChangedWhenProgramVisible(p);
				}
			}
		} else s_timerCounter = 0;
		if(0 == (s_timerCounter & 3)) Timer1s?.Invoke();
	}
	static POINT s_mousePos;

	/// <summary>
	/// When cursor position changed while the main window is visible.
	/// Called at max 0.25 s rate, not for each change.
	/// Cursor can be in any window. Does not depend on UAC.
	/// Receives cursor position in screen.
	/// </summary>
	internal static event Action<POINT> MousePosChangedWhenProgramVisible;

	static bool _RestartAsAdmin(string[] args)
	{
		if(Debugger.IsAttached) return false;
		try {
			//int pid = 
			Au.Util.LibTaskScheduler.RunTask(@"Au", "Au.Editor", true, args);
			//Api.AllowSetForegroundWindow(pid); //fails and has no sense, because it's Au.CL.exe running as SYSTEM
		}
		catch(Exception ex) { //probably this program is not installed (no scheduled task)
			Debug_.Dialog(ex);
			return false;
		}
		return true;
	}

	internal static void Test()
	{
		//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tv();
		//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tb();
		//ETest.DevTools.CreatePngImagelistFileFromIconFiles_il_tb_big();

		//RunUac.Test();

		//Output.LibUseQM2 = true; Output.Clear();
		//using(var h = Au.Util.WinHook.Keyboard(k => {
		//	Print($"{k.Key}, {!k.IsUp}");
		//	if(k.Key == KKey.Up && !k.IsUp) 400.ms();

		//	return false;
		//})) AuDialog.Show("hook");
	}
}
