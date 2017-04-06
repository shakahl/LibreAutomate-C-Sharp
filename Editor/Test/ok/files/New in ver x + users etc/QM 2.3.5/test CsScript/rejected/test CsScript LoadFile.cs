/exe
 out

PF
CsScript x.Init
PN
x.x.LoadFile(_s.expandpath("$qm$\hello.cs"))
PN
rep 5
	_i=x.Call("Script.Test")
	PN
PO
 out _i

 BEGIN PROJECT
 main_function  test CsScript LoadFile
 exe_file  $my qm$\test CsScript LoadFile.qmm
 flags  6
 guid  {9671D050-0920-4528-A544-D8640496910D}
 END PROJECT
