 shows "quickmacros2" service state

out
int scm=OpenSCManager(0 0 GENERIC_READ)
if(!scm) end _s.dllerror

int sc=OpenService(scm "quickmacros2" SERVICE_QUERY_STATUS)
if(sc)
	SERVICE_STATUS z
	if(QueryServiceStatus(sc &z))
		if(z.dwCurrentState=SERVICE_RUNNING) out "running"
		else out "stopped or paused"
	CloseServiceHandle sc
else out _s.dllerror

CloseServiceHandle scm
