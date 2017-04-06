int pid = ProcessNameToId("notepad" 0 1)
__ProcessMemory m.Alloc(pid 0 1) ;;just opens process, does not allocate
int scp
m.ReadOther(+0x0100A5f1, &scp, 2)
err end _error
out scp

int pid = ProcessNameToId("notepad" 0 1)
int pHandle = OpenProcess(PROCESS_ALL_ACCESS 0 pid)
int scp
if(ReadProcessMemory(pHandle, +0x0100A5f1, &scp, 2, 0))
	out scp
else
	out "aa"
CloseHandle(pHandle)
