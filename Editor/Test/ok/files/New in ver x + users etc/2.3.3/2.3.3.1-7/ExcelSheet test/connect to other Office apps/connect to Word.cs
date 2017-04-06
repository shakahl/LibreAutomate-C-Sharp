ifi- "Microsoft Word"
	 run "winword.exe"
	run "winword.exe" "" "" "" 2
	 run "winword.exe" "" "" "" SW_SHOWNA
	1

typelib Word {00020905-0000-0000-C000-000000000046} 8.3
Word.Application a._getactive
out a
 ExcelSheet es.Init

 OfficeApp