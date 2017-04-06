 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	int h=CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" 0 0 20 968 1179 hWnd 1)
	SHDocVw.WebBrowser b._getcontrol(h)
	b.Navigate("about:blank")
	case WM_DESTROY
	