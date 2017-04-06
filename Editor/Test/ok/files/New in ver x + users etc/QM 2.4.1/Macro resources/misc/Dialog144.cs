 \Dialog_Editor
 /exe

if(!ShowDialog("Dialog144" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""

 BEGIN PROJECT
 main_function  Dialog144
 exe_file  $my qm$\Dialog144.qmm
 flags  6
 guid  {79D2E10A-70B4-452C-B2BD-2026075E99C0}
 END PROJECT
