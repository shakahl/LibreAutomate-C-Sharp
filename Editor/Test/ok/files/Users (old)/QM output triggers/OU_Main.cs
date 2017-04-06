int+ g_ou_pwndproc
if(g_ou_pwndproc) ret
if(GetCurrentThreadId!=GetWindowThreadProcessId(_hwndqm 0)) ;;this is not QM main thread
	atend OU_End
	SendMessage _hwndqm WM_SETTEXT 3 "''OU_Main''" ;;run OU_Main in QM main thread
	wait -1 ;;wait indefinitely, because finally must be called OU_End

 this is QM main thread, and we can subclass QM output window
g_ou_pwndproc=SubclassWindow(id(2201 _hwndqm) &OU_Wndproc)
