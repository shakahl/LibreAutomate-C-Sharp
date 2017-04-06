 /
function# ARRAY(RASCONN)&a ;;Returns details of currently active connections.

#compile rasapi

a.create(10); a[0].dwSize=sizeof(RASCONN)
int r n cb=10*sizeof(RASCONN)
r=RasEnumConnections(&a[0] &cb &n)
a.redim(n)
ret r ;;returns 0 or error code
