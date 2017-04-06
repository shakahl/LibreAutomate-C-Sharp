/
function# hWnd message wParam lParam
if(hWnd) goto messages

RECT-- myrect

int+ g_test_atom
if(!g_test_atom) g_test_atom=RegWinClass("qm_test_wnd" &qm_test_wnd)

hWnd=CreateWindowEx(0 "qm_test_wnd" 0 WS_POPUPWINDOW|WS_CAPTION|WS_VISIBLE 0 0 1024 768 0 0 _hinst 0)
 SetTimer hWnd 1 1 0
 MessageLoop
 SendMessage hWnd WM_CLOSE 0 0
 out IsWindow(hWnd)

ret
 messages
sel message
	case WM_CREATE
	case WM_COMMAND
	case WM_NOTIFY
	case WM_SIZE
	case WM_CLOSE
	 ret
	case WM_DESTROY
	out "destr"
	 PostQuitMessage 0

ret DefWindowProc(hWnd message wParam lParam)
