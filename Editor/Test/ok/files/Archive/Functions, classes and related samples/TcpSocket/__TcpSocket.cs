class TcpSocket -m_socket -!m_isServer

type __sockaddr_in @sin_family @sin_port sin_addr !sin_zero[8] 

 use different function names to avoid conflicts
dll ws2_32
	[shutdown]#sock_shutdown s how
	[send]#sock_send s $buf len flags
	[recv]#sock_recv s $buf len flags
	[connect]#sock_connect s !*name namelen
	[bind]#sock_bind s !*name namelen
	[listen]#sock_listen s backlog
	[accept]#sock_accept s !*addr *addrlen
	[inet_ntoa]$sock_inet_ntoa in
	[select]#sock_select nfds fd_set*readfds fd_set*writefds fd_set*exceptfds timeval*timeout

#ifndef ERR_FAILED
def ERR_FAILED "573 failed"
#endif
