 /exe
AddTrayIcon
int h=win("Quick Macros" "QM_Editor")
int n
rep
	1
	if(!IsWindow(h)) break
	if(IsHungAppWindow(h))
		 out "hung"
		n+1
		if(n>5)
			 clo h
			 5
			 if(!IsWindow(h)) break
			ShutDownProcess h 2
			break

 BEGIN PROJECT
 main_function  QmKillIfHung
 exe_file  $desktop$\QmKillIfHung.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4C5DAAB0-F665-43F0-9F59-9F9C3F2200FD}
 END PROJECT
