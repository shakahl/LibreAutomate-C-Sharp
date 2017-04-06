
Services.clsServices ss._create
VARIANT v="quickmacros2"
Services.clsService s=ss.Item(v)
sel _command
	case "/start"
	s.StartService
	case "/stop"
	s.StopService

 BEGIN PROJECT
 main_function  start_stop_qm_service
 exe_file  $qm$\app_plus\start_stop_qm_service.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {7A8F646E-D0D6-4185-BFCC-8937EC7898AB}
 END PROJECT
