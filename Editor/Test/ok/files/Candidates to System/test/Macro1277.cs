 /exe 1

out
 act "Excel"
ExcelSheet es.Init
 es.SelectCell(5 12)

int nr nc r1 c1
 out es.GetUsedRange(nr nc r1 c1)
out es.GetSelectedRange(nr nc r1 c1)
out "%i %i %i %i" nr nc r1 c1

 BEGIN PROJECT
 main_function  Macro1277
 exe_file  $my qm$\Macro1277.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {E1AA86A7-337C-44E3-97C7-EC36E7A07993}
 END PROJECT
