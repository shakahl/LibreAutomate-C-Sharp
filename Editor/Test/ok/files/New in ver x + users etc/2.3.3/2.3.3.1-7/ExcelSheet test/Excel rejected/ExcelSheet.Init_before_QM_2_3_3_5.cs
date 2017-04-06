function [`sheet] [flags] [$book] [ExcelSheet&context] ;;flags: 1 activate sheet, 2 activate book, 4 open or new file, 8 open or new file in new Excel instance, 8|16 make Excel visible and leave running

 Connects to a worksheet in Excel; optionally opens or creates a workbook file.
 Call this function before calling other ExcelSheet functions.

 sheet - worksheet name.
   Default: "" - the active worksheet.
   Also can be 1-based worksheet index (does not include charts etc).
 book - workbook name, eg "Book1.xls". If omitted or "", connects to the active workbook. 
   With flags 4 or 8, book must be file path, eg "$personal$\Book1.xls". If omitted or "", creates new workbook (but does not save to file).
     QM 2.3.3. Does not reopen file.
 context (QM 2.3.3) - another variable. If used, the function connects to a worksheet in the same Excel instance as context; if book is "" - in the same workbook as context.

 REMARKS
 On Windows Vista/7, the macro process (QM or exe) must run with same integrity level as Excel.
   Usually Excel runs as User; QM runs as Admin or uiAccess. Then Init fails. You have these choices:
     1. Run macro in separate process as User: add /exe 1 line at the beginning, like in the example. The best choice in most cases.
     2. Run QM as User. Not recommended, because then QM does not work with some programs etc.
     3. Create exe from the macro and run it not from QM (eg double click in Explorer).
     4. Run Excel as admin. This may be necessary if need to run the macro as admin for some reason, eg if it gets data from an admin application. The QM run() function has a flag for admin.
     5. Use flag 8 with this function. It creates new Excel process of same integrity level as of macro process.

 This will happen when destroying or reinitializing the variable:
   If Excel is not user-controlled (eg started using Init with flag 8 and without 16), closes the workbook without saving changes. Excel process ends if it was the last workbook. 
   Else does not close anything, even if Init opened a file.

 If there are several Excel processes, connects to the process that first registered itself as an Excel process, usually it is the first started Excel process, not necessary the active window. Except with flag 8.
 Activating a sheet/book does not mean activating Excel window. It just brings the sheet/book on top of other sheets/books. To activate Excel window, use es.Activate(4) or act "Excel".

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
 es.Activate(4) ;;activate Excel
 out es.ws.Name

  open 2 files in new hidden Excel process
 ExcelSheet es1.Init("" 8 "$documents$\Book1.xls")
 ExcelSheet es2.Init("" 4 "$documents$\Book2.xls" es1)


if(ws) __Release

Excel.Application a
if flags&8
	if(&context) end ERR_BADARG
	 g1
	a._create
	if flags&16
		a.Visible=-1; a.UserControl=-1
		act a.Hwnd; err ;;or would flash taskbar
else if &context
	a=context.ws.Application
else
	a._getactive
	err
		 force registering in ROT
		int h=win
		if wintest(h "" "" "EXCEL")
			act "+Shell_TrayWnd"; err
			act h
			a._getactive; err
		
		if !a
			if(flags&4 and !win("" "" "EXCEL")) flags|16; goto g1
			lpstr _; if(_winnt>=6) _="[][9]Try to run this macro in separate process as User. Set it in Properties."
			end F"{ERR_FAILED} to connect to Excel.{_}"

Excel.Workbook b
if empty(book)
	if(flags&(4|8)) b=a.Workbooks.Add
	else if(&context) b=context.ws.Parent
	else b=a.ActiveWorkbook; if(!b) end "a workbook must be open"
else
	if flags&(4|8)
		str sfp.expandpath(book)
		if(flags&8=0) b=a.Workbooks.Item(_s.getfilename(sfp 1)); err
		if(!b) b=a.Workbooks.Open(sfp); err end ERR_FILEOPEN
	else
		b=a.Workbooks.Item(book); err end F"{ERR_BADARG} book"
	if(flags&2) b.Activate

int iss
sel(__VtReal(sheet)) case [VT_I4,VT_DISPATCH] iss=sheet.lVal; case VT_BSTR iss=1
if iss
	ws=b.Worksheets.Item(sheet); err end F"{ERR_BADARG} sheet"
	if(flags&1) ws.Activate
else
	ws=b.ActiveSheet; err ;;e_nointerface if chart
	if(!ws) end F"{ERR_FAILED} to get active worksheet"

err+ end _error

 notes:
 On Vista/7 fails if current process has different integrity level than Excel.
 Also tried accessibleobjectfromwindow. It does not have the above problem, but has other more severe problems. See GetExcelApplication.

 tested: Excel does not allow to open 2 files with same filename from 2 folders.
