 /exe 1
ExcelSheet es.Init

int r c nr nc ;;row, column, num rows, num cols
nr=es.NumRows(nc) ;;get number of rows and cols
ARRAY(str) a.createlb(nc 1) ;;cells of a row
str s ;;result
for r 1 nr+1 ;;for each row
	for(c 1 nc+1) es.GetCell(a[c] c r) ;;get cells of this row
	YourFunction r a s ;;pass a to your function, and let it store result in s
	es.SetCell(s nc+1 r) ;;set cell at the end of the row

 BEGIN PROJECT
 main_function  Macro1620
 exe_file  $my qm$\Macro1620.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5E892509-6BE6-411D-8E37-A4BBB877C765}
 END PROJECT
