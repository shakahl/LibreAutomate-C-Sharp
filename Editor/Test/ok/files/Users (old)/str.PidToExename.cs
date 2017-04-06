 /
function$ pid [flags] ;;flags: 1 full

 Gets program name from process id.

 EXAMPLE
 int pid
 if(!GetWindowThreadProcessId(win("" "Shell_TrayWnd") &pid)) ret
 out _s.PidToExename(pid 1)


#if _winnt

int hp hm i
this.len=0
hp=OpenProcess(PROCESS_QUERY_INFORMATION|PROCESS_VM_READ 0 pid); if(!hp) ret
if(EnumProcessModules(hp &hm 4 &i))
	this.all(MAX_PATH)
	if(flags&1) this.fix(GetModuleFileNameEx(hp hm this MAX_PATH))
	else this.fix(GetModuleBaseName(hp hm this MAX_PATH)); if(this.endi(".exe")) this.fix(this.len-4)
CloseHandle(hp)
ret this

#endif
