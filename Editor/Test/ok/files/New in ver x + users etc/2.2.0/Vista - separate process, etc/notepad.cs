 /exe 2
out GetProcessUacInfo(0 1)
act "Notepad"
key a
1
key B
men 33 win("Notepad" "Notepad") ;;Font...
0.5
clo "Font"; err

 BEGIN PROJECT
 main_function  notepad
 exe_file  $common documents$\my qm\Macro473.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  38
 end_hotkey  0
 guid  {D7546D56-F1E6-4DED-997E-AA21E3A1B34C}
 END PROJECT
