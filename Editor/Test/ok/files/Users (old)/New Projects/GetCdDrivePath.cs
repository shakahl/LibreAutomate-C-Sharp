 /
function# str&s

 int i
 foreach s i FE_Drive 32
	 out s

Wsh.FileSystemObject fso._create
Wsh.Drive dr
foreach dr fso.Drives
	if(dr.DriveType=4) s=dr.Path; ret 1
