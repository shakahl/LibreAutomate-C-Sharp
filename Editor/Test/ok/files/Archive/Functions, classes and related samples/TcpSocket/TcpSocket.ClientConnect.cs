function $server @port

 Creates client socket and connects to server.
 Error if failed.

 server - server address (eg "www.xxx.com"), or computer name, or IP ("xxx.xxx.xxx.xxx").
   To connect to a server on this computer, can be used "localhost" or "127.0.0.1" or "".
 port - port number.


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
