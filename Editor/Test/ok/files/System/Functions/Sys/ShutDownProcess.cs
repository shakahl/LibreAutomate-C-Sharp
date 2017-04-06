 /
function# `hwndOrExename [flags] [exitCode] ;;flags: 1 process id, 2 try to close softly, 4 all, 8 messages, 16 current user session only

 Terminates a process (running program).
 Returns number of terminated processes.
 Error if fails.

 hwndOrExename - program name (without path and .exe), or window handle, or process id (flag 1).
 flags:
   1 - hwndOrExename is process id.
   2 - at first close process windows. Terminate process only if cannot close, eg if hung, of if the process is still alive after 5 seconds.
   4 - terminate all processes matching program name.
   8 - when waiting, process messages. Use this flag when calling this function from thread that has windows or COM events.
   16 (QM 2.2.0) - terminate only process(es) running in current user session (fast user switching).
 exitCode - exit code to be used by the process if terminated not softly.

 REMARKS
 Use this function to terminate a process, especially if it is hung or windowless (use clo otherwise). You can use IsHungAppWindow to check whether a window is hung.
 May leave invalid tab in the taskbar.

 EXAMPLES
 ShutDownProcess("NOTEPAD")
 ShutDownProcess(win("" "Notepad"))


int ph pid hwnd ok i wts(flags&16=0)

sel hwndOrExename.vt
	case VT_I4
	pid=hwndOrExename.lVal
	
	if(flags&1=0) ;;hwnd
		hwnd=pid
		if(!IsWindow(hwnd)) ret
		if(WinTest(hwnd "Ghost") and IsHungAppWindow(hwnd)) end "It is a ghost window. To find correct window, use win('''' ''class name'')." ;;process is EXPLORER on XP, but DWM on Vista+
		GetWindowThreadProcessId(hwnd &pid)
	
	case VT_BSTR
	_s=hwndOrExename
	flags|1
	
	if(flags&4) ;;all
		ARRAY(int) a
		ProcessNameToId _s &a !wts
		for(i 0 a.len) if(ShutDownProcess(a[i] flags~4 exitCode)) ok+1
		err+ end _error
		ret ok
	
	pid=ProcessNameToId(_s 0 !wts)
	case else end ERR_BADARG

if(!pid) ret

if(flags&2) ;;try WM_CLOSE
	POINT p.x=pid
	if(flags&8) opt waitmsg 1; wait 0 H mac("sub.Softly" "" &p)
	else sub.Softly &p
	if(p.y) ret 1 ;;closed softly

if(wts)
	ok=WTSTerminateProcess(0 pid exitCode)
	if(!ok and GetLastError!=ERROR_INVALID_PARAMETER) goto g1 ;;on some computers WTSTerminateProcess fails
else
	 g1
	ph=OpenProcess(PROCESS_TERMINATE 0 pid)
	if(ph) ok=TerminateProcess(ph exitCode); CloseHandle(ph)

if(!ok and GetLastError!=ERROR_INVALID_PARAMETER) end "" 16
ret ok


#sub Softly
function POINT*p

ARRAY(int) a a1 a2
int i n h

 get all top-level windows of that process, visible first
opt hidden 1
if(!win("" "" p.x 0 "" a)) ret

 move visible toolwindows after other visible windows
for i 0 a.len
	h=a[i]
	if(!IsWindowVisible(h)) break
	if(GetWinStyle(h 1)&WS_EX_TOOLWINDOW) a2[]=h; else a1[]=h
n=a1.len
for(i 0 n) a[i]=a1[i]
for(i 0 a2.len) a[i+n]=a2[i]
if(!n) n=1

 close all
spe 10
for i 0 a.len
	h=a[i]
	if(!IsWindow(h)) continue
	if(IsHungAppWindow(h)) break
	clo h; err continue
	if(IsWindow(h)) wait iif(i<n 1.0 0.2) -WC h; err

 if all closed, wait for process exit, else terminate now
for(i 0 a.len) if(IsWindow(a[i])) ret
__Handle ph=OpenProcess(SYNCHRONIZE 0 p.x)
if ph
	wait 5 H ph; err ret
	p.y=1
