 \
function slower

SetThreadAffinityMask GetCurrentThread 1
if(slower) SetThreadPriority GetCurrentThread -1

int t1=timeGetTime
rep 1000
	int x=1+2
	Sleep 1
int t2=timeGetTime
out "%s %i" iif(slower "slower" "normal") t2-t1
