 /Macro2115
function! hwnd

if(_winnt<6) ret

int pid; if(!GetWindowThreadProcessId(hwnd &pid)) ret
__Handle hp=OpenProcess(PROCESS_QUERY_X_INFORMATION 0 pid); if(!hp) end ERR_FAILED 16
dll- shcore #GetProcessDpiAwareness hprocess *value
int is
if(GetProcessDpiAwareness(hp &is)) ret
 out is
ret !is
