/exe

#if !EXE
UnloadDll("$desktop$\tcchook.dll")
#else
#exe addtextof "C example - global hook in dll2"
#endif

__Tcc x.Compile("" "$desktop$\tcchook.dll" 2)

dll- "$desktop$\tcchook.dll" #Hook on

if Hook(1) ;;hook
	mes "Move/click mouse and see what comes to DebugView. Don't close this message box now.[][]At first download and run DebugView."
	Hook(0) ;;unhook
else mes "failed to hook"
#if !EXE
UnloadDll("$desktop$\tcchook.dll")
#endif

 BEGIN PROJECT
 main_function  C example - global hook in dll2
 exe_file  $my qm$\C example - global hook in dll2.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {AC37223A-FB09-4C0F-A762-AD8C0C113484}
 END PROJECT

#ret

#include <windows.h>

#define EXPORT __declspec(dllexport)

LRESULT CALLBACK MouseProc(int code, WPARAM wParam, LPARAM lParam);

HMODULE g_hmod;

//printf for dll. You can see the text in DebugView: http://technet.microsoft.com/en-us/sysinternals/bb896647.aspx
void printf_dll(LPCSTR fmt, ...)
{
	char b[1000];
	if(!fmt || !*fmt) fmt="\n";
	_vsnprintf(b, 999, fmt, (va_list)(&fmt+1));
	OutputDebugString(b);
}
#define printf printf_dll

BOOL __stdcall DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved)
{
	switch(fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		g_hmod=hinstDLL;
		DisableThreadLibraryCalls(hinstDLL);
		break;
	case DLL_PROCESS_DETACH:
		break;
	}

	return 1;
}

EXPORT BOOL Hook(BOOL on)
{
static HHOOK hhook;
if(hhook)
	{
	UnhookWindowsHookEx(hhook);
	hhook=0;
	}
if(on)
	{
	//hhook=SetWindowsHookEx(WH_MOUSE, MouseProc, g_hmod, 0);
	hhook=SetWindowsHookEx(WH_KEYBOARD, MouseProc, g_hmod, 0);
	if(!hhook) return 0;
	}

return 1;
}

LRESULT CALLBACK MouseProc(int code, WPARAM wParam, LPARAM lParam)
{
if(code!=HC_ACTION) goto g1;

printf("%s: message %i.", __func__, wParam);

g1:
return CallNextHookEx(0, code, wParam, lParam);
}


