 /
function! hwnd

int pid
GetWindowThreadProcessId(hwnd &pid)
if(!pid) ret
__Handle hp=OpenProcess(PROCESS_QUERY_X_INFORMATION 0 pid)
 __Handle hp=OpenProcess(PROCESS_ALL_ACCESS 0 pid)
 out hp
rep
	int r=WaitForInputIdle(hp 100)
	if(r=0) ret 1
	if(r!WAIT_TIMEOUT) ret
