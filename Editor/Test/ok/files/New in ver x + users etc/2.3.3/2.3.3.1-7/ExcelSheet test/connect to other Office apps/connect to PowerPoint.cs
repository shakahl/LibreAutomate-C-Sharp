ifi- "Microsoft PowerPoint"
	run "powerpnt.exe"
	 run "powerpnt.exe" "" "" "" 2
	 run "powerpnt.exe" "" "" "" SW_SHOWNA
	1

typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8
PowerPoint.Application a._getactive
out a
 ExcelSheet es.Init

 OfficeApp