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

using Au;
using Au.Types;
using static Au.NoClass;

/// <summary>
/// Communicates with Au.Tasks process through pipe. Auto-starts the process if need.
/// </summary>
static class IpcWithTasks
{
	const string c_errorRunAuTasks = "Failed to start or connect to Au.Tasks.exe process. It is used to run tasks as admin etc.";

	static RegisteredWaitHandle _registeredAutoRestartProcess;

	/// <summary>
	/// Calls Program._Action() in Au.Tasks process. This is a low-level Call version.
	/// Returns _Action's return value, or 0 if fails, or 255 if !autoStart and process not running.
	/// </summary>
	/// <param name="autoStart">Start Au.Tasks process if not running. If false, then returns 255.</param>
	/// <param name="admin">If true, uses admin process, else non-admin non-uiAccess process. Tip: <see cref="CanRunTasksAs"/>.</param>
	/// <param name="args">Serialized arguments to pass to the process. Tip: <see cref="Au.Util.LibSerializer.Serialize"/>.</param>
	public static unsafe byte CallLL(bool autoStart, bool admin, byte[] args)
	{
		var pipeName = @"\\.\pipe\Au.Tasks-" + (admin ? "H-" : "M-") + Process_.CurrentSessionId.ToString();
		//pipeName = @"\\.\pipe\Au.Tasks-H-2"; //test from other user session. Works.

		for(int nTry = 3; nTry > 0; nTry--) {
			if(!Api.WaitNamedPipe(pipeName, 1000)) {
				switch(Native.GetError()) {
				case Api.ERROR_FILE_NOT_FOUND:
					if(!autoStart) return 255;
					if(!_StartAuTasksProcess(admin)) return 0;
					if(!WaitFor.Condition(10, () => Api.WaitNamedPipe(pipeName, -1))) continue;
					break;
				default:
					Debug_.Print(Native.GetErrorMessage());
					continue;
				}
			} else if(autoStart && _registeredAutoRestartProcess == null) {
				Print("_registeredAutoRestartProcess null");
				int pid;
#if true
				pid = Process_.GetProcessId("Au.Tasks.exe", ofThisSession: true);
#else //this way would be more precise/correct/fast, but it breaks pipe, don't know why. The same if called before waitnamedpipe.
				using(var hPipe = Api.CreateFile(pipeName, Api.GENERIC_READ, 0, null, Api.OPEN_EXISTING)) {
					if(!GetNamedPipeServerProcessId(hPipe.DangerousGetHandle(), out pid)) Debug_.Print(Native.GetErrorMessage());
				}
#endif
				if(pid != 0) _SetAutoRestartAuTasksProcess(_PidToWaitHandle(pid), admin);
			}
			//Perf.Next();
			fixed (byte* p = args) if(Api.CallNamedPipe(pipeName, p, args.Length, out byte R, 1, out int nRead, 1000)) return R;
			Debug_.Print(Native.GetErrorMessage());
		}
		Print(c_errorRunAuTasks);
		return 0;
	}

	/// <summary>
	/// Calls Program._Action() in Au.Tasks process. Auto-starts the process.
	/// Returns _Action's return value, or 0 if fails.
	/// </summary>
	/// <param name="admin">If true, uses admin process, else non-admin non-uiAccess process. Tip: <see cref="CanRunTasksAs"/>.</param>
	/// <param name="args">Arguments to pass to the process. Uses <see cref="Au.Util.LibSerializer.Serialize"/>.</param>
	public static byte Call(bool admin, params Au.Util.LibSerializer.Value[] args)
		=> CallLL(true, admin, Au.Util.LibSerializer.Serialize(args));

	/// <summary>
	/// Calls Program._Action() in Au.Tasks process if it is running.
	/// Returns _Action's return value, or 0 if fails, or 255 if process not running.
	/// </summary>
	/// <param name="admin">If true, uses admin process, else non-admin non-uiAccess process. Tip: <see cref="CanRunTasksAs"/>.</param>
	/// <param name="args">Arguments to pass to the process. Uses <see cref="Au.Util.LibSerializer.Serialize"/>.</param>
	public static byte CallIfRunning(bool admin, params Au.Util.LibSerializer.Value[] args)
		=> CallLL(false, admin, Au.Util.LibSerializer.Serialize(args));

