 /Macro2337
function hwnd what str&s [flags] ;;what: 0 command line, 1 current directory, 2 image path, 3 environment variables.  flags: 1 hwnd is process id.

 Does not work with 64-bit processes.

int is64=IsWindow64Bit(hwnd flags&1)
if(is64<0) end ERR_FAILED
if(is64) end "this function does not work with 64-bit processes" ;;pbi.PebBaseAddress is 0

int offs
sel what
	case 0 offs=0x40
	case 1 offs=0x24
	case 2 offs=0x38
	case 3 offs=0x44
	case else end ERR_BADARG

__ProcessMemory m.Alloc(hwnd 0 flags&1)
int hp=m.hprocess

PROCESS_BASIC_INFORMATION pbi
if(NtQueryInformationProcess(hp 0 &pbi sizeof(pbi) 0)) end ERR_FAILED
byte* pp
m.ReadOther(&pp pbi.PebBaseAddress+16 4)
BSTR b
UNICODE_STRING us
m.ReadOther(&us pp+offs sizeof(us))
pp=us.Buffer
int i n
if what=3 ;;not UNICODE_STRING, length unknown
	MEMORY_BASIC_INFORMATION mi
	VirtualQueryEx(hp pp &mi sizeof(mi))
	n=mi.RegionSize-(pp-mi.BaseAddress)
	if(n>0x10000) n=0x10000 ;;assume max 32K chars. Usually 2-4K. On XP max 32K, on 7 etc unlimited.
	n/2; b.alloc(n)
	m.ReadOther(b.pstr pp n*2)
	for(i 0 n) if(b[i]=0) if(b[i+1]) b[i]=10; else break ;;replace \0 to \n until \0\0
	s.ansi(b -1 i)
else
	n=us.Length
	b.alloc(n)
	m.ReadOther(b.pstr pp n*2)
	s=b
