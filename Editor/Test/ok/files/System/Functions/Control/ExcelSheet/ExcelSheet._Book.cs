function'Excel.Workbook [`name] [flags] ;;flags: 1 open file, 2-32 name match flags (2 wildcard, 4 beginning, 8 end, 16 middle, 32 rx)

 Gets Excel.Workbook object of this or other workbook; also can open.
 Error if not found.

 name - workbook name, like "book1.xls". Case insensitive.
   Also can be 1-based index.
   If omitted or "", gets workbook of this worksheet.
   If flag 1, must be file path; opens the file if not open; other flags not used.

 REMARKS
 An Excel.Workbook variable represents an open workbook.
 You can use it to manage the workbook, its worksheets and other related objects.
 For example, get workbook name, file path, activate, close, save with different format, manage sheets (get active, list, add, delete...), add styles...

 Added in: QM 2.3.3.
 Errors: <> (Excel errors)

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
 Excel.Workbook wb=es._Book ;;get workbook of this worksheet
 out wb.Name
 
 wb=es._Book("test.xls") ;;get an open workbook
 wb.Activate
 
  list worksheets
 Excel.Worksheet ws
 foreach ws wb.Worksheets
 	out ws.Name
 
  open a workbook file and copy this worksheet to it
 Excel.Workbook wb=es._Book("$documents$\test.xls" 1)
 es.SheetAdd("" wb "copy")
 wb.Save
 wb.Close; wb=0


WS

Excel.Workbook b

if flags&1
	str sfp.expandpath(name)
	b=ws.Application.Workbooks.Item(_s.getfilename(sfp 1)); err
	if(!b) b=ws.Application.Workbooks.Open(sfp); err end ERR_FILEOPEN
	ret b

sel __VtReal(name)
	case 0 ret ws.Parent
	case VT_BSTR
	case VT_I4 flags=0
	case VT_DISPATCH if(name.pdispVal) ret name.pdispVal; else end ERR_BADARG
	case else end ERR_BADARG

if flags&62=0
	b=ws.Application.Workbooks.Item(name); err
	if(b) ret b
else
	_s=name
	foreach b ws.Application.Workbooks
		if(SelStr(flags|1 b.Name _s)) ret b

end ERR_OBJECT

err+ end _error
