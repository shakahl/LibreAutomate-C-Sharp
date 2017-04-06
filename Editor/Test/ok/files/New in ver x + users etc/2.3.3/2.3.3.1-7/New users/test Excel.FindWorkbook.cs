 /exe 1
 ExcelSheet es.Init("sheet1" 2 "book3.xls")
 es.GetCell(_s 1 1); out _s

ExcelSheet es; str name
if(!es.FindWorkbook("book?.xls" 2 name)) end "not found"
es.Init("" 2 name)


 BEGIN PROJECT
 main_function  Macro1569
 exe_file  $my qm$\Macro1569.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {BBDA1965-387F-4E51-8BFE-DE1BDA54D87A}
 END PROJECT
