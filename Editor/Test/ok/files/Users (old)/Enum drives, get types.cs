dll kernel32 #GetLogicalDrives
dll kernel32 #GetDriveType $nDrive

int i dt dm
str s(" :") s2
dm=GetLogicalDrives ;;32-bit mask of available drives
for i 0 32
	if(dm>>i&1)
		s[0]='A'+i
		dt=GetDriveType(s); if(dt>6) dt=0
		s2.getl("UNKNOWN[]NO_ROOT_DIR[]REMOVABLE[]FIXED[]REMOTE[]CDROM[]RAMDISK" dt)
		out "%s %s" s s2
