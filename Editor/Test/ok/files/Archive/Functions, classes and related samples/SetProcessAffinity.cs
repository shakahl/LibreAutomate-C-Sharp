 /
function hwnd affinityMask [flags] ;;flags: 1 hwnd is process id

 Sets affinity of a process (running program). It is which processors the process can use.
 On failure generates error.

 hwnd - handle of some window of that process.
 affinityMask - sum of flags, where flag 1 is for CPU0, flag 2 is for CPU1, flag 4 is for CPU2, flag 8 is for CPU3, .... For example, affinityMask 2 allows to use only CPU1.

 EXAMPLE
 SetProcessAffinity ProcessNameToId("notepad") 1 1 ;;let notepad use only CPU0


int pid ok
if(flags&1) pid=hwnd; else GetWindowThreadProcessId(hwnd &pid)
__Handle hp=OpenProcess(PROCESS_SET_INFORMATION 0 pid); if(!hp) goto g1
ok=SetProcessAffinityMask(hp affinityMask)
 g1
if(!ok) end _s.dllerror
