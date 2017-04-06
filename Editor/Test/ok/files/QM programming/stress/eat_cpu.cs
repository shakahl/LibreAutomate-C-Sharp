/exe
function flags ;;flags: 1 end "eat_cpu" threads instead

if(flags&1) shutdown -6 0 "eat_cpu"; ret

 AddTrayIcon
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_LOWEST
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL
rep
	ifk(Ax) ret ;;Alt+X
	
 BEGIN PROJECT
 main_function  eat_cpu
 exe_file  $my qm$\eat_cpu.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {9F9B9607-2043-4627-A854-BA85B128589A}
 END PROJECT
