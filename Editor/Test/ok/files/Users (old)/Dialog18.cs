\Dialog_Editor

str controls = "3 4 5"
str c3Che e4 cb5
cb5="one[]two[]three"

rget c3Che "check" "\MyAppName"
rget e4 "edit" "\MyAppName"
int i
rget i "cb" "\MyAppName" 0 0


if(!ShowDialog("Dialog18" 0 &controls)) ret

rset c3Che "check" "\MyAppName"
rset e4 "edit" "\MyAppName"
i=val(cb5)
rset i "cb" "\MyAppName" 0 0

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 221 133 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54012003 0x0 10 8 48 12 "Check"
 4 Edit 0x54030080 0x200 8 26 96 14 ""
 5 ComboBox 0x54230243 0x0 8 48 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010901 "" ""
