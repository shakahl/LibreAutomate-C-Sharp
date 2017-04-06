function$ $sinp from nc

 Unicode versdion of "get" macro, it also serves left

 Error if from invalid.
 If nc < 0 or too big, gets all right part.

str s.unicode(sinp) ;;convert to UTF-16
from*2; nc*2
if(from<0 or from>s.len) end ERR_BADARG
if(nc<0 or from+nc>s.len) nc=s.len-from
this.ansi(s+from -1 nc/2)
ret this
