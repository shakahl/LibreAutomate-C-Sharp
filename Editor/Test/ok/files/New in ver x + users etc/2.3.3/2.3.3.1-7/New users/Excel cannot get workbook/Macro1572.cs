/exe 1

str vb=
 Set xlApp=GetObject(,"Excel.Application")
 Set es1=xlApp.ActiveSheet
 MsgBox not Nothing is es1
VbsExec2 vb
 if es1 is Nothing then
 MsgBox "failed"
 end if
 es1.Cells("A:20").Value="OK"

 BEGIN PROJECT
 main_function  Macro1572
 exe_file  $my qm$\Macro1572.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {B69F1E52-8E0B-49EB-ACEE-DD834A50FF78}
 END PROJECT
