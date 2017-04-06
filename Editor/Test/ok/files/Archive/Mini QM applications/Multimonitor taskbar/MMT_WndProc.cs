 /MMT_Main
function# hWnd message wParam lParam

sel message
	case WM_CREATE: MMT_wm_create hWnd
	case WM_DESTROY: MMT_wm_destroy
	case WM_COMMAND: MMT_wm_command wParam
	case WM_NOTIFY: ret MMT_wm_notify(+lParam)
	case WM_TIMER: sel(wParam) case 1 MMT_Buttons
	case WM_SIZE: MMT_wm_size
	case WM_APP: MMT_wm_appbar wParam lParam

ret DefWindowProcW(hWnd message wParam lParam)
