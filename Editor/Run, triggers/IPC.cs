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
/// Communicates with Au.HI process through pipe. Auto-starts the process if need.
/// </summary>
static class IpcWithHI
{
	const string c_errorRunHi = "Failed to start or connect to Au.HI process. It is used to run tasks as admin etc.";

	static RegisteredWaitHandle _registeredAutoRestartProcess;

	/// <summary>
	/// Calls Program._Action() in Au.HI process. This is a low-level Call version.
	/// Returns _Action's return value, or 0 if fails, or -1 if !autoStart and process not running.
	/// </summary>
	/// <param name="autoStart">Start Au.HI process if not running. If false, then returns -1.</param>
	/// <param name="args">Serialized arguments to pass to the process. Tip: <see cref="Au.Util.LibSerializer.Serialize"/>.</param>
	public static unsafe int CallLL(bool autoStart, byte[] args)
	{
		var pipeName = @"\\.\pipe\Au.HI-" + Process_.CurrentSessionId.ToString();
		//pipeName = @"\\.\pipe\Au.HI-2"; //test from other user session. Works.

		for(int nTry = 3; nTry > 0; nTry--) {
			if(!Api.WaitNamedPipe(pipeName, 1000)) {
				switch(Native.GetError()) {
				case Api.ERROR_FILE_NOT_FOUND:
					if(!autoStart) return -1;
					if(!_StartHiProcess()) return 0;
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
				pid = Process_.GetProcessId("Au.HI.exe", ofThisSession: true);
#else //this way would be more precise/correct/fast, but it breaks pipe, don't know why. The same if called before waitnamedpipe.
				using(var hPipe = Api.CreateFile(pipeName, Api.GENERIC_READ, 0, null, Api.OPEN_EXISTING)) {
					if(!GetNamedPipeServerProcessId(hPipe.DangerousGetHandle(), out pid)) Debug_.Print(Native.GetErrorMessage());
				}
#endif
				if(pid != 0) _SetAutoRestartHiProcess(_PidToWaitHandle(pid));
			}
			//Perf.Next();
			fixed (byte* p = args) if(Api.CallNamedPipe(pipeName, p, args.Length, out int R, 4, out int nRead, 1000)) return R;
			Debug_.Print(Native.GetErrorMessage());
		}
		Print(c_errorRunHi);
		return 0;
	}

	/// <summary>
	/// Calls Program._Action() in Au.HI process. Auto-starts the process.
	/// Returns _Action's return value, or 0 if fails.
	/// </summary>
	/// <param name="args">Arguments to pass to the process. Uses <see cref="Au.Util.LibSerializer.Serialize"/>.</param>
	public static int Call(params Au.Util.LibSerializer.Value[] args)
		=> CallLL(true, Au.Util.LibSerializer.Serialize(args));

	/// <summary>
	/// Calls Program._Action() in Au.HI process if it is running.
	/// Returns _Action's return value, or 0 if fails, or -1 if process not running.
	/// </summary>
	/// <param name="args">Arguments to pass to the process. Uses <see cref="Au.Util.LibSerializer.Serialize"/>.</param>
	public static int CallIfRunning(params Au.Util.LibSerializer.Value[] args)
		=> CallLL(false, Au.Util.LibSerializer.Serialize(args));

	static bool _StartHiProcess()
	{
		Debug.Assert(CanRunHi());
		try {
			WaitHandle wh = null;
			var hiPath = Folders.ThisAppBS + "Au.HI.exe";
			int pid = 0;
			try { pid = Au.Util.LibTaskScheduler.RunTask("Au", "Au.HI"); }
			catch(Exception ex) { Debug_.Print(ex); }
			if(pid != 0) {
				wh = _PidToWaitHandle(pid);
			} else {
				var rr = Shell.Run(hiPath, null, SRFlags.Admin | SRFlags.NeedProcessHandle);
				wh = rr.ProcessHandle;
			}

			//auto-restart process if crashed/killed. Would be not necessary, but it may contain trigger engines.
			Debug.Assert(wh != null);
			_SetAutoRestartHiProcess(wh);
		}
		catch(AuException ex) { Print(c_errorRunHi + "\r\n\t" + ex.Message); return false; } //eg user cancelled the UAC consent dialog
		return true;
	}

	static void _SetAutoRestartHiProcess(WaitHandle hProcess)
	{
		if(hProcess == null) return;
		_registeredAutoRestartProcess = ThreadPool.RegisterWaitForSingleObject(hProcess, (context, wasSignaled) => {
			_registeredAutoRestartProcess.Unregister(hProcess); _registeredAutoRestartProcess = null;
			hProcess.Dispose();
			_StartHiProcess();
			//Thread_.Start(() => _StartHiProcess();
			Debug_.Print("Au.HI process died");
			//Print(Thread_.NativeId);
		}, null, -1, true);
		//TODO: restart only if trigger engines are there. Then also need to restart trigger engines.
		//TODO: make _registeredAutoRestartProcess thread-safe.
	}

	static WaitHandle _PidToWaitHandle(int pid) => Au.Util.LibKernelWaitHandle.FromProcessId(pid, Api.SYNCHRONIZE);

	/// <summary>
	/// Returns true if UAC IL of this process is Medium or UIAccess.
	/// </summary>
	public static bool CanRunHi()
	{
		switch(Process_.UacInfo.ThisProcess.IntegrityLevel) { case UacIL.Medium: case UacIL.UIAccess: return true; }
		return false;
	}
}
