 /
function# [goonline]

 Obsolete. Use IntIsConnected and/or IntGoOnline.
 Returns: 1 connected, -1 connected but offline, 0 not connected.

int flags
if(InternetGetConnectedState(&flags 0) and flags&INTERNET_CONNECTION_CONFIGURED)
	if(flags&INTERNET_CONNECTION_OFFLINE)
		if(goonline) ret iif(InternetGoOnline(0 0 0) 1 -1)
		else ret -1
	ret 1
