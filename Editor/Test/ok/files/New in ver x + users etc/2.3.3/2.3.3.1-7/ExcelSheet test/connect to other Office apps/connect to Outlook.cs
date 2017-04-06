ifi- "Microsoft Outlook"
	 run "outlook.exe"
	run "outlook.exe" "" "" "" 2
	 run "outlook.exe" "" "" "" SW_SHOWNA
	1

typelib Outlook {00062FFF-0000-0000-C000-000000000046} 9.2
Outlook.Application a._getactive
out a
 ExcelSheet es.Init

 OfficeApp