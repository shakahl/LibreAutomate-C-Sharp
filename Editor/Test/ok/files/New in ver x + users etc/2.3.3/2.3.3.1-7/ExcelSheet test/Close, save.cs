out
 Q &q
 ExcelSheet es1.Init("" 8 "$documents$\Book1.xls")
 ExcelSheet es2.Init("" 4 "$documents$\Book2.xls" es1)

 ExcelSheet es1.Init("" 4 "$documents$\Book1.xls")
ExcelSheet es1.Init("" 4)
 ExcelSheet es1.Init("" 8)
es1.SetCell("tt" 1 1)

 Excel.Application a=es1.ws.Application
 out a.UserControl
 a.Visible=-1
 10
 out a.UserControl

es1.Close
 es1.Close(16)
 es1.Close(1|16)
 es1.Close(2|16)

 es1.Save("$documents$\__saveas2.xls" 4)


 out es1.ws
 out es2.ws
 Q &qq; outq

 Excel.Workbook b
 foreach b es1.ws.Application.Workbooks
	 out b.name
 
 5

 Excel.Workbook b=es2._Book
 b.Close
 es2
 1