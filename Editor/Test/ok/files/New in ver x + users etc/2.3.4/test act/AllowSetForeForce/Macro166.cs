 /exe 1
out
int w=win("Calculator")
 int w=_hwndqm
rep 4
	2
	act w
	continue
	 goto gsfw
	 ShowWindow(GetForegroundWindow, SW_MINIMIZE)
	 SendMessage(GetForegroundWindow, WM_SYSCOMMAND, SC_MINIMIZE, 0);
	
	 SendMessage(w, WM_SYSCOMMAND, SC_MINIMIZE, 0);
	 SendMessage(w, WM_SYSCOMMAND, SC_RESTORE, 0);
	
	 SendMessageTimeout(w, WM_SYSCOMMAND, SC_TASKLIST, 0, 0, 1000, &_i);
	 key W{}
	 SetForegroundWindow w
	
	out AllowSetForeForce
	 gsfw
	SetForegroundWindow w
	 ShellExecute 0 0 _s.expandpath("$qm$\Macro1668.exe") 0 0 SW_NORMAL
	 ShellExecute 0 0 "calc.exe" 0 0 SW_NORMAL
	 WinExec "calc.exe" SW_NORMAL
	 run "Macro1668.exe"
	
	 DestroyWindow(hh)
	continue
	
	
	
	
	keybd_event 0 0 0 0
	out AllowSetForegroundWindow(GetCurrentProcessId)
	 out AllowActivateWindows
	out AllowSetForegroundWindow(GetCurrentProcessId)
	 SwitchToThisWindow w 1
	SetForegroundWindow w

