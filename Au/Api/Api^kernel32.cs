using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace Au.Types
{
	internal static unsafe partial class Api
	{
		[DllImport("kernel32.dll", SetLastError = true)] //note: without 'SetLastError = true' Marshal.GetLastWin32Error is unaware that we set the code to 0 etc and returns old captured error code
		internal static extern void SetLastError(int errCode);

		internal const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		internal const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
		internal const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;

		[DllImport("kernel32.dll", EntryPoint = "FormatMessageW")]
		internal static extern int FormatMessage(uint dwFlags, IntPtr lpSource, int code, uint dwLanguageId, char** lpBuffer, int nSize, IntPtr Arguments);

		[DllImport("kernel32.dll", EntryPoint = "SetDllDirectoryW", SetLastError = true)]
		internal static extern bool SetDllDirectory(string lpPathName);

		[DllImport("kernel32.dll")]
		internal static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

		[DllImport("kernel32.dll")]
		internal static extern long GetTickCount64();

		[DllImport("kernel32.dll")]
		internal static extern bool QueryUnbiasedInterruptTime(out long UnbiasedTime);

		[DllImport("kernel32.dll", EntryPoint = "CreateEventW", SetLastError = true)]
		internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetEvent(IntPtr hEvent);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		//[DllImport("kernel32.dll")]
		//internal static extern uint SignalObjectAndWait(IntPtr hObjectToSignal, IntPtr hObjectToWaitOn, uint dwMilliseconds, bool bAlertable);
		//note: don't know why, this often is much slower than setevent/waitforsingleobject.

		[DllImport("kernel32.dll")] //note: no SetLastError = true
		internal static extern bool CloseHandle(IntPtr hObject);

		//currently not used
		//[DllImport("kernel32.dll")] //note: no SetLastError = true
		//internal static extern bool CloseHandle(HandleRef hObject);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetCurrentThread();

		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentThreadId();

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentProcessId();

		[DllImport("kernel32.dll", EntryPoint = "QueryFullProcessImageNameW", SetLastError = true)]
		internal static extern bool QueryFullProcessImageName(IntPtr hProcess, bool nativeFormat, [Out] char[] lpExeName, ref int lpdwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr CreateFileMapping(IntPtr hFile, SECURITY_ATTRIBUTES lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		//[DllImport("kernel32.dll", EntryPoint = "OpenFileMappingW", SetLastError = true)]
		//internal static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, LPARAM dwNumberOfBytesToMap);

		//[DllImport("kernel32.dll", SetLastError = true)]
		//internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string name);
		//see also Util.Misc.GetModuleHandleOf(Type|Assembly).

		[DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string lpLibFileName);

		[DllImport("kernel32.dll")]
		internal static extern bool FreeLibrary(IntPtr hLibModule);

		[DllImport("kernel32.dll", BestFitMapping = false, SetLastError = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

		internal const uint PROCESS_TERMINATE = 0x0001;
		internal const uint PROCESS_CREATE_THREAD = 0x0002;
		internal const uint PROCESS_SET_SESSIONID = 0x0004;
		internal const uint PROCESS_VM_OPERATION = 0x0008;
		internal const uint PROCESS_VM_READ = 0x0010;
		internal const uint PROCESS_VM_WRITE = 0x0020;
		internal const uint PROCESS_DUP_HANDLE = 0x0040;
		internal const uint PROCESS_CREATE_PROCESS = 0x0080;
		internal const uint PROCESS_SET_QUOTA = 0x0100;
		internal const uint PROCESS_SET_INFORMATION = 0x0200;
		internal const uint PROCESS_QUERY_INFORMATION = 0x0400;
		internal const uint PROCESS_SUSPEND_RESUME = 0x0800;
		internal const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
		internal const uint PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;
		internal const uint DELETE = 0x00010000;
		internal const uint READ_CONTROL = 0x00020000;
		internal const uint WRITE_DAC = 0x00040000;
		internal const uint WRITE_OWNER = 0x00080000;
		internal const uint SYNCHRONIZE = 0x00100000;
		internal const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		internal const uint STANDARD_RIGHTS_READ = READ_CONTROL;
		internal const uint STANDARD_RIGHTS_WRITE = READ_CONTROL;
		internal const uint STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
		internal const uint STANDARD_RIGHTS_ALL = 0x001F0000;
		internal const uint TIMER_MODIFY_STATE = 0x2;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", EntryPoint = "GetFullPathNameW", SetLastError = true)]
		internal static extern int GetFullPathName(string lpFileName, int nBufferLength, [Out] char[] lpBuffer, char** lpFilePart);

		[DllImport("kernel32.dll", EntryPoint = "GetLongPathNameW", SetLastError = true)]
		internal static extern int GetLongPathName(string lpszShortPath, [Out] char[] lpszLongPath, int cchBuffer);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool ProcessIdToSessionId(int dwProcessId, out int pSessionId);

		internal const uint PAGE_NOACCESS = 0x1;
		internal const uint PAGE_READONLY = 0x2;
		internal const uint PAGE_READWRITE = 0x4;
		internal const uint PAGE_WRITECOPY = 0x8;
		internal const uint PAGE_EXECUTE = 0x10;
		internal const uint PAGE_EXECUTE_READ = 0x20;
		internal const uint PAGE_EXECUTE_READWRITE = 0x40;
		internal const uint PAGE_EXECUTE_WRITECOPY = 0x80;
		internal const uint PAGE_GUARD = 0x100;
		internal const uint PAGE_NOCACHE = 0x200;
		internal const uint PAGE_WRITECOMBINE = 0x400;

		internal const uint MEM_COMMIT = 0x1000;
		internal const uint MEM_RESERVE = 0x2000;
		internal const uint MEM_DECOMMIT = 0x4000;
		internal const uint MEM_RELEASE = 0x8000;
		internal const uint MEM_RESET = 0x80000;
		internal const uint MEM_TOP_DOWN = 0x100000;
		internal const uint MEM_WRITE_WATCH = 0x200000;
		internal const uint MEM_PHYSICAL = 0x400000;
		internal const uint MEM_RESET_UNDO = 0x1000000;
		internal const uint MEM_LARGE_PAGES = 0x20000000;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr VirtualAlloc(IntPtr lpAddress, LPARAM dwSize, uint flAllocationType = MEM_COMMIT | MEM_RESERVE, uint flProtect = PAGE_EXECUTE_READWRITE);

		[DllImport("kernel32.dll")]
		internal static extern bool VirtualFree(IntPtr lpAddress, LPARAM dwSize = default, uint dwFreeType = MEM_RELEASE);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr VirtualAllocEx(HandleRef hProcess, IntPtr lpAddress, LPARAM dwSize, uint flAllocationType = MEM_COMMIT | MEM_RESERVE, uint flProtect = PAGE_EXECUTE_READWRITE);

		[DllImport("kernel32.dll")]
		internal static extern bool VirtualFreeEx(HandleRef hProcess, IntPtr lpAddress, LPARAM dwSize = default, uint dwFreeType = MEM_RELEASE);

		[DllImport("kernel32.dll", EntryPoint = "GetFileAttributesW", SetLastError = true)]
		internal static extern System.IO.FileAttributes GetFileAttributes(string lpFileName);

		[DllImport("kernel32.dll", EntryPoint = "SetFileAttributesW", SetLastError = true)]
		internal static extern bool SetFileAttributes(string lpFileName, System.IO.FileAttributes dwFileAttributes);

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal struct WIN32_FILE_ATTRIBUTE_DATA
		{
			public System.IO.FileAttributes dwFileAttributes;
			public long ftCreationTime;
			public long ftLastAccessTime;
			public long ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
		}

		[DllImport("kernel32.dll", EntryPoint = "GetFileAttributesExW", SetLastError = true)]
		internal static extern bool GetFileAttributesEx(string lpFileName, int zero, out WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

		[DllImport("kernel32.dll", EntryPoint = "SearchPathW", SetLastError = true)]
		internal static extern int SearchPath(string lpPath, string lpFileName, string lpExtension, int nBufferLength, [Out] char[] lpBuffer, char** lpFilePart);

		internal const uint BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE = 0x1;
		internal const uint BASE_SEARCH_PATH_DISABLE_SAFE_SEARCHMODE = 0x10000;
		internal const uint BASE_SEARCH_PATH_PERMANENT = 0x8000;

		[DllImport("kernel32.dll")]
		internal static extern bool SetSearchPathMode(uint Flags);

		internal const uint SEM_FAILCRITICALERRORS = 0x1;

		[DllImport("kernel32.dll")]
		internal static extern uint SetErrorMode(uint uMode);

		[DllImport("kernel32.dll")]
		internal static extern uint GetErrorMode();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetThreadPriority(IntPtr hThread, int nPriority);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr LocalAlloc(uint uFlags, LPARAM uBytes);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr LocalFree(void* hMem);

		[DllImport("kernel32.dll", EntryPoint = "lstrcpynW")]
		internal static extern char* lstrcpyn(char* sTo, char* sFrom, int sToBufferLength);

		[DllImport("kernel32.dll", EntryPoint = "lstrcpynW")]
		internal static extern char* lstrcpyn(char* sTo, string sFrom, int sToBufferLength);

		internal struct FILETIME
		{
			public uint dwLowDateTime;
			public uint dwHighDateTime;

			public static implicit operator long(FILETIME ft) { return (long)((ulong)ft.dwHighDateTime << 32 | ft.dwLowDateTime); } //in Release faster than *(long*)&ft
		}

		[DllImport("kernel32.dll")]
		internal static extern void GetSystemTimeAsFileTime(out long lpSystemTimeAsFileTime);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);

		[DllImport("kernel32.dll")]
		internal static extern bool Wow64RevertWow64FsRedirection(IntPtr OlValue);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcessHeap();
		[DllImport("kernel32.dll")]
		internal static extern void* HeapAlloc(IntPtr hHeap, uint dwFlags, LPARAM dwBytes);
		[DllImport("kernel32.dll")]
		internal static extern void* HeapReAlloc(IntPtr hHeap, uint dwFlags, void* lpMem, LPARAM dwBytes);
		[DllImport("kernel32.dll")]
		internal static extern bool HeapFree(IntPtr hHeap, uint dwFlags, void* lpMem);

		internal const int CP_UTF8 = 65001;

		[DllImport("kernel32.dll")]
		internal static extern int MultiByteToWideChar(uint CodePage, uint dwFlags, byte* lpMultiByteStr, int cbMultiByte, char* lpWideCharStr, int cchWideChar);

		[DllImport("kernel32.dll")]
		internal static extern int WideCharToMultiByte(uint CodePage, uint dwFlags, string lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, IntPtr lpDefaultChar, int* lpUsedDefaultChar);

		[DllImport("kernel32.dll")]
		internal static extern int WideCharToMultiByte(uint CodePage, uint dwFlags, char* lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, IntPtr lpDefaultChar, int* lpUsedDefaultChar);

		internal struct STARTUPINFO
		{
			public uint cb;
			public IntPtr lpReserved;
			public IntPtr lpDesktop;
			public IntPtr lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public ushort wShowWindow;
			public ushort cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[DllImport("kernel32.dll", EntryPoint = "GetStartupInfoW")]
		internal static extern void GetStartupInfo(out STARTUPINFO lpStartupInfo);

		internal const uint FILE_READ_DATA = 0x1;
		internal const uint FILE_LIST_DIRECTORY = 0x1;
		internal const uint FILE_WRITE_DATA = 0x2;
		internal const uint FILE_ADD_FILE = 0x2;
		internal const uint FILE_APPEND_DATA = 0x4;
		internal const uint FILE_ADD_SUBDIRECTORY = 0x4;
		internal const uint FILE_CREATE_PIPE_INSTANCE = 0x4;
		internal const uint FILE_READ_EA = 0x8;
		internal const uint FILE_WRITE_EA = 0x10;
		internal const uint FILE_EXECUTE = 0x20;
		internal const uint FILE_TRAVERSE = 0x20;
		internal const uint FILE_DELETE_CHILD = 0x40;
		internal const uint FILE_READ_ATTRIBUTES = 0x80;
		internal const uint FILE_WRITE_ATTRIBUTES = 0x100;
		internal const uint FILE_ALL_ACCESS = 0x1F01FF;
		internal const uint FILE_GENERIC_READ = 0x120089;
		internal const uint FILE_GENERIC_WRITE = 0x120116;
		internal const uint FILE_GENERIC_EXECUTE = 0x1200A0;

		internal const int CREATE_NEW = 1;
		internal const int CREATE_ALWAYS = 2;
		internal const int OPEN_EXISTING = 3;
		internal const int OPEN_ALWAYS = 4;
		internal const int TRUNCATE_EXISTING = 5;

		internal const uint FILE_SHARE_READ = 0x1;
		internal const uint FILE_SHARE_WRITE = 0x2;
		internal const uint FILE_SHARE_DELETE = 0x4;
		internal const uint FILE_SHARE_ALL = 0x7;

		internal const uint GENERIC_READ = 0x80000000;
		internal const uint GENERIC_WRITE = 0x40000000;

		//internal const uint FILE_ATTRIBUTE_READONLY = 0x1;
		//internal const uint FILE_ATTRIBUTE_HIDDEN = 0x2;
		//internal const uint FILE_ATTRIBUTE_SYSTEM = 0x4;
		//internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
		//internal const uint FILE_ATTRIBUTE_ARCHIVE = 0x20;
		//internal const uint FILE_ATTRIBUTE_DEVICE = 0x40;
		internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
		//internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x100;
		//internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x200;
		//internal const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x400;
		//internal const uint FILE_ATTRIBUTE_COMPRESSED = 0x800;
		//internal const uint FILE_ATTRIBUTE_OFFLINE = 0x1000;
		//internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000;
		//internal const uint FILE_ATTRIBUTE_ENCRYPTED = 0x4000;
		//internal const uint FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x8000;
		//internal const uint FILE_ATTRIBUTE_VIRTUAL = 0x10000;
		//internal const uint FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x20000;
		//internal const uint FILE_ATTRIBUTE_EA = 0x40000;

		internal const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
		internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
		internal const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
		internal const uint FILE_FLAG_RANDOM_ACCESS = 0x10000000;
		internal const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000;
		internal const uint FILE_FLAG_DELETE_ON_CLOSE = 0x4000000;
		internal const uint FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
		internal const uint FILE_FLAG_POSIX_SEMANTICS = 0x1000000;
		internal const uint FILE_FLAG_SESSION_AWARE = 0x800000;
		internal const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x200000;
		internal const uint FILE_FLAG_OPEN_NO_RECALL = 0x100000;
		internal const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 0x80000;
		internal const uint FILE_FLAG_OPEN_REQUIRING_OPLOCK = 0x40000;

		[DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, SECURITY_ATTRIBUTES lpSecurityAttributes, int creationDisposition, uint dwFlagsAndAttributes = FILE_ATTRIBUTE_NORMAL, IntPtr hTemplateFile = default);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool WriteFile(SafeFileHandle hFile, void* lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, OVERLAPPED* lpOverlapped = null);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool ReadFile(SafeFileHandle hFile, void* lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, OVERLAPPED* lpOverlapped = null);

		internal struct OVERLAPPED
		{
			public LPARAM Internal;
			public LPARAM InternalHigh;

			[StructLayout(LayoutKind.Explicit)]
			internal struct TYPE_2
			{
				internal struct TYPE_1
				{
					public uint Offset;
					public uint OffsetHigh;
				}
				[FieldOffset(0)] public TYPE_1 _1;
				[FieldOffset(0)] public IntPtr Pointer;
			}
			public TYPE_2 _3;
			public IntPtr hEvent;
		}

		internal struct BY_HANDLE_FILE_INFORMATION
		{
			public uint dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public uint dwVolumeSerialNumber;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint nNumberOfLinks;
			public uint nFileIndexHigh;
			public uint nFileIndexLow;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		internal const int FILE_BEGIN = 0;
		internal const int FILE_CURRENT = 1;
		internal const int FILE_END = 2;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetFilePointerEx(SafeFileHandle hFile, long liDistanceToMove, long* lpNewFilePointer, uint dwMoveMethod);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool SetEndOfFile(SafeFileHandle hFile);

		[DllImport("kernel32.dll", EntryPoint = "CreateMailslotW", SetLastError = true)]
		internal static extern SafeFileHandle CreateMailslot(string lpName, uint nMaxMessageSize, uint lReadTimeout, SECURITY_ATTRIBUTES lpSecurityAttributes);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetMailslotInfo(SafeFileHandle hMailslot, uint* lpMaxMessageSize, out int lpNextSize, out int lpMessageCount, uint* lpReadTimeout = null);

		internal struct SYSTEMTIME
		{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMilliseconds;
		}

		[DllImport("kernel32.dll")]
		internal static extern void GetLocalTime(out SYSTEMTIME lpSystemTime);

		[DllImport("kernel32.dll")]
		internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref int AppModelIDLength, [Out] char[] sbAppUserModelID);

		[DllImport("kernel32.dll", EntryPoint = "GetEnvironmentVariableW", SetLastError = true)]
		internal static extern int GetEnvironmentVariable(string lpName, [Out] char[] lpBuffer, int nSize);

		[DllImport("kernel32.dll", EntryPoint = "ExpandEnvironmentStringsW")]
		internal static extern int ExpandEnvironmentStrings(string lpSrc, [Out] char[] lpDst, int nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int GetProcessId(IntPtr Process);

		internal struct WIN32_FIND_DATA
		{
			public System.IO.FileAttributes dwFileAttributes;
			public Api.FILETIME ftCreationTime;
			public Api.FILETIME ftLastAccessTime;
			public Api.FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			public fixed char cFileName[260];
			public fixed char cAlternateFileName[14];

			internal unsafe string Name
			{
				get
				{
					fixed (char* p = cFileName) {
						if(p[0] == '.') {
							if(p[1] == '\0') return null;
							if(p[1] == '.' && p[2] == '\0') return null;
						}
						return new string(p);
					}
				}
			}
		}

		[DllImport("kernel32.dll", EntryPoint = "FindFirstFileW", SetLastError = true)]
		internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll", EntryPoint = "FindNextFileW", SetLastError = true)]
		internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll")]
		internal static extern bool FindClose(IntPtr hFindFile);

#if TEST_FINDFIRSTFILEEX
			internal enum FINDEX_INFO_LEVELS
			{
				FindExInfoStandard,
				FindExInfoBasic,
				FindExInfoMaxInfoLevel
			}

			internal const uint FIND_FIRST_EX_LARGE_FETCH = 0x2;

			[DllImport("kernel32.dll", EntryPoint = "FindFirstFileExW")]
			internal static extern IntPtr FindFirstFileEx(string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, out WIN32_FIND_DATA lpFindFileData, int fSearchOp, IntPtr lpSearchFilter, uint dwAdditionalFlags);
#endif

		internal const uint MOVEFILE_REPLACE_EXISTING = 0x1;
		internal const uint MOVEFILE_COPY_ALLOWED = 0x2;
		internal const uint MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;
		internal const uint MOVEFILE_WRITE_THROUGH = 0x8;
		internal const uint MOVEFILE_CREATE_HARDLINK = 0x10;
		internal const uint MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20;

		[DllImport("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
		internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

		//[DllImport("kernel32.dll", EntryPoint = "CopyFileW", SetLastError = true)]
		//internal static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

		internal const uint COPY_FILE_FAIL_IF_EXISTS = 0x1;
		internal const uint COPY_FILE_RESTARTABLE = 0x2;
		internal const uint COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x4;
		internal const uint COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x8;
		internal const uint COPY_FILE_COPY_SYMLINK = 0x800;
		internal const uint COPY_FILE_NO_BUFFERING = 0x1000;

		[DllImport("kernel32.dll", EntryPoint = "CopyFileExW", SetLastError = true)]
		internal static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, IntPtr lpData, int* pbCancel, uint dwCopyFlags);

		internal delegate uint LPPROGRESS_ROUTINE(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, uint dwStreamNumber, uint dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

		[DllImport("kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true)]
		internal static extern bool DeleteFile(string lpFileName);

		[DllImport("kernel32.dll", EntryPoint = "RemoveDirectoryW", SetLastError = true)]
		internal static extern bool RemoveDirectory(string lpPathName);

		[DllImport("kernel32.dll", EntryPoint = "CreateDirectoryW", SetLastError = true)]
		internal static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes); //ref SECURITY_ATTRIBUTES

		[DllImport("kernel32.dll", EntryPoint = "CreateDirectoryExW", SetLastError = true)]
		internal static extern bool CreateDirectoryEx(string lpTemplateDirectory, string lpNewDirectory, IntPtr lpSecurityAttributes); //ref SECURITY_ATTRIBUTES

		[DllImport("kernel32.dll", EntryPoint = "GlobalAddAtomW")]
		internal static extern ushort GlobalAddAtom(string lpString);

		[DllImport("kernel32.dll")]
		internal static extern ushort GlobalDeleteAtom(ushort nAtom);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool ReadProcessMemory(HandleRef hProcess, IntPtr lpBaseAddress, void* lpBuffer, LPARAM nSize, LPARAM* lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool WriteProcessMemory(HandleRef hProcess, IntPtr lpBaseAddress, void* lpBuffer, LPARAM nSize, LPARAM* lpNumberOfBytesWritten);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static IntPtr CreateActCtx(ref ACTCTX actctx);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static bool DeactivateActCtx(int dwFlags, IntPtr lpCookie);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static bool GetCurrentActCtx(out IntPtr handle);

		internal const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;
		internal const int ACTCTX_FLAG_RESOURCE_NAME_VALID = 0x008;

		internal struct ACTCTX
		{
			public uint cbSize;
			public uint dwFlags;
			public string lpSource;
			public ushort wProcessorArchitecture;
			public ushort wLangId;
			public IntPtr lpAssemblyDirectory;
			public IntPtr lpResourceName;
			public IntPtr lpApplicationName;
			public IntPtr hModule;
		}

		//TODO: remove these if unused

		[DllImport("kernel32.dll")]
		internal static extern bool SwitchToThread();

		internal const ushort ALL_PROCESSOR_GROUPS = 0xFFFF;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern uint GetMaximumProcessorCount(ushort GroupNumber);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetProcessAffinityMask(IntPtr hProcess, out LPARAM lpProcessAffinityMask, out LPARAM lpSystemAffinityMask);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern LPARAM SetThreadAffinityMask(IntPtr hThread, LPARAM dwThreadAffinityMask);


		internal const uint THREAD_QUERY_LIMITED_INFORMATION = 0x800;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, int dwThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

		internal const uint GMEM_FIXED = 0x0;
		internal const uint GMEM_MOVEABLE = 0x2;
		internal const uint GMEM_ZEROINIT = 0x40;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GlobalAlloc(uint uFlags, LPARAM dwBytes);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GlobalFree(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GlobalUnlock(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern LPARAM GlobalSize(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetFileSizeEx(SafeFileHandle hFile, out long lpFileSize);


	}
}
