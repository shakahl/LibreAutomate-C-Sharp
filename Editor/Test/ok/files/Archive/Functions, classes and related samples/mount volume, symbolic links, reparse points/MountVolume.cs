 /
function ~vol ~mountpoint

 Mounts volume vol to mountpoint.
 Error if fails.

 vol - volume name, eg "D:\".
 mountpoint - folder that will be mount point of vol. Can exist or not, as mount point or empty folder.


if(!vol.len or !mountpoint.len) end ES_BADARG
if(!vol.end("\")) vol+"\"
mountpoint.expandpath
mountpoint.rtrim("\")
if(dir(mountpoint 2)) UnmountVolume mountpoint
mkdir mountpoint; err end _error
mountpoint+"\"
 Vista bug: incorrect icon on desktop if the folder did not exist.
 To fix it, here could be inserted some delay.
 However (another Windows bug?) with 1 s delay, the folder becomes not as shortcut,
 and when you try to delete it, deletes all files in the volume. With eg
 5 s delay works well. Or maybe randomly, I did not test enough.
 Tried SHChangeNotify.

if(!GetVolumeNameForVolumeMountPoint(vol _s.all(100) 100) or !SetVolumeMountPointW(@mountpoint @_s.lpstr)) end _s.dllerror
