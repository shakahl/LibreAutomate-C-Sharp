 /exe
 \Dialog_Editor

str controls = "3"
str c3Che
 if(!ShowDialog("Dialog124" 0 &controls)) ret
if(!ShowDialog("" 0 &controls)) ret
 if(!ShowDialog) ret
 if(!ShowDialog("wwwwwwwwwwww")) ret

 _s=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54012003 0x0 14 38 48 12 "Check"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 if(!ShowDialog(_s)) ret


 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54012003 0x0 14 38 48 12 "Check"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""

 BEGIN PROJECT
 main_function  Dialog124
 exe_file  $my qm$\Dialog124.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {A927B92D-8C0D-4432-B7F6-7614C73DBBEE}
 END PROJECT
