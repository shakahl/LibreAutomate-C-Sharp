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
//using System.Linq;

using Au.Types;
using static Au.AStatic;

//CONSIDER: add an option to inject and execute the task in any process/thread.
//	[assembly: Inject("firefox.exe", windowName="* - Firefox")]

namespace Au
{
	/// <summary>
	/// Automation task - a running script.
	/// </summary>
	public static class ATask
	{
		/// <summary>
		/// In an automation task process of a script with role miniProgram (defaut) returns script file name without extension.
		/// In other processes returns <see cref="AppDomain.FriendlyName"/>, like "ProgramFile.exe".
		/// </summary>
		public static string Name => s_name ??= AppDomain.CurrentDomain.FriendlyName;
		static string s_name;

		/// <summary>
		/// In an automation task process tells whether the task runs in host process (default), editor process or own .exe process. It matches meta role.
		/// In other processes always returns <b>ExeProgram</b>.
		/// </summary>
		public static unsafe ATRole Role { get; private set; }

		internal static unsafe void LibInit(ATRole role, string name = null)
		{
			Role = role;
			if(name != null) s_name = name;
		}

		/// <summary>
		/// Starts an automation task. Does not wait.
		/// </summary>
		/// <returns>
		/// Native process id of the task process.
		/// Returns 0 if task start is deferred because a script is running (see meta option ifRunning).
		/// Returns 0 if role editorExtension; then waits until the task ends.
		/// </returns>
		/// <param name="script">Script name like "Script5.cs", or path like @"\Folder\Script5.cs".</param>
		/// <param name="args">Command line arguments. In script it will be variable <i>args</i>. Should not contain '\0' characters.</param>
		/// <exception cref="FileNotFoundException">Script file not found.</exception>
		/// <exception cref="AuException">Failed to start script.</exception>
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

		static int _Run(int mode, string script, string[] args, out string resultS, Action<string> resultA = null)
		{
			var w = WndMsg; if(w.Is0) throw new AuException("Au editor not found."); //CONSIDER: run editor program, if installed
			bool needResult = 0 != (mode & 2); resultS = null;
			using var tr = new _TaskResults();
			if(needResult && !tr.Init()) throw new AuException("*get task results");

			var data = Util.LibSerializer.Serialize(script, args, tr.pipeName);
			int pid = (int)AWnd.More.CopyDataStruct.SendBytes(w, 100, data, mode);
			switch((ERunResult)pid) {
			case ERunResult.failed: throw new AuException("*start task");
			case ERunResult.notFound: throw new FileNotFoundException($"Script '{script}' not found.");
			case ERunResult.editorThread: case ERunResult.deferred: return 0;
			}

			if(0 != (mode & 1)) {
				using var hProcess = LibWaitHandle.FromProcessId(pid, Api.SYNCHRONIZE | Api.PROCESS_QUERY_LIMITED_INFORMATION);
				if(hProcess == null) throw new AuException("*wait for task");

				if(!needResult) hProcess.WaitOne(-1);
				else if(!tr.WaitAndRead(hProcess, resultA)) throw new AuException("*get task result");
				else if(resultA == null) resultS = tr.ResultString;

				if(!Api.GetExitCodeProcess(hProcess.SafeWaitHandle.DangerousGetHandle(), out pid)) pid = int.MinValue;
			}
			return pid;
		}

		internal enum ERunResult //note: sync with ERunResult in Au.CL.cpp.
		{
			failed = 0,
			deferred = -1,
			notFound = -2,
			editorThread = -3,
		}

		unsafe struct _TaskResults : IDisposable
		{
			LibHandle _hPipe;
			public string pipeName;
			string _s;
			StringBuilder _sb;

			public bool Init()
			{
				var tid = AThread.NativeId;
				pipeName = @"\\.\pipe\Au.CL-" + tid.ToString();
				_hPipe = Api.CreateNamedPipe(pipeName,
					Api.PIPE_ACCESS_INBOUND | Api.FILE_FLAG_OVERLAPPED, //use async pipe because also need to wait for task process exit
					Api.PIPE_TYPE_MESSAGE | Api.PIPE_READMODE_MESSAGE | Api.PIPE_REJECT_REMOTE_CLIENTS,
					1, 0, 0, 0, Api.SECURITY_ATTRIBUTES.ForPipes);
				return !_hPipe.Is0;
			}

