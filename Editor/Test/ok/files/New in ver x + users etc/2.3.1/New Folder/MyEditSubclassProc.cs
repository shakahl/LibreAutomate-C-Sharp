 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_DESTROY
	case [WM_KEYDOWN,WM_KEYUP]
	sel wParam ;;virtual key code
		case [VK_DOWN,VK_UP,VK_PRIOR,VK_NEXT]
		 relay these keys to the listbox and not to the edit box
		SendMessage id(4 GetParent(hWnd)) message wParam lParam
		ret

int wndproc=GetProp(hWnd "wndproc"); if(!wndproc) ret
ret CallWindowProcW(wndproc hWnd message wParam lParam)
