 /
function# hwnd message wParam lParam

 <help #IDH_EXTOOLBAR>Toolbar hook help</help>

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case 666
	out 1
	
	case WM_DESTROY
	
