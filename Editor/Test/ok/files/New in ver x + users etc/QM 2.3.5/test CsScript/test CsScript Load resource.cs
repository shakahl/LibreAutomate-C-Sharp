 /exe
 out

PF
CsScript x.Init
PN
x.Load(":44 $my qm$\test\cs.dll")
PN

rep 5
	_i=x.Call("Test.Add" 10 5)
	PN
PO
out _i

 BEGIN PROJECT
 main_function  test CsScript Load resource
 exe_file  $my qm$\test CsScript Load resource.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  22
 guid  {0BD2594F-4BA6-4963-80DF-01F7FA14B49E}
 END PROJECT
