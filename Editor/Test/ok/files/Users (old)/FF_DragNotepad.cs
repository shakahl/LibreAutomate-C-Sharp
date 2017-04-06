function# iid FILTER&f

if(getopt(nargs)) ;;called as filter function, on mouse-down
	if(!wintest(f.hwnd "" "Notepad")) ret
	mac iid f.hwnd ;;run itself immediately, without waiting for mouse up
	ret -1 ;;eat

 Now is called by above mac.
 While right button is pressed, moves Notepad.
int h=val(_command)
POINT p1 p2; GetCursorPos &p1
rep
	0.025
	ifk-((2)) break
	GetCursorPos &p2
	int dx(p2.x-p1.x) dy(p2.y-p1.y)
	if(!dx and !dy) continue
	p1=p2
	RECT r; GetWindowRect h &r
	mov r.left+dx r.top+dy h
		