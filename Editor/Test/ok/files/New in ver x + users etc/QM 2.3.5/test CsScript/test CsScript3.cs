/exe
long t0 t1 t2; rget t0 "perf"; t1=perf
out "----"

str code.getmacro("cs1")
#exe addtextof "cs1"

 speed: 61178  42046  9663  
 speed: 64523  44844  3768  3533  9924  

PF
 rget _s "dd_pict" "Software\GinDi\QM2\Settings"
 Dir d; d.dir("%temp%\qm-csscript" 1)
 PN
CsScript x.Init
 PN;PO;ret
 CsScript x.Init_UseDllExport
 CsScript x.Init_UseQM
 x.SetOptions("noFileCache=true")
 x.SetOptions("inMemoryAsm=true")
 x.SetOptions("inMemoryAsm=")
 x.SetOptions(1 "tempDir=%downloads%\qmcs")
 x.SetOptions("debugConfig=true")
 PN
int i
for i 0 1
	 _s=code; _s.findreplace("return x;" F"return x+{i};" 4); x.AddCode(_s 0)
	x.AddCode("macro:cs1" 0)
	PN

 x.Call("Output")
rep 7
	_i=x.Call("Test.Add" 10 5)
	PN
 PN
PO
out _i

 ngen: 90 9 2.

 t2=perf; out F"{t1-t0} {t2-t1}"; rset t2 "perf"

 BEGIN PROJECT
 main_function  test CsScript3
 exe_file  $my qm$\test CsScript3.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {B3B2B91C-DA49-4602-8FDF-43F465009B4E}
 END PROJECT
