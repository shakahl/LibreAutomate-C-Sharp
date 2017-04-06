\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 500 50 "Dialog"
 3 QM_Grid 0x56031041 0x200 0 0 500 50 "0x0,0,0,0x0,0x0[]Edit,,,[]Edit B,,16,[]Edit ML,,8,[]Combo,,1,[]Combo SB,,25,[]Check,,2,[]Date,,3,[]Time,,11,[]none,,7,"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3"
str qmg3x
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret


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
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	GRID.QM_NMLVDATA* cd=+nh
	NMLVDISPINFO* di=+nh
	NMLISTVIEW* nlv=+nh
	NMITEMACTIVATE* na=+nh
	sel nh.code
		
		 These notifications are from QM_Grid.
		 All text coming from QM_Grid is in QM format (UTF-8 or ANSI, depending on QM Unicode mode).
		
		case LVN_BEGINLABELEDIT ;;when begins cell edit mode
		out "begin edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		if di.item.iSubItem=2
			g.SetButtonProp(1 "Browse...")
		
		case LVN_ENDLABELEDIT ;;when ends cell edit mode
		out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		
		case GRID.LVN_QG_BUTTONCLICK ;;when user clicks button
		out "button: item=%i subitem=%i text=%s" cd.item cd.subitem cd.txt
		if cd.subitem=2
			if(OpenSaveDialog(0 _s)) _s.setwintext(cd.hctrl)
		
		case GRID.LVN_QG_COMBOFILL ;;when user clicks combo box arrow
		out "combo fill: item=%i subitem=%i" cd.item cd.subitem
		if cd.subitem=3 or cd.subitem=4
			TO_CBFill cd.hcb "one[]two[]show inp"
		
		case GRID.LVN_QG_COMBOITEMCLICK ;;when user clicks combo box item
		out "combo click: item=%i subitem=%i cbitem=%i text=%s" cd.item cd.subitem cd.cbindex cd.txt
		if cd.cbindex=2
			if inp(_s)
				_s.setwintext(cd.hctrl)
				ret DT_Ret(hDlg 1)
		
		case GRID.LVN_QG_CHANGE ;;when user changes grid content
		if(cd.hctrl) out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" cd.item cd.subitem cd.txt _s.getwintext(cd.hctrl)
		else out "grid changed" ;;eg row deleted
		
		 These notifications are from SysListView32.
		 All text coming from SysListView32 is in UTF-16 format.
		
		case LVN_INSERTITEM ;;when user inserts new row
		out "inserted %i" nlv.iItem
		
		case LVN_ITEMCHANGED ;;when row state changes. If user selects multiple items with Shift, this is sent once.
		if(nlv.iItem<0 or nlv.uNewState&LVIS_SELECTED=0) ret
		out "selected %i" nlv.iItem ;;selected listview item index
		
		case [NM_CLICK,NM_DBLCLK,NM_RCLICK] ;;when user clicks a row or empty space, and it does not begin cell edit mode
		out "row click: %i %i" na.iItem na.iSubItem
		
		case LVN_ITEMACTIVATE ;;when user clicks or double clicks a row (depends on list view extended style), and it does not begin cell edit mode
		out "item activated: %i %i" na.iItem na.iSubItem
		
		case LVN_COLUMNCLICK ;;click header
		g.Sort(4|0x10000 nlv.iSubItem)
