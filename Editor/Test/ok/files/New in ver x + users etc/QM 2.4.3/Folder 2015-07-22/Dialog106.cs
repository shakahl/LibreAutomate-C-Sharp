\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ListBox 0x54230109 0x200 0 0 96 48 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3"
str lb3
lb3="&one[]&two"
if(!ShowDialog(dd 0 &controls)) ret
out lb3
