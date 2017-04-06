\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str HTML=
 <style>
 table { border: 2px solid #D1D7DC; border-collapse: collapse; margin: 10px; width: 90%; }
 td,th { border: 1px solid #D1D7DC; border-collapse: collapse; padding-left: 4px; }
 </style>
 <form>
 <table>



str-- folder
if(BrowseForFolder(folder "$documents$" 4 "Choose Directory with .xls Files to merge")) 
	out folder
else
	ret

ExcelSheet esm.Init(0 8)
Excel.Application xlApp=esm.ws.Application
Excel.Workbook wbm=xlApp.ActiveWorkbook
 copy sheets from other workbooks
Dir d
int-- nrows

foreach(d F"{folder.expandpath}\*.xls" FE_Dir) ;;for each file
	str sPath=d.FileName(1)
	str filename=d.FileName; filename.fix(filename.len-4)
	str hfilename=F"<tr><td colspan=''2''><b>{sPath}</b></td></tr>"
	HTML.addline(hfilename)
	Excel.Workbook wb=xlApp.Workbooks.Open(sPath)
	Excel.Worksheet ws
	 nrows+1
	foreach ws wb.Sheets ;;for each sheet
		nrows+1
		str Name=ws.Name
		HTML.addline(F"<tr><tr><td align=''right'' width=''40px''><input checked type=''checkbox'' name=''sheet'' value=''{filename} {Name}'' /></td><td>{Name}</td></tr>")
	wb.Close
str END=
 </table>
 </form>
HTML.addline(END)
str ax3SHD=HTML
ax3SHD.setfile("$temp$\x1.htm")
ax3SHD="$temp$\x1.htm"


if(!ShowDialog("SheetSelector" &SheetSelector &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 440 337 "Merge Excel Files"
 2 Button 0x54030000 0x4 388 316 48 14 "Close"
 3 ActiveX 0x54030000 0x0 0 0 440 312 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 334 316 48 14 "Merge"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case 4
	Htm el
	 for each row
	for int'i 0 nrows
		el=htm("INPUT" "" "" hDlg 0 i+1)

			el=htm("INPUT" "" "" hDlg 0 i 0x20)
			str s=el.Attribute("value")
			if(el.Checked)
				str MergeSheets.addline(s)
	MergeExcelSheets MergeSheets folder
				
	case IDCANCEL
ret 1
