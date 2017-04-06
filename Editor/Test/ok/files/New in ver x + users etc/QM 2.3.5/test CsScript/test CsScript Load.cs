 /exe
 out

PF
CsScript x.Init
 PN
 x.SetOptions("debugConfig=true")
 x.SetOptions("inMemoryAsm=")
 PN
x.Load("q:\my qm\test\cs.dll")
PN

rep 5
	_s=x.Call("Test.MsgBox" 10 5)
	PN
PO
out _s

 BEGIN PROJECT
 main_function  test CsScript Load
 exe_file  $my qm$\test CsScript LoadAssembly.qmm
 flags  6
 guid  {BF6FD2E2-D54F-4122-9C52-1D3692F713B8}
 END PROJECT
