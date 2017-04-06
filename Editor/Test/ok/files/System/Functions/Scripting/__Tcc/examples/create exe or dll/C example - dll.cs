UnloadDll("$desktop$\tccdll.dll")
__Tcc x.Compile("" "$desktop$\tccdll.dll" 2)

dll- "$desktop$\tccdll.dll" #PublicFunc $s

out PublicFunc("test")
UnloadDll("$desktop$\tccdll.dll") ;;for testing

#ret

#include <windows.h>

#define EXPORT __declspec(dllexport)

int PrivateFunc()
{
	return 5;
}

EXPORT int PublicFunc(LPSTR s)
{
	if(PrivateFunc()==5)
	{
	MessageBox(0, s, __func__, MB_TOPMOST);
	}
	return 5;
}

//function DllMain is called when loading/unloading the dll, and also for threads. But it is not necessary.
BOOL __stdcall DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved)
{
	switch(fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		MessageBox(0, "loaded", __func__, MB_TOPMOST);
		break;
	case DLL_PROCESS_DETACH:
		MessageBox(0, "unloaded", __func__, MB_TOPMOST);
		break;
	}

	return 1;
}
