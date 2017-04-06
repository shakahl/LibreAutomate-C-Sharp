 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	CreateControl 0 "Static" 0 0 0 0 50 20 hWnd 111
	SetTimer hWnd 21990 1000 0
	goto g1
	
	case WM_TIMER
	sel wParam
		case 21990
		 g1
		_s.format("The g_var variable now is %i" g_var)
		_s.setwintext(id(111 hWnd))
	
	case WM_SIZE
	RECT r; GetClientRect(hWnd &r)
	siz r.right r.bottom id(111 hWnd)
