 /exe
zip "$desktop$\test\test.zip" "$desktop$\test\qm.exe"
zip- "$desktop$\test\test.zip" "$desktop$\test\unzip"

 BEGIN PROJECT
 main_function  test zip2
 exe_file  $my qm$\Macro1674.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {F155AC72-7DEE-4904-AB26-790D755AD0B7}
 END PROJECT
