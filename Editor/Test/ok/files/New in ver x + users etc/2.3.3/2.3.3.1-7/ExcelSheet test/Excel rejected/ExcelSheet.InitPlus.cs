function flags [$otherFiles] ;;flags: 1 load addins, 2 load startup files

 Loads installed Excel addins (.xla, .xll) and/or startup files.

 otherFiles - an optional list of other files to load. Can contain wildcard characters.

 REMARKS
 When Excel started by Init, it does not load addins and startup files. If need, call this function after Init.
 There are 2 Excel startup folders - the default startup folder (XLSTART) and the folder specified in Excel Options -> General. Loads all files from both.
 More info: <link>http://support.microsoft.com/kb/213489</link>.

 Added in: QM 2.3.3.


WS

__ExcelState _.Init(this 1)
Excel.Application a=ws.Application

if flags&1
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
			b.RunAutoMacros(Excel.xlAutoOpen); err
			
			case else continue ;;maybe an Automation addin, then cannot Open, Excel shows alert
		
		err+ failed=1
		if(failed) out "Failed to load %s" sPath

str s ss
if flags&2
	s=a.StartupPath; if(s.len) s+"\*"; ss.addline(s)
	s=a.AltStartupPath; if(s.len) s+"\*"; ss.addline(s)

if(!empty(otherFiles)) ss.addline(otherFiles)

foreach s ss
	 out s
	Dir d
	foreach(d s FE_Dir)
		sPath=d.FileName(1)
		out sPath
		b=a.Workbooks.Item(_s.getfilename(sPath 1)); err b=0
		if(b) continue
		b=a.Workbooks.Open(sPath); err out "Failed to load %s" sPath; continue
		b.RunAutoMacros(Excel.xlAutoOpen); err

err+ end _error
