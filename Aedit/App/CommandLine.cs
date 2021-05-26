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
//using System.Linq;

using Au;
using Au.Types;
using Au.Util;

//FUTURE: /portable
//	1. Set AFolders.ThisAppDocuments etc in AFolders.ThisApp\Portable.
//	2. Don't restart as admin.
//	3. Don't allow to set option to run at startup.
//	4. Etc.

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
		bool activateWnd = true;
		if(a.Length > 0) {
			//AOutput.Write(a);

			for(int i = 0; i < a.Length; i++) {
				if(a[i].Starts('-')) a[i] = a[i].ReplaceAt(0, 1, "/");
				if(a[i].Starts('/')) a[i] = a[i].Lower();
			}

			s = a[0];
			if(s.Starts('/')) {
				for(int i = 0; i < a.Length; i++) {
					s = a[i];
					switch(s) {
					case "/test":
						if(++i < a.Length) TestArg = a[i];
						break;
					case "/v":
						StartVisible = true;
						break;
					default:
						ADialog.ShowError("Unknown command line parameter", s);
						return true;
					}
					//rejected: /h start hidden. Not useful.
				}
			} else { //one or more files
				if(a.Length == 1 && FilesModel.IsWorkspaceDirectoryOrZip_ShowDialogOpenImport(s, out cmd)) {
					switch(cmd) {
					case 1: WorkspaceDirectory = s; break;
					case 2: _importWorkspace = s; break;
					}
				} else {
					cmd = 3;
					_importFiles = a;
				}
			}
		}

		//single instance
		//SHOULDDO: Api.CreateMutex()
		s_mutex = new Mutex(true, "Aedit.Mutex.m3gVxcTJN02pDrHiQ00aSQ", out bool createdNew);
		if(createdNew) return false;

		var w = AWnd.FindFast(null, c_msgWndClassName, true);
		if(!w.Is0) {
			if(activateWnd) {
				AWnd wMain = (AWnd)w.Send(Api.WM_USER);
				if(!wMain.Is0) {
					try { wMain.Activate(); }
					catch(Exception ex) { ADebug.Print(ex); }
				}
			}

			switch(cmd) {
			case 3: //import files
				s = string.Join("\0", a);
				break;
			}
			if(cmd != 0) {
				AWnd.More.CopyDataStruct.SendString(w, cmd, s);
			}
		}
		return true;
	}
	static Mutex s_mutex; //GC

	/// <summary>
	/// null or argument after "/test".
	/// </summary>
	public static string TestArg;

	/// <summary>
	/// true if /v
	/// </summary>
	public static bool StartVisible;

	public static void OnProgramLoaded()
	{
		AWnd.More.UacEnableMessages(Api.WM_COPYDATA, /*Api.WM_DROPFILES, 0x0049,*/ Api.WM_USER, Api.WM_CLOSE);
		//WM_COPYDATA, WM_DROPFILES and undocumented WM_COPYGLOBALDATA=0x0049 should enable drag/drop from lower UAC IL processes, but only through WM_DROPFILES/DragAcceptFiles, not OLE D&D.

		AWnd.More.RegisterWindowClass(c_msgWndClassName, _WndProc);
		_msgWnd = AWnd.More.CreateMessageOnlyWindow(c_msgWndClassName);

		if(_importWorkspace != null || _importFiles != null) {
			ATimer.After(10, _ => {
				try {
					App.Wmain.ZShowAndActivate();
					if(_importWorkspace != null) App.Model.ImportWorkspace(_importWorkspace);
					else App.Model.ImportFiles(_importFiles);
				}
				catch(Exception ex) { AOutput.Write(ex.Message); }
			});
		}
	}

	/// <summary>
	/// null or workspace file specified in command line.
	/// </summary>
	public static string WorkspaceDirectory;

	static string _importWorkspace;
	static string[] _importFiles;

	static AWnd _msgWnd;
	const string c_msgWndClassName = "Aedit.m3gVxcTJN02pDrHiQ00aSQ";

	/// <summary>
	/// The message-only window.
	/// Don't call before the program is fully inited and OnMainFormLoaded called.
	/// </summary>
	public static AWnd MsgWnd => _msgWnd;

	static nint _WndProc(AWnd w, int message, nint wParam, nint lParam)
	{
		switch(message) {
		case Api.WM_COPYDATA:
			if(App.Loaded >= EProgramState.Unloading) return default;
			try { return _WmCopyData(wParam, lParam); }
			catch(Exception ex) { AOutput.Write(ex.Message); }
			return default;
		case Api.WM_USER:
			if(App.Loaded >= EProgramState.Unloading) return default;
			int i = (int)wParam;
			switch(i) {
			case 0:
				return App.Hwnd.Handle;
			case 10:
				UacDragDrop.AdminProcess.OnTransparentWindowCreated((AWnd)lParam);
				break;
			case 20: //from Triggers.DisabledEverywhere
				TriggersAndToolbars.OnDisableTriggers();
				break;
			}
			return 0;
		case RunningTasks.WM_TASK_ENDED: //WM_USER+900
			App.Tasks.TaskEnded2(wParam, lParam);
			return 0;
		}

		return Api.DefWindowProc(w, message, wParam, lParam);
	}

	static nint _WmCopyData(nint wParam, nint lParam)
	{
		var c = new AWnd.More.CopyDataStruct(lParam);
		int action = c.DataId;
		bool isString = action < 100;
		string s = isString ? c.GetString() : null;
		byte[] b = isString ? null : c.GetBytes();
		switch(action) {
		case 1:
			FilesModel.LoadWorkspace(s);
			break;
		case 2:
			App.Model.ImportWorkspace(s);
			break;
		case 3:
			Api.ReplyMessage(1); //avoid 'wait' cursor while we'll show task dialog
			App.Model.ImportFiles(s.Split('\0'));
			break;
		case 4:
			var f1 = APath.IsFullPath(s) ? App.Model.FindByFilePath(s) : App.Model.Find(s, null);
			if(f1 != null) App.Model.OpenAndGoTo(f1, (int)wParam - 1);
			else AWarning.Write($"Script '{s}' not found.", -1);
			break;
		case 99: //run script from Au.CL.exe command line
		case 100: //run script from script (ATask.Run/RunWait)
			int mode = (int)wParam; //1 - wait, 3 - wait and get ATask.WriteResult output
			string script; string[] args; string pipeName = null;
			if(action == 99) {
				var a = AStringUtil.CommandLineToArray(s); if(a.Length == 0) return 0;
				int nRemove = 0;
				if(0 != (mode & 2)) pipeName = a[nRemove++];
				script = a[nRemove++];
				args = a.Length == nRemove ? null : a.RemoveAt(0, nRemove);
			} else {
				var d = Serializer_.Deserialize(b);
				script = d[0]; args = d[1]; pipeName = d[2];
			}
			var f = App.Model?.FindFile(script);
			if(f == null) {
				if(action == 99) AOutput.Write($"Command line: script '{script}' not found."); //else the caller script will throw exception
				return (int)ATask.RunResult_.notFound;
			}
			return CompileRun.CompileAndRun(true, f, args, noDefer: 0 != (mode & 1), wrPipeName: pipeName);
		case 110: //received from our non-admin drop-target process on OnDragEnter
			return UacDragDrop.AdminProcess.DragEvent((int)wParam, b);
		//case 120: //go to edit user-defined menu or toolbar source code
		//	CiGoTo.EditMenuOrToolbar(b);
		//	break;
		default:
			Debug.Assert(false);
			return 0;
		}
		return 1;
	}
}
