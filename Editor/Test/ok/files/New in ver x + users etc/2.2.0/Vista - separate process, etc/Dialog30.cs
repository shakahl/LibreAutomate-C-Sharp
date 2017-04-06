 /exe 1
 \Dialog_Editor
 mes 1

 run "iexplore.exe"
if(!ShowDialog("Dialog30" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80844 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

 BEGIN PROJECT
 main_function  Dialog30
 exe_file  $common documents$\my qm\Dialog30.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {A770DB64-D2B0-4136-A5FB-B346C25AA71D}
 END PROJECT
