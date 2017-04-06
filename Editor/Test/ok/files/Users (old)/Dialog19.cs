\Dialog_Editor

str controls = "3"
str cb3
cb3="item 0[]item 1[]item 2"
int i; rget i "cb" "\MyAppName" 0 0; if(i>2) i=2
CB_InitDialogVariable cb3 i
if(!ShowDialog("" 0 &controls)) ret
i=val(cb3); rset i "cb" "\MyAppName" 0 0

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ComboBox 0x54230243 0x0 8 8 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010901 "" ""
