str IP
GetIpAddress "" IP

 IP="84.32.122.84"
IP="84.32.120.112"

GetIpAddress "" _s ;;just to call WSAStartup
int ia=inet_addr(IP)

type SOCKADDR @sa_family !sa_data[14]
dll ws2_32 #GetNameInfoW SOCKADDR*pSockaddr SockaddrLength @*pNodeBuffer NodeBufferSize @*pServiceBuffer ServiceBufferSize Flags

SOCKADDR a
type sockaddr_in @sin_family @sin_port sin_addr $sin_zero[8]
sockaddr_in sa.sin_family=AF_INET
sa.sin_addr=ia
BSTR b.alloc(1100)
if(GetNameInfoW(+&sa sizeof(sa) b b.len 0 0 0)) end "failed"
out b
