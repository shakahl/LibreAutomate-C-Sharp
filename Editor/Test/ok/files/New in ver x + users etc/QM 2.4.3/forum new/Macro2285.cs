 /exe
RedirectQmOutput &RedirectQmOutputToLogFile
ExeOutputWindow 
 out ;;delete the log file
out "test" ;;write to the log file
if mes("Open log file?" "" "YN")
	run "notepad.exe" F"''{_logfile}''"

 BEGIN PROJECT
 main_function  Macro2285
 exe_file  $my qm$\Macro2285.qmm
 flags  6
 guid  {6E18D134-E9E7-499B-9501-95FCC77A3CF6}
 END PROJECT
