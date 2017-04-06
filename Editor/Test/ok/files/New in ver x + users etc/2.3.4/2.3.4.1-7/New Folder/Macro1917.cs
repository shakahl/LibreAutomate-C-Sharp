 /exe

 out LoadLibrary(_s.expandpath("$temp$\qmzip.dll"))
out LoadLibrary(_s.expandpath("$desktop$\test.dll"))
mes 1

 mes 1 "" "t"
 MessageBox 0 "" "" MB_TOPMOST|MB_SETFOREGROUND

 BEGIN PROJECT
 main_function  Macro1917
 exe_file  $my qm$\Macro1917.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {F872EEB1-17AF-4486-8C54-A3B158A6212D}
 END PROJECT
