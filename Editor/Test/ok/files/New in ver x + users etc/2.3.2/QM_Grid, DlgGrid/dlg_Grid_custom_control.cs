\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
if(!ShowDialog("" &dlg_Grid_custom_control &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 110 "0x0,0,0,2[]A,,,[]B,,,[]"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "*" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 SetProp(g "sub" SubclassWindow(g &WndProc23))
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
		case LVN_BEGINLABELEDIT ;;when begins cell edit mode
		out "begin edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		int he=g.Send(LVM_GETEDITCONTROL)
		 outw he
		RECT r; GetWindowRect he &r; MapWindowPoints 0 g +&r 2
		siz 0 0 he
		 he=CreateControl(0 "Button" "" 0 r.left r.top r.right-r.left r.bottom-r.top g 2455)
		he=CreateControl(0 "SysDateTimePick32" "" 0 r.left r.top r.right-r.left r.bottom-r.top g 2455)
		SetFocus he
		
		case LVN_ENDLABELEDIT ;;when ends cell edit mode
		out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		 outw g.Send(LVM_GETEDITCONTROL)
