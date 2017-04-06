\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 656 344 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd 0 0)) ret

 BEGIN PROJECT
 main_function  Dialog165
 exe_file  $my qm$\Dialog165.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {280A8F80-65CC-4FDA-A24E-68AF7DA18069}
 END PROJECT
