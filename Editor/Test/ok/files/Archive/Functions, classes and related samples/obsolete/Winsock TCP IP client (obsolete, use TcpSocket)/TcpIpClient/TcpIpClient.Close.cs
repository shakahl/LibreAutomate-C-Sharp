function!

 Closes socket. It also disconnects from server.
 Returns 1 if succeeded or already closed. If failed, returns 0.


if(m_socket and closesocket(m_socket)) ret
m_socket=0
ret 1
