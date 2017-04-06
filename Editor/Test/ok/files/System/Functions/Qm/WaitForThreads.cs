 /
function ^timeout $thread1 [$thread2] [$thread3] [$thread4] [$thread5] [$thread6] [$thread7] [$thread8] [$thread9] [$thread10]

 Waits until all specified threads will end.

 timeout - max wait time in seconds. If 0, infinite. Error if a thread is still running after that time.
 thread1-thread10 - names of thread main functions.
   If sub-function, can be like "ParentName:SubName".

 Added in: QM 2.3.0.

 EXAMPLE
 mac "Function64"
 mac "Function67"
 WaitForThreads 0 "Function64" "Function67"


if(timeout<0) end ERR_BADARG
int i nt=getopt(nargs)-1
lpstr* t=&thread1
QMTHREAD q

opt waitmsg -1
0.1
rep
	for(i 0 nt) if(EnumQmThreads(&q 1 0 t[i])) break
	if(i=nt) break
	int t1=GetTickCount
	wait timeout H q.threadhandle; err end _error
	if(timeout>0) timeout-GetTickCount-t1/1000.0; if(timeout<=0) timeout=0.01