	static bool _StartAuTasksProcess(bool admin)
	{
		Debug.Assert(CanRunTasksAs(admin));
		try {
			WaitHandle wh = null;
			var tasksPath = Folders.ThisAppBS + "Au.Tasks.exe";
			if(admin) {
				//TODO: what if it is not that Au.Tasks.exe? Maybe pass tasksPath as cmdline, and let it run it if different.
				int pid = 0;
				try { pid = Au.Util.LibTaskScheduler.RunTask("Au", "Au.Tasks"); }
				catch(Exception ex) { Debug_.Print(ex); }
				if(pid != 0) {
					wh = _PidToWaitHandle(pid);
				} else {
					var rr = Shell.Run(tasksPath, null, SRFlags.Admin | SRFlags.NeedProcessHandle);
					wh = rr.ProcessHandle;
				}
			} else if(Process_.UacInfo.ThisProcess.IntegrityLevel == UacIL.UIAccess) {
				wh = Process_.LibStart(tasksPath, null, inheritUiaccess: false, ret: Process_.EStartReturn.WaitHandle) as WaitHandle;
			} else {
				wh = Process_.LibStartUserIL(tasksPath, null, ret: Process_.EStartReturn.WaitHandle) as WaitHandle;
			}

			//auto-restart process if crashed/killed. Would be not necessary, but it may contain trigger engines.
			Debug.Assert(wh != null);
			_SetAutoRestartAuTasksProcess(wh, admin);
		}
		catch(AuException ex) { Print(c_errorRunAuTasks + "\r\n\t" + ex.Message); return false; } //eg user cancelled the UAC consent dialog
		return true;
	}

	static void _SetAutoRestartAuTasksProcess(WaitHandle hProcess, bool admin)
	{
		if(hProcess == null) return;
		_registeredAutoRestartProcess = ThreadPool.RegisterWaitForSingleObject(hProcess, (context, wasSignaled) => {
			_registeredAutoRestartProcess.Unregister(hProcess); _registeredAutoRestartProcess = null;
			_StartAuTasksProcess((bool)context);
			//Thread_.Start(() => _StartAuTasksProcess((bool)context));
			Debug_.Print("Au.Tasks process died");
			//Print(Thread_.NativeId);
		}, admin, -1, true);
		//TODO: restart only if trigger engines are there. Then also need to restart trigger engines.
		//TODO: make _registeredAutoRestartProcess thread-safe.
	}

	static WaitHandle _PidToWaitHandle(int pid)
	{
		WaitHandle wh = null;
		try { wh = new Au.Util.LibKernelWaitHandle(Au.Util.LibKernelHandle.OpenProcess(pid, Api.SYNCHRONIZE), true); }
		catch(Exception ex) { Debug_.Print(ex); }
		return wh;
	}

	/// <summary>
	/// If admin==true, returns true if UAC IL of this process is Medium or UIAccess.
	/// If admin==false, returns true if UAC IL of this process is High or UIAccess.
	/// </summary>
	public static bool CanRunTasksAs(bool admin)
	{
		var IL = Process_.UacInfo.ThisProcess.IntegrityLevel;
		switch(IL) {
		case UacIL.High: return !admin && !Process_.UacInfo.IsUacDisabled;
		case UacIL.Medium: return admin;
		case UacIL.UIAccess: return true;
		}
		return false;
	}
}

//rejected. Instead use CommandLine.MsgWindow. 
///// <summary>
///// Native message-only window in the main thread. Processes various messages, for example "run script".
///// </summary>
//static class IpcMsgWindow
//{
//	const string c_className = "Au.Editor.Msg";
//	static Wnd s_wnd;

//	public static void Create()
//	{
//		Wnd.Misc.MyWindow.RegisterClass(c_className);
//	}

//	public static void Destroy()
//	{

//	}

//	class _MsgWindow :Wnd.Misc.MyWindow
//	{

//	}
//}
