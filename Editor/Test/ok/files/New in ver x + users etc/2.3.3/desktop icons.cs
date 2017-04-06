out

 define rectangle in desktop, top-right
RECT r
r.right=ScreenWidth; r.left=r.right/2; r.bottom=ScreenHeight/2

 find desktop listview control that contains icons
int hwnd=child("" "SysListView32" win("Program Manager" "Progman"))

 enumerate desktop icons
int i
for i 1 1000000
	Acc a=acc("A*" "LISTITEM" hwnd "" "" 0x1001 0 0 "" 0 i); err break ;;find next icon "A*" on desktop
	str s=a.Name
	 out s
	s-"$desktop$\"
	if(!dir(s 1)) continue ;;skip if it is not folder
	 out s
	int x y
	a.Location(x y)
	if(!PtInRect(&r x y)) continue ;;skip if the icon is not in the rectangle
	out s
	
	 s+"\X.txt"
	 mov s somewhere
	
