\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_tab_ownerdraw" &dlg_tab_ownerdraw)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTabControl32 0x54030040 0x0 0 0 224 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""
 3 SysTabControl32 0x54032040 0x0 0 0 224 110 ""

ret
 messages
sel message
	case WM_INITDIALOG
	TO_TabAddTabs hDlg 3 "one[]two0000000000000[]three" ;;note: it is a private System function, and may be changed/deleted in the future. Make your copy.
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
