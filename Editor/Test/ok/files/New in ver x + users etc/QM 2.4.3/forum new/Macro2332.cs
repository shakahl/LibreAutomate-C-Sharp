\Dialog_Editor

str controls = "4"
str qmg4x

qmg4x=
 one
 two
 <////2>checked

if(!ShowDialog("" &sub.DlgProc &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 138 106 "QM_Grid"
 4 QM_Grid 0x56831041 0x0 18 14 64 78 "0x36,0,0,4,0x840[]A,,,"
 3 Button 0x54032000 0x0 84 40 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2040107 "*" "" "" ""

#sub DlgProc
function# hDlg message wParam lParam
DlgGrid g.Init(hDlg 4)
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret

 messages2
sel wParam
	case 3
	g.RowCheck(1 1)
ret 1


 messages3
NMHDR* nh=+lParam
NMITEMACTIVATE* na=+nh
NMLISTVIEW* nlv=+nh
sel nh.idFrom
	case 4
	sel nh.code
		case LVN_ITEMCHANGED
		if(nlv.iItem<0 or nlv.uNewState&LVIS_SELECTED=0) ret
		out "selected %i" nlv.iItem ;;selected listview item index
		
		case NM_RCLICK ret DT_Ret(hDlg 1)