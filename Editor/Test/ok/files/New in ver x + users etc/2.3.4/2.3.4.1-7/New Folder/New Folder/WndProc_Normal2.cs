 /Macro1827
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	out
	 get settings, create controls, etc
	
	case WM_DESTROY
	 save settings, etc
	PostQuitMessage 0 ;;delete this line if it is not main window
	
	case WM_PAINT
	out "p"
	
	case WM_ERASEBKGND
	out "e"
	ret 1
	
	case WM_NCPAINT
	out "n"
	 out "n %i %i" wParam lParam
	  RECT r; out GetRgnBox(wParam &r); zRECT r
	 __Hdc dc.FromWindowDC(hWnd 1)
	 out dc.dc
	 out ExtTextOut(dc.dc 10 50 0 0 "Test" 4 0)
	 ret

ret DefWindowProc(hWnd message wParam lParam)
 ret DefWindowProcW(hWnd message wParam lParam) ;;use this instead with Unicode window class
