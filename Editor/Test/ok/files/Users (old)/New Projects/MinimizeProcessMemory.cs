 /
function# hwnd [flags] ;;flags: 1 hwnd is pid

int pid
if(flags&1) pid=hwnd
else GetWindowThreadProcessId(hwnd &pid)

__Handle hp=OpenProcess(PROCESS_SET_QUOTA 0 pid)
ret SetProcessWorkingSetSize(hp -1 -1)
