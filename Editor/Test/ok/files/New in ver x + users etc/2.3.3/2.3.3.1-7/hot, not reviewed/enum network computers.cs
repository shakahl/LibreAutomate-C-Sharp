def MAX_PREFERRED_LENGTH 0xFFFFFFFF
def SV_TYPE_NT 0x00001000
type SERVER_INFO_100 sv100_platform_id @*sv100_name
dll netapi32 #NetServerEnum @*servername level !**bufptr prefmaxlen *entriesread *totalentries servertype @*domain *resume_handle
dll netapi32 #NetApiBufferFree !*Buffer

SERVER_INFO_100* p
int n nt
if(NetServerEnum(0 100 &p MAX_PREFERRED_LENGTH &n &nt SV_TYPE_NT 0 0)) end "failed"

int i
for i 0 n
	str s.ansi(p[i].sv100_name)
	out s
	
	 GetIpAddress s _s; out _s

NetApiBufferFree p
