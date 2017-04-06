
 NOTE: CreateRemoteThread fails if the target process is 64-bit, therefore this method of injecting code is useless. To inject dll, use getmsg hook and 64-bit exe/dll.

__Tcc x
int* a=x.Compile("" "func2[]__EndOfCode" 0 "user32")
 out call(x.f 4)

 call(x.f 4)
 ret

 int w=_hwndqm
int w=win("Untitled - Notepad" "Notepad")
 int w=win("" "Shell_TrayWnd")
 int w=win("Visual")
 int w=win("Firefox")
 int w=win("Dreamweaver")
 int w=win("Internet Explorer")
Q &q
int codeLen=a[1]-a[0]
__ProcessMemory m.Alloc(w codeLen 0)
m.Write(+a[0] codeLen)
Q &qq

__Handle th=CreateRemoteThread(m.hprocess 0 0 m.address 0 0 &_i)
Q &qqq
wait 0 H th
Q &qqqq
outq
if(GetExitCodeThread(th &_i)) out _i
if(GetWindowThreadProcessId(w &_i)) out _i

#ret

#include <windows.h>

int func2(int x)
{
char s[]={'t',0};
//__asm__("int $3");
FARPROC f;
//f=(FARPROC)MessageBox;
//(f)(0, s, s, MB_TOPMOST);
f=(FARPROC)GetCurrentProcessId;
int r=(f)();
__asm__("movl %0,%%eax; leave;ret $4" : : "g" (r)); //return r
}

void __EndOfCode(){}
