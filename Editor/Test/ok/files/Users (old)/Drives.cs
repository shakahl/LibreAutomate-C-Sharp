 gets paths and types of all drives

out
Wsh.FileSystemObject fso._create
Wsh.Drive dr
foreach dr fso.Drives
	out dr.Path
	out dr.DriveType ;;0 unknown, 1 removable, 2 fixed, 3 network, 4 CD-ROM, 5 RAM disk
	out "---"
