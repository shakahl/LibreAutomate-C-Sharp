 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	case WM_COMMAND
	case WM_NOTIFY
	case WM_SIZE
	case WM_DESTROY
	PostQuitMessage 0

ret DefWindowProcW(hWnd message wParam lParam)
