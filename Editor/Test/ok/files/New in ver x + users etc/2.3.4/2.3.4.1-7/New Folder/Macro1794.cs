out
int pid=GetCurrentProcessId()
Q &q
int hsnap=CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0)
THREADENTRY32 te.dwSize=sizeof(te)
Thread32First(hsnap &te)
rep
	if te.th32OwnerProcessID=pid
		 out te.th32ThreadID
		int ht=OpenThread(THREAD_ALL_ACCESS 0 te.th32ThreadID)
		out ht
		CloseHandle ht
	if(!Thread32Next(hsnap &te)) break

CloseHandle hsnap
Q &qq
outq
