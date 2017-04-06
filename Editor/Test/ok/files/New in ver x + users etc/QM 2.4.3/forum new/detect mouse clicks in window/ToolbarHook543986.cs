 /
function# hWnd message wParam lParam

 <help #IDH_EXTOOLBAR>Toolbar hook help</help>

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_INITDIALOG
	mac "detect_mouse_clicks_in_window_58736" "" hWnd
	
	case WM_DESTROY
	
