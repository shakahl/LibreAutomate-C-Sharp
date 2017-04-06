\Dialog_Editor

_monitor=2

int h=ShowDialog("" 0 0 _hwndqm 1)
 int h=ShowDialog("" 0 0 _hwndqm 0 0 0 0 100 -30)
opt waitmsg 1
0 -WC h

 BEGIN DIALOG
 0 "" 0x90C80245 0x100 0 0 223 134 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""
