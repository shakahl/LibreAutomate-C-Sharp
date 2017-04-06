 /exe
 \Dialog_Editor

str controls = "3"
str sb3
sb3=":77 $desktop$\Ąč ﯔﮥ A.bmp"
if(!ShowDialog("Dialog49" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x5400100E 0x20000 6 6 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

 BEGIN PROJECT
 main_function  Dialog49
 exe_file  $my qm$\Dialog49.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {1EB80939-286F-4EC7-9C20-03728F0FF80A}
 END PROJECT
