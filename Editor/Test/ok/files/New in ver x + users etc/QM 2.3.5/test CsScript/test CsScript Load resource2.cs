 /exe
long t0 t1 t2; rget t0 "perf"; t1=perf
out "---"

PF
CsScript x.Init
 CsScript x.Init_UseDllExport
PN
x.Load(":44 $my qm$\test\cs.dll")
 _s.getfile("q:\my qm\test\cs.dll"); x.LoadFromMemory(_s _s.len)
 x.SetOptions("inMemoryAsm=true")
 x.Load("q:\my qm\test\cs.dll")
PN

rep 5
	_i=x.Call("Test.Add" 10 5)
	PN
PO
 out _s

t2=perf; out F"{t1-t0} {t2-t1}"; rset t2 "perf"

 BEGIN PROJECT
 main_function  test CsScript Load resource2
 exe_file  $my qm$\test CsScript Load resource2.qmm
 flags  22
 guid  {809CDEE0-B653-4AC2-A66A-CE0F73FBC857}
 END PROJECT
