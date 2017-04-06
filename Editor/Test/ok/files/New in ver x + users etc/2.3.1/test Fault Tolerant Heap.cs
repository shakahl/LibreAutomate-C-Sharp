 /exe
int h=GlobalAlloc(0 10000)
out h
rep 1000
	memset +h 0 10000
	GlobalFree h

 BEGIN PROJECT
 main_function  Macro1241
 exe_file  $my qm$\Macro1241-4.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {CAE76769-0AD6-4780-A06E-F7F8B30452EB}
 END PROJECT
