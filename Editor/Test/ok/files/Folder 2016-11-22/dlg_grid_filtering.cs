\Dialog_Editor

str sItems=
 January
 February
 March
 April
 May
 June
 July
 August
;
rep(10) sItems+sItems
 out numlines(sItems) ;;8192

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 0 0 96 14 ""
 4 QM_Grid 0x5603504D 0x200 0 16 96 98 "0x12,0,0,0x0,0x0[]A,80%,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3 4"
str e3 qmg4x
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc v
function# hDlg message wParam lParam
DlgGrid g.Init(hDlg 4)
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 300 0
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		g.RowsDeleteAll
		str s sEdit.getwintext(id(3 hDlg))
		foreach s sItems
			if(sEdit.len and find(s sEdit 0 1)<0) continue
			g.RowAddSetSA(-1 &s 1)
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	SetTimer hDlg 1 300 0
	
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4
	NMITEMACTIVATE* na=+nh
	sel nh.code
		case NM_CLICK
		if na.iItem>=0
			out "row click: %i %s" na.iItem g.CellGet(na.iItem 0)
