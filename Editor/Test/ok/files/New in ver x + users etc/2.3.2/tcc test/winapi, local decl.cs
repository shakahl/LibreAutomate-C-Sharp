str c=
 //#include <windef.h>
 //int STDCALL MessageBoxA(int hWnd, char* lpText, char* lpCaption, int uType);
 #define __stdcall __attribute__((stdcall))
 int __stdcall MessageBoxA(int hWnd, char* lpText, char* lpCaption, int uType);
 void __stdcall OutputDebugStringA(char* s);
 void main()
 {
 OutputDebugStringA("aaa");
 MessageBoxA(0, "aaaa", "bbbb", 0);
 OutputDebugStringA("bbb");
 }

Q &q
__Tcc x.Compile(c "main" 0 "user32")
Q &qq
outq

call x.f

