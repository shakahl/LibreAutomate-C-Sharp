out
int h r si ne
str s.all(1024)
NETRESOURCE* n=+s

if(WNetOpenEnum(RESOURCE_CONTEXT RESOURCETYPE_DISK 0 0 &h)) ret

rep
	si=s.nc; ne=1
	r=WNetEnumResource(h &ne n &si)
	if(r)
		if(r!=ERROR_MORE_DATA) break
		s.all(si); n=+s; si=s.nc; ne=1
		if(WNetEnumResource(h &ne n &si)) break
	
	if(n.dwDisplayType!=RESOURCEDISPLAYTYPE_SERVER) continue
	out n.lpRemoteName

WNetCloseEnum(h)
