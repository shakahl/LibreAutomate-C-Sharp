 /exe

 Console .exe for macro "test ConsoleProcess.WriteStdin".


str s
ExeConsoleWrite "waiting for input 1"
ExeConsoleRead s
mes s "Console"
ExeConsoleWrite "waiting for input 2"
ExeConsoleRead s
mes s "Console"

 BEGIN PROJECT
 exe_file  $my qm$\console3.exe
 flags  70
 END PROJECT
