//#define NEED_CALLER //rejected. Too slow and generates much garbage.

/*
Single global server is supported in this user session.
Single local server is supported in this process.

AOutputServer does not implement an output window etc. It just collects messages and notifies an output window. Asynchronously.

How global output server/client is implemented:
	Single server and multiple clients.
	Server receives messages sent by clients.
	Clients - processes that send text messages to the server. The server's process also can be client.
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
	Single server and single client (the same process).
	Uses waitable timer, but not mailslot/SM. Instead of mailslot, adds messages directly to _messages. Instead of SM, uses static variables.

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
//using System.Linq;

using Au.Types;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Receives messages sent by <see cref="AOutput.Write"/>.
	/// </summary>
	/// <remarks>
	/// If server is global, clients can be multiple processes, including this. Else only this process.
	/// Works asynchronously, to make writing messages faster.
	/// When a client writes a message, the message arrives to the server with some delay and is placed in a queue.
	/// You then can get/remove messages from the queue (call <see cref="GetMessage"/>) and display them in a window (for example).
	/// You can be notified about new messages.
	/// 
	/// Recommended setup (see example):
	/// 1. When your application starts, create an <b>AOutputServer</b> instance and assign to a static variable. Call <see cref="Start"/>.
	/// 2. When your application creates its output window, call <see cref="SetNotifications"/> to set window/message for notifications.
	/// 3. In window procedure, when received the notification message, get/remove/display all new output messages.
	/// 4. Call <see cref="Stop"/> when closing the window.
	/// </remarks>
	/// <example>
	/// Simple program with output window.
	/// <code><![CDATA[
	/// using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
	/// using System.Windows.Forms;
	/// using System.Threading.Tasks;
	/// using System.Runtime.InteropServices;
	/// 
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
	/// 	}
	/// 
	/// 	protected override void OnHandleCreated(EventArgs e) {
	/// 		_os.SetNotifications(this.Hwnd(), WM_APP);
	/// 		base.OnHandleCreated(e);
	/// 	}
	/// 
	/// 	protected override void WndProc(ref Message m) {
	/// 		if(m.Msg == WM_APP) _ProcessMessages();
	/// 		base.WndProc(ref m);
	/// 	}
	/// 	
	/// 	internal const int WM_APP = 0x8000;
	/// 
	/// 	void _ProcessMessages()
	/// 	{
	/// 		while(_os.GetMessage(out var m)) {
	/// 			switch(m.Type) {
	/// 			case OutServMessageType.Clear:
	/// 				_tb.Clear();
	/// 				break;
	/// 			case OutServMessageType.Write:
	/// 				//_tb.AppendText(m.Text);
	/// 				_tb.AppendText($"{DateTime.FromFileTimeUtc(m.TimeUtc).ToLocalTime()}  {m.Caller}  {m.Text}");
	/// 				break;
	/// 			}
	/// 		}
	/// 	}
	/// 
	/// 	static AOutputServer _os = new AOutputServer(isGlobal: false);
	/// 
	/// 	[STAThread]
	/// 	public static void Main()
	/// 	{
	/// 		_os.Start();
	/// 
	/// 		//test Write and Clear, before and after creating window
	/// 		AOutput.IgnoreConsole = true;
	/// 		AOutput.Write("test before setting notifications");
	/// 		Task.Run(() => { 1.s(); AOutput.Write("test after"); 1.s(); AOutput.Clear(); 1.s(); AOutput.Write("test after Clear"); });
	/// 
	/// 		Application.Run(new OutputFormExample());
	/// 		_os.Stop();
	/// 	}
	/// }
	/// ]]></code>
	/// </example>
	public unsafe class AOutputServer
	{
		//info:
		//Although global and local servers are implemented quite differently, the interface is almost the same. For this and other reasons I decided to use single class.
		//For local server, the thread and kernel timer would be not necessary. Instead could use just a user timer. But it has some limitations etc.

		readonly System.Collections.Concurrent.ConcurrentQueue<OutServMessage> _messages = new System.Collections.Concurrent.ConcurrentQueue<OutServMessage>(); //all received and still not removed messages that were sent by clients when they call AOutput.Write etc
		Handle_ _mailslot; //used if global
		AWaitableTimer _timer; //used always
		AWnd _notifWnd;
		int _notifMsg;
		bool _isStarted;
		bool _isGlobal;
		bool _isLocalTimer;

		/// <param name="isGlobal">
		/// If true, will receive output from all processes that don't have local server.
		/// </param>
		public AOutputServer(bool isGlobal) => _isGlobal = isGlobal;

		/// <summary>
		/// Starts server.
		/// Returns false if server already exists (if global - in any process).
		/// </summary>
		/// <exception cref="AuException">Failed.</exception>
		public bool Start() {
			lock (this) {
				if (_isGlobal) {
					var m = Api.CreateMailslot(MailslotName_, 0, 0, Api.SECURITY_ATTRIBUTES.ForLowIL);
					if (m.Is0) {
						var e = ALastError.Code;
						if (e == Api.ERROR_ALREADY_EXISTS) return false; //called not first time, or exists in another process
						throw new AuException(e, "*create mailslot");
					}

					_mailslot = m;
					_CreateTimerAndThread();
					_SM->IsServer = 1;
				} else {
					if (AOutput.s_localServer != null) return false;

					_CreateTimerAndThread();
					AOutput.s_localServer = this;
				}

				_isStarted = true;
			}
			return true;
		}

		void _CreateTimerAndThread() {
			try {
				if (_isGlobal) _timer = AWaitableTimer.Create(false, TimerName_);
				else _timer = AWaitableTimer.Create();

				AThread.Start(_Thread, sta: false);
			}
			catch {
				if (_isGlobal) _mailslot.Dispose();
				_timer?.Close(); _timer = null;
				throw;
			}
		}

		/// <summary>
		/// Stops server.
		/// </summary>
		public void Stop() {
			lock (this) {
				if (!_isStarted) return;
				_isStarted = false;
				if (_isGlobal) {
					_mailslot.Dispose();
					_SM->IsServer = 0;
				} else {
					AOutput.s_localServer = null;
				}
				_timer?.Set(0); //break thread loop; use minimal time. //info: the thread will dispose _timer and set=null
			}
		}

		/// <summary>
		/// Calls Stop.
		/// </summary>
		~AOutputServer() => Stop();

		/// <summary>
		/// Sets window/message to be notified about server events.
		/// </summary>
		/// <param name="w">Your window that displays output, or any other window. Its window procedure on <i>message</i> should call <see cref="GetMessage"/> until it returns false. See example in class help.</param>
		/// <param name="message">Windows message to send to <i>w</i> when one or more output events are available. For example WM_USER or WM_APP.</param>
		/// <remarks>
		/// </remarks>
		public void SetNotifications(AWnd w, int message) {
			_notifMsg = message;
			_notifWnd = w;
			if (!w.Is0) _timer?.Set(30);
		}

		void _Thread() {
			try {
				for (int period = 1000; ;) {
					bool isTimerEvent = _timer.WaitOne(period); //true if timer event, false if timeout
					if (isTimerEvent) {
						if (_isGlobal) _SM->IsTimer = 0;
						else _isLocalTimer = false;
					}

					lock (this) {
						if (!_isStarted) {
							_timer.Dispose(); _timer = null;
							break;
						}

						if (_isGlobal) { //read messages from mailslot and add to _messages. Else messages are added directly to _messages.
							while (Api.GetMailslotInfo(_mailslot, null, out var nextSize, out var msgCount) && msgCount > 0) {
								//note: GetMailslotInfo makes Process Hacker show constant 24 B/s I/O total rate. Does not depend on period.
								fixed (byte* b0 = AMemoryArray.Byte_(nextSize + 4)) {
									var b = b0; //+4 for "\r\n"
									bool ok = Api.ReadFile(_mailslot, b, nextSize, out var readSize) && readSize == nextSize;
									if (ok) {
										long time = 0; string s = null, caller = null;
										var mtype = (OutServMessageType)(*b++);
										switch (mtype) {
										case OutServMessageType.Write:
											if (nextSize < 10) { ok = false; break; } //type, time(8), lenCaller
											time = *(long*)b; b += 8;
											int lenCaller = *b++;
											if (lenCaller > 0) {
												if (10 + lenCaller * 2 > nextSize) { ok = false; break; }
												caller = new string((char*)b, 0, lenCaller);
												b += lenCaller * 2;
											}
											int len = (nextSize - (int)(b - b0)) / 2;
											if (!NoNewline) {
												char* p = (char*)(b0 + nextSize);
												p[0] = '\r'; p[1] = '\n';
												len += 2;
											}
											s = new string((char*)b, 0, len);
											break;
										case OutServMessageType.Clear:
											if (nextSize != 1) ok = false;
											break;
										default:
											ok = false;
											break;
										}
										Debug.Assert(ok);
										if (ok) {
											var m = new OutServMessage(mtype, s, time, caller);
											_AddMessage(m);
										}
									}
								}
								if (msgCount == 1) break;
							}
						}
					}

					if (!_notifWnd.Is0 && !_messages.IsEmpty) {

						//AOutput.QM2.Write($"{_messages.Count}, {_ToMB(_memSize)}, {_ToMB(GC.GetTotalMemory(false))}");

						if (!_notifWnd.IsAlive) break;
						_notifWnd.Send(_notifMsg);
					}

					if (isTimerEvent) period = 50; //check after 50 ms, to avoid 1000 ms delay in case a client did not set timer because _SM->IsTimer was still 1 although the timer was already signaled
					else period = 1000; //check every 1000 ms, for full reliability

					//Console.WriteLine($"{period}");
				}
			}
			catch (Exception ex) {
				ADebug.Dialog(ex);
			}
		}

		//static string _ToMB(long n) => Math.Round(n / 1048576d, 3).ToStringInvariant();

		/// <summary>
		/// Adds s directly to _messages and sets timer.
		/// If s is null, it is 'Clear' command.
		/// Else if !NoNewline, appends "\r\n".
		/// Used with local server only.
		/// </summary>
		internal void LocalWrite_(string s, long time = 0, string caller = null) {
			Debug.Assert(!_isGlobal);
			if (!NoNewline && s != null) s += "\r\n";
			var m = new OutServMessage(s == null ? OutServMessageType.Clear : OutServMessageType.Write, s, time, caller);
			_AddMessage(m);
			if (!_isLocalTimer) { _timer?.Set(10); _isLocalTimer = true; }
		}

		void _AddMessage(OutServMessage m) {
			//_memSize += _GetMessageMemorySize(m);
			_messages.Enqueue(m);
		}

		//static int _GetMessageMemorySize(OutServMessage m) => 50 + m.Text.Lenn() * 2;
		//int _memSize;

		/// <summary>
		/// Gets next message and removes from the queue.
		/// Returns false if there are no messages.
		/// </summary>
		/// <remarks>
		/// Messages are added to an internal queue when clients call <see cref="AOutput.Write"/> etc. They contain the text, time, etc. This function gets the oldest message and removes it from the queue.
		/// </remarks>
		public bool GetMessage(out OutServMessage m) {
			if (!_messages.TryDequeue(out m)) return false;
			//_memSize -= _GetMessageMemorySize(m);
			return true;
		}

		/// <summary>
		/// Gets the count of messages in the queue.
		/// </summary>
		public int MessageCount => _messages.Count;

		/// <summary>
		/// Let messages don't end with "\r\n".
		/// </summary>
		/// <remarks>
		/// This can be used for performance, to avoid string copying when using local server. Does not affect performance of global server.
		/// </remarks>
		public bool NoNewline { get; set; }

#if NEED_CALLER
			/// <summary>
			/// Let clients provide the caller method of Write.
			/// Note: It makes these methods much slower, especially when thread stack is big. Also generates much garbage. To find caller method is used <see cref="StackTrace"/> class.
			/// See also: <see cref="IntroduceWriterClass"/>.
			/// </summary>
			public bool NeedCallerMethod
			{
				get => _isGlobal ? (_SM->NeedCaller != 0) : _localNeedCaller;
				set { if(_isGlobal) _SM->NeedCaller = (byte)(value ? 1 : 0); else _localNeedCaller = value; }
			}
			bool _localNeedCaller;

			internal static bool NeedCallerMethod_
			{
				get { var t = s_localServer; return (t != null) ? t.NeedCallerMethod : _SM->NeedCaller != 0; }
			}
#endif

		/// <summary>
		/// Gets mailslot name like <c>@"\\.\mailslot\Au.AOutput\" + sessionId</c>.
		/// </summary>
		internal static string MailslotName_ {
			get {
				if (_mailslotName == null) {
					_mailslotName = @"\\.\mailslot\Au.AOutput\" + AProcess.ProcessSessionId.ToString();
				}
				return _mailslotName;
			}
		}
		static string _mailslotName;

		/// <summary>
		/// Gets waitable timer name like "timer.Au.AOutput".
		/// </summary>
		internal static string TimerName_ => "timer.Au.AOutput";

		/// <summary>
		/// Shared memory variables. Used with global server only.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Size = 16)] //note: this struct is in shared memory. Size must be same in all library versions.
		internal struct SharedMemoryData_
		{
			public byte IsServer, IsTimer;
#if NEED_CALLER
				public byte NeedCaller;
#endif
		}
		internal static SharedMemoryData_* _SM => &SharedMemory_.Ptr->outp;
	}
}

namespace Au
{
	using Au.Util;

	public static partial class AOutput
	{
		[MethodImpl(MethodImplOptions.NoInlining)] //for stack trace
		static void _WriteToOutputServer(string s) {
			Debug.Assert(s != null);

			Api.GetSystemTimeAsFileTime(out var time);

			string caller = ATask.Name;
#if NEED_CALLER
			if(AOutputServer.NeedCallerMethod_) {
				//info: this func always called from WriteDirectly, which is usually called through Writer, Write. But it is public and can be called directly.
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
				//info: here we don't optimize caller strings like AOutputServer does, because StackTrace creates much more garbage.
			}
#endif

			var loc = s_localServer;
			if (loc != null) loc.LocalWrite_(s, time, caller);
			else s_client.WriteLine(s, time, caller);
		}

		static void _ClearToOutputServer() {
			var loc = s_localServer;
			if (loc != null) loc.LocalWrite_(null);
			else s_client.Clear();
		}

		static readonly _ClientOfGlobalServer s_client = new _ClientOfGlobalServer();
		internal static AOutputServer s_localServer; //null if we don't have a local server

		unsafe class _ClientOfGlobalServer
		{
			//info: the mailslot/timer are implicitly disposed when process ends.

			Handle_ _mailslot;
			AWaitableTimer _timer;
			long _sizeWritten;

			public void WriteLine(string s, long time, string caller) {
				lock (_lockObj1) {
					if (!_Connect()) return;

					int lenS = s.Length, lenCaller = (caller != null) ? Math.Min(caller.Length, 255) : 0;
					int lenAll = 1 + 8 + 1 + lenCaller * 2 + lenS * 2; //type, time, lenCaller, caller, s
					bool ok;
					fixed (byte* b = AMemoryArray.Byte_(lenAll)) {
						b[0] = (byte)OutServMessageType.Write; //type
						*(long*)(b + 1) = time; //time
						b[9] = (byte)lenCaller; if (lenCaller != 0) fixed (char* p = caller) Api.memcpy(b + 10, p, lenCaller * 2); //caller
						if (lenS != 0) fixed (char* p = s) Api.memcpy(b + 10 + lenCaller * 2, p, lenS * 2); //s

						g1:
						ok = Api.WriteFile(_mailslot, b, lenAll, out _);
						if (!ok && _ReopenMailslot()) goto g1;
					}

					if (ok) {
						_SetTimer();

						//prevent overflow of mailslot and _messages
						_sizeWritten += lenAll;
						if (_sizeWritten > 1_000_000) {
							while (Api.GetFileSizeEx(_mailslot, out _sizeWritten) && _sizeWritten > 300_000) Thread.Sleep(15);
							//note: these numbers are carefully adjusted for best performance etc
						}
					}
				}
			}

			public void Clear() {
				lock (_lockObj1) {
					if (!_Connect()) return;

					g1:
					byte b = (byte)OutServMessageType.Clear;
					bool ok = Api.WriteFile(_mailslot, &b, 1, out _);
					if (!ok && _ReopenMailslot()) goto g1;
					Debug.Assert(ok);

					if (ok) _SetTimer();
				}
			}

			//If last error says that server's mailslot closed, closes client's mailsot/timer and tries to reopen. If reopened, returns true.
			bool _ReopenMailslot() {
				if (ALastError.Code == Api.ERROR_HANDLE_EOF) { //server's mailslot closed
					_Close();
					if (_Connect()) return true;
				} else {
					Debug.Assert(false);
				}
				return false;
			}

			void _SetTimer() {
				if (AOutputServer._SM->IsTimer == 0) {
					if (_timer.Set(10)) AOutputServer._SM->IsTimer = 1;
				}
			}

			void _Close() {
				if (!_mailslot.Is0) {
					_mailslot.Dispose();
					_timer.Close(); _timer = null;
				}
			}

			bool _Connect() {
				if (AOutputServer._SM->IsServer == 0) {
					_Close();
					return false;
				}

				if (_mailslot.Is0) {
					_mailslot = CreateFile_(AOutputServer.MailslotName_, true);
					if (_mailslot.Is0) return false;

					_timer = AWaitableTimer.Open(AOutputServer.TimerName_, noException: true);
					if (_timer == null) {
						_mailslot.Dispose();
						return false;
					}
				}

				return true;
			}
		}

#if NEED_CALLER
		static readonly List<Type> _writerTypes = new List<Type>() { typeof(AOutput), typeof(_OutputWriter) };

		/// <summary>
		/// Introduces a class that contain methods designed to write to the output.
		/// Purpose - when server's <see cref="AOutputServer.NeedCallerMethod"/> is true, skip methods of this class when searching for the caller method in the call stack.
		/// For example, if you created class PrintColored that contains methods PrintRed, PrintGreen and PrintBlue, you should execute this code in its static constructor: <c>AOutput.IntroduceWriterClass(typeof(PrintColored));</c>.
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
	}
}

namespace Au.Types
{
	/// <summary>
	/// See <see cref="OutServMessage.Type"/>.
	/// </summary>
	public enum OutServMessageType
	{
		/// <summary>
		/// Add line to the output window.
		/// All <see cref="OutServMessage"/> members can be used.
		/// </summary>
		Write,

		/// <summary>
		/// Clear the output window.
		/// Only <see cref="OutServMessage.Type"/> is used.
		/// </summary>
		Clear,
	}

	/// <summary>
	/// Contains message text and/or related info.
	/// More info: <see cref="AOutputServer"/>, <see cref="AOutputServer.GetMessage"/>.
	/// </summary>
	public class OutServMessage
	{
		/// <summary>
		/// Message type.
		/// Currently there are 2 types - Write and Clear.
		/// </summary>
		public OutServMessageType Type { get; }

		/// <summary>
		/// Message text.
		/// Used with OutServMessageType.Write.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Message time in FILETIME format, UTC.
		/// Used with OutServMessageType.Write.
		/// To convert to string: <c>DateTime.FromFileTimeUtc(m.TimeUtc).ToLocalTime().ToString()</c>.
		/// </summary>
		public long TimeUtc { get; }

#if NEED_CALLER
			/// <summary>
			/// The <see cref="ATask.Name"/> property value of the process that called <see cref="AOutput.Write"/>.
			/// Used with OutServMessageType.Write.
			/// If <see cref="NeedCallerMethod"/> is true, also includes the caller method. Format: "scriptname:type.method".
			/// </summary>
			public string Caller { get; }

			internal OutServMessage(OutServMessageType type, string text, long time, string caller)
			{
				Type = type;
				Text = text;
				TimeUtc = time;
				Caller = caller;
			}
#else
		/// <summary>
		/// The <see cref="ATask.Name"/> property value of the process that called <see cref="AOutput.Write"/>.
		/// Used with OutServMessageType.Write.
		/// </summary>
		public string Caller { get; }

		internal OutServMessage(OutServMessageType type, string text, long time, string caller) {
			Type = type;
			Text = text;
			TimeUtc = time;
			Caller = caller;
		}
#endif
		///
		public override string ToString() {
			//in editor used for output history

			if (Type != OutServMessageType.Write) return "";
			var k = DateTime.FromFileTimeUtc(TimeUtc).ToLocalTime();
			return $"{k.ToString()}  |  {Caller}\r\n{Text}";
		}
	}
}
