 /exe 2
out
 MessageBox 0 "" "" MB_TOPMOST
out wait(0 K)
 out wait(0 K)
 out wait(0 K R)
 out wait(0 K R)
 out wait(0 M)
 out wait(0 M)

 BEGIN PROJECT
 main_function  test wait K M
 exe_file  $my qm$\test wait K M.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5F6F9AE6-1EF9-489D-A453-BA8CCB9DFE40}
 END PROJECT
