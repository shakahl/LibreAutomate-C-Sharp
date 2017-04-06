 /
function# [hwnd] [threadid]

 Returns 1 if the window belongs to a macro thread.
 If hwnd is 0, returns handle of the first found visible macro window, or 0 if there are no such windows. Gives priority to windows of current thread.
 If threadid is not 0, applies only to the specified thread. Use GetCurrentThreadId for current thread.


int i j n pid tid pidq tidq
ARRAY(int) ah

if(hwnd)
	tid=GetWindowThreadProcessId(hwnd &pid)
	if(threadid) ret tid=threadid
	tidq=GetWindowThreadProcessId(_hwndqm &pidq)
	if(pid!=pidq or tid=tidq) ret
else
	int istid=threadid
	if(!istid) threadid=GetCurrentThreadId ;;try current thread first
	if(GetThreadWindows(ah threadid))
		for(j 0 ah.len) if(!hid(ah[j])) ret ah[j]
	if(istid) ret

n=EnumQmThreads
ARRAY(QMTHREAD) a.create(n)
for i 0 EnumQmThreads(&a[0] n)
	threadid=a[i].threadid
	if(hwnd)
		if(tid=threadid) ret 1
	else
		if(GetThreadWindows(ah threadid))
			for(j 0 ah.len) if(!hid(ah[j])) ret ah[j]
