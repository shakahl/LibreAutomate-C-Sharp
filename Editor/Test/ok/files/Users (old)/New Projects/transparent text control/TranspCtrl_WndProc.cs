 /dlg_test_transp_control
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	SetWinStyle hWnd WS_EX_TRANSPARENT 5
	SetTimer hWnd 1 500 0 ;;repaints in case another control paints itself. However this adds more flickering.
	
	case WM_TIMER
	sel wParam
		case 1 goto g1
		
	case WM_ERASEBKGND
	ret
	
	case WM_PAINT
	PAINTSTRUCT ps
	int dc=BeginPaint(hWnd &ps)
	int of=SelectObject(dc SendMessage(hWnd WM_GETFONT 0 0))
	RECT r; GetClientRect hWnd &r
	
	SetBkMode dc TRANSPARENT
	SetTextColor dc 0xff0000
	str s; s.getwintext(hWnd)
	DrawTextW dc @s -1 &r 0
	
	SelectObject dc of
	EndPaint hWnd &ps
	
	case WM_SETTEXT
	 g1
	 erase background. I don't know a better way than repainting parent window
	GetClientRect hWnd &r; MapWindowPoints hWnd GetParent(hWnd) +&r 2
	RedrawWindow GetParent(hWnd) &r 0 RDW_INVALIDATE|RDW_ERASE|RDW_ALLCHILDREN
	
	case WM_SETFONT
	SetWindowLong hWnd 0 wParam
	case WM_GETFONT
	ret GetWindowLong(hWnd 0)
	
	case WM_NCHITTEST
	ret HTTRANSPARENT

ret DefWindowProcW(hWnd message wParam lParam)
