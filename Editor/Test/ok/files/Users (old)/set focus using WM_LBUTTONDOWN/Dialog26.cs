 \Dialog_Editor

str controls = "3"
str rea3
if(!ShowDialog("" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 134 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 4 4 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""
