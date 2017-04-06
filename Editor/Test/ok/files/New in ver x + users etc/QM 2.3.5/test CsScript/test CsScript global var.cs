/exe

PF
 #opt nowarnings 1
CsScript+ g_cs
if !g_cs.x
	g_cs.AddCode("macro:cs1")
PN
rep 7
	_i=g_cs.Call("Test.Add" 10 5)
	PN
PO
out _i
