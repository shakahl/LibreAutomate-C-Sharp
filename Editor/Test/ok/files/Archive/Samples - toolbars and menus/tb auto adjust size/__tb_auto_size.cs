 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	TB_AdjustSize hWnd 0
	SetTimer hWnd 1 1000 0 ;;delete this if autoadjusting not needed later
	
	 SetWinStyle id(9999 hWnd) TBSTYLE_WRAPABLE 2
	
	case WM_TIMER
	sel wParam
		case 1
		if(GetCapture) ret
		TB_AdjustSize hWnd 0
	
