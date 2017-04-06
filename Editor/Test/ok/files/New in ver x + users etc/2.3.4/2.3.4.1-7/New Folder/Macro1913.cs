 /exe

 #exe addfile "$qm$\qmzip.dll" 12 252
 #exe addfile "$qm$\qmzip.dll" 12

 zip "$desktop$\test.zip" "q:\app\*.txt"
zip "$desktop$\test.zip" "q:\app\qmcl.exe"

 BEGIN PROJECT
 main_function  Macro1913
 exe_file  $my qm$\Macro1913.exe
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
 guid  {8AC4063F-A024-4F64-83A7-F2D4787699D6}
 END PROJECT
