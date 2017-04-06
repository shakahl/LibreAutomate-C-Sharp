 /exe
sub.Local
 sub_A.Moko
 #compile "ShowDialog"


#sub Local
out 1

#sub LocalUnused
out 2

 BEGIN PROJECT
 main_function  Macro2600
 exe_file  $my qm$\Macro2600.qmm
 flags  7
 guid  {D57981E4-39D1-4943-800D-8FB30F38C12A}
 END PROJECT

 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

if(!ShowDialog(dd 0 0)) ret
