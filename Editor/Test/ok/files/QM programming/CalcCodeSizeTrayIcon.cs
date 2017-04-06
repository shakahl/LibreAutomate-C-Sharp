 /exe
AddTrayIcon "" "calc code size" "CalcCodeSize"
wait -1

 BEGIN PROJECT
 main_function  CalcCodeSizeTrayIcon
 exe_file  $my qm$\CalcCodeSizeTrayIcon.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {F21F74C6-2930-420E-9A1F-9F558D5C76D7}
 END PROJECT
