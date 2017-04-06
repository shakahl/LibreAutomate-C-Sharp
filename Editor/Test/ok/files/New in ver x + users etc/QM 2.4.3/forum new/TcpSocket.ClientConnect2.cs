function $server $portEtc [timeoutMS]

 Tried to use WSAConnectByName because it supports timeout.
 Fails: Socket error: The specified class was not found.

 Creates client socket and connects to server.
 Error if failed.

 server - server address (eg "www.xxx.com"), or computer name, or IP ("xxx.xxx.xxx.xxx").
   To connect to a server on this computer, can be used "localhost" or "127.0.0.1" or "".
 portEtc - port number and tcp or udp, like "25/tcp". Well-known strings are listed in file "%WINDIR%\system32\drivers\etc\services".
 timeoutMS - if not 0, the time, in milliseconds, to wait for a response before aborting the call.




if(_winnt<6) ClientConnect(server val(portEtc)); err end _error

dll- ws2_32 [WSAConnectByNameA]#WSAConnectByName s $nodename $servicename *LocalAddressLength SOCKADDR*LocalAddress *RemoteAddressLength SOCKADDR*RemoteAddress timeval*timeout OVERLAPPED*Reserved

CreateSocket

if timeoutMS
	timeval t; timeval* pt=&t
	t.tv_sec=timeoutMS/1000
	t.tv_usec=timeoutMS%1000*1000

if(!WSAConnectByName(m_socket server portEtc 0 0 0 0 pt 0)) E(1)
