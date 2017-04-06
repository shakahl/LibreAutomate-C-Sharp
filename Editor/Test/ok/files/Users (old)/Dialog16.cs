 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 134 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 12 12 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;not necessary in QM >= 2.1.9
	str& variable=+DT_GetParam(hDlg)
	variable.setwintext(id(3 hDlg))
	ret 1 ;;not necessary in QM >= 2.1.9
	case WM_DESTROY DT_DeleteData(hDlg) ;;not necessary in QM >= 2.1.9
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	DT_Ok hDlg ;;not necessary in QM >= 2.1.9
	case IDCANCEL DT_Cancel hDlg ;;not necessary in QM >= 2.1.9
ret 1
