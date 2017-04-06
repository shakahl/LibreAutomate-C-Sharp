/exe
out
int w=win("Test Pipe" "QM_TP_Class")
int r ok f
 w=_hwndqm

 act w;; 0.5
type MAILSLOTMSG !nul !typ @message { nArgs []param } { a[1] []!data[1] }

 __Handle mutex=CreateMutex(0 0 "mutex_test_pipe")
__Handle ev=CreateEvent(0 0 0 "Global\QM test event")
__Handle wt=CreateWaitableTimer(0 0 "Global\QM test timer")
 out wt
Q &q
 __Handle ms=CreateFile("\\.\mailslot\qm test mailslot" GENERIC_WRITE FILE_SHARE_READ 0 OPEN_EXISTING 0 0)
__Handle ms=CreateFile("\\.\mailslot\1\__QM_main" GENERIC_WRITE FILE_SHARE_READ|FILE_SHARE_WRITE 0 OPEN_EXISTING 0 0)
Q &qq; outq
 out ms

 str s="test data[192]" 
str s="test data ąčę" 
f=SMTO_ABORTIFHUNG
f|SMTO_BLOCK
rep 1
	int+ g_reset=1
	Q &q
	 WaitForSingleObject(mutex 1000)
	 Q &qq
	
	 WriteFile(ms s 0 &_i 0)
	rep 1
		 WriteFile(ms s s.len &_i 0)
		
		MAILSLOTMSG m.typ=1
		m.message=_unicode
		 m.message=0
		 m.message=CP_UTF8
		_s.format("%.8m%s" &m s)
		 m.message=1200; _s.format("%.8m%.26m" &m s.unicode)
		WriteFile(ms _s _s.len &_i 0)
	Q &qq
	 out _i
	 out GetFileSize(ms &_i)
	 out GetFilePointer(ms &_i)
	 int mSize mCount
	 out GetMailslotInfo(ms 0 &mSize &mCount 0)
	 out "%i %i" mSize mCount	
	
	 ok=SendMessageTimeoutW(w WM_APP+1589 0 0 f 1000 &r)
	 ok=SendMessageW(w WM_APP+1589 0 0)
	
	 OutputDebugString "send 1"
	 word p1=perf
	 ok=PostMessageW(w WM_APP+1589 0 0)
	 ok=SendNotifyMessageW(w WM_APP+1589 0 0)
	 SetEvent ev
	long t=-30000000
	 t=0
	t=-1
	rep 1
		 SetWaitableTimer(wt +&t 0 0 0 0)
		PostMessage _hwndqm WM_USER 0 0
		 1
	 SetTimer w 2 0 0
	
	 COPYDATASTRUCT c.cbData=4; c.lpData=&_i
	 ok=SendMessageTimeoutW(w WM_COPYDATA 0 &c f 1000 &r)
	 ok=SendNotifyMessageW(w WM_COPYDATA 0 &c)
	 ok=SendMessageW(w WM_COPYDATA 0 &c)
	
	if(!g_reset) out "received first"
	 OutputDebugString "send 2"
	Q &qqq
	outq
	 word p2=perf; out F"{p1/1000.0%%.3f} {p2/1000.0%%.3f}"
	
	 Q &qqq
	 ReleaseMutex(mutex)
	 Q &qqqq
	 outq
 out "%i %i" ok r

 BEGIN PROJECT
 main_function  send message and mutex
 exe_file  $my qm$\send message and mutex.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {9BD21E7B-678A-4992-BDB4-E8342EAB1AA8}
 END PROJECT
