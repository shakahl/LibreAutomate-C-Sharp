
 Frees memory allocated by Alloc.
 Called implicitly when destroying the variable.


if(m_mem) VirtualFreeEx m_hproc m_mem 0 MEM_RELEASE; m_mem=0
if(m_hproc) CloseHandle m_hproc; m_hproc=0
