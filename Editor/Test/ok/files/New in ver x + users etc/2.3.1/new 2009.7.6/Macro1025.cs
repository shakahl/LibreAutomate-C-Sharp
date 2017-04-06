 /exe
 if(!ShowDialog("dlg_recent_menu_sample" &dlg_recent_menu_sample)) ret
 ->
if(!ShowDialog("dlg_recent_menu_sample" &dlg_recent_menu_sample 0 0 0 0 0 0 0 0 "" "dlg_recent_menu_sample")) ret


 BEGIN PROJECT
 main_function  Macro1025
 exe_file  $my qm$\Macro1025.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {1B544E5C-4C10-4A06-AC07-C16F0D666DEC}
 END PROJECT
