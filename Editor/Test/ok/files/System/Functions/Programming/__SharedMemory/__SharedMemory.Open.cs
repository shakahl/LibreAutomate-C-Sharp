function!* $name

 Opens shared memory block.
 Returns pointer to the memory in this process. It is the member variable mem.

 name - name of the memory block.


Close
m_hmapfile = OpenFileMappingW(FILE_MAP_ALL_ACCESS 0 @name)
if(!m_hmapfile) ret
mem = MapViewOfFile(m_hmapfile FILE_MAP_ALL_ACCESS 0 0 0)
ret mem
