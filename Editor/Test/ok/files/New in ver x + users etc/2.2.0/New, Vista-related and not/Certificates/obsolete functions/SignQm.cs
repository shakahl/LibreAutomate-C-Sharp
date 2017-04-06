#if EXE

int hqm=win("" "QM_Editor")
if(hqm)
	men 101 hqm ;;Exit Program
	wait 30 WP hqm
	2

if(!SignCode("qm.exe" 1)) ret
run "$qm$\qm.exe" "v"

 BEGIN PROJECT
 main_function  SignQm
 exe_file  $qm$\SignQm.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {79BFDA73-0C06-48F4-9F68-ED4AC206F7D7}
 END PROJECT
