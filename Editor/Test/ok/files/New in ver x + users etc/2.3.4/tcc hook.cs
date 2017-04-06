 /exe 1
#exe addtextof "tcc hook"
__Tcc x.Compile("" "$temp$\qm\tcchook.dll" 2)

int hm=LoadLibrary(_s.expandpath("$temp$\qm\tcchook.dll"))
int Hook=GetProcAddress(hm "Hook")

if call(Hook 1) ;;hook
	mes "Press a key and see what comes to DebugView. Don't close this message box now.[][]At first download and run DebugView."
	call(Hook 0) ;;unhook
else mes "failed to hook"

FreeLibrary hm

 BEGIN PROJECT
 main_function  tcc hook
 exe_file  $my qm$\C example - global hook in dll2.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {2189527B-084C-4F6E-917E-3FD8C803D771}
 END PROJECT

#ret

#include <windows.h>

#define EXPORT __declspec(dllexport)

LRESULT CALLBACK HookProc(int code, WPARAM wParam, LPARAM lParam);

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

BOOL __stdcall AllowSetForegroundWindow(DWORD dwProcessId);

void ASFW()
{
//static BOOL (__stdcall*AllowSetForegroundWindow)(DWORD dwProcessId);
//AllowSetForegroundWindow=GetProcAddress(GetModuleHandle("user32"), "AllowSetForegroundWindow");
//printf("%i", AllowSetForegroundWindow);

printf("ASFW: %i", AllowSetForegroundWindow(-1));
}

BOOL __stdcall DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved)
{
	switch(fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		g_hmod=hinstDLL;
		DisableThreadLibraryCalls(hinstDLL);
		printf("Attached to: %s", GetCommandLine());
		ASFW();
		break;
	case DLL_PROCESS_DETACH:
		break;
	}

	return 1;
}

HWND __stdcall GetShellWindow();

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
	//DWORD tid=GetWindowThreadProcessId(GetShellWindow(), 0);
	DWORD tid=GetWindowThreadProcessId(FindWindow("Shell_TrayWnd", 0), 0);
	if(!tid) return 0;
	hhook=SetWindowsHookEx(WH_GETMESSAGE, HookProc, g_hmod, tid);
	if(!hhook) return 0;
	}

return 1;
}

LRESULT CALLBACK HookProc(int code, WPARAM wParam, LPARAM lParam)
{
if(code!=HC_ACTION) goto g1;

//printf("%s: %i %i.", __func__, wParam, lParam);
ASFW();

g1:
return CallNextHookEx(0, code, wParam, lParam);
}
