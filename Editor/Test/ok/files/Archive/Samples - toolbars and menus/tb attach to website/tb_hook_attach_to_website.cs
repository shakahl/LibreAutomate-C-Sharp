 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG ;;note: in QM < 2.2.0, this message is not sent. Use WM_CREATE instead.
	SetTimer hWnd 100 500 0
	
	case WM_TIMER
	str s.getwintext(GetToolbarOwner(hWnd))
	if(find(s "Quick Macros Forum")<0) clo hWnd
	
