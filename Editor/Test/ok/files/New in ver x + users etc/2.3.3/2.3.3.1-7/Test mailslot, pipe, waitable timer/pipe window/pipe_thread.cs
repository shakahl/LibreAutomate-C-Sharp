 \pipe_window

 int* m=share

 #ret

SECURITY_ATTRIBUTES* sap
SECURITY_ATTRIBUTES sa.nLength = sizeof(sa)
SECURITY_DESCRIPTOR sd
if(InitializeSecurityDescriptor(&sd 1) and SetSecurityDescriptorDacl(&sd 1 0 0)) sa.lpSecurityDescriptor=&sd; sap=&sa

__Handle ev=CreateEvent(0 0 0 "Global\QM test event")
__Handle wt=CreateWaitableTimer(sap 0 "Global\QM test timer")
 out wt
__Handle ms=CreateMailslot("\\.\mailslot\qm test mailslot" 0 0 sap)
 out ms
int+ g_ms=ms

rep
	 wait(0 H ev)
	wait(0 H wt)
	 Sleep 0
	 OutputDebugString "received"
	int+ g_reset=0
	 Q &qqqq; outq
	 word p1=perf; out F"tim: {p1/1000.0%%.3f}"
	
	read_mailslot ms
