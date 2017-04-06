 /exe 1

str vb=
 Set xlApp=GetObject(,"Excel.Application")
 Set es1=xlApp.ActiveSheet
 MsgBox not Nothing is es1
VbsExec vb
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
 guid  {C00FAD8D-B73C-488A-A950-ACDCEF8938F9}
 END PROJECT
