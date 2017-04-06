ShellDialog 3
int h=wait(10 WA "Shut Down Windows"); err ret
CB_SelectItem id(2001 h) 0
but 1 h

 BEGIN PROJECT
 main_function  VistaSwitchUser
 exe_file  $desktop$\VistaSwitchUser.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {C5949ADD-46B8-4040-BCFF-9B4E72CAE531}
 END PROJECT
