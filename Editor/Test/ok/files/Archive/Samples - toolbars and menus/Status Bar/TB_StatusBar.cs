 /
function# hWnd message wParam lParam

 Toolbar hook function that creates control that displays text.

sel message
	case WM_CREATE
	 CreateControl 0 "Static" "" 0 0 0 0 0 hWnd 1 ;;this would be faster but does not support colors
	CreateControl 0 "RichEdit20W" "" WS_DISABLED 0 0 0 0 hWnd 1
	goto g1
	
	case WM_SIZE
	 g1
	RECT r; GetClientRect hWnd &r
	MoveWindow id(1 hWnd) 0 0 r.right r.bottom 1
