
#ifdef __WsInit ;;QM 2.3.5
__WsInit
#else
type __TcpSocket_Init -!m_inited
__TcpSocket_Init+ __ts_init
if !__ts_init
	lock
	if !__ts_init
		WSADATA wsaData
		if(WSAStartup(0x202 &wsaData)) end ERR_FAILED 2
		__ts_init=1
	lock-
#endif

Close

_i=socket(AF_INET SOCK_STREAM IPPROTO_TCP); if(_i=INVALID_SOCKET) E
m_socket=_i
