 /
function# [$tryurl]
 
 Returns 1 if there is alive Internet connection.
 If tryurl is used, tries to connect to specified URL, and returns 1 if it is reachable, or 0 if not.
 You can also use IntIsConnected.

 Requires Win2000/XP

type QOCINFO dwSize dwFlags dwInSpeed dwOutSpeed
dll sensapi #IsNetworkAlive *lpdwFlags
dll sensapi #IsDestinationReachable $lpszDestination QOCINFO*lpQOCInfo

if(len(tryurl))
	QOCINFO q.dwSize=sizeof(QOCINFO)
	if(!IsDestinationReachable(tryurl &q)) ret
else
	if(!IsNetworkAlive(&_i)) ret
	if(_i&2=0) ret

ret 1
