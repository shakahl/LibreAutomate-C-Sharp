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
	/// Writes text to the output window, console or log file.
	/// </summary>
	[DebuggerStepThrough]
	public static partial class AOutput
	{
		//note:
		//This library does not redirect Console.WriteLine, unless user calls AOutput.RedirectConsoleOutput.
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
		/// - <see cref="IsConsoleProcess"/> is false.
		/// - <see cref="IgnoreConsole"/> is true.
		/// - <see cref="LogFile"/> is not null.
		/// - The startup info of this process tells to not show console window and to not redirect the standard output.
		/// </remarks>
		public static bool IsWritingToConsole {
			get {
				if (!IsConsoleProcess || IgnoreConsole || LogFile != null) return false;
				if (!_isVisibleConsole.HasValue) {
					Api.GetStartupInfo(out var x);
					_isVisibleConsole = x.hStdOutput != default || 0 == (x.dwFlags & 1) || 0 != x.wShowWindow; //redirected stdout, or visible console window
				}
				return _isVisibleConsole.GetValueOrDefault();
			}
		}
		static bool? _isVisibleConsole;

		/// <summary>
		/// If true, Write and related functions in console process don't use the console window. Then everything is like in non-console process.
		/// </summary>
		/// <seealso cref="RedirectConsoleOutput"/>
		/// <seealso cref="RedirectDebugOutput"/>
		public static bool IgnoreConsole { get; set; }

		/// <summary>
		/// Clears the output window or console text (if <see cref="IsWritingToConsole"/>) or log file (if <see cref="LogFile"/> not null).
		/// </summary>
		public static void Clear() {
			if (LogFile != null) {
				_ClearToLogFile();
			} else if (IsWritingToConsole) {
				try { Console.Clear(); } catch { } //exception if redirected, it is documented
			} else if (QM2.UseQM2) {
				QM2.Clear();
			} else {
				_ClearToOutputServer();
			}
		}

		/// <summary>
		/// Writes text + <c>"\r\n"</c> to the output.
		/// </summary>
		/// <param name="value">
		/// Text.
		/// If "" or null, writes empty line. To write "null" if variable s is null, use code <c>AOutput.Write((object)s);</c>.
		/// </param>
		/// <remarks>
		/// Can display links, colors, images, etc. More info: [](xref:output_tags).
		/// 
		/// Where the text goes:
		/// - If redirected, to wherever it is redirected. See <see cref="Writer"/>.
		/// - Else if using log file (<see cref="LogFile"/> not null), writes to the file.
		/// - Else if using console (<see cref="IsWritingToConsole"/> returns true), writes to console.
		/// - Else if using local <see cref="AOutputServer"/> (in this process), writes to it.
		/// - Else if exists global <see cref="AOutputServer"/> (in any process), writes to it.
		/// - Else nowhere.
		/// </remarks>
		public static void Write(string value) {
			Writer.WriteLine(value);
		}

		/// <summary>
		/// Writes value of any type to the output.
		/// </summary>
		/// <param name="value">Value of any type. If null, writes "null".</param>
		/// <remarks>
		/// Calls <see cref="object.ToString"/> and <see cref="Write(string)"/>.
		/// If the type is unsigned integer (uint, ulong, ushort, byte), writes in hexadecimal format with prefix "0x".
		/// 
		/// This overload is used for all types except: strings, arrays, generic collections. They have own overloads; to use this function need to cast to object.
		/// For <b>Span</b> and other ref struct types use <c>AOutput.Write(x.ToString());</c>.
		/// </remarks>
		public static void Write(object value) {
			Write(ObjectToString_(value));
		}
		//FUTURE: support Span<T>, eg ReadOnlySpan<char>

		/// <summary>
		/// Converts object to string like <see cref="Write(object)"/> does.
		/// </summary>
		internal static string ObjectToString_(object value) {
			switch (value) {
			case null: return "null";
			case string t: return t;
			case ulong or uint or ushort or byte or nuint or System.Collections.IEnumerable or System.Collections.DictionaryEntry:
				using (new StringBuilder_(out var b)) {
					ObjectToString_(b, value, false);
					return b.ToString();
				}
			default: return value.ToString();
			}
		}

		internal static void ObjectToString_(StringBuilder b, object value, bool compact) {
			switch (value) {
			case null: b.Append("null"); break;
			case string s: b.Append(s); break;
			case ulong u: _Unsigned(b, u); break;
			case uint u: _Unsigned(b, u); break;
			case ushort u: _Unsigned(b, u); break;
			case byte u: _Unsigned(b, u); break;
			case nuint u: _Unsigned(b, u); break;
			case char[] a:
				b.Append("{ ");
				foreach (var c in a) {
					if (c < ' ') b.Append('x').Append(((byte)c).ToString("X"));
					else b.Append(c);
					b.Append(' ');
				}
				b.Append('}');
				break; //always compact
			case System.Collections.IEnumerable e:
				if (compact) b.Append("{ ");
				string sep = null;
				foreach (var v in e) {
					if (sep == null) sep = compact ? ", " : "\r\n"; else b.Append(sep);
					ObjectToString_(b, v, compact);
				}
				if (compact) b.Append(" }");
				break;
			case System.Collections.DictionaryEntry de:
				b.AppendFormat("[{0}, {1}]", de.Key, de.Value);
				break;
			default: b.Append(value); break;
			}

			static void _Unsigned(StringBuilder b, ulong u) => b.Append("0x").Append(u.ToString("X"));
		}

		/// <summary>
		/// Writes array, List, Dictionary or other generic collection to the output, as a list of items separated by "\r\n".
		/// </summary>
		/// <param name="value">Array or generic collection of any type. If null, writes "null".</param>
		/// <remarks>
		/// Calls <see cref="Write(string)"/>.
		/// </remarks>
		public static void Write<T>(IEnumerable<T> value) {
			Write(value switch { null => "null", char[] a => ObjectToString_(a), _ => string.Join("\r\n", value) });
		}

		/// <summary>
		/// Writes multiple arguments of any type to the output, using separator ", ".
		/// </summary>
		/// <remarks>
		/// If a value is null, writes "null".
		/// If a value is unsigned integer (uint, ulong, ushort, byte), writes in hexadecimal format with prefix "0x".
		/// </remarks>
		public static void Write(object value1, object value2, params object[] more) {
			Write(MultiToString_(value1, value2, more));
		}
		static readonly object[] s_oaNull = { null };

		internal static string MultiToString_(object value1, object value2, params object[] more) {
			if (more == null) more = s_oaNull; //workaround for: if third argument is null, we receive null and not array containing null
			using (new StringBuilder_(out var b)) {
				for (int i = 0, n = 2 + more.Length; i < n; i++) {
					if (i > 0) b.Append(", ");
					ObjectToString_(b, i == 0 ? value1 : (i == 1 ? value2 : more[i - 2]), compact: true);
				}
				return b.ToString();

				//rejected: escape strings (eg if contains characters "\r\n,\0"):
				//	it can damage formatting tags etc;
				//	the string may be already escaped, eg AWnd.ToString or AAcc.ToString;
				//	we don't know whether the caller wants it;
				//	let the caller escape it if wants, it's easy.
			}
		}

		/// <summary>
		/// Gets or sets object that actually writes text when is called <see cref="Write"/>.
		/// </summary>
		/// <remarks>
		/// If you want to redirect or modify or just monitor output text, use code like in the example. It is known as "output redirection".
		/// Redirection is applied to whole process, not just this thread.
		/// Redirection affects <see cref="Write"/>, <see cref="RedirectConsoleOutput"/> and <see cref="RedirectDebugOutput"/>. It does not affect <see cref="WriteDirectly"/> and <see cref="Clear"/>.
		/// Don't call <see cref="Write"/> in method <b>WriteLine</b> of your writer class. It would call itself and create stack overflow. Call <see cref="WriteDirectly"/>, like in the example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// [STAThread]
		/// static void Main()
		/// {
		/// 	AOutput.Writer = new TestOutputWriter();
		/// 
		/// 	AOutput.Write("test");
		/// }
		/// 
		/// class TestOutputWriter :TextWriter
		/// {
		/// 	public override void WriteLine(string value) { AOutput.WriteDirectly("redirected: " + value); }
		/// 	public override Encoding Encoding => Encoding.Unicode;
		/// }
		/// ]]></code>
		/// </example>
		public static TextWriter Writer { get; set; } = new _OutputWriter();

		/// <summary>
		/// Our default writer class for the Writer property.
		/// </summary>
		[DebuggerStepThrough]
		class _OutputWriter : TextWriter
		{
			StringBuilder _b;

			public override Encoding Encoding => Encoding.Unicode;
			public override void WriteLine(string value) { WriteDirectly(_PrependBuilder(value)); }
			public override void Write(string value) {
				//QM2.Write($"'{value}'");
				if (value.NE()) return;
				if (value.Ends('\n')) {
					WriteLine(value[..^(value.Ends("\r\n") ? 2 : 1)]);
				} else {
					(_b ??= new StringBuilder()).Append(value);
				}
			}
			string _PrependBuilder(string value) {
				if (_b != null && _b.Length > 0) {
					value = _b.ToString() + value;
					_b.Clear();
				}
				return value;
			}
			public override void Flush() {
				var s = _PrependBuilder(null);
				if (!s.NE()) WriteDirectly(s);
			}
		}

		/// <summary>
		/// Writes string value + "\r\n" to the output window or console (if <see cref="IsWritingToConsole"/>) or log file (if <see cref="LogFile"/> not null).
		/// Unlike <see cref="Write"/>, the string is not passed to <see cref="Writer"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)] //for stack trace, used in _WriteToOutputServer
		public static void WriteDirectly(string value) {
			value ??= "";
			//QM2.Write($"'{value}'");

			if (LogFile != null) _WriteToLogFile(value);
			else if (IsWritingToConsole) Console.WriteLine(value);
			else if (QM2.UseQM2) QM2.Write(value);
			else _WriteToOutputServer(value);
		}

		/// <summary>
		/// Let <b>Console.WriteX</b> methods in non-console process write to the same destination as <see cref="Write"/>.
		/// </summary>
		/// <remarks>
		/// If <b>Console.Write</b> text does not end with '\n' character, it is buffered and not displayed until called again with text ending with '\n' character or until called <b>Console.WriteLine</b>.
		/// <b>Console.Clear</b> will not clear output; it will throw exception.
		/// </remarks>
		public static bool RedirectConsoleOutput {
			set {
				if (value) {
					if (_prevConsoleOut != null || IsConsoleProcess) return;
					_prevConsoleOut = Console.Out;
					Console.SetOut(Writer);
				} else if (_prevConsoleOut != null) {
					Console.SetOut(_prevConsoleOut);
					_prevConsoleOut = null;
				}
			}
			get => _prevConsoleOut != null;
		}
		static TextWriter _prevConsoleOut;

		/// <summary>
		/// Let <b>Debug.Write</b>, <b>Trace.Write</b> and similar methods also write to the same destination as <see cref="Write"/>.
		/// </summary>
		/// <remarks>
		/// Does not replace existing <b>Debug.Write</b> etc destinations, just add new destination.
		/// 
		/// If <b>Debug/Trace.Write</b> text does not end with '\n' character, it is buffered and not displayed until called again with text ending with '\n' character or until called <b>Debug/Trace.WriteLine</b>.
		/// 
		/// Tip: To write to the output window even in console process, set <c>AOutput.IgnoreConsole=true;</c> before calling this method first time.
		/// </remarks>
		public static bool RedirectDebugOutput {
			set {
				if (value) {
					if (_traceListener != null) return;
					//Trace.Listeners.Add(IsWritingToConsole ? (new ConsoleTraceListener()) : (new TextWriterTraceListener(Writer)));
					Trace.Listeners.Add(_traceListener = new TextWriterTraceListener(Writer));
					//speed: 6100
				} else if (_traceListener != null) {
					Trace.Listeners.Remove(_traceListener);
					_traceListener = null;
				}
			}
			get => _traceListener != null;
		}
		static TextWriterTraceListener _traceListener;

		/// <summary>
		/// Sets log file path.
		/// When set (not null), text passed to <see cref="Write"/> will be written to the file.
		/// If value is null - restores default behavior.
		/// </summary>
		/// <remarks>
		/// The first <see cref="Write"/> etc call (in this process) creates or opens the file and deletes old content if the file already exists.
		/// 
		/// Also supports mailslots. For <b>LogFile</b> use mailslot name, as documented in <msdn>CreateMailslot</msdn>. Multiple processes can use the same mailslot.
		/// </remarks>
		/// <exception cref="ArgumentException">The 'set' function throws this exception if the value is not full path and not null.</exception>
		public static string LogFile {
			get => _logFile;
			set {
				lock (_lockObj1) {
					if (_hFile != null) {
						_hFile.Close();
						_hFile = null;
					}
					if (value != null) {
						_logFile = APath.Normalize(value);
					} else _logFile = null;
				}
			}

		}
		static string _logFile;
		static _LogFile _hFile;
		static readonly object _lockObj1 = new();

		/// <summary>
		/// Let Write etc also add current time when using log file (see <see cref="LogFile"/>).
		/// The time is local, not UTC.
		/// </summary>
		public static bool LogFileTimestamp { get; set; }

		static void _WriteToLogFile(string s) {
			lock (_lockObj1) {
				if (_hFile == null) {
					g1:
					_hFile = _LogFile.Open();
					if (_hFile == null) {
						var e = ALastError.Code;
						if (e == Api.ERROR_SHARING_VIOLATION) {
							var u = APath.MakeUnique(_logFile, false);
							if (u != _logFile) { _logFile = u; goto g1; }
						}
						var logf = _logFile;
						_logFile = null;
						AWarning.Write($"Failed to create or open log file '{logf}'. {ALastError.MessageFor(e)}");
						WriteDirectly(s);
						return;
					}
				}
				_hFile.WriteLine(s);
			}
		}

		static void _ClearToLogFile() {
			lock (_lockObj1) {
				if (_hFile == null) {
					try { AFile.Delete(_logFile); } catch { }
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

			Handle_ _h;
			string _name;

			/// <summary>
			/// Opens LogFile file handle for writing.
			/// Uses CREATE_ALWAYS, GENERIC_WRITE, FILE_SHARE_READ.
			/// </summary>
			public static _LogFile Open() {
				var path = LogFile;
				var h = CreateFile_(path, false);
				if (h.Is0) return null;
				return new _LogFile() { _h = h, _name = path };
			}

			/// <summary>
			/// Writes s + "\r\n" and optionally timestamp.
			/// </summary>
			/// <remarks>
			/// If fails to write to file: Sets LogFile=null, which closes file handle. Writes a warning and s to the output window or console.
			/// </remarks>
			public bool WriteLine(string s) {
				bool ok;
				int n = Encoding.UTF8.GetByteCount(s ??= "") + 1;
				fixed (byte* b = AMemoryArray.Byte_(n + 35)) {
					if (LogFileTimestamp) {
						Api.GetLocalTime(out var t);
						Api.wsprintfA(b, "%i-%02i-%02i %02i:%02i:%02i.%03i   ", __arglist(t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond, t.wMilliseconds));
						int nn = BytePtr_.Length(b);
						Encoding.UTF8.GetBytes(s, new Span<byte>(b + nn, n));
						n += nn;
						if (s.Starts("<>")) {
							Api.memmove(b + 2, b, nn);
							b[0] = (byte)'<'; b[1] = (byte)'>';
						}
					} else {
						Encoding.UTF8.GetBytes(s, new Span<byte>(b, n));
					}
					b[n - 1] = 13; b[n++] = 10;

					ok = Api.WriteFile(_h, b, n, out _);
				}
				if (!ok) {
					string emsg = ALastError.Message;
					LogFile = null;
					AWarning.Write($"Failed to write to log file '{_name}'. {emsg}");
					WriteDirectly(s);
					//Debug.Assert(false);
				}
				return ok;
			}

			/// <summary>
			/// Sets file size = 0.
			/// </summary>
			public bool Clear() {
				bool ok = Api.SetFilePointerEx(_h, 0, null, Api.FILE_BEGIN) && Api.SetEndOfFile(_h);
				Debug.Assert(ok);
				return ok;
			}

			/// <summary>
			/// Closes file handle.
			/// </summary>
			public void Close() => _h.Dispose();
		}

		/// <summary>
		/// Calls Api.CreateFile to open file or mailslot.
		/// </summary>
		/// <param name="name">File path or mailslot name.</param>
		/// <param name="openExisting">Use OPEN_EXISTING. If false, uses CREATE_ALWAYS.</param>
		internal static Handle_ CreateFile_(string name, bool openExisting) {
			return Api.CreateFile(name, Api.GENERIC_WRITE, Api.FILE_SHARE_READ, default, openExisting ? Api.OPEN_EXISTING : Api.CREATE_ALWAYS);

			//tested: CREATE_ALWAYS works with mailslot too. Does not erase messages. Undocumented what to use.
		}

		///
#if DEBUG
		public
#else
		internal
#endif
		static class QM2
		{
			/// <summary>
			/// Sets to use QM2 as the output server.
			/// </summary>
			public static bool UseQM2 { get; set; }

			/// <summary>
			/// Clears QM2 output pane.
			/// </summary>
			public static void Clear() => _WriteToQM2(null);

			/// <summary>
			/// Writes line to QM2.
			/// </summary>
			public static void Write(object o) => _WriteToQM2(o?.ToString() ?? "");

			/// <summary>
			/// Writes multiple arguments of any type to the output, using separator ", ".
			/// </summary>
			/// <remarks>
			/// If a value is null, writes "null".
			/// If a value is unsigned integer (uint, ulong, ushort, byte), writes in hexadecimal format with prefix "0x".
			/// </remarks>
			public static void Write(object value1, object value2, params object[] more) {
				Write(MultiToString_(value1, value2, more));
			}

			/// <param name="s">If null, clears output.</param>
			static void _WriteToQM2(string s) {
				if (!_hwndQM2.IsAlive) {
					_hwndQM2 = Api.FindWindow("QM_Editor", null);
					if (_hwndQM2.Is0) return;
				}
				_hwndQM2.SendS(Api.WM_SETTEXT, -1, s);
			}
			static AWnd _hwndQM2;
		}
	}
}
