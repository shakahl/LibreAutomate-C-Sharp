 /
function $symlink

 Gets symbolic link target.
 Error if fails.
 Symbolic links are supported on Vista and later.


__HFile hf.Create(symlink OPEN_EXISTING GENERIC_READ 0 FILE_FLAG_OPEN_REPARSE_POINT)
this.all(2000 2 0)
int n
if(!DeviceIoControl(hf FSCTL_GET_REPARSE_POINT 0 0 this this.len &n 0)) end _s.dllerror
if(n<=22) end ES_FAILED
this.get(this 20 n-20)

this.ansi
n=find(this "\??"); if(n<0) end ES_FAILED
this.fix(n)

err+ end _error
