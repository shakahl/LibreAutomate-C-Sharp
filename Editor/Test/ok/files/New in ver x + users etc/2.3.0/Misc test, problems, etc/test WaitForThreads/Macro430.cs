/exe
mac "Function64"
mac "Function67"
 2
 mac "Function64"
 mac "Function67"
WaitForThreads 0 "Function64" "Function67"
out "ok"

 BEGIN PROJECT
 main_function  Macro430
 exe_file  $my qm$\Macro430.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {10931E3A-A2F2-4560-A5A9-2EB2CA977D66}
 END PROJECT
