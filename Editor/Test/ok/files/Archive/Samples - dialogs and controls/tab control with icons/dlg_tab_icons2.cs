\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_tab_icons2)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTabControl32 0x54030040 0x0 0 0 224 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__ImageList-- il.Load("$qm$\il_qm.bmp") ;;load an imagelist craeted with QM imagelist editor
	int htb=id(3 hDlg)
	SendMessage htb TCM_SETIMAGELIST 0 il
	TabControlAddTabs htb "A,2[]B[]C,15"
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
sel nh.code
	case TCN_SELCHANGE
	_i=SendMessage(nh.hwndFrom TCM_GETCURSEL 0 0)
	out "Tab selected: %i" _i
