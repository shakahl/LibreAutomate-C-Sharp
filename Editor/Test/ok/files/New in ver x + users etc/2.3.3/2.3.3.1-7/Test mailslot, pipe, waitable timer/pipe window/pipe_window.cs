/exe
function# hWnd message wParam lParam
if(hWnd) goto messages

MainWindow "Test Pipe" "@QM_TP_Class" &pipe_window ScreenWidth-150 600 150 100 0 0 0 WS_EX_TOPMOST

 messages
sel message
	case WM_CREATE
	mac "pipe_thread"
	
	case WM_APP+1589
	 _i=m[0]
	 m[0]=5
	 ReplyMessage 6
	 out "window: %i" _i
	 OutputDebugString "received"
	int+ g_reset=0
	word p1=perf; out F"msg: {p1/1000.0%%.3f}"
	
	
	case WM_COPYDATA
	 out "WM_COPYDATA"
	ret 7
	
	case WM_TIMER
	sel wParam
		case 2
		KillTimer hWnd 2
		int+ g_ms
		read_mailslot g_ms
	
	case WM_DESTROY
	shutdown -6 0 "pipe_thread"
	PostQuitMessage 0

ret DefWindowProcW(hWnd message wParam lParam)
