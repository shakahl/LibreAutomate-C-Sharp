 assuming \\Q7C\app is mapped to drive Z:
str path="Z:\app\qm.exe" ;;path on remote computer
str s.all(500)
int n=500
int e=WNetGetUniversalName(path UNIVERSAL_NAME_INFO_LEVEL s &n)
if(e) end _s.dllerror("" "" e)
lpstr unc; memcpy &unc s 4
out unc ;;\\Q7C\app\app\qm.exe
 or out s+4
