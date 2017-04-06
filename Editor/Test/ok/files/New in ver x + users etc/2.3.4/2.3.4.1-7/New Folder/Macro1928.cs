 /exe
out 1

 lpstr s=GetCommandLine
 out s
 out PathGetArgs(s)
 out _s.fix(GetModuleFileName(GetModuleHandle(0) _s.all(300) 300))
 mes 1

 BEGIN PROJECT
 main_function  Macro1928
 exe_file  $my qm$\Macro1928.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {AB763203-1297-4E0F-9C3F-CBEDDDFA6242}
 END PROJECT
