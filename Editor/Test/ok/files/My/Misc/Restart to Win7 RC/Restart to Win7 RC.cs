 /exe

sel list("Change date and reboot[]Restore date" "Info: Win7 RC (32 and 64 bit) expires 2010.03.01." "Reboot to Win7 RC")
	case 1
	Win7RcCDate "1/1/2010"
	shutdown 2
	
	case 2
	Win7RcCDate ""

 BEGIN PROJECT
 main_function  Restart to Win7 RC
 exe_file  $desktop$\Restart to Win7 RC.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5725825E-DE98-42CB-BC85-163BA591FC3C}
 END PROJECT
