dll user32 #IsHungAppWindow hWnd

int window_handle
run "notepad.exe" "" "" "" 0x800 "- Notepad" window_handle
rep
	1
	if(!IsWindow(window_handle)) break ;;closed
	if(IsHungAppWindow(window_handle))
		ShutDownProcess window_handle
