 10
rep
	0.5
	out F"loop: thread {GetCurrentThreadId}"
	
	lock ;;makes more reliable; prevents executing code between lock and lock- by multiple threads simultaneously. Delete the lock/lock- lines if don't know how to make it work in your case.
	
	wait 0 -V g_state_of_thread_slave ;;wait if paused
	
	ifa "Notepad" ;;if Notepad window active
		g_state_of_thread_slave=1 ;;pause other threads
		mes F"thread {GetCurrentThreadId} is doing something"
		g_state_of_thread_slave=0 ;;continue other threads
		
	lock-
