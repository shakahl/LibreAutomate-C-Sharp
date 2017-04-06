function# str&s nBytesMax [timeout] [flags] ;;flags: 1 append

 Receives data.
 Error if failed.
 Returns: 0 all data received (sender closed connection), 1 possibly more data exists, 2 timeout. In most cases returns 1.

 s - variable for data.
 nBytesMax - max number of bytes to receive. Default: 0.
   This function may receive nBytesMax or less data. Can receive less in two cases:
      1. Not all data is received. To get more data, call this function again, until you somehow know that all data is received.
      2. All data is received, but nBytesMax is specified greater. It's OK.
   If 0, receives all, until sender closes connection or shuts down sending. However, if server does not do it, this function will hang; in such case use nBytesMax.
 timeout - number of seconds to wait for data. If 0, waits forever.


if(flags&1=0) s.fix(0)
int nr(8100) nr2 nrAll
if(nBytesMax and nBytesMax<nr) nr=nBytesMax
str sb.all(nr 2)

rep
	nr=sb.len
	if(nBytesMax and nrAll+nr>nBytesMax) nr=nBytesMax-nrAll; if(!nr) break
	
	if(!Wait(1 timeout)) ret 2
	nr2=sock_recv(m_socket sb nr 0)
	 out nr2
	if(nr2<0) E
	if(!nr2) ret 0
	
	s.fromn(s s.len sb nr2); nrAll+nr2
	
	if(nBytesMax and nr2!nr) break
	
	if(!timeout) timeout=10000

ret 1
