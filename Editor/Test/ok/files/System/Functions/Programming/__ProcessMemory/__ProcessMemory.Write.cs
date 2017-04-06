function !*ptr nBytes [offset]

 Writes to the memory allocated by Alloc.
 Error if fails.

 ptr - local memory. It can be a string, or address of a variable, or other memory.
 nBytes - number of bytes.
 offset - offset in the allocated memory.


_i=WriteProcessMemory(m_hproc m_mem+offset ptr nBytes 0)
err end _error
if(!_i) end "" 16
