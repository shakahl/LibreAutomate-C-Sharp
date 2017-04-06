out
 int hwndExcel=win("" "XLMAIN" "EXCEL")
int hwndExcel=win("" "MS-SDIa" "EXCEL")
outw hwndExcel
ExcelSheet es.Init("" 0 "" hwndExcel)
 ExcelSheet es.Init("" 0 "Book1.xls" hwndExcel)
out es.ws.Name

 ExcelSheet es2.Init("Sheet2" 0 "" es)
ExcelSheet es2.Init("copythis" 0 "Book1.xls" es)
out es2.ws.Name
