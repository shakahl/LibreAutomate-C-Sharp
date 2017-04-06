\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
if(!ShowDialog("" &dlg_edit_autocomplete &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 2 2 96 14 ""
 4 QM_Grid 0x56031041 0x0 2 18 168 78 "0x0,0,0,0,0x0[]A,,,[]combo,,1,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	str-- t_data
	t_data="one[]two[]three[]four[]five[]six[]seven[]eight[]nine"
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	Edit_AutoComplete lParam t_data
	case IDOK
	case IDCANCEL
ret 1

 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4
	GRID.QM_NMLVDATA* cd=+nh
	sel nh.code
		case GRID.LVN_QG_COMBOFILL ;;when user clicks combo box arrow
		 out "combo fill: item=%i subitem=%i" cd.item cd.subitem
		if cd.subitem=1
			TO_CBFill cd.hcb t_data
		
		case GRID.LVN_QG_CHANGE ;;when user changes grid content
		 out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" cd.item cd.subitem cd.txt _s.getwintext(cd.hctrl)
		if cd.hctrl and cd.subitem=1
			Edit_AutoComplete cd.hctrl t_data
