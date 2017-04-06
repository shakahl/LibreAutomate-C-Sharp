 /
function# hWnd message wParam lParam

int+ g_tbkb_aw

sel message
	case WM_COMMAND
	if(g_tbkb_aw and hWnd=win)
		act g_tbkb_aw; err
	g_tbkb_aw=0
	
	case WM_ACTIVATE
	if(wParam=0) g_tbkb_aw=0
