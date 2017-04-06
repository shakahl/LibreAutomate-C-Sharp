 /tb_moz_control
function# hWnd message wParam lParam

typelib MOZILLACONTROLLib {1339B53E-3453-11D2-93B9-000000000000} 1.0

sel message
	case WM_CREATE
	int h=CreateControl(0 "ActiveX" "MOZILLACONTROLLib.MozillaBrowser" 0 0 0 100 100 hWnd 3)
	MOZILLACONTROLLib.MozillaBrowser a
	a._getcontrol(id(3 hWnd))
	a.Navigate("http://www.google.com")
	
	case WM_DESTROY
	case WM_SIZE
	RECT r; GetClientRect hWnd &r
	siz r.right r.bottom id(3 hWnd)
