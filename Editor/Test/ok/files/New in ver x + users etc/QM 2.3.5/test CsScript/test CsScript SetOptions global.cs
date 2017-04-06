 /exe
 out

str code.getmacro("cs1")

PF
CsScript x.SetOptions(1 "noFileCache=true[]compilerOptions=/d:TRACE")
PN
x.Init
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
 main_function  test CsScript SetOptions global
 exe_file  $my qm$\test CsScript SetOptions global.qmm
 flags  6
 guid  {5324F9E8-8601-43EE-9430-F9D2EA1C235A}
 END PROJECT
