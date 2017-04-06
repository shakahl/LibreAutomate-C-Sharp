function! str&s [nBytesMax] [flags] ;;flags: 1 append

 Receives server's response.
 Returns 1. If failed, returns 0.

 s - receives server's response.
 nBytesMax - max number of bytes to receive. Default: 0.
   If 0, receives all, until server closes connection. However, if server does not close connection, this function will hang; in such case use nBytesMax.
   If not 0, this function may receive nBytesMax or less data. If less, s.len will be < nBytesMax. It can happen in two cases:
      1. Not all server data is received (server may send data in parts). To get more data, call this function again, until you somehow know that all data is received.
      2. All server data is received, but nBytesMax is specified greater. It's OK.


if(flags&1=0) s.fix(0)
int nr(8100) nr2 nrAll
if(nBytesMax and nBytesMax<nr) nr=nBytesMax
str sb.all(nr 2)

rep
	nr=sb.len
	if(nBytesMax and nrAll+nr>nBytesMax) nr=nBytesMax-nrAll; if(!nr) break
	
	nr2=recv(m_socket sb nr 0)
	 out nr2
	if(nr2<0) ret
	
	if(nr2) s.fromn(s s.len sb nr2); nrAll+nr2
	
	if(nBytesMax) if(nr2!nr) break
	else if(!nr2) break

ret 1
