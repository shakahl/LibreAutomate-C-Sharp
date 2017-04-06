 /
function $threadName flags ;;flags: 0 pause, 1 resume

 Pauses (suspends) or resumes a QM thread.

 threadName - <help #IDP_THREADS>thread</help> name.
   This function pauses/resumes all threads of threadName.
   If "", pauses/resumes all macro threads (except self), but not function, menu etc threads.

 REMARKS
 Call this function from another thread.
 You must always resume paused threads. QM does not know about paused threads and cannot end them correctly.
 If you pause a thread several times (when it is already paused), you must resume it the same number of times (or more, it is safe). It will remain paused if you resume it just once.

 EXAMPLE
 ThreadPauseResume "Function293" 0
 mes "resume"
 ThreadPauseResume "Function293" 1


int h i n m=empty(threadName)
n=EnumQmThreads(0 0 0 threadName); if(!n) ret
ARRAY(QMTHREAD) a.create(n)
EnumQmThreads &a[0] n 0 threadName
for i 0 a.len
	if(a[i].threadid=GetCurrentThreadId) continue
	if m
		QMITEM q; qmitem(a[i].qmitemid 0 q)
		if(q.itype) continue
	h=a[i].threadhandle
	if(flags&1) ResumeThread h
	else SuspendThread h
