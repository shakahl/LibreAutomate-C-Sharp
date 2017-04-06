ExcelSheet es.Init ;;active worksheet

ExcelSheet es3.Init("" 4 "$documents$\test.xls")
Excel.Workbook b=es3._Book
es3=es.SheetAdd("" b "copy")
es3.Save
b.Close

 Excel.Workbook b=es.ws.Application.Workbooks.Open(_s.expandpath("$documents$\test.xls"))
