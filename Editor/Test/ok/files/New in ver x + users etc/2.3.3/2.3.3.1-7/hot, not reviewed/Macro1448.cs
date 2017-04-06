str IP
GetIpAddress "" IP

 IP="84.32.120.112"

GetIpAddress "" _s ;;just to call WSAStartup
int ia=inet_addr(IP)
hostent* hp = gethostbyaddr(+&ia 4 AF_INET)
out hp.h_name
