 /
function# hWnd message wParam lParam

int- t_oldwndproc

sel message
	case WM_VSCROLL
	out "WM_VSCROLL: LOWORD(wParam)=%i HIWORD(wParam)=%i" wParam&0xffff wParam>>16

ret CallWindowProcW(t_oldwndproc hWnd message wParam lParam)
