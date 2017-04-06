 /exe

 out "string"
 spe
 #exe addtextof "Macro514"
 #exe addfile "$qm$\MailBee.dll" 10
 #exe addfile "$qm$\qmzip.dll" 10
 #exe addfile "$qm$\unicows.dll" 10
 Dialog34 0 0 0 0
SearchInFiles

 BEGIN PROJECT
 main_function  f
 exe_file  $my qm$\exe.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  38
 end_hotkey  0
 guid  {64B5E793-74D4-4198-983A-65F34AE0EF11}
 END PROJECT

 qmzip.dll;pdh.dll;winmm.dll;psapi.dll;wtsapi32.dll;oleacc.dll;ws2_32.dll
OnScreenDisplay