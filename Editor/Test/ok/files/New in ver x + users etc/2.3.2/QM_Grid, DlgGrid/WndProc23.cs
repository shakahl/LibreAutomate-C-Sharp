 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case [WM_NCHITTEST,WM_SETCURSOR,WM_MOUSEMOVE]
	case else
	OutWinMsg message wParam lParam

ret CallWindowProcW(GetProp(hWnd "sub") hWnd message wParam lParam)
