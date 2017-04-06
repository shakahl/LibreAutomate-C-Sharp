int timeout=5 ;;s
int numLastTaps=10

 ______________________________

int t n
ARRAY(int) a.create(numLastTaps)
rep
	out avg
	t=timeGetTime
	if(n<numLastTaps) n+1
	else memmove &a[0] &a[1] n-1*4
	
	wait timeout KF T; err ret
