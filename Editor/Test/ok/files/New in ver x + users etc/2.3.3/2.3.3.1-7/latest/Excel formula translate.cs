 /exe 1
 This is simplified code that ExcelSheet.Init uses with flag 4 (open file in existing Excel process).
str book.expandpath("$documents$\book1.xls")
Excel.Application a._getactive ;;attach to a running Excel process
Excel.Workbook b=a.Workbooks.Open(book) ;;open the workbook. Does not use other (optional) arguments.
Excel.Worksheet ws=b.ActiveSheet

out ws.Name

 typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.3
 Office.LanguageSettings k=a.LanguageSettings
 out k.LanguageID

 b.RunAutoMacros(Excel.xlAutoOpen)



 BEGIN PROJECT
 main_function  Macro1629
 exe_file  $my qm$\Macro1629.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {5A20527D-B504-4D87-A735-044C45F3126B}
 END PROJECT
