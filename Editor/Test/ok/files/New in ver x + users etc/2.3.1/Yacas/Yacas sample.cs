/exe
#compile yacas

yacas_eval "5*3^2"
if(yacas_error) out yacas_error
else
	if(!empty(yacas_output)) out "Output: %s" yacas_output
	out "Result: %s" yacas_result

 BEGIN PROJECT
 main_function  Yacas sample
 exe_file  $my qm$\Yacas sample.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {0AAB1735-1B27-4CF7-A2B8-1F46A73B7E6E}
 END PROJECT
