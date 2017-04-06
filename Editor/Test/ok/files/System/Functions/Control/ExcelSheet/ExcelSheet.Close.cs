function [save] ;;save: 0 auto (ask if visible, don't save if hidden), 1 save, 2 don't save, 16 (flag) close Excel if it was last workbook

 Closes this workbook and clears this variable.

 save - whether to save the file if modified:
   0 (default):
     If Excel is macro-controlled (started using Init with flag 8): don't save, don't prompt user to save, and not error.
     Else prompt user to save. Not error if user does not save.
   1 - always save.
     If it is a new unsaved workbook that does not have an associated file: if Excel is user-controlled, shows "Save As" dialog, else error.
   2 - don't save.
   16 (flag) - close Excel if there are no more open files.
     If Excel is macro-controlled, it usually quits even without this flag, if all files closed, addins not loaded, and all Excel objects released.

 REMARKS
 You may not need to call this function if Excel is macro-controlled (started using Init with flag 8). When destroying the variable, the file will be closed like if you would call Close with flags 2|16 (don't save, close Excel).

 Added in: QM 2.3.3.
 Errors: <>, Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init("" 4 "$documents$\book1.xls")
  ...
 es.Close(1|16) ;;save, close file, close Excel


WS

Excel.Application a=ws.Application
Excel.Workbook b=ws.Parent

sel save&3
	case 1
	if(!b.Saved and !a.UserControl) _s=b.Path; if(!_s.len) end ERR_SAVE ;;if hidden, would display dialog behind other windows without taskbar button
	b.Close(1 @ 1) ;;tested: the Filename arg does not work
	
	case 2
	b.Close(0 @ 0)
	
	case 0
	if(a.UserControl) b.Close; else b.Close(0 @ 0)
	
	case else end ERR_BADARG

ws=0
b=0
if save&16 and !a.Workbooks.Count
	int hwnd=__ExcelHwnd(a)
	a.Quit; a=0
	
	 workaround for Excel 2003 bug: does no quit if used accessibleobjectfromwindow
	wait 2 WD hwnd ;;normally 0.18 s
	err clo hwnd; err

err+ end _error
