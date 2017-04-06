 /
function nCode wParam EVENTMSG*e
out nCode
if(nCode=0)
	int i=1
	if(e.message=WM_KEYDOWN || e.message=WM_SYSKEYDOWN)
		out e.paramL

ret CallNextHookEx(hjhook nCode wParam e)
