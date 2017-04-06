 /
function# hWnd message wParam lParam

sel message
	case WM_SIZE
	PostMessage hWnd WM_APP 0 0
	
	case WM_APP
	siz 26 1000 id(9999 hWnd)
