function! n
n=sqrt(n/2+1)
 out n
 Q &q

dll- kernel32 #QueryThreadCycleTime ThreadHandle %*CycleTime
int xp=_winnt<6
 int xp=1

if !xp
	__Handle ht=OpenThread(THREAD_QUERY_LIMITED_INFORMATION 0 m_tid)
	if(!ht) xp=1

 SetThreadPriority GetCurrentThread -2
 SetThreadPriority ht 2

long t pt
int i ni R
for i 0 1000
	if(xp) t=GetThreadContextSwitches(m_tid m_pid); if(!t) break
	else if(!QueryThreadCycleTime(ht &t)) break
	
	if i
		if t=pt
			ni+1
			if(ni>=n) R=1; break
		else ni=0
	pt=t
	
	SleepMP

 out "%i %i" n i

 SetThreadPriority GetCurrentThread 0
 SetThreadPriority ht 0

 Q &qq; outq
ret R

 NOTE: this is unreliable. Eg Word 97 always uses some CPU; would wait forever.
