

int hp=GetCurrentProcess

dll- kernel32 #GetProcessHandleCount hProcess *pdwHandleCount
int nh; GetProcessHandleCount(hp &nh); err ;;XP SP1

PROCESS_MEMORY_COUNTERS x.cb=sizeof(x)
if(!GetProcessMemoryInfo(hp &x sizeof(x))) ret

out "MEMORY KB: Working set %i, Paged/NP pool %i/%i, Page file %i.  HANDLES: Kernel %i, GDI %i, User %i" x.WorkingSetSize/1024 x.QuotaPagedPoolUsage/1024 x.QuotaNonPagedPoolUsage/1024 x.PagefileUsage/1024 nh GetGuiResources(hp 0) GetGuiResources(hp 1)
