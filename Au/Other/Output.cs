//#define NEED_CALLER //rejected. Too slow and generates much garbage.

/*
When Write etc is called, where the string goes:
	If redirected, to wherever it is redirected.
	Else if using log file (LogFile not null), writes to the file.
	Else if using console (IsWritingToConsole), writes to console.
	Else if using local Output.Server (in this appdomain), writes to it.
	Else if exists global Output.Server (in any process/appdomain), writes to it.
	Else nowhere.

Only single global server is supported in this user session.
Only single local server is supported in this appdomain.

Output.Server does not implement an output window etc. It just collects messages and notifies an output window. Asynchronously.

How global output server/client is implemented:
	Single server and multiple clients.
	Server receives messages sent by clients.
	Clients - processes and appdomains that send text messages to the server. The server's appdomain also can be client.
	For IPC is used mailslot, waitable timer and shared memory (SM).
	Server:
		Creates mailslot and timer. Sets a bool about it in SM.
		Waits for timer and reads messages from mailslot.
			For better reliability, also checks mailslot periodically.
			If messages available, notifies an output window.
		On exit clears the bool in SM.
	Client, when sending a text message:
		If the SM bool is not set - discards the message, and closes mailslot if was open. Else:
		If not still done, opens mailslot.
		Writes message to mailslot.
		Sets timer if not set. Uses another bool in SM to indicate that the timer is set; server clears it.

How local output server/client is implemented:
	Similar to global. Differences:
	Single server and single client (the same appdomain).
	Uses waitable timer, but not mailslot/SM. Instead of mailslot, adds messages directly to Messages. Instead of SM, uses static variables.

*/


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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using Microsoft.Win32.SafeHandles;
using System.Collections.Concurrent;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Writes text to the output window, console or log file.
	/// </summary>
	//[DebuggerStepThrough]
	public static class Output
	{
		//note:
		//This library does not redirect Console.WriteLine, unless user calls Output.RedirectConsoleOutput.
		//One of reasons - there is no way to auto-run a class library initialization code that would redirect. Static ctors run before the class is used first time, not when assembly loaded.

		/// <summary>
		/// Returns true if this is a console process.
		/// </summary>
		public static bool IsConsoleProcess => _isConsole;
		static readonly bool _isConsole = Console.OpenStandardInput(1) != Stream.Null;

		/// <summary>
		/// Returns true if is writing to console, false if to the output window or log file. Assuming that <see cref="Writer"/> is not changed.
		/// Does not write to console in these cases: <see cref="IsConsoleProcess"/> is false. <see cref="IgnoreConsole"/> is true. <see cref="LogFile"/> is not null. The startup info of this process tells to not show console window and to not redirect the standard output.
		/// </summary>
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
			} else if(LibWriteToQM2) {
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
		/// <seealso cref="PrintHex(object)"/>
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
			else if(LibWriteToQM2) _WriteToQM2(value);
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
					try { Files.Delete(_logFile); } catch { }
				} else {
					_hFile.Clear();
				}
			}
		}

		static Server s_localServer; //null if we don't have a local server
		static _ClientOfGlobalServer s_client = new _ClientOfGlobalServer();

		[MethodImpl(MethodImplOptions.NoInlining)] //for stack trace
		static void _WriteToOutputServer(string s)
		{
			Debug.Assert(s != null);

			Api.GetSystemTimeAsFileTime(out var time);

			string caller = _domainName;
#if NEED_CALLER
			if(Server.LibNeedCallerMethod) {
				//info: this func always called from WriteDirectly, which is usually called through Writer, Write, Print, etc. But it is public and can be called directly.
				var k = new StackTrace(2); //skip this func and WriteDirectly
				lock(_writerTypes) {
					for(int i = 0, n = k.FrameCount; i < n; i++) {
						var m = k.GetFrame(i).GetMethod();
						var t = m.DeclaringType;
						if(_writerTypes.Contains(t)) continue;
						caller = caller + ":" + t.Name + "." + m.Name;
						break;
					}
				}
				//speed: with 'new StackFrame(i)' usually slower, regardless of stack size. Faster only when 1 loop, maybe 2.
				//info: here we don't optimize caller strings like Server does, because StackTrace creates much more garbage.
			}
#endif

			var loc = s_localServer;
			if(loc != null) loc.LocalWrite(s, time, caller);
			else s_client.WriteLine(s, time, caller);

			//TODO: if text is written at a high rate, the server may be too slow to get it. Can accumulate gigabytes and crash.
			//Example:
			//this code executes maybe one minute. The server gets all text in one time. Then its StringBuilder's capacity is > 870 MB, and total memory should be several GB.
			//for(int i = 0; i < 10000000; i++) {
			//	Output.Write("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq");
			//}
			//this code is OK. Server gets text maybe 10 times.
			//for(int i = 0; i < 1000; i++) {
			//	Output.Write("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq");
			//	Time.Sleep(1);
			//}
		}
		static string _domainName = AppDomain.CurrentDomain.FriendlyName; //cache because the property returns new string object everytime. Maybe the name can be changed, but unlikely somebody would know and do it, never mind.

		static void _ClearToOutputServer()
		{
			var loc = s_localServer;
			if(loc != null) loc.LocalWrite(null);
			else s_client.Clear();
		}

