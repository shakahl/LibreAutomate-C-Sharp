int+ g_tid_key=GetCurrentThreadId
MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	sel m.message
		case WM_KEYDOWN
		_key3 key((m.wParam))
	DispatchMessage &m
