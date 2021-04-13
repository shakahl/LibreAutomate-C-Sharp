using Au.Types;
using Au.Util;
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

namespace Au
{
	/// <summary>
	/// Automation task - a running script.
	/// </summary>
	/// <seealso cref="AProcess"/>
	public static class ATask
	{
		/// <summary>
		/// In an automation task process of a script with role miniProgram (defaut) returns script file name without extension.
		/// In other processes returns <see cref="AppDomain.FriendlyName"/>, like "ProgramFile".
		/// </summary>
		public static string Name => s_name ??= AppDomain.CurrentDomain.FriendlyName; //info: in framework 4 with ".exe", now without
		static string s_name;

		/// <summary>
		/// In an automation task process tells whether the task runs in host process (default), editor process or own .exe process. It matches meta role.
		/// In other processes always returns <b>ExeProgram</b>.
		/// </summary>
		public static ATRole Role { get; private set; }

		internal static void Init_(ATRole role, string name = null) {
			Role = role;
			if (name != null) s_name = name;
		}

		/// <summary>
		/// Gets task's assembly containing Main function.
		/// </summary>
		/// <remarks>
		/// In an automation task process of a script with role miniProgram (defaut) it is not the same as <see cref="Assembly.GetEntryAssembly"/> which then gets the host assembly.
		/// </remarks>
		public static Assembly MainAssembly => s_mainAssembly ??= Assembly.GetEntryAssembly();
		internal static Assembly s_mainAssembly;

		/// <summary>
		/// Starts an automation task. Does not wait.
		/// </summary>
		/// <returns>
		/// Native process id of the task process.
		/// Returns 0 if task start is deferred because a script is running (see meta option ifRunning).
		/// Returns 0 if role editorExtension; then waits until the task ends.
		/// Returns -1 if failed to start the script, for example it contains errors or cannot run because a script is running.
		/// </returns>
		/// <param name="script">Script name like "Script5.cs", or path like @"\Folder\Script5.cs".</param>
		/// <param name="args">Command line arguments. In script it will be variable <i>args</i>. Should not contain '\0' characters.</param>
		/// <exception cref="FileNotFoundException">Script file not found.</exception>
		public static int Run(string script, params string[] args)
			=> _Run(0, script, args, out _);

		/// <summary>
		/// Starts an automation task and waits until it ends.
		/// More info: <see cref="Run"/>.
		/// </summary>
		/// <returns>The exit code of the task process. See <see cref="Environment.ExitCode"/>.</returns>
		/// <exception cref="FileNotFoundException">Script file not found.</exception>
		/// <exception cref="AuException">Failed to start script.</exception>
		public static int RunWait(string script, params string[] args)
			=> _Run(1, script, args, out _);

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <summary>
		/// Starts an automation task, waits until it ends and gets its strings written by <see cref="WriteResult"/>.
		/// More info: <see cref="Run"/>.
		/// </summary>
		/// <param name="results">Receives <see cref="WriteResult"/> output.</param>
		/// <returns>The exit code of the task process. See <see cref="Environment.ExitCode"/>.</returns>
		/// <exception cref="FileNotFoundException">Script file not found.</exception>
		/// <exception cref="AuException">Failed to start script.</exception>
		public static int RunWait(out string results, string script, params string[] args)
			=> _Run(3, script, args, out results);

		/// <summary>
		/// Starts an automation task, waits until it ends and gets its strings written by <see cref="WriteResult"/>.
		/// More info: <see cref="Run"/>.
		/// </summary>
		/// <param name="results">Receives <see cref="WriteResult"/> output whenever the task calls it.</param>
		/// <returns>The exit code of the task process. See <see cref="Environment.ExitCode"/>.</returns>
		/// <exception cref="FileNotFoundException">Script file not found.</exception>
		/// <exception cref="AuException">Failed to start script.</exception>
		public static int RunWait(Action<string> results, string script, params string[] args)
			=> _Run(3, script, args, out _, results);
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

