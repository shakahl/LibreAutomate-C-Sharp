 Run this function to turn on remapping. Run again to turn off.

function nCode wParam KBDLLHOOKSTRUCT&h

if(getopt(nargs)=0)
	if(getopt(nthreads)>1) shutdown -6 0 "remap_keyboard_LL_hook2"; ret
	int keyhook=SetWindowsHookEx(WH_KEYBOARD_LL &remap_keyboard_LL_hook2 _hinst 0)
	MessageLoop
	ret

 ---- this code runs on each key down and up event -----

  debug
 FormatKeyString h.vkCode 0 &_s
 out "%s %s%s" _s iif(h.flags&LLKHF_UP "up" "down") iif(h.flags&LLKHF_INJECTED ", injected" "")

if(h.flags&LLKHF_INJECTED=0)
	if h.flags&LLKHF_UP=0
		sel h.vkCode
			case 'F' mou+ 0 1; ret 1 ;;2
			case 'D' lef; ret 1 ;;5
	else
		sel h.vkCode
			case ['F','D'] ret 1

ret CallNextHookEx(0 nCode wParam &h)
