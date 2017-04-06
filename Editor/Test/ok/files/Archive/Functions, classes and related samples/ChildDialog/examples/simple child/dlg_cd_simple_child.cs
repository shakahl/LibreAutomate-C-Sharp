 \Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10000448 0x200 0 0 255 255 ""
 3 Button 0x54032009 0x0 2 86 72 13 "Option 1 of 3"
 4 Button 0x54002009 0x0 2 158 64 13 "Option 2 of 3"
 5 Button 0x54002009 0x0 2 240 66 13 "Option 3 of 3"
 8 Static 0x54000000 0x0 4 36 24 13 "Static"
 9 Edit 0x54030080 0x200 30 36 48 14 ""
 10 Static 0x54000003 0x0 4 4 16 16 ""
 12 Button 0x54012003 0x0 2 54 48 12 "Check"
 END DIALOG
 DIALOG EDITOR: "DCSCONTROLS843" 0x2030003 "" "" ""

 When creating a child dialog, in dialog editor unset most default styles of the dialog.
 Set only WS_VISIBLE (if you need it), DS_CONTROL, and optionally WS_EX_CLIENTEDGE or other style that adds frame.
 Parent dialog style should include WS_EX_CONTROLPARENT. Other styles can be default or as you need.
