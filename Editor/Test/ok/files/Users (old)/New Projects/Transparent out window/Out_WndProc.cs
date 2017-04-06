 /Out_Main
function# hWnd message wParam lParam
OUT_VAR- v
type WINDOWPLACEMENT Length flags showCmd POINT'ptMinPosition POINT'ptMaxPosition RECT'rcNormalPosition
dll user32 #GetWindowPlacement hWnd WINDOWPLACEMENT*lpwndpl
sel message
	case WM_CREATE: Out_wm_create hWnd
	case WM_COMMAND:
	case WM_MOVE: rset lParam "out pos"
	case WM_SIZE: rset lParam "out size"
	MoveWindow v.hwndedit 0 0 lParam&0xffff lParam>>16 1
	case WM_DESTROY: if(!v.caller) PostQuitMessage 0
ret DefWindowProc(hWnd message wParam lParam)
