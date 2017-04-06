 \
function nCode wParam MSLLHOOKSTRUCT&h

 This function runs when you press X1 (4-th) mouse button.
 It toggles mouse left button behavior - run or don't run macro "multi_click".
 When macro can run, shows tray icon.
 Macro will not run with Ctrl etc. Also if QM or macro disabled. To disable/enable QM, press Ctrl+Alt+Shift+D.
 Macro will stop when mouse left button released.


if getopt(nargs)=0 ;;if this function runs not as a hook function
	if getopt(nthreads)>1 ;;if thread running, end it and exit
		shutdown -6 0 "mouse_left_macro_on_off"
		ret
	
	 run thread: add tray icon, set mouse hook, and wait
	AddTrayIcon "$qm$\mouse.ico" "Macro ''multi_click'' will run on mouse left button down.[]To disable it, click mouse X1 button or Ctrl_click this icon."
	int hh=SetWindowsHookEx(WH_MOUSE_LL &mouse_left_macro_on_off _hinst 0)
	MessageLoop
	UnhookWindowsHookEx hh
	ret

 ---- this code runs on each mouse event while the thread is running -----

if(h.flags&LLMHF_INJECTED) goto nNH ;;ignore macro-generated mouse events

int+ g_multi_click ;;global variable used to stop macro "multi_click"
sel wParam
	case WM_LBUTTONDOWN
	if(dis&2 or dis("multi_click") or GetMod) goto nNH ;;don't run macro if QM or macro is disabled, or if Ctrl etc
	
	 Here you can add code to run this macro only in certain conditions, for example if certain window is active. Example:
	 int w=win; if(!w or !wintest(w "Name" "Class")) goto nNH
	
	g_multi_click=1
	mac "multi_click" ;;run macro
	ret 1 ;;eat this event
	
	case WM_LBUTTONUP
	if g_multi_click
		g_multi_click=0 ;;stop macro
		ret 1 ;;eat this event

 nNH
ret CallNextHookEx(0 nCode wParam &h)
