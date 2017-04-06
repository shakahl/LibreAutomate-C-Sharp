\Dialog_Editor
str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 24 20 48 13 "same"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

 if(!ShowDialog("dialog definition" 0)) ret
 if(!ShowDialog(dd 0 0 0 2)) ret
 if(!ShowDialog("" 0)) ret
 if(!ShowDialog("simple dialog" 0)) ret

#exe addtextof "simple dialog"
str s="simple dialog"
if(!ShowDialog(s 0)) ret
