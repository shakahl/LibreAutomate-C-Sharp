 /
function# str&ip

 Retrieves IP address of this computer.
 If there are multiple IP addresses, they are stored in multiple lines.
 Returns 1 on success, 0 on failure.

 EXAMPLE
 str ip
 if(GetMyIpAddress(ip))
	 out ip


int+ __wsa_init
if(!__wsa_init)
	__wsa_init=1
	WSADATA wsaData
	WSAStartup(0x0002 &wsaData)

hostent* hp = gethostbyname(0); if(!hp) ret
int i
ip=""
for i 0 1000
	if(hp.h_addr_list[i] && hp.h_length>=4) else break
	if(i) ip+="[]"
	byte* b=hp.h_addr_list[i]
	ip.formata("%u.%u.%u.%u" b[0] b[1] b[2] b[3])
if(ip=="127.0.0.1") ip=""
ret 1
