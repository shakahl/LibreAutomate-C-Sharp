\Dialog_Editor

str controls = "3 4"
str e3 cb4
if(!ShowDialog("Dialog130" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 8 11 96 14 "" "Edit"
 4 ComboBox 0x54230242 0x0 8 28 96 213 "" ".2 Combo"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x203050A "*" "" "" ""
