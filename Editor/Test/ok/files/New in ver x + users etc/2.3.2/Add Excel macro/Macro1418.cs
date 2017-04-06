 /exe 1
act "Excel"
ExcelSheet es.Init
es.RunMacro("test")

 BEGIN PROJECT
 main_function  Macro1418
 exe_file  $my qm$\Macro1418.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {E59966A0-CF24-4E65-8020-3CDCEF6CC5D0}
 END PROJECT
