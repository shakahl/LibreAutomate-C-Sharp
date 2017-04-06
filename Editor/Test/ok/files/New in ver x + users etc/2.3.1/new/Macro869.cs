 /exe
int h=LoadLibrary("q:\app\qmhook32.dll")
OutputDebugString(_s.format("%i" GetCurrentThreadId))
 mac "Function81"
mes 1
0.1
 OutputDebugString(_s.format("%i" GetModuleHandle("qmhook32")))
FreeLibrary(h)


 BEGIN PROJECT
 main_function  Macro869
 exe_file  $my qm$\dll test.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D7BC5D94-A8C9-4FFC-B087-11500F2A33D0}
 END PROJECT
