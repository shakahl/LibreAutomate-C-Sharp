 /
function# hwnd message wParam lParam

sel message
	case WM_INITDIALOG
	int hh=SetWindowsHookEx(WH_MOUSE_LL &sub.Hook_WH_MOUSE_LL _hinst 0)
	SetProp(hwnd "MHCC" hh)
	case WM_DESTROY
	UnhookWindowsHookEx GetProp(hwnd "MHCC")
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hwnd 1
		clo hwnd


#sub Hook_WH_MOUSE_LL
function# nCode message MSLLHOOKSTRUCT&m
if(nCode<0) goto gNext

 if(message!=WM_MOUSEMOVE) OutWinMsg message 0 0 _s; out "%s at %i %i" _s m.pt.x m.pt.y

if message=WM_LBUTTONUP
	ARRAY(int) a; int i
	win("" "QM_toolbar" "" 0 "GetProp=MHCC" a)
	for(i 0 a.len) SetTimer a[i] 1 10 0

 gNext
ret CallNextHookEx(0 nCode message &m)
