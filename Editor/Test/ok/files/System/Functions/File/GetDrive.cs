 /
function'Wsh.Drive $driveletter

 Use this function to get information about a drive. Returns Wsh.Drive object for the drive.
 Error if the specified drive does not exist.
 
 driveletter - drive letter, optionally with : and \. Examples: "C", "c:", C:\".

 EXAMPLE
 Wsh.Drive d=GetDrive("c")
 if(d.IsReady)
	 double asp=d.AvailableSpace
	 out asp/1024/1024 ;;MB
 else out "not ready"


Wsh.FileSystemObject fso._create
Wsh.Drive d=fso.Drives.Item(driveletter); err end ERR_DRIVE
ret d
