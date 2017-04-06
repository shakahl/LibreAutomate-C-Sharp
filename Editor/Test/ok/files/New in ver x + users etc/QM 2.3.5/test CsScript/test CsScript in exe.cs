 /exe
 out

str code.getmacro("cs1")

PF
CsScript x.Init
PN
x.AddCode(code 0)
PN
rep 5
	_i=x.Call("Test.Add" 10 5)
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript in exe
 exe_file  $my qm$\test CsScript.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {2DE0E228-9E11-49A0-A758-C46129860A09}
 END PROJECT
