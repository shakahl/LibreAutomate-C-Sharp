 /
function# hWnd message wParam lParam

 Disables all items in the right-click menu.

sel message
	case WM_COMMAND if(!lParam) ret 1
