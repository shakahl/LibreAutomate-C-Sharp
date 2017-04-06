\Dialog_Editor

str controls = "4"
str e4h
if(!ShowDialog("Dialog47" 0 &controls)) ret
 if(!ShowDialog("Dialog47" 0 &controls 0 1)) ret
 mes 1

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 223 135 "ėįš Ϡ"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 20 56 48 12 "ąčę"
 4 Edit 0x54030080 0x200 86 32 96 14 "hąčęk"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "*" "" ""
