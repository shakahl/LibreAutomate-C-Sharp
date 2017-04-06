/exe
out
dll "qm.exe" #GetThreadId2 hThread
type SYSTEM_HANDLE ProcessId !ObjectTypeNumber !Flags @Handle !*Object GrantedAccess
str s.all(100000*sizeof(SYSTEM_HANDLE) 2)
int* nh=s
SYSTEM_HANDLE* ah=s+4

Q &q
out NtQuerySystemInformation(16 s s.len &_i)
Q &qq
out *nh
out _i

int pid=GetCurrentProcessId
int ct=GetCurrentThreadId
out ct
int i
for i 0 *nh
	SYSTEM_HANDLE& r=ah[i]
	if(r.ProcessId==pid)
		out "%i %i %i" r.ObjectTypeNumber r.Handle r.Object
		 if(!NtQueryObject(r.Handle 2
		int tid=GetThreadId2(r.Handle)
		 if(tid) out tid
		if(tid=ct) mes r.ObjectTypeNumber
		 spe; continue
		 if r.ObjectTypeNumber=8 ;;thread
			 int tid=GetThreadId2(r.Handle)
			  out "%i %i" r.Handle tid
			 if tid and tid!=ct
				 out SuspendThread(r.Handle)
				 ResumeThread(r.Handle)
		 int ht=
	 else out "-"
Q &qqq
outq

 BEGIN PROJECT
 main_function  Macro1795
 exe_file  $my qm$\Macro1795.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {E6D20B17-4AC8-4FAF-B1A7-BDC34A12E491}
 END PROJECT
