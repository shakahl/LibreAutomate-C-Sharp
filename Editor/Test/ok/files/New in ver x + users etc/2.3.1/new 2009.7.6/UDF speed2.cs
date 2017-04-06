/exe
out
0.1
rep 20
	int t1=perf
	rep 1000
		continue
	int t2=perf
	rep 1000
		Function_0
	int t3=perf
	rep 1000
		Function_2 1 1
		 Function_2 _i &_i
	int t4=perf
	out "%i %i %i" t2-t1 t3-t2 t4-t3

 26 150 205
 26 104 156  without ECS
 26 95 145  without ECS
 26 93 144
 25 85 140

 BEGIN PROJECT
 main_function  UDF speed2
 exe_file  $my qm$\UDF speed2.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {C2E8EBF1-B887-45B5-8CC8-AF9AFF59C0D5}
 END PROJECT
