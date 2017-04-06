__Tcc x2.Compile("" "main" 0 "user32[]gdi32")
call(x2.f "Hello!" 9)

#ret

#include <windows.h>

void main(LPSTR s, int i)
{
char b[1000];
_snprintf(b, 999, "%s\r\n%i", s, i);
OutputDebugString(b);
MessageBox(0, b, "C code", 0);
}

//We add user32 as a library. It is needed because MessageBox is in user32.dll.
//The other 2 functions are from msvcrt and kernel32. These libraries are always added implicitly.
//The gdi32 is not used here. Just for example. Not error if you add libraries and don't use functions from them.

//Only a minimal set of Windows API headers and libraries come with QM.
//They are from the TCC library, which uses files from MinGW: http://www.mingw.org/.
//If you need more, get MinGW's "w32api" package.

//To link with Windows system DLLs, TCC uses import definition files (.def) instead of libraries.
//You'll find more defs in the MinGW's "w32api" package.
//In the TCC website you can download a 'tiny_impdef' program to make additional .def files for any DLL.
