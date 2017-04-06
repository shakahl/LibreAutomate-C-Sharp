function!* hWnd nBytes [flags] ;;flags: 1 hWnd is process id

 Allocates memory in context of other process.
 Returns allocated memory address in that process.
 Error if fails.

 hWnd - a window that belongs to the process. If flag 1 used - process id.
 nBytes - number of bytes to allocate.
   The function may allocate more memory, because it allocates integer number of memory pages.
   Page size is 4 KB.
   For example, if nBytes is 100, allocates 4 KB; if nBytes is 5 KB, allocates 8 KB.
   If 0, does not allocate memory. Then you can use only WriteOther, ReadOther and hprocess.

 REMARKS
 Fails if the process belongs to another user.
 On Vista/7/8/10, fails if QM is running as User (or uiAccess, if used flag 1) and the process has higher integrity level.

 QM 2.3.3. The memory now can contain code that can be executed. Previously would fail to execute if DEP enabled.


Free
int pid
if(flags&1) pid=hWnd
else if(!GetWindowThreadProcessId(hWnd &pid)) end "" 16

m_hproc=OpenProcess(PROCESS_VM_OPERATION|PROCESS_VM_READ|PROCESS_VM_WRITE|PROCESS_DUP_HANDLE|SYNCHRONIZE|PROCESS_CREATE_THREAD|PROCESS_QUERY_INFORMATION 0 pid)
if(!m_hproc)
	int e=GetLastError
	if(_winnt>=6 and flags&1=0) m_hproc=GetProcessHandleFromHwnd(hWnd) ;;qm must be uiAccess
	if(!m_hproc) end "" 16 e

if(nBytes)
	m_mem=VirtualAllocEx(m_hproc 0 nBytes MEM_RESERVE|MEM_COMMIT PAGE_EXECUTE_READWRITE)
	if(!m_mem) _s.dllerror; Free; end _s

ret m_mem

 tested: in 64-bit processes allocates only in the 32-bit address space (max 2 GB). Fails if it is not free.
