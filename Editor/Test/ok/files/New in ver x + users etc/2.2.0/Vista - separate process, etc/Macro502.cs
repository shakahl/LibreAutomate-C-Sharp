/exe 0
 run "notepad.exe"
 run "notepad.exe" "" "" "" 0x10000
run "notepad.exe" "" "" "" 0x10000|0x400
 wait 0 H run("notepad.exe")
 wait 0 H run("notepad.exe" "" "" "" 0x10000)
 2

 BEGIN PROJECT
 main_function  Macro502
 exe_file  $my qm$\Macro502.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {2BDBD79C-A907-4068-9668-CAD22CCBE9DC}
 END PROJECT
