using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Xml.Linq;
//using System.Xml.XPath;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Windows shell functions.
	/// Windows shell manages files, folders (directories), shortcuts and virtual objects such as Control Panel.
	/// </summary>
	public static partial class Shell
	{
		/// <summary>
		/// The same as <see cref="Path_.Normalize"/>(CanBeUrlOrShell|DoNotPrefixLongPath), but ignores non-full path (returns s).
		/// </summary>
		/// <param name="s">File-system path or URL or "::...".</param>
		static string _Normalize(string s)
		{
			s = Path_.ExpandEnvVar(s);
			if(!Path_.IsFullPath(s)) return s; //note: not EEV. Need to expand to ":: " etc, and EEV would not do it.
			return Path_.LibNormalize(s, PNFlags.DoNotPrefixLongPath, true);
		}

		/// <summary>
		/// Runs/opens a program, document, folder, URL, new email, Control Panel item etc.
		/// By default returns process id. If used flag WaitForExit, returns process exit code.
		/// Returns 0 if did not start new process or did not get process handle, usually because opened the document in an existing process.
		/// </summary>
		/// <param name="file">
		/// What to run. Can be:
		/// Full path of a file or folder. Examples: <c>@"C:\folder\file.txt"</c>, <c>Folders.System + "notepad.exe"</c>, <c>@"%Folders.System%\notepad.exe"</c>.
		/// Filename of a file or folder, like <c>"notepad.exe"</c>. The function calls <see cref="Files.SearchPath"/>.
		/// Path relative to <see cref="Folders.ThisApp"/>. Examples: <c>"x.exe"</c>, <c>@"subfolder\x.exe"</c>, <c>@".\subfolder\x.exe"</c>, <c>@"..\another folder\x.exe"</c>.
		/// URL. Examples: <c>"http://a.b.c/d"</c>, <c>"file:///path"</c>.
		/// Email, like <c>"mailto:a@b.c"</c>. Subject, body etc also can be specified, and Google knows how.
		/// Shell object's ITEMIDLIST like <c>":: HexEncodedITEMIDLIST"</c>. See <see cref="Pidl.ToHexString"/>, <see cref="Folders.Virtual"/>. Can be used to open virtual folders and items like Control Panel.
		/// Shell object's parsing name, like <c>@"::{CLSID}"</c>. See <see cref="Pidl.ToShellString"/>, <see cref="Folders.VirtualPidl"/>. Can be used to open virtual folders and items like Control Panel.
		/// To run a Windows Store App, use <c>@"shell:AppsFolder\WinStoreAppId"</c> format. Examples: <c>@"shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"</c>, <c>@"shell:AppsFolder\windows.immersivecontrolpanel_cw5n1h2txyewy!microsoft.windows.immersivecontrolpanel"</c>. To discover the string use <see cref="Wnd.Misc.GetWindowsStoreAppId"/> or Google.
		/// Supports environment variables, like <c>@"%TMP%\file.txt"</c>. See <see cref="Path_.ExpandEnvVar"/>.
		/// </param>
		/// <param name="args">
		/// Command line arguments.
		/// This function expands environment variables if starts with "%" or "\"%".
		/// </param>
		/// <param name="flags"></param>
		/// <param name="more">Allows to specify more parameters.</param>
		/// <exception cref="AuException">Failed. For example, the file does not exist.</exception>
		/// <remarks>
		/// It works like when you double-click an icon. It may start new process or not. For example it may just activate window if the program is already running.
		/// Uses API <msdn>ShellExecuteEx</msdn>.
		/// Similar to <see cref="Process.Start(string, string)"/>.
		/// The returned process id can be used to find a window of the new process. Example:
		/// <code><![CDATA[
		/// Wnd w = WaitFor.WindowActive(10, "*- Notepad", "Notepad", Shell.Run("notepad.exe"));
		/// ]]></code>
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("*- Notepad", "Notepad");
		/// if(w.Is0) { Shell.Run("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind); }
		/// w.Activate();
		/// ]]></code>
		/// </example>
		/// <seealso cref="Wnd.FindOrRun"/>
		public static int Run(string file, string args = null, SRFlags flags = 0, SRMoreParams more = null)
		{
			//TODO: return RunResult object that contains process id or exit code.
			//	It also contains other info that helps Wnd.Find and WaitFor.WindowActive etc to find correct window.
			//	It also could contain unclosed process handle, if a flag is set.
			//	Pass it to Wnd.Find as programEtc.

			var x = new Api.SHELLEXECUTEINFO();
			x.cbSize = Api.SizeOf(x);
			x.fMask = Api.SEE_MASK_NOZONECHECKS | Api.SEE_MASK_NOASYNC | Api.SEE_MASK_NOCLOSEPROCESS | Api.SEE_MASK_CONNECTNETDRV | Api.SEE_MASK_UNICODE;
			x.nShow = Api.SW_SHOWNORMAL;
			if(more != null) {
				more.ProcessHandle = null;
				x.lpVerb = more.Verb;
				x.lpDirectory = Path_.ExpandEnvVar(more.CurrentDirectory);
				if(!more.OwnerWindow.IsNull) x.hwnd = more.OwnerWindow.Wnd.WndWindow;
				switch(more.WindowState) {
				case ProcessWindowStyle.Hidden: x.nShow = Api.SW_HIDE; break;
				case ProcessWindowStyle.Minimized: x.nShow = Api.SW_SHOWMINIMIZED; break;
				case ProcessWindowStyle.Maximized: x.nShow = Api.SW_SHOWMAXIMIZED; break;
				}
			}

			if(0 == (flags & SRFlags.ShowErrorUI)) x.fMask |= Api.SEE_MASK_FLAG_NO_UI;
			if(x.lpVerb != null) x.fMask |= Api.SEE_MASK_INVOKEIDLIST; //makes slower. But verbs are rarely used.
			if(0 == (flags & SRFlags.WaitForExit)) x.fMask |= Api.SEE_MASK_NO_CONSOLE;

			file = _NormalizeFile(false, file, out bool isFullPath, out bool isShellPath);
			Pidl pidl = null;
			if(isShellPath) { //":: HexEncodedITEMIDLIST" or "::{CLSID}..." (we convert it too because the API does not support many)
				pidl = Pidl.FromString(file); //does not throw
				if(pidl != null) {
					x.lpIDList = pidl;
					x.fMask |= Api.SEE_MASK_INVOKEIDLIST;
				} else x.lpFile = file;
			} else {
				x.lpFile = file;

				if(x.lpDirectory == null && isFullPath) x.lpDirectory = Path_.GetDirectoryPath(file);
			}
			if(!Empty(args)) x.lpParameters = Path_.ExpandEnvVar(args);

			Wnd.Misc.EnableActivate();

			bool ok = false;
			try {
				ok = Api.ShellExecuteEx(ref x);
			}
			finally {
				pidl?.Dispose();
			}
			if(!ok) throw new AuException(0, $"*run '{file}'");

			bool waitForExit = 0 != (flags & SRFlags.WaitForExit);
			bool callerNeedsHandle = more != null && more.NeedProcessHandle;
			Process_.LibProcessWaitHandle ph = null;
			if(x.hProcess != Zero) {
				if(waitForExit || callerNeedsHandle) ph = new Process_.LibProcessWaitHandle(x.hProcess);
			}

			try {
				Api.AllowSetForegroundWindow(Api.ASFW_ANY);

				if(x.lpVerb != null && !Application.MessageLoop)
					Thread.CurrentThread.Join(50); //need min 5-10 for Properties. And not Sleep.

				if(ph != null) {
					if(waitForExit) ph.WaitOne();
					if(callerNeedsHandle) more.ProcessHandle = ph;
					if(waitForExit) return Api.GetExitCodeProcess(x.hProcess, out var ec) ? (int)ec : 0;
				}
				if(x.hProcess != Zero) return Process_.GetProcessId(x.hProcess);
			}
			finally {
				if(!callerNeedsHandle || more.ProcessHandle == null) {
					if(ph != null) ph.Dispose();
					else if(x.hProcess != Zero) Api.CloseHandle(x.hProcess);
				}
			}

			return 0;

			//tested: works well in MTA apartment.
			//rejected: in QM2, run also has a 'window' parameter. However it just makes limited, unclear etc, and therefore rarely used. Instead use Wnd.FindOrRun or Find/Run/Wait like in the examples.
			//rejected: in QM2, run also has 'autodelay'. Better don't add such hidden things. Let the script decide what to do.
		}

		/// <summary>
		/// Calls <see cref="Run"/> and handles exceptions. All parameters are the same.
		/// If Run throws exception, shows warning with its Message and returns int.MinValue.
		/// This is useful when you don't care whether Run succeeded, for example in AuMenu menu item command handlers. Using try/catch there would not look good.
		/// Handles only exception of type AuException. It is thrown when fails, usually when the file does not exist.
		/// </summary>
		/// <seealso cref="Output.Warning"/>
		/// <seealso cref="AuScriptOptions.DisableWarnings"/>
		[MethodImpl(MethodImplOptions.NoInlining)] //uses stack
		public static int TryRun(string s, string args = null, SRFlags flags = 0, SRMoreParams more = null)
		{
			try {
				return Run(s, args, flags, more);
			}
			catch(AuException e) {
				Output.Warning(e.Message, 1);
				return int.MinValue;
			}
		}

		//Used by Run and RunConsole.
		static string _NormalizeFile(bool runConsole, string file, out bool isFullPath, out bool isShellPath)
		{
			isShellPath = isFullPath = false;
			file = Path_.ExpandEnvVar(file);
			if(Empty(file)) throw new ArgumentException();
			if(runConsole || !(isShellPath = Path_.LibIsShellPath(file))) {
				if(isFullPath = Path_.IsFullPath(file)) {
					var fl = runConsole ? PNFlags.DoNotExpandDosPath : PNFlags.DoNotExpandDosPath | PNFlags.DoNotPrefixLongPath;
					file = Path_.LibNormalize(file, fl, true);

					//ShellExecuteEx supports long path prefix for exe but not for documents.
					//Process.Run supports long path prefix, except when the exe is .NET.
					if(!runConsole) file = Path_.UnprefixLongPath(file);

					if(Files.Misc.DisableRedirection.IsSystem64PathIn32BitProcess(file) && !Files.ExistsAsAny(file)) {
						file = Files.Misc.DisableRedirection.GetNonRedirectedSystemPath(file);
					}
				} else if(!Path_.IsUrl(file)) {
					//ShellExecuteEx searches everywhere except in app folder.
					//Process.Run prefers current directory.
					var s2 = Files.SearchPath(file);
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
		/// Returns the process exit code. Usually a non-0 value means error.
		/// </summary>
		/// <param name="output">
		/// Receives the output text.
		/// Console programs have two output text streams - standard output and standard error. This function gets both, and the error text is always after the output text.
		/// </param>
		/// <param name="file">
		/// Path or name of an .exe or .bat file. Can be:
		/// Full path. Examples: <c>@"C:\folder\x.exe"</c>, <c>Folders.System + "x.exe"</c>, <c>@"%Folders.System%\x.exe"</c>.
		/// Filename, like <c>"x.exe"</c>. The function calls <see cref="Files.SearchPath"/>.
		/// Path relative to <see cref="Folders.ThisApp"/>. Examples: <c>"x.exe"</c>, <c>@"subfolder\x.exe"</c>, <c>@".\subfolder\x.exe"</c>, <c>@"..\another folder\x.exe"</c>.
		/// Supports environment variables, like <c>@"%TMP%\x.bat"</c>. See <see cref="Path_.ExpandEnvVar"/>.
		/// </param>
		/// <param name="args">
		/// Command line arguments.
		/// This function expands environment variables if starts with "%" or "\"%".
		/// </param>
		/// <param name="curDir">Working directory. Default - <see cref="Directory.GetCurrentDirectory"/> of this process.</param>
		/// <param name="textEncoding">Text encoding used to convert console's ANSI text to C# Unicode text.</param>
		/// <exception cref="Win32Exception">Failed, for example file not found.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Process.Start()"/>, <see cref="Process.WaitForExit"/>, <see cref="StreamReader.ReadToEnd"/>.</exception>
		/// <remarks>
		/// Does not show the console window.
		/// Uses <see cref="Process.Start()"/>. Does not use Windows shell API.
		/// See also <see cref="Run"/>.
		/// </remarks>
		public static int RunConsole(out string output, string file, string args = null, string curDir = null, Encoding textEncoding = null)
		{
			output = null;
			using(var p = new Process()) {
				p.StartInfo.FileName = _NormalizeFile(true, file, out _, out _);
				if(args != null) p.StartInfo.Arguments = Path_.ExpandEnvVar(args);
				if(curDir != null) p.StartInfo.WorkingDirectory = Path_.ExpandEnvVar(curDir);
				if(textEncoding != null) {
					p.StartInfo.StandardOutputEncoding = textEncoding;
					p.StartInfo.StandardErrorEncoding = textEncoding;
				}

				p.StartInfo.UseShellExecute = false;
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;

				p.Start();

				string so = p.StandardOutput.ReadToEnd();
				string se = p.StandardError.ReadToEnd();
				if(!Empty(se)) {
					if(Empty(so)) so = se;
					else so = so + "\r\n" + se;
				}

				p.WaitForExit(); //FUTURE: can support timeout

				output = so;
				return p.ExitCode;
			}
		}

		/// <summary>
		/// Runs a console program (hidden), waits until its process ends, and prints its output text.
		/// Calls <see cref="RunConsole(out string, string, string, string, Encoding)"/> and <see cref="Print(string)"/>.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="args"></param>
		/// <param name="directory"></param>
		/// <param name="textEncoding"></param>
		public static int RunConsole(string file, string args = null, string directory = null, Encoding textEncoding = null)
		{
			int ec = RunConsole(out var s, file, args, directory, textEncoding);
			if(!Empty(s)) Print(s);
			return ec;
		}

		/// <summary>
		/// Opens parent folder in Explorer and selects the file.
		/// Returns null if fails, for example if the file does not exist.
		/// </summary>
		/// <param name="path">
		/// Full path of a file or directory or other shell object.
		/// Supports @"%environmentVariable%\..." (see <see cref="Path_.ExpandEnvVar"/>) and "::..." (see <see cref="Pidl.ToHexString"/>).
		/// </param>
		public static bool SelectFileInExplorer(string path)
		{
			using(var pidl = Pidl.FromString(path)) {
				if(pidl == null) return false;
				return 0 == Api.SHOpenFolderAndSelectItems(pidl, 0, null, 0);
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// flags for <see cref="Shell.Run"/>.
	/// </summary>
	[Flags]
	public enum SRFlags
	{
		/// <summary>
		/// Show error message box if fails, for example if file not found.
		/// Note: this does not disable exceptions. Still need exception handling. Or call <see cref="Shell.TryRun"/>.
		/// </summary>
		ShowErrorUI = 1,

		/// <summary>
		/// If started new process, wait until it exits.
		/// Uses <see cref="WaitHandle.WaitOne()"/>.
		/// </summary>
		WaitForExit = 2,
	}

	/// <summary>
	/// More parameters for <see cref="Shell.Run"/>.
	/// </summary>
	public class SRMoreParams
	{
		/// <summary>
		/// Initial "current directory" for the new process.
		/// If this is not set (null), the function gets parent directory path of the specified file that is to run, if possible (if its full path is specified or found). It's because some incorrectly designed programs look for their files in "current directory", and fail to start if initial "current directory" is not set to the program's directory.
		/// If this is "" or invalid or the function cannot find full path, the new process will inherit "current directory" of this process.
		/// </summary>
		public string CurrentDirectory;

		/// <summary>
		/// File's right-click menu command, also known as verb. For example "edit", "print", "properties". The default verb is bold in the menu.
		/// Not all menu items will work. Some may have different name than in the menu. Use verb "RunAs" for "Run as administrator".
		/// </summary>
		public string Verb;

		/// <summary>
		/// A window that may be used as owner window of error message box.
		/// Also, new window should be opened on the same screen. However many programs ignore it.
		/// </summary>
		public WndOrControl OwnerWindow;

		/// <summary>
		/// Preferred window state.
		/// Many programs ignore it.
		/// </summary>
		public ProcessWindowStyle WindowState;

		//no. If need, caller can get window and call EnsureInScreen etc.
		//public Screen Screen;
		//this either does not work or I could not find a program that uses default window position (does not save/restore)
		//if(more.Screen != null) { x._14.hMonitor = (IntPtr)more.Screen.GetHashCode(); x.fMask |= Api.SEE_MASK_HMONITOR; } //GetHashCode gets HMONITOR

		/// <summary>
		/// Get process handle, if possible.
		/// The <see cref="ProcessHandle"/> property will contain it.
		/// </summary>
		public bool NeedProcessHandle;

		/// <summary>
		/// This is an [Out] value.
		/// When the function returns, if <see cref="NeedProcessHandle"/> was set to true, contains process handle in a <see cref="WaitHandle"/> variable.
		/// null if did not start new process (eg opened the document in an existing process) or did not get process handle for some other reason.
		/// Note: WaitHandle is disposable.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// //this code does the same as Shell.Run(@"notepad.exe", flags: SRFlags.WaitForExit);
		/// var p = new SRMoreParams() { NeedProcessHandle = true };
		/// Shell.Run(@"notepad.exe", more: p);
		/// using(var h = p.ProcessHandle) h?.WaitOne();
		/// ]]></code>
		/// </example>
		public WaitHandle ProcessHandle { get; internal set; }
	}
}
