int serv,scm;
scm=OpenSCManager(0,0,SC_MANAGER_CREATE_SERVICE);
if(!scm) ret 0;
serv=OpenService(scm,"RunAsEx temporary service",SERVICE_ALL_ACCESS);

if(serv)
	out DeleteService(serv);
	CloseServiceHandle(serv);

CloseServiceHandle(scm);
