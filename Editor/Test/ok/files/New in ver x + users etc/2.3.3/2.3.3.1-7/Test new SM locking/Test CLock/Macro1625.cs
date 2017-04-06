out "--------"
dll "qm.exe"
	#LLock timeout
	LUnlock

int h=mac("thread1")
0.1
 wait 0 H h; 3; mac("thread2"); 0.1

out LLock(5000)
LUnlock
