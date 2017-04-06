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

TcpSocket* x._new

wait timeoutMS/1000.0 H mac("sub.Thread" "" x server port)

memcpy &this x sizeof(TcpSocket)
memset x 0 sizeof(TcpSocket)
x._delete


#sub Thread
function TcpSocket*x str'server port

x.ClientConnect(server port)
