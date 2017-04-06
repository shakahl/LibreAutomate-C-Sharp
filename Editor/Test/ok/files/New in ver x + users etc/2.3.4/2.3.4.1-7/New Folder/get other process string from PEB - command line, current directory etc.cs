 http://wj32.org/wp/2009/01/24/howto-get-the-command-line-of-processes/

int w=win("Save As" "#32770")

__ProcessMemory m.Alloc(w 1000)

PROCESS_BASIC_INFORMATION pbi
if(NtQueryInformationProcess(m.hprocess 0 &pbi sizeof(pbi) 0)) ret
byte* pp
m.ReadOther(&pp pbi.PebBaseAddress+16 4)
 out pp
UNICODE_STRING us
m.ReadOther(&us pp+0x24 8) ;;current directory
 m.ReadOther(&us pp+0x40 8) ;;command line
 m.ReadOther(&us pp+0x38 8) ;;image path
out us.Length
BSTR b.alloc(us.Length)
m.ReadOther(b.pstr us.Buffer us.Length*2)
out b

 note: this code is only for 32-bit. Tested only on Windows 7.
