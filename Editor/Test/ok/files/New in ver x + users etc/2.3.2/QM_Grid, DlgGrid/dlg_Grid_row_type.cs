\Dialog_Editor

str controls = "3"
str qmg3
qmg3=
 <0>edit,x
 <1>combo,x
 <2>check,Yes
 <7>read-only,x
 <8>edit multiline,x
 <9>combo sorted,x
 <16>edit+button,x
 <17>combo+button,x
 
if(!ShowDialog("" 0 &controls)) ret
out qmg3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x54030000 0x0 0 0 224 110 "7,0,0,2[]A,,[]B,,[]"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "*" "" ""
