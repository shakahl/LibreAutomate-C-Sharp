out
ExcelSheet es.Init
 ExcelSheet es.Init(4)
 out es.ws.Name

Excel.Workbook wb
 wb=es._Book
 wb=es._Book("mokesciai_20100112.xls")
 wb=es._Book("mokes" 4)
 wb=es._Book("*kes*" 2)
 wb=es._Book(".xls" 8)
 wb=es._Book("kes" 16)
 wb=es._Book(".+kes.+" 32)
 wb=es._Book(2)
 wb=es._Book("Boo" 4)
 out wb.Name

Excel.Worksheet x
 x=es._Sheet("Sheet3")
 x=es._Sheet("Sheet*" "" 2)
 x=es._Sheet(3)
 x=es._Sheet("qwerty")
 x=es._Sheet("qwerty" "test.xls")
 x=es._Sheet("qwert?" "test.xls" 2)
 x=es._Sheet("qwerty" 2)
 x=es._Sheet("qwerty" "")
 x=es._Sheet("qwerty" es._Book("test.xls"))
 wb=es._Book("test.xls")
 x=es._Sheet("qwerty" wb)
 out x.Name

 wb=es._Book("test.xls")
 wb.Close

 wb=es._Book ;;get workbook of this worksheet
 wb.SaveAs(_s.expandpath("$documents$\test ExcelSheet.csv") Excel.xlCSV @ @ @ @ 1) ;;save in CSV format
 wb.Saved=TRUE
 wb.Close
 out wb.FullName ;;file path, if saved
 out wb.Path
 Excel.Worksheet ws; foreach(ws wb.Worksheets) out ws.Name ;;list sheets
 out wb.Windows

 out es.WorksheetRename("test")
 out es.WorksheetRename("123456789012345678901234567890abcd")
 out es.WorksheetRename("a[k]b?*c\/d:")
 out es.WorksheetRename("qwerty")

 ExcelSheet es2=es.SheetAdd("test")
 es2.SetCell("z" 1 1)
 2
 es2.SheetDelete

 ExcelSheet es2=es.SheetAdd("to other book" "test.xls")
 es2.SetCell("z" 1 1)

 ExcelSheet es.Init
 ExcelSheet es2=es._Sheet("sheet3")
 es2.SheetDelete

 es=es.SheetAdd("new")
  for(_i 1 es.ws.Parent.Worksheets.Count) 
 for _i 1 es.ws.Parent.Worksheets.Count
	 ExcelSheet es2=

 int i n=es.ws.Parent
 Excel.Workbook b=es._Book
 Excel.Worksheet x
 foreach(x b.Worksheets) x.Delete

 Excel.Workbook b=es._Book
 Excel.Sheets c=b.Worksheets
  c.Delete
 Excel.Worksheet x
 foreach(x c) x.Delete
  int i n=b.Sheets.C
 es=c.Add

 es.SheetRename("hhh")
 1
 es.SheetRename("BBB" "hhh")
 es.SheetRename("BBB" "hhh" "test.xls")
 es.SheetRename("CCC" "Chart1")

 Excel.Workbook b=es.ws.Parent
 out b.Sheets.Count

 es.SheetDelete("BBB")
 es.SheetDelete("new2" "Book6")
 out es.ws

 ExcelSheet es2=es.SheetAdd("test" "" Excel.xlWorksheet)
 2
 es2.SheetDelete

 IDispatch d=es.SheetAdd("chart" "" Excel.xlChart)
 IDispatch d=es.SheetAdd("chart" "" "timecard.xlt") ;;error
 Excel.Worksheet d=es.SheetAdd("chart" "" "C:\Program Files\Microsoft Office\Templates\1033\timecard.xlt")
 2
 es.SheetDelete(d)

 str s
 s=es.Cell(0 0 "G1")
 DATE d=s
 d=d+1
 out d

 DateTime d
 d.FromStr(es.Cell(0 0 "G1")) ;;get as DateTime
 d.AddParts(1) ;;add 1 day
 es.SetCell(d.ToStr 0 0 "G1") ;;set cell from DateTime

 out es.Cell(6 1)
 out es.Cell("F" 1)
 out es.Cell("F1")
 out es.Cell(0 0)
 out es.Cell("")
 out es.Cell("_1")

 IDispatch d=es.SheetAdd("test" "" "")
 2
 es.SheetDelete(d)
 es.SheetDelete("test")
 es.SheetDelete("test" "test.xls")

 Excel.Range r=es.ws.Cells.Item(1 "A")
 Excel.Range r=es.ws.Cells.Item(1)
 Excel.Range r=es.ws.Cells.Item("1" "A")
 out r.Value
