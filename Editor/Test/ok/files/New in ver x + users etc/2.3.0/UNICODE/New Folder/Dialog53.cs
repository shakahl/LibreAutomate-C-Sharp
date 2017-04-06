 /exe
 \Dialog_Editor

str controls = "3 4"
str e3 e4
if(!ShowDialog("Dialog53" 0 &controls)) ret
out e4

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 8 6 96 14 ""
 4 Edit 0x54030020 0x200 8 26 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""

 BEGIN PROJECT
 main_function  Dialog53
 exe_file  $my qm$\Dialog53.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {B797CC05-F3A4-44E5-9355-9A5657E81424}
 END PROJECT
