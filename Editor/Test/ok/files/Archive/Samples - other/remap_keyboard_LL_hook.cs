function nCode wParam KBDLLHOOKSTRUCT&h

if(getopt(nargs)=0)
	if(getopt(nthreads)>1) ret
	int keyhook=SetWindowsHookEx(WH_KEYBOARD_LL &remap_keyboard_LL_hook _hinst 0)
	MessageLoop
	ret

 ---- this code runs on each key down and up event -----

  debug
 FormatKeyString h.vkCode 0 &_s
 out "%s %s%s" _s iif(h.flags&LLKHF_UP "up" "down") iif(h.flags&LLKHF_INJECTED ", injected" "")

if(h.flags&LLKHF_INJECTED=0)
	
	 remaps:
	 A to B,
	 B to C
	 F8 to right arrow
	
	int r
	sel h.vkCode
		case 'A' r='B'
		case 'B' r='C'
		case VK_F8 r=VK_RIGHT
		 ...
		
	if(r)
		if(h.flags&LLKHF_UP=0) key (r)
		ret 1
		

ret CallNextHookEx(0 nCode wParam &h)
