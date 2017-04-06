 /
function# iid FILTER&f

if f.hwnd2 and GetWindowThreadProcessId(f.hwnd2 0)=GetWindowThreadProcessId(GetShellWindow 0)
	Acc a=acc(f.x f.y)
	if(a.a and a.Role=ROLE_SYSTEM_LIST) ret iid

ret -2
