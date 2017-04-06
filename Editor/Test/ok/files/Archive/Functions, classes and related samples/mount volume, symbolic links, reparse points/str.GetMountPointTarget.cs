 /
function $mountpoint

 Gets mount point target.
 Error if fails.
 The target is volume name in form "\\?\Volume\{GUID}\". With many functions it can be used instead of drive letter eg "D:\".


__HFile hf.Create(mountpoint OPEN_EXISTING GENERIC_READ 0 FILE_FLAG_OPEN_REPARSE_POINT|FILE_FLAG_BACKUP_SEMANTICS)
this.all(2000 2 0)
int n
if(!DeviceIoControl(hf FSCTL_GET_REPARSE_POINT 0 0 this this.len &n 0)) end _s.dllerror
if(n<=22) end ES_FAILED
this.get(this 20 n-20)

this.ansi
this.fix; this-"\\"

err+ end _error
