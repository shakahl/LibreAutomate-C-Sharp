function $server @port [timeoutMS]

 Creates client socket and connects to server.
 Error if failed.

 server - server address (eg "www.xxx.com"), or computer name, or IP ("xxx.xxx.xxx.xxx").
   To connect to a server on this computer, can be used "localhost" or "127.0.0.1" or "".
 port - port number.
 timeoutMS - if 0, just calls ClientConnect. Else calls ClientConnect in other thread and waits max 


opt noerrorshere 1
if(!timeoutMS) ret ClientConnect(server port)

if(m_socket) Close


int ht=mac("sub.Thread" "" server port)
wait timeoutMS/1000.0 H ht

 GetExitCodeThread(ht &m_socket)
GetExitCodeThread(ht &_i)
out _i


#sub Thread


CreateSocket

hostent* hp
int ip
__sockaddr_in sa

ip=inet_addr(server)
if(ip==INADDR_NONE or ip=0) hp=gethostbyname(server)
else hp=gethostbyaddr(+&ip 4 AF_INET)
if(!hp) E(1)

int* p=hp.h_addr_list[0]
sa.sin_addr=*p
sa.sin_family=AF_INET
sa.sin_port=htons(port)

if(sock_connect(m_socket &sa sizeof(sa))) E(1)


 TcpSocket* x._new

 wait timeoutMS/1000.0 H mac("sub.Thread" "" x server port)
 
 memcpy &this x sizeof(TcpSocket)
 memset x 0 sizeof(TcpSocket)
 x._delete
 
 
 #sub Thread
 function TcpSocket*x str'server port
 
 x.ClientConnect(server port)
