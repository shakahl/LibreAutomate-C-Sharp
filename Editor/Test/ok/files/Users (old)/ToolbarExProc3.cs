 /Toolbar7
function# hWnd message wParam lParam

sel message
	case WM_SETCURSOR
	if(!(wParam=hWnd or GetDlgCtrlID(wParam)=9999)) ret
	sel lParam
		case WM_LBUTTONUP<<16|HTCLIENT ;;or WM_LBUTTONDOWN
		POINT p; GetCursorPos &p; ScreenToClient hWnd &p
		out "%i %i" p.x p.y
		 now launch macros (mac) depending on coordinates
	
	 case WM_CREATE
	 SetWindowLong hWnd GWL_HWNDPARENT 0
	 ont- hWnd
	 
	 case WM_MOUSEACTIVATE
	 ret 1
