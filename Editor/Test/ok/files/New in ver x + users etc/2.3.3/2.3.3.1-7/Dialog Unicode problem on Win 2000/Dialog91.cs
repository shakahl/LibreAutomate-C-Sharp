\Dialog_Editor
str controls = "3"
str e3
e3="ąčęėįšųūž ПФЦЭЮ"
if(!ShowDialog("" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "ansi"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 6 8 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""
