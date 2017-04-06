 /exe
 \Dialog_Editor

_i=&Drag

str controls = "3"
str si3
si3=":2 notepad.exe,0"
if(!ShowDialog("Dialog113" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000003 0x0 10 18 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 BEGIN PROJECT
 main_function  Dialog113
 exe_file  $my qm$\Dialog113.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  $desktop$\test\qm.res
 version_csv  FileVersion,8.5
 flags  22
 guid  {54814887-BCE7-42E0-9365-ADC56EE52DC8}
 END PROJECT
