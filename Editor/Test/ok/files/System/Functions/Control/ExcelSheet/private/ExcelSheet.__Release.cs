int myProp=__VarProp_Remove(&this)
if(!ws) ret

if myProp&1
	Excel.Application a=ws.Application ;;note: need to use variable, or Excel may quit too early, before closing other files
	if !a.UserControl
		Excel.Workbook b=ws.Parent
		ws=0
		b.Close(0 @ 0); b=0 ;;don't save etc
		if(!a.Workbooks.Count) a.Quit
		 Normally Quit() not necessary; Excel quits when all books closed and objects released.
		 However does not quit if addins loaded, and probably in some other conditions that i don't know.

err+ out "Warning: error (RT) in ExcelSheet destructor: %s" _error.description
ws=0
