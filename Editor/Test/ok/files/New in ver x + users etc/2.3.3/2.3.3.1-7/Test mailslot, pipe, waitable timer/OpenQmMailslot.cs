 /
function#

int sid; ProcessIdToSessionId(GetCurrentProcessId() &sid)
int ms=CreateFile(F"\\.\mailslot\{sid}\__QM_main" GENERIC_WRITE FILE_SHARE_READ|FILE_SHARE_WRITE 0 OPEN_EXISTING 0 0)
if(ms>0) ret ms
out _s.dllerror
