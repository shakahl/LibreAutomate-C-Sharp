str se="$desktop$\tccwin.exe"
__Tcc x.Compile("" se 1)

run se "/aaa"

#ret

#include <windows.h>

//will craete GUI app if there is function WinMain
int __stdcall WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
MessageBox(0, lpCmdLine, "GUI exe", MB_TOPMOST);
return 0;
}

//Note that we don't add libraries. It is not necessary because __Tcc implicitly adds all the default libraries for GUI exes and dlls.
