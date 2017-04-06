 When using SetThreadContext to end this thread, it works only if this thread is not waiting.
 If it is waiting, ends only when stops waiting.

 rep
	 _i=0

Sleep 4000

 WaitForSingleObject GetCurrentThread 4000
 WaitForSingleObjectEx GetCurrentThread 4000 1