void AllApi()
{
if(!CloseHandle) return;
if(!CreateEventA) return;
if(!CreateFileA) return;
if(!CreateThread) return;
//if(!CreateToolhelp32Snapshot) return;
if(!DeleteCriticalSection) return;
if(!EnterCriticalSection) return;
if(!ExitProcess) return;
if(!FormatMessageA) return;
if(!FreeEnvironmentStringsA) return;
if(!FreeEnvironmentStringsW) return;
if(!GetACP) return;
if(!GetCommandLineA) return;
if(!GetCPInfo) return;
if(!GetCurrentProcess) return;
if(!GetCurrentProcessId) return;
if(!GetCurrentThread) return;
if(!GetCurrentThreadId) return;
if(!GetEnvironmentStrings) return;
if(!GetEnvironmentStringsW) return;
if(!GetFileType) return;
if(!GetLastError) return;
if(!GetLocaleInfoA) return;
if(!GetLongPathNameW) return;
if(!GetModuleFileNameA) return;
if(!GetModuleFileNameW) return;
if(!GetModuleHandleA) return;
if(!GetModuleHandleW) return;
if(!GetOEMCP) return;
if(!GetProcAddress) return;
if(!GetProcessHeap) return;
if(!GetStartupInfoA) return;
if(!GetStdHandle) return;
if(!GetStringTypeA) return;
if(!GetStringTypeW) return;
if(!GetSystemTimeAsFileTime) return;
if(!GetTickCount) return;
if(!HeapAlloc) return;
if(!HeapCreate) return;
if(!HeapDestroy) return;
if(!HeapFree) return;
if(!HeapReAlloc) return;
if(!HeapSize) return;
//if(!InitializeCriticalSectionAndSpinCount) return;
//if(!InterlockedCompareExchange) return;
//if(!InterlockedDecrement) return;
//if(!InterlockedIncrement) return;
if(!IsDebuggerPresent) return;
if(!IsValidCodePage) return;
if(!LCMapStringA) return;
if(!LCMapStringW) return;
if(!LeaveCriticalSection) return;
if(!LoadLibraryA) return;
if(!lstrcatA) return;
if(!lstrcpynA) return;
if(!MapViewOfFile) return;
if(!MultiByteToWideChar) return;
if(!OpenFileMappingA) return;
if(!OpenProcess) return;
if(!OutputDebugStringA) return;
//if(!Process32FirstW) return;
//if(!Process32NextW) return;
//if(!ProcessIdToSessionId) return;
if(!PulseEvent) return;
if(!QueryPerformanceCounter) return;
if(!QueryPerformanceFrequency) return;
if(!RaiseException) return;
//if(!RtlUnwind) return;
if(!SetEvent) return;
if(!SetHandleCount) return;
if(!SetLastError) return;
if(!SetThreadPriority) return;
if(!SetUnhandledExceptionFilter) return;
if(!Sleep) return;
if(!TerminateProcess) return;
if(!TlsAlloc) return;
if(!TlsFree) return;
if(!TlsGetValue) return;
if(!TlsSetValue) return;
if(!UnhandledExceptionFilter) return;
if(!UnmapViewOfFile) return;
if(!VirtualAlloc) return;
if(!VirtualFree) return;
if(!VirtualQuery) return;
if(!WaitForSingleObject) return;
if(!WideCharToMultiByte) return;
if(!WriteFile) return;

//if(!AllowSetForegroundWindow) return;
if(!CallNextHookEx) return;
if(!CharLowerBuffW) return;
if(!CharLowerW) return;
if(!CharUpperBuffW) return;
if(!CharUpperW) return;
if(!ChildWindowFromPointEx) return;
if(!EnumChildWindows) return;
if(!FindWindowA) return;
//if(!GetAncestor) return;
if(!GetClassLongW) return;
if(!GetClassNameA) return;
if(!GetClassNameW) return;
if(!GetClientRect) return;
if(!GetDesktopWindow) return;
if(!GetDlgCtrlID) return;
if(!GetDlgItem) return;
if(!GetDoubleClickTime) return;
if(!GetForegroundWindow) return;
//if(!GetGUIThreadInfo) return;
if(!GetKeyboardLayout) return;
if(!GetKeyboardState) return;
if(!GetKeyState) return;
if(!GetMessageW) return;
if(!GetMonitorInfoA) return;
if(!GetParent) return;
if(!GetSystemMetrics) return;
if(!GetWindow) return;
if(!GetWindowLongA) return;
if(!GetWindowLongW) return;
if(!GetWindowRect) return;
if(!GetWindowTextW) return;
if(!GetWindowThreadProcessId) return;
//if(!InternalGetWindowText) return;
if(!IsCharAlphaNumericW) return;
if(!IsChild) return;
if(!IsIconic) return;
if(!IsWindow) return;
if(!IsWindowVisible) return;
if(!MapWindowPoints) return;
if(!MessageBoxA) return;
//if(!MonitorFromPoint) return;
if(!PostMessageW) return;
if(!PostThreadMessageW) return;
if(!PtInRect) return;
//if(!RealChildWindowFromPoint) return;
if(!ScreenToClient) return;
if(!SendMessageTimeoutW) return;
if(!SendNotifyMessageW) return;
if(!SetKeyboardState) return;
if(!SetWindowsHookExA) return;
if(!SetWindowsHookExW) return;
//if(!SetWinEventHook) return;
if(!ToAscii) return;
if(!ToUnicodeEx) return;
if(!UnhookWindowsHookEx) return;
//if(!UnhookWinEvent) return;
if(!WindowFromPoint) return;
if(!wvsprintfA) return;

}
