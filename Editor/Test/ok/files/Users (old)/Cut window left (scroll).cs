dll user32 #ScrollWindow hWnd XAmount YAmount RECT*lpRect RECT*lpClipRect

int h=ShowDialog("" 0 0 0 1)
opt waitmsg 1
1
int x y cx cy
GetWinXY h x y cx cy
if(cx>=500)
	x+100
	cx-200
	ScrollWindow h -100 0 0 0
	MoveWindow h x y cx cy 1
2

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 381 137 "Form"
 1 Button 0x54030001 0x4 6 118 48 14 "OK"
 2 Button 0x54030000 0x4 58 118 48 14 "Cancel"
 3 Static 0x54000000 0x0 4 56 48 12 "Left"
 4 Static 0x54000000 0x0 168 56 48 12 "Center"
 5 Static 0x54000000 0x0 330 56 48 12 "Right"
 END DIALOG
 DIALOG EDITOR: "" 0x2010901 "" ""
