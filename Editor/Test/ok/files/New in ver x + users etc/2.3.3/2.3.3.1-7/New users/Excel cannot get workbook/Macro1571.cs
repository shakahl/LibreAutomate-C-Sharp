 /exe 1

ExcelSheet es1

 this code is simplified Init version
IDispatch xlApp._getactive("Excel.Application")
es1=xlApp.ActiveSheet


es1.SetCell("OK" 1 22)

 BEGIN PROJECT
 main_function  Macro1571
 exe_file  $my qm$\Macro1571.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D14903F0-327F-4056-8DD4-2D36D426A9AE}
 END PROJECT
