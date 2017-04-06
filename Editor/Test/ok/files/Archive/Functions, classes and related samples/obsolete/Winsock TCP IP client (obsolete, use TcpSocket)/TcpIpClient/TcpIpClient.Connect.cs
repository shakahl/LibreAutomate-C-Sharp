function! $server @port

 Creates socket and connects to server.
 Returns 1. If failed, returns 0.

 server - server address (eg "www.xxx.com"), or computer name, or IP ("xxx.xxx.xxx.xxx").
   To connect to a server on this computer, can be used "localhost" or "127.0.0.1".
 port - port number, eg 80 for http.


if(!__WinsockInit) ret
Close

_i=socket(AF_INET SOCK_STREAM IPPROTO_TCP); if(_i==INVALID_SOCKET) ret
m_socket=_i

type __sockaddr_in @sin_family @sin_port sin_addr !sin_zero[8] 

hostent* hp
int ip
__sockaddr_in sa

ip=inet_addr(server)
if(ip==INADDR_NONE or ip=0) hp=gethostbyname(server)
else hp=gethostbyaddr(+&ip 4 AF_INET)
if(!hp) ret

int* p=hp.h_addr_list[0]
sa.sin_addr=*p
sa.sin_family=AF_INET
sa.sin_port=htons(port)

ret !connect(m_socket +&sa sizeof(sa))
