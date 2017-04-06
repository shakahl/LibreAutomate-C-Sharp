 /
function hwnd priority [flags]

 Sets priority class of a process (running program).
 On failure generates error.
 hwnd - handle of some window of that process.
 flags - 1 hwnd is process id.
 priority - one of x_PRIORITY_x constants:
   def NORMAL_PRIORITY_CLASS 0x00000020 ;;normal, default
   def IDLE_PRIORITY_CLASS 0x00000040 ;;lowest
   def HIGH_PRIORITY_CLASS 0x00000080
   def REALTIME_PRIORITY_CLASS 0x00000100 ;;highest, dangerous
   def BELOW_NORMAL_PRIORITY_CLASS 0x00004000 ;;*
   def ABOVE_NORMAL_PRIORITY_CLASS 0x00008000 ;;*
    * not supported on NT/98/Me

 EXAMPLES
 int hwnd=win("Notepad")
 SetProcessPriority hwnd IDLE_PRIORITY_CLASS
  Eat all CPU time. Press Pause to stop.
 rep() int i=0
  Try to work now in notepad...
 
 int pid=ProcessNameToId("notepad")
 if(!pid) end "the process is not running"
 SetProcessPriority pid BELOW_NORMAL_PRIORITY_CLASS 1


int pid ph ok
if(flags&1) pid=hwnd; else GetWindowThreadProcessId(hwnd &pid)
ph=OpenProcess(PROCESS_SET_INFORMATION 0 pid); if(!ph) goto g1
ok=SetPriorityClass(ph priority)
 g1
if(!ok) _s.dllerror
if(ph) CloseHandle ph
if(!ok) end _s
