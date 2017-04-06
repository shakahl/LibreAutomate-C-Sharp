 /Macro529
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	 CreateControl(0 "ToolbarWindow32" 0 TBSTYLE_FLAT 0 0 200 30 hWnd 3)
	CreateControl(0x80 "ToolbarWindow32" 0 0x54008b65 0 0 200 30 hWnd 3)
	 InvalidateRect hWnd 0 0
	case WM_COMMAND
	case WM_NOTIFY
	case WM_SIZE
	SendMessage id(3 hWnd) TB_AUTOSIZE 0 0
	case WM_DESTROY
	PostQuitMessage 0
	
	 case WM_PAINT
	 RECT rc; GetClientRect hWnd &rc
	 PAINTSTRUCT p
	 int hdc=BeginPaint(hWnd &p)
	  FillRect hdc &rc COLOR_BTNFACE+1
	 OSD_ProcExample hWnd hdc rc.right rc.bottom 0
	 EndPaint hWnd &p
	 ret
	
	case WM_ERASEBKGND
	RECT rc; GetClientRect hWnd &rc
	int hdc=wParam
	 FillRect hdc &rc COLOR_BTNFACE+1
	 OSD_ProcExample hWnd hdc rc.right rc.bottom 0
	
	int bmp=LoadPictureFile("$my qm$\Macro514.bmp" 0)
	int brush=CreatePatternBrush(bmp)
	FillRect hdc &rc brush
	DeleteObject brush
	DeleteObject bmp
	
	ret 1

ret DefWindowProc(hWnd message wParam lParam)
