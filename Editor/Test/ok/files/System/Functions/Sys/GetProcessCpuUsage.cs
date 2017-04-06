 /
function# hwnd [measuretime] [flags] ;;flags: 1 hwnd is process id.

 Returns CPU (processor) usage (0 - 100%) of a process (running program).

 hwnd - handle of some window that belongs to the process, or process id.
 measuretime - number of milliseconds to measure CPU usage. Default and minimal is 100 ms. When it is 100 ms, precision is 10% or worse. When it is bigger, precision is better.

 REMARKS
 For some processes, does not work in nonadministrator account.

 EXAMPLES
 int hwnd=win("Windows Media Player")
 rep
	 out GetProcessCpuUsage(hwnd 1000)

 int pid=ProcessNameToId("WMPLAYER" 0 1)
 rep
	 out GetProcessCpuUsage(pid 1000 1)


int pid hp t
long tce tkernel tuser t1 t2

if(!hwnd) end ERR_BADARG

int+ ___debugprivilege
if(!___debugprivilege) ___debugprivilege=1; SetPrivilege "SeDebugPrivilege"

if(flags&1) pid=hwnd; else GetWindowThreadProcessId(hwnd &pid)
hp=OpenProcess(PROCESS_QUERY_X_INFORMATION 0 pid); if(!hp) end "" 16

if(measuretime<100) measuretime=100
opt waitmsg -1
t=GetTickCount
GetProcessTimes hp +&tce +&tce +&tkernel +&tuser
t1=tkernel+tuser
wait measuretime/1000.0
GetProcessTimes hp +&tce +&tce +&tkernel +&tuser
t=GetTickCount-t
t2=tkernel+tuser

CloseHandle hp

t2-t1
t2/100
t2/t
if(t2>100) t2=100
ret t2
