function !*ptr nBytes [offset]

 Reads from the memory allocated by Alloc.
 Error if fails.

 ptr - local memory address where to store the data. Must be at least nBytes of size.
 nBytes - number of bytes to read.
 offset - offset in the allocated memory.


_i=ReadProcessMemory(m_hproc m_mem+offset ptr nBytes 0)
err end _error
if(!_i) end "" 16