		static int _Run(int mode, string script, string[] args, out string resultS, Action<string> resultA = null) {
			var w = WndMsg_; if (w.Is0) throw new AuException("Au editor not found."); //CONSIDER: run editor program, if installed
			bool waitMode = 0 != (mode & 1), needResult = 0 != (mode & 2); resultS = null;
			using var tr = new _TaskResults();
			if (needResult && !tr.Init()) throw new AuException("*get task results");

			var data = Serializer_.Serialize(script, args, tr.pipeName);
			int pid = (int)AWnd.More.CopyDataStruct.SendBytes(w, 100, data, mode);
			switch ((RunResult_)pid) {
			case RunResult_.failed: return !waitMode ? -1 : throw new AuException("*start task"); //don't throw, eg maybe cannot run because other "runSingle" script is running
			case RunResult_.notFound: throw new FileNotFoundException($"Script '{script}' not found.");
			case RunResult_.editorThread: case RunResult_.deferred: return 0;
			}

			if (waitMode) {
				using var hProcess = WaitHandle_.FromProcessId(pid, Api.SYNCHRONIZE | Api.PROCESS_QUERY_LIMITED_INFORMATION);
				if (hProcess == null) throw new AuException("*wait for task");

				if (!needResult) hProcess.WaitOne(-1);
				else if (!tr.WaitAndRead(hProcess, resultA)) throw new AuException("*get task result");
				else if (resultA == null) resultS = tr.ResultString;

				if (!Api.GetExitCodeProcess(hProcess.SafeWaitHandle.DangerousGetHandle(), out pid)) pid = int.MinValue;
			}
			return pid;
		}

		internal enum RunResult_ //note: sync with ERunResult in Au.CL.cpp.
		{
			failed = 0,
			deferred = -1,
			notFound = -2,
			editorThread = -3,
		}

		unsafe struct _TaskResults : IDisposable
		{
			Handle_ _hPipe;
			public string pipeName;
			string _s;
			StringBuilder _sb;

			public bool Init() {
				var tid = AThread.Id;
				pipeName = @"\\.\pipe\Au.CL-" + tid.ToString();
				_hPipe = Api.CreateNamedPipe(pipeName,
					Api.PIPE_ACCESS_INBOUND | Api.FILE_FLAG_OVERLAPPED, //use async pipe because also need to wait for task process exit
					Api.PIPE_TYPE_MESSAGE | Api.PIPE_READMODE_MESSAGE | Api.PIPE_REJECT_REMOTE_CLIENTS,
					1, 0, 0, 0, Api.SECURITY_ATTRIBUTES.ForPipes);
				return !_hPipe.Is0;
			}

			public bool WaitAndRead(WaitHandle hProcess, Action<string> results) {
				bool R = false;
				char* b = null; const int bLen = 7900;
				var ev = new ManualResetEvent(false);
				try {
					var ha = new WaitHandle[2] { ev, hProcess };
					for (bool useSB = false; ; useSB = results == null) {
						var o = new Api.OVERLAPPED { hEvent = ev.SafeWaitHandle.DangerousGetHandle() };
						if (!Api.ConnectNamedPipe(_hPipe, &o)) {
							int e = ALastError.Code;
							if (e != Api.ERROR_PIPE_CONNECTED) {
								if (e != Api.ERROR_IO_PENDING) break;
								int wr = WaitHandle.WaitAny(ha);
								if (wr != 0) { Api.CancelIo(_hPipe); R = true; break; } //task ended
								if (!Api.GetOverlappedResult(_hPipe, ref o, out _, false)) { Api.DisconnectNamedPipe(_hPipe); break; }
							}
						}

						if (b == null) b = (char*)AMemory.Alloc(bLen);
						bool readOK;
						while (((readOK = Api.ReadFile(_hPipe, b, bLen, out int n, null)) || (ALastError.Code == Api.ERROR_MORE_DATA)) && n > 0) {
							n /= 2;
							if (!readOK) useSB = true;
							//AOutput.Write(useSB, n);
							if (useSB) { //rare
								_sb ??= new StringBuilder(bLen);
								if (results == null && _s != null) _sb.Append(_s);
								_s = null;
								_sb.Append(b, n);
							} else {
								_s = new string(b, 0, n);
							}
							if (readOK) {
								if (results != null) {
									results(ResultString);
									_sb?.Clear();
								}
								break;
							}
							//note: MSDN says must use OVERLAPPED with ReadFile too, but works without it.
						}
						Api.DisconnectNamedPipe(_hPipe);
						if (!readOK) break;
					}
				}
				finally {
					ev.Dispose();
					AMemory.Free(b);
				}
				return R;
			}

			public string ResultString => _s ?? _sb?.ToString();

			public void Dispose() => _hPipe.Dispose();
		};

		/// <summary>
		/// Finds editor's message-only window used with WM_COPYDATA etc.
		/// </summary>
		internal static AWnd WndMsg_ => s_wndMsg.FindFastCached_(ref s_wmTime, null, "Aedit.m3gVxcTJN02pDrHiQ00aSQ", true);
		static AWnd s_wndMsg;
		static long s_wmTime;

