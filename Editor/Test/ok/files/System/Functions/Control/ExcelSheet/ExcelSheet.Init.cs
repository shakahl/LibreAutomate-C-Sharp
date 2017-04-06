function [`sheet] [flags] [$book] [`context] ;;flags: 1 activate sheet, 2 activate book, 4 open or new file, 8 open or new file in new Excel process, 16 make Excel visible, don't close, activate, 0x100 load addins, 0x200 load startup files

 Connects to a worksheet in Excel; optionally opens a workbook.
 Call this function before calling other ExcelSheet functions.

 sheet - worksheet name.
   Default: "" - the active worksheet.
   Also can be 1-based worksheet index (does not include charts etc).
 flags:
   1 - activate the sheet and its workbook.
   2 - activate workbook of the sheet. Note: these flags don't activate Excel window; use es.Activate(4) or act "Excel".
   4 - open file or create new workbook in existing or new Excel process.
     If book omitted or "", creates new workbook.
     Else book must be file path, eg "$documents$\Book1.xls". Opens the file in Excel, or connects to it if already open.
     If Excel is not running, works like with flags 8|16, ie creates new visible Excel process.
   8 - open file or create new workbook in new Excel process.
     Same as flag 4, but always creates new process.
     By default the process is hidden and macro-controlled, ie will be closed by the macro.
   16 (usually with flag 8) - make Excel process visible and user-controlled, ie don't close.
     QM 2.3.3. Activates Excel window. It works with any other flags.
   0x100 - load addins (.xla, .xll).
   0x200 - load files from Excel startup folders.
     Flags 0x100 and 0x200 used only when this function starts new Excel process (flag 8 or 4). By default Excel would not load addins and startup files. If Excel is already running, and was started normally (user, run, etc), it loads everything (don't need these flags).
 book - workbook.
   If flag 4 or 8 used, it must be file path or not used. Read more above.
   Else if omitted or "", connects to the active workbook in the first found Excel process (or process of context). 
   Else it must be filename, eg "Book1.xls". Connects to the workbook if it is open in Excel.
 context (QM 2.3.3) - can be:
   Another ExcelSheet variable.
     The function connects to a worksheet in the same Excel process as context; if book is "" - in the same workbook as context (if flag 4 - adds new workbook).
   Excel window handle.
     The function connects to Excel process of the window.
     Can be main window (class "XLMAIN") or other (eg a workbook taskbar button window, class "MS-SDIa").
     Read more in Remarks.
   Default: "".

 REMARKS
 By default, connects to a running Excel process. Error if Excel not running.
 With flag 8 - creates new process.
 With flag 4 - connects or creates new.

 When destroying or reinitializing the variable:
   If Init opened or created workbook in hidden (macro-controlled) Excel process, closes the workbook without saving changes.
     If it was the last workbook, Excel process ends. After that, you should not use other variables that refer to this Excel instance.
     Anyway, when testing macro, you should look in Task Manager to make sure that there are no abandoned hidden Excel processes.
   Else (if Excel is visible, user-controlled) does not close anything, even if Init opened a file.

 On Windows Vista/7/8/10, the macro process (QM or exe) should run with same integrity level (IL) as Excel.
   Usually Excel runs as User; QM runs as Admin or uiAccess. Then Init may fail (and always fails in QM < 2.3.3 or with Excel < 2000).
   You have these choices:
     1. Run macro in separate process as User: add /exe 1 line at the beginning, like in the example. The best choice in most cases.
     2. Run QM as User. Not recommended, because then QM does not work with some programs etc.
     3. Create exe from the macro and run it not from QM (eg double click in Explorer).
     4. Run Excel as admin. This may be necessary if need to run the macro as admin for some reason, eg if it gets data from an admin application. The QM run() function has a flag for admin.
     5. Use flag 8 with this function. It creates new Excel process of same IL as of macro process.

 If there are several Excel processes:
   If book is specified and it is open in Excel, connects to it. However may fail if different IL (read above); then use Excel window handle (context).
   Else connects to the first found Excel process. It may be not the topmost Excel window. It even can be hidden (macro-controlled).

 If used window handle (context), this function uses different method to connect to Excel.
   Also uses this method if fails to connect normally because of different IL.
   However not always possible to connect using this method. Fails if there are no workbooks or addins loaded; fails if Excel < 2000; fails if Excel has higher IL (admin).
   Also, there is a bug in older Excel versions, eg 2003. In certain conditions Excel may not quit when expected. Look in Task Manager, and change your macro code if you see abandoned hidden Excel processes there. Fixed in Excel 2010.

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
Excel.Workbook b
Excel.Worksheet wsContext ;;info: don't call through IDispatch, Excel bug
str sb.expandpath(book)

if __VtReal(context)
	if(flags&8) end ERR_BADARG
	sel context.vt
		case VT_DISPATCH wsContext=context.pdispVal; a=wsContext.Application
		case VT_I4 a=__AppFromHwnd(context.lVal); if(!a) end F"{ERR_HWND} (context)"
		case else end ERR_BADARG
else if flags&8
	a=__CreateApp(flags&0x300)
else
	if sb.len ;;is it open?
		if(flags&4 or findc(sb ".")<0) b._getactive(0 0 sb); err ;;full path or new workbook name
		else b._getactive(0 0 _s.from("*\" sb)); err ;;filename.ext
		if(b) goto gWS
	
	a._getactive
	err ;;probably different IL, or still not registered
		int wExcel=win("" "XLMAIN" "EXCEL")
		if !wExcel
			if(flags&4) flags|16; a=__CreateApp(flags&0x300)
			lpstr moreInfo="Excel not running."
		else
			int activeExcel=wExcel=win
			rep(2) activeExcel^1; SendMessageW wExcel WM_ACTIVATEAPP activeExcel 0 ;;register
			a._getactive
			err a=__AppFromHwnd(wExcel) ;;probably different IL
			if(_winnt>=6) moreInfo="Try to run this macro in separate process as User. <help ''ExcelSheet.Init''>More info</help>."
		
		if(!a) end F"{ERR_FAILED} to connect to Excel. {moreInfo}"

int myWB
if sb.len
	if flags&(4|8)
		if(flags&8=0) b=a.Workbooks.Item(_s.getfilename(sb 1)); err
		if(!b) myWB=1; b=a.Workbooks.Open(sb); err end ERR_FILEOPEN
	else
		b=a.Workbooks.Item(sb); err end F"{ERR_BADARG} book"
else
	if(flags&(4|8)) myWB=1; b=a.Workbooks.Add
	else if(wsContext) b=wsContext.Parent
	else b=a.ActiveWorkbook; if(!b) end F"{ERR_FAILED} to get active workbook" ;;tested: succeeds when connected to a hidden Excel, but fails when Excel is visible but there is no visible workbooks

if(myWB and flags&16=0) if(flags&8 or !a.UserControl) __VarProp_Set &this 1 ;;if opened/created workbook in hidden Excel, close it in dtor (__Release())

 gWS
int iss
sel(__VtReal(sheet)) case [VT_I4,VT_DISPATCH] iss=sheet.lVal; case VT_BSTR iss=1
if iss
	ws=b.Worksheets.Item(sheet); err end F"{ERR_BADARG} sheet"
	if(flags&1) ws.Activate
else
	ws=b.ActiveSheet; err ;;e_nointerface if chart
	if(!ws) end F"{ERR_FAILED} to get active worksheet"

if(flags&2) b.Activate
if(flags&16) Activate(4) ;;or would flash taskbar; also makes visible and user-control

err+ end _error

 notes:
 tested: Excel does not allow to open 2 files with same filename from 2 folders.

 if flags&4 and sb.len, could use _getfile. But then we have too little control. Need to make visible or not?; Excel asks to save; etc.

 When Excel started normally (user etc), it registers Application in ROT when first time deactivated main Excel window (WM_ACTIVATEAPP).
 When started by _create etc, it receives command line /Automation, and registers immediately.
 Always registers workbooks immediately.

 a._getactive is much faster than b._getactive(... file).
