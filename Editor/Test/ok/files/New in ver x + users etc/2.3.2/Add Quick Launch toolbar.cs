int w1=win("" "Shell_TrayWnd")
int w2=child("" "RebarWindow32" w1)
rig 7 13 w2
'tRn
int w3=wait(5 win("New Toolbar - Choose a folder" "#32770"))
outp "%userprofile%\Desktop\Quick Launch"
but id(1 w3)
 
int w4=child("Quick Launch" "ToolbarWindow32" w1)
rig 1 1 w4
'DDDY
rig 1 1 w4
'DDDDY
