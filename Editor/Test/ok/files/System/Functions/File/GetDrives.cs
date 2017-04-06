 /
function# ARRAY(Wsh.Drive)&a [drivetype] ;;drivetype: 1 removable (flash, floppy), 2 fixed (hard drive), 3 network, 4 CD-ROM, 5 RAM disk

 Use this function to enumerate available drives and get their properties.
 Returns the number of elements in the array.

 a - variable for results. The function populates it with Wsh.Drive objects, where each element represents one of available drives.
 drivetype - if used and not 0, retrieves only drives of the specified type.

 EXAMPLE
 ARRAY(Wsh.Drive) a; int i j; str s
 GetDrives a
 for i 0 a.len
	 s=a[i].DriveLetter
	 if(a[i].IsReady)
		 j=a[i].AvailableSpace/1024/1024 ;;free space available to the current user
		 out "%s free space=%i MB" s j
	 else out "%s not ready" s
	 ;;note: if a function returns BSTR or VARIANT, assign it to a str or other variable (like in the example), because BSTR and VARIANT cannot be used with most QM functions


a=0
Wsh.FileSystemObject fso._create
Wsh.Drive dr
foreach dr fso.Drives
	if(drivetype && drivetype!=dr.DriveType) continue
	a[]=dr
ret a.len
