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
	GRID.QM_NMLVDATA* cd=+nh
	sel nh.code
		 case LVN_ENDLABELEDIT ;;when ends cell edit mode
		 NMLVDISPINFO* di=+nh
		 out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		
		case GRID.LVN_QG_CHANGE ;;when user changes grid content
		if(cd.hctrl) out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" cd.item cd.subitem cd.txt _s.getwintext(cd.hctrl)
		else out "grid changed" ;;eg row deleted
		
		if cd.hctrl and cd.subitem<2
			str s1 s2 s3
			 get text from current cell. It is still not updated, therefore need to get text of the temporary edit control.
			s1.getwintext(cd.hctrl)
			 get text from another cell
			s2=g.CellGet(cd.item cd.subitem^1)
			 set text of third cell
			s3=val(s1)+val(s2)
			g.CellSet(cd.item 2 s3)
			
