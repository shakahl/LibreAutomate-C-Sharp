 /
function# hwnd priority [flags] ;;flags: 1 hwnd is pid

int pid
if(flags&1) pid=hwnd
else GetWindowThreadProcessId(hwnd &pid)

__Handle hp=OpenProcess(PROCESS_SET_INFORMATION 0 pid)

ret SetPriorityClass(hp priority)
