 /exe
 WshExec "WScript.Echo ''test''" _s; out _s
WshExec "" _s; out _s
 WshExec "macro:exe auto add - WshExec" _s; out _s

 BEGIN PROJECT
 main_function  exe auto add - WshExec
 exe_file  $my qm$\Macro2050.qmm
 flags  6
 guid  {DBCD56EB-950C-44E9-B9B9-4A0EEB4CCBC2}
 END PROJECT
#ret
WScript.Echo "test2"
