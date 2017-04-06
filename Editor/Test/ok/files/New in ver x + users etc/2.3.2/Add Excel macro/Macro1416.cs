 /exe 1
ExcelSheet es.Init
Excel.Workbook wb=es.ws.Application.ActiveWorkbook

 use IDispatch because I don't know in which type library is VBProject
IDispatch objWorkbook=wb
IDispatch xlmodule = objWorkbook.VBProject.VBComponents.Add(1)
err mes- "Failed. You probably need to change Excel security settings.[][]Click menu Tools -> Macro -> Security.[]In Publishers tab check 'Trust access to Visual Basic project'." "" "x"

str strCode =
 sub test()
  msgbox "Inside the macro"
 end sub
 
xlmodule.CodeModule.AddFromString(strCode)


 BEGIN PROJECT
 main_function  Macro1416
 exe_file  $my qm$\Macro1416.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {61C0A098-DB9B-42DA-BC6F-C2DDBC91718E}
 END PROJECT
