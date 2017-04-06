 /
function# $cl [flags] ;;flags: 1 wait until exits, 2 return handle, 4 inherit uiAccess

 Calls CreateProcess.
 By default returns pid. If flag 2 - handle. If flag 1 - exit code.
 Error if fails.

 cl - program path, optionally followed by command line arguments.
   Program path can be enclosed in quotes. Should be enclosed if contains spaces.
 flags:
   1 - wait until exits and return the exit code.

 NOTES
 CreateProcess fails if would show UAC consent. Use run() instead.

 EXAMPLE
 int ec=CreateProcessSimple("''$my qm$\this_is_my_exe.exe'' /a ''b''" 1)
 out ec


opt noerrorshere 1

sel cl 2
	case ["$*","%*"] cl=_s.expandpath(cl)
	case ["''$*","''%*"] _s.expandpath(cl+1); _s-"''"; cl=_s

STARTUPINFOW si.cb=sizeof(si)
PROCESS_INFORMATION pi
int R ok
if flags&4
	__Handle hToken
	OpenProcessToken(GetCurrentProcess() TOKEN_QUERY|TOKEN_DUPLICATE|TOKEN_ASSIGN_PRIMARY &hToken)
	ok=CreateProcessAsUserW(hToken 0 @cl 0 0 0 0 0 0 &si &pi)
else
	ok=CreateProcessW(0 @cl 0 0 0 0 0 0 &si &pi)
if(!ok) end _s.dllerror
CloseHandle pi.hThread
R=pi.dwProcessId

if flags&1
	opt waitmsg -1
	wait 0 H pi.hProcess
	GetExitCodeProcess pi.hProcess &R

if(flags&2) R=pi.hProcess; else CloseHandle pi.hProcess

ret R
