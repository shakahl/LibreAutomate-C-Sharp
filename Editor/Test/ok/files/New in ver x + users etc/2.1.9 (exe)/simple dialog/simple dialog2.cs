\Dialog_Editor

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 24 20 48 13 "same"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

if(!ShowDialog("simple dialog2" 0)) ret
