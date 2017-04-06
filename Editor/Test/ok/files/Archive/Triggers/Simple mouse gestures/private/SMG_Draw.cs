 /
function a b c POINT&p0

int-- t_smg_h
POINT-- t_smg_pp

sel b
	case 1 ;;start
	t_smg_h=CreateWindowEx(WS_EX_TOOLWINDOW|WS_EX_TOPMOST|WS_EX_LAYERED|WS_EX_TRANSPARENT|WS_EX_NOACTIVATE "#32770" 0 WS_POPUP p0.x-200 p0.y-200 400 400 0 0 _hinst 0)
	SetLayeredWindowAttributes(t_smg_h GetSysColor(COLOR_BTNFACE) 200 3)
	hid- t_smg_h
	t_smg_pp.x=200; t_smg_pp.y=200
	SetTimer(t_smg_h 1 10 &SMG_Draw)
	
	case 2 ;;stop
	KillTimer t_smg_h 1
	DestroyWindow t_smg_h
	
	case else ;;timer
	POINT p2
	xm p2 t_smg_h 0
	if(!memcmp(&t_smg_pp &p2 sizeof(POINT))) ret
	t_smg_pp=p2
	
	int hdc=GetDC(t_smg_h)
	RECT r.right=400; r.bottom=400; FillRect(hdc &r COLOR_BTNFACE+1)
	int hpen(CreatePen(0 8 0xff0000)) oldpen(SelectObject(hdc hpen))
	MoveToEx(hdc 200 200 0); LineTo(hdc p2.x p2.y)
	DeleteObject SelectObject(hdc oldpen)
	ReleaseDC(t_smg_h hdc)
