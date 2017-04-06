str sdll="$desktop$\stdcall.dll"
UnloadDll sdll

__Tcc x.Compile("" sdll 2)

dll- "$desktop$\stdcall.dll"
	#exported_Func a b
	#exported_Func2 a b
	#VbRun $program [$cmdLine]

out exported_Func(1 2)
out exported_Func2(1 2)
 out VbRun("notepad.exe")

#ret

#include <windows.h>

//Shows how to export dll functions with stdcall calling convention without name decoration. Such functions can be used in VB.

//Create 2 functions.
//The first (private) function contains real code.
int real_Func(int a, int b)
{
return a+b;
}

//The second (exported) function calls it and uses asm to make it stdcall.
__declspec(dllexport)
int exported_Func(int a, int b)
{
real_Func(a,b);
__asm__("leave;ret $8"); //make stdcall; 8 is size of all parameters (4+4)
}

//Alternatively, place code in same function, store the return value in a variable and return it with asm.
__declspec(dllexport)
int exported_Func2(int a, int b)
{
int r=a+b;
__asm__("movl %0,%%eax; leave;ret $8" : : "g" (r)); //return r
}

//Also don't need 2 functions if the function returns a dll function.
__declspec(dllexport)
int VbRun(LPSTR program, LPSTR cmdLine)
{
_spawnlp(0, program, " ", cmdLine, 0);
__asm__("leave;ret $8"); //make stdcall
}
