str sdll="$desktop$\vbrun.dll"
UnloadDll sdll

__Tcc x.Compile("" sdll 2)

dll- "$desktop$\vbrun.dll" VbRun $program $cmdLine

out VbRun("Q:\My QM\Macro 1496.exe" "command line etc")

#ret

#include <windows.h>

__declspec(dllexport)
int VbRun(LPSTR program, LPSTR cmdLine)
{
_spawnlp(0, program, " ", cmdLine, 0);
__asm__("leave;ret $8"); //make stdcall
}
