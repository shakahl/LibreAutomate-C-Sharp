\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib prjSCGrid {28FDFE2B-68C4-11D4-92BF-8364E6807441} 13.8
 typelib prjSCGrid {BDE7D104-7E95-11D4-B68F-D74C9D5E7B57} 4.1

if(!ShowDialog("DlgSCGrid" &DlgSCGrid 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 8 8 176 82 "prjSCGrid.SCGrid"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	prjSCGrid.SCGrid sc3._getcontrol(id(3 hDlg))
	 sc3._setevents("sc3___SCGrid")
	
	sc3.CaptionEnabled=-1
	sc3.Text(0 0)="A"
	sc3.Text(0 1)="B"

	 stdole.StdFont f._create
	 f.Name="Courier New"
	 f.Underline=-1
	 sc3.Font=f

	ARRAY(VARIANT) a.create(2 2)
	a[0 0]="a"; a[1 0]="b" ;;first row
	a[0 1]="c"; a[1 1]="d"
	sc3.LoadFromArray(a -1)
	sc3.RowMode(2)=3 ;;editable
	
	sc3.CellBackColor(1 1)=0xffff
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
