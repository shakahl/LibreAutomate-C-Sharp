using System;
using System.Collections.Generic;
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
//using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;

static class CommandLine
{
	/// <summary>
	/// Processes command line of this program.
	/// Returns true if this instance must exit: 1. If finds previous program instance; then sends the command line to it if need. 2. If incorrect command line.
	/// </summary>
	public static bool OnProgramStarted(string[] a)
	{
		string s = null;
		int cmd = 0;
		if(a.Length > 0) {
			//Print(a);

			for(int i = 0; i < a.Length; i++) if(a[i].StartsWith_('-')) a[i] = a[i].ReplaceAt_(0, 1, "/");
			for(int i = 0; i < a.Length; i++) if(a[i].StartsWith_('/')) a[i] = a[i].ToLower_();

			s = a[0];
			if(s.StartsWith_('/')) {
				for(int i = 0; i < a.Length; i++) {
					s = a[i];
					switch(s) {
					//case "/x":
					//	if(cmd != 0 || ++i == a.Length) { Console.WriteLine("/x used incorrectly"); return true; }
					//	cmd = ;
					//	break;
					default:
						Console.WriteLine("unknown: " + s);
						return true;
					}
				}
			} else { //one or more files
				if(a.Length == 1 && FilesModel.IsWorkspaceDirectory(s)) {
					switch(cmd = AuDialog.ShowEx("Workspace", s, "1 Import|2 Open|0 Cancel", footerText: FilesModel.GetSecurityInfo(true))) {
					case 1: _importWorkspace = s; break;
					case 2: WorkspaceDirectory = s; break;
					}
				} else {
					cmd = 3;
					_importFiles = a;
				}
			}
		}

		var w = Wnd.FindFast(null, c_msgClass);
		if(w.Is0) return false;

		if(a.Length == 0) { //activate main window
			Wnd wMain = (Wnd)w.Send(Api.WM_USER);
			if(!wMain.Is0) wMain.ActivateLL();
		}

		switch(cmd) {
		case 3: //import files
			s = string.Join("\0", a);
			break;
		}
		if(cmd != 0) {
			Wnd.Misc.InterProcessSendData(w, cmd, s);
		}
		return true;
	}

	public static void OnAfterCreatedFormAndOpenedWorkspace()
	{
		try {
			if(_importWorkspace != null) Model.ImportWorkspace(_importWorkspace);
			else if(_importFiles != null) Model.ImportFiles(_importFiles);
		}
		catch(Exception ex) { Print(ex.Message); }

		Wnd.Misc.InterProcessEnableReceivingWM_COPYDATA();
		Wnd.Misc.MyWindow.RegisterClass(c_msgClass);
		_msgWnd = new MsgWindow();
		_msgWnd.CreateMessageWindow(c_msgClass);
	}

	/// <summary>
	/// null or workspace file specified in command line.
	/// </summary>
	public static string WorkspaceDirectory;

	static string _importWorkspace;
	static string[] _importFiles;

	const string c_msgClass = "Au.Editor.Msg";
	static MsgWindow _msgWnd;

	class MsgWindow :Wnd.Misc.MyWindow
	{
		protected override LPARAM WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
		{
			switch(message) {
			case Api.WM_COPYDATA:
				if(MainForm.IsClosed) return default;
				try { return _WmCopyData(wParam, lParam); }
				catch(Exception ex) { Print(ex.Message); }
				return default;
			case Api.WM_USER: //return main form window handle
				if(MainForm.IsClosed) return default;
				return MainForm.Handle;
			}

			return base.WndProc(w, message, wParam, lParam);
		}
	}

	static unsafe LPARAM _WmCopyData(LPARAM wParam, LPARAM lParam)
	{
		//Wnd wSender = (Wnd)wParam;
		int action = Wnd.Misc.InterProcessGetData(lParam, out var s, out var b, dataId => dataId >= 100);
		switch(action) {
		case 1:
			Model.ImportWorkspace(s);
			break;
		case 2:
			Panels.Files.LoadWorkspace(s);
			break;
		case 3:
			Api.ReplyMessage(1); //avoid 'wait' cursor while we'll show task dialog
			Model.ImportFiles(s.Split_("\0"));
			break;
		case 99: { //run script from command line; sent by Au.CL.exe
				var a = Au.Util.StringMisc.CommandLineToArray(s);
				if(a.Length == 0) return 0;
				var script = a[0];
				var f = Model?.Find(script, false);
				if(f == null) { Print($"Command line: script '{script}' not found."); return 2; }
				a = a.Length == 1 ? null : a.RemoveAt_(0);
				Run.CompileAndRun(true, f, a);
				//TODO: support start+wait
			}
			break;
		case 100: { //run script from script
				var a = Au.Util.LibSerializer.Deserialize(b);
				var script = a[0];
				var f = Model?.Find(script, false);
				if(f == null) return 2; //let the caller script throw 'script not found' exception
				Run.CompileAndRun(true, f, a[1]);
			}
			break;
		default:
			Debug.Assert(false);
			return 0;
		}
		return 1;
	}
}
