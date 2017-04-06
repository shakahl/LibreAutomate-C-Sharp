 /
function ^timeS hwnd

 Obsolete. Use <help>wait</help>.


if(hwnd=0) ret
rep
	0.2
	if(IsWindowEnabled(hwnd)) ret
	if(timeS) timeS-0.2; if(timeS<=0) end ERR_TIMEOUT
