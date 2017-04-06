 /exe 1
typelib Excel
ExcelSheet es.Init
Excel.Application a=es.ws.Application

a.ActiveWorkbook.PivotCaches.Add(xlDatabase, "Sheet1!R1C1:R6C3").CreatePivotTable("", "PivotTable2", @, xlPivotTableVersion10)
Excel.Worksheet ws=a.ActiveSheet
Range r=ws.Cells.Item(3, 1)
ws.PivotTableWizard(@ @ r)
r.Select
Excel.PivotTable t=ws.PivotTables("PivotTable2")
t.PivotFields("name").Orientation = xlRowField
t.PivotFields("name").Position = 1
t.PivotFields("date").Orientation = xlColumnField
t.PivotFields("date").Position = 1
t.AddDataField(t.PivotFields("score"), "Sum of score", xlSum)

 BEGIN PROJECT
 main_function  Macro904
 exe_file  $my qm$\Macro904.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {3B28B1AB-112B-4F8B-9DEA-67CCB9FAE694}
 END PROJECT
