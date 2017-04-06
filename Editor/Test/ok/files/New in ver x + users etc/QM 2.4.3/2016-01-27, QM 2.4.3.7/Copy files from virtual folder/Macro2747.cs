IShellFolder d
SHGetDesktopFolder &d
out d

Shell32.Folder k=sub.QueryService(d uuidof(Shell32.Folder)) ;;fails
out k


#sub QueryService
function'IUnknown IUnknown'iFrom GUID&iid [GUID&guidService]

IUnknown u
if(!&guidService) &guidService=iid
IServiceProvider sp=+iFrom
sp.QueryService(guidService iid &u); err u=0

err+
ret u
