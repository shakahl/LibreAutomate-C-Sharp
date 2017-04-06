 /
function# [double'maxWaitTime]

 Waits until mouse movement begins.
 Returns 1 if movement began, 0 if not.

 maxWaitTime - number of seconds to wait. Default or 0 - infinite.


int wt=maxWaitTime*1000
int t1=GetTickCount
POINT p p0
xm p0
rep
	0.05
	xm p
	if(memcmp(&p &p0 sizeof(POINT))) ret 1
	if(wt and GetTickCount-t1>=wt) ret
