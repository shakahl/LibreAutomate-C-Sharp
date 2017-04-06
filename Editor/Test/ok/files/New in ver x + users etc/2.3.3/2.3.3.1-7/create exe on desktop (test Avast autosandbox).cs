 str se="$desktop$\test.exe"
str se="$desktop$\test.exe"
__Tcc x.Compile("" se 1)

run se "/aaa"

#ret

#include <windows.h>

//will craete GUI app if there is function WinMain
int __stdcall WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
MessageBox(0, lpCmdLine, "GUI exea", 0);
return 0;
}
