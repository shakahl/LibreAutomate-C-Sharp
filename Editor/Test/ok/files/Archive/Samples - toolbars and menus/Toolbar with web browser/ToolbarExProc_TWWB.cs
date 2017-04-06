 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	int h=CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" 0 0 20 400 200 hWnd 100)
	_s=""; _s.setwintext(h) ;;opens blank page. Instead of "" you also can pass an url or HTML. Or open web page using Navigate (see below).
	
	 SHDocVw.WebBrowser b._getcontrol(h)
	 b.Navigate("http://www.google.com")
	
	case WM_SIZE
	RECT r; GetClientRect(hWnd &r)
	siz r.right r.bottom-20 id(100 hWnd)
	
	case WM_DESTROY
	
