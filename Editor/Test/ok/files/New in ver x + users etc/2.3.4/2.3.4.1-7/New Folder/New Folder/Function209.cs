int am asm
if(!GetProcessAffinityMask(GetCurrentProcess &am &asm)) ret
out am
SetProcessAffinityMask GetCurrentProcess am&1
SetThreadPriority GetCurrentThread THREAD_PRIORITY_TIME_CRITICAL

out __FUNCTION__
Q &q
int n
memset g_smt 0 g_smt.len
rep 1000
	if(memcmp(g_smt g_smt0 g_smt.len)) n+1
if(n) out "overwritten %i times" n
Q &qq; outq

SetThreadPriority GetCurrentThread THREAD_PRIORITY_NORMAL
SetProcessAffinityMask GetCurrentProcess am
