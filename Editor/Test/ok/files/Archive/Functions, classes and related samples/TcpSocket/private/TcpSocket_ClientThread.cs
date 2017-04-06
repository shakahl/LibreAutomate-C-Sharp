 \
function clientSock clientIp clientFunc param

TcpSocket x.set_handle(clientSock)
call clientFunc &x sock_inet_ntoa(clientIp) param 0

sock_shutdown x.handle SD_BOTH
x.set_handle(0) ;;don't close
