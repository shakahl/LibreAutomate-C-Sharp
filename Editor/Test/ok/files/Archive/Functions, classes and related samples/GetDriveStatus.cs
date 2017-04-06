 /
function# $drivename ;;returns: 0 does not exist, 1 ready, 2 not ready

 EXAMPLES
 out GetDriveStatus("a:\")
 out GetDriveStatus("\\MyServer\MyShare\")


int i=GetDriveType(drivename)
if(i=DRIVE_NO_ROOT_DIR) ret 0

int pem=SetErrorMode(SEM_FAILCRITICALERRORS)
i=GetDiskFreeSpaceEx(drivename 0 0 0)
SetErrorMode(pem)
if(i) ret 1
ret 2
