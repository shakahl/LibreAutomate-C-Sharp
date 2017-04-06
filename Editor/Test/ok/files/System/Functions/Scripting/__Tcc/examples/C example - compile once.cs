 Although C code is fast, compiling it is quite slow.
 Especially when using big header files, like windows.h.
 You can store code in a global or thread __Tcc variable, and compile only 1 time.
 Run this macro several times to see how it works.

PerfFirst ;;measure speed

int compileOnce=32 ;;remove =32 if want to recompile always, eg when editing C code
__Tcc+ g_tcc_529
if !g_tcc_529.f or !compileOnce
	g_tcc_529.Compile("" "main" 0|compileOnce "user32")

PerfNext; PerfOut ;;measure speed

outw call(g_tcc_529.f)

#ret

#include <windows.h>

void main()
{
HWND h=FindWindow("QM_editor", 0);
}
