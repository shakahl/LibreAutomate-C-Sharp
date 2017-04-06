sel list("Start[]Stop[][]Install[]Uninstall[]Is installed[]Is running[]Wait 10s" "QM Service")
	case 1 out ControlQmService(3)
	case 2 out ControlQmService(4)
	case 4 out ControlQmService(1)
	case 5 out ControlQmService(2)
	case 6 out ControlQmService(5)
	case 7 out ControlQmService(6)
	case 8 out ControlQmService(10000)

 BEGIN PROJECT
 main_function  QmService
 exe_file  $desktop$\QmService.exe
 icon  $qm$\macro.ico
 manifest  $my qm$\QmService.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {0851B405-86F8-459F-A6AD-FB73FB7063B7}
 END PROJECT
