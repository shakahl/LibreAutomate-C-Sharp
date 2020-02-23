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
using Au.Util;

namespace Au
{
	/// <summary>
	/// This class contains static functions to execute or open programs, other files, folders, web pages, etc.
	/// </summary>
	public static partial class AExec
	{
		/// <summary>
		/// Runs/opens a program, document, directory (folder), URL, new email, Control Panel item etc.
		/// The returned <see cref="RResult"/> variable contains some process info - process id etc.
		/// </summary>
		/// <param name="file">
		/// What to run. Can be:
		/// Full path of a file or directory. Examples: <c>@"C:\file.txt"</c>, <c>AFolders.System + "notepad.exe"</c>, <c>@"%AFolders.System%\notepad.exe"</c>.
		/// Filename of a file or directory, like <c>"notepad.exe"</c>. The function calls <see cref="AFile.SearchPath"/>.
		/// Path relative to <see cref="AFolders.ThisApp"/>. Examples: <c>"x.exe"</c>, <c>@"subfolder\x.exe"</c>, <c>@".\subfolder\x.exe"</c>, <c>@"..\another folder\x.exe"</c>.
		/// URL. Examples: <c>"http://a.b.c/d"</c>, <c>"file:///path"</c>.
		/// Email, like <c>"mailto:a@b.c"</c>. Subject, body etc also can be specified, and Google knows how.
		/// Shell object's ITEMIDLIST like <c>":: ITEMIDLIST"</c>. See <see cref="APidl.ToBase64String"/>, <see cref="AFolders.Virtual"/>. Can be used to open virtual folders and items like Control Panel.
		/// Shell object's parsing name, like <c>@"::{CLSID}"</c>. See <see cref="APidl.ToShellString"/>, <see cref="AFolders.VirtualPidl"/>. Can be used to open virtual folders and items like Control Panel.
		/// To run a Windows Store App, use <c>@"shell:AppsFolder\WinStoreAppId"</c> format. Examples: <c>@"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"</c>, <c>@"shell:AppsFolder\windows.immersivecontrolpanel_cw5n1h2txyewy!microsoft.windows.immersivecontrolpanel"</c>. To discover the string use <see cref="AWnd.More.GetWindowsStoreAppId"/> or Google.
		/// Supports environment variables, like <c>@"%TMP%\file.txt"</c>. See <see cref="APath.ExpandEnvVar"/>.
		/// </param>
		/// <param name="args">
		/// Command line arguments.
		/// This function expands environment variables if starts with <c>"%"</c> or <c>"\"%"</c>.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="curDirEtc">
		/// Allows to specify more parameters: current directory, verb, window state, etc.
		/// If string, it sets initial current directory for the new process. Use "" to get it from <i>file</i>. More info: <see cref="ROptions.CurrentDirectory"/>.
		/// </param>
		/// <exception cref="ArgumentException">Used both ROptions.Verb and RFlags.Admin.</exception>
		/// <exception cref="AuException">Failed. For example, the file does not exist.</exception>
		/// <remarks>
		/// It works like when you double-click a file icon. It may start new process or not. For example it may just activate window if the program is already running.
		/// Uses API <msdn>ShellExecuteEx</msdn>.
		/// Similar to <see cref="Process.Start(string, string)"/>.
		/// </remarks>
		/// <seealso cref="AWnd.FindOrRun"/>
		/// <example>
		/// Run notepad and wait for its window.
		/// <code><![CDATA[
		/// AExec.Run("notepad.exe");
		/// AWnd w = AWnd.Wait(10, true, "*- Notepad", "Notepad");
		/// ]]></code>
		/// Run notepad or activate its window.
		/// <code><![CDATA[
		/// AWnd w = AWnd.FindOrRun("*- Notepad", run: () => AExec.Run("notepad.exe"));
		/// ]]></code>
		/// </example>
		public static RResult Run(string file, string args = null, RFlags flags = 0, ROptions curDirEtc = null)
		{
			//SHOULDDO: from UAC IL admin run as user by default. Add flag UacInherit.

			Api.SHELLEXECUTEINFO x = default;
			x.cbSize = Api.SizeOf(x);
			x.fMask = Api.SEE_MASK_NOZONECHECKS | Api.SEE_MASK_NOASYNC | Api.SEE_MASK_NOCLOSEPROCESS | Api.SEE_MASK_CONNECTNETDRV | Api.SEE_MASK_UNICODE;
			x.nShow = Api.SW_SHOWNORMAL;

			bool curDirFromFile = false;
			var more = curDirEtc;
			if(more != null) {
				x.lpVerb = more.Verb;
				var cd = more.CurrentDirectory; if(cd != null) { if(cd.Length == 0) curDirFromFile = true; else cd = APath.ExpandEnvVar(cd); }
				x.lpDirectory = cd;
				if(!more.OwnerWindow.IsEmpty) x.hwnd = more.OwnerWindow.Wnd.Window;
				switch(more.WindowState) {
				case ProcessWindowStyle.Hidden: x.nShow = Api.SW_HIDE; break;
				case ProcessWindowStyle.Minimized: x.nShow = Api.SW_SHOWMINIMIZED; break;
				case ProcessWindowStyle.Maximized: x.nShow = Api.SW_SHOWMAXIMIZED; break;
				}
			}

			if(flags.Has(RFlags.Admin)) {
				if(more?.Verb != null && !more.Verb.Eqi("runas")) throw new ArgumentException("Cannot use Verb with flag Admin");
				x.lpVerb = "runas";
			} else if(x.lpVerb != null) x.fMask |= Api.SEE_MASK_INVOKEIDLIST; //makes slower. But verbs are rarely used.

			if(0 == (flags & RFlags.ShowErrorUI)) x.fMask |= Api.SEE_MASK_FLAG_NO_UI;
			if(0 == (flags & RFlags.WaitForExit)) x.fMask |= Api.SEE_MASK_NO_CONSOLE;

			file = _NormalizeFile(false, file, out bool isFullPath, out bool isShellPath);
			APidl pidl = null;
			if(isShellPath) { //":: Base64ITEMIDLIST" or "::{CLSID}..." (we convert it too because the API does not support many)
				pidl = APidl.FromString(file); //does not throw
				if(pidl != null) {
					x.lpIDList = pidl.UnsafePtr;
					x.fMask |= Api.SEE_MASK_INVOKEIDLIST;
				} else x.lpFile = file;
			} else {
				x.lpFile = file;

				if(curDirFromFile && isFullPath) x.lpDirectory = APath.GetDirectoryPath(file);
			}
			if(!Empty(args)) x.lpParameters = APath.ExpandEnvVar(args);

			AWnd.More.EnableActivate();

			bool ok = false;
			try {
				ok = Api.ShellExecuteEx(ref x);
			}
			finally {
				pidl?.Dispose();
			}
			if(!ok) throw new AuException(0, $"*run '{file}'");

			var R = new RResult();
			bool waitForExit = 0 != (flags & RFlags.WaitForExit);
			bool needHandle = flags.Has(RFlags.NeedProcessHandle);
			WaitHandle_ ph = null;
			if(!x.hProcess.Is0) {
				if(waitForExit || needHandle) ph = new WaitHandle_(x.hProcess, true);
				if(!waitForExit) R.ProcessId = AProcess.ProcessIdFromHandle(x.hProcess);
			}

			try {
				Api.AllowSetForegroundWindow(Api.ASFW_ANY);

				if(x.lpVerb != null && Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
					Thread.CurrentThread.Join(50); //need min 5-10 for file Properties. And not Sleep.

				if(ph != null) {
					if(waitForExit) {
						ph.WaitOne();
						if(Api.GetExitCodeProcess(x.hProcess, out var ec)) R.ProcessExitCode = ec;
					}
					if(needHandle) R.ProcessHandle = ph;
				}
			}
			finally {
				if(R.ProcessHandle == null) {
					if(ph != null) ph.Dispose();
					else x.hProcess.Dispose();
				}
			}

			return R;

			//tested: works well in MTA thread.
			//rejected: in QM2, run also has a 'window' parameter. However it just makes limited, unclear etc, and therefore rarely used. Instead use AWnd.FindOrRun or Find/Run/Wait like in the examples.
			//rejected: in QM2, run also has 'autodelay'. Better don't add such hidden things. Let the script decide what to do.
		}

		/// <summary>
		/// Calls <see cref="Run"/> and handles exceptions. All parameters are the same.
		/// If <b>Run</b> throws exception, prints it as warning and returns null.
		/// </summary>
		/// <remarks>
		/// This is useful when you don't care whether <b>Run</b> succeeded and don't want to use try/catch.
		/// Handles only exception of type AuException. It is thrown when fails, usually when the file does not exist.
		/// </remarks>
		/// <seealso cref="PrintWarning"/>
		/// <seealso cref="OptDebug.DisableWarnings"/>
		/// <seealso cref="AWnd.FindOrRun"/>
		[MethodImpl(MethodImplOptions.NoInlining)] //uses stack
		public static RResult TryRun(string s, string args = null, RFlags flags = 0, ROptions curDirEtc = null)
		{
			try {
				return Run(s, args, flags, curDirEtc);
			}
			catch(AuException e) {
				PrintWarning(e.Message, 1);
				return null;
			}
		}

		//Used by Run and RunConsole.
		static string _NormalizeFile(bool runConsole, string file, out bool isFullPath, out bool isShellPath)
		{
			isShellPath = isFullPath = false;
			file = APath.ExpandEnvVar(file);
			if(Empty(file)) throw new ArgumentException();
			if(runConsole || !(isShellPath = APath.IsShellPath_(file))) {
				if(isFullPath = APath.IsFullPath(file)) {
					var fl = runConsole ? PNFlags.DontExpandDosPath : PNFlags.DontExpandDosPath | PNFlags.DontPrefixLongPath;
					file = APath.Normalize_(file, fl, true);

					//ShellExecuteEx supports long path prefix for exe but not for documents.
					//Process.Run supports long path prefix, except when the exe is .NET.
					if(!runConsole) file = APath.UnprefixLongPath(file);

					if(ADisableFsRedirection.IsSystem64PathIn32BitProcess(file) && !AFile.ExistsAsAny(file)) {
						file = ADisableFsRedirection.GetNonRedirectedSystemPath(file);
					}
				} else if(!APath.IsUrl(file)) {
					//ShellExecuteEx searches everywhere except in app folder.
					//Process.Run prefers current directory.
					var s2 = AFile.SearchPath(file);
					if(s2 != null) {
						file = s2;
						isFullPath = true;
					}
				}
			}
			return file;
		}

		/// <summary>
		/// Runs a console program, waits until its process ends, and gets its output text.
		/// This overload prints text lines in real time.
		/// </summary>
		/// <param name="exe">
		/// Path or name of an .exe or .bat file. Can be:
		/// Full path. Examples: <c>@"C:\folder\x.exe"</c>, <c>AFolders.System + "x.exe"</c>, <c>@"%AFolders.System%\x.exe"</c>.
		/// Filename, like <c>"x.exe"</c>. This function calls <see cref="AFile.SearchPath"/>.
		/// Path relative to <see cref="AFolders.ThisApp"/>. Examples: <c>"x.exe"</c>, <c>@"subfolder\x.exe"</c>, <c>@".\subfolder\x.exe"</c>, <c>@"..\another folder\x.exe"</c>.
		/// Supports environment variables, like <c>@"%TMP%\x.bat"</c>. See <see cref="APath.ExpandEnvVar"/>.
		/// </param>
		/// <param name="args">null or command line arguments.</param>
		/// <param name="curDir">
		/// Initial current directory of the new process.
		/// - If null, uses <c>Directory.GetCurrentDirectory()</c>.
		/// - Else if "", calls <c>APath.GetDirectoryPath(exe)</c>.
		/// - Else calls <see cref="APath.ExpandEnvVar"/>.
		/// </param>
		/// <param name="encoding">
		/// Console's text encoding.
		/// If null (default), uses the default console text encoding (API <msdn>GetOEMCP</msdn>); it is not Unicode. Programs that display Unicode text use <see cref="Encoding.UTF8"/>.
		/// </param>
		/// <returns>The process exit code. Usually a non-0 value means error.</returns>
		/// <exception cref="AuException">Failed, for example file not found.</exception>
		/// <remarks>
		/// The console window is hidden. The text that would be displayed in it is redirected to this function.
		/// 
		/// Console programs have two output text streams - standard output and standard error. This function gets both.
		/// Alternatively use <see cref="Process.Start"/>. It gets the output and error streams separately, and some lines may be received in incorrect order in time.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// string v = "example";
		/// int r1 = AExec.RunConsole(@"Q:\Test\console1.exe", $@"/an ""{v}"" /etc");
		/// 
		/// int r2 = AExec.RunConsole(s => Print(s), @"Q:\Test\console2.exe");
		/// 
		/// int r3 = AExec.RunConsole(out var text, @"Q:\Test\console3.exe", encoding: Encoding.UTF8);
		/// Print(text);
		/// ]]></code>
		/// </example>
		public static unsafe int RunConsole(string exe, string args = null, string curDir = null, Encoding encoding = null)
		{
			return _RunConsole(s => Print(s), null, exe, args, curDir, encoding);
		}

		/// <summary>
		/// Runs a console program, waits until its process ends, and gets its output text.
		/// This overload uses a callback function that receives text lines in real time.
		/// </summary>
		/// <param name="output">A callback function that receives the output text. It receives single full line at a time, without line break characters.</param>
		/// <param name="exe"></param>
		/// <param name="args"></param>
		/// <param name="curDir"></param>
		/// <param name="encoding"></param>
		/// <exception cref="AuException">Failed, for example file not found.</exception>
		public static unsafe int RunConsole(Action<string> output, string exe, string args = null, string curDir = null, Encoding encoding = null)
		{
			return _RunConsole(output, null, exe, args, curDir, encoding);
		}

		/// <summary>
		/// Runs a console program, waits until its process ends, and gets its output text when it ends.
		/// </summary>
		/// <param name="output">A variable that receives the output text.</param>
		/// <param name="exe"></param>
		/// <param name="args"></param>
		/// <param name="curDir"></param>
		/// <param name="encoding"></param>
		/// <exception cref="AuException">Failed, for example file not found.</exception>
		public static unsafe int RunConsole(out string output, string exe, string args = null, string curDir = null, Encoding encoding = null)
		{
			var b = new StringBuilder();
			var r = _RunConsole(null, b, exe, args, curDir, encoding);
			output = b.ToString();
			return r;
		}

		static unsafe int _RunConsole(Action<string> outAction, StringBuilder outStr, string exe, string args, string curDir, Encoding encoding)
		{
			exe = _NormalizeFile(true, exe, out _, out _);
			//args = APath.ExpandEnvVar(args); //rejected

			var ps = new ProcessStarter_(exe, args, curDir, rawExe: true);

			Handle_ hProcess = default;
			var sa = new Api.SECURITY_ATTRIBUTES(null) { bInheritHandle = 1 };
			if(!Api.CreatePipe(out Handle_ hOutRead, out Handle_ hOutWrite, sa, 0)) throw new AuException(0);

			byte* b = null; char* c = null;
			try {
				Api.SetHandleInformation(hOutRead, 1, 0); //remove HANDLE_FLAG_INHERIT

				ps.si.dwFlags |= Api.STARTF_USESTDHANDLES | Api.STARTF_USESHOWWINDOW;
				ps.si.hStdOutput = hOutWrite;
				ps.si.hStdError = hOutWrite;
				ps.flags |= Api.CREATE_NEW_CONSOLE;

				if(!ps.StartLL(out var pi, inheritHandles: true)) throw new AuException(0);
				hOutWrite.Dispose(); //important: must be here
				pi.hThread.Dispose();
				hProcess = pi.hProcess;

				//variables for 'prevent getting partial lines'
				bool needLines = outStr == null /*&& !flags.Has(RCFlags.RawText)*/;
				int offs = 0; bool skipN = false;

				int bSize = 8000;
				b = (byte*)AMemory.Alloc(bSize);

				for(bool ended = false; !ended;) {
					if(bSize - offs < 1000) { //part of 'prevent getting partial lines' code
						b = (byte*)AMemory.ReAlloc(b, bSize *= 2);
						AMemory.Free(c); c = null;
					}

					if(Api.ReadFile(hOutRead, b + offs, bSize - offs, out int nr)) {
						if(nr == 0) continue;
						nr += offs;
					} else {
						if(ALastError.Code != Api.ERROR_BROKEN_PIPE) throw new AuException(0);
						//process ended
						if(offs == 0) break;
						nr = offs;
						offs = 0;
						ended = true;
					}

					//prevent getting partial lines. They can be created by the console program, or by the above code when buffer too small.
					int moveFrom = 0;
					if(needLines) {
						if(skipN) { //if was split between \r and \n, remove \n now
							skipN = false;
							if(b[0] == '\n') Api.memmove(b, b + 1, --nr);
							if(nr == 0) continue;
						}
						int i;
						for(i = nr; i > 0; i--) { var k = b[i - 1]; if(k == '\n' || k == '\r') break; }
						if(i == nr) { //ends with \n or \r
							offs = 0;
							if(b[--nr] == '\r') skipN = true;
							else if(nr > 0 && b[nr - 1] == '\r') nr--;
						} else if(i > 0) { //contains \n or \r
							moveFrom = i;
							offs = nr - i;
							if(b[--i] == '\n' && i > 0 && b[i - 1] == '\r') i--;
							nr = i;
						} else if(!ended) {
							offs = nr;
							continue;
						}
					}

					if(c == null) c = (char*)AMemory.Alloc(bSize * 2);
					if(encoding == null) {
						if((encoding = s_oemEncoding) == null) {
							Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
							var oemCP = Api.GetOEMCP();
							try { encoding = Encoding.GetEncoding(oemCP); }
							catch { encoding = Encoding.GetEncoding(437); }
							s_oemEncoding = encoding;
						}
					}
					int nc = encoding.GetChars(b, nr, c, bSize);

					if(moveFrom > 0) Api.memmove(b, b + moveFrom, offs); //part of 'prevent getting partial lines' code

					var s = new string(c, 0, nc);
					if(needLines) {
						if(s.FindAny("\r\n") < 0) outAction(s);
						else foreach(var k in s.Segments(SegSep.Line)) outAction(s[k.start..k.end]);
					} else {
						outStr.Append(s);
					}
				}

				if(!Api.GetExitCodeProcess(hProcess, out int ec)) ec = int.MinValue;
				return ec;
			}
			finally {
				hProcess.Dispose();
				hOutRead.Dispose();
				hOutWrite.Dispose();
				AMemory.Free(b);
				AMemory.Free(c);
			}
		}

		static Encoding s_oemEncoding;

		/// <summary>
		/// Opens parent folder in Explorer and selects the specified file.
		/// Returns null if fails, for example if the file does not exist.
		/// </summary>
		/// <param name="path">
		/// Full path of a file or directory or other shell object.
		/// Supports <c>@"%environmentVariable%\..."</c> (see <see cref="APath.ExpandEnvVar"/>) and <c>"::..."</c> (see <see cref="APidl.ToBase64String"/>).
		/// </param>
		public static bool SelectInExplorer(string path)
		{
			using var pidl = APidl.FromString(path);
			if(pidl == null) return false;
			return 0 == Api.SHOpenFolderAndSelectItems(pidl.HandleRef, 0, null, 0);
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AExec.Run"/>.
	/// </summary>
	[Flags]
	public enum RFlags
	{
		/// <summary>
		/// Show error message box if fails, for example if file not found.
		/// Note: this does not disable exceptions. Still need exception handling. Or call <see cref="AExec.TryRun"/>.
		/// </summary>
		ShowErrorUI = 1,

		/// <summary>
		/// If started new process, wait until it exits.
		/// </summary>
		WaitForExit = 2,

		/// <summary>
		/// Get process handle (<see cref="RResult.ProcessHandle"/>), if possible.
		/// </summary>
		NeedProcessHandle = 4,

		/// <summary>
		/// Run as administrator, probably with UAC consent dialog.
		/// Uses verb "runas", therefore other verb cannot be specified.
		/// </summary>
		Admin = 8,
	}

	/// <summary>
	/// More parameters for <see cref="AExec.Run"/>.
	/// </summary>
	/// <remarks>
	/// Implicit conversion from <b>string</b> sets <see cref="CurrentDirectory"/>.
	/// </remarks>
	public class ROptions
	{
		/// <summary>
		/// Sets <see cref="CurrentDirectory"/>.
		/// </summary>
		public static implicit operator ROptions(string curDir) => new ROptions { CurrentDirectory = curDir };

		/// <summary>
		/// Initial current directory for the new process.
		/// If null (default), the new process will inherit the curent directory of this process.
		/// If "", the function gets parent directory path from the <i>file</i> parameter, if possible (if full path is specified or found). If not possible, same as null.
		/// <note>Some programs look for their files in current directory and fail to start if it is not the program's directory.</note>
		/// </summary>
		public string CurrentDirectory;

		/// <summary>
		/// File's right-click menu command, also known as verb. For example "edit", "print", "properties". The default verb is bold in the menu.
		/// Not all menu items will work. Some may have different name than in the menu.
		/// </summary>
		public string Verb;

		/// <summary>
		/// Owner window for error message boxes.
		/// Also, new window should be opened on the same screen. However many programs ignore it.
		/// </summary>
		public AnyWnd OwnerWindow;

		/// <summary>
		/// Preferred window state.
		/// Many programs ignore it.
		/// </summary>
		public ProcessWindowStyle WindowState;

		//no. If need, caller can get window and call EnsureInScreen etc.
		//public AScreen Screen;
		//this either does not work or I could not find a program that uses default window position (does not save/restore)
		//if(!more.Screen.IsNull) { x._14.hMonitor = more.Screen.ToDevice().Handle; x.fMask |= Api.SEE_MASK_HMONITOR; }
	}

	/// <summary>
	/// Results of <see cref="AExec.Run"/>.
	/// </summary>
	public class RResult
	{
		/// <summary>
		/// The exit code of the process.
		/// 0 if no flag <b>WaitForExit</b> or if cannot wait.
		/// </summary>
		/// <remarks>
		/// Usually the exit code is 0 or a process-defined error code.
		/// </remarks>
		public int ProcessExitCode { get; internal set; }

		/// <summary>
		/// The process id.
		/// 0 if used flag <b>WaitForExit</b> or if did not start new process (eg opened the document in an existing process) or if cannot get it.
		/// </summary>
		public int ProcessId { get; internal set; }

		/// <summary>
		/// If used flag <b>NeedProcessHandle</b>, contains process handle. Later the <see cref="WaitHandle"/> variable must be disposed.
		/// null if no flag or if did not start new process (eg opened the document in an existing process) or if cannot get it.
		/// </summary>
		/// <example>
		/// This code does the same as <c>AExec.Run(@"notepad.exe", flags: SRFlags.WaitForExit);</c>
		/// <code><![CDATA[
		/// var r = AExec.Run(@"notepad.exe", flags: SRFlags.NeedProcessHandle);
		/// using(var h = r.ProcessHandle) h?.WaitOne();
		/// ]]></code>
		/// </example>
		public WaitHandle ProcessHandle { get; internal set; }

		/// <summary>
		/// Returns <see cref="ProcessId"/> as string.
		/// </summary>
		public override string ToString()
		{
			return ProcessId.ToString();
		}
	}

	///// <summary>
	///// Flags for <see cref="AExec.RunConsole"/>.
	///// </summary>
	//[Flags]
	//public enum RCFlags
	//{
	//	/// <summary>
	//	/// Let the <i>output</i> callback function receive raw text; it can be one or more lines of text with line break characters. If false (default), it is single line without line break characters.
	//	/// </summary>
	//	RawText = 1,
	//}
}
