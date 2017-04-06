 /exe

 Run this to test MakeExe_Console, ConsoleOut2 and ConsoleIn2.


ConsoleOut2 "Type something and press Enter"
str s
ConsoleIn2 s
ConsoleOut2 F"s=''{s}''"
1

 BEGIN PROJECT
 exe_file  $my qm$\QM-console.exe
 on_after  MakeExe_Console
 flags  6
 END PROJECT
