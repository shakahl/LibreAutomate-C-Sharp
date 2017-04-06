 /
function# [$connection]

 Checks dial-up networking connection state.
 Returns 1 if connected, 0 if not.
 If connection is not specified - default connection.


#compile rasapi

str s=connection; if(!s.len) RasGetDefConn(s 1)
ARRAY(RASCONN) a
int i r=RasGetConnections(a); if(r or !a.len) ret
RASCONNSTATUS rs.dwSize=sizeof(RASCONNSTATUS)
for i 0 a.len
	if(s.len and q_stricmp(&a[i].szEntryName s)) continue
	if(RasGetConnectStatus(a[i].hrasconn &rs)) ret
	ret rs.rasconnstate=RASCS_Connected
