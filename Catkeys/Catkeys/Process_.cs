using System;
using System.Collections.Generic;
using System.Text;
//using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.ComponentModel;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
using System.IO;
using System.Security.Principal;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public static class Process_
	{
		[DllImport("kernel32.dll")]
		static extern bool QueryFullProcessImageNameW(IntPtr hProcess, bool nativeFormat, [Out] StringBuilder lpExeName, ref int lpdwSize);

		static string _GetProcessName(uint processId, bool fullPath, bool dontEnumerate = false, bool unDOS = false)
		{
			if(processId == 0) return null;
			string R = null;

			using(var ph = new ProcessHandle_(processId)) {
				if(!ph.Is0) {
					//info:
					//In non-admin process fails if the process is of another user session; then use the slooow enumeration.
					//Also fails for some system processes: nvvsvc, nvxdsync, dwm. For dwm fails even in admin process.

					bool getNormal = fullPath || unDOS; //getting native path is faster, but it gets like "\Device\HarddiskVolume5\Windows\SysWOW64\notepad.exe" and there is no API to convert to normal
					int size = 300; var sb = new StringBuilder(size);
					if(QueryFullProcessImageNameW(ph, !getNormal, sb, ref size)) {
						bool retry = false;
						g1:
						R = sb.ToString();
						if(!fullPath) R = GetFileNameWithoutExe(R);

						if(!retry && R.IndexOf('~') >= 0) { //DOS path?
							size = sb.EnsureCapacity(300);
							if(QueryFullProcessImageNameW(ph, false, sb, ref size)) {
								string s = sb.ToString();
								if(0 != Api.GetLongPathName(s, sb, (uint)sb.EnsureCapacity(300))) { retry = true; goto g1; }
							}
						}

						return R;
					}
				} else if(!dontEnumerate && !fullPath) {
					EnumProcesses(p =>
						{
							if(p.ProcessID != processId) return false;
							R = GetFileNameWithoutExe(p.ProcessName);
							//Out(R);
							return true;
						});
				}
			}

			return R;
		}

		public struct ProcessInfo
		{
			public uint SessionID;
			public uint ProcessID;
#pragma warning disable 649 //says _ProcessName never used
			IntPtr _ProcessName;
#pragma warning restore
			public IntPtr UserSid;

			/// <summary>
			/// Pocess executable file name without ".exe" extension. Not full path.
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
		static extern bool WTSEnumerateProcessesW(IntPtr serverHandle, uint reserved, uint version, out IntPtr ppProcessInfo, out uint pCount);

		[DllImport("wtsapi32.dll", SetLastError = false)]
		static extern void WTSFreeMemory(IntPtr memory);

		/// <summary>
		/// Calls callback function for each process, until the function returns true.
		/// </summary>
		/// <param name="f">Lambda etc function that is called for each process. The parameter contains process id, name, user session id and pointer to SID.</param>
		/// <param name="ofThisSession">Get processes only of this user session (skip services etc).</param>
		/// <returns></returns>
		public static bool EnumProcesses(Func<ProcessInfo, bool> f, bool ofThisSession = false)
		{
			uint sessionId = 0;
			if(ofThisSession) {
				if(!Api.ProcessIdToSessionId(Api.GetCurrentProcessId(), out sessionId)) return false;
			}

			IntPtr pp; uint n;
			if(!WTSEnumerateProcessesW(Zero, 0, 1, out pp, out n)) return false;
			try {
				unsafe
				{
					ProcessInfo* p = (ProcessInfo*)pp;
					for(int i = 1; i < n; i++) { //i=1 because the first process is inaccessible, its name is empty
						if(ofThisSession && p[i].SessionID != sessionId) continue;
						if(f(p[i])) break;
					}
				}
			} finally { WTSFreeMemory(pp); }

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
		/// Gets process executable file name without ".exe", or full path.
		/// Returns null if fails.
		/// </summary>
		/// <param name="processId">Process id. If you have a window, use its ProcessId property.</param>
		/// <param name="fullPath">Get full path. Note: Fails to get full path if the process belongs to another user session, unless current process is admin; also fails to get full path of some system processes.</param>
		/// <param name="noSlowAPI">When the fast API QueryFullProcessImageName fails, don't try to use another much slower API WTSEnumerateProcesses. Not used if fullPath is true.</param>
		public static string GetProcessName(uint processId, bool fullPath = false, bool noSlowAPI = false)
		{
			return _GetProcessName(processId, fullPath, noSlowAPI);
		}

		/// <summary>
		/// Returns list of process id of all processes whose names match processName.
		/// Returns empty list if there are no matching processes.
		/// </summary>
		/// <param name="processName">Process name. String by default is interpreted as wildcard, case-insensitive.</param>
		/// <param name="fullPath">If false, processName is filename without ".exe". If true, processName is full path. Note: Fails to get full path if the process belongs to another user session, unless current process is admin; also fails to get full path of some system processes.</param>
		public static List<uint> GetProcessesByName(WildStringI processName, bool fullPath = false)
		{
			List<uint> a = new List<uint>();
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
		///// <summary>
		///// Returns Dictionary (id, name) of all processes whose names match processName wildcard.
		///// Returns null if there are no matching processes.
		///// </summary>
		///// <param name="processName">Process name wildcard. String by default is interpreted as wildcard, case-insensitive.</param>
		///// <param name="fullPath">If false, processName is filename without ".exe". If true, processName is full path. Note: Fails to get full path if the process belongs to another user session, unless current process is admin; also fails to get full path of some system processes.</param>
		//public static Dictionary<uint, string> GetProcessesByName(WildStringI processName, bool fullPath = false)
		//{
		//	Dictionary<uint, string> a = null;
		//	EnumProcesses(p =>
		//	{
		//		string s;
		//		if(fullPath) {
		//			s = GetProcessName(p.ProcessID, true);
		//			if(s == null) return false;
		//		} else s = GetFileNameWithoutExe(p.ProcessName);

		//		if(processName.Match(s)) {
		//			if(a == null) a = new Dictionary<uint, string>();
		//			a.Add(p.ProcessID, s);
		//		}
		//		return false;
		//	});

		//	return a;
		//}

		///// <summary>
		///// Gets window process executable file name without ".exe", or full path.
		///// Returns null if fails.
		///// </summary>
		///// <param name="w">Window.</param>
		///// <param name="fullPath">Get full path. Note: Fails to get full path if the process belongs to another user session, unless current process is admin; also fails to get full path of some system processes.</param>
		//public static string GetWindowProcessName(Wnd w, bool fullPath = false)
		//{
		//	return _GetProcessName(w.ProcessId, fullPath);
		//}

		/// <summary>
		/// Removes path and ".exe" extension from file name.
		/// Does not remove other extensions.
		/// </summary>
		/// <param name="fileName">A file name or full path. Can be null.</param>
		public static string GetFileNameWithoutExe(string fileName)
		{
			if(fileName == null) return null;
			if(fileName.EndsWith_(".exe")) return Path.GetFileNameWithoutExtension(fileName);
			return Path.GetFileName(fileName);
		}
	}

	#region UacInfo

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

		~UacInfo() { _Dispose(); }

		public void Dispose()
		{
			_Dispose();
			GC.SuppressFinalize(this);
		}
		#endregion

		IntPtr _htoken;

		public IntPtr TokenHandle { get { return _htoken; } }

		/// <summary>
		/// Gets true if the last called property function failed.
		/// Normally getting properties should never fail. Only the GetOfProcess method can fail, then it returns null.
		/// </summary>
		public bool Failed { get; private set; }

		/// <summary>
		/// UacInfo.IntegrityLevel.
		/// </summary>
		public enum IL { Untrusted, Low, Medium, UIAccess, High, System, Protected, Unknown = 100 }

		/// <summary>
		/// UacInfo.Elevation.
		/// </summary>
		public enum ElevationType { Unknown, Default, Full, Limited }

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
						uint siz; ElevationType elev;
						if(!Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenElevationType, &elev, 4, out siz)) _haveElevation = 2;
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
						uint siz; uint uia;
						if(!Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenUIAccess, &uia, 4, out siz)) _haveIsUIAccess = 2;
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
		/// Returns true if the process is a Windows store app.
		/// Processes of Medium non-uiAccess integrity level cannot access/automate Windows store app windows on Windows 8 (but can on Windows 10).
		/// </summary>
		public bool IsAppContainer
		{
			get
			{
				if(WinVer < Win8) return false;
				unsafe
				{
					uint siz; uint isac;
					if(Failed = !Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenIsAppContainer, &isac, 4, out siz)) return false;
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
		///		Low - very limited rights. Used by Internet Explorer tab processes, Windows store apps.
		///		Medium - limited rights. Most processes (unless UAC turned off).
		///		UIAccess - Medium IL + can access/automate High IL windows (user interface).
		///			Note: Only the IntegrityLevelAndUIAccess property can return UIAccess. This property returns High instead (the same as in Process Explorer).
		///		High - most rights. Processes that run as administrator.
		///		System - almost all rights. Services, some system processes.
		///		Protected - undocumented. Never seen.
		///		Unknown - failed to get IL. Never seen.
		/// The IL enum member values can be used like <c>if(x.IntegrityLevel > IL.Medium) ...</c>.
		/// If UAC is turned off, most non-service processes on administrator account have High IL; on non-administrator - Medium.
		/// </summary>
		public IL IntegrityLevel
		{
			get { return _GetIntegrityLevel(false); }
		}

		/// <summary>
		/// The same as IntegrityLevel, but can return UIAccess.
		/// </summary>
		public IL IntegrityLevelAndUIAccess
		{
			get { return _GetIntegrityLevel(true); }
		}

		IL _GetIntegrityLevel(bool andUIAccess)
		{
			if(_haveIntegrityLevel == 0) {
				unsafe
				{
					uint siz;
					Api.GetTokenInformation(_htoken, Api.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, null, 0, out siz);
					if(Marshal.GetLastWin32Error() != Api.ERROR_INSUFFICIENT_BUFFER) _haveIntegrityLevel = 2;
					else {
						TOKEN_MANDATORY_LABEL* tml = (TOKEN_MANDATORY_LABEL*)Marshal.AllocHGlobal((int)siz); //TODO: don't use Marshal Alloc/Free, because in other place it was slow etc
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
			IntPtr hToken;
			if(!Api.OpenProcessToken(hProcess, Api.TOKEN_QUERY | Api.TOKEN_QUERY_SOURCE, out hToken)) return null;
			return new UacInfo(hToken);
		}

		/// <summary>
		/// Opens process access token and creates UacInfo object that holds it.
		/// Returns UacInfo object. Then you can use its properties.
		/// Returns null if failed. For example fails for services and some other processes if current process is not administrator.
		/// To get UacInfo of this process, instead use UacInfo.ThisProcess.
		/// </summary>
		/// <param name="processId">Process id. If you have a window, use its ProcessId property.</param>
		public static UacInfo GetOfProcess(uint processId)
		{
			if(processId == 0) return null;
			using(var hp = new ProcessHandle_(processId)) {
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
		///// Note: although the name incluses 'Admin', this function does not check whether the user is in Administrators group; it returns true if <c>UacInfo.ThisProcess.IntegrityLevelAndUIAccess ˃= UacInfo.IL.High</c>.
		///// If UAC is turned off, on administrator account most processes have High IL. On non-administrator account most processes always have Medium or Low IL.
		///// </summary>
		//public static bool IsAdmin
		//{
		//	get { return ThisProcess.IntegrityLevelAndUIAccess >= IL.High; }
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
					} catch { }
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
			if(WinVer >= Win8) return false; //UAC cannot be disabled
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
			} catch { }
			return r == 0;
		}
	}

	#endregion

	/// <summary>
	/// Opens and manages a process handle.
	/// Calls Api.CloseHandle in Dispose (which normally is implicitly called at the end of <c>using(...){...}</c>) or in finalizer (which is called later by the GC).
	/// </summary>
	internal sealed class ProcessHandle_ :IDisposable
	{
		IntPtr _h;

		#region IDisposable Support

		void _Dispose()
		{
			if(_h != Zero) { Api.CloseHandle(_h); _h = Zero; }
		}

		~ProcessHandle_() { _Dispose(); }

		public void Dispose()
		{
			_Dispose();
			GC.SuppressFinalize(this);
		}
		#endregion

		//public ProcessHandle_() { }

		/// <summary>
		/// Attaches a kernel handle to this new object.
		/// No exception when handle is invalid.
		/// </summary>
		/// <param name="handle"></param>
		public ProcessHandle_(IntPtr handle) { _h = handle; }

		/// <summary>
		/// Opens a process handle.
		/// Calls Api.OpenProcess.
		/// No exception when fails; use Is0.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <param name="desiredAccess">Desired access, as documented in MSDN -> OpenProcess.</param>
		public ProcessHandle_(uint processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION) { _Open(processId, desiredAccess); }

		/// <summary>
		/// Opens window's process handle.
		/// This overload is more powerful: if Api.OpenProcess fails, it tries Api.GetProcessHandleFromHwnd, which can open higher integrity level processes, but only if current process is uiAccess and desiredAccess includes only Api.PROCESS_DUP_HANDLE, Api.PROCESS_VM_OPERATION, Api.PROCESS_VM_READ, Api.PROCESS_VM_WRITE, Api.SYNCHRONIZE.
		/// No exception when fails; use Is0.
		/// </summary>
		/// <param name="w">Window.</param>
		/// <param name="desiredAccess">Desired access, as documented in MSDN -> OpenProcess.</param>
		public ProcessHandle_(Wnd w, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION) { _Open(w.ProcessId, desiredAccess, w); }

		void _Open(uint processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION, Wnd processWindow = default(Wnd))
		{
			if(processId != 0) _h = Api.OpenProcess(desiredAccess, false, processId);
			if(Is0 && !processWindow.Is0
				&& (desiredAccess & ~(Api.PROCESS_DUP_HANDLE | Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE | Api.SYNCHRONIZE)) == 0
				&& UacInfo.ThisProcess.IsUIAccess
				)
				_h = GetProcessHandleFromHwnd(processWindow);
			if(Is0) GC.SuppressFinalize(this);
		}

		[DllImport("oleacc.dll")]
		static extern IntPtr GetProcessHandleFromHwnd(Wnd hwnd);

		//void _Open(uint processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		//{
		//	if(processId != 0) _h = Api.OpenProcess(desiredAccess, false, processId);
		//	if(Is0) GC.SuppressFinalize(this);
		//}

		//public static implicit operator ProcessHandle_(IntPtr handle) { return handle == Zero ? null : new ProcessHandle_(handle); } //unsafe, because does not dispose the left-side object immediately if it already holds a handle; but it is unlikely, and not dangerous, because the handle eventually would be disposed by the finalizer.
		public static implicit operator IntPtr(ProcessHandle_ p) { return p._h; }

		public bool Is0 { get { return _h == Zero; } }

	}

	/// <summary>
	/// Allocates, writes and reads memory in other process.
	/// </summary>
	public sealed class ProcessMemory :IDisposable
	{
		ProcessHandle_ _hproc;

		#region IDisposable Support

		~ProcessMemory() { _Dispose(); }

		public void Dispose()
		{
			_Dispose();
			GC.SuppressFinalize(this);
		}

		void _Dispose()
		{
			if(_hproc == null) return;
			if(Mem != Zero) { VirtualFreeEx(_hproc, Mem, 0, MEM_RELEASE); Mem = Zero; }
			_hproc.Dispose();
			_hproc = null;
		}

		#endregion

		/// <summary>
		/// Process handle.
		/// Opened with access PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE.
		/// </summary>
		public IntPtr ProcessHandle { get { return _hproc; } }

		/// <summary>
		/// Address of memory allocated in that process. Invalid in this process.
		/// </summary>
		public IntPtr Mem { get; private set; }

		void _Alloc(uint pid, Wnd w, int nBytes)
		{
			const uint fl = PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE;
			_hproc = w.Is0 ? new ProcessHandle_(pid, fl) : new ProcessHandle_(w, fl);
			if(_hproc.Is0) { _Dispose(); throw new CatkeysException("Failed to open process handle."); }

			if(nBytes != 0) {
				Mem = VirtualAllocEx(_hproc, Zero, nBytes, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
				if(Mem == Zero) { _Dispose(); throw new CatkeysException("Failed to allocate process memory."); }
			}
		}

		/// <summary>
		/// Opens window's process handle and optionally allocates memory in that process.
		/// </summary>
		/// <param name="w">A window in that process.</param>
		/// <param name="nBytes">If not 0, allocates this number of bytes of memory in that process.</param>
		/// <exception cref="CatkeysException">Throws when fails to open process handle (usually because of UAC) or allocate memory.</exception>
		/// <remarks>This is the preferred constructor when the process has windows. It works with windows of UAC High integrity level when this process is Medium+uiAccess.</remarks>
		public ProcessMemory(Wnd w, int nBytes)
		{
			w.ValidateThrow();
			_Alloc(0, w, nBytes);
		}

		/// <summary>
		/// Opens window's process handle and optionally allocates memory in that process.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <param name="nBytes">If not 0, allocates this number of bytes of memory in that process.</param>
		/// <exception cref="CatkeysException">Thrown when fails to open process handle or allocate memory.</exception>
		public ProcessMemory(uint processId, int nBytes)
		{
			_Alloc(processId, Wnd0, nBytes);
		}

		/// <summary>
		/// Copies a string from this process to the memory allocated in that process by the constructor.
		/// In that process the string is writted as '\0'-terminated UTF-16 string. For it is used (s.Length+1)*2 bytes of memory in that process (+1 for the '\0', *2 because UTF-16 character size is 2 bytes).
		/// Returns false if fails.
		/// </summary>
		/// <param name="s">A string in this process.</param>
		/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
		public bool WriteUnicodeString(string s, int offsetBytes = 0)
		{
			if(Mem == Zero) return false;
			return WriteProcessMemoryW(_hproc, Mem + offsetBytes, s, (s.Length + 1) * 2, Zero);
		}

		/// <summary>
		/// Copies a string from this process to the memory allocated in that process by the constructor.
		/// In that process the string is writted as '\0'-terminated ANSI string. For it is used s.Length+1 bytes of memory in that process (+1 for the '\0').
		/// Returns false if fails.
		/// </summary>
		/// <param name="s">A string in this process. This function writes string converted to ANSI, not exact C# string which is Unicode UTF-16.</param>
		/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
		public bool WriteAnsiString(string s, int offsetBytes = 0)
		{
			if(Mem == Zero) return false;
			return WriteProcessMemoryA(_hproc, Mem + offsetBytes, s, s.Length + 1, Zero);
		}

		string _ReadString(bool ansiString, int nChars, int offsetBytes)
		{
			if(Mem == Zero) return null;
			var sb = new StringBuilder(nChars);
			bool ok = ansiString
				? ReadProcessMemoryA(_hproc, Mem + offsetBytes, sb, nChars, Zero)
				: ReadProcessMemoryW(_hproc, Mem + offsetBytes, sb, nChars * 2, Zero);
			return ok ? sb.ToString() : null;
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
		/// <param name="nChars">Number of characters to copy. In that process a character is 1 byte, in this process will be 2 bytes (normal C# string).</param>
		/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
		public string ReadAnsiString(int nChars, int offsetBytes = 0)
		{
			return _ReadString(true, nChars, offsetBytes);
		}

		/// <summary>
		/// Copies a value-type variable or other memory from this process to the memory in that process allocated by the constructor.
		/// Returns false if fails.
		/// </summary>
		/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
		/// <param name="nBytes">Number of bytes to copy.</param>
		/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
		public unsafe bool Write(void* ptr, int nBytes, int offsetBytes = 0)
		{
			if(Mem == Zero) return false;
			return WriteProcessMemory(_hproc, Mem + offsetBytes, ptr, nBytes, Zero);
		}

		/// <summary>
		/// Copies a value-type variable or other memory from this process to a known memory address in that process.
		/// Returns false if fails.
		/// </summary>
		/// <param name="ptrDestinationInThatProcess">Memory address in that process where to copy memory from this process.</param>
		/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
		/// <param name="nBytes">Number of bytes to copy.</param>
		public unsafe bool WriteOther(IntPtr ptrDestinationInThatProcess, void* ptr, int nBytes)
		{
			return WriteProcessMemory(_hproc, ptrDestinationInThatProcess, ptr, nBytes, Zero);
		}

		/// <summary>
		/// Copies from the memory in that process allocated by the constructor to a value-type variable or other memory in this process.
		/// Returns false if fails.
		/// </summary>
		/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
		/// <param name="nBytes">Number of bytes to copy.</param>
		/// <param name="offsetBytes">Offset in the memory allocated by the constructor.</param>
		public unsafe bool Read(void* ptr, int nBytes, int offsetBytes = 0)
		{
			if(Mem == Zero) return false;
			return ReadProcessMemory(_hproc, Mem + offsetBytes, ptr, nBytes, Zero);
		}

		/// <summary>
		/// Copies from a known memory address in that process to a value-type variable or other memory in this process.
		/// Returns false if fails.
		/// </summary>
		/// <param name="ptrSourceInThatProcess">Memory address in that process from where to copy memory.</param>
		/// <param name="ptr">Unsafe address of a value type variable or other memory in this process.</param>
		/// <param name="nBytes">Number of bytes to copy.</param>
		public unsafe bool ReadOther(IntPtr ptrSourceInThatProcess, void* ptr, int nBytes)
		{
			return ReadProcessMemory(_hproc, ptrSourceInThatProcess, ptr, nBytes, Zero);
		}

		//Cannot get pointer if generic type. Could try Marshal.StructureToPtr etc but I don't like it.
		//public unsafe bool Write<T>(ref T v, int offsetBytes = 0) where T : struct
		//{
		//	int n = Marshal.SizeOf(v.GetType());
		//	return Write(&v, n, offsetBytes);
		//	Marshal.StructureToPtr(v, m, false); ...
		//}

		[DllImport("kernel32.dll")]
		static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll")]
		static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

		[DllImport("kernel32.dll")]
		static extern unsafe bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		static extern unsafe bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
		static extern bool ReadProcessMemoryW(IntPtr hProcess, IntPtr lpBaseAddress, [Out] StringBuilder lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
		static extern bool WriteProcessMemoryW(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
		static extern bool ReadProcessMemoryA(IntPtr hProcess, IntPtr lpBaseAddress, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
		static extern bool WriteProcessMemoryA(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.LPStr)] string lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

		const uint MEM_RESERVE = 8192;
		const uint MEM_RELEASE = 32768;
		const uint MEM_COMMIT = 4096;
		const uint PAGE_EXECUTE_READWRITE = 64;
		const int PROCESS_VM_WRITE = 32;
		const int PROCESS_VM_READ = 16;
		const int PROCESS_VM_OPERATION = 8;
	};

}
