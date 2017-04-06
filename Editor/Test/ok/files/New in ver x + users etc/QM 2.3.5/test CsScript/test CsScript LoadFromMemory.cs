/exe
 out

str sd.getfile("q:\my qm\test\cs.dll")

PF
CsScript x.Init
 PN
 ARRAY(byte) b.create(sd.len); memcpy &b[0] sd sd.len
 x.x.LoadFromMemory(b)
x.LoadFromMemory(sd sd.len)
PN

rep 5
	_s=x.Call("Test.MsgBox" 10 5)
	PN
PO
out _s

 BEGIN PROJECT
 main_function  test CsScript LoadFromMemory
 exe_file  $my qm$\test CsScript LoadFromMemory.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {9F6EC558-5578-4360-B1D0-195CBFB07670}
 END PROJECT
