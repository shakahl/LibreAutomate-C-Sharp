out
 1
 rep(1000000) int i=5

int w=win("" "Notepad")
int pid
int tid=GetWindowThreadProcessId(w &pid)
 __Handle ht=OpenThread(THREAD_GET_CONTEXT|THREAD_QUERY_INFORMATION|THREAD_SUSPEND_RESUME 0 tid)
 out ht
 int ht=GetCurrentThread
tid=GetCurrentThreadId
pid=GetCurrentProcessId

out GetThreadContextSwitches(tid pid)

 type THREAD_TIMES_INFORMATION %CreationTime %ExitTime %KernelTime %UserTime
 THREAD_TIMES_INFORMATION t
 if(NtQueryInformationThread(ht 1 &t sizeof(t) &_i)) ret
  out _i
 out t.UserTime
 out t.KernelTime
 out t.CreationTime/10000
 out t.ExitTime

 long t
 if(NtQueryInformationThread(ht 11 &t sizeof(t) &_i)) ret
  if(NtQueryInformationThread(ht 11 &t sizeof(t) &_i)) ret
 out _i
 out t

 type SYSTEM_PERFORMANCE_INFORMATION %IdleProcessTime %IoReadTransferCount %IoWriteTransferCount %IoOtherTransferCount IoReadOperationCount IoWriteOperationCount IoOtherOperationCount AvailablePages CommittedPages CommitLimit PeakCommitment PageFaultCount CopyOnWriteCount TransitionCount CacheTransitionCount DemandZeroCount PageReadCount PageReadIoCount CacheReadCount CacheIoCount DirtyPagesWriteCount DirtyWriteIoCount MappedPagesWriteCount MappedWriteIoCount PagedPoolPages NonPagedPoolPages PagedPoolAllocs PagedPoolFrees NonPagedPoolAllocs NonPagedPoolFrees FreeSystemPtes ResidentSystemCodePage TotalSystemDriverPages TotalSystemCodePages NonPagedPoolLookasideHits PagedPoolLookasideHits Spare3Count ResidentSystemCachePage ResidentPagedPoolPage ResidentSystemDriverPage CcFastReadNoWait CcFastReadWait CcFastReadResourceMiss CcFastReadNotPossible CcFastMdlReadNoWait CcFastMdlReadWait CcFastMdlReadResourceMiss CcFastMdlReadNotPossible CcMapDataNoWait CcMapDataWait CcMapDataNoWaitMiss CcMapDataWaitMiss CcPinMappedDataCount CcPinReadNoWait CcPinReadWait CcPinReadNoWaitMiss CcPinReadWaitMiss CcCopyReadNoWait CcCopyReadWait CcCopyReadNoWaitMiss CcCopyReadWaitMiss CcMdlReadNoWait CcMdlReadWait CcMdlReadNoWaitMiss CcMdlReadWaitMiss CcReadAheadIos CcLazyWriteIos CcLazyWritePages CcDataFlushes CcDataPages ContextSwitches FirstLevelTbFills SecondLevelTbFills SystemCalls
 SYSTEM_PERFORMANCE_INFORMATION t
 rep 100
	 if(NtQuerySystemInformation(SystemPerformanceInformation &t sizeof(t) &_i)) ret
	 out t.ContextSwitches

 type SYSTEM_PROCESS_INFORMATION NextEntryOffset NumberOfThreads %SpareLi1 %SpareLi2 %SpareLi3 %CreateTime %UserTime %KernelTime UNICODE_STRING'ImageName BasePriority UniqueProcessId InheritedFromUniqueProcessId HandleCount SessionId PageDirectoryBase PeakVirtualSize VirtualSize PageFaultCount PeakWorkingSetSize WorkingSetSize QuotaPeakPagedPoolUsage QuotaPagedPoolUsage QuotaPeakNonPagedPoolUsage QuotaNonPagedPoolUsage PagefileUsage PeakPagefileUsage PrivatePageCount %ReadOperationCount %WriteOperationCount %OtherOperationCount %ReadTransferCount %WriteTransferCount %OtherTransferCount
 SYSTEM_THREAD_INFORMATION TH[1]

 type THREAD_CYCLE_TIME_INFORMATION %AccumulatedCycles %CurrentCycleCount
 THREAD_CYCLE_TIME_INFORMATION t
 if(NtQueryInformationThread(ht 23 &t sizeof(t) &_i)) ret
 out t.AccumulatedCycles
 out t.CurrentCycleCount

 if(SuspendThread(ht)=-1) ret
 CONTEXT c
 if(GetThreadContext(ht &c))
	 out c.Eip
	 out c.Esi
 ResumeThread ht
