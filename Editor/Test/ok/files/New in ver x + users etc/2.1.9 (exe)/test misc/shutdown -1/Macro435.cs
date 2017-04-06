AddTrayIcon
2
 shutdown -1
 clo _hwndqm
Exit
10
mes "ended"
 if(!ShowDialog("Macro435" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 134 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""
