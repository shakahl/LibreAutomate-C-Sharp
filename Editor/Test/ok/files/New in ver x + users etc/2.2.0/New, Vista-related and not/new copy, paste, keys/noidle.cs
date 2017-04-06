if(getopt(nthreads)>2) ret
AddTrayIcon
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
int i
for i 0 1000000000
	rep 200000
		ifk(F12) ret
	 out i
	0.001
