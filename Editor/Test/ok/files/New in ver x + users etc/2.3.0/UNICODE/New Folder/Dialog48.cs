\Dialog_Editor

str controls = "3"
str rea3="&$desktop$\document.rtf"
if(!ShowDialog("Dialog48" 0 &controls)) ret
out rea3

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 0 87 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""
