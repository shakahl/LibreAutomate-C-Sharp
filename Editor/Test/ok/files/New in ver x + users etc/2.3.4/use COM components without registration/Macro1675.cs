 /exe 1
out
 Services.clsServices ses._create
Services.clsServices ses._create(0 "$qm$\ARServicesMgr.DLL")
 Services.clsServices ses=CreateComObjectUnregistered("$qm$\ARServicesMgr.DLL" uuidof(Services.clsServices) uuidof(Services._clsServices))
Services.clsService se
foreach se ses
	out se.DisplayName

 BEGIN PROJECT
 main_function  Macro1675
 exe_file  $my qm$\Macro1675.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {BD64F703-3CA0-4553-93EB-AF5C02FBA523}
 END PROJECT
