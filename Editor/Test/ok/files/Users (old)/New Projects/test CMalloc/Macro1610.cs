 /exe

rep 20
	Q &q
	 rep(1000) _i=0
	rep(1000) q_free(q_malloc(100))
	Q &qq
	outq

 BEGIN PROJECT
 main_function  Macro1610
 exe_file  $my qm$\Macro1610.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {92CD6CDF-8035-41DE-9BC8-F9F58EC18E1B}
 END PROJECT
