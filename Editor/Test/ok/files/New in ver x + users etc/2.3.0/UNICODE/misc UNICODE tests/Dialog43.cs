/exe
\Dialog_Editor

if(!ShowDialog("Dialog43" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialogąčę"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 6 8 48 12 "Textąčę"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

 BEGIN PROJECT
 main_function  Dialog43
 exe_file  $my qm$\Dialog43.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {CCCF8BBE-8E53-4918-AFF7-97F6DE399076}
 END PROJECT
