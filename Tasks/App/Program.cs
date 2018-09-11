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
using System.IO.Pipes;

using Au;
using Au.Types;
using static Au.NoClass;

static class Program
{
	//[DllImport("kernel32.dll", EntryPoint = "CreateNamedPipeW", SetLastError = true)]
	//internal static extern IntPtr CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, Api.SECURITY_ATTRIBUTES lpSecurityAttributes);
	//internal const uint PIPE_ACCESS_DUPLEX = 0x3;
	//internal const uint PIPE_TYPE_MESSAGE = 0x4;
	//internal const uint PIPE_READMODE_MESSAGE = 0x2;
	//internal const uint PIPE_REJECT_REMOTE_CLIENTS = 0x8;

	static bool s_isAdmin;

	static unsafe void Main(string[] args)
	{
		//Output.LibUseQM2 = true; Output.Clear();

		//hide hourglass cursor. Parent process cannot do it if uses shellexecute.
		//rejected. Slower startup. Then .NET creates new thread with GDI+ hook window.
		//Wnd.Misc.PostThreadMessage(0); Api.GetMessage(out var m, default, 0, 0);

		s_isAdmin = Process_.UacInfo.IsAdmin;

		NamedPipeServerStream pipe = null;
		//need to set security, else nonadmin client cannot connect. Now only low IL processes can't.
#if false //works too, but slightly slower when not ngened
		//var sa = Api.SECURITY_ATTRIBUTES.Common; //no security, eg allows low IL processes to write
		var sa = new Api.SECURITY_ATTRIBUTES("D:(A;;0x12019b;;;AU)"); //like of PipeSecurity that allows ReadWrite for AuthenticatedUserSid
		var pipeName = @"\\.\pipe\Au.Tasks-" + (b_isAdmin ? "H-" : "M-") + Process_.CurrentSessionId.ToString();
		var h = CreateNamedPipe(pipeName, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_REJECT_REMOTE_CLIENTS, 1, 0, 0, 0, sa);
		if(h == (IntPtr)(-1)) {
			if(Native.GetError() == Api.ERROR_ACCESS_DENIED) return; //a simple way to implement single-instance process
			Debug_.Print(Native.GetErrorMessage());
			return;
		}
		pipe = new NamedPipeServerStream(PipeDirection.InOut, false, false, new Microsoft.Win32.SafeHandles.SafePipeHandle(h, true));
#else
		var pipeName = "Au.Tasks-" + (s_isAdmin ? "H-" : "M-") + Process_.CurrentSessionId.ToString();
		try {
			var ps = new PipeSecurity();
			var sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.AuthenticatedUserSid, null);
			ps.AddAccessRule(new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
			pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, 0, 0, 0, ps);
		}
		catch(UnauthorizedAccessException) { return; } //a simple way to implement single-instance process
		catch(Exception ex) { AuDialog.ShowError("Failed to create pipe", ex.ToString()); return; }
#endif

		try {
			using(pipe) {
				var b = new byte[4000];
				for(; ; ) {
					pipe.WaitForConnection();
					try {
						//if(s_isAdmin && !_Security(pipe)) continue;
						int len = 0;
						for(; ; ) {
							int n = pipe.Read(b, len, b.Length - len);
							len += n;
							if(len < b.Length || pipe.IsMessageComplete) break;
							if(len > 1_000_000) { len = 0; break; } //1024000. Editor checks length too.
							var t = new byte[b.Length * 4];
							b.CopyTo(t, 0);
							b = t;
						}
						byte R = 0;
						if(len > 0) {
							string s;
							fixed (byte* p = b) s = new string((char*)p, 0, len / 2);
							R = _Action(s);
						}
						pipe.WriteByte(R);
					}
					catch(IOException ex) { //probably client process terminated while we worked, and the pipe is now broken
#if DEBUG
						AuDialog.ShowError("Exception", ex.ToString());
#else
						var _ = ex;
						500.ms();
#endif
					}
					finally {
						try { pipe.Disconnect(); } catch { }
					}
				}
			}
		}
		catch(Exception ex) {
			AuDialog.ShowError("Exception", ex.ToString());
		}
		finally {
			s_tasks?.Dispose(true);
			RunningTasks2.FinishOffHungTasks();
		}
	}

	static byte _Action(string s)
	{
		//Print(s);
		try {
			var c = new CsvTable(s);
			if(c.ColumnCount != 2 || c[1, 0] != "wnd") return 0;
			int taskId = c.GetInt(0, 1);
			Wnd wEditor = (Wnd)(LPARAM)c.GetInt(1, 1);

			var tasks = s_tasks;
			if(tasks == null || tasks.Window != wEditor) {
				s_tasks = tasks = new RunningTasks2(wEditor);
				var t = new Thread(_EditorWatcherThread) { IsBackground = true };
				t.Start(tasks);
			}

			switch(c[0, 0]) {
			case "run": return tasks.RunTask(taskId, c);
			case "end": return tasks.EndTask(taskId);
			}
		}
		catch(Exception ex) { Debug_.Print(ex); }
		return 0;
	}

	static RunningTasks2 s_tasks;

	//Ends tasks when editor process exits.
	static void _EditorWatcherThread(object o)
	{
		var tasks = o as RunningTasks2;
		try { tasks.Window.WaitForClosed(0, true); } catch(Exception ex) { Debug_.Print(ex); return; }
		tasks.Dispose(false);
		if(tasks == s_tasks) s_tasks = null;
	}

	//rejected. There are other ways to use this process to UAC-elevate malware, eg write script in collection, inject dll in editor, manipulate editor.
	///// <summary>
	///// Gets pipe client process id, and returns false if its program is in other folder than this program.
	///// It is to prevent unknown programs to use this process to UAC-elevate themselves.
	///// This program usually will be in Program Files, therefore they cannot copy their files to its folder if they are not admin.
	///// </summary>
	///// <param name="pipe"></param>
	//static bool _Security(NamedPipeServerStream pipe)
	//{
	//	Perf.First();
	//	if(Api.GetNamedPipeClientProcessId(pipe.SafePipeHandle.DangerousGetHandle(), out int pid)) { //quite fast
	//		Perf.Next();
	//		long t = Time.Milliseconds;
	//		if(pid == s_secPid && t - s_secTime < 3000) { s_secTime = t; return true; }
	//		var s = Process_.GetName(pid, fullPath: true); //quite slow
	//		if(s != null && s.StartsWith_(Folders.ThisAppBS, true)) { Perf.NW(); s_secPid = pid; s_secTime = t; return true; }
	//	}
	//	return false;
	//}
	//static int s_secPid;
	//static long s_secTime;
}
