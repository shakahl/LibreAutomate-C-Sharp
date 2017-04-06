 \Dialog_Editor
function# $text [$title] [x] [y]

 Simple message box with ability to set location.
 Function returns 1 on OK, 0 on Cancel, or button id
 if you press other button. To add more buttons,
 press Run button in QM toolbar and edit the dialog.


str controls = "0 1000"
str dlg e1000

e1000=text
dlg=iif(len(title) title "QM Message")

int owner=CreateWindowEx(0 +32770 +0 0 x y 0 0 win("" "Shell_TrayWnd") 0 _hinst 0)
int r notstyle
if(getopt(nargs)>2) notstyle=DS_CENTER

r=ShowDialog("mes2" 0 &controls owner 0 0 notstyle)
DestroyWindow owner
ret r

 BEGIN DIALOG
 0 "" 0x10CC0A44 0x100 0 0 226 58 ""
 1 Button 0x54030001 0x4 8 40 48 14 "OK"
 2 Button 0x54030000 0x4 60 40 48 14 "Cancel"
 1000 Edit 0x44231844 0x4 4 4 220 32 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "" ""
