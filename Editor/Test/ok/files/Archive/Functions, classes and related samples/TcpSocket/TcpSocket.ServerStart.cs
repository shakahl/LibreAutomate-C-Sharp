function @port clientFunc [param] [flags] ;;flags: 1 don't wait

 Creates server socket and listens (or starts listening).
 Error if failed.

 port - port number. Recommended values are between 5000 and 65000.
 clientFunc - address of a user-defined function that will be called when a client connects.
   The function must begin with:
   function TcpSocket&client $clientIp param !*reserved
      client - client socket. You can call Send and Receive with it.
      clientIp - client IP string.
      param - param of ServerStart.
      reserved - currently not used.
   The function runs in separate thread for each connection, therefore multiple clients can connect simultaneously.
   It is possible that the thread runs after ending function/thread that called ServerStart. Therefore use param carefully.
 param - some value to pass to the clientFunc function.
 flags:
   1 - start listening and don't wait. If not used, this function does not return until you end this thread or call Close.

 REMARKS
 When using in dialog procedure, use TcpSocket variable with thread scope, and call this function with flag 1.


CreateSocket

__sockaddr_in sa
sa.sin_family=AF_INET
sa.sin_port=htons(port)
if(sock_bind(m_socket &sa sizeof(sa))) E(1)
if(sock_listen(m_socket SOMAXCONN)) E(1)
m_isServer=1

_i=mac("TcpSocket_ServerThread" "" m_socket clientFunc param)
if(flags&1) ret
opt waitmsg -1
wait 0 H _i

 notes:
 Always uses other thread because otherwise could not correctly end this thread. Now, when ending this thread, dtor closes socket, then accept() fails and it breaks loop.
