 /
function idletime_s ;;obsolete. Use WaitIdle instead.

 Waits until mouse and keyboard are not used for idletime_s seconds.
 Max idletime_s is 2000000 (about 23 days).


if(idletime_s<0 or idletime_s>2000000) end ES_BADARG
idletime_s*1000
opt waitmsg -1
LASTINPUTINFO in.cbSize=sizeof(in)
rep
	GetLastInputInfo &in
	int t=GetTickCount-in.dwTime
	if(t>idletime_s) break
	1
