 /
function# pid str&exename [flags] ;;flags: 1 full

 Gets program name from process id.

 EXAMPLE
 int pid; str s
 if(!GetWindowThreadProcessId(win("" "Shell_TrayWnd") &pid)) ret
 if(!PidToExename(pid s 1)) ret
 out s


#if _winnt

int hp hm i
exename.len=0
hp=OpenProcess(PROCESS_QUERY_INFORMATION|PROCESS_VM_READ 0 pid); if(!hp) ret
if(EnumProcessModules(hp &hm 4 &i))
	exename.all(MAX_PATH)
	if(flags&1) exename.fix(GetModuleFileNameEx(hp hm exename MAX_PATH))
	else exename.fix(GetModuleBaseName(hp hm exename MAX_PATH)); if(exename.endi(".exe")) exename.fix(exename.len-4)
CloseHandle(hp)
ret exename.len

#endif
