 /
function#

 Returns QM memory usage, KB.
 It is what you see in Task Manager, "Commit Size" column.


PROCESS_MEMORY_COUNTERS z.cb=sizeof(z)
if GetProcessMemoryInfo(GetCurrentProcess &z sizeof(z))
	ret z.PagefileUsage/1024
