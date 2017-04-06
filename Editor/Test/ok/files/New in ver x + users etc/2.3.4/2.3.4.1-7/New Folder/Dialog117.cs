 /exe
\Dialog_Editor

if(!ShowDialog("Dialog117" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030404 "*" "" "" ""

 BEGIN PROJECT
 main_function  Dialog117
 exe_file  $my qm$\Dialog117.qmm
 flags  6
 guid  {663AC40F-5EC8-4652-BB41-6402DD4F897E}
 END PROJECT
