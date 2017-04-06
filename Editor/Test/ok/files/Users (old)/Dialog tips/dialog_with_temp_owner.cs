\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 dialog without caption and taskbar button

int hh=CreateWindowEx(WS_EX_TOPMOST "#32770" 0 0 0 0 0 0 0 0 _hinst 0)
int r=ShowDialog("dialog_with_temp_owner" &dialog_with_temp_owner 0 hh)
DestroyWindow(hh)
if(!r) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 134 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	SetWinStyle hDlg WS_POPUP 8
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
