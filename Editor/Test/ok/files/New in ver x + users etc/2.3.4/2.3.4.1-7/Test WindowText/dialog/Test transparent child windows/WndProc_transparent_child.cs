 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	
	case WM_NCHITTEST
	 out "WM_NCHITTEST: %i %i" lParam&0xffff lParam>>16
	if(GetWinId(hWnd)=4) ret HTTRANSPARENT
	
	case WM_ERASEBKGND
	if(GetWinId(hWnd)=5) ret
	
	case WM_PAINT
	if(GetWinId(hWnd)=5)
		PAINTSTRUCT ps
		BeginPaint hWnd &ps
		EndPaint hWnd &ps
		ret
	

ret DefWindowProcW(hWnd message wParam lParam)
