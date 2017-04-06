 /
function RECT&r style exstyle [hwnd] [flags] ;;flags: 1 has menu, 2 except scrollbars, 4 only scrollbars

 Calls AdjustWindowRectEx.
 If hwnd, uses its style exstyle, not the parameters.

if(hwnd) style=GetWinStyle(hwnd); exstyle=GetWinStyle(hwnd 1)

if(flags&4=0) AdjustWindowRectEx(&r style flags&1 exstyle)
if(flags&2=0)
	if(style&WS_VSCROLL) r.right+GetSystemMetrics(SM_CXVSCROLL)
	if(style&WS_HSCROLL) r.bottom+GetSystemMetrics(SM_CYHSCROLL)
