 /exe
#exe addfile "$temp$\EZ_AddFiles.zip"



 BEGIN PROJECT
 main_function  exe_zip
 exe_file  $my qm$\exe_zip.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  EZ_AddFiles
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4D4BC96D-F74C-4018-96AD-FBEB5A20D817}
 END PROJECT
