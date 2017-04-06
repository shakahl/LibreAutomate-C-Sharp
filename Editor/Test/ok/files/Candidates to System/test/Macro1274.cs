 /exe 1
 out
act "Excel"

ExcelSheet es.Init
 es.SelectCell(2 2)
 es.SelectRange("")
 es.SelectRange("H8")
 es.SelectRange("A21:C33")
es.SelectRange("A:C")
 es.SelectRange("3:3")

 BEGIN PROJECT
 main_function  Macro1274
 exe_file  $my qm$\Macro1274.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {18793046-C742-46B3-A303-B0E9251B9297}
 END PROJECT
