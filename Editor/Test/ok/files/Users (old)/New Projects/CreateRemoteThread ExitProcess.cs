int pid tid ph th

 int h=win("Notepad")
 GetWindowThreadProcessId(h &pid)

pid=ProcessNameToId("notepad" 0)
out pid

ph=OpenProcess(PROCESS_CREATE_THREAD|PROCESS_QUERY_INFORMATION|PROCESS_VM_OPERATION|PROCESS_VM_WRITE|PROCESS_VM_READ 0 pid)
out ph
if(!ph) ret

th=CreateRemoteThread(ph 0 0 &ExitProcess 0 0 &tid)
out th

1
CloseHandle ph
CloseHandle th
