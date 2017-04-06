 /
function $drive ;;drive: eg "D:", can be unavailable/ejected

 Deletes the memory cache of all files in specified drive so that next time a file read function would get data from disk (and create new cache).

if(!FileExists(drive 1)) ret

int w=win("VMware Player" "VMPlayerFrame")
if(w) end "DeleteFileCacheAll error: deleting file cache would crash VMware Player."

str sd.from("\\.\" drive)
__HFile f.Create(sd OPEN_EXISTING FILE_READ_DATA FILE_SHARE_READ)
err ;;deletes cache, then fails
