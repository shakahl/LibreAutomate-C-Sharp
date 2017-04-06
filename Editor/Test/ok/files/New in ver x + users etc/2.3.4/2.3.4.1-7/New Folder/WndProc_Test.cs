 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	 get settings, create controls, etc
	
	case WM_DESTROY
	 save settings, etc
	PostQuitMessage 0 ;;delete this line if it is not main window
	
	case WM_SIZE
	 resize some controls
	
	case WM_COMMAND
	 this message from controls or menu
	
	case WM_NOTIFY
	 this message from common controls

int R=iif(IsWindowUnicode(hWnd) DefWindowProcW(hWnd message wParam lParam) DefWindowProc(hWnd message wParam lParam))
ret R
