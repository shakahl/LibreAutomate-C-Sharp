 /exe 1
 Use functions of ExcelSheet class to get and set cell
 values easier than directly with Excel functions.

 get cells A3 and B3
str a3 b3
ExcelSheet es.Init
es.GetCell(a3 1 3)
es.GetCell(b3 2 3)
out a3
out b3

 select range of cells
ExcelSheet es2.Init
es2.ws.Range("A1:B2").Select


 BEGIN PROJECT
 main_function  Easier Excel functions2
 exe_file  $desktop$\Easier Excel functions2.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {A3E1BD89-3426-4E4C-BABA-F74213E96524}
 END PROJECT
