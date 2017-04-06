
 Closes socket.
 Also disconnects, if connected.
 If it is server, stops it. However does not end threads of client connections.
 Called implicitly when destroying the variable.


if m_socket
	sock_shutdown(m_socket SD_BOTH)
	closesocket(m_socket)
	m_socket=0
	m_isServer=0