		/// <summary>
		/// Writes a string result for the task that called <see cref="RunWait(out string, string, string[])"/> or <see cref="RunWait(Action{string}, string, string[])"/> to run this task, or for the program that executed "Au.CL.exe" to run this task with command line like "Au.CL.exe **Script5.cs".
		/// Returns false if this task was not started in such a way. Returns null if failed to write, except when s is null/"".
		/// </summary>
		/// <param name="s">A string. This function does not append newline characters.</param>
		/// <remarks>
		/// The <b>RunWait</b>(Action) overload can read the string in real time.
		/// The <b>RunWait</b>(out string) overload gets all strings joined when the task ends.
		/// The program that executed "Au.CL.exe" to run this task with command line like "Au.CL.exe **Script5.cs" can read the string from the redirected standard output in real time, or the string is written to its console in real time. The string encoding is UTF8; if you use a bat file or cmd.exe and want to get correct Unicode text, execute this before, to change console code page to UTF-8: <c>chcp 65001</c>.
		/// </remarks>
		public static unsafe bool WriteResult(string s) {
			var pipeName = Environment.GetEnvironmentVariable("ATask.WriteResult.pipe");
			if (pipeName == null) return false;
			if (!s.NE()) {
				if (!Api.WaitNamedPipe(pipeName, 3000)) goto ge;
				using (var pipe = Api.CreateFile(pipeName, Api.GENERIC_WRITE, 0, default, Api.OPEN_EXISTING, 0)) {
					if (pipe.Is0) goto ge;
					fixed (char* p = s) if (!Api.WriteFile(pipe, p, s.Length * 2, out _)) goto ge;
				}
			}
			return true;
			ge:
			ADebug.PrintNativeError_();
			return false;
		}

		//public static void EndEvent(bool desktopSwitch = false, bool pcSleep = false, KHotkey hotkey = default, int exitCode = 0) {
		//	AThread.Start(() => {
		//		//Thread.Sleep(100);
		//		using var hook = new AHookAcc(AccEVENT.SYSTEM_DESKTOPSWITCH, 0, k => {
		//			k.hook.Dispose();
		//			Environment.Exit(exitCode);
		//		});

		//		while (Api.GetMessage(out var m) > 0) Api.DispatchMessage(m);
		//	}, background: true, sta: false);
		//}

		/// <summary>
		/// Sets to end this process when the active desktop has been switched (PC locked, Ctrl+Alt+Delete, screen saver, UAC consent, etc).
		/// </summary>
		/// <remarks>
		/// It allows to end this process with hotkeys Win+L (lock computer) and Ctrl+Alt+Delete.
		/// 
		/// Most mouse, keyboard, clipboard and window functions don't work when other desktop is active. Many of them then throw exception, and the script would end anyway.
		/// </remarks>
		/// <param name="exitCode">Process exit code. See <see cref="Environment.Exit"/>.</param>
		public static void EndOnDesktopSwitch(int exitCode = 0) {
			if (s_endOnDesktopSwitch) return; s_endOnDesktopSwitch = true;
			AThread.Start(() => {
				//Thread.Sleep(100);
				using var hook = new AHookAcc(AccEVENT.SYSTEM_DESKTOPSWITCH, 0, k => {
					k.hook.Dispose();
					Environment.Exit(exitCode);
				});

				while (Api.GetMessage(out var m) > 0) Api.DispatchMessage(m);
			}, background: true, sta: false);

			//tested: on Win+L works immediately. OS switches desktop 2 times. At first briefly, then makes defaul again, then on key etc switches again to show password field.
		}

		//TODO: maybe these should be events.

		/// <summary>
		/// Sets to end this process when computer is going to sleep or hibernate.
		/// </summary>
		/// <param name="exitCode">Process exit code. See <see cref="Environment.Exit"/>.</param>
		public static void EndOnSleep(int exitCode = 0) {
			if (s_endOnSleep) return; s_endOnSleep = true;
			AThread.Start(() => {
				//Thread.Sleep(100);
				var w = AWnd.Internal_.CreateWindowDWP(messageOnly: false, t_eocWP = (w, m, wp, lp) => {
					if (m == Api.WM_POWERBROADCAST && wp == Api.PBT_APMSUSPEND) {
						Environment.Exit(exitCode);
					}
					return Api.DefWindowProc(w, m, wp, lp);
				}); //message-only windows don't receive WM_POWERBROADCAST

				//instead of above could use SystemEvents.PowerModeChanged, but I don't trust it. Anyway need thread and message loop.

				while (Api.GetMessage(out var m) > 0) Api.DispatchMessage(m);
			}, background: true, sta: false);
		}

		static bool s_endOnSleep, s_endOnDesktopSwitch;
		[ThreadStatic] static Native.WNDPROC t_eocWP;

		//TODO: remove "end runSingle task on sleep" from editor.
		//CONSIDER: add meta runSingle in default template.
		//SHOULDDO: in meta bool properties allow to omit "true".

