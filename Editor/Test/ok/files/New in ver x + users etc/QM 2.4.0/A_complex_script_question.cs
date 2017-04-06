function nCode wParam MSLLHOOKSTRUCT&h

 Run this function.
 Move mouse to the point to test.
 Run this function again to end it.
 Change the <CHANGE> parts if need, and run again.


if getopt(nargs)=0 ;;if this function runs not as a hook function
	if getopt(nthreads)>1 ;;if thread running, end it and exit
		shutdown -6 0 "A_complex_script_question"
		ret
	
	RECT- r
	SetRect &r 513 385 513 385; InflateRect &r 10 10 ;;<CHANGE>
	
	 run thread: add tray icon, set mouse hook, and wait
	AddTrayIcon "$qm$\mouse.ico" "A_complex_script_question"
	int hh=SetWindowsHookEx(WH_MOUSE_LL &A_complex_script_question _hinst 0)
	MessageLoop
	UnhookWindowsHookEx hh
	ret

 ---- this code runs on each mouse event while the thread is running -----

if(h.flags&LLMHF_INJECTED) goto gNH ;;ignore macro-generated mouse events

sel wParam
	case WM_MOUSEMOVE
	if PtInRect(&r h.pt.x h.pt.y)
		out "mouse is at a certain part of the screen" ;;<CHANGE>

 gNH
ret CallNextHookEx(0 nCode wParam &h)
