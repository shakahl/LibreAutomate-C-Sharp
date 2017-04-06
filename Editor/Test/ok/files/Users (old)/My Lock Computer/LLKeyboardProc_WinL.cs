 /
function nCode wParam KBDLLHOOKSTRUCT*lParam

int+ __eat_Win
int+ __eat_L
if(nCode>=0)
	if(lParam.flags&LLKHF_INJECTED=0)
		int up=lParam.flags&LLKHF_UP
		if(!up)
			sel lParam.vkCode
				case 'L'
				if(__eat_L) ret 1
				int winpressed
				ifk((VK_LWIN)) winpressed=VK_LWIN; else ifk((VK_RWIN)) winpressed=VK_RWIN
				if winpressed
					__eat_Win=1; __eat_L=1
					keybd_event 'L' MapVirtualKey('R' 0) 2 0
					keybd_event winpressed MapVirtualKey(winpressed 0) 3 0
					mac "MyLockComputer"
					ret 1
				case [VK_LWIN,VK_RWIN] if(__eat_Win) ret 1
		else
			sel lParam.vkCode
				case 'L' if(__eat_L) __eat_L=0; ret 1
				case [VK_LWIN,VK_RWIN] if(__eat_Win) __eat_Win=0; ret 1

ret CallNextHookEx(__llkhook_winl nCode wParam lParam)
