function'IDispatch hwndExcel

int w1 pid
w1=child("" "EXCEL7" hwndExcel)
if !w1 ;;not main window?
	if(!GetWindowThreadProcessId(hwndExcel &pid)) ret
	hwndExcel=win("" "XLMAIN" pid)
	w1=child("" "EXCEL7" hwndExcel)

Excel.Window w
if(AccessibleObjectFromWindow(w1 OBJID_NATIVEOM uuidof(w) &w)) ret
ret w.Application

err+

 notes:
 When using AccessibleObjectFromWindow:
   Does not work in Excel 97.
   Excel bug: if using Excel.Application A=w.Application: exception when calling A functions. Workaround: assign to IDispatch at first.
   Excel 2003 bug, fixed in later versions: does not quit on a.Quit. Workaround: hwnd=a.Hwnd; a.Quit; a=0; clo hwnd
 If there are no workbooks open or addins loaded (addins add hidden workbooks), there is no EXCEL7 child window.
 tested: does not work with CommandBar as documented. Gets IAccessible instead.
