/exe
 lock lock_test
lock lock_test_m "lock_test_mutex"
 lock lock_test_m "lock_test_mutex" 500; err out "timeout"
out __FUNCTION__
1

 BEGIN PROJECT
 main_function  Function108
 exe_file  $my qm$\Function108.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {3E9DF673-C839-47C5-AB86-1E34D0FA386F}
 END PROJECT
