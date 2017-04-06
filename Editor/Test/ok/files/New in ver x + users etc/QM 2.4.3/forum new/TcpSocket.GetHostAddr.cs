function# $server

ip=inet_addr(server)
if(ip==INADDR_NONE or ip=0) hp=gethostbyname(server)
else hp=gethostbyaddr(+&ip 4 AF_INET)
if(!hp) E(1)

int* p=hp.h_addr_list[0]
ret *p
