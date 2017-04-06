function [what] [`sheet] [`book] ;;what: 0,1 worksheet, 2 workbook, 4 Excel

 Activates this or other sheet, workbook, or/and Excel window.

 what - flags for what to activate. Default: 1.
   If Excel is hidden, flag 4 also unhides it and makes it user-controlled, ie macro will not auto close it.
 sheet - a sheet to activate.
   Can be name, 1-based index, or object.
   Default: "" - this worksheet.
 book - a workbook to activate (in this Excel instance).
   Can be name, 1-based index, or object.
   Default: "" - this workbook.

 REMARKS
 Activating a sheet/book does not mean activating Excel window. It just brings the sheet/book on top of other sheets/books. To activate Excel window, use flag 4.
 Tip: To activate a cell, use SelectCell with flag 1.

 Added in: QM 2.3.3.
 Errors: <ExcelSheet._Sheet>

 EXAMPLES
 es.Activate ;;activate this worksheet
 es.Activate(1|4) ;;activate this worksheet and Excel window
 es.Activate(1 "Sheet2") ;;activate other sheet in this workbook
 es.Activate(2|4 "" "Book2.xls") ;;activate other workbook and Excel window


WS

if(!what) what=1

if what&1
	IDispatch d=_Sheet(sheet book)
	__CallWsOrChart(d 3)

if what&2
	Excel.Workbook b=_Book(book); err end F"{ERR_FILE} (book)"
	b.Activate
	 note: Maybe don't need this if flag 1. Activating a sheet also activates its workbook.
	 note: If workbook hidden: does not unhide, does not activate, not error.

if what&4
	Excel.Application a=ws.Application
	act __ExcelHwnd(a)
	if(!a.UserControl) a.UserControl=-1

err+ E
