function $server @port [timeoutMS]

 Creates client socket and connects to server.
 Error if failed.

 server - server address (eg "www.xxx.com"), or computer name, or IP ("xxx.xxx.xxx.xxx").
   To connect to a server on this computer, can be used "localhost" or "127.0.0.1" or "".
 port - port number.
 timeoutMS - if 0, just calls ClientConnect. Else calls ClientConnect in other thread and waits max 


opt noerrorshere 1
if(!timeoutMS) ret ClientConnect(server port)

this.Close

type __TCPSOCK_CLIENTCONNECT TcpSocket*_this ~server @port QMERROR*pErr
__TCPSOCK_CLIENTCONNECT* x._new
x._this=&this; x.server=server; x.port=port

wait timeoutMS/1000.0 H mac("sub.Thread" "" x)


#sub Thread
function __TCPSOCK_CLIENTCONNECT*x

x._this.ClientConnect(x.server x.port)
