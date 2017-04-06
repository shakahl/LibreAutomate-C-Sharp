 /
function! $var

 Gets environment variable var from registry and updates it in this process.

 var - environment variable name.

 REMARKS
 When a process is starting, in most cases it receives a copy of environment variables of its parent process.
 The first process (Windows Explorer) reads them from registry. It seems that processes started by the main Windows Explorer process also receive values from registry, but not sure is it always true, may need to log off/on.
 When somebody changes a system environment variable, variables in currently running processes are not updated. Their child processes also will receive old values.
 This function updates the specified variable in this process (QM).
 Then processes started by RunConsole2() also will receive the updated value. Processes started by run() probably will still receive old values.
 More info: http://support.microsoft.com/kb/104011 (How to propagate environment variables to the system).

 EXAMPLE
 out
 out GetEnvVar("test" _s)
 RunConsole2 "setx.exe test ''delete me'' /M" ;;set a system environment variable in registry for testing. /M is for all users, requires admin rights.
 ReloadEnvVar "test"
 out GetEnvVar("test" _s)
 RunConsole2 "$my qm$\function271.exe" ;;QM-created exe with this code: out GetEnvVar("test" _s)


if(!rget(_s var "Environment") and !rget(_s var "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" HKEY_LOCAL_MACHINE)) ret
ret SetEnvVar(var _s)
