 /test_MessageLoopOptions
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	CreateControl(0 "Button" "&Aaa" WS_TABSTOP 0 0 100 60 hWnd 3)
	CreateControl(0 "Edit" "" WS_TABSTOP 0 100 100 60 hWnd 4)
	MessageLoopOptions 2|0x100 0 0 "5 Ca"
	 MessageLoopOptions 2|0x100 0 0 "5 Ca" hWnd
	
	case WM_DESTROY
	 save settings, etc
	PostQuitMessage 0 ;;delete this line if it is not main window
	
	case WM_LBUTTONUP
	ShowDialog("Dialog116" &Dialog116 0 hWnd 1)
	
	case WM_COMMAND
	outx wParam


int R=iif(IsWindowUnicode(hWnd) DefWindowProcW(hWnd message wParam lParam) DefWindowProc(hWnd message wParam lParam))
ret R
