int i n
WINAPI2.WTS_PROCESS_INFO* p
WINAPI2.WTSEnumerateProcesses(0 0 1 &p &n)
for i 0 n
	out p[i].pProcessName
WINAPI2.WTSFreeMemory(p)
