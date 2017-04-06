 /exe 1
out
int w=win("Calculator" "CalcFrame")
 int w=_hwndqm
rep 1
	3
	 goto gsfw
	 int w1=win("" "Shell_TrayWnd")
	 lef 1270 19 w1
	 act w
	 min win
	 SetWindowState
	 ShowWindow(GetForegroundWindow, SW_MINIMIZE)
	 SendMessage(GetForegroundWindow, WM_SYSCOMMAND, SC_MINIMIZE, 0);
	
	 SetForegroundWindow w
	
	 SendMessage(w, WM_SYSCOMMAND, SC_MINIMIZE, 0);
	 SendMessage(w, WM_SYSCOMMAND, SC_RESTORE, 0);
	
	 SendMessage(w, WM_SYSCOMMAND, SC_HOTKEY, w);
	 SendMessageTimeout(w, WM_SYSCOMMAND, SC_PREVWINDOW, 0, 0, 1000, &_i);
	 SendMessageTimeout(w, WM_SYSCOMMAND, SC_SCREENSAVE, 0, 0, 1000, &_i);
	
	 SendMessageTimeout(w, WM_SYSCOMMAND, SC_TASKLIST, 0, 0, 1000, &_i);
	 key W{}
	 SetForegroundWindow w
	
	 int hh=CreateWindowEx(0 "#32770" "dd" WS_OVERLAPPEDWINDOW|WS_VISIBLE 0 0 200 200 0 0 _hinst 0)
	 int hh=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" "dd" WS_OVERLAPPEDWINDOW 0 0 200 200 0 0 _hinst 0)
	 int hh=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" "dd" WS_OVERLAPPEDWINDOW 0 0 0 0 0 0 _hinst 0)
	Q &q
	int hh=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" "dd" WS_POPUP 0 0 0 0 0 0 _hinst 0)
	 0
	 outw win
	 out AllowSetForegroundWindow(GetCurrentProcessId)
	 min hh
	 SetWindowState hh 6
	SetWindowState hh 6 1
	outw win
	out AllowSetForegroundWindow(GetCurrentProcessId)
	 res hh
	 SetWindowState hh 9
	SetWindowState hh 9 1
	outw win
	out AllowSetForegroundWindow(GetCurrentProcessId)
	DestroyWindow(hh)
	 0
	outw win
	out AllowSetForegroundWindow(GetCurrentProcessId)
	Q &qq; outq
	 gsfw
	 outw w
	SetForegroundWindow w
	ret
	
	
	
	
	keybd_event 0 0 0 0
	out AllowSetForegroundWindow(GetCurrentProcessId)
	 out AllowActivateWindows
	out AllowSetForegroundWindow(GetCurrentProcessId)
	 SwitchToThisWindow w 1
	SetForegroundWindow w

 BEGIN PROJECT
 main_function  Macro1694
 exe_file  $my qm$\Macro1694.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {2E9217EE-AC5C-44BF-8F10-669C809F4704}
 END PROJECT

 idea: since taskbar can activate any window, maybe we can inject into explorer and call AllowSetForegroundWindow.
