function nCode wParam KBDLLHOOKSTRUCT&h

if(getopt(nargs)=0)
	if(getopt(nthreads)>1) ret
	int keyhook=SetWindowsHookEx(WH_KEYBOARD_LL &remap_keyboard_LL_hook3 _hinst 0)
	MessageLoop
	ret

 ---- this code runs on each key down and up event -----

 5.5
1
 debug
FormatKeyString h.vkCode 0 &_s
out "%s %s%s" _s iif(h.flags&LLKHF_UP "up" "down") iif(h.flags&LLKHF_INJECTED ", injected" "")

ret CallNextHookEx(0 nCode wParam &h)
