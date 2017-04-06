int i
ARRAY(byte*) a.create(8)
for i 0 a.len
	a[i]=VirtualAlloc(0 1 MEM_RESERVE PAGE_READWRITE)
	out "0x%X" a[i]

for i 0 a.len
	VirtualFree(a[i] 0 MEM_RELEASE)
