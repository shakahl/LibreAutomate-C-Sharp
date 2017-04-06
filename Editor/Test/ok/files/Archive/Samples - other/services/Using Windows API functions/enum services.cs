 Shows list of services.
 This method is not so easy but faster than clsServices.

out
int scm=OpenSCManager(0 0 SC_MANAGER_ENUMERATE_SERVICE)
if(!scm) end _s.dllerror

int i n ne=64*1024/sizeof(ENUM_SERVICE_STATUS)
ARRAY(ENUM_SERVICE_STATUS) a.create(ne)
if(!EnumServicesStatus(scm SERVICE_WIN32 SERVICE_STATE_ALL &a[0] ne*sizeof(ENUM_SERVICE_STATUS) &i &n 0))
 if(!EnumServicesStatus(scm SERVICE_WIN32 SERVICE_INACTIVE &a[0] ne*sizeof(ENUM_SERVICE_STATUS) &i &n 0))
	_s.dllerror
	CloseServiceHandle scm
	end _s

for i 0 n
	out a[i].lpDisplayName

CloseServiceHandle scm
