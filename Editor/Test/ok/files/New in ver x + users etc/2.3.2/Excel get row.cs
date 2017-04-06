 /exe 1
ARRAY(str) a
ExcelSheet es.Init

 get selected row index. If selected single column, select A-W.
int r nc
es.GetSelectedRange(0 nc r)
if(nc=1) es.SelectRange(F"A{r}:W{r}")

 get data from selection
es.GetCells(a "sel")

  make selected cells green
 Excel.Range sr=es.ws.Application.Selection
 sr.Interior.ColorIndex = 4
 sr.Interior.Pattern = Excel.xlSolid
  or (does not erase Undo)
 Acc ac=acc("Fill Color" "DROPLIST" win("" "XLMAIN") "MsoCommandBar" "" 0x1001)
 ac.Mouse(1)

 es.SetCell("content" 23 r)

 select next row
r+1
es.SelectRange(F"A{r}:W{r}")

 results
int i
for i 0 a.len(1)
	out a[i 0]
 A in a[0 0], B in a[1 0], C in a[2 0], ...


 BEGIN PROJECT
 main_function  Excel get row
 exe_file  $my qm$\Macro1456.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {EC9DFF59-96F9-49E2-8F93-D308B622450B}
 END PROJECT
