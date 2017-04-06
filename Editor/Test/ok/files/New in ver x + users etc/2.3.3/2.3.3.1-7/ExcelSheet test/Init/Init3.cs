out
Excel.Application a
ExcelSheet es
ExcelSheet es2

 es.Init("" 8)
es.Init("" 8 "$documents$\Book1.xls")
 es.Init("" 8|16 "$documents$\test.xls")
 es.LoadAddins(1)

 es.Init("" 4 "$documents$\Book1.xls")
 es.Init("" 2|4|16 "$documents$\Book1.xls")
 es.Init("Sheet1" 1|2|4|16 "$documents$\Book1.xls")
 es.Init("" 8|16)
 es.Init
 es.Init("" 0 "book1.xls")
 es.Init("sheet1" 0 "book1.xls")
 es.Init("sheet1" 1 "book1.xls")
 es.Init("sheet1" 1|4|16 "book2.xls")
 out es.Cell("A1")

 2
 es.Init("" 8)

 es2.Init
 es2.Init("" 0 "" es)
 es2.Init("" 0 "book1.xls")
 es2.Init("" 4 "" es)
 es2.Init("" 4 "$documents$\Book1.xls" es)
 es2.Init("" 4 "$documents$\Book2.xls" es)
 es2.Init("" 4 "$documents$\Book1.xls")
 es2.Init("" 4 "$documents$\Book2.xls")
 Function199 es
 es2.Init("" 4 "" es)

 es.Activate(4)

 Excel.Application ea=es.ws.Application
 Excel.Workbook wb
 Excel.Worksheet ws
 foreach wb ea.Workbooks
	 str wbName=wb.Name
	 int nSheets=wb.Worksheets.Count
	 out "%i worksheets in %s:" nSheets wbName

 a=es.ws.Application

2

 es.Close
 3
