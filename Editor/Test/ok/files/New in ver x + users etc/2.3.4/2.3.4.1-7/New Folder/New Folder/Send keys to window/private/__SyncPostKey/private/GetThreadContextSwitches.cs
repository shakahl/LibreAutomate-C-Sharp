 /
function# tid pid

type CLIENT_ID UniqueProcess UniqueThread
type VM_COUNTERS PeakVirtualSize VirtualSize PageFaultCount PeakWorkingSetSize WorkingSetSize QuotaPeakPagedPoolUsage QuotaPagedPoolUsage QuotaPeakNonPagedPoolUsage QuotaNonPagedPoolUsage PagefileUsage PeakPagefileUsage
type IO_COUNTERS %ReadOperationCount %WriteOperationCount %OtherOperationCount %ReadTransferCount %WriteTransferCount %OtherTransferCount
type SYSTEM_THREAD_INFORMATION %KernelTime %UserTime %CreateTime WaitTime StartAddress CLIENT_ID'ClientId Priority BasePriority ContextSwitchCount State WaitReason
type SYSTEM_PROCESS_INFORMATION NextEntryOffset NumberOfThreads %Reserved[3] %CreateTime %UserTime %KernelTime UNICODE_STRING'ImageName BasePriority ProcessId InheritedFromProcessId HandleCount Reserved2[2] PrivatePageCount VM_COUNTERS'VirtualMemoryCounters IO_COUNTERS'IoCounters SYSTEM_THREAD_INFORMATION'Threads[1]

str s.all(200000)
if(NtQuerySystemInformation(SystemProcessInformation s s.nc &_i)) ;;note: calling without first allocating would give required buffer size, but whole code would be 2 times slower. Memory allocation is fast; can allocate much more than need, anyway will use physical memory only as much as need.
	if(!_i) ret
	s.all(_i+20000)
	if(NtQuerySystemInformation(SystemProcessInformation s s.nc &_i)) ret

SYSTEM_PROCESS_INFORMATION* a=s
rep
	if(a.ProcessId=pid) break
	 out a.ProcessId
	 out _s.ansi(a.ImageName.Buffer -1 a.ImageName.Length)
	if(!a.NextEntryOffset) ret
	a+a.NextEntryOffset

int i
for i 0 a.NumberOfThreads
	if(a.Threads[i].ClientId.UniqueThread=tid) ret a.Threads[i].ContextSwitchCount

 speed: normally < 1 ms
