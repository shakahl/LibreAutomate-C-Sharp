function !*ptrDest !*ptrSrc nBytes

 Writes to memory of process specified with Alloc. The memory may be other than allocated by Alloc.
 Error if fails.

 ptrDest - process memory address where to store the data.
 ptrSrc - local memory address.
 nBytes - number of bytes to write.


_i=WriteProcessMemory(m_hproc ptrDest ptrSrc nBytes 0)
err end _error
if(!_i) end "" 16
