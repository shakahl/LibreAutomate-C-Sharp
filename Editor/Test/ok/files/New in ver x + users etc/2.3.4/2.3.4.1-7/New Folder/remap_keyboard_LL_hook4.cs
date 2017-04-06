function nCode wParam MSLLHOOKSTRUCT&h

if(getopt(nargs)=0)
	if(getopt(nthreads)>1) ret
	int keyhook=SetWindowsHookEx(WH_MOUSE_LL &remap_keyboard_LL_hook4 _hinst 0)
	MessageLoop
	ret

 ---- this code runs on each key down and up event -----

 5.5
 1
 debug
sel wParam
	case [WM_MBUTTONDOWN,WM_MBUTTONUP]
	6
	out wParam=WM_MBUTTONDOWN
	

ret CallNextHookEx(0 nCode wParam &h)
