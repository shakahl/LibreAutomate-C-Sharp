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
using System.Security.Principal;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Process functions. Extends <see cref="Process"/>.
	/// </summary>
	public static unsafe class Process_
	{
		/// <summary>
		/// Gets process executable file name without ".exe", or full path.
		/// Returns null if fails.
		/// </summary>
		/// <param name="processId">Process id. If you have a window, use <see cref="Wnd.ProcessId"/>.</param>
		/// <param name="fullPath">Get full path. Note: Fails to get full path if the process belongs to another user session, unless current process is running as administrator; also fails to get full path of some system processes.</param>
		/// <param name="noSlowAPI">When the fast API QueryFullProcessImageName fails, don't try to use another much slower API WTSEnumerateProcesses. Not used if fullPath is true.</param>
		public static string GetProcessName(int processId, bool fullPath = false, bool noSlowAPI = false)
		{
			return _GetProcessName(processId, fullPath, noSlowAPI);
		}

		static string _GetProcessName(int processId, bool fullPath, bool dontEnumerate = false)
		{
			if(processId == 0) return null;
			string R = null;

			using(var ph = LibProcessHandle.FromId(processId)) {
				if(!ph.Is0) {
					//In non-admin process fails if the process is of another user session.
					//Also fails for some system processes: nvvsvc, nvxdsync, dwm. For dwm fails even in admin process.

					//getting native path is faster, but it gets like "\Device\HarddiskVolume5\Windows\System32\notepad.exe" and I don't know API to convert to normal
					if(_QueryFullProcessImageName(ph, !fullPath, out var s)) {
						R = s;
						if(R.IndexOf('~') >= 0) { //DOS path?
							if(fullPath || _QueryFullProcessImageName(ph, false, out s)) {
								R = Path_.LibExpandDosPath(s);
								if(!fullPath) fixed (char* p = R) R = _GetFileNameWithoutExe(p, R.Length);
							}
						}
					}
				} else if(!dontEnumerate && !fullPath) { //the slow way. Can get only names, not paths.
					using(new _AllProcesses(out var p, out int n)) {
						for(int i = 0; i < n; i++)
							if(p[i].processID == processId) {
								R = p[i].ProcessName;
								break;
							}
					}
				}
			}

			return R;

			//Would be good to cache process names. But it's difficult because process id can be reused.
			//	tested: a process id is reused after creating ~100 processes (and waiting until exits). It takes ~2 s.
			//	The window finder is optimized to call this once for each process and not for each window.
		}

		static bool _QueryFullProcessImageName(IntPtr hProcess, bool getFilename, out string s)
		{
			s = null;
			for(int na = 300; ; na *= 2) {
				var b = Util.Buffers.LibChar(ref na);
				if(Api.QueryFullProcessImageName(hProcess, getFilename, b, ref na)) {
					if(getFilename) s = _GetFileNameWithoutExe(b, na);
					else s = b.LibToStringCached(na);
					return true;
				}
				if(Native.GetError() != Api.ERROR_INSUFFICIENT_BUFFER) return false;
			}
		}

#if USE_WTS //simple, safe, but ~2 times slower
		struct _AllProcesses :IDisposable
		{
			ProcessInfoInternal* _p;

			public _AllProcesses(out ProcessInfoInternal* p, out int count)
			{
				if(WTSEnumerateProcessesW(default, 0, 1, out p, out count)) _p = p; else _p = null;
			}

			public void Dispose()
			{
				if(_p != null) WTSFreeMemory(_p);
			}

			[DllImport("wtsapi32.dll", SetLastError = true)]
			static extern bool WTSEnumerateProcessesW(IntPtr serverHandle, uint reserved, uint version, out ProcessInfoInternal* ppProcessInfo, out int pCount);

			[DllImport("wtsapi32.dll", SetLastError = false)]
			static extern void WTSFreeMemory(ProcessInfoInternal* memory);
		}
#else //the .NET Process class uses this. But it creates about 0.4 MB of garbage.
		struct _AllProcesses :IDisposable
		{
			ProcessInfoInternal* _p;

			public _AllProcesses(out ProcessInfoInternal* pi, out int count)
			{
				_p = null;
				SYSTEM_PROCESS_INFORMATION* b = null;
				try {
					for(int na = 300_000; ;) {
						b = (SYSTEM_PROCESS_INFORMATION*)Util.NativeHeap.Alloc(na);

						int status = NtQuerySystemInformation(5, b, na, out na);
						//Print(na); //eg 224000

						if(status == 0) break;
						if(status != STATUS_INFO_LENGTH_MISMATCH) throw new AuException(status);
						var t = b; b = null; Util.NativeHeap.Free(t);
					}

					SYSTEM_PROCESS_INFORMATION* p;
					int nProcesses = 0, nbNames = 0;
					for(p = b; p->NextEntryOffset != 0; p = (SYSTEM_PROCESS_INFORMATION*)((byte*)p + p->NextEntryOffset)) {
						nProcesses++;
						nbNames += p->NameLength; //bytes, not chars
					}
					count = nProcesses;
					_p = (ProcessInfoInternal*)Util.NativeHeap.Alloc(nProcesses * sizeof(ProcessInfoInternal) + nbNames);
					ProcessInfoInternal* r = _p;
					char* names = (char*)(_p + nProcesses);
					for(p = b; p->NextEntryOffset != 0; p = (SYSTEM_PROCESS_INFORMATION*)((byte*)p + p->NextEntryOffset), r++) {
						r->processID = (int)p->UniqueProcessId;
						r->sessionID = (int)p->SessionId;
						int len = p->NameLength / 2;
						r->processNameLen = len;
						if(len > 0) {
							//copy name to _p memory because it's in the huge buffer that will be released in this func
							r->processNamePtr = names;
							Api.memcpy(names, (char*)p->NamePtr, len * 2);
							names += len;
						} else r->processNamePtr = null; //Idle
					}
					pi = _p;
				}
				finally { Util.NativeHeap.Free(b); }
			}

			public void Dispose()
			{
				Util.NativeHeap.Free(_p);
			}

			[DllImport("ntdll.dll")]
			static extern int NtQuerySystemInformation(int five, SYSTEM_PROCESS_INFORMATION* SystemInformation, int SystemInformationLength, out int ReturnLength);

#pragma warning disable 649, 169 //unused fields
			struct SYSTEM_PROCESS_INFORMATION
			{
				internal uint NextEntryOffset;
				internal uint NumberOfThreads;
				long SpareLi1;
				long SpareLi2;
				long SpareLi3;
				internal long CreateTime;
				internal long UserTime;
				internal long KernelTime;

				internal ushort NameLength;   // UNICODE_STRING   
				internal ushort MaximumNameLength;
				internal IntPtr NamePtr;     // This will point into the data block returned by NtQuerySystemInformation

				internal int BasePriority;
				internal IntPtr UniqueProcessId;
				internal IntPtr InheritedFromUniqueProcessId;
				internal uint HandleCount;
				internal uint SessionId;

				//unused members
			}
#pragma warning restore 649, 169

			const int STATUS_INFO_LENGTH_MISMATCH = unchecked((int)0xC0000004);
		}
#endif

		//Use ProcessInfoInternal and ProcessInfo because with WTSEnumerateProcessesW _ProcessName must be IntPtr, and then WTSFreeMemory frees its memory.
		//	GetProcesses() converts ProcessInfoInternal to ProcessInfo where ProcessName is string. Almost same speed.
		internal unsafe struct ProcessInfoInternal
		{
#pragma warning disable 649 //says never used
			public int sessionID;
			public int processID;
			public char* processNamePtr;
#if USE_WTS
			public IntPtr userSid;
#else
			public int processNameLen;
#endif
#pragma warning restore

			/// <summary>
			/// Process executable file name without ".exe". Not full path.
			/// If contains '~', tries to unexpand DOS path.
			/// Don't call multiple times, because always converts from raw char*.
			/// </summary>
			public string ProcessName
			{
				get
				{
					if(processNamePtr == null) {
						if(processID == 0) return "Idle";
						return null;
					}
					string R = _GetFileNameWithoutExe(processNamePtr
#if !USE_WTS
						, processNameLen
#endif
						);
					if(R.IndexOf('~') >= 0) {
						string s = null;
						using(var ph = LibProcessHandle.FromId(processID)) {
							if(!ph.Is0 && _QueryFullProcessImageName(ph, false, out s)) {
								s = Path_.LibExpandDosPath(s);
								fixed (char* p = s) R = _GetFileNameWithoutExe(p, s.Length);
							}
						}
					}
					return R;
				}
			}
		}

		/// <summary>
		/// Contains process id, name and session id.
		/// </summary>
		/// <tocexclude />
		public struct ProcessInfo
		{
			/// <summary>User session id.</summary>
			public int SessionId;

			/// <summary>Process id.</summary>
			public int ProcessId;

			/// <summary>Process executable file name without ".exe". Not full path.</summary>
			public string ProcessName;

			//public IntPtr UserSid; //where is its memory?

			///
			public ProcessInfo(int session, int pid, string name)
			{
				SessionId = session; ProcessId = pid; ProcessName = name;
			}

			///
			public override string ToString()
			{
				return ProcessName;
			}
		}

		/// <summary>
		/// Gets processes array that contains process name, id and session id.
		/// </summary>
		/// <param name="ofThisSession">Get processes only of this user session (skip services etc).</param>
		/// <exception cref="AuException">Failed. Unlikely.</exception>
		public static ProcessInfo[] GetProcesses(bool ofThisSession = false)
		{
			using(new _AllProcesses(out var p, out int n)) {
				if(n == 0) throw new AuException();
				if(ofThisSession) {
					int sessionId = GetSessionId();

					var t = new List<ProcessInfo>(n / 2);
					for(int i = 0; i < n; i++) {
						if(p[i].sessionID != sessionId) continue;
						t.Add(new ProcessInfo(p[i].sessionID, p[i].processID, p[i].ProcessName));
					}
					return t.ToArray();
				} else {
					var a = new ProcessInfo[n];
					for(int i = 0; i < n; i++) {
						a[i] = new ProcessInfo(p[i].sessionID, p[i].processID, p[i].ProcessName);
					}
					return a;
				}
			}
		}

		/// <summary>
		/// Gets array of process id of all processes whose names match processName.
		/// Returns empty array if there are no matching processes.
		/// </summary>
		/// <param name="processName">
		/// Process name.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// </param>
		/// <param name="fullPath">If false, processName is filename without ".exe". If true, processName is full path. Note: Fails to get full path if the process belongs to another user session, unless current process is running as administrator; also fails to get full path of some system processes.</param>
		/// <param name="ofThisSession">Get processes only of this user session (skip services etc).</param>
		/// <exception cref="ArgumentException">
		/// processName is "" or null.
		/// Invalid wildcard expression ("**options " or regular expression).
		/// </exception>
		public static int[] GetProcessesByName(string processName, bool fullPath = false, bool ofThisSession = false)
		{
			if(Empty(processName)) throw new ArgumentException();
			List<int> a = null;
			LibGetProcessesByName(ref a, processName, fullPath, ofThisSession);
			if(a == null) return Array.Empty<int>();
			return a.ToArray();
		}

		internal static void LibGetProcessesByName(ref List<int> a, Wildex processName, bool fullPath = false, bool ofThisSession = false)
		{
			if(a != null) a.Clear();

			int sessionId = ofThisSession ? GetSessionId() : 0;

			using(new _AllProcesses(out var p, out int n)) {
				for(int i = 0; i < n; i++) {
					if(ofThisSession && p[i].sessionID != sessionId) continue;
					string s;
					if(fullPath) {
						s = GetProcessName(p[i].processID, true);
						if(s == null) continue;
					} else s = p[i].ProcessName;

					if(processName.Match(s)) {
						if(a == null) a = new List<int>();
						a.Add(p[i].processID);
					}
				}
			}
		}

		static string _GetFileNameWithoutExe(char[] s, int len = -1)
		{
			fixed (char* p = s) return _GetFileNameWithoutExe(p, len);
		}

		static string _GetFileNameWithoutExe(char* s, int len = -1)
		{
			if(s == null) return null;
			if(len < 0) len = Util.LibCharPtr.Length(s);
			if(Util.LibCharPtr.EndsWith(s, len, ".exe", true)) len -= 4;
			char* ss = s + len;
			for(; ss > s; ss--) if(ss[-1] == '\\' || ss[-1] == '/') break;
			return Util.StringCache.LibAdd(ss, len - (int)(ss - s));
		}

		/// <summary>
		/// Opens and manages a process handle.
		/// Must be disposed.
		/// </summary>
		internal struct LibProcessHandle :IDisposable
		{
			//note: this must be struct, not class, because in some cases used very frequently and would create much garbage.

			IntPtr _h;

			///
			public IntPtr Handle => _h;

			///
			public bool Is0 { get { return _h == default; } }

			///
			public void Dispose()
			{
				if(_h != default) { Api.CloseHandle(_h); _h = default; }
			}

			/// <summary>
			/// Attaches a kernel handle to this new variable.
			/// No exception when handle is invalid.
			/// </summary>
			/// <param name="handle"></param>
			public LibProcessHandle(IntPtr handle) { _h = handle; }

			/// <summary>
			/// Opens process handle.
			/// Calls API OpenProcess.
			/// Returns null if fails. Supports Native.GetError().
			/// </summary>
			/// <param name="processId">Process id.</param>
			/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
			public static LibProcessHandle FromId(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
			{
				_Open(out var h, processId, desiredAccess);
				return new LibProcessHandle(h);
			}

			/// <summary>
			/// Opens window's process handle.
			/// This overload is more powerful: if API OpenProcess fails, it tries GetProcessHandleFromHwnd, which can open higher integrity level processes, but only if current process is uiAccess and desiredAccess includes only PROCESS_DUP_HANDLE, PROCESS_VM_OPERATION, PROCESS_VM_READ, PROCESS_VM_WRITE, SYNCHRONIZE.
			/// Returns null if fails. Supports Native.GetError().
			/// </summary>
			/// <param name="w"></param>
			/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
			public static LibProcessHandle FromWnd(Wnd w, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
			{
				_Open(out var h, w.ProcessId, desiredAccess, w);
				return new LibProcessHandle(h);
			}

			static bool _Open(out IntPtr R, int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION, Wnd processWindow = default)
			{
				R = default;
				int e = 0;
				if(processId != 0) {
					R = Api.OpenProcess(desiredAccess, false, processId);
					if(R != default) return true;
					e = Native.GetError();
				}
				if(!processWindow.Is0) {
					if((desiredAccess & ~(Api.PROCESS_DUP_HANDLE | Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE | Api.SYNCHRONIZE)) == 0
					&& UacInfo.ThisProcess.IsUIAccess
					) R = Api.GetProcessHandleFromHwnd(processWindow);

					if(R != default) return true;
					Api.SetLastError(e);
				}
				return false;
			}

			public static implicit operator IntPtr(LibProcessHandle p) { return p._h; }
		}

		/// <summary>
		/// Process handle that is derived from WaitHandle.
		/// When don't need to wait, use LibProcessHandle, it's more lightweight and has more creation methods.
		/// </summary>
		internal class LibProcessWaitHandle :WaitHandle
		{
			public LibProcessWaitHandle(IntPtr nativeHandle, bool owndHandle = true)
			{
				base.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(nativeHandle, owndHandle);
			}
		}

		/// <summary>
		/// Allocates, writes and reads memory in other process.
		/// </summary>
		/// <remarks>
		/// Objects of this class must be disposed. Example: <c>using(var pm=new Process_.Memory(...)) { ... }</c>.
		/// </remarks>
		public sealed unsafe class Memory :IDisposable
		{
			LibProcessHandle _hproc;
			HandleRef _HprocHR => new HandleRef(this, _hproc);

			///
			~Memory() { _Dispose(); }

			///
			public void Dispose()
			{
				_Dispose();
				GC.SuppressFinalize(this);
			}

			void _Dispose()
			{
				if(_hproc.Is0) return;
				if(Mem != default) {
					var mem = Mem; Mem = default;
					if(!_doNotFree) {
						if(!Api.VirtualFreeEx(_HprocHR, mem)) PrintWarning("Failed to free process memory. " + Native.GetErrorMessage());
					}
				}
				_hproc.Dispose();
			}

			/// <summary>
			/// Process handle.
			/// Opened with access PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE.
			/// </summary>
			public IntPtr ProcessHandle => _hproc;

			/// <summary>
			/// Address of memory allocated in that process.
			/// </summary>
			/// <remarks>
			/// The address is invalid in this process.
			/// </remarks>
			public IntPtr Mem { get; private set; }

			/// <summary>
			/// Sets an address of memory in that process that is to be used by the read and write functions.
			/// </summary>
			/// <param name="mem">A memory address in that process.</param>
			/// <param name="freeWhenDisposing">
			/// Let the Dispose method (or finalizer) call API <msdn>VirtualFreeEx</msdn> to free mem. The memory must be allocated with API <msdn>VirtualAllocEx</msdn> (by any process) or <msdn>VirtualAlloc</msdn> (by that process).
			/// If false, mem can be any memory in that process, and this variable will not free it. Alternatively you can use <see cref="ReadOther"/> and <see cref="WriteOther"/>.</param>
			/// <exception cref="InvalidOperationException">This variable already has Mem, unless it was set by this function with <paramref name="freeWhenDisposing"/> = false.</exception>
			/// <remarks>
			/// This function can be used if this variable was created with <i>nBytes</i> = 0. Else exception. Also exception if this function previously called with <paramref name="freeWhenDisposing"/> = true.
			/// </remarks>
			public void SetMem(IntPtr mem, bool freeWhenDisposing)
			{
				if(Mem != default && !_doNotFree) throw new InvalidOperationException();
				_doNotFree = !freeWhenDisposing;
				Mem = mem;
			}
			bool _doNotFree;

			void _Alloc(int pid, Wnd w, int nBytes)
			{
				string err = null;
				const uint fl = Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE;
				_hproc = w.Is0 ? LibProcessHandle.FromId(pid, fl) : LibProcessHandle.FromWnd(w, fl);
				if(_hproc.Is0) { err = "Failed to open process handle."; goto ge; }

				if(nBytes != 0) {
					Mem = Api.VirtualAllocEx(_HprocHR, default, nBytes);
					if(Mem == default) { err = "Failed to allocate process memory."; goto ge; }
				}
				return;
				ge:
				var e = new AuException(0, err);
				_Dispose();
				throw e;
			}

			/// <summary>
			/// Opens window's process handle and optionally allocates memory in that process.
			/// </summary>
			/// <param name="w">A window in that process.</param>
			/// <param name="nBytes">If not 0, allocates this number of bytes of memory in that process.</param>
			/// <remarks>This is the preferred constructor when the process has windows. It works with windows of <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> High integrity level when this process is Medium+uiAccess.</remarks>
			/// <exception cref="WndException">w invalid.</exception>
			/// <exception cref="AuException">Failed to open process handle (usually because of UAC) or allocate memory.</exception>
			public Memory(Wnd w, int nBytes)
			{
				w.ThrowIfInvalid();
				_Alloc(0, w, nBytes);
			}

			/// <summary>
			/// Opens window's process handle and optionally allocates memory in that process.
			/// </summary>
			/// <param name="processId">Process id.</param>
			/// <param name="nBytes">If not 0, allocates this number of bytes of memory in that process.</param>
			/// <exception cref="AuException">Failed to open process handle (usually because of <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink>) or allocate memory.</exception>
			public Memory(int processId, int nBytes)
			{
				_Alloc(processId, default, nBytes);
			}

			/// <summary>
			/// Copies a string from this process to the memory allocated in that process by the constructor.
			/// In that process the string is written as '\0'-terminated UTF-16 string. For it is used (s.Length+1)*2 bytes of memory in that process (+1 for the '\0', *2 because UTF-16 character size is 2 bytes).
			/// Returns false if fails.
			/// </summary>
			/// <param name="s">A string in this process.</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			public bool WriteUnicodeString(string s, int offsetBytes = 0)
			{
				if(Mem == default) return false;
				if(Empty(s)) return true;
				fixed (char* p = s) {
					return Api.WriteProcessMemory(_HprocHR, Mem + offsetBytes, p, (s.Length + 1) * 2, null);
				}
			}

			/// <summary>
			/// Copies a string from this process to the memory allocated in that process by the constructor.
			/// In that process the string is written as '\0'-terminated ANSI string, in default or specified encoding.
			/// Returns false if fails.
			/// </summary>
			/// <param name="s">A string in this process. Normal C# string (UTF-16), not ANSI.</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			/// <param name="enc">If null, uses system's default ANSI encoding.</param>
			public bool WriteAnsiString(string s, int offsetBytes = 0, Encoding enc = null)
			{
				if(Mem == default) return false;
				if(Empty(s)) return true;
				if(enc == null) enc = Encoding.Default;
				var a = enc.GetBytes(s + "\0");
				fixed (byte* p = a) {
					return Api.WriteProcessMemory(_HprocHR, Mem + offsetBytes, p, a.Length, null);
				}
			}

			string _ReadString(bool ansiString, int nChars, int offsetBytes, bool findLength, Encoding enc = null, bool cache = false)
			{
				if(Mem == default) return null;
				int na = nChars; if(!ansiString) na *= 2;
				var b = Util.Buffers.LibChar((na + 1) / 2);
				fixed (char* p = b.A) {
					if(!Api.ReadProcessMemory(_HprocHR, Mem + offsetBytes, p, na, null)) return null;
					if(findLength) {
						if(ansiString) nChars = Util.LibCharPtr.Length((byte*)p, nChars);
						else nChars = Util.LibCharPtr.Length(p, nChars);
					}
				}
				if(ansiString) return b.LibToStringFromAnsi(nChars, enc);
				if(cache) return b.LibToStringCached(nChars);
				return b.ToString(nChars);
			}

			/// <summary>
			/// Copies a string from the memory in that process allocated by the constructor to this process.
			/// Returns the copied string, or null if fails.
			/// In that process the string must be in Unicode UTF-16 format (ie not ANSI).
			/// </summary>
			/// <param name="nChars">Number of characters to copy. In both processes a character is 2 bytes.</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			/// <param name="findLength">Find true string length by searching for '\0' character in nChars range. If false, the returned string is of nChars length even if contains '\0' characters.</param>
			public string ReadUnicodeString(int nChars, int offsetBytes = 0, bool findLength = false)
			{
				return _ReadString(false, nChars, offsetBytes, findLength);
			}

			/// <summary>
			/// The same as <see cref="ReadUnicodeString"/> but uses our StringCache.
			/// </summary>
			internal string LibReadUnicodeStringCached(int nChars, int offsetBytes = 0, bool findLength = false)
			{
				return _ReadString(false, nChars, offsetBytes, findLength, cache: true);
			}

			/// <summary>
			/// Copies a string from the memory in that process allocated by the constructor to this process.
			/// Returns the copies string, or null if fails.
			/// In that process the string must be in ANSI format (ie not Unicode UTF-16).
			/// </summary>
			/// <param name="nBytes">Number bytes to copy. In that process a character is 1 or more bytes (depending on encoding). In this process will be 2 bytes (normal C# string).</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			/// <param name="findLength">Find true string length by searching for '\0' character in nBytes range of the ANSI string.</param>
			/// <param name="enc">If null, uses system's default ANSI encoding.</param>
			public string ReadAnsiString(int nBytes, int offsetBytes = 0, bool findLength = false, Encoding enc = null)
			{
				return _ReadString(true, nBytes, offsetBytes, findLength, enc);
			}

			/// <summary>
			/// Copies a value-type variable or other memory from this process to the memory in that process allocated by the constructor.
			/// Returns false if fails.
			/// </summary>
			/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
			/// <param name="nBytes">Number of bytes to copy.</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			public bool Write(void* ptr, int nBytes, int offsetBytes = 0)
			{
				if(Mem == default) return false;
				return Api.WriteProcessMemory(_HprocHR, Mem + offsetBytes, ptr, nBytes, null);
			}

			/// <summary>
			/// Copies a value-type variable or other memory from this process to a known memory address in that process.
			/// Returns false if fails.
			/// </summary>
			/// <param name="ptrDestinationInThatProcess">Memory address in that process where to copy memory from this process.</param>
			/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
			/// <param name="nBytes">Number of bytes to copy.</param>
			/// <seealso cref="SetMem"/>
			public bool WriteOther(IntPtr ptrDestinationInThatProcess, void* ptr, int nBytes)
			{
				return Api.WriteProcessMemory(_HprocHR, ptrDestinationInThatProcess, ptr, nBytes, null);
			}

			/// <summary>
			/// Copies from the memory in that process allocated by the constructor to a value-type variable or other memory in this process.
			/// Returns false if fails.
			/// </summary>
			/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
			/// <param name="nBytes">Number of bytes to copy.</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			public bool Read(void* ptr, int nBytes, int offsetBytes = 0)
			{
				if(Mem == default) return false;
				return Api.ReadProcessMemory(_HprocHR, Mem + offsetBytes, ptr, nBytes, null);
			}

			/// <summary>
			/// Copies from a known memory address in that process to a value-type variable or other memory in this process.
			/// Returns false if fails.
			/// </summary>
			/// <param name="ptrSourceInThatProcess">Memory address in that process from where to copy memory.</param>
			/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
			/// <param name="nBytes">Number of bytes to copy.</param>
			/// <seealso cref="SetMem"/>
			public bool ReadOther(IntPtr ptrSourceInThatProcess, void* ptr, int nBytes)
			{
				return Api.ReadProcessMemory(_HprocHR, ptrSourceInThatProcess, ptr, nBytes, null);
			}

			//Cannot get pointer if generic type. Could try Marshal.StructureToPtr etc but I don't like it. Didn't test Unsafe.
			//public bool Write<T>(ref T v, int offsetBytes = 0) where T : struct
			//{
			//	int n = Marshal.SizeOf(v.GetType());
			//	return Write(&v, n, offsetBytes);
			//	Marshal.StructureToPtr(v, m, false); ...
			//}
		}

		/// <summary>
		/// Holds an access token (security info) of a process and provides various security info, eg <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level.
		/// </summary>
		public sealed class UacInfo :IDisposable
		{
			#region IDisposable Support

			void _Dispose()
			{
				if(_htoken != default) { Api.CloseHandle(_htoken); _htoken = default; }
			}

			///
			~UacInfo() { _Dispose(); }

			///
			public void Dispose()
			{
				_Dispose();
				GC.SuppressFinalize(this);
			}
			#endregion

			IntPtr _htoken;
			HandleRef _HtokenHR => new HandleRef(this, _htoken);

			/// <summary>
			/// The access token handle.
			/// </summary>
			/// <remarks>
			/// The handle is managed by this variable and will be closed when disposing or GC-collecting it. Use <see cref="GC.KeepAlive"/> where need.
			/// </remarks>
			public IntPtr UnsafeTokenHandle => _htoken;

			/// <summary>
			/// Gets true if the last called property function failed.
			/// Normally getting properties should never fail. Only <see cref="GetOfProcess"/> can fail (then it returns null).
			/// </summary>
			public bool Failed { get; private set; }

#pragma warning disable 1591 //XML doc
			/// <summary>
			/// <see cref="IntegrityLevel"/>.
			/// </summary>
			/// <tocexclude />
			public enum IL { Untrusted, Low, Medium, UIAccess, High, System, Protected, Unknown = 100 }

			/// <summary>
			/// <see cref="Elevation"/>.
			/// </summary>
			/// <tocexclude />
			public enum ElevationType { Unknown, Default, Full, Limited }
#pragma warning restore 1591

			ElevationType _Elevation; byte _haveElevation;
			/// <summary>
			/// Gets process <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> elevation type.
			/// Elevation types:
			/// Full - runs as administrator (High or System integrity level).
			/// Limited - runs as standard user (Medium, Medium+UIAccess or Low integrity level) on administrator user session.
			/// Default - all processes in this user session run as admin, or all as standard user. Can be: non-administrator user session; service session; UAC is turned off.
			/// Unknown - failed to get. Normally it never happens; only <see cref="GetOfProcess"/> can fail (then it returns null).
			/// This property is rarely useful. Instead use other properties of this class.
			/// </summary>
			public ElevationType Elevation
			{
				get
				{
					if(_haveElevation == 0) {
						unsafe {
							ElevationType elev;
							if(!Api.GetTokenInformation(_HtokenHR, Api.TOKEN_INFORMATION_CLASS.TokenElevationType, &elev, 4, out var siz)) _haveElevation = 2;
							else {
								_haveElevation = 1;
								_Elevation = elev;
							}
						}
					}
					if(Failed = (_haveElevation == 2)) return ElevationType.Unknown;
					return _Elevation;
				}
			}

			bool _isUIAccess; byte _haveIsUIAccess;
			/// <summary>
			/// Returns true if the process has <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">uiAccess</conceptualLink> property.
			/// A uiAccess process can access/automate all windows of processes running in the same user session.
			/// Most processes don't have this property. They cannot access/automate windows of higher integrity level (High, System, uiAccess) processes and Windows 8 store apps. For example, cannot send keys and Windows messages.
			/// Note: High IL (admin) processes also can have this property, therefore <c>IsUIAccess</c> is not the same as <c>IntegrityLevelAndUIAccess==IL.UIAccess</c> (<see cref="IntegrityLevelAndUIAccess"/> returns <b>UIAccess</b> only for Medium+uiAccess processes; for High+uiAccess processes it returns <b>High</b>). Some Windows API work slightly differently with uiAccess and non-uiAccess admin processes.
			/// This property is rarely useful. Instead use other properties of this class.
			/// </summary>
			public bool IsUIAccess
			{
				get
				{
					if(_haveIsUIAccess == 0) {
						unsafe {
							uint uia;
							if(!Api.GetTokenInformation(_HtokenHR, Api.TOKEN_INFORMATION_CLASS.TokenUIAccess, &uia, 4, out var siz)) _haveIsUIAccess = 2;
							else {
								_haveIsUIAccess = 1;
								_isUIAccess = uia != 0;
							}
						}
					}
					if(Failed = (_haveIsUIAccess == 2)) return false;
					return _isUIAccess;
				}
			}

			//not very useful. Returns false for ApplicationFrameWindow. Can use Wnd.IsWindows10StoreApp.
			///// <summary>
			///// Returns true if the process is a Windows Store app.
			///// </summary>
			//public unsafe bool IsAppContainer
			//{
			//	get
			//	{
			//		if(!Ver.MinWin8) return false;
			//		uint isac;
			//		if(Failed = !Api.GetTokenInformation(_HtokenHR, Api.TOKEN_INFORMATION_CLASS.TokenIsAppContainer, &isac, 4, out var siz)) return false;
			//		return isac != 0;
			//	}
			//}

#pragma warning disable 649
			struct TOKEN_MANDATORY_LABEL { internal IntPtr Sid; internal uint Attributes; }
#pragma warning restore 649
			const uint SECURITY_MANDATORY_UNTRUSTED_RID = 0x00000000;
			const uint SECURITY_MANDATORY_LOW_RID = 0x00001000;
			const uint SECURITY_MANDATORY_MEDIUM_RID = 0x00002000;
			const uint SECURITY_MANDATORY_MEDIUM_PLUS_RID = SECURITY_MANDATORY_MEDIUM_RID + 0x100;
			const uint SECURITY_MANDATORY_HIGH_RID = 0x00003000;
			const uint SECURITY_MANDATORY_SYSTEM_RID = 0x00004000;
			const uint SECURITY_MANDATORY_PROTECTED_PROCESS_RID = 0x00005000;

			IL _integrityLevel; byte _haveIntegrityLevel;
			/// <summary>
			/// Gets process <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level (IL).
			/// IL from lowest to highest value:
			///		<b>Untrusted</b> - the most limited rights. Very rare.
			///		<b>Low</b> - very limited rights. Used by Internet Explorer tab processes, Windows Store apps.
			///		<b>Medium</b> - limited rights. Most processes (unless UAC turned off).
			///		<b>UIAccess</b> - Medium IL + can access/automate High IL windows (user interface).
			///			Note: Only the <see cref="IntegrityLevelAndUIAccess"/> property can return <b>UIAccess</b>. This property returns <b>High</b> instead (the same as in Process Explorer).
			///		<b>High</b> - most rights. Processes that run as administrator.
			///		<b>System</b> - almost all rights. Services, some system processes.
			///		<b>Protected</b> - undocumented. Rare.
			///		<b>Unknown</b> - failed to get IL. Unlikely.
			/// The IL enum member values can be used like <c>if(x.IntegrityLevel > IL.Medium) ...</c> .
			/// If UAC is turned off, most non-service processes on administrator account have High IL; on non-administrator - Medium.
			/// </summary>
			public IL IntegrityLevel
			{
				get => _GetIntegrityLevel(false);
			}

			/// <summary>
			/// The same as <see cref="IntegrityLevel"/>, but can return <b>UIAccess</b>.
			/// </summary>
			public IL IntegrityLevelAndUIAccess
			{
				get => _GetIntegrityLevel(true);
			}

			IL _GetIntegrityLevel(bool andUIAccess)
			{
				if(_haveIntegrityLevel == 0) {
					unsafe {
						Api.GetTokenInformation(_HtokenHR, Api.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, null, 0, out var siz);
						if(Native.GetError() != Api.ERROR_INSUFFICIENT_BUFFER) _haveIntegrityLevel = 2;
						else {
							var b = stackalloc byte[(int)siz];
							var tml = (TOKEN_MANDATORY_LABEL*)b;
							if(!Api.GetTokenInformation(_HtokenHR, Api.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, tml, siz, out siz)) _haveIntegrityLevel = 2;
							uint x = *Api.GetSidSubAuthority(tml->Sid, (uint)(*Api.GetSidSubAuthorityCount(tml->Sid) - 1));

							if(x < SECURITY_MANDATORY_LOW_RID) _integrityLevel = IL.Untrusted;
							else if(x < SECURITY_MANDATORY_MEDIUM_RID) _integrityLevel = IL.Low;
							else if(x < SECURITY_MANDATORY_HIGH_RID) _integrityLevel = IL.Medium;
							else if(x < SECURITY_MANDATORY_SYSTEM_RID) {
								if(IsUIAccess && Elevation != ElevationType.Full) _integrityLevel = IL.UIAccess; //fast. Note: don't use if(andUIAccess) here.
								else _integrityLevel = IL.High;
							} else if(x < SECURITY_MANDATORY_PROTECTED_PROCESS_RID) _integrityLevel = IL.System;
							else _integrityLevel = IL.Protected;
						}
					}
				}
				if(Failed = (_haveIntegrityLevel == 2)) return IL.Unknown;
				if(!andUIAccess && _integrityLevel == IL.UIAccess) return IL.High;
				return _integrityLevel;
			}

			UacInfo(IntPtr hToken) { _htoken = hToken; }

			static UacInfo _Create(IntPtr hProcess)
			{
				if(!Api.OpenProcessToken(hProcess, Api.TOKEN_QUERY | Api.TOKEN_QUERY_SOURCE, out var hToken)) return null;
				return new UacInfo(hToken);
			}

			/// <summary>
			/// Opens process access token and creates/returns new <see cref="UacInfo"/> variable that holds it. Then you can use its properties.
			/// Returns null if failed. For example fails for services and some other processes if current process is not administrator.
			/// To get <b>UacInfo</b> of this process, instead use <see cref="ThisProcess"/>.
			/// </summary>
			/// <param name="processId">Process id. If you have a window, use <see cref="Wnd.ProcessId"/>.</param>
			public static UacInfo GetOfProcess(int processId)
			{
				if(processId == 0) return null;
				using(var hp = LibProcessHandle.FromId(processId)) {
					if(hp.Is0) return null;
					return _Create(hp);
				}
			}

			/// <summary>
			/// Gets <see cref="UacInfo"/> variable for this process.
			/// </summary>
			public static UacInfo ThisProcess
			{
				get
				{
					if(_thisProcess == null) {
						_thisProcess = _Create(Api.GetCurrentProcess());
						Debug.Assert(_thisProcess != null);
					}
					return _thisProcess;
				}
			}
			static UacInfo _thisProcess;

			/// <summary>
			/// Returns true if this process is running as administrator, ie if the user belongs to the local Administrators group and the process is not limited by <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink>.
			/// This function for example can be used to check whether you can write to protected locations in the file system and registry.
			/// </summary>
			public static bool IsAdmin
			{
				get
				{
					if(!_haveIsAdmin) {
						try {
							WindowsIdentity id = WindowsIdentity.GetCurrent();
							WindowsPrincipal principal = new WindowsPrincipal(id);
							_isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
						}
						catch { }
						_haveIsAdmin = true;
					}
					return _isAdmin;
				}
			}
			static bool _isAdmin, _haveIsAdmin;

			/*
			public struct SID_IDENTIFIER_AUTHORITY
			{
				public byte b0, b1, b2, b3, b4, b5;
			}

			[DllImport("advapi32.dll")]
			public static extern bool AllocateAndInitializeSid(in SID_IDENTIFIER_AUTHORITY pIdentifierAuthority, byte nSubAuthorityCount, uint nSubAuthority0, uint nSubAuthority1, uint nSubAuthority2, uint nSubAuthority3, uint nSubAuthority4, uint nSubAuthority5, uint nSubAuthority6, uint nSubAuthority7, out IntPtr pSid);

			[DllImport("advapi32.dll")]
			public static extern bool CheckTokenMembership(IntPtr TokenHandle, IntPtr SidToCheck, out bool IsMember);

			[DllImport("advapi32.dll")]
			public static extern IntPtr FreeSid(IntPtr pSid);

			public const int SECURITY_BUILTIN_DOMAIN_RID = 32;
			public const int DOMAIN_ALIAS_RID_ADMINS = 544;

			//This is from CheckTokenMembership reference.
			//In QM2 it is very fast, but here quite slow first time, although then becomes the fastest. Advapi32.dll is already loaded, but maybe it loads other dlls.
			//IsUserAnAdmin first time can be slowest. It loads shell32.dll.
			//The .NET principal etc first time usually is fastest, althoug later is slower several times.
			//All 3 tested on admin and user accounts, also when UAC is turned off, also with System IL.
			public static bool IsAdmin
			{
				get
				{
					var NtAuthority = new SID_IDENTIFIER_AUTHORITY() { b5 = 5 }; //SECURITY_NT_AUTHORITY
					IntPtr AdministratorsGroup;
					if(!AllocateAndInitializeSid(NtAuthority, 2,
						SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS,
						0, 0, 0, 0, 0, 0,
						out AdministratorsGroup
						))
						return false;
					bool R;
					if(!CheckTokenMembership(default, AdministratorsGroup, out R)) R = false;
					FreeSid(AdministratorsGroup);
					return R;
				}
			}
			*/

			/// <summary>
			/// Returns true if <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> is disabled (turned off) on this Windows 7 computer.
			/// On Windows 8 and 10 UAC cannot be disabled, although you can disable UAC elevation consent dialogs.
			/// </summary>
			public static bool IsUacDisabled
			{
				get
				{
					if(!_haveIsUacDisabled) {
						_isUacDisabled = _IsUacDisabled();
						_haveIsUacDisabled = true;
					}
					return _isUacDisabled;
				}
			}

			static bool _isUacDisabled, _haveIsUacDisabled;
			static bool _IsUacDisabled()
			{
				if(Ver.MinWin8) return false; //UAC cannot be disabled
				UacInfo x = ThisProcess;
				switch(x.Elevation) {
				case ElevationType.Full:
				case ElevationType.Limited:
					return false;
				}
				if(x.IsUIAccess) return false;

				int r = 1;
				try {
					r = (int)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", 1);
				}
				catch { }
				return r == 0;
			}
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
		/// Returns 0 if failed. Supports <see cref="Native.GetError"/>.
		/// Calls API <msdn>GetProcessId</msdn>.
		/// </summary>
		/// <param name="processHandle">Process handle.</param>
		public static int GetProcessId(IntPtr processHandle)
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
		/// Gets user session id of a process.
		/// Returns -1 if failed. Supports <see cref="Native.GetError"/>.
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
		public static int GetSessionId()
		{
			return GetSessionId(Api.GetCurrentProcessId());
		}
	}
}
