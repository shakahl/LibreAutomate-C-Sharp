AddTrayIcon
int t1=GetTickCount
str s="text"
rep 1000 ;;about 10 seconds
	 WaitForSingleObject(g_mutex1 -1)
	lock
	 lock mut "QM_lock_mutex"
	s.setfile("$desktop$\test.txt")
	lock-
	 lock- mut
	 ReleaseMutex(g_mutex1)
	0.01
int t2=GetTickCount
out t2-t1
