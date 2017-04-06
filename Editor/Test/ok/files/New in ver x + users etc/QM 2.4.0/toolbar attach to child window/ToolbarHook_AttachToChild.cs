 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_INITDIALOG
	SetTimer hWnd 1 500 0
	
	case WM_TIMER
	sel wParam
		case 1
		if(IsWindowVisible(GetToolbarOwner(hWnd))) hid- hWnd; else hid hWnd
	
	case WM_DESTROY
	
