 /Sample_ExToolbar
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	CreateControl WS_EX_STATICEDGE "Edit" "Extended toolbar" 0 130 0 150 20 hWnd 3
	CreateControl 0 "Button" "x" 0 100 0 20 20 hWnd 4
	CreateControl 0 "Static" 0 0 0 50 150 20 hWnd 5
	SetTimer(hWnd 1 1000 0); goto timer
	
	case WM_DESTROY
	
	case WM_COMMAND
	if(wParam=4) SetDlgItemText hWnd 3 ""
	
	case WM_TIMER
	if(wParam=1)
		 timer
		_s.time("%a %b %#d %H:%M"); _s.setwintext(id(5 hWnd))
