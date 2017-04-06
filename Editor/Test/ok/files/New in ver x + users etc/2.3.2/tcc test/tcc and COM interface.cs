 Should be possible to call tcc-compiled functions using COM interface instead of call().
 Because __Tcc variable structure is like of a COM object: first member is pointer to array of function pointers.
 Currently this code does not work. Exception. Need to find why.

interface ICFunctions :IUnknown
	Func1(a $b)
	#Func2(c)

 normally these would be global
__Tcc x
ICFunctions i

x.Compile("" "")
_i=&x
memcpy &i &_i 4

i.Func1(5 "test")
out i.Func2(5)

#ret

#include <windows.h>

//IUnknown functions
int __stdcall AddRef(void*vtbl)
{
return 1;
}
int __stdcall Release(void*vtbl)
{
return 1;
}
int __stdcall QueryInterface(void*vtbl, GUID*iid, void**ppvObject)
{
return 0;
}

//ICFunctions functions
void __stdcall Func1(void*vtbl, int a, LPSTR b)
{
printf("%i %s", a, b);
}
int __stdcall Func2(void*vtbl, int c)
{
return c*2;
}
