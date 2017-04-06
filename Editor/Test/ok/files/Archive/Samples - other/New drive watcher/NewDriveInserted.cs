 \
function str'drive

Wsh.Drive d=GetDrive(drive)

 out drive
 out d.DriveType
 out d.IsReady

sel d.DriveType
	case 1 ;;flash
	
	case 4 ;;CD
	ret
	
	case else ;;should never be
	ret


 add your flash drive copying code here.
 example:

str n=d.VolumeName; if(n.len) n.fix(50 2); n-" ("; n+")"
str dest.from("$desktop$\flash drives\" d.SerialNumber n)
out "<>Copying <link>%s</link> to %s[]Please wait..." drive dest
cop _s.from(drive "*") dest 0x2C0~FOF_FILESONLY
err out _error.description; ret
out "<>Finished copying  <link>%s</link> to <link>%s</link>" drive dest

 note: several drives can be copied simultaneously
