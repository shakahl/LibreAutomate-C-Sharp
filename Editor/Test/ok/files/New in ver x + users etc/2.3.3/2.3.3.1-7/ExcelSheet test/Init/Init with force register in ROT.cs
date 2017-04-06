out
ifi- "Microsoft Excel"
	  run "excel.exe"
	  run "excel.exe" "" "" "" 2
	  run "excel.exe" "/Automation" "" "" 2
	run "excel.exe" "" "" "" SW_SHOWNA
	1

ExcelSheet es

es.Init
 es.Init("Sheet1" 3 "Book1")
 es.Init("" 0 "Book1")



out es.Cell("A1")
int+ g_excel; g_excel+1; out g_excel
