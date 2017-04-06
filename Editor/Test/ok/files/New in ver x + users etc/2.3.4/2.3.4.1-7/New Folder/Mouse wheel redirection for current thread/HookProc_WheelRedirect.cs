 /
function# code wParam MSG&m

if code=HC_ACTION and m.message=WM_MOUSEWHEEL
	int h=child(mouse)
	if(!h or GetWindowThreadProcessId(h 0)!GetCurrentThreadId) ret 1
	m.hwnd=h

ret CallNextHookEx(0 code wParam &m)
