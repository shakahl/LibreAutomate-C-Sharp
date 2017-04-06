\Dialog_Editor

 if(!ShowDialog("" 0 0 0 0 0 0 0 1 -100)) ret
if(!ShowDialog("" 0 0 0 64 0 0 0 1 -100)) ret

 BEGIN DIALOG
 0 "" 0x90C80244 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""

 center
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"

 no center
 0 "" 0x90C80244 0x100 0 0 223 135 "Dialog"
