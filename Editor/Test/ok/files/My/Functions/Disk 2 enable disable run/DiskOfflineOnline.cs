 /
function! diskNumber !off [$logicalDrive] ;;off: 0 online, 1 offline

 Makes a disk (hard/physical drive) offline (disabled, hidden) or online.
 When a disk is offline, user and software cannot see and access it.
 Returns: 1 success, 0 failed.

 diskNumber - 0-based disk number.
   You can see disk numbers and make offline/online manually in Start -> Administrative Tools -> Computer Management (compmgmt.msc).
 logicalDrive - a drive on the disk, like "E:".
   If you don't provide this, the function fails when enabling if already enabled or disabling if already disabled.
   Waits max 10 s until the drive becomes available/unavailable.

 EXAMPLE
 DiskOfflineOnline 1 1 "E:" ;;disable the second hard drive, where logical drive "E:" is


if(!empty(logicalDrive) and dir(logicalDrive 1)!0 = !off) ret 1

str so=iif(off "off" "on")
str s=
F
 select disk {diskNumber}
 {so}line disk

str sf.expandpath("$temp qm$\DiskOfflineOnline.txt")
s.setfile(sf)

int ok
ok=0=run("diskpart.exe" F"/s ''{sf}''" "" "" 0x10410)
if ok
	if(empty(logicalDrive)) ret 1
	
	rep 100
		ok=dir(logicalDrive 1)!0; if(off) ok=!ok
		if(ok) break; else 0.1
	10

ret ok

err+

 notes:
 On error gives an undocumented error code and useless info.
 Does not work with command line >logfile.txt. Could use runconsole2, but need admin.
 Sometimes, when enabling, makes readonly. Don't know why. To fix, disable/enable manually in Computer Management.
