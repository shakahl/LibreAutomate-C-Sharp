 /exe 1

ExcelSheet es1

 this code is simplified Init version
Excel.Application xlApp._getactive
Excel.Workbook wb=xlApp.Workbooks.Item("Book1")
es1=wb.ActiveSheet


es1.SetCell("OK" 1 22)

 BEGIN PROJECT
 main_function  Macro1575
 exe_file  $my qm$\Macro1575.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {7C508F0F-503B-437D-8C55-076BAF90FBE0}
 END PROJECT
