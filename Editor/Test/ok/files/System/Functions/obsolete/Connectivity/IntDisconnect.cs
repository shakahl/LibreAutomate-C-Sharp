 /
function# [$connection]

 Disconnects a dial-up networking connection.
 If connection is not specified, disconnects all.
 Returns: 1 successfully disconnected, -1 was not connected, 0 failed.


#compile rasapi

ARRAY(RASCONN) a
int lc i c rr r=RasGetConnections(a); if(r) ret RasError(r "cannot disconnect" 1)
if(!a.len) ret -1
lc=len(connection)
RASCONNSTATUS rcs.dwSize=sizeof(RASCONNSTATUS)

for i 0 a.len
	if(lc and q_stricmp(&a[i].szEntryName connection)) continue
	c=1
	r=RasHangUp(a[i].hrasconn)
	if(r) rr=1; RasError(r _s.format("cannot disconnect from %s" &a[i].szEntryName) 1)
	else
		opt waitmsg -1
		rep(100) if(RasGetConnectStatus(a[i].hrasconn &rcs)=ERROR_INVALID_HANDLE) break; else 0.1
if(rr) ret
ret iif(c 1 -1)
