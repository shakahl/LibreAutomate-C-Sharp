\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("dialog_mdi" &dialog_mdi &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 4 Button 0x54032000 0x0 32 116 48 14 "Button"
 3 QM_DlgInfo 0x54C00000 0x20000 62 30 146 68 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030605 "" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetWinStyle id(3 hDlg) WS_CAPTION 1|8
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 DestroyWindow(id(3 hDlg))
ret 1
