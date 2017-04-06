def _DLL_TEST_TCC "$desktop$\tccdll.dll"
 Note: To change _DLL_TEST_TCC value, need to restart QM.

 _____________________________________

 Create dll from C code below #ret.

UnloadDll(_DLL_TEST_TCC)
__Tcc x.Compile("" _DLL_TEST_TCC 2)

 _____________________________________

 When you already have dll, you can use it in macros like any other dll. Example:

dll- _DLL_TEST_TCC #TccDllFunc $s
out TccDllFunc("test")


#ret

#include <windows.h>

#define EXPORT __declspec(dllexport)
#define printf printf_qm

void printf_qm(LPSTR frm, ...)
{
	HWND h=FindWindow("QM_Editor", 0); if(!h) return;
	char b[3000];
	vsnprintf(b, sizeof(b), frm, (va_list)(&frm+1));
	SendMessage(h, WM_SETTEXT, -1, (LPARAM)b);
	return 5;
}

EXPORT int TccDllFunc(LPSTR s)
{
	MessageBox(0, s, __func__, MB_TOPMOST);
	return 5;
}

//function DllMain is called when loading/unloading the dll, and also for threads. But it is not necessary.
BOOL __stdcall DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved)
{
	switch(fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		printf("tccdll loaded");
		break;
	case DLL_PROCESS_DETACH:
		printf("tccdll unloaded");
		break;
	}

	return 1;
}
