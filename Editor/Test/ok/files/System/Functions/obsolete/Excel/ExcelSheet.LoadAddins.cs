function [flags] ;;obsolete. Use Init with flag 0x100.  flags: 1 run auto_open macros

Excel.Application a=ws.Application
Excel.AddIn x
foreach x a.AddIns
	str sPath=""
	if(!x.Installed) continue
	sPath=x.FullName
	int failed=0
	sel sPath 3
		case "*.xll"
		failed=!a.RegisterXLL(sPath)
		
		case "*.xl*" ;;xla, xlam
		Excel.Workbook b=a.Workbooks.Open(sPath) ;;xla
		if(flags&1) b.RunAutoMacros(Excel.xlAutoOpen); err
		
		case else continue ;;maybe an Automation addin, then cannot Open, Excel shows alert
	
	err+ failed=1
	if(failed) out "Failed to load %s" sPath
