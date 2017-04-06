 /
function# ARRAY(str)&aa ;;Returns names of all existing connections.

#compile rasapi

ARRAY(RASENTRYNAME) a.create(10); a[0].dwSize=sizeof(RASENTRYNAME)
int i r n cb=10*sizeof(RASENTRYNAME)
r=RasEnumEntries(0 0 &a[0] &cb &n)
if(r=ERROR_BUFFER_TOO_SMALL)
	a.create(cb/sizeof(RASENTRYNAME)); a[0].dwSize=sizeof(RASENTRYNAME)
	r=RasEnumEntries(0 0 &a[0] &cb &n)
if(r) aa.redim; ret r ;;returns 0 or error code
aa.redim(n)
for(i 0 n)
	lpstr ls=&a[i].szEntryName
	aa[i]=ls
