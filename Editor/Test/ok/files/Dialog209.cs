 /exe 1

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 16 12 194 13 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

str controls = "3"
str e3
e3="1234567890 123456789"
if(!ShowDialog(dd 0 &controls)) ret

 BEGIN PROJECT
 main_function  Dialog209
 exe_file  $my qm$\Dialog209.qmm
 flags  6
 guid  {266D0147-B1CB-48C5-8EB9-8CA14E2955E1}
 END PROJECT
