function! $name [flags] [str&getName] [Excel.Workbook&getWorkbook] ;;flags: 2 wildcard, 4 beginning, 8 end, 16 partial, 32 rx

 Finds an open workbook.
 Returns 1 if found, 0 if not.

 name - workbook name, like "book1.xls". Case insensitive.
 flags - how to compare name. Default: must match exactly. For example, use flag 32 if name is regular expression.
 getName - variable that receives name of the found workbook.
 getWorkbook - variable that receives COM interface of the found workbook.

 NOTES
 The variable can be not initialized. Then this function calls Init().

 EXAMPLE
 ExcelSheet es; str name
 if(!es.FindWorkbook("book?.xls" 2 name)) end "not found"
 es.Init("" 2 name)


if(!ws) Init
Excel.Workbook b
foreach b ws.Application.Workbooks
	_s=b.Name
	if SelStr(flags|1 _s name)
		if(&getName) getName=_s
		if(&getWorkbook) getWorkbook=b
		ret 1

err+ end _error
