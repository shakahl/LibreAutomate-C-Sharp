function $server @port [^timeoutS]

 Creates client socket and connects to server. Same as ClientConnect but with timeout.
 Error if failed.

 server - server address (eg "www.xxx.com"), or computer name, or IP ("xxx.xxx.xxx.xxx").
   To connect to a server on this computer, can be used "localhost" or "127.0.0.1" or "".
 port - port number.
 timeoutS - if 0, just calls ClientConnect. Else calls ClientConnect in other thread and waits max timeoutS s; error on timeout.


opt noerrorshere 1
if(!timeoutS) ret ClientConnect(server port)

if(m_socket) Close

type __TCPSOCK_CLIENTCONNECT TcpSocket'sock ~server @port !freeThis !isError
__TCPSOCK_CLIENTCONNECT* x._new
x.server=server; x.port=port

wait timeoutS H mac("sub.Thread" "" x)
err ;;timeout
	x.freeThis=1
	end _error

if x.isError ;;x.sock.ClientConnect error
	_s=x.server
	x._delete
	end _s

 success. Copy from x.sock to this, and clear x.sock to disable dtor.
memcpy &this &x.sock sizeof(TcpSocket)
memset &x.sock 0 sizeof(TcpSocket)
x._delete


#sub Thread
function __TCPSOCK_CLIENTCONNECT*x

x.sock.ClientConnect(x.server x.port)
err if(!x.freeThis) x.server=_error.description; x.isError=1
if(x.freeThis) x._delete ;;timeout

 TODO: now x is not deleted if main thread is ended eg by the user
