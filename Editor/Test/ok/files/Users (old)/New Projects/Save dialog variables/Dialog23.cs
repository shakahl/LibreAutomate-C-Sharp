\Dialog_Editor

str controls = "3 4 5 6 7 8 9 10 11 12 13"
str e3 e4 c5Che o6Opt o7Opt o8Opt rea9 cb10 cb11 lb12 lb13
 RegDialogVariables 0 &controls "\testrdv"
RegDialogVariables 0 &controls "\testrdv" "4 5"
if(!ShowDialog("Dialog23" 0 &controls)) ret
 RegDialogVariables 1 &controls "\testrdv"
RegDialogVariables 1 &controls "\testrdv" "4 5"

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 163 "Dialog"
 1 Button 0x54030001 0x4 118 146 48 14 "OK"
 2 Button 0x54030000 0x4 170 146 48 14 "Cancel"
 3 Edit 0x54030080 0x200 8 6 96 14 ""
 4 Edit 0x54030080 0x200 8 24 96 14 ""
 5 Button 0x54012003 0x0 8 42 48 12 "Check"
 6 Button 0x54032009 0x0 8 58 48 12 "Option first"
 7 Button 0x54002009 0x0 8 72 48 12 "Option next"
 8 Button 0x54002009 0x0 8 86 48 12 "Option next"
 9 RichEdit20A 0x54233044 0x200 112 6 96 48 ""
 10 ComboBox 0x54230242 0x0 112 62 96 213 ""
 11 ComboBox 0x54230243 0x0 112 78 96 213 ""
 12 ListBox 0x54230101 0x200 6 100 50 34 ""
 13 ListBox 0x54230109 0x200 60 100 50 34 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""
