UnloadDll("$desktop$\tcchook.dll")
__Tcc x.Compile("" "$desktop$\tcchook.dll" 2)

dll- "$desktop$\tcchook.dll" #Hook on

if Hook(1) ;;hook
	mes "Press a key and see what comes to DebugView. Don't close this message box now.[][]At first download and run DebugView."
	Hook(0) ;;unhook
else mes "failed to hook"
UnloadDll("$desktop$\tcchook.dll")

#ret

#include <windows.h>

#define EXPORT __declspec(dllexport)

LRESULT CALLBACK KeyboardProc(int code, WPARAM wParam, LPARAM lParam);

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
	hhook=SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, g_hmod, 0);
	if(!hhook) return 0;
	}

return 1;
}

LRESULT CALLBACK KeyboardProc(int code, WPARAM wParam, LPARAM lParam)
{
if(code!=HC_ACTION) goto g1;

printf("%s: key %i %s.", __func__, wParam, lParam&0x80000000 ? "released" : "pressed");

g1:
return CallNextHookEx(0, code, wParam, lParam);
}
