 /exe
out

str code.getmacro("cs1")

PF
CsScript x.Init
PN
  debugConfig = true 
str s=
  noFileCache = true 
 inMemoryAsm=
 compilerOptions=/d:TRAC,DEBU
 altCompiler=alt
 references=defref
 searchDirs = sdirs
 appDirs=csscript; debug;

 tempDir temp

PF
rep 1
	x.SetOptions(s 1)
PN; PO
 x.SetOptions("noFileCache=true[]compilerOptions=/d:TRACE")
ret

PN
x.AddCode(code 0)
PN
rep 5
	 _i=x.Call("Test.Add" 10 5)
	x.Call("Output")
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript SetOptions
 exe_file  $my qm$\test CsScript SetOptions.qmm
 flags  6
 guid  {B21834C2-2303-4327-9E35-CDE9492C6B6F}
 END PROJECT
