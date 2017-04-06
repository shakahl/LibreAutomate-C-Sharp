 /exe
 \Dialog_Editor
if(!ShowDialog("Dialog80" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030103 "" "" ""

 BEGIN PROJECT
 main_function  Dialog80
 exe_file  $my qm$\Dialog80.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D8C20BB0-412B-436E-9D56-489DEDD15DB2}
 END PROJECT
