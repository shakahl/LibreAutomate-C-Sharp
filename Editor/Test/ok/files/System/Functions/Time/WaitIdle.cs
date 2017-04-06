 /
function idleTimeS [waitMaxS]

 Waits for user input idle.
 It is time since the last mouse or keyboard event.
 Error on timeout.

 idleTimeS - idle time. Seconds, 0 to 2000000.
 waitMaxS - maximal time to wait. Seconds, 0 to 2000000. If 0, waits forever, else throws error after waitMaxS seconds.

 Added in: QM 2.3.2.

 EXAMPLE
 WaitIdle 30 3600; err ret ;;wait for user input idle max 1 hour. Idle time is 30 s.


if(idleTimeS<0 or idleTimeS>2000000 or waitMaxS<0 or waitMaxS>2000000) end ERR_BADARG
waitMaxS*1000
opt waitmsg -1
int t0=GetTickCount
rep
	if(GetIdleTime>=idleTimeS) break
	1
	if(waitMaxS and GetTickCount-t0>=waitMaxS) end ERR_TIMEOUT