#if NEED_CALLER
		static readonly List<Type> _writerTypes = new List<Type>() { typeof(Output), typeof(_OutputWriter), typeof(NoClass) };

		/// <summary>
		/// Introduces a class that contain methods designed to write to the output.
		/// Purpose - when server's <see cref="Server.NeedCallerMethod"/> is true, skip methods of this class when searching for the caller method in the call stack.
		/// For example, if you created class PrintColored that contains methods PrintRed, PrintGreen and PrintBlue, you should execute this code in its static constructor: <c>Output.IntroduceWriterClass(typeof(PrintColored));</c>.
		/// Also use this if you redirect output using a writer class that calls WriteDirectly.
		/// Not used when writing to console or log file.
		/// </summary>
		public static void IntroduceWriterClass(Type t)
		{
			lock(_writerTypes) {
				if(!_writerTypes.Contains(t)) _writerTypes.Add(t);
			}
		}
#endif

		/// <summary>
		/// Sets to use QM2 as the output server.
		/// Eg can be used to debug the Output class itself. Although can instead use console.
		/// </summary>
		internal static bool LibWriteToQM2 { get; set; }

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

		/// <summary>
		/// Calls Api.CreateFile to open file or mailslot.
		/// </summary>
		/// <param name="name">File path or mailslot name.</param>
		/// <param name="openExisting">Use OPEN_EXISTING. If false, uses CREATE_ALWAYS.</param>
		static SafeFileHandle _CreateFile(string name, bool openExisting)
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
				var h = _CreateFile(path, false);
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

		unsafe class _ClientOfGlobalServer
		{
			//info: the mailslot/timer are implicitly disposed when appdomain ends.

			SafeFileHandle _mailslot;
			Util.WaitableTimer _timer;

			public void WriteLine(string s, long time, string caller)
			{
				lock(_lockObj1) {
					if(!_Connect()) return;

					int lenS = s.Length, lenCaller = (caller != null) ? Math.Min(caller.Length, 255) : 0;
					int lenAll = 1 + 8 + 1 + lenCaller * 2 + lenS * 2; //type, time, lenCaller, caller, s
					bool ok;
					fixed (byte* b = Util.Buffers.LibByte(lenAll)) {
						b[0] = (byte)Server.MessageType.Write; //type
						*(long*)(b + 1) = time; //time
						b[9] = (byte)lenCaller; if(lenCaller != 0) fixed (char* p = caller) Api.memcpy(b + 10, p, lenCaller * 2); //caller
						if(lenS != 0) fixed (char* p = s) Api.memcpy(b + 10 + lenCaller * 2, p, lenS * 2); //s

						g1:
						ok = Api.WriteFile(_mailslot, b, lenAll, out var nWritten);
						if(!ok && _ReopenMailslot()) goto g1;
					}

					if(ok) _SetTimer();
				}
			}

			public void Clear()
			{
				lock(_lockObj1) {
					if(!_Connect()) return;

					g1:
					byte b = (byte)Server.MessageType.Clear;
					bool ok = Api.WriteFile(_mailslot, &b, 1, out var nWritten);
					if(!ok && _ReopenMailslot()) goto g1;
					Debug.Assert(ok);

					if(ok) _SetTimer();
				}
			}

			//If last error says that server's mailslot closed, closes client's mailsot/timer and tries to reopen. If reopened, returns true.
			bool _ReopenMailslot()
			{
				if(Native.GetError() == Api.ERROR_HANDLE_EOF) { //server's mailslot closed
					_Close();
					if(_Connect()) return true;
				} else {
					Debug.Assert(false);
				}
				return false;
			}

			void _SetTimer()
			{
				if(Server._SM->IsTimer == 0) {
					if(_timer.Set(30)) Server._SM->IsTimer = 1;
				}
			}

			void _Close()
			{
				if(_mailslot != null) {
					_mailslot.Close(); _mailslot = null;
					_timer.Close(); _timer = null;
				}
			}

			bool _Connect()
			{
				if(Server._SM->IsServer == 0) {
					_Close();
					return false;
				}

				if(_mailslot == null) {
					_mailslot = _CreateFile(Server.LibMailslotName, true);
					if(_mailslot == null) return false;

					_timer = Util.WaitableTimer.Open(Server.LibTimerName, noException: true);
					if(_timer == null) {
						_mailslot.Close(); _mailslot = null;
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Receives messages sent from clients when they call Output.Write and related methods.
		/// If server is global, clients can be multiple appdomains and processes, including this. Else only this appdomain.
		/// </summary>
		/// <remarks>
		/// Works asynchronously, to make writing messages faster.
		/// When a client writes a message, the message arrives to the server with some delay and is placed in the <see cref="Messages"/> queue.
		/// You then can get/remove messages (call Messages.TryDequeue) and display them in a window (for example).
		/// You can be notified about new messages.
		/// 
		/// Recommended setup (see example):
		/// 1. When your application starts, create an Output.Server instance and assign to a static variable. Call Start.
		/// 2. When your application creates its output window, call <see cref="SetNotifications"/> to register callback function.
		/// 3. In the callback function get/remove/display all new messages.
		/// 4. Call Stop when closing the window. Optional but recommended.
		/// </remarks>
		/// <example>
		/// Simple program with output window.
		/// <code><![CDATA[
		/// class OutputFormExample :Form
		/// {
		/// 	TextBox _tb;
		/// 
		/// 	public OutputFormExample()
		/// 	{
		/// 		_tb = new TextBox();
		/// 		_tb.ReadOnly = true;
		/// 		_tb.Multiline = true;
		/// 		_tb.ScrollBars = ScrollBars.Both;
		/// 		_tb.WordWrap = false;
		/// 		_tb.Dock = DockStyle.Fill;
		/// 		_tb.TabStop = false;
		/// 		this.Controls.Add(_tb);
		/// 
		/// 		_os.SetNotifications(_ProcessMessages, this);
		/// 	}
		/// 
		/// 	void _ProcessMessages()
		/// 	{
		/// 		while(_os.Messages.TryDequeue(out var m)) {
		/// 			switch(m.Type) {
		/// 			case Output.Server.MessageType.Clear:
		/// 				_tb.Clear();
		/// 				break;
		/// 			case Output.Server.MessageType.Write:
		/// 				//_tb.AppendText(m.Text);
		/// 				_tb.AppendText($"{DateTime.FromFileTimeUtc(m.TimeUtc).ToLocalTime()}  {m.Domain}  {m.Text}");
		/// 				break;
		/// 			}
		/// 		}
		/// 	}
		/// 
		/// 	static Output.Server _os = new Output.Server(false);
		/// 
		/// 	[STAThread]
		/// 	public static void Main()
		/// 	{
		/// 		_os.Start();
		/// 
		///			//test Print and Clear, before and after creating window
		/// 		Output.IgnoreConsole = true;
		/// 		Print("test before setting notifications");
		/// 		Task.Run(() => { 1.s(); Print("test after"); 1.s(); Output.Clear(); 1.s(); Print("test after Clear"); });
		/// 
		/// 		var f = new OutputFormExample();
		/// 		f.ShowDialog();
		/// 		_os.Stop();
		/// 	}
		/// }
		/// ]]></code>
		/// </example>
		public unsafe class Server
		{
			/// <summary>
			/// See <see cref="Message.Type"/>.
			/// </summary>
			/// <tocexclude />
			public enum MessageType
			{
				/// <summary>
				/// Add line to the output window.
				/// All Message members can be used.
				/// </summary>
				Write,

				/// <summary>
				/// Clear the output window.
				/// Other Message members are not used.
				/// </summary>
				Clear,
			}

			/// <summary>
			/// Contains message text and/or related info.
			/// See <see cref="Messages"/>.
			/// </summary>
			/// <tocexclude />
			public class Message
			{
				/// <summary>
				/// Message type.
				/// Currently there are 2 types - Write and Clear.
				/// </summary>
				public MessageType Type { get; }

				/// <summary>
				/// Message text.
				/// Used with MessageType.Write.
				/// </summary>
				public string Text { get; set; }

				/// <summary>
				/// Message time in FILETIME format, UTC.
				/// Used with MessageType.Write.
				/// To convert to string: <c>DateTime.FromFileTimeUtc(m.TimeUtc).ToLocalTime().ToString()</c>.
				/// </summary>
				public long TimeUtc { get; }

#if NEED_CALLER
				/// <summary>
				/// The name of the appdomain that called the Write/Print/etc method.
				/// Used with MessageType.Write.
				/// If <see cref="NeedCallerMethod"/> is true, also includes the caller method. Format: "appdomain:type.method".
				/// </summary>
				public string Caller { get; }

				internal Message(MessageType type, string text, long time, string caller)
				{
					Type = type;
					Text = text;
					TimeUtc = time;
					Caller = caller;
				}
#else
				/// <summary>
				/// The name of the appdomain that called the Write/Print/etc method.
				/// Used with MessageType.Write.
				/// </summary>
				public string Domain { get; }

				internal Message(MessageType type, string text, long time, string domain)
				{
					Type = type;
					Text = text;
					TimeUtc = time;
					Domain = domain;
				}
#endif
			}

			/// <summary>
			/// Contains all received and still not removed messages that were sent by clients when they call Output.Write etc.
			/// Call TryDequeue to get/remove them.
			/// </summary>
			public ConcurrentQueue<Message> Messages { get; } = new ConcurrentQueue<Message>();

			//info:
			//Although global and local servers are implemented quite differently, the interface is almost the same. For this and other reasons I decided to use single class.
			//For local server, the thread and kernel timer would be not necessary. Instead could use just a user timer. But it has some limitations etc.

			SafeFileHandle _mailslot; //used if global, else null
			Util.WaitableTimer _timer; //used always
			Action _callback;
			Control _callbackControl;
			bool _isStarted;
			bool _isGlobal;
			bool _isLocalTimer;

			readonly object _lockObj2 = new object();

			/// <param name="isGlobal">Serve all appdomains and processes that don't have local server.</param>
			public Server(bool isGlobal) => _isGlobal = isGlobal;

			/// <summary>
			/// Starts server.
			/// Returns false if server already exists (if global - in any process).
			/// </summary>
			/// <exception cref="AuException">Failed.</exception>
			public bool Start()
			{
				lock(_lockObj2) {
					if(_isGlobal) {
						var m = Api.CreateMailslot(LibMailslotName, 0, 0, Api.SECURITY_ATTRIBUTES.Common);
						if(m.IsInvalid) {
							var e = Native.GetError();
							m.SetHandleAsInvalid();
							if(e == Api.ERROR_ALREADY_EXISTS) return false; //called not first time, or exists in another process/appdomain
							throw new AuException(e, "*create mailslot");
						}

						_mailslot = m;
						_CreateTimerAndThread();
						_SM->IsServer = 1;
					} else {
						if(s_localServer != null) return false;

						_CreateTimerAndThread();
						s_localServer = this;
					}

					_isStarted = true;
				}
				return true;
			}

			void _CreateTimerAndThread()
			{
				try {
					if(_isGlobal) _timer = Util.WaitableTimer.Create(false, LibTimerName);
					else _timer = Util.WaitableTimer.Create();

					var th = new Thread(_Thread) { IsBackground = true };
					th.Start();
				}
				catch {
					if(_isGlobal) { _mailslot.Close(); _mailslot = null; }
					_timer?.Close(); _timer = null;
					throw;
				}
			}

			/// <summary>
			/// Stops server.
			/// </summary>
			public void Stop()
			{
				lock(_lockObj2) {
					if(!_isStarted) return;
					_isStarted = false;
					if(_isGlobal) {
						_mailslot.Close(); _mailslot = null;
						_SM->IsServer = 0;
					} else {
						s_localServer = null;
					}
					_timer?.Set(0); //break thread loop; use minimal time. //info: the thread will dispose _timer and set=null
				}
			}

			/// <summary>
			/// Calls Stop.
			/// </summary>
			~Server() => Stop();

			/// <summary>
			/// Sets callback function, to be notified about server events.
			/// </summary>
			/// <param name="cbFunc">
			/// Callback function's delegate. Called when one or more messages are available.
			/// It should get/remove messages from <see cref="Messages"/> queue (call TryDequeue).
			/// See example in class help.
			/// </param>
			/// <param name="c">A control or form. The callback function will be called in its thread (<see cref="Control.BeginInvoke(Delegate)"/>). If null, the callback function will be called in other thread.</param>
			public void SetNotifications(Action cbFunc, Control c = null)
			{
				lock(_lockObj2) {
					_callbackControl = c;
					_callback = cbFunc;
					if(cbFunc != null && c != null) {
						if(c.IsHandleCreated) _timer?.Set(30);
						else c.HandleCreated += (unu, sed) => _timer?.Set(30);
					}
				}
			}

			void _Thread()
			{
				try {
					for(int period = 1000; ;) {
						bool isTimerEvent = _timer.WaitOne(period); //true if timer event, false if timeout
						if(isTimerEvent) {
							if(_isGlobal) _SM->IsTimer = 0;
							else _isLocalTimer = false;
						}

						Control cc; Action cbFunc;
						lock(_lockObj2) {
							cc = _callbackControl;
							cbFunc = _callback;

							if(!_isStarted || (cc != null && cc.IsDisposed)) {
								_timer.Dispose(); _timer = null;
								break;
							}

							if(_isGlobal) { //read messages from mailslot and add to Messages. Else messages are added directly to Messages.
								while(Api.GetMailslotInfo(_mailslot, null, out var nextSize, out var msgCount) && msgCount > 0) {
									fixed (byte* b0 = Util.Buffers.LibByte(nextSize + 4)) {
										var b = b0; //+4 for "\r\n"
										bool ok = Api.ReadFile(_mailslot, b, nextSize, out var readSize) && readSize == nextSize;
										if(ok) {
											long time = 0; string s = null, caller = null;
											var mtype = (MessageType)(*b++);
											switch(mtype) {
											case MessageType.Write:
												if(nextSize < 10) { ok = false; break; } //type, time(8), lenCaller
												time = *(long*)b; b += 8;
												int lenCaller = *b++;
												if(lenCaller > 0) {
													if(10 + lenCaller * 2 > nextSize) { ok = false; break; }
													caller = Util.StringCache.LibAdd((char*)b, lenCaller);
													b += lenCaller * 2;
												}
												int len = (nextSize - (int)(b - b0)) / 2;
												if(!NoNewline) {
													char* p = (char*)(b0 + nextSize);
													p[0] = '\r'; p[1] = '\n';
													len += 2;
												}
												s = new string((char*)b, 0, len);
												break;
											case MessageType.Clear:
												if(nextSize != 1) ok = false;
												break;
											default:
												ok = false;
												break;
											}
											Debug.Assert(ok);
											if(ok) {
												var m = new Message(mtype, s, time, caller);
												Messages.Enqueue(m);
											}
										}
									}
									if(msgCount == 1) break;
								}
							}
						}

						if(cbFunc != null && !Messages.IsEmpty) {
							if(cc == null) cbFunc();
							else if(cc.IsHandleCreated) cc.BeginInvoke(cbFunc);
						}

						if(isTimerEvent) period = 50; //check after 50 ms, to avoid 1000 ms delay in case a client did not set timer because _SM->IsTimer was still 1 although the timer was already signaled
						else period = 1000; //check every 1000 ms, for full reliability

						//Console.WriteLine($"{period}");
					}
				}
				catch(Exception ex) {
					Debug_.Dialog(ex.Message);
				}
			}

			/// <summary>
			/// Adds s directly to Messages and sets timer.
			/// If s is null, it is 'Clear' command.
			/// Else if !NoNewline, appends "\r\n".
			/// Used with local server only.
			/// </summary>
			internal void LocalWrite(string s, long time = 0, string caller = null)
			{
				Debug.Assert(!_isGlobal);
				if(!NoNewline && s != null) s += "\r\n";
				var m = new Message(s == null ? MessageType.Clear : MessageType.Write, s, time, caller);
				Messages.Enqueue(m);
				if(!_isLocalTimer) { _timer?.Set(30); _isLocalTimer = true; }
			}

			/// <summary>
			/// Let messages don't end with "\r\n".
			/// </summary>
			/// <remarks>
			/// This can be used for performance, to avoid string copying when using local server. Does not affect performance of global server.
			/// </remarks>
			public bool NoNewline { get; set; }

#if NEED_CALLER
			/// <summary>
			/// Let clients provide the caller method of Write, Print etc.
			/// Note: It makes these methods much slower, especially when thread stack is big. Then the speed usually is similar to Console.WriteLine. Also it generates much garbage. To find caller method is used <see cref="StackTrace"/> class.
			/// See also: <see cref="IntroduceWriterClass"/>.
			/// </summary>
			public bool NeedCallerMethod
			{
				get => _isGlobal ? (_SM->NeedCaller != 0) : _localNeedCaller;
				set { if(_isGlobal) _SM->NeedCaller = (byte)(value ? 1 : 0); else _localNeedCaller = value; }
			}
			bool _localNeedCaller;

			internal static bool LibNeedCallerMethod
			{
				get { var t = s_localServer; return (t != null) ? t.NeedCallerMethod : _SM->NeedCaller != 0; }
			}
#endif

			/// <summary>
			/// Gets mailslot name like @"\\.\mailslot\Au.Output\" + sessionId.
			/// </summary>
			internal static string LibMailslotName
			{
				get
				{
					if(_mailslotName == null) {
						_mailslotName = @"\\.\mailslot\Au.Output\" + Process_.GetSessionId().ToString(); //session-global namespace
					}
					return _mailslotName;
				}
			}
			static string _mailslotName;

			/// <summary>
			/// Gets waitable timer name like "timer.Au.Output".
			/// </summary>
			internal static string LibTimerName => "timer.Au.Output";

			/// <summary>
			/// Shared memory variables. Used with global server only.
			/// </summary>
			internal struct LibSharedMemoryData
			{
				//BE CAREFUL: this struct is in shared memory. Max allowed size is 16.

				public byte IsServer, IsTimer;
#if NEED_CALLER
				public byte NeedCaller;
#endif
			}
			internal static LibSharedMemoryData* _SM => &Util.LibSharedMemory.Ptr->outp;
		}
	}
}
