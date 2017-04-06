/exe
 lock lock_test
lock lock_test_m "lock_test_mutex"
 lock lock_test_m "lock_test_mutex" 500; err out "timeout"
out __FUNCTION__
1
 ExitThread 0

 BEGIN PROJECT
 main_function  Function107
 exe_file  $my qm$\Function107.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {0E0384FD-CDBD-4C0D-ABAF-9B0E3083B58F}
 END PROJECT
