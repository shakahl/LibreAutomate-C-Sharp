 /Toolbar65
function% hwnd message wParam lParam

 <help #IDH_EXTOOLBAR>Toolbar hook help</help>

OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_INITDIALOG
	
	case WM_DESTROY
	min 0
	
	case WM_ERASEBKGND
	 ret 1
	ret 0x100000000
