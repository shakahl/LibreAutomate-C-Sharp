function hctrl ctrl updown

 hctrl: handle of active control
 ctrl: 1 - active is control 1, 2 - control 2
 updown: 1 - up, 2 - down

sel ctrl
	case 1
	sel updown
		case 1 key U
		case 2 key D
	case 2
	 PostMessage hctrl WM_VSCROLL updown-1 0
	RECT r; GetWindowRect(hctrl &r)
	int x y
	x=r.right-10
	if(updown=1) y=r.top+10
	else y=r.bottom-10 ;;30 if there is horizontal scrollbar too
	SetCursorPos(x y)
	lef
