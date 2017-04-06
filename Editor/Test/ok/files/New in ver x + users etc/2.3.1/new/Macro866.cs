out
__Handle ht=CreateWaitableTimer(0 0 0)
long t=-10000000
out SetWaitableTimer(ht +&t 1000 &TimerAPCProc +100 0)

 MessageLoop

 rep
	 SleepEx(100 1)

 opt waitmsg 1
 wait 5
