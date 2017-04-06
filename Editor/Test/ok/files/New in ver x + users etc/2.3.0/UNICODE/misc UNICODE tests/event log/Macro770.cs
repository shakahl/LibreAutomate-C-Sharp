/exe
Wsh.WshShell sh._create
VARIANT v=4
rep 3
	int h=RegisterEventSource(0 "QM")
	lpstr s1("msg1") s2("msg2")
	ReportEvent h EVENTLOG_INFORMATION_TYPE 1 1 0 2 0 &s1 0
	DeregisterEventSource h

	 sh.LogEvent(v "message Ąčę" "")

 BEGIN PROJECT
 main_function  Macro770
 exe_file  $my qm$\Macro770.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {386D926D-252E-45B2-BEE1-EE3C8F2D9DE4}
 END PROJECT
