function nAsm

#if !EXE

int+ ___CsScript_nAsm
if(nAsm=___CsScript_nAsm) ret
___CsScript_nAsm=nAsm
 out nAsm
if(nAsm%100) ret
out nAsm

PROCESS_MEMORY_COUNTERS pm.cb=sizeof(pm)
if(GetProcessMemoryInfo(GetCurrentProcess &pm pm.cb))
	out pm.WorkingSetSize/1024/1024
	if(pm.WorkingSetSize/1024/1024>30) SetProcessWorkingSetSize GetCurrentProcess -1 -1
