 /exe
 \Dialog_Editor

ExeQmGridDll

str controls = "3"
str qmg3x
if(!ShowDialog("Dialog100" 0 &controls)) ret
if(!ShowDialog("Dialog100" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 87 96 48 "0x0,0,0,0,0x0[]A,,,[]B,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030307 "*" "" ""

 BEGIN PROJECT
 main_function  Dialog100
 exe_file  $my qm$\Dialog100.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {AB099E05-9E0B-476F-B479-9F90679B7915}
 END PROJECT
