long tce tkernel tuser t1 t2

int ht=GetCurrentThread; if(!ht) end _s.dllerror

 rep 200000
	 int i=9
1

out GetThreadTimes(ht +&tce +&tce +&tkernel +&tuser)
out tkernel+tuser/10000

 precision = about 15 ms
