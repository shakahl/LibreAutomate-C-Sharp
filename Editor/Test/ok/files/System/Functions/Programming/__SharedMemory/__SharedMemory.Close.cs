
 Closes this shared memory block.
 Called implicitly by destructor.
 The memory is deallocated when all processes (that used it) closed it or ended.


if(mem) UnmapViewOfFile(mem); mem=0
if(m_hmapfile) CloseHandle(m_hmapfile); m_hmapfile=0
