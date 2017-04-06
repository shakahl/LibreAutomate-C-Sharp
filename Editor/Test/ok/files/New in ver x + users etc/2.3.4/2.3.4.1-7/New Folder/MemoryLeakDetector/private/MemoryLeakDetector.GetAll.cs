 /MemoryLeakDetector help
function ___MLD&x

int hp=GetCurrentProcess

PROCESS_MEMORY_COUNTERS z.cb=sizeof(z)
if GetProcessMemoryInfo(hp &z sizeof(z))
	x.PageFile=z.PagefileUsage
	x.WorkingSet=z.WorkingSetSize
	x.PagedPool=z.QuotaPagedPoolUsage
	x.NonPagedPool=z.QuotaNonPagedPoolUsage

dll- kernel32 #GetProcessHandleCount hProcess *pdwHandleCount
GetProcessHandleCount(hp &x.nKernel); err ;;XP SP1

x.nGdi=GetGuiResources(hp 0)
x.nUser=GetGuiResources(hp 1)

type __INT100 i[100]
__INT100 ha
int i n nh=GetProcessHeaps(100 &ha)
for i 0 nh
	HeapLock(ha.i[i])
	PROCESS_HEAP_ENTRY e.lpData=0
	rep
		if(!HeapWalk(ha.i[i] &e)) break
		if(!(e.wFlags&PROCESS_HEAP_ENTRY_BUSY)) continue
		x.nHeapAllocations+1
		x.Heap+e.cbData+e.cbOverhead
	HeapUnlock(ha.i[i])
