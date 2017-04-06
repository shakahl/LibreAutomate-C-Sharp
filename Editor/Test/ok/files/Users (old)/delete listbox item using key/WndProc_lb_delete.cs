 /
function# hWnd message wParam lParam

sel message
	case WM_KEYDOWN
	sel wParam
		case VK_DELETE
		_i=LB_SelectedItem(hWnd)
		if(_i>=0) SendMessage hWnd LB_DELETESTRING _i 0

ret CallWindowProcW(GetProp(hWnd "WndProc_lb_delete") hWnd message wParam lParam)
