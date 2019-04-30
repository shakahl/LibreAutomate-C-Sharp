//#define USE_WTS

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
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Process functions. Extends <see cref="Process"/>.
	/// </summary>
	public static unsafe partial class Process_
	{
		/// <summary>
		/// Gets process executable file name (like "notepad.exe") or full path.
		/// Returns null if fails.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <param name="fullPath">
		/// Get full path.
		/// Note: Fails to get full path if the process belongs to another user session, unless current process is running as administrator; also fails to get full path of some system processes.
		/// </param>
		/// <param name="noSlowAPI">When the fast API QueryFullProcessImageName fails, don't try to use another much slower API WTSEnumerateProcesses. Not used if *fullPath* is true.</param>
		/// <remarks>
		/// This function is much slower than getting window name or class name.
		/// </remarks>
		/// <seealso cref="Wnd.ProgramName"/>
		/// <seealso cref="Wnd.ProgramPath"/>
		/// <seealso cref="Wnd.ProcessId"/>
		public static string GetName(int processId, bool fullPath = false, bool noSlowAPI = false)
		{
			if(processId == 0) return null;
			string R = null;

			//var t = Time.PerfMicroseconds;
			//if(s_time != 0) Print(t - s_time);
			//s_time = t;

			using(var ph = LibHandle.OpenProcess(processId)) {
				if(!ph.Is0) {
					//In non-admin process fails if the process is of another user session.
					//Also fails for some system processes: nvvsvc, nvxdsync, dwm. For dwm fails even in admin process.

					//getting native path is faster, but it gets like "\Device\HarddiskVolume5\Windows\System32\notepad.exe" and I don't know API to convert to normal
					if(_QueryFullProcessImageName(ph, !fullPath, out var s)) {
						R = s;
						if(Path_.LibIsPossiblyDos(R)) {
							if(fullPath || _QueryFullProcessImageName(ph, false, out s)) {
								R = Path_.LibExpandDosPath(s);
								if(!fullPath) R = _GetFileName(R);
							}
						}
					}
				} else if(!noSlowAPI && !fullPath) { //the slow way. Can get only names, not paths.
					using(new _AllProcesses(out var p, out int n)) {
						for(int i = 0; i < n; i++)
							if(p[i].processID == processId) {
								R = p[i].GetName(cannotOpen: true);
								break;
							}
					}
				}
			}

			return R;

			//Would be good to cache process names here. But process id can be reused quickly. Use LibGetNameCached instead.
			//	tested: a process id is reused after creating ~100 processes (and waiting until exits). It takes ~2 s.
			//	The window finder is optimized to call this once for each process and not for each window.
		}

		/// <summary>
		/// Same as GetName, but faster when called several times for same window, like <c>if(w.ProgramName=="A" || w.ProgramName=="B")</c>.
		/// </summary>
		internal static string LibGetNameCached(Wnd w, int processId, bool fullPath = false)
		{
			if(processId == 0) return null;
			var cache = _LastWndProps.OfThread;
			cache.Begin(w);
			var R = fullPath ? cache.ProgramPath : cache.ProgramName;
			if(R == null) {
				R = GetName(processId, fullPath);
				if(fullPath) cache.ProgramPath = R; else cache.ProgramName = R;
			}
			return R;
		}

		class _LastWndProps
		{
			Wnd _w;
			long _time;
			internal string ProgramName, ProgramPath;

			internal void Begin(Wnd w)
			{
				var t = Api.GetTickCount64();
				if(w != _w || t - _time > 300) { _w = w; ProgramName = ProgramPath = null; }
				_time = t;
			}

			[ThreadStatic] static _LastWndProps _ofThread;
			internal static _LastWndProps OfThread => _ofThread ?? (_ofThread = new _LastWndProps());
		}

		static bool _QueryFullProcessImageName(IntPtr hProcess, bool getFilename, out string s)
		{
			s = null;
			for(int na = 300; ; na *= 2) {
				var b = Util.Buffers.LibChar(ref na);
				if(Api.QueryFullProcessImageName(hProcess, getFilename, b, ref na)) {
					if(getFilename) s = _GetFileName(b, na);
					else s = b.LibToStringCached(na);
					return true;
				}
				if(WinError.Code != Api.ERROR_INSUFFICIENT_BUFFER) return false;
			}
		}

#if USE_WTS //simple, safe, but ~2 times slower
		struct _AllProcesses :IDisposable
		{
			LibProcessInfo* _p;

			public _AllProcesses(out LibProcessInfo* p, out int count)
			{
				if(WTSEnumerateProcessesW(default, 0, 1, out p, out count)) _p = p; else _p = null;
			}

			public void Dispose()
			{
				if(_p != null) WTSFreeMemory(_p);
			}

			[DllImport("wtsapi32.dll", SetLastError = true)]
			static extern bool WTSEnumerateProcessesW(IntPtr serverHandle, uint reserved, uint version, out LibProcessInfo* ppProcessInfo, out int pCount);

			[DllImport("wtsapi32.dll", SetLastError = false)]
			static extern void WTSFreeMemory(LibProcessInfo* memory);
		}
#else //the .NET Process class uses this. But it creates about 0.4 MB of garbage.
		struct _AllProcesses : IDisposable
		{
			LibProcessInfo* _p;

			public _AllProcesses(out LibProcessInfo* pi, out int count)
			{
				_p = null;
				Api.SYSTEM_PROCESS_INFORMATION* b = null;
				try {
					for(int na = 300_000; ;) {
						b = (Api.SYSTEM_PROCESS_INFORMATION*)Util.NativeHeap.Alloc(na);

						int status = Api.NtQuerySystemInformation(5, b, na, out na);
						//Print(na); //eg 224000

						if(status == 0) break;
						if(status != Api.STATUS_INFO_LENGTH_MISMATCH) throw new AuException(status);
						var t = b; b = null; Util.NativeHeap.Free(t);
					}

					Api.SYSTEM_PROCESS_INFORMATION* p;
					int nProcesses = 0, nbNames = 0;
					for(p = b; p->NextEntryOffset != 0; p = (Api.SYSTEM_PROCESS_INFORMATION*)((byte*)p + p->NextEntryOffset)) {
						nProcesses++;
						nbNames += p->NameLength; //bytes, not chars
					}
					count = nProcesses;
					_p = (LibProcessInfo*)Util.NativeHeap.Alloc(nProcesses * sizeof(LibProcessInfo) + nbNames);
					LibProcessInfo* r = _p;
					char* names = (char*)(_p + nProcesses);
					for(p = b; p->NextEntryOffset != 0; p = (Api.SYSTEM_PROCESS_INFORMATION*)((byte*)p + p->NextEntryOffset), r++) {
						r->processID = (int)p->UniqueProcessId;
						r->sessionID = (int)p->SessionId;
						int len = p->NameLength / 2;
						r->nameLen = len;
						if(len > 0) {
							//copy name to _p memory because it's in the huge buffer that will be released in this func
							r->namePtr = names;
							Api.memcpy(names, (char*)p->NamePtr, len * 2);
							names += len;
						} else r->namePtr = null; //Idle
					}
					pi = _p;
				}
				finally { Util.NativeHeap.Free(b); }
			}

			public void Dispose()
			{
				Util.NativeHeap.Free(_p);
			}
		}
#endif

		//Use LibProcessInfo and ProcessInfo because with WTSEnumerateProcessesW _ProcessName must be IntPtr, and then WTSFreeMemory frees its memory.
		//	AllProcesses() converts LibProcessInfo to ProcessInfo where Name is string. Almost same speed.
		internal unsafe struct LibProcessInfo
		{
#pragma warning disable 649 //never used
			public int sessionID;
			public int processID;
			public char* namePtr;
			public int nameLen;
#if USE_WTS
			public IntPtr userSid;
#endif
#pragma warning restore 649

			/// <summary>
			/// Gets process executable file name (like "notepad.exe"). Not full path.
			/// If contains looks like a DOS path and !cannotOpen, tries to unexpand DOS path.
			/// Don't call multiple times, because always converts from raw char*.
			/// </summary>
			public string GetName(bool cannotOpen = false)
			{
				if(namePtr == null) {
					if(processID == 0) return "Idle";
					return null;
				}
				string R = Util.StringCache.LibAdd(namePtr, nameLen);
				if(!cannotOpen && Path_.LibIsPossiblyDos(R)) {
					using(var ph = LibHandle.OpenProcess(processID)) {
						if(!ph.Is0 && _QueryFullProcessImageName(ph, false, out var s)) {
							R = _GetFileName(Path_.LibExpandDosPath(s));
						}
					}
				}
				return R;
			}
		}

		/// <summary>
		/// Gets basic info of all processes: name, id, session id.
		/// </summary>
		/// <param name="ofThisSession">Get processes only of this user session (skip services etc).</param>
		/// <exception cref="AuException">Failed. Unlikely.</exception>
		public static ProcessInfo[] AllProcesses(bool ofThisSession = false)
		{
			using(new _AllProcesses(out var p, out int n)) {
				if(n == 0) throw new AuException();
				int sessionId = 0, ns = n;
				if(ofThisSession) {
					sessionId = CurrentSessionId;
					for(int i = 0; i < n; i++) if(p[i].sessionID != sessionId) ns--;
				}
				var a = new ProcessInfo[ns];
				for(int i = 0, j = 0; i < n; i++) {
					if(ofThisSession && p[i].sessionID != sessionId) continue;
					a[j++] = new ProcessInfo(p[i].sessionID, p[i].processID, p[i].GetName());
				}
				return a;
			}
		}

		/// <summary>
		/// Gets process ids of all processes of the specified program.
		/// Returns array containing 0 or more elements.
		/// </summary>
		/// <param name="processName">
		/// Process executable file name, like "notepad.exe".
		/// String format: [](xref:wildcard_expression).
		/// </param>
		/// <param name="fullPath">
		/// *processName* is full path.
		/// Note: Fails to get full path if the process belongs to another user session, unless current process is running as administrator; also fails to get full path of some system processes.
		/// </param>
		/// <param name="ofThisSession">Get processes only of this user session.</param>
		/// <exception cref="ArgumentException">
		/// - *processName* is "" or null.
		/// - Invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// </exception>
		public static int[] GetProcessIds(string processName, bool fullPath = false, bool ofThisSession = false)
		{
			if(Empty(processName)) throw new ArgumentException();
			List<int> a = null;
			LibGetProcessesByName(ref a, processName, fullPath, ofThisSession);
			return a?.ToArray() ?? Array.Empty<int>();
		}

		/// <summary>
		/// Gets process id of the first found process of the specified program.
		/// Returns 0 if not found.
		/// More info: <see cref="GetProcessIds"/>.
		/// </summary>
		/// <exception cref="ArgumentException"/>
		public static int GetProcessId(string processName, bool fullPath = false, bool ofThisSession = false)
		{
			if(Empty(processName)) throw new ArgumentException();
			List<int> a = null;
			return LibGetProcessesByName(ref a, processName, fullPath, ofThisSession, true);
		}

		internal static int LibGetProcessesByName(ref List<int> a, Wildex processName, bool fullPath = false, bool ofThisSession = false, bool first = false)
		{
			a?.Clear();

			int sessionId = ofThisSession ? CurrentSessionId : 0;

			using(new _AllProcesses(out var p, out int n)) {
				for(int i = 0; i < n; i++) {
					if(ofThisSession && p[i].sessionID != sessionId) continue;
					string s = fullPath ? GetName(p[i].processID, true) : p[i].GetName();
					if(s == null) continue;

					if(processName.Match(s)) {
						if(first) return p[i].processID;
						if(a == null) a = new List<int>();
						a.Add(p[i].processID);
					}
				}
			}

			return 0;
		}

		static string _GetFileName(char* s, int len)
		{
			if(s == null) return null;
			char* ss = s + len;
			for(; ss > s; ss--) if(ss[-1] == '\\' || ss[-1] == '/') break;
			return Util.StringCache.LibAdd(ss, len - (int)(ss - s));
		}

		static string _GetFileName(string s)
		{
			fixed (char* p = s) return _GetFileName(p, s.Length);
		}

		static string _GetFileName(char[] s, int len)
		{
			fixed (char* p = s) return _GetFileName(p, len);
		}

		/// <summary>
		/// Calls API <msdn>GetCurrentProcessId</msdn>.
		/// </summary>
		public static int CurrentProcessId => Api.GetCurrentProcessId();

		/// <summary>
		/// Returns current process handle.
		/// Calls API <msdn>GetCurrentProcess</msdn>.
		/// Don't need to close the handle.
		/// </summary>
		public static IntPtr CurrentProcessHandle => Api.GetCurrentProcess();

		/// <summary>
		/// Gets process id from handle.
		/// Returns 0 if failed. Supports <see cref="WinError"/>.
		/// Calls API <msdn>GetProcessId</msdn>.
		/// </summary>
		/// <param name="processHandle">Process handle.</param>
		public static int ProcessIdFromHandle(IntPtr processHandle)
		{
			return Api.GetProcessId(processHandle);
			//speed: 250 ns
		}

		//public static Process GetProcessObject(IntPtr processHandle)
		//{
		//	int pid = GetProcessId(processHandle);
		//	if(pid == 0) return null;
		//	return Process.GetProcessById(pid); //slow, makes much garbage, at first gets all processes just to throw exception if pid not found...
		//}

		/// <summary>
		/// Gets user session id of process.
		/// Returns -1 if failed. Supports <see cref="WinError"/>.
		/// Calls API <msdn>ProcessIdToSessionId</msdn>.
		/// </summary>
		/// <param name="processId">Process id.</param>
		public static int GetSessionId(int processId)
		{
			if(!Api.ProcessIdToSessionId(processId, out var R)) return -1;
			return R;
		}

		/// <summary>
		/// Gets user session id of this process.
		/// Calls API <msdn>ProcessIdToSessionId</msdn> and <msdn>GetCurrentProcessId</msdn>.
		/// </summary>
		public static int CurrentSessionId => GetSessionId(Api.GetCurrentProcessId());

		/// <summary>
		/// Gets version info of process executable file.
		/// Return null if fails.
		/// </summary>
		/// <param name="processId">Process id.</param>
		public static FileVersionInfo GetVersionInfo(int processId)
		{
			var s = GetName(processId, true);
			if(s != null) {
				try { return FileVersionInfo.GetVersionInfo(s); } catch { }
			}
			return null;
		}

		/// <summary>
		/// Gets description of process executable file.
		/// Return null if fails.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <remarks>
		/// Calls <see cref="GetVersionInfo"/> and <see cref="FileVersionInfo.FileDescription"/>.
		/// </remarks>
		public static string GetDescription(int processId) => GetVersionInfo(processId)?.FileDescription;
	}
}

namespace Au.Types
{
	/// <summary>
	/// Contains process id, name and session id.
	/// </summary>
	public struct ProcessInfo
	{
		/// <summary>User session id.</summary>
		public int SessionId;

		/// <summary>Process id.</summary>
		public int ProcessId;

		/// <summary>Executable file name, like "notepad.exe".</summary>
		public string Name;

		//public IntPtr UserSid; //where is its memory?

		///
		public ProcessInfo(int session, int pid, string name)
		{
			SessionId = session; ProcessId = pid; Name = name;
		}

		///
		public override string ToString() => Name;
	}
}
