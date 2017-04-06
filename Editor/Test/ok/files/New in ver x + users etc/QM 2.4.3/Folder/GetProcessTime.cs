 /
function `exeNameOrWindowHandle [DateTime&tStart] [long&tCPU] [flags] ;;flags: 1 tStart UTC

 Gets process (running program) start and/or CPU time.

 exeNameOrWindowHandle - program name, path or window handle.
 tStart - variable that receives the process start time.
 tCPU - variable that receives the amount of time that the process has executed in kernel mode and user mode (sum).

 EXAMPLES
 DateTime tStart
 GetProcessTime "notepad" tStart
 out tStart.ToStr(4)
 
 long tCPU
 int w=win("" "Shell_TrayWnd")
 GetProcessTime w 0 tCPU
 out DateTime.TimeSpanToStr(tCPU 2|4)


int pid
sel exeNameOrWindowHandle.vt
	case VT_BSTR pid=ProcessNameToId(exeNameOrWindowHandle); if(!pid) end "process not found"
	case VT_I4 GetWindowThreadProcessId(exeNameOrWindowHandle &pid); if(!pid) end "window not found"
	case else end ERR_BADARG

__Handle hp=OpenProcess(PROCESS_QUERY_X_INFORMATION 0 pid); if(!hp) end ERR_FAILED 16
DateTime t1 t2 t3 t4
if(!GetProcessTimes(hp +&t1 +&t2 +&t3 +&t4)) end ERR_FAILED 16
if(&tStart) tStart=t1; if(flags&1=0) tStart.UtcToLocal
if(&tCPU) tCPU=t3+t4
