 /exe
 ConsoleOut2 "Type something" 2
 ConsoleOut2 " and press Enter"
 ConsoleOut2 "Test[]error" 1

mes 1
ConsoleOut2 "Type ąčę"
str s
ConsoleIn2 s
ConsoleOut2 F"s=''{s}''"
2

 BEGIN PROJECT
 main_function  QM-console2
 exe_file  $my qm$\QM-console2.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 on_after  MakeExe_Console
 flags  6
 guid  {30776211-464C-4391-B2B1-60E1C2D5CC0D}
 END PROJECT

#ret
"Q:\My QM\QM-console2.exe"
