 \Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10000048 0x0 0 0 240 137 ""
 500 QM_Edit 0x50030080 0x204 0 0 240 12 ""
 501 Edit 0x50030080 0x204 0 16 240 12 "" "Control.[]Can be child(...), id(...), control id (a number) or handle (variable).[]If 'Control' selected but this field is empty, will use the focused control in the active window."
 511 Button 0x50032009 0x4 2 30 42 18 "Window"
 512 Button 0x50002009 0x4 46 30 42 18 "Control"
 510 Button 0x50002009 0x4 90 30 42 18 "Screen"
 520 Static 0x50000003 0x4 138 31 24 21 "" ".1 Drag and drop.[]Right click for more options."
 521 Button 0x58032000 0x4 168 32 36 14 "Test"
 522 Button 0x58032000 0x4 204 32 36 14 "Edit..."
 529 Static 0x54000010 0x20000 0 52 240 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "" "" "" ""
