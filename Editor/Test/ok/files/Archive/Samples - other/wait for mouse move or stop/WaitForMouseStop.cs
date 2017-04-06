 /
function double'idleTime

 Waits until mouse movement ends.

 idleTime - number of seconds mouse must be not moving.


POINT p pp
int i t0 it=idleTime*1000
xm pp
rep
	0.05
	xm p
	if(memcmp(&p &pp sizeof(POINT))) i=0
	else if(!i) i=1; t0=GetTickCount
	else if(GetTickCount-t0>=it) ret
	pp=p
