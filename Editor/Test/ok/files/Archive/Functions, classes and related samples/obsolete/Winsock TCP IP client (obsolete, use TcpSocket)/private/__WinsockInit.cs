function!

 Calls WSAStartup, if not yet called.
 Automatically calls WSACleanup later.
 Returns 1 if successful or already called.
 Returns 0 if failed.


class __WSASHUTDOWN -!m_inited
__WSASHUTDOWN+ __wsa_sd

if !__wsa_sd
	lock
	if !__wsa_sd
		WSADATA wsaData
		if(WSAStartup(0x202 &wsaData)) ret
		__wsa_sd=1

ret 1
