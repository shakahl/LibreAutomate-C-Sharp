 /exe 1
out "%u" timeGetTime()
 run "qm.exe"
run "q:\app\qm.exe" "v" "" "*" ;;qm in app (local)

 BEGIN PROJECT
 main_function  Macro1179
 exe_file  $my qm$\Macro1179.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {B1C27449-E786-42B4-BF58-F485124AA29F}
 END PROJECT
