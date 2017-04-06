ARRAY(int) a
int t1=timeGetTime
rep 10 ;;max 64
	a[]=mac("test_post" "" _s)
 wait until all threads ended
if(WaitForMultipleObjects(a.len &a[0] 1 INFINITE)=WAIT_FAILED) end _s.dllerror
out timeGetTime-t1
