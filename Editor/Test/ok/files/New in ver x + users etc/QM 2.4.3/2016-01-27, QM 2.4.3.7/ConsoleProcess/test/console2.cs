 /exe

 Console .exe for macro "test ConsoleProcess.ReadStdout".


ExeConsoleWrite _command
ExeConsoleWrite GetCurDir
ExeConsoleWrite "test error output" 1

int i
for i 1 6
	ExeConsoleWrite F"i={i}"
	0.5

ret 7

 BEGIN PROJECT
 exe_file  $my qm$\console2.exe
 flags  70
 END PROJECT
