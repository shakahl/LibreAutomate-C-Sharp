 /
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	case WM_COMMAND
	case WM_NOTIFY
	NMHDR& nh=+lParam
	out nh.cod

ret CallWindowProcW(__subclass_qmcode_parent.m_oldproc hWnd message wParam lParam)
