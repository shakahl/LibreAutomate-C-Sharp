rep
	out F"loop: thread {GetCurrentThreadId}"
	0.5
	
	lock ;;prevents executing code between lock and lock- by multiple threads simultaneously. Other threads will be paused.
	
	ifa "Notepad" ;;if Notepad window active
		mes F"thread {GetCurrentThreadId} is doing something"
		
	lock-
