 \Macro93
function# hDlg message wParam lParam
if(hDlg) goto messages
ret
 messages
sel message
	case WM_INITDIALOG
	Function13
	 deb
	DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
	case WM_SETCURSOR
	deb;; 100
	int t=1
	t=2
	out 1
	out 2
	ClearOutput
	 deb-
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case IDOK
	deb
	int i=1
	DT_Ok hDlg
	i=7
	case IDCANCEL DT_Cancel hDlg
ret 1
 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2010600 "" ""
