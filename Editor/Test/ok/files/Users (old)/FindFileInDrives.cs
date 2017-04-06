 /
function# $filename str&sPath [$drives] [driveType] [flags] [DATE'date1] [DATE'date2] ;;driveType: 0 fixed (hard drive)(default), 2 removable (eg flash), 3 same as 0, 4 network, 5 CD, 6 ramdisk;  flags: 0 file, 1 folder, 2 any, 4 not in folders, 8 only in folders, 16 date created

 Finds file and stores full path into sPath.
 Searches in specified drives or in drives of specified type.

 filename - filename, eg "qm.exe"
 sPath - will receive full path
 drives - string consisting of drive letters, eg "cef". Can be "".
 driveType - search in drives of this type. Used only if drives is "".
 date1, date2 - file modified time must be between date1 and date2.

 EXAMPLE
 str s
 if(FindFileInDrives("qm.exe" s)) ;;search for qm.exe in all hard drives
	 out s
 else
	 out "not found"


def DRIVE_UNKNOWN    0
def DRIVE_NO_ROOT_DIR 1
def DRIVE_REMOVABLE  2
def DRIVE_FIXED      3
def DRIVE_REMOTE     4
def DRIVE_CDROM      5
def DRIVE_RAMDISK    6
dll kernel32 #GetLogicalDrives
dll kernel32 #GetDriveType $nDrive

sPath.fix(0)
str s.from(" :\" filename) sd; int i dt dm; Dir d
int nd=len(drives)
if(nd)
	for i 0 nd
		s[0]=drives[i]
		foreach(d s FE_Dir 0x4^flags date1 date2) sPath=d.FileName(1); ret 1
else
	if(!driveType) driveType=DRIVE_FIXED
	sd=" :\"
	dm=GetLogicalDrives ;;32-bit mask of available drives
	for i 0 26
		if(dm>>i&1=0) continue
		sd[0]='A'+i
		dt=GetDriveType(sd); if(dt>6) dt=0
		if(dt!=driveType) continue
		s[0]='A'+i
		foreach(d s FE_Dir 0x4^flags date1 date2) sPath=d.FileName(1); ret 1
