 /
function# hWnd message wParam lParam

int+ __procSTC

sel message
	case [0,13]
	case else
	out "%i %i" message WM_NCHITTEST
	
 if(message=WM_NCHITTEST) ret HTCLIENT

ret CallWindowProc(__procSTC hWnd message wParam lParam)
