 /exe
out

2
int i
for i 0 10000
	out i
	wait 0 H mac("test_CScript_memory_thread")
	 1
	 if(i%100=0) SetProcessWorkingSetSize GetCurrentProcess -1 -1
