 /exe 1
ExcelSheet es.Init ;;connect to Excel

ARRAY(int) ad ;;rows to delete
ARRAY(str) ar ;;row cells
int i n row

 for each row in used range
row=es.ws.UsedRange.Row
foreach ar "" FE_ExcelRow
	 is empty?
	for(i 0 ar.len) if(ar[i].len) break
	if(i=ar.len) ad[]=row ;;empty
	row+1

 delete starting from bottom
for i ad.len-1 -1 -1
	Excel.Range r=es.ws.Rows.Item(ad[i])
	r.Delete(Excel.xlUp)


 BEGIN PROJECT
 main_function  Macro616
 exe_file  $my qm$\Macro616.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5E89E901-EB3D-40ED-9A4F-940769F54813}
 END PROJECT
