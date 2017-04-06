 /
function# hWnd message wParam lParam

int- t_flags

sel message
	case WM_CREATE
	RECT r
	GetClientRect hWnd &r
	int ty(18) bcx
	if(t_flags&1) bcx=40
	CreateControl(0 "Static" 0 0 0 ty r.right r.bottom-ty hWnd 3)
	CreateControl(0 "Static" 0 0 0 0 r.right-bcx ty hWnd 4)
	if(bcx) CreateControl(0 "Button" "End" 0 r.right-bcx 0 bcx ty hWnd 5)
	
	RECT- t_r; AdjustWindowPos hWnd &t_r 3
	SetWindowPos hWnd 0 0 0 0 0 SWP_SHOWWINDOW|SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOZORDER
	__g_mpw_hwnd=hWnd
	
	int-- t_time=timeGetTime
	SetTimer hWnd 1 1000 0
	MPW_WndProc hWnd WM_TIMER 1 0
	
	case WM_TIMER
	sel wParam
		case 1
		str s=TimeSpanToStr(timeGetTime-t_time+100*10000L)
		s-"Time: "
		s.setwintext(id(4 hWnd 1))
	
	case WM_DESTROY
	PostQuitMessage 0
	
	case WM_COMMAND
	sel wParam
		case 5
		int- t_iid
		shutdown -6 0 t_iid
	
	case WM_USER+10
	int-- t_color; t_color=lParam
	lpstr st=+wParam; _s=st; _s.setwintext(id(3 hWnd 1))
	
	case WM_CTLCOLORSTATIC
	int idc=GetDlgCtrlID(lParam)
	GetClientRect lParam &r
	SetBkMode wParam TRANSPARENT
	if idc=3 ;;text
		 draw gradient
		TRIVERTEX x1 x2
		x2.x=r.right; x2.y=r.bottom
		x2.Red=0xA000; x2.Green=0xC000; x2.Blue=0x6000
		x1.Red=0xE000; x1.Green=0xF000; x1.Blue=0xC000
		GRADIENT_RECT r1.LowerRight=1
		GdiGradientFill wParam &x1 2 &r1 1 GRADIENT_FILL_RECT_V
		SetTextColor wParam t_color
		ret GetStockObject(NULL_BRUSH)
	else ;;time
		 black, green text
		SetTextColor wParam 0x40C080
		ret GetStockObject(BLACK_BRUSH)

ret DefWindowProcW(hWnd message wParam lParam)
