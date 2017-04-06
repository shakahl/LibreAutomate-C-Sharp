/Dialog_Editor
str controls = "3"
str ListBox3("&one[]two[]&three[]four")
if(!ShowDialog("DialogAccSelect" 0 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 155 57 "Form"
 1 Button 0x54030001 0x4 104 8 48 14 "OK"
 2 Button 0x54030000 0x4 104 28 48 14 "Cancel"
 3 ListBox 0x54230109 0x200 0 6 96 48 ""
 END DIALOG
