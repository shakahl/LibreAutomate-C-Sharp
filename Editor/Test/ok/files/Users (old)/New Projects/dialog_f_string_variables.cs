 \Dialog_Editor

int x=20
str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dial"
 3 Static 0x54000000 0x0 2 6 48 10 "Text"
 4 Edit 0x54030080 0x200 {x}=52 4 96 14 ""
 5 Static 0x54000000 0x0 2 24 90 10 "Text bb ghghg ghghghg"
 6 Edit 0x54030080 0x200 96 22 96 14 ""
 7 QM_Grid 0x56031041 0x200 2 42 96 48 "0x0,0,0,0,0x0[]A,,,[]B,,,"
 8 Edit 0x54200844 0x20000 100 42 120 50 "Some looooooooooooooooooooooooooooooooooo nnnnnnnnnnnnnnnnnnnnnnnnnnn ggggggggggggggggggg[]Text"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030509 "*" "" "" ""

str controls = "4 6 7 8"
str e4 e6 qmg7x e8Som
if(!ShowDialog(dd 0 &controls)) ret
