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
//using System.Linq;
//using System.Xml.Linq;
using Microsoft.Win32.SafeHandles;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Writes text to the output window, console or log file.
	/// </summary>
	/// <remarks>
	/// When <see cref="Write"/>, <b>Print</b>, etc is called, where the text goes:
	/// <list type="bullet">
	/// <item>If redirected, to wherever it is redirected. See <see cref="Writer"/>.</item>
	/// <item>Else if using log file (<see cref="LogFile"/> not null), writes to the file.</item>
	/// <item>Else if using console (<see cref="IsWritingToConsole"/> returns true), writes to console.</item>
	/// <item>Else if using local <see cref="Util.OutputServer"/> (in this appdomain), writes to it.</item>
	/// <item>Else if exists global <see cref="Util.OutputServer"/> (in any process/appdomain), writes to it.</item>
	/// <item>Else nowhere.</item>
	/// </list>
	/// </remarks>
	//[DebuggerStepThrough]
	public static partial class Output
	{
		//note:
		//This library does not redirect Console.WriteLine, unless user calls Output.RedirectConsoleOutput.
		//One of reasons - there is no way to auto-run a class library initialization code that would redirect.

		/// <summary>
		/// Returns true if this is a console process.
		/// </summary>
		public static bool IsConsoleProcess => _isConsole;
		static readonly bool _isConsole = Console.OpenStandardInput(1) != Stream.Null;

		/// <summary>
		/// Returns true if is writing to console, false if to the output window or log file. Assuming that <see cref="Writer"/> is not changed.
		/// </summary>
		/// <remarks>
		/// Does not write to console in these cases:
		/// <list type="bullet">
		/// <item><see cref="IsConsoleProcess"/> is false.</item>
		/// <item><see cref="IgnoreConsole"/> is true.</item>
		/// <item><see cref="LogFile"/> is not null.</item>
		/// <item>The startup info of this process tells to not show console window and to not redirect the standard output.</item>
		/// </list>
		/// </remarks>
		public static bool IsWritingToConsole
		{
			get
			{
				if(!IsConsoleProcess || IgnoreConsole || LogFile != null) return false;
				if(!_isVisibleConsole.HasValue) {
					Api.GetStartupInfo(out var x);
					_isVisibleConsole = x.hStdOutput != default || 0 == (x.dwFlags & 1) || 0 != x.wShowWindow; //redirected stdout, or visible console window
				}
				return _isVisibleConsole.GetValueOrDefault();
			}
		}
		static bool? _isVisibleConsole;

		/// <summary>
		/// If true, Write and related functions in console process don't not use the console window. Then everything is like in non-console process.
		/// </summary>
		/// <seealso cref="RedirectConsoleOutput"/>
		/// <seealso cref="RedirectDebugOutput"/>
		public static bool IgnoreConsole { get; set; }

		/// <summary>
		/// Clears the output window or console text (if <see cref="IsWritingToConsole"/>) or log file (if <see cref="LogFile"/> not null).
		/// </summary>
		public static void Clear()
		{
			if(LogFile != null) {
				_ClearToLogFile();
			} else if(IsWritingToConsole) {
				try { Console.Clear(); } catch { } //exception if redirected, it is documented
			} else if(LibUseQM2) {
				_WriteToQM2(null);
			} else {
				_ClearToOutputServer();
			}
		}

		/// <summary>
		/// Writes string + "\r\n" to the output window or console (if <see cref="IsWritingToConsole"/>) or log file (if <see cref="LogFile"/> not null).
		/// </summary>
		/// <seealso cref="Print(string)"/>
		/// <seealso cref="Print(object)"/>
		/// <seealso cref="Print{T}(IEnumerable{T})"/>
		/// <seealso cref="Print(object, object, object[])"/>
		/// <seealso cref="PrintListEx{T}(IEnumerable{T}, string, int)"/>
		public static void Write(string value)
		{
			Writer.WriteLine(value);
		}

		/// <summary>
		/// Gets or sets object that actually writes text when is called Output.Write, Print and similar functions.
		/// </summary>
		/// <remarks>
		/// If you want to redirect, modify or just monitor output text, use code like in the example. It is known as "output redirection".
		/// Redirection is applied to whole appdomain, and does not affect other appdomains.
		/// Redirection affects Output.Write, Print and similar functions, also Output.RedirectConsoleOutput and Output.RedirectDebugOutput. It does not affect Output.WriteDirectly and Output.Clear.
		/// Don't call Output.Write, Print etc in method WriteLine(string) of your writer class. It would call itself and create stack overflow. But you can call Output.WriteDirectly.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// [STAThread]
		/// static void Main()
		/// {
		/// 	Output.Writer = new TestOutputWriter();
		/// 
		/// 	Print("test");
		/// }
		/// 
		/// class TestOutputWriter :TextWriter
		/// {
		/// 	public override void WriteLine(string value) { Output.WriteDirectly("redir: " + value); }
		/// 	public override Encoding Encoding => Encoding.Unicode;
		/// }
		/// ]]></code>
		/// </example>
		public static TextWriter Writer { get; set; } = new _OutputWriter();

		/// <summary>
		/// Our default writer class for the Writer property.
		/// </summary>
		class _OutputWriter :TextWriter
		{
			public override void WriteLine(string value) { WriteDirectly(value); }
			public override Encoding Encoding => Encoding.Unicode;
		}

		/// <summary>
		/// Writes string value + "\r\n" to the output window or console (if <see cref="IsWritingToConsole"/>) or log file (if <see cref="LogFile"/> not null).
		/// Unlike Output.Write, Print etc, the string is not passed to <see cref="Writer"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)] //for stack trace, used in _WriteToOutputServer
		public static void WriteDirectly(string value)
		{
			if(value == null) value = "";

			if(LogFile != null) _WriteToLogFile(value);
			else if(IsWritingToConsole) Console.WriteLine(value);
			else if(LibUseQM2) _WriteToQM2(value);
			else _WriteToOutputServer(value);
		}

		/// <summary>
		/// Let Console.WriteX methods in non-console process write to the same destination as Output.Write etc.
		/// </summary>
		/// <remarks>
		/// Console.Write will write line, like Console.WriteLine.
		/// Console.Clear will not clear output; it will throw exception.
		/// </remarks>
		public static bool RedirectConsoleOutput
		{
			set
			{
				if(value) {
					if(_prevConsoleOut != null || IsConsoleProcess) return;
					_prevConsoleOut = Console.Out;
					Console.SetOut(Writer);
					//speed: 870
				} else if(_prevConsoleOut != null) {
					Console.SetOut(_prevConsoleOut);
					_prevConsoleOut = null;
				}
			}
		}
		static TextWriter _prevConsoleOut;

		/// <summary>
		/// Let Debug.WriteX and Trace.WriteX methods write to the same destination as Output.Write etc.
		/// </summary>
		/// <remarks>
		/// Tip: To write to the output window even in console process, set <c>Output.IgnoreConsole=true;</c> before calling this method first time.
		/// </remarks>
		public static bool RedirectDebugOutput
		{
			set
			{
				if(value) {
					if(_traceListener != null) return;
					//Trace.Listeners.Add(IsWritingToConsole ? (new ConsoleTraceListener()) : (new TextWriterTraceListener(Writer)));
					Trace.Listeners.Add(_traceListener = new TextWriterTraceListener(Writer));
					//speed: 6100
				} else if(_traceListener != null) {
					Trace.Listeners.Remove(_traceListener);
				}
			}
		}
		static TextWriterTraceListener _traceListener;

		/// <summary>
		/// Sets log file path.
		/// When set (not null), text passed to Output.Write, Print and similar functions will be written to the file. Assuming that <see cref="Writer"/> is not changed.
		/// If value is null - restores default behavior.
		/// </summary>
		/// <remarks>
		/// The first Write etc call (in this appdomain) creates or opens the file and deletes old content if the file already exists.
		/// Multiple appdomains cannot use the same file. If the file is open for writing, Write makes unique filename and changes LogFile value.
		/// 
		/// Also supports mailslots. For LogFile use mailslot name, as documented in <msdn>CreateMailslot</msdn>. Multiple appdomains and processes can use the same mailslot.
		/// </remarks>
		/// <exception cref="ArgumentException">The 'set' function throws this exception if the value is not full path and not null.</exception>
		public static string LogFile
		{
			get => _logFile;
			set
			{
				lock(_lockObj1) {
					if(_hFile != null) {
						_hFile.Close();
						_hFile = null;
					}
					if(value != null) {
						_logFile = Path_.Normalize(value);
					} else _logFile = null;
				}
			}

		}
		static string _logFile;
		static _LogFile _hFile;
		static readonly object _lockObj1 = new object();

		/// <summary>
		/// Let Write etc also add current time when using log file (see <see cref="LogFile"/>).
		/// The time is local, not UTC.
		/// </summary>
		public static bool LogFileTimestamp { get; set; }

		static void _WriteToLogFile(string s)
		{
			lock(_lockObj1) {
				if(_hFile == null) {
					g1:
					_hFile = _LogFile.Open();
					if(_hFile == null) {
						var e = Native.GetError();
						if(e == Api.ERROR_SHARING_VIOLATION) {
							var u = Path_.MakeUnique(_logFile, false);
							if(u != _logFile) { _logFile = u; goto g1; }
						}
						var logf = _logFile;
						_logFile = null;
						PrintWarning($"Failed to create or open log file '{logf}'. {Native.GetErrorMessage(e)}");
						WriteDirectly(s);
						return;
					}
				}
				_hFile.WriteLine(s);
			}
		}

		static void _ClearToLogFile()
		{
			lock(_lockObj1) {
				if(_hFile == null) {
					try { File_.Delete(_logFile); } catch { }
				} else {
					_hFile.Clear();
				}
			}
		}

		unsafe class _LogFile
		{
			//info: We don't use StreamWriter. It creates more problems than would make easier.
			//	Eg its finalizer does not write to file. If we try to Close it in our finalizer, it throws 'already disposed'.
			//	Also we don't need such buffering. Better to write to the OS file buffer immediately, it's quite fast.

			SafeFileHandle _h;
			string _name;

			/// <summary>
			/// Opens LogFile file handle for writing.
			/// Uses CREATE_ALWAYS, GENERIC_WRITE, FILE_SHARE_READ.
			/// </summary>
			/// <remarks>
			/// Multiple appdomains cannot use the same file. Each appdomain overwrites it when opens first time.
			/// </remarks>
			public static _LogFile Open()
			{
				var path = LogFile;
				var h = LibCreateFile(path, false);
				if(h == null) return null;
				return new _LogFile() { _h = h, _name = path };
			}

			/// <summary>
			/// Writes s + "\r\n" and optionally timestamp.
			/// </summary>
			/// <remarks>
			/// If fails to write to file: Sets LogFile=null, which closes file handle. Writes a warning and s to the output window or console.
			/// </remarks>
			public bool WriteLine(string s)
			{
				bool ok;
				int n = Convert_.Utf8LengthFromString(s) + 1;
				fixed (byte* b = Util.Buffers.LibByte(n + 35)) {
					if(LogFileTimestamp) {
						Api.GetLocalTime(out var t);
						Api.wsprintfA(b, "%i-%02i-%02i %02i:%02i:%02i.%03i   ", __arglist(t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond, t.wMilliseconds));
						int nn = Util.LibCharPtr.Length(b);
						Convert_.Utf8FromString(s, b + nn, n);
						n += nn;
						if(s.StartsWith_("<>")) {
							Api.memmove(b + 2, b, nn);
							b[0] = (byte)'<'; b[1] = (byte)'>';
						}
					} else {
						Convert_.Utf8FromString(s, b, n);
					}
					b[n - 1] = 13; b[n++] = 10;

					ok = Api.WriteFile(_h, b, n, out var nWritten);
				}
				if(!ok) {
					string emsg = Native.GetErrorMessage();
					LogFile = null;
					PrintWarning($"Failed to write to log file '{_name}'. {emsg}");
					WriteDirectly(s);
					//Debug.Assert(false);
				}
				return ok;
			}

			/// <summary>
			/// Sets file size = 0.
			/// </summary>
			public bool Clear()
			{
				bool ok = Api.SetFilePointerEx(_h, 0, null, Api.FILE_BEGIN) && Api.SetEndOfFile(_h);
				Debug.Assert(ok);
				return ok;
			}

			/// <summary>
			/// Closes file handle.
			/// </summary>
			public void Close() { if(_h != null) { _h.Close(); _h = null; } }
		}

		/// <summary>
		/// Calls Api.CreateFile to open file or mailslot.
		/// </summary>
		/// <param name="name">File path or mailslot name.</param>
		/// <param name="openExisting">Use OPEN_EXISTING. If false, uses CREATE_ALWAYS.</param>
		internal static SafeFileHandle LibCreateFile(string name, bool openExisting)
		{
			var h = Api.CreateFile(name, Api.GENERIC_WRITE, Api.FILE_SHARE_READ, null, openExisting ? Api.OPEN_EXISTING : Api.CREATE_ALWAYS);
			if(h.IsInvalid) {
				var e = Native.GetError();
				h.SetHandleAsInvalid();
				Native.SetError(e);
				return null;
			}
			return h;

			//tested: CREATE_ALWAYS works with mailslot too. Does not erase messages. Undocumented what to use.
		}

		/// <summary>
		/// Sets to use QM2 as the output server.
		/// </summary>
#if DEBUG
		public
#else
		internal
#endif
		static bool LibUseQM2 { get; set; }

		/// <param name="s">If null, clears output.</param>
		static void _WriteToQM2(string s)
		{
			if(!_hwndQM2.IsAlive) {
				_hwndQM2 = Api.FindWindow("QM_Editor", null);
				if(_hwndQM2.Is0) return;
			}
			_hwndQM2.SendS(Api.WM_SETTEXT, -1, s);
		}
		static Wnd _hwndQM2;

		internal static void LibWriteQM2(object o) => _WriteToQM2(o?.ToString() ?? "");

		internal static void LibClearQM2() => _WriteToQM2(null);
	}
}
