function'int* int*m

int+ g_codeHeap
if(!g_codeHeap) g_codeHeap=HeapCreate(HEAP_CREATE_ENABLE_EXECUTE 0 0);;TODO: HeapDestroy

int size(m[0]) nFunc(m[1]) i
int* R=HeapAlloc(g_codeHeap 0 size)
memcpy R m size

for(i 2 nFunc+2) R[i]+R ;;offset -> adress

ret R+8
