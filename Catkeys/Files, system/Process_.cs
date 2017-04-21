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
using System.Security.Principal;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Extends the .NET class Process.
	/// Also has some thread functions.
	/// </summary>
	//[DebuggerStepThrough]
	public static class Process_
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern unsafe bool QueryFullProcessImageNameW(IntPtr hProcess, bool nativeFormat, char* lpExeName, ref int lpdwSize);

		static unsafe bool _QueryFullProcessImageName(IntPtr hProcess, bool nativeFormat, out string s)
		{
			s = null;
			var b = Util.LibCharBuffer.Common; int size = b.Max(300);
			g1: if(!QueryFullProcessImageNameW(hProcess, nativeFormat, b.Alloc(size), ref size)) {
				if(Native.GetError() == Api.ERROR_INSUFFICIENT_BUFFER) { size *= 2; goto g1; }
				return false;
			}
			s = b.ToString(size);
			return true;
		}

		static unsafe string _GetProcessName(int processId, bool fullPath, bool dontEnumerate = false, bool unDOS = false)
		{
			if(processId == 0) return null;
			string R = null;

			using(var ph = new LibProcessHandle(processId)) {
				if(!ph.Is0) {
					//info:
					//In non-admin process fails if the process is of another user session; then use the slooow enumeration.
					//Also fails for some system processes: nvvsvc, nvxdsync, dwm. For dwm fails even in admin process.

					bool getNormal = fullPath || unDOS; //getting native path is faster, but it gets like "\Device\HarddiskVolume5\Windows\SysWOW64\notepad.exe" and there is no API to convert to normal
					if(_QueryFullProcessImageName(ph, !getNormal, out var s)) {
						R = s;
						if(!fullPath) R = GetFileNameWithoutExe(R);

						if(R.IndexOf('~') >= 0) { //DOS path?
							if(getNormal || _QueryFullProcessImageName(ph, false, out s)) {
								R = Path_.LibExpandDosPath(s);
								if(!fullPath) R = GetFileNameWithoutExe(R);
							}
						}
					}
				} else if(!dontEnumerate && !fullPath) {
					EnumProcesses(p =>
						{
							if(p.ProcessID != processId) return false;
							R = GetFileNameWithoutExe(p.ProcessName);
							//Print(R);
							return true;
						});
				}
			}

			return R;
		}

		//Use ProcessInfoInternal and ProcessInfo because with WTSEnumerateProcessesW _ProcessName must be IntPtr, and then WTSFreeMemory frees its memory, therefore GetProcesses() converts ProcessInfoInternal to ProcessInfo where ProcessName is string. Almost same speed.
		internal struct ProcessInfoInternal
		{
#pragma warning disable 649 //says never used
			public int SessionID;
			public int ProcessID;
			IntPtr _ProcessName;
			public IntPtr UserSid;
#pragma warning restore

			/// <summary>
			/// Process executable file name without ".exe". Not full path.
			/// </summary>
			public string ProcessName
			{
				get
				{
					string R = Marshal.PtrToStringUni(_ProcessName);
					if(R.IndexOf('~') >= 0) {
						string t = _GetProcessName(ProcessID, false, true, true);
						if(t != null) R = t;
					} else R = GetFileNameWithoutExe(R);
					return R;
				}
			}
		}

		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern bool WTSEnumerateProcessesW(IntPtr serverHandle, uint reserved, uint version, out IntPtr ppProcessInfo, out int pCount);

		[DllImport("wtsapi32.dll", SetLastError = false)]
		static extern void WTSFreeMemory(IntPtr memory);

		/// <summary>
		/// Calls callback function for each process, until the function returns true.
		/// </summary>
		/// <param name="f">Lambda etc function that is called for each process.</param>
		/// <param name="ofThisSession">Get processes only of this user session (skip services etc).</param>
		internal static unsafe bool EnumProcesses(Func<ProcessInfoInternal, bool> f, bool ofThisSession = false)
		{
			int sessionId = 0;
			if(ofThisSession) {
				if(!Api.ProcessIdToSessionId(Api.GetCurrentProcessId(), out sessionId)) return false;
			}

			if(!WTSEnumerateProcessesW(Zero, 0, 1, out IntPtr pp, out int n)) return false;
			try {
				ProcessInfoInternal* p = (ProcessInfoInternal*)pp;
				for(int i = 1; i < n; i++) { //i=1 because the first process is inaccessible, its name is empty
					if(ofThisSession && p[i].SessionID != sessionId) continue;
					if(f(p[i])) break;
				}
			}
			finally { WTSFreeMemory(pp); }

			return true;

			//Other ways to enumerate processes or get process id:

			////speed: 15% slower
			//var pi = new Api.PROCESSENTRY32(); pi.dwSize = Api.SizeOf(pi);
			//IntPtr hSnap = Api.CreateToolhelp32Snapshot(Api.TH32CS_SNAPPROCESS, 0); if(hSnap == Zero) return null;
			//for(bool ok = Api.Process32First(hSnap, ref pi); ok; ok = Api.Process32Next(hSnap, ref pi)) {
			//	if(pi.th32ProcessID == processId) {
			//		R = RemoveExeFromFileName(pi.szExeFile);
			//		break;
			//	}
			//}
			//Api.CloseHandle(hSnap);

			////speed: 30% slower
			//Process p = Process.GetProcessById((int)processId);
			//if(p != null) try { R = p.ProcessName; } catch { } //fails
		}

		/// <summary>
		/// Contains process id, name, user session id.
		/// </summary>
		/// <tocexclude />
		public struct ProcessInfo
		{
			///
			public int SessionID;
			///
			public int ProcessID;
			/// <summary>
			/// Process executable file name without ".exe". Not full path.
			/// </summary>
			public string ProcessName;
			//public IntPtr UserSid; //where is its memory?
		}

		/// <summary>
		/// Gets processes array that contains process name, id and session id.
		/// </summary>
		/// <param name="ofThisSession">Get processes only of this user session (skip services etc).</param>
		public static unsafe ProcessInfo[] GetProcesses(bool ofThisSession = false)
		{
			int sessionId = 0;
			if(ofThisSession) {
				if(!Api.ProcessIdToSessionId(Api.GetCurrentProcessId(), out sessionId)) return null;
			}

			if(!WTSEnumerateProcessesW(Zero, 0, 1, out IntPtr pp, out int n)) return null;

			var t = new List<ProcessInfo>((int)n);
			ProcessInfoInternal* p = (ProcessInfoInternal*)pp;
			for(int i = 1; i < n; i++) { //i=1 because the first process is inaccessible, its name is empty
				if(ofThisSession && p[i].SessionID != sessionId) continue;
				t.Add(new ProcessInfo() { SessionID = p[i].SessionID, ProcessID = p[i].ProcessID, ProcessName = p[i].ProcessName });
			}
			WTSFreeMemory(pp);

			return t.ToArray();
		}

		/// <summary>
		/// Gets process executable file name without ".exe", or full path.
		/// Returns null if fails.
		/// </summary>
		/// <param name="processId">Process id. If you have a window, use its <see cref="Wnd.ProcessId">ProcessId</see> property.</param>
		/// <param name="fullPath">Get full path. Note: Fails to get full path if the process belongs to another user session, unless current process is admin; also fails to get full path of some system processes.</param>
		/// <param name="noSlowAPI">When the fast API QueryFullProcessImageName fails, don't try to use another much slower API WTSEnumerateProcesses. Not used if fullPath is true.</param>
		public static string GetProcessName(int processId, bool fullPath = false, bool noSlowAPI = false)
		{
			return _GetProcessName(processId, fullPath, noSlowAPI);
		}

		/// <summary>
		/// Returns list of process id of all processes whose names match processName.
		/// Returns empty list if there are no matching processes.
		/// </summary>
		/// <param name="processName">
		/// Process name.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// </param>
		/// <param name="fullPath">If false, processName is filename without ".exe". If true, processName is full path. Note: Fails to get full path if the process belongs to another user session, unless current process is admin; also fails to get full path of some system processes.</param>
		/// <exception cref="ArgumentException">
		/// processName is "" or null.
		/// Invalid wildcard expression ("**options|" or regular expression).
		/// </exception>
		public static List<int> GetProcessesByName(string processName, bool fullPath = false)
		{
			if(Empty(processName)) throw new ArgumentException();
			return GetProcessesByName((Wildex)processName, fullPath);
		}

		internal static List<int> GetProcessesByName(Wildex processName, bool fullPath = false)
		{
			List<int> a = new List<int>();
			EnumProcesses(p =>
			{
				string s;
				if(fullPath) {
					s = GetProcessName(p.ProcessID, true);
					if(s == null) return false;
				} else s = GetFileNameWithoutExe(p.ProcessName);

				if(processName.Match(s)) {
					a.Add(p.ProcessID);
				}
				return false;
			});

			return a;
		}

		/// <summary>
		/// Removes path and ".exe" extension from file name.
		/// Does not remove other extensions.
		/// </summary>
		/// <param name="fileName">A file name or full path. Can be null.</param>
		public static string GetFileNameWithoutExe(string fileName)
		{
			if(fileName == null) return null;
			if(fileName.EndsWith_(".exe")) return Path_.GetFileNameWithoutExtension(fileName);
			return Path_.GetFileName(fileName);
		}

		/// <summary>
		/// Opens and manages a process handle.
		/// Calls API <msdn>CloseHandle</msdn> in Dispose (which normally is implicitly called at the end of <c>using(...){...}</c>) or in finalizer (which is called later by the GC).
		/// </summary>
		internal sealed class LibProcessHandle :IDisposable
		{
			IntPtr _h;

			#region IDisposable Support

			void _Dispose()
			{
				if(_h != Zero) { Api.CloseHandle(_h); _h = Zero; }
			}

			~LibProcessHandle() { _Dispose(); }

			public void Dispose()
			{
				_Dispose();
				GC.SuppressFinalize(this);
			}
			#endregion

			//public LibProcessHandle() { }

			/// <summary>
			/// Attaches a kernel handle to this new object.
			/// No exception when handle is invalid.
			/// </summary>
			/// <param name="handle"></param>
			public LibProcessHandle(IntPtr handle) { _h = handle; }

			/// <summary>
			/// Opens a process handle.
			/// Calls API OpenProcess.
			/// No exception when fails; use Is0. Supports Native.GetError().
			/// </summary>
			/// <param name="processId">Process id.</param>
			/// <param name="desiredAccess">Desired access, as documented in MSDN -> OpenProcess.</param>
			public LibProcessHandle(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION) { _Open(processId, desiredAccess); }

			/// <summary>
			/// Opens window's process handle.
			/// This overload is more powerful: if API OpenProcess fails, it tries GetProcessHandleFromHwnd, which can open higher integrity level processes, but only if current process is uiAccess and desiredAccess includes only PROCESS_DUP_HANDLE, PROCESS_VM_OPERATION, PROCESS_VM_READ, PROCESS_VM_WRITE, SYNCHRONIZE.
			/// No exception when fails; use Is0. Supports Native.GetError().
			/// </summary>
			/// <param name="w">Window.</param>
			/// <param name="desiredAccess">Api.PROCESS_</param>
			public LibProcessHandle(Wnd w, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION) { _Open(w.ProcessId, desiredAccess, w); }

			void _Open(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION, Wnd processWindow = default(Wnd))
			{
				int e = 0;
				if(processId != 0) {
					_h = Api.OpenProcess(desiredAccess, false, processId);
					if(!Is0) return;
					e = Native.GetError();
				}
				if(!processWindow.Is0) {
					if((desiredAccess & ~(Api.PROCESS_DUP_HANDLE | Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE | Api.SYNCHRONIZE)) == 0
					&& UacInfo.ThisProcess.IsUIAccess
					) _h = GetProcessHandleFromHwnd(processWindow);

					if(Is0) Api.SetLastError(e);
				}
			}

			[DllImport("oleacc.dll")]
			static extern IntPtr GetProcessHandleFromHwnd(Wnd hwnd);

			//void _Open(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
			//{
			//	if(processId != 0) _h = Api.OpenProcess(desiredAccess, false, processId);
			//	if(Is0) GC.SuppressFinalize(this);
			//}

			//public static implicit operator LibProcessHandle(IntPtr handle) { return handle == Zero ? null : new LibProcessHandle(handle); } //unsafe, because does not dispose the left-side object immediately if it already holds a handle; but it is unlikely, and not dangerous, because the handle eventually would be disposed by the finalizer.
			public static implicit operator IntPtr(LibProcessHandle p) { return p._h; }

			public bool Is0 { get => _h == Zero; }

		}

		/// <summary>
		/// Process handle that is inherited from WaitHandle.
		/// When don't need to wait, use LibProcessHandle, it's more lightweight and has more creation methods. Or use .NET Process class.
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
		public sealed unsafe class Memory :IDisposable
		{
			LibProcessHandle _hproc;

			#region IDisposable Support

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
				if(_hproc == null) return;
				if(Mem != Zero) { Api.VirtualFreeEx(_hproc, Mem); Mem = Zero; }
				_hproc.Dispose();
				_hproc = null;
			}

			#endregion

			/// <summary>
			/// Process handle.
			/// Opened with access PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE.
			/// </summary>
			public IntPtr ProcessHandle { get => _hproc; }

			/// <summary>
			/// Address of memory allocated in that process. Invalid in this process.
			/// </summary>
			public IntPtr Mem { get; private set; }

			void _Alloc(int pid, Wnd w, int nBytes)
			{
				const uint fl = Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE;
				_hproc = w.Is0 ? new LibProcessHandle(pid, fl) : new LibProcessHandle(w, fl);
				if(_hproc.Is0) { var e = new CatException(0, "Failed to open process handle."); _Dispose(); throw e; }

				if(nBytes != 0) {
					Mem = Api.VirtualAllocEx(_hproc, Zero, nBytes);
					if(Mem == Zero) { var e = new CatException(0, "Failed to allocate process memory."); _Dispose(); throw e; }
				}
			}

			/// <summary>
			/// Opens window's process handle and optionally allocates memory in that process.
			/// </summary>
			/// <param name="w">A window in that process.</param>
			/// <param name="nBytes">If not 0, allocates this number of bytes of memory in that process.</param>
			/// <remarks>This is the preferred constructor when the process has windows. It works with windows of UAC High integrity level when this process is Medium+uiAccess.</remarks>
			/// <exception cref="WndException">w invalid.</exception>
			/// <exception cref="CatException">Failed to open process handle (usually because of UAC) or allocate memory.</exception>
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
			/// <exception cref="CatException">Failed to open process handle (usually because of UAC) or allocate memory.</exception>
			public Memory(int processId, int nBytes)
			{
				_Alloc(processId, Wnd0, nBytes);
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
				if(Mem == Zero) return false;
				if(Empty(s)) return true;
				fixed (char* p = s) {
					return WriteProcessMemory(_hproc, Mem + offsetBytes, p, (s.Length + 1) * 2, null);
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
				if(Mem == Zero) return false;
				if(Empty(s)) return true;
				if(enc == null) enc = Encoding.Default;
				var a = enc.GetBytes(s + "\0");
				fixed (byte* p = a) {
					return WriteProcessMemory(_hproc, Mem + offsetBytes, p, a.Length, null);
				}
			}

			string _ReadString(bool ansiString, int nChars, int offsetBytes, Encoding enc = null)
			{
				if(Mem == Zero) return null;
				var b = Util.LibCharBuffer.Common;
				int na = nChars; if(!ansiString) na *= 2;
				if(!ReadProcessMemory(_hproc, Mem + offsetBytes, b.Alloc((na + 1) / 2), na, null)) return null;
				if(!ansiString) return b.ToString();
				return b.ToStringFromAnsi(enc);
			}

			/// <summary>
			/// Copies a string from the memory in that process allocated by the constructor to this process.
			/// Returns the copies string, or null if fails.
			/// In that process the string must be in Unicode UTF-16 format (ie not ANSI).
			/// </summary>
			/// <param name="nChars">Number of characters to copy. In both processes a character is 2 bytes.</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			public string ReadUnicodeString(int nChars, int offsetBytes = 0)
			{
				return _ReadString(false, nChars, offsetBytes);
			}

			/// <summary>
			/// Copies a string from the memory in that process allocated by the constructor to this process.
			/// Returns the copies string, or null if fails.
			/// In that process the string must be in ANSI format (ie not Unicode UTF-16).
			/// </summary>
			/// <param name="nBytes">Number bytes to copy. In that process a character is 1 or more bytes (depending on encoding), in this process will be 2 bytes (normal C# string).</param>
			/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
			/// <param name="enc">If null, uses system's default ANSI encoding.</param>
			public string ReadAnsiString(int nBytes, int offsetBytes = 0, Encoding enc = null)
			{
				return _ReadString(true, nBytes, offsetBytes, enc);
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
				if(Mem == Zero) return false;
				return WriteProcessMemory(_hproc, Mem + offsetBytes, ptr, nBytes, null);
			}

			/// <summary>
			/// Copies a value-type variable or other memory from this process to a known memory address in that process.
			/// Returns false if fails.
			/// </summary>
			/// <param name="ptrDestinationInThatProcess">Memory address in that process where to copy memory from this process.</param>
			/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
			/// <param name="nBytes">Number of bytes to copy.</param>
			public bool WriteOther(IntPtr ptrDestinationInThatProcess, void* ptr, int nBytes)
			{
				return WriteProcessMemory(_hproc, ptrDestinationInThatProcess, ptr, nBytes, null);
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
				if(Mem == Zero) return false;
				return ReadProcessMemory(_hproc, Mem + offsetBytes, ptr, nBytes, null);
			}

			/// <summary>
			/// Copies from a known memory address in that process to a value-type variable or other memory in this process.
			/// Returns false if fails.
			/// </summary>
			/// <param name="ptrSourceInThatProcess">Memory address in that process from where to copy memory.</param>
			/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
			/// <param name="nBytes">Number of bytes to copy.</param>
			public bool ReadOther(IntPtr ptrSourceInThatProcess, void* ptr, int nBytes)
			{
				return ReadProcessMemory(_hproc, ptrSourceInThatProcess, ptr, nBytes, null);
			}

			//Cannot get pointer if generic type. Could try Marshal.StructureToPtr etc but I don't like it.
			//public bool Write<T>(ref T v, int offsetBytes = 0) where T : struct
			//{
			//	int n = Marshal.SizeOf(v.GetType());
			//	return Write(&v, n, offsetBytes);
			//	Marshal.StructureToPtr(v, m, false); ...
			//}

			[DllImport("kernel32.dll")]
			static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, LPARAM nSize, LPARAM* lpNumberOfBytesRead);

			[DllImport("kernel32.dll")]
			static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, LPARAM nSize, LPARAM* lpNumberOfBytesWritten);
		}

		/// <summary>
		/// Holds access token (security info) of a process and provides various security info, eg UAC integrity level.
		/// </summary>
		public class UacInfo :IDisposable
		{
			#region IDisposable Support

			void _Dispose()
			{
				if(_htoken != Zero) { Api.CloseHandle(_htoken); _htoken = Zero; }
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

			/// <summary>
			/// Access token handle.
			/// </summary>
			public IntPtr TokenHandle { get => _htoken; }

			/// <summary>
			/// Gets true if the last called property function failed.
			/// Normally getting properties should never fail. Only the GetOfProcess method can fail, then it returns null.
			/// </summary>
			public bool Failed { get; private set; }

#pragma warning disable 1591 //XML doc
			/// <summary>
			/// <see cref="UacInfo.IntegrityLevel"/>.
			/// </summary>
			/// <tocexclude />
			public enum IL { Untrusted, Low, Medium, UIAccess, High, System, Protected, Unknown = 100 }

			/// <summary>
			/// <see cref="UacInfo.Elevation"/>.
			/// </summary>
			/// <tocexclude />
			public enum ElevationType { Unknown, Default, Full, Limited }
#pragma warning restore 1591

			ElevationType _Elevation; byte _haveElevation;
			/// <summary>
			/// Gets process UAC elevation type.
			/// Elevation types:
			/// Full - runs as administrator (High or System integrity level).
			/// Limited - runs as standard user (Medium, Medium+UIAccess or Low integrity level) on administrator user session.
			/// Default - all processes in this user session run as admin, or all as standard user. Can be: non-administrator user session; service session; UAC is turned off.
			/// Unknown - failed to get. Normally it never happens; only GetOfProcess can fail, then it returns null.
			/// This property is rarely useful. Instead use other properties of this class.
			/// </summary>
			public ElevationType Elevation
			{
				get
				{
					if(_haveElevation == 0) {
						unsafe
						{
							ElevationType elev;
							if(!Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenElevationType, &elev, 4, out var siz)) _haveElevation = 2;
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
			/// Returns true if the process has uiAccess property.
			/// A uiAccess process can access/automate all windows of processes running in the same user session.
			/// Most processes don't have this property and cannot access/automate windows of higher integrity level (High, System, Middle+uiAccess) processes and Windows 8 store apps. For example, cannot send keys and Windows messages.
			/// Note: High IL (admin) processes also can have this property, therefore IsUIAccess is not the same as IntegrityLevelAndUIAccess==IL.UIAccess (IntegrityLevelAndUIAccess returns IL.UIAccess only for Medium+uiAccess processes; for High+uiAccess processes it returns IL.High). Some Windows API work slightly differently with uiAccess and non-uiAccess admin processes.
			/// This property is rarely useful. Instead use other properties of this class.
			/// </summary>
			public bool IsUIAccess
			{
				get
				{
					if(_haveIsUIAccess == 0) {
						unsafe
						{
							uint uia;
							if(!Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenUIAccess, &uia, 4, out var siz)) _haveIsUIAccess = 2;
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

			/// <summary>
			/// Returns true if the process is a Windows Store app.
			/// </summary>
			public bool IsAppContainer
			{
				get
				{
					if(!Ver.MinWin8) return false;
					unsafe
					{
						uint isac;
						if(Failed = !Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenIsAppContainer, &isac, 4, out var siz)) return false;
						return isac != 0;
					}
				}
			}

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
			/// Gets process UAC integrity level (IL).
			/// IL from lowest to highest value:
			///		Untrusted - the most limited rights. Very rare.
			///		Low - very limited rights. Used by Internet Explorer tab processes, Windows Store apps.
			///		Medium - limited rights. Most processes (unless UAC turned off).
			///		UIAccess - Medium IL + can access/automate High IL windows (user interface).
			///			Note: Only the <see cref="IntegrityLevelAndUIAccess"/> property can return UIAccess. This property returns High instead (the same as in Process Explorer).
			///		High - most rights. Processes that run as administrator.
			///		System - almost all rights. Services, some system processes.
			///		Protected - undocumented. Never seen.
			///		Unknown - failed to get IL. Never seen.
			/// The IL enum member values can be used like <c>if(x.IntegrityLevel > IL.Medium) ...</c> .
			/// If UAC is turned off, most non-service processes on administrator account have High IL; on non-administrator - Medium.
			/// </summary>
			public IL IntegrityLevel
			{
				get => _GetIntegrityLevel(false);
			}

			/// <summary>
			/// The same as IntegrityLevel, but can return UIAccess.
			/// </summary>
			public IL IntegrityLevelAndUIAccess
			{
				get => _GetIntegrityLevel(true);
			}

			IL _GetIntegrityLevel(bool andUIAccess)
			{
				if(_haveIntegrityLevel == 0) {
					unsafe
					{
						Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, null, 0, out var siz);
						if(Native.GetError() != Api.ERROR_INSUFFICIENT_BUFFER) _haveIntegrityLevel = 2;
						else {
							TOKEN_MANDATORY_LABEL* tml = (TOKEN_MANDATORY_LABEL*)Marshal.AllocHGlobal((int)siz);
							if(tml == null) _haveIntegrityLevel = 2;
							else {
								if(!Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, tml, siz, out siz)) _haveIntegrityLevel = 2;
								uint x = *Api.GetSidSubAuthority(tml->Sid, (uint)(*Api.GetSidSubAuthorityCount(tml->Sid) - 1));

								//Output.WriteHex(IL);
								if(x < SECURITY_MANDATORY_LOW_RID) _integrityLevel = IL.Untrusted;
								else if(x < SECURITY_MANDATORY_MEDIUM_RID) _integrityLevel = IL.Low;
								else if(x < SECURITY_MANDATORY_HIGH_RID) _integrityLevel = IL.Medium;
								else if(x < SECURITY_MANDATORY_SYSTEM_RID) {
									if(IsUIAccess && Elevation != ElevationType.Full) _integrityLevel = IL.UIAccess; //fast. Note: don't use if(andUIAccess) here.
									else _integrityLevel = IL.High;
								} else if(x < SECURITY_MANDATORY_PROTECTED_PROCESS_RID) _integrityLevel = IL.System;
								else _integrityLevel = IL.Protected;

								Marshal.FreeHGlobal((IntPtr)tml);
							}
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
			/// Opens process access token and creates UacInfo object that holds it.
			/// Returns UacInfo object. Then you can use its properties.
			/// Returns null if failed. For example fails for services and some other processes if current process is not administrator.
			/// To get UacInfo of this process, instead use UacInfo.ThisProcess.
			/// </summary>
			/// <param name="processId">Process id. If you have a window, use its <see cref="Wnd.ProcessId">ProcessId</see> property.</param>
			public static UacInfo GetOfProcess(int processId)
			{
				if(processId == 0) return null;
				using(var hp = new LibProcessHandle(processId)) {
					if(hp.Is0) return null;
					return _Create(hp);
				}
			}

			/// <summary>
			/// Gets UacInfo object of current process.
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

			///// <summary>
			///// Returns true if this process has UAC integrity level (IL) High or System, which means that it has most administrative privileges.
			///// Returns false if this process has lower UAC integrity level (Medium, Medium+UIAccess, Low, Untrusted).
			///// Note: although the name incluses 'Admin', this function does not check whether the user is in Administrators group; it returns true if <c>UacInfo.ThisProcess.IntegrityLevelAndUIAccess &gt;= UacInfo.IL.High</c> .
			///// If UAC is turned off, on administrator account most processes have High IL. On non-administrator account most processes always have Medium or Low IL.
			///// </summary>
			//public static bool IsAdmin
			//{
			//	get => ThisProcess.IntegrityLevelAndUIAccess >= IL.High;
			//}

			/// <summary>
			/// Returns true if current process is running as administrator, ie if the user belongs to the local Administrators group and the process is not limited by UAC.
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
			public static extern bool AllocateAndInitializeSid([In] ref SID_IDENTIFIER_AUTHORITY pIdentifierAuthority, byte nSubAuthorityCount, uint nSubAuthority0, uint nSubAuthority1, uint nSubAuthority2, uint nSubAuthority3, uint nSubAuthority4, uint nSubAuthority5, uint nSubAuthority6, uint nSubAuthority7, out IntPtr pSid);

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
					var NtAuthority = new SID_IDENTIFIER_AUTHORITY(); NtAuthority.b5 = 5; //SECURITY_NT_AUTHORITY
					IntPtr AdministratorsGroup;
					if(!AllocateAndInitializeSid(ref NtAuthority, 2,
						SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS,
						0, 0, 0, 0, 0, 0,
						out AdministratorsGroup
						))
						return false;
					bool R;
					if(!CheckTokenMembership(Zero, AdministratorsGroup, out R)) R = false;
					FreeSid(AdministratorsGroup);
					return R;
				}
			}
			*/

			/// <summary>
			/// Returns true if UAC is disabled (turned off) on this Windows 7 computer.
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
		/// Calls API <msdn>GetCurrentThreadId</msdn>.
		/// </summary>
		public static int CurrentThreadId { get => Api.GetCurrentThreadId(); }

		/// <summary>
		/// Calls API <msdn>GetCurrentProcessId</msdn>.
		/// </summary>
		public static int CurrentProcessId { get => Api.GetCurrentProcessId(); }

		/// <summary>
		/// Returns current thread handle.
		/// Calls API <msdn>GetCurrentThread</msdn>.
		/// Don't need to close the handle.
		/// </summary>
		public static IntPtr CurrentThreadHandle { get => Api.GetCurrentThread(); }

		/// <summary>
		/// Returns current process handle.
		/// Calls API <msdn>GetCurrentProcess</msdn>.
		/// Don't need to close the handle.
		/// </summary>
		public static IntPtr CurrentProcessHandle { get => Api.GetCurrentProcess(); }

		/// <summary>
		/// Gets process id from handle.
		/// Returns 0 if failed. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="processHandle">Native process handle.</param>
		public static int GetProcessId(IntPtr processHandle)
		{
			return _Api.GetProcessId(processHandle);
			//speed: 250 ns
		}

		//public static Process GetProcessObject(IntPtr processHandle)
		//{
		//	int pid = GetProcessId(processHandle);
		//	if(pid == 0) return null;
		//	return Process.GetProcessById(pid); //slow, makes tons of garbage, at first gets all processes just to throw exception if pid not found...
		//}

		static partial class _Api
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern int GetProcessId(IntPtr Process);

		}
	} //Process_

}
