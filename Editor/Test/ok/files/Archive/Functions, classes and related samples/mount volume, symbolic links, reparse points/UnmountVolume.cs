 /
function ~mountpoint

 Unmounts mountpoint and deletes the empty folder.
 Error if fails.


mountpoint.expandpath
if(!mountpoint.end("\")) mountpoint+"\"

if(!DeleteVolumeMountPointW(@mountpoint)) end _s.dllerror
del mountpoint.rtrim("\"); err end _error
