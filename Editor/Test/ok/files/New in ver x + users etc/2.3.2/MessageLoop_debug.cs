 /
function# [flags]

 for debugging exception in clipboard_copy_triggers

MSG m
rep
	if(GetMessage(&m 0 0 0)<1 or m.message=2000)
		if(flags&1 and m.message=WM_QUIT) PostQuitMessage(m.wParam)
		ret m.wParam
	if(flags&2 and IsDialogMessage(GetActiveWindow &m)) continue
	
	TranslateMessage &m
	DispatchMessage &m
	err+
		OutWinMsg m.message m.wParam m.lParam &_s
		out "%s    %s   (%s)" _s _error.description _error.line

