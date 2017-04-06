 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG ;;note: in QM < 2.2.0, this message is not sent. Use WM_CREATE instead.
	
	case WM_MOVE
	int h=GetToolbarOwner(hWnd)
	 out h
	
	case WM_DESTROY
	
