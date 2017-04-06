 /
function htb ho

if(!IsWindow(ho))
	clo htb
else if(IsWindowVisible(ho) and !IsIconic(ho))
	Zorder htb GetWindow(ho GW_HWNDPREV) SWP_NOACTIVATE
	Transparent htb 256
else
	Transparent htb 0
 note: uses Transparent instead of hid because QM would unhide
