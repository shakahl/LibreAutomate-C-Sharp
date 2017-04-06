 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG ;;note: in QM < 2.2.0, this message is not sent. Use WM_CREATE instead.
	
	case WM_DESTROY
	
	case WM_MOVE
	RECT r; GetWindowRect hWnd &r
	int ho=GetToolbarOwner(hWnd)
	mov r.left r.bottom ho
	
	  or this
	 case WM_WINDOWPOSCHANGED
	 WINDOWPOS* wp=+lParam
	 if(wp.flags&SWP_NOMOVE) ret
	 RECT r; GetWindowRect hWnd &r
	 int ho=GetToolbarOwner(hWnd)
	 mov r.left r.bottom ho
