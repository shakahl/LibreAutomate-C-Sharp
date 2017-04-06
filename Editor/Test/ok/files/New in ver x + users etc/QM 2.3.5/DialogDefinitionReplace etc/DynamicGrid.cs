 /Dialog_Editor
function Dlgw Dlgh $gridCSV

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 200 119 "Stats in detail"
 3 QM_Grid 0x54030000 0x0 0 0 200 94 "0x10,0,0,0,0x0[]'' '',,,[]A,,,[]B,,,[]C,,,[]D,,,"
 1 Button 0x54030001 0x0 98 102 48 14 "OK"
 2 Button 0x54030000 0x0 150 102 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030507 "" "" "" ""

DialogDefinitionReplace 2 dd 0 Dlgw Dlgh
DialogDefinitionReplace 2 dd 3 Dlgw+1 Dlgh-21
DialogDefinitionReplace 1 dd 1 Dlgw/2-55 Dlgh-19
DialogDefinitionReplace 1 dd 2 Dlgw/2-5 Dlgh-19

str controls = "3"
str qmg3x
qmg3x=gridCSV

if(!ShowDialog(dd &DynamicGrid_dialog &controls)) ret
