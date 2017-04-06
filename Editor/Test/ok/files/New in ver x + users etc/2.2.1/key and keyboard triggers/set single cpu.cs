int mask=3 ;;use mask 1 to set single cpu. Use mask 3 to restore.

ARRAY(int) ap
EnumProcessesEx &ap 0 2
int i
for i 0 ap.len
	int hp=OpenProcess(PROCESS_SET_INFORMATION 0 ap[i])
	if(!hp) continue
	
	SetProcessAffinityMask hp mask
	
	CloseHandle hp
	
	