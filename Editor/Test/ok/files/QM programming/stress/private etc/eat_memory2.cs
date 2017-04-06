 /exe
function [MB]
AddTrayIcon
0.1
 key Wm
 1
 act "+QM_Editor"
 ret
out "memory begin"

if(!MB) MB=iif(_winnt=6 400 100)
_s.all(MB*1024*1024 2 32)

out "memory end"
 ret
rep
	1
	_s.set(32)

 BEGIN PROJECT
 main_function  eat_memory2
 exe_file  $my qm$\eat_memory2.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  635
 guid  {6B40B196-04C8-4951-94F1-666318B027B9}
 END PROJECT
