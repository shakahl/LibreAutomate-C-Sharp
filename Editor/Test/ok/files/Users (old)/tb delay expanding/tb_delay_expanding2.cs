 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG ;;note: in QM < 2.2.0, this message is not sent. Use WM_CREATE instead.
	
	case WM_DESTROY
	
	case WM_SETCURSOR
	int+ g_tbdelexp
	POINT+ g_tbdep
	
	if(!g_tbdelexp)
		g_tbdelexp=1
		GetCursorPos &g_tbdep
		SetTimer hWnd 9678 50 0
		ret 1
	else if
		POINT p
		GetCursorPos &p
	
	case WM_TIMER
	if(wParam=9678)
		if(win(mouse)!=hWnd)
			KillTimer hWnd 9678
			g_tbdelexp=0
		else
			g_tbdelexp+1
