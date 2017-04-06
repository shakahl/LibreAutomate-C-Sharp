function# [conn]

 Disconnects from the Internet.
 Obsolete. Use IntDisconnect.
 Returns: 1 success, 0 failed.
 conn is connection id, returned by IntDial.
 If conn is 0, breaks all connections.

if(conn) ret InternetHangUp(conn 0)=0
ret IntDisconnect
