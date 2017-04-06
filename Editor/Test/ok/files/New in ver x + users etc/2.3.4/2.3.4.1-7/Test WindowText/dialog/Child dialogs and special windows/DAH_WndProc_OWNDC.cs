 /dlg_apihook
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

BSTR s
RECT r
sel message
	case WM_CREATE
	 out GetClassLong(hWnd GCL_STYLE)
	
	case WM_ERASEBKGND
	s="on WM_ERASEBKGND"; ExtTextOutW wParam 0 35 0 0 s s.len 0
	 ValidateRect hWnd 0
	ret 1
	
	case WM_PAINT
	 out "p"
	  ValidateRect hWnd 0
	 __Hdc dc.FromWindowDC(hWnd)
	 s="on WM_PAINT no BeginPaint"; out s; ExtTextOutW dc.dc 0 0 0 0 s s.len 0
	 ret
	
	PAINTSTRUCT ps
	int hdc=BeginPaint(hWnd &ps)
	__Font-- t_font; if(!t_font) t_font.Create("Courier New"); SelectObject hdc t_font
	
	 int hr=CreateRectRgn(20 6 60 40)
	 SelectClipRgn hdc hr
	 DeleteObject hr
	
	s="Text in own DC"; ExtTextOutW hdc 0 0 0 0 s s.len 0
	EndPaint hWnd &ps

ret DefWindowProcW(hWnd message wParam lParam) ;;use this instead with Unicode window class
