out
DisassembleObj "Q:\app\Release\TestObj.obj" "Q:\Downloads\objconv\TestObj.s"
if(!AssembleElf("Q:\Downloads\objconv\TestObj.s" "Q:\Downloads\objconv\TestObj.o")) ret

 dll "qm.exe" #GetSehPrologEpilog epilog
 ARRAY(int) af.create(2)
 af[0]=GetSehPrologEpilog(0)
 af[1]=GetSehPrologEpilog(1)

__Tcc2 x
#if 0
 int* f=x.Compile(":Q:\Downloads\objconv\TestObj.o" "TestK" 0 "user32")
int* f=x.Compile("int Te(){return TestK2();}" "Te" 0 "Q:\Downloads\objconv\TestObj.o[]user32")
 int* f=x.Compile(":Q:\Downloads\objconv\TestObj.o" "TestK" 0 "user32" "_SEH_prolog[]_SEH_epilog" &af[0])

 outx f[0]
 outb +f[0] 20

_hresult=100
out call(f[0])

#else
UnloadDll "Q:\Downloads\objconv\test.dll"
 x.Compile("" "Q:\Downloads\objconv\test.dll" 2)
 x.Compile(":Q:\Downloads\objconv\TestObj.o" "Q:\Downloads\objconv\test.dll" 2)
x.Compile("__declspec(dllexport)int UdfTestK(){ return TestK2(); }" "Q:\Downloads\objconv\test.dll" 2 "Q:\Downloads\objconv\TestObj.o")

 x.Compile("" "Q:\Downloads\objconv\TestObj-TCC.o" 3)
 x.Compile(":Q:\Downloads\objconv\TestObj-TCC.o" "Q:\Downloads\objconv\test.dll" 2)

 dll- "Q:\Downloads\objconv\test.dll" #TestK
dll- "Q:\Downloads\objconv\test.dll" #UdfTestK
_hresult=100
 out TestK
out UdfTestK
 out GetCurrentProcessId


#ret
__declspec(dllexport)
int TestK()
{
	//return strlen("test");
	//return strlen("test")+GetCurrentProcessId();
	return GetCurrentProcessId()+1;
	//return 7;//GetTickCount()+strlen("strii")+PrivFunc(2, 3);
}
