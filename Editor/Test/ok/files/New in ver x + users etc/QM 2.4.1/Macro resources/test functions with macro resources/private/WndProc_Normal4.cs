 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	 get settings, create controls, etc
	
	case WM_DESTROY
	 save settings, etc
	PostQuitMessage 0 ;;enable this line if the thread must end when this window closed
	
	case WM_SIZE
	 resize some controls
	
	case WM_COMMAND
	 this message from controls or menu
	
	case WM_NOTIFY
	 this message from controls


int R=DefWindowProcW(hWnd message wParam lParam)
ret R

 If the window class is not Unicode, replace DefWindowProcW to DefWindowProc.