			public bool WaitAndRead(WaitHandle hProcess, Action<string> results)
			{
				bool R = false;
				char* b = null; const int bLen = 7900;
				var ev = new ManualResetEvent(false);
				try {
					var ha = new WaitHandle[2] { ev, hProcess };
					for(bool useSB = false; ; useSB = results == null) {
						var o = new Api.OVERLAPPED { hEvent = ev.SafeWaitHandle.DangerousGetHandle() };
						if(!Api.ConnectNamedPipe(_hPipe, &o)) {
							int e = ALastError.Code; if(e != Api.ERROR_IO_PENDING) break;
							int wr = WaitHandle.WaitAny(ha);
							if(wr != 0) { Api.CancelIo(_hPipe); R = true; break; } //task ended
							if(!Api.GetOverlappedResult(_hPipe, ref o, out _, false)) { Api.DisconnectNamedPipe(_hPipe); break; }
						}

						if(b == null) b = (char*)Util.AMemory.Alloc(bLen);
						bool readOK;
						while(((readOK = Api.ReadFile(_hPipe, b, bLen, out int n, null)) || (ALastError.Code == Api.ERROR_MORE_DATA)) && n > 0) {
							n /= 2;
							if(!readOK) useSB = true;
							//Print(useSB, n);
							if(useSB) { //rare
								if(_sb == null) _sb = new StringBuilder(bLen);
								if(results == null && _s != null) _sb.Append(_s);
								_s = null;
								_sb.Append(b, n);
							} else {
								_s = new string(b, 0, n);
							}
							if(readOK) {
								if(results != null) {
									results(ResultString);
									_sb?.Clear();
								}
								break;
							}
							//note: MSDN says must use OVERLAPPED with ReadFile too, but works without it.
						}
						Api.DisconnectNamedPipe(_hPipe);
						if(!readOK) break;
					}
				}
				finally {
					ev.Dispose();
					Util.AMemory.Free(b);
				}
				return R;
			}

			public string ResultString => _s ?? _sb?.ToString();

			public void Dispose() => _hPipe.Dispose();
		};

		/// <summary>
		/// Finds editor's message-only window used with WM_COPYDATA etc.
		/// </summary>
		internal static AWnd WndMsg {
			get {
				if(!s_wndMsg.IsAlive) { //TODO: timeout
					s_wndMsg = AWnd.FindFast(null, "Au.Editor.Msg", true);
				}
				return s_wndMsg;
			}
		}
		static AWnd s_wndMsg;

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
		public static unsafe bool WriteResult(string s)
		{
			var pipeName = Environment.GetEnvironmentVariable("ATask.WriteResult.pipe");
			if(pipeName == null) return false;
			if(!Empty(s)) {
				if(!Api.WaitNamedPipe(pipeName, 3000)) goto ge;
				using(var pipe = Api.CreateFile(pipeName, Api.GENERIC_WRITE, 0, default, Api.OPEN_EXISTING, 0)) {
					if(pipe.Is0) goto ge;
					fixed (char* p = s) if(!Api.WriteFile(pipe, p, s.Length * 2, out _)) goto ge;
				}
			}
			return true;
			ge:
			ADebug.LibPrintNativeError();
			return false;
		}

		///// <summary>
		///// Calls the first non-static method of the derived class.
		///// The method must have 0 parameters.
		///// </summary>
		//public void CallFirstMethod()
		//{
		//	//Print(this.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Length);
		//	//foreach(var m in this.GetType().GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly)) {
		//	//	Print(m.Name);
		//	//}

		//	var a = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		//	if(a.Length == 0) { Print("Info: Script code should be in a non-static method. Example:\nvoid Script() { Print(\"test\"); }"); return; }

		//	_CallMethod(a[0], null);

		//	//From MSDN: The GetMethods method does not return methods in a particular order, such as alphabetical or declaration order. Your code must not depend on the order in which methods are returned, because that order varies.
		//	//But in my experience it returns methods in declaration order. At least when che class is in single file.
		//}

		///// <summary>
		///// Calls a non-static method of the derived class by name.
		///// </summary>
		///// <param name="name">Method name. The method must have 0 or 1 parameter.</param>
		///// <param name="eventData">An argument.</param>
		//public void CallTriggerMethod(string name, object eventData)
		//{
		//	var m = GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		//	//Print(m);
		//	if(m == null) { Print($"Error: Method {name} not found."); return; }
		//	_CallMethod(m, eventData);
		//	//object[] a=null; if(parameter!=null) a=new object[1] { parameter };
		//	//m.Invoke(this, a);
		//}

		//void _CallMethod(MethodInfo m, object arg)
		//{
		//	int n = m.GetParameters().Length;
		//	if(n != 0 && n != 1) { Print($"Error: Method {m.Name} must have 0 or 1 parameter."); return; }
		//	try {
		//		m.Invoke(this, n == 0 ? null : new object[1] { arg });
		//	}
		//	catch(Exception e) {
		//		Print($"Error: Failed to call method {m.Name}. {e.Message}");
		//	}
		//}

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
