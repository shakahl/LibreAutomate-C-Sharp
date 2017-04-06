 /
function# nCode wParam MSG&m
if(nCode!MSGF_MENU) goto gNext

sel m.message
	case [WM_TIMER,WM_PAINT]
	case WM_LBUTTONUP
	 outw m.hwnd
	Acc a.FromMouse
	out a.Name ;;does not work with owner-draw menu
	
	 case else OutWinMsg m.message m.wParam m.lParam

 gNext
ret CallNextHookEx(0 nCode wParam &m)

 note: cannot hook windows of other processes.
