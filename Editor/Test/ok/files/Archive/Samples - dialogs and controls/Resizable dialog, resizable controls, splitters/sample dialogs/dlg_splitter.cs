\Dialog_Editor

 Simplest dialog with splitter.

str controls = "3 4"
str lb3 e4
if(!ShowDialog("dlg_splitter" 0 &controls)) ret
 if error, forgot to run function InitSplitter

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ListBox 0x54230101 0x200 4 4 96 110 ""
 4 Edit 0x54231044 0x200 104 4 116 110 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 6 QM_Splitter 0x54000000 0x0 100 4 4 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""
