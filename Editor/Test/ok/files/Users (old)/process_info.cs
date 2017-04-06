 /
function# pid

 Returns process working set size in KB.
 Returns 0 if failed.

 pid - process ID.

 EXAMPLE
 int pid=ProcessNameToId("firefox")
 out process_info(pid)


__Handle hpr=OpenProcess(PROCESS_QUERY_INFORMATION|PROCESS_VM_READ 0 pid); if(!hpr) ret

PROCESS_MEMORY_COUNTERS pm
if(GetProcessMemoryInfo(hpr &pm sizeof(PROCESS_MEMORY_COUNTERS)))
	ret pm.WorkingSetSize/1024
