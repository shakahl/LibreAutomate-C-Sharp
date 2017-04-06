int n=1024*1024*8
byte* m=VirtualAlloc(0 n MEM_RESERVE PAGE_READWRITE)
out m

int i
for i 0 5
	2
	out VirtualAlloc(m+(i*1024*1024) 1024*1024 MEM_COMMIT PAGE_READWRITE)
5
out VirtualFree(m 0 MEM_RELEASE)
