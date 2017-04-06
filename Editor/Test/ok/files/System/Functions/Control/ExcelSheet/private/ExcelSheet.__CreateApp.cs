function'Excel.Application flags ;;flags: 0x100 load addins, 0x200 load startup files

 Creates Excel.Application. Optionally loads installed Excel addins (.xla, .xll) and/or startup files.

 REMARKS
 When Excel started by COM, it does not load addins and startup files.
 There are 2 Excel startup folders - the default startup folder (XLSTART) and the folder specified in Excel Options -> General. Loads all files from both.
 More info: <link>http://support.microsoft.com/kb/213489</link>.
 No errors.


Excel.Application a._create; err end _error
if(!flags) ret a

__ExcelState _.Init(this 1)
Excel.Workbook b
str sPath

if flags&0x100
	Excel.AddIn x
	foreach x a.AddIns
		if(!x.Installed) continue
		sPath=x.FullName
		int failed=0
		err-
		sel sPath 3
			case "*.xll"
			failed=!a.RegisterXLL(sPath)
			
			case "*.xl*" ;;xla, xlam
			b=a.Workbooks.Open(sPath) ;;xla
			b.RunAutoMacros(Excel.xlAutoOpen); err
			
			case else continue ;;maybe an Automation addin, then cannot Open, Excel shows alert
		
		err+ failed=1
		if(failed) out "Failed to load %s" sPath

if flags&0x200
	str s ss
	s=a.StartupPath; if(s.len) ss.addline(s)
	s=a.AltStartupPath; if(s.len) ss.addline(s)
	foreach s ss
		s+"\*"
		Dir d
		foreach(d s FE_Dir)
			sPath=d.FullPath
			b=a.Workbooks.Open(sPath); err out "Failed to load %s" sPath; continue
			b.RunAutoMacros(Excel.xlAutoOpen); err

err+ out _error.description

ret a
