function!* $name size [flags]

 Creates shared memory block.
 Returns pointer to the memory in this process. It is the member variable mem. Returns 0 if failed.

 name - some unique string that identifies this memory block. Other processes can open the memory using this name.
 size - number of bytes to allocate.
 flags:
   1 - low integrity level processes can open.


Close
m_hmapfile = CreateFileMappingW(-1 iif(flags&1 GetSecurityAttributes 0) PAGE_READWRITE 0 size @name)
if(!m_hmapfile) ret
mem = MapViewOfFile(m_hmapfile FILE_MAP_ALL_ACCESS 0 0 0)
ret mem
