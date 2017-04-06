\Dialog_Editor

str controls = "4"
str qmg4x

qmg4x=
 one
 two
 <////2>checked

if(!ShowDialog("" &sub.DlgProc &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 96 106 "QM_Grid"
 4 QM_Grid 0x56831041 0x0 18 14 64 78 "0x36,0,0,4,0x840[]A,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2040105 "*" "" "" ""


#sub DlgProc
function# hDlg message wParam lParam

DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret

 messages2
sel wParam
	case IDOK
ret 1

 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4
	sel nh.code
		case LVN_ITEMCHANGED
		int row isChecked
		if g.RowIsCheckNotification(lParam row isChecked)
			out "row %i %schecked" row iif(isChecked "" "un")
		else if g.RowIsSelectNotification(lParam row)
			out "row %i selected" row
