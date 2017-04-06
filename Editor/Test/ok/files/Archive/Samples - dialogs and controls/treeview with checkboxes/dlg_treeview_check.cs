\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_treeview_check)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 203 "Dialog"
 3 SysTreeView32 0x54030000 0x0 2 2 164 178 ""
 4 Button 0x54032000 0x0 172 6 48 14 "Refresh"
 5 Button 0x54032000 0x0 172 30 48 22 "Flash Selected"
 6 Button 0x54032000 0x0 172 58 48 22 "Activate Selected"
 1 Button 0x54030001 0x4 118 186 48 14 "OK"
 2 Button 0x54030000 0x4 170 186 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	TvAddWindows id(3 hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 4 ;;Refresh
	TvAddWindows id(3 hDlg)
	
	case [5,6] ;;flash or activate selected
	ARRAY(int) ah; int i
	TvGetItems id(3 hDlg) 0 ah 1
	for i 0 ah.len
		sel wParam
			case 5 FlashWindow ah[i] 1
			case 6 act ah[i]; err

	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ;;treeview
	sel nh.code
		case TVN_SELCHANGED ;;treeview item selected
		NMTREEVIEW* ntv=+nh
		int h=ntv.itemNew.lParam ;;window handle (set by TvAdd)
		out "item selected: %s" _s.getwintext(h)
