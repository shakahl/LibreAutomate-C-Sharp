 /exe 1
typelib Excel
ExcelSheet es.Init
Excel.Application a=es.ws.Application

a.Charts.Add
a.ActiveChart.Location(xlLocationAsNewSheet)

 BEGIN PROJECT
 main_function  Macro906
 exe_file  $my qm$\Macro906.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {2B3D5285-146E-4941-84AA-88426485FA60}
 END PROJECT
