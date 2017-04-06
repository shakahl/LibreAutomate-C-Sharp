 /HW_Main2
function# hWnd message wParam lParam

ref WINAPI2

sel message
	case WM_CREATE
	CreateControl(0 "Button" "Text" 0 30 30 80 30 hWnd 3 _hfont)
	
	 DWM_BLURBEHIND b
	 b.dwFlags=DWM_BB_ENABLE
	 b.fEnable=1
	 DwmEnableBlurBehindWindow(hWnd &b)
	
	MARGINS m.cxLeftWidth=-1
	DwmExtendFrameIntoClientArea(hWnd &m)
	
	case WM_ERASEBKGND
	RECT r; GetClientRect hWnd &r
	FillRect GetDC(hWnd) &r GetStockObject(BLACK_BRUSH)
	ret 1
	
	case WM_SIZE
	 case WM_CLOSE: ret ;;this would prevent destroying
	case WM_DESTROY: PostQuitMessage 0
	
	case WM_NCHITTEST
	DwmDefWindowProc(hWnd message wParam lParam &_i)
	ret _i

ret DefWindowProc(hWnd message wParam lParam)
