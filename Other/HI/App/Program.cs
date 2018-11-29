//#define ADMIN_TRIGGERS

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

	static unsafe void Main(string[] args)
	{
		//Output.LibUseQM2 = true; Output.Clear();

		//Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1; //test how works with 1 CPU

		//hide hourglass cursor. Parent process cannot do it if uses shellexecute.
		//rejected. Slower startup. Then .NET creates new thread with GDI+ hook window.
		//Wnd.Misc.PostThreadMessage(0); Api.GetMessage(out var m, default, 0, 0);

		NamedPipeServerStream pipe = null;
		//need to set security, else nonadmin client cannot connect. Now only low IL processes can't.
#if false //works too, but slightly slower when not ngened
		//var sa = Api.SECURITY_ATTRIBUTES.Common; //no security, eg allows low IL processes to write
		var sa = new Api.SECURITY_ATTRIBUTES("D:(A;;0x12019b;;;AU)"); //like of PipeSecurity that allows ReadWrite for AuthenticatedUserSid
		var pipeName = @"\\.\pipe\Au.HI-" + Process_.CurrentSessionId.ToString();
		var h = CreateNamedPipe(pipeName, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_REJECT_REMOTE_CLIENTS, 1, 0, 0, 0, sa);
		if(h == (IntPtr)(-1)) {
			if(Native.GetError() == Api.ERROR_ACCESS_DENIED) return; //a simple way to implement single-instance process
			Debug_.Print(Native.GetErrorMessage());
			return;
		}
		pipe = new NamedPipeServerStream(PipeDirection.InOut, false, false, new Microsoft.Win32.SafeHandles.SafePipeHandle(h, true));
#else
		var pipeName = "Au.HI-" + Process_.CurrentSessionId.ToString();
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
						//if(!_Security(pipe)) continue;
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
						int R = 0;
						if(len > 0) {
							R = _Action(b);
						}
						pipe.Write(BitConverter.GetBytes(R), 0, 4);
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
			EditorProcessContext.Shutdown();
		}
	}

	static int _Action(byte[] b)
	{
		//Print(s);
		try {
			var a = Au.Util.LibSerializer.Deserialize(b);
			var epc = EditorProcessContext.GetEPC((Wnd)(LPARAM)a[1]._i);
			int action = a[0]._i;
			if(action <= 100) { //task actions
				switch(action) {
				case 1:
					if(Process_.LibStartLL(out var pi, a[2], a[3])) {
						int R = pi.dwProcessId;
						pi.Dispose();
						return R;
						//CONSIDER: try to set parent process = editor. But only if LibStartUserIL sets parent=editor and not explorer.
						//	API:
						//	EXTENDED_STARTUPINFO_PRESENT, STARTUPINFOEX
						//	InitializeProcThreadAttributeList
						//	UpdateProcThreadAttribute(PROC_THREAD_ATTRIBUTE_PARENT_PROCESS).
						//	DeleteProcThreadAttributeList
					}
					return 0;
				}
#if ADMIN_TRIGGERS
			} else if(action <= 200) { //trigger actions
				var trig = epc.Triggers;
				switch(action) {
				case 101: trig.Start(a[2]); break;
				case 102: trig.Stop(); break;
				}
				return 1;
#endif
			}
		}
		catch(Exception ex) { Debug_.Print(ex); }
		return 0;
	}

	//rejected. There are other ways to use this process to UAC-elevate malware, eg write script or assembly in workspace directory, set editor text and run, inject dll in editor.
	//	CONSIDER: add option 'Secure': run editor as admin, protect workspace directory, only admin processes can open pipe.
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

class EditorProcessContext
{
	public Wnd Window { get; private set; }
#if ADMIN_TRIGGERS
	public TriggersInHi Triggers { get; private set; }
#endif
	static EditorProcessContext s_epc;

	EditorProcessContext(Wnd wEditor)
	{
		Window = wEditor;
#if ADMIN_TRIGGERS
		Triggers = new TriggersInHi(wEditor);
#endif
		var t = new Thread(_EditorWatcherThread) { IsBackground = true };
		t.Start();
	}

	public static EditorProcessContext GetEPC(Wnd wEditor)
	{
		lock(typeof(EditorProcessContext)) {
			if(s_epc == null || s_epc.Window != wEditor) {
				s_epc?._Dispose(false);
				s_epc = new EditorProcessContext(wEditor);
			}
			return s_epc;
		}
	}

	public static void Shutdown()
	{
		lock(typeof(EditorProcessContext)) s_epc?._Dispose(true);
	}

	void _Dispose(bool onExit)
	{
#if ADMIN_TRIGGERS
		Triggers.Dispose();
#endif
	}

	//Ends tasks etc when editor process exits.
	void _EditorWatcherThread()
	{
		try { Window.WaitForClosed(0, true); } //waits for process handle
		catch(Exception ex) { Debug_.Print(ex); return; }
		lock(typeof(EditorProcessContext)) {
			_Dispose(false);
			if(s_epc == this) s_epc = null;
		}
	}
}
