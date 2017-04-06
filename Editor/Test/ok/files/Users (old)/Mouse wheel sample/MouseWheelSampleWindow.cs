/Dialog_Editor

str controls = "3 4"
str e3 r4

e3="Up/Down[]2[]3[]4[]5[]6[]7[]8[]9"
r4="Scrollbar[]2[]3[]4[]5[]6[]7[]8[]9"

if(!ShowDialog("MouseWheelSampleWindow" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 155 70 "Mouse wheel sample"
 1 Button 0x54030001 0x4 106 4 48 14 "OK"
 3 Edit 0x54231044 0x204 4 4 96 30 ""
 4 RICHEDIT 0x54231044 0x204 4 36 96 30 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010200
