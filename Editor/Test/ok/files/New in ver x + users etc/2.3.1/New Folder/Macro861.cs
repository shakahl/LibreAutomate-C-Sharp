act win("Excel" "XLMAIN") ;;activate the main window. Use this because act does it better than SetForegroundWindow.
SetForegroundWindow win("workbook name.xls") ;;try to activate the hidden window. Unlike act, this code does not give error. 
