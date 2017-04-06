 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	SetTimer hWnd 3 1000 0
	
	case WM_TIMER
	TB_AltAttach hWnd GetProp(hWnd "ho")
