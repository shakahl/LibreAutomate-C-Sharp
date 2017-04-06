/exe
 out list("One[]Two" "text" "cap" 100 200 10 2)
out list("8 One[]&5 Two[]2 Four" "text" "" 0 -100 5 50 1|2)
list "" "text"

 BEGIN PROJECT
 main_function  Macro567
 exe_file  $my qm$\Macro567.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {231D7B65-BB4E-4567-B4CE-7A73EF9530A0}
 END PROJECT
