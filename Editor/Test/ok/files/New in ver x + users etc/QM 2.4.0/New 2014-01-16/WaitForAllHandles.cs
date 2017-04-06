 /
function ^waitMax ARRAY(int)&a

 Note: in QM 2.4.0 added wait HMA to wait for all handles.

 Waits until all handles in array are signaled.

 waitmax - max time to wait, s. Error on timeout. 0 is infinite.
 a - array of handles. Max 64.

 Can wait for ended threads, ended processes, events (SetEvent), mutexes and other kernel objects supported by <help>WaitForMultipleObjects</help>.

 EXAMPLE
  start 2 threads and wait until both are ended
 ARRAY(int) ah
 ah[]=mac("Thread1")
 ah[]=mac("Thread2")
 WaitForAllHandles 0 ah


if(waitMax<0 or waitMax>2000000) end ERR_BADARG
if(a.len<1 or a.len>64) end ERR_BADARG

 duplicate handles to avoid "invalid handle" error eg when waiting for multiple threads and QM closes a thread handle
ARRAY(__Handle) ad.create(a.len); int i hcp(GetCurrentProcess)
for(i 0 a.len) if(!DuplicateHandle(hcp a[i] hcp &ad[i] 0 0 DUPLICATE_SAME_ACCESS)) end ERR_FAILED 16

int wt(waitMax*1000) t0(GetTickCount)
rep
	sel WaitForMultipleObjects(ad.len &ad[0] 1 100)
		case WAIT_TIMEOUT
		case -1 end ERR_FAILED 16
		case else ret
	if(wt and GetTickCount-t0>=wt) end ERR_TIMEOUT
	if(getopt(waitmsg 1)) 0
