 /
function# str&name int&drivetype [drivetypemask]

 This function is used with foreach, to enumerate drives.
 drivetype - if 0, all drive types match. Can be combination of flags:
   1 unknown, 2 no root dir, 4 removable, 8 fixed (hard drive), 16 remote (network), 32 cdrom (CD, DVD), 64 ramdisk


int i dt dm
str s(" :\") s2
if(i=0)
	dm=GetLogicalDrives ;;32-bit mask of available drives
	for i 0 26
		if(dm>>i&1)
			s[0]='A'+i
			dt=GetDriveType(s); if(dt>6) dt=0
		s2.getl("UNKNOWN[]NO_ROOT_DIR[]REMOVABLE[]FIXED[]REMOTE[]CDROM[]RAMDISK" dt)
		out "%s %s" s s2
