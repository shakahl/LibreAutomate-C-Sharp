function nCode wParam MSLLHOOKSTRUCT&h

if getopt(nargs)=0
	if(getopt(nthreads)>1) ret
	int-- t_hh=SetWindowsHookEx(WH_MOUSE_LL &mouse_middle_and_wheel_trigger _hinst 0)
	MessageLoop
	ret

 ---- this code runs on each mouse event while wheel is pressed -----

int-- t_wheel_used
sel wParam
	case WM_MBUTTONUP
	UnhookWindowsHookEx t_hh
	if(!t_wheel_used) mid
	shutdown -7
	
	case WM_MOUSEWHEEL
	t_wheel_used=1
	if h.mouseData>0
		out "forward"
	else
		out "backward"
	ret 1 ;;eat

ret CallNextHookEx(0 nCode wParam &h)
