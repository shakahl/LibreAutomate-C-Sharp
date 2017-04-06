 /dlg_apihook
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	 out GetClassLong(hWnd GCL_STYLE)
	
	case WM_PAINT
	PAINTSTRUCT ps
	int hdc=BeginPaint(hWnd &ps)
	 int L=SetLayout(hdc LAYOUT_RTL)
	 out L
	ExtTextOutW hdc 0 0 0 0 L"Text" 4 0
	 SetLayout(hdc L)
	EndPaint hWnd &ps

ret DefWindowProcW(hWnd message wParam lParam) ;;use this instead with Unicode window class