		/// <summary>
		/// Sets a hotkey that ends this process.
		/// </summary>
		/// <param name="hotkey">
		/// See <see cref="ARegisteredHotkey.Register"/>.
		/// Good hotkeys are Pause, madia/browser/launch keys, Insert, Up, PageUp. Don't use F12, Shift+numpad, hotkeys with Esc, Space, Tab, Sleep. Don't use keys and modifiers used in script (<b>AKeys.Key</b> etc).
		/// </param>
		/// <param name="exitCode">Process exit code. See <see cref="Environment.Exit"/>.</param>
		/// <exception cref="InvalidOperationException">Calling second time.</exception>
		/// <remarks>
		/// This isn't a reliable way to end script. Consider <see cref="EndOnDesktopSwitch"/>, <see cref="EndOnSleep"/>.
		/// - Does not work if the hotkey is registered by any process or used by Windows.
		/// - Does not work or is delayed if user input is blocked by an <see cref="AInputBlocker"/> or keyboard hook. For example <see cref="AKeys.Key"/> and similar functions block input by default.
		/// - Most single-key and Shift+key hotkeys don't work when the active window has higher UAC integrity level (eg admin) than this process. Media keys may work.
		/// - If several processes call this function with same hotkey, the hotkey ends one process at a time.
		/// </remarks>
		public static void EndHotkey(KHotkey hotkey, int exitCode = 0) {
			AThread.Start(() => {
				var (mod, key) = ARegisteredHotkey.Normalize_(hotkey);
				var atom = Api.GlobalAddAtom("Au.EndHotkey");

#if true
				while (!Api.RegisterHotKey(default, atom, mod, key)) Thread.Sleep(100); //several tasks may register same hotkey
				Api.GetMessage(out _, default, Api.WM_HOTKEY, Api.WM_HOTKEY);
				Environment.Exit(exitCode);
#else
				//workaround for: no WM_HOTKEY if the active window has higher UAC IL.
				//	Use WM_SETHOTKEY too. It works with higher IL windows.
				//	But can't use it alone. It is unreliable. Incorrectly documented, partially working, etc. Eg does not work when taskbar is active.
				//rejected. Too dirty and unreliable.
				//	And don't encourage to use a hotkey to end script, as it is unreliable. Let use EndOnDesktopSwitch or EndOnSleep.

				bool workaround = hotkey.Mod is 0 or KMod.Shift && !AUac.IsAdmin;
				var timerW = workaround ? Api.SetTimer(default, 0, 1000, null) : default; //avoid creating windows etc for short scripts

				var timerR = Api.RegisterHotKey(default, atom, mod, key) ? default : Api.SetTimer(default, 0, 100, null);

				while (Api.GetMessage(out var m) > 0) {
					if (m.message == Api.WM_HOTKEY && m.wParam == atom) {
						Environment.Exit(exitCode);
					}

					if (m.message == Api.WM_TIMER && m.wParam != default) {
						if (m.wParam == timerR) {
							//workaround for: fails to register if another task uses the same hotkey
							if (Api.RegisterHotKey(default, atom, mod, key)) Api.KillTimer(default, timerR);
						} else if (m.wParam == timerW) {
							Api.KillTimer(default, timerW);
							var w = AWnd.More.CreateMessageOnlyWindow((w, m, wp, lp) => {
								//AWnd.More.PrintMsg(w, m, wp, lp); //no WM_SYSCOMMAND
								//if (m == Api.WM_ACTIVATE && wp == 1) w.Post(Api.WM_HOTKEY, atom);
								if (m == Api.WM_ACTIVATE && wp == 1) Environment.Exit(exitCode);
								return Api.DefWindowProc(w, m, wp, lp);
							}, AWnd.Internal_.WindowClassDWP);
							w.SetStyle(WS.VISIBLE, WSFlags.Add); //or w.ShowL(true); //does not make visible because it is a message-only window, but enables wm_sethotkey
							var r = Api.DefWindowProc(w, Api.WM_SETHOTKEY, AMath.MakeWord((int)key, (int)hotkey.Mod), default); //not MakeUint
						}
					}

					Api.DispatchMessage(m);
				}
#endif
				//don't delete hotkey/atom/window, because in most cases the process ends not because of the hotkey
			}, background: true, sta: false);
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// <see cref="ATask.Role"/>.
	/// </summary>
	public enum ATRole
	{
		/// <summary>
		/// The task runs as normal .exe program.
		/// It can be started from editor or not. It can run on computers where editor not installed.
		/// </summary>
		ExeProgram,

		/// <summary>
		/// The task runs in Au.Task.exe process.
		/// It can be started only from editor.
		/// </summary>
		MiniProgram,

		/// <summary>
		/// The task runs in editor process.
		/// </summary>
		EditorExtension,
	}
}
