
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 8 48 14 "&Button"
 4 Static 0x54000000 0x0 8 28 48 13 "&Text"
 5 Edit 0x54030080 0x200 60 28 96 13 ""
 6 Static 0x54000000 0x0 8 48 48 12 "Text"
 7 Edit 0x54030080 0x200 60 48 96 12 ""
 8 Static 0x54000000 0x0 8 64 48 12 "Text"
 9 ComboBox 0x54230242 0x0 60 64 96 213 ""
 10 Button 0x54012003 0x0 164 28 48 10 "Check"
 11 Button 0x54012003 0x0 164 48 48 10 "Check"
 12 Button 0x54012003 0x0 164 64 48 10 "Check"
 13 SysTreeView32 0x54030000 0x0 8 80 22 22 ""
 14 SysListView32 0x54030000 0x0 36 80 22 22 ""
 15 RichEdit20A 0x54233044 0x200 60 80 22 22 ""
 16 ComboBox 0x54230243 0x0 64 8 96 213 ""
 17 ListBox 0x54230101 0x200 88 80 24 22 ""
 18 Button 0x54032000 0x0 120 84 48 14 "Button"
 19 Button 0x54032000 0x0 172 84 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

str controls = "5 7 9 10 11 12 15 16 17"
str e5 e7 cb9 c10Che c11Che c12Che re15 cb16 lb17
if(!ShowDialog(dd 0 &controls)) ret
