function# hwnd access [flags] ;;1 hwnd is process id

 Calls OpenProcess and initializes this variable.
 Returns process handle (this variable), or 0 if fails.

 hwnd - window hanle.
 access - <help>OpenProcess</help> dwDesiredAccess parameter.
   If PROCESS_QUERY_LIMITED_INFORMATION used on Windows XP, replaces it with PROCESS_QUERY_INFORMATION.


Close
if(flags&1) _i=hwnd; if(!_i) ret
else if(!GetWindowThreadProcessId(hwnd &_i)) ret

if(access&PROCESS_QUERY_LIMITED_INFORMATION and _winver<0x600) access~PROCESS_QUERY_LIMITED_INFORMATION; access|PROCESS_QUERY_INFORMATION

handle=OpenProcess(access 0 _i)
ret handle
