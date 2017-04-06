 /
function# hWnd message wParam lParam

type TBATC hOwner prevx prevy
TBATC* t=+GetProp(hWnd "TBATC")

sel message
	case WM_INITDIALOG
	SetProp hWnd "TBATC" t._new
	t.hOwner=GetToolbarOwner(hWnd)
	SetTimer hWnd 1 100 0
	
	case WM_DESTROY
	t._delete; RemoveProp(hWnd "TBATC")
	
	case WM_TIMER
	if(!t or win!t.hOwner) ret
	
	RECT r rr
	GetWindowRect hWnd &r
	GetWindowRect t.hOwner &rr
	if(r.top<rr.top+20) ret
	
	int x y cx cy
	if(!GetCaretXY(x y cx cy)) ret
	if(x!t.prevx or y!t.prevy)
		mov x-5 y+cy+iif(cy 10 30) hWnd
		t.prevx=x; t.prevy=y
