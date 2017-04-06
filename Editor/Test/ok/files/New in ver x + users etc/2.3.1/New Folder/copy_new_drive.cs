 /exe
SetLastError 0
__Handle m=CreateMutex(0 1 "mutex_copy_new_drive")
if(!m) mes- "Error"
if(GetLastError=ERROR_ALREADY_EXISTS)
	if(mes("copy_new_drive already running. Do you want to end it?" "copy_new_drive" "YN?")='Y')
		ShutDownProcess "copy_new_drive" 16 ;;this should end the older process, but need more testing
	mes 1
	ret

NewDriveWatcher 0 0 0 0

 BEGIN PROJECT
 main_function  copy_new_drive
 exe_file  $desktop$\copy_new_drive.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4C0516F5-0C9A-4BCE-9212-2F5CE04F4CFD}
 END PROJECT
