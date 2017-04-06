\Dialog_Editor

 _monitor=2

if(!ShowDialog("" 0 0 _hwndqm)) ret
if(!ShowDialog("" 0 0 _hwndqm 0 0 0 0 100 -30)) ret
if(!ShowDialog("" 0 0 _hwndqm 0 0 0 0 -1280 -1024)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 134 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""
