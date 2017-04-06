 /
function# hWnd message wParam lParam

sel message
	case WM_SETCURSOR
	int+ g_tbdelexp
	if(g_tbdelexp=0)
		SetTimer hWnd 9678 500 0 ;;expand after 500 ms of not moving mouse
		ret 1
	
	case WM_TIMER
	if(wParam=9678)
		if(win(mouse)=hWnd) g_tbdelexp=1; mou+ 1 0; mou+ -1 0
		else g_tbdelexp=0; KillTimer hWnd 9678
