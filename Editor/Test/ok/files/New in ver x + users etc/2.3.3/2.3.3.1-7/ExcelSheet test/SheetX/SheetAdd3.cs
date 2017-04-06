ExcelSheet es.Init
IDispatch d
 d=es.SheetAdd("nnn" "" "$pf$\Microsoft Office\Templates\1033\timecard.xlt")
 d=es.SheetAdd("nnn" "" Excel.xlChart)
 d=es.SheetAdd("nnn" "" "copy")
 d=es.SheetAdd("nnn")
 d=es.SheetAdd("" "test.xls")
 d=es.SheetAdd("nnn" "test.xls")
 d=es.SheetAdd("" "test.xls" "copy")
 d=es.SheetAdd("CO" "test.xls" "copy")
 d=es.SheetAdd("cha" "test.xls" Excel.xlChart)
d=es.SheetAdd("nnn" "test.xls" "$pf$\Microsoft Office\Templates\1033\timecard.xlt")
 2
 es.SheetDelete(d)

 Excel.Workbook b=es._Book
 IDispatch x=b.ActiveSheet
 out x.Name
