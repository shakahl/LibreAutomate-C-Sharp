ExcelSheet es.Init ;;active worksheet

Excel.Workbook b=es._Book("$documents$\test.xls" 1)
es.SheetAdd("" b "copy")
b.Save
 b.Close; b=0

 str sFile.expandpath("$documents$\test.xls")
 Excel.Workbook b=es.ws.Application.Workbooks.Open(sFile)
 es.SheetAdd("" b "copy")
 b.Save
 b.Close; b=0
