 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	CreateControl 0 "Static" "" 0 0 0 0 0 hWnd 1 _hfont
	goto g1
	
	case WM_SIZE
	 g1
	RECT r; GetClientRect hWnd &r
	MoveWindow id(1 hWnd) 0 0 r.right r.bottom 1
