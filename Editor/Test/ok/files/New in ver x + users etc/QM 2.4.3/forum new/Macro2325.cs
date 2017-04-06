 start thread that waits for a window or something and then temporarily suspends this thread
QMTHREAD qt; GetQmThreadInfo(0 &qt)
mac "thread_wait_window" "" qt.threadhandle

 main macro code example
int i
for i 0 1000
	1
	out i
