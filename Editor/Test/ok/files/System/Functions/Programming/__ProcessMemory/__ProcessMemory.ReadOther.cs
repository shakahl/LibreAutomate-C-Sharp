function !*ptrDest !*ptrSrc nBytes

 Reads from memory of process specified with Alloc. The memory may be other than allocated by Alloc.
 Error if fails.

 ptrDest - local memory address where to store the data.
 ptrSrc - process memory address.
 nBytes - number of bytes to read.


_i=ReadProcessMemory(m_hproc ptrSrc ptrDest nBytes 0)
err end _error
if(!_i) end "" 16
