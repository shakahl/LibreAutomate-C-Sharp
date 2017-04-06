 /
function# hWnd message wParam lParam

dll kernel32 Sleep dwMilliseconds

sel message
	 case WM_PASTE
	case WM_KEYDOWN
	 0.2
	Sleep 200

ret CallWindowProc(subc hWnd message wParam lParam)
