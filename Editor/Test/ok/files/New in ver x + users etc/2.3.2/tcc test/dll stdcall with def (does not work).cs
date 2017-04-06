 This does not work. TCC does not export the functions. Don't waste time with it.


 create def file for testing. Normally you would create it with notepad. You need def file if want to export undecorated __stdcall functions.
str defText=
 LIBRARY tccdll2.dll
 EXPORTS
 PublicFunc
 PublicFunc2
str defPath.expandpath("$temp$\tccdll2.def")
defText.setfile(defPath)
defPath.getpath(defPath "")

str se="$desktop$\tccdll2.dll"
__Tcc x.Compile("" se 2 "user32[]tccdll2" "" 0 "" "" defPath)

run se

#ret

#include <windows.h>

#define EXPORT __declspec(dllexport)

int PrivateFunc()
{
return 5;
}

int __stdcall PublicFunc(LPSTR s)
{
MessageBox(0, s, "PublicFunc", 0);
return 0;
}

EXPORT
int __stdcall PublicFunc2(LPSTR s)
{
MessageBox(0, s, "PublicFunc2", 0);
return 0;
}
