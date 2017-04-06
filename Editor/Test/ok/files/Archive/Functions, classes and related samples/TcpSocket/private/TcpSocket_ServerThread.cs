 \
function servSock clientFunc param

__sockaddr_in sa
rep
	_i=sizeof(sa)
	int sock=sock_accept(servSock &sa &_i)
	if(sock=INVALID_SOCKET) break ;;probably closed servSock
	mac "TcpSocket_ClientThread" "" sock sa.sin_addr clientFunc param; err out _error.description
