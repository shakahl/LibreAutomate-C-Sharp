function'IDispatch `name [`book] [flags] ;;flags: 2-32 name match flags (2 wildcard, 4 beginning, 8 end, 16 middle, 32 rx)

 Finds or gets a sheet and returns its object (Excel.Worksheet if it's a worksheet).
 Error if not found.

 name - sheet.
   Can be name, 1-based index, or object (simply returns the object).
 book - parent workbook.
   Default: "" - workbook of this worksheet.
   Can be name, 1-based index, or object.

 REMARKS
 An Excel.Worksheet variable represents an open worksheet.
 You can use it to manage the worksheet and related objects.
 Don't need to use this function to get worksheet of this variable. It's the ws member of this variable.
 You can assign the return value to an ExcelSheet variable (this or other).
 This function finds sheets of any type. Error if you assign it to a variable of type that does not match sheet type. For example, use Excel.Chart if the sheet is a chart.
 Names are case insensitive.

 Added in: QM 2.3.3.
 Errors: <> (Excel errors)

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
 Excel.Worksheet x=es._Sheet("Sheet3")
 out x.Name
 x.Activate
 
 ExcelSheet es3=es._Sheet(3)
 out es.Cell(1 1)


WS

IDispatch x
Excel.Workbook b=this._Book(book); err end F"{ERR_FILE} (book)"
 
sel __VtReal(name)
	case 0 ret ws
	case VT_BSTR
	case VT_I4 flags=0
	case VT_DISPATCH if(name.pdispVal) ret name.pdispVal; else end ERR_BADARG
	case else end ERR_BADARG

if flags&62=0
	x=b.Sheets.Item(name); err
	if(x) ret x
else
	str sn(name) _sn
	foreach x b.Sheets
		__CallWsOrChart(x 0 _sn); err continue
		if(SelStr(flags|1 _sn sn)) ret x

end ERR_OBJECT

err+ end _error
