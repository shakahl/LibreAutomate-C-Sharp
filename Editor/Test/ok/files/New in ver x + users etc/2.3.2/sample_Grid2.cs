\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &sample_Grid2)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 351 135 "Dialog"
 3 QM_Grid 0x54030000 0x0 0 0 352 114 "0x0,0,0,0,0x0[]A,,,[]B,,,[]A+B,,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

ret
 messages
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
	case 3
	sel nh.code
		case LVN_ENDLABELEDIT ;;when ends cell edit mode
		NMLVDISPINFO* di=+nh
		 out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		
		if di.item.iSubItem<2
			str s1 s2 s3
			s1=di.item.pszText ;;get text of current cell
			s2=g.CellGet(di.item.iItem di.item.iSubItem^1) ;;get text of other cell
			s3=val(s1)+val(s2)
			g.CellSet(di.item.iItem 2 s3) ;;set text of third cell
