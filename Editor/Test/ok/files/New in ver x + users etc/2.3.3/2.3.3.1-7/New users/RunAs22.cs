 /
function $prog $user [$domain] [$password] [int&hprocess] [flags] ;;flags: 1 hidden

 Runs program as specified user.
 Error if fails.

 prog - program name or full path, optionally followed by command line parameters. If program path contains spaces, enclose into "". Examples: "prog.exe", "prog.exe /params", "c:\progra~1\prog\prog.exe /params", "''c:\program files\prog\prog.exe'' /params". Must be executable. Documents and shortcuts are not supported.
 user - user name.
 domain - domain or computer name. Can be omitted or "" for local computer.
 password - password. If omitted or "", asks at run time.
 hprocess - if used, receives process handle.

 EXAMPLES
 RunAs22 "notepad.exe" "Admin" ;;will ask for password
 
 __Handle hproc
 RunAs22 "c:\test.bat" "User" "" "ttr09knn" hproc 1
 wait 0 H hproc


str s ss
if(&hprocess) hprocess=0
if(!len(domain)) domain="." ;;local computer
if(!len(password)) if(inp(s "" "Password" "*")) password=s; else ret
BSTR s1(user) s2(domain) s3(password) s4(ss.expandpath(prog))

STARTUPINFOW si; si.cb=sizeof(si)
if(flags&1) si.dwFlags|STARTF_USESHOWWINDOW
PROCESS_INFORMATION pi
if(CreateProcessWithLogonW(+s1 +s2 +s3 LOGON_WITH_PROFILE 0 +s4 CREATE_DEFAULT_ERROR_MODE 0 0 &si &pi))
	if(pi.hThread != -1) CloseHandle(pi.hThread)
	if(pi.hProcess != -1) if(&hprocess) hprocess=pi.hProcess; else CloseHandle(pi.hProcess)
else end s.dllerror
